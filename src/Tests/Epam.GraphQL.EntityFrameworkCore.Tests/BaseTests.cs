// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Data.Common;
using System.Linq;
using Epam.GraphQL.Infrastructure;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NUnit.Framework;

namespace Epam.GraphQL.EntityFrameworkCore.Tests
{
    public class BaseTests : IDisposable
    {
        private readonly DbConnection _connection;

        private ILoggerFactory _loggerFactory;

        public BaseTests()
        {
            _connection = new SqliteConnection("DataSource=:memory:");
        }

        protected TestDbContext Context { get; private set; }

        public void Dispose()
        {
            _connection.Dispose();
            _loggerFactory.Dispose();
        }

        [SetUp]
        public void SetUp()
        {
            _loggerFactory = LoggerFactory.Create(
                builder => builder
                    .ClearProviders()
                    .AddConsole()
                    .AddFilter((category, logLevel) => (category == DbLoggerCategory.Database.Command.Name && logLevel == LogLevel.Information) || category == Constants.Logging.Category));

            var options = new DbContextOptionsBuilder<TestDbContext>()
                .UseSqlite(_connection)
                .Options;

            _connection.Open();
            using var context = new TestDbContext(options);
            context.Database.EnsureCreated();
            Seed(context);

            options = new DbContextOptionsBuilder<TestDbContext>()
                .UseLoggerFactory(_loggerFactory)
                .EnableSensitiveDataLogging()
                .UseSqlite(_connection)
                .Options;

            Context = new TestDbContext(options);
        }

        [TearDown]
        public void TearDown()
        {
            Context.Database.EnsureDeleted();
            _connection.Close();
            _loggerFactory.Dispose();
        }

        protected void TestMutation(Action<Query<TestUserContext>> queryBuilder, Action<Mutation<TestUserContext>> mutationBuilder, string query, string expected)
        {
            var expectedResult = ExecuteHelpers.Deserialize(expected);
            var queryType = GraphQLTypeBuilder.CreateQueryType(queryBuilder);
            var mutationType = GraphQLTypeBuilder.CreateMutationType(mutationBuilder);
            var actualResult = ExecuteHelpers.ExecuteQuery<TestUserContext>(
                queryType,
                mutationType,
                schemaOptionBuilder => schemaOptionBuilder.UseLoggerFactory(_loggerFactory),
                executionBuilder => executionBuilder
                    .ThrowOnUnhandledException(true)
                    .WithExecutionContext(new TestUserContext(Context))
                    .WithDbContext(Context)
                    .Query(query));

            Assert.IsNull(actualResult.Errors, actualResult.Errors != null ? string.Join(",", actualResult.Errors.Select(e => e.Message)) : null);

            var converter = new DecimalFormatConverter();
            var expectedJson = JsonConvert.SerializeObject(expectedResult, converter);
            var actualJson = JsonConvert.SerializeObject(actualResult.Data, converter);
            Assert.AreEqual(expectedJson, actualJson);
        }

        protected void TestQuery(Action<Query<TestUserContext>> queryBuilder, string query, string expected)
        {
            var expectedResult = ExecuteHelpers.Deserialize(expected);
            var queryType = GraphQLTypeBuilder.CreateQueryType(queryBuilder);
            var mutationType = GraphQLTypeBuilder.CreateMutationType<TestUserContext>(_ => { });

            var actualResult = ExecuteHelpers.ExecuteQuery<TestUserContext>(
                queryType,
                mutationType,
                schemaOptionBuilder => schemaOptionBuilder.UseLoggerFactory(_loggerFactory),
                executionBuilder => executionBuilder
                    .ThrowOnUnhandledException(true)
                    .WithExecutionContext(new TestUserContext(Context))
                    .WithDbContext(Context)
                    .Query(query));

            Assert.IsNull(actualResult.Errors, actualResult.Errors != null ? string.Join(",", actualResult.Errors.Select(e => e.Message)) : null);

            var expectedJson = JsonConvert.SerializeObject(expectedResult);
            var actualJson = JsonConvert.SerializeObject(actualResult.Data);
            Assert.AreEqual(expectedJson, actualJson);
        }

        private static void Seed(TestDbContext context)
        {
            var alpha = new Unit { Name = "Alpha" };
            var beta = new Unit { Name = "Beta", Parent = alpha };

            context.Add(alpha);
            context.Add(beta);

            var linoelLivermore = new Person { Id = 1, FullName = "Linoel Livermore", Salary = 4015.69m, Unit = alpha, HireDate = new DateTime(2000, 1, 20) };
            var sophieGandley = new Person { Id = 2, FullName = "Sophie Gandley", Salary = 2381.91m, Manager = linoelLivermore, Unit = alpha, HireDate = new DateTime(2010, 6, 14) };
            var hannieEveritt = new Person { Id = 3, FullName = "Hannie Everitt", Salary = 1393.08m, Manager = linoelLivermore, Unit = alpha, HireDate = new DateTime(2015, 3, 1) };
            var floranceGoodricke = new Person { Id = 4, FullName = "Florance Goodricke", Salary = 549.33m, Manager = sophieGandley, Unit = beta, HireDate = new DateTime(2013, 9, 19), TerminationDate = new DateTime(2019, 10, 1) };
            var aldonExley = new Person { Id = 5, FullName = "Aldon Exley", Salary = 3389.21m, Manager = sophieGandley, Unit = beta, HireDate = new DateTime(2015, 3, 21), TerminationDate = new DateTime(2017, 2, 19) };
            var waltonAlvarez = new Person { Id = 6, FullName = "Walton Alvarez", Salary = 3436.75m, Manager = aldonExley, Unit = beta, HireDate = new DateTime(2011, 7, 29), TerminationDate = new DateTime(2018, 5, 10) };

            context.Add(linoelLivermore);
            context.Add(sophieGandley);
            context.Add(hannieEveritt);
            context.Add(floranceGoodricke);
            context.Add(aldonExley);
            context.Add(waltonAlvarez);

            context.SaveChanges();
        }
    }
}
