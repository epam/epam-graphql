// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Relay;
using Epam.GraphQL.Sorters;
using GraphQL;

namespace Epam.GraphQL.Configuration.Implementations
{
    internal class ProxyAccessor<TEntity, TExecutionContext> : IProxyAccessor<TEntity, TExecutionContext>
    {
        private static MethodInfo? _createGroupSelectorExpressionMethodInfo;
        private static MethodInfo? _createSelectorExpressionMethodInfo;
        private readonly IObjectGraphTypeConfigurator<TEntity, TExecutionContext> _owner;
        private readonly HashSet<LambdaExpression> _members = new(ExpressionEqualityComparer.Instance);
        private readonly Dictionary<LambdaExpression, string> _expressionNames = new(ExpressionEqualityComparer.Instance);
        private readonly Dictionary<string, FieldDependencies<TExecutionContext>> _conditionMembers = new(StringComparer.Ordinal);
        private readonly HashSet<LoadEntityHook<TEntity, TExecutionContext>> _loadEntityHooks = new();
        private readonly object _lock = new();

        private readonly ConcurrentDictionary<ICollection<string>, Expression<Func<TExecutionContext, TEntity, Proxy<TEntity>>>> _createSelectorExpressionCache = new(new CollectionEqualityComparer<string>(StringComparer.Ordinal));
        private readonly ConcurrentDictionary<ICollection<string>, Type> _concreteProxyTypeCache = new(new CollectionEqualityComparer<string>(StringComparer.Ordinal));
        private Type? _baseProxyGenericType;
        private Type? _proxyGenericType;
        private Type? _proxyType;
        private Type? _baseProxyType;

        public ProxyAccessor(IObjectGraphTypeConfigurator<TEntity, TExecutionContext> owner)
        {
            _owner = owner;
        }

        public IReadOnlyList<IField<TEntity, TExecutionContext>> Fields => _owner.Fields;

        public Type ProxyGenericType
        {
            get
            {
                Configure();
                return _proxyGenericType;
            }
        }

        public Type BaseProxyGenericType
        {
            get
            {
                ConfigureBase();
                return _baseProxyGenericType;
            }
        }

        public Type ProxyType
        {
            get
            {
                if (_proxyType == null)
                {
                    _proxyType = ProxyGenericType.MakeGenericType(typeof(TEntity));
                }

                return _proxyType;
            }
        }

        public Type BaseProxyType
        {
            get
            {
                if (_baseProxyType == null)
                {
                    _baseProxyType = BaseProxyGenericType.MakeGenericType(typeof(TEntity));
                }

                return _baseProxyType;
            }
        }

        public bool HasHooks => _loadEntityHooks.Count > 0;

        [MemberNotNull(nameof(_proxyGenericType))]
        public void Configure()
        {
            lock (_lock)
            {
                if (_proxyGenericType != null)
                {
                    return;
                }

                var fieldTypes = new Dictionary<string, Type>();
                var i = 1;

                foreach (var dep in _conditionMembers)
                {
                    foreach (var depExpr in dep.Value.DependentOn)
                    {
                        AddNewField(dep.Key, depExpr);
                    }
                }

                foreach (var e in _members)
                {
                    AddNewField(string.Empty, e);
                }

                _proxyGenericType = fieldTypes.MakeProxyType(BaseProxyGenericType, typeof(TEntity).Name);

                void AddNewField(string fieldPrefix, LambdaExpression depExpr)
                {
                    if (!_expressionNames.ContainsKey(depExpr))
                    {
                        // Try to look up for existing expression field with the same lambda expression
                        var existingField = Fields
                            .OfType<IExpressionFieldConfiguration<TEntity, TExecutionContext>>()
                            .FirstOrDefault(existing =>
                                ExpressionEqualityComparer.Instance.Equals(existing.OriginalExpression, depExpr));

                        string? newName = null;

                        if (existingField != null)
                        {
                            // The expression field exists. Use its name
                            newName = existingField.Name.ToCamelCase();
                        }
                        else if (depExpr.IsProperty())
                        {
                            // This is the case when expression field doesn't exist, but expression itself is a property,
                            // so try to use a name of that property.
                            var propName = depExpr.GetPropertyInfo().Name.ToCamelCase();

                            // Try to lookup for existing non-expression field with the property name
                            if (!Fields.Any(
                                    field => field.Name.ToCamelCase().Equals(propName, StringComparison.Ordinal)))
                            {
                                // Field with the property name doesn't exist. Use a name of the property
                                newName = propName;
                                fieldTypes.Add(newName, depExpr.Body.Type);
                            }
                        }

                        if (newName == null)
                        {
                            // No match with existing names. Use generic name
                            newName = $"{fieldPrefix}${i++}";
                            fieldTypes.Add(newName, depExpr.Body.Type);
                        }

                        _expressionNames.Add(depExpr, newName);
                    }
                }
            }
        }

