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
using Epam.GraphQL.Extensions;

namespace Epam.GraphQL.Helpers
{
    internal static class ExpressionHelpers
    {
        public static Expression<Func<T1, TResult>> Compose<T1, T2, TResult>(Expression<Func<T1, T2>> first, Expression<Func<T2, TResult>> second)
        {
            var paramForReplace = first.Parameters[0];
            var param = Expression.Parameter(paramForReplace.Type, paramForReplace.Name);
            var body = ParameterRebinder<Expression, Expression>.ReplaceParameter(first.Body, paramForReplace, param);

            return Expression.Lambda<Func<T1, TResult>>(ParameterRebinder<Expression, Expression>.ReplaceParameter(second.Body, second.Parameters[0], body), param);
        }

        public static LambdaExpression Compose(LambdaExpression first, LambdaExpression second)
        {
            var paramForReplace = first.Parameters[0];
            var param = Expression.Parameter(paramForReplace.Type, paramForReplace.Name);
            var body = ParameterRebinder<Expression, Expression>.ReplaceParameter(first.Body, paramForReplace, param);

            return Expression.Lambda(ParameterRebinder<Expression, Expression>.ReplaceParameter(second.Body, second.Parameters[0], body), param);
        }

        public static Expression<Func<T, T1, TResult>> Compose<T, T1, T2, TResult>(Expression<Func<T, T1, T2>> first, Expression<Func<T2, TResult>> second)
        {
            var paramForReplace1 = first.Parameters[0];
            var param1 = Expression.Parameter(paramForReplace1.Type, paramForReplace1.Name);

            var paramForReplace2 = first.Parameters[1];
            var param2 = Expression.Parameter(paramForReplace2.Type, paramForReplace2.Name);

            var body = ParameterRebinder<Expression, Expression>.ReplaceParameters(
                new Dictionary<Expression, Expression>
                {
                    [paramForReplace1] = param1,
                    [paramForReplace2] = param2,
                },
                first.Body);

            return Expression.Lambda<Func<T, T1, TResult>>(ParameterRebinder<Expression, Expression>.ReplaceParameter(second.Body, second.Parameters[0], body), param1, param2);
        }

        public static Expression<Func<T, T1, TResult>> Compose<T, T1, T2, TResult>(Expression<Func<T, T1, T2>> first, Expression<Func<T, T2, TResult>> second)
        {
            var paramForReplace1 = first.Parameters[0];
            var param1 = Expression.Parameter(paramForReplace1.Type, paramForReplace1.Name);

            var paramForReplace2 = first.Parameters[1];
            var param2 = Expression.Parameter(paramForReplace2.Type, paramForReplace2.Name);

            var body = ParameterRebinder<Expression, Expression>.ReplaceParameters(
                new Dictionary<Expression, Expression>
                {
                    [paramForReplace1] = param1,
                    [paramForReplace2] = param2,
                },
                first.Body);

            return Expression.Lambda<Func<T, T1, TResult>>(ParameterRebinder<Expression, Expression>.ReplaceParameter(second.Body, second.Parameters[1], body), param1, param2);
        }

        public static Expression<Func<T, T1, TResult>> SafeCompose<T, T1, T2, TResult>(Expression<Func<T, T1, T2>> first, Expression<Func<T, T2, TResult>> second)
        {
            if ((!typeof(T2).IsValueType || typeof(T2).IsNullable()) && (!typeof(TResult).IsValueType || typeof(TResult).IsNullable()))
            {
                var paramForReplace1 = first.Parameters[0];
                var param1 = Expression.Parameter(paramForReplace1.Type, paramForReplace1.Name);

                var paramForReplace2 = first.Parameters[1];
                var param2 = Expression.Parameter(paramForReplace2.Type, paramForReplace2.Name);

                var body = ParameterRebinder<Expression, Expression>.ReplaceParameters(
                    new Dictionary<Expression, Expression>
                    {
                        [paramForReplace1] = param1,
                        [paramForReplace2] = param2,
                    },
                    first.Body);

                var secondBody = ParameterRebinder<Expression, Expression>.ReplaceParameters(
                    new Dictionary<Expression, Expression>(ExpressionEqualityComparer.Instance)
                    {
                        [body] = body,
                        [second.Parameters[1]] = body,
                    },
                    second.Body);

                var testExpr = Expression.Equal(body, Expression.Constant(null, body.Type));
                var conditionExpr = Expression.Condition(testExpr, Expression.Constant(null, secondBody.Type), secondBody);

                return Expression.Lambda<Func<T, T1, TResult>>(conditionExpr, param1, param2);
            }

            return Compose(first, second);
        }

