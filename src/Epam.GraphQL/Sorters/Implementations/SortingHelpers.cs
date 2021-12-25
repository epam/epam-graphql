// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Search;
using GraphQL;

namespace Epam.GraphQL.Sorters.Implementations
{
    internal static class SortingHelpers
    {
        public static IOrderedQueryable<TChildEntity> ApplySort<TChildEntity, TExecutionContext>(
            IResolveFieldContext context,
            IQueryable<TChildEntity> queryable,
            IReadOnlyList<ISorter<TExecutionContext>> sorters,
            IOrderedSearcher<TChildEntity, TExecutionContext> searcher,
            Func<IQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>> applyNaturalOrderBy,
            Func<IOrderedQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>> applyNaturalThenBy)
        {
            var sorting = context.GetSorting();
            var search = context.GetSearch();

            var fields = sorting
                .Select(o => (sorters.Single(s => string.Equals(s.Name, o.Field, StringComparison.Ordinal)), o.Direction));

            var query = queryable.ApplyOrderBy(fields.Select(f => (f.Item1.BuildExpression(context.GetUserContext<TExecutionContext>()), f.Direction)));

            if (query == null)
            {
                if (!string.IsNullOrEmpty(search) && searcher != null)
                {
                    query = searcher.ApplySearchOrderBy(queryable, search);

                    if (applyNaturalThenBy != null)
                    {
                        return applyNaturalThenBy(query);
                    }

                    return query;
                }

                if (applyNaturalOrderBy != null)
                {
                    return applyNaturalOrderBy(queryable);
                }

                return (IOrderedQueryable<TChildEntity>)queryable;
            }

            if (!string.IsNullOrEmpty(search) && searcher != null)
            {
                query = searcher.ApplySearchThenBy(query, search);
            }

            if (applyNaturalThenBy != null)
            {
                return applyNaturalThenBy(query);
            }

            return query;
        }

        public static IOrderedQueryable<Proxy<TChildEntity>> ApplyGroupSort<TChildEntity, TExecutionContext>(
            IResolveFieldContext context,
            IQueryable<Proxy<TChildEntity>> queryable,
            IReadOnlyList<ISorter<TExecutionContext>> sorters,
            IProxyAccessor<TChildEntity, TExecutionContext> proxyAccessor)
        {
            var sourceType = proxyAccessor.ProxyType;

            var sorting = context.GetSorting();
            var search = context.GetSearch();

            var sortFields = sorting
                .Select(o => (sorters.Single(s => string.Equals(s.Name, o.Field, StringComparison.Ordinal)), o.Direction));

            if (sortFields.Any(f => !(f.Item1?.IsGroupable ?? false)))
            {
                throw new ExecutionError($"Cannot find field(s) for sorting: {string.Join(", ", sorting.Select(o => o.Field))}");
            }

            var subFields = context.GetGroupConnectionQueriedFields()
                .Where(name => !sorting.Any(s => string.Equals(s.Field, name, StringComparison.Ordinal)))
                .Select(name => proxyAccessor.Fields.FirstOrDefault(field => string.Equals(field.Name, name, StringComparison.Ordinal)));

            var sort = sortFields.Select(f => (proxyAccessor.GetProxyExpression(f.Item1.OriginalExpression), f.Direction))
                .Concat(subFields.Select(f => (proxyAccessor.GetProxyExpression(f.OriginalExpression), SortDirection.Asc)));

            var query = queryable.ApplyOrderBy(sort) ?? queryable;

            return (IOrderedQueryable<Proxy<TChildEntity>>)query.Cast<Proxy<TChildEntity>>();
        }
    }
}
