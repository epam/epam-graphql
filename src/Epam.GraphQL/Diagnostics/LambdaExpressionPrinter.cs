// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Linq.Expressions;
using Epam.GraphQL.Helpers;

namespace Epam.GraphQL.Diagnostics
{
    internal class LambdaExpressionPrinter : IPrinter
    {
        private readonly LambdaExpression? _expression;

        public LambdaExpressionPrinter(LambdaExpression? expression)
        {
            _expression = expression;
        }

        public string Print()
        {
            return _expression == null
                ? "null"
                : ExpressionPrinter.Print(_expression, noIndent: true);
        }
    }
}
