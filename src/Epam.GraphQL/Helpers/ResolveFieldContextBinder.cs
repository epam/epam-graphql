// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using Epam.GraphQL.Extensions;
using GraphQL;

#nullable enable

namespace Epam.GraphQL.Helpers
{
    internal class ResolveFieldContextBinder
    {
        private readonly Dictionary<(IResolveFieldContext Context, Delegate Delegate), object> _contexts = new(
            new ValueTupleEqualityComparer<IResolveFieldContext, Delegate>(ResolveFieldContextEqualityComparer.Instance));

        public TResult Bind<TResult>(IResolveFieldContext context, Func<IResolveFieldContext, TResult> func) => (TResult)_contexts.GetOrAdd((context, func), key =>
        {
            var context = key.Context;
            var func = (Func<IResolveFieldContext, TResult>)key.Delegate;
            TResult result = func(context)!;

            return result;
        });

        public Func<T, TResult> Bind<T, TResult>(IResolveFieldContext context, Func<IResolveFieldContext, T, TResult> func) => (Func<T, TResult>)_contexts.GetOrAdd((context, func), key =>
        {
            var context = key.Context;
            var func = (Func<IResolveFieldContext, T, TResult>)key.Delegate;
            Func<T, TResult> result = arg => func(context, arg);

            return result;
        });
    }
}
