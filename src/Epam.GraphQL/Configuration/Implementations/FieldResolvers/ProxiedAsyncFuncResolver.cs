// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.TaskBatcher;
using GraphQL;
using GraphQL.DataLoader;

#nullable enable

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal class ProxiedAsyncFuncResolver<TEntity, TReturnType> : IResolver<TEntity>
        where TEntity : class
    {
        private readonly Func<IResolveFieldContext, IDataLoader<TEntity, Proxy<TReturnType>>> _resolver;
        private readonly Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, Proxy<TReturnType>>> _proxiedResolver;

        public ProxiedAsyncFuncResolver(
            Func<IResolveFieldContext, IDataLoader<TEntity, Proxy<TReturnType>>> resolver,
            Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, Proxy<TReturnType>>> proxiedResolver)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _proxiedResolver = proxiedResolver ?? throw new ArgumentNullException(nameof(proxiedResolver));
        }

        public IDataLoader<TEntity, object?> GetBatchLoader(IResolveFieldContext context)
        {
            return _resolver(context).Then(FuncConstants<Proxy<TReturnType>?>.WeakIdentity);
        }

        public IDataLoader<Proxy<TEntity>, object?> GetProxiedBatchLoader(IResolveFieldContext context)
        {
            return _proxiedResolver(context).Then(FuncConstants<Proxy<TReturnType>?>.WeakIdentity);
        }

        public object Resolve(IResolveFieldContext context)
        {
            var batchLoader = new Lazy<IDataLoader<TEntity, Proxy<TReturnType>>>(() => context.Bind(_resolver));
            var proxiedBatchLoader = new Lazy<IDataLoader<Proxy<TEntity>, Proxy<TReturnType>>>(() => context.Bind(_proxiedResolver));

            return context.Source is Proxy<TEntity> proxy
                ? proxiedBatchLoader.Value.LoadAsync(proxy)
                : batchLoader.Value.LoadAsync((TEntity)context.Source);
        }
    }
}
