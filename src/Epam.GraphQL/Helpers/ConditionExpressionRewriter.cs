// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Extensions;

namespace Epam.GraphQL.Helpers
{
    internal static class ConditionExpressionRewriter
    {
        public static Func<IEnumerable<T1>, Expression<Func<T2, bool>>> Rewrite<T1, T2>(Expression<Func<T1, T2, bool>> expression)
        {
            return items => (Expression<Func<T2, bool>>)Optimize(BinaryExpressionVisitor<T1>.VisitExpr(items.Distinct(), expression));
        }

        public static LambdaExpression Optimize(LambdaExpression expression)
        {
            var result = expression;
            LambdaExpression previousResult;
            do
            {
                previousResult = result;
                result = (LambdaExpression)new OptimizeVisitor(result.Parameters.ToArray()).Visit(result);
            }
            while (result != previousResult);

            return result;
        }

        private class OptimizeVisitor : ExpressionVisitor
        {
            private readonly ParameterExpression[] _parameters;

            public OptimizeVisitor(ParameterExpression[] parameters)
            {
                _parameters = parameters;
            }

            protected override Expression VisitConstant(ConstantExpression node)
            {
                if (node.Type.IsEnumerableType() && !node.Type.IsArray)
                {
                    var elementType = node.Type.GetEnumerableElementType();
                    var result = CachedReflectionInfo.ForEnumerable.ToArray(elementType).InvokeAndHoistBaseException(null, node.Value);
                    return Expression.Constant(result);
                }

                return base.VisitConstant(node);
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (!node.ContainsAnyParameter(_parameters) && node.Arguments.All(expr => expr.NodeType is ExpressionType.Constant or ExpressionType.Lambda))
                {
                    var args = new object[node.Arguments.Count];
                    var index = 0;

                    foreach (var arg in node.Arguments)
                    {
                        if (arg is ConstantExpression constantExpr)
                        {
                            args[index] = constantExpr.Value;
                        }
                        else if (arg is LambdaExpression lambdaExpression)
                        {
                            args[index] = lambdaExpression.Compile();
                        }
                        else
                        {
                            return base.VisitMethodCall(node);
                        }

                        index++;
                    }

                    return Expression.Constant(node.Method.InvokeAndHoistBaseException(node.Object, args));
                }

                return base.VisitMethodCall(node);
            }
        }

        private class BinaryExpressionVisitor<T1> : ExpressionVisitor
        {
            private readonly ParameterExpression _independendParam;
            private readonly ParameterExpression _dependendParam;
            private readonly Expression _items;

            private BinaryExpressionVisitor(ParameterExpression dependendParam, ParameterExpression independendParam, Expression items)
            {
                _items = items;
                _independendParam = independendParam;
                _dependendParam = dependendParam;
            }

            public static LambdaExpression VisitExpr<T2>(IEnumerable<T1> items, Expression<Func<T1, T2, bool>> expr)
            {
                var constItems = Expression.Constant(items, typeof(IEnumerable<T1>));
                var visitor = new BinaryExpressionVisitor<T1>(expr.Parameters[0], expr.Parameters[1], constItems);

                var newExpr = (LambdaExpression)visitor.Visit(expr);
                return Expression.Lambda(newExpr.Body, expr.Parameters[1]);
            }

