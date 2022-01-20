// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Collections.Generic;
using GraphQL.Types.Relay.DataObjects;

namespace Epam.GraphQL.Relay
{
    internal class Connection<TItemsType, TEdgesType>
    {
        public int TotalCount { get; set; }

        public PageInfo? PageInfo { get; set; }

        public TEdgesType? Edges { get; set; }

        public TItemsType? Items { get; set; }
    }

    internal class Connection<T> : Connection<IEnumerable<T>, IEnumerable<Edge<T>>>
    {
    }
}
