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
    internal sealed class RootConnectionLoaderField<TChildLoader, TChildEntity, TExecutionContext> :
        RootConnectionLoaderFieldBase<
            RootConnectionLoaderField<TChildLoader, TChildEntity, TExecutionContext>,
            TChildLoader,
            TChildEntity,
            TExecutionContext>,
        IConnectionField<TChildEntity, TExecutionContext>,
        IConnectionField,
        IVoid
        where TChildEntity : class
        where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
    {
        private readonly IGraphTypeDescriptor<TExecutionContext> _graphType;

        public RootConnectionLoaderField(
            FieldConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            IRootQueryableResolver<TChildEntity, TExecutionContext> resolver,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType,
            LazyQueryArguments? arguments,
            ISearcher<TChildEntity, TExecutionContext>? searcher,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> naturalSorters)
            : base(
                  configurationContext,
                  parent,
                  name,
                  resolver,
                  elementGraphType,
                  arguments,
                  searcher,
                  naturalSorters)
        {
            _graphType = GraphTypeDescriptor.Create<ConnectionGraphType<TChildLoader, TChildEntity, TExecutionContext>, TExecutionContext>();
        }

        public override IGraphTypeDescriptor<TExecutionContext> GraphType => _graphType;

        public override IFieldResolver Resolver => QueryableFieldResolver.AsConnection();

        protected override RootConnectionLoaderField<TChildLoader, TChildEntity, TExecutionContext> ReplaceResolver(
            FieldConfigurationContext configurationContext,
            IRootQueryableResolver<TChildEntity, TExecutionContext> resolver)
        {
            return new RootConnectionLoaderField<TChildLoader, TChildEntity, TExecutionContext>(
                configurationContext,
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
