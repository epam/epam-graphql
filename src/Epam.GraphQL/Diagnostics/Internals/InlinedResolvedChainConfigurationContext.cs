// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;

namespace Epam.GraphQL.Diagnostics.Internals
{
    internal class InlinedResolvedChainConfigurationContext : ResolvedChainConfigurationContext,
        IInlinedResolvedChainConfigurationContext
    {
        public InlinedResolvedChainConfigurationContext(ResolvedChainConfigurationContext toClone, Func<ConfigurationContext, IConfigurationContext> additionalChildFactory)
            : base(toClone, additionalChildFactory)
        {
            Build = (IChainArgumentConfigurationContext)Children[Children.Count - 1];
        }

        public IChainArgumentConfigurationContext Build { get; }

        IInlinedChainConfigurationContext IInlinedChainConfigurationContext.OptionalArgument(Delegate? arg)
        {
            return (IInlinedChainConfigurationContext)OptionalArgument(arg);
        }

        IInlinedChainConfigurationContext IInlinedChainConfigurationContext.Argument(Delegate arg)
        {
            return (IInlinedChainConfigurationContext)Argument(arg);
        }
    }
}
