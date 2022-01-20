// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.TaskBatcher;
using GraphQL;
using GraphQL.DataLoader;

namespace Epam.GraphQL.Configuration.Implementations
{
    internal abstract class LoadEntityHook<TEntity, TExecutionContext>
    {
        public abstract IDataLoader<TKey, TKey> ExecuteAsync<TKey>(Func<TKey, Proxy<TEntity>> key, IResolveFieldContext context);
    }

    internal class LoadEntityHook<TEntity, TEntityProxy, TExecutionContext> : LoadEntityHook<TEntity, TEntityProxy, TEntityProxy, TExecutionContext>
    {
        public LoadEntityHook(
            ProxyAccessor<TEntity, TExecutionContext> proxyAccessor,
            Expression<Func<TEntity, TEntityProxy>> proxyExpression,
            Action<TExecutionContext, TEntityProxy> hook)
            : base(
                  proxyAccessor,
                  proxyExpression,
                  hook,
                  (ctx, items) => items.ToDictionary(item => item))
        {
        }
    }

    internal class LoadEntityHook<TEntity, TKey, TEntityProxy, TExecutionContext> : LoadEntityHook<TEntity, TExecutionContext>
    {
        private readonly Expression<Func<TEntity, TKey>> _keyExpression;
        private readonly ProxyAccessor<TEntity, TExecutionContext> _proxyAccessor;
        private readonly Action<TExecutionContext, TEntityProxy> _hook;
        private readonly Lazy<Func<Proxy<TEntity>, TKey>> _proxyKeyGetter;
        private readonly Func<IResolveFieldContext, IDataLoader<TKey, TEntityProxy?>> _resolver;
        private readonly Delegate _batchFunc;

        public LoadEntityHook(
            ProxyAccessor<TEntity, TExecutionContext> proxyAccessor,
            Expression<Func<TEntity, TKey>> keyExpression,
            Action<TExecutionContext, TEntityProxy> hook,
            Func<TExecutionContext, IEnumerable<TKey>, IDictionary<TKey, TEntityProxy>> batchFunc)
            : this(
                  proxyAccessor,
                  keyExpression,
                  hook,
                  CreateResolver(batchFunc),
                  batchFunc)
        {
        }

        public LoadEntityHook(
            ProxyAccessor<TEntity, TExecutionContext> proxyAccessor,
            Expression<Func<TEntity, TKey>> keyExpression,
            Action<TExecutionContext, TEntityProxy> hook,
            Func<TExecutionContext, IEnumerable<TKey>, Task<IDictionary<TKey, TEntityProxy>>> batchFunc)
            : this(
                  proxyAccessor,
                  keyExpression,
                  hook,
                  CreateResolver(batchFunc),
                  batchFunc)
        {
        }

        private LoadEntityHook(
            ProxyAccessor<TEntity, TExecutionContext> proxyAccessor,
            Expression<Func<TEntity, TKey>> keyExpression,
            Action<TExecutionContext, TEntityProxy> hook,
            Func<IResolveFieldContext, IDataLoader<TKey, TEntityProxy?>> resolver,
            Delegate batchFunc)
        {
            _proxyAccessor = proxyAccessor;
            _keyExpression = keyExpression;
            _hook = hook;
            _batchFunc = batchFunc;
            _resolver = resolver;
            _proxyAccessor.AddMember(keyExpression);
            _proxyKeyGetter = new Lazy<Func<Proxy<TEntity>, TKey>>(() => ((Expression<Func<Proxy<TEntity>, TKey>>)_proxyAccessor.GetProxyExpression(_keyExpression).CastFirstParamTo<Proxy<TEntity>>()).Compile());
        }

        private Func<Proxy<TEntity>, TKey> KeyGetter => _proxyKeyGetter.Value;

        public override IDataLoader<T, T> ExecuteAsync<T>(Func<T, Proxy<TEntity>> key, IResolveFieldContext context)
        {
            var batcher = context.GetBatcher();

            Func<(T, Proxy<TEntity>), TKey> getterFunc = item => KeyGetter(item.Item2);

            var batchLoader = getterFunc
                .Then(
                    FuncConstants<TKey>.IsNull,
                    BatchLoader.FromResult(FuncConstants<TKey, TEntityProxy?>.DefaultResultFunc),
                    _resolver(context));

            Func<T, (T, Proxy<TEntity>)> func = arg => (arg, key(arg));

            var result = func.Then(batchLoader.Then((proxy, result) =>
            {
                if (result != null)
                {
                    _hook(context.GetUserContext<TExecutionContext>(), result);
                }

                return proxy.Item1;
            }));

            return result;
        }

        public override bool Equals(object obj)
        {
            return obj is LoadEntityHook<TEntity, TKey, TEntityProxy, TExecutionContext> loadEntityHook
                && ExpressionEqualityComparer.Instance.Equals(_keyExpression, loadEntityHook._keyExpression)
                && _hook == loadEntityHook._hook
                && _batchFunc == loadEntityHook._batchFunc;
        }

        public override int GetHashCode()
        {
            var hashCode = default(HashCode);
            hashCode.Add(_keyExpression, ExpressionEqualityComparer.Instance);
            hashCode.Add(_hook);
            hashCode.Add(_batchFunc);
            return hashCode.ToHashCode();
        }

        private static Func<IResolveFieldContext, IDataLoader<TKey, TEntityProxy?>> CreateResolver(Func<TExecutionContext, IEnumerable<TKey>, IDictionary<TKey, TEntityProxy>> batchFunc)
        {
            return context =>
            {
                var batcher = context.GetBatcher();
                return batcher.Get(context.GetPath, context.GetUserContext<TExecutionContext>(), batchFunc);
            };
        }

        private static Func<IResolveFieldContext, IDataLoader<TKey, TEntityProxy?>> CreateResolver(Func<TExecutionContext, IEnumerable<TKey>, Task<IDictionary<TKey, TEntityProxy>>> batchFunc)
        {
            return context =>
            {
                var batcher = context.GetBatcher();
                return batcher.Get(context.GetPath, context.GetUserContext<TExecutionContext>(), batchFunc);
            };
        }
    }
}
