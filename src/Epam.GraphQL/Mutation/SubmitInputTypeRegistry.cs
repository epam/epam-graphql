// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Loaders;

#nullable enable

namespace Epam.GraphQL.Mutation
{
    internal class SubmitInputTypeRegistry<TExecutionContext>
    {
        private readonly Dictionary<Type, Dictionary<string, SubmitInputTypeRegistryRecord<TExecutionContext>>> _map = new();
        private readonly RelationRegistry<TExecutionContext> _registry;

        public SubmitInputTypeRegistry(RelationRegistry<TExecutionContext> registry)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }

        public IDictionary<string, Type> GetInputTypeMap(Type mutationType)
        {
            if (!_map.TryGetValue(mutationType, out Dictionary<string, SubmitInputTypeRegistryRecord<TExecutionContext>> subMap))
            {
                subMap = new Dictionary<string, SubmitInputTypeRegistryRecord<TExecutionContext>>();
                _map[mutationType] = subMap;
            }

            return subMap.ToDictionary(kv => kv.Key, kv => typeof(List<>).MakeGenericType(kv.Value.EntityType));
        }

        public void Register<TLoader, TEntity, TId>(Type mutationType, string fieldName)
            where TLoader : MutableLoader<TEntity, TId, TExecutionContext>
            where TEntity : class
        {
            var newRecord = SubmitInputTypeRegistryRecord<TExecutionContext>.Create<TLoader, TEntity, TId>(fieldName, _registry);

            if (!_map.TryGetValue(mutationType, out Dictionary<string, SubmitInputTypeRegistryRecord<TExecutionContext>> subMap))
            {
                subMap = new Dictionary<string, SubmitInputTypeRegistryRecord<TExecutionContext>>();
                _map[mutationType] = subMap;
            }

            if (subMap.TryGetValue(fieldName, out var record))
            {
                if (!record.Equals(newRecord))
                {
                    throw new ArgumentException("An item with the same key has been already added");
                }
            }
            else
            {
                subMap.Add(fieldName, newRecord);
            }
        }

        public void Register(Type mutationType, string fieldName, Type loaderType, Type entityType, Type idType)
        {
            var registerMethodInfo = GetType().GetGenericMethod(nameof(Register), new[] { loaderType, entityType, idType }, new[] { typeof(Type), typeof(string) });
            registerMethodInfo.InvokeAndHoistBaseException(this, mutationType, fieldName);
        }

        public void ForEach(Type mutationType, Action<SubmitInputTypeRegistryRecord<TExecutionContext>> action)
        {
            foreach (var record in _map[mutationType])
            {
                action(record.Value);
            }
        }

        public Type GetEntityTypeByFieldName(Type mutationType, string fieldName)
        {
            if (_map.TryGetValue(mutationType, out var subMap))
            {
                if (subMap.TryGetValue(fieldName, out var record))
                {
                    return record.EntityType;
                }
            }

            throw new ArgumentException("Cannot find item");
        }

        public Type GetLoaderTypeByFieldName(Type mutationType, string fieldName)
        {
            if (_map.TryGetValue(mutationType, out var subMap))
            {
                if (subMap.TryGetValue(fieldName, out var record))
                {
                    return record.ConfiguratorType;
                }
            }

            throw new ArgumentException("Cannot find item");
        }

        public Type GetLoaderTypeByEntityType(Type mutationType, Type entityType)
        {
            if (_map.TryGetValue(mutationType, out var subMap))
            {
                foreach (var kv in subMap)
                {
                    if (kv.Value.EntityType.IsAssignableFrom(entityType))
                    {
                        return kv.Value.ConfiguratorType;
                    }
                }
            }

            throw new ArgumentException("Cannot find item");
        }

        public bool IsRegistered(Type mutationType, Type entityType)
        {
            if (_map.TryGetValue(mutationType, out var subMap))
            {
                foreach (var kv in subMap)
                {
                    if (kv.Value.EntityType.IsAssignableFrom(entityType))
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        public string GetFieldNameByEntityType(Type mutationType, Type entityType)
        {
            if (_map.TryGetValue(mutationType, out var subMap))
            {
                foreach (var kv in subMap)
                {
                    if (kv.Value.EntityType.IsAssignableFrom(entityType))
                    {
                        return kv.Key;
                    }
                }
            }

            throw new ArgumentException("Cannot find item");
        }
    }
}
