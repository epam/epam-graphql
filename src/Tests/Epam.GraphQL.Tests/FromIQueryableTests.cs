// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NSubstitute;
using NUnit.Framework;

namespace Epam.GraphQL.Tests
{
    [TestFixture]
    public class FromIQueryableTests : BaseTests
    {
        private Func<TestUserContext, IQueryable<Person>> _getPeople;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();

            _getPeople = Substitute.For<Func<TestUserContext, IQueryable<Person>>>();

            _getPeople
                .Invoke(Arg.Any<TestUserContext>())
                .Returns(_ => FakeData.People.AsQueryable());
        }

        [Test(Description = "FromIQueryable(...)/Where(...)/ConfigureFrom")]
        public void TestFromIQueryableWithWhereConfigureFrom()
        {
            var peopleLoader = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.FullName);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            var unitLoader = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("emps")
                        .FromIQueryable(_getPeople, (u, p) => u.Id == p.UnitId, personBuilder => personBuilder.ConfigureFrom(peopleLoader));
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(unitLoader, "units");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        units {
                            items {
                                id 
                                emps { 
                                    id
                                    fullName
                                }
                            }
                        }
                    }",
                @"{
                    units: {
                        items: [{
                            id: 1,
                            emps: [{
                                id: 1,
                                fullName: ""Linoel Livermore""
                            }, {
                                id: 2,
                                fullName: ""Sophie Gandley""
                            }, {
                                id: 3,
                                fullName: ""Hannie Everitt""
                            }]
                        }, {
                            id: 2, 
                            emps: [{
                                id: 4,
                                fullName: ""Florance Goodricke""
                            }, {
                                id: 5,
                                fullName: ""Aldon Exley""
                            }, {
                                id: 6,
                                fullName: ""Walton Alvarez""
                            }]
                        }] 
                    }
                }");

            _getPeople
                .Received(1)
                .Invoke(Arg.Any<TestUserContext>());
        }

        [Test(Description = "FromIQueryable(...)/Where(...)/ConfigureFrom/Fragment")]
        public void TestFromIQueryableWithWhereConfigureFromFragment()
        {
            var peopleLoader = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            var unitLoader = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("emps")
                        .FromIQueryable(_getPeople, (u, p) => u.Id == p.UnitId, personBuilder => personBuilder.ConfigureFrom(peopleLoader));
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(unitLoader, "units");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        units {
                            items {
                                id 
                                emps {
                                    ...person 
                                }
                            }
                        }
                    }
                    fragment person on Person {
                        id
                    }",
                @"{
                    units: {
                        items: [{
                            id: 1,
                            emps: [{
                                id: 1
                            }, {
                                id: 2
                            }, {
                                id: 3
                            }]
                        }, {
                            id: 2, 
                            emps: [{
                                id: 4
                            }, {
                                id: 5
                            }, {
                                id: 6
                            }]
                        }] 
                    }
                }");

            _getPeople
                .Received(1)
                .Invoke(Arg.Any<TestUserContext>());
        }

        [Test(Description = "FromIQueryable(...)/Where(...)")]
        public void TestFromIQueryableWithWhereReverseOrder()
        {
            var unitLoader = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("emps")
                        .FromIQueryable(_getPeople, (u, p) => p.UnitId == u.Id, personBuilder =>
                        {
                            personBuilder.Field(p => p.Id);
                        });
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(unitLoader, "units");
            }

            TestHelpers.TestQuery(Builder, "query t { units { items { id emps { id } } } }", "{ units: { items: [{id: 1, emps: [{id: 1}, {id: 2}, {id: 3}]}, {id: 2, emps: [{id: 4}, {id: 5}, {id: 6}]}] } }", null, null);

            _getPeople
                .Received(1)
                .Invoke(Arg.Any<TestUserContext>());
        }

        [Test(Description = "FromIQueryable(...)/Where(...)/EmptyArray")]
        public void TestFromIQueryableWithWhereEmptyArray()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("manager")
                        .FromIQueryable(_getPeople, (p, m) => p.ManagerId == m.Id, personBuilder =>
                        {
                            personBuilder.Field(p => p.Id);
                        });
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        people {
                            items {
                                id
                                manager {
                                    id
                                }
                            }
                        }
                    }",
                @"{
                    people: {
                        items: [{
                            id: 1,
                            manager: []
                        }, {
                            id: 2,
                            manager: [{
                                id: 1
                            }]
                        }, {
                            id: 3,
                            manager: [{
                                id: 1
                            }]
                        }, {
                            id: 4,
                            manager: [{
                                id: 2
                            }]
                        }, {
                            id: 5,
                            manager: [{
                                id: 2
                            }]
                        }, {
                            id: 6,
                            manager: [{
                                id: 5
                            }]
                        }]
                    }
                }",
                null,
                null);

            _getPeople
                .Received(1)
                .Invoke(Arg.Any<TestUserContext>());
        }

        [Test(Description = "FromIQueryable(...)/Where(<complicated condition>)/")]
        public void TestFromIQueryableWithWhereComplicatedCondition()
        {
            var unitLoader = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("emps")
                        .FromIQueryable(_getPeople, (u, p) => u.Id == p.UnitId && p.Id == 1, personBuilder =>
                        {
                            personBuilder.Field(p => p.Id);
                        });
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(unitLoader, "units");
            }

            TestHelpers.TestQuery(
                Builder,
                @"query {
                            units {
                                items {
                                    id
                                    emps {
                                        id
                                    }
                                }
                            }
                        }",
                @"{
                            units: {
                                items: [{
                                    id: 1,
                                    emps: [{
                                        id: 1
                                    }]
                                }, {
                                    id: 2,
                                    emps: []
                                }]
                            }
                        }",
                null,
                null);

            _getPeople
                .Received(1)
                .Invoke(Arg.Any<TestUserContext>());
        }
    }
}
