// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Search;
using Epam.GraphQL.Sorters.Implementations;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ChildFields
{
    internal class QueryableField<TEntity, TReturnType, TExecutionContext> : QueryableFieldBase<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        public QueryableField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Func<TExecutionContext, IQueryable<TReturnType>> query,
            Expression<Func<TEntity, TReturnType, bool>>? condition,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> elementGraphType,
            ISearcher<TReturnType, TExecutionContext>? searcher,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> naturalSorters)
            : base(
                  registry,
                  parent,
                  name,
                  query: ctx => query(ctx.GetUserContext<TExecutionContext>()),
                  transform: (ctx, items) => items,
                  condition,
                  elementGraphType,
                  elementGraphType.Configurator ?? throw new NotSupportedException(),
                  arguments: null,
                  searcher,
                  naturalSorters)
        {
        }

        public QueryableField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IQueryableResolver<TEntity, TReturnType, TExecutionContext> resolver,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> elementGraphType,
            IObjectGraphTypeConfigurator<TReturnType, TExecutionContext>? configurator,
            LazyQueryArguments? arguments,
            ISearcher<TReturnType, TExecutionContext>? searcher,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> naturalSorters)
            : base(
                  registry,
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

        public override QueryableFieldBase<TEntity, TReturnType, TExecutionContext> ApplyConnection(Expression<Func<IQueryable<TReturnType>, IOrderedQueryable<TReturnType>>> order)
        {
            var connectionField = new ConnectionQueryableField<TEntity, TReturnType, TExecutionContext>(
                Registry,
                Parent,
                Name,
                QueryableFieldResolver,
                ElementGraphType,
                ObjectGraphTypeConfigurator ?? throw new NotSupportedException(),
                Arguments,
                Searcher,
                order.GetSorters());
            return ApplyField(connectionField);
        }

        protected override EnumerableFieldBase<TEntity, TReturnType1, TExecutionContext> CreateSelect<TReturnType1>(Expression<Func<TReturnType, TReturnType1>> selector, IGraphTypeDescriptor<TReturnType1, TExecutionContext> graphType)
        {
            var queryableField = new QueryableField<TEntity, TReturnType1, TExecutionContext>(
                Registry,
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

        protected override QueryableFieldBase<TEntity, TReturnType, TExecutionContext> ReplaceResolver(IQueryableResolver<TEntity, TReturnType, TExecutionContext> resolver)
        {
            return new QueryableField<TEntity, TReturnType, TExecutionContext>(
                Registry,
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
