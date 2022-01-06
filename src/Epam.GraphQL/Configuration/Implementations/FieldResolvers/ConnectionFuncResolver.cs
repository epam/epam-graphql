// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using Epam.GraphQL.Extensions;
using GraphQL;

#nullable enable

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal class ConnectionFuncResolver<TEntity, TReturnType, TExecutionContext> : QueryableFuncResolver<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        public ConnectionFuncResolver(
            IProxyAccessor<TReturnType, TExecutionContext>? proxyAccessor,
            Func<IResolveFieldContext, IQueryable<TReturnType>> resolver,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> transform,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>>? sorter)
            : base(proxyAccessor, resolver, transform, sorter)
        {
        }

        protected override QueryableFuncResolver<TEntity, TAnotherReturnType, TExecutionContext> Create<TAnotherReturnType>(
            IProxyAccessor<TAnotherReturnType, TExecutionContext>? proxyAccessor,
            Func<IResolveFieldContext, IQueryable<TAnotherReturnType>> resolver,
            Func<IResolveFieldContext, IQueryable<TAnotherReturnType>, IQueryable<TAnotherReturnType>> transform,
            Func<IResolveFieldContext, IQueryable<TAnotherReturnType>, IQueryable<TAnotherReturnType>>? sorter)
        {
            return new ConnectionFuncResolver<TEntity, TAnotherReturnType, TExecutionContext>(
                proxyAccessor,
                resolver,
                transform,
                sorter ?? throw new ArgumentNullException(nameof(sorter)));
        }

        protected override IEnumerable<string> GetQueriedFields(IResolveFieldContext context)
        {
            return context.GetConnectionQueriedFields();
        }
    }
}
