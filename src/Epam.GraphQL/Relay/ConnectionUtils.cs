// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Infrastructure;
using GraphQL.Types.Relay.DataObjects;

namespace Epam.GraphQL.Relay
{
    internal static class ConnectionUtils
    {
        public static Connection<TSource> ToConnection<TSource>(
            IQueryable<TSource> query,
            IChainConfigurationContext configurationContext,
            Func<string> stepNameFactory,
            IQueryExecuter executer,
            int? first,
            int? last,
            int? before,
            int? after,
            bool shouldComputeCount,
            bool shouldComputeEndOffset,
            bool shouldComputeEdges,
            bool shouldComputeItems)
        {
            Guards.ThrowIfNegative(first, nameof(first));
            Guards.ThrowIfNegative(last, nameof(last));

            if (first.HasValue && last.HasValue)
            {
                throw new ArgumentException("Cannot use `first` in conjunction with `last`.");
            }

            if (after < 0)
            {
                after = null;
            }

            if (before < 0)
            {
                before = null;
            }

            Paginatior<TSource> wrapper = Paginator.From(executer, configurationContext, stepNameFactory, query, shouldMaterialize: shouldComputeEndOffset || shouldComputeCount);
            PaginatorResult<TSource> result;

            if (after >= before)
            {
                result = wrapper
                    .SkipIncluding(after)
                    .Take(first)
                    .TakeLast(last)
                    .Materialize();

                // No previous page means that the whole data was returned so "after" item was not found.
                if (!result.HasPreviousPage)
                {
                    // Before only
                    result = wrapper
                        .TakeBefore(before)
                        .Take(first)
                        .TakeLast(last)
                        .Materialize();
                }
            }
            else
            {
                result = wrapper
                    .SkipIncluding(after)
                    .TakeBefore(before)
                    .Take(first)
                    .TakeLast(last)
                    .Materialize();
            }

            int? totalCount = null;

            if (shouldComputeCount)
            {
                totalCount = result.TotalCount;
                if (totalCount == null)
                {
                    totalCount = executer.Execute(configurationContext, stepNameFactory, query, query => query.Count(), nameof(Queryable.Count));
                }
            }

            IEnumerable<Edge<TSource>>? edges = null;
            IEnumerable<TSource>? items = null;

            if (shouldComputeEdges && shouldComputeItems)
            {
                var materializedItems = result
                    .Page
                    .ToList();

                items = materializedItems;
                edges = materializedItems.Select((node, index) => new Edge<TSource> { Node = node, Cursor = (index + result.StartOffset).ToString() });
            }
            else
            {
                if (shouldComputeEdges)
                {
                    edges = result
                        .Page
                        .Select((node, index) => new Edge<TSource> { Node = node, Cursor = (index + result.StartOffset).ToString() });
                }

                if (shouldComputeItems)
                {
                    items = result.Page;
                }
            }

            return new Connection<TSource>
            {
                Edges = edges,
                Items = items,
                PageInfo = new PageInfo
                {
                    StartCursor = result.StartOffset?.ToString(CultureInfo.InvariantCulture),
                    EndCursor = result.EndOffset?.ToString(CultureInfo.InvariantCulture),
                    HasPreviousPage = result.HasPreviousPage,
                    HasNextPage = result.HasNextPage,
                },
                TotalCount = totalCount ?? -1, // TODO: Connection.TotalCount should be nullable
            };
        }
    }
}
