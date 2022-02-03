// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Builders.Common;
using Epam.GraphQL.Builders.Common.Implementations;

namespace Epam.GraphQL.Loaders
{
    public abstract class Projection<TEntity, TExecutionContext> : ProjectionBase<TEntity, TExecutionContext>
        where TEntity : class
    {
        protected internal IHasFilterableAndSortableAndGroupable<TEntity, TReturnType> Field<TReturnType>(Expression<Func<TEntity, TReturnType>> expression, string? deprecationReason = null)
           => new FilterableAndSortableAndGroupableFieldBuilder<TEntity, TReturnType, TExecutionContext>(AddField(null, expression, deprecationReason));

        protected internal IHasFilterableAndSortableAndGroupable<TEntity, TReturnType> Field<TReturnType>(string name, Expression<Func<TEntity, TReturnType>> expression, string? deprecationReason = null)
            => new FilterableAndSortableAndGroupableFieldBuilder<TEntity, TReturnType, TExecutionContext>(AddField(name, expression, deprecationReason));

        protected internal IHasFilterableAndSortableAndGroupable<TEntity, TReturnType> Field<TReturnType>(string name, Expression<Func<TExecutionContext, TEntity, TReturnType>> expression, string? deprecationReason = null)
            => new FilterableAndSortableAndGroupableFieldBuilder<TEntity, TReturnType, TExecutionContext>(AddField(name, expression, deprecationReason));

        protected internal void Filter<TValueType>(string name, Func<TValueType, Expression<Func<TEntity, bool>>> filterPredicateFactory)
            => AddFilter(name, filterPredicateFactory);

        protected internal void Filter<TValueType>(string name, Func<TExecutionContext, TValueType, Expression<Func<TEntity, bool>>> filterPredicateFactory)
            => AddFilter(name, filterPredicateFactory);

        protected internal void Sorter<TValueType>(string name, Expression<Func<TEntity, TValueType>> selector)
            => AddSorter(name, selector);
    }
}
