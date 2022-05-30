// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Builders.Loader;

namespace Epam.GraphQL.Diagnostics
{
    internal interface IResolvedChainConfigurationContext : IChainConfigurationContext
    {
        IChainArgumentConfigurationContext Resolver { get; }

        new IResolvedChainConfigurationContext Argument(Delegate arg);

        new IResolvedChainConfigurationContext OptionalArgument(Delegate? arg);

        new IInlinedResolvedChainConfigurationContext OptionalArgument<TReturnType, TExecutionContext>(Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? arg)
            where TReturnType : class;
    }
}
