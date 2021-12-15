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

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal class EnumerableFuncResolver<TEntity, TReturnType, TExecutionContext> : FuncResolver<TEntity, IEnumerable<TReturnType>>, IEnumerableResolver<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        public EnumerableFuncResolver(Func<IResolveFieldContext, TEntity, IEnumerable<TReturnType>> resolver)
            : base(resolver)
        {
        }

        public IEnumerableResolver<TEntity, TSelectType, TExecutionContext> Select<TSelectType>(Expression<Func<TEntity, TReturnType, TSelectType>> selector)
        {
            var compiledSelector = selector.Compile();
            return new EnumerableFuncResolver<TEntity, TSelectType, TExecutionContext>((ctx, src) => Resolver(ctx, src)?.Select(arg => compiledSelector(src, arg)));
        }

        public IEnumerableResolver<TEntity, TSelectType, TExecutionContext> Select<TSelectType>(Expression<Func<TReturnType, TSelectType>> selector, IProxyAccessor<TSelectType, TExecutionContext> selectTypeProxyAccessor)
        {
            var entityParam = Expression.Parameter(typeof(TEntity));
            var lambda = Expression.Lambda<Func<TEntity, TReturnType, TSelectType>>(selector.Body, entityParam, selector.Parameters[0]);

            return Select(lambda);
        }

        public IEnumerableResolver<TEntity, TReturnType, TExecutionContext> Where(Expression<Func<TReturnType, bool>> predicate)
        {
            return new EnumerableFuncResolver<TEntity, TReturnType, TExecutionContext>((ctx, src) => Resolver(ctx, src)?.AsQueryable().Where(predicate));
        }

        public IResolver<TEntity> SingleOrDefault()
        {
            return new FuncResolver<TEntity, TReturnType>((ctx, src) => Resolver(ctx, src).SafeNull().SingleOrDefault());
        }

        public IResolver<TEntity> FirstOrDefault()
        {
            return new FuncResolver<TEntity, TReturnType>((ctx, src) => Resolver(ctx, src).SafeNull().FirstOrDefault());
        }
    }
}
