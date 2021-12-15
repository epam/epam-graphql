// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using GraphQL.Types;

namespace Epam.GraphQL.Configuration.Implementations.Descriptors
{
    internal static class GraphTypeDescriptor
    {
        public static IGraphTypeDescriptor<TExecutionContext> Create<T, TExecutionContext>()
            where T : IGraphType
        {
            return new GraphTypeDescriptor<TExecutionContext>(typeof(T), () => null, null);
        }

        public static IGraphTypeDescriptor<TExecutionContext> Create<TExecutionContext>(Type type)
        {
            return new GraphTypeDescriptor<TExecutionContext>(type, () => null, null);
        }
    }

    internal class GraphTypeDescriptor<TExecutionContext> : IGraphTypeDescriptor<TExecutionContext>
    {
        private readonly Lazy<Type> _type;
        private readonly Lazy<IObjectGraphTypeConfigurator<TExecutionContext>> _configurator;
        private readonly Lazy<IGraphType> _graphType;

        public GraphTypeDescriptor(Type type, IGraphType graphType, IObjectGraphTypeConfigurator<TExecutionContext> configurator)
            : this(() => type, () => graphType, () => configurator)
        {
        }

        public GraphTypeDescriptor(Type type, Func<IGraphType> graphTypeFactory, IObjectGraphTypeConfigurator<TExecutionContext> configurator)
            : this(() => type, graphTypeFactory, () => configurator)
        {
        }

        protected GraphTypeDescriptor(Func<Type> typeFactory, Func<IGraphType> graphTypeFactory, Func<IObjectGraphTypeConfigurator<TExecutionContext>> configuratorFactory)
        {
            _type = new Lazy<Type>(typeFactory);
            _configurator = new Lazy<IObjectGraphTypeConfigurator<TExecutionContext>>(configuratorFactory);
            _graphType = new Lazy<IGraphType>(graphTypeFactory);
        }

        public IGraphType GraphType => _graphType.Value;

        public Type Type => _type.Value;

        public IObjectGraphTypeConfigurator<TExecutionContext> Configurator => _configurator.Value;

        public void Validate()
        {
            if (Type == null && GraphType == null)
            {
                throw new InvalidOperationException($"Type cannot be coerced effectively to a GraphQL type");
            }
        }
    }

    internal class GraphTypeDescriptor<TReturnType, TExecutionContext> : GraphTypeDescriptor<TExecutionContext>, IGraphTypeDescriptor<TReturnType, TExecutionContext>
    {
        public GraphTypeDescriptor(Type type, IGraphType graphType, IObjectGraphTypeConfigurator<TReturnType, TExecutionContext> configurator)
            : base(type, graphType, configurator)
        {
        }

        public GraphTypeDescriptor(Type type, Func<IGraphType> graphTypeFactory, IObjectGraphTypeConfigurator<TReturnType, TExecutionContext> configurator)
            : base(type, graphTypeFactory, configurator)
        {
        }

        public GraphTypeDescriptor(RelationRegistry<TExecutionContext> registry, bool isInput)
            : base(() => registry.GenerateGraphType<TReturnType>(isInput), () => null, () => registry.GetObjectGraphTypeConfigurator(typeof(TReturnType), null))
        {
        }

        IObjectGraphTypeConfigurator<TReturnType, TExecutionContext> IGraphTypeDescriptor<TReturnType, TExecutionContext>.Configurator => (IObjectGraphTypeConfigurator<TReturnType, TExecutionContext>)Configurator;
    }
}
