// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.EntityFrameworkCore;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Mutation;
using Epam.GraphQL.Types;
using GraphQL;
using GraphQL.Language.AST;
using Microsoft.Extensions.DependencyInjection;

#nullable enable

namespace Epam.GraphQL.Savers
{
    internal static class DbContextSaver
    {
        public static ResolveOptions DefaultOptions { get; } = new ResolveOptions();

        public static async Task<object> PerformManualMutationAndGetResult<TExecutionContext>(RelationRegistry<TExecutionContext> registry, IEnumerable<object> entities, Mutation<TExecutionContext> mutation, IResolveFieldContext context, ResolveOptions options)
        {
            var executionContext = (GraphQLContext<TExecutionContext>)context.UserContext["ctx"];
            var profiler = executionContext.Profiler;

            using (profiler.Step(nameof(PerformManualMutationAndGetResult)))
            {
                var results = await SaveChangesAndReload(registry, entities, mutation, context, options).ConfigureAwait(false);

                var afterSaveEntities = await mutation.DoAfterSave(
                    context,
                    results
                        .SelectMany(result => result.ProcessedItems)
                        .Where(item => item.Payload != null)
                        .Select(item => item.Payload!)).ConfigureAwait(false);

                var afterSaveResults = afterSaveEntities.Any()
                    ? await SaveChangesAndReload(
                        registry,
                        afterSaveEntities,
                        mutation,
                        context,
                        DefaultOptions).ConfigureAwait(false)
                    : Enumerable.Empty<ISaveResult<TExecutionContext>>();

                using (profiler.Step("Prepare output"))
                {
                    results = results
                        .Concat(afterSaveResults)
                        .GroupBy(r => r.EntityType)
                        .Select(group => group.Aggregate((first, second) => first.Merge(second)));

                    return mutation.CreateSubmitOutput(ConvertItems(results));
                }
            }
        }

        public static async Task<object> PerformManualMutationAndGetResult<TExecutionContext, TData>(RelationRegistry<TExecutionContext> registry, MutationResult<TData> mutationResult, Mutation<TExecutionContext> mutation, IResolveFieldContext context, ResolveOptions options)
        {
            var executionContext = (GraphQLContext<TExecutionContext>)context.UserContext["ctx"];
            var profiler = executionContext.Profiler;

            using (profiler.Step(nameof(PerformManualMutationAndGetResult)))
            {
                var results = await SaveChangesAndReload(registry, mutationResult.Payload, mutation, context, options).ConfigureAwait(false);
                var afterSaveEntities = await mutation.DoAfterSave(
                    context,
                    results
                        .SelectMany(result => result.ProcessedItems)
                        .Where(item => item.Payload != null)
                        .Select(item => item.Payload!)).ConfigureAwait(false);

                var afterSaveResults = await SaveChangesAndReload(registry, afterSaveEntities, mutation, context, options).ConfigureAwait(false);

                using (profiler.Step("Prepare output"))
                {
                    results = results
                        .Concat(afterSaveResults)
                        .GroupBy(r => r.EntityType)
                        .Select(group => group.Aggregate((first, second) => first.Merge(second)));

                    return mutation.CreateSubmitOutput(ConvertItems(results), mutationResult.Data);
                }
            }
        }

        public static Task<object> PerformManualMutationAndGetResult<TExecutionContext>(RelationRegistry<TExecutionContext> registry, IMutationResult mutationResult, Mutation<TExecutionContext> mutation, IResolveFieldContext context, Type dataType, ResolveOptions options)
        {
            var methodInfo = typeof(DbContextSaver).GetGenericMethod(
                nameof(PerformManualMutationAndGetResult),
                new[] { typeof(TExecutionContext), dataType },
                new[] { typeof(RelationRegistry<TExecutionContext>), typeof(MutationResult<>).MakeGenericType(dataType), typeof(Mutation<TExecutionContext>), typeof(IResolveFieldContext), typeof(ResolveOptions) });
            return (Task<object>)methodInfo.InvokeAndHoistBaseException(null, registry, mutationResult, mutation, context, options);
        }

