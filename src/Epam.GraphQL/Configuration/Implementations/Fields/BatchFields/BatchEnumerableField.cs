// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;
using Epam.GraphQL.Diagnostics;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.Fields.BatchFields
{
    internal class BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext> : TypedField<TEntity, IEnumerable<TReturnType>, TExecutionContext>,
        IFieldSupportsEditSettings<TEntity, IEnumerable<TReturnType>, TExecutionContext>,
        IFieldSupportsApplySelect<TEntity, IEnumerable<TReturnType>, TExecutionContext>,
        IFieldSupportsApplyBatchUnion<TEntity, TExecutionContext>
    {
        public BatchEnumerableField(
            IResolvedChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> graphType)
            : this(
                  configurationContext,
                  parent,
                  name,
                  new BatchEnumerableKeyResolver<TEntity, TKeyType, TReturnType, TExecutionContext>(configurationContext, name, keySelector, batchFunc, parent.ProxyAccessor),
                  graphType)
        {
        }

        public BatchEnumerableField(
            IResolvedChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> graphType)
            : this(
                  configurationContext,
                  parent,
                  name,
                  new BatchEnumerableTaskKeyResolver<TEntity, TKeyType, TReturnType, TExecutionContext>(configurationContext, name, keySelector, batchFunc, parent.ProxyAccessor),
                  graphType)
        {
        }

        private BatchEnumerableField(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IBatchResolver<TEntity, IEnumerable<TReturnType>> batchResolver,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> elementGraphType)
            : base(
                  configurationContext,
                  parent,
                  name)
        {
            BatchFieldResolver = batchResolver;
            ElementGraphType = elementGraphType;
            EditSettings = new FieldEditSettings<TEntity, IEnumerable<TReturnType>, TExecutionContext>();
        }

        public override IGraphTypeDescriptor<TExecutionContext> GraphType => ElementGraphType.MakeListDescriptor();

        public override IFieldResolver Resolver => BatchFieldResolver;

        protected IGraphTypeDescriptor<TReturnType, TExecutionContext> ElementGraphType { get; }

        protected IBatchResolver<TEntity, IEnumerable<TReturnType>> BatchFieldResolver { get; set; }

        public IFieldSupportsEditSettings<TEntity, T, TExecutionContext> ApplySelect<T>(
            IInlinedChainConfigurationContext configurationContext,
            Func<IEnumerable<TReturnType>, T> selector,
            Action<IInlineObjectBuilder<T, TExecutionContext>>? build)
        {
            return Parent.ApplySelect(configurationContext, this, BatchFieldResolver.Select(selector), build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
        {
            return Parent.ApplyBatchUnion(configurationContext, this, BatchFieldResolver, GraphType, batchFunc, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            Func<IEnumerable<TEntity>, IDictionary<TEntity, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
        {
            return Parent.ApplyBatchUnion(configurationContext, this, BatchFieldResolver, GraphType, batchFunc, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType1>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            Expression<Func<TEntity, TKeyType1>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType1>, IDictionary<TKeyType1, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
        {
            return Parent.ApplyBatchUnion(configurationContext, this, BatchFieldResolver, GraphType, keySelector, batchFunc, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType1>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            Expression<Func<TEntity, TKeyType1>> keySelector,
            Func<IEnumerable<TKeyType1>, IDictionary<TKeyType1, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
        {
            return Parent.ApplyBatchUnion(configurationContext, this, BatchFieldResolver, GraphType, keySelector, batchFunc, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
        {
            return Parent.ApplyBatchUnion(configurationContext, this, BatchFieldResolver, GraphType, batchFunc, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
        {
            return Parent.ApplyBatchUnion(configurationContext, this, BatchFieldResolver, GraphType, batchFunc, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType1>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            Expression<Func<TEntity, TKeyType1>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType1>, Task<IDictionary<TKeyType1, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
        {
            return Parent.ApplyBatchUnion(configurationContext, this, BatchFieldResolver, GraphType, keySelector, batchFunc, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType1>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            Expression<Func<TEntity, TKeyType1>> keySelector,
            Func<IEnumerable<TKeyType1>, Task<IDictionary<TKeyType1, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
        {
            return Parent.ApplyBatchUnion(configurationContext, this, BatchFieldResolver, GraphType, keySelector, batchFunc, build);
        }
    }
}
