// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Infrastructure;
using Epam.GraphQL.Loaders;

#nullable enable

namespace Epam.GraphQL.Extensions
{
    internal static class QueryableExtensions
    {
        public static IAsyncEnumerable<IGrouping<TProperty, TTransformedEntity>> GroupByValues<TEntity, TTransformedEntity, TProperty>(
            this IQueryable<TEntity> query,
            IEnumerable<TProperty> propValues,
            Expression<Func<TEntity, TProperty>> propSelector,
            Expression<Func<TEntity, TTransformedEntity>> transform,
            Func<string> stepNameFactory,
            IQueryExecuter queryExecuter,
            ILoaderHooksExecuter<TTransformedEntity>? hooksExecuter,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>? sorters)
        {
            var filtered = query.Where(ExpressionHelpers.MakeContainsExpression(propValues, propSelector));
            return GroupBy(filtered, propSelector, transform, stepNameFactory, queryExecuter, hooksExecuter, sorters);
        }

        public static IAsyncEnumerable<IGrouping<TProperty, TTransformedEntity>> GroupByValues<TEntity, TTransformedEntity, TProperty>(
            this IQueryable<TEntity> query,
            IEnumerable<TProperty> propValues,
            Expression<Func<TEntity, TProperty?>> propSelector,
            Expression<Func<TEntity, TTransformedEntity>> transform,
            Func<string> stepNameFactory,
            IQueryExecuter queryExecuter,
            ILoaderHooksExecuter<TTransformedEntity>? hooksExecuter,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>? sorters)
            where TProperty : struct
        {
            var keySelector = ExpressionHelpers.MakeValueAccessExpression(propSelector);
            var filtered = query.Where(ExpressionHelpers.MakeContainsExpression(propValues, propSelector));
            return GroupBy(filtered, keySelector, transform, stepNameFactory, queryExecuter, hooksExecuter, sorters);
        }

        public static IQueryable<T> SafeWhere<T>(this IQueryable<T> query, Expression<Func<T, bool>>? condition) =>
            condition == null ? query : query.Where(condition);

        public static IOrderedQueryable<TChildEntity> ApplyOrderBy<TChildEntity>(this IQueryable<TChildEntity> query, LambdaExpression expression, SortDirection sortDirection)
        {
            var sourceType = expression.Parameters[0].Type;
            var resultType = expression.GetResultType();

            return sortDirection == SortDirection.Asc
                ? CachedReflectionInfo.ForQueryable.OrderBy(sourceType, resultType).InvokeAndHoistBaseException<IOrderedQueryable<TChildEntity>>(null, query, expression)
                : CachedReflectionInfo.ForQueryable.OrderByDescending(sourceType, resultType).InvokeAndHoistBaseException<IOrderedQueryable<TChildEntity>>(null, query, expression);
        }

        public static IOrderedQueryable<T> ApplyOrderBy<T>(this IQueryable<T> query, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> sorters)
        {
            IOrderedQueryable<T>? orderedQuery = null;
            foreach (var sorter in sorters)
            {
                orderedQuery = orderedQuery == null
                    ? query.ApplyOrderBy(sorter.SortExpression, sorter.SortDirection)
                    : orderedQuery.ApplyThenBy(sorter.SortExpression, sorter.SortDirection);
            }

            if (orderedQuery == null)
            {
                throw new ArgumentException("The list of sorters must contain one element at least.", nameof(sorters));
            }

            return orderedQuery;
        }

        public static IOrderedQueryable<T> ApplyThenBy<T>(this IOrderedQueryable<T> query, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> sorters)
        {
            return sorters.Aggregate(query, (acc, sorter) => acc.ApplyThenBy(sorter.SortExpression, sorter.SortDirection));
        }

        public static IOrderedQueryable<TChildEntity> ApplyThenBy<TChildEntity>(this IOrderedQueryable<TChildEntity> query, LambdaExpression expression, SortDirection sortDirection)
        {
            var sourceType = expression.Parameters[0].Type;
            var resultType = expression.GetResultType();

            return sortDirection == SortDirection.Asc
                ? CachedReflectionInfo.ForQueryable.ThenBy(sourceType, resultType).InvokeAndHoistBaseException<IOrderedQueryable<TChildEntity>>(null, query, expression)
                : CachedReflectionInfo.ForQueryable.ThenByDescending(sourceType, resultType).InvokeAndHoistBaseException<IOrderedQueryable<TChildEntity>>(null, query, expression);
        }

