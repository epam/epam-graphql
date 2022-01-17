// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

namespace Epam.GraphQL.Filters.Inputs
{
    internal class ComparisonsFilter<TItemType, TListItemType> : InFilter<TItemType, TListItemType>
    {
        public TItemType Gt { get; set; } = default!;

        public TItemType Lt { get; set; } = default!;

        public TItemType Gte { get; set; } = default!;

        public TItemType Lte { get; set; } = default!;
    }
}
