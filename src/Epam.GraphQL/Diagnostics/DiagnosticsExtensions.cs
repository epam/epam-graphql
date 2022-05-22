// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Text;

namespace Epam.GraphQL.Diagnostics
{
    internal static class DiagnosticsExtensions
    {
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
