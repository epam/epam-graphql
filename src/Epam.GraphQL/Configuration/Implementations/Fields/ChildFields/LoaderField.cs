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
using Epam.GraphQL.Enums;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Search;
using Epam.GraphQL.Sorters.Implementations;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ChildFields
{
    internal class LoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext> :
        LoaderFieldBase<
            LoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>,
            ILoaderField<TEntity, TChildEntity, TExecutionContext>,
            TEntity,
            TChildLoader,
            TChildEntity,
            TExecutionContext>,
        ILoaderField<TEntity, TChildEntity, TExecutionContext>
        where TChildEntity : class
        where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
    {
        public LoaderField(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType,
            LazyQueryArguments? arguments,
            ISearcher<TChildEntity, TExecutionContext>? searcher,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> naturalSorters)
            : base(
                  configurationContext,
                  parent,
                  name,
                  condition,
                  elementGraphType,
                  arguments,
                  searcher,
                  naturalSorters)
        {
        }

        private LoaderField(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IQueryableResolver<TEntity, TChildEntity, TExecutionContext> resolver,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType,
            TChildLoader loader,
            LazyQueryArguments? arguments,
            ISearcher<TChildEntity, TExecutionContext>? searcher,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> naturalSorters)
            : base(
                  configurationContext,
                  parent,
                  name,
                  resolver,
                  elementGraphType,
                  loader,
                  arguments,
                  searcher,
                  naturalSorters)
        {
        }

        public IVoid AsConnection(Expression<Func<IQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>>> naturalOrder)
        {
            var connectionField = new ConnectionLoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>(
                ConfigurationContext.Chain(nameof(AsConnection)).Argument(naturalOrder),
                Parent,
                Name,
                QueryableFieldResolver,
                ElementGraphType,
                Arguments,
                Searcher,
                naturalOrder.GetSorters());
            return ApplyField(connectionField);
        }

        public IConnectionField AsConnection()
        {
            var connectionField = new ConnectionLoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>(
                ConfigurationContext.Chain(nameof(AsConnection)),
                Parent,
                Name,
                QueryableFieldResolver,
                ElementGraphType,
                Arguments,
                Searcher,
                Loader.ApplyNaturalOrderBy(Enumerable.Empty<TChildEntity>().AsQueryable()).GetSorters());
            return ApplyField(connectionField);
        }

        protected override LoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext> ReplaceResolver(
            IChainConfigurationContext configurationContext,
            IQueryableResolver<TEntity, TChildEntity, TExecutionContext> resolver)
        {
            var queryableField = new LoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>(
                configurationContext,
                Parent,
                Name,
                resolver,
                ElementGraphType,
                Loader,
                Arguments,
                Searcher,
                NaturalSorters);

            return queryableField;
        }
    }

    internal sealed class LoaderField<TLoader, TChildLoader, TEntity, TChildEntity, TExecutionContext> : LoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>
        where TLoader : Loader<TEntity, TExecutionContext>, new()
        where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
        where TChildEntity : class
    {
        public LoaderField(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            RelationType relationType,
            Expression<Func<TChildEntity, TEntity>>? navigationProperty,
            Expression<Func<TEntity, TChildEntity>>? reverseNavigationProperty,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType)
            : base(configurationContext, parent, name, condition, elementGraphType, arguments: null, searcher: null, naturalSorters: SortingHelpers.Empty)
        {
            parent.Registry.Register(typeof(TChildLoader), typeof(TLoader), condition.SwapOperands(), reverseNavigationProperty, navigationProperty, relationType);
        }
    }
}
