// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Epam.GraphQL.Enums;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Metadata;
using Epam.GraphQL.TaskBatcher;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Types;
using TypeExtensions = Epam.GraphQL.Extensions.TypeExtensions;

namespace Epam.GraphQL.Configuration.Implementations.Relations
{
    internal class Relation<TEntity, TEntityLoader, TChildEntity, TProperty, TChildProperty, TExecutionContext> : IRelation
        where TEntityLoader : Loader<TEntity, TExecutionContext>, new()
        where TEntity : class
    {
        private readonly Func<Type, PropertyInfo> _getPrimaryKeyPropertyInfo;
        private readonly Expression<Func<TEntity, TProperty>> _property;
        private readonly Expression<Func<TEntity, TChildEntity>> _navigationProperty;
        private readonly Expression<Func<TChildEntity, TChildProperty>> _childProperty;
        private readonly Expression<Func<TChildEntity, TEntity>> _childNavigationProperty;
        private readonly RelationType _relationType;
        private readonly TEntityLoader _securityCheckLoader;
        private readonly TEntityLoader _parentLoader;
        private readonly RelationRegistry<TExecutionContext> _registry;

        public Relation(
            RelationRegistry<TExecutionContext> registry,
            Expression<Func<TEntity, TProperty>> property,
            Expression<Func<TEntity, TChildEntity>> navigationProperty,
            Expression<Func<TChildEntity, TChildProperty>> childProperty,
            Expression<Func<TChildEntity, TEntity>> childNavigationProperty,
            Func<Type, PropertyInfo> getPrimaryKeyPropertyInfo,
            RelationType relationType)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _property = property ?? throw new ArgumentNullException(nameof(property));
            _childProperty = childProperty ?? throw new ArgumentNullException(nameof(childProperty));
            _getPrimaryKeyPropertyInfo = getPrimaryKeyPropertyInfo ?? throw new ArgumentNullException(nameof(getPrimaryKeyPropertyInfo));
            _navigationProperty = navigationProperty;
            _childNavigationProperty = childNavigationProperty;
            _relationType = relationType;

            _parentLoader = _registry.ResolveLoader<TEntityLoader, TEntity>();
            if (_relationType == RelationType.Aggregation)
            {
                _securityCheckLoader = _parentLoader;
            }

            PropertyName = property.NameOf();
            ChildPropertyName = childProperty.NameOf();
            EntityType = typeof(TEntity);
            ChildEntityType = typeof(TChildEntity);
            PropertyType = typeof(TProperty);
            ChildPropertyType = typeof(TChildProperty);
            ChildNavigationPropertyName = childNavigationProperty?.GetPropertyInfo()?.Name;
        }

        public Type EntityType { get; set; }

        protected Type ChildEntityType { get; set; }

        protected Type PropertyType { get; set; }

        protected Type ChildPropertyType { get; set; }

        protected string PropertyName { get; set; }

        protected string ChildPropertyName { get; set; }

        protected string ChildNavigationPropertyName { get; set; }

        private bool IsPropertyId => IsPrimaryKey(_property);

        private bool IsChildPropertyId => IsPrimaryKey(_childProperty);

        private Func<TChildEntity, TChildProperty> ChildPropertyGetter => _childProperty.GetGetter();

        private Action<TChildEntity, TChildProperty> ChildPropertySetter => _childProperty.GetSetter();

        private Func<TChildEntity, TEntity> ChildNavigationPropertyGetter => _childNavigationProperty?.GetGetter();

        private Action<TChildEntity, TEntity> ChildNavigationPropertySetter => _childNavigationProperty?.GetSetter();

        private Action<TEntity, TChildEntity> NavigationPropertySetter => _navigationProperty?.GetSetter();

        public override int GetHashCode() => HashCode.Combine(EntityType, ChildEntityType, PropertyType, ChildPropertyType, PropertyName, ChildPropertyName, ChildNavigationPropertyName);

        public override bool Equals(object obj) => Equals(obj as Relation<TEntity, TEntityLoader, TChildEntity, TProperty, TChildProperty, TExecutionContext>);

