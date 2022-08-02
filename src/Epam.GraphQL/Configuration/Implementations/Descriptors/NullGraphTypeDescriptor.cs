// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Diagnostics;
using GraphQL.Types;

namespace Epam.GraphQL.Configuration.Implementations.Descriptors
{
    internal class NullGraphTypeDescriptor<TExecutionContext> : IGraphTypeDescriptor<TExecutionContext>
    {
        public IGraphType? GraphType => throw new NotImplementedException();

        public Type? Type => throw new NotImplementedException();

        public IObjectGraphTypeConfigurator<TExecutionContext>? Configurator => throw new NotImplementedException();

        public void Validate(IChainConfigurationContext configurationContext)
        {
            // Do nothing
        }
    }
}
