// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Enums;
using Epam.GraphQL.Configuration.Implementations.Fields.ExpressionFields;
using Epam.GraphQL.Enums;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;

namespace Epam.GraphQL.Filters.Implementations
{
    internal class StructInlineFilter<TEntity, TReturnType, TExecutionContext> : BaseInlineFilter<TEntity, TReturnType, TReturnType?, TReturnType, TExecutionContext>
        where TEntity : class
        where TReturnType : struct
    {
        public StructInlineFilter(ExpressionField<TEntity, TReturnType, TExecutionContext> field, TReturnType[] defaultValues, NullOption? nullValue)
            : base(field, defaultValues, nullValue)
        {
        }

        protected override Expression<Func<TEntity, bool>> BuildContainsAsExpression(TExecutionContext context, IEnumerable<TReturnType> list)
        {
            return ExpressionHelpers.MakeContainsExpression(list, BuildExpression(context));
        }

        protected override Expression<Func<TEntity, bool>> BuildIsNullExpression(TExecutionContext context, bool isNull)
        {
            return isNull
                ? FuncConstants<TEntity>.FalseExpression
                : FuncConstants<TEntity>.TrueExpression;
        }

        protected override Expression<Func<TEntity, bool>> BuildComparisonExpression(TExecutionContext context, ComparisonType comparisonType, TReturnType? value)
        {
            if (value.HasValue)
            {
                return BuildExpression(context).MakeComparisonExpression(comparisonType, value.Value);
            }

            throw new NotSupportedException();
        }
    }
}
