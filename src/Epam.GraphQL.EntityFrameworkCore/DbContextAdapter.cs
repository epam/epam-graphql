// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Epam.GraphQL.EntityFrameworkCore
{
    internal class DbContextAdapter : IDataContext
    {
        private static MethodInfo? _asNoTracking;
        private readonly DbContext _dbContext;

        public DbContextAdapter(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        private static MethodInfo AsNoTracking => _asNoTracking ??= new Func<IQueryable<object>, IQueryable<object>>(EntityFrameworkQueryableExtensions.AsNoTracking).GetMethodInfo().GetGenericMethodDefinition();

        public void AddRange<TEntity>(IEnumerable<TEntity> entityList)
            where TEntity : class
        {
            _dbContext.AddRange(entityList);
        }

        public void DetachEntity<TEntity>(TEntity entity)
            where TEntity : class
        {
            _dbContext.Entry(entity).State = EntityState.Detached;
        }

        public IQueryable<TEntity> GetQueryable<TEntity>()
            where TEntity : class
        {
            throw new NotImplementedException();
        }

        public IAsyncEnumerable<TEntity> QueryableToAsyncEnumerable<TEntity>(IQueryable<TEntity> query)
        {
            return query.AsAsyncEnumerable();
        }

        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync().ConfigureAwait(false);
        }

        public Task ExecuteInTransactionAsync(Func<Task> action)
        {
            return _dbContext.Database.CreateExecutionStrategy().ExecuteInTransactionAsync(
                action,
                () => Task.FromResult(false));
        }

        public IQueryable<T> QueryableToAsNoTrackingQueryable<T>(IQueryable<T> query)
        {
            if (typeof(T).IsValueType)
            {
                return query;
            }

            var asNoTracking = AsNoTracking.MakeGenericMethod(typeof(T));

            return (IQueryable<T>)asNoTracking.Invoke(null, new[] { query });
        }
    }
}
