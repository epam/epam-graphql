// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Epam.GraphQL.Configuration;
using Epam.GraphQL.Loaders;
using GraphQL.Types;

#nullable enable

namespace Epam.GraphQL.Types
{
    internal class GroupEdgeGraphType<TChildLoader, TChildEntity, TExecutionContext> : ObjectGraphType<object>
        where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
        where TChildEntity : class
    {
        public GroupEdgeGraphType(RelationRegistry<TExecutionContext> registry)
        {
            var typeName = registry.GetProjectionTypeName<TChildLoader, TChildEntity>(false);

            Name = $"{typeName}GroupEdge";

            Description = $"An edge in a group connection from an object to another object of type `{typeName}`.";

            Field<NonNullGraphType<StringGraphType>>()
                .Name("cursor")
                .Description("A cursor for use in pagination");

            Field<GroupResultGraphType<TChildLoader, TChildEntity, TExecutionContext>>()
                .Name("node")
                .Description("The item at the end of the edge");
        }
    }
}
