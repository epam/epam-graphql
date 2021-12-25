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
    // TODO Make base class for queryable resolvers, move Transform to this class
    internal class QueryableAsyncFuncResolver<TEntity, TReturnType, TExecutionContext> : IQueryableResolver<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        private readonly string _fieldName;
        private readonly Expression<Func<TEntity, TReturnType, bool>> _condition;
        private readonly IProxyAccessor<TEntity, TExecutionContext> _outerProxyAccessor;
        private readonly IProxyAccessor<TReturnType, TExecutionContext> _innerProxyAccessor;
        private readonly Func<IResolveFieldContext, IQueryable<TReturnType>> _resolver;
        private readonly Func<IResolveFieldContext, IDataLoader<TEntity, IGrouping<TEntity, Proxy<TReturnType>>>> _batchTaskResolver;
        private readonly Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, IGrouping<Proxy<TEntity>, Proxy<TReturnType>>>> _proxiedBatchTaskResolver;
        private readonly Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> _transform;

        public QueryableAsyncFuncResolver(
            string fieldName,
            Func<IResolveFieldContext, IQueryable<TReturnType>> resolver,
            Expression<Func<TEntity, TReturnType, bool>> condition,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> transform,
            IProxyAccessor<TEntity, TExecutionContext> outerProxyAccessor,
            IProxyAccessor<TReturnType, TExecutionContext> innerProxyAccessor)
        {
            _fieldName = fieldName ?? throw new ArgumentNullException(nameof(resolver));
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _condition = condition ?? throw new ArgumentNullException(nameof(condition));
            _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            _outerProxyAccessor = outerProxyAccessor ?? throw new ArgumentNullException(nameof(outerProxyAccessor));
            _innerProxyAccessor = innerProxyAccessor ?? throw new ArgumentNullException(nameof(innerProxyAccessor));

            var factorizationResults = ExpressionHelpers.Factorize(condition);
            outerProxyAccessor.AddMembers(fieldName, innerProxyAccessor, factorizationResults);

            _batchTaskResolver = GetBatchTaskResolver(resolver, condition, transform);
            _proxiedBatchTaskResolver = GetProxiedBatchTaskResolver(resolver, condition, transform);
        }

        private Func<IResolveFieldContext, IDataLoader<TEntity, IEnumerable<Proxy<TReturnType>>>> Resolver => ctx => _batchTaskResolver(ctx).Then(grouping => grouping.SafeNull());

        private Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, IEnumerable<Proxy<TReturnType>>>> ProxiedResolver => ctx => _proxiedBatchTaskResolver(ctx).Then(grouping => grouping.SafeNull());

        public object Resolve(IResolveFieldContext context)
        {
            var batchLoader = new Lazy<IDataLoader<TEntity, IEnumerable<Proxy<TReturnType>>>>(() => context.Bind(Resolver));
            var proxiedBatchLoader = new Lazy<IDataLoader<Proxy<TEntity>, IEnumerable<Proxy<TReturnType>>>>(() => context.Bind(ProxiedResolver));

            return context.Source is Proxy<TEntity> proxy
                ? proxiedBatchLoader.Value.LoadAsync(proxy)
                : batchLoader.Value.LoadAsync((TEntity)context.Source);
        }

        public IDataLoader<TEntity, object> GetBatchLoader(IResolveFieldContext context)
        {
            return Resolver(context).Then(FuncConstants<IEnumerable<Proxy<TReturnType>>>.WeakIdentity);
        }

        public IDataLoader<Proxy<TEntity>, object> GetProxiedBatchLoader(IResolveFieldContext context)
        {
            return ProxiedResolver(context).Then(FuncConstants<IEnumerable<Proxy<TReturnType>>>.WeakIdentity);
        }

        public IEnumerableResolver<TEntity, TSelectType, TExecutionContext> Select<TSelectType>(Expression<Func<TEntity, TReturnType, TSelectType>> selector)
        {
            var factorizationResult = ExpressionHelpers.Factorize(selector);
            _outerProxyAccessor.AddMembers(_fieldName, _innerProxyAccessor, factorizationResult);

            var outerProxyParam = Expression.Parameter(typeof(Proxy<TEntity>));
            var innerProxyParam = Expression.Parameter(typeof(Proxy<TReturnType>));
            var outers = factorizationResult.LeftExpressions
                .Select(e => _outerProxyAccessor.GetProxyExpression(e).CastFirstParamTo<Proxy<TEntity>>())
                .Select(e => e.Body.ReplaceParameter(e.Parameters[0], outerProxyParam))
                .ToList();

            var inners = factorizationResult.RightExpressions
                .Select(e => _innerProxyAccessor.GetProxyExpression(e).CastFirstParamTo<Proxy<TReturnType>>())
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

            var exprBody = ExpressionHelpers.ParameterRebinder.ReplaceParameters(paramMap, factorizationResult.Expression.Body);
            var expr = Expression.Lambda<Func<Proxy<TEntity>, Proxy<TReturnType>, TSelectType>>(exprBody, outerProxyParam, innerProxyParam);

            var compiledSelector = selector.Compile();
            var compiledExpr = expr.Compile();

            return new EnumerableAsyncFuncResolver<TEntity, TSelectType, TExecutionContext>(
                ctx => Resolver(ctx)
                    .Then((source, items) => items.Select(item => compiledSelector(source, item.GetOriginal()))),
                ctx => ProxiedResolver(ctx)
                    .Then((source, items) => items.Select(item => compiledExpr(source, item))));
        }

        public IQueryableResolver<TEntity, TSelectType, TExecutionContext> Select<TSelectType>(Expression<Func<TReturnType, TSelectType>> selector, IProxyAccessor<TSelectType, TExecutionContext> selectTypeProxyAccessor)
        {
            if (selectTypeProxyAccessor == null)
            {
                return new QueryableAsyncFuncResolver<TEntity, TReturnType, TSelectType, TExecutionContext>(
                    _fieldName,
                    ctx => _transform(ctx, _resolver(ctx)),
                    _condition,
                    selector,
                    _outerProxyAccessor,
                    _innerProxyAccessor);
            }

            return new ProxiedQueryableAsyncFuncResolver<TEntity, TReturnType, TSelectType, TExecutionContext>(
                _fieldName,
                ctx => _transform(ctx, _resolver(ctx)),
                _condition,
                selector,
                _outerProxyAccessor,
                _innerProxyAccessor,
                selectTypeProxyAccessor);
        }

        public IQueryableResolver<TEntity, TReturnType, TExecutionContext> Select(Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> selector)
        {
            return new QueryableAsyncFuncResolver<TEntity, TReturnType, TExecutionContext>(_fieldName, ctx => selector(ctx, _resolver(ctx)), _condition, _transform, _outerProxyAccessor, _innerProxyAccessor);
        }

        public IQueryableResolver<TEntity, TReturnType, TExecutionContext> Where(Expression<Func<TReturnType, bool>> predicate)
        {
            return new QueryableAsyncFuncResolver<TEntity, TReturnType, TExecutionContext>(_fieldName, ctx => _resolver(ctx).Where(predicate), _condition, _transform, _outerProxyAccessor, _innerProxyAccessor);
        }

        IEnumerableResolver<TEntity, TReturnType, TExecutionContext> IEnumerableResolver<TEntity, TReturnType, TExecutionContext>.Where(Expression<Func<TReturnType, bool>> predicate)
        {
            return Where(predicate);
        }

        IEnumerableResolver<TEntity, TSelectType, TExecutionContext> IEnumerableResolver<TEntity, TReturnType, TExecutionContext>.Select<TSelectType>(Expression<Func<TReturnType, TSelectType>> selector, IProxyAccessor<TSelectType, TExecutionContext> selectTypeProxyAccessor)
        {
            return Select(selector, selectTypeProxyAccessor);
        }

        public IResolver<TEntity> SingleOrDefault()
        {
            return new ProxiedAsyncFuncResolver<TEntity, TReturnType>(
                ctx => Resolver(ctx)
                    .Then(items => items.SingleOrDefault()),
                ctx => ProxiedResolver(ctx)
                    .Then(items => items.SingleOrDefault()));
        }

        public IResolver<TEntity> FirstOrDefault()
        {
            return new ProxiedAsyncFuncResolver<TEntity, TReturnType>(
                ctx => Resolver(ctx)
                    .Then(items => items.FirstOrDefault()),
                ctx => ProxiedResolver(ctx)
                    .Then(items => items.FirstOrDefault()));
        }

        private Func<IResolveFieldContext, IDataLoader<TEntity, IGrouping<TEntity, Proxy<TReturnType>>>> GetBatchTaskResolver(
            Func<IResolveFieldContext, IQueryable<TReturnType>> resolver,
            Expression<Func<TEntity, TReturnType, bool>> condition,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> queryableTransform)
        {
            var factorizationResult = ExpressionHelpers.FactorizeCondition(condition);

            var outerExpression = factorizationResult.LeftExpression;
            var innerExpression = factorizationResult.RightExpression;
            var rightCondition = factorizationResult.RightCondition;

            return context =>
            {
                var result = context
                    .Get<TEntity, TReturnType, Proxy<TReturnType>>(
                        ctx => queryableTransform(ctx, resolver(ctx).SafeWhere(rightCondition)),
                        Transform,
                        outerExpression,
                        innerExpression,
                        _innerProxyAccessor.CreateHooksExecuter(context.GetUserContext<TExecutionContext>()));

                return result;
            };
        }

        private Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, IGrouping<Proxy<TEntity>, Proxy<TReturnType>>>> GetProxiedBatchTaskResolver(
            Func<IResolveFieldContext, IQueryable<TReturnType>> resolver,
            Expression<Func<TEntity, TReturnType, bool>> condition,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> queryableTransform)
        {
            var factorizationResult = ExpressionHelpers.FactorizeCondition(condition);

            var outerExpression = new Lazy<LambdaExpression>(() => _outerProxyAccessor.GetProxyExpression(factorizationResult.LeftExpression));
            var innerExpression = factorizationResult.RightExpression;
            var rightCondition = factorizationResult.RightCondition;

            return context =>
            {
                var result = context
                    .Get<Proxy<TEntity>, TReturnType, Proxy<TReturnType>>(
                        ctx => queryableTransform(ctx, resolver(ctx).SafeWhere(rightCondition)),
                        Transform,
                        outerExpression.Value,
                        innerExpression,
                        _innerProxyAccessor.CreateHooksExecuter(context.GetUserContext<TExecutionContext>()));

                return result;
            };
        }

        private Expression<Func<TReturnType, Proxy<TReturnType>>> Transform(IResolveFieldContext context)
        {
            var fieldNames = context.GetQueriedFields();
            var lambda = _innerProxyAccessor.CreateSelectorExpression(fieldNames);

            var ctx = context.GetUserContext<TExecutionContext>();
            return lambda.BindFirstParameter(ctx);
        }
    }

    internal class QueryableAsyncFuncResolver<TEntity, TChildEntity, TReturnType, TExecutionContext> : IQueryableResolver<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        private readonly Func<IResolveFieldContext, IDataLoader<TEntity, IGrouping<TEntity, TReturnType>>> _batchTaskResolver;
        private readonly Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, IGrouping<Proxy<TEntity>, TReturnType>>> _proxiedBatchTaskResolver;
        private readonly Func<IResolveFieldContext, IQueryable<TChildEntity>> _resolver;
        private readonly Expression<Func<TEntity, TChildEntity, bool>> _condition;
        private readonly Expression<Func<TChildEntity, TReturnType>> _transform;
        private readonly IProxyAccessor<TEntity, TExecutionContext> _outerProxyAccessor;
        private readonly IProxyAccessor<TChildEntity, TExecutionContext> _innerProxyAccessor;
        private readonly string _fieldName;

        public QueryableAsyncFuncResolver(
            string fieldName,
            Func<IResolveFieldContext, IQueryable<TChildEntity>> resolver,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            Expression<Func<TChildEntity, TReturnType>> transform,
            IProxyAccessor<TEntity, TExecutionContext> outerProxyAccessor,
            IProxyAccessor<TChildEntity, TExecutionContext> innerProxyAccessor)
        {
            _fieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _condition = condition ?? throw new ArgumentNullException(nameof(condition));
            _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            _outerProxyAccessor = outerProxyAccessor ?? throw new ArgumentNullException(nameof(outerProxyAccessor));
            _innerProxyAccessor = innerProxyAccessor ?? throw new ArgumentNullException(nameof(innerProxyAccessor));
            _batchTaskResolver = GetBatchTaskResolver(resolver, condition, transform);
            _proxiedBatchTaskResolver = GetProxiedBatchTaskResolver(resolver, condition, transform);

            _innerProxyAccessor.AddMember(_transform);
        }

        private Func<IResolveFieldContext, IDataLoader<TEntity, IEnumerable<TReturnType>>> Resolver => ctx => _batchTaskResolver(ctx).Then(grouping => grouping.SafeNull());

        private Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, IEnumerable<TReturnType>>> ProxiedResolver => ctx => _proxiedBatchTaskResolver(ctx).Then(grouping => grouping.SafeNull());

        public object Resolve(IResolveFieldContext context)
        {
            var batchLoader = new Lazy<IDataLoader<TEntity, IEnumerable<TReturnType>>>(() => context.Bind(Resolver));
            var proxiedBatchLoader = new Lazy<IDataLoader<Proxy<TEntity>, IEnumerable<TReturnType>>>(() => context.Bind(ProxiedResolver));

            return context.Source is Proxy<TEntity> proxy
                ? proxiedBatchLoader.Value.LoadAsync(proxy)
                : batchLoader.Value.LoadAsync((TEntity)context.Source);
        }

        public IDataLoader<TEntity, object> GetBatchLoader(IResolveFieldContext context)
        {
            return Resolver(context).Then(FuncConstants<IEnumerable<TReturnType>>.WeakIdentity);
        }

        public IDataLoader<Proxy<TEntity>, object> GetProxiedBatchLoader(IResolveFieldContext context)
        {
            return ProxiedResolver(context).Then(FuncConstants<IEnumerable<TReturnType>>.WeakIdentity);
        }

        public IEnumerableResolver<TEntity, TSelectType, TExecutionContext> Select<TSelectType>(Expression<Func<TEntity, TReturnType, TSelectType>> selector)
        {
            throw new NotSupportedException();
        }

        public IQueryableResolver<TEntity, TSelectType, TExecutionContext> Select<TSelectType>(Expression<Func<TReturnType, TSelectType>> selector, IProxyAccessor<TSelectType, TExecutionContext> selectTypeProxyAccessor)
        {
            _innerProxyAccessor.RemoveMember(_transform);

            if (selectTypeProxyAccessor == null)
            {
                return new QueryableAsyncFuncResolver<TEntity, TChildEntity, TSelectType, TExecutionContext>(
                    _fieldName,
                    _resolver,
                    _condition,
                    ExpressionHelpers.Compose(_transform, selector),
                    _outerProxyAccessor,
                    _innerProxyAccessor);
            }

            return new ProxiedQueryableAsyncFuncResolver<TEntity, TChildEntity, TSelectType, TExecutionContext>(
                _fieldName,
                _resolver,
                _condition,
                ExpressionHelpers.Compose(_transform, selector),
                _outerProxyAccessor,
                _innerProxyAccessor,
                selectTypeProxyAccessor);
        }

        public IQueryableResolver<TEntity, TReturnType, TExecutionContext> Select(Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> selector)
        {
            throw new NotSupportedException();
        }

        public IQueryableResolver<TEntity, TReturnType, TExecutionContext> Where(Expression<Func<TReturnType, bool>> predicate)
        {
            _innerProxyAccessor.RemoveMember(_transform);

            return new QueryableAsyncFuncResolver<TEntity, TChildEntity, TReturnType, TExecutionContext>(
                _fieldName,
                ctx => _resolver(ctx).Where(ExpressionHelpers.Compose(_transform, predicate)),
                _condition,
                _transform,
                _outerProxyAccessor,
                _innerProxyAccessor);
        }

        IEnumerableResolver<TEntity, TSelectType, TExecutionContext> IEnumerableResolver<TEntity, TReturnType, TExecutionContext>.Select<TSelectType>(Expression<Func<TReturnType, TSelectType>> selector, IProxyAccessor<TSelectType, TExecutionContext> selectTypeProxyAccessor)
        {
            return Select(selector, selectTypeProxyAccessor);
        }

        IEnumerableResolver<TEntity, TReturnType, TExecutionContext> IEnumerableResolver<TEntity, TReturnType, TExecutionContext>.Where(Expression<Func<TReturnType, bool>> predicate)
        {
            return Where(predicate);
        }

        public IResolver<TEntity> SingleOrDefault()
        {
            return new AsyncFuncResolver<TEntity, TReturnType>(
                ctx => Resolver(ctx)
                    .Then(items => items.SingleOrDefault()),
                ctx => ProxiedResolver(ctx)
                    .Then(items => items.SingleOrDefault()));
        }

        public IResolver<TEntity> FirstOrDefault()
        {
            return new AsyncFuncResolver<TEntity, TReturnType>(
                ctx => Resolver(ctx)
                    .Then(items => items.FirstOrDefault()),
                ctx => ProxiedResolver(ctx)
                    .Then(items => items.FirstOrDefault()));
        }

        private Func<IResolveFieldContext, IDataLoader<TEntity, IGrouping<TEntity, TReturnType>>> GetBatchTaskResolver(
            Func<IResolveFieldContext, IQueryable<TChildEntity>> resolver,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            Expression<Func<TChildEntity, TReturnType>> transform)
        {
            if (!ExpressionHelpers.TryFactorizeCondition(condition, out var factorizationResult))
            {
                throw new ArgumentException($"Cannot translate condition {condition}.", nameof(condition));
            }

            var outerExpression = factorizationResult.LeftExpression;
            var innerExpression = factorizationResult.RightExpression;
            var rightCondition = factorizationResult.RightCondition;

            if (_innerProxyAccessor.HasHooks)
            {
                return context =>
                {
                    var result = context
                        .Get<TEntity, TChildEntity, Tuple<Proxy<TChildEntity>, TReturnType>>(
                            ctx => resolver(ctx).SafeWhere(rightCondition),
                            ctx => Transform2(ctx, transform),
                            outerExpression,
                            innerExpression,
                            new LoaderHooksExecuter<TChildEntity, TReturnType, TExecutionContext>(context.GetUserContext<TExecutionContext>(), _innerProxyAccessor))
                        .Then(group => Grouping.Create(group.Key, group.Select(g => g.Item2)));

                    return result;
                };
            }

            return context =>
            {
                var result = context
                    .Get<TEntity, TChildEntity, TReturnType>(
                        ctx => resolver(ctx).SafeWhere(rightCondition),
                        ctx => Transform(ctx, transform),
                        outerExpression,
                        innerExpression,
                        null); // TBD Should be null here?

                return result;
            };
        }

        private Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, IGrouping<Proxy<TEntity>, TReturnType>>> GetProxiedBatchTaskResolver(
            Func<IResolveFieldContext, IQueryable<TChildEntity>> resolver,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            Expression<Func<TChildEntity, TReturnType>> transform)
        {
            if (!ExpressionHelpers.TryFactorizeCondition(condition, out var factorizationResult))
            {
                throw new ArgumentException($"Cannot translate condition {condition}.", nameof(condition));
            }

            var outerExpression = new Lazy<LambdaExpression>(() => _outerProxyAccessor.GetProxyExpression(factorizationResult.LeftExpression));
            var innerExpression = factorizationResult.RightExpression;
            var rightCondition = factorizationResult.RightCondition;

            if (_innerProxyAccessor.HasHooks)
            {
                return context =>
                {
                    var result = context
                        .Get<Proxy<TEntity>, TChildEntity, Tuple<Proxy<TChildEntity>, TReturnType>>(
                            ctx => resolver(ctx).SafeWhere(rightCondition),
                            ctx => Transform2(ctx, transform),
                            outerExpression.Value,
                            innerExpression,
                            new LoaderHooksExecuter<TChildEntity, TReturnType, TExecutionContext>(context.GetUserContext<TExecutionContext>(), _innerProxyAccessor))
                        .Then(group => group == null ? null : Grouping.Create(group.Key, group.Select(g => g.Item2)));

                    return result;
                };
            }

            return context =>
            {
                var result = context
                    .Get<Proxy<TEntity>, TChildEntity, TReturnType>(
                        ctx => resolver(ctx).SafeWhere(rightCondition),
                        ctx => Transform(ctx, transform),
                        outerExpression.Value,
                        innerExpression,
                        null); // TBD Should be null here?

                return result;
            };
        }

        private Expression<Func<TChildEntity, TReturnType>> Transform(IResolveFieldContext context, Expression<Func<TChildEntity, TReturnType>> transform)
        {
            var fieldNames = context.GetQueriedFields();
            var proxiedSelector = (Expression<Func<Proxy<TChildEntity>, TReturnType>>)_innerProxyAccessor.GetProxyExpression(transform).CastFirstParamTo<Proxy<TChildEntity>>();
            var lambda = _innerProxyAccessor.CreateSelectorExpression(fieldNames);
            var result = ExpressionRewriter.Rewrite(ExpressionHelpers.Compose(lambda, proxiedSelector));

            var ctx = context.GetUserContext<TExecutionContext>();
            return result.BindFirstParameter(ctx);
        }

        private Expression<Func<TChildEntity, Tuple<Proxy<TChildEntity>, TReturnType>>> Transform2(IResolveFieldContext context, Expression<Func<TChildEntity, TReturnType>> transform)
        {
            var fieldNames = context.GetQueriedFields();
            var proxiedSelector = (Expression<Func<Proxy<TChildEntity>, TReturnType>>)_innerProxyAccessor.GetProxyExpression(transform).CastFirstParamTo<Proxy<TChildEntity>>();
            var executionContextParam = Expression.Parameter(typeof(TExecutionContext));
            var entityParam = Expression.Parameter(typeof(TChildEntity));

            var childEntityLambda = _innerProxyAccessor.CreateSelectorExpression(fieldNames);
            var resultLambda = ExpressionHelpers.Compose(childEntityLambda, proxiedSelector);

            var childEntityProxyExpr = childEntityLambda.Body
                .ReplaceParameter(childEntityLambda.Parameters[0], executionContextParam)
                .ReplaceParameter(childEntityLambda.Parameters[1], entityParam);

            var valueExpr = resultLambda.Body
                .ReplaceParameter(resultLambda.Parameters[0], executionContextParam)
                .ReplaceParameter(resultLambda.Parameters[1], entityParam);

            var newExpr = Expression.New(
                typeof(Tuple<Proxy<TChildEntity>, TReturnType>).GetConstructor(new[] { typeof(Proxy<TChildEntity>), typeof(TReturnType) }),
                childEntityProxyExpr,
                valueExpr);

            var result = ExpressionRewriter.Rewrite(
                Expression.Lambda<Func<TExecutionContext, TChildEntity, Tuple<Proxy<TChildEntity>, TReturnType>>>(newExpr, executionContextParam, entityParam));

            var ctx = context.GetUserContext<TExecutionContext>();
            return result.BindFirstParameter(ctx);
        }
    }
}
