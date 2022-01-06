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

#nullable enable

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal class ProxiedQueryableAsyncFuncResolver<TEntity, TChildEntity, TReturnType, TExecutionContext> : IQueryableResolver<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        private readonly Func<IResolveFieldContext, IDataLoader<TEntity, IGrouping<TEntity, Proxy<TReturnType>>>> _batchTaskResolver;
        private readonly Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, IGrouping<Proxy<TEntity>, Proxy<TReturnType>>>> _proxiedBatchTaskResolver;
        private readonly Func<IResolveFieldContext, IQueryable<TChildEntity>> _resolver;
        private readonly Expression<Func<TEntity, TChildEntity, bool>> _condition;
        private readonly Expression<Func<TChildEntity, TReturnType>> _transform;
        private readonly IProxyAccessor<TEntity, TExecutionContext> _outerProxyAccessor;
        private readonly IProxyAccessor<TChildEntity, TExecutionContext> _innerProxyAccessor;
        private readonly IProxyAccessor<TReturnType, TExecutionContext> _returnTypeProxyAccessor;
        private readonly string _fieldName;

        public ProxiedQueryableAsyncFuncResolver(
            string fieldName,
            Func<IResolveFieldContext, IQueryable<TChildEntity>> resolver,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            Expression<Func<TChildEntity, TReturnType>> transform,
            IProxyAccessor<TEntity, TExecutionContext> outerProxyAccessor,
            IProxyAccessor<TChildEntity, TExecutionContext> innerProxyAccessor,
            IProxyAccessor<TReturnType, TExecutionContext> returnTypeProxyAccessor)
        {
            _fieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _condition = condition ?? throw new ArgumentNullException(nameof(condition));
            _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            _outerProxyAccessor = outerProxyAccessor ?? throw new ArgumentNullException(nameof(outerProxyAccessor));
            _innerProxyAccessor = innerProxyAccessor ?? throw new ArgumentNullException(nameof(innerProxyAccessor));
            _returnTypeProxyAccessor = returnTypeProxyAccessor ?? throw new ArgumentNullException(nameof(returnTypeProxyAccessor));
            _batchTaskResolver = GetBatchTaskResolver<TEntity>(resolver, condition, transform, FuncConstants<LambdaExpression>.Identity);
            _proxiedBatchTaskResolver = GetBatchTaskResolver<Proxy<TEntity>>(resolver, condition, transform, leftExpression => _outerProxyAccessor.GetProxyExpression(leftExpression));

            _innerProxyAccessor.AddMember(_transform);
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

        public IDataLoader<TEntity, object?> GetBatchLoader(IResolveFieldContext context)
        {
            return Resolver(context).Then(FuncConstants<IEnumerable<Proxy<TReturnType>>>.WeakIdentity);
        }

        public IDataLoader<Proxy<TEntity>, object?> GetProxiedBatchLoader(IResolveFieldContext context)
        {
            return ProxiedResolver(context).Then(FuncConstants<IEnumerable<Proxy<TReturnType>>>.WeakIdentity);
        }

        public IQueryableResolver<TEntity, TSelectType, TExecutionContext> Select<TSelectType>(Expression<Func<TReturnType, TSelectType>> selector, IProxyAccessor<TSelectType, TExecutionContext>? selectTypeProxyAccessor)
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

        public IQueryableResolver<TEntity, TReturnType, TExecutionContext> Where(Expression<Func<TReturnType, bool>> predicate)
        {
            _innerProxyAccessor.RemoveMember(_transform);

            return new ProxiedQueryableAsyncFuncResolver<TEntity, TChildEntity, TReturnType, TExecutionContext>(
                _fieldName,
                ctx => _resolver(ctx).Where(ExpressionHelpers.Compose(_transform, predicate)),
                _condition,
                _transform,
                _outerProxyAccessor,
                _innerProxyAccessor,
                _returnTypeProxyAccessor);
        }

        IEnumerableResolver<TEntity, TSelectType, TExecutionContext> IEnumerableResolver<TEntity, TReturnType, TExecutionContext>.Select<TSelectType>(Expression<Func<TReturnType, TSelectType>> selector, IProxyAccessor<TSelectType, TExecutionContext>? selectTypeProxyAccessor)
        {
            return Select(selector, selectTypeProxyAccessor);
        }

        IEnumerableResolver<TEntity, TReturnType, TExecutionContext> IEnumerableResolver<TEntity, TReturnType, TExecutionContext>.Where(Expression<Func<TReturnType, bool>> predicate)
        {
            return Where(predicate);
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

        public IEnumerableResolver<TEntity, TSelectType, TExecutionContext> Select<TSelectType>(Expression<Func<TEntity, TReturnType, TSelectType>> selector)
        {
            throw new NotSupportedException();
        }

        public IQueryableResolver<TEntity, TReturnType, TExecutionContext> Select(Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> selector)
        {
            throw new NotSupportedException();
        }

        public IQueryableResolver<TEntity, TReturnType, TExecutionContext> Reorder(Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> sorter)
        {
            // TODO Come up with how to reorder
            return this;
        }

        public IResolver<TEntity> AsGroupConnection(
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<Proxy<TReturnType>>> selector,
            Func<IResolveFieldContext, IQueryable<Proxy<TReturnType>>, IQueryable<Proxy<TReturnType>>> sorter)
        {
            throw new NotSupportedException();
        }

        public IResolver<TEntity> AsConnection()
        {
            throw new NotSupportedException();
        }

        private Func<IResolveFieldContext, IDataLoader<TOuterEntity, IGrouping<TOuterEntity, Proxy<TReturnType>>>> GetBatchTaskResolver<TOuterEntity>(
            Func<IResolveFieldContext, IQueryable<TChildEntity>> resolver,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            Expression<Func<TChildEntity, TReturnType>> transform,
            Func<LambdaExpression, LambdaExpression> leftExpressionConverter)
        {
            var factorizationResult = ExpressionHelpers.FactorizeCondition(condition);

            var outerExpression = leftExpressionConverter(factorizationResult.LeftExpression);
            var innerExpression = factorizationResult.RightExpression;
            var rightCondition = factorizationResult.RightCondition;

            if (_innerProxyAccessor.HasHooks)
            {
                return context =>
                {
                    var result = context
                        .Get<TOuterEntity, TChildEntity, Tuple<Proxy<TChildEntity>, Proxy<TReturnType>>>(
                            ctx => resolver(ctx).SafeWhere(rightCondition),
                            ctx => Transform2(ctx, transform),
                            outerExpression,
                            innerExpression,
                            new LoaderHooksExecuter<TChildEntity, Proxy<TReturnType>, TExecutionContext>(context.GetUserContext<TExecutionContext>(), _innerProxyAccessor))
                        .Then(group => Grouping.Create(group.Key, group.Select(g => g.Item2)));

                    return result;
                };
            }

            return context =>
            {
                var result = context
                    .Get<TOuterEntity, TChildEntity, Proxy<TReturnType>>(
                        ctx => resolver(ctx).SafeWhere(rightCondition),
                        ctx => Transform(ctx, transform),
                        outerExpression,
                        innerExpression,
                        _returnTypeProxyAccessor.CreateHooksExecuter(context.GetUserContext<TExecutionContext>()));

                return result;
            };
        }

        private Expression<Func<TChildEntity, Proxy<TReturnType>>> Transform(IResolveFieldContext context, Expression<Func<TChildEntity, TReturnType>> transform)
        {
            var fieldNames = context.GetQueriedFields();

            var transformedReturnLambda = _returnTypeProxyAccessor.CreateSelectorExpression(fieldNames);

            var proxiedSelector = (Expression<Func<Proxy<TChildEntity>, TReturnType>>)_innerProxyAccessor.GetProxyExpression(transform).CastFirstParamTo<Proxy<TChildEntity>>();
            var transformedLambda = _innerProxyAccessor.CreateSelectorExpression(fieldNames);
            var transformedResult = ExpressionRewriter.Rewrite(ExpressionHelpers.Compose(transformedLambda, proxiedSelector));

            var result = ExpressionHelpers.SafeCompose(transformedResult, transformedReturnLambda);

            var ctx = context.GetUserContext<TExecutionContext>();
            return result.BindFirstParameter(ctx);
        }

        private Expression<Func<TChildEntity, Tuple<Proxy<TChildEntity>, Proxy<TReturnType>>>> Transform2(IResolveFieldContext context, Expression<Func<TChildEntity, TReturnType>> transform)
        {
            var fieldNames = context.GetQueriedFields();
            var transformedReturnLambda = _returnTypeProxyAccessor.CreateSelectorExpression(fieldNames);
            var proxiedSelector = (Expression<Func<Proxy<TChildEntity>, TReturnType>>)_innerProxyAccessor.GetProxyExpression(transform).CastFirstParamTo<Proxy<TChildEntity>>();
            var childEntityLambda = _innerProxyAccessor.CreateSelectorExpression(fieldNames);
            var transformedResult = ExpressionRewriter.Rewrite(ExpressionHelpers.Compose(childEntityLambda, proxiedSelector));
            var resultLambda = ExpressionHelpers.SafeCompose(transformedResult, transformedReturnLambda);

            var executionContextParam = Expression.Parameter(typeof(TExecutionContext));
            var entityParam = Expression.Parameter(typeof(TChildEntity));

            var childEntityProxyExpr = childEntityLambda.Body
                .ReplaceParameter(childEntityLambda.Parameters[0], executionContextParam)
                .ReplaceParameter(childEntityLambda.Parameters[1], entityParam);

            var valueExpr = resultLambda.Body
                .ReplaceParameter(resultLambda.Parameters[0], executionContextParam)
                .ReplaceParameter(resultLambda.Parameters[1], entityParam);

            var newExpr = Expression.New(
                typeof(Tuple<Proxy<TChildEntity>, Proxy<TReturnType>>).GetConstructor(new[] { typeof(Proxy<TChildEntity>), typeof(Proxy<TReturnType>) }),
                childEntityProxyExpr,
                valueExpr);
            var result = ExpressionRewriter.Rewrite(
                Expression.Lambda<Func<TExecutionContext, TChildEntity, Tuple<Proxy<TChildEntity>, Proxy<TReturnType>>>>(newExpr, executionContextParam, entityParam));

            var ctx = context.GetUserContext<TExecutionContext>();
            return result.BindFirstParameter(ctx);
        }
    }
}
