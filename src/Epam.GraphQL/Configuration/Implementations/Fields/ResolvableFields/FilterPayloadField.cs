// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Filters;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields
{
    internal class FilterPayloadField<TExecutionContext> : IArgument<PayloadFieldsContext<TExecutionContext>>
    {
        private readonly Lazy<IInlineFilters<TExecutionContext>> _inlineFilters;
        private readonly Type _projectionType;
        private readonly Type _entityType;

        public FilterPayloadField(RelationRegistry<TExecutionContext> registry, string name, Type projectionType, Type entityType)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            _projectionType = projectionType;
            _entityType = entityType;

            _inlineFilters = new Lazy<IInlineFilters<TExecutionContext>>(() =>
            {
                var configurator = registry.GetObjectGraphTypeConfigurator(_entityType, _projectionType);

                if (configurator == null)
                {
                    throw new NotSupportedException();
                }

                var inlineFilters = configurator.CreateInlineFilters();
                return inlineFilters;
            });
        }

        public string Name { get; }

        public Type InputType => _inlineFilters.Value.FilterType;

        public TOut GetValue<TOut>(PayloadFieldsContext<TExecutionContext> context)
        {
            var filter = context.GetPropertyValue(Name);
            var executionContext = context.ExecutionContext;
            var predicate = (object)_inlineFilters.Value.BuildExpression(executionContext, filter);
            return (TOut)predicate;
        }
    }
}
