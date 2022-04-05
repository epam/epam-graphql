// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Text;
using Epam.GraphQL.Extensions;

namespace Epam.GraphQL.Diagnostics
{
    internal class ArrayPrinter<T> : IPrinter<T[]?>
    {
        private readonly IPrinter<T> _itemPrinter;

        public ArrayPrinter(IPrinter<T> itemPrinter)
        {
            _itemPrinter = itemPrinter;
        }

        public string Print(T[]? value)
        {
            if (value == null)
            {
                return "null";
            }

            if (value.Length == 0)
            {
                return $"Array.Empty<{typeof(T).HumanizedName()}>()";
            }

            var builder = new StringBuilder();

            builder.Append("new ");
            builder.Append(typeof(T).HumanizedName());
            builder.Append("[] {");

            for (var i = 0; i < value.Length; i++)
            {
                if (i > 0)
                {
                    builder.Append(", ");
                }

                builder.Append(_itemPrinter.Print(value[i]));
            }

            builder.Append('}');

            return builder.ToString();
        }
    }
}
