// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Epam.GraphQL.Helpers
{
    internal class EmptyGrouping<TKey, TElement> : IGrouping<TKey, TElement>
    {
        public EmptyGrouping(TKey key)
        {
            Key = key;
        }

        public TKey Key { get; private set; }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public IEnumerator<TElement> GetEnumerator()
        {
            return Enumerable.Empty<TElement>().GetEnumerator();
        }
    }
}
