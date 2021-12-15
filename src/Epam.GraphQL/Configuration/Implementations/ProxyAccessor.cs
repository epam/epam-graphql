// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using GraphQL;

namespace Epam.GraphQL.Configuration.Implementations
{
    internal class ProxyAccessor<TEntity, TExecutionContext> : IProxyAccessor<TEntity, TExecutionContext>
    {
        private readonly IReadOnlyList<IField<TExecutionContext>> _fields;
        private readonly HashSet<LambdaExpression> _members = new(ExpressionEqualityComparer.Instance);
        private readonly Dictionary<LambdaExpression, string> _expressionNames = new(ExpressionEqualityComparer.Instance);
        private readonly Dictionary<string, FieldDependencies<TExecutionContext>> _conditionMembers = new(StringComparer.Ordinal);
        private readonly HashSet<LoadEntityHook<TEntity, TExecutionContext>> _loadEntityHooks = new();

        private readonly ConcurrentDictionary<ICollection<string>, Expression<Func<TExecutionContext, TEntity, Proxy<TEntity>>>> _createSelectorExpressionCache = new(new CollectionEqualityComparer<string>(StringComparer.Ordinal));
        private readonly ConcurrentDictionary<ICollection<string>, Type> _concreteProxyTypeCache = new(new CollectionEqualityComparer<string>(StringComparer.Ordinal));
        private Type _proxyGenericType;
        private Type _proxyType;

        public ProxyAccessor(IReadOnlyList<IField<TExecutionContext>> fields)
        {
            _fields = fields ?? throw new ArgumentNullException(nameof(fields));
        }

        public Type ProxyGenericType
        {
            get
            {
                if (_proxyGenericType == null)
                {
                    Configure();
                }

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
                var fields = _fields.Where(field => fieldNames.Contains(field.Name, StringComparer.Ordinal));

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

        public void Configure()
        {
            if (_proxyGenericType == null)
            {
                var fieldTypes = new Dictionary<string, Type>();

                foreach (var calculatedField in _fields)
                {
                    var name = calculatedField.Name;

                    if (calculatedField.FieldType != null)
                    {
                        fieldTypes.Add(name.ToCamelCase(), calculatedField.FieldType);
                    }
                }

                var i = 1;

                foreach (var dep in _conditionMembers)
                {
                    foreach (var depExpr in dep.Value.DependentOn)
                    {
                        if (!_expressionNames.ContainsKey(depExpr))
                        {
                            var newName = $"<>{dep.Key}${i++}";
                            fieldTypes.Add(newName, depExpr.Body.Type);
                            _expressionNames.Add(depExpr, newName);
                        }
                    }
                }

                foreach (var e in _members)
                {
                    if (!_expressionNames.ContainsKey(e))
                    {
                        var newName = $"<>${i++}";
                        fieldTypes.Add(newName, e.Body.Type);
                        _expressionNames.Add(e, newName);
                    }
                }

                if (_fields.Any(f => f.IsGroupable))
                {
                    fieldTypes.Add("<>$count", typeof(int));
                }

                _proxyGenericType = fieldTypes.MakeProxyType(typeof(TEntity).Name);
            }
        }

        public LambdaExpression GetProxyExpression(LambdaExpression originalExpression)
        {
            var param = Expression.Parameter(typeof(Proxy<TEntity>));
            var castedParam = Expression.MakeUnary(ExpressionType.TypeAs, param, ProxyType);

            if (originalExpression.Body is UnaryExpression unaryExpr
                && (originalExpression.Body.NodeType == ExpressionType.Convert || originalExpression.Body.NodeType == ExpressionType.ConvertChecked || originalExpression.Body.NodeType == ExpressionType.TypeAs))
            {
                var unaryLambda = Expression.Lambda(unaryExpr.Operand, originalExpression.Parameters);
                var propInfo = ProxyType.GetProperty(_expressionNames[unaryLambda], BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                var member = Expression.Property(castedParam, propInfo);
                var expr = Expression.MakeUnary(unaryExpr.NodeType, member, unaryExpr.Type);
                var result = Expression.Lambda(expr, param);
                return result;
            }
            else
            {
                var member = Expression.Property(castedParam, ProxyType.GetProperty(_expressionNames[originalExpression], BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase));
                var result = Expression.Lambda(member, param);
                return result;
            }
        }

        public Expression<Func<TExecutionContext, TEntity, Proxy<TEntity>>> CreateSelectorExpression(IEnumerable<string> fieldNames)
        {
            return _createSelectorExpressionCache.GetOrAdd(fieldNames.ToList(), fieldNames =>
            {
                var proxyType = GetConcreteProxyType(fieldNames);
                var createGroupingExpressionMethodInfo = GetType().GetGenericMethod(
                    nameof(CreateSelectorExpression),
                    new[] { proxyType },
                    new[] { typeof(IEnumerable<string>) },
                    BindingFlags.Instance | BindingFlags.NonPublic);

                return createGroupingExpressionMethodInfo.InvokeAndHoistBaseException<Expression<Func<TExecutionContext, TEntity, Proxy<TEntity>>>>(this, fieldNames);
            });
        }

        public Expression<Func<TExecutionContext, TEntity, TType>> CreateGroupSelectorExpression<TType>(IEnumerable<string> fieldNames)
        {
            var unorderedFields = _fields.Where(field => field.IsExpression && field.IsGroupable && fieldNames.Any(name => field.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));
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
                        var expr = f.Expression.ReplaceFirstParameter(contextParam);
                        return Expression.Bind(
                            typeof(TType).GetProperty(f.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase),
                            expr.Body.ReplaceParameter(expr.Parameters[0], entiyParam));
                    }));

            return Expression.Lambda<Func<TExecutionContext, TEntity, TType>>(initExpr, contextParam, entiyParam);
        }

