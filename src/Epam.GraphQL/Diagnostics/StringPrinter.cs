// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

namespace Epam.GraphQL.Diagnostics
{
    internal class StringPrinter : IPrinter
    {
        private readonly string? _value;

        public StringPrinter(string? value)
        {
            _value = value;
        }

        public string Print()
        {
            return _value != null ? $"\"{_value}\"" : "null";
        }
    }
}
