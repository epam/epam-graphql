// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Collections;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace Epam.GraphQL.Helpers
{
    internal static class Grouping
    {
        public static IGrouping<TKey, TElement> Create<TKey, TElement>(TKey key, IEnumerable<TElement> elements)
            => new Grouping<TKey, TElement>(key, elements);
    }

    internal class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        private readonly IEnumerable<TElement> _elements;

        public Grouping(TKey key, IEnumerable<TElement> elements)
        {
            Key = key;
            _elements = elements;
        }

        public TKey Key { get; }

        public IEnumerator<TElement> GetEnumerator()
        {
            return _elements.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
