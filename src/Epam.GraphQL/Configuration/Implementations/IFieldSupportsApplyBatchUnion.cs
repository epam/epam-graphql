// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration.Implementations.Fields.BatchFields;
using Epam.GraphQL.Diagnostics;

namespace Epam.GraphQL.Configuration.Implementations
{
    internal interface IFieldSupportsApplyBatchUnion<TEntity, TExecutionContext>
        where TEntity : class
    {
        BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            IInlinedChainConfigurationContext configurationContext,
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class;

        BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            IInlinedChainConfigurationContext configurationContext,
            Func<IEnumerable<TEntity>, IDictionary<TEntity, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class;

        BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType>(
            IInlinedChainConfigurationContext configurationContext,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class;

        BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType>(
            IInlinedChainConfigurationContext configurationContext,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class;

        BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            IInlinedChainConfigurationContext configurationContext,
            Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class;

        BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            IInlinedChainConfigurationContext configurationContext,
            Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class;

        BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType>(
            IInlinedChainConfigurationContext configurationContext,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class;

        BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType>(
            IInlinedChainConfigurationContext configurationContext,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, IDictionary<TKeyType, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class;
    }
}
