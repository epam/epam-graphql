// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.TaskBatcher;
using GraphQL;
using GraphQL.DataLoader;

namespace Epam.GraphQL.Configuration.Implementations
{
    internal class LoaderHooksExecuter<T, TExecutionContext> : ILoaderHooksExecuter<Proxy<T>>
    {
        private readonly IEnumerable<LoadEntityHook<T, TExecutionContext>> _hooks;
        private readonly IResolveFieldContext _context;

        public LoaderHooksExecuter(IEnumerable<LoadEntityHook<T, TExecutionContext>> hooks, IResolveFieldContext context)
        {
            _hooks = hooks;
            _context = context;
        }

        public IDataLoader<TKey, TKey> Execute<TKey>(Func<TKey, Proxy<T>> key)
        {
            return BatchLoader.WhenAll(_hooks.Select(hook => hook.ExecuteAsync(key, _context)))
                .Then(all => all.First());
        }
    }

    internal class LoaderHooksExecuter<T, TReturnType, TExecutionContext> : ILoaderHooksExecuter<Tuple<Proxy<T>, TReturnType>>
    {
        private readonly ILoaderHooksExecuter<Proxy<T>>? _executer;

        public LoaderHooksExecuter(IResolveFieldContext context, IProxyAccessor<T, TExecutionContext> proxyAccessor)
        {
            _executer = proxyAccessor.CreateHooksExecuter(context);
        }

        public IDataLoader<TKey, TKey> Execute<TKey>(Func<TKey, Tuple<Proxy<T>, TReturnType>> key)
        {
            if (_executer == null)
            {
                throw new NotSupportedException();
            }

            return _executer.Execute<TKey>(item => key(item).Item1);
        }
    }
}
