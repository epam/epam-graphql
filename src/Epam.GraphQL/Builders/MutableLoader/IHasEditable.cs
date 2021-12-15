// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Builders.MutableLoader
{
    public interface IHasEditable<TEntity, TReturnType, TExecutionContext>
    {
        void Editable();

        void EditableIf(Func<IFieldChange<TEntity, TReturnType, TExecutionContext>, bool> predicate, Func<IFieldChange<TEntity, TReturnType, TExecutionContext>, string> reason = null);

        void BatchedEditableIf<TItem>(Func<IEnumerable<TEntity>, IEnumerable<KeyValuePair<TEntity, TItem>>> batchFunc, Func<IBatchFieldChange<TEntity, TReturnType, TItem, TExecutionContext>, bool> predicate, Func<IBatchFieldChange<TEntity, TReturnType, TItem, TExecutionContext>, string> reason = null);

        void BatchedEditableIf<TItem>(Func<TExecutionContext, IEnumerable<TEntity>, IEnumerable<KeyValuePair<TEntity, TItem>>> batchFunc, Func<IBatchFieldChange<TEntity, TReturnType, TItem, TExecutionContext>, bool> predicate, Func<IBatchFieldChange<TEntity, TReturnType, TItem, TExecutionContext>, string> reason = null);
    }
}
