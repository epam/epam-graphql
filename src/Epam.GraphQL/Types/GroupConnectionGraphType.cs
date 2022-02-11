// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Epam.GraphQL.Configuration.Implementations;
using Epam.GraphQL.Extensions;
using GraphQL.Types;
using GraphQL.Types.Relay;

namespace Epam.GraphQL.Types
{
    internal class GroupConnectionGraphType<TChildEntity, TExecutionContext> : ObjectGraphType<object>
    {
        public GroupConnectionGraphType(IGraphTypeDescriptor<TChildEntity, TExecutionContext> graphType)
        {
            // TODO Type name should be the same as an entity type name (e.g. for two loaders for the same entity and field set)
            var typeName = graphType.Configurator?.Name ?? typeof(TChildEntity).GraphQLTypeName(false);

            Name = $"{typeName}GroupConnection";

            Field<IntGraphType>()
                .Name("totalCount")
                .Description(
                    "A count of the total number of objects in this connection, ignoring pagination. " +
                    "This allows a client to fetch the first five objects by passing \"5\" as the argument " +
                    "to `first`, then fetch the total count so it could display \"5 of 83\", for example. " +
                    "In cases where we employ infinite scrolling or don't have an exact count of entries, " +
                    "this field will return `null`.");

            Field<NonNullGraphType<PageInfoType>>()
                .Name("pageInfo")
                .Description("Information to aid in pagination.");

            var resultGraphType = new GroupResultGraphType<TChildEntity, TExecutionContext>(graphType);

            AddField(
                new FieldType
                {
                    Name = "edges",
                    Description = "Information to aid in pagination.",
                    ResolvedType = new ListGraphType(new GroupEdgeGraphType<TChildEntity, TExecutionContext>(typeName, resultGraphType)),
                });

            AddField(
                new FieldType
                {
                    Name = "items",
                    ResolvedType = new ListGraphType(resultGraphType),
                });
        }
    }
}
