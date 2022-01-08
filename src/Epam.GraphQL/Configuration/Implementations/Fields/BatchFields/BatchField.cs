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
using GraphQL;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.Fields.BatchFields
{
    internal class BatchField<TEntity, TReturnType, TExecutionContext> : TypedField<TEntity, TReturnType, TExecutionContext>,
        IFieldSupportsApplySelect<TEntity, TReturnType, TExecutionContext>,
        IFieldSupportsEditSettings<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        private readonly IGraphTypeDescriptor<TExecutionContext> _graphType;

        public BatchField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc,
            IGraphTypeDescriptor<TExecutionContext> graphType)
            : this(
                registry,
                parent,
                name,
                new BatchResolver<TEntity, TReturnType, TExecutionContext>(name, batchFunc, parent.ProxyAccessor),
                graphType)
        {
        }

        public BatchField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc,
            IGraphTypeDescriptor<TExecutionContext> graphType)
            : this(
                registry,
                parent,
                name,
                new BatchTaskResolver<TEntity, TReturnType, TExecutionContext>(name, batchFunc, parent.ProxyAccessor),
                graphType)
        {
        }

        protected BatchField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IBatchResolver<TEntity, TReturnType> resolver,
            IGraphTypeDescriptor<TExecutionContext> graphType)
            : base(registry, parent, name)
        {
            _graphType = graphType;

            FieldResolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            EditSettings = new FieldEditSettings<TEntity, TReturnType, TExecutionContext>();
        }

        public override IGraphTypeDescriptor<TExecutionContext> GraphType
        {
            get
            {
                var graphType = _graphType;

                if (Parent is InputObjectGraphTypeConfigurator<TEntity, TExecutionContext> && !EditSettings.IsMandatoryForUpdate)
                {
                    graphType = graphType.UnwrapIfNonNullable();
                }

                return graphType;
            }
        }

        public override bool CanResolve => FieldResolver != null;

        protected IBatchResolver<TEntity, TReturnType> FieldResolver { get; }

        public override object Resolve(IResolveFieldContext context)
        {
            return FieldResolver.Resolve(context);
        }

        public SelectField<TEntity, TReturnType1, TExecutionContext> ApplySelect<TReturnType1>(Func<TReturnType, TReturnType1> selector)
        {
            return Parent.ApplySelect<TReturnType1>(this, FieldResolver.Select(selector));
        }

        public SelectField<TEntity, TReturnType1, TExecutionContext> ApplySelect<TReturnType1>(Func<TReturnType, TReturnType1> selector, Action<IInlineObjectBuilder<TReturnType1, TExecutionContext>> build)
            where TReturnType1 : class
        {
            // TODO Implement ApplySelect method
            throw new NotImplementedException();
        }

        IFieldSupportsEditSettings<TEntity, TReturnType1, TExecutionContext> IFieldSupportsApplySelect<TEntity, TReturnType, TExecutionContext>.ApplySelect<TReturnType1>(Func<TReturnType, TReturnType1> selector)
        {
            return ApplySelect(selector);
        }

        IFieldSupportsEditSettings<TEntity, TReturnType1, TExecutionContext> IFieldSupportsApplySelect<TEntity, TReturnType, TExecutionContext>.ApplySelect<TReturnType1>(Func<TReturnType, TReturnType1> selector, Action<IInlineObjectBuilder<TReturnType1, TExecutionContext>> build)
        {
            return ApplySelect(selector, build);
        }

        protected override IFieldResolver GetResolver()
        {
            return FieldResolver;
        }
    }

    internal class BatchField<TEntity, TKeyType, TReturnType, TExecutionContext> : BatchField<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        public BatchField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc,
            IGraphTypeDescriptor<TExecutionContext> graphType)
            : this(
                registry,
                parent,
                name,
                new BatchKeyResolver<TEntity, TKeyType, TReturnType, TExecutionContext>(name, keySelector, batchFunc, parent.ProxyAccessor),
                graphType)
        {
        }

        public BatchField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc,
            IGraphTypeDescriptor<TExecutionContext> graphType)
            : this(
                registry,
                parent,
                name,
                new BatchTaskKeyResolver<TEntity, TKeyType, TReturnType, TExecutionContext>(name, keySelector, batchFunc, parent.ProxyAccessor),
                graphType)
        {
        }

        protected BatchField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IBatchResolver<TEntity, TReturnType> resolver,
            IGraphTypeDescriptor<TExecutionContext> graphType)
            : base(registry, parent, name, resolver, graphType)
        {
        }
    }
}
