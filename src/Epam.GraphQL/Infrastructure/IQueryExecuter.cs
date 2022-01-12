// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;

namespace Epam.GraphQL.Infrastructure
{
    internal interface IQueryExecuter
    {
        IAsyncEnumerable<TEntity> ToAsyncEnumerable<TEntity>(Func<string> stepNameFactory, IQueryable<TEntity> query);

        IEnumerable<TEntity> ToEnumerable<TEntity>(Func<string> stepNameFactory, IQueryable<TEntity> query);

        List<TEntity> ToList<TEntity>(Func<string> stepNameFactory, IQueryable<TEntity> query);

        TReturnType Execute<TEntity, TReturnType>(Func<string> stepNameFactory, IQueryable<TEntity> query, Func<IQueryable<TEntity>, TReturnType> transform, string transformName);
    }
}
