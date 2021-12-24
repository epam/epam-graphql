// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;

namespace Epam.GraphQL.Filters
{
    internal interface IInlineFilters
    {
        Type FilterType { get; }

        LambdaExpression BuildExpression(object executionContext, object filter);
    }

    internal interface IInlineFilters<TEntity, TExecutionContext> : IInlineFilters, IFilter<TEntity, TExecutionContext>
    {
        new Type FilterType { get; }
    }
}