            protected override Expression VisitBinary(BinaryExpression node)
            {
                // If only right parameter depepends on parameter for rewriting...
                if (!node.Left.ContainsParameter(_dependendParam) && node.Right.ContainsParameter(_dependendParam))
                {
                    var rightNode = VisitExpr(node.Right, _dependendParam, _independendParam, _items);

                    // (e, d) => e.Id == d.Id
                    // (e, d) => 1 == d.Id
                    if (node.NodeType == ExpressionType.Equal)
                    {
                        // (e, d) => e.Id == d.Id    ------------->    items.Select(d => d.Id).Contains(e.Id)
                        // (e, d) => 1 == d.Id       ------------->    items.Select(d => d.Id).Contains(1)
                        return MakeContainsExpression(node.Left, rightNode, _items);
                    }

                    if (node.NodeType == ExpressionType.NotEqual)
                    {
                        return Expression.Not(MakeContainsExpression(node.Left, rightNode, _items));
                    }

                    if (IsLogicalExpression(node))
                    {
                        return Expression.MakeBinary(node.NodeType, node.Left, rightNode);
                    }

                    throw new NotSupportedException();
                }

                // If only left parameter depepends on parameter for rewriting...
                if (node.Left.ContainsParameter(_dependendParam) && !node.Right.ContainsParameter(_dependendParam))
                {
                    var leftNode = VisitExpr(node.Left, _dependendParam, _independendParam, _items);

                    if (node.NodeType == ExpressionType.Equal)
                    {
                        return MakeContainsExpression(node.Right, leftNode, _items);
                    }

                    if (node.NodeType == ExpressionType.NotEqual)
                    {
                        return Expression.Not(MakeContainsExpression(node.Right, leftNode, _items));
                    }

                    if (IsLogicalExpression(node))
                    {
                        return Expression.MakeBinary(node.NodeType, leftNode, node.Right);
                    }

                    throw new NotSupportedException();
                }

                // If both parameters depepends on parameter for rewriting...
                if (node.Left.ContainsParameter(_dependendParam) && node.Right.ContainsParameter(_dependendParam))
                {
                    // (e, d) => e.Id == d.Id && d.Id == 1
                    // (e, d) => d.Id == 1 && e.Id == d.Id
                    // (e, d) => e.Id == d.Id && e.AnotherId == d.AnotherId
                    // (e, d) => e.Id == d.Id || d.Id == 1
                    if (IsLogicalExpression(node))
                    {
                        // (e, d) => e.Id == d.Id && d.Id == 1
                        // (e, d) => e.Id == d.Id || d.Id == 1
                        if (node.Left.ContainsParameter(_independendParam))
                        {
                            // (e, d) => d.Id == 1 && d.Id == 1
                            if (node.NodeType is ExpressionType.And or ExpressionType.AndAlso)
                            {
                                return VisitExpr(
                                    node.Left,
                                    _dependendParam,
                                    _independendParam,
                                    MakeWhereExpression(_items, node.Right));
                            }

                            // (e, d) => d.Id == 1 || d.Id == 1
                            return Expression.MakeBinary(
                                node.NodeType,
                                VisitExpr(node.Left, _dependendParam, _independendParam, _items),
                                MakeAnyExpression(MakeWhereExpression(_items, node.Right)));
                        }

                        // (e, d) => d.Id == 1 && e.Id == d.Id
                        // (e, d) => d.Id == 1 || e.Id == d.Id
                        if (node.Right.ContainsParameter(_independendParam))
                        {
                            // (e, d) => d.Id == 1 && e.Id == d.Id
                            if (node.NodeType is ExpressionType.And or ExpressionType.AndAlso)
                            {
                                return VisitExpr(
                                    node.Right,
                                    _dependendParam,
                                    _independendParam,
                                    MakeWhereExpression(_items, node.Left));
                            }

                            // (e, d) => d.Id == 1 || e.Id == d.Id
                            return Expression.MakeBinary(
                                node.NodeType,
                                MakeAnyExpression(MakeWhereExpression(_items, node.Left)),
                                VisitExpr(node.Right, _dependendParam, _independendParam, _items));
                        }

                        // (e, d) => e.Id == d.Id && e.AnotherId == d.AnotherId
                        if (node.Left.ContainsParameter(_independendParam) && node.Right.ContainsParameter(_independendParam))
                        {
                            return Expression.MakeBinary(
                                node.NodeType,
                                VisitExpr(node.Left, _dependendParam, _independendParam, _items),
                                VisitExpr(node.Right, _dependendParam, _independendParam, _items));
                        }

                        // (e, d) => d.Id == 1 && d.Id == 1
                        // (e, d) => d.Id == 1 || d.Id == 1
                        return Expression.MakeBinary(
                            node.NodeType,
                            MakeAnyExpression(MakeWhereExpression(_items, node.Left)),
                            MakeAnyExpression(MakeWhereExpression(_items, node.Right)));
                    }

                    throw new NotSupportedException();
                }

                // Expression does not depend on parameter for rewriting...
                return node;
            }

            private static Expression VisitExpr(Expression expr, ParameterExpression dependendParam, ParameterExpression independendParam, Expression items)
            {
                var visitor = new BinaryExpressionVisitor<T1>(dependendParam, independendParam, items);
                return visitor.Visit(expr);
            }

            // TODO Move all this stuff to ExpressionExtensions
            private static bool IsLogicalExpression(Expression expression)
            {
                var nodeType = expression.NodeType;
                return nodeType is ExpressionType.And
                    or ExpressionType.AndAlso
                    or ExpressionType.Or
                    or ExpressionType.OrElse
                    or ExpressionType.Not;
            }

            private static Expression MakeAnyExpression(Expression items)
            {
                var anyMethodInfo = CachedReflectionInfo.ForEnumerable.Any<T1>();
                return Expression.Call(anyMethodInfo, items);
            }

            private Expression MakeWhereExpression(Expression items, Expression condition)
            {
                var whereMethodInfo = CachedReflectionInfo.ForEnumerable.Where<T1>();

                var paramExpression = Expression.Parameter(_dependendParam.Type);

                var predicateExpression = Expression.Lambda(
                    condition.ReplaceParameter(_dependendParam, paramExpression),
                    paramExpression);

                return Expression.Call(whereMethodInfo, items, predicateExpression);
            }

            private Expression MakeContainsExpression(Expression valueToCheck, Expression selectorExpression, Expression itemsExpression)
            {
                var paramExpression = Expression.Parameter(_dependendParam.Type);

                var keySelectorExpression = Expression.Lambda(
                    selectorExpression.ReplaceParameter(_dependendParam, paramExpression),
                    paramExpression);

                var selectMethodInfo = CachedReflectionInfo.ForEnumerable.Select(_dependendParam.Type, selectorExpression.Type);
                var selectCallExpr = Expression.Call(
                    selectMethodInfo,
                    itemsExpression,
                    keySelectorExpression);

                var distinctMethodInfo = CachedReflectionInfo.ForEnumerable.Distinct(selectorExpression.Type);
                var distinctCallExpr = Expression.Call(distinctMethodInfo, selectCallExpr);

                if (selectorExpression.Type.IsNullable())
                {
                    var whereMethodInfo = CachedReflectionInfo.ForEnumerable.Where(selectorExpression.Type);
                    var lambdaParam = Expression.Parameter(selectorExpression.Type);
                    var lambdaExpression = Expression.Lambda(Expression.NotEqual(lambdaParam, Expression.Constant(null)), lambdaParam);

                    distinctCallExpr = Expression.Call(whereMethodInfo, distinctCallExpr, lambdaExpression);
                }

                var containsMethodInfo = CachedReflectionInfo.ForEnumerable.Contains(selectorExpression.Type);
                return Expression.Call(containsMethodInfo, distinctCallExpr, valueToCheck);
            }
        }
    }
}
