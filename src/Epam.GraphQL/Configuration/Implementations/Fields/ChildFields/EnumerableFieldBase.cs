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
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Helpers;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ChildFields
{
    internal abstract class EnumerableFieldBase<TThis, TThisIntf, TResolverIntf, TEntity, TReturnType, TExecutionContext> :
        TypedField<TEntity, IEnumerable<TReturnType>, TExecutionContext>,
        IEnumerableField<TThisIntf, TEntity, TReturnType, TExecutionContext>,
        IFieldSupportsEditSettings<TEntity, IEnumerable<TReturnType>, TExecutionContext>
        where TEntity : class
        where TThis : EnumerableFieldBase<TThis, TThisIntf, TResolverIntf, TEntity, TReturnType, TExecutionContext>, TThisIntf
    {
        protected EnumerableFieldBase(
            MethodCallConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IEnumerableResolver<TResolverIntf, TEntity, TReturnType, TExecutionContext> resolver,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> elementGraphType,
            LazyQueryArguments? arguments)
            : base(
                  configurationContext,
                  parent,
                  name)
        {
            ElementGraphType = elementGraphType;
            EditSettings = new FieldEditSettings<TEntity, IEnumerable<TReturnType>, TExecutionContext>();
            EnumerableFieldResolver = resolver;
            Arguments = arguments;
        }

        public override IGraphTypeDescriptor<TExecutionContext> GraphType => ElementGraphType.MakeListDescriptor();

        public override IFieldResolver Resolver => EnumerableFieldResolver;

        protected IGraphTypeDescriptor<TReturnType, TExecutionContext> ElementGraphType { get; }

        protected IEnumerableResolver<TResolverIntf, TEntity, TReturnType, TExecutionContext> EnumerableFieldResolver { get; }

        public IEnumerableField<TEntity, TReturnType1, TExecutionContext> Select<TReturnType1>(Expression<Func<TReturnType, TReturnType1>> selector)
        {
            var graphType = Parent.GetGraphQLTypeDescriptor<TReturnType1>(this);
            var enumerableField = new EnumerableField<TEntity, TReturnType1, TExecutionContext>(
                ConfigurationContext.NextOperation(nameof(Select)).Argument(selector),
                Parent,
                Name,
                EnumerableFieldResolver.Select(selector, graphType.Configurator?.ProxyAccessor),
                graphType,
                Arguments);

            return ApplyField(enumerableField);
        }

        public IEnumerableField<TEntity, TReturnType1, TExecutionContext> Select<TReturnType1>(Expression<Func<TEntity, TReturnType, TReturnType1>> selector)
        {
            var graphType = Parent.GetGraphQLTypeDescriptor<TReturnType1>(this);
            var enumerableField = new EnumerableField<TEntity, TReturnType1, TExecutionContext>(
                ConfigurationContext.NextOperation(nameof(Select)).Argument(selector),
                Parent,
                Name,
                EnumerableFieldResolver.Select(selector),
                graphType,
                Arguments);

            return ApplyField(enumerableField);
        }

        public IEnumerableField<TEntity, TReturnType1, TExecutionContext> Select<TReturnType1>(
            Expression<Func<TReturnType, TReturnType1>> selector,
            Action<IInlineObjectBuilder<TReturnType1, TExecutionContext>>? build = default)
            where TReturnType1 : class
        {
            var configurationContext = ConfigurationContext.NextOperation<TReturnType1>(nameof(Select))
                .Argument(selector)
                .OptionalArgument(build);

            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext);

            var enumerableField = new EnumerableField<TEntity, TReturnType1, TExecutionContext>(
                configurationContext.Parent,
                Parent,
                Name,
                EnumerableFieldResolver.Select(selector, graphType.Configurator?.ProxyAccessor),
                graphType,
                Arguments);

            return ApplyField(enumerableField);
        }

        public IVoid SingleOrDefault(Expression<Func<TReturnType, bool>>? predicate)
        {
            if (predicate != null)
            {
                var where = ApplyWhere(
                    ConfigurationContext.NextOperation(nameof(SingleOrDefault)).Argument(predicate),
                    predicate);
                return Parent.ApplySelect<TReturnType>(where.ConfigurationContext, where, where.EnumerableFieldResolver.SingleOrDefault(), ElementGraphType);
            }

            return Parent.ApplySelect<TReturnType>(
                ConfigurationContext.NextOperation(nameof(SingleOrDefault)),
                this,
                EnumerableFieldResolver.SingleOrDefault(),
                ElementGraphType);
        }

        public IVoid FirstOrDefault(Expression<Func<TReturnType, bool>>? predicate)
        {
            if (predicate != null)
            {
                var where = ApplyWhere(
                    ConfigurationContext.NextOperation(nameof(FirstOrDefault)).Argument(predicate),
                    predicate);
                return Parent.ApplySelect<TReturnType>(where.ConfigurationContext, where, where.EnumerableFieldResolver.FirstOrDefault(), ElementGraphType);
            }

            return Parent.ApplySelect<TReturnType>(
                ConfigurationContext.NextOperation(nameof(FirstOrDefault)),
                this,
                EnumerableFieldResolver.FirstOrDefault(),
                ElementGraphType);
        }

        public TThisIntf Where(Expression<Func<TReturnType, bool>> predicate)
        {
            return ApplyWhere(
                ConfigurationContext.NextOperation(nameof(Where)).Argument(predicate),
                predicate);
        }

        public TThis ApplyWhere(MethodCallConfigurationContext configurationContext, Expression<Func<TReturnType, bool>> predicate)
        {
            var whereField = CreateWhere(configurationContext, predicate);
            return ApplyField(whereField);
        }

        protected abstract TThis CreateWhere(MethodCallConfigurationContext configurationContext, Expression<Func<TReturnType, bool>> predicate);
    }
}
