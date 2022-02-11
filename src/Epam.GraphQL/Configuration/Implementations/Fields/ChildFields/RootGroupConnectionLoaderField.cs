// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Collections.Generic;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Search;
using Epam.GraphQL.Types;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ChildFields
{
    internal sealed class RootGroupConnectionLoaderField<TChildLoader, TChildEntity, TExecutionContext> :
        RootConnectionLoaderFieldBase<
            RootGroupConnectionLoaderField<TChildLoader, TChildEntity, TExecutionContext>,
            TChildLoader,
            TChildEntity,
            TExecutionContext>
        where TChildEntity : class
        where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
    {
        private readonly IGraphTypeDescriptor<TExecutionContext> _graphType;

        public RootGroupConnectionLoaderField(
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            IRootQueryableResolver<TChildEntity, TExecutionContext> resolver,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType,
            LazyQueryArguments? arguments,
            ISearcher<TChildEntity, TExecutionContext>? searcher,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> naturalSorters)
            : base(
                  parent,
                  name,
                  resolver,
                  elementGraphType,
                  arguments,
                  searcher,
                  naturalSorters)
        {
            _graphType = new GraphTypeDescriptor<TExecutionContext>(
                type: typeof(GroupConnectionGraphType<TChildEntity, TExecutionContext>),
                graphTypeFactory: () => new GroupConnectionGraphType<TChildEntity, TExecutionContext>(elementGraphType),
                configurator: elementGraphType.Configurator);
        }

        public override IFieldResolver Resolver => QueryableFieldResolver
            .AsGroupConnection(Loader.ObjectGraphTypeConfigurator.Sorters);

        public override IGraphTypeDescriptor<TExecutionContext> GraphType => _graphType;

        protected override RootGroupConnectionLoaderField<TChildLoader, TChildEntity, TExecutionContext> ReplaceResolver(IRootQueryableResolver<TChildEntity, TExecutionContext> resolver)
        {
            return new RootGroupConnectionLoaderField<TChildLoader, TChildEntity, TExecutionContext>(
                Parent,
                Name,
                resolver,
                ElementGraphType,
                Arguments,
                Searcher,
                NaturalSorters!);
        }
    }
}
