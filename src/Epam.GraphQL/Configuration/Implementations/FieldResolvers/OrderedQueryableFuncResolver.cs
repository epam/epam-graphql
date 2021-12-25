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

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal class OrderedQueryableFuncResolver<TEntity, TReturnType, TExecutionContext> : IOrderedQueryableResolver<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        private readonly Func<IResolveFieldContext, IQueryable<TReturnType>> _resolver;
        private readonly Func<IResolveFieldContext, IQueryable<TReturnType>, IOrderedQueryable<TReturnType>> _sorter;
        private readonly Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> _transform;

        public OrderedQueryableFuncResolver(
            IProxyAccessor<TReturnType, TExecutionContext> proxyAccessor,
            Func<IResolveFieldContext, IQueryable<TReturnType>> resolver,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> transform,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IOrderedQueryable<TReturnType>> sorter)
        {
            ProxyAccessor = proxyAccessor;
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _sorter = sorter ?? throw new ArgumentNullException(nameof(sorter));
            _transform = transform ?? throw new ArgumentNullException(nameof(transform));

            Resolver = ctx => _sorter(ctx, _transform(ctx, _resolver(ctx)));
        }

        protected Func<IResolveFieldContext, IOrderedQueryable<TReturnType>> Resolver { get; }

        protected IProxyAccessor<TReturnType, TExecutionContext> ProxyAccessor { get; }

        public object Resolve(IResolveFieldContext context)
        {
            var query = context.ExecuteQuery(ctx => Resolver(ctx).Select(Transform(ctx)));
            return query;
        }

        public IDataLoader<TEntity, object> GetBatchLoader(IResolveFieldContext context)
        {
            return BatchLoader.FromResult<TEntity, object>(Resolver(context).Select(Transform(context)));
        }

        public IDataLoader<Proxy<TEntity>, object> GetProxiedBatchLoader(IResolveFieldContext context)
        {
            return BatchLoader.FromResult<Proxy<TEntity>, object>(Resolver(context).Select(Transform(context)));
        }

        public IOrderedQueryableResolver<TEntity, TReturnType, TExecutionContext> Select(
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> selector,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IOrderedQueryable<TReturnType>> sorter)
        {
            return Create(ProxyAccessor, ctx => selector(ctx, _resolver(ctx)), _transform, sorter);
        }

        public IEnumerableResolver<TEntity, TSelectType, TExecutionContext> Select<TSelectType>(Expression<Func<TEntity, TReturnType, TSelectType>> selector)
        {
            // TODO Make EnumerableFuncResolver's resolver independed from source
            var compiledSelector = selector.Compile();
            return new EnumerableFuncResolver<TEntity, TSelectType, TExecutionContext>(
                (ctx, src) => Resolver(ctx).Select(item => compiledSelector(src, item)));
        }

        public IOrderedQueryableResolver<TEntity, TReturnType, TExecutionContext> Where(Expression<Func<TReturnType, bool>> predicate)
        {
            return Create(ProxyAccessor, ctx => _resolver(ctx).Where(predicate), _transform, _sorter);
        }

        public IResolver<TEntity> Select<TSelectType>(Func<IResolveFieldContext, IOrderedQueryable<TReturnType>, TSelectType> selector)
        {
            return new FuncResolver<TEntity, TSelectType>(
                (ctx, src) =>
                {
                    var resolved = Resolver(ctx);
                    var selected = selector(ctx, resolved);

                    return selected;
                });
        }

        public IOrderedQueryableResolver<TEntity, TAnotherReturnType, TExecutionContext> Select<TAnotherReturnType>(
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TAnotherReturnType>> selector,
            Func<IResolveFieldContext, IQueryable<TAnotherReturnType>, IOrderedQueryable<TAnotherReturnType>> sorter)
            where TAnotherReturnType : class
        {
            return Create(null, ctx => selector(ctx, _transform(ctx, _resolver(ctx))), (ctx, query) => query, sorter);
        }

        public IEnumerableResolver<TEntity, TSelectType, TExecutionContext> Select<TSelectType>(Expression<Func<TReturnType, TSelectType>> selector, IProxyAccessor<TSelectType, TExecutionContext> selectTypeProxyAccessor)
        {
            var entityParam = Expression.Parameter(typeof(TEntity));
            var lambda = Expression.Lambda<Func<TEntity, TReturnType, TSelectType>>(selector.Body, entityParam, selector.Parameters[0]);

            return Select(lambda);
        }

        IEnumerableResolver<TEntity, TReturnType, TExecutionContext> IEnumerableResolver<TEntity, TReturnType, TExecutionContext>.Where(Expression<Func<TReturnType, bool>> predicate)
        {
            return Where(predicate);
        }

        IQueryableResolver<TEntity, TSelectType, TExecutionContext> IQueryableResolver<TEntity, TReturnType, TExecutionContext>.Select<TSelectType>(Expression<Func<TReturnType, TSelectType>> selector, IProxyAccessor<TSelectType, TExecutionContext> selectTypeProxyAccessor)
        {
            throw new NotSupportedException();
        }

        IQueryableResolver<TEntity, TReturnType, TExecutionContext> IQueryableResolver<TEntity, TReturnType, TExecutionContext>.Select(Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> selector)
        {
            return Select(selector, _sorter);
        }

        IQueryableResolver<TEntity, TReturnType, TExecutionContext> IQueryableResolver<TEntity, TReturnType, TExecutionContext>.Where(Expression<Func<TReturnType, bool>> predicate)
        {
            return Where(predicate);
        }

        public IResolver<TEntity> SingleOrDefault()
        {
            return new ProxiedFuncResolver<TEntity, TReturnType>(ctx => ctx.ExecuteQuery(ctx => Resolver(ctx).Select(Transform(ctx)), query => query.SingleOrDefault(), nameof(Queryable.SingleOrDefault)));
        }

        public IResolver<TEntity> FirstOrDefault()
        {
            return new ProxiedFuncResolver<TEntity, TReturnType>(ctx => ctx.ExecuteQuery(ctx => Resolver(ctx).Select(Transform(ctx)), query => query.FirstOrDefault(), nameof(Queryable.FirstOrDefault)));
        }

        public IResolver<TEntity> AsConnection()
        {
            var connectionResolver = Resolvers.ToConnection<TEntity, TReturnType, TExecutionContext>(ProxyAccessor);

            return new FuncResolver<TEntity, Connection<Proxy<TReturnType>>>(
                (ctx, src) =>
                {
                    var resolved = Resolver(ctx).Select(Transform(ctx));
                    var selected = connectionResolver(ctx, (IOrderedQueryable<Proxy<TReturnType>>)resolved);

                    return selected;
                });
        }

        protected virtual IOrderedQueryableResolver<TEntity, TAnotherReturnType, TExecutionContext> Create<TAnotherReturnType>(
            IProxyAccessor<TAnotherReturnType, TExecutionContext> proxyAccessor,
            Func<IResolveFieldContext, IQueryable<TAnotherReturnType>> resolver,
            Func<IResolveFieldContext, IQueryable<TAnotherReturnType>, IQueryable<TAnotherReturnType>> transform,
            Func<IResolveFieldContext, IQueryable<TAnotherReturnType>, IOrderedQueryable<TAnotherReturnType>> sorter)
        {
            return new OrderedQueryableFuncResolver<TEntity, TAnotherReturnType, TExecutionContext>(proxyAccessor, resolver, transform, sorter);
        }

        protected virtual Expression<Func<TReturnType, Proxy<TReturnType>>> Transform(IResolveFieldContext context)
        {
            var fieldNames = GetQueriedFields(context);
            var lambda = ProxyAccessor.CreateSelectorExpression(fieldNames);

            var ctx = context.GetUserContext<TExecutionContext>();
            return lambda.BindFirstParameter(ctx);
        }

        protected virtual IEnumerable<string> GetQueriedFields(IResolveFieldContext context)
        {
            return context.GetQueriedFields();
        }
    }
}
