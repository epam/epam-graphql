// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.MutableLoader;
using Epam.GraphQL.Builders.MutableLoader.Implementations;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Configuration.Implementations;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Savers;
using Epam.GraphQL.TaskBatcher;
using GraphQL;
using GraphQL.DataLoader;
using TypeExtensions = Epam.GraphQL.Extensions.TypeExtensions;

namespace Epam.GraphQL.Loaders
{
    public abstract class MutableLoader<TEntity, TId, TExecutionContext> : IdentifiableLoader<TEntity, TId, TExecutionContext>, IMutableLoader<TExecutionContext>
        where TEntity : class
    {
        internal override bool ShouldConfigureInputType => true;

        private Action<TEntity, TId> IdSetter => IdExpression.GetSetter();

        public abstract bool IsFakeId(TId id);

        ISaveResult<TExecutionContext> IMutableLoader<TExecutionContext>.CreateSaveResultFromValues(string fieldName, IEnumerable<IInputItem> values) =>
            CreateSaveResultFromValues(fieldName, values.Cast<InputItem<TEntity>>());

        ISaveResult<TExecutionContext> IMutableLoader<TExecutionContext>.CreateSaveResultFromValues(string fieldName, IEnumerable<object> values) =>
            CreateSaveResultFromValues(fieldName, values.Cast<TEntity>());

        Task<IEnumerable<ISaveResult<TExecutionContext>>> IMutableLoader<TExecutionContext>.MutateAsync(IResolveFieldContext context, ISaveResult<TExecutionContext> previousSaveResult) => MutateAsync(context, (SaveResult<TEntity, TId, TExecutionContext>)previousSaveResult);

        bool IMutableLoader<TExecutionContext>.IsFakeId(object? id) => id != null && IsFakeId((TId)id);

        Task IMutableLoader<TExecutionContext>.ReloadAsync(IResolveFieldContext context, ISaveResult<TExecutionContext> saveResult, IEnumerable<string> fieldNames) => ReloadAsync(context, (SaveResult<TEntity, TId, TExecutionContext>)saveResult, fieldNames);

        protected internal new IHasFilterableAndSortableAndOnWriteAndEditableAndMandatoryForUpdateAndReferenceToAndDefault<TEntity, TReturnType, TExecutionContext> Field<TReturnType>(
            Expression<Func<TEntity, TReturnType>> expression,
            string? deprecationReason = null)
        {
            var field = AddField(expression, deprecationReason);
            return new FieldBuilder<TEntity, TReturnType, TExecutionContext>(Registry, GetType(), field);
        }

        protected internal new IHasFilterableAndSortableAndOnWriteAndEditableAndMandatoryForUpdateAndReferenceToAndDefault<TEntity, TReturnType, TExecutionContext> Field<TReturnType>(
            string name,
            Expression<Func<TEntity, TReturnType>> expression,
            string? deprecationReason = null)
        {
            var field = AddField(name, expression, deprecationReason);
            return new FieldBuilder<TEntity, TReturnType, TExecutionContext>(Registry, GetType(), field);
        }

        protected internal new IHasFilterableAndSortableAndOnWriteAndEditableAndMandatoryForUpdateAndReferenceToAndDefault<TEntity, TReturnType, TExecutionContext> Field<TReturnType>(
            string name,
            Expression<Func<TExecutionContext, TEntity, TReturnType>> expression,
            string? deprecationReason = null)
        {
            var field = AddField(name, expression, deprecationReason);
            return new FieldBuilder<TEntity, TReturnType, TExecutionContext>(Registry, GetType(), field);
        }

        protected internal new IMutableLoaderFieldBuilder<TEntity, TExecutionContext> Field(string name, string? deprecationReason = null)
        {
            var fieldType = AddField(name, deprecationReason);
            var fieldBuilderType = typeof(MutableLoaderFieldBuilder<,,>).MakeGenericType(typeof(TEntity), GetType(), typeof(TExecutionContext));
            return (IMutableLoaderFieldBuilder<TEntity, TExecutionContext>)fieldBuilderType.CreateInstanceAndHoistBaseException(Registry, fieldType);
        }

        protected internal virtual Task<bool> CanSaveAsync(IExecutionContextAccessor<TExecutionContext> context, TEntity entity, bool isNew)
        {
#pragma warning disable CS0618 // Type or member is obsolete
            return CanSaveAsync((IUserContextAccessor<TExecutionContext>)context, entity, isNew);
#pragma warning restore CS0618 // Type or member is obsolete
        }

        [Obsolete("Consider using CanSaveAsync with the first argument of type IExecutionContextAccessor<TExecutionContext>).")]
        protected virtual async Task<bool> CanSaveAsync(
            IUserContextAccessor<TExecutionContext> context,
            TEntity entity,
            bool isNew)
        {
            Guards.ThrowIfNull(context, nameof(context));

            if (!await CanViewAsync((GraphQLContext<TExecutionContext>)context, entity).GetResultAsync().ConfigureAwait(false))
            {
                return false;
            }

            return await ((GraphQLContext<TExecutionContext>)context).Registry.CanViewParentAsync(GetType(), (GraphQLContext<TExecutionContext>)context, entity).ConfigureAwait(false);
        }

        /// <summary>
        /// Lifetime hook, allows to perform extra operations before inserting a new <paramref name="entity"/> in a db.
        /// </summary>
        protected virtual void BeforeCreate(TExecutionContext context, TEntity entity)
        {
        }

        /// <summary>
        /// Asynchronous lifetime hook, allows to perform extra operations before inserting a new <paramref name="entity"/> in a db.
        /// <br/><b>Note:</b> Default implementation performs call of the <see cref="BeforeCreate(TExecutionContext, TEntity)"/>.
        /// </summary>
        protected virtual Task BeforeCreateAsync(TExecutionContext context, TEntity entity)
        {
            BeforeCreate(context, entity);
            return Task.CompletedTask;
        }

        /// <summary>
        /// Lifetime hook, allows to perform extra operations before updating a <paramref name="entity"/> in a db.
        /// </summary>
        protected virtual void BeforeUpdate(TExecutionContext context, TEntity entity)
        {
        }

        /// <summary>
        /// Asynchronous lifetime hook, allows to perform extra operations before updating a <paramref name="entity"/> in a db.
        /// <br/><b>Note:</b> Default implementation performs call of the <see cref="BeforeUpdate(TExecutionContext, TEntity)"/>.
        /// </summary>
        protected virtual Task BeforeUpdateAsync(TExecutionContext context, TEntity entity)
        {
            BeforeUpdate(context, entity);
            return Task.CompletedTask;
        }

        private SaveResult<TEntity, TId, TExecutionContext> CreateSaveResultFromValues(string fieldName, IEnumerable<InputItem<TEntity>> entities) => new(
            pendingItems: entities
                .Select(entity =>
                    new SaveResultItem<TEntity, TId>(
                        getId: GetId,
                        payload: entity.Payload,
                        isNew: IsFakeId(GetId(entity.Payload)),
                        properties: entity.Properties))
                .ToList(),
            processedItems: new List<SaveResultItem<TEntity?, TId>>(),
            postponedItems: new List<SaveResultItem<TEntity, TId>>(),
            loader: this,
            fieldName: fieldName);

        private SaveResult<TEntity, TId, TExecutionContext> CreateSaveResultFromValues(string fieldName, IEnumerable<TEntity> entities) => new(
            processedItems: entities
                .Select(entity =>
                    new SaveResultItem<TEntity?, TId>(
                        getId: e => GetId(e ?? throw new NotSupportedException()),
                        payload: entity,
                        isNew: EqualityComparer<TId?>.Default.Equals(GetId(entity), default),
                        properties: new Dictionary<string, object?>()))
                .ToList(),
            pendingItems: new List<SaveResultItem<TEntity, TId>>(),
            postponedItems: new List<SaveResultItem<TEntity, TId>>(),
            loader: this,
            fieldName: fieldName);

        private async Task<IEnumerable<ISaveResult<TExecutionContext>>> MutateAsync(IResolveFieldContext context, SaveResult<TEntity, TId, TExecutionContext> previousSaveResult)
        {
            var profiler = context.GetProfiler();

            using (profiler.Step($"{GetType().HumanizedName()}.{nameof(MutateAsync)}"))
            {
                var dbContext = context.GetDataContext();
                var pendingItems = previousSaveResult.PendingItems;

                if (!pendingItems.Any())
                {
                    return Enumerable.Repeat(previousSaveResult, 1);
                }

                var itemsToCreate = pendingItems
                    .Where(item => item.IsNew && !Registry.HasFakePropertyValues(GetType(), item.Payload, item.Properties));

                await CreateNewItemsAsync(context, profiler, dbContext, itemsToCreate).ConfigureAwait(false);

                var itemsToUpdate = pendingItems
                    .Where(item => !item.IsNew && !Registry.HasFakePropertyValues(GetType(), item.Payload, item.Properties)
                        && !Registry.HasFakePropertyValuesPostponedForSave(item.Payload, item.Properties));

                await UpdateExistingItemsAsync(context, profiler, itemsToUpdate).ConfigureAwait(false);

                using (profiler.Step($"PrepareResult"))
                {
                    var postponedItems = pendingItems
                        .Where(item => Registry.HasFakePropertyValues(GetType(), item.Payload, item.Properties));

                    var postponedForSaveItems =
                        pendingItems
                            .Where(item => !Registry.HasFakePropertyValues(GetType(), item.Payload, item.Properties)
                                && Registry.HasFakePropertyValuesPostponedForSave(item.Payload, item.Properties))
                            .Union(previousSaveResult.PostponedItems);

                    var processedItems = previousSaveResult.ProcessedItems.ToList();
#pragma warning disable CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.
                    processedItems.AddRange(itemsToCreate);
                    processedItems.AddRange(itemsToUpdate);
#pragma warning restore CS8620 // Argument cannot be used for parameter due to differences in the nullability of reference types.

                    var result = new List<ISaveResult<TExecutionContext>>
                    {
                        new SaveResult<TEntity, TId, TExecutionContext>(
                            pendingItems: postponedItems.ToList(),
                            processedItems: processedItems,
                            postponedItems: postponedForSaveItems.ToList(),
                            loader: this,
                            fieldName: previousSaveResult.FieldName),
                    };

                    return result;
                }
            }
        }

#pragma warning disable CA1502
#pragma warning disable CA1506
        private async Task UpdateExistingItemsAsync(IResolveFieldContext context, IProfiler profiler, IEnumerable<SaveResultItem<TEntity, TId>> itemsToUpdate)
#pragma warning restore CA1506
#pragma warning restore CA1502
        {
            var graphqlContext = (GraphQLContext<TExecutionContext>?)context.UserContext["ctx"];
            Guards.AssertIfNull(graphqlContext);

            if (itemsToUpdate.Any())
            {
                using (profiler.Step($"Update"))
                {
                    List<IGrouping<TId, TEntity>> itemsFound;
                    var errors = new List<string>();
                    IEnumerable<TEntity> items;

                    using (profiler.Step($"Checks"))
                    {
                        var batcher = context.GetBatcher();
                        var queryExecuter = context.GetQueryExecuter();
                        var hooksExecuter = ObjectGraphTypeConfigurator.ProxyAccessor.CreateHooksExecuter(context);

                        var itemsFoundTasks = itemsToUpdate
                            .Select(item => BatchHelpers
                                .GetLoaderQueryFactory<MutableLoader<TEntity, TId, TExecutionContext>, TEntity, TId, TExecutionContext>(
                                    () => "Checks:All",
                                    this,
                                    IdExpression)(profiler, queryExecuter, null, context.GetUserContext<TExecutionContext>()) // TBD hooksExecuter is null here
                                .LoadAsync(GetId(item.Payload)));

                        itemsFound = new List<IGrouping<TId, TEntity>>();
                        foreach (var itemFoundTask in itemsFoundTasks)
                        {
                            var itemFoundTaskResult = await itemFoundTask.GetResultAsync().ConfigureAwait(false);
                            if (itemFoundTaskResult != null && itemFoundTaskResult.Any())
                            {
                                itemsFound.Add(itemFoundTaskResult);
                            }
                        }

                        var itemsNotFound = itemsToUpdate.Where(item => !itemsFound.Any(i => i.Key.Equals(item.Id)));
                        var itemsHavingMoreThanOneItem = itemsToUpdate.Where(item => itemsFound.Any(i => i.Key.Equals(item.Id) && i.Count() > 1));

                        errors.AddRange(itemsNotFound.Select(item => $"Cannot update entity: Entity was not found (type: {typeof(TEntity).HumanizedName()}, id: {GetId(item.Payload)})."));
                        errors.AddRange(itemsHavingMoreThanOneItem.Select(item => $"Cannot update entity: More than one entity was found (type: {typeof(TEntity).HumanizedName()}: id = {GetId(item.Payload)})."));

                        if (errors.Any())
                        {
                            throw new ExecutionError(string.Join("\n\r", errors));
                        }
                    }

                    items = itemsFound.Select(group => group.Single());
                    using (profiler.Step($"CustomChecks"))
                    {
                        var resolvedEntities = itemsToUpdate.Select(nextItemToUpdate =>
                        {
                            // TODO Optimization of proxy creation
                            var prevEntity = items.Single(i => EqualityComparer<TId>.Default.Equals(GetId(i), nextItemToUpdate.Id));
                            var prevEntityProxy = prevEntity; // TODO transform by calling InputObjectGraphTypeConfigurator.ProxyAccessor.CreateSelectorExpression(context.UserContext, nextItemToUpdate.Properties.Keys).Compile()
                            var fieldsAndNextValues = nextItemToUpdate.Properties
                                .Select(kv => (InputObjectGraphTypeConfigurator.FindFieldByName(kv.Key), kv.Value));

                            List<(IField<TEntity, TExecutionContext> Field, ValueTask<object?> Result, object? Value)>
                                resolveFieldTasks = new();

                            foreach (var prop in nextItemToUpdate.Properties)
                            {
                                var field = InputObjectGraphTypeConfigurator.FindFieldByName(prop.Key);
                                var result = field.Resolver.ResolveAsync(new ResolveFieldContext
                                {
                                    // TODO Don't create another IResolveFieldContext; reuse existing one
                                    UserContext = context.UserContext,
                                    Source = prevEntityProxy,
                                });

                                resolveFieldTasks.Add((field, result, prop.Value));
                            }

                            return (nextItemToUpdate, prevEntity, resolveFieldTasks);
                        }).ToArray();

                        var tasksForWait = resolvedEntities
                            .Select(resolvedEntity => resolvedEntity.resolveFieldTasks.Select(t => t.Result).ToArray());

                        var resolvedTasks = new List<object?[]>();
                        foreach (var taskForWait in tasksForWait)
                        {
                            var resolved = new object?[taskForWait.Length];

                            for (int i = 0; i < taskForWait.Length; i++)
                            {
                                var result = await taskForWait[i].ConfigureAwait(false);
                                resolved[i] = result is IDataLoaderResult dataLoaderResult
                                    ? await dataLoaderResult.GetResultAsync().ConfigureAwait(false)
                                    : result;
                            }

                            resolvedTasks.Add(resolved);
                        }

                        var canEditTasks = new List<(string, TEntity, IDataLoaderResult<(bool, string)>)>();

                        for (int i = 0; i < resolvedEntities.Length; i++)
                        {
                            var nextEntity = resolvedEntities[i].nextItemToUpdate;
                            var prevEntity = resolvedEntities[i].prevEntity;

                            for (int j = 0; j < resolvedEntities[i].resolveFieldTasks.Count; j++)
                            {
                                var (field, _, next) = resolvedEntities[i].resolveFieldTasks[j];
                                var prevValue = resolvedTasks[i][j];
                                var nextValue = next;
                                if (nextValue != null)
                                {
                                    nextValue = nextValue.GetPropertyValue(field.FieldType);
                                }

                                var shouldCheck = (nextValue == null && prevValue != null)
                                    || (nextValue != null && prevValue == null)
                                    || (prevValue != null && !prevValue.Equals(nextValue));

                                if (shouldCheck)
                                {
                                    var fieldChange = FieldChange.Create(field.FieldType, context.GetUserContext<TExecutionContext>(), prevEntity, prevValue, nextValue);
                                    var canEditTask = field.CanEdit(context).LoadAsync(fieldChange);
                                    canEditTasks.Add((field.Name, nextEntity.Payload, canEditTask));
                                }
                            }

                            prevEntity.CopyProperties(
                                nextEntity.Payload,
                                resolvedEntities[i].resolveFieldTasks.Select(fv => fv.Field)
                                    .OfType<IExpressionFieldConfiguration<TEntity, TExecutionContext>>()
                                    .Where(field => field.PropertyInfo != null && field.EditSettings.OnWrite == null && field.EditSettings.OnWriteAsync == null)
                                    .Select(field => field.PropertyInfo!));
                        }

                        var resolvedCanEdiTasks = new List<(bool, string)>();
                        foreach (var canEdiTask in canEditTasks)
                        {
                            var canEditTaskResult = await canEdiTask.Item3.GetResultAsync().ConfigureAwait(false);
                            resolvedCanEdiTasks.Add(canEditTaskResult);
                        }

                        for (var i = 0; i < canEditTasks.Count; i++)
                        {
                            var (fieldName, entity, _) = canEditTasks[i];
                            var (canEdit, disableReason) = resolvedCanEdiTasks[i];
                            if (!canEdit)
                            {
                                errors.Add($"Cannot update entity: Cannot change field `{fieldName}` of entity (type: {typeof(TEntity).HumanizedName()}, id: {GetId(entity)}): {disableReason}");
                            }
                        }

                        if (errors.Any())
                        {
                            throw new ExecutionError(string.Join("\n\r", errors));
                        }
                    }

                    using (profiler.Step($"CanSave"))
                    {
                        var itemsToCheck = new List<TEntity>();
                        foreach (var item in itemsToUpdate)
                        {
                            var payload = items.Single(i => EqualityComparer<TId>.Default.Equals(GetId(i), item.Id));

                            payload.CopyProperties(
                                item.Payload,
                                item.Properties.Keys
                                    .Select(fieldName => InputObjectGraphTypeConfigurator.FindFieldByName(fieldName))
                                    .OfType<IExpressionFieldConfiguration<TEntity, TExecutionContext>>()
                                    .Where(field => field.PropertyInfo != null && field.EditSettings.OnWrite == null && field.EditSettings.OnWriteAsync == null)
                                    .Select(field => field.PropertyInfo!));
                            itemsToCheck.Add(payload);
                        }

                        var canUpdateItemTasks = itemsToCheck.Select(item => CanSaveAsync(graphqlContext, item, false));
                        var canUpdate = true;
                        foreach (var canUpdateItemTask in canUpdateItemTasks)
                        {
                            if (!await canUpdateItemTask.ConfigureAwait(false))
                            {
                                canUpdate = false;
                            }
                        }

                        if (!canUpdate)
                        {
                            throw new ExecutionError($"Cannot update entity (type: {typeof(TEntity).HumanizedName()}): Unauthorized.");
                        }
                    }

                    using (profiler.Step($"Save"))
                    {
                        foreach (var item in itemsToUpdate)
                        {
                            var itemToUpdate = items.Single(i => EqualityComparer<TId>.Default.Equals(GetId(i), item.Id));

                            try
                            {
                                await BeforeUpdateAsync(context.GetUserContext<TExecutionContext>(), itemToUpdate).ConfigureAwait(false);
                            }
                            catch (Exception e)
                            {
                                throw new ExecutionError($"{GetType().HumanizedName()}.{nameof(BeforeUpdate)} has thrown exception:\r\n{e.GetType()}\r\n{e.Message}", e);
                            }

                            await CustomSave(context, item, itemToUpdate).ConfigureAwait(false);

                            item.Payload = itemToUpdate;
                        }
                    }
                }
            }
        }

        private async Task CreateNewItemsAsync(IResolveFieldContext context, IProfiler profiler, IDataContext dbContext, IEnumerable<SaveResultItem<TEntity, TId>> itemsToCreate)
        {
            var graphqlContext = (GraphQLContext<TExecutionContext>?)context.UserContext["ctx"];
            Guards.AssertIfNull(graphqlContext);

            if (itemsToCreate.Any())
            {
                using (profiler.Step($"Create"))
                {
                    using (profiler.Step($"CanCreate"))
                    {
                        var canCreateTasks = itemsToCreate.Select(item => CanSaveAsync(graphqlContext, item.Payload, true));

                        var canCreate = true;
                        foreach (var canCreateTask in canCreateTasks)
                        {
                            if (!await canCreateTask.ConfigureAwait(false))
                            {
                                canCreate = false;
                                break;
                            }
                        }

                        if (!canCreate)
                        {
                            throw new ExecutionError($"Cannot create entity (type: {typeof(TEntity).HumanizedName()}): Unauthorized.");
                        }
                    }

                    var payloadOnly = itemsToCreate.Select(r => r.Payload);

                    foreach (var item in itemsToCreate)
                    {
                        foreach (var field in InputObjectGraphTypeConfigurator.Fields
                            .Where(f => f.EditSettings != null && f.EditSettings.GetDefaultValue != null)
                            .OfType<IExpressionFieldConfiguration<TEntity, TExecutionContext>>()
                            .Where(f => f.PropertyInfo != null))
                        {
                            var defaultValue = field.EditSettings!.GetDefaultValue!(context, item.Payload);
                            item.Payload.SetPropertyValue(field.PropertyInfo!, defaultValue);
                        }
                    }

                    using (profiler.Step($"BeforeCreate"))
                    {
                        foreach (var item in payloadOnly)
                        {
                            await BeforeCreateAsync(context.GetUserContext<TExecutionContext>(), item).ConfigureAwait(false);
                        }
                    }

                    using (profiler.Step($"Checks"))
                    {
                        var errors = itemsToCreate.SelectMany(item =>
                        {
                            var fieldTypes = item.Properties.Keys
                                .Select(propName => InputObjectGraphTypeConfigurator.FindFieldByName(propName))
                                .OfType<IExpressionFieldConfiguration<TEntity, TExecutionContext>>()
                                .Where(field => field.PropertyInfo != null)
                                .Select(field => field.PropertyInfo!.PropertyType)
                                .Where(propertyType => propertyType.IsValueType && !TypeExtensions.IsNullable(propertyType))
                                .ToList();

                            var fields = InputObjectGraphTypeConfigurator.Fields
                                .OfType<IExpressionFieldConfiguration<TEntity, TExecutionContext>>()
                                .Where(field => field.PropertyInfo != null && field.PropertyInfo.PropertyType.IsValueType && !TypeExtensions.IsNullable(field.PropertyInfo.PropertyType)
                                    && fieldTypes.All(fieldType => fieldType != field.PropertyInfo.PropertyType))
                                .Where(field => field.EditSettings?.GetDefaultValue == null);

                            return fields
                                .Select(field => $"Cannot create entity: Field `{field.Name}` cannot be null (type: {typeof(TEntity).HumanizedName()}, id: {GetId(item.Payload)}).");
                        });

                        if (errors.Any())
                        {
                            throw new ExecutionError(string.Join("\n\r", errors));
                        }
                    }

                    using (profiler.Step($"CustomSave"))
                    {
                        foreach (var item in itemsToCreate)
                        {
                            IdSetter(item.Payload, default!);
                            await CustomSave(context, item, item.Payload).ConfigureAwait(false);
                        }
                    }

                    dbContext.AddRange(payloadOnly);
                }
            }
        }

        private async Task CustomSave(IResolveFieldContext context, SaveResultItem<TEntity, TId> item, TEntity itemToUpdate)
        {
            var customProps = item.Properties.Keys
                .Where(propName =>
                {
                    var field = InputObjectGraphTypeConfigurator.FindFieldByName(propName);
                    return field.EditSettings != null && field.EditSettings.OnWrite != null && !field.EditSettings.IsReadOnly;
                });

            foreach (var propName in customProps)
            {
                var field = InputObjectGraphTypeConfigurator.FindFieldByName(propName);
                var value = item.Properties[propName];
                field.EditSettings!.OnWrite!(context, itemToUpdate, value);
            }

            customProps = item.Properties.Keys
                .Where(propName =>
                {
                    var field = InputObjectGraphTypeConfigurator.FindFieldByName(propName);
                    return field.EditSettings != null && field.EditSettings.OnWriteAsync != null && !field.EditSettings.IsReadOnly;
                });

            foreach (var propName in customProps)
            {
                var field = InputObjectGraphTypeConfigurator.FindFieldByName(propName);
                var value = item.Properties[propName];
                await field.EditSettings!.OnWriteAsync!(context, itemToUpdate, value).ConfigureAwait(false);
            }
        }

        private async Task ReloadAsync(IResolveFieldContext context, SaveResult<TEntity, TId, TExecutionContext> saveResult, IEnumerable<string> fieldNames)
        {
            if (!fieldNames.Any())
            {
                return;
            }

            fieldNames = fieldNames.Concat(new[] { IdExpression.NameOf() }).Distinct();

            var profiler = context.GetProfiler();
            using (profiler.Step($"{GetType().HumanizedName()}.{nameof(ReloadAsync)}"))
            {
                var batcher = context.GetBatcher();
                var ctx = context.GetUserContext<TExecutionContext>();
                var dataContext = context.GetDataContext();
                var queryExecuter = context.GetQueryExecuter();
                var batch = BatchHelpers.GetLoaderQueryFactory<MutableLoader<TEntity, TId, TExecutionContext>, TEntity, TId, TExecutionContext>(
                    () => "ReloadAsync", this, IdExpression)(profiler, queryExecuter, null, ctx); // TBD hookExecuter is null here

                var entities = saveResult.ProcessedItems;

                var payloads = entities
                    .Where(e => e.Payload != null)
                    .Select(e => e.Payload!);

                payloads.ForEach(item => dataContext.DetachEntity(item));

                var ids = payloads.Select(GetId);

                // That portion of reload goes against batcher
                var reloadedEntities = await batch.LoadAsync(ids).GetResultAsync().ConfigureAwait(false);

                var errors = new List<string>();

                foreach (var entity in entities)
                {
                    if (entity.Payload == null)
                    {
                        continue;
                    }

                    var id = GetId(entity.Payload);
                    var result = reloadedEntities
                        .Where(reloaded => reloaded != null && EqualityComparer<TId>.Default.Equals(reloaded.Key, id))
                        .ToList();

                    if (!result.Any())
                    {
                        entity.Payload = default;
                    }
                    else if (result.Count > 1)
                    {
                        errors.Add($"Cannot reload entity: More than one entity was found (type: {typeof(TEntity).HumanizedName()}: id = {id})");
                    }
                    else
                    {
                        var group = result.Single().ToList();
                        if (!group.Any())
                        {
                            entity.Payload = default;
                        }
                        else if (group.Count > 1)
                        {
                            errors.Add($"Cannot reload entity: More than one entity was found (type: {typeof(TEntity).HumanizedName()}: id = {id})");
                        }
                        else
                        {
                            entity.Payload = group.Single();
                        }
                    }
                }

                if (errors.Any())
                {
                    throw new ExecutionError(string.Join("\n\r", errors));
                }
            }
        }
    }
}
