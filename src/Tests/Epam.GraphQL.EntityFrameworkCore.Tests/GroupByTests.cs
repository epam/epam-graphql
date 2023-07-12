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
    public class GroupByTests : BaseTests
    {
        [Test]
        public void TestGroupConnectionQueryItems()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.ManagerId).Groupable();
                    loader.Field(p => p.UnitId).Groupable();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: context => context.DbContext.People);

            void Builder(Query<TestUserContext> query)
            {
                query
                    .GroupConnection(personLoaderType, "peopleGroups");
            }

            TestQuery(
                Builder,
                @"
                    query {
                        peopleGroups {
                            items {
                                item {
                                    unitId
                                }
                            }
                        }
                    }",
                @"{
                    peopleGroups: {
                        items: [{
                            item: {
                                unitId: 1
                            }
                        },{
                            item: {
                                unitId: 2
                            }
                        }]
                    }
                }");
        }

        [Test]
        public void TestGroupConnectionQueryItemsAndOrderBy()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.ManagerId).Groupable();
                    loader.Field(p => p.UnitId).Sortable().Groupable();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: context => context.DbContext.People);

            void Builder(Query<TestUserContext> query)
            {
                query
                    .GroupConnection(personLoaderType, "peopleGroups");
            }

            TestQuery(
                Builder,
                @"
                    query {
                        peopleGroups(sorting: {field: ""unitId"", direction: DESC}) {
                            items {
                                item {
                                    unitId
                                }
                            }
                        }
                    }",
                @"{
                    peopleGroups: {
                        items: [{
                            item: {
                                unitId: 2
                            }
                        },{
                            item: {
                                unitId: 1
                            }
                        }]
                    }
                }");
        }

        [Test]
        public void TestGroupConnectionQueryItemsAndCount()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.ManagerId).Groupable();
                    loader.Field(p => p.UnitId).Groupable();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: context => context.DbContext.People);

            void Builder(Query<TestUserContext> query)
            {
                query
                    .GroupConnection(personLoaderType, "peopleGroups");
            }

            TestQuery(
                Builder,
                @"
                    query {
                        peopleGroups {
                            items {
                                item {
                                    unitId
                                }
                                count
                            }
                        }
                    }",
                @"{
                    peopleGroups: {
                        items: [{
                            item: {
                                unitId: 1
                            },
                            count: 3
                        },{
                            item: {
                                unitId: 2
                            },
                            count: 3
                        }]
                    }
                }");
        }

        [Test]
        public void TestGroupConnectionQueryItemsWithFilter()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.ManagerId).Filterable().Groupable();
                    loader.Field(p => p.UnitId).Groupable();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: context => context.DbContext.People);

            void Builder(Query<TestUserContext> query)
            {
                query
                    .GroupConnection(personLoaderType, "peopleGroups");
            }

            TestQuery(
                Builder,
                @"
                    query {
                        peopleGroups(filter: {managerId: {in: [1,2]}}) {
                            items {
                                item {
                                    unitId
                                }
                                count
                            }
                        }
                    }",
                @"{
                    peopleGroups: {
                        items: [{
                            item: {
                                unitId: 1
                            },
                            count: 2
                        },{
                            item: {
                                unitId: 2
                            },
                            count: 2
                        }]
                    }
                }");
        }

        [Test]
        public void TestGroupConnectionWithCalculatedField()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.ManagerId).Groupable();
                    loader.Field("unitId1", p => p.UnitId * 2).Groupable();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: context => context.DbContext.People);

            void Builder(Query<TestUserContext> query)
            {
                query
                    .GroupConnection(personLoaderType, "peopleGroups");
            }

            TestQuery(
                Builder,
                @"
                    query {
                        peopleGroups {
                            items {
                                item {
                                    unitId1
                                }
                                count
                            }
                        }
                    }",
                @"{
                    peopleGroups: {
                        items: [{
                            item: {
                                unitId1: 2
                            },
                            count: 3
                        },{
                            item: {
                                unitId1: 4
                            },
                            count: 3
                        }]
                    }
                }");
        }

        [Test]
        public void TestGroupConnectionQueryFromNavigationCollection()
        {
            var unitLoaderType = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id).Groupable();
                    loader.Field("empId", p => p.Employees.Select(e => e.Id).FirstOrDefault()).Groupable();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: context => context.DbContext.Units);

            void Builder(Query<TestUserContext> query)
            {
                query
                    .GroupConnection(unitLoaderType, "units");
            }

            TestQuery(
                Builder,
                @"
                    query {
                        units {
                            items {
                                item {
                                    empId
                                }
                            }
                        }
                    }",
                @"{
                    units: {
                        items: [{
                            item: {
                                empId: 1
                            }
                        },{
                            item: {
                                empId: 4
                            }
                        }]
                    }
                }");
        }

        [Test]
        public void TestFromIQueryableGroupConnection()
        {
            static void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("peopleGroups")
                    .FromIQueryable(
                        ctx => Queryable.Select(ctx.DbContext.People, p => p.UnitId),
                        builder =>
                        {
                            builder.Field("id", x => x).Groupable();
                            builder.Field("twice", x => 2 * x).Groupable();
                        })
                        .AsGroupConnection();
            }

            TestQuery(
                Builder,
                @"
                    query {
                        peopleGroups {
                            items {
                                item {
                                    id
                                    twice
                                }
                                count
                            }
                        }
                    }",
                @"{
                    peopleGroups: {
                        items: [{
                            item: {
                                id: 1,
                                twice: 2
                            },
                            count: 3
                        },{
                            item: {
                                id: 2,
                                twice: 4
                            },
                            count: 3
                        }]
                    }
                }");
        }
    }
}
