// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Search;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ChildFields
{
    internal abstract class RootLoaderFieldBase<TThis, TThisIntf, TChildLoader, TChildEntity, TExecutionContext> :
        RootQueryableFieldBase<
            TThis,
            TThisIntf,
            TChildEntity,
            TExecutionContext>
        where TChildEntity : class
        where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
        where TThis : RootLoaderFieldBase<TThis, TThisIntf, TChildLoader, TChildEntity, TExecutionContext>, TThisIntf
    {
        protected RootLoaderFieldBase(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType,
            LazyQueryArguments? arguments,
            ISearcher<TChildEntity, TExecutionContext>? searcher,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> naturalSorters)
            : this(
                  configurationContext,
                  parent,
                  name,
                  parent.Registry.ResolveLoader<TChildLoader, TChildEntity>(),
                  elementGraphType,
                  arguments,
                  searcher,
                  naturalSorters)
        {
        }

        protected RootLoaderFieldBase(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            IRootQueryableResolver<TChildEntity, TExecutionContext> resolver,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType,
            LazyQueryArguments? arguments,
            ISearcher<TChildEntity, TExecutionContext>? searcher,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> naturalSorters)
            : this(
                  configurationContext,
                  parent,
                  name,
                  resolver,
                  elementGraphType,
                  parent.Registry.ResolveLoader<TChildLoader, TChildEntity>(),
                  arguments,
                  searcher,
                  naturalSorters)
        {
        }

        protected RootLoaderFieldBase(
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
                  loader.ObjectGraphTypeConfigurator,
                  arguments,
                  searcher,
                  naturalSorters)
        {
            Loader = loader;
        }

        private RootLoaderFieldBase(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            TChildLoader loader,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType,
            LazyQueryArguments? arguments,
            ISearcher<TChildEntity, TExecutionContext>? searcher,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> naturalSorters)
            : base(
                  configurationContext,
                  parent,
                  name,
                  query: context => loader.DoGetBaseQuery(context.GetUserContext<TExecutionContext>()),
                  transform: (ctx, query) => loader.DoApplySecurityFilter(ctx.GetUserContext<TExecutionContext>(), query),
                  elementGraphType,
                  loader.ObjectGraphTypeConfigurator,
                  arguments,
                  searcher,
                  naturalSorters)
        {
            Loader = loader;
        }

        protected TChildLoader Loader { get; }

        public override TThisIntf WithFilter<TLoaderFilter, TFilter>()
        {
            if (HasFilter)
            {
                throw new NotSupportedException($"{typeof(TChildEntity).HumanizedName()}: Simultaneous use of .WithFilter() and .Filterable() is not supported. Consider to use either .WithFilter() or .Filterable().");
            }

            return base.WithFilter<TLoaderFilter, TFilter>();
        }
    }
}
