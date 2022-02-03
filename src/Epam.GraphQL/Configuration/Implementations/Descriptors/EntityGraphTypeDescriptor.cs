// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using GraphQL.Types;

namespace Epam.GraphQL.Configuration.Implementations.Descriptors
{
    internal class EntityGraphTypeDescriptor<TProjection, TReturnType, TExecutionContext> : IGraphTypeDescriptor<TReturnType, TExecutionContext>
        where TReturnType : class
        where TProjection : ProjectionBase<TReturnType, TExecutionContext>, new()
    {
        private readonly Lazy<Type> _type;

        public EntityGraphTypeDescriptor(RelationRegistry<TExecutionContext> registry, bool isInput)
        {
            _type = new Lazy<Type>(() => isInput
                ? registry.GetInputEntityGraphType<TProjection, TReturnType>()
                : registry.GetEntityGraphType<TProjection, TReturnType>());

            var projection = registry.ResolveLoader<TProjection, TReturnType>();
            Configurator = isInput
                ? projection.InputObjectGraphTypeConfigurator
                : projection.ObjectGraphTypeConfigurator;
        }

        public IGraphType? GraphType => null;

        public Type? Type => _type.Value;

        public IObjectGraphTypeConfigurator<TReturnType, TExecutionContext> Configurator { get; }

        IObjectGraphTypeConfigurator<TExecutionContext> IGraphTypeDescriptor<TExecutionContext>.Configurator => Configurator;

        public void Validate()
        {
            Guards.ThrowInvalidOperationIf(Type == null, $"The type: {typeof(TReturnType).HumanizedName()} cannot be coerced effectively to a GraphQL type");
        }
    }
}
