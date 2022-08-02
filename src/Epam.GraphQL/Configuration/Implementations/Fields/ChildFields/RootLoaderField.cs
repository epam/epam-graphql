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
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Search;
using Epam.GraphQL.Sorters.Implementations;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ChildFields
{
    internal sealed class RootLoaderField<TChildLoader, TChildEntity, TExecutionContext> :
        RootLoaderFieldBase<
            RootLoaderField<TChildLoader, TChildEntity, TExecutionContext>,
            IRootLoaderField<TChildEntity, TExecutionContext>,
            TChildLoader,
            TChildEntity,
            TExecutionContext>,
        IRootLoaderField<TChildEntity, TExecutionContext>
        where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
    {
        public RootLoaderField(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType,
            LazyQueryArguments? arguments,
            ISearcher<TChildEntity, TExecutionContext>? searcher,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> naturalSorters)
            : base(
                  configurationContext,
                  parent,
                  name,
                  elementGraphType,
                  arguments,
                  searcher,
                  naturalSorters)
        {
        }

        private RootLoaderField(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            IRootQueryableResolver<TChildEntity, TExecutionContext> resolver,
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
            var connectionField = new RootConnectionLoaderField<TChildLoader, TChildEntity, TExecutionContext>(
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

        public IVoid AsConnection()
        {
            var connectionField = new RootConnectionLoaderField<TChildLoader, TChildEntity, TExecutionContext>(
                ConfigurationContext.Chain(nameof(AsConnection)),
                Parent,
                Name,
                QueryableFieldResolver,
                ElementGraphType,
                Arguments,
                Searcher,
#pragma warning disable CS0618 // Type or member is obsolete
                Loader.ApplyNaturalOrderBy(Enumerable.Empty<TChildEntity>().AsQueryable()).GetSorters());
#pragma warning restore CS0618 // Type or member is obsolete
            return ApplyField(connectionField);
        }

        public IVoid AsGroupConnection()
        {
            var connectionField = new RootGroupConnectionLoaderField<TChildLoader, TChildEntity, TExecutionContext>(
                ConfigurationContext.Chain(nameof(AsGroupConnection)),
                Parent,
                Name,
                QueryableFieldResolver,
                ElementGraphType,
                Arguments,
                Searcher,
                SortingHelpers.Empty);
            return ApplyField(connectionField);
        }

        protected override RootLoaderField<TChildLoader, TChildEntity, TExecutionContext> ReplaceResolver(IChainConfigurationContext configurationContext, IRootQueryableResolver<TChildEntity, TExecutionContext> resolver)
        {
            var queryableField = new RootLoaderField<TChildLoader, TChildEntity, TExecutionContext>(
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
}
