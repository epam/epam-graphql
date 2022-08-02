// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;

namespace Epam.GraphQL.Diagnostics.Internals
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
    }
}
