// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Helpers;
using GraphQL;
using GraphQL.DataLoader;

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal interface IBatchResolver<TEntity> : IResolver<TEntity>
        where TEntity : class
    {
        IDataLoader<TEntity, object?> GetBatchLoader(IResolveFieldContext context);

        IDataLoader<Proxy<TEntity>, object?> GetProxiedBatchLoader(IResolveFieldContext context);
    }

    internal interface IBatchResolver<TEntity, TReturnType> : IBatchResolver<TEntity>
        where TEntity : class
    {
        IBatchResolver<TEntity, TSelectType> Select<TSelectType>(Func<TEntity, TReturnType, TSelectType> selector);

        IBatchResolver<TEntity, TSelectType> Select<TSelectType>(Func<TReturnType, TSelectType> selector);
    }
}
