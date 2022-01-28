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
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using GraphQL;

namespace Epam.GraphQL.Configuration.Implementations
{
    internal class ProxyAccessor<TEntity, TExecutionContext> : IProxyAccessor<TEntity, TExecutionContext>
    {
        private static MethodInfo? _createGroupSelectorExpressionMethodInfo;
        private static MethodInfo? _createSelectorExpressionMethodInfo;
        private readonly HashSet<LambdaExpression> _members = new(ExpressionEqualityComparer.Instance);
        private readonly Dictionary<LambdaExpression, string> _expressionNames = new(ExpressionEqualityComparer.Instance);
        private readonly Dictionary<string, FieldDependencies<TExecutionContext>> _conditionMembers = new(StringComparer.Ordinal);
        private readonly HashSet<LoadEntityHook<TEntity, TExecutionContext>> _loadEntityHooks = new();

        private readonly ConcurrentDictionary<ICollection<string>, Expression<Func<TExecutionContext, TEntity, Proxy<TEntity>>>> _createSelectorExpressionCache = new(new CollectionEqualityComparer<string>(StringComparer.Ordinal));
        private readonly ConcurrentDictionary<ICollection<string>, Type> _concreteProxyTypeCache = new(new CollectionEqualityComparer<string>(StringComparer.Ordinal));
        private Type? _proxyGenericType;
        private Type? _proxyType;

        public ProxyAccessor(IReadOnlyList<IField<TEntity, TExecutionContext>> fields)
        {
            Fields = fields ?? throw new ArgumentNullException(nameof(fields));
        }

        public IReadOnlyList<IField<TEntity, TExecutionContext>> Fields { get; }

        public Type ProxyGenericType
        {
            get
            {
                Configure();
                return _proxyGenericType;
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

        public bool HasHooks => _loadEntityHooks.Count > 0;

        public Type GetConcreteProxyType(IEnumerable<string> fieldNames)
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

                var dependOnOriginal = dependendFields.Any(dep => dep.DependOnAllMembers);

                var dependencies = dependendFields
                    .SelectMany(dep => dep.DependentOn)
                    .Select(dep => _expressionNames[dep]);

                var conditionalFieldNames = conditionalFields.Select(field => field.Name);

                return type.MakeInstantiatedProxyType<TEntity>(
                    fieldNames
                            .Where(name => !conditionalFieldNames.Contains(name, StringComparer.Ordinal))
                        .Concat(_members.Select(m => _expressionNames[m]))
                        .Concat(dependencies)
                        .Distinct(),
                    dependOnOriginal);
            });
        }

