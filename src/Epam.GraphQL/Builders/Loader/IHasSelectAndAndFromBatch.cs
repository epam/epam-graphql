// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Epam.GraphQL.Builders.Loader
{
    public interface IHasSelectAndAndFromBatch<TEntity, TReturnType, TExecutionContext> :
        IHasSelect<TReturnType, TExecutionContext>
        where TEntity : class
    {
        IHasSelectAndAndFromBatch<TEntity, IEnumerable<object>, TExecutionContext> AndFromBatch<TAnotherReturnType>(
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class;

        IHasSelectAndAndFromBatch<TEntity, IEnumerable<object>, TExecutionContext> AndFromBatch<TAnotherReturnType>(
            Func<IEnumerable<TEntity>, IDictionary<TEntity, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class;

        IHasSelectAndAndFromBatch<TEntity, IEnumerable<object>, TExecutionContext> AndFromBatch<TAnotherReturnType, TKeyType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class;

        IHasSelectAndAndFromBatch<TEntity, IEnumerable<object>, TExecutionContext> AndFromBatch<TAnotherReturnType, TKeyType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, IDictionary<TKeyType, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class;
    }
}
