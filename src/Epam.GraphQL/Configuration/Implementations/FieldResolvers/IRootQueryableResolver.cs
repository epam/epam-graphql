// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Sorters;
using GraphQL;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal interface IRootQueryableResolver<TReturnType, TExecutionContext> : IRootEnumerableResolver<TReturnType, TExecutionContext>
    {
        IRootQueryableResolver<TSelectType, TExecutionContext> Select<TSelectType>(Expression<Func<TReturnType, TSelectType>> selector, IProxyAccessor<TSelectType, TExecutionContext>? selectTypeProxyAccessor);

        // For filter and search
        IRootQueryableResolver<TReturnType, TExecutionContext> Select(Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> selector);

        IRootQueryableResolver<TReturnType, TExecutionContext> Where(Expression<Func<TReturnType, bool>> predicate);

        IRootQueryableResolver<TReturnType, TExecutionContext> Reorder(Func<IResolveFieldContext, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>> sorters);

        IFieldResolver AsGroupConnection(IEnumerable<ISorter<TExecutionContext>> sorters);

        IFieldResolver AsConnection();
    }
}
