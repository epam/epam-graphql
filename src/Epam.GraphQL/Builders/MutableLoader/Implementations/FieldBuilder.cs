// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Common;
using Epam.GraphQL.Builders.Common.Implementations;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Configuration.Implementations;
using Epam.GraphQL.Configuration.Implementations.Fields.ExpressionFields;
using Epam.GraphQL.Enums;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Builders.MutableLoader.Implementations
{
    internal class FieldBuilder<TEntity, TReturnType, TExecutionContext> : FilterableAndSortableAndGroupableFieldBuilder<TEntity, TReturnType, TExecutionContext>,
        IHasFilterableAndSortableAndOnWriteAndEditableAndMandatoryForUpdateAndReferenceToAndDefault<TEntity, TReturnType, TExecutionContext>,
        IHasFilterableAndSortableAndOnWriteAndEditableAndMandatoryForUpdateAndReferenceTo<TEntity, TReturnType, TExecutionContext>,
        IHasFilterableAndSortableAndOnWriteAndEditableAndMandatoryForUpdate<TEntity, TReturnType, TExecutionContext>,
        IHasFilterableAndSortableAndOnWriteAndEditable<TEntity, TReturnType, TExecutionContext>,
        IHasFilterableAndSortableAndOnWrite<TEntity, TReturnType, TExecutionContext>,
        IHasFilterableAndSortableAndGroupable<TEntity, TReturnType>,
        IHasSortableAndGroupable<TEntity>
        where TEntity : class
    {
        private readonly Type _loaderType;
        private readonly RelationRegistry<TExecutionContext> _registry;

        internal FieldBuilder(RelationRegistry<TExecutionContext> registry, Type loaderType, ExpressionField<TEntity, TReturnType, TExecutionContext> field)
            : base(field)
        {
            _registry = registry;
            _loaderType = loaderType;
        }

        public IHasFilterableAndSortableAndOnWriteAndEditableAndMandatoryForUpdate<TEntity, TReturnType, TExecutionContext> ReferencesTo<TParentEntity, TParentEntityLoader>(
            Expression<Func<TParentEntity, TReturnType>> property,
            Expression<Func<TEntity, TParentEntity>> navigationProperty,
            RelationType relationType)
            where TParentEntity : class
            where TParentEntityLoader : Loader<TParentEntity, TExecutionContext>, IIdentifiableLoader, new()
        {
            Guards.ThrowIfNull(property, nameof(property));
            Guards.ThrowIfNull(navigationProperty, nameof(navigationProperty));

            var parentParam = Expression.Parameter(typeof(TParentEntity));
            var childParam = Expression.Parameter(typeof(TEntity));
            var condition = Expression.Lambda<Func<TEntity, TParentEntity, bool>>(
                Expression.Equal(
                    Expression.Property(childParam, Field.PropertyInfo),
                    property.ReplaceParameter(parentParam)),
                childParam,
                parentParam);

            _registry.Register(_loaderType, typeof(TParentEntityLoader), condition, null, navigationProperty, relationType);

            return this;
        }

        public IHasFilterableAndSortableAndOnWriteAndEditable<TEntity, TReturnType, TExecutionContext> MandatoryForUpdate()
        {
            Field.EditSettings?.MandatoryForUpdate();
            return this;
        }

        public IHasFilterableAndSortableAndOnWrite<TEntity, TReturnType, TExecutionContext> EditableIf(
            Func<IFieldChange<TEntity, TReturnType, TExecutionContext>, bool> predicate,
            Func<IFieldChange<TEntity, TReturnType, TExecutionContext>, string>? reason)
        {
            Field.EditSettings?.EditableIf(predicate, reason);
            return this;
        }

        public IHasFilterableAndSortableAndOnWrite<TEntity, TReturnType, TExecutionContext> BatchedEditableIf<TItem>(
            Func<IEnumerable<TEntity>, IEnumerable<KeyValuePair<TEntity, TItem>>> batchFunc,
            Func<IBatchFieldChange<TEntity, TReturnType, TItem, TExecutionContext>, bool> predicate,
            Func<IBatchFieldChange<TEntity, TReturnType, TItem, TExecutionContext>, string>? reason)
        {
            Field.EditSettings?.EditableIf(_registry.WrapFuncByUnusedContext(batchFunc), predicate, reason);
            return this;
        }

        public IHasFilterableAndSortableAndOnWrite<TEntity, TReturnType, TExecutionContext> BatchedEditableIf<TItem>(
            Func<TExecutionContext, IEnumerable<TEntity>, IEnumerable<KeyValuePair<TEntity, TItem>>> batchFunc,
            Func<IBatchFieldChange<TEntity, TReturnType, TItem, TExecutionContext>, bool> predicate,
            Func<IBatchFieldChange<TEntity, TReturnType, TItem, TExecutionContext>, string>? reason)
        {
            Field.EditSettings?.EditableIf(batchFunc, predicate, reason);
            return this;
        }

        public IHasFilterableAndSortableAndOnWrite<TEntity, TReturnType, TExecutionContext> Editable()
        {
            Field.EditSettings?.Editable();
            return this;
        }

        public IHasFilterableAndSortableAndOnWriteAndEditableAndMandatoryForUpdateAndReferenceTo<TEntity, TReturnType, TExecutionContext> Default(Func<TEntity, TReturnType> selector)
        {
            return Default((_, entity) => selector(entity));
        }

        public IHasFilterableAndSortableAndOnWriteAndEditableAndMandatoryForUpdateAndReferenceTo<TEntity, TReturnType, TExecutionContext> Default(Func<TExecutionContext, TEntity, TReturnType> selector)
        {
            Field.EditSettings?.Default(selector);
            return this;
        }

        public IHasFilterableAndSortableAndGroupable<TEntity, TReturnType> OnWrite(Action<TExecutionContext, TEntity, TReturnType> save)
        {
            Field.EditSettings?.SetOnWrite(save);
            return this;
        }

        public IHasFilterableAndSortableAndGroupable<TEntity, TReturnType> OnWrite(Func<TExecutionContext, TEntity, TReturnType, Task> save)
        {
            Field.EditSettings?.SetOnWrite(save);
            return this;
        }
    }
}
