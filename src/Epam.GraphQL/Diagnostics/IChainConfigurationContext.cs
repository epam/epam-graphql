// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;

namespace Epam.GraphQL.Diagnostics
{
    internal interface IChainConfigurationContext : IChildConfigurationContext
    {
        IChainConfigurationContext Chain(string operation);

        IChainConfigurationContext Chain<T>(string operation);

        IChainConfigurationContext Chain<T1, T2>(string operation);

        IChainConfigurationContext Argument(string arg);

        IChainConfigurationContext Argument(Delegate arg);

        IChainConfigurationContext OptionalArgument(Delegate? arg);

        IChainConfigurationContext OptionalArgument<T>(T[]? arg);

        IResolvedChainConfigurationContext Argument<TKey, TValue>(Func<IEnumerable<TKey>, IEnumerable<KeyValuePair<TKey, TValue>>> arg);

        IResolvedChainConfigurationContext Argument<TContext, TKey, TValue>(Func<TContext, IEnumerable<TKey>, IEnumerable<KeyValuePair<TKey, TValue>>> arg);

        IResolvedChainConfigurationContext Argument<TKey, TValue>(Func<IEnumerable<TKey>, Task<IDictionary<TKey, TValue>>> arg);

        IResolvedChainConfigurationContext Argument<TContext, TKey, TValue>(Func<TContext, IEnumerable<TKey>, Task<IDictionary<TKey, TValue>>> arg);

        IInlinedChainConfigurationContext Argument<TReturnType, TExecutionContext>(Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> arg);

        IInlinedChainConfigurationContext OptionalArgument<TReturnType, TExecutionContext>(Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? arg);

        IChainConfigurationContext Argument(LambdaExpression arg);

        IChainConfigurationContext OptionalArgument(LambdaExpression? arg);
    }
}
