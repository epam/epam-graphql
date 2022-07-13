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

    internal class LoaderHooksExecuter<T, TTransformed, TReturnType, TExecutionContext> : ILoaderHooksExecuter<Tuple<TTransformed, TReturnType>>
    {
        private readonly ILoaderHooksExecuter<TTransformed>? _executer;

        public LoaderHooksExecuter(IResolveFieldContext context, IProxyAccessor<T, TTransformed, TExecutionContext> proxyAccessor)
        {
            _executer = proxyAccessor.CreateHooksExecuter(context);
        }

        public IDataLoader<TKey, TKey> Execute<TKey>(Func<TKey, Tuple<TTransformed, TReturnType>> key)
        {
            Guards.ThrowNotSupportedIf(_executer == null);

            return _executer.Execute<TKey>(item => key(item).Item1);
        }
    }
}
