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

#nullable enable

namespace Epam.GraphQL.Filters.Implementations
{
    internal class StringInlineFilter<TEntity, TExecutionContext> : BaseInlineFilter<TEntity, string?, string?, string?, TExecutionContext>
        where TEntity : class
    {
        public StringInlineFilter(ExpressionField<TEntity, string, TExecutionContext> field, string[]? defaultValues, NullOption? nullValue)
            : base(field, defaultValues, nullValue)
        {
        }

        protected override Expression<Func<TEntity, bool>> BuildContainsAsExpression(TExecutionContext context, IEnumerable<string?> list)
        {
            return ExpressionHelpers.MakeContainsExpression(list, BuildExpression(context));
        }

        protected override Expression<Func<TEntity, bool>> BuildIsNullExpression(TExecutionContext context, bool isNull)
        {
            var result = BuildExpression(context).MakeComparisonExpression(ComparisonType.Eq, default);
            if (!isNull)
            {
                result = result.Not();
            }

            return result;
        }

        protected override Expression<Func<TEntity, bool>> BuildComparisonExpression(TExecutionContext context, ComparisonType comparisonType, string? value)
        {
            if (comparisonType is not ComparisonType.Eq and not ComparisonType.Neq)
            {
                throw new ArgumentException($"Value should be either {nameof(ComparisonType)}.{nameof(ComparisonType.Eq)} or {nameof(ComparisonType)}.{nameof(ComparisonType.Neq)}.", nameof(comparisonType));
            }

            return BuildExpression(context).MakeComparisonExpression(comparisonType, value);
        }
    }
}
