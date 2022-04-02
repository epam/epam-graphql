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
        IFieldSupportsApplySelect<TEntity, IEnumerable<TReturnType>, TExecutionContext>
        where TEntity : class
    {
        public BatchEnumerableField(
            FieldConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> elementGraphType)
            : this(
                  configurationContext,
                  parent,
                  name,
                  new BatchEnumerableKeyResolver<TEntity, TKeyType, TReturnType, TExecutionContext>(name, keySelector, batchFunc, parent.ProxyAccessor),
                  elementGraphType)
        {
        }

        public BatchEnumerableField(
            FieldConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> elementGraphType)
            : this(
                  configurationContext,
                  parent,
                  name,
                  new BatchEnumerableTaskKeyResolver<TEntity, TKeyType, TReturnType, TExecutionContext>(name, keySelector, batchFunc, parent.ProxyAccessor),
                  elementGraphType)
        {
        }

        protected BatchEnumerableField(
            FieldConfigurationContext configurationContext,
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
            FieldConfigurationContext configurationContext,
            Func<IEnumerable<TReturnType>, T> selector)
        {
            return Parent.ApplySelect<T>(configurationContext, this, BatchFieldResolver.Select(selector));
        }

        public IFieldSupportsEditSettings<TEntity, T, TExecutionContext> ApplySelect<T>(
            FieldConfigurationContext configurationContext,
            Func<IEnumerable<TReturnType>, T> selector,
            Action<IInlineObjectBuilder<T, TExecutionContext>>? build)
            where T : class
        {
            return Parent.ApplySelect(configurationContext, this, BatchFieldResolver.Select(selector), build);
        }
    }
}
