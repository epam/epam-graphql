// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Helpers;
using GraphQL;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal class FuncResolver<TReturnType, TTransformedReturnType, TExecutionContext> : IFieldResolver
    {
        private readonly IProxyAccessor<TReturnType, TTransformedReturnType, TExecutionContext> _proxyAccessor;
        private readonly Func<IResolveFieldContext, TTransformedReturnType> _resolver;

        public FuncResolver(
            IProxyAccessor<TReturnType, TTransformedReturnType, TExecutionContext> proxyAccessor,
            Func<IResolveFieldContext, TTransformedReturnType> resolver)
        {
            _proxyAccessor = proxyAccessor;
            _resolver = resolver;
        }

        public object? Resolve(IResolveFieldContext context)
        {
            var executer = _proxyAccessor.CreateHooksExecuter(context);

            if (executer != null)
            {
                return executer
                    .Execute(FuncConstants<TTransformedReturnType>.Identity)
                    .LoadAsync(_resolver(context));
            }

            return _resolver(context);
        }
    }
}
