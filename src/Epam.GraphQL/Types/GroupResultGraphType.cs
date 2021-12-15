// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Epam.GraphQL.Configuration;
using Epam.GraphQL.Loaders;
using GraphQL.Types;

namespace Epam.GraphQL.Types
{
    internal class GroupResultGraphType<TChildLoader, TChildEntity, TExecutionContext> : ObjectGraphType<object>
        where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
        where TChildEntity : class
    {
        public GroupResultGraphType(RelationRegistry<TExecutionContext> registry)
        {
            var typeName = registry.GetProjectionTypeName<TChildLoader, TChildEntity>(false);

            Name = $"{typeName}GroupResult";

            Field<NonNullGraphType<GroupGraphType<TChildLoader, TChildEntity, TExecutionContext>>>()
                .Name("item");

            Field<NonNullGraphType<IntGraphType>>()
                .Name("count");
        }
    }
}
