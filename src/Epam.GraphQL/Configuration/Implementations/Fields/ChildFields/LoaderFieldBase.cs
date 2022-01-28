// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Search;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ChildFields
{
    internal abstract class LoaderFieldBase<TThis, TThisIntf, TEntity, TChildLoader, TChildEntity, TExecutionContext> :
        QueryableFieldBase<
            TThis,
            TThisIntf,
            TEntity,
            TChildEntity,
            TExecutionContext>
        where TEntity : class
        where TChildEntity : class
        where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
        where TThis : LoaderFieldBase<TThis, TThisIntf, TEntity, TChildLoader, TChildEntity, TExecutionContext>, TThisIntf
    {
        protected LoaderFieldBase(
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TChildEntity, bool>>? condition,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType,
            LazyQueryArguments? arguments,
            ISearcher<TChildEntity, TExecutionContext>? searcher,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> naturalSorters)
            : this(
                  parent,
                  name,
                  parent.Registry.ResolveLoader<TChildLoader, TChildEntity>(),
                  condition,
                  elementGraphType,
                  arguments,
                  searcher,
                  naturalSorters)
        {
        }

        protected LoaderFieldBase(
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IQueryableResolver<TEntity, TChildEntity, TExecutionContext> resolver,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType,
            LazyQueryArguments? arguments,
            ISearcher<TChildEntity, TExecutionContext>? searcher,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> naturalSorters)
            : this(
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

        protected LoaderFieldBase(
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IQueryableResolver<TEntity, TChildEntity, TExecutionContext> resolver,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType,
            TChildLoader loader,
            LazyQueryArguments? arguments,
            ISearcher<TChildEntity, TExecutionContext>? searcher,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> naturalSorters)
            : base(
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

        private LoaderFieldBase(
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            TChildLoader loader,
            Expression<Func<TEntity, TChildEntity, bool>>? condition,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType,
            LazyQueryArguments? arguments,
            ISearcher<TChildEntity, TExecutionContext>? searcher,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> naturalSorters)
            : base(
                  parent,
                  name,
                  query: context => loader.DoGetBaseQuery(context.GetUserContext<TExecutionContext>()),
                  transform: (ctx, query) => loader.DoApplySecurityFilter(ctx.GetUserContext<TExecutionContext>(), query),
                  condition,
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
