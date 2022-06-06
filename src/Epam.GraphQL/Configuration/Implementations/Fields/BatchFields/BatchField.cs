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
    internal class BatchField<TEntity, TKeyType, TReturnType, TExecutionContext> : TypedField<TEntity, TReturnType, TExecutionContext>,
        IFieldSupportsApplySelect<TEntity, TReturnType, TExecutionContext>,
        IFieldSupportsEditSettings<TEntity, TReturnType, TExecutionContext>,
        IFieldSupportsApplyBatchUnion<TEntity, TExecutionContext>
    {
        private readonly IGraphTypeDescriptor<TExecutionContext> _graphType;

        public BatchField(
            IResolvedChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc,
            IGraphTypeDescriptor<TExecutionContext> graphType)
            : this(
                configurationContext,
                parent,
                name,
                new BatchKeyResolver<TEntity, TKeyType, TReturnType, TExecutionContext>(configurationContext, name, keySelector, batchFunc, parent.ProxyAccessor),
                graphType)
        {
        }

        public BatchField(
            IResolvedChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc,
            IGraphTypeDescriptor<TExecutionContext> graphType)
            : this(
                configurationContext,
                parent,
                name,
                new BatchTaskKeyResolver<TEntity, TKeyType, TReturnType, TExecutionContext>(configurationContext, name, keySelector, batchFunc, parent.ProxyAccessor),
                graphType)
        {
        }

        private BatchField(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IBatchResolver<TEntity, TReturnType> resolver,
            IGraphTypeDescriptor<TExecutionContext> graphType)
            : base(configurationContext, parent, name)
        {
            _graphType = graphType;

            BatchFieldResolver = resolver;
            EditSettings = new FieldEditSettings<TEntity, TReturnType, TExecutionContext>();
        }

        public override IGraphTypeDescriptor<TExecutionContext> GraphType
        {
            get
            {
                var graphType = _graphType;

                if (Parent is InputObjectGraphTypeConfigurator<TEntity, TExecutionContext> && EditSettings != null && !EditSettings.IsMandatoryForUpdate)
                {
                    graphType = graphType.UnwrapIfNonNullable();
                }

                return graphType;
            }
        }

        public override IFieldResolver Resolver => BatchFieldResolver;

        protected IBatchResolver<TEntity, TReturnType> BatchFieldResolver { get; }

        public SelectField<TEntity, TReturnType1, TExecutionContext> ApplySelect<TReturnType1>(
            IInlinedChainConfigurationContext configurationContext,
            Func<TReturnType, TReturnType1> selector,
            Action<IInlineObjectBuilder<TReturnType1, TExecutionContext>>? build)
        {
            return Parent.ApplySelect(configurationContext, this, BatchFieldResolver.Select(selector), build);
        }

        IFieldSupportsEditSettings<TEntity, TReturnType1, TExecutionContext> IFieldSupportsApplySelect<TEntity, TReturnType, TExecutionContext>.ApplySelect<TReturnType1>(
            IInlinedChainConfigurationContext configurationContext,
            Func<TReturnType, TReturnType1> selector,
            Action<IInlineObjectBuilder<TReturnType1, TExecutionContext>>? build)
        {
            return ApplySelect(configurationContext, selector, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            return Parent.ApplyBatchUnion(configurationContext, this, BatchFieldResolver, GraphType, batchFunc, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            Func<IEnumerable<TEntity>, IDictionary<TEntity, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            return Parent.ApplyBatchUnion(configurationContext, this, BatchFieldResolver, GraphType, batchFunc, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType1>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            Expression<Func<TEntity, TKeyType1>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType1>, IDictionary<TKeyType1, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            return Parent.ApplyBatchUnion(configurationContext, this, BatchFieldResolver, GraphType, keySelector, batchFunc, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType1>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            Expression<Func<TEntity, TKeyType1>> keySelector,
            Func<IEnumerable<TKeyType1>, IDictionary<TKeyType1, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            return Parent.ApplyBatchUnion(configurationContext, this, BatchFieldResolver, GraphType, keySelector, batchFunc, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            return Parent.ApplyBatchUnion(configurationContext, this, BatchFieldResolver, GraphType, batchFunc, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            return Parent.ApplyBatchUnion(configurationContext, this, BatchFieldResolver, GraphType, batchFunc, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType1>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            Expression<Func<TEntity, TKeyType1>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType1>, Task<IDictionary<TKeyType1, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            return Parent.ApplyBatchUnion(configurationContext, this, BatchFieldResolver, GraphType, keySelector, batchFunc, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType1>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            Expression<Func<TEntity, TKeyType1>> keySelector,
            Func<IEnumerable<TKeyType1>, Task<IDictionary<TKeyType1, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            return Parent.ApplyBatchUnion(configurationContext, this, BatchFieldResolver, GraphType, keySelector, batchFunc, build);
        }
    }
}
