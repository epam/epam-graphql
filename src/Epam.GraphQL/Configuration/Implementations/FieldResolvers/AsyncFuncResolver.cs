// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.TaskBatcher;
using GraphQL;
using GraphQL.DataLoader;

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal class AsyncFuncResolver<TEntity, TReturnType> : IResolver<TEntity>, IBatchResolver<TEntity, TReturnType>
        where TEntity : class
    {
        public AsyncFuncResolver(
            Func<IResolveFieldContext, IDataLoader<TEntity, TReturnType>> resolver,
            Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, TReturnType>> proxiedResolver)
        {
            Resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            ProxiedResolver = proxiedResolver ?? throw new ArgumentNullException(nameof(proxiedResolver));
        }

        protected Func<IResolveFieldContext, IDataLoader<TEntity, TReturnType>> Resolver { get; }

        protected Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, TReturnType>> ProxiedResolver { get; }

        public IDataLoader<TEntity, object> GetBatchLoader(IResolveFieldContext context)
        {
            return Resolver(context).Then(FuncConstants<TReturnType>.WeakIdentity);
        }

        public IDataLoader<Proxy<TEntity>, object> GetProxiedBatchLoader(IResolveFieldContext context)
        {
            return ProxiedResolver(context).Then(FuncConstants<TReturnType>.WeakIdentity);
        }

        public object Resolve(IResolveFieldContext context)
        {
            return context.Source is Proxy<TEntity> proxy
                ? ProxiedResolver(context).LoadAsync(proxy)
                : Resolver(context).LoadAsync((TEntity)context.Source);
        }

        public IBatchResolver<TEntity, TSelectType> Select<TSelectType>(Func<TEntity, TReturnType, TSelectType> selector)
        {
            return new AsyncFuncResolver<TEntity, TSelectType>(ctx => Resolver(ctx).Then(selector), ctx => ProxiedResolver(ctx).Then((e, r) => selector(e.GetOriginal(), r)));
        }

        public IBatchResolver<TEntity, TSelectType> Select<TSelectType>(Func<TReturnType, TSelectType> selector)
        {
            return new AsyncFuncResolver<TEntity, TSelectType>(ctx => Resolver(ctx).Then(selector), ctx => ProxiedResolver(ctx).Then(selector));
        }
    }
}
