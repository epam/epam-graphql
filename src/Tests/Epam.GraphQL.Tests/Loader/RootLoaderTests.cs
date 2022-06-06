// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Collections.Generic;
using System.Linq;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NSubstitute;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Loader
{
    [TestFixture]
    public class RootLoaderTests : BaseTests
    {
        [Test]
        public void TestQueryItems()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("people")
                    .FromLoader<Person, TestUserContext>(personLoaderType);
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        people {
                            id
                        }
                    }",
                @"{
                    people: [
                        { id: 1 },
                        { id: 2 },
                        { id: 3 },
                        { id: 4 },
                        { id: 5 },
                        { id: 6 }
                    ]
                }");
        }

        [Test]
        public void TestQueryItemsInt()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<int, TestUserContext>(
                onConfigure: loader => loader.Field("id", id => id),
                getBaseQuery: _ => FakeData.People.Select(p => p.Id).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("people")
                    .FromLoader<int, TestUserContext>(personLoaderType);
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        people {
                            id
                        }
                    }",
                @"{
                    people: [
                        { id: 1 },
                        { id: 2 },
                        { id: 3 },
                        { id: 4 },
                        { id: 5 },
                        { id: 6 }
                    ]
                }");
        }

        [Test]
        public void TestQueryItemsViaShortcut()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Field<Person, TestUserContext>("people", personLoaderType);
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        people {
                            id
                        }
                    }",
                @"{
                    people: [
                        { id: 1 },
                        { id: 2 },
                        { id: 3 },
                        { id: 4 },
                        { id: 5 },
                        { id: 6 }
                    ]
                }");
        }

        [Test]
        public void TestQueryItemsDisableTracking()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("people")
                    .FromLoader<Person, TestUserContext>(personLoaderType);
            }

            var dataAdapter = Substitute.For<IDataContext>();
            dataAdapter.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<Proxy<Person>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<Proxy<Person>>>(0));

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        people {
                            id
                        }
                    }",
                @"{
                    people: [
                        { id: 1 },
                        { id: 2 },
                        { id: 3 },
                        { id: 4 },
                        { id: 5 },
                        { id: 6 }
                    ]
                }",
                dataAdapter);

            dataAdapter.Received(1).QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<Proxy<Person>>>());
        }

        [Test]
        public void TestQueryItemsFragment()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("people")
                    .FromLoader<Person, TestUserContext>(personLoaderType);
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        people {
                            ...person
                        }
                    }
                    fragment person on Person {
                        id
                    }",
                @"{
                    people: [
                        { id: 1 },
                        { id: 2 },
                        { id: 3 },
                        { id: 4 },
                        { id: 5 },
                        { id: 6 }
                    ]
                }");
        }

        [Test]
        public void TestWithCalculatedField()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field("id", p => 2 * p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("people")
                    .FromLoader<Person, TestUserContext>(personLoaderType);
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        people {
                            id
                        }
                    }",
                @"{
                    people: [
                        { id: 2 },
                        { id: 4 },
                        { id: 6 },
                        { id: 8 },
                        { id: 10 },
                        { id: 12 }
                    ]
                }");
        }

        [Test]
        public void TestWithRenamedField()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field("identity", p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("people")
                    .FromLoader<Person, TestUserContext>(personLoaderType);
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        people {
                            identity
                        }
                    }",
                @"{
                    people: [
                        { identity: 1 },
                        { identity: 2 },
                        { identity: 3 },
                        { identity: 4 },
                        { identity: 5 },
                        { identity: 6 }
                    ]
                }");
        }

        [Test]
        public void TestQueryItemsAsConnection()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("people")
                    .FromLoader<Person, TestUserContext>(personLoaderType)
                    .AsConnection(people => people.OrderByDescending(person => person.Id));
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        people {
                            items {
                                id
                            }
                        }
                    }",
                @"{
                    people: {
                        items: [
                            { id: 6 },
                            { id: 5 },
                            { id: 4 },
                            { id: 3 },
                            { id: 2 },
                            { id: 1 }
                        ]
                    }
                }");
        }

        [Test]
        public void TestQueryItemsWithSearch()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("people")
                    .FromLoader<Person, TestUserContext>(personLoaderType)
                    .WithSearch(GraphQLTypeBuilder.CreateSearcherType<Person, TestUserContext>((people, context, search) => people.Where(person => person.FullName.Contains(search))));
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        people(search: ""a"") {
                            id
                        }
                    }",
                @"{
                    people: [
                        { id: 2 },
                        { id: 3 },
                        { id: 4 },
                        { id: 6 }
                    ]
                }");
        }

        [Test]
        public void TestQueryItemsWithWere()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("people")
                    .FromLoader<Person, TestUserContext>(personLoaderType)
                    .Where(p => p.Id < 4);
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        people {
                            id
                        }
                    }",
                @"{
                    people: [
                        { id: 1 },
                        { id: 2 },
                        { id: 3 }
                    ]
                }");
        }

        public static class CustomObject
        {
            public static CustomObject<T> Create<T>(T fieldValue) => new() { TestField = fieldValue };
        }

        public class PersonFilter : Input
        {
            public List<int> Ids { get; set; }
        }

        public class CustomObject<T>
        {
            public T TestField { get; set; }
        }
    }
}
