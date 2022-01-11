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
    internal class ConnectionGraphType<TChildLoader, TChildEntity, TExecutionContext> : ObjectGraphType<object>
        where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
        where TChildEntity : class
    {
        private const string ItemsDescription = "A list of all of the objects returned in the connection. This is a convenience field provided " +
                "for quickly exploring the API; rather than querying for \"{ edges { node } }\" when no edge data " +
                "is needed, this field can be used instead. Note that when clients like Relay need to fetch " +
                "the \"cursor\" field on the edge to enable efficient pagination, this shortcut cannot be used, " +
                "and the full \"{ edges { node } } \" version should be used instead.";

        public ConnectionGraphType(RelationRegistry<TExecutionContext> registry)
        {
            // TODO Type name should be the same as an entity type name (e.g. for two loaders for the same entity and field set)
            var typeName = registry.GetProjectionTypeName<TChildLoader, TChildEntity>(false);

            Name = $"{typeName}Connection";
            Description = $"A connection from an object to a list of objects of type `{typeName}`.";

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

            Field<ListGraphType<EdgeGraphType<TChildLoader, TChildEntity, TExecutionContext>>>()
                .Name("edges")
                .Description("Information to aid in pagination.");

            Field(
                typeof(ListGraphType<>).MakeGenericType(registry.GetEntityGraphType<TChildLoader, TChildEntity>()),
                "items",
                ItemsDescription);
        }
    }

    internal class ConnectionGraphType<TReturnType, TExecutionContext> : ObjectGraphType<object>
    {
        private const string ItemsDescription = "A list of all of the objects returned in the connection. This is a convenience field provided " +
                "for quickly exploring the API; rather than querying for \"{ edges { node } }\" when no edge data " +
                "is needed, this field can be used instead. Note that when clients like Relay need to fetch " +
                "the \"cursor\" field on the edge to enable efficient pagination, this shortcut cannot be used, " +
                "and the full \"{ edges { node } } \" version should be used instead.";

        public ConnectionGraphType(IObjectGraphTypeConfigurator<TReturnType, TExecutionContext> configurator)
        {
            // TODO Type name should be the same as an entity type name (e.g. for two loaders for the same entity and field set)
            var typeName = configurator.Name;

            Name = $"{typeName}Connection";
            Description = $"A connection from an object to a list of objects of type `{typeName}`.";

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

            Field(
                typeof(ListGraphType<>).MakeGenericType(configurator.GenerateGraphType()),
                "items",
                ItemsDescription);

            Field(
                typeof(ListGraphType<>).MakeGenericType(typeof(EdgeGraphType<>).MakeGenericType(configurator.GenerateGraphType())),
                "edges",
                "Information to aid in pagination.");
        }
    }
}
