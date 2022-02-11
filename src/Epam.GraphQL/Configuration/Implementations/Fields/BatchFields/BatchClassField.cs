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

namespace Epam.GraphQL.Configuration.Implementations.Fields.BatchFields
{
    internal class BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext> : BatchField<TEntity, TKeyType, TReturnType, TExecutionContext>,
        IFieldSupportsApplyBatchUnion<TEntity, TExecutionContext>
        where TEntity : class
        where TReturnType : class
    {
        public BatchClassField(
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc,
            IGraphTypeDescriptor<TExecutionContext> graphType)
            : base(
                parent,
                name,
                new BatchKeyResolver<TEntity, TKeyType, TReturnType, TExecutionContext>(name, keySelector, batchFunc, parent.ProxyAccessor),
                graphType)
        {
        }

        public BatchClassField(
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc,
            IGraphTypeDescriptor<TExecutionContext> graphType)
            : base(
                parent,
                name,
                new BatchTaskKeyResolver<TEntity, TKeyType, TReturnType, TExecutionContext>(name, keySelector, batchFunc, parent.ProxyAccessor),
                graphType)
        {
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            return Parent.ApplyBatchUnion(this, BatchFieldResolver, GraphType, batchFunc, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            Func<IEnumerable<TEntity>, IDictionary<TEntity, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            return Parent.ApplyBatchUnion(this, BatchFieldResolver, GraphType, batchFunc, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType1>(
            Expression<Func<TEntity, TKeyType1>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType1>, IDictionary<TKeyType1, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            return Parent.ApplyBatchUnion(this, BatchFieldResolver, GraphType, keySelector, batchFunc, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType1>(
            Expression<Func<TEntity, TKeyType1>> keySelector,
            Func<IEnumerable<TKeyType1>, IDictionary<TKeyType1, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            return Parent.ApplyBatchUnion(this, BatchFieldResolver, GraphType, keySelector, batchFunc, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            return Parent.ApplyBatchUnion(this, BatchFieldResolver, GraphType, batchFunc, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            return Parent.ApplyBatchUnion(this, BatchFieldResolver, GraphType, batchFunc, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType1>(
            Expression<Func<TEntity, TKeyType1>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType1>, Task<IDictionary<TKeyType1, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            return Parent.ApplyBatchUnion(this, BatchFieldResolver, GraphType, keySelector, batchFunc, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType1>(
            Expression<Func<TEntity, TKeyType1>> keySelector,
            Func<IEnumerable<TKeyType1>, Task<IDictionary<TKeyType1, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            return Parent.ApplyBatchUnion(this, BatchFieldResolver, GraphType, keySelector, batchFunc, build);
        }
    }
}