        public LambdaExpression Rewrite(LambdaExpression expression, LambdaExpression originalExpression)
        {
            var param = Expression.Parameter(ProxyType);
            var unaryExpr = expression.Body as UnaryExpression;

            if (unaryExpr != null
                && (expression.Body.NodeType == ExpressionType.Convert || expression.Body.NodeType == ExpressionType.ConvertChecked || expression.Body.NodeType == ExpressionType.TypeAs))
            {
                expression = Expression.Lambda(unaryExpr.Operand, expression.Parameters);
            }

            if (_expressionNames.TryGetValue(expression, out var originalExpressionName))
            {
                var propInfo = ProxyType.GetProperty(originalExpressionName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                var member = Expression.Property(param, propInfo);
                Expression expr = unaryExpr != null
                    ? Expression.MakeUnary(unaryExpr.NodeType, member, unaryExpr.Type)
                    : member;

                var result = Expression.Lambda(expr, param);
                return result;
            }

            var fieldName = Fields.OfType<IExpressionFieldConfiguration<TEntity, TExecutionContext>>()
                .FirstOrDefault(field => ExpressionEqualityComparer.Instance.Equals(originalExpression, field.OriginalExpression))
                ?.Name;

            Guards.AssertType<TEntity>(fieldName == null);

            var fieldMember = Expression.Property(param, ProxyType.GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase));
            var fieldResult = Expression.Lambda(fieldMember, param);
            return fieldResult;
        }

        public Expression<Func<Proxy<TEntity>, T>> Rewrite<T>(Expression<Func<TEntity, T>> originalExpression)
        {
            return (Expression<Func<Proxy<TEntity>, T>>)Rewrite(originalExpression, originalExpression).CastFirstParamTo<Proxy<TEntity>>();
        }

        public Expression<Func<TExecutionContext, TEntity, Proxy<TEntity>>> CreateSelectorExpression(IEnumerable<string> fieldNames)
        {
            return _createSelectorExpressionCache.GetOrAdd(fieldNames.ToList(), fieldNames =>
            {
                _createSelectorExpressionMethodInfo ??= ReflectionHelpers.GetMethodInfo<IEnumerable<string>, LambdaExpression>(
                    CreateSelectorExpression<Proxy<TEntity>>);

                var proxyType = GetConcreteProxyType(fieldNames);
                return _createSelectorExpressionMethodInfo
                    .MakeGenericMethod(proxyType)
                    .InvokeAndHoistBaseException<Expression<Func<TExecutionContext, TEntity, Proxy<TEntity>>>>(this, fieldNames);
            });
        }

        public IQueryable<IGroupResult<Proxy<TEntity>>> GroupBy(IResolveFieldContext context, IQueryable<TEntity> query)
        {
            var subFields = context.GetGroupConnectionQueriedFields();
            var aggregateQueriedFields = context.GetGroupConnectionAggregateQueriedFields();

            var sourceType = GetConcreteProxyType(subFields);

            // ApplyGroupBy
            // Actual type of lambda returning value is loader.ObjectGraphTypeConfigurator.ExtendedType, not typeof(TChildEntity)
            // entity => new EntityExt { Country = entity.Country, Language = entity.Language }
            var keyExpression = CreateGroupSelectorExpression(sourceType, subFields).BindFirstParameter(context.GetUserContext<TExecutionContext>());

            // (entity, rows) => new { Item = entity, Count = rows.Count() }
            var resultExpression = CreateGroupKeySelectorExpression(sourceType, context, subFields, aggregateQueriedFields);

            // .GroupBy(entity => new { Country = entity.Country, Language = entity.Language }, (entity, rows) => new { Item = entity, Count = rows.Count() })
            var groupBy = query.ApplyGroupBy(keyExpression, resultExpression);

            return (IQueryable<IGroupResult<Proxy<TEntity>>>)groupBy;
        }

