// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Implementations.Fields.Helpers;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.TaskBatcher;
using GraphQL;
using GraphQL.DataLoader;

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal class QueryableFuncResolver<TEntity, TReturnType, TExecutionContext> : IQueryableResolver<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        private readonly Func<IResolveFieldContext, IQueryable<TReturnType>> _resolver;
        private readonly IProxyAccessor<TReturnType, TExecutionContext> _proxyAccessor;
        private readonly Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> _transform;

        public QueryableFuncResolver(
            IProxyAccessor<TReturnType, TExecutionContext> proxyAccessor,
            Func<IResolveFieldContext, IQueryable<TReturnType>> resolver,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> transform)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            _proxyAccessor = proxyAccessor;
        }

        public object Resolve(IResolveFieldContext context)
        {
            return _proxyAccessor == null
                ? context.ExecuteQuery(ctx => _transform(ctx, _resolver(ctx)))
                : _proxyAccessor.CreateHooksExecuter(context.GetUserContext<TExecutionContext>()).ExecuteHooks(
                    context.ExecuteQuery(ctx => _transform(ctx, _resolver(ctx)).Select(Transform(ctx))));
        }

        public IDataLoader<TEntity, object> GetBatchLoader(IResolveFieldContext context)
        {
            return BatchLoader.FromResult<TEntity, object>(_transform(context, _resolver(context)));
        }

        public IDataLoader<Proxy<TEntity>, object> GetProxiedBatchLoader(IResolveFieldContext context)
        {
            return BatchLoader.FromResult<Proxy<TEntity>, object>(_transform(context, _resolver(context)));
        }

        public IQueryableResolver<TEntity, TReturnType, TExecutionContext> Select(Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> selector)
        {
            return new QueryableFuncResolver<TEntity, TReturnType, TExecutionContext>(_proxyAccessor, ctx => selector(ctx, _resolver(ctx)), _transform);
        }

        public IEnumerableResolver<TEntity, TSelectType, TExecutionContext> Select<TSelectType>(Expression<Func<TEntity, TReturnType, TSelectType>> selector)
            => throw new NotSupportedException();

        public IQueryableResolver<TEntity, TReturnType, TExecutionContext> Where(Expression<Func<TReturnType, bool>> predicate)
        {
            return new QueryableFuncResolver<TEntity, TReturnType, TExecutionContext>(_proxyAccessor, ctx => _resolver(ctx).Where(predicate), _transform);
        }

        public IQueryableResolver<TEntity, TSelectType, TExecutionContext> Select<TSelectType>(Expression<Func<TReturnType, TSelectType>> selector, IProxyAccessor<TSelectType, TExecutionContext> selectTypeProxyAccessor)
        {
            return new QueryableFuncResolver<TEntity, TSelectType, TExecutionContext>(
                selectTypeProxyAccessor,
                ctx => _transform(ctx, _resolver(ctx)).Select(selector),
                (ctx, query) => query);
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
            return _proxyAccessor == null
                ? new FuncResolver<TEntity, TReturnType>((ctx, src) => ctx.ExecuteQuery(ctx => _transform(ctx, _resolver(ctx)), query => query.SingleOrDefault(), nameof(Queryable.SingleOrDefault)))
                : new ProxiedFuncResolver<TEntity, TReturnType>(ctx => _proxyAccessor.CreateHooksExecuter(ctx.GetUserContext<TExecutionContext>()).ExecuteHooks(ctx.ExecuteQuery(ctx => _transform(ctx, _resolver(ctx)).Select(Transform(ctx)), query => query.SingleOrDefault(), nameof(Queryable.SingleOrDefault))));
        }

        public IResolver<TEntity> FirstOrDefault()
        {
            return _proxyAccessor == null
                ? new FuncResolver<TEntity, TReturnType>((ctx, src) => ctx.ExecuteQuery(ctx => _transform(ctx, _resolver(ctx)), query => query.FirstOrDefault(), nameof(Queryable.FirstOrDefault)))
                : new ProxiedFuncResolver<TEntity, TReturnType>(ctx => _proxyAccessor.CreateHooksExecuter(ctx.GetUserContext<TExecutionContext>()).ExecuteHooks(ctx.ExecuteQuery(ctx => _transform(ctx, _resolver(ctx)).Select(Transform(ctx)), query => query.FirstOrDefault(), nameof(Queryable.FirstOrDefault))));
        }

        private Expression<Func<TReturnType, Proxy<TReturnType>>> Transform(IResolveFieldContext context)
        {
            var fieldNames = context.GetQueriedFields();
            var lambda = _proxyAccessor.CreateSelectorExpression(fieldNames);

            var ctx = context.GetUserContext<TExecutionContext>();
            return lambda.BindFirstParameter(ctx);
        }
    }
}
