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
        private readonly string _fieldName;
        private readonly IProxyAccessor<TEntity, TExecutionContext> _outerProxyAccessor;

        public EnumerableAsyncFuncResolver(
            string fieldName,
            Func<IResolveFieldContext, IDataLoader<TEntity, IEnumerable<TReturnType>?>> resolver,
            Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, IEnumerable<TReturnType>?>> proxiedResolver,
            IProxyAccessor<TEntity, TExecutionContext> outerProxyAccessor)
            : base(resolver, proxiedResolver)
        {
            _fieldName = fieldName;
            _outerProxyAccessor = outerProxyAccessor;
        }

        public IEnumerableResolver<TEntity, TSelectType, TExecutionContext> Select<TSelectType>(Expression<Func<TEntity, TReturnType, TSelectType>> selector)
        {
            var factorizationResult = ExpressionHelpers.Factorize(selector);
            _outerProxyAccessor.AddMembers(_fieldName, factorizationResult.LeftExpressions);

            var proxiedSelector = new Lazy<Func<Proxy<TEntity>, TReturnType, TSelectType>>(() =>
            {
                var outerProxyParam = Expression.Parameter(typeof(Proxy<TEntity>));
                var innerParam = Expression.Parameter(typeof(TReturnType));
                var outers = factorizationResult.LeftExpressions
                    .Select(e => _outerProxyAccessor.GetProxyExpression(e).CastFirstParamTo<Proxy<TEntity>>())
                    .Select(e => e.Body.ReplaceParameter(e.Parameters[0], outerProxyParam))
                    .ToList();

                var inners = factorizationResult.RightExpressions
                    .Select(e => e.Body.ReplaceParameter(e.Parameters[0], innerParam))
                    .ToList();

                var paramMap = new Dictionary<ParameterExpression, Expression>();

                var parameters = factorizationResult.Expression.Parameters;

                for (int i = 0; i < outers.Count; i++)
                {
                    paramMap.Add(parameters[i], outers[i]);
                }

                for (int i = 0; i < inners.Count; i++)
                {
                    paramMap.Add(parameters[i + outers.Count], inners[i]);
                }

                var exprBody = ExpressionHelpers.ParameterRebinder.ReplaceParameters(paramMap, factorizationResult.Expression.Body);
                var expr = Expression.Lambda<Func<Proxy<TEntity>, TReturnType, TSelectType>>(exprBody, outerProxyParam, innerParam);

                var compiledExpr = expr.Compile();

                return compiledExpr;
            });

            var compiledSelector = selector.Compile();

            return new EnumerableAsyncFuncResolver<TEntity, TSelectType, TExecutionContext>(
                _fieldName,
                ctx => Resolver(ctx).Then(
                    (src, arg) => arg?.Select(item => compiledSelector(src, item))),
                ctx => ProxiedResolver(ctx).Then(
                    (src, arg) => arg?.Select(item => proxiedSelector.Value(src, item))),
                _outerProxyAccessor);
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
                _fieldName,
                ctx => Resolver(ctx).Then(arg => arg?.AsQueryable().Where(predicate).AsEnumerable()),
                ctx => ProxiedResolver(ctx).Then(arg => arg?.AsQueryable().Where(predicate).AsEnumerable()),
                _outerProxyAccessor);
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
