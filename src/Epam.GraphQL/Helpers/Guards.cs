// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;

namespace Epam.GraphQL.Helpers
{
    internal static class Guards
    {
        public static void ThrowIfNull<T>([NotNull] T? argument, string paramName)
        {
            if (argument is null)
            {
                throw new ArgumentNullException(paramName);
            }
        }

        public static void ThrowIfNullOrEmpty([NotNull] string? argument, string paramName)
        {
            ThrowIfNull(argument, paramName);

            if (argument.Length == 0)
            {
                throw new ArgumentException("String argument is empty", paramName);
            }
        }

        public static void ThrowIfParameterless(LambdaExpression expression, string paramName)
        {
            if (expression.Parameters.Count == 0)
            {
                throw new ArgumentException("Expression must have one parameter at least.", paramName);
            }
        }

        public static void ThrowIfNegative(int? value, string paramName)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(paramName, $"`{paramName}` is out of range. Must be non-negative or null.");
            }
        }
    }
}
