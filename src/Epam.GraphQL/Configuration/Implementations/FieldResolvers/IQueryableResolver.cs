// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Loaders;
using GraphQL;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal interface IQueryableResolver<TEntity, TReturnType, TExecutionContext> : IEnumerableResolver<IQueryableResolver<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        // For filter and search
        IQueryableResolver<TEntity, TReturnType, TExecutionContext> Select(Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> selector);

        IQueryableResolver<TEntity, TReturnType, TExecutionContext> Reorder(Func<IResolveFieldContext, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>> sorters);

        IFieldResolver AsConnection();
    }
}
