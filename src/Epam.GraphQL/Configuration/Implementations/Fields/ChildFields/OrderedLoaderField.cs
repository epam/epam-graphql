// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Search;
using Epam.GraphQL.Sorters.Implementations;
using GraphQL;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ChildFields
{
#pragma warning disable CA1501
    internal class OrderedLoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext> : LoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>
#pragma warning restore CA1501
        where TEntity : class
        where TChildEntity : class
        where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
    {
        private readonly Func<IQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>> _orderBy;
        private readonly Func<IOrderedQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>> _thenBy;

        protected OrderedLoaderField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            TChildLoader loader,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType,
            IOrderedQueryableResolver<TEntity, TChildEntity, TExecutionContext> resolver,
            LazyQueryArguments arguments,
            Func<IQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>> orderBy,
            Func<IOrderedQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>> thenBy)
            : base(registry, parent, name, condition, loader, elementGraphType, resolver, arguments)
        {
            _orderBy = orderBy ?? throw new ArgumentNullException(nameof(orderBy));
            _thenBy = thenBy ?? throw new ArgumentNullException(nameof(thenBy));
        }

        protected OrderedLoaderField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            TChildLoader loader,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType,
            LazyQueryArguments arguments,
            Func<IQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>> orderBy,
            Func<IOrderedQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>> thenBy)
            : base(registry, parent, name, condition, loader, elementGraphType, CreateResolver(name, parent, condition, loader, orderBy, thenBy), arguments)
        {
            _orderBy = orderBy ?? throw new ArgumentNullException(nameof(orderBy));
            _thenBy = thenBy ?? throw new ArgumentNullException(nameof(thenBy));
        }

        protected IOrderedQueryableResolver<TEntity, TChildEntity, TExecutionContext> OrderedQueryableFieldResolver => (IOrderedQueryableResolver<TEntity, TChildEntity, TExecutionContext>)EnumerableFieldResolver;

        public override EnumerableField<TEntity, TChildEntity, TExecutionContext> ApplyWhere(Expression<Func<TChildEntity, bool>> condition)
        {
            EnumerableFieldResolver = OrderedQueryableFieldResolver.Where(condition);
            return this;
        }

        protected static Func<IResolveFieldContext, IQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>> ApplySort(
            TChildLoader loader,
            ISearcher<TChildEntity, TExecutionContext> searcher,
            Func<IQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>> orderBy,
            Func<IOrderedQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>> thenBy)
        {
            return (context, children) => SortingHelpers.ApplySort(
                context,
                children,
                loader.ObjectGraphTypeConfigurator.Sorters,
                searcher as IOrderedSearcher<TChildEntity, TExecutionContext>,
                orderBy,
                thenBy);
        }

        protected override QueryableField<TEntity, TChildEntity, TExecutionContext> ApplySelector(Func<IResolveFieldContext, IQueryable<TChildEntity>, IQueryable<TChildEntity>> selector)
        {
            EnumerableFieldResolver = OrderedQueryableFieldResolver.Select(selector, ApplySort(Loader, Searcher, _orderBy, _thenBy));
            return this;
        }

        private static IOrderedQueryableResolver<TEntity, TChildEntity, TExecutionContext> CreateResolver(
            string fieldName,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            TChildLoader loader,
            Func<IQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>> orderBy,
            Func<IOrderedQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>> thenBy)
        {
            return condition == null
                ? new OrderedQueryableFuncResolver<TEntity, TChildEntity, TExecutionContext>(loader.ObjectGraphTypeConfigurator.ProxyAccessor, GetQuery(loader), (ctx, query) => loader.DoApplySecurityFilter(ctx.GetUserContext<TExecutionContext>(), query), ApplySort(loader, null, orderBy, thenBy))
                : new OrderedQueryableAsyncFuncResolver<TEntity, TChildEntity, TExecutionContext>(fieldName, GetQuery(loader), condition, (ctx, query) => loader.DoApplySecurityFilter(ctx.GetUserContext<TExecutionContext>(), query), ApplySort(loader, null, orderBy, thenBy), parent.ProxyAccessor, loader.ObjectGraphTypeConfigurator.ProxyAccessor);
        }
    }
}
