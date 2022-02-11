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
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.Fields.BatchFields
{
    internal class BatchField<TEntity, TKeyType, TReturnType, TExecutionContext> : TypedField<TEntity, TReturnType, TExecutionContext>,
        IFieldSupportsApplySelect<TEntity, TReturnType, TExecutionContext>,
        IFieldSupportsEditSettings<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        private readonly IGraphTypeDescriptor<TExecutionContext> _graphType;

        public BatchField(
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc,
            IGraphTypeDescriptor<TExecutionContext> graphType)
            : this(
                parent,
                name,
                new BatchKeyResolver<TEntity, TKeyType, TReturnType, TExecutionContext>(name, keySelector, batchFunc, parent.ProxyAccessor),
                graphType)
        {
        }

        public BatchField(
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc,
            IGraphTypeDescriptor<TExecutionContext> graphType)
            : this(
                parent,
                name,
                new BatchTaskKeyResolver<TEntity, TKeyType, TReturnType, TExecutionContext>(name, keySelector, batchFunc, parent.ProxyAccessor),
                graphType)
        {
        }

        protected BatchField(
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IBatchResolver<TEntity, TReturnType> resolver,
            IGraphTypeDescriptor<TExecutionContext> graphType)
            : base(parent, name)
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

        public SelectField<TEntity, TReturnType1, TExecutionContext> ApplySelect<TReturnType1>(Func<TReturnType, TReturnType1> selector)
        {
            return Parent.ApplySelect<TReturnType1>(this, BatchFieldResolver.Select(selector));
        }

        public SelectField<TEntity, TReturnType1, TExecutionContext> ApplySelect<TReturnType1>(
            Func<TReturnType, TReturnType1> selector,
            Action<IInlineObjectBuilder<TReturnType1, TExecutionContext>>? build)
            where TReturnType1 : class
        {
            // TODO Implement ApplySelect method
            throw new NotImplementedException();
        }

        IFieldSupportsEditSettings<TEntity, TReturnType1, TExecutionContext> IFieldSupportsApplySelect<TEntity, TReturnType, TExecutionContext>.ApplySelect<TReturnType1>(
            Func<TReturnType, TReturnType1> selector)
        {
            return ApplySelect(selector);
        }

        IFieldSupportsEditSettings<TEntity, TReturnType1, TExecutionContext> IFieldSupportsApplySelect<TEntity, TReturnType, TExecutionContext>.ApplySelect<TReturnType1>(
            Func<TReturnType, TReturnType1> selector,
            Action<IInlineObjectBuilder<TReturnType1, TExecutionContext>>? build)
        {
            return ApplySelect(selector, build);
        }
    }
}
