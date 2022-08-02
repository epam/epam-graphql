// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Mutation
{
    internal class SubmitInputTypeRegistry<TExecutionContext>
    {
        private static MethodInfo? _registerMethodInfo;
        private readonly Dictionary<string, SubmitInputTypeRegistryRecord<TExecutionContext>> _map = new();
        private readonly RelationRegistry<TExecutionContext> _registry;

        public SubmitInputTypeRegistry(RelationRegistry<TExecutionContext> registry)
        {
            _registry = registry;
        }

        public IDictionary<string, Type> GetInputTypeMap()
        {
            return _map.ToDictionary(kv => kv.Key, kv => typeof(List<>).MakeGenericType(kv.Value.EntityType));
        }

        public void Register<TLoader, TEntity, TId>(string fieldName)
            where TLoader : MutableLoader<TEntity, TId, TExecutionContext>, new()
            where TEntity : class
        {
            var newRecord = SubmitInputTypeRegistryRecord<TExecutionContext>.Create<TLoader, TEntity, TId>(fieldName, _registry);

            if (_map.TryGetValue(fieldName, out var record))
            {
                if (!record.Equals(newRecord))
                {
                    throw new ArgumentException("An item with the same key has been already added");
                }
            }
            else
            {
                _map.Add(fieldName, newRecord);
            }
        }

        public void Register(string fieldName, Type loaderType, Type entityType, Type idType)
        {
            _registerMethodInfo ??= ReflectionHelpers.GetMethodInfo<string>(Register<DummyMutableLoader<TExecutionContext>, object, int>);
            var registerMethodInfo = _registerMethodInfo.MakeGenericMethod(loaderType, entityType, idType);
            registerMethodInfo.InvokeAndHoistBaseException(this, fieldName);
        }

        public void ForEach(Action<SubmitInputTypeRegistryRecord<TExecutionContext>> action)
        {
            foreach (var record in _map)
            {
                action(record.Value);
            }
        }

        public Type GetEntityTypeByFieldName(string fieldName)
        {
            if (_map.TryGetValue(fieldName, out var record))
            {
                return record.EntityType;
            }

            throw new ArgumentException("Cannot find item");
        }

        public Type GetLoaderTypeByFieldName(string fieldName)
        {
            if (_map.TryGetValue(fieldName, out var record))
            {
                return record.ConfiguratorType;
            }

            throw new ArgumentException("Cannot find item");
        }

        public IMutableLoader<TExecutionContext> GetMutableLoaderByFieldName(string fieldName)
        {
            if (_map.TryGetValue(fieldName, out var record))
            {
                return record.MutableLoader;
            }

            throw new ArgumentException("Cannot find item");
        }

        public IMutableLoader<TExecutionContext> GetMutableLoaderByEntityType(Type entityType)
        {
            foreach (var kv in _map)
            {
                if (kv.Value.EntityType.IsAssignableFrom(entityType))
                {
                    return kv.Value.MutableLoader;
                }
            }

            throw new ArgumentException("Cannot find item");
        }

        public bool IsRegistered<TEntity>()
        {
            foreach (var kv in _map)
            {
                if (kv.Value.EntityType.IsAssignableFrom(typeof(TEntity)))
                {
                    return true;
                }
            }

            return false;
        }

        public string GetFieldNameByEntityType(Type entityType)
        {
            foreach (var kv in _map)
            {
                if (kv.Value.EntityType.IsAssignableFrom(entityType))
                {
                    return kv.Key;
                }
            }

            throw new ArgumentException("Cannot find item");
        }
    }
}