        public static async Task<IEnumerable<ISaveResult<TExecutionContext>>> SaveChangesAndReload<TExecutionContext>(RelationRegistry<TExecutionContext> registry, IEnumerable<object>? entities, Mutation<TExecutionContext> mutation, IResolveFieldContext context, ResolveOptions options)
        {
            var dataContext = context.GetDataContext();
            var batcher = context.GetBatcher();
            var profiler = context.GetProfiler();

            var doNotSaveChanges = options.FindExtension<EFCoreResolveOptionsExtension>()?.DoNotSaveChanges ?? false;
            var doNotAddEntityToDbContext = options.FindExtension<EFCoreResolveOptionsExtension>()?.DoNotAddNewEntitiesToDbContext ?? false;
            var submitInputTypeRegistry = registry.GetRequiredService<SubmitInputTypeRegistry<TExecutionContext>>();

            using (profiler.Step(nameof(SaveChangesAndReload)))
            {
                List<ISaveResult<TExecutionContext>> results = new();
                using (profiler.Step("Save changes"))
                {
                    var mutationType = mutation.GetType();

                    var entityGroups = entities
                        .GroupBy(entity => entity.GetType());

                    foreach (var group in entityGroups)
                    {
                        var entityType = group.Key;
                        var fieldName = submitInputTypeRegistry.GetFieldNameByEntityType(mutationType, entityType);
                        var loader = registry.ResolveLoader(submitInputTypeRegistry.GetLoaderTypeByEntityType(mutationType, entityType));
                        var saveResult = loader.CreateSaveResultFromValues(mutationType, fieldName, group);

                        var newEntities = saveResult.ProcessedItems.Where(r => r.IsNew).Select(r => r.Payload);
                        if (newEntities.Any() && !doNotAddEntityToDbContext)
                        {
                            var addRangeMethodInfo = typeof(IDataContext).GetMethod(nameof(dataContext.AddRange)).MakeGenericMethod(entityType);
                            var castMethodInfo = CachedReflectionInfo.ForEnumerable.Cast(entityType);

                            var castedEntities = castMethodInfo.InvokeAndHoistBaseException(null, newEntities);

                            addRangeMethodInfo.InvokeAndHoistBaseException(dataContext, castedEntities);
                        }

                        results.Add(saveResult);
                    }

                    if (!doNotSaveChanges)
                    {
                        await dataContext.SaveChangesAsync().ConfigureAwait(false);
                    }
                }

                if (!doNotSaveChanges)
                {
                    using (profiler.Step("Reload"))
                    {
                        batcher.Reset();
                        foreach (var result in results)
                        {
                            // Assume that FE don't know about new items from AfterSave hook,
                            // so, update ids of them
                            foreach (var item in result.ProcessedItems)
                            {
                                item.UpdateId();
                            }
                        }

                        foreach (var result in results)
                        {
                            var fields = GetFieldsForReload(result, context);
                            await result.Loader.ReloadAsync(context, result, fields).ConfigureAwait(false);
                        }
                    }
                }

                return results;
            }
        }

        public static Dictionary<string, IList<ISaveResultItem>> ConvertItems<TExecutionContext>(IEnumerable<ISaveResult<TExecutionContext>> results)
        {
            var processedItems = new Dictionary<string, IList<ISaveResultItem>>();

            foreach (var result in results)
            {
                var itemsToAdd = result.ProcessedItems
                    .GroupBy(item => item.Id)
                    .Select(group => group.Aggregate((first, second) => first.Merge(second)))
                    .ToList();

                processedItems.Add(
                    result.FieldName,
                    itemsToAdd);
            }

            return processedItems;
        }

        public static IEnumerable<string> GetFieldsForReload<TExecutionContext>(ISaveResult<TExecutionContext> result, IResolveFieldContext context)
        {
            var initialDictionary = context.SubFields;

            if (context.ReturnType.GetType().IsGenericType && (context.ReturnType.GetType().GetGenericTypeDefinition() == typeof(MutationResultGraphType<,,>)))
            {
                if (!initialDictionary.ContainsKey("payload"))
                {
                    return Enumerable.Empty<string>();
                }

                initialDictionary = initialDictionary["payload"].SelectionSet.Children.Cast<Field>().ToDictionary(f => f.Name);
            }

            if (!initialDictionary.ContainsKey(result.FieldName))
            {
                return Enumerable.Empty<string>();
            }

            initialDictionary = initialDictionary[result.FieldName].SelectionSet.Children.Cast<Field>().ToDictionary(f => f.Name);

            if (!initialDictionary.ContainsKey("payload"))
            {
                return Enumerable.Empty<string>();
            }

            var fields = initialDictionary["payload"].GetSubFieldsNames(context.Fragments, _ => true);

            return fields;
        }
    }

    internal class DbContextSaver<TExecutionContext>
    {
        private readonly Dictionary<string, IMutableLoader<TExecutionContext>> _loaders = new();
        private readonly IDictionary<string, IEnumerable<IInputItem>> _inputItems;
        private readonly Mutation<TExecutionContext> _mutation;
        private readonly RelationRegistry<TExecutionContext> _registry;
        private readonly SubmitInputTypeRegistry<TExecutionContext> _submitInputTypeRegistry;

        public DbContextSaver(RelationRegistry<TExecutionContext> registry, Mutation<TExecutionContext> mutation, IResolveFieldContext context, IDictionary<string, IEnumerable<IInputItem>> inputItems)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            _inputItems = inputItems ?? throw new ArgumentNullException(nameof(inputItems));
            _mutation = mutation ?? throw new ArgumentNullException(nameof(mutation));
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
            _submitInputTypeRegistry = registry.GetRequiredService<SubmitInputTypeRegistry<TExecutionContext>>();

            foreach (var kv in inputItems)
            {
                if (inputItems[kv.Key] != null)
                {
                    _loaders.Add(kv.Key, registry.ResolveLoader(_submitInputTypeRegistry.GetLoaderTypeByFieldName(mutation.GetType(), kv.Key)));
                }
            }
        }

        private IResolveFieldContext Context { get; }

