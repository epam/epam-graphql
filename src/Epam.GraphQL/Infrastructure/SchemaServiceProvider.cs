// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Mutation;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

namespace Epam.GraphQL.Infrastructure
{
    internal class SchemaServiceProvider<TExecutionContext> : IServiceProvider
    {
        private readonly ConcurrentDictionary<Type, object> _cache = new();
        private readonly Lazy<RelationRegistry<TExecutionContext>> _registry;
        private readonly Lazy<SubmitInputTypeRegistry<TExecutionContext>> _submitInputTypeRegistry;
        private readonly Lazy<IEnumerable<IGraphTypeMappingProvider>> _graphTypeMappingProvider;

        public SchemaServiceProvider()
        {
            _registry = new Lazy<RelationRegistry<TExecutionContext>>(() => new RelationRegistry<TExecutionContext>(this));
            _submitInputTypeRegistry = new Lazy<SubmitInputTypeRegistry<TExecutionContext>>(() => new SubmitInputTypeRegistry<TExecutionContext>(_registry.Value));
            _graphTypeMappingProvider = new Lazy<IEnumerable<IGraphTypeMappingProvider>>(() => new List<IGraphTypeMappingProvider>(new[]
                {
                    new AutoRegisteringGraphTypeMappingProvider(),
                }));
        }

        public object GetService(Type type)
        {
            if (type == typeof(RelationRegistry<TExecutionContext>))
            {
                return _registry.Value;
            }

            if (type == typeof(SubmitInputTypeRegistry<TExecutionContext>))
            {
                return _submitInputTypeRegistry.Value;
            }

            if (type == typeof(IEnumerable<IGraphTypeMappingProvider>))
            {
                return _graphTypeMappingProvider.Value;
            }

            Guards.ThrowInvalidOperationIf(!typeof(IGraphType).IsAssignableFrom(type), $"Cannot resolve type {type}");

            return GetObject(type);
        }

        private object GetObject(Type type)
        {
            return _cache.GetOrAdd(type, CreateInstance);
        }

        private object CreateInstance(Type type)
        {
            return type.CreateInstanceAndHoistBaseException(
                type.GetConstructors()
                    .SelectMany(x => x.GetParameters())
                    .Select(x => this.GetRequiredService(x.ParameterType))
                    .ToArray());
        }
    }
}
