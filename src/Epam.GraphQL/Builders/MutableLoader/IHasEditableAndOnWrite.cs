// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Threading.Tasks;

namespace Epam.GraphQL.Builders.MutableLoader
{
    public interface IHasEditableAndOnWrite<TEntity, TReturnType, TExecutionContext> : IHasEditable<TEntity, TReturnType, TExecutionContext>
    {
        IHasEditable<TEntity, TReturnType, TExecutionContext> OnWrite(Action<TExecutionContext, TEntity, TReturnType> save);

        IHasEditable<TEntity, TReturnType, TExecutionContext> OnWrite(Func<TExecutionContext, TEntity, TReturnType, Task> save);
    }
}
