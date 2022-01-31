// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Filters;
using Epam.GraphQL.Filters.Implementations;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ExpressionFields
{
    internal class StructExpressionField<TEntity, TReturnType, TExecutionContext> : ExpressionField<TEntity, TReturnType, TReturnType, TExecutionContext>
        where TReturnType : struct
        where TEntity : class
    {
        public StructExpressionField(BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent, Expression<Func<TEntity, TReturnType>> expression, string? name)
            : base(parent, expression, name)
        {
        }

        public StructExpressionField(BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent, Expression<Func<TExecutionContext, TEntity, TReturnType>> expression, string name)
            : base(parent, expression, name)
        {
        }

        public override IInlineFilter<TExecutionContext> OnCreateInlineFilter()
        {
            return new StructInlineFilter<TEntity, TReturnType, TExecutionContext>(this, DefaultValues, NullValue);
        }
    }
}
