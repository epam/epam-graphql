// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Savers
{
    internal class SaveResult<TEntity, TId, TExecutionContext> : ISaveResult<TExecutionContext>
    {
        public SaveResult(
            List<SaveResultItem<TEntity?, TId>> processedItems,
            List<SaveResultItem<TEntity, TId>> pendingItems,
            List<SaveResultItem<TEntity, TId>> postponedItems,
            IMutableLoader<TExecutionContext> loader,
            string fieldName)
        {
            ProcessedItems = processedItems;
            PendingItems = pendingItems;
            PostponedItems = postponedItems;
            Loader = loader;
            FieldName = fieldName;
        }

        public List<SaveResultItem<TEntity?, TId>> ProcessedItems { get; set; }

        public List<SaveResultItem<TEntity, TId>> PendingItems { get; set; }

        public List<SaveResultItem<TEntity, TId>> PostponedItems { get; set; }

        public IMutableLoader<TExecutionContext> Loader { get; set; }

        public string FieldName { get; set; }

        public Type EntityType => typeof(TEntity);

        IList<ISaveResultItem> ISaveResult<TExecutionContext>.ProcessedItems => ProcessedItems.Cast<ISaveResultItem>().ToList();

        IList<ISaveResultItem> ISaveResult<TExecutionContext>.PendingItems => PendingItems.Cast<ISaveResultItem>().ToList();

        IList<ISaveResultItem> ISaveResult<TExecutionContext>.PostponedItems => PostponedItems.Cast<ISaveResultItem>().ToList();

        ISaveResult<TExecutionContext> ISaveResult<TExecutionContext>.CloneAndMovePostponedToPending() => CloneAndMovePostponedToPending();

        ISaveResult<TExecutionContext> ISaveResult<TExecutionContext>.Merge(ISaveResult<TExecutionContext> obj) => Merge((SaveResult<TEntity, TId, TExecutionContext>)obj);

        public SaveResult<TEntity, TId, TExecutionContext> CloneAndMovePostponedToPending()
        {
            return new SaveResult<TEntity, TId, TExecutionContext>(
                processedItems: ProcessedItems,
                pendingItems: PostponedItems,
                postponedItems: new List<SaveResultItem<TEntity, TId>>(),
                loader: Loader,
                fieldName: FieldName);
        }

        public SaveResult<TEntity, TId, TExecutionContext> Merge(SaveResult<TEntity, TId, TExecutionContext> obj)
        {
            var pendingItems = PendingItems.Concat(obj.PendingItems)
                .GroupBy(item => item.Id)
                .Select(group => group.Aggregate((first, second) => first.Merge(second)))
                .ToList();

            return new SaveResult<TEntity, TId, TExecutionContext>(
                processedItems: ProcessedItems.Concat(obj.ProcessedItems).ToList(),
                pendingItems: pendingItems,
                postponedItems: obj.PostponedItems.Concat(obj.PostponedItems).ToList(),
                loader: Loader,
                fieldName: FieldName);
        }
    }
}
