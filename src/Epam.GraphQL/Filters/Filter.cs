// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Infrastructure;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Filters
{
    public abstract class Filter<TEntity, TFilter, TExecutionContext> : IFilter<TEntity, TExecutionContext>
        where TFilter : Input
    {
        Type IFilter<TEntity, TExecutionContext>.FilterType => typeof(TFilter);

        IQueryable<TEntity> IFilter<TEntity, TExecutionContext>.All(ISchemaExecutionListener listener, IQueryable<TEntity> query, TExecutionContext context, object filter) =>
            All(listener, query, context, (TFilter)filter);

        public override int GetHashCode() => GetType().GetHashCode();

        public override bool Equals(object obj) => Equals(obj as IFilter<TEntity, TExecutionContext>);

        public bool Equals(IFilter<TEntity, TExecutionContext>? other) => other != null
            && other.GetType() == GetType();

        protected abstract IQueryable<TEntity> ApplyFilter(TExecutionContext context, IQueryable<TEntity> query, TFilter filter);

        private IQueryable<TEntity> All(ISchemaExecutionListener listener, IQueryable<TEntity> query, TExecutionContext context, TFilter filter)
        {
            var predicate = listener.GetAdditionalFilter<TExecutionContext, TEntity, TFilter>(context, filter);
            query = query.SafeWhere(predicate);
            query = ApplyFilter(context, query, filter);

            return query;
        }
    }
}
