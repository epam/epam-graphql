// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using Epam.GraphQL.Diagnostics;
using GraphQL.Types;

namespace Epam.GraphQL.Configuration.Implementations.Descriptors
{
    internal class ListGraphTypeDescriptor<TExecutionContext> : IGraphTypeDescriptor<TExecutionContext>
    {
        private readonly Lazy<IGraphType?> _graphType;
        private readonly Lazy<Type?> _type;
        private readonly IGraphTypeDescriptor<TExecutionContext> _elementDescriptor;

        public ListGraphTypeDescriptor(IGraphTypeDescriptor<TExecutionContext> elementDescriptor)
        {
            _elementDescriptor = elementDescriptor;
            _graphType = new Lazy<IGraphType?>(() => elementDescriptor.GraphType != null ? new ListGraphType(elementDescriptor.GraphType) : null);
            _type = new Lazy<Type?>(() => elementDescriptor.Type != null ? typeof(ListGraphType<>).MakeGenericType(elementDescriptor.Type) : null);
        }

        public IGraphType? GraphType => _graphType.Value;

        public Type? Type => _type.Value;

        public IObjectGraphTypeConfigurator<TExecutionContext>? Configurator => _elementDescriptor.Configurator;

        public void Validate(MethodCallConfigurationContext configurationContext)
        {
            _elementDescriptor.Validate(configurationContext);
        }
    }

    internal class ListGraphTypeDescriptor<TReturnType, TEnumerable, TExecutionContext> : ListGraphTypeDescriptor<TExecutionContext>, IGraphTypeDescriptor<TEnumerable, TExecutionContext>
        where TEnumerable : IEnumerable<TReturnType>
    {
        public ListGraphTypeDescriptor(IGraphTypeDescriptor<TReturnType, TExecutionContext> elementDescriptor)
            : base(elementDescriptor)
        {
        }

        IObjectGraphTypeConfigurator<TEnumerable, TExecutionContext>? IGraphTypeDescriptor<TEnumerable, TExecutionContext>.Configurator => throw new NotImplementedException();
    }
}
