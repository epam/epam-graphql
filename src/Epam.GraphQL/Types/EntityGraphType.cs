// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using GraphQL.Types;

#nullable enable

namespace Epam.GraphQL.Types
{
    internal class EntityGraphType<TProjection, TEntity, TExecutionContext> : ObjectGraphType<object>, IHasGetEntityType
        where TProjection : ProjectionBase<TEntity, TExecutionContext>, new()
        where TEntity : class
    {
        public EntityGraphType(RelationRegistry<TExecutionContext> registry)
        {
            IsTypeOf = obj => obj is TEntity or Proxy<TEntity>;
            registry.ConfigureGraphType<TProjection, TEntity>(this);
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
