// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Implementations.Fields.ExpressionFields;

#nullable enable

namespace Epam.GraphQL.Builders.Common.Implementations
{
    internal class SortableAndGroupableFieldBuilder<TEntity, TReturnType, TExecutionContext> :
        GroupableFieldBuilder<TEntity, TReturnType, TExecutionContext>,
        IHasSortableAndGroupable<TEntity>,
        IHasSortable<TEntity, IEmptyBuilder>
        where TEntity : class
    {
        internal SortableAndGroupableFieldBuilder(ExpressionField<TEntity, TReturnType, TExecutionContext> field)
            : base(field)
        {
        }

        public IHasGroupable Sortable()
        {
            Field.Sortable();
            return this;
        }

        public IHasGroupable Sortable<TValue>(Expression<Func<TEntity, TValue>> sorter)
        {
            Field.Sortable(sorter);
            return this;
        }

        IEmptyBuilder IHasSortable<TEntity, IEmptyBuilder>.Sortable()
        {
            Sortable();
            return EmptyBuilder.Instance;
        }

        IEmptyBuilder IHasSortable<TEntity, IEmptyBuilder>.Sortable<TValue>(Expression<Func<TEntity, TValue>> sorter)
        {
            Sortable(sorter);
            return EmptyBuilder.Instance;
        }
    }
}
