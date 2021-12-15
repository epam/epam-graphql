// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;

namespace Epam.GraphQL.Tests.Helpers
{
    public static class ExpressionExtensions
    {
        public static string ToTestName(this Expression expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            return expression.ToString()
                .Replace("=>", "⇒", StringComparison.InvariantCulture)
                .Replace("!=", "≠", StringComparison.InvariantCulture)
                .Replace("==", "≡", StringComparison.InvariantCulture)
                .Replace(".", "․", StringComparison.InvariantCulture)
                .Replace("=", "←", StringComparison.InvariantCulture)
                .Replace("(", "〈", StringComparison.InvariantCulture)
                .Replace(")", "〉", StringComparison.InvariantCulture);
        }
    }
}
