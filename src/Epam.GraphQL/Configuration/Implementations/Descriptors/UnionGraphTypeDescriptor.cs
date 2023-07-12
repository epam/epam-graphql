// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Helpers;
using GraphQL.Types;

namespace Epam.GraphQL.Configuration.Implementations.Descriptors
{
    internal class UnionGraphTypeDescriptor<TExecutionContext> : IGraphTypeDescriptor<TExecutionContext>
    {
        private readonly List<IGraphTypeDescriptor<TExecutionContext>> _graphTypes;
        private readonly List<Type> _types;

        public UnionGraphTypeDescriptor(
            IField<TExecutionContext> field,
            IEnumerable<IGraphTypeDescriptor<TExecutionContext>> graphTypes,
            IEnumerable<Type> types)
        {
            _graphTypes = new List<IGraphTypeDescriptor<TExecutionContext>>(graphTypes);
            _types = new List<Type>(types);
            FieldType = ReflectionHelpers.GetTheBestCommonBaseType(_types);
            Name = field.Parent.GetGraphQLTypeName(FieldType, null, field);
        }

        public Type FieldType { get; }

        public IGraphType GraphType
        {
            get
            {
                var unionType = new UnionGraphType
                {
                    Name = Name,
                };

                foreach (var type in _graphTypes)
                {
                    if (type.GraphType != null)
                    {
                        unionType.AddPossibleType((IObjectGraphType)type.GraphType);
                    }
                    else
                    {
                        Guards.AssertIfNull(type.Type);
                        unionType.Type(type.Type);
                    }
                }

                return unionType;
            }
        }

        public Type? Type => null;

        public IObjectGraphTypeConfigurator<TExecutionContext>? Configurator => null;

        public string Name { get; }

        public UnionGraphTypeDescriptor<TExecutionContext> Add(
            IField<TExecutionContext> field,
            IGraphTypeDescriptor<TExecutionContext> graphType,
            Type type)
        {
            return new UnionGraphTypeDescriptor<TExecutionContext>(
                field,
                _graphTypes.Concat(Enumerable.Repeat(graphType, 1)),
                _types.Concat(Enumerable.Repeat(type, 1)));
        }

        public void Validate(IChainConfigurationContext configurationContext)
        {
            foreach (var type in _graphTypes)
            {
                type.Validate(configurationContext);
            }
        }
    }
}
