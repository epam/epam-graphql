// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Extensions;
using GraphQL.Types;

#nullable enable

namespace Epam.GraphQL.Configuration.Implementations.Descriptors
{
    internal class ObjectGraphTypeDescriptor<TReturnType, TExecutionContext> : IGraphTypeDescriptor<TReturnType, TExecutionContext>
        where TReturnType : class
    {
        private readonly Lazy<(IGraphType? GraphType, Type? Type)> _type;

        public ObjectGraphTypeDescriptor(IField<TExecutionContext> parent, RelationRegistry<TExecutionContext> registry, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build, bool isInput)
        {
            var resolver = registry.Register(parent, build, isInput);
            Configurator = resolver.ResolveConfigurator();

            _type = new Lazy<(IGraphType?, Type?)>(() => ResolveType(parent, registry, build, isInput));
        }

        public IGraphType? GraphType => _type.Value.GraphType;

        public Type? Type => _type.Value.Type;

        public IObjectGraphTypeConfigurator<TReturnType, TExecutionContext> Configurator { get; }

        IObjectGraphTypeConfigurator<TExecutionContext> IGraphTypeDescriptor<TExecutionContext>.Configurator => Configurator;

        public void Validate()
        {
            if (Type == null && GraphType == null)
            {
                throw new InvalidOperationException($"The type: {typeof(TReturnType).HumanizedName()} cannot be coerced effectively to a GraphQL type");
            }
        }

        private static (IGraphType? GraphType, Type? Type) ResolveType(IField<TExecutionContext> parent, RelationRegistry<TExecutionContext> registry, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build, bool isInputField)
        {
            if (build != null)
            {
                var resolver = registry.Register(parent, build, isInputField);
                return resolver.Resolve();
            }

            return (null, registry.GenerateGraphType<TReturnType>(isInputField));
        }
    }
}
