// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Linq;
using Epam.GraphQL.Tests;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NUnit.Framework;

namespace Epam.GraphQL.EntityFrameworkCore.Tests
{
    [TestFixture]
    public class FromIQueryableTests : BaseTests
    {
        [Test(Description = "FromIQueryable(...)/Where(...)")]
        public void TestFromIQueryableWithWhere()
        {
            var unitLoader = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("emps")
                        .FromIQueryable(
                            context => context.DbContext.People,
                            (u, p) => u.Id == p.UnitId,
                            personBuilder => personBuilder.Field(p => p.Id));
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: context => context.DbContext.Units);

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(unitLoader, "units");
            }

            TestQuery(
                Builder,
                @"
                    query {
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
        }

        [Test(Description = "FromIQueryable(..., selector)/Where(...)")]
        public void TestFromIQueryableWithSelectorAndWhere()
        {
            var unitLoader = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("emps")
                        .FromIQueryable(
                            context => context.DbContext.People,
                            (u, p) => u.Id == p.UnitId)
                        .Select(p => p.Id);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: context => context.DbContext.Units);

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(unitLoader, "units");
            }

            TestQuery(
                Builder,
                @"
                    query {
                        units {
                            items {
                                id
                                emps
                            }
                        }
                    }",
                @"{
                    units: {
                        items: [{
                            id: 1,
                            emps: [1, 2, 3]
                        }, {
                            id: 2,
                            emps: [4, 5, 6]
                        }]
                    }
                }");
        }

        [Test]
        public void TestFromIQueryableWithSingleOrDefault()
        {
            var unitLoader = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("emps")
                        .FromIQueryable(
                            context => context.DbContext.People,
                            (u, p) => u.Id == p.UnitId)
                        .SingleOrDefault(p => p.Id == 1);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: context => context.DbContext.Units);

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(unitLoader, "units");
            }

            TestQuery(
                Builder,
                @"
                    query {
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
                            emps: {
                                id: 1
                            }
                        }, {
                            id: 2,
                            emps: null
                        }]
                    }
                }");
        }

        [Test(Description = "FromIQueryable(...)/Where(...)/Where(...)")]
        public void TestFromIQueryableWithWhereAndWhere()
        {
            var unitLoader = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("emps")
                        .FromIQueryable(
                            context => context.DbContext.People,
                            (u, p) => u.Id == p.UnitId,
                            personBuilder => personBuilder.Field(p => p.Id))
                        .Where(p => p.Id == 1);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: context => context.DbContext.Units);

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(unitLoader, "units");
            }

            TestQuery(
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
                }");
        }

        [Test(Description = "FromIQueryable(...)/Where(<complicated condition>)/")]
        public void TestFromIQueryableWithWhereComplicatedCondition()
        {
            var unitLoader = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("emps")
                        .FromIQueryable(
                            context => context.DbContext.People,
                            (u, p) => u.Id == p.UnitId && p.Id == 1,
                            personBuilder => personBuilder.Field(p => p.Id));
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: context => context.DbContext.Units);

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(unitLoader, "units");
            }

            TestQuery(
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
                }");
        }
    }
}
