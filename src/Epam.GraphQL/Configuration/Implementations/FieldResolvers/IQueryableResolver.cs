// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Helpers;
using GraphQL;

#nullable enable

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal interface IQueryableResolver<TEntity, TReturnType, TExecutionContext> : IEnumerableResolver<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        new IQueryableResolver<TEntity, TSelectType, TExecutionContext> Select<TSelectType>(Expression<Func<TReturnType, TSelectType>> selector, IProxyAccessor<TSelectType, TExecutionContext>? selectTypeProxyAccessor);

        // For filter and search
        IQueryableResolver<TEntity, TReturnType, TExecutionContext> Select(Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> selector);

        new IQueryableResolver<TEntity, TReturnType, TExecutionContext> Where(Expression<Func<TReturnType, bool>> predicate);

        IQueryableResolver<TEntity, TReturnType, TExecutionContext> Reorder(Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> sorter);

        IResolver<TEntity> AsGroupConnection(
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<Proxy<TReturnType>>> selector,
            Func<IResolveFieldContext, IQueryable<Proxy<TReturnType>>, IQueryable<Proxy<TReturnType>>> sorter);

        IResolver<TEntity> AsConnection();
    }
}
