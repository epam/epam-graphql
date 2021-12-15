﻿// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal class BatchEnumerableResolver<TEntity, TReturnType, TExecutionContext> : BatchResolver<TEntity, IEnumerable<TReturnType>, TExecutionContext>
        where TEntity : class
    {
        public BatchEnumerableResolver(
            string fieldName,
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc,
            ProxyAccessor<TEntity, TExecutionContext> proxyAccessor)
            : base(fieldName, batchFunc, proxyAccessor)
        {
        }
    }
}