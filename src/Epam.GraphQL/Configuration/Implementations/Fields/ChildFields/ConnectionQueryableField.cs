// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Collections.Generic;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Search;
using Epam.GraphQL.Types;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ChildFields
{
    internal sealed class ConnectionQueryableField<TEntity, TReturnType, TExecutionContext> :
        QueryableFieldBase<
            ConnectionQueryableField<TEntity, TReturnType, TExecutionContext>,
            IConnectionField<TReturnType, TExecutionContext>,
            TEntity,
            TReturnType,
            TExecutionContext>,
        IConnectionField<TReturnType, TExecutionContext>,
        IVoid
    {
        private readonly IGraphTypeDescriptor<TExecutionContext> _graphType;

        public ConnectionQueryableField(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IQueryableResolver<TEntity, TReturnType, TExecutionContext> resolver,
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
            _graphType = new GraphTypeDescriptor<TReturnType, TExecutionContext>(
                type: typeof(ConnectionGraphType<TReturnType, TExecutionContext>),
                graphTypeFactory: () => new ConnectionGraphType<TReturnType, TExecutionContext>(elementGraphType),
                configurator);

            Argument<string>(
                "after",
                "Only look at connected edges with cursors greater than the value of `after`.");

            Argument<int?>(
                "first",
                "Specifies the number of edges to return starting from `after` or the first entry if `after` is not specified.");

            Argument<string>(
                "before",
                "Only look at connected edges with cursors smaller than the value of `before`.");

            Argument<int?>(
                "last",
                "Specifies the number of edges to return counting reversely from `before`, or the last entry if `before` is not specified.");
        }

        public override IGraphTypeDescriptor<TExecutionContext> GraphType => _graphType;

        public override IFieldResolver Resolver => QueryableFieldResolver.AsConnection();

        protected override ConnectionQueryableField<TEntity, TReturnType, TExecutionContext> ReplaceResolver(
            IChainConfigurationContext configurationContext,
            IQueryableResolver<TEntity, TReturnType, TExecutionContext> resolver)
        {
            return new ConnectionQueryableField<TEntity, TReturnType, TExecutionContext>(
                configurationContext,
                Parent,
                Name,
                resolver,
                ElementGraphType,
                ObjectGraphTypeConfigurator,
                Arguments,
                Searcher,
                NaturalSorters);
        }
    }
}
