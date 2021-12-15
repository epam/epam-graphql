// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Filters;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Search;
using Epam.GraphQL.Types;
using GraphQL;
using GraphQL.Types;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ChildFields
{
    internal class QueryableField<TEntity, TReturnType, TExecutionContext> : EnumerableField<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        private readonly Expression<Func<TEntity, TReturnType, bool>> _condition;
        private Func<TExecutionContext, IQueryable<TReturnType>> _query;

        public QueryableField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Func<TExecutionContext, IQueryable<TReturnType>> query,
            Expression<Func<TEntity, TReturnType, bool>> condition,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> elementGraphType)
            : this(
                  registry,
                  parent,
                  name,
                  CreateResolver(parent, name, query, condition, elementGraphType, elementGraphType?.Configurator),
                  elementGraphType,
                  elementGraphType?.Configurator,
                  arguments: null)
        {
            _query = query;
            _condition = condition;
        }

        protected QueryableField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IQueryableResolver<TEntity, TReturnType, TExecutionContext> resolver,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> elementGraphType,
            IObjectGraphTypeConfigurator<TReturnType, TExecutionContext> configurator,
            LazyQueryArguments arguments)
            : this(
                  registry,
                  parent,
                  name,
                  elementGraphType,
                  configurator,
                  arguments)
        {
            EnumerableFieldResolver = resolver;
        }

        private QueryableField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> elementGraphType,
            IObjectGraphTypeConfigurator<TReturnType, TExecutionContext> configurator,
            LazyQueryArguments arguments)
            : base(
                  registry,
                  parent,
                  name,
                  elementGraphType)
        {
            Arguments = arguments;
            ObjectGraphTypeConfigurator = configurator;

            if (HasFilter)
            {
                Argument("filter", () => new QueryArgument(Registry.GenerateInputGraphType(ObjectGraphTypeConfigurator.CreateInlineFilters().FilterType))
                {
                    Name = "filter",
                });
            }

            var sortableFields = ObjectGraphTypeConfigurator?.Sorters.Select(f => f.Name).ToArray();
            if (sortableFields != null && sortableFields.Any())
            {
                Argument("sorting", new ListGraphType(new SortingOptionGraphType(typeof(TReturnType).Name, sortableFields)));
            }
        }

        public virtual bool HasFilter => ObjectGraphTypeConfigurator?.HasInlineFilters ?? false;

        protected ISearcher<TReturnType, TExecutionContext> Searcher { get; private set; }

        protected IObjectGraphTypeConfigurator<TReturnType, TExecutionContext> ObjectGraphTypeConfigurator { get; private set; }

        protected IQueryableResolver<TEntity, TReturnType, TExecutionContext> QueryableFieldResolver => (IQueryableResolver<TEntity, TReturnType, TExecutionContext>)EnumerableFieldResolver;

        public override EnumerableField<TEntity, TReturnType, TExecutionContext> ApplyWhere(Expression<Func<TReturnType, bool>> condition)
        {
            var oldQuery = _query;
            _query = ctx => oldQuery(ctx).Where(condition);
            EnumerableFieldResolver = QueryableFieldResolver.Where(condition);
            return this;
        }

        public virtual QueryableField<TEntity, TReturnType, TExecutionContext> ApplyFilter<TLoaderFilter, TFilter>()
            where TLoaderFilter : Filter<TReturnType, TFilter, TExecutionContext>
            where TFilter : Input
        {
            if (HasFilter)
            {
                throw new InvalidOperationException("Cannot apply filter twice.");
            }

            Registry.RegisterInputAutoObjectGraphType<TFilter>();
            var loaderFilterType = typeof(TLoaderFilter);
            var filter = Registry.ResolveFilter<TReturnType>(loaderFilterType);

            Argument("filter", filter.FilterType);
            return ApplySelector(GetFilteredQuery(filter));
        }

        public QueryableField<TEntity, TReturnType, TExecutionContext> ApplySearch<TSearcher>()
        {
            var searcherBaseType = TypeHelpers.FindMatchingGenericBaseType(typeof(TSearcher), typeof(Searcher<,>));

            if (searcherBaseType == null)
            {
                throw new ArgumentException($"Cannot find the corresponding base type for filter: {typeof(TSearcher)}");
            }

            if (Searcher != null)
            {
                throw new InvalidOperationException("Cannot apply search twice.");
            }

            Searcher = Registry.ResolveSearcher<TReturnType>(typeof(TSearcher));
            Argument("search", typeof(string));
            return ApplySelector(GetSearchQuery(Searcher));
        }

        public virtual QueryableField<TEntity, TReturnType, TExecutionContext> ApplyConnection(Expression<Func<IQueryable<TReturnType>, IOrderedQueryable<TReturnType>>> order)
        {
            return ApplyField(new ConnectionQueryableField<TEntity, TReturnType, TExecutionContext>(Registry, Parent, Name, _query, _condition, ObjectGraphTypeConfigurator, Arguments, order.Compile(), order.GetThenBy().Compile()));
        }

        protected static Func<IResolveFieldContext, IQueryable<TReturnType>> GetQuery(IObjectGraphTypeConfigurator<TReturnType, TExecutionContext> configurator, Func<TExecutionContext, IQueryable<TReturnType>> queryFactory)
        {
            return context =>
            {
                var filter = configurator != null && configurator.HasInlineFilters ? configurator.CreateInlineFilters() : null;
                var ctx = context.GetUserContext<TExecutionContext>();
                var listener = context.GetListener();

                var query = queryFactory(ctx);

                if (filter != null)
                {
                    query = filter.All(listener, query, ctx, context.GetFilterValue(filter.FilterType), context.GetFilterFieldNames());
                }

                var sorters = configurator?.Sorters;

                if (sorters != null)
                {
                    var sorting = context.GetSorting();

                    var fields = sorting
                        .Select(o => (sorters.Single(s => string.Equals(s.Name, o.Field, StringComparison.Ordinal)), o.Direction))
                        .ToArray();

                    if (fields.Any())
                    {
                        query = query.ApplyOrderBy(fields.Select(f => (f.Item1.BuildExpression(context.GetUserContext<TExecutionContext>()), f.Direction)));
                    }
                }

                return query;
            };
        }

        protected override EnumerableField<TEntity, TReturnType1, TExecutionContext> CreateSelect<TReturnType1>(Expression<Func<TReturnType, TReturnType1>> selector, IGraphTypeDescriptor<TReturnType1, TExecutionContext> graphType)
        {
            var queryableField = new QueryableField<TEntity, TReturnType1, TExecutionContext>(
                Registry,
                Parent,
                Name,
                QueryableFieldResolver.Select(selector, graphType.Configurator?.ProxyAccessor),
                graphType,
                graphType?.Configurator,
                Arguments);

            return queryableField;
        }

        protected virtual QueryableField<TEntity, TReturnType, TExecutionContext> ApplySelector(Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> selector)
        {
            EnumerableFieldResolver = QueryableFieldResolver.Select(selector);
            return this;
        }

        private static Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> GetFilteredQuery(IFilter<TReturnType, TExecutionContext> filter)
        {
            return (context, query) =>
            {
                var listener = context.GetListener();
                return filter.All(listener, query, context.GetUserContext<TExecutionContext>(), context.GetFilterValue(filter.FilterType), context.GetFilterFieldNames());
            };
        }

        private static Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> GetSearchQuery(ISearcher<TReturnType, TExecutionContext> searcher)
        {
            return (context, query) =>
            {
                var result = query;
                if (!string.IsNullOrEmpty(context.GetSearch()))
                {
                    result = searcher.All(result, context.GetUserContext<TExecutionContext>(), context.GetSearch());
                }

                return result;
            };
        }

        private static IQueryableResolver<TEntity, TReturnType, TExecutionContext> CreateResolver(
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Func<TExecutionContext, IQueryable<TReturnType>> query,
            Expression<Func<TEntity, TReturnType, bool>> condition,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> elementGraphType,
            IObjectGraphTypeConfigurator<TReturnType, TExecutionContext> configurator)
        {
            if (condition != null)
            {
                return new QueryableAsyncFuncResolver<TEntity, TReturnType, TExecutionContext>(
                    name,
                    context => GetQuery(configurator, query)(context),
                    condition,
                    (ctx, items) => items,
                    parent.ProxyAccessor,
                    elementGraphType.Configurator?.ProxyAccessor);
            }

            return new QueryableFuncResolver<TEntity, TReturnType, TExecutionContext>(
                elementGraphType.Configurator?.ProxyAccessor,
                context => GetQuery(configurator, query)(context),
                (ctx, items) => items);
        }
    }
}
