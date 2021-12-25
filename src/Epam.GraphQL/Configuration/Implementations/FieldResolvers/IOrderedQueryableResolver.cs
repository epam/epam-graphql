// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Linq.Expressions;
using GraphQL;

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal interface IOrderedQueryableResolver<TEntity, TReturnType, TExecutionContext> : IQueryableResolver<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        IOrderedQueryableResolver<TEntity, TReturnType, TExecutionContext> Select(
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> selector,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IOrderedQueryable<TReturnType>> sorter);

        IOrderedQueryableResolver<TEntity, TAnotherReturnType, TExecutionContext> Select<TAnotherReturnType>(
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TAnotherReturnType>> selector,
            Func<IResolveFieldContext, IQueryable<TAnotherReturnType>, IOrderedQueryable<TAnotherReturnType>> sorter)
            where TAnotherReturnType : class;

        IResolver<TEntity> Select<TSelectType>(Func<IResolveFieldContext, IOrderedQueryable<TReturnType>, TSelectType> selector);

        new IOrderedQueryableResolver<TEntity, TReturnType, TExecutionContext> Where(Expression<Func<TReturnType, bool>> predicate);

        IResolver<TEntity> AsConnection();
    }
}