        public static Expression<Func<TItem, bool>> MakeContainsExpression<TItem, TId>(IEnumerable<TId> ids, Expression<Func<TItem, TId>> keySelector)
        {
            var value = Tuple.Create(ids);
            var valueConstExpression = Expression.Constant(value);
            var idExpression = Expression.Property(valueConstExpression, CachedReflectionInfo.ForTuple<IEnumerable<TId>>.Item1);
            var paramExpression = Expression.Parameter(typeof(TItem), keySelector.Parameters[0].Name);

            var keySelectorExpression = ParameterRebinder<Expression, ParameterExpression>.ReplaceParameter(keySelector.Body, keySelector.Parameters[0], paramExpression);

            var callExpression = Expression.Call(CachedReflectionInfo.ForEnumerable.Contains<TId>(), idExpression, keySelectorExpression);
            return Expression.Lambda<Func<TItem, bool>>(callExpression, paramExpression);
        }

        public static Expression<Func<TItem, bool>> MakeContainsExpression<TItem, TId>(IEnumerable<TId> ids, Expression<Func<TItem, TId?>> keySelector)
            where TId : struct
        {
            var value = Tuple.Create(ids);
            var valueConstExpression = Expression.Constant(value);
            var idExpression = Expression.Property(valueConstExpression, CachedReflectionInfo.ForTuple<IEnumerable<TId>>.Item1);
            var paramExpression = Expression.Parameter(typeof(TItem), keySelector.Parameters[0].Name);

            var keySelectorExpression = ParameterRebinder<Expression, ParameterExpression>.ReplaceParameter(keySelector.Body, keySelector.Parameters[0], paramExpression);

            var valueAccessExpression = Expression.Property(keySelectorExpression, CachedReflectionInfo.ForNullable<TId>.Value);
            var hasValueAccessExpression = Expression.Property(keySelectorExpression, CachedReflectionInfo.ForNullable<TId>.HasValue);
            var callExpression = Expression.Call(CachedReflectionInfo.ForEnumerable.Contains<TId>(), idExpression, valueAccessExpression);
            var andExpression = Expression.AndAlso(hasValueAccessExpression, callExpression);
            return Expression.Lambda<Func<TItem, bool>>(andExpression, paramExpression);
        }

        public static Expression<Func<TEntity, TProperty>> MakeValueAccessExpression<TEntity, TProperty>(Expression<Func<TEntity, TProperty?>> selector)
            where TProperty : struct
        {
            return ValueAccessExpressionCacher<TEntity, TProperty>.MakeValueAccessExpression(selector);
        }

        public static Expression<Func<object?, object?>> MakeWeakLambdaExpression(LambdaExpression selector)
        {
            selector.ShouldHaveOnlyOneParameter(nameof(selector));

            var paramExpression = Expression.Parameter(typeof(object), selector.Parameters[0].Name);
            var convertParamExpression = Expression.Convert(paramExpression, selector.Parameters[0].Type);
            var keySelectorExpression = ParameterRebinder<Expression, UnaryExpression>.ReplaceParameter(selector.Body, selector.Parameters[0], convertParamExpression);
            var convertResultExpression = Expression.Convert(keySelectorExpression, typeof(object));
            var result = Expression.Lambda<Func<object?, object?>>(convertResultExpression, paramExpression);

            return result;
        }

        public static LambdaExpression MakeIdentity(Type type)
        {
            var param = Expression.Parameter(type);
            return Expression.Lambda(param, param);
        }

        public static ExpressionFactorizationResult Factorize<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> expression)
        {
            return FactorizationVisitor.Factorize(expression);
        }

        public static ConditionFactorizationResult<T2> FactorizeCondition<T1, T2>(Expression<Func<T1, T2, bool>> condition)
        {
            if (TryFactorizeCondition(condition, out var result))
            {
                return result!;
            }

            throw new ArgumentOutOfRangeException(nameof(condition), $"Cannot use expression {condition} as a relation between {typeof(T1).HumanizedName()} and {typeof(T2).HumanizedName()} types.");
        }

