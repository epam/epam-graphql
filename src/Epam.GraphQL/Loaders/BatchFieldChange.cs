// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

namespace Epam.GraphQL.Loaders
{
    internal class BatchFieldChange<TEntity, T, TBatchReturnType, TExecutionContext> : FieldChange<TEntity, T, TExecutionContext>,
        IBatchFieldChange<TEntity, T, TBatchReturnType, TExecutionContext>
    {
        public BatchFieldChange(
            TExecutionContext context,
            TEntity entity,
            T? previousValue,
            T? nextValue,
            TBatchReturnType? batchEntity)
            : base(context, entity, previousValue, nextValue)
        {
            BatchEntity = batchEntity;
        }

        public TBatchReturnType? BatchEntity { get; set; }
    }
}
