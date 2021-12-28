// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Extensions;
using GraphQL;

#nullable enable

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal class ConnectionAsyncFuncResolver<TEntity, TReturnType, TExecutionContext> : QueryableAsyncFuncResolver<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        public ConnectionAsyncFuncResolver(
            string fieldName,
            Func<IResolveFieldContext, IQueryable<TReturnType>> resolver,
            Expression<Func<TEntity, TReturnType, bool>> condition,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> transform,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IOrderedQueryable<TReturnType>>? sorter,
            IProxyAccessor<TEntity, TExecutionContext> outerProxyAccessor,
            IProxyAccessor<TReturnType, TExecutionContext> innerProxyAccessor)
            : base(fieldName, resolver, condition, transform, sorter, outerProxyAccessor, innerProxyAccessor)
        {
        }

        protected override IQueryableResolver<TEntity, TReturnType, TExecutionContext> Create(
            string fieldName,
            Func<IResolveFieldContext, IQueryable<TReturnType>> resolver,
            Expression<Func<TEntity, TReturnType, bool>> condition,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> transform,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IOrderedQueryable<TReturnType>>? sorter,
            IProxyAccessor<TEntity, TExecutionContext> outerProxyAccessor,
            IProxyAccessor<TReturnType, TExecutionContext> innerProxyAccessor)
        {
            return new ConnectionAsyncFuncResolver<TEntity, TReturnType, TExecutionContext>(
                fieldName,
                resolver,
                condition,
                transform,
                sorter ?? throw new ArgumentNullException(nameof(sorter)),
                outerProxyAccessor,
                innerProxyAccessor);
        }

        protected override IEnumerable<string> GetQueriedFields(IResolveFieldContext context)
        {
            return context.GetConnectionQueriedFields();
        }
    }
}
