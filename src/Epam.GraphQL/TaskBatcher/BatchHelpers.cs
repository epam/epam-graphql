// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Infrastructure;
using Epam.GraphQL.Loaders;
using GraphQL.DataLoader;

#nullable enable

namespace Epam.GraphQL.TaskBatcher
{
    internal static class BatchHelpers
    {
        private static readonly ConcurrentDictionary<(Type OuterQueryType, Type InnerQueryType, LambdaExpression ParentExpression, LambdaExpression ChildExpression, Delegate Transform), Delegate> _queryWithPropConditionFuncs = new(
            new ValueTupleEqualityComparer<Type, Type, LambdaExpression, LambdaExpression, Delegate>(null, null, ExpressionEqualityComparer.Instance, ExpressionEqualityComparer.Instance));

        private static readonly ConcurrentDictionary<object, Delegate> _loaderQueriesCache = new();

        private static readonly ConcurrentDictionary<(Func<string> StepNameFactory, LambdaExpression KeySelector, Delegate? DefaultFactory, Delegate Transform), Delegate> _groupByValuesCache = new(
            new ValueTupleEqualityComparer<Func<string>, LambdaExpression, Delegate?, Delegate>(secondItemComparer: ExpressionEqualityComparer.Instance));

        public static Func<IProfiler, IQueryExecuter, ILoaderHooksExecuter<TEntity>, TExecutionContext, IDataLoader<TProperty, IGrouping<TProperty, TEntity>?>> GetLoaderQueryFactory<TLoader, TEntity, TProperty, TExecutionContext>(
            Func<string> stepNameFactory,
            TLoader loader,
            Expression<Func<TEntity, TProperty>> propSelector)
            where TLoader : Loader<TEntity, TExecutionContext>
            where TEntity : class
        {
            return (Func<IProfiler, IQueryExecuter, ILoaderHooksExecuter<TEntity>, TExecutionContext, IDataLoader<TProperty, IGrouping<TProperty, TEntity>?>>)_loaderQueriesCache.GetOrAdd(loader, key =>
            {
                var loader = (TLoader)key;
                var factory = Get<TEntity, TEntity, TProperty, TExecutionContext>(stepNameFactory, propSelector, id => new EmptyGrouping<TProperty, TEntity>(id), ctx => FuncConstants<TEntity>.IdentityExpression);
                Func<IProfiler, IQueryExecuter, ILoaderHooksExecuter<TEntity>, TExecutionContext, IDataLoader<TProperty, IGrouping<TProperty, TEntity>?>> result = (profiler, queryExecuter, hooksExecuter, context) => factory(context, profiler, queryExecuter, hooksExecuter, loader.All(context), null);
                return result;
            });
        }

