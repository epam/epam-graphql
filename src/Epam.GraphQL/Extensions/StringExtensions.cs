// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

#nullable enable

using System;

namespace Epam.GraphQL.Extensions
{
    internal static class StringExtensions
    {
        public static string? CapitalizeFirstLetter(this string? str)
        {
            if (string.IsNullOrEmpty(str))
            {
                return str;
            }

            if (str!.Length == 1)
            {
                return str.ToUpperInvariant();
            }

            return $"{str.Substring(0, 1).ToUpperInvariant()}{str[1..]}";
        }

        public static string ToJsonString(this string? str)
        {
            if (str == null)
            {
                return "null";
            }

            return $"\"{str.Replace("\"", "\\\"", StringComparison.InvariantCulture)}\"";
        }
    }
}
