// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
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

        IInlinedChainConfigurationContext Argument<TReturnType, TExecutionContext>(Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> arg)
            where TReturnType : class;

        IInlinedChainConfigurationContext OptionalArgument<TReturnType, TExecutionContext>(Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? arg)
            where TReturnType : class;

        IChainConfigurationContext Argument(LambdaExpression arg);

        IChainConfigurationContext OptionalArgument(LambdaExpression? arg);
    }
}
