// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using Epam.GraphQL.Extensions;
using GraphQL;

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

        public static void ThrowArgumentExceptionIf([DoesNotReturnIf(true)] bool shouldThrow, string message, string paramName)
        {
            if (shouldThrow)
            {
                throw new ArgumentException(message, paramName);
            }
        }

        public static void ThrowIfNullOrEmpty([NotNull] string? argument, string paramName)
        {
            ThrowIfNull(argument, paramName);

            if (argument.Length == 0)
            {
                throw new ArgumentException("String value cannot be empty.", paramName);
            }
        }

        public static void ThrowIfNegative(int? value, string paramName)
        {
            if (value < 0)
            {
                throw new ArgumentOutOfRangeException(paramName, $"`{paramName}` is out of range. Must be non-negative or null.");
            }
        }

        public static void ThrowInvalidOperationIf([DoesNotReturnIf(true)] bool shouldThrow, string message)
        {
            if (shouldThrow)
            {
                throw new InvalidOperationException(message);
            }
        }

        public static void AssertField([DoesNotReturnIf(true)] bool shouldThrow, IResolveFieldContext context)
        {
            if (shouldThrow)
            {
                throw new InvalidOperationException($"Processing of the field `{context.GetPath()}` failed. This may indicate either a bug (you can create an issue https://epa.ms/gql-bug) or a limitation in Epam.GraphQL.");
            }
        }

        public static void AssertType<T>([DoesNotReturnIf(true)] bool shouldThrow)
        {
            if (shouldThrow)
            {
                throw new InvalidOperationException($"Processing of the type `{typeof(T).HumanizedName()}` failed. This may indicate either a bug (you can create an issue https://epa.ms/gql-bug) or a limitation in Epam.GraphQL.");
            }
        }

        public static void ThrowNotSupportedIf([DoesNotReturnIf(true)] bool shouldThrow)
        {
            if (shouldThrow)
            {
                throw new NotSupportedException();
            }
        }

        public static void ShouldHaveOneParameterAtLeast(this LambdaExpression expression, string paramName)
        {
            if (expression.Parameters.Count == 0)
            {
                throw new ArgumentException("Expression must have one parameter at least.", paramName);
            }
        }

        public static void ShouldHaveOnlyOneParameter(this LambdaExpression expression, string paramName)
        {
            if (expression.Parameters.Count == 0)
            {
                throw new ArgumentException("Expression must have only one parameter.", paramName);
            }
        }
    }
}
