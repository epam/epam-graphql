// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields
{
    internal static class ResolvedFieldResolverFactory
    {
        public static IFieldResolver Create<TReturnType>(
            Func<IResolveFieldContext, TReturnType> resolver)
        {
            return new FuncFieldResolver<object, TReturnType>(resolver);
        }

        public static IFieldResolver Create<TReturnType>(
            Func<IResolveFieldContext, Task<TReturnType>> resolver)
        {
            return new FuncFieldResolver<object, TReturnType>(arg => new ValueTask<TReturnType?>(resolver(arg)));
        }
    }
}