        public IReadOnlyList<(LambdaExpression SortExpression, SortDirection SortDirection)> GetGroupSort(
            IResolveFieldContext context,
            IEnumerable<ISorter<TExecutionContext>> sorters)
        {
            var queriedFields = context.GetGroupConnectionQueriedFields();
            var aggregateQueriedFields = context.GetGroupConnectionAggregateQueriedFields();

            var sourceType = GetConcreteProxyType(queriedFields);

            var sorting = context.GetSorting();
            var search = context.GetSearch();

            var sortFields = sorting
                .Select(o => (sorters.Single(s => string.Equals(s.Name, o.Field, StringComparison.Ordinal)), o.Direction));

            if (sortFields.Any(f => !(f.Item1?.IsGroupable ?? false)))
            {
                throw new ExecutionError($"Cannot sort by the following fields: {string.Join(", ", sorting.Select(o => $"`{o.Field}`"))}. Consider making them groupable.");
            }

            var groupResultType = typeof(GroupResult<>).MakeGenericType(sourceType);
            var param = Expression.Parameter(groupResultType);
            var itemProp = Expression.Property(param, groupResultType.GetProperty(nameof(GroupResult<object>.Item)));
            var getter = Expression.Lambda(itemProp, param);

            var subFields = context.GetGroupConnectionQueriedFields()
                .Where(name => !sorting.Any(s => string.Equals(s.Field, name, StringComparison.Ordinal)))
                .Select(name => Fields.FirstOrDefault(field => string.Equals(field.Name, name, StringComparison.Ordinal)))
                .OfType<IExpressionFieldConfiguration<TEntity, TExecutionContext>>();

            var ctx = context.GetUserContext<TExecutionContext>();
            var result = sortFields.Select(f => (ExpressionHelpers.Compose(getter, Rewrite(f.Item1.BuildExpression(ctx), f.Item1.BuildOriginalExpression(ctx))), f.Direction))
                .Concat(subFields.Select(f => (ExpressionHelpers.Compose(getter, Rewrite(f.OriginalExpression, f.OriginalExpression)), SortDirection.Asc)));

            return result.ToList();
        }

        public ILoaderHooksExecuter<Proxy<TEntity>>? CreateHooksExecuter(IResolveFieldContext context)
        {
            if (!HasHooks)
            {
                return null;
            }

            return new LoaderHooksExecuter<TEntity, TExecutionContext>(_loadEntityHooks, context);
        }

        public void ReplaceField(IField<TExecutionContext> oldField, IField<TExecutionContext> newField)
        {
            if (_conditionMembers.TryGetValue(oldField.Name, out var members))
            {
                _conditionMembers.Remove(oldField.Name);
                _conditionMembers.Add(newField.Name, members);
            }
        }

        public void AddMembers<TChildEntity, TTransformedChildEntity>(
            string childFieldName,
            IProxyAccessor<TChildEntity, TTransformedChildEntity, TExecutionContext>? childProxyAccessor,
            ExpressionFactorizationResult factorizationResult)
        {
            AddMembers(childFieldName, factorizationResult.LeftExpressions.Select(ExpressionExtensions.RemoveConvert));

            if (childProxyAccessor != null)
            {
                childProxyAccessor.AddMembers(factorizationResult.RightExpressions.Select(ExpressionExtensions.RemoveConvert));
            }
        }

        public void AddMembers(string childFieldName, IEnumerable<LambdaExpression> members)
        {
            if (!_conditionMembers.TryGetValue(childFieldName, out var dependendMembers))
            {
                dependendMembers = new FieldDependencies<TExecutionContext>();
                _conditionMembers.Add(childFieldName, dependendMembers);
            }

            dependendMembers.AddRange(members.Select(ExpressionExtensions.RemoveConvert));
        }

        public void AddMember<TResult>(string childFieldName, Expression<Func<TEntity, TResult>> member)
        {
            if (!_conditionMembers.TryGetValue(childFieldName, out var dependendMembers))
            {
                dependendMembers = new FieldDependencies<TExecutionContext>();
                _conditionMembers.Add(childFieldName, dependendMembers);
            }

            dependendMembers.Add(member);
        }

        public void AddMember<TResult>(Expression<Func<TEntity, TResult>> member)
        {
            _members.Add(member);
        }

        public void AddMembers(IEnumerable<LambdaExpression> members)
        {
            _members.UnionWith(members);
        }

        public void RemoveMember<TResult>(Expression<Func<TEntity, TResult>> member)
        {
            _members.Remove(member);
        }

