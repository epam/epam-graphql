// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Builders.Loader.Implementations;
using Epam.GraphQL.Configuration.Enums;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using GraphQL;
using GraphQL.DataLoader;

namespace Epam.GraphQL.Loaders
{
    public abstract class Loader<TEntity, TExecutionContext> : Projection<TEntity, TExecutionContext>, ILoader<TEntity, TExecutionContext>
        where TEntity : class
    {
        protected internal NullOption NullValues => NullOption.NullValues;

        protected internal NullOption NotNullValues => NullOption.NotNullValues;

        public IQueryable<TEntity> All(TExecutionContext context)
        {
            var query = DoGetBaseQuery(context);
            query = DoApplySecurityFilter(context, query);
            return query;
        }

        public virtual IOrderedQueryable<TEntity> ApplyNaturalOrderBy(IQueryable<TEntity> query)
        {
            throw new NotImplementedException($"You must override {nameof(ApplyNaturalOrderBy)} method or pass order expression to Connection()/AsConnection() call.");
        }

        public virtual IOrderedQueryable<TEntity> ApplyNaturalThenBy(IOrderedQueryable<TEntity> query)
        {
            throw new NotImplementedException($"You must override {nameof(ApplyNaturalThenBy)} method or pass order expression to Connection()/AsConnection() call.");
        }

        internal IQueryable<TEntity> DoGetBaseQuery(TExecutionContext context) => GetBaseQuery(context);

        internal IQueryable<TEntity> DoApplySecurityFilter(TExecutionContext context, IQueryable<TEntity> query)
        {
            var result = ApplySecurityFilter(context, query);
            if (!QueryableChecker.IsSubExpression(result, query))
            {
                throw new ExecutionError($"{GetType().HumanizedName()}.{nameof(ApplySecurityFilter)}:  You cannot query data from anywhere (e.g. using context), please use passed `{nameof(query)}` parameter.");
            }

            return result;
        }

        internal IDataLoaderResult<bool> CanViewAsync(GraphQLContext<TExecutionContext> context, TEntity entity)
        {
            if (!DoApplySecurityFilter(context.ExecutionContext, Enumerable.Repeat(entity, 1).AsQueryable()).Contains(entity))
            {
                return new DataLoaderResult<bool>(false);
            }

            return new DataLoaderResult<bool>(Registry.CanViewParentAsync(GetType(), context, entity));
        }

        protected internal new ILoaderFieldBuilder<TEntity, TExecutionContext> Field(string name, string? deprecationReason = null)
        {
            var fieldType = AddField(name, deprecationReason);
            var fieldBuilderType = typeof(LoaderFieldBuilder<,,>).MakeGenericType(typeof(TEntity), GetType(), typeof(TExecutionContext));
            return (ILoaderFieldBuilder<TEntity, TExecutionContext>)fieldBuilderType.CreateInstanceAndHoistBaseException(Registry, fieldType);
        }

        /// <summary>
        /// Registers a delegate that will be executed for each entity, queried by loader and ready to be sent to a GraphQL client.
        /// </summary>
        /// <typeparam name="T">The type of the second parameter of the <paramref name="action"/>.</typeparam>
        /// <param name="expression">The expression to calculate a value for passing to the <paramref name="action"/> as a first argument.</param>
        /// <param name="action">
        /// The delegate to be executed for each entity, queried by loader and ready to be sent to a GraphQL client.
        /// The first argument of the callback is an execution context, globally passed to a GraphQL query.
        /// The second argument of the callback is a value, calculated using <paramref name="expression"/> for queried entity.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="expression"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
        protected internal void OnEntityLoaded<T>(
            Expression<Func<TEntity, T>> expression,
            Action<TExecutionContext, T> action)
        {
            Guards.ThrowIfNull(expression, nameof(expression));
            Guards.ThrowIfNull(action, nameof(action));

            AddOnEntityLoaded(expression, action);
        }

        /// <summary>
        /// Registers a delegate that will be executed for each entity, queried by loader and ready to be sent to a GraphQL client.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys that depend on queried entities.</typeparam>
        /// <typeparam name="T">The type of the second parameter of the <paramref name="action"/>.</typeparam>
        /// <param name="keyExpression">The expression to be used for calculating keys for passing to the <paramref name="fetch"/>.</param>
        /// <param name="fetch">The delegate to be executed for calculating values, which will pass to the <paramref name="action"/>.</param>
        /// <param name="action">
        /// The delegate to be executed for each entity, queried by loader and ready to be sent to a GraphQL client.
        /// The first argument of the callback is an execution context, globally passed to a GraphQL query.
        /// The second argument of the callback is a value, fetched using <paramref name="fetch"/> for queried entity.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="keyExpression"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="fetch"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
        protected internal void OnEntityLoaded<TKey, T>(
            Expression<Func<TEntity, TKey>> keyExpression,
            Func<TExecutionContext, IEnumerable<TKey>, IDictionary<TKey, T>> fetch,
            Action<TExecutionContext, T> action)
        {
            Guards.ThrowIfNull(keyExpression, nameof(keyExpression));
            Guards.ThrowIfNull(fetch, nameof(fetch));
            Guards.ThrowIfNull(action, nameof(action));

            AddOnEntityLoaded(keyExpression, fetch, action);
        }

        /// <summary>
        /// Registers a delegate that will be executed for each entity, queried by loader and ready to be sent to a GraphQL client.
        /// </summary>
        /// <typeparam name="TKey">The type of the keys that depend on queried entities.</typeparam>
        /// <typeparam name="T">The type of the second parameter of the <paramref name="action"/>.</typeparam>
        /// <param name="keyExpression">The expression to be used for calculating keys for passing to the <paramref name="fetch"/>.</param>
        /// <param name="fetch">The asynchronous delegate to be executed for calculating values, which will pass to the <paramref name="action"/>.</param>
        /// <param name="action">
        /// The delegate to be executed for each entity, queried by loader and ready to be sent to a GraphQL client.
        /// The first argument of the callback is an execution context, globally passed to a GraphQL query.
        /// The second argument of the callback is a value, fetched using <paramref name="fetch"/> for queried entity.
        /// </param>
        /// <exception cref="ArgumentNullException"><paramref name="keyExpression"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="fetch"/> is <see langword="null"/>.</exception>
        /// <exception cref="ArgumentNullException"><paramref name="action"/> is <see langword="null"/>.</exception>
        protected internal void OnEntityLoaded<TKey, T>(
            Expression<Func<TEntity, TKey>> keyExpression,
            Func<TExecutionContext, IEnumerable<TKey>, Task<IDictionary<TKey, T>>> fetch,
            Action<TExecutionContext, T> action)
        {
            Guards.ThrowIfNull(keyExpression, nameof(keyExpression));
            Guards.ThrowIfNull(fetch, nameof(fetch));
            Guards.ThrowIfNull(action, nameof(action));

            AddOnEntityLoaded(keyExpression, fetch, action);
        }

        protected abstract IQueryable<TEntity> GetBaseQuery(TExecutionContext context);

        protected virtual IQueryable<TEntity> ApplySecurityFilter(TExecutionContext context, IQueryable<TEntity> query) => query;
    }
}
