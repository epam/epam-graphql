// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Linq;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.Mock;
using Epam.GraphQL.Tests.TestData;
using NUnit.Framework;

namespace Epam.GraphQL.Tests
{
    public partial class FromLoaderTests
    {
        [Test]
        public void TestFromLoaderWithSelect()
        {
            var unitLoader = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.Name);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => Enumerable.Repeat(FakeData.Alpha, 1).AsQueryable());

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("unit")
                        .FromLoader<Person, Unit, TestUserContext>(unitLoader, (p, u) => p.UnitId == u.Id)
                        .Select(u => new Unit { Id = u.Id });
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(1).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                Builder,
                @"query {
                    people {
                        items {
                            unit {
                                id
                            }
                        }
                    }
                }",
                @"{
                    people: {
                        items: [{
                            unit: [{
                                id: 1
                            }]
                        }]
                    }
                }");
        }

        [Test(Description = "Fix for https://git.epam.com/epm-ppa/epam-graphql/-/issues/72")]
        public void TestFromLoaderWithSelectEntity()
        {
            var unitLoader = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.HeadId);
                    loader.Field(u => u.Name);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => Enumerable.Repeat(FakeData.Alpha, 1).AsQueryable());

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("unit")
                        .FromLoader<Person, Unit, TestUserContext>(unitLoader, (p, u) => p.UnitId == u.Id)
                        .Select((p, u) => new Unit { Id = u.Id, HeadId = p.ManagerId });
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => Enumerable.Repeat(FakeData.SophieGandley, 1).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                Builder,
                @"query {
                    people {
                        items {
                            unit {
                                id
                                headId
                            }
                        }
                    }
                }",
                @"{
                    people: {
                        items: [{
                            unit: [{
                                id: 1,
                                headId: 1
                            }]
                        }]
                    }
                }");
        }

        [Test(Description = "Test for https://git.epam.com/epm-ppa/epam-graphql/-/issues/73")]
        public void TestFromLoaderWithSelectAnonymousType()
        {
            var unitLoader = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.Name);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => Enumerable.Repeat(FakeData.Alpha, 1).AsQueryable());

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("unit")
                        .FromLoader<Person, Unit, TestUserContext>(unitLoader, (p, u) => p.UnitId == u.Id)
                        .Select(u => new { u.Id });
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(1).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                Builder,
                @"query {
                    people {
                        items {
                            unit {
                                id
                            }
                        }
                    }
                }",
                @"{
                    people: {
                        items: [{
                            unit: [{
                                id: 1
                            }]
                        }]
                    }
                }",
                new DataContextMock());
        }

        [Test]
        public void TestFromLoaderWithSelectValueType()
        {
            var unitLoader = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.Name);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => Enumerable.Repeat(FakeData.Alpha, 1).AsQueryable());

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("unit")
                        .FromLoader<Person, Unit, TestUserContext>(unitLoader, (p, u) => p.UnitId == u.Id)
                        .Select(u => u.Id);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(1).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(Builder, "query t { people { items { unit } } }", "{ people: { items: [{ unit: [1] }] } }", null, null);
        }
    }
}
