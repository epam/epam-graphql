// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epam.GraphQL.Adapters;
using Epam.GraphQL.Infrastructure;

namespace Epam.GraphQL
{
    [InternalApi]
    public interface IDataContext : IQueryableToAsyncEnumerableConverter, IQueryableToAsNoTrackingQueryableConverter
    {
        void AddRange<TEntity>(IEnumerable<TEntity> entityList)
            where TEntity : class;

        void DetachEntity<TEntity>(TEntity entity)
            where TEntity : class;

        Task SaveChangesAsync();

        IQueryable<TEntity> GetQueryable<TEntity>()
            where TEntity : class;

        Task ExecuteInTransactionAsync(Func<Task> action);
    }
}
