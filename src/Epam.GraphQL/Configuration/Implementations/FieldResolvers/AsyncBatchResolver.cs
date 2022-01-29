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
    internal class AsyncBatchResolver<TEntity, TReturnType> : AsyncFuncResolver<TEntity, TReturnType>, IBatchResolver<TEntity, TReturnType>
       where TEntity : class
    {
        public AsyncBatchResolver(
            Func<IResolveFieldContext, IDataLoader<TEntity, TReturnType>> resolver,
            Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, TReturnType>> proxiedResolver)
            : base(resolver, proxiedResolver)
        {
        }

        public IDataLoader<TEntity, object?> GetBatchLoader(IResolveFieldContext context)
        {
            return Resolver(context).Then(FuncConstants<TReturnType?>.WeakIdentity);
        }

        public IDataLoader<Proxy<TEntity>, object?> GetProxiedBatchLoader(IResolveFieldContext context)
        {
            return ProxiedResolver(context).Then(FuncConstants<TReturnType?>.WeakIdentity);
        }

        public IBatchResolver<TEntity, TSelectType> Select<TSelectType>(Func<TEntity, TReturnType, TSelectType> selector)
        {
            return new AsyncBatchResolver<TEntity, TSelectType>(
                ctx => Resolver(ctx).Then(selector),
                ctx => ProxiedResolver(ctx).Then(ProxiedSelector));

            TSelectType ProxiedSelector(Proxy<TEntity> proxy, TReturnType value)
            {
                return selector(proxy.GetOriginal(), value);
            }
        }

        public IBatchResolver<TEntity, TSelectType> Select<TSelectType>(Func<TReturnType, TSelectType> selector)
        {
            return new AsyncBatchResolver<TEntity, TSelectType>(
                ctx => Resolver(ctx).Then(selector),
                ctx => ProxiedResolver(ctx).Then(selector));
        }
    }
}
