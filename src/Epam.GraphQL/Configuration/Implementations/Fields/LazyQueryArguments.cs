// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Types;

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal class LazyQueryArguments : IEnumerable<LazyQueryArgument>
    {
        private readonly List<LazyQueryArgument> _items;

        public LazyQueryArguments()
            : this(Array.Empty<LazyQueryArgument>())
        {
        }

        public LazyQueryArguments(params LazyQueryArgument[] arguments)
        {
            _items = arguments.ToList();
        }

        public int Count => _items.Count;

        public LazyQueryArgument this[int index]
        {
            get => _items[index];
            set => _items[index] = value;
        }

        public IEnumerator<LazyQueryArgument> GetEnumerator() => _items.GetEnumerator();

        public void Add(LazyQueryArgument item) => _items.Add(item);

        public QueryArguments ToQueryArguments() => new(_items.Select(item => item.ToQueryArgument()));

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
}
