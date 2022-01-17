// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Linq;
using System.Linq.Expressions;

namespace Epam.GraphQL.Helpers
{
    internal class QueryableChecker : ExpressionVisitor
    {
        private readonly Expression _subExpression;

        private bool _isSubExpressionCalled;

        private QueryableChecker(Expression subExpression)
        {
            _subExpression = subExpression;

            if (subExpression is MethodCallExpression m && m.Object is ConstantExpression c)
            {
                _subExpression = c;
            }
        }

        public static bool IsSubExpression<T>(IQueryable<T> result, IQueryable<T> query)
        {
            if (result == query)
            {
                return true;
            }

            if (result.Expression == query.Expression)
            {
                return true;
            }

            var visitor = new QueryableChecker(query.Expression);
            visitor.Visit(result.Expression);
            return visitor._isSubExpressionCalled;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            if (node.Value == _subExpression || (_subExpression is ConstantExpression c && c.Value == node.Value))
            {
                _isSubExpressionCalled = true;
            }

            return base.VisitConstant(node);
        }

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Arguments.FirstOrDefault() == _subExpression)
            {
                _isSubExpressionCalled = true;
            }

            return base.VisitMethodCall(node);
        }
    }
}
