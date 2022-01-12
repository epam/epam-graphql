// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Linq.Expressions;
using System.Reflection;
using Epam.GraphQL.Filters;

namespace Epam.GraphQL.Configuration.Implementations
{
    internal interface IExpressionField<TEntity, TExecutionContext> : IField<TEntity, TExecutionContext>
    {
        PropertyInfo? PropertyInfo { get; }

        LambdaExpression ContextExpression { get; }

        LambdaExpression OriginalExpression { get; }

        bool IsFilterable { get; set; }

        bool IsGroupable { get; }

        IInlineFilter<TExecutionContext> CreateInlineFilter();
    }
}
