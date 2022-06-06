// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.TaskBatcher;
using GraphQL;
using GraphQL.DataLoader;

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal class EnumerableAsyncFuncResolver<TEntity, TReturnType, TExecutionContext> :
        EnumerableAsyncFuncResolverBase<IEnumerableResolver<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TReturnType, TExecutionContext>,
        IEnumerableResolver<TEntity, TReturnType, TExecutionContext>
    {
        public EnumerableAsyncFuncResolver(
            string fieldName,
            Func<IResolveFieldContext, IDataLoader<TEntity, IEnumerable<TReturnType>>> resolver,
            Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, IEnumerable<TReturnType>>> proxiedResolver,
            IProxyAccessor<TEntity, TExecutionContext> outerProxyAccessor)
            : base(fieldName, outerProxyAccessor, IdentityProxyAccessor<TReturnType, TExecutionContext>.Instance)
        {
            Resolver = resolver;
            ProxiedResolver = proxiedResolver;
        }

        protected override Func<IResolveFieldContext, IDataLoader<TEntity, IEnumerable<TReturnType>>> Resolver { get; }

        protected override Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, IEnumerable<TReturnType>>> ProxiedResolver { get; }

        public override IEnumerableResolver<TEntity, TSelectType, TExecutionContext> Select<TSelectType>(Expression<Func<TReturnType, TSelectType>> selector, IProxyAccessor<TSelectType, TExecutionContext>? selectTypeProxyAccessor)
        {
            var entityParam = Expression.Parameter(typeof(TEntity));
            var lambda = Expression.Lambda<Func<TEntity, TReturnType, TSelectType>>(selector.Body, entityParam, selector.Parameters[0]);

            return Select(lambda);
        }

        public override IEnumerableResolver<TEntity, TReturnType, TExecutionContext> Where(Expression<Func<TReturnType, bool>> predicate)
        {
            return new EnumerableAsyncFuncResolver<TEntity, TReturnType, TExecutionContext>(
                FieldName,
                ctx => Resolver(ctx).Then(arg => arg.AsQueryable().Where(predicate).AsEnumerable()),
                ctx => ProxiedResolver(ctx).Then(arg => arg.AsQueryable().Where(predicate).AsEnumerable()),
                OuterProxyAccessor);
        }
    }
}
