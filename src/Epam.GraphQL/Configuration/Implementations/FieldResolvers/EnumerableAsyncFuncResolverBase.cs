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
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal abstract class EnumerableAsyncFuncResolverBase<TThis, TEntity, TReturnType, TTransformedReturnType, TExecutionContext> :
        IEnumerableResolver<TThis, TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        public EnumerableAsyncFuncResolverBase(
            string fieldName,
            IProxyAccessor<TEntity, TExecutionContext> outerProxyAccessor,
            IProxyAccessor<TReturnType, TTransformedReturnType, TExecutionContext> returnTypeProxyAccessor)
        {
            FieldName = fieldName;
            OuterProxyAccessor = outerProxyAccessor;
            ReturnTypeProxyAccessor = returnTypeProxyAccessor;
        }

        protected IProxyAccessor<TEntity, TExecutionContext> OuterProxyAccessor { get; }

        protected IProxyAccessor<TReturnType, TTransformedReturnType, TExecutionContext> ReturnTypeProxyAccessor { get; }

        protected string FieldName { get; }

        protected abstract Func<IResolveFieldContext, IDataLoader<TEntity, IEnumerable<TTransformedReturnType>>> Resolver { get; }

        protected abstract Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, IEnumerable<TTransformedReturnType>>> ProxiedResolver { get; }

        public object Resolve(IResolveFieldContext context)
        {
            return context.Source is Proxy<TEntity> proxy
                ? ProxiedResolver(context).LoadAsync(proxy)
                : Resolver(context).LoadAsync((TEntity)context.Source);
        }

        public abstract IEnumerableResolver<TEntity, TSelectType, TExecutionContext> Select<TSelectType>(
            Expression<Func<TReturnType, TSelectType>> selector,
            IProxyAccessor<TSelectType, TExecutionContext>? selectTypeProxyAccessor);

        public abstract TThis Where(Expression<Func<TReturnType, bool>> predicate);

        public IFieldResolver SingleOrDefault()
        {
            return new AsyncFuncResolver<TEntity, TTransformedReturnType>(
                ctx => Resolver(ctx)
                    .Then(items => items.SafeNull().SingleOrDefault()),
                ctx => ProxiedResolver(ctx)
                    .Then(items => items.SafeNull().SingleOrDefault()));
        }

        public IFieldResolver FirstOrDefault()
        {
            return new AsyncFuncResolver<TEntity, TTransformedReturnType>(
                ctx => Resolver(ctx)
                    .Then(items => items.SafeNull().FirstOrDefault()),
                ctx => ProxiedResolver(ctx)
                    .Then(items => items.SafeNull().FirstOrDefault()));
        }

        public IEnumerableResolver<TEntity, TSelectType, TExecutionContext> Select<TSelectType>(Expression<Func<TEntity, TReturnType, TSelectType>> selector)
        {
            var factorizationResult = ExpressionHelpers.Factorize(selector);
            AddMembers(factorizationResult);

            var proxiedSelector = new Lazy<Func<Proxy<TEntity>, TTransformedReturnType, TSelectType>>(() =>
            {
                var outerProxyParam = Expression.Parameter(typeof(Proxy<TEntity>));
                var innerProxyParam = Expression.Parameter(typeof(TTransformedReturnType));
                var outers = factorizationResult.LeftExpressions
                    .Select(e => OuterProxyAccessor.Rewrite(e).CastFirstParamTo<Proxy<TEntity>>())
                    .Select(e => e.Body.ReplaceParameter(e.Parameters[0], outerProxyParam))
                    .ToList();

                var inners = factorizationResult.RightExpressions
                    .Select(TransformInnerExpression)
                    .Select(e => e.Body.ReplaceParameter(e.Parameters[0], innerProxyParam))
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

                var exprBody = factorizationResult.Expression.Body.ReplaceParameters(paramMap);
                var expr = Expression.Lambda<Func<Proxy<TEntity>, TTransformedReturnType, TSelectType>>(exprBody, outerProxyParam, innerProxyParam);

                var compiledExpr = expr.Compile();

                return compiledExpr;
            });

            var compiledSelector = new Lazy<Func<TEntity, TTransformedReturnType, TSelectType>>(() =>
            {
                var outerParam = Expression.Parameter(typeof(TEntity));
                var innerProxyParam = Expression.Parameter(typeof(TTransformedReturnType));
                var outers = factorizationResult.LeftExpressions
                    .Select(e => e.Body.ReplaceParameter(e.Parameters[0], outerParam))
                    .ToList();

                var inners = factorizationResult.RightExpressions
                    .Select(TransformInnerExpression)
                    .Select(e => e.Body.ReplaceParameter(e.Parameters[0], innerProxyParam))
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

                var exprBody = factorizationResult.Expression.Body.ReplaceParameters(paramMap);
                var expr = Expression.Lambda<Func<TEntity, TTransformedReturnType, TSelectType>>(exprBody, outerParam, innerProxyParam);

                var compiledExpr = expr.Compile();

                return compiledExpr;
            });

            return new EnumerableAsyncFuncResolver<TEntity, TSelectType, TExecutionContext>(
                FieldName,
                ctx => Resolver(ctx).Then(Continuation),
                ctx => ProxiedResolver(ctx).Then(ProxiedContinuation),
                OuterProxyAccessor);

            IEnumerable<TSelectType> Continuation(TEntity source, IEnumerable<TTransformedReturnType> items)
            {
                return items.Select(item => compiledSelector.Value(source, item)).AsEnumerable();
            }

            IEnumerable<TSelectType> ProxiedContinuation(Proxy<TEntity> source, IEnumerable<TTransformedReturnType> items)
            {
                return items.Select(item => proxiedSelector.Value(source, item)).AsEnumerable();
            }
        }

        protected LambdaExpression TransformInnerExpression(LambdaExpression expression)
        {
            return ReturnTypeProxyAccessor.Rewrite(expression).CastFirstParamTo<TTransformedReturnType>();
        }

        protected void AddMembers(ExpressionFactorizationResult factorizationResult)
        {
            OuterProxyAccessor.AddMembers(FieldName, ReturnTypeProxyAccessor, factorizationResult);
        }

        protected ILoaderHooksExecuter<TTransformedReturnType>? CreateHooksExecuter(IResolveFieldContext context)
        {
            return ReturnTypeProxyAccessor.CreateHooksExecuter(context);
        }
    }
}
