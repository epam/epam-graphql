// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.TaskBatcher;
using GraphQL;
using GraphQL.DataLoader;

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal class BatchCompoundResolver<TEntity, TExecutionContext> : IBatchCompoundResolver<TEntity, TExecutionContext>
    {
        private readonly List<IBatchResolver<TEntity>> _resolvers = new();

        public BatchCompoundResolver(params IBatchResolver<TEntity>[] resolvers)
            : this(resolvers.AsEnumerable())
        {
        }

        private BatchCompoundResolver(IEnumerable<IBatchResolver<TEntity>> resolvers)
        {
            _resolvers.AddRange(resolvers);
        }

        public IBatchCompoundResolver<TEntity, TExecutionContext> Add(IBatchResolver<TEntity> resolver)
        {
            return new BatchCompoundResolver<TEntity, TExecutionContext>(
                _resolvers.Concat(Enumerable.Repeat(resolver, 1)));
        }

        public IDataLoader<TEntity, object?> GetBatchLoader(IResolveFieldContext context) => GetBatchLoader<object?>(context);

        public IDataLoader<Proxy<TEntity>, object?> GetProxiedBatchLoader(IResolveFieldContext context) => GetProxiedBatchLoader<object?>(context);

        public IBatchResolver<TEntity, TSelectType> Select<TSelectType>(Func<IEnumerable<object>, TSelectType> selector)
        {
            return new BatchResolverBase<TEntity, TSelectType>(
                context => GetBatchLoader<IEnumerable<object>>(context).Then(selector),
                context => GetProxiedBatchLoader<IEnumerable<object>>(context).Then(selector));
        }

        public ValueTask<object?> ResolveAsync(IResolveFieldContext context)
        {
            Guards.AssertIfNull(context.Source);

            if (context.Source is Proxy<TEntity> proxy)
            {
                var proxiedBatchLoader = context.Bind(GetProxiedBatchLoader);
                return new ValueTask<object?>(proxiedBatchLoader.LoadAsync(proxy));
            }

            var batchLoader = context.Bind(GetBatchLoader);
            return new ValueTask<object?>(batchLoader.LoadAsync((TEntity)context.Source));
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
