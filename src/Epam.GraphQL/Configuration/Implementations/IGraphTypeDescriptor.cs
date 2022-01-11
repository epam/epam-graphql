// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using GraphQL.Types;

#nullable enable

namespace Epam.GraphQL.Configuration.Implementations
{
    internal interface IGraphTypeDescriptor<TExecutionContext>
    {
        IGraphType? GraphType { get; }

        Type? Type { get; }

        IObjectGraphTypeConfigurator<TExecutionContext>? Configurator { get; }

        void Validate();
    }

    internal interface IGraphTypeDescriptor<TType, TExecutionContext> : IGraphTypeDescriptor<TExecutionContext>
    {
        new IObjectGraphTypeConfigurator<TType, TExecutionContext>? Configurator { get; }
    }
}
