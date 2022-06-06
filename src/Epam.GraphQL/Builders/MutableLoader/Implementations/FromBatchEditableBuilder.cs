// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Configuration.Implementations;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Builders.MutableLoader.Implementations
{
    internal class FromBatchEditableBuilder<TSourceType, TReturnType, TExecutionContext> :
        IHasEditable<TSourceType, TReturnType, TExecutionContext>,
        IHasEditableAndOnWrite<TSourceType, TReturnType, TExecutionContext>,
        IHasEditableAndOnWriteAndMandatoryForUpdate<TSourceType, TReturnType, TExecutionContext>
    {
        internal FromBatchEditableBuilder(RelationRegistry<TExecutionContext> registry, IFieldSupportsEditSettings<TSourceType, TReturnType, TExecutionContext> field)
        {
            Guards.ThrowIfNull(field.EditSettings, nameof(field.EditSettings));

            Field = (IField<TSourceType, TExecutionContext>)field;
            Registry = registry;
            Settings = field.EditSettings;
        }

        protected RelationRegistry<TExecutionContext> Registry { get; }

        protected IField<TSourceType, TExecutionContext> Field { get; }

        protected IFieldEditSettings<TSourceType, TReturnType, TExecutionContext> Settings { get; }

        public IHasEditable<TSourceType, TReturnType, TExecutionContext> OnWrite(Action<TExecutionContext, TSourceType, TReturnType> save)
        {
            Settings.SetOnWrite(save);
            return this;
        }

        public IHasEditable<TSourceType, TReturnType, TExecutionContext> OnWrite(Func<TExecutionContext, TSourceType, TReturnType, Task> save)
        {
            Settings.SetOnWrite(save);
            return this;
        }

        public void EditableIf(
            Func<IFieldChange<TSourceType, TReturnType, TExecutionContext>, bool> predicate,
            Func<IFieldChange<TSourceType, TReturnType, TExecutionContext>, string>? reason)
        {
            Settings.EditableIf(predicate, reason);
        }

        public void Editable()
        {
            Settings.Editable();
        }

        public void BatchedEditableIf<TItem>(
            Func<IEnumerable<TSourceType>, IEnumerable<KeyValuePair<TSourceType, TItem>>> batchFunc,
            Func<IBatchFieldChange<TSourceType, TReturnType, TItem, TExecutionContext>, bool> predicate,
            Func<IBatchFieldChange<TSourceType, TReturnType, TItem, TExecutionContext>, string>? reason)
        {
            var configurationContext = Field.ConfigurationContext
                .Chain(nameof(BatchedEditableIf))
                .Argument(batchFunc)
                .Argument(predicate)
                .OptionalArgument(reason);

            Settings.BatchedEditableIf(configurationContext, Registry.WrapFuncByUnusedContext(batchFunc), predicate, reason);
        }

        public void BatchedEditableIf<TItem>(
            Func<TExecutionContext, IEnumerable<TSourceType>, IEnumerable<KeyValuePair<TSourceType, TItem>>> batchFunc,
            Func<IBatchFieldChange<TSourceType, TReturnType, TItem, TExecutionContext>, bool> predicate,
            Func<IBatchFieldChange<TSourceType, TReturnType, TItem, TExecutionContext>, string>? reason)
        {
            var configurationContext = Field.ConfigurationContext
                .Chain(nameof(BatchedEditableIf))
                .Argument(batchFunc)
                .Argument(predicate)
                .OptionalArgument(reason);

            Settings.BatchedEditableIf(configurationContext, batchFunc, predicate, reason);
        }

        public IHasEditableAndOnWrite<TSourceType, TReturnType, TExecutionContext> MandatoryForUpdate()
        {
            Settings.MandatoryForUpdate();
            return this;
        }
    }
}
