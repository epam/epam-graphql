// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;

namespace Epam.GraphQL.Diagnostics
{
    internal interface IInlinedChainConfigurationContext : IChainConfigurationContext
    {
        IChainArgumentConfigurationContext Build { get; }

        new IInlinedChainConfigurationContext Argument(Delegate arg);

        new IInlinedChainConfigurationContext OptionalArgument(Delegate? arg);
    }
}
