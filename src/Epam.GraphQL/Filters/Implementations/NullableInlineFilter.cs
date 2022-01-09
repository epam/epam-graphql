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
    internal class NullableInlineFilter<TEntity, TReturnType, TExecutionContext> : BaseInlineFilter<TEntity, TReturnType?, TReturnType?, TReturnType, TExecutionContext>
        where TEntity : class
        where TReturnType : struct
    {
        public NullableInlineFilter(ExpressionField<TEntity, TReturnType?, TExecutionContext> field, TReturnType[] defaultValues, NullOption? nullValue)
            : base(field, defaultValues, nullValue)
        {
        }

        protected override Expression<Func<TEntity, bool>> BuildContainsAsExpression(TExecutionContext context, IEnumerable<TReturnType> list)
        {
            var propExpr = BuildExpression(context);
            var result = ExpressionHelpers.MakeContainsExpression(list, propExpr);

            return result;
        }

        protected override Expression<Func<TEntity, bool>> BuildIsNullExpression(TExecutionContext context, bool isNull)
        {
            var result = BuildExpression(context).MakeComparisonExpression(ComparisonType.Eq, null);
            if (!isNull)
            {
                result = result.Not();
            }

            return result;
        }

        protected override Expression<Func<TEntity, bool>> BuildComparisonExpression(TExecutionContext context, ComparisonType comparisonType, TReturnType? value)
        {
            return BuildExpression(context).MakeComparisonExpression(comparisonType, value);
        }
    }
}
