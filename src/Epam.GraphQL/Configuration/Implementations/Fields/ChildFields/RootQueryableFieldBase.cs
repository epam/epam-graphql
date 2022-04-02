// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Filters;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Search;
using Epam.GraphQL.Sorters;
using Epam.GraphQL.Sorters.Implementations;
using Epam.GraphQL.Types;
using GraphQL;
using GraphQL.Types;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ChildFields
{
    internal abstract class RootQueryableFieldBase<TThis, TThisIntf, TReturnType, TExecutionContext> :
        RootEnumerableFieldBase<TThis, TReturnType, TExecutionContext>
        where TThis : RootQueryableFieldBase<TThis, TThisIntf, TReturnType, TExecutionContext>, TThisIntf
    {
        protected RootQueryableFieldBase(
            FieldConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            Func<IResolveFieldContext, IQueryable<TReturnType>> query,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> transform,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> elementGraphType,
            IObjectGraphTypeConfigurator<TReturnType, TExecutionContext>? configurator,
            LazyQueryArguments? arguments,
            ISearcher<TReturnType, TExecutionContext>? searcher,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> naturalSorters)
            : this(
                  configurationContext,
                  parent,
                  name,
                  CreateResolver(
                      query,
                      transform,
                      searcher,
                      naturalSorters,
                      configurator),
                  elementGraphType,
                  configurator,
                  arguments,
                  searcher,
                  naturalSorters)
        {
        }

        protected RootQueryableFieldBase(
            FieldConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            IRootQueryableResolver<TReturnType, TExecutionContext> resolver,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> elementGraphType,
            IObjectGraphTypeConfigurator<TReturnType, TExecutionContext>? configurator,
            LazyQueryArguments? arguments,
            ISearcher<TReturnType, TExecutionContext>? searcher,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> naturalSorters)
            : base(
                  configurationContext,
                  parent,
                  name,
                  resolver,
                  elementGraphType)
        {
            Arguments = arguments;
            ObjectGraphTypeConfigurator = configurator;
            NaturalSorters = naturalSorters;

            if (HasFilter)
            {
                Argument("filter", CreateFilterArgument);
            }

            var sortableFields = ObjectGraphTypeConfigurator?.Sorters.Select(f => f.Name).ToArray();
            if (sortableFields != null && sortableFields.Any())
            {
                Argument("sorting", new ListGraphType(new SortingOptionGraphType(typeof(TReturnType).Name, sortableFields)));
            }

            Searcher = searcher;
            if (searcher != null)
            {
                Argument("search", typeof(string));
            }

            QueryArgument CreateFilterArgument()
            {
                Guards.ThrowNotSupportedIf(ObjectGraphTypeConfigurator == null);

                return new QueryArgument(Registry.GenerateInputGraphType(ObjectGraphTypeConfigurator.CreateInlineFilters().FilterType))
                {
                    Name = "filter",
                };
            }
        }

        public virtual bool HasFilter => ObjectGraphTypeConfigurator?.HasInlineFilters ?? false;

        protected IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> NaturalSorters { get; }

        protected ISearcher<TReturnType, TExecutionContext>? Searcher { get; private set; }

        protected IObjectGraphTypeConfigurator<TReturnType, TExecutionContext>? ObjectGraphTypeConfigurator { get; private set; }

        protected virtual IRootQueryableResolver<TReturnType, TExecutionContext> QueryableFieldResolver => QueryableFieldResolverBase.Reorder(ApplySort(ObjectGraphTypeConfigurator?.Sorters, Searcher, NaturalSorters));

        protected IRootQueryableResolver<TReturnType, TExecutionContext> QueryableFieldResolverBase => (IRootQueryableResolver<TReturnType, TExecutionContext>)EnumerableFieldResolver;

        public void Argument<TArgumentType>(string name, string? description = null) => Argument(name, typeof(TArgumentType), description);

        public virtual TThisIntf WithFilter<TLoaderFilter, TFilter>()
            where TLoaderFilter : Filter<TReturnType, TFilter, TExecutionContext>
            where TFilter : Input
        {
            ConfigurationContext.AddErrorIf(HasFilter, "Cannot apply filter twice.");

            Registry.RegisterInputAutoObjectGraphType<TFilter>();
            var loaderFilterType = typeof(TLoaderFilter);
            var filter = Registry.ResolveFilter<TReturnType>(loaderFilterType);

            Argument("filter", filter.FilterType);

            var field = ReplaceResolver(
                ConfigurationContext.NextOperation<TLoaderFilter, TFilter>(nameof(WithFilter)),
                QueryableFieldResolverBase.Select(GetFilteredQuery(filter)));

            return ApplyField(field);
        }

        public TThisIntf WithSearch<TSearcher>()
            where TSearcher : ISearcher<TReturnType, TExecutionContext>
        {
            ConfigurationContext.AddErrorIf(Searcher != null, "Cannot apply search twice.");

            Searcher = Registry.ResolveSearcher<TSearcher, TReturnType>();
            Argument("search", typeof(string));
            var field = ReplaceResolver(
                ConfigurationContext.NextOperation<TSearcher>(nameof(WithSearch)),
                QueryableFieldResolverBase.Select(GetSearchQuery(Searcher)));

            return ApplyField(field);
        }

        public new TThisIntf Where(Expression<Func<TReturnType, bool>> predicate)
        {
            var enumerableField = CreateWhere(
                ConfigurationContext.NextOperation(nameof(Where)).Argument(predicate),
                predicate);
            return ApplyField(enumerableField);
        }

        protected abstract TThis ReplaceResolver(FieldConfigurationContext configurationContext, IRootQueryableResolver<TReturnType, TExecutionContext> resolver);

        protected override TThis CreateWhere(FieldConfigurationContext configurationContext, Expression<Func<TReturnType, bool>> predicate)
        {
            var queryableField = ReplaceResolver(
                configurationContext,
                QueryableFieldResolverBase.Where(predicate));
            return queryableField;
        }

        protected override RootEnumerableFieldBase<TReturnType1, TExecutionContext> CreateSelect<TReturnType1>(
            FieldConfigurationContext configurationContext,
            Expression<Func<TReturnType, TReturnType1>> selector,
            IGraphTypeDescriptor<TReturnType1, TExecutionContext> graphType)
        {
            var queryableField = new RootQueryableField<TReturnType1, TExecutionContext>(
                configurationContext,
                Parent,
                Name,
                QueryableFieldResolver.Select(selector, graphType.Configurator?.ProxyAccessor),
                graphType,
                graphType.Configurator,
                Arguments,
                searcher: null,
                naturalSorters: SortingHelpers.Empty);

            return queryableField;
        }

        private static IRootQueryableResolver<TReturnType, TExecutionContext> CreateResolver(
            Func<IResolveFieldContext, IQueryable<TReturnType>> query,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> transform,
            ISearcher<TReturnType, TExecutionContext>? searcher,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> naturalSorters,
            IObjectGraphTypeConfigurator<TReturnType, TExecutionContext>? configurator)
        {
            var sorters = configurator?.Sorters;

            if (configurator != null)
            {
                return new QueryableFuncResolver<TReturnType, Proxy<TReturnType>, TExecutionContext>(
                    configurator.ProxyAccessor,
                    GetQuery(configurator, query),
                    transform,
                    ApplySort(sorters, searcher, naturalSorters));
            }

            return new QueryableFuncResolver<TReturnType, TReturnType, TExecutionContext>(
                IdentityProxyAccessor<TReturnType, TExecutionContext>.Instance,
                GetQuery(null, query),
                transform,
                ApplySort(sorters, searcher, naturalSorters));

            static Func<IResolveFieldContext, IQueryable<TReturnType>> GetQuery(
                IObjectGraphTypeConfigurator<TReturnType, TExecutionContext>? configurator,
                Func<IResolveFieldContext, IQueryable<TReturnType>> queryFactory)
            {
                return context =>
                {
                    var filter = configurator != null && configurator.HasInlineFilters ? configurator.CreateInlineFilters() : null;

                    var query = queryFactory(context);

                    if (filter != null)
                    {
                        var listener = context.GetListener();
                        var ctx = context.GetUserContext<TExecutionContext>();
                        query = filter.All(listener, query, ctx, context.GetFilterValue(filter.FilterType));
                    }

                    return query;
                };
            }
        }

        private static Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> GetFilteredQuery(IFilter<TReturnType, TExecutionContext> filter)
        {
            return (context, query) =>
            {
                var listener = context.GetListener();
                return filter.All(listener, query, context.GetUserContext<TExecutionContext>(), context.GetFilterValue(filter.FilterType));
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

        private static Func<IResolveFieldContext, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>> ApplySort(
            IReadOnlyList<ISorter<TExecutionContext>>? sorters,
            ISearcher<TReturnType, TExecutionContext>? searcher,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> naturalSorters)
        {
            return context => SortingHelpers.GetSort(
                context,
                sorters,
                searcher as IOrderedSearcher<TReturnType, TExecutionContext>,
                naturalSorters);
        }
    }
}