        public bool Equals(Relation<TEntity, TEntityLoader, TChildEntity, TProperty, TChildProperty, TExecutionContext> obj) => obj != null
            && obj.EntityType == EntityType && obj.ChildEntityType == ChildEntityType
            && obj.PropertyType == PropertyType && obj.PropertyName == PropertyName
            && obj.ChildPropertyType == ChildPropertyType && obj.ChildPropertyName == ChildPropertyName
            && obj.ChildNavigationPropertyName == ChildNavigationPropertyName;

        public override string ToString()
        {
            return $"Relation: type = {typeof(TEntity).HumanizedName()} childType = {typeof(TChildEntity).HumanizedName()} prop = {PropertyName}, childProp {ChildPropertyName}, childNavigationProp {ChildNavigationPropertyName}";
        }

        public bool HasFakePropertyValue(object childEntity, IDictionary<string, object> childPropertyValues)
        {
            if (childEntity is TChildEntity e)
            {
                return HasFakePropertyValue(e);
            }

            return false;
        }

        public void UpdateFakeProperties(object entity, object childEntity, IDictionary<string, object> childPropertyValues, object fakePropertyValue)
        {
            if (entity is TEntity p && childEntity is TChildEntity c && fakePropertyValue is TProperty prop)
            {
                UpdateFakeProperties(p, c, prop);
            }
        }

        public bool CanViewParent(object context, object entity)
        {
            if (entity is TChildEntity e && context is TExecutionContext c)
            {
                return CanViewParent(c, e);
            }

            throw new InvalidOperationException();
        }

        public IDataLoaderResult<bool> CanViewParentAsync(object context, object entity)
        {
            if (entity is TChildEntity e && context is GraphQLContext<TExecutionContext> c)
            {
                return CanViewParentAsync(c, e);
            }

            throw new InvalidOperationException();
        }

        public IDataLoaderResult<bool> CanViewParentAsync(GraphQLContext<TExecutionContext> context, TChildEntity entity)
        {
            if (_relationType == RelationType.Aggregation)
            {
                if (ChildNavigationPropertyGetter != null)
                {
                    var profiler = context.Profiler;

                    using (profiler.Step($"CanViewParentAsync:{typeof(TChildEntity).HumanizedName()}"))
                    {
                        var parent = ChildNavigationPropertyGetter(entity);
                        if (parent != null)
                        {
                            return _securityCheckLoader.CanViewAsync(context, parent);
                        }
                    }
                }

                var prop = ChildPropertyGetter(entity);
                if (prop == null)
                {
                    return new DataLoaderResult<bool>(true);
                }

                if (IsPropertyId)
                {
                    var batcher = context.Batcher;
                    var profiler = context.Profiler;

                    if (typeof(TProperty).IsAssignableFrom(typeof(TChildProperty)))
                    {
                        var castedParentLoader = _securityCheckLoader as IdentifiableLoader<TEntity, TProperty, TExecutionContext>;
                        var factory = BatchHelpers.GetLoaderQueryFactory<TEntityLoader, TEntity, TProperty, TExecutionContext>(
                            () => $"CanViewParentAsync:{typeof(TChildEntity).HumanizedName()}",
                            _securityCheckLoader,
                            castedParentLoader.IdExpression);

                        return factory(profiler, context.QueryExecuter, null, context.ExecutionContext) // TBD hooksExecuter == null here
                            .Then(r => r.SafeNull().Any())
                            .LoadAsync((TProperty)Convert.ChangeType(prop, typeof(TChildProperty), CultureInfo.InvariantCulture));
                    }
                    else if (typeof(TChildProperty).IsAssignableFrom(typeof(TProperty)))
                    {
                        var castedParentLoader = _securityCheckLoader as IdentifiableLoader<TEntity, TProperty, TExecutionContext>;
                        var factory = BatchHelpers.GetLoaderQueryFactory<TEntityLoader, TEntity, TProperty, TExecutionContext>(
                            () => $"CanViewParentAsync:{typeof(TChildEntity).HumanizedName()}",
                            _securityCheckLoader,
                            castedParentLoader.IdExpression);
                        return factory(profiler, context.QueryExecuter, null, context.ExecutionContext) // TBD hooksExecuter == null here
                            .Then(r => r.SafeNull().Any())
                            .LoadAsync((TProperty)Convert.ChangeType(prop, typeof(TProperty), CultureInfo.InvariantCulture));
                    }
                }
            }

            return new DataLoaderResult<bool>(true);
        }

