// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;
using Epam.GraphQL.Enums;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Search;

#nullable enable

namespace Epam.GraphQL.Configuration.Implementations.Fields.ChildFields
{
    internal class LoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext> : QueryableFieldBase<TEntity, TChildEntity, TExecutionContext>
        where TEntity : class
        where TChildEntity : class
        where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
    {
        public LoaderField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TChildEntity, bool>>? condition,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType,
            LazyQueryArguments? arguments,
            ISearcher<TChildEntity, TExecutionContext>? searcher,
            Func<IQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>>? orderBy,
            Func<IOrderedQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>>? thenBy)
            : this(
                  registry,
                  parent,
                  name,
                  registry.ResolveLoader<TChildLoader, TChildEntity>(),
                  condition,
                  elementGraphType,
                  arguments,
                  searcher,
                  orderBy,
                  thenBy)
        {
        }

        protected LoaderField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IQueryableResolver<TEntity, TChildEntity, TExecutionContext> resolver,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType,
            LazyQueryArguments? arguments,
            ISearcher<TChildEntity, TExecutionContext>? searcher,
            Func<IQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>>? orderBy,
            Func<IOrderedQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>>? thenBy)
            : this(
                  registry,
                  parent,
                  name,
                  resolver,
                  elementGraphType,
                  registry.ResolveLoader<TChildLoader, TChildEntity>(),
                  arguments,
                  searcher,
                  orderBy,
                  thenBy)
        {
        }

        private LoaderField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IQueryableResolver<TEntity, TChildEntity, TExecutionContext> resolver,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType,
            TChildLoader loader,
            LazyQueryArguments? arguments,
            ISearcher<TChildEntity, TExecutionContext>? searcher,
            Func<IQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>>? orderBy,
            Func<IOrderedQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>>? thenBy)
            : base(
                  registry,
                  parent,
                  name,
                  resolver,
                  elementGraphType,
                  loader.ObjectGraphTypeConfigurator,
                  arguments,
                  searcher,
                  orderBy,
                  thenBy)
        {
            Loader = loader;
        }

        private LoaderField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            TChildLoader loader,
            Expression<Func<TEntity, TChildEntity, bool>>? condition,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType,
            LazyQueryArguments? arguments,
            ISearcher<TChildEntity, TExecutionContext>? searcher,
            Func<IQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>>? orderBy,
            Func<IOrderedQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>>? thenBy)
            : base(
                  registry,
                  parent,
                  name,
                  query: context => loader.DoGetBaseQuery(context.GetUserContext<TExecutionContext>()),
                  transform: (ctx, query) => loader.DoApplySecurityFilter(ctx.GetUserContext<TExecutionContext>(), query),
                  condition,
                  elementGraphType,
                  loader.ObjectGraphTypeConfigurator,
                  arguments,
                  searcher,
                  orderBy,
                  thenBy)
        {
            Loader = loader;
        }

        protected TChildLoader Loader { get; }

        public ConnectionLoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext> ApplyConnection()
        {
            var connectionField = new ConnectionLoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>(
                Registry,
                Parent,
                Name,
                QueryableFieldResolver,
                ElementGraphType,
                Arguments,
                Searcher,
                Loader.ApplyNaturalOrderBy,
                Loader.ApplyNaturalThenBy);
            return ApplyField(connectionField);
        }

        public override QueryableFieldBase<TEntity, TChildEntity, TExecutionContext> ApplyConnection(Expression<Func<IQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>>> order)
        {
            var connectionField = new ConnectionLoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>(
                Registry,
                Parent,
                Name,
                QueryableFieldResolver,
                ElementGraphType,
                Arguments,
                Searcher,
                order.Compile(),
                order.GetThenBy().Compile());
            return ApplyField(connectionField);
        }

        public override QueryableFieldBase<TEntity, TChildEntity, TExecutionContext> ApplyFilter<TLoaderFilter, TFilter>()
        {
            if (HasFilter)
            {
                throw new NotSupportedException($"{typeof(TChildEntity).HumanizedName()}: Simultaneous use of .WithFilter() and .Filterable() is not supported. Consider to use either .WithFilter() or .Filterable().");
            }

            return base.ApplyFilter<TLoaderFilter, TFilter>();
        }

        protected override EnumerableFieldBase<TEntity, TReturnType1, TExecutionContext> CreateSelect<TReturnType1>(Expression<Func<TChildEntity, TReturnType1>> selector, IGraphTypeDescriptor<TReturnType1, TExecutionContext> graphType)
        {
            var queryableField = new QueryableField<TEntity, TReturnType1, TExecutionContext>(
                Registry,
                Parent,
                Name,
                QueryableFieldResolver.Select(selector, graphType.Configurator?.ProxyAccessor),
                graphType,
                graphType.Configurator,
                Arguments,
                searcher: null,
                orderBy: null,
                thenBy: null);

            return queryableField;
        }

        protected override QueryableFieldBase<TEntity, TChildEntity, TExecutionContext> ReplaceResolver(IQueryableResolver<TEntity, TChildEntity, TExecutionContext> resolver)
        {
            var queryableField = new LoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>(
                Registry,
                Parent,
                Name,
                resolver,
                ElementGraphType,
                Loader,
                Arguments,
                Searcher,
                OrderBy,
                ThenBy);

            return queryableField;
        }
    }

#pragma warning disable CA1501
    internal class LoaderField<TLoader, TChildLoader, TEntity, TChildEntity, TExecutionContext> : LoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>
#pragma warning restore CA1501
        where TLoader : Loader<TEntity, TExecutionContext>, new()
        where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
        where TEntity : class
        where TChildEntity : class
    {
        public LoaderField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            RelationType relationType,
            Expression<Func<TChildEntity, TEntity>> navigationProperty,
            Expression<Func<TEntity, TChildEntity>> reverseNavigationProperty,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType)
            : base(registry, parent, name, condition, elementGraphType, arguments: null, searcher: null, orderBy: null, thenBy: null)
        {
            registry.Register(typeof(TChildLoader), typeof(TLoader), condition.SwapOperands(), reverseNavigationProperty, navigationProperty, relationType);
        }
    }
}