        public static Func<TExecutionContext, IProfiler, IQueryExecuter, ILoaderHooksExecuter<TTransformedChildEntity>?, IQueryable<TChildEntity>, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>?, IDataLoader<TEntity, IGrouping<TEntity, TTransformedChildEntity>>> GetQueryJoinFactory<TEntity, TChildEntity, TTransformedChildEntity, TExecutionContext>(
            Func<string> stepNameFactory,
            Func<TExecutionContext, Expression<Func<TChildEntity, TTransformedChildEntity>>> transform,
            LambdaExpression parentPropExpression,
            LambdaExpression childPropExpression)
        {
            Func<TExecutionContext, IProfiler, IQueryExecuter, ILoaderHooksExecuter<TTransformedChildEntity>?, IQueryable<TChildEntity>, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>?, IDataLoader<TEntity, IGrouping<TEntity, TTransformedChildEntity>>> Factory((Type OuterQueryType, Type InnerQueryType, LambdaExpression ParentExpression, LambdaExpression ChildExpression, Delegate Transform) key)
            {
                childPropExpression = Expression.Lambda(childPropExpression.Body.RemoveConvert(), childPropExpression.Parameters);
                parentPropExpression = Expression.Lambda(parentPropExpression.Body.RemoveConvert(), parentPropExpression.Parameters);

                var childPropType = childPropExpression.ReturnType;
                var parentPropType = parentPropExpression.ReturnType;

                var funcType = typeof(Func<,>).MakeGenericType(typeof(TChildEntity), childPropType);
                var childPropertyExpressionType = typeof(Expression<>).MakeGenericType(funcType);

                var expressionType = typeof(Expression<>).MakeGenericType(funcType);

                Type groupingType;
                Func<TExecutionContext, IProfiler, IQueryExecuter, ILoaderHooksExecuter<TTransformedChildEntity>?, IQueryable<TChildEntity>, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>?, IDataLoader<TEntity, IEnumerable<TTransformedChildEntity>?>> get;

                var leftPropertyGetter = ExpressionHelpers.MakeWeakLambdaExpression(parentPropExpression).Compile();

                var isParentPropTypeNullable = parentPropType.IsNullable();
                var unwrappedParentPropType = parentPropType.UnwrapIfNullable();

                var convert = FuncConstants<object?>.Identity;

                if (parentPropType.IsNullable())
                {
                    convert = value => value == null ? null : Convert.ChangeType(value, unwrappedParentPropType, CultureInfo.InvariantCulture);
                }

                if (parentPropType.IsValueType && childPropType.IsNullable() && childPropType.UnwrapIfNullable() == parentPropType)
                {
                    groupingType = typeof(IGrouping<,>).MakeGenericType(parentPropType, typeof(TChildEntity));
                    get = MakeGetNullable<TEntity, TChildEntity, TTransformedChildEntity, TExecutionContext>(
                        childPropType.UnwrapIfNullable(),
                        source => convert(leftPropertyGetter(source)),
                        transform,
                        childPropExpression,
                        stepNameFactory);
                }
                else
                {
                    groupingType = typeof(IGrouping<,>).MakeGenericType(childPropType, typeof(TChildEntity));
                    get = MakeGet<TEntity, TChildEntity, TTransformedChildEntity, TExecutionContext>(
                        childPropType,
                        source => convert(leftPropertyGetter(source)),
                        transform,
                        childPropExpression,
                        stepNameFactory);
                }

                return (context, batcher, queryExecuter, hooksExecuter, query, sortings) => get(
                    context,
                    batcher,
                    queryExecuter,
                    hooksExecuter,
                    query,
                    sortings).Then<TEntity, IEnumerable<TTransformedChildEntity>?, IGrouping<TEntity, TTransformedChildEntity>>(
                        (source, grouping) => new Grouping<TEntity, TTransformedChildEntity>(source, grouping ?? Enumerable.Empty<TTransformedChildEntity>()));
            }

            var taskFactory = (Func<TExecutionContext, IProfiler, IQueryExecuter, ILoaderHooksExecuter<TTransformedChildEntity>?, IQueryable<TChildEntity>, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>?, IDataLoader<TEntity, IGrouping<TEntity, TTransformedChildEntity>>>)_queryWithPropConditionFuncs.GetOrAdd((typeof(TEntity), typeof(TChildEntity), parentPropExpression, childPropExpression, transform), Factory);
            return taskFactory;
        }

        private static Func<TExecutionContext, IProfiler, IQueryExecuter, ILoaderHooksExecuter<TTransformedEntity>?, IQueryable<TEntity>, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>?, IDataLoader<TProperty, IGrouping<TProperty, TTransformedEntity>?>> Get<TEntity, TTransformedEntity, TProperty, TExecutionContext>(
            Func<string> stepNameFactory,
            Expression<Func<TEntity, TProperty>> propSelector,
            Func<TProperty, IGrouping<TProperty, TTransformedEntity>>? defaultFactory,
            Func<TExecutionContext, Expression<Func<TEntity, TTransformedEntity>>> transform)
        {
            if (propSelector == null)
            {
                throw new ArgumentNullException(nameof(propSelector));
            }

            Func<TExecutionContext, IProfiler, IQueryExecuter, ILoaderHooksExecuter<TTransformedEntity>?, IQueryable<TEntity>, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>?, IDataLoader<TProperty, IGrouping<TProperty, TTransformedEntity>?>> Factory((Func<string> StepNameFactory, LambdaExpression KeySelector, Delegate? DefaultFactory, Delegate Transform) key)
            {
                var transform = (Func<TExecutionContext, Expression<Func<TEntity, TTransformedEntity>>>)key.Transform;
                var propSelector = (Expression<Func<TEntity, TProperty>>)key.KeySelector;
                var defaultFactory = (Func<TProperty, IGrouping<TProperty, TTransformedEntity>>?)key.DefaultFactory;
                var stepNameFactory = key.StepNameFactory;
                return (context, profiler, queryExecuter, hooksExecuter, query, sortings) => Get(
                    stepNameFactory,
                    ids => query.GroupByValues(ids, propSelector, transform(context), stepNameFactory, queryExecuter, hooksExecuter, sortings),
                    g => g.Key,
                    defaultFactory)(profiler);
            }

            return (Func<TExecutionContext, IProfiler, IQueryExecuter, ILoaderHooksExecuter<TTransformedEntity>?, IQueryable<TEntity>, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>?, IDataLoader<TProperty, IGrouping<TProperty, TTransformedEntity>?>>)_groupByValuesCache.GetOrAdd((stepNameFactory, propSelector, defaultFactory, transform), Factory);
        }

