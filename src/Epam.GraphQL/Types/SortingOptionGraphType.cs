// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Epam.GraphQL.Loaders;
using GraphQL.Types;

#nullable enable

namespace Epam.GraphQL.Types
{
    internal class SortingOptionGraphType : InputObjectGraphType<SortingOption>
    {
        internal SortingOptionGraphType(string typeName, string[] fieldNames)
        {
            // TODO Type name should be taken from registry?
            Name = $"{typeName}{nameof(SortingOption)}";
            Field(s => s.Field).Configure(field =>
            {
                field.ResolvedType = new FieldNameGraphType(typeName, fieldNames);
                field.Name = "field";
            });
            Field<SortDirectionGraphType>("direction", resolve: s => s.Source.Direction)
                .ResolvedType = new SortDirectionGraphType();
        }
    }
}
