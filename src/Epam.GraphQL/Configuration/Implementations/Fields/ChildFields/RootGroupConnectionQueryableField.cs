// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Search;
using Epam.GraphQL.Sorters;
using Epam.GraphQL.Types;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ChildFields
{
    internal sealed class RootGroupConnectionQueryableField<TReturnType, TExecutionContext> :
        RootQueryableFieldBase<
            RootGroupConnectionQueryableField<TReturnType, TExecutionContext>,
            IConnectionField<TReturnType, TExecutionContext>,
            TReturnType,
            TExecutionContext>,
        IConnectionField<TReturnType, TExecutionContext>,
        IVoid
    {
        private readonly IGraphTypeDescriptor<TExecutionContext> _graphType;

        public RootGroupConnectionQueryableField(
            MethodCallConfigurationContext configurationContext,
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
            _graphType = new GraphTypeDescriptor<TReturnType, TExecutionContext>(
                type: typeof(GroupConnectionGraphType<TReturnType, TExecutionContext>),
                graphTypeFactory: () => new GroupConnectionGraphType<TReturnType, TExecutionContext>(elementGraphType),
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

        public override IFieldResolver Resolver => QueryableFieldResolver.AsGroupConnection(Enumerable.Empty<ISorter<TExecutionContext>>());

        protected override RootGroupConnectionQueryableField<TReturnType, TExecutionContext> ReplaceResolver(
            MethodCallConfigurationContext configurationContext,
            IRootQueryableResolver<TReturnType, TExecutionContext> resolver)
        {
            return new RootGroupConnectionQueryableField<TReturnType, TExecutionContext>(
                configurationContext,
                Parent,
                Name,
                resolver,
                ElementGraphType,
                ObjectGraphTypeConfigurator!,
                Arguments,
                Searcher,
                NaturalSorters!);
        }
    }
}
