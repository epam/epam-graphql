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

#nullable enable

namespace Epam.GraphQL.Configuration.Implementations.Fields.BatchFields
{
    internal class BatchEnumerableField<TEntity, TReturnType, TExecutionContext> : TypedField<TEntity, IEnumerable<TReturnType>, TExecutionContext>,
        IFieldSupportsEditSettings<TEntity, IEnumerable<TReturnType>, TExecutionContext>,
        IFieldSupportsApplySelect<TEntity, IEnumerable<TReturnType>, TExecutionContext>
        where TEntity : class
    {
        public BatchEnumerableField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> elementGraphType)
            : this(
                  registry,
                  parent,
                  name,
                  new BatchEnumerableResolver<TEntity, TReturnType, TExecutionContext>(name, batchFunc, parent.ProxyAccessor),
                  elementGraphType)
        {
        }

        public BatchEnumerableField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> elementGraphType)
            : this(
                  registry,
                  parent,
                  name,
                  new BatchEnumerableTaskResolver<TEntity, TReturnType, TExecutionContext>(name, batchFunc, parent.ProxyAccessor),
                  elementGraphType)
        {
        }

        protected BatchEnumerableField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IBatchResolver<TEntity, IEnumerable<TReturnType>> batchResolver,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> elementGraphType)
            : base(
                  registry,
                  parent,
                  name)
        {
            BatchFieldResolver = batchResolver;
            ElementGraphType = elementGraphType;
            EditSettings = new FieldEditSettings<TEntity, IEnumerable<TReturnType>, TExecutionContext>();
        }

        public override IGraphTypeDescriptor<TExecutionContext> GraphType => ElementGraphType.MakeListDescriptor();

        public override bool CanResolve => FieldResolver != null;

        public virtual IResolver<TEntity> FieldResolver => BatchFieldResolver;

        protected IGraphTypeDescriptor<TReturnType, TExecutionContext> ElementGraphType { get; }

        protected IBatchResolver<TEntity, IEnumerable<TReturnType>> BatchFieldResolver { get; set; }

        public override object Resolve(IResolveFieldContext context)
        {
            return FieldResolver.Resolve(context);
        }

        public IFieldSupportsEditSettings<TEntity, T, TExecutionContext> ApplySelect<T>(Func<IEnumerable<TReturnType>, T> selector)
        {
            return Parent.ApplySelect<T>(this, BatchFieldResolver.Select(selector));
        }

        public IFieldSupportsEditSettings<TEntity, T, TExecutionContext> ApplySelect<T>(Func<IEnumerable<TReturnType>, T> selector, Action<IInlineObjectBuilder<T, TExecutionContext>>? build)
            where T : class
        {
            return Parent.ApplySelect(this, BatchFieldResolver.Select(selector), build);
        }

        protected override IFieldResolver GetResolver()
        {
            return FieldResolver;
        }
    }

    internal class BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext> : BatchEnumerableField<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        public BatchEnumerableField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> elementGraphType)
            : this(
                  registry,
                  parent,
                  name,
                  new BatchEnumerableKeyResolver<TEntity, TKeyType, TReturnType, TExecutionContext>(name, keySelector, batchFunc, parent.ProxyAccessor),
                  elementGraphType)
        {
        }

        public BatchEnumerableField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> elementGraphType)
            : this(
                  registry,
                  parent,
                  name,
                  new BatchEnumerableTaskKeyResolver<TEntity, TKeyType, TReturnType, TExecutionContext>(name, keySelector, batchFunc, parent.ProxyAccessor),
                  elementGraphType)
        {
        }

        protected BatchEnumerableField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IBatchResolver<TEntity, IEnumerable<TReturnType>> batchResolver,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> elementGraphType)
            : base(
                  registry,
                  parent,
                  name,
                  batchResolver,
                  elementGraphType)
        {
        }
    }
}
