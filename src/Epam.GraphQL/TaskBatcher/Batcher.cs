// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using GraphQL;
using GraphQL.DataLoader;

namespace Epam.GraphQL.TaskBatcher
{
    internal class Batcher : IBatcher
    {
        private readonly Dictionary<(Delegate Loader, object Context), object> _batchTaskCache = new();

        private readonly Dictionary<(IResolveFieldContext, Type, Type, Type), object> _queryFactoriesCache = new(
            new ValueTupleEqualityComparer<IResolveFieldContext, Type, Type, Type>(firstItemComparer: ResolveFieldContextEqualityComparer.Instance));

        public Batcher(IProfiler profiler)
        {
            Profiler = profiler ?? throw new ArgumentNullException(nameof(profiler));
        }

        internal IProfiler Profiler { get; }

        public IDataLoader<TId, TItem?> Get<TId, TItem, TExecutionContext>(
            Func<string> stepNameFactory,
            TExecutionContext context,
            Func<TExecutionContext, IEnumerable<TId>, IEnumerable<KeyValuePair<TId, TItem>>> loader)
        {
            return (IDataLoader<TId, TItem?>)_batchTaskCache!.GetOrAdd((loader, context), Factory!);

            IDataLoader<TId, TItem?> Factory((Delegate Loader, object Context) key)
            {
                var loader = (Func<TExecutionContext, IEnumerable<TId>, IEnumerable<KeyValuePair<TId, TItem>>>)key.Loader;
                var context = (TExecutionContext)key.Context;
                var curriedLoader = loader.Curry()(context);
                return new BatchLoader<TId, TItem>(curriedLoader, stepNameFactory, Profiler);
            }
        }

        public IDataLoader<TId, TItem?> Get<TId, TItem, TExecutionContext>(
            Func<string> stepNameFactory,
            TExecutionContext context,
            Func<TExecutionContext, IEnumerable<TId>, Task<IDictionary<TId, TItem>>> loader)
        {
            return (IDataLoader<TId, TItem?>)_batchTaskCache!.GetOrAdd((loader, context), Factory!);

            IDataLoader<TId, TItem?> Factory((Delegate Loader, object Context) key)
            {
                var loader = (Func<TExecutionContext, IEnumerable<TId>, Task<IDictionary<TId, TItem>>>)key.Loader;
                var context = (TExecutionContext)key.Context;
                var curriedLoader = loader.Curry()(context);
                return new TaskBatchLoader<TId, TItem>(curriedLoader, stepNameFactory, Profiler);
            }
        }

        public IDataLoader<TOuterEntity, IGrouping<TOuterEntity, TTransformedInnerEntity>> Get<TOuterEntity, TInnerEntity, TTransformedInnerEntity>(
            IResolveFieldContext context,
            Func<IResolveFieldContext, IQueryable<TInnerEntity>> queryFactory,
            Func<IResolveFieldContext, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>>? sorters,
            Func<IResolveFieldContext, Expression<Func<TInnerEntity, TTransformedInnerEntity>>> transform,
            LambdaExpression outerExpression,
            LambdaExpression innerExpression,
            ILoaderHooksExecuter<TTransformedInnerEntity>? hooksExecuter)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return (IDataLoader<TOuterEntity, IGrouping<TOuterEntity, TTransformedInnerEntity>>)_queryFactoriesCache.GetOrAdd((context, typeof(TOuterEntity), typeof(TInnerEntity), typeof(TTransformedInnerEntity)), Factory);

            IDataLoader<TOuterEntity, IGrouping<TOuterEntity, TTransformedInnerEntity>> Factory((IResolveFieldContext Context, Type, Type, Type) key)
            {
                var context = key.Context;
                var query = queryFactory(context);
                var queryExecuter = context.GetQueryExecuter();
                var factory = BatchHelpers.GetQueryJoinFactory<TOuterEntity, TInnerEntity, TTransformedInnerEntity, IResolveFieldContext>(context.GetPath, transform, outerExpression, innerExpression);
                return factory(context, Profiler, queryExecuter, hooksExecuter, query, sorters?.Invoke(context));
            }
        }

        public void Reset()
        {
            _batchTaskCache.Clear();
            _queryFactoriesCache.Clear();
        }
    }
}
