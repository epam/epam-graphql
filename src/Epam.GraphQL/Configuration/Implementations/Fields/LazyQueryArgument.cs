// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using GraphQL.Types;

#nullable enable

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal class LazyQueryArgument
    {
        public LazyQueryArgument(string name, Func<QueryArgument> factory)
        {
            Name = name;
            Factory = factory ?? throw new ArgumentNullException(nameof(factory));
        }

        public LazyQueryArgument(string name, Type graphType)
            : this(name, () => new QueryArgument(graphType)
            {
                Name = name,
            })
        {
        }

        public string Name { get; }

        public Func<QueryArgument> Factory { get; set; }

        public QueryArgument ToQueryArgument() => Factory();
    }
}
