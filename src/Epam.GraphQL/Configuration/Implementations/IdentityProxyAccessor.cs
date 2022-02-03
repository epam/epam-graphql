// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Relay;
using Epam.GraphQL.Sorters;
using GraphQL;

namespace Epam.GraphQL.Configuration.Implementations
{
    internal class IdentityProxyAccessor<TEntity, TExecutionContext>
        : IProxyAccessor<TEntity, TEntity, TExecutionContext>
    {
        public static IProxyAccessor<TEntity, TEntity, TExecutionContext> Instance { get; } = new IdentityProxyAccessor<TEntity, TExecutionContext>();

        public bool HasHooks => false;

        private static Expression<Func<TExecutionContext, TEntity, TEntity>> Identity => (_, entity) => entity;

        public void AddMember<TResult>(Expression<Func<TEntity, TResult>> member)
        {
        }

        public void AddMembers(IEnumerable<LambdaExpression> members)
        {
        }

        public ILoaderHooksExecuter<TEntity>? CreateHooksExecuter(IResolveFieldContext context)
        {
            return null;
        }

        public Expression<Func<TExecutionContext, TEntity, TEntity>> CreateSelectorExpression(IEnumerable<string> fieldNames)
        {
            return Identity;
        }

        public IReadOnlyList<(LambdaExpression SortExpression, SortDirection SortDirection)> GetGroupSort(IResolveFieldContext context, IEnumerable<ISorter<TExecutionContext>> sorters)
        {
            return new List<(LambdaExpression SortExpression, SortDirection SortDirection)>();
        }

        public IQueryable<IGroupResult<TEntity>> GroupBy(IResolveFieldContext context, IQueryable<TEntity> query)
        {
            var hasItem = context.HasGroupConnectionItemField();
            var aggregateQueriedFields = context.GetGroupConnectionAggregateQueriedFields();

            if (aggregateQueriedFields.Contains("count"))
            {
                if (hasItem)
                {
                    return query
                        .GroupBy(x => x)
                        .Select(x => new GroupResult<TEntity>
                        {
                            Item = x.Key,
                            Count = x.Count(),
                        });
                }

                return query
                    .GroupBy(x => 0)
                    .Select(x => new GroupResult<TEntity>
                    {
                        Count = x.Count(),
                    });
            }

            Guards.AssertField(!hasItem, context);

            return query
                .GroupBy(x => x)
                .Select(x => new GroupResult<TEntity>
                {
                    Item = x.Key,
                });
        }

        public void RemoveMember<TResult>(Expression<Func<TEntity, TResult>> member)
        {
        }

        public Expression<Func<TEntity, T>> Rewrite<T>(Expression<Func<TEntity, T>> originalExpression)
        {
            return originalExpression;
        }

        public LambdaExpression Rewrite(LambdaExpression originalExpression)
        {
            return originalExpression;
        }
    }
}