        [MemberNotNull(nameof(_proxyGenericType))]
        public void Configure()
        {
            if (_proxyGenericType != null)
            {
                return;
            }

            var fieldTypes = new Dictionary<string, Type>();
            var i = 1;

            foreach (var calculatedField in Fields)
            {
                var name = calculatedField.Name;
                fieldTypes.Add(name.ToCamelCase(), calculatedField.FieldType);
            }

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

            if (Fields.OfType<IExpressionField<TEntity, TExecutionContext>>().Any(f => f.IsGroupable))
            {
                fieldTypes.Add("$count", typeof(int));
            }

            _proxyGenericType = fieldTypes.MakeProxyType(typeof(TEntity).Name);

            void AddNewField(string fieldPrefix, LambdaExpression depExpr)
            {
                if (!_expressionNames.ContainsKey(depExpr))
                {
                    // Try to look up for existing expression field with the same lambda expression
                    var existingField = Fields
                        .OfType<IExpressionField<TEntity, TExecutionContext>>()
                        .FirstOrDefault(existing => ExpressionEqualityComparer.Instance.Equals(existing.OriginalExpression, depExpr));

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
                        if (!Fields.Any(field => field.Name.ToCamelCase().Equals(propName, StringComparison.Ordinal)))
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

        public LambdaExpression GetProxyExpression(LambdaExpression originalExpression)
        {
            var param = Expression.Parameter(ProxyType);

            if (originalExpression.Body is UnaryExpression unaryExpr
                && (originalExpression.Body.NodeType == ExpressionType.Convert || originalExpression.Body.NodeType == ExpressionType.ConvertChecked || originalExpression.Body.NodeType == ExpressionType.TypeAs))
            {
                var unaryLambda = Expression.Lambda(unaryExpr.Operand, originalExpression.Parameters);
                if (_expressionNames.TryGetValue(unaryLambda, out var unaryLambdaName))
                {
                    var propInfo = ProxyType.GetProperty(unaryLambdaName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                    var member = Expression.Property(param, propInfo);
                    var expr = Expression.MakeUnary(unaryExpr.NodeType, member, unaryExpr.Type);
                    var result = Expression.Lambda(expr, param);
                    return result;
                }
            }
            else if (_expressionNames.TryGetValue(originalExpression, out var originalExpressionName))
            {
                var member = Expression.Property(param, ProxyType.GetProperty(originalExpressionName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase));
                var result = Expression.Lambda(member, param);
                return result;
            }

            var field = Fields.OfType<IExpressionField<TEntity, TExecutionContext>>()
                .FirstOrDefault(field => ExpressionEqualityComparer.Instance.Equals(originalExpression, field.OriginalExpression));

            if (field != null)
            {
                var member = Expression.Property(param, ProxyType.GetProperty(field.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase));
                var result = Expression.Lambda(member, param);
                return result;
            }

            throw new NotSupportedException();
        }

        public Expression<Func<TExecutionContext, TEntity, Proxy<TEntity>>> CreateSelectorExpression(IEnumerable<string> fieldNames)
        {
            return _createSelectorExpressionCache.GetOrAdd(fieldNames.ToList(), fieldNames =>
            {
                _createSelectorExpressionMethodInfo ??= typeof(ProxyAccessor<TEntity, TExecutionContext>)
                    .GetNonPublicGenericMethod(method => method
                        .HasName(nameof(CreateSelectorExpression))
                        .HasOneGenericTypeParameter()
                        .Parameter<IEnumerable<string>>());

                var proxyType = GetConcreteProxyType(fieldNames);

                return _createSelectorExpressionMethodInfo
                    .MakeGenericMethod(proxyType)
                    .InvokeAndHoistBaseException<Expression<Func<TExecutionContext, TEntity, Proxy<TEntity>>>>(this, fieldNames);
            });
        }

        public Expression<Func<TExecutionContext, TEntity, TType>> CreateGroupSelectorExpression<TType>(IEnumerable<string> fieldNames)
        {
            var unorderedFields = Fields
                .OfType<IExpressionField<TEntity, TExecutionContext>>()
                .Where(field => field.IsGroupable && fieldNames.Any(name => field.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
            var orderedFields = fieldNames.Select(f => unorderedFields.First(u => u.Name.Equals(f, StringComparison.OrdinalIgnoreCase)));

            var ctor = typeof(TType).GetConstructors().Single(c => c.GetParameters().Length == 0);

            var contextParam = Expression.Parameter(typeof(TExecutionContext));
            var entiyParam = Expression.Parameter(typeof(TEntity));

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

        public LambdaExpression CreateGroupSelectorExpression(Type entityType, IEnumerable<string> fieldNames)
        {
            _createGroupSelectorExpressionMethodInfo ??= typeof(ProxyAccessor<TEntity, TExecutionContext>)
                .GetPublicGenericMethod(method => method
                    .HasName(nameof(CreateGroupSelectorExpression))
                    .HasOneGenericTypeParameter()
                    .Parameter<IEnumerable<string>>());

            return _createGroupSelectorExpressionMethodInfo
                .MakeGenericMethod(entityType)
                .InvokeAndHoistBaseException<LambdaExpression>(this, fieldNames);
        }

        public LambdaExpression CreateGroupKeySelectorExpression(IEnumerable<string> subFields, IEnumerable<string> aggregateQueriedFields)
        {
            var fields = Fields
                .OfType<IExpressionField<TEntity, TExecutionContext>>()
                .Where(field => field.IsGroupable && subFields.Any(name => field.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
            var sourceType = GetConcreteProxyType(subFields.Concat(aggregateQueriedFields));

            var groupingType = typeof(IGrouping<,>).MakeGenericType(sourceType, sourceType);
            var param = Expression.Parameter(groupingType);
            var entityExpr = Expression.Property(param, groupingType.GetProperty("Key"));

            var ctor = sourceType.GetConstructors().Single(c => c.GetParameters().Length == 0);
            var newExpr = Expression.New(ctor);
            var initExpr = Expression.MemberInit(
                newExpr,
                fields
                    .Select(f =>
                    {
                        var propInfo = sourceType.GetProperty(f.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        return Expression.Bind(propInfo, Expression.Property(entityExpr, propInfo));
                    })
                    .Concat(aggregateQueriedFields
                        .SafeNull()
                        .Select(f =>
                        {
                            if (f == "$count")
                            {
                                var propInfo = sourceType.GetProperty("$count", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                                var countMethodInfo = CachedReflectionInfo.ForEnumerable.Count(sourceType);
                                var methodCallExpr = Expression.Call(null, countMethodInfo, param);

                                return Expression.Bind(propInfo, methodCallExpr);
                            }

                            throw new NotSupportedException();
                        })));

            return Expression.Lambda(initExpr, param);
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

        public void AddMembers<TChildEntity>(string childFieldName, IProxyAccessor<TChildEntity, TExecutionContext> childProxyAccessor, ExpressionFactorizationResult factorizationResult)
        {
            AddMembers(childFieldName, factorizationResult.LeftExpressions.SelectMany(ExpressionHelpers.GetMembers));
            childProxyAccessor.AddMembers(factorizationResult.RightExpressions.SelectMany(ExpressionHelpers.GetMembers));
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

        public void RemoveMember<TResult>(Expression<Func<TEntity, TResult>> member)
        {
            _members.Remove(member);
        }

        public void AddAllMembers(string childFieldName)
        {
            if (!_conditionMembers.TryGetValue(childFieldName, out var fieldDependencies))
            {
                fieldDependencies = new FieldDependencies<TExecutionContext>();
                _conditionMembers.Add(childFieldName, fieldDependencies);
            }

            fieldDependencies.DependOnAllMembers = true;
        }

        public void AddMembers(IEnumerable<LambdaExpression> members)
        {
            _members.UnionWith(members);
        }

        public void AddLoadHook<T>(Expression<Func<TEntity, T>> proxyExpression, Action<TExecutionContext, T> hook)
        {
            _loadEntityHooks.Add(new LoadEntityHook<TEntity, T, TExecutionContext>(this, proxyExpression, hook));
        }

        public void AddLoadHook<TKey, T>(
            Expression<Func<TEntity, TKey>> keyExpression,
            Func<TExecutionContext, IEnumerable<TKey>, IDictionary<TKey, T>> batchFunc,
            Action<TExecutionContext, T> hook)
        {
            _loadEntityHooks.Add(new LoadEntityHook<TEntity, TKey, T, TExecutionContext>(this, keyExpression, hook, batchFunc));
        }

        public void AddLoadHook<TKey, T>(
            Expression<Func<TEntity, TKey>> keyExpression,
            Func<TExecutionContext, IEnumerable<TKey>, Task<IDictionary<TKey, T>>> batchFunc,
            Action<TExecutionContext, T> hook)
        {
            _loadEntityHooks.Add(new LoadEntityHook<TEntity, TKey, T, TExecutionContext>(this, keyExpression, hook, batchFunc));
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

            foreach (var expressionField in queriedFields.OfType<IExpressionField<TEntity, TExecutionContext>>())
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

            var dependOnAllMembers = _conditionMembers
                .Any(kv => kv.Value.DependOnAllMembers && queriedFields.Select(field => field.Name).Contains(kv.Key));

            if (dependOnAllMembers)
            {
                builder.Property(ProxyType.GetProperty("$original"), FuncConstants<TEntity>.IdentityExpression);
            }

            var result = builder.Lambda();
            return Expression.Lambda<Func<TExecutionContext, TEntity, Proxy<TEntity>>>(result.Body, contextParam, result.Parameters[0]);
        }

        private void AddMembers(string childFieldName, IEnumerable<LambdaExpression> members)
        {
            if (!_conditionMembers.TryGetValue(childFieldName, out var dependendMembers))
            {
                dependendMembers = new FieldDependencies<TExecutionContext>();
                _conditionMembers.Add(childFieldName, dependendMembers);
            }

            dependendMembers.AddRange(members);
        }
    }
}
