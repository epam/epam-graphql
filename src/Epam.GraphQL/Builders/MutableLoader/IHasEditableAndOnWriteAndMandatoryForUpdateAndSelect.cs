// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Builders.Loader;

namespace Epam.GraphQL.Builders.MutableLoader
{
    public interface IHasEditableAndOnWriteAndMandatoryForUpdateAndSelect<TEntity, TReturnType, TExecutionContext> : IHasEditable<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        IHasEditableAndOnWriteAndMandatoryForUpdate<TEntity, TReturnType1, TExecutionContext> Select<TReturnType1>(Func<TReturnType, TReturnType1> selector)
            where TReturnType1 : struct;

        IHasEditableAndOnWriteAndMandatoryForUpdate<TEntity, TReturnType1?, TExecutionContext> Select<TReturnType1>(Func<TReturnType, TReturnType1?> selector)
            where TReturnType1 : struct;

        IHasEditableAndOnWriteAndMandatoryForUpdate<TEntity, string, TExecutionContext> Select(Func<TReturnType, string> selector);

        IHasEditableAndOnWriteAndMandatoryForUpdate<TEntity, TReturnType1, TExecutionContext> Select<TReturnType1>(Func<TReturnType, TReturnType1> selector, Action<IInlineObjectBuilder<TReturnType1, TExecutionContext>> build = null)
            where TReturnType1 : class;
    }
}
