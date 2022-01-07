// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Linq;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NUnit.Framework;

namespace Epam.GraphQL.EntityFrameworkCore.Tests
{
    [TestFixture]
    public class QueryTests : BaseTests
    {
        [Test]
        public void QueryFromNavigationCollection()
        {
            var unitLoaderType = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("empId", p => p.Employees.Select(e => e.Id).FirstOrDefault());
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: context => context.DbContext.Units);

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(unitLoaderType, "units");
            }

            TestQuery(
                Builder,
                @"
                    query {
                        units {
                            items {
                                id
                                empId
                            }
                        }
                    }",
                @"{
                    units: {
                        items: [{
                            id: 1,
                            empId: 1
                        },{
                            id: 2,
                            empId: 4
                        }]
                    }
                }");
        }

        [Test]
        public void QueryFromNestedLoaderWithSorting()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id).Sortable();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: context => context.DbContext.People);

            var unitLoaderType = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("people")
                        .FromLoader<Person>(personLoaderType, (u, p) => u.Id == p.UnitId);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: context => context.DbContext.Units);

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(unitLoaderType, "units");
            }

            TestQuery(
                Builder,
                @"
                    query {
                        units {
                            items {
                                id
                                people(sorting: { field: ""id"", direction: DESC }) {
                                    id
                                }
                            }
                        }
                    }",
                @"{
                    units: {
                        items: [{
                            id: 1,
                            people: [{
                                id: 3
                            },{
                                id: 2
                            },{
                                id: 1
                            }]
                        },{
                            id: 2,
                            people: [{
                                id: 6
                            },{
                                id: 5
                            },{
                                id: 4
                            }]
                        }]
                    }
                }");
        }
    }
}
