// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Epam.GraphQL.Configuration;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Savers;
using GraphQL.Types;

namespace Epam.GraphQL.Types
{
    internal class SubmitOutputItemGraphType<TProjection, TEntity, TId, TExecutionContext> : ObjectGraphType<SaveResultItem<TEntity, TId>>
        where TProjection : Projection<TEntity, TExecutionContext>, new()
    {
        public SubmitOutputItemGraphType(RelationRegistry<TExecutionContext> registry)
        {
            // TODO Type name should be taken from registry?
            Name = $"{typeof(TEntity).Name}SubmitOutput";
            Field(registry.GenerateGraphType(typeof(TId)), "id", resolve: ctx => ctx.Source.Id);
            Field(registry.GetEntityGraphType<TProjection, TEntity>(), "payload", resolve: ctx => ctx.Source.Payload);
        }
    }
}
