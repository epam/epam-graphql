// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.TaskBatcher;
using GraphQL;
using GraphQL.DataLoader;

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal class ProxiedFuncResolver<TEntity, TReturnType, TExecutionContext> : IResolver<TEntity>
        where TEntity : class
    {
        private readonly IProxyAccessor<TReturnType, TExecutionContext> _proxyAccessor;
        private readonly Func<IResolveFieldContext, Proxy<TReturnType>> _resolver;

        public ProxiedFuncResolver(
            IProxyAccessor<TReturnType, TExecutionContext> proxyAccessor,
            Func<IResolveFieldContext, Proxy<TReturnType>> resolver)
        {
            _proxyAccessor = proxyAccessor;
            _resolver = resolver;
        }

        public IDataLoader<TEntity, object?> GetBatchLoader(IResolveFieldContext context)
        {
            return BatchLoader.FromResult<TEntity, object?>(Resolve(context));
        }

        public IDataLoader<Proxy<TEntity>, object?> GetProxiedBatchLoader(IResolveFieldContext context)
        {
            return BatchLoader.FromResult<Proxy<TEntity>, object?>(Resolve(context));
        }

        public object Resolve(IResolveFieldContext context)
        {
            var executer = _proxyAccessor.CreateHooksExecuter(context);

            if (executer != null)
            {
                return executer
                    .Execute(FuncConstants<Proxy<TReturnType>>.Identity)
                    .LoadAsync(_resolver(context));
            }

            return _resolver(context);
        }
    }
}
