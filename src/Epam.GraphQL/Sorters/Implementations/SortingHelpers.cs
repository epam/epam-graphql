// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Search;
using GraphQL;

namespace Epam.GraphQL.Sorters.Implementations
{
    internal static class SortingHelpers
    {
        public static IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> Empty => Enumerable.Empty<(LambdaExpression SortExpression, SortDirection SortDirection)>();

        public static IReadOnlyList<(LambdaExpression SortExpression, SortDirection SortDirection)> GetSort<TChildEntity, TExecutionContext>(
            IResolveFieldContext context,
            IReadOnlyList<ISorter<TExecutionContext>>? sorters,
            IOrderedSearcher<TChildEntity, TExecutionContext>? searcher,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> naturalSorters)
        {
            var sorting = context.GetSorting();
            var search = context.GetSearch();
            var result = new List<(LambdaExpression SortExpression, SortDirection SortDirection)>();

            result.AddRange(sorting
                .Select(o => (sorters.Single(s => string.Equals(s.Name, o.Field, StringComparison.Ordinal)).BuildExpression(context.GetUserContext<TExecutionContext>()), o.Direction)));

            if (!string.IsNullOrEmpty(search) && searcher != null)
            {
                result.AddRange(searcher.ApplySearchOrderBy(Enumerable.Empty<TChildEntity>().AsQueryable(), search).GetSorters());
            }

            var lastSorters = naturalSorters.Where(sorter => result.All(field => !ExpressionEqualityComparer.Instance.Equals(field.SortExpression, sorter.SortExpression)));
            result.AddRange(lastSorters);

            return result;
        }
    }
}
