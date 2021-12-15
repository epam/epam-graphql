// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Epam.GraphQL.Metadata;
using GraphQL.Introspection;
using GraphQL.Types;

namespace Epam.GraphQL.Types
{
    internal class ForeignKeyMetadataGraphType : ObjectGraphType<ForeignKeyMetadata>
    {
        public ForeignKeyMetadataGraphType(IGraphType fieldGraphType, IGraphType typeGraphType)
        {
            Field<NonNullGraphType<__Type>>("toType", null, null, context => context.Source.ToType)
                .ResolvedType = new NonNullGraphType(typeGraphType);

            Field<ListGraphType<NonNullGraphType<__Field>>>("fromField", null, null, context => context.Source.FromField)
                .ResolvedType = new ListGraphType(new NonNullGraphType(fieldGraphType));

            Field<ListGraphType<NonNullGraphType<__Field>>>("toField", null, null, context => context.Source.ToField)
                .ResolvedType = new ListGraphType(new NonNullGraphType(fieldGraphType));
        }
    }
}
