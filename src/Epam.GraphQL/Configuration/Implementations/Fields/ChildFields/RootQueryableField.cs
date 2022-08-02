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
    internal sealed class RootQueryableField<TReturnType, TExecutionContext> :
        RootQueryableFieldBase<
            RootQueryableField<TReturnType, TExecutionContext>,
            IRootQueryableField<TReturnType, TExecutionContext>,
            TReturnType,
            TExecutionContext>,
        IRootQueryableField<TReturnType, TExecutionContext>
    {
        public RootQueryableField(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            Func<TExecutionContext, IQueryable<TReturnType>> query,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> elementGraphType,
            ISearcher<TReturnType, TExecutionContext>? searcher,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> naturalSorters)
            : base(
                  configurationContext,
                  parent,
                  name,
                  query: ctx => query(ctx.GetUserContext<TExecutionContext>()),
                  transform: (ctx, items) => items,
                  elementGraphType,
                  elementGraphType.Configurator,
                  arguments: null,
                  searcher,
                  naturalSorters)
        {
        }

        public RootQueryableField(
            IChainConfigurationContext configurationContext,
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
                  elementGraphType,
                  configurator,
                  arguments,
                  searcher,
                  naturalSorters)
        {
        }

        public IVoid AsConnection(Expression<Func<IQueryable<TReturnType>, IOrderedQueryable<TReturnType>>> order)
        {
            var connectionField = new RootConnectionQueryableField<TReturnType, TExecutionContext>(
                ConfigurationContext.Chain(nameof(AsConnection)).Argument(order),
                Parent,
                Name,
                QueryableFieldResolver,
                ElementGraphType,
                ObjectGraphTypeConfigurator,
                Arguments,
                Searcher,
                order.GetSorters());
            return ApplyField(connectionField);
        }

        public IVoid AsGroupConnection()
        {
            var connectionField = new RootGroupConnectionQueryableField<TReturnType, TExecutionContext>(
                ConfigurationContext.Chain(nameof(AsGroupConnection)),
                Parent,
                Name,
                QueryableFieldResolver,
                ElementGraphType,
                ObjectGraphTypeConfigurator,
                Arguments,
                Searcher,
                SortingHelpers.Empty);
            return ApplyField(connectionField);
        }

        protected override RootQueryableField<TReturnType, TExecutionContext> ReplaceResolver(IChainConfigurationContext configurationContext, IRootQueryableResolver<TReturnType, TExecutionContext> resolver)
        {
            var queryableField = new RootQueryableField<TReturnType, TExecutionContext>(
                configurationContext,
                Parent,
                Name,
                resolver,
                ElementGraphType,
                ObjectGraphTypeConfigurator,
                Arguments,
                Searcher,
                NaturalSorters);

            return queryableField;
        }
    }
}
