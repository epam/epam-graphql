// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Diagnostics;

namespace Epam.GraphQL.Filters
{
    internal interface IInlineFilters<TExecutionContext>
    {
        Type FilterType { get; }

        LambdaExpression BuildExpression(TExecutionContext executionContext, object? filter);

        void Validate(IConfigurationContext configurationContext);
    }

    internal interface IInlineFilters<TEntity, TExecutionContext> : IInlineFilters<TExecutionContext>, IFilter<TEntity, TExecutionContext>
    {
        new Type FilterType { get; }
    }
}