        public static bool TryFactorizeCondition<T1, T2>(Expression<Func<T1, T2, bool>> expression, [NotNullWhen(true)] out ConditionFactorizationResult<T2>? result)
        {
            var factorizationResult = Factorize(expression);

            if (factorizationResult.LeftExpressions.Count == 1 && factorizationResult.RightExpressions.Count != 0)
            {
                var leftExpression = factorizationResult.LeftExpressions[0];
                LambdaExpression? rightExpression = null;
                Expression<Func<T2, bool>>? rightCondition = null;

                var operands = GetAndAlsoExpressions(factorizationResult.Expression.Body);
                var parameters = factorizationResult.Expression.Parameters;
                var leftParam = parameters[0];

                var equalOperand = operands.Single(op => op.ContainsParameter(leftParam));

                if (equalOperand.NodeType == ExpressionType.Equal)
                {
                    var index = -1;
                    for (int i = 0; i < parameters.Count - 1; i++)
                    {
                        if (equalOperand.ContainsParameter(parameters[i + 1]))
                        {
                            index = i;
                            break;
                        }
                    }

                    if (index != -1)
                    {
                        var rightExpressions = factorizationResult.RightExpressions;
                        Expression? left = null;
                        var param = Expression.Parameter(typeof(T2));

                        for (int i = 0; i < rightExpressions.Count; i++)
                        {
                            if (i != index)
                            {
                                var right = ParameterRebinder<Expression, ParameterExpression>.ReplaceParameter(rightExpressions[i].Body, rightExpressions[i].Parameters[0], param);

                                if (left == null)
                                {
                                    left = right;
                                    continue;
                                }

                                left = Expression.AndAlso(left, right);
                            }
                            else
                            {
                                rightExpression = rightExpressions[i];
                            }
                        }

                        if (left != null)
                        {
                            rightCondition = Expression.Lambda<Func<T2, bool>>(left, param);
                        }

                        if (rightExpression != null)
                        {
                            result = new ConditionFactorizationResult<T2>(leftExpression, rightExpression, rightCondition);
                            return true;
                        }
                    }
                }
            }

            result = null;

            return false;
        }

        public static MemberInitBuilder<TResult> MakeMemberInit<TResult>(Type paramType)
        {
            return new MemberInitBuilder<TResult>(paramType);
        }

        private static IReadOnlyList<Expression> GetAndAlsoExpressions(Expression expression)
        {
            var expressions = new List<Expression>();

            while (expression is BinaryExpression binaryExpression && expression.NodeType == ExpressionType.AndAlso)
            {
                expressions.Add(binaryExpression.Right);
                expression = binaryExpression.Left;
            }

            expressions.Add(expression);
            expressions.Reverse();

            return expressions;
        }

        public class MemberInitBuilder<TResult>
        {
            private readonly Dictionary<PropertyInfo, LambdaExpression> _assignments = new();
            private readonly ParameterExpression _param;

            public MemberInitBuilder(Type paramType)
            {
                _param = Expression.Parameter(paramType);
            }

            public MemberInitBuilder<TResult> Property(PropertyInfo property, LambdaExpression initializer)
            {
                if (_assignments.TryGetValue(property, out var existingInitializer))
                {
                    if (!ExpressionEqualityComparer.Instance.Equals(initializer, existingInitializer))
                    {
                        throw new InvalidOperationException($"Attempt to create two different initilizers ({existingInitializer} and {initializer})for a property {property.Name} of type {typeof(TResult)}.");
                    }

                    return this;
                }

                _assignments.Add(property, initializer);
                return this;
            }

            public LambdaExpression Lambda()
            {
                var ctor = typeof(TResult).GetConstructors().Single(c => c.GetParameters().Length == 0);
                var newExpr = Expression.New(ctor);
                var memberAssignments = _assignments.Select(kv => Expression.Bind(kv.Key, kv.Value.Body.ReplaceParameter(kv.Value.Parameters[0], _param)));
                var initExpr = Expression.MemberInit(newExpr, memberAssignments);

                return Expression.Lambda(initExpr, _param);
            }
        }

