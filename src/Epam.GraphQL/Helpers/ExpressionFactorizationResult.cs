// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Collections.Generic;
using System.Linq.Expressions;

namespace Epam.GraphQL.Helpers
{
    internal class ExpressionFactorizationResult
    {
        public ExpressionFactorizationResult(IReadOnlyList<LambdaExpression> leftExpressions, IReadOnlyList<LambdaExpression> rightExpressions, LambdaExpression expression)
        {
            LeftExpressions = leftExpressions;
            RightExpressions = rightExpressions;
            Expression = expression;
        }

        public IReadOnlyList<LambdaExpression> LeftExpressions { get; set; }

        public IReadOnlyList<LambdaExpression> RightExpressions { get; set; }

        public LambdaExpression Expression { get; set; }
    }
}
