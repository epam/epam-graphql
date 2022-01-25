// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Globalization;
using GraphQL.Language.AST;
using GraphQL.Types;

namespace Epam.GraphQL.Types
{
    internal class LegacyDateTimeGraphType : DateTimeGraphType
    {
        public LegacyDateTimeGraphType()
        {
            Name = "DateTime";
        }

        public override object? ParseLiteral(IValue value)
        {
            return value switch
            {
                NullValue => null,
                StringValue stringValue => DateTimeOffset.Parse(stringValue.Value, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal).UtcDateTime,
                _ => ThrowLiteralConversionError(value),
            };
        }

        public override object? ParseValue(object? value)
        {
            return value switch
            {
                null => null,
                string stringValue => DateTimeOffset.Parse(stringValue, DateTimeFormatInfo.InvariantInfo, DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal).UtcDateTime,
                DateTime dateTimeValue => dateTimeValue,
                _ => ThrowValueConversionError(value),
            };
        }
    }
}
