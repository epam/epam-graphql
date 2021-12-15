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

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal class FuncResolver<TEntity, TReturnType> : IResolver<TEntity>
        where TEntity : class
    {
        public FuncResolver(Func<IResolveFieldContext, TEntity, TReturnType> resolver)
        {
            Resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
        }

        protected Func<IResolveFieldContext, TEntity, TReturnType> Resolver { get; }

        public IDataLoader<TEntity, object> GetBatchLoader(IResolveFieldContext context)
        {
            return BatchLoader.FromResult<TEntity, object>(Resolver(context, (TEntity)context.Source));
        }

        public IDataLoader<Proxy<TEntity>, object> GetProxiedBatchLoader(IResolveFieldContext context)
        {
            return BatchLoader.FromResult<Proxy<TEntity>, object>(Resolver(context, ((Proxy<TEntity>)context.Source).GetOriginal()));
        }

        public object Resolve(IResolveFieldContext context)
        {
            var boundResolver = context.Bind(Resolver);
            return context.Source is Proxy<TEntity> proxy
                ? boundResolver(proxy.GetOriginal())
                : boundResolver((TEntity)context.Source);
        }
    }
}
