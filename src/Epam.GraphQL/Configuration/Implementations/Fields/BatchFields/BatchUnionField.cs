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
    internal class BatchUnionField<TEntity, TExecutionContext> : FieldBase<TEntity, TExecutionContext>,
        IFieldSupportsApplySelect<TEntity, IEnumerable<object>, TExecutionContext>,
        IFieldSupportsApplyBatchUnion<TEntity, TExecutionContext>,
        IFieldSupportsEditSettings<TEntity, IEnumerable<object>, TExecutionContext>
    {
        private readonly IBatchCompoundResolver<TEntity, TExecutionContext> _resolver;
        private readonly UnionGraphTypeDescriptor<TExecutionContext> _unionGraphType;

        // TODO Implement unions of more than two batches
        public BatchUnionField(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IBatchResolver<TEntity> firstResolver,
            IBatchResolver<TEntity> secondResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            Type firstType,
            IGraphTypeDescriptor<TExecutionContext> secondGraphType,
            Type secondType)
           : this(
               configurationContext,
               parent,
               name,
               new BatchCompoundResolver<TEntity, TExecutionContext>(firstResolver, secondResolver),
               field => new UnionGraphTypeDescriptor<TExecutionContext>(
                   field,
                   new[] { firstGraphType, secondGraphType },
                   new[] { firstType, secondType }))
        {
        }

        private BatchUnionField(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IBatchCompoundResolver<TEntity, TExecutionContext> compoundResolver,
            Func<IField<TExecutionContext>, UnionGraphTypeDescriptor<TExecutionContext>> descriptorFactory)
            : base(configurationContext, parent, name)
        {
            _resolver = compoundResolver;

            _unionGraphType = descriptorFactory(this);

            EditSettings = new FieldEditSettings<TEntity, IEnumerable<object>, TExecutionContext>();
        }

        public override Type FieldType => _unionGraphType.FieldType;

        public new IFieldEditSettings<TEntity, IEnumerable<object>, TExecutionContext>? EditSettings
        {
            get => (IFieldEditSettings<TEntity, IEnumerable<object>, TExecutionContext>?)base.EditSettings;
            set => base.EditSettings = value;
        }

        public override IGraphTypeDescriptor<TExecutionContext> GraphType => _unionGraphType.MakeListDescriptor();

        public override IFieldResolver Resolver => _resolver;

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
        {
            // TODO Implement ApplyBatchUnion method
            throw new NotImplementedException();
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            Func<IEnumerable<TEntity>, IDictionary<TEntity, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
        {
            // TODO Implement ApplyBatchUnion method
            throw new NotImplementedException();
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
        {
            // TODO Implement ApplyBatchUnion method
            throw new NotImplementedException();
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, IDictionary<TKeyType, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
        {
            // TODO Implement ApplyBatchUnion method
            throw new NotImplementedException();
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
        {
            // TODO Implement ApplyBatchUnion method
            throw new NotImplementedException();
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
        {
            // TODO Implement ApplyBatchUnion method
            throw new NotImplementedException();
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
        {
            // TODO Implement ApplyBatchUnion method
            throw new NotImplementedException();
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
        {
            // TODO Implement ApplyBatchUnion method
            throw new NotImplementedException();
        }

        public SelectField<TEntity, TReturnType1, TExecutionContext> ApplySelect<TReturnType1>(
            IInlinedChainConfigurationContext configurationContext,
            Func<IEnumerable<object>, TReturnType1> selector,
            Action<IInlineObjectBuilder<TReturnType1, TExecutionContext>>? build)
        {
            return Parent.ApplySelect(configurationContext, this, _resolver.Select(selector), build);
        }

        IFieldSupportsEditSettings<TEntity, TReturnType1, TExecutionContext> IFieldSupportsApplySelect<TEntity, IEnumerable<object>, TExecutionContext>.ApplySelect<TReturnType1>(
            IInlinedChainConfigurationContext configurationContext,
            Func<IEnumerable<object>, TReturnType1> selector,
            Action<IInlineObjectBuilder<TReturnType1, TExecutionContext>>? build)
        {
            return ApplySelect(configurationContext, selector, build);
        }
    }
}
