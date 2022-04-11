// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Filters;
using Epam.GraphQL.Filters.Implementations;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ExpressionFields
{
    internal class NullableExpressionField<TEntity, TReturnType, TExecutionContext> : ExpressionField<TEntity, TReturnType?, TExecutionContext>
        where TReturnType : struct
        where TEntity : class
    {
        public NullableExpressionField(
            MethodCallConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            Expression<Func<TEntity, TReturnType?>> expression,
            string? name)
            : base(configurationContext, parent, expression, name)
        {
        }

        public NullableExpressionField(
            MethodCallConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            Expression<Func<TExecutionContext, TEntity, TReturnType?>> expression,
            string name)
            : base(configurationContext, parent, expression, name)
        {
        }

        protected override IInlineFilter<TExecutionContext> OnCreateInlineFilter()
        {
            return new NullableInlineFilter<TEntity, TReturnType, TExecutionContext>(this, DefaultValues, NullValue);
        }
    }
}
