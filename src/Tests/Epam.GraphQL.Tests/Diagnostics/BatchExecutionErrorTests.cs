// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.Mock;
using Epam.GraphQL.Tests.TestData;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Diagnostics
{
    [TestFixture]
    public class BatchExecutionErrorTests : BaseTests
    {
        [Test]
        public void FromBatchReturnsNullDictionary()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader
                        .Field("test")
                        .FromBatch(BatchFunc);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable());

            static IDictionary<Person, int> BatchFunc(IEnumerable<Person> items)
            {
                return null;
            }

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("people")
                    .FromLoader<Person, TestUserContext>(personLoaderType);
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);

            var result = ExecuteHelpers.ExecuteQuery(
                queryType,
                @"query {
                    people {
                        test
                    }
                }");

            Assert.That(
                result.Errors,
                Is.Not.Empty.And.All.Message.EqualTo(TestHelpers.ConcatLines(
                    "Error during resolving field `people.0.test`. Batch delegate has returned null.",
                    "PersonLoader:",
                    "public override void OnConfigure()",
                    "{",
                    "    Field(\"test\")",
                    "        .FromBatch<int>(BatchFunc); // <-----",
                    "}")));
        }

        [Test]
        public void FromBatchWithBuildReturnsNullDictionary()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader
                        .Field("test")
                        .FromBatch(BatchFunc, build => build.Field(p => p.Id));
                },
                getBaseQuery: _ => FakeData.People.AsQueryable());

            static IDictionary<Person, Person> BatchFunc(IEnumerable<Person> items)
            {
                return null;
            }

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("people")
                    .FromLoader<Person, TestUserContext>(personLoaderType);
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);

            var result = ExecuteHelpers.ExecuteQuery(
                queryType,
                @"query {
                    people {
                        test {
                            id
                        }
                    }
                }");

            Assert.That(
                result.Errors,
                Is.Not.Empty.And.All.Message.EqualTo(TestHelpers.ConcatLines(
                    "Error during resolving field `people.0.test`. Batch delegate has returned null.",
                    "PersonLoader:",
                    "public override void OnConfigure()",
                    "{",
                    "    Field(\"test\")",
                    "        .FromBatch<Person>(",
                    "            BatchFunc, // <-----",
                    "            build =>",
                    "            {",
                    "                // ...",
                    "            });",
                    "}")));
        }

        [Test]
        public void FromBatchTaskReturnsNull()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader
                    .Field("test")
                    .FromBatch(BatchFunc),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            static Task<IDictionary<Person, int>> BatchFunc(IEnumerable<Person> items)
            {
                return null;
            }

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("people")
                    .FromLoader<Person, TestUserContext>(personLoaderType);
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);

            var result = ExecuteHelpers.ExecuteQuery(
                queryType,
                @"query {
                    people {
                        test
                    }
                }");

            Assert.That(
                result.Errors,
                Is.Not.Empty.And.All.Message.EqualTo(TestHelpers.ConcatLines(
                    "Error during resolving field `people.0.test`. Batch delegate has returned null.",
                    "PersonLoader:",
                    "public override void OnConfigure()",
                    "{",
                    "    Field(\"test\")",
                    "        .FromBatch<int>(BatchFunc); // <-----",
                    "}")));
        }

        [Test]
        public void FromBatchWithBuildTaskReturnsNull()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader
                    .Field("test")
                    .FromBatch(BatchFunc, build => build.Field(p => p.Id)),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            static Task<IDictionary<Person, Person>> BatchFunc(IEnumerable<Person> items)
            {
                return null;
            }

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("people")
                    .FromLoader<Person, TestUserContext>(personLoaderType);
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);

            var result = ExecuteHelpers.ExecuteQuery(
                queryType,
                @"query {
                    people {
                        test {
                            id
                        }
                    }
                }");

            Assert.That(
                result.Errors,
                Is.Not.Empty.And.All.Message.EqualTo(TestHelpers.ConcatLines(
                    "Error during resolving field `people.0.test`. Batch delegate has returned null.",
                    "PersonLoader:",
                    "public override void OnConfigure()",
                    "{",
                    "    Field(\"test\")",
                    "        .FromBatch<Person>(",
                    "            BatchFunc, // <-----",
                    "            build =>",
                    "            {",
                    "                // ...",
                    "            });",
                    "}")));
        }

        [Test]
        public void FromBatchTaskReturnsNullDictionary()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader
                    .Field("test")
                    .FromBatch(BatchFunc),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            static Task<IDictionary<Person, int>> BatchFunc(IEnumerable<Person> items)
            {
                return Task.FromResult<IDictionary<Person, int>>(null);
            }

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("people")
                    .FromLoader<Person, TestUserContext>(personLoaderType);
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);

            var result = ExecuteHelpers.ExecuteQuery(
                queryType,
                @"query {
                    people {
                        test
                    }
                }");

            Assert.That(
                result.Errors,
                Is.Not.Empty.And.All.Message.EqualTo(TestHelpers.ConcatLines(
                    "Error during resolving field `people.0.test`. Batch delegate has returned null dictionary.",
                    "PersonLoader:",
                    "public override void OnConfigure()",
                    "{",
                    "    Field(\"test\")",
                    "        .FromBatch<int>(BatchFunc); // <-----",
                    "}")));
        }

        [Test]
        public void FromBatchThrowsException()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader
                        .Field("test")
                        .FromBatch(BatchFunc);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable());

            static IDictionary<Person, int> BatchFunc(IEnumerable<Person> items)
            {
                throw new NotSupportedException("Something went wrong");
            }

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("people")
                    .FromLoader<Person, TestUserContext>(personLoaderType);
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);

            var result = ExecuteHelpers.ExecuteQuery(
                queryType,
                @"query {
                    people {
                        test
                    }
                }");

            Assert.That(
                result.Errors,
                Is.Not.Empty
                    .And.All.Message.EqualTo(TestHelpers.ConcatLines(
                        "Error during resolving field `people.0.test`. Batch delegate has thrown an exception. See an inner exception for details.",
                        "PersonLoader:",
                        "public override void OnConfigure()",
                        "{",
                        "    Field(\"test\")",
                        "        .FromBatch<int>(BatchFunc); // <-----",
                        "}"))
                    .And.InnerException.TypeOf<NotSupportedException>().And.InnerException.Message.EqualTo("Something went wrong"));
        }

        [Test]
        public void FromBatchTaskThrowsException()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader
                    .Field("test")
                    .FromBatch(BatchFunc),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            static Task<IDictionary<Person, int>> BatchFunc(IEnumerable<Person> items)
            {
                throw new NotSupportedException("Something went wrong");
            }

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("people")
                    .FromLoader<Person, TestUserContext>(personLoaderType);
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);

            var result = ExecuteHelpers.ExecuteQuery(
                queryType,
                @"query {
                    people {
                        test
                    }
                }");

            Assert.That(
                result.Errors,
                Is.Not.Empty
                    .And.All.Message.EqualTo(TestHelpers.ConcatLines(
                        "Error during resolving field `people.0.test`. Batch delegate has thrown an exception. See an inner exception for details.",
                        "PersonLoader:",
                        "public override void OnConfigure()",
                        "{",
                        "    Field(\"test\")",
                        "        .FromBatch<int>(BatchFunc); // <-----",
                        "}"))
                    .And.InnerException.TypeOf<NotSupportedException>().And.InnerException.Message.EqualTo("Something went wrong"));
        }

        [Test]
        public void OnEntityLoaderReturnsNull()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.OnEntityLoaded(p => p.Id, BatchFunc, (ctx, id) => { });
                },
                getBaseQuery: _ => FakeData.People.AsQueryable());

            static IDictionary<int, int> BatchFunc(TestUserContext ctx, IEnumerable<int> ids)
            {
                return null;
            }

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("people")
                    .FromLoader<Person, TestUserContext>(personLoaderType);
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);

            var result = ExecuteHelpers.ExecuteQuery(
                queryType,
                @"query {
                    people {
                        id
                    }
                }");

            Assert.That(
                result.Errors,
                Is.Not.Empty.And.All.Message.EqualTo(TestHelpers.ConcatLines(
                    "Error during resolving field `people`. Batch delegate has returned null.",
                    "PersonLoader:",
                    "public override void OnConfigure()",
                    "{",
                    "    Field(p => p.Id);",
                    string.Empty,
                    "    OnEntityLoaded(",
                    "        p => p.Id,",
                    "        BatchFunc, // <-----",
                    "        (ctx, id) => ...);",
                    "}")));
        }

        [Test]
        public void OnEntityLoaderTaskReturnsNull()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.OnEntityLoaded(p => p.Id, BatchFunc, (ctx, id) => { });
                },
                getBaseQuery: _ => FakeData.People.AsQueryable());

            static Task<IDictionary<int, int>> BatchFunc(TestUserContext ctx, IEnumerable<int> ids)
            {
                return Task.FromResult<IDictionary<int, int>>(null);
            }

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("people")
                    .FromLoader<Person, TestUserContext>(personLoaderType);
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);

            var result = ExecuteHelpers.ExecuteQuery(
                queryType,
                @"query {
                    people {
                        id
                    }
                }");

            Assert.That(
                result.Errors,
                Is.Not.Empty.And.All.Message.EqualTo(TestHelpers.ConcatLines(
                    "Error during resolving field `people`. Batch delegate has returned null dictionary.",
                    "PersonLoader:",
                    "public override void OnConfigure()",
                    "{",
                    "    Field(p => p.Id);",
                    string.Empty,
                    "    OnEntityLoaded(",
                    "        p => p.Id,",
                    "        BatchFunc, // <-----",
                    "        (ctx, id) => ...);",
                    "}")));
        }

        [Test]
        public void BatchEditableIfReturnsNullDictionary()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(x => x.Id);
                    loader.Field(x => x.FullName)
                        .BatchedEditableIf(BatchFunc, change => false);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable());

            static IDictionary<Person, int> BatchFunc(IEnumerable<Person> items)
            {
                return null;
            }

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("people")
                    .FromLoader<Person, TestUserContext>(personLoaderType);
            }

            void MutationBuilder(Mutation<TestUserContext> mutation)
            {
                mutation
                    .SubmitField(personLoaderType, "people");
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);
            var mutationType = GraphQLTypeBuilder.CreateMutationType<TestUserContext>(MutationBuilder);
            var dataContext = new DataContextMock();

            var result = ExecuteHelpers.ExecuteQuery(
                queryType,
                mutationType,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 1,
                            fullName: ""test""
                        }]
                    }) {
                        people {
                            id
                        }
                    }
                }",
                dataContext);

            Assert.That(
                result.Errors,
                Is.Not.Empty.And.All.Message.EqualTo(TestHelpers.ConcatLines(
                    "Error during resolving field `submit`. Batch delegate has returned null.",
                    "PersonLoader:",
                    "public override void OnConfigure()",
                    "{",
                    "    Field(x => x.Id);",
                    string.Empty,
                    "    Field(x => x.FullName)",
                    "        .BatchedEditableIf(",
                    "            BatchFunc, // <-----",
                    "            change => ...);",
                    "}")));
        }
    }
}