        public class ParameterRebinder<TKey, T> : ExpressionVisitor
            where T : Expression
            where TKey : Expression
        {
            private readonly IReadOnlyDictionary<TKey, T> _map;

            protected ParameterRebinder(IReadOnlyDictionary<TKey, T> map)
            {
                _map = map;
            }

            public static Expression ReplaceParameters(IReadOnlyDictionary<TKey, T> map, Expression exp)
            {
                return new ParameterRebinder<TKey, T>(map).Visit(exp);
            }

            public static Expression ReplaceParameter(
                Expression expression,
                TKey parameterExpression,
                T newExpression) => ReplaceParameters(
                    new Dictionary<TKey, T>
                    {
                        [parameterExpression] = newExpression,
                    },
                    expression);

            public override Expression Visit(Expression p)
            {
                if (p is TKey key && _map.TryGetValue(key, out var replacement))
                {
                    return replacement;
                }

                return base.Visit(p);
            }
        }

        private static class ValueAccessExpressionCacher<TEntity, TProperty>
            where TProperty : struct
        {
            private static readonly ConcurrentDictionary<Expression<Func<TEntity, TProperty?>>, Expression<Func<TEntity, TProperty>>> _cache = new(
                ExpressionEqualityComparer.Instance);

            public static Expression<Func<TEntity, TProperty>> MakeValueAccessExpression(Expression<Func<TEntity, TProperty?>> selector)
            {
                return _cache.GetOrAdd(selector, selector =>
                {
                    var paramExpression = Expression.Parameter(typeof(TEntity), selector.Parameters[0].Name);
                    var selectorExpression = ParameterRebinder<Expression, Expression>.ReplaceParameter(selector.Body, selector.Parameters[0], paramExpression);
                    var valueAccessExpression = Expression.Property(selectorExpression, CachedReflectionInfo.ForNullable<TProperty>.Value);
                    var result = Expression.Lambda<Func<TEntity, TProperty>>(valueAccessExpression, paramExpression);

                    return result;
                });
            }
        }

        private class FactorizationVisitor : ExpressionVisitor
        {
            private readonly List<Expression> _leftExpressions = new();
            private readonly List<Expression> _rightExpressions = new();
            private readonly IReadOnlyList<ParameterExpression> _parameters;
            private readonly List<ParameterExpression> _leftParameters = new();
            private readonly List<ParameterExpression> _rightParameters = new();

            private FactorizationVisitor(IReadOnlyCollection<ParameterExpression> parameters)
            {
                _parameters = parameters.ToList();
            }

            public static ExpressionFactorizationResult Factorize<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> expression)
            {
                var visitor = new FactorizationVisitor(expression.Parameters);
                var resultExpression = visitor.Visit(expression.Body);
                var result = new ExpressionFactorizationResult(
                    leftExpressions: visitor._leftExpressions.Select(e => Expression.Lambda(e, visitor._parameters[0])).ToList(),
                    rightExpressions: visitor._rightExpressions.Select(e => Expression.Lambda(e, visitor._parameters[1])).ToList(),
                    expression: Expression.Lambda(resultExpression, visitor._leftParameters.Concat(visitor._rightParameters)));

                return result;
            }

            public override Expression Visit(Expression node)
            {
                var parameters = ParameterVisitor.GetParameters(node, _parameters);

                if (parameters.Count == 1)
                {
                    if (parameters[0] == _parameters[0])
                    {
                        var parameter = Expression.Parameter(node.Type, $"Left_{_leftParameters.Count}");
                        _leftParameters.Add(parameter);
                        _leftExpressions.Add(node);
                        return parameter;
                    }
                    else
                    {
                        var parameter = Expression.Parameter(node.Type, $"Right_{_rightParameters.Count}");
                        _rightParameters.Add(parameter);
                        _rightExpressions.Add(node);
                        return parameter;
                    }
                }

                return base.Visit(node);
            }
        }

        private class ParameterVisitor : ExpressionVisitor
        {
            private readonly HashSet<ParameterExpression> _parameters = new();
            private readonly HashSet<ParameterExpression> _allowedParameters;

            private ParameterVisitor(IEnumerable<ParameterExpression> allowedParameters)
            {
                _allowedParameters = new HashSet<ParameterExpression>(allowedParameters);
            }

            public static IReadOnlyList<ParameterExpression> GetParameters(Expression expression, IEnumerable<ParameterExpression> allowedParameters)
            {
                var visitor = new ParameterVisitor(allowedParameters);
                visitor.Visit(expression);
                return visitor._parameters.ToList();
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (_allowedParameters.Contains(node))
                {
                    _parameters.Add(node);
                }

                return base.VisitParameter(node);
            }
        }
    }
}
