// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal interface IEnumerableResolver<out TThis, TEntity, TReturnType, TExecutionContext> : IFieldResolver
        where TEntity : class
    {
        IEnumerableResolver<TEntity, TSelectType, TExecutionContext> Select<TSelectType>(Expression<Func<TEntity, TReturnType, TSelectType>> selector);

        IEnumerableResolver<TEntity, TSelectType, TExecutionContext> Select<TSelectType>(Expression<Func<TReturnType, TSelectType>> selector, IProxyAccessor<TSelectType, TExecutionContext>? selectTypeProxyAccessor);

        TThis Where(Expression<Func<TReturnType, bool>> predicate);

        IFieldResolver SingleOrDefault();

        IFieldResolver FirstOrDefault();
    }

    internal interface IEnumerableResolver<TEntity, TReturnType, TExecutionContext> : IEnumerableResolver<IEnumerableResolver<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
    }
}
