// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Epam.GraphQL.Configuration;
using Epam.GraphQL.Loaders;
using GraphQL.Types;

namespace Epam.GraphQL.Types
{
    internal class InputEntityGraphType<TProjection, TEntity, TExecutionContext> : InputObjectGraphType<TEntity>
        where TProjection : ProjectionBase<TEntity, TExecutionContext>, new()
    {
        public InputEntityGraphType(RelationRegistry<TExecutionContext> registry)
        {
            registry.ConfigureInputGraphType<TProjection, TEntity>(this);
        }
    }
}
