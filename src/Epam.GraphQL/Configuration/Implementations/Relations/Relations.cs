// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Epam.GraphQL.Enums;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Configuration.Implementations.Relations
{
    internal abstract class Relations
    {
        public abstract Type ChildEntityType { get; }

        public abstract IEnumerable<IRelation> Items { get; }

        public abstract bool HasFakePropertyValues(object entity, IDictionary<string, object?> propertyValues);

        public abstract void UpdateFakeProperties(object? parent, object? child, IDictionary<string, object?> childPropertyValues, object? propertyValue);
    }

    internal class Relations<TChildEntity> : Relations
    {
        private readonly HashSet<IRelation> _relations = new();
        private readonly List<IRelation> _postponedRelations = new();

        public override Type ChildEntityType => typeof(TChildEntity);

        public override IEnumerable<IRelation> Items => _relations.Concat(_postponedRelations);

        public void Register<TEntity, TLoader, TProperty, TChildProperty, TExecutionContext>(
            RelationRegistry<TExecutionContext> registry,
            Expression<Func<TEntity, TProperty>> property,
            Expression<Func<TEntity, TChildEntity>>? navigationProperty,
            Expression<Func<TChildEntity, TChildProperty>> childProperty,
            Expression<Func<TChildEntity, TEntity>>? childNavigationProperty,
            Func<Type, PropertyInfo?> getPrimaryKeyPropertyInfo,
            RelationType relationType)
            where TLoader : Loader<TEntity, TExecutionContext>, new()
            where TEntity : class
        {
            var relation = new Relation<TEntity, TLoader, TChildEntity, TProperty, TChildProperty, TExecutionContext>(registry, property, navigationProperty, childProperty, childNavigationProperty, getPrimaryKeyPropertyInfo, relationType);

            if (!_relations.Contains(relation))
            {
                _relations.Add(relation);
            }
        }

        public void Register<TPropertyType, TChildPropertyType>(string propName, Predicate<TPropertyType> isFakePropValue, Func<object, object?> idGetter)
        {
            var relation = new ForeignKeyRelation<TPropertyType>(propName, isFakePropValue, idGetter);
            _postponedRelations.Add(relation);
        }

        public override bool HasFakePropertyValues(object entity, IDictionary<string, object?> propertyValues)
        {
            if (entity is TChildEntity e)
            {
                return HasFakePropertyValues(e, propertyValues);
            }

            throw new InvalidOperationException();
        }

        public bool HasFakePropertyValues(TChildEntity childEntity, IDictionary<string, object?> childPropertyValues) => Items.Any(relation => relation.HasFakePropertyValue(childEntity, childPropertyValues));

        public override void UpdateFakeProperties(object? parent, object? child, IDictionary<string, object?> childPropertyValues, object? propertyValue)
        {
            if (child is TChildEntity childEntity)
            {
                UpdateFakeProperties(parent, childEntity, childPropertyValues, propertyValue);
            }
        }

        public async Task<bool> CanViewParentAsync<TExecutionContext>(GraphQLContext<TExecutionContext> context, TChildEntity entity)
        {
            var tasks = Items.Select(relation => relation.CanViewParentAsync(context, entity).GetResultAsync());

            foreach (var task in tasks)
            {
                if (!await task.ConfigureAwait(false))
                {
                    return false;
                }
            }

            return true;
        }

        private void UpdateFakeProperties(object? entity, TChildEntity childEntity, IDictionary<string, object?> childPropertyValues, object? fakePropertyValue)
        {
            foreach (var relation in Items)
            {
                relation.UpdateFakeProperties(entity, childEntity, childPropertyValues, fakePropertyValue);
            }
        }
    }
}
