// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using Epam.GraphQL.Helpers;

#nullable enable

namespace Epam.GraphQL.Configuration.Implementations
{
    internal class LoaderHooksExecuter<T, TExecutionContext> : ILoaderHooksExecuter<Proxy<T>>
    {
        private readonly IEnumerable<LoadEntityHook<T, TExecutionContext>> _hooks;
        private readonly TExecutionContext _executionContext;

        public LoaderHooksExecuter(IEnumerable<LoadEntityHook<T, TExecutionContext>> hooks, TExecutionContext executionContext)
        {
            _hooks = hooks;
            _executionContext = executionContext;
        }

        public void Execute(Proxy<T> entity)
        {
            foreach (var hook in _hooks)
            {
                hook.Execute(_executionContext, entity);
            }
        }
    }

    internal class LoaderHooksExecuter<T, TReturnType, TExecutionContext> : ILoaderHooksExecuter<Tuple<Proxy<T>, TReturnType>>
    {
        private readonly ILoaderHooksExecuter<Proxy<T>>? _executer;

        public LoaderHooksExecuter(TExecutionContext context, IProxyAccessor<T, TExecutionContext> proxyAccessor)
        {
            _executer = proxyAccessor.CreateHooksExecuter(context);
        }

        public void Execute(Tuple<Proxy<T>, TReturnType> entity)
        {
            _executer?.Execute(entity.Item1);
        }
    }
}
