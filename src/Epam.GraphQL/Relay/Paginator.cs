// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using Epam.GraphQL.Infrastructure;

namespace Epam.GraphQL.Relay
{
    internal static class Paginator
    {
        public static Paginatior<TSource> From<TSource>(IQueryExecuter executer, Func<string> stepNameFactory, IQueryable<TSource> query, bool shouldMaterialize)
        {
            return new Paginatior<TSource>(executer, stepNameFactory, query, shouldMaterialize);
        }
    }

    internal class Paginatior<TSource> : IPaginatorWithoutSkip<TSource>
    {
        private readonly IQueryExecuter _executer;
        private readonly Func<string> _stepNameFactory;
        private readonly IQueryable<TSource> _query;
        private readonly bool _shouldMaterialize;
        private int _skipCount;
        private int _takeCount;
        private int _takeLastCount;
        private int _takeBeforeCount;
        private bool _shouldTake;
        private bool _shouldSkip;
        private bool _shouldTakeLast;
        private bool _shouldTakeBefore;

        public Paginatior(IQueryExecuter executer, Func<string> stepNameFactory, IQueryable<TSource> query, bool shouldMaterialize)
        {
            _query = query;
            _executer = executer;
            _stepNameFactory = stepNameFactory;
            _shouldMaterialize = shouldMaterialize;
        }

        public IPaginatorWithoutSkip<TSource> SkipIncluding(int? index)
        {
            if (index.HasValue)
            {
                if (_shouldTake)
                {
                    throw new InvalidOperationException("Cannot perform skip after take.");
                }

                _skipCount += index.Value + 1;
                _shouldSkip = true;
            }

            return this;
        }

        public IPaginatorWithoutSkip<TSource> Take(int? count)
        {
            if (count.HasValue)
            {
                if (_shouldTake)
                {
                    _takeCount = Math.Min(count.Value, _takeCount);
                }
                else
                {
                    _takeCount = count.Value;
                    _shouldTake = true;
                }
            }

            return this;
        }

        public IPaginatorWithoutSkip<TSource> TakeLast(int? count)
        {
            if (count.HasValue)
            {
                if (_shouldTakeLast)
                {
                    _takeLastCount = Math.Min(count.Value, _takeCount);
                }
                else
                {
                    _takeLastCount = count.Value;
                    _shouldTakeLast = true;
                }
            }

            return this;
        }

        public IPaginatorWithoutSkip<TSource> TakeBefore(int? index)
        {
            if (index.HasValue)
            {
                _shouldTakeBefore = true;
                _takeBeforeCount = index.Value;
                if (_shouldSkip)
                {
                    return Take(index.Value - _skipCount);
                }

                return Take(index.Value);
            }

            return this;
        }

#pragma warning disable CA1502
        public PaginatorResult<TSource> Materialize()
#pragma warning restore CA1502
        {
            // Dragons live here...
            var query = _query;

            if (!_shouldMaterialize && !_shouldSkip && !_shouldTake && !_shouldTakeBefore && !_shouldTakeLast)
            {
                return new PaginatorResult<TSource>
                {
                    StartOffset = 0,
                    Page = _executer.ToEnumerable(_stepNameFactory, query),
                };
            }

            var liveSkipCount = _skipCount - (_shouldSkip ? 1 : 0);

            if (_shouldSkip)
            {
                // Skip items excluding the last one.
                // It is intended to ensure that we did not skip the whole data sequence.
                query = query.Skip(liveSkipCount);
            }

            var liveTakeCount = _takeCount + 1 + (_shouldSkip ? 1 : 0);

            if (_shouldTake)
            {
                // Take items including the next item and maybe including previous one (if 'Skip' was performed).
                query = query.Take(liveTakeCount);
            }

            var sample = _executer.ToList(_stepNameFactory, query);
            var shouldRemoveLast = false;
            var skipCanceled = false;
            var hasNextPage = false;
            var hasPreviousPage = false;
            int? offset = 0;
            int? totalCount = null;

            if (_shouldTake)
            {
                if (sample.Count > 0)
                {
                    if (sample.Count == liveTakeCount)
                    {
                        if (!_shouldTakeBefore || _takeBeforeCount > 0)
                        {
                            hasNextPage = true;
                        }

                        shouldRemoveLast = true;
                    }
                    else
                    {
                        // If sample.Count < liveTakeCount then 'Take' operation gave us one item at least.
                        // It means that we reached the end of data sequence, so there is no the next page for current page.
                        totalCount = liveSkipCount + sample.Count;
                    }
                }
                else if (_shouldSkip)
                {
                    // Query is being executed the second time because 'Take' gave us no items. It is possible by two reasons:
                    // 1) we reached the end of data sequence by skiping items. This case will be handled below by taking items again.
                    // 2) the sequence is initially empty. This case should not be handled here.

                    // Again, we apply the same technique (take one more item from data sequence) to ensure that we have items after the current page.
                    sample = _executer.ToList(_stepNameFactory, _query.Take(_takeCount + 1));

                    skipCanceled = true;

                    if (sample.Count == _takeCount + 1)
                    {
                        hasNextPage = true;
                        shouldRemoveLast = true;
                    }
                }
            }
            else
            {
                // 'Take' operation was not performed, so we've got all the items from the sequence till the end.
                totalCount = liveSkipCount + sample.Count;
            }

            if (sample.Count > 0)
            {
                // Remove extra items if necessary...
                if (shouldRemoveLast)
                {
                    sample.RemoveAt(sample.Count - 1);
                }

                if (_shouldSkip && !skipCanceled)
                {
                    sample.RemoveAt(0);
                    offset = _skipCount;
                }

                if (!skipCanceled && !hasNextPage && sample.Count == 0)
                {
                    offset = null;
                }
            }
            else
            {
                skipCanceled = true;

                if (_shouldSkip && !_shouldTake)
                {
                    // If there is no items after skipping (and we did not perform taking), try to materialize query again.
                    sample = _executer.ToList(_stepNameFactory, _query);
                }

                if (sample.Count == 0)
                {
                    offset = null;
                }

                totalCount = sample.Count;
            }

            if (_shouldSkip && !skipCanceled)
            {
                hasPreviousPage = true;
            }

            if (_shouldTakeLast)
            {
                var removeCount = Math.Max(0, sample.Count - _takeLastCount);
                if (removeCount > 0)
                {
                    offset += removeCount;
                    sample.RemoveRange(0, removeCount);
                    hasPreviousPage = true;
                }

                if (sample.Count == 0 && !_shouldTakeBefore)
                {
                    hasPreviousPage = false;
                    offset = null;
                }
            }

            // Reset wrapper
            _skipCount = 0;
            _takeCount = 0;
            _takeLastCount = 0;
            _takeBeforeCount = 0;
            _shouldTake = false;
            _shouldSkip = false;
            _shouldTakeLast = false;
            _shouldTakeBefore = false;

            return new PaginatorResult<TSource>
            {
                StartOffset = offset,
                EndOffset = offset + Math.Max(sample.Count - 1, 0),
                HasPreviousPage = hasPreviousPage,
                HasNextPage = hasNextPage,
                Page = sample,
                TotalCount = totalCount,
            };
        }
    }
}
