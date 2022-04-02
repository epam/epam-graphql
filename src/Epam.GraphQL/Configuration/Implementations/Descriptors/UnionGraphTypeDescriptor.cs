// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Epam.GraphQL.Diagnostics;
using GraphQL.Types;

namespace Epam.GraphQL.Configuration.Implementations.Descriptors
{
    internal class UnionGraphTypeDescriptor<TExecutionContext> : IGraphTypeDescriptor<TExecutionContext>
    {
        private readonly List<IGraphTypeDescriptor<TExecutionContext>> _types = new();

        public IGraphType GraphType
        {
            get
            {
                var unionType = new UnionGraphType
                {
                    Name = Name,
                };

                foreach (var type in _types)
                {
                    if (type.GraphType != null)
                    {
                        unionType.AddPossibleType((IObjectGraphType)type.GraphType);
                    }
                    else
                    {
                        unionType.Type(type.Type);
                    }
                }

                return unionType;
            }
        }

        public Type? Type => null;

        public IObjectGraphTypeConfigurator<TExecutionContext>? Configurator => null;

        [NotNull]
        public string? Name { get; set; }

        public void Add(IGraphTypeDescriptor<TExecutionContext> graphType)
        {
            _types.Add(graphType);
        }

        public void Validate(FieldConfigurationContext configurationContext)
        {
            foreach (var type in _types)
            {
                type.Validate(configurationContext);
            }
        }
    }
}
