// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Threading.Tasks;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Diagnostics
{
    [TestFixture]
    public class ResolveErrorTests : BaseTests
    {
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
            var mutationType = GraphQLTypeBuilder.CreateMutationType<TestUserContext>(_ => { });

            var result = ExecuteHelpers.ExecuteQuery(
                queryType,
                mutationType,
                @"query {
                    test
                }");

            Assert.That(
                result.Errors,
                Is.Not.Empty.And.All.Message.EqualTo(TestHelpers.ConcatLines(
                    "Error during resolving field `test`. See an inner exception for details.",
                    "Query:",
                    "public override void OnConfigure()",
                    "{",
                    "    Field(\"test\")",
                    "        .Resolve<int>(Resolver); // <-----",
                    "}"))
                    .And.InnerException.TypeOf<InvalidOperationException>());
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
            var mutationType = GraphQLTypeBuilder.CreateMutationType<TestUserContext>(_ => { });

            var result = ExecuteHelpers.ExecuteQuery(
                queryType,
                mutationType,
                @"query {
                    test
                }");

            Assert.That(
                result.Errors,
                Is.Not.Empty.And.All.Message.EqualTo(TestHelpers.ConcatLines(
                    "Error during resolving field `test`. See an inner exception for details.",
                    "Query:",
                    "public override void OnConfigure()",
                    "{",
                    "    Field(\"test\")",
                    "        .Resolve<int>(Resolver); // <-----",
                    "}"))
                    .And.InnerException.TypeOf<InvalidOperationException>());
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
            var mutationType = GraphQLTypeBuilder.CreateMutationType<TestUserContext>(_ => { });

            var result = ExecuteHelpers.ExecuteQuery(
                queryType,
                mutationType,
                @"query {
                    people {
                        test
                    }
                }");

            Assert.That(
                result.Errors,
                Is.Not.Empty.And.All.Message.EqualTo(TestHelpers.ConcatLines(
                    "Error during resolving field `people.0.test`. See an inner exception for details.",
                    "PersonLoader:",
                    "public override void OnConfigure()",
                    "{",
                    "    Field(\"test\")",
                    "        .Resolve<int>(Resolver); // <-----",
                    "}"))
                    .And.InnerException.TypeOf<InvalidOperationException>());
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
            var mutationType = GraphQLTypeBuilder.CreateMutationType<TestUserContext>(_ => { });

            var result = ExecuteHelpers.ExecuteQuery(
                queryType,
                mutationType,
                @"query {
                    people {
                        test
                    }
                }");

            Assert.That(
                result.Errors,
                Is.Not.Empty.And.All.Message.EqualTo(TestHelpers.ConcatLines(
                    "Error during resolving field `people.0.test`. See an inner exception for details.",
                    "PersonLoader:",
                    "public override void OnConfigure()",
                    "{",
                    "    Field(\"test\")",
                    "        .Resolve<int>(Resolver); // <-----",
                    "}"))
                    .And.InnerException.TypeOf<InvalidOperationException>());
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
            var mutationType = GraphQLTypeBuilder.CreateMutationType<TestUserContext>(_ => { });

            var result = ExecuteHelpers.ExecuteQuery(
                queryType,
                mutationType,
                @"query {
                    test
                }");

            Assert.That(
                result.Errors,
                Is.Not.Empty.And.All.Message.EqualTo(TestHelpers.ConcatLines(
                    "Error during resolving field `test`. See an inner exception for details.",
                    "Query:",
                    "public override void OnConfigure()",
                    "{",
                    "    Field(\"test\")",
                    "        .Argument<int?>(\"arg1\")",
                    "        .Resolve<int>(Resolver); // <-----",
                    "}"))
                    .And.InnerException.TypeOf<InvalidOperationException>());
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
            var mutationType = GraphQLTypeBuilder.CreateMutationType<TestUserContext>(_ => { });

            var result = ExecuteHelpers.ExecuteQuery(
                queryType,
                mutationType,
                @"query {
                    test
                }");

            Assert.That(
                result.Errors,
                Is.Not.Empty.And.All.Message.EqualTo(TestHelpers.ConcatLines(
                    "Error during resolving field `test`. See an inner exception for details.",
                    "Query:",
                    "public override void OnConfigure()",
                    "{",
                    "    Field(\"test\")",
                    "        .Argument<int?>(\"arg1\")",
                    "        .Resolve<int>(Resolver); // <-----",
                    "}"))
                    .And.InnerException.TypeOf<InvalidOperationException>());
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
            var mutationType = GraphQLTypeBuilder.CreateMutationType<TestUserContext>(_ => { });

            var result = ExecuteHelpers.ExecuteQuery(
                queryType,
                mutationType,
                @"query {
                    test
                }");

            Assert.That(
                result.Errors,
                Is.Not.Empty.And.All.Message.EqualTo(TestHelpers.ConcatLines(
                    "Error during resolving field `test`. See an inner exception for details.",
                    "Query:",
                    "public override void OnConfigure()",
                    "{",
                    "    Field(\"test\")",
                    "        .Argument<int?>(\"arg1\")",
                    "        .Argument<int?>(\"arg2\")",
                    "        .Resolve<int>(Resolver); // <-----",
                    "}"))
                    .And.InnerException.TypeOf<InvalidOperationException>());
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
            var mutationType = GraphQLTypeBuilder.CreateMutationType<TestUserContext>(_ => { });

            var result = ExecuteHelpers.ExecuteQuery(
                queryType,
                mutationType,
                @"query {
                    test
                }");

            Assert.That(
                result.Errors,
                Is.Not.Empty.And.All.Message.EqualTo(TestHelpers.ConcatLines(
                    "Error during resolving field `test`. See an inner exception for details.",
                    "Query:",
                    "public override void OnConfigure()",
                    "{",
                    "    Field(\"test\")",
                    "        .Argument<int?>(\"arg1\")",
                    "        .Argument<int?>(\"arg2\")",
                    "        .Resolve<int>(Resolver); // <-----",
                    "}"))
                    .And.InnerException.TypeOf<InvalidOperationException>());
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
            var mutationType = GraphQLTypeBuilder.CreateMutationType<TestUserContext>(_ => { });

            var result = ExecuteHelpers.ExecuteQuery(
                queryType,
                mutationType,
                @"query {
                    test
                }");

            Assert.That(
                result.Errors,
                Is.Not.Empty.And.All.Message.EqualTo(TestHelpers.ConcatLines(
                    "Error during resolving field `test`. See an inner exception for details.",
                    "Query:",
                    "public override void OnConfigure()",
                    "{",
                    "    Field(\"test\")",
                    "        .Argument<int?>(\"arg1\")",
                    "        .Argument<int?>(\"arg2\")",
                    "        .Argument<int?>(\"arg3\")",
                    "        .Resolve<int>(Resolver); // <-----",
                    "}"))
                    .And.InnerException.TypeOf<InvalidOperationException>());
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
            var mutationType = GraphQLTypeBuilder.CreateMutationType<TestUserContext>(_ => { });

            var result = ExecuteHelpers.ExecuteQuery(
                queryType,
                mutationType,
                @"query {
                    test
                }");

            Assert.That(
                result.Errors,
                Is.Not.Empty.And.All.Message.EqualTo(TestHelpers.ConcatLines(
                    "Error during resolving field `test`. See an inner exception for details.",
                    "Query:",
                    "public override void OnConfigure()",
                    "{",
                    "    Field(\"test\")",
                    "        .Argument<int?>(\"arg1\")",
                    "        .Argument<int?>(\"arg2\")",
                    "        .Argument<int?>(\"arg3\")",
                    "        .Resolve<int>(Resolver); // <-----",
                    "}"))
                    .And.InnerException.TypeOf<InvalidOperationException>());
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
            var mutationType = GraphQLTypeBuilder.CreateMutationType<TestUserContext>(_ => { });

            var result = ExecuteHelpers.ExecuteQuery(
                queryType,
                mutationType,
                @"query {
                    test
                }");

            Assert.That(
                result.Errors,
                Is.Not.Empty.And.All.Message.EqualTo(TestHelpers.ConcatLines(
                    "Error during resolving field `test`. See an inner exception for details.",
                    "Query:",
                    "public override void OnConfigure()",
                    "{",
                    "    Field(\"test\")",
                    "        .Argument<int?>(\"arg1\")",
                    "        .Argument<int?>(\"arg2\")",
                    "        .Argument<int?>(\"arg3\")",
                    "        .Argument<int?>(\"arg4\")",
                    "        .Resolve<int>(Resolver); // <-----",
                    "}"))
                    .And.InnerException.TypeOf<InvalidOperationException>());
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
            var mutationType = GraphQLTypeBuilder.CreateMutationType<TestUserContext>(_ => { });

            var result = ExecuteHelpers.ExecuteQuery(
                queryType,
                mutationType,
                @"query {
                    test
                }");

            Assert.That(
                result.Errors,
                Is.Not.Empty.And.All.Message.EqualTo(TestHelpers.ConcatLines(
                    "Error during resolving field `test`. See an inner exception for details.",
                    "Query:",
                    "public override void OnConfigure()",
                    "{",
                    "    Field(\"test\")",
                    "        .Argument<int?>(\"arg1\")",
                    "        .Argument<int?>(\"arg2\")",
                    "        .Argument<int?>(\"arg3\")",
                    "        .Argument<int?>(\"arg4\")",
                    "        .Resolve<int>(Resolver); // <-----",
                    "}"))
                    .And.InnerException.TypeOf<InvalidOperationException>());
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
            var mutationType = GraphQLTypeBuilder.CreateMutationType<TestUserContext>(_ => { });

            var result = ExecuteHelpers.ExecuteQuery(
                queryType,
                mutationType,
                @"query {
                    test
                }");

            Assert.That(
                result.Errors,
                Is.Not.Empty.And.All.Message.EqualTo(TestHelpers.ConcatLines(
                    "Error during resolving field `test`. See an inner exception for details.",
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
                    "}"))
                    .And.InnerException.TypeOf<InvalidOperationException>());
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
            var mutationType = GraphQLTypeBuilder.CreateMutationType<TestUserContext>(_ => { });

            var result = ExecuteHelpers.ExecuteQuery(
                queryType,
                mutationType,
                @"query {
                    test
                }");

            Assert.That(
                result.Errors,
                Is.Not.Empty.And.All.Message.EqualTo(TestHelpers.ConcatLines(
                    "Error during resolving field `test`. See an inner exception for details.",
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
                    "}"))
                    .And.InnerException.TypeOf<InvalidOperationException>());
        }
    }
}
