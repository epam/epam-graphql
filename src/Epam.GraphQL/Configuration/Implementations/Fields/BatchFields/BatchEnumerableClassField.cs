// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;
using Epam.GraphQL.Diagnostics;

namespace Epam.GraphQL.Configuration.Implementations.Fields.BatchFields
{
    internal class BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext> : BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext>,
        IFieldSupportsApplyBatchUnion<TEntity, TExecutionContext>
        where TEntity : class
        where TReturnType : class
    {
        public BatchEnumerableClassField(
            MethodCallConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> graphType)
            : base(
                  configurationContext,
                  parent,
                  name,
                  new BatchEnumerableKeyResolver<TEntity, TKeyType, TReturnType, TExecutionContext>(name, keySelector, batchFunc, parent.ProxyAccessor),
                  graphType)
        {
        }

        public BatchEnumerableClassField(
            MethodCallConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> graphType)
            : base(
                  configurationContext,
                  parent,
                  name,
                  new BatchEnumerableTaskKeyResolver<TEntity, TKeyType, TReturnType, TExecutionContext>(name, keySelector, batchFunc, parent.ProxyAccessor),
                  graphType)
        {
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            MethodCallArgumentConfigurationContext configurationContext,
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            // TODO Implement ApplyBatchUnion method
            throw new NotImplementedException();
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            MethodCallArgumentConfigurationContext configurationContext,
            Func<IEnumerable<TEntity>, IDictionary<TEntity, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            // TODO Implement ApplyBatchUnion method
            throw new NotImplementedException();
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType1>(
            MethodCallArgumentConfigurationContext configurationContext,
            Expression<Func<TEntity, TKeyType1>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType1>, IDictionary<TKeyType1, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            // TODO Implement ApplyBatchUnion method
            throw new NotImplementedException();
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType1>(
            MethodCallArgumentConfigurationContext configurationContext,
            Expression<Func<TEntity, TKeyType1>> keySelector,
            Func<IEnumerable<TKeyType1>, IDictionary<TKeyType1, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
        where TAnotherReturnType : class
        {
            // TODO Implement ApplyBatchUnion method
            throw new NotImplementedException();
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            MethodCallArgumentConfigurationContext configurationContext,
            Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            // TODO Implement ApplyBatchUnion method
            throw new NotImplementedException();
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            MethodCallArgumentConfigurationContext configurationContext,
            Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            // TODO Implement ApplyBatchUnion method
            throw new NotImplementedException();
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType1>(
            MethodCallArgumentConfigurationContext configurationContext,
            Expression<Func<TEntity, TKeyType1>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType1>, Task<IDictionary<TKeyType1, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            // TODO Implement ApplyBatchUnion method
            throw new NotImplementedException();
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType1>(
            MethodCallArgumentConfigurationContext configurationContext,
            Expression<Func<TEntity, TKeyType1>> keySelector,
            Func<IEnumerable<TKeyType1>, Task<IDictionary<TKeyType1, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            // TODO Implement ApplyBatchUnion method
            throw new NotImplementedException();
        }
    }
}
