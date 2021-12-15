// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Extensions;

#nullable enable

namespace Epam.GraphQL.Helpers
{
    internal static class ExpressionRewriter
    {
        public static Expression Rewrite(Expression expression)
            => RewriteImpl(expression);

        public static Expression<Func<T, TResult>> Rewrite<T, TResult>(Expression<Func<T, TResult>> expression)
            => RewriteImpl(expression);

        public static Expression<Func<T1, T2, TResult>> Rewrite<T1, T2, TResult>(Expression<Func<T1, T2, TResult>> expression)
            => RewriteImpl(expression);

        public static TExpression RewriteImpl<TExpression>(TExpression expression)
            where TExpression : Expression
        {
            var visitor = new RewriteVisitor();
            var result = visitor.Visit(expression);
            return (TExpression)result;
        }

        private class RewriteVisitor : ExpressionVisitor
        {
            protected override Expression VisitBinary(BinaryExpression node)
            {
                if (node.NodeType == ExpressionType.AndAlso)
                {
                    var left = Rewrite(node.Left);
                    var right = Rewrite(node.Right);

                    if (left.IsFalse() || right.IsFalse())
                    {
                        return Expression.Constant(false);
                    }

                    if (left.IsTrue())
                    {
                        return right;
                    }

                    if (right.IsTrue())
                    {
                        return left;
                    }
                }
                else if (node.NodeType == ExpressionType.OrElse)
                {
                    var left = Rewrite(node.Left);
                    var right = Rewrite(node.Right);

                    if (left.IsTrue() || right.IsTrue())
                    {
                        return Expression.Constant(true);
                    }

                    if (left.IsFalse())
                    {
                        return right;
                    }

                    if (right.IsFalse())
                    {
                        return left;
                    }
                }

                return base.VisitBinary(node);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                var newExpression = node.Expression as NewExpression;

                if (newExpression == null)
                {
                    var memberInitExpression = node.Expression as MemberInitExpression;

                    if (memberInitExpression == null
                        && node.Expression is UnaryExpression unaryExpression
                        && unaryExpression.NodeType == ExpressionType.TypeAs)
                    {
                        newExpression = unaryExpression.Operand as NewExpression;

                        if (newExpression == null)
                        {
                            memberInitExpression = unaryExpression.Operand as MemberInitExpression;
                        }
                    }

                    if (memberInitExpression != null)
                    {
                        var binding = memberInitExpression.Bindings.FirstOrDefault(binding => binding.Member == node.Member);

                        if (binding is not null and MemberAssignment memberAssignment)
                        {
                            return memberAssignment.Expression;
                        }
                    }
                }

                if (newExpression != null && newExpression.Members != null)
                {
                    var indexOfMember = newExpression.Members.IndexOf(node.Member);
                    if (indexOfMember >= 0)
                    {
                        return newExpression.Arguments[indexOfMember];
                    }
                }

                return base.VisitMember(node);
            }
        }
    }
}
