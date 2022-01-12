// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Epam.GraphQL.Metadata;
using GraphQL.Introspection;
using GraphQL.Types;

namespace Epam.GraphQL.Types
{
    internal class TypeMetadataGraphType : ObjectGraphType<TypeMetadata>
    {
        public TypeMetadataGraphType(IGraphType fieldGraphType, IGraphType typeGraphType)
        {
            Field<ListGraphType<NonNullGraphType<__Field>>>("primaryKey", null, null, context => context.Source.PrimaryKey)
                .ResolvedType = new ListGraphType(new NonNullGraphType(fieldGraphType));

            Field<ListGraphType<ForeignKeyMetadataGraphType>>("foreignKeys", null, null, context => context.Source.ForeignKeys)
                .ResolvedType = new ListGraphType(new ForeignKeyMetadataGraphType(fieldGraphType, typeGraphType));
        }
    }
}
