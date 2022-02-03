// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Epam.GraphQL.Configuration.Enums;
using Epam.GraphQL.Configuration.Implementations.Fields.ExpressionFields;
using Epam.GraphQL.Helpers;

namespace Epam.GraphQL.Builders.Common.Implementations
{
    internal class FilterableAndSortableAndGroupableFieldBuilder<TEntity, TReturnType, TExecutionContext> : SortableAndGroupableFieldBuilder<TEntity, TReturnType, TExecutionContext>,
        IHasFilterableAndSortableAndGroupable<TEntity, TReturnType>,
        IHasFilterableAndSortable<TEntity, TReturnType>
        where TEntity : class
    {
        internal FilterableAndSortableAndGroupableFieldBuilder(ExpressionField<TEntity, TReturnType, TExecutionContext> field)
            : base(field)
        {
        }

        public IHasSortableAndGroupable<TEntity> Filterable()
        {
            Field.Filterable();
            return this;
        }

        public IHasSortableAndGroupable<TEntity> Filterable(params TReturnType[] defaultValues)
        {
            Field.Filterable(defaultValues);
            return this;
        }

        public IHasSortableAndGroupable<TEntity> Filterable(NullOption nullValue)
        {
            Field.Filterable(nullValue);
            return this;
        }

        IHasSortable<TEntity, IVoid> IHasFilterable<TEntity, TReturnType, IHasSortable<TEntity, IVoid>>.Filterable()
        {
            Filterable();
            return this;
        }

        IHasSortable<TEntity, IVoid> IHasFilterable<TEntity, TReturnType, IHasSortable<TEntity, IVoid>>.Filterable(params TReturnType[] defaultValues)
        {
            Filterable(defaultValues);
            return this;
        }

        IHasSortable<TEntity, IVoid> IHasFilterable<TEntity, TReturnType, IHasSortable<TEntity, IVoid>>.Filterable(NullOption nullValue)
        {
            Filterable(nullValue);
            return this;
        }
    }
}
