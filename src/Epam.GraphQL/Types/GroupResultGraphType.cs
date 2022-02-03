// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Epam.GraphQL.Configuration.Implementations;
using Epam.GraphQL.Extensions;
using GraphQL.Types;

namespace Epam.GraphQL.Types
{
    internal class GroupResultGraphType<TReturnType, TExecutionContext> : ObjectGraphType<object>
    {
        public GroupResultGraphType(IGraphTypeDescriptor<TReturnType, TExecutionContext> graphType)
        {
            var typeName = graphType.Configurator?.Name ?? typeof(TReturnType).GraphQLTypeName(false);

            Name = $"{typeName}GroupResult";

            if (graphType.Configurator == null)
            {
                Field(graphType.Type, "item");
            }
            else
            {
                AddField(new FieldType()
                {
                    Name = "item",
                    ResolvedType = new NonNullGraphType(new GroupGraphType<TReturnType, TExecutionContext>(graphType.Configurator)),
                });
            }

            Field<NonNullGraphType<IntGraphType>>()
                .Name("count");
        }
    }
}
