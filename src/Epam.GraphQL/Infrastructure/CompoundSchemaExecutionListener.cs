// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Extensions;

namespace Epam.GraphQL.Infrastructure
{
    internal class CompoundSchemaExecutionListener : ISchemaExecutionListener
    {
        private readonly IEnumerable<ISchemaExecutionListener> _listeners;

        public CompoundSchemaExecutionListener(IEnumerable<ISchemaExecutionListener>? listeners)
        {
            _listeners = listeners ?? Enumerable.Empty<ISchemaExecutionListener>();
        }

        public Expression<Func<TEntity, bool>>? GetAdditionalFilter<TExecutionContext, TEntity, TFilter>(TExecutionContext context, TFilter filter)
        {
            return _listeners
                .Select(listener => listener.GetAdditionalFilter<TExecutionContext, TEntity, TFilter>(context, filter))
                .Aggregate<Expression<Func<TEntity, bool>>?, Expression<Func<TEntity, bool>>?>(null, ExpressionExtensions.SafeAnd);
        }
    }
}
