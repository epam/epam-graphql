// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Builders.Loader.Implementations;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Extensions;
using GraphQL.Types;

namespace Epam.GraphQL.Configuration.Implementations.Descriptors
{
    internal class ObjectGraphTypeDescriptor<TReturnType, TExecutionContext> : IGraphTypeDescriptor<TReturnType, TExecutionContext>
        where TReturnType : class
    {
        private readonly Lazy<(IGraphType? GraphType, Type? Type)> _type;
        private readonly IInlineGraphTypeResolver<TReturnType, TExecutionContext> _resolver;

        public ObjectGraphTypeDescriptor(
            IField<TExecutionContext> parent,
            RelationRegistry<TExecutionContext> registry,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build,
            IConfigurationContext configurationContext,
            bool isInput)
        {
            _resolver = registry.Register(parent, build, configurationContext, isInput);
            Configurator = _resolver.ResolveConfigurator();

            _type = new Lazy<(IGraphType?, Type?)>(() => ResolveType(registry, build, isInput));
        }

        public IGraphType? GraphType => _type.Value.GraphType;

        public Type? Type => _type.Value.Type;

        public IObjectGraphTypeConfigurator<TReturnType, TExecutionContext> Configurator { get; }

        IObjectGraphTypeConfigurator<TExecutionContext> IGraphTypeDescriptor<TExecutionContext>.Configurator => Configurator;

        public void Validate(MethodCallConfigurationContext configurationContext)
        {
            _resolver.Validate(configurationContext);

            configurationContext.AddErrorIf(
                Type == null && GraphType == null,
                $"The type: {typeof(TReturnType).HumanizedName()} cannot be coerced effectively to a GraphQL type",
                configurationContext);
        }

        private (IGraphType? GraphType, Type? Type) ResolveType(
            RelationRegistry<TExecutionContext> registry,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build,
            bool isInputField)
        {
            if (build != null)
            {
                return _resolver.Resolve();
            }

            return (null, registry.GenerateGraphType<TReturnType>(isInputField));
        }
    }
}
