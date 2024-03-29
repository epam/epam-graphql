// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Loaders;
using GraphQL.Types;
using GraphQLParser;
using GraphQLParser.AST;

namespace Epam.GraphQL.Types
{
    internal class SortDirectionGraphType : ScalarGraphType
    {
        public SortDirectionGraphType()
        {
            Name = nameof(SortDirection);
        }

        public override object? ParseLiteral(GraphQLValue value)
        {
            if (value is GraphQLEnumValue enumValue)
            {
                return ParseValue(enumValue.Name);
            }

            if (value is GraphQLStringValue stringValue)
            {
                return ParseValue(stringValue.Value);
            }

            return ThrowLiteralConversionError(value);
        }

        public override object ParseValue(object? value)
        {
            if (value is GraphQLName name)
            {
                value = name.StringValue;
            }

            if (value is ROM rom)
            {
                value = rom.ToString();
            }

            if (value is string stringValue)
            {
                if (stringValue.Equals("ASC", StringComparison.Ordinal) || stringValue.Equals("asc", StringComparison.Ordinal))
                {
                    return SortDirection.Asc;
                }

                if (stringValue.Equals("DESC", StringComparison.Ordinal) || stringValue.Equals("desc", StringComparison.Ordinal))
                {
                    return SortDirection.Desc;
                }
            }

            return ThrowValueConversionError(value);
        }

        public override object? Serialize(object? value)
        {
            if (value is SortDirection direction)
            {
                if (direction == SortDirection.Asc)
                {
                    return "ASC";
                }

                if (direction == SortDirection.Desc)
                {
                    return "DESC";
                }
            }

            return ThrowSerializationError(value);
        }
    }
}
