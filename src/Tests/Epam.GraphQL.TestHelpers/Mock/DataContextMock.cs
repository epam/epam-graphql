// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Epam.GraphQL.Tests.Mock
{
    public class DataContextMock : IDataContext
    {
        public void AddRange<TEntity>(IEnumerable<TEntity> entityList)
            where TEntity : class
        {
        }

        public void DetachEntity<TEntity>(TEntity entity)
            where TEntity : class
        {
        }

        public Task ExecuteInTransactionAsync(Func<Task> action) => action?.Invoke();

        public IQueryable<TEntity> GetQueryable<TEntity>()
            where TEntity : class
        {
            throw new NotImplementedException();
        }

        public IQueryable<T> QueryableToAsNoTrackingQueryable<T>(IQueryable<T> query) => query;

        public IAsyncEnumerable<TEntity> QueryableToAsyncEnumerable<TEntity>(IQueryable<TEntity> query) => query.ToAsyncEnumerable();

        public Task SaveChangesAsync() => Task.CompletedTask;
    }
}
