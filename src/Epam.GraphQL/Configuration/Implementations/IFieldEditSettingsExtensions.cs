// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.TaskBatcher;
using GraphQL;
using GraphQL.DataLoader;

namespace Epam.GraphQL.Configuration.Implementations
{
    internal static class IFieldEditSettingsExtensions
    {
        public static void MandatoryForUpdate<TEntity, TReturnType, TExecutionContext>(this IFieldEditSettings<TEntity, TReturnType, TExecutionContext> settings)
        {
            settings.IsMandatoryForUpdate = true;
        }

        public static void Default<TEntity, TReturnType, TExecutionContext>(this IFieldEditSettings<TEntity, TReturnType, TExecutionContext> settings, Func<TExecutionContext, TEntity, TReturnType> selector)
        {
            settings.GetDefaultValue = (ctx, entity) => selector(ctx.GetUserContext<TExecutionContext>(), (TEntity)entity);
        }

        public static void Editable<TEntity, TReturnType, TExecutionContext>(this IFieldEditSettings<TEntity, TReturnType, TExecutionContext> settings)
        {
            EditableIf(settings, FuncConstants<IFieldChange<TEntity, TExecutionContext>>.TruePredicate, FuncConstants<IFieldChange<TEntity, TExecutionContext>, string>.DefaultResultFunc);
        }

        public static void EditableIf<TEntity, TReturnType, TExecutionContext>(this IFieldEditSettings<TEntity, TReturnType, TExecutionContext> settings, Func<IFieldChange<TEntity, TReturnType, TExecutionContext>, bool> predicate, Func<IFieldChange<TEntity, TReturnType, TExecutionContext>, string> reason)
        {
            settings.CanEdit = ctx => BatchLoader.FromResult<IFieldChange<TEntity, TExecutionContext>, (bool, string)>(change => (predicate((IFieldChange<TEntity, TReturnType, TExecutionContext>)change), reason.Safe()((IFieldChange<TEntity, TReturnType, TExecutionContext>)change)));
        }

        public static void EditableIf<TEntity, TReturnType, TExecutionContext, TItem>(this IFieldEditSettings<TEntity, TReturnType, TExecutionContext> settings, Func<TExecutionContext, IEnumerable<TEntity>, IEnumerable<KeyValuePair<TEntity, TItem>>> batchFunc, Func<IBatchFieldChange<TEntity, TReturnType, TItem, TExecutionContext>, bool> predicate, Func<IBatchFieldChange<TEntity, TReturnType, TItem, TExecutionContext>, string> reason)
        {
            settings.CanEdit = GetEditableIf<TEntity, TReturnType, TItem, TExecutionContext, (bool, string)>(batchFunc, item => (predicate(item), reason.Safe()(item)), nameof(EditableIf));
        }

        public static void EditableIf<TEntity, TReturnType, TExecutionContext, TItem>(this IFieldEditSettings<TEntity, TReturnType, TExecutionContext> settings, Func<IFieldChange<TEntity, TExecutionContext>, bool> predicate, Func<IFieldChange<TEntity, TExecutionContext>, string> reason = null)
        {
            var сanEdit = BatchLoader.FromResult<IFieldChange<TEntity, TExecutionContext>, (bool, string)>(change => (predicate(change), reason.Safe()(change)));
            settings.CanEdit = ctx => сanEdit;
        }

        public static void BatchedEditableIf<TEntity, TReturnType, TExecutionContext, TItem>(this IFieldEditSettings<TEntity, TReturnType, TExecutionContext> settings, Func<TExecutionContext, IEnumerable<TEntity>, IEnumerable<KeyValuePair<TEntity, TItem>>> batchFunc, Func<IBatchFieldChange<TEntity, TReturnType, TItem, TExecutionContext>, bool> predicate, Func<IBatchFieldChange<TEntity, TReturnType, TItem, TExecutionContext>, string> reason = null)
        {
            settings.CanEdit = GetEditableIf<TEntity, TReturnType, TItem, TExecutionContext, (bool, string)>(batchFunc, item => (predicate(item), reason.Safe()(item)), nameof(BatchedEditableIf));
        }

        public static void SetOnWrite<TEntity, TReturnType, TExecutionContext>(this IFieldEditSettings<TEntity, TReturnType, TExecutionContext> settings, Action<TExecutionContext, TEntity, TReturnType> save)
        {
            settings.OnWrite = (ctx, entity, result) => save(ctx.GetUserContext<TExecutionContext>(), entity, result);
        }

        public static void SetOnWrite<TEntity, TReturnType, TExecutionContext>(this IFieldEditSettings<TEntity, TReturnType, TExecutionContext> settings, Func<TExecutionContext, TEntity, TReturnType, Task> save)
        {
            settings.OnWriteAsync = (ctx, entity, result) => save(ctx.GetUserContext<TExecutionContext>(), entity, result);
        }

        private static Func<IResolveFieldContext, IDataLoader<IFieldChange<TEntity, TExecutionContext>, TResult>> GetEditableIf<TEntity, TReturnType, TItem, TExecutionContext, TResult>(Func<TExecutionContext, IEnumerable<TEntity>, IEnumerable<KeyValuePair<TEntity, TItem>>> batchFunc, Func<IBatchFieldChange<TEntity, TReturnType, TItem, TExecutionContext>, TResult> func, string callerName)
        {
            IDataLoader<IFieldChange<TEntity, TExecutionContext>, (IFieldChange<TEntity, TExecutionContext> Change, TItem Item)> BatchCall(IResolveFieldContext context)
            {
                Func<IFieldChange<TEntity, TExecutionContext>, TEntity> entityGetter = change => change.Entity;
                var batcher = context.GetBatcher();
                var task = batcher.Get(() => callerName, context.GetUserContext<TExecutionContext>(), batchFunc);

                return entityGetter.Then(task)
                    .Then<IFieldChange<TEntity, TExecutionContext>, TItem, (IFieldChange<TEntity, TExecutionContext>, TItem)>((change, entity) => (change, entity));
            }

            return context => BatchCall(context).Then(arg =>
            {
                var (change, result) = arg;
                var batchFieldChange = new BatchFieldChange<TEntity, TReturnType, TItem, TExecutionContext>(
                    context.GetUserContext<TExecutionContext>(),
                    change.Entity,
                    ((IFieldChange<TEntity, TReturnType, TExecutionContext>)change).PreviousValue,
                    ((IFieldChange<TEntity, TReturnType, TExecutionContext>)change).NextValue,
                    result);

                return func(batchFieldChange);
            });
        }
    }
}
