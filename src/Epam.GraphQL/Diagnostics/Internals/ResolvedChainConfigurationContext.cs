// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Builders.Loader;

namespace Epam.GraphQL.Diagnostics.Internals
{
    internal class ResolvedChainConfigurationContext : ChainConfigurationContext, IResolvedChainConfigurationContext
    {
        public ResolvedChainConfigurationContext(ChainConfigurationContext toClone, Func<ConfigurationContext, IConfigurationContext> additionalChildFactory)
            : base(toClone, additionalChildFactory)
        {
            Resolver = (IChainArgumentConfigurationContext)Children[Children.Count - 1];
        }

        protected ResolvedChainConfigurationContext(ResolvedChainConfigurationContext toClone, Func<ConfigurationContext, IConfigurationContext> additionalChildFactory)
            : base(toClone, additionalChildFactory)
        {
            Resolver = toClone.Resolver;
        }

        public IChainArgumentConfigurationContext Resolver { get; }

        IResolvedChainConfigurationContext IResolvedChainConfigurationContext.Argument(Delegate arg)
        {
            return (IResolvedChainConfigurationContext)Argument(arg);
        }

        IResolvedChainConfigurationContext IResolvedChainConfigurationContext.OptionalArgument(Delegate? arg)
        {
            return (IResolvedChainConfigurationContext)OptionalArgument(arg);
        }

        IInlinedResolvedChainConfigurationContext IResolvedChainConfigurationContext.OptionalArgument<TReturnType, TExecutionContext>(Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? arg)
        {
            return new InlinedResolvedChainConfigurationContext(this, parent => arg == null
                ? ChainArgumentConfigurationContext.Create((ChainConfigurationContext)parent, arg, optional: true)
                : ChainArgumentConfigurationContext.Create((ChainConfigurationContext)parent, arg));
        }
    }
}