        private static Func<TExecutionContext, IProfiler, IQueryExecuter, ILoaderHooksExecuter<TTransformedEntity>?, IQueryable<TEntity>, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>?, IDataLoader<TProperty, IGrouping<TProperty, TTransformedEntity>?>> GetNullable<TEntity, TTransformedEntity, TProperty, TExecutionContext>(
            Func<string> stepNameFactory,
            Expression<Func<TEntity, TProperty?>> propSelector,
            Func<TProperty, IGrouping<TProperty, TTransformedEntity>>? defaultFactory,
            Func<TExecutionContext, Expression<Func<TEntity, TTransformedEntity>>> transform)
            where TProperty : struct
        {
            if (propSelector == null)
            {
                throw new ArgumentNullException(nameof(propSelector));
            }

            Func<TExecutionContext, IProfiler, IQueryExecuter, ILoaderHooksExecuter<TTransformedEntity>?, IQueryable<TEntity>, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>?, IDataLoader<TProperty, IGrouping<TProperty, TTransformedEntity>?>> Factory((Func<string> StepNameFactory, LambdaExpression KeySelector, Delegate? DefaultFactory, Delegate Transform) key)
            {
                var transform = (Func<TExecutionContext, Expression<Func<TEntity, TTransformedEntity>>>)key.Transform;
                var propSelector = (Expression<Func<TEntity, TProperty?>>)key.KeySelector;
                var defaultFactory = (Func<TProperty, IGrouping<TProperty, TTransformedEntity>>?)key.DefaultFactory;
                var stepNameFactory = key.StepNameFactory;
                return (context, profiler, queryExecuter, hooksExecuter, query, sortings) => Get(
                    stepNameFactory,
                    ids => query.GroupByValues(ids, propSelector, transform(context), stepNameFactory, queryExecuter, hooksExecuter, sortings),
                    g => g.Key,
                    defaultFactory)(profiler);
            }

            return (Func<TExecutionContext, IProfiler, IQueryExecuter, ILoaderHooksExecuter<TTransformedEntity>?, IQueryable<TEntity>, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>?, IDataLoader<TProperty, IGrouping<TProperty, TTransformedEntity>?>>)_groupByValuesCache.GetOrAdd((stepNameFactory, propSelector, defaultFactory, transform), Factory);
        }

        private static Func<TExecutionContext, IProfiler, IQueryExecuter, ILoaderHooksExecuter<TTransformedChildEntity>?, IQueryable<TChildEntity>, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>?, IDataLoader<TEntity, IEnumerable<TTransformedChildEntity>?>> MakeGet<TEntity, TChildEntity, TTransformedChildEntity, TExecutionContext>(
            Type childPropertyType,
            Func<TEntity, object?> propGetter,
            Func<TExecutionContext, Expression<Func<TChildEntity, TTransformedChildEntity>>> transform,
            LambdaExpression propertyExpression,
            Func<string> stepNameFactory)
        {
            if (childPropertyType == typeof(string))
            {
                return MakeGetHelper(propGetter, transform, propertyExpression, stepNameFactory);
            }

            // First fetch the generic form
            var helper = typeof(BatchHelpers).GetGenericMethod(
                nameof(MakeGetHelper),
                new Type[] { typeof(TEntity), typeof(TChildEntity), typeof(TTransformedChildEntity), childPropertyType, typeof(TExecutionContext) },
                new Type[] { typeof(Func<TEntity, object?>), typeof(Func<TExecutionContext, Expression<Func<TChildEntity, TTransformedChildEntity>>>), typeof(LambdaExpression), typeof(Func<string>) },
                BindingFlags.Static | BindingFlags.NonPublic);

            // Now call it. The null argument is because it’s a static method.
            var ret = helper.Invoke(null, new object[] { propGetter, transform, propertyExpression, stepNameFactory });

            // Cast the result to the right kind of delegate and return it
            return (Func<TExecutionContext, IProfiler, IQueryExecuter, ILoaderHooksExecuter<TTransformedChildEntity>?, IQueryable<TChildEntity>, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>?, IDataLoader<TEntity, IEnumerable<TTransformedChildEntity>?>>)ret;
        }

