// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;
using GraphQL;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ChildFields
{
    internal abstract class EnumerableFieldBase<TEntity, TReturnType, TExecutionContext> : TypedField<TEntity, IEnumerable<TReturnType>, TExecutionContext>,
        IFieldSupportsEditSettings<TEntity, IEnumerable<TReturnType>, TExecutionContext>
        where TEntity : class
    {
        public EnumerableFieldBase(
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IEnumerableResolver<TEntity, TReturnType, TExecutionContext> resolver,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> elementGraphType)
            : base(
                  parent,
                  name)
        {
            ElementGraphType = elementGraphType;
            EditSettings = new FieldEditSettings<TEntity, IEnumerable<TReturnType>, TExecutionContext>();
            EnumerableFieldResolver = resolver;
        }

        public override IGraphTypeDescriptor<TExecutionContext> GraphType => ElementGraphType.MakeListDescriptor();

        public override bool CanResolve => FieldResolver != null;

        public virtual IResolver<TEntity> FieldResolver => EnumerableFieldResolver;

        protected IGraphTypeDescriptor<TReturnType, TExecutionContext> ElementGraphType { get; }

        protected IEnumerableResolver<TEntity, TReturnType, TExecutionContext> EnumerableFieldResolver { get; }

        public override object? Resolve(IResolveFieldContext context)
        {
            return FieldResolver?.Resolve(context);
        }

        public EnumerableFieldBase<TEntity, TReturnType, TExecutionContext> ApplyWhere(Expression<Func<TReturnType, bool>> predicate)
        {
            var enumerableField = CreateWhere(predicate);
            return ApplyField(enumerableField);
        }

        public EnumerableFieldBase<TEntity, TReturnType1, TExecutionContext> ApplySelect<TReturnType1>(Expression<Func<TReturnType, TReturnType1>> selector)
        {
            var graphType = Parent.GetGraphQLTypeDescriptor<TReturnType1>(this);
            var enumerableField = CreateSelect(selector, graphType);
            return ApplyField(enumerableField);
        }

        public EnumerableField<TEntity, TReturnType1, TExecutionContext> ApplySelect<TReturnType1>(Expression<Func<TEntity, TReturnType, TReturnType1>> selector)
        {
            var graphType = Parent.GetGraphQLTypeDescriptor<TReturnType1>(this);
            var enumerableField = new EnumerableField<TEntity, TReturnType1, TExecutionContext>(
                Parent,
                Name,
                EnumerableFieldResolver.Select(selector),
                graphType);

            return ApplyField(enumerableField);
        }

        public EnumerableFieldBase<TEntity, TReturnType1, TExecutionContext> ApplySelect<TReturnType1>(Expression<Func<TReturnType, TReturnType1>> selector, Action<IInlineObjectBuilder<TReturnType1, TExecutionContext>>? build)
            where TReturnType1 : class
        {
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build);
            var enumerableField = CreateSelect(selector, graphType);
            return ApplyField(enumerableField);
        }

        public SelectField<TEntity, TReturnType, TExecutionContext> ApplySingleOrDefault(Expression<Func<TReturnType, bool>>? predicate)
        {
            if (predicate != null)
            {
                var where = ApplyWhere(predicate);
                return where.ApplySingleOrDefault(null);
            }

            return Parent.ApplySelect<TReturnType>(this, EnumerableFieldResolver.SingleOrDefault(), ElementGraphType);
        }

        public SelectField<TEntity, TReturnType, TExecutionContext> ApplyFirstOrDefault(Expression<Func<TReturnType, bool>>? predicate)
        {
            if (predicate != null)
            {
                var where = ApplyWhere(predicate);
                return where.ApplyFirstOrDefault(null);
            }

            return Parent.ApplySelect<TReturnType>(this, EnumerableFieldResolver.FirstOrDefault(), ElementGraphType);
        }

        protected abstract EnumerableFieldBase<TEntity, TReturnType1, TExecutionContext> CreateSelect<TReturnType1>(Expression<Func<TReturnType, TReturnType1>> selector, IGraphTypeDescriptor<TReturnType1, TExecutionContext> graphType);

        protected abstract EnumerableFieldBase<TEntity, TReturnType, TExecutionContext> CreateWhere(Expression<Func<TReturnType, bool>> predicate);

        protected override IFieldResolver GetResolver()
        {
            return FieldResolver;
        }
    }
}
