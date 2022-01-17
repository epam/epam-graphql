// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.TaskBatcher;
using GraphQL;
using GraphQL.DataLoader;

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal class EnumerableAsyncFuncResolver<TEntity, TReturnType, TExecutionContext> : AsyncFuncResolver<TEntity, IEnumerable<TReturnType>?>, IEnumerableResolver<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        public EnumerableAsyncFuncResolver(
            Func<IResolveFieldContext, IDataLoader<TEntity, IEnumerable<TReturnType>?>> resolver,
            Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, IEnumerable<TReturnType>?>> proxiedResolver)
            : base(resolver, proxiedResolver)
        {
        }

        public IEnumerableResolver<TEntity, TSelectType, TExecutionContext> Select<TSelectType>(Expression<Func<TEntity, TReturnType, TSelectType>> selector)
        {
            var compiledSelector = selector.Compile();
            return new EnumerableAsyncFuncResolver<TEntity, TSelectType, TExecutionContext>(
                ctx => Resolver(ctx).Then(
                    (src, arg) => arg?.AsQueryable().Select(item => compiledSelector(src, item)).AsEnumerable()),
                ctx => ProxiedResolver(ctx).Then(
                    (src, arg) => arg?.AsQueryable().Select(item => compiledSelector(src.GetOriginal(), item)).AsEnumerable()));
        }

        public IEnumerableResolver<TEntity, TSelectType, TExecutionContext> Select<TSelectType>(Expression<Func<TReturnType, TSelectType>> selector, IProxyAccessor<TSelectType, TExecutionContext>? selectTypeProxyAccessor)
        {
            var entityParam = Expression.Parameter(typeof(TEntity));
            var lambda = Expression.Lambda<Func<TEntity, TReturnType, TSelectType>>(selector.Body, entityParam, selector.Parameters[0]);

            return Select(lambda);
        }

        public IEnumerableResolver<TEntity, TReturnType, TExecutionContext> Where(Expression<Func<TReturnType, bool>> predicate)
        {
            return new EnumerableAsyncFuncResolver<TEntity, TReturnType, TExecutionContext>(
                ctx => Resolver(ctx).Then(arg => arg?.AsQueryable().Where(predicate).AsEnumerable()),
                ctx => ProxiedResolver(ctx).Then(arg => arg?.AsQueryable().Where(predicate).AsEnumerable()));
        }

        public IResolver<TEntity> SingleOrDefault()
        {
            return new AsyncFuncResolver<TEntity, TReturnType>(
                ctx => Resolver(ctx).Then(items => items.SafeNull().SingleOrDefault()),
                ctx => ProxiedResolver(ctx).Then(items => items.SafeNull().SingleOrDefault()));
        }

        public IResolver<TEntity> FirstOrDefault()
        {
            return new AsyncFuncResolver<TEntity, TReturnType>(
                ctx => Resolver(ctx).Then(items => items.SafeNull().FirstOrDefault()),
                ctx => ProxiedResolver(ctx).Then(items => items.SafeNull().FirstOrDefault()));
        }
    }
}