        public void AddLoadHook<T>(
            Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory,
            Expression<Func<TEntity, T>> proxyExpression,
            Action<TExecutionContext, T> hook)
        {
            var loadEntityHook = new LoadEntityHook<TEntity, T, TExecutionContext>(configurationContextFactory, this, proxyExpression, hook);
            _loadEntityHooks.Remove(loadEntityHook);
            _loadEntityHooks.Add(loadEntityHook);
        }

        public void AddLoadHook<TKey, T>(
            Func<IChainConfigurationContextOwner, IResolvedChainConfigurationContext> configurationContextFactory,
            Expression<Func<TEntity, TKey>> keyExpression,
            Func<TExecutionContext, IEnumerable<TKey>, IDictionary<TKey, T>> batchFunc,
            Action<TExecutionContext, T> hook)
        {
            var loadEntityHook = new LoadEntityHook<TEntity, TKey, T, TExecutionContext>(configurationContextFactory, this, keyExpression, hook, batchFunc);
            _loadEntityHooks.Remove(loadEntityHook);
            _loadEntityHooks.Add(loadEntityHook);
        }

        public void AddLoadHook<TKey, T>(
            Func<IChainConfigurationContextOwner, IResolvedChainConfigurationContext> configurationContextFactory,
            Expression<Func<TEntity, TKey>> keyExpression,
            Func<TExecutionContext, IEnumerable<TKey>, Task<IDictionary<TKey, T>>> batchFunc,
            Action<TExecutionContext, T> hook)
        {
            var loadEntityHook = new LoadEntityHook<TEntity, TKey, T, TExecutionContext>(configurationContextFactory, this, keyExpression, hook, batchFunc);
            _loadEntityHooks.Remove(loadEntityHook);
            _loadEntityHooks.Add(loadEntityHook);
        }

        private Type GetConcreteProxyType(IEnumerable<string> fieldNames)
        {
            return _concreteProxyTypeCache.GetOrAdd(fieldNames.ToList(), fieldNames =>
            {
                var type = ProxyGenericType; // Force type creation

                // TODO Refactor
                var fields = Fields.Where(field => fieldNames.Contains(field.Name, StringComparer.Ordinal));

                var conditionalFields = fields
                    .Where(field => _conditionMembers.ContainsKey(field.Name));

                var dependendFields = conditionalFields
                    .Select(field => _conditionMembers[field.Name]);

                var dependencies = dependendFields
                    .SelectMany(dep => dep.DependentOn)
                    .Select(dep => _expressionNames[dep]);

                var conditionalFieldNames = conditionalFields.Select(field => field.Name);

                return type.MakeInstantiatedProxyType<TEntity>(
                    fieldNames
                            .Where(name => !conditionalFieldNames.Contains(name, StringComparer.Ordinal))
                        .Concat(_members.Select(m => _expressionNames[m]))
                        .Concat(dependencies)
                        .Distinct());
            });
        }