        private static Func<TExecutionContext, IProfiler, IQueryExecuter, ILoaderHooksExecuter<TTransformedChildEntity>?, IQueryable<TChildEntity>, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>?, IDataLoader<TEntity, IEnumerable<TTransformedChildEntity>?>> MakeGetHelper<TEntity, TChildEntity, TTransformedChildEntity, TProperty, TExecutionContext>(
            Func<TEntity, object?> propGetter,
            Func<TExecutionContext, Expression<Func<TChildEntity, TTransformedChildEntity>>> transform,
            LambdaExpression propertyExpression,
            Func<string> stepNameFactory)
            where TProperty : struct
        {
            var param = Expression.Parameter(typeof(TChildEntity));
            var convertedParam = Expression.Convert(param, propertyExpression.Parameters[0].Type);
            var exprBody = propertyExpression.Body.ReplaceParameter(propertyExpression.Parameters[0], convertedParam);
            var expr = Expression.Lambda<Func<TChildEntity, TProperty>>(exprBody, param);
            var task = Get(stepNameFactory, expr, null, transform);
            Func<TEntity, TProperty?> strictPropGetter = e => (TProperty?)propGetter(e);
            Func<TProperty?, TProperty> propTransform = p => p!.Value;

            IDataLoader<TEntity, IEnumerable<TTransformedChildEntity>?> Ret(TExecutionContext context, IProfiler profiler, IQueryExecuter queryExecuter, ILoaderHooksExecuter<TTransformedChildEntity>? hooksExecuter, IQueryable<TChildEntity> queryable, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>? sortings)
            {
                return strictPropGetter.Then(
                    prop => prop == null,
                    BatchLoader.FromResult(FuncConstants<TProperty?, IGrouping<TProperty, TTransformedChildEntity>?>.DefaultResultFunc),
                    propTransform.Then(task(context, profiler, queryExecuter, hooksExecuter, queryable, sortings))).Then(FuncConstants<IGrouping<TProperty, TTransformedChildEntity>?, IEnumerable<TTransformedChildEntity>?>.Cast);
            }

            return Ret;
        }

        private static Func<TExecutionContext, IProfiler, IQueryExecuter, ILoaderHooksExecuter<TTransformedChildEntity>?, IQueryable<TChildEntity>, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>?, IDataLoader<TEntity, IEnumerable<TTransformedChildEntity>?>> MakeGetHelper<TEntity, TChildEntity, TTransformedChildEntity, TExecutionContext>(
            Func<TEntity, object?> propGetter,
            Func<TExecutionContext, Expression<Func<TChildEntity, TTransformedChildEntity>>> transform,
            LambdaExpression propertyExpression,
            Func<string> stepNameFactory)
        {
            var param = Expression.Parameter(typeof(TChildEntity));
            var convertedParam = Expression.Convert(param, propertyExpression.Parameters[0].Type);
            var exprBody = propertyExpression.Body.ReplaceParameter(propertyExpression.Parameters[0], convertedParam);
            var expr = Expression.Lambda<Func<TChildEntity, string>>(exprBody, param);
            var task = Get(stepNameFactory, expr, null, transform);
            Func<TEntity, string?> strictPropGetter = e => (string?)propGetter(e);
            Func<string?, string> propTransform = p => p!;

            IDataLoader<TEntity, IEnumerable<TTransformedChildEntity>?> Ret(TExecutionContext context, IProfiler profiler, IQueryExecuter queryExecuter, ILoaderHooksExecuter<TTransformedChildEntity>? hooksExecuter, IQueryable<TChildEntity> queryable, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>? sortings)
            {
                return strictPropGetter.Then(
                    prop => prop == null,
                    BatchLoader.FromResult(FuncConstants<string?, IGrouping<string, TTransformedChildEntity>?>.DefaultResultFunc),
                    propTransform.Then(task(context, profiler, queryExecuter, hooksExecuter, queryable, sortings))).Then(FuncConstants<IGrouping<string, TTransformedChildEntity>?, IEnumerable<TTransformedChildEntity>?>.Cast);
            }

            return Ret;
        }

