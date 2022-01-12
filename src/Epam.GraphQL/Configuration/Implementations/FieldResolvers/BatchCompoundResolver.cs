// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.TaskBatcher;
using GraphQL;
using GraphQL.DataLoader;

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal class BatchCompoundResolver<TEntity, TExecutionContext> : IBatchCompoundResolver<TEntity, TExecutionContext>
        where TEntity : class
    {
        private readonly List<IResolver<TEntity>> _resolvers = new();

        public void Add(IResolver<TEntity> resolver)
        {
            _resolvers.Add(resolver);
        }

        public IDataLoader<TEntity, object?> GetBatchLoader(IResolveFieldContext context) => GetBatchLoader<object?>(context);

        public IDataLoader<Proxy<TEntity>, object?> GetProxiedBatchLoader(IResolveFieldContext context) => GetProxiedBatchLoader<object?>(context);

        public IBatchResolver<TEntity, TSelectType> Select<TSelectType>(Func<TEntity, IEnumerable<object>, TSelectType> selector)
        {
            return new AsyncBatchResolver<TEntity, TSelectType>(
                context => GetBatchLoader<IEnumerable<object>>(context).Then(selector),
                context => GetProxiedBatchLoader<IEnumerable<object>>(context).Then((e, r) => selector(e.GetOriginal(), r)));
        }

        public IBatchResolver<TEntity, TSelectType> Select<TSelectType>(Func<IEnumerable<object>, TSelectType> selector)
        {
            return new AsyncBatchResolver<TEntity, TSelectType>(
                context => GetBatchLoader<IEnumerable<object>>(context).Then(selector),
                context => GetProxiedBatchLoader<IEnumerable<object>>(context).Then(selector));
        }

        public object Resolve(IResolveFieldContext context)
        {
            if (context.Source is Proxy<TEntity> proxy)
            {
                var proxiedBatchLoader = context.Bind(GetProxiedBatchLoader);
                return proxiedBatchLoader.LoadAsync(proxy);
            }

            var batchLoader = context.Bind(GetBatchLoader);
            return batchLoader.LoadAsync((TEntity)context.Source);
        }

        private IDataLoader<TEntity, TReturnType> GetBatchLoader<TReturnType>(IResolveFieldContext context)
        {
            return BatchLoader.WhenAll(_resolvers.Select(resolver => resolver.GetBatchLoader(context)))
                .Then(items => (TReturnType)items.Where(item => item != null));
        }

        private IDataLoader<Proxy<TEntity>, TReturnType> GetProxiedBatchLoader<TReturnType>(IResolveFieldContext context)
        {
            return BatchLoader.WhenAll(_resolvers.Select(resolver => resolver.GetProxiedBatchLoader(context)))
                .Then(items => (TReturnType)items.Where(item => item != null));
        }
    }
}
