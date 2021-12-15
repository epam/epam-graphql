// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Helpers;

#nullable enable

namespace Epam.GraphQL.Configuration.Implementations
{
    internal abstract class LoadEntityHook<TEntity, TExecutionContext>
    {
        public abstract void Execute(TExecutionContext executionContext, Proxy<TEntity> proxy);
    }

    internal class LoadEntityHook<TEntity, TEntityProxy, TExecutionContext> : LoadEntityHook<TEntity, TExecutionContext>
    {
        private readonly ProxyAccessor<TEntity, TExecutionContext> _proxyAccessor;
        private readonly Expression<Func<TEntity, TEntityProxy>> _proxyExpression;
        private readonly Action<TExecutionContext, TEntityProxy> _hook;

        public LoadEntityHook(ProxyAccessor<TEntity, TExecutionContext> proxyAccessor, Expression<Func<TEntity, TEntityProxy>> proxyExpression, Action<TExecutionContext, TEntityProxy> hook)
        {
            _proxyAccessor = proxyAccessor;
            _proxyExpression = proxyExpression;
            _hook = hook;
        }

        public override void Execute(TExecutionContext executionContext, Proxy<TEntity> proxy)
        {
            var expr = (Expression<Func<Proxy<TEntity>, TEntityProxy>>)_proxyAccessor.GetProxyExpression(_proxyExpression);
            var getter = expr.Compile();
            _hook(executionContext, getter(proxy));
        }

        public override bool Equals(object obj)
        {
            return obj is LoadEntityHook<TEntity, TEntityProxy, TExecutionContext> loadEntityHook
                && ExpressionEqualityComparer.Instance.Equals(_proxyExpression, loadEntityHook._proxyExpression)
                && _hook == loadEntityHook._hook;
        }

        public override int GetHashCode()
        {
            var hashCode = default(HashCode);
            hashCode.Add(_proxyExpression, ExpressionEqualityComparer.Instance);
            hashCode.Add(_hook);
            return hashCode.ToHashCode();
        }
    }
}
