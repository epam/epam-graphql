// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Loaders;
using GraphQL.Types;

namespace Epam.GraphQL.Mutation
{
    internal class SubmitInputTypeRegistryRecord<TExecutionContext>
    {
        private readonly RelationRegistry<TExecutionContext> _registry;

        private SubmitInputTypeRegistryRecord(
            RelationRegistry<TExecutionContext> registry,
            Type configuratorType,
            Type entityType,
            string fieldName,
            Type idType)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            ConfiguratorType = configuratorType;
            EntityType = entityType;
            FieldName = fieldName;
            IdType = idType;
        }

        public Type ConfiguratorType { get; }

        public Type EntityType { get; }

        public Type IdType { get; }

        public string FieldName { get; }

        public Type InputType
        {
            get
            {
                var itemType = _registry.GetInputEntityGraphType(ConfiguratorType, EntityType);
                return typeof(ListGraphType<>).MakeGenericType(itemType);
            }
        }

        public Type OutputType
        {
            get
            {
                var itemType = _registry.GetSubmitOutputItemGraphType(ConfiguratorType, EntityType, IdType);
                return typeof(ListGraphType<>).MakeGenericType(itemType);
            }
        }

        public static SubmitInputTypeRegistryRecord<TExecutionContext> Create<TConfigurator, TEntity, TId>(string fieldName, RelationRegistry<TExecutionContext> registry)
            where TConfigurator : MutableLoader<TEntity, TId, TExecutionContext>
            where TEntity : class
        {
            return new SubmitInputTypeRegistryRecord<TExecutionContext>(
                registry: registry,
                configuratorType: typeof(TConfigurator),
                entityType: typeof(TEntity),
                fieldName: fieldName,
                idType: typeof(TId));
        }

        public override bool Equals(object obj) => Equals(obj as SubmitInputTypeRegistryRecord<TExecutionContext>);

        public bool Equals(SubmitInputTypeRegistryRecord<TExecutionContext>? obj) => obj != null
            && obj.ConfiguratorType == ConfiguratorType
            && obj.EntityType == EntityType
            && obj.IdType == IdType
            && obj.FieldName == FieldName;

        public override int GetHashCode() => HashCode.Combine(ConfiguratorType, EntityType, IdType, FieldName);
    }
}