        public static IQueryable ApplyGroupBy(this IQueryable query, LambdaExpression expression)
        {
            var sourceType = query.ElementType;
            var resultType = expression.GetResultType();
            return CachedReflectionInfo.ForQueryable.GroupBy(sourceType, resultType).InvokeAndHoistBaseException<IQueryable>(null, query, expression);
        }

        public static IQueryable ApplySelect(this IQueryable query, LambdaExpression expression)
        {
            var sourceType = query.ElementType;
            var resultType = expression.GetResultType();
            return CachedReflectionInfo.ForQueryable.Select(sourceType, resultType).InvokeAndHoistBaseException<IQueryable>(null, query, expression);
        }

        public static IReadOnlyList<(LambdaExpression SortExpression, SortDirection SortDirection)> GetSorters<T>(this IQueryable<T> query)
        {
            return ExpressionExtensions.SortVisitor<T>.GetSorters(query.Expression);
        }

        private static Expression<Func<TValue, KeyValue<TKey, TNewValue>>> CreateExpression<TKey, TValue, TNewValue>(
            Expression<Func<TValue, TKey>> keySelector,
            Expression<Func<TValue, TNewValue>> transform)
        {
            var param = Expression.Parameter(typeof(TValue));
            var ctor = typeof(KeyValue<TKey, TNewValue>).GetConstructors().Single(c => c.GetParameters().Length == 0);
            var newExpr = Expression.New(ctor);
            var keyBinding = Expression.Bind(
                typeof(KeyValue<TKey, TNewValue>).GetProperty(nameof(KeyValue<TKey, TNewValue>.Key)),
                keySelector.Body.ReplaceParameter(keySelector.Parameters[0], param));
            var valueBinding = Expression.Bind(
                typeof(KeyValue<TKey, TNewValue>).GetProperty(nameof(KeyValue<TKey, TNewValue>.Value)),
                transform.Body.ReplaceParameter(transform.Parameters[0], param));

            var initExpr = Expression.MemberInit(newExpr, keyBinding, valueBinding);

            var result = ExpressionRewriter.Rewrite(Expression.Lambda<Func<TValue, KeyValue<TKey, TNewValue>>>(initExpr, param));
            return result;
        }

        private static async IAsyncEnumerable<IGrouping<TProperty, TTransformedEntity>> GroupBy<TEntity, TTransformedEntity, TProperty>(
            IQueryable<TEntity> query,
            Expression<Func<TEntity, TProperty>> propSelector,
            Expression<Func<TEntity, TTransformedEntity>> transform,
            Func<string> stepNameFactory,
            IQueryExecuter queryExecuter,
            ILoaderHooksExecuter<TTransformedEntity>? hooksExecuter,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>? sorters)
        {
            var orderedQuery = query.OrderBy(propSelector);

            if (sorters != null)
            {
                orderedQuery = orderedQuery.ApplyThenBy(sorters);
            }

            var resultQuery = orderedQuery.Select(CreateExpression(propSelector, transform));
            var orderedItems = queryExecuter.ToAsyncEnumerable(stepNameFactory, resultQuery);

            var comparer = EqualityComparer<TProperty>.Default;
            Grouping<TProperty, TTransformedEntity>? grouping = null;

            await foreach (var item in orderedItems)
            {
                hooksExecuter?.Execute(item.Value);

                if (grouping == null)
                {
                    grouping = new Grouping<TProperty, TTransformedEntity>(item.Key, item.Value);
                }
                else
                {
                    var newKey = item.Key;

                    if (comparer.Equals(grouping.Key, newKey))
                    {
                        grouping.Add(item.Value);
                    }
                    else
                    {
                        yield return grouping;
                        grouping = new Grouping<TProperty, TTransformedEntity>(newKey, item.Value);
                    }
                }
            }

            if (grouping != null)
            {
                yield return grouping;
            }
        }

#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
        internal class KeyValue<TKey, TValue>
        {
            public TKey Key { get; set; }

            public TValue Value { get; set; }
        }
#pragma warning restore CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.

        private class Grouping<TKey, TElement> : IGrouping<TKey, TElement>
        {
            private readonly List<TElement> _elements;

            public Grouping(TKey key, TElement element)
            {
                Key = key;
                _elements = new List<TElement>
                {
                    element,
                };
            }

            public TKey Key { get; }

            public void Add(TElement element)
            {
                _elements.Add(element);
            }

            public IEnumerator<TElement> GetEnumerator()
            {
                return _elements.GetEnumerator();
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
        }
    }
}
