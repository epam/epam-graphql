// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Threading.Tasks;
using Epam.GraphQL.Helpers;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal class AsyncFuncResolver<TEntity, TReturnType> : IFieldResolver
    {
        public AsyncFuncResolver(
            Func<IResolveFieldContext, IDataLoader<TEntity, TReturnType>> resolver,
            Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, TReturnType>> proxiedResolver)
        {
            Resolver = resolver;
            ProxiedResolver = proxiedResolver;
        }

        protected Func<IResolveFieldContext, IDataLoader<TEntity, TReturnType>> Resolver { get; }

        protected Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, TReturnType>> ProxiedResolver { get; }

        public ValueTask<object?> ResolveAsync(IResolveFieldContext context)
        {
            Guards.AssertIfNull(context.Source);

            return context.Source is Proxy<TEntity> proxy
                ? new ValueTask<object?>(ProxiedResolver(context).LoadAsync(proxy))
                : new ValueTask<object?>(Resolver(context).LoadAsync((TEntity)context.Source));
        }
    }
}
