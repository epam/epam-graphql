// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;
using Epam.GraphQL.Search;
using Epam.GraphQL.Sorters.Implementations;
using GraphQL;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ChildFields
{
#pragma warning disable CA1501
    internal class OrderedQueryableField<TEntity, TReturnType, TExecutionContext> : QueryableField<TEntity, TReturnType, TExecutionContext>
#pragma warning restore CA1501
        where TEntity : class
    {
        private readonly Func<IQueryable<TReturnType>, IOrderedQueryable<TReturnType>> _orderBy;
        private readonly Func<IOrderedQueryable<TReturnType>, IOrderedQueryable<TReturnType>> _thenBy;

        protected OrderedQueryableField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> elementGraphType,
            IObjectGraphTypeConfigurator<TReturnType, TExecutionContext> configurator,
            LazyQueryArguments arguments,
            IOrderedQueryableResolver<TEntity, TReturnType, TExecutionContext> resolver,
            Func<IQueryable<TReturnType>, IOrderedQueryable<TReturnType>> orderBy,
            Func<IOrderedQueryable<TReturnType>, IOrderedQueryable<TReturnType>> thenBy)
            : base(
                  registry,
                  parent,
                  name,
                  resolver,
                  elementGraphType,
                  configurator,
                  arguments)
        {
            _orderBy = orderBy ?? throw new ArgumentNullException(nameof(orderBy));
            _thenBy = thenBy ?? throw new ArgumentNullException(nameof(thenBy));
        }

        protected IOrderedQueryableResolver<TEntity, TReturnType, TExecutionContext> OrderedQueryableFieldResolver => (IOrderedQueryableResolver<TEntity, TReturnType, TExecutionContext>)EnumerableFieldResolver;

        public override EnumerableField<TEntity, TReturnType, TExecutionContext> ApplyWhere(Expression<Func<TReturnType, bool>> condition)
        {
            EnumerableFieldResolver = OrderedQueryableFieldResolver.Where(condition);
            return this;
        }

        protected static Func<IResolveFieldContext, IQueryable<TReturnType>, IOrderedQueryable<TReturnType>> ApplySort(
            IObjectGraphTypeConfigurator<TReturnType, TExecutionContext> configurator,
            ISearcher<TReturnType, TExecutionContext> searcher,
            Func<IQueryable<TReturnType>, IOrderedQueryable<TReturnType>> orderBy,
            Func<IOrderedQueryable<TReturnType>, IOrderedQueryable<TReturnType>> thenBy)
        {
            return (context, children) => SortingHelpers.ApplySort<TEntity, TReturnType, TExecutionContext>(
                context,
                children,
                configurator,
                searcher as IOrderedSearcher<TReturnType, TExecutionContext>,
                orderBy,
                thenBy);
        }

        protected override QueryableField<TEntity, TReturnType, TExecutionContext> ApplySelector(Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> selector)
        {
            EnumerableFieldResolver = OrderedQueryableFieldResolver.Select(selector).Reorder(ApplySort(ObjectGraphTypeConfigurator, Searcher, _orderBy, _thenBy));
            return this;
        }
    }
}
