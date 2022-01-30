// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using GraphQL;
using GraphQL.DataLoader;

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal class ProxiedAsyncFuncResolver<TEntity, TReturnType> : IResolver<TEntity>
        where TEntity : class
    {
        public ProxiedAsyncFuncResolver(
            Func<IResolveFieldContext, IDataLoader<TEntity, Proxy<TReturnType>>> resolver,
            Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, Proxy<TReturnType>>> proxiedResolver)
        {
            Resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            ProxiedResolver = proxiedResolver ?? throw new ArgumentNullException(nameof(proxiedResolver));
        }

        protected Func<IResolveFieldContext, IDataLoader<TEntity, Proxy<TReturnType>>> Resolver { get; }

        protected Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, Proxy<TReturnType>>> ProxiedResolver { get; }

        public object Resolve(IResolveFieldContext context)
        {
            var batchLoader = new Lazy<IDataLoader<TEntity, Proxy<TReturnType>>>(() => context.Bind(Resolver));
            var proxiedBatchLoader = new Lazy<IDataLoader<Proxy<TEntity>, Proxy<TReturnType>>>(() => context.Bind(ProxiedResolver));

            return context.Source is Proxy<TEntity> proxy
                ? proxiedBatchLoader.Value.LoadAsync(proxy)
                : batchLoader.Value.LoadAsync((TEntity)context.Source);
        }
    }
}