        public async Task<Dictionary<string, IList<ISaveResultItem>>> SaveEntitiesAsync()
        {
            var profiler = Context.GetProfiler();

            using (profiler.Step(nameof(SaveEntitiesAsync)))
            {
                var results = _loaders
                    .Select(kv => kv.Value.CreateSaveResultFromValues(_mutation.GetType(), kv.Key, _inputItems[kv.Key]));

                var db = Context.GetDataContext();
                await db.ExecuteInTransactionAsync(async () =>
                {
                    results = await SavePendingItems(results).ConfigureAwait(false);

                    // Save postponed items
                    if (results.Any(result => result.PostponedItems.Count > 0))
                    {
                        results = results.Select(result => result.CloneAndMovePostponedToPending());
                        UpdatePostponedRelations(results);

                        results = await SavePendingItems(results).ConfigureAwait(false);

                        if (results.Any(result => result.PostponedItems.Any()))
                        {
                            throw new ExecutionError("Internal error: save");
                        }
                    }

                    var afterSaveEntities = await _mutation.DoAfterSave(
                        Context,
                        results
                            .SelectMany(result => result.ProcessedItems)
                            .Where(item => item.Payload != null)
                            .Select(item => item.Payload!)).ConfigureAwait(false);

                    var afterSaveResults = await DbContextSaver.SaveChangesAndReload(
                        _registry,
                        afterSaveEntities,
                        _mutation,
                        Context,
                        DbContextSaver.DefaultOptions).ConfigureAwait(false);

                    using (profiler.Step("Reload"))
                    {
                        var batcher = Context.GetBatcher();

                        batcher.Reset();

                        foreach (var result in results)
                        {
                            await result.Loader.ReloadAsync(Context, result, DbContextSaver.GetFieldsForReload(result, Context)).ConfigureAwait(false);
                        }

                        results = results
                            .Concat(afterSaveResults)
                            .GroupBy(r => r.EntityType)
                            .Select(group => group.Aggregate((first, second) => first.Merge(second)));
                    }
                }).ConfigureAwait(false);

                return DbContextSaver.ConvertItems(results);
            }
        }

        private static IEnumerable<ISaveResult<TExecutionContext>> Merge(IEnumerable<ISaveResult<TExecutionContext>> saveResults)
        {
            return saveResults
                .GroupBy(result => result.FieldName)
                .Select(group => group.Aggregate((first, second) => first.Merge(second)));
        }

        private async Task<IEnumerable<ISaveResult<TExecutionContext>>> SavePendingItems(IEnumerable<ISaveResult<TExecutionContext>> saveResults)
        {
            var profiler = Context.GetProfiler();

            using (profiler.Step(nameof(SavePendingItems)))
            {
                var results = saveResults;
                var tryCount = results.Sum(result => result.PendingItems.Count);

                while (results.Any(result => result.PendingItems.Count > 0))
                {
                    var tasks = new List<IEnumerable<ISaveResult<TExecutionContext>>>();
                    foreach (var result in results)
                    {
                        var loader = result.Loader;
                        tasks.Add(await loader.MutateAsync(Context, result).ConfigureAwait(false));
                    }

                    results = tasks.SelectMany(r => r);

                    results = Merge(results);

                    UpdateRelations(results);

                    await Context.GetDataContext().SaveChangesAsync().ConfigureAwait(false);

                    if (tryCount-- < 0)
                    {
                        throw new ExecutionError("Circular reference detected.");
                    }
                }

                return results;
            }
        }

        private void UpdateRelations(IEnumerable<ISaveResult<TExecutionContext>> results)
        {
            foreach (var result in results)
            {
                var loader = result.Loader;

                foreach (var resultItem in result.ProcessedItems)
                {
                    if (loader.IsFakeId(resultItem.Id))
                    {
                        foreach (var kv in results)
                        {
                            var childEntityType = _submitInputTypeRegistry.GetEntityTypeByFieldName(_mutation.GetType(), kv.FieldName);
                            var childLoaderType = _submitInputTypeRegistry.GetLoaderTypeByFieldName(_mutation.GetType(), kv.FieldName);
                            foreach (var item in kv.PendingItems)
                            {
                                _registry.UpdateFakePropertyValues(resultItem.Payload, item.Payload, item.Properties, resultItem.Id, childLoaderType, childEntityType);
                            }
                        }
                    }
                }
            }
        }

        private void UpdatePostponedRelations(IEnumerable<ISaveResult<TExecutionContext>> results)
        {
            foreach (var result in results)
            {
                var loader = result.Loader;

                foreach (var resultItem in result.ProcessedItems)
                {
                    if (loader.IsFakeId(resultItem.Id))
                    {
                        foreach (var kv in results)
                        {
                            var childEntityType = _submitInputTypeRegistry.GetEntityTypeByFieldName(_mutation.GetType(), kv.FieldName);
                            foreach (var item in kv.PendingItems)
                            {
                                _registry.UpdatePostponedFakePropertyValues(resultItem.Payload, item.Payload, item.Properties, resultItem.Id, childEntityType);
                            }
                        }
                    }
                }
            }
        }
    }
}
