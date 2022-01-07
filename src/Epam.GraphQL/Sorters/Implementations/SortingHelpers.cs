// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Search;
using GraphQL;

#nullable enable

namespace Epam.GraphQL.Sorters.Implementations
{
    internal static class SortingHelpers
    {
        public static IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> Empty => Enumerable.Empty<(LambdaExpression SortExpression, SortDirection SortDirection)>();

        public static IQueryable<TChildEntity> ApplySort<TChildEntity, TExecutionContext>(
            IResolveFieldContext context,
            IQueryable<TChildEntity> queryable,
            IReadOnlyList<ISorter<TExecutionContext>>? sorters,
            IOrderedSearcher<TChildEntity, TExecutionContext>? searcher,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> naturalSorters)
        {
            var sorting = context.GetSorting();
            var search = context.GetSearch();

            var userSorters = sorting
                .Select(o => (sorters.Single(s => string.Equals(s.Name, o.Field, StringComparison.Ordinal)).BuildExpression(context.GetUserContext<TExecutionContext>()), o.Direction))
                .ToArray();

            var lastSorters = naturalSorters.Where(sorter => userSorters.All(field => !ExpressionEqualityComparer.Instance.Equals(field.Item1, sorter.SortExpression)));

            if (userSorters.Length > 0)
            {
                var query = queryable.ApplyOrderBy(userSorters);

                if (!string.IsNullOrEmpty(search) && searcher != null)
                {
                    return searcher.ApplySearchThenBy(query, search).ApplyThenBy(naturalSorters);
                }

                return lastSorters.Any() ? query.ApplyThenBy(lastSorters) : query;
            }

            if (!string.IsNullOrEmpty(search) && searcher != null)
            {
                return searcher.ApplySearchOrderBy(queryable, search).ApplyThenBy(naturalSorters);
            }

            return lastSorters.Any() ? queryable.ApplyOrderBy(lastSorters) : queryable;
        }

        public static IQueryable<Proxy<TChildEntity>> ApplyGroupSort<TChildEntity, TExecutionContext>(
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
                throw new ExecutionError($"Cannot sort by the following fields: {string.Join(", ", sorting.Select(o => $"`{o.Field}`"))}. Consider making them groupable.");
            }

            var subFields = context.GetGroupConnectionQueriedFields()
                .Where(name => !sorting.Any(s => string.Equals(s.Field, name, StringComparison.Ordinal)))
                .Select(name => proxyAccessor.Fields.FirstOrDefault(field => string.Equals(field.Name, name, StringComparison.Ordinal)));

            var sort = sortFields.Select(f => (proxyAccessor.GetProxyExpression(f.Item1.OriginalExpression), f.Direction))
                .Concat(subFields.Select(f => (proxyAccessor.GetProxyExpression(f.OriginalExpression ?? throw new NotSupportedException()), SortDirection.Asc)));

            var query = sort.Any() ? queryable.ApplyOrderBy(sort) : queryable;

            return query;
        }
    }
}
