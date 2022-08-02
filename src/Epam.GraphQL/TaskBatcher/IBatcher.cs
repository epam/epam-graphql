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
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Loaders;
using GraphQL;
using GraphQL.DataLoader;

namespace Epam.GraphQL.TaskBatcher
{
    internal interface IBatcher
    {
        IDataLoader<TId, TItem?> Get<TId, TItem, TExecutionContext>(
            IResolvedChainConfigurationContext configurationContext,
            Func<string> stepNameFactory,
            TExecutionContext context,
            Func<TExecutionContext, IEnumerable<TId>, IEnumerable<KeyValuePair<TId, TItem>>> loader);

        IDataLoader<TId, TItem?> Get<TId, TItem, TExecutionContext>(
            IResolvedChainConfigurationContext configurationContext,
            Func<string> stepNameFactory,
            TExecutionContext context,
            Func<TExecutionContext, IEnumerable<TId>, Task<IDictionary<TId, TItem>>> loader);

        IDataLoader<TOuterEntity, IGrouping<TOuterEntity, TTransformedInnerEntity>> Get<TOuterEntity, TInnerEntity, TTransformedInnerEntity>(
            IResolveFieldContext context,
            Func<IResolveFieldContext, IQueryable<TInnerEntity>> queryFactory,
            Func<IResolveFieldContext, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>>? sort,
            Func<IResolveFieldContext, Expression<Func<TInnerEntity, TTransformedInnerEntity>>> transform,
            LambdaExpression outerExpression,
            LambdaExpression innerExpression,
            ILoaderHooksExecuter<TTransformedInnerEntity>? hooksExecuter);

        void Reset();
    }
}
