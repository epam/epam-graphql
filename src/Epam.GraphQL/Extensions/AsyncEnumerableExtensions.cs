// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

#nullable enable

namespace Epam.GraphQL.Extensions
{
    internal static class AsyncEnumerableExtensions
    {
        public static ValueTask<Dictionary<TKey, TElement>> ToDictionaryAsync<TSource, TKey, TElement>(this IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey>? comparer, CancellationToken cancellationToken = default)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (keySelector == null)
            {
                throw new ArgumentNullException(nameof(keySelector));
            }

            if (elementSelector == null)
            {
                throw new ArgumentNullException(nameof(elementSelector));
            }

            return Core(source, keySelector, elementSelector, comparer, cancellationToken);

            static async ValueTask<Dictionary<TKey, TElement>> Core(IAsyncEnumerable<TSource> source, Func<TSource, TKey> keySelector, Func<TSource, TElement> elementSelector, IEqualityComparer<TKey>? comparer, CancellationToken cancellationToken)
            {
                var d = new Dictionary<TKey, TElement>(comparer);

                await foreach (var item in source.WithCancellation(cancellationToken).ConfigureAwait(false))
                {
                    var key = keySelector(item);
                    var value = elementSelector(item);

                    d.Add(key, value);
                }

                return d;
            }
        }
    }
}