        public LambdaExpression CreateGroupSelectorExpression(Type entityType, IEnumerable<string> fieldNames)
        {
            var createGroupingExpressionMethodInfo = GetType().GetGenericMethod(
                nameof(CreateGroupSelectorExpression),
                new[] { entityType },
                new[] { typeof(IEnumerable<string>) });

            return createGroupingExpressionMethodInfo.InvokeAndHoistBaseException<LambdaExpression>(this, fieldNames);
        }

        public ILoaderHooksExecuter<Proxy<TEntity>> CreateHooksExecuter(TExecutionContext executionContext)
        {
            if (!HasHooks)
            {
                return null;
            }

            return new LoaderHooksExecuter<TEntity, TExecutionContext>(_loadEntityHooks, executionContext);
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
            (childProxyAccessor as ProxyAccessor<TChildEntity, TExecutionContext>).AddMembers(factorizationResult.RightExpressions.SelectMany(ExpressionHelpers.GetMembers));
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

        public void AddLoadHook<T>(Expression<Func<TEntity, T>> proxyExpression, Action<TExecutionContext, T> hook)
        {
            if (proxyExpression == null)
            {
                throw new ArgumentNullException(nameof(proxyExpression));
            }

            if (hook == null)
            {
                throw new ArgumentNullException(nameof(hook));
            }

            _members.Add(proxyExpression);
            _loadEntityHooks.Add(new LoadEntityHook<TEntity, T, TExecutionContext>(this, proxyExpression, hook));
        }

        private Expression<Func<TExecutionContext, TEntity, Proxy<TEntity>>> CreateSelectorExpression<TType>(IEnumerable<string> fieldNames)
            where TType : Proxy<TEntity>
        {
            // TODO Refactor it (should we query 'queriedFields'?)
            var queriedFields = _fields.Where(field => fieldNames.SafeNull().Any(name => field.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));

            var ctor = typeof(TType).GetConstructors().Single(c => c.GetParameters().Length == 0);

            var contextParam = Expression.Parameter(typeof(TExecutionContext));
            var entityParam = Expression.Parameter(typeof(TEntity));

            var newExpr = Expression.New(ctor);

            var properties = ProxyType.GetProperties(BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)
                .OrderBy(p => p.Name)
                .ThenBy(p => p.DeclaringType != ProxyType)
                .ToList();

            var bindings = queriedFields.Where(field => field.IsExpression).Select(f =>
            {
                var expr = f.Expression.ReplaceFirstParameter(contextParam);
                return Expression.Bind(
                    properties.First(p => p.Name.Equals(f.Name, StringComparison.OrdinalIgnoreCase)),
                    expr.Body.ReplaceParameter(expr.Parameters[0], entityParam));
            }).ToList();

            var allMembers = _conditionMembers
                .Where(kv => queriedFields.Select(field => field.Name).Contains(kv.Key))
                .SelectMany(kv => kv.Value.DependentOn)
                .Concat(_members)
                .Distinct<LambdaExpression>(ExpressionEqualityComparer.Instance);

            bindings.AddRange(allMembers.Select(m => Expression.Bind(
                    ProxyType.GetProperty(_expressionNames[m], BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase),
                    m.Body.ReplaceParameter(m.Parameters[0], entityParam))));

            var dependOnAllMembers = _conditionMembers
                .Any(kv => kv.Value.DependOnAllMembers && queriedFields.Select(field => field.Name).Contains(kv.Key));

            if (dependOnAllMembers)
            {
                bindings.Add(Expression.Bind(ProxyType.GetProperty("$original"), entityParam));
            }

            var initExpr = Expression.MemberInit(newExpr, bindings);

            var result = Expression.Lambda<Func<TExecutionContext, TEntity, Proxy<TEntity>>>(initExpr, contextParam, entityParam);
            return result;
        }

        private void AddMembers(IEnumerable<LambdaExpression> members)
        {
            _members.UnionWith(members);
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
