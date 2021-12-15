// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;

namespace Epam.GraphQL.Savers
{
    internal class SaveResultItem<TEntity, TId> : ISaveResultItem
    {
        public TId Id { get; set; }

        public TEntity Payload { get; set; }

        public bool IsNew { get; set; }

        public IDictionary<string, object> Properties { get; set; }

        object ISaveResultItem.Id
        {
            get => Id;
            set => Id = (TId)value;
        }

        object ISaveResultItem.Payload => Payload;

        public SaveResultItem<TEntity, TId> Merge(SaveResultItem<TEntity, TId> item)
        {
            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            return new SaveResultItem<TEntity, TId>()
            {
                Id = Id,
                Payload = item.Payload,
                IsNew = IsNew,
                Properties = Properties.Concat(item.Properties)
                    .GroupBy(p => p.Key)
                    .ToDictionary(g => g.Key, g => g.Last().Value),
            };
        }

        ISaveResultItem ISaveResultItem.Merge(ISaveResultItem item) => Merge((SaveResultItem<TEntity, TId>)item);
    }
}
