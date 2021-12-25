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
    internal class OrderedQueryableAsyncFuncResolver<TEntity, TReturnType, TExecutionContext> : IOrderedQueryableResolver<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        private readonly string _fieldName;
        private readonly Expression<Func<TEntity, TReturnType, bool>> _condition;
        private readonly Func<IResolveFieldContext, IQueryable<TReturnType>> _resolver;
        private readonly Func<IResolveFieldContext, IQueryable<TReturnType>, IOrderedQueryable<TReturnType>> _sorter;
        private readonly Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> _transform;
        private readonly Lazy<Func<IResolveFieldContext, IDataLoader<TEntity, IGrouping<TEntity, Proxy<TReturnType>>>>> _batchTaskResolver;
        private readonly Lazy<Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, IGrouping<Proxy<TEntity>, Proxy<TReturnType>>>>> _proxiedBatchTaskResolver;
        private readonly IProxyAccessor<TEntity, TExecutionContext> _outerProxyAccessor;

        public OrderedQueryableAsyncFuncResolver(
            string fieldName,
            Func<IResolveFieldContext, IQueryable<TReturnType>> resolver,
            Expression<Func<TEntity, TReturnType, bool>> condition,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> transform,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IOrderedQueryable<TReturnType>> sorter,
            IProxyAccessor<TEntity, TExecutionContext> outerProxyAccessor,
            IProxyAccessor<TReturnType, TExecutionContext> innerProxyAccessor)
        {
            _fieldName = fieldName ?? throw new ArgumentNullException(nameof(fieldName));
            _outerProxyAccessor = outerProxyAccessor ?? throw new ArgumentNullException(nameof(outerProxyAccessor));
            InnerProxyAccessor = innerProxyAccessor ?? throw new ArgumentNullException(nameof(innerProxyAccessor));
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _condition = condition ?? throw new ArgumentNullException(nameof(condition));
            _transform = transform ?? throw new ArgumentNullException(nameof(transform));
            _sorter = sorter ?? throw new ArgumentNullException(nameof(sorter));
            _batchTaskResolver = new Lazy<Func<IResolveFieldContext, IDataLoader<TEntity, IGrouping<TEntity, Proxy<TReturnType>>>>>(
                () => GetBatchTaskResolver(_resolver, _condition, _transform, _sorter));
            _proxiedBatchTaskResolver = new Lazy<Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, IGrouping<Proxy<TEntity>, Proxy<TReturnType>>>>>(
                () => GetProxiedBatchTaskResolver(_resolver, _condition, _transform, _sorter));

            var factorizationResults = ExpressionHelpers.Factorize(condition);
            outerProxyAccessor.AddMembers(fieldName, innerProxyAccessor, factorizationResults);
        }

        protected Func<IResolveFieldContext, IDataLoader<TEntity, IOrderedQueryable<Proxy<TReturnType>>>> Resolver => ctx => _batchTaskResolver.Value(ctx).Then(items => (IOrderedQueryable<Proxy<TReturnType>>)items.SafeNull().AsQueryable());

        protected Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, IOrderedQueryable<Proxy<TReturnType>>>> ProxiedResolver => ctx => _proxiedBatchTaskResolver.Value(ctx).Then(items => (IOrderedQueryable<Proxy<TReturnType>>)items.SafeNull().AsQueryable());

        protected IProxyAccessor<TReturnType, TExecutionContext> InnerProxyAccessor { get; }

        public object Resolve(IResolveFieldContext context)
        {
            var batchLoader = new Lazy<IDataLoader<TEntity, IOrderedQueryable<Proxy<TReturnType>>>>(() => context.Bind(Resolver));
            var proxiedBatchLoader = new Lazy<IDataLoader<Proxy<TEntity>, IOrderedQueryable<Proxy<TReturnType>>>>(() => context.Bind(ProxiedResolver));

            return context.Source is Proxy<TEntity> proxy
                ? proxiedBatchLoader.Value.LoadAsync(proxy)
                : batchLoader.Value.LoadAsync((TEntity)context.Source);
        }

        public IDataLoader<TEntity, object> GetBatchLoader(IResolveFieldContext context)
        {
            return Resolver(context).Then(FuncConstants<IOrderedQueryable<object>>.WeakIdentity);
        }

        public IDataLoader<Proxy<TEntity>, object> GetProxiedBatchLoader(IResolveFieldContext context)
        {
            return ProxiedResolver(context).Then(FuncConstants<IOrderedQueryable<object>>.WeakIdentity);
        }

        public IOrderedQueryableResolver<TEntity, TReturnType, TExecutionContext> Select(
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> selector,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IOrderedQueryable<TReturnType>> sorter)
        {
            return Create(_fieldName, ctx => selector(ctx, _resolver(ctx)), _condition, _transform, sorter, _outerProxyAccessor, InnerProxyAccessor);
        }

        public IEnumerableResolver<TEntity, TSelectType, TExecutionContext> Select<TSelectType>(Expression<Func<TEntity, TReturnType, TSelectType>> selector)
        {
            var factorizationResults = ExpressionHelpers.Factorize(selector);
            _outerProxyAccessor.AddMembers(_fieldName, InnerProxyAccessor, factorizationResults);

            var compiledSelector = selector.Compile();

            return new EnumerableAsyncFuncResolver<TEntity, TSelectType, TExecutionContext>(
                ctx => _batchTaskResolver.Value(ctx).Then((source, items) => items.SafeNull().Select(item => compiledSelector(source, item.GetOriginal())).AsEnumerable()),
                ctx => _proxiedBatchTaskResolver.Value(ctx).Then((source, items) => items.SafeNull().Select(item => compiledSelector(source.GetOriginal(), item.GetOriginal())).AsEnumerable()));
        }

        public IResolver<TEntity> Select<TSelectType>(Func<IResolveFieldContext, IOrderedQueryable<TReturnType>, TSelectType> selector)
        {
            return new AsyncFuncResolver<TEntity, TSelectType>(
                ctx => _batchTaskResolver.Value(ctx).Then(items => selector(ctx, (IOrderedQueryable<TReturnType>)items.SafeNull().AsQueryable().Select(proxy => proxy.GetOriginal()))),
                ctx => _proxiedBatchTaskResolver.Value(ctx).Then(items => selector(ctx, (IOrderedQueryable<TReturnType>)items.SafeNull().AsQueryable().Select(proxy => proxy.GetOriginal()))));
        }

        public IOrderedQueryableResolver<TEntity, TReturnType, TExecutionContext> Where(Expression<Func<TReturnType, bool>> predicate)
        {
            return Create(_fieldName, ctx => _resolver(ctx).Where(predicate), _condition, _transform, _sorter, _outerProxyAccessor, InnerProxyAccessor);
        }

        public IOrderedQueryableResolver<TEntity, TAnotherReturnType, TExecutionContext> Select<TAnotherReturnType>(
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TAnotherReturnType>> selector,
            Func<IResolveFieldContext, IQueryable<TAnotherReturnType>, IOrderedQueryable<TAnotherReturnType>> sorter)
            where TAnotherReturnType : class
        {
            throw new NotSupportedException();
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
            return new ProxiedAsyncFuncResolver<TEntity, TReturnType>(
                ctx => _batchTaskResolver.Value(ctx)
                    .Then(items => items.SafeNull().SingleOrDefault()),
                ctx => _proxiedBatchTaskResolver.Value(ctx)
                    .Then(items => items.SafeNull().SingleOrDefault()));
        }

        public IResolver<TEntity> FirstOrDefault()
        {
            return new ProxiedAsyncFuncResolver<TEntity, TReturnType>(
                ctx => _batchTaskResolver.Value(ctx)
                    .Then(items => items.SafeNull().FirstOrDefault()),
                ctx => _proxiedBatchTaskResolver.Value(ctx)
                    .Then(items => items.SafeNull().FirstOrDefault()));
        }

        public IResolver<TEntity> AsConnection()
        {
            return new AsyncFuncResolver<TEntity, Connection<Proxy<TReturnType>>>(
                ctx => _batchTaskResolver.Value(ctx).Then(items => Resolvers.Resolve(ctx, (IOrderedQueryable<Proxy<TReturnType>>)items.SafeNull().AsQueryable())),
                ctx => _proxiedBatchTaskResolver.Value(ctx).Then(items => Resolvers.Resolve(ctx, (IOrderedQueryable<Proxy<TReturnType>>)items.SafeNull().AsQueryable())));
        }

        protected virtual IOrderedQueryableResolver<TEntity, TReturnType, TExecutionContext> Create(
            string fieldName,
            Func<IResolveFieldContext, IQueryable<TReturnType>> resolver,
            Expression<Func<TEntity, TReturnType, bool>> condition,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> transform,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IOrderedQueryable<TReturnType>> sorter,
            IProxyAccessor<TEntity, TExecutionContext> outerProxyAccessor,
            IProxyAccessor<TReturnType, TExecutionContext> innerProxyAccessor)
        {
            return new OrderedQueryableAsyncFuncResolver<TEntity, TReturnType, TExecutionContext>(fieldName, resolver, condition, transform, sorter, outerProxyAccessor, innerProxyAccessor);
        }

        protected virtual Expression<Func<TReturnType, Proxy<TReturnType>>> Transform(IResolveFieldContext context)
        {
            var fieldNames = GetQueriedFields(context);
            var lambda = InnerProxyAccessor.CreateSelectorExpression(fieldNames);

            var ctx = context.GetUserContext<TExecutionContext>();
            return lambda.BindFirstParameter(ctx);
        }

        protected virtual IEnumerable<string> GetQueriedFields(IResolveFieldContext context)
        {
            return context.GetQueriedFields();
        }

        private Func<IResolveFieldContext, IDataLoader<TEntity, IGrouping<TEntity, Proxy<TReturnType>>>> GetBatchTaskResolver(
            Func<IResolveFieldContext, IQueryable<TReturnType>> resolver,
            Expression<Func<TEntity, TReturnType, bool>> condition,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> queryTransform,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IOrderedQueryable<TReturnType>> sorter)
        {
            if (!ExpressionHelpers.TryFactorizeCondition(condition, out var factorizationResult))
            {
                throw new ArgumentException($"Cannot translate condition {condition}.", nameof(condition));
            }

            var outerExpression = factorizationResult.LeftExpression;
            var innerExpression = factorizationResult.RightExpression;
            var rightCondition = factorizationResult.RightCondition;

            return context =>
            {
                var result = context
                    .Get<TEntity, TReturnType, Proxy<TReturnType>>(
                        ctx => sorter(ctx, queryTransform(ctx, resolver(ctx))),
                        Transform,
                        outerExpression,
                        innerExpression,
                        InnerProxyAccessor.CreateHooksExecuter(context.GetUserContext<TExecutionContext>()));

                return result;
            };
        }

        private Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, IGrouping<Proxy<TEntity>, Proxy<TReturnType>>>> GetProxiedBatchTaskResolver(
            Func<IResolveFieldContext, IQueryable<TReturnType>> resolver,
            Expression<Func<TEntity, TReturnType, bool>> condition,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> queryTransform,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IOrderedQueryable<TReturnType>> sorter)
        {
            if (!ExpressionHelpers.TryFactorizeCondition(condition, out var factorizationResult))
            {
                throw new ArgumentException($"Cannot translate condition {condition}.", nameof(condition));
            }

            var outerExpression = new Lazy<LambdaExpression>(() => _outerProxyAccessor.GetProxyExpression(factorizationResult.LeftExpression));
            var innerExpression = factorizationResult.RightExpression;
            var rightCondition = factorizationResult.RightCondition;

            return context =>
            {
                var result = context
                    .Get<Proxy<TEntity>, TReturnType, Proxy<TReturnType>>(
                        ctx => sorter(ctx, queryTransform(ctx, resolver(ctx))),
                        Transform,
                        outerExpression.Value,
                        innerExpression,
                        InnerProxyAccessor.CreateHooksExecuter(context.GetUserContext<TExecutionContext>()));

                return result;
            };
        }
    }
}
