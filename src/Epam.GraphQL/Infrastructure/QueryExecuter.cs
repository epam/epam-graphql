// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Epam.GraphQL.Adapters;
using Epam.GraphQL.Diagnostics;
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

        public IEnumerable<TEntity> ToEnumerable<TEntity>(IConfigurationContext configurationContext, Func<string> stepNameFactory, IQueryable<TEntity> query)
        {
            try
            {
                query = QueryableToAsNoTrackingQueryable(query);

                if (_logger.IsEnabled(Constants.Logging.BeforeQuery.Level))
                {
                    _logger.Log(Constants.Logging.BeforeQuery.Level, Constants.Logging.BeforeQuery.EventId, "{LinqOperation}:\r\n{QueryExpression}", "AsEnumerable", ExpressionPrinter.Print(query.Expression));
                }

                return new EnumerableWrapper<TEntity>(configurationContext, query, stepNameFactory);
            }
            catch (Exception e)
            {
                throw new ExecutionError(configurationContext.GetRuntimeError($"Error during resolving field `{stepNameFactory()}`. See an inner exception for details.", configurationContext), e);
            }
        }

        public List<TEntity> ToList<TEntity>(IConfigurationContext configurationContext, Func<string> stepNameFactory, IQueryable<TEntity> query)
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
                throw new ExecutionError(configurationContext.GetRuntimeError($"Error during resolving field `{stepNameFactory()}`. See an inner exception for details.", configurationContext), e);
            }
        }

        public IAsyncEnumerable<TEntity> ToAsyncEnumerable<TEntity>(IConfigurationContext configurationContext, Func<string> stepNameFactory, IQueryable<TEntity> query)
        {
            try
            {
                query = QueryableToAsNoTrackingQueryable(query);

                if (_logger.IsEnabled(Constants.Logging.BeforeQuery.Level))
                {
                    _logger.Log(Constants.Logging.BeforeQuery.Level, Constants.Logging.BeforeQuery.EventId, "{StepName}\r\n{LinqOperation}:\r\n{QueryExpression}", stepNameFactory(), "Async Query", ExpressionPrinter.Print(query.Expression));
                }

                return new AsyncEnumerableWrapper<TEntity>(configurationContext, QueryableToAsyncEnumerable(query), stepNameFactory);
            }
            catch (Exception e)
            {
                throw new ExecutionError(configurationContext.GetRuntimeError($"Error during resolving field `{stepNameFactory()}`. See an inner exception for details.", configurationContext), e);
            }
        }

        public TReturnType Execute<TEntity, TReturnType>(
            IConfigurationContext configurationContext,
            Func<string> stepNameFactory,
            IQueryable<TEntity> query,
            Func<IQueryable<TEntity>, TReturnType> transform,
            string transformName)
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
                throw new ExecutionError(configurationContext.GetRuntimeError($"Error during resolving field `{stepNameFactory()}`. See an inner exception for details.", configurationContext), e);
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
            return _queryableToAsyncEnumerableConverter != null && _queryableToAsyncEnumerableConverter.CanConvert(query)
                ? _queryableToAsyncEnumerableConverter.Convert(query)
                : QueryableToAsyncEnumerableImpl(query);
        }

        private class EnumerableWrapper<T> : IEnumerable<T>
        {
            private readonly IEnumerable<T> _enumerable;
            private readonly Func<string> _stepNameFactory;
            private readonly IConfigurationContext _configurationContext;

            public EnumerableWrapper(IConfigurationContext configurationContext, IEnumerable<T> enumerable, Func<string> stepNameFactory)
            {
                _enumerable = enumerable;
                _stepNameFactory = stepNameFactory;
                _configurationContext = configurationContext;
            }

            public IEnumerator<T> GetEnumerator()
            {
                try
                {
                    return new EnumeratorWrapper<T>(_configurationContext, _enumerable.GetEnumerator(), _stepNameFactory);
                }
                catch (Exception e)
                {
                    throw new ExecutionError(_configurationContext.GetRuntimeError($"Error during resolving field `{_stepNameFactory()}`. See an inner exception for details.", _configurationContext), e);
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }

        private class EnumeratorWrapper<T> : IEnumerator<T>
        {
            private readonly IEnumerator<T> _enumerator;
            private readonly Func<string> _stepNameFactory;
            private readonly IConfigurationContext _configurationContext;

            public EnumeratorWrapper(IConfigurationContext configurationContext, IEnumerator<T> enumerator, Func<string> stepNameFactory)
            {
                _enumerator = enumerator;
                _stepNameFactory = stepNameFactory;
                _configurationContext = configurationContext;
            }

            public T Current
            {
                get
                {
                    try
                    {
                        return _enumerator.Current;
                    }
                    catch (Exception e)
                    {
                        throw new ExecutionError(_configurationContext.GetRuntimeError($"Error during resolving field `{_stepNameFactory()}`. See an inner exception for details.", _configurationContext), e);
                    }
                }
            }

            object? IEnumerator.Current => Current;

            public void Dispose()
            {
                try
                {
                    _enumerator.Dispose();
                }
                catch (Exception e)
                {
                    throw new ExecutionError(_configurationContext.GetRuntimeError($"Error during resolving field `{_stepNameFactory()}`. See an inner exception for details.", _configurationContext), e);
                }
            }

            public bool MoveNext()
            {
                try
                {
                    return _enumerator.MoveNext();
                }
                catch (Exception e)
                {
                    throw new ExecutionError(_configurationContext.GetRuntimeError($"Error during resolving field `{_stepNameFactory()}`. See an inner exception for details.", _configurationContext), e);
                }
            }

            public void Reset()
            {
                try
                {
                    _enumerator.Reset();
                }
                catch (Exception e)
                {
                    throw new ExecutionError(_configurationContext.GetRuntimeError($"Error during resolving field `{_stepNameFactory()}`. See an inner exception for details.", _configurationContext), e);
                }
            }
        }

        private class AsyncEnumerableWrapper<T> : IAsyncEnumerable<T>
        {
            private readonly IAsyncEnumerable<T> _enumerable;
            private readonly Func<string> _stepNameFactory;
            private readonly IConfigurationContext _configurationContext;

            public AsyncEnumerableWrapper(IConfigurationContext configurationContext, IAsyncEnumerable<T> enumerable, Func<string> stepNameFactory)
            {
                _enumerable = enumerable;
                _stepNameFactory = stepNameFactory;
                _configurationContext = configurationContext;
            }

            public IAsyncEnumerator<T> GetAsyncEnumerator(CancellationToken cancellationToken)
            {
                try
                {
                    return new AsyncEnumeratorWrapper<T>(_configurationContext, _enumerable.GetAsyncEnumerator(cancellationToken), _stepNameFactory);
                }
                catch (Exception e)
                {
                    throw new ExecutionError(_configurationContext.GetRuntimeError($"Error during resolving field `{_stepNameFactory()}`. See an inner exception for details.", _configurationContext), e);
                }
            }
        }

        private class AsyncEnumeratorWrapper<T> : IAsyncEnumerator<T>
        {
            private readonly IAsyncEnumerator<T> _enumerator;
            private readonly Func<string> _stepNameFactory;
            private readonly IConfigurationContext _configurationContext;

            public AsyncEnumeratorWrapper(IConfigurationContext configurationContext, IAsyncEnumerator<T> enumerator, Func<string> stepNameFactory)
            {
                _enumerator = enumerator;
                _stepNameFactory = stepNameFactory;
                _configurationContext = configurationContext;
            }

            public T Current
            {
                get
                {
                    try
                    {
                        return _enumerator.Current;
                    }
                    catch (Exception e)
                    {
                        throw new ExecutionError(_configurationContext.GetRuntimeError($"Error during resolving field `{_stepNameFactory()}`. See an inner exception for details.", _configurationContext), e);
                    }
                }
            }

            public async ValueTask DisposeAsync()
            {
                try
                {
                    await _enumerator.DisposeAsync().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    throw new ExecutionError(_configurationContext.GetRuntimeError($"Error during resolving field `{_stepNameFactory()}`. See an inner exception for details.", _configurationContext), e);
                }
            }

            public async ValueTask<bool> MoveNextAsync()
            {
                try
                {
                    return await _enumerator.MoveNextAsync().ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    throw new ExecutionError(_configurationContext.GetRuntimeError($"Error during resolving field `{_stepNameFactory()}`. See an inner exception for details.", _configurationContext), e);
                }
            }
        }
    }
}