        private Expression<Func<TExecutionContext, TEntity, Proxy<TEntity>>> CreateSelectorExpression<TType>(IEnumerable<string> fieldNames)
            where TType : Proxy<TEntity>
        {
            // TODO Refactor it (should we query 'queriedFields'?)
            var queriedFields = Fields.Where(field => fieldNames.SafeNull().Any(name => field.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));

            var builder = ExpressionHelpers.MakeMemberInit<TType>(typeof(TEntity));

            var contextParam = Expression.Parameter(typeof(TExecutionContext));

            var properties = ProxyType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
                .OrderBy(p => p.Name)
                .ThenBy(p => p.DeclaringType != ProxyType)
                .ToList();

            foreach (var expressionField in queriedFields.OfType<IExpressionFieldConfiguration<TEntity, TExecutionContext>>())
            {
                var boundExpr = expressionField.ContextExpression.ReplaceFirstParameter(contextParam);
                var propertyInfo = properties.First(p => p.Name.Equals(expressionField.Name, StringComparison.OrdinalIgnoreCase));
                builder.Property(propertyInfo, boundExpr);
            }

            var allMembers = _conditionMembers
                .Where(kv => queriedFields.Select(field => field.Name).Contains(kv.Key))
                .SelectMany(kv => kv.Value.DependentOn)
                .Concat(_members)
                .Distinct<LambdaExpression>(ExpressionEqualityComparer.Instance);

            foreach (var memberExpression in allMembers)
            {
                var propertyInfo = ProxyType.GetProperty(_expressionNames[memberExpression], BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                builder.Property(propertyInfo, memberExpression);
            }

            var result = builder.Lambda();
            return Expression.Lambda<Func<TExecutionContext, TEntity, Proxy<TEntity>>>(result.Body, contextParam, result.Parameters[0]);
        }

        private LambdaExpression CreateGroupSelectorExpression(Type entityType, IEnumerable<string> fieldNames)
        {
            _createGroupSelectorExpressionMethodInfo ??= ReflectionHelpers.GetMethodInfo<IEnumerable<string>, LambdaExpression>(
                CreateGroupSelectorExpression<Proxy<TEntity>>);

            return _createGroupSelectorExpressionMethodInfo
                .MakeGenericMethod(entityType)
                .InvokeAndHoistBaseException<LambdaExpression>(this, fieldNames);
        }

        private Expression<Func<TExecutionContext, TEntity, TType>> CreateGroupSelectorExpression<TType>(IEnumerable<string> fieldNames)
        {
            var unorderedFields = Fields
                .OfType<IExpressionFieldConfiguration<TEntity, TExecutionContext>>()
                .Where(field => field.IsGroupable && fieldNames.Any(name => field.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
            var orderedFields = fieldNames.Select(f => unorderedFields.First(u => u.Name.Equals(f, StringComparison.OrdinalIgnoreCase)));

            var contextParam = Expression.Parameter(typeof(TExecutionContext));
            var entiyParam = Expression.Parameter(typeof(TEntity));

            if (orderedFields.Any())
            {
                var ctor = typeof(TType).GetConstructors().Single(c => c.GetParameters().Length == 0);

                var newExpr = Expression.New(ctor);
                var initExpr = Expression.MemberInit(
                    newExpr,
                    orderedFields
                        .Select(f =>
                        {
                            var expr = f.ContextExpression.ReplaceFirstParameter(contextParam);
                            return Expression.Bind(
                                typeof(TType).GetProperty(f.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase),
                                expr.Body.ReplaceParameter(expr.Parameters[0], entiyParam));
                        }));

                return Expression.Lambda<Func<TExecutionContext, TEntity, TType>>(initExpr, contextParam, entiyParam);
            }

            var nullExpression = Expression.Constant(null, typeof(TType));
            return Expression.Lambda<Func<TExecutionContext, TEntity, TType>>(nullExpression, contextParam, entiyParam);
        }

        private LambdaExpression CreateGroupKeySelectorExpression(
            Type keyType,
            IResolveFieldContext context,
            IEnumerable<string> subFields,
            IEnumerable<string> aggregateQueriedFields)
        {
            var fields = Fields
                .OfType<IExpressionFieldConfiguration<TEntity, TExecutionContext>>()
                .Where(field => field.IsGroupable && subFields.Any(name => field.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
            var sourceType = GetConcreteProxyType(subFields);

            var keyParam = Expression.Parameter(keyType);
            var enumerableParam = Expression.Parameter(typeof(IEnumerable<TEntity>));

            var groupResultType = typeof(GroupResult<>).MakeGenericType(sourceType);
            var groupResultCtor = groupResultType.GetConstructors().Single(c => c.GetParameters().Length == 0);
            var groupResultNew = Expression.New(groupResultCtor);

            var bindings = new List<MemberBinding>();

            if (subFields.Any())
            {
                bindings.Add(Expression.Bind(groupResultType.GetProperty(nameof(GroupResult<object>.Item)), keyParam));
            }

            if (aggregateQueriedFields.Contains("count"))
            {
                var countMethodInfo = CachedReflectionInfo.ForEnumerable.Count(typeof(TEntity));
                var methodCallExpr = Expression.Call(null, countMethodInfo, enumerableParam);
                bindings.Add(Expression.Bind(groupResultType.GetProperty(nameof(GroupResult<object>.Count)), methodCallExpr));
            }

            Guards.AssertField(bindings.Count == 0, context);

            var groupResultInit = Expression.MemberInit(groupResultNew, bindings);

            var result = Expression.Lambda(groupResultInit, keyParam, enumerableParam);
            return result;
        }

        [MemberNotNull(nameof(_baseProxyGenericType))]
        private void ConfigureBase()
        {
            if (_baseProxyGenericType != null)
            {
                return;
            }

            var fieldTypes = Fields.ToDictionary(f => f.Name.ToCamelCase(), f => f.FieldType);
            _baseProxyGenericType = fieldTypes.MakeBaseProxyType(typeof(TEntity).Name);
        }
    }
}
