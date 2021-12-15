// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Search;
using GraphQL;

namespace Epam.GraphQL.Sorters.Implementations
{
    internal static class SortingHelpers
    {
        public static IOrderedQueryable<TChildEntity> ApplySort<TEntity, TChildEntity, TExecutionContext>(
            IResolveFieldContext context,
            IQueryable<TChildEntity> queryable,
            IObjectGraphTypeConfigurator<TChildEntity, TExecutionContext> objectGraphTypeConfigurator,
            IOrderedSearcher<TChildEntity, TExecutionContext> searcher,
            Func<IQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>> applyNaturalOrderBy,
            Func<IOrderedQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>> applyNaturalThenBy)
        {
            var sorting = context.GetSorting();
            var search = context.GetSearch();

            var sorters = objectGraphTypeConfigurator.Sorters;

            var fields = sorting
                .Select(o => (sorters.Single(s => string.Equals(s.Name, o.Field, StringComparison.Ordinal)), o.Direction));

            var query = queryable.ApplyOrderBy(fields.Select(f => (f.Item1.BuildExpression(context.GetUserContext<TExecutionContext>()), f.Direction)));

            if (query == null)
            {
                if (!string.IsNullOrEmpty(search) && searcher != null)
                {
                    query = searcher.ApplySearchOrderBy(queryable, search);
                    return applyNaturalThenBy(query);
                }

                return applyNaturalOrderBy(queryable);
            }

            if (!string.IsNullOrEmpty(search) && searcher != null)
            {
                query = searcher.ApplySearchThenBy(query, search);
            }

            return applyNaturalThenBy(query);
        }
    }
}
