// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Connection
{
    [TestFixture]
    public class ConnectionOrderingTests : BaseTests
    {
        [Test]
        public void ShouldOrderQueryItems()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection<Person, TestUserContext>(personLoaderType, "people", query => query.OrderByDescending(x => x.Id));
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
        public void ShouldWorkWithFromLoader()
        {
            var unitLoaderType = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader => loader.Field(p => p.Id),
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("units")
                        .FromLoader<Unit>(unitLoaderType, (p, u) => p.UnitId == u.Id);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection<Person, TestUserContext>(personLoaderType, "people", query => query.OrderByDescending(x => x.Id));
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        people {
                            items {
                                id
                                units {
                                    id
                                }
                            }
                        }
                    }",
                @"{
                    people: {
                        items: [{
                            id: 6,
                            units: [{
                                id: 2
                            }]
                        }, {
                            id: 5,
                            units: [{
                                id: 2
                            }]
                        }, {
                            id: 4,
                            units: [{
                                id: 2
                            }]
                        }, {
                            id: 3,
                            units: [{
                                id: 1
                            }]
                        }, {
                            id: 2,
                            units: [{
                                id: 1
                            }]
                        }, {
                            id: 1,
                            units: [{
                                id: 1
                            }]
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldOrderQueryItemsWithSorter()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Sorter("fullName", p => p.FullName);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection<Person, TestUserContext>(personLoaderType, "people", query => query.OrderByDescending(x => x.Id));
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        people(sorting: {field: ""fullName""}) {
                            items {
                                id
                            }
                        }
                    }",
                @"{
                    people: {
                        items: [
                            { id: 5 },
                            { id: 4 },
                            { id: 3 },
                            { id: 1 },
                            { id: 2 },
                            { id: 6 }
                        ]
                    }
                }");
        }

        [Test]
        public void ShouldThrowIfOrderDoesNotDefined()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQueryError(
                Builder,
                typeof(NotImplementedException),
                "You must override ApplyNaturalOrderBy method or pass order expression to Connection()/AsConnection() call.",
                @"
                    query {
                        people {
                            items {
                                id
                            }
                        }
                    }");
        }

        [Test]
        public void ShouldWorkWithFromLoaderAsConnection()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            var unitLoaderType = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("people")
                        .FromLoader<Person>(personLoaderType, (u, p) => u.Id == p.UnitId)
                        .AsConnection(query => query.OrderByDescending(p => p.Id));
                },
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection<Unit, TestUserContext>(unitLoaderType, "units", query => query.OrderByDescending(x => x.Id));
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        units {
                            items {
                                id
                                people {
                                    items {
                                        id
                                    }
                                }
                            }
                        }
                    }",
                @"{
                    units: {
                        items: [{
                            id: 2,
                            people: {
                                items: [{
                                    id: 6
                                }, {
                                    id: 5
                                }, {
                                    id: 4
                                }]
                            }
                        }, {
                            id: 1,
                            people: {
                                items: [{
                                    id: 3
                                }, {
                                    id: 2
                                }, {
                                    id: 1
                                }]
                            }
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldWorkWithFromQueryAsConnection()
        {
            var unitLoaderType = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("people")
                        .FromIQueryable(_ => FakeData.People.AsQueryable(), (u, p) => u.Id == p.UnitId)
                        .AsConnection(query => query.OrderByDescending(p => p.Id));
                },
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection<Unit, TestUserContext>(unitLoaderType, "units", query => query.OrderByDescending(x => x.Id));
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        units {
                            items {
                                id
                                people {
                                    items {
                                        id
                                    }
                                }
                            }
                        }
                    }",
                @"{
                    units: {
                        items: [{
                            id: 2,
                            people: {
                                items: [{
                                    id: 6
                                }, {
                                    id: 5
                                }, {
                                    id: 4
                                }]
                            }
                        }, {
                            id: 1,
                            people: {
                                items: [{
                                    id: 3
                                }, {
                                    id: 2
                                }, {
                                    id: 1
                                }]
                            }
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldWorkWithFromQueryWithBuilderAsConnection()
        {
            var unitLoaderType = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("people")
                        .FromIQueryable(
                            _ => FakeData.People.AsQueryable(),
                            (u, p) => u.Id == p.UnitId,
                            builder => builder.Field(p => p.Id))
                        .AsConnection(query => query.OrderByDescending(p => p.Id));
                },
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection<Unit, TestUserContext>(unitLoaderType, "units", query => query.OrderByDescending(x => x.Id));
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        units {
                            items {
                                id
                                people {
                                    items {
                                        id
                                    }
                                }
                            }
                        }
                    }",
                @"{
                    units: {
                        items: [{
                            id: 2,
                            people: {
                                items: [{
                                    id: 6
                                }, {
                                    id: 5
                                }, {
                                    id: 4
                                }]
                            }
                        }, {
                            id: 1,
                            people: {
                                items: [{
                                    id: 3
                                }, {
                                    id: 2
                                }, {
                                    id: 1
                                }]
                            }
                        }]
                    }
                }");
        }
    }
}
