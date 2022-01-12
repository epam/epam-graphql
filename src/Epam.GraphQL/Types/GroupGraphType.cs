// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using GraphQL.Types;

namespace Epam.GraphQL.Types
{
    internal class GroupGraphType<TProjection, TEntity, TExecutionContext> : ObjectGraphType<Proxy<TEntity>>, IHasGetEntityType
        where TProjection : ProjectionBase<TEntity, TExecutionContext>, new()
        where TEntity : class
    {
        public GroupGraphType(RelationRegistry<TExecutionContext> registry)
        {
            registry.ConfigureGroupGraphType<TProjection, TEntity>(this);
        }

        public Type GetEntityType()
        {
            return typeof(TEntity);
        }

        public Type GetProjectionType()
        {
            return typeof(TProjection);
        }
    }
}
