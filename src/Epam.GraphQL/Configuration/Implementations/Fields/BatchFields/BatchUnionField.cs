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
using Epam.GraphQL.Helpers;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.Fields.BatchFields
{
    internal class BatchUnionField<TEntity, TExecutionContext> : FieldBase<TEntity, TExecutionContext>,
        IFieldSupportsApplySelect<TEntity, IEnumerable<object>, TExecutionContext>,
        IFieldSupportsApplyBatchUnion<TEntity, TExecutionContext>,
        IFieldSupportsEditSettings<TEntity, IEnumerable<object>, TExecutionContext>
        where TEntity : class
    {
        private readonly IBatchCompoundResolver<TEntity, TExecutionContext> _resolver;
        private readonly UnionGraphTypeDescriptor<TExecutionContext> _unionGraphType = new();
        private readonly Type _fieldType;

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
           : base(configurationContext, parent, name)
        {
            _resolver = new BatchCompoundResolver<TEntity, TExecutionContext>();

            _resolver.Add(firstResolver);
            _resolver.Add(secondResolver);

            _fieldType = ReflectionHelpers.GetTheBestCommonBaseType(firstType, secondType);
            _unionGraphType.Add(firstGraphType);
            _unionGraphType.Add(secondGraphType);
            _unionGraphType.Name = parent.GetGraphQLTypeName(_fieldType, null, this);

            EditSettings = new FieldEditSettings<TEntity, IEnumerable<object>, TExecutionContext>();
        }

        public override Type FieldType => _fieldType;

        public new IFieldEditSettings<TEntity, IEnumerable<object>, TExecutionContext>? EditSettings
        {
            get => (IFieldEditSettings<TEntity, IEnumerable<object>, TExecutionContext>?)base.EditSettings;
            set => base.EditSettings = value;
        }

        public override IGraphTypeDescriptor<TExecutionContext> GraphType => _unionGraphType.MakeListDescriptor();

        public override IFieldResolver Resolver => _resolver;

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            IInlinedChainConfigurationContext configurationContext,
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            // TODO Implement ApplyBatchUnion method
            throw new NotImplementedException();
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            IInlinedChainConfigurationContext configurationContext,
            Func<IEnumerable<TEntity>, IDictionary<TEntity, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            // TODO Implement ApplyBatchUnion method
            throw new NotImplementedException();
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType>(
            IInlinedChainConfigurationContext configurationContext,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            // TODO Implement ApplyBatchUnion method
            throw new NotImplementedException();
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType>(
            IInlinedChainConfigurationContext configurationContext,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, IDictionary<TKeyType, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            // TODO Implement ApplyBatchUnion method
            throw new NotImplementedException();
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            IInlinedChainConfigurationContext configurationContext,
            Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            // TODO Implement ApplyBatchUnion method
            throw new NotImplementedException();
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType>(
            IInlinedChainConfigurationContext configurationContext,
            Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            // TODO Implement ApplyBatchUnion method
            throw new NotImplementedException();
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType>(
            IInlinedChainConfigurationContext configurationContext,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            // TODO Implement ApplyBatchUnion method
            throw new NotImplementedException();
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TAnotherReturnType, TKeyType>(
            IInlinedChainConfigurationContext configurationContext,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
            where TAnotherReturnType : class
        {
            // TODO Implement ApplyBatchUnion method
            throw new NotImplementedException();
        }

        public SelectField<TEntity, TReturnType1, TExecutionContext> ApplySelect<TReturnType1>(
            IChainConfigurationContext configurationContext,
            Func<IEnumerable<object>, TReturnType1> selector)
        {
            return Parent.ApplySelect<TReturnType1>(configurationContext, this, _resolver.Select(selector));
        }

        public SelectField<TEntity, TReturnType1, TExecutionContext> ApplySelect<TReturnType1>(
            IInlinedChainConfigurationContext configurationContext,
            Func<IEnumerable<object>, TReturnType1> selector,
            Action<IInlineObjectBuilder<TReturnType1, TExecutionContext>>? build)
            where TReturnType1 : class
        {
            return Parent.ApplySelect(configurationContext, this, _resolver.Select(selector), build);
        }

        IFieldSupportsEditSettings<TEntity, TReturnType1, TExecutionContext> IFieldSupportsApplySelect<TEntity, IEnumerable<object>, TExecutionContext>.ApplySelect<TReturnType1>(
            IChainConfigurationContext configurationContext,
            Func<IEnumerable<object>, TReturnType1> selector)
        {
            return ApplySelect(configurationContext, selector);
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
