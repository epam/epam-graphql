// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using Epam.GraphQL.Adapters;
using Epam.GraphQL.Helpers;
using GraphQL;
using Microsoft.Extensions.Logging;

namespace Epam.GraphQL.Infrastructure
{
    internal class QueryExecuter : IQueryExecuter
    {
        private readonly ILogger _logger;
        private readonly IQueryableToAsyncEnumerableConverter? _queryableToAsyncEnumerableConverter;

        public QueryExecuter(
            ILogger logger,
            IQueryableToAsyncEnumerableConverter? queryableToAsyncEnumerableConverter,
            IQueryableToAsNoTrackingQueryableConverter? queryableToAsNoTrackingQueryableConverter)
        {
            _logger = logger;
            _queryableToAsyncEnumerableConverter = queryableToAsyncEnumerableConverter;
            QueryableToAsNoTrackingQueryableConverter = queryableToAsNoTrackingQueryableConverter;
        }

        public IQueryableToAsNoTrackingQueryableConverter? QueryableToAsNoTrackingQueryableConverter { get; set; }

        public IEnumerable<TEntity> ToEnumerable<TEntity>(Func<string> stepNameFactory, IQueryable<TEntity> query)
        {
            try
            {
                query = QueryableToAsNoTrackingQueryable(query);

                if (_logger.IsEnabled(Constants.Logging.BeforeQuery.Level))
                {
                    _logger.Log(Constants.Logging.BeforeQuery.Level, Constants.Logging.BeforeQuery.EventId, "{LinqOperation}:\r\n{QueryExpression}", "AsEnumerable", ExpressionPrinter.Print(query.Expression));
                }

                return query.AsEnumerable();
            }
            catch (Exception e)
            {
                throw new ExecutionError($"Error during resolving node `{stepNameFactory()}`. See inner exception for details.", e);
            }
        }

        public List<TEntity> ToList<TEntity>(Func<string> stepNameFactory, IQueryable<TEntity> query)
        {
            try
            {
                query = QueryableToAsNoTrackingQueryable(query);

                if (_logger.IsEnabled(Constants.Logging.BeforeQuery.Level))
                {
                    _logger.Log(Constants.Logging.BeforeQuery.Level, Constants.Logging.BeforeQuery.EventId, "{LinqOperation}:\r\n{QueryExpression}", "ToList", ExpressionPrinter.Print(query.Expression));
                }

                return query.ToList();
            }
            catch (Exception e)
            {
                throw new ExecutionError($"Error during resolving node `{stepNameFactory()}`. See inner exception for details.", e);
            }
        }

        public IAsyncEnumerable<TEntity> ToAsyncEnumerable<TEntity>(Func<string> stepNameFactory, IQueryable<TEntity> query)
        {
            try
            {
                query = QueryableToAsNoTrackingQueryable(query);

                if (_logger.IsEnabled(Constants.Logging.BeforeQuery.Level))
                {
                    _logger.Log(Constants.Logging.BeforeQuery.Level, Constants.Logging.BeforeQuery.EventId, "{StepName}\r\n{LinqOperation}:\r\n{QueryExpression}", stepNameFactory(), "Async Query", ExpressionPrinter.Print(query.Expression));
                }

                return QueryableToAsyncEnumerable(query);
            }
            catch (Exception e)
            {
                throw new ExecutionError($"Error during resolving node `{stepNameFactory()}`. See inner exception for details.", e);
            }
        }

        public TReturnType Execute<TEntity, TReturnType>(Func<string> stepNameFactory, IQueryable<TEntity> query, Func<IQueryable<TEntity>, TReturnType> transform, string transformName)
        {
            try
            {
                query = QueryableToAsNoTrackingQueryable(query);

                if (_logger.IsEnabled(Constants.Logging.BeforeQuery.Level))
                {
                    _logger.Log(Constants.Logging.BeforeQuery.Level, Constants.Logging.BeforeQuery.EventId, "{StepName}\r\n{LinqOperation}:\r\n{QueryExpression}", stepNameFactory(), transformName, ExpressionPrinter.Print(query.Expression));
                }

                var result = transform(query);
                return result;
            }
            catch (Exception e)
            {
                throw new ExecutionError($"Error during resolving node `{stepNameFactory()}`. See inner exception for details.", e);
            }
        }

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously
        private static async IAsyncEnumerable<TEntity> QueryableToAsyncEnumerableImpl<TEntity>(IQueryable<TEntity> query)
#pragma warning restore CS1998 // Async method lacks 'await' operators and will run synchronously
        {
            foreach (var entity in query)
            {
                yield return entity;
            }
        }

        private IQueryable<TEntity> QueryableToAsNoTrackingQueryable<TEntity>(IQueryable<TEntity> query)
        {
            return QueryableToAsNoTrackingQueryableConverter == null
                ? query
                : QueryableToAsNoTrackingQueryableConverter.QueryableToAsNoTrackingQueryable(query);
        }

        private IAsyncEnumerable<TEntity> QueryableToAsyncEnumerable<TEntity>(IQueryable<TEntity> query)
        {
            return _queryableToAsyncEnumerableConverter != null
                ? _queryableToAsyncEnumerableConverter.QueryableToAsyncEnumerable(query)
                : QueryableToAsyncEnumerableImpl(query);
        }
    }
}
