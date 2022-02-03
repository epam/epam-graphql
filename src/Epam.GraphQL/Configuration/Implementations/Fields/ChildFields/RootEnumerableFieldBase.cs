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
using Epam.GraphQL.Helpers;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ChildFields
{
    internal abstract class RootEnumerableFieldBase<TReturnType, TExecutionContext> : TypedField<object, IEnumerable<TReturnType>, TExecutionContext>,
        IRootEnumerableField<TReturnType, TExecutionContext>,
        IFieldSupportsEditSettings<object, IEnumerable<TReturnType>, TExecutionContext>
    {
        protected RootEnumerableFieldBase(
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            IRootEnumerableResolver<TReturnType, TExecutionContext> resolver,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> elementGraphType)
            : base(
                  parent,
                  name)
        {
            ElementGraphType = elementGraphType;
            EditSettings = new FieldEditSettings<object, IEnumerable<TReturnType>, TExecutionContext>();
            EnumerableFieldResolver = resolver;
        }

        public override IGraphTypeDescriptor<TExecutionContext> GraphType => ElementGraphType.MakeListDescriptor();

        public override IFieldResolver Resolver => EnumerableFieldResolver;

        protected IGraphTypeDescriptor<TReturnType, TExecutionContext> ElementGraphType { get; }

        protected IRootEnumerableResolver<TReturnType, TExecutionContext> EnumerableFieldResolver { get; }

        public IRootEnumerableField<TReturnType1, TExecutionContext> Select<TReturnType1>(Expression<Func<TReturnType, TReturnType1>> selector)
        {
            var graphType = Parent.GetGraphQLTypeDescriptor<TReturnType1>(this);
            var enumerableField = CreateSelect(selector, graphType);
            return ApplyField(enumerableField);
        }

        public IRootEnumerableField<TReturnType1, TExecutionContext> Select<TReturnType1>(Expression<Func<TReturnType, TReturnType1>> selector, Action<IInlineObjectBuilder<TReturnType1, TExecutionContext>>? build = default)
            where TReturnType1 : class
        {
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build);
            var enumerableField = CreateSelect(selector, graphType);
            return ApplyField(enumerableField);
        }

        public IVoid SingleOrDefault(Expression<Func<TReturnType, bool>>? predicate)
        {
            if (predicate != null)
            {
                var where = Where(predicate);
                return where.SingleOrDefault(null);
            }

            return Parent.ApplySelect<TReturnType>(this, EnumerableFieldResolver.SingleOrDefault(), ElementGraphType);
        }

        public IVoid FirstOrDefault(Expression<Func<TReturnType, bool>>? predicate)
        {
            if (predicate != null)
            {
                var where = Where(predicate);
                return where.FirstOrDefault(null);
            }

            return Parent.ApplySelect<TReturnType>(this, EnumerableFieldResolver.FirstOrDefault(), ElementGraphType);
        }

        public IRootEnumerableField<TReturnType, TExecutionContext> Where(Expression<Func<TReturnType, bool>> predicate)
        {
            var enumerableField = CreateWhereImpl(predicate);
            return ApplyField(enumerableField);
        }

        protected abstract RootEnumerableFieldBase<TReturnType1, TExecutionContext> CreateSelect<TReturnType1>(Expression<Func<TReturnType, TReturnType1>> selector, IGraphTypeDescriptor<TReturnType1, TExecutionContext> graphType);

        protected abstract RootEnumerableFieldBase<TReturnType, TExecutionContext> CreateWhereImpl(Expression<Func<TReturnType, bool>> predicate);
    }

    internal abstract class RootEnumerableFieldBase<TThis, TReturnType, TExecutionContext> : RootEnumerableFieldBase<TReturnType, TExecutionContext>
        where TThis : RootEnumerableFieldBase<TThis, TReturnType, TExecutionContext>
    {
        protected RootEnumerableFieldBase(
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            IRootEnumerableResolver<TReturnType, TExecutionContext> resolver,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> elementGraphType)
            : base(
                  parent,
                  name,
                  resolver,
                  elementGraphType)
        {
        }

        protected abstract TThis CreateWhere(Expression<Func<TReturnType, bool>> predicate);

        protected override RootEnumerableFieldBase<TReturnType, TExecutionContext> CreateWhereImpl(Expression<Func<TReturnType, bool>> predicate)
        {
            return CreateWhere(predicate);
        }
    }
}
