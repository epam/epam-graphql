// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Linq;

#nullable enable

namespace Epam.GraphQL.Search
{
    public abstract class OrderedSearcher<TEntity, TExecutionContext> : Searcher<TEntity, TExecutionContext>, IOrderedSearcher<TEntity, TExecutionContext>
    {
        public abstract IOrderedQueryable<TEntity> ApplySearchOrderBy(IQueryable<TEntity> query, string search);

        public abstract IOrderedQueryable<TEntity> ApplySearchThenBy(IOrderedQueryable<TEntity> query, string search);
    }
}
