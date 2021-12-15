// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Epam.GraphQL.Enums;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;

#nullable enable

namespace Epam.GraphQL.Extensions
{
    internal static class ExpressionExtensions
    {
        private static readonly Dictionary<ComparisonType, Func<Expression, Expression, BinaryExpression>> _comparisonFactories = new()
        {
            [ComparisonType.Eq] = Expression.Equal,
            [ComparisonType.Neq] = Expression.NotEqual,
            [ComparisonType.Gt] = Expression.GreaterThan,
            [ComparisonType.Lt] = Expression.LessThan,
            [ComparisonType.Gte] = Expression.GreaterThanOrEqual,
            [ComparisonType.Lte] = Expression.LessThanOrEqual,
        };

        public static bool ContainsParameter(this Expression expression, ParameterExpression parameter)
        {
            return ContainsAnyParameter(expression, parameter);
        }

        public static bool ContainsAnyParameter(this Expression expression, params ParameterExpression[] parameters)
        {
            return ParameterVisitor.ContainsAnyParameter(expression, parameters);
        }

        public static ExpressionInfo GetExpressionInfo<T1, T2>(this Expression<Func<T1, T2, bool>> expression)
        {
            if (expression.Body is not BinaryExpression logicalExpression)
            {
                throw new NotSupportedException("Only binary expressions are supported.");
            }

            if (logicalExpression.NodeType != ExpressionType.Equal)
            {
                throw new NotSupportedException("Only binary expressions of type ExpressionType.Equal are supported.");
            }

            var leftExpression = logicalExpression.Left.RemoveConvert();
            var rightExpression = logicalExpression.Right.RemoveConvert();

            var leftPropInfo = (leftExpression as MemberExpression)?.Member as PropertyInfo ??
                               throw new NotSupportedException(
                                   "Left operand does not refer to a property of the first argument of expression.");
            var rightPropInfo = (rightExpression as MemberExpression)?.Member as PropertyInfo ??
                                throw new NotSupportedException(
                                    "Right operand does not refer to a property of the second argument of expression.");

            var leftParameter = GetParameterOfType<T1>(leftExpression);
            if (leftParameter == null)
            {
                throw new NotSupportedException(
                    "Left operand does not refer to a property of the first argument of expression.");
            }

            var rightParameter = GetParameterOfType<T2>(rightExpression);
            if (rightParameter == null)
            {
                throw new NotSupportedException(
                    "Right operand does not refer to a property of the second argument of expression.");
            }

            if (leftParameter == expression.Parameters[1] && rightParameter == expression.Parameters[0])
            {
                return new ExpressionInfo
                {
                    RightPropertyInfo = leftPropInfo,
                    LeftPropertyInfo = rightPropInfo,
                    RightExpression = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(T1), leftPropInfo.PropertyType), leftExpression, leftParameter),
                    LeftExpression = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(T2), rightPropInfo.PropertyType), rightExpression, rightParameter),
                };
            }

            return new ExpressionInfo
            {
                LeftPropertyInfo = leftPropInfo,
                RightPropertyInfo = rightPropInfo,
                LeftExpression = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(T1), leftPropInfo.PropertyType), leftExpression, leftParameter),
                RightExpression = Expression.Lambda(typeof(Func<,>).MakeGenericType(typeof(T2), rightPropInfo.PropertyType), rightExpression, rightParameter),
            };
        }

        public static Expression RemoveConvert(this Expression expression)
        {
            if (expression is UnaryExpression unaryExpr
                && (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked))
            {
                return RemoveConvert(unaryExpr.Operand);
            }

            return expression;
        }

        public static Expression RemoveQuote(this Expression expression)
        {
            if (expression is UnaryExpression unaryExpr
                && (expression.NodeType == ExpressionType.Quote))
            {
                return RemoveQuote(unaryExpr.Operand);
            }

            return expression;
        }

        public static Expression RemoveTypeAsAndConvert(this Expression expression)
        {
            while (expression is UnaryExpression unaryExpr
                && (expression.NodeType == ExpressionType.Convert || expression.NodeType == ExpressionType.ConvertChecked || expression.NodeType == ExpressionType.TypeAs))
            {
                return RemoveTypeAsAndConvert(unaryExpr.Operand);
            }

            return expression;
        }

        public static ParameterExpression? MatchParameterAccess(this Expression expression)
        {
            expression = RemoveTypeAsAndConvert(expression);
            if (expression is MemberExpression memberExpression)
            {
                var expr = RemoveTypeAsAndConvert(memberExpression.Expression);
                if (expr is ParameterExpression paramExpr)
                {
                    return paramExpr;
                }

                return MatchParameterAccess(expr);
            }

            if (expression is MethodCallExpression methodCallExpression)
            {
                var expr = RemoveTypeAsAndConvert(methodCallExpression.Object);
                if (expr is ParameterExpression paramExpr)
                {
                    return paramExpr;
                }

                return MatchParameterAccess(expr);
            }

            return null;
        }

        public static Expression ReplaceParameter<TParamType, TReturnType>(this Expression<Func<TParamType, TReturnType>> expression, ParameterExpression parameter)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (!expression.Parameters[0].Type.IsAssignableFrom(parameter.Type))
            {
                throw new InvalidCastException($"Cannot replace parameter of type {expression.Parameters[0].Type.Name} by parameter of type {parameter.Type.Name}.");
            }

            return ExpressionHelpers.ParameterRebinder.ReplaceParameter(expression.Body, expression.Parameters[0], parameter);
        }

        public static Expression ReplaceParameter(this Expression expression, ParameterExpression parameter, Expression parameterReplacement)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (parameter == null)
            {
                throw new ArgumentNullException(nameof(parameter));
            }

            if (parameterReplacement == null)
            {
                throw new ArgumentNullException(nameof(parameterReplacement));
            }

            return ExpressionHelpers.ParameterRebinder.ReplaceParameter(expression, parameter, parameterReplacement);
        }

        public static LambdaExpression ReplaceFirstParameter(this LambdaExpression expression, Expression parameterReplacement)
        {
            if (expression.Parameters.Count == 0)
            {
                throw new ArgumentException("Expression must have one parameter at least.", nameof(expression));
            }

            if (!expression.Parameters[0].Type.IsAssignableFrom(parameterReplacement.Type))
            {
                throw new ArgumentException($"Cannot replace parameter of type {expression.Parameters[0].Type.Name} by expression of type {parameterReplacement.Type.Name}.", nameof(expression));
            }

            var body = expression.Body.ReplaceParameter(expression.Parameters[0], parameterReplacement);
            var result = Expression.Lambda(body, expression.Parameters.Skip(1));

            return result;
        }

        public static Expression<Func<T2, TResult>> BindFirstParameter<T1, T2, TResult>(this Expression<Func<T1, T2, TResult>> expression, T1 value)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var secondParam = Expression.Parameter(expression.Parameters[1].Type, expression.Parameters[1].Name);
            var body = expression.Body
                .ReplaceParameter(expression.Parameters[0], Expression.Constant(value, typeof(T1)))
                .ReplaceParameter(expression.Parameters[1], secondParam);

            var result = Expression.Lambda<Func<T2, TResult>>(body, secondParam);

            return result;
        }

        public static LambdaExpression BindFirstParameter<T>(this LambdaExpression expression, T value)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var body = expression.Body.ReplaceParameter(expression.Parameters[0], Expression.Constant(value, typeof(T)));
            var result = Expression.Lambda(body, expression.Parameters.Skip(1));

            return result;
        }

        public static bool IsProperty<TSource, TProperty>(this Expression<Func<TSource, TProperty>> propertyLambda)
        {
            if (propertyLambda.Body is not MemberExpression member)
            {
                return false;
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                return false;
            }

            if (member.Expression is not ParameterExpression)
            {
                return false;
            }

            var type = typeof(TSource);
            if (type != propInfo.ReflectedType && !propInfo.ReflectedType.IsAssignableFrom(type))
            {
                return false;
            }

            return true;
        }

        public static PropertyInfo GetPropertyInfo<TSource, TProperty>(
            this Expression<Func<TSource, TProperty>> propertyLambda)
        {
            MemberExpression? member;

            if (propertyLambda.Body is UnaryExpression unaryExpression && unaryExpression.NodeType == ExpressionType.Convert)
            {
                member = unaryExpression.Operand as MemberExpression;
                if (member == null)
                {
                    throw new ArgumentException($"Expression '{propertyLambda}' does not refer to a property.");
                }
            }
            else
            {
                member = propertyLambda.Body as MemberExpression;
                if (member == null)
                {
                    throw new ArgumentException($"Expression '{propertyLambda}' refers to a method, not a property.");
                }
            }

            var propInfo = member.Member as PropertyInfo;
            if (propInfo == null)
            {
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a field, not a property.");
            }

            var type = typeof(TSource);
            if (type != propInfo.ReflectedType && !propInfo.ReflectedType.IsAssignableFrom(type))
            {
                throw new ArgumentException(
                    $"Expression '{propertyLambda}' refers to a property that is not from type {type}.");
            }

            if (member.Expression is not ParameterExpression)
            {
                throw new ArgumentException($"Expression '{propertyLambda}' refers to a nested member.");
            }

            return type.GetProperty(propInfo.Name) ?? propInfo;
        }

        public static Action<TSource, TProperty> GetSetter<TSource, TProperty>(
            this Expression<Func<TSource, TProperty>> propertyLambda)
        {
            var propInfo = propertyLambda.GetPropertyInfo();
            var setMethodInfo = propInfo.GetSetMethod();
            if (setMethodInfo == null)
            {
                throw new ArgumentException(
                    $"Expression '{propertyLambda}' refers to a property that does not have a setter.");
            }

#pragma warning disable CS8601 // Possible null reference assignment.
            return (source, value) => setMethodInfo.Invoke(source, new object[] { value });
#pragma warning restore CS8601 // Possible null reference assignment.
        }

        public static Func<TSource, TProperty> GetGetter<TSource, TProperty>(
            this Expression<Func<TSource, TProperty>> propertyLambda)
        {
            var propInfo = propertyLambda.GetPropertyInfo();
            var getMethodInfo = propInfo.GetGetMethod();
            if (getMethodInfo == null)
            {
                throw new ArgumentException(
                    $"Expression '{propertyLambda}' refers to a property that does not have a getter.");
            }

            return source => (TProperty)getMethodInfo.Invoke(source, null);
        }

        /// <summary>
        ///     Negates the predicate.
        /// </summary>
        public static Expression<Func<T, bool>> Not<T>(this Expression<Func<T, bool>> expression)
        {
            var negated = Expression.Not(expression.Body);
            return Expression.Lambda<Func<T, bool>>(negated, expression.Parameters);
        }

        /// <summary>
        ///     Combines the first predicate with the second using the logical "and".
        /// </summary>
        public static Expression<Func<T, bool>> And<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.AndAlso);
        }

        /// <summary>
        ///     Combines the first predicate with the second using the logical "or".
        /// </summary>
        public static Expression<Func<T, bool>> Or<T>(
            this Expression<Func<T, bool>> first,
            Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.OrElse);
        }

        public static Expression<Func<TEntity, bool>>? SafeAnd<TEntity>(this Expression<Func<TEntity, bool>>? left, Expression<Func<TEntity, bool>>? right)
        {
            if (left == null)
            {
                return right;
            }

            if (right == null)
            {
                return left;
            }

            return left.And(right);
        }

        public static Expression<Func<T2, T1, TResult>> SwapOperands<T1, T2, TResult>(
            this Expression<Func<T1, T2, TResult>> expression)
        {
            if (expression.Body is not BinaryExpression logicalExpression)
            {
                return Expression.Lambda<Func<T2, T1, TResult>>(
                    expression.Body,
                    expression.Parameters[1],
                    expression.Parameters[0]);
            }

            logicalExpression = Expression.MakeBinary(
                logicalExpression.NodeType,
                logicalExpression.Right,
                logicalExpression.Left);

            return Expression.Lambda<Func<T2, T1, TResult>>(
                logicalExpression,
                expression.Parameters[1],
                expression.Parameters[0]);
        }

        public static IReadOnlyList<(LambdaExpression SortExpression, SortDirection SortDirection)> GetSorters<T>(this Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderExpression)
        {
            return SortVisitor<T>.GetSorters(orderExpression);
        }

        public static Expression<Func<IOrderedQueryable<T>, IOrderedQueryable<T>>> GetThenBy<T>(this Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderExpression)
        {
            var sorters = orderExpression.GetSorters();

            if (sorters.Count == 0)
            {
                throw new ArgumentException("Expression must contain one IQueryable<>.OrderBy call at least.", nameof(orderExpression));
            }

            var param = Expression.Parameter(typeof(IOrderedQueryable<T>));
            Expression body = param;
            foreach (var sorter in sorters)
            {
                var thenByCall = Expression.Quote(sorter.SortExpression);
                var methodInfo = sorter.SortDirection == SortDirection.Asc
                    ? CachedReflectionInfo.ForQueryable.ThenBy(typeof(T), sorter.SortExpression.ReturnType)
                    : CachedReflectionInfo.ForQueryable.ThenByDescending(typeof(T), sorter.SortExpression.ReturnType);

                body = Expression.Call(methodInfo, body, thenByCall);
            }

            return Expression.Lambda<Func<IOrderedQueryable<T>, IOrderedQueryable<T>>>(body, param);
        }

        public static Expression<Func<TEntity, bool>> MakeComparisonExpression<TEntity, TPropertyType>(this Expression<Func<TEntity, TPropertyType>> propertyExpression, ComparisonType comparisonType, TPropertyType value)
        {
            var exprParam = Expression.Parameter(typeof(TEntity));

            var factory = _comparisonFactories[comparisonType];

            var equalsExpr = factory(
                propertyExpression.ReplaceParameter(exprParam),
                Expression.Constant(value, typeof(TPropertyType)));

            var result = Expression.Lambda<Func<TEntity, bool>>(equalsExpr, exprParam);
            return result;
        }

        public static Type GetResultType(this LambdaExpression expression)
        {
            var funcType = expression.GetType().GetGenericArguments().First();
            var resultType = funcType.GetGenericArguments().Last();

            return resultType;
        }

        public static bool IsTrue(this Expression expression)
        {
            return expression is ConstantExpression constantExpr && constantExpr.Value is bool value && value;
        }

        public static bool IsFalse(this Expression expression)
        {
            return expression is ConstantExpression constantExpr && constantExpr.Value is bool value && !value;
        }

        /// <summary>
        ///     Combines the first expression with the second using the specified merge function.
        /// </summary>
        private static Expression<T> Compose<T>(
            this Expression<T> first,
            Expression<T> second,
            Func<Expression, Expression, Expression> merge)
        {
            // zip parameters (map from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] })
                           .ToDictionary(p => p.s, p => p.f as Expression);

            // replace parameters in the second lambda expression with the parameters in the first
            var secondBody = ExpressionHelpers.ParameterRebinder.ReplaceParameters(map, second.Body);

            // create a merged lambda expression with parameters from the first expression
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        private static ParameterExpression? GetParameterOfType<T>(Expression expression)
        {
            if (expression is not MemberExpression memberExpression)
            {
                return null;
            }

            if (memberExpression.Expression is ParameterExpression parameter)
            {
                return parameter.Type == typeof(T) ? parameter : null;
            }

            return GetParameterOfType<T>(memberExpression.Expression);
        }

        private class ParameterVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression[] _parameters;
            private bool _containsAnyParam;

            private ParameterVisitor(ParameterExpression[] parameters)
            {
                _parameters = parameters;
            }

            public static bool ContainsAnyParameter(Expression expr, ParameterExpression[] parameters)
            {
                var visitor = new ParameterVisitor(parameters);
                visitor.Visit(expr);
                return visitor._containsAnyParam;
            }

            protected override Expression VisitParameter(ParameterExpression node)
            {
                if (_parameters.Contains(node))
                {
                    _containsAnyParam = true;
                }

                return base.VisitParameter(node);
            }
        }

        private class SortVisitor<T> : ExpressionVisitor
        {
            private readonly List<(LambdaExpression SortExpression, SortDirection SortDirection)> _sorters = new();
            private bool _stop;

            public static IReadOnlyList<(LambdaExpression SortExpression, SortDirection SortDirection)> GetSorters(Expression<Func<IQueryable<T>, IOrderedQueryable<T>>> orderExpression)
            {
                var visitor = new SortVisitor<T>();

                visitor.Visit(orderExpression.Body);

                visitor._sorters.Reverse();
                return visitor._sorters;
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (node.Method.IsGenericMethod && node.Arguments.Count >= 2)
                {
                    var method = node.Method.GetGenericMethodDefinition();
                    var expr = node.Arguments[1].RemoveQuote();

                    if (method == CachedReflectionInfo.ForQueryable.GenericOrderBy || method == CachedReflectionInfo.ForQueryable.GenericOrderByWithComparer)
                    {
                        return AddSorter(node, expr, SortDirection.Asc, true);
                    }
                    else if (method == CachedReflectionInfo.ForQueryable.GenericOrderByDescending || method == CachedReflectionInfo.ForQueryable.GenericOrderByDescendingWithComparer)
                    {
                        return AddSorter(node, expr, SortDirection.Desc, true);
                    }
                    else if (method == CachedReflectionInfo.ForQueryable.GenericThenBy || method == CachedReflectionInfo.ForQueryable.GenericThenByWithComparer)
                    {
                        return AddSorter(node, expr, SortDirection.Asc, false);
                    }
                    else if (method == CachedReflectionInfo.ForQueryable.GenericThenByDescending || method == CachedReflectionInfo.ForQueryable.GenericThenByDescendingWithComparer)
                    {
                        return AddSorter(node, expr, SortDirection.Desc, false);
                    }
                }

                throw new ArgumentException($"Expression should not contain method calls except OrderBy, OrderByDescending, ThenBy and ThenByDescending. Call of {node.Method.Name} method was found.");
            }

            protected override Expression VisitLambda<TDelegate>(Expression<TDelegate> node)
            {
                return node;
            }

            private Expression AddSorter(MethodCallExpression node, Expression expr, SortDirection direction, bool stop)
            {
                if (!_stop)
                {
                    _sorters.Add(((LambdaExpression)expr, direction));
                }

                _stop = stop;
                return base.VisitMethodCall(node);
            }
        }
    }

    internal class ExpressionInfo
    {
        public PropertyInfo LeftPropertyInfo { get; set; } = null!;

        public PropertyInfo RightPropertyInfo { get; set; } = null!;

        public LambdaExpression LeftExpression { get; set; } = null!;

        public LambdaExpression RightExpression { get; set; } = null!;
    }
}
