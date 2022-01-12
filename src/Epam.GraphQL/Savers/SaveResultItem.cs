// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace Epam.GraphQL.Savers
{
    internal class SaveResultItem<TEntity, TId> : ISaveResultItem
    {
        private readonly Func<TEntity, TId> _getId;

        public SaveResultItem(
            Func<TEntity, TId> getId,
            TEntity payload,
            bool isNew,
            IDictionary<string, object?> properties)
        {
            _getId = getId;
            Payload = payload;
            IsNew = isNew;
            Properties = properties;
            Id = _getId(Payload);
        }

        public TId Id { get; private set; }

        public TEntity Payload { get; set; }

        public bool IsNew { get; }

        public IDictionary<string, object?> Properties { get; }

        object? ISaveResultItem.Id => Id;

        object? ISaveResultItem.Payload => Payload;

        public SaveResultItem<TEntity, TId> Merge(SaveResultItem<TEntity, TId> item)
        {
            return new SaveResultItem<TEntity, TId>(
                getId: _getId,
                payload: item.Payload,
                isNew: IsNew,
                properties: Properties.Concat(item.Properties)
                    .GroupBy(p => p.Key)
                    .ToDictionary(g => g.Key, g => g.Last().Value));
        }

        public void UpdateId()
        {
            Id = _getId(Payload);
        }

        ISaveResultItem ISaveResultItem.Merge(ISaveResultItem item) => Merge((SaveResultItem<TEntity, TId>)item);
    }
}
