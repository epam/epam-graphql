// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Epam.Contracts.Models;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Tests.TestData;
using GraphQL.Execution;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Helpers
{
    public static class TestHelpers
    {
        public static void TestQuery(
            Action<Query<TestUserContext>> builder,
            string query,
            string expected,
            IDataContext dataContext = null,
            Action checks = null,
            Action beforeExecute = null,
            Action<SchemaOptionsBuilder<TestUserContext>> optionsBuilder = null)
        {
            beforeExecute?.Invoke();

            var expectedResult = ExecuteHelpers.Deserialize(expected);
            var queryType = GraphQLTypeBuilder.CreateQueryType(builder);

            using var loggerFactory = CreateLoggerFactory();
            var actualResult = ExecuteHelpers.ExecuteQuery(
                queryType,
                query,
                dataContext,
                configure: schemaOptionsBuilder =>
                {
                    schemaOptionsBuilder.UseLoggerFactory(loggerFactory);
                    optionsBuilder?.Invoke(schemaOptionsBuilder);
                });

            Assert.IsNull(actualResult.Errors, actualResult.Errors != null ? string.Join(",", actualResult.Errors.Select(e => e.Message)) : null);

            var expectedJson = JsonConvert.SerializeObject(expectedResult);
            var actualJson = JsonConvert.SerializeObject(((ExecutionNode)actualResult.Data).ToValue());
            Assert.AreEqual(expectedJson, actualJson);

            checks?.Invoke();
        }

        public static void TestQuery(
            Action<Query<TestUserContext>> builder,
            string query,
            string expected,
            Action<SchemaExecutionOptionsBuilder<TestUserContext>> executionOptionsBuilder)
        {
            var expectedResult = ExecuteHelpers.Deserialize(expected);
            var queryType = GraphQLTypeBuilder.CreateQueryType(builder);

            using var loggerFactory = CreateLoggerFactory();
            var actualResult = ExecuteHelpers.ExecuteQuery<TestUserContext>(
                queryType,
                configure: schemaOptionsBuilder => schemaOptionsBuilder.UseLoggerFactory(loggerFactory),
                configureExecutionOptions: schemaExecutionOptionsBuilder =>
                {
                    schemaExecutionOptionsBuilder.Query(query);
                    executionOptionsBuilder(schemaExecutionOptionsBuilder);
                });

            Assert.IsNull(actualResult.Errors, actualResult.Errors != null ? string.Join(",", actualResult.Errors.Select(e => e.Message)) : null);

            var expectedJson = JsonConvert.SerializeObject(expectedResult);
            var actualJson = JsonConvert.SerializeObject(((ExecutionNode)actualResult.Data).ToValue());
            Assert.AreEqual(expectedJson, actualJson);
        }

        public static void TestLoader<TEntity>(Action<Loader<TEntity, TestUserContext>> builder, Func<TestUserContext, IQueryable<TEntity>> getBaseQuery, string connectionName, string query, string expected)
            where TEntity : IHasId<int>
        {
            var loaderType = GraphQLTypeBuilder.CreateLoaderType(
                    onConfigure: builder,
                    getBaseQuery: getBaseQuery,
                    applyNaturalOrderBy: q => q.OrderBy(e => e.Id),
                    applyNaturalThenBy: q => q.ThenBy(e => e.Id));

            var expectedResult = ExecuteHelpers.Deserialize(expected);
            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(q => q.Connection(loaderType, connectionName));

            using var loggerFactory = CreateLoggerFactory();
            var actualResult = ExecuteHelpers.ExecuteQuery(
                queryType,
                query,
                configure: schemaOptionsBuilder => schemaOptionsBuilder.UseLoggerFactory(loggerFactory));

            Assert.IsNull(actualResult.Errors, actualResult.Errors != null ? string.Join(",", actualResult.Errors.Select(e => e.Message)) : null);

            var expectedJson = JsonConvert.SerializeObject(expectedResult);
            var actualJson = JsonConvert.SerializeObject(((ExecutionNode)actualResult.Data).ToValue());
            Assert.AreEqual(expectedJson, actualJson);
        }

        public static void TestMutableLoader<TEntity>(
            Action<MutableLoader<TEntity, int, TestUserContext>> builder,
            Func<TestUserContext, IQueryable<TEntity>> getBaseQuery,
            string connectionName,
            string query,
            string expected)
            where TEntity : class, IHasId<int>
        {
            var loaderType = GraphQLTypeBuilder.CreateMutableLoaderType(
                    onConfigure: builder,
                    getBaseQuery: getBaseQuery);

            var expectedResult = ExecuteHelpers.Deserialize(expected);
            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(q => q.Connection(loaderType, connectionName));
            var mutationType = GraphQLTypeBuilder.CreateMutationType<TestUserContext>(m => m.SubmitField(loaderType, "test"));

            using var loggerFactory = CreateLoggerFactory();
            var actualResult = ExecuteHelpers.ExecuteQuery(
                queryType,
                mutationType,
                query,
                configure: schemaOptionsBuilder => schemaOptionsBuilder.UseLoggerFactory(loggerFactory));

            Assert.IsNull(actualResult.Errors, actualResult.Errors != null ? string.Join(",", actualResult.Errors.Select(e => e.Message)) : null);

            var expectedJson = JsonConvert.SerializeObject(expectedResult);
            var actualJson = JsonConvert.SerializeObject(((ExecutionNode)actualResult.Data).ToValue());
            Assert.AreEqual(expectedJson, actualJson);
        }

        public static void TestInlineBuilder<TEntity>(
            Action<IInlineObjectBuilder<TEntity, TestUserContext>> builder,
            Func<TestUserContext, IQueryable<TEntity>> getBaseQuery,
            string fieldName,
            string query,
            string expected)
            where TEntity : IHasId<int>
        {
            var expectedResult = ExecuteHelpers.Deserialize(expected);
            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(q => q.Field(fieldName).Resolve(ctx => getBaseQuery(ctx), builder));

            using var loggerFactory = CreateLoggerFactory();
            var actualResult = ExecuteHelpers.ExecuteQuery(
                queryType,
                query,
                configure: schemaOptionsBuilder => schemaOptionsBuilder.UseLoggerFactory(loggerFactory));

            Assert.IsNull(actualResult.Errors, actualResult.Errors != null ? string.Join(",", actualResult.Errors.Select(e => e.Message)) : null);

            var expectedJson = JsonConvert.SerializeObject(expectedResult);
            var actualJson = JsonConvert.SerializeObject(((ExecutionNode)actualResult.Data).ToValue());
            Assert.AreEqual(expectedJson, actualJson);
        }

        public static void TestQueryError(Action<Query<TestUserContext>> builder, Type exceptionType, string message, string query, Action<SchemaOptionsBuilder<TestUserContext>> optionsBuilder = null)
        {
            Assert.Throws(Is.TypeOf(exceptionType).And.Message.EqualTo(message), () =>
                {
                    var queryType = GraphQLTypeBuilder.CreateQueryType(builder);

                    using var loggerFactory = CreateLoggerFactory();
                    var result = ExecuteHelpers.ExecuteQuery(
                        queryType,
                        query,
                        configure: schemaOptionsBuilder =>
                        {
                            schemaOptionsBuilder.UseLoggerFactory(loggerFactory);
                            optionsBuilder?.Invoke(schemaOptionsBuilder);
                        });
                    if (result.Errors != null && result.Errors.Any())
                    {
                        throw result.Errors.First();
                    }
                });
        }

        public static void TestMutation(Action<Query<TestUserContext>> queryBuilder, Action<Mutation<TestUserContext>> mutationBuilder, IDataContext dataContext, string query, string expected, Action<IDataContext> checks = null, Action beforeExecute = null, Func<TestUserContext, IEnumerable<object>, Task<IEnumerable<object>>> afterSave = null)
        {
            beforeExecute?.Invoke();
            var expectedResult = ExecuteHelpers.Deserialize(expected);
            var queryType = GraphQLTypeBuilder.CreateQueryType(queryBuilder);
            var mutationType = GraphQLTypeBuilder.CreateMutationType(mutationBuilder, afterSave);

            using var loggerFactory = CreateLoggerFactory();
            var actualResult = ExecuteHelpers.ExecuteQuery(
                queryType,
                mutationType,
                query,
                dataContext,
                configure: schemaOptionsBuilder => schemaOptionsBuilder.UseLoggerFactory(loggerFactory));

            Assert.IsNull(actualResult.Errors, actualResult.Errors != null ? string.Join(",", actualResult.Errors.Select(e => e.Message)) : null);

            var converter = new DecimalFormatConverter();
            var expectedJson = JsonConvert.SerializeObject(expectedResult, converter);
            var actualJson = JsonConvert.SerializeObject(((ExecutionNode)actualResult.Data).ToValue(), converter);
            Assert.AreEqual(expectedJson, actualJson);

            checks?.Invoke(dataContext);
        }

        public static string ConcatLines(params string[] lines)
        {
            Guards.ThrowIfNull(lines, nameof(lines));

            var sb = new StringBuilder();

            for (int i = 0; i < lines.Length; i++)
            {
                if (i > 0)
                {
                    sb.AppendLine();
                }

                sb.Append(lines[i]);
            }

            return sb.ToString();
        }

        private static ILoggerFactory CreateLoggerFactory() => LoggerFactory.Create(
            builder => builder
                .ClearProviders()
                    .AddConsole()
                    .AddFilter((category, logLevel) => logLevel == LogLevel.Trace));
    }
}
