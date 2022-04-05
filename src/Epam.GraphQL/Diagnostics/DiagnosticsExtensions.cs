// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Text;

namespace Epam.GraphQL.Diagnostics
{
    internal static class DiagnosticsExtensions
    {
        public static string Print(this MethodBase methodBase)
        {
            if (TryGetLocalName(methodBase.Name, out var methodBody))
            {
                return methodBody;
            }

            if (IsAnonymous(methodBase.Name))
            {
                return $"{methodBase.PrintArguments()} => ...";
            }

            return methodBase.Name;

            static bool IsAnonymous(string methodName)
            {
                return methodName.Contains('<', StringComparison.Ordinal) && methodName.Contains(">b__", StringComparison.Ordinal);
            }

            static bool TryGetLocalName(string methodName, [NotNullWhen(true)] out string? localName)
            {
                if (methodName.Contains('<', StringComparison.Ordinal))
                {
                    var startPos = methodName.IndexOf(">g__", StringComparison.Ordinal);
                    var endPos = methodName.IndexOf("|", StringComparison.Ordinal);

                    if (startPos >= 0 && endPos >= 0)
                    {
                        localName = methodName.Substring(startPos + 4, endPos - startPos - 4);
                        return true;
                    }
                }

                localName = null;
                return false;
            }
        }

        public static string PrintArguments(this MethodBase methodBase)
        {
            var parameterNames = methodBase.GetParameters().Select(param => param.Name).ToArray();

            var parameters = parameterNames.Length switch
            {
                0 => "()",
                1 => $"{parameterNames[0]}",
                _ => $"({string.Join(", ", parameterNames)})",
            };

            return parameters;
        }

        public static string ToString(this IConfigurationContext context, int indent, params IConfigurationContext[] choosenItems)
        {
            var sb = new StringBuilder();
            context.Append(sb, choosenItems, indent);

            return sb.ToString();
        }

        public static void AddError(this IConfigurationContext context, string message, params IConfigurationContext[] invalidItems)
        {
            GetRoot(context).AddError(message, invalidItems);
        }

        public static void AddErrorIf(this IConfigurationContext context, bool condition, string message, params IConfigurationContext[] invalidItems)
        {
            if (condition)
            {
                context.AddError(message, invalidItems);
            }
        }

        public static string GetError(this IConfigurationContext context, string message, params IConfigurationContext[] invalidItems)
        {
            return GetRoot(context).GetError(message, invalidItems);
        }

        public static void ThrowErrors(this IConfigurationContext context)
        {
            GetRoot(context).ThrowErrors();
        }

        public static IRootConfigurationContext GetRoot(this IConfigurationContext context)
        {
            if (context.Parent == null)
            {
                return (IRootConfigurationContext)context;
            }

            return GetRoot(context.Parent);
        }
    }
}
