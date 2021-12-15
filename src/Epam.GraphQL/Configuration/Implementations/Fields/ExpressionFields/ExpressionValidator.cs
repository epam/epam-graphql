// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Epam.GraphQL.Extensions;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ExpressionFields
{
    internal static class ExpressionValidator
    {
        private static readonly Lazy<MemberVerifyingExpressionVisitor> _memberVerifyingExpressionVisitor = new(() => new MemberVerifyingExpressionVisitor());

        public static void Validate<TEntity, TResult>(Expression<Func<TEntity, TResult>> expression)
        {
            Validate<TEntity>(expression);
        }

        public static void Validate<TExecutionContext, TEntity, TResult>(Expression<Func<TExecutionContext, TEntity, TResult>> expression)
        {
            Validate<TEntity>(expression);
        }

        private static void Validate<TEntity>(LambdaExpression expression)
        {
            _memberVerifyingExpressionVisitor.Value.Visit(expression);
        }

        private class MemberVerifyingExpressionVisitor : ExpressionVisitor
        {
            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                if (!node.Method.IsStatic && node.MatchParameterAccess() == null)
                {
                    throw new InvalidOperationException($"Client projection ({node}) contains a call of instance method '{node.Method.Name}' of type '{node.Method.DeclaringType.HumanizedName()}'. This could potentially cause memory leak. Consider making the method static so that it does not capture constant in the instance.");
                }

                return base.VisitMethodCall(node);
            }

            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Member.DeclaringType.GetMembers(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Contains(node.Member) && node.MatchParameterAccess() == null)
                {
                    throw new InvalidOperationException($"Client projection ({node}) contains an access to a property/field that doesn't match any parameter. This could potentially cause memory leak. Consider making the method static so that it does not capture constant in the instance.");
                }

                return base.VisitMember(node);
            }
        }
    }
}
