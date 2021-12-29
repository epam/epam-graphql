// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Implementations.Fields.Helpers;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Relay;
using Epam.GraphQL.TaskBatcher;
using GraphQL;
using GraphQL.DataLoader;

#nullable enable

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal class QueryableFuncResolver<TEntity, TReturnType, TExecutionContext> : IQueryableResolver<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        private readonly Func<IResolveFieldContext, IQueryable<TReturnType>> _resolver;
        private readonly IProxyAccessor<TReturnType, TExecutionContext>? _proxyAccessor;
        private readonly Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> _transform;
        private readonly Func<IResolveFieldContext, IQueryable<TReturnType>, IOrderedQueryable<TReturnType>>? _sorter;

        public QueryableFuncResolver(
            IProxyAccessor<TReturnType, TExecutionContext>? proxyAccessor,
            Func<IResolveFieldContext, IQueryable<TReturnType>> resolver,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> transform,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IOrderedQueryable<TReturnType>>? sorter)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            _sorter = sorter;
            _proxyAccessor = proxyAccessor;
        }

        private Func<IResolveFieldContext, IQueryable<TReturnType>> Resolver =>
            _sorter == null
                ? ctx => _transform(ctx, _resolver(ctx))
                : ctx => _sorter(ctx, _transform(ctx, _resolver(ctx)));

        public object Resolve(IResolveFieldContext context)
        {
            return _proxyAccessor == null
                ? context.ExecuteQuery(Resolver)
                : _proxyAccessor.CreateHooksExecuter(context.GetUserContext<TExecutionContext>()).ExecuteHooks(
                    context.ExecuteQuery(ctx => Resolver(ctx).Select(Transform(ctx))));
        }

        public IDataLoader<TEntity, object?> GetBatchLoader(IResolveFieldContext context)
        {
            return BatchLoader.FromResult<TEntity, object?>(Resolver(context).Select(Transform(context)));
        }

        public IDataLoader<Proxy<TEntity>, object?> GetProxiedBatchLoader(IResolveFieldContext context)
        {
            return BatchLoader.FromResult<Proxy<TEntity>, object?>(Resolver(context).Select(Transform(context)));
        }

        public IQueryableResolver<TEntity, TReturnType, TExecutionContext> Select(Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> selector)
        {
            return Create(_proxyAccessor, ctx => selector(ctx, _resolver(ctx)), _transform, _sorter);
        }

        public IQueryableResolver<TEntity, TSelectType, TExecutionContext> Select<TSelectType>(
            Expression<Func<TReturnType, TSelectType>> selector,
            IProxyAccessor<TSelectType, TExecutionContext>? selectTypeProxyAccessor)
        {
            return Create(
                selectTypeProxyAccessor,
                ctx => Resolver(ctx).Select(selector),
                (ctx, query) => query,
                null);
        }

        IEnumerableResolver<TEntity, TSelectType, TExecutionContext> IEnumerableResolver<TEntity, TReturnType, TExecutionContext>.Select<TSelectType>(
            Expression<Func<TReturnType, TSelectType>> selector,
            IProxyAccessor<TSelectType, TExecutionContext>? selectTypeProxyAccessor)
        {
            return Select(selector, selectTypeProxyAccessor);
        }

        public IResolver<TEntity> SingleOrDefault()
        {
            return _proxyAccessor == null
                ? new FuncResolver<TEntity, TReturnType>((ctx, src) => ctx.ExecuteQuery(Resolver, query => query.SingleOrDefault(), nameof(Queryable.SingleOrDefault)))
                : new ProxiedFuncResolver<TEntity, TReturnType>(ctx => _proxyAccessor.CreateHooksExecuter(ctx.GetUserContext<TExecutionContext>()).ExecuteHooks(ctx.ExecuteQuery(ctx => Resolver(ctx).Select(Transform(ctx)), query => query.SingleOrDefault(), nameof(Queryable.SingleOrDefault))));
        }

        public IResolver<TEntity> FirstOrDefault()
        {
            return _proxyAccessor == null
                ? new FuncResolver<TEntity, TReturnType>((ctx, src) => ctx.ExecuteQuery(Resolver, query => query.FirstOrDefault(), nameof(Queryable.FirstOrDefault)))
                : new ProxiedFuncResolver<TEntity, TReturnType>(ctx => _proxyAccessor.CreateHooksExecuter(ctx.GetUserContext<TExecutionContext>()).ExecuteHooks(ctx.ExecuteQuery(ctx => Resolver(ctx).Select(Transform(ctx)), query => query.FirstOrDefault(), nameof(Queryable.FirstOrDefault))));
        }

        public IQueryableResolver<TEntity, TReturnType, TExecutionContext> Reorder(Func<IResolveFieldContext, IQueryable<TReturnType>, IOrderedQueryable<TReturnType>> sorter)
        {
            return Create(_proxyAccessor, _resolver, _transform, sorter);
        }

        public IResolver<TEntity> AsGroupConnection(
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<Proxy<TReturnType>>> selector,
            Func<IResolveFieldContext, IQueryable<Proxy<TReturnType>>, IOrderedQueryable<Proxy<TReturnType>>> sorter)
        {
            var connectionResolver = Resolvers.ToGroupConnection<TReturnType, TExecutionContext>();

            var resolver = Create(null, ctx => selector(ctx, _transform(ctx, _resolver(ctx))), (ctx, query) => query, sorter);

            return new FuncResolver<TEntity, Connection<object>>(
                (ctx, src) =>
                {
                    var resolved = resolver.Resolver(ctx);
                    var selected = connectionResolver(ctx, (IOrderedQueryable<Proxy<TReturnType>>)resolved); // TODO Get rid of cast

                    return selected;
                });
        }

        public IQueryableResolver<TEntity, TReturnType, TExecutionContext> Where(Expression<Func<TReturnType, bool>> predicate)
        {
            return Create(_proxyAccessor, ctx => _resolver(ctx).Where(predicate), _transform, _sorter);
        }

        IEnumerableResolver<TEntity, TReturnType, TExecutionContext> IEnumerableResolver<TEntity, TReturnType, TExecutionContext>.Where(Expression<Func<TReturnType, bool>> predicate)
        {
            return Where(predicate);
        }

        public IEnumerableResolver<TEntity, TSelectType, TExecutionContext> Select<TSelectType>(Expression<Func<TEntity, TReturnType, TSelectType>> selector)
            => throw new NotSupportedException();

        public IResolver<TEntity> AsConnection()
        {
            var resolver = new ConnectionFuncResolver<TEntity, TReturnType, TExecutionContext>(_proxyAccessor, _resolver, _transform, _sorter);
            var connectionResolver = Resolvers.ToConnection<TEntity, TReturnType, TExecutionContext>(_proxyAccessor);

            return new FuncResolver<TEntity, Connection<Proxy<TReturnType>>>(
                (ctx, src) =>
                {
                    var resolved = resolver.Resolver(ctx).Select(resolver.Transform(ctx));
                    var selected = connectionResolver(ctx, (IOrderedQueryable<Proxy<TReturnType>>)resolved);

                    return selected;
                });
        }

        protected virtual QueryableFuncResolver<TEntity, TAnotherReturnType, TExecutionContext> Create<TAnotherReturnType>(
            IProxyAccessor<TAnotherReturnType, TExecutionContext>? proxyAccessor,
            Func<IResolveFieldContext, IQueryable<TAnotherReturnType>> resolver,
            Func<IResolveFieldContext, IQueryable<TAnotherReturnType>, IQueryable<TAnotherReturnType>> transform,
            Func<IResolveFieldContext, IQueryable<TAnotherReturnType>, IOrderedQueryable<TAnotherReturnType>>? sorter)
        {
            return new QueryableFuncResolver<TEntity, TAnotherReturnType, TExecutionContext>(proxyAccessor, resolver, transform, sorter);
        }

        protected virtual IEnumerable<string> GetQueriedFields(IResolveFieldContext context)
        {
            return context.GetQueriedFields();
        }

        private Expression<Func<TReturnType, Proxy<TReturnType>>> Transform(IResolveFieldContext context)
        {
            if (_proxyAccessor == null)
            {
                throw new NotSupportedException();
            }

            var fieldNames = GetQueriedFields(context);
            var lambda = _proxyAccessor.CreateSelectorExpression(fieldNames);

            var ctx = context.GetUserContext<TExecutionContext>();
            return lambda.BindFirstParameter(ctx);
        }
    }
}
