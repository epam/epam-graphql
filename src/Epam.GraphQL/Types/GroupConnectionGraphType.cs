// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Epam.GraphQL.Configuration;
using Epam.GraphQL.Loaders;
using GraphQL.Types;
using GraphQL.Types.Relay;

#nullable enable

namespace Epam.GraphQL.Types
{
    internal class GroupConnectionGraphType<TChildLoader, TChildEntity, TExecutionContext> : ObjectGraphType<object>
        where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
        where TChildEntity : class
    {
        public GroupConnectionGraphType(RelationRegistry<TExecutionContext> registry)
        {
            // TODO Type name should be the same as an entity type name (e.g. for two loaders for the same entity and field set)
            var typeName = registry.GetProjectionTypeName<TChildLoader, TChildEntity>(false);

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

            Field<ListGraphType<GroupEdgeGraphType<TChildLoader, TChildEntity, TExecutionContext>>>()
                .Name("edges")
                .Description("Information to aid in pagination.");

            Field<ListGraphType<GroupResultGraphType<TChildLoader, TChildEntity, TExecutionContext>>>()
                .Name("items");
        }
    }
}
