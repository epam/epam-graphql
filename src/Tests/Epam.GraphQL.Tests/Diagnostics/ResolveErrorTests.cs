// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Threading.Tasks;
using Epam.GraphQL.Infrastructure;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using Microsoft.Extensions.Logging;
using NSubstitute;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Diagnostics
{
    [TestFixture]
    public class ResolveErrorTests : BaseTests
    {
        private ILoggerFactory _loggerFactory;
        private MockLogger _logger;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            _loggerFactory = Substitute.For<ILoggerFactory>();
            _logger = Substitute.For<MockLogger>();
            _loggerFactory.CreateLogger(Arg.Any<string>())
                .Returns(_logger);
        }

        [Test]
        public void Resolve()
        {
            static int Resolver(TestUserContext context)
            {
                throw new InvalidOperationException();
            }

            static void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("test")
                    .Resolve(Resolver);
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);

            Assert.Throws<InvalidOperationException>(() => ExecuteHelpers.ExecuteQuery(
                queryType,
                @"query {
                    test
                }",
                configure: optionsBuilder => optionsBuilder.UseLoggerFactory(_loggerFactory)));

            var expected = TestHelpers.ConcatLines(
                "Error during resolving field `test`.",
                "Query:",
                "public override void OnConfigure()",
                "{",
                "    Field(\"test\")",
                "        .Resolve<int>(Resolver); // <-----",
                "}");

            _logger.Received()
                .Log(
                    Constants.Logging.ExecutionError.Level,
                    Arg.Is<string>(message => message.Equals(expected, StringComparison.Ordinal)));
        }

        [Test]
        public void ResolveTask()
        {
            static async Task<int> Resolver(TestUserContext context)
            {
                return await ResolverCore().ConfigureAwait(false);

                static Task<int> ResolverCore()
                {
                    throw new InvalidOperationException();
                }
            }

            static void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("test")
                    .Resolve(Resolver);
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);

            Assert.Throws<InvalidOperationException>(() => ExecuteHelpers.ExecuteQuery(
                queryType,
                @"query {
                    test
                }",
                configure: optionsBuilder => optionsBuilder.UseLoggerFactory(_loggerFactory)));

            var expected = TestHelpers.ConcatLines(
                "Error during resolving field `test`.",
                "Query:",
                "public override void OnConfigure()",
                "{",
                "    Field(\"test\")",
                "        .Resolve<int>(Resolver); // <-----",
                "}");

            _logger.Received()
                .Log(
                    Constants.Logging.ExecutionError.Level,
                    Arg.Is<string>(message => message.Equals(expected, StringComparison.Ordinal)));
        }

        [Test]
        public void NestedResolve()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field("test").Resolve(Resolver),
                getBaseQuery: _ => FakeData.People.Take(1).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("people")
                    .FromLoader<Person, TestUserContext>(personLoaderType);
            }

            static Task<int> Resolver(TestUserContext context, Person person)
            {
                throw new InvalidOperationException();
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);

            Assert.Throws<InvalidOperationException>(() => ExecuteHelpers.ExecuteQuery(
                queryType,
                @"query {
                    people {
                        test
                    }
                }",
                configure: optionsBuilder => optionsBuilder.UseLoggerFactory(_loggerFactory)));

            var expected = TestHelpers.ConcatLines(
                "Error during resolving field `people.0.test`.",
                "PersonLoader:",
                "public override void OnConfigure()",
                "{",
                "    Field(\"test\")",
                "        .Resolve<int>(Resolver); // <-----",
                "}");

            _logger.Received()
                .Log(
                    Constants.Logging.ExecutionError.Level,
                    Arg.Is<string>(message => message.Equals(expected, StringComparison.Ordinal)));
        }

        [Test]
        public void NestedResolveTask()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field("test").Resolve(Resolver),
                getBaseQuery: _ => FakeData.People.Take(1).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("people")
                    .FromLoader<Person, TestUserContext>(personLoaderType);
            }

            static async Task<int> Resolver(TestUserContext context, Person person)
            {
                return await ResolverCore().ConfigureAwait(false);

                static Task<int> ResolverCore()
                {
                    throw new InvalidOperationException();
                }
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);

            Assert.Throws<InvalidOperationException>(() => ExecuteHelpers.ExecuteQuery(
                queryType,
                @"query {
                    people {
                        test
                    }
                }",
                configure: optionsBuilder => optionsBuilder.UseLoggerFactory(_loggerFactory)));

            var expected = TestHelpers.ConcatLines(
                "Error during resolving field `people.0.test`.",
                "PersonLoader:",
                "public override void OnConfigure()",
                "{",
                "    Field(\"test\")",
                "        .Resolve<int>(Resolver); // <-----",
                "}");

            _logger.Received()
                .Log(
                    Constants.Logging.ExecutionError.Level,
                    Arg.Is<string>(message => message.Equals(expected, StringComparison.Ordinal)));
        }

        [Test]
        public void ResolveOneArg()
        {
            static int Resolver(TestUserContext context, int? arg1)
            {
                throw new InvalidOperationException();
            }

            static void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("test")
                    .Argument<int?>("arg1")
                    .Resolve(Resolver);
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);

            Assert.Throws<InvalidOperationException>(() => ExecuteHelpers.ExecuteQuery(
                queryType,
                @"query {
                    test
                }",
                configure: optionsBuilder => optionsBuilder.UseLoggerFactory(_loggerFactory)));

            var expected = TestHelpers.ConcatLines(
                "Error during resolving field `test`.",
                "Query:",
                "public override void OnConfigure()",
                "{",
                "    Field(\"test\")",
                "        .Argument<int?>(\"arg1\")",
                "        .Resolve<int>(Resolver); // <-----",
                "}");

            _logger.Received()
                .Log(
                    Constants.Logging.ExecutionError.Level,
                    Arg.Is<string>(message => message.Equals(expected, StringComparison.Ordinal)));
        }

        [Test]
        public void ResolveTaskOneArg()
        {
            static async Task<int> Resolver(TestUserContext context, int? arg1)
            {
                return await ResolverCore().ConfigureAwait(false);

                static Task<int> ResolverCore()
                {
                    throw new InvalidOperationException();
                }
            }

            static void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("test")
                    .Argument<int?>("arg1")
                    .Resolve(Resolver);
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);

            Assert.Throws<InvalidOperationException>(() => ExecuteHelpers.ExecuteQuery(
                queryType,
                @"query {
                    test
                }",
                configure: optionsBuilder => optionsBuilder.UseLoggerFactory(_loggerFactory)));

            var expected = TestHelpers.ConcatLines(
                "Error during resolving field `test`.",
                "Query:",
                "public override void OnConfigure()",
                "{",
                "    Field(\"test\")",
                "        .Argument<int?>(\"arg1\")",
                "        .Resolve<int>(Resolver); // <-----",
                "}");

            _logger.Received()
                .Log(
                    Constants.Logging.ExecutionError.Level,
                    Arg.Is<string>(message => message.Equals(expected, StringComparison.Ordinal)));
        }

        [Test]
        public void ResolveTwoArgs()
        {
            static int Resolver(TestUserContext context, int? arg1, int? arg2)
            {
                throw new InvalidOperationException();
            }

            static void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("test")
                    .Argument<int?>("arg1")
                    .Argument<int?>("arg2")
                    .Resolve(Resolver);
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);

            Assert.Throws<InvalidOperationException>(() => ExecuteHelpers.ExecuteQuery(
                queryType,
                @"query {
                    test
                }",
                configure: optionsBuilder => optionsBuilder.UseLoggerFactory(_loggerFactory)));

            var expected = TestHelpers.ConcatLines(
                "Error during resolving field `test`.",
                "Query:",
                "public override void OnConfigure()",
                "{",
                "    Field(\"test\")",
                "        .Argument<int?>(\"arg1\")",
                "        .Argument<int?>(\"arg2\")",
                "        .Resolve<int>(Resolver); // <-----",
                "}");

            _logger.Received()
                .Log(
                    Constants.Logging.ExecutionError.Level,
                    Arg.Is<string>(message => message.Equals(expected, StringComparison.Ordinal)));
        }

        [Test]
        public void ResolveTaskTwoArgs()
        {
            static async Task<int> Resolver(TestUserContext context, int? arg1, int? arg2)
            {
                return await ResolverCore().ConfigureAwait(false);

                static Task<int> ResolverCore()
                {
                    throw new InvalidOperationException();
                }
            }

            static void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("test")
                    .Argument<int?>("arg1")
                    .Argument<int?>("arg2")
                    .Resolve(Resolver);
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);

            Assert.Throws<InvalidOperationException>(() => ExecuteHelpers.ExecuteQuery(
                queryType,
                @"query {
                    test
                }",
                configure: optionsBuilder => optionsBuilder.UseLoggerFactory(_loggerFactory)));

            var expected = TestHelpers.ConcatLines(
                "Error during resolving field `test`.",
                "Query:",
                "public override void OnConfigure()",
                "{",
                "    Field(\"test\")",
                "        .Argument<int?>(\"arg1\")",
                "        .Argument<int?>(\"arg2\")",
                "        .Resolve<int>(Resolver); // <-----",
                "}");

            _logger.Received()
                .Log(
                    Constants.Logging.ExecutionError.Level,
                    Arg.Is<string>(message => message.Equals(expected, StringComparison.Ordinal)));
        }

        [Test]
        public void ResolveThreeArgs()
        {
            static int Resolver(TestUserContext context, int? arg1, int? arg2, int? arg3)
            {
                throw new InvalidOperationException();
            }

            static void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("test")
                    .Argument<int?>("arg1")
                    .Argument<int?>("arg2")
                    .Argument<int?>("arg3")
                    .Resolve(Resolver);
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);

            Assert.Throws<InvalidOperationException>(() => ExecuteHelpers.ExecuteQuery(
                queryType,
                @"query {
                    test
                }",
                configure: optionsBuilder => optionsBuilder.UseLoggerFactory(_loggerFactory)));

            var expected = TestHelpers.ConcatLines(
                "Error during resolving field `test`.",
                "Query:",
                "public override void OnConfigure()",
                "{",
                "    Field(\"test\")",
                "        .Argument<int?>(\"arg1\")",
                "        .Argument<int?>(\"arg2\")",
                "        .Argument<int?>(\"arg3\")",
                "        .Resolve<int>(Resolver); // <-----",
                "}");

            _logger.Received()
                .Log(
                    Constants.Logging.ExecutionError.Level,
                    Arg.Is<string>(message => message.Equals(expected, StringComparison.Ordinal)));
        }

        [Test]
        public void ResolveTaskThreeArgs()
        {
            static async Task<int> Resolver(TestUserContext context, int? arg1, int? arg2, int? arg3)
            {
                return await ResolverCore().ConfigureAwait(false);

                static Task<int> ResolverCore()
                {
                    throw new InvalidOperationException();
                }
            }

            static void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("test")
                    .Argument<int?>("arg1")
                    .Argument<int?>("arg2")
                    .Argument<int?>("arg3")
                    .Resolve(Resolver);
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);

            Assert.Throws<InvalidOperationException>(() => ExecuteHelpers.ExecuteQuery(
                queryType,
                @"query {
                    test
                }",
                configure: optionsBuilder => optionsBuilder.UseLoggerFactory(_loggerFactory)));

            var expected = TestHelpers.ConcatLines(
                "Error during resolving field `test`.",
                "Query:",
                "public override void OnConfigure()",
                "{",
                "    Field(\"test\")",
                "        .Argument<int?>(\"arg1\")",
                "        .Argument<int?>(\"arg2\")",
                "        .Argument<int?>(\"arg3\")",
                "        .Resolve<int>(Resolver); // <-----",
                "}");

            _logger.Received()
                .Log(
                    Constants.Logging.ExecutionError.Level,
                    Arg.Is<string>(message => message.Equals(expected, StringComparison.Ordinal)));
        }

        [Test]
        public void ResolveFourArgs()
        {
            static int Resolver(TestUserContext context, int? arg1, int? arg2, int? arg3, int? arg4)
            {
                throw new InvalidOperationException();
            }

            static void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("test")
                    .Argument<int?>("arg1")
                    .Argument<int?>("arg2")
                    .Argument<int?>("arg3")
                    .Argument<int?>("arg4")
                    .Resolve(Resolver);
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);

            Assert.Throws<InvalidOperationException>(() => ExecuteHelpers.ExecuteQuery(
                queryType,
                @"query {
                    test
                }",
                configure: optionsBuilder => optionsBuilder.UseLoggerFactory(_loggerFactory)));

            var expected = TestHelpers.ConcatLines(
                "Error during resolving field `test`.",
                "Query:",
                "public override void OnConfigure()",
                "{",
                "    Field(\"test\")",
                "        .Argument<int?>(\"arg1\")",
                "        .Argument<int?>(\"arg2\")",
                "        .Argument<int?>(\"arg3\")",
                "        .Argument<int?>(\"arg4\")",
                "        .Resolve<int>(Resolver); // <-----",
                "}");

            _logger.Received()
                .Log(
                    Constants.Logging.ExecutionError.Level,
                    Arg.Is<string>(message => message.Equals(expected, StringComparison.Ordinal)));
        }

        [Test]
        public void ResolveTaskFourArgs()
        {
            static async Task<int> Resolver(TestUserContext context, int? arg1, int? arg2, int? arg3, int? arg4)
            {
                return await ResolverCore().ConfigureAwait(false);

                static Task<int> ResolverCore()
                {
                    throw new InvalidOperationException();
                }
            }

            static void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("test")
                    .Argument<int?>("arg1")
                    .Argument<int?>("arg2")
                    .Argument<int?>("arg3")
                    .Argument<int?>("arg4")
                    .Resolve(Resolver);
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);

            Assert.Throws<InvalidOperationException>(() => ExecuteHelpers.ExecuteQuery(
                queryType,
                @"query {
                    test
                }",
                configure: optionsBuilder => optionsBuilder.UseLoggerFactory(_loggerFactory)));

            var expected = TestHelpers.ConcatLines(
                "Error during resolving field `test`.",
                "Query:",
                "public override void OnConfigure()",
                "{",
                "    Field(\"test\")",
                "        .Argument<int?>(\"arg1\")",
                "        .Argument<int?>(\"arg2\")",
                "        .Argument<int?>(\"arg3\")",
                "        .Argument<int?>(\"arg4\")",
                "        .Resolve<int>(Resolver); // <-----",
                "}");

            _logger.Received()
                .Log(
                    Constants.Logging.ExecutionError.Level,
                    Arg.Is<string>(message => message.Equals(expected, StringComparison.Ordinal)));
        }

        [Test]
        public void ResolveFiveArgs()
        {
            static int Resolver(TestUserContext context, int? arg1, int? arg2, int? arg3, int? arg4, int? arg5)
            {
                throw new InvalidOperationException();
            }

            static void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("test")
                    .Argument<int?>("arg1")
                    .Argument<int?>("arg2")
                    .Argument<int?>("arg3")
                    .Argument<int?>("arg4")
                    .Argument<int?>("arg5")
                    .Resolve(Resolver);
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);

            Assert.Throws<InvalidOperationException>(() => ExecuteHelpers.ExecuteQuery(
                queryType,
                @"query {
                    test
                }",
                configure: optionsBuilder => optionsBuilder.UseLoggerFactory(_loggerFactory)));

            var expected = TestHelpers.ConcatLines(
                "Error during resolving field `test`.",
                "Query:",
                "public override void OnConfigure()",
                "{",
                "    Field(\"test\")",
                "        .Argument<int?>(\"arg1\")",
                "        .Argument<int?>(\"arg2\")",
                "        .Argument<int?>(\"arg3\")",
                "        .Argument<int?>(\"arg4\")",
                "        .Argument<int?>(\"arg5\")",
                "        .Resolve<int>(Resolver); // <-----",
                "}");

            _logger.Received()
                .Log(
                    Constants.Logging.ExecutionError.Level,
                    Arg.Is<string>(message => message.Equals(expected, StringComparison.Ordinal)));
        }

        [Test]
        public void ResolveTaskFiveArgs()
        {
            static async Task<int> Resolver(TestUserContext context, int? arg1, int? arg2, int? arg3, int? arg4, int? arg5)
            {
                return await ResolverCore().ConfigureAwait(false);

                static Task<int> ResolverCore()
                {
                    throw new InvalidOperationException();
                }
            }

            static void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("test")
                    .Argument<int?>("arg1")
                    .Argument<int?>("arg2")
                    .Argument<int?>("arg3")
                    .Argument<int?>("arg4")
                    .Argument<int?>("arg5")
                    .Resolve(Resolver);
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);

            Assert.Throws<InvalidOperationException>(() => ExecuteHelpers.ExecuteQuery(
                queryType,
                @"query {
                    test
                }",
                configure: optionsBuilder => optionsBuilder.UseLoggerFactory(_loggerFactory)));

            var expected = TestHelpers.ConcatLines(
                "Error during resolving field `test`.",
                "Query:",
                "public override void OnConfigure()",
                "{",
                "    Field(\"test\")",
                "        .Argument<int?>(\"arg1\")",
                "        .Argument<int?>(\"arg2\")",
                "        .Argument<int?>(\"arg3\")",
                "        .Argument<int?>(\"arg4\")",
                "        .Argument<int?>(\"arg5\")",
                "        .Resolve<int>(Resolver); // <-----",
                "}");

            _logger.Received()
                .Log(
                    Constants.Logging.ExecutionError.Level,
                    Arg.Is<string>(message => message.Equals(expected, StringComparison.Ordinal)));
        }

        public abstract class MockLogger : ILogger
        {
            void ILogger.Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter) =>
                Log(logLevel, formatter(state, exception));

            public abstract void Log(LogLevel logLevel, string message);

            public virtual bool IsEnabled(LogLevel logLevel) => true;

            public abstract IDisposable BeginScope<TState>(TState state);
        }
    }
}