        private static Func<TExecutionContext, IProfiler, IQueryExecuter, ILoaderHooksExecuter<TTransformedChildEntity>?, IQueryable<TChildEntity>, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>?, IDataLoader<TEntity, IEnumerable<TTransformedChildEntity>?>> MakeGetNullable<TEntity, TChildEntity, TTransformedChildEntity, TExecutionContext>(
            Type propertyType,
            Func<TEntity, object?> propGetter,
            Func<TExecutionContext, Expression<Func<TChildEntity, TTransformedChildEntity>>> transform,
            LambdaExpression propertyExpression,
            Func<string> stepNameFactory)
        {
            // First fetch the generic form
            var helper = typeof(BatchHelpers).GetGenericMethod(
                nameof(MakeGetNullableHelper),
                new Type[] { typeof(TEntity), typeof(TChildEntity), typeof(TTransformedChildEntity), propertyType, typeof(TExecutionContext) },
                new Type[] { typeof(Func<TEntity, object?>), typeof(Func<TExecutionContext, Expression<Func<TChildEntity, TTransformedChildEntity>>>), typeof(LambdaExpression), typeof(Func<string>) },
                BindingFlags.Static | BindingFlags.NonPublic);

            // Now call it. The null argument is because it’s a static method.
            var ret = helper.Invoke(null, new object[] { propGetter, transform, propertyExpression, stepNameFactory });

            // Cast the result to the right kind of delegate and return it
            return (Func<TExecutionContext, IProfiler, IQueryExecuter, ILoaderHooksExecuter<TTransformedChildEntity>?, IQueryable<TChildEntity>, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>?, IDataLoader<TEntity, IEnumerable<TTransformedChildEntity>?>>)ret;
        }

        private static Func<TExecutionContext, IProfiler, IQueryExecuter, ILoaderHooksExecuter<TTransformedChildEntity>?, IQueryable<TChildEntity>, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>?, IDataLoader<TEntity, IEnumerable<TTransformedChildEntity>?>> MakeGetNullableHelper<TEntity, TChildEntity, TTransformedChildEntity, TProperty, TExecutionContext>(
            Func<TEntity, object?> propGetter,
            Func<TExecutionContext, Expression<Func<TChildEntity, TTransformedChildEntity>>> transform,
            LambdaExpression propertyExpression,
            Func<string> stepNameFactory)
            where TProperty : struct
        {
            var param = Expression.Parameter(typeof(TChildEntity));
            var convertedParam = Expression.Convert(param, propertyExpression.Parameters[0].Type);
            var exprBody = propertyExpression.Body.ReplaceParameter(propertyExpression.Parameters[0], convertedParam);
            var expr = Expression.Lambda<Func<TChildEntity, TProperty?>>(exprBody, param);
            var task = GetNullable(stepNameFactory, expr, null, transform);
            Func<TEntity, TProperty?> strictPropGetter = e => (TProperty?)propGetter(e);
            Func<TProperty?, TProperty> propTransform = p => p!.Value;

            IDataLoader<TEntity, IEnumerable<TTransformedChildEntity>?> Ret(TExecutionContext context, IProfiler profiler, IQueryExecuter queryExecuter, ILoaderHooksExecuter<TTransformedChildEntity>? hooksExecuter, IQueryable<TChildEntity> queryable, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>? sortings)
            {
                return strictPropGetter.Then(
                    FuncConstants<TProperty?>.IsNull,
                    BatchLoader.FromResult(FuncConstants<TProperty?, IGrouping<TProperty, TTransformedChildEntity>?>.DefaultResultFunc),
                    propTransform.Then(task(context, profiler, queryExecuter, hooksExecuter, queryable, sortings))).Then(FuncConstants<IGrouping<TProperty, TTransformedChildEntity>?, IEnumerable<TTransformedChildEntity>?>.Cast);
            }

            return Ret;
        }

        private static Func<IProfiler, IDataLoader<TId, TItem?>> Get<TId, TItem>(
            Func<string> stepNameFactory,
            Func<IEnumerable<TId>, IAsyncEnumerable<TItem>> loader,
            Func<TItem, TId> keySelector,
            Func<TId, TItem?>? defaultFactory = null)
        {
            async IAsyncEnumerable<KeyValuePair<TId, TItem>> BatchFunc(IEnumerable<TId> ids)
            {
                await foreach (var id in loader(ids))
                {
                    yield return new KeyValuePair<TId, TItem>(keySelector(id), id);
                }
            }

            return profiler => new AsyncBatchLoader<TId, TItem>(BatchFunc, defaultFactory ?? (_ => default), stepNameFactory, profiler);
        }
    }
}
