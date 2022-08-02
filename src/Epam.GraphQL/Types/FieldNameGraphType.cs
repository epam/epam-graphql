// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using GraphQL.Language.AST;
using GraphQL.Types;

namespace Epam.GraphQL.Types
{
    internal class FieldNameGraphType : StringGraphType
    {
        private readonly string[] _fieldNames;

        public FieldNameGraphType(string typeName, string[] fieldNames)
            : base()
        {
            // TODO Type name should be taken from registry?
            Name = $"{typeName}FieldName";
            _fieldNames = fieldNames;
        }

        public override object? ParseValue(object? value)
        {
            return ValidateResult(base.ParseValue(value));
        }

        public override object? ParseLiteral(IValue value)
        {
            return ValidateResult(base.ParseLiteral(value));
        }

        private string? ValidateResult(object? value)
        {
            var result = value as string;
            if (result != null && !_fieldNames.Contains(result))
            {
                throw new FormatException($"Failed to parse {Name} from input '{value}'. Input should be a string ({string.Join(", ", _fieldNames.Select(name => $"'{name}'"))}).");
            }

            return result;
        }
    }
}