        public ForeignKeyMetadata GetForeignKeyMetadata(IComplexGraphType childGraphType)
        {
            if (IsPropertyId)
            {
                // TODO: This works only in a few cases. Make field registry for every type.
                var graphType = _registry.ResolveObjectGraphTypeWrapper<TEntityLoader, TEntity>();
                var childFieldType = childGraphType.GetField(_childProperty.NameOf().ToCamelCase());
                var fieldType = graphType.GetField(_property.NameOf().ToCamelCase());

                if (fieldType == null)
                {
                    return null;
                }

                return new ForeignKeyMetadata
                {
                    ToType = graphType,
                    ToField = ItemToArray(fieldType),
                    FromField = ItemToArray(childFieldType),
                };
            }

            return null;
        }

        private static T[] ItemToArray<T>(T value)
        {
            if (value != null)
            {
                return new[] { value };
            }

            return null;
        }

        private bool IsPrimaryKey<TType, TProp>(Expression<Func<TType, TProp>> property)
        {
            var propInfo = _getPrimaryKeyPropertyInfo(typeof(TType));
            return propInfo != null && property.IsProperty() && propInfo == property.GetPropertyInfo();
        }

        private bool HasFakePropertyValue(TChildEntity child)
        {
            if (_childNavigationProperty != null && _navigationProperty == null && _childProperty.IsProperty())
            {
                if (_parentLoader is not IMutableLoader<TExecutionContext> mutableLoader)
                {
                    if (_parentLoader is IIdentifiableLoader)
                    {
                        return false;
                    }

                    throw new InvalidOperationException($"Cannot check fakeness of property value. IsFakePropertyValue predicate wasn't provided for Relation: {this}");
                }

                var childPropertyValue = ChildPropertyGetter(child);
                if (TypeExtensions.IsNullable(typeof(TChildProperty)) && childPropertyValue == null)
                {
                    return false;
                }

                return mutableLoader.IsFakeId(childPropertyValue);
            }

            if (_navigationProperty != null && _childNavigationProperty != null && IsChildPropertyId)
            {
                throw new InvalidOperationException($"Cannot check fakeness of property value. Both navigationProperty and reverseNavigationProperty were provided for identity property {_childProperty} of type {typeof(TChildEntity)}. You must provide either navigationProperty or reverseNavigationProperty: {this}");
            }

            return false;
        }

        private void UpdateFakeProperties(TEntity entity, TChildEntity childEntity, TProperty fakePropertyValue)
        {
            if (HasFakePropertyValue(childEntity) && ChildPropertyGetter(childEntity).Equals(fakePropertyValue))
            {
                if (_childNavigationProperty == null && _navigationProperty == null)
                {
                    throw new NotSupportedException($"Cannot update relation between {typeof(TEntity)} and {typeof(TChildEntity)}: navigation property was not supplied.");
                }

                if (((!_childNavigationProperty?.GetPropertyInfo()?.CanWrite) ?? false) && ((!_navigationProperty?.GetPropertyInfo()?.CanWrite) ?? false))
                {
                    throw new NotSupportedException($"Cannot update relation between {typeof(TEntity)} and {typeof(TChildEntity)}: navigation property doesn't have setter.");
                }

                ChildPropertySetter(childEntity, default);
                ChildNavigationPropertySetter?.Invoke(childEntity, entity);
                if (ChildNavigationPropertyGetter == null)
                {
                    NavigationPropertySetter?.Invoke(entity, childEntity);
                }
            }
        }
    }
}
