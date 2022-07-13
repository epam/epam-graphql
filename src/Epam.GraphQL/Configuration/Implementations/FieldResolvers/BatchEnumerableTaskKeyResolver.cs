// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Diagnostics;

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal class BatchEnumerableTaskKeyResolver<TEntity, TKey, TReturnType, TExecutionContext> : BatchTaskKeyResolver<TEntity, TKey, IEnumerable<TReturnType>, TExecutionContext>
    {
        public BatchEnumerableTaskKeyResolver(
            IResolvedChainConfigurationContext configurationContext,
            string fieldName,
            Expression<Func<TEntity, TKey>> keySelector,
            Func<TExecutionContext, IEnumerable<TKey>, Task<IDictionary<TKey, IEnumerable<TReturnType>>>> batchFunc,
            ProxyAccessor<TEntity, TExecutionContext> proxyAccessor)
            : base(configurationContext, fieldName, keySelector, batchFunc, proxyAccessor)
        {
        }
    }
}
