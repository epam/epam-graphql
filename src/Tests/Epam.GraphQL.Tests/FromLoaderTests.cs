// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;

namespace Epam.GraphQL.Tests
{
    [TestFixture]
    public partial class FromLoaderTests : BaseTests
    {
        [Test]
        public void TestFromLoaderNullableIntToIntRelation()
        {
            var unitLoader = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.Name);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("unit")
                        .FromLoader<Unit>(unitLoader, (p, u) => p.UnitId == u.Id);
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
                @"
                query t {
                    people {
                        items {
                            unit {
                                id
                                name
                            }
                        }
                    }
                }",
                @"{
                    people: {
                        items: [{
                            unit: [{
                                id: 1,
                                name: ""Alpha""
                            }]
                        }]
                    }
                }");
        }

        [Test]
        public void TestFromLoaderNullableStringToStringRelation()
        {
            var unitLoader = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.StringId);
                    loader.Field(u => u.Name);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.StringId),
                applyNaturalThenBy: q => q.ThenBy(p => p.StringId),
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("unit")
                        .FromLoader<Unit>(unitLoader, (p, u) => p.UnitStringId == u.StringId);
                    loader.Field(p => p.UnitStringId);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(1).Concat(new[] { new Person { Id = 2 } }).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                query t {
                    people {
                        items {
                            id
                            unit {
                                stringId
                                name
                            }
                        }
                    }
                }",
                @"{
                    people: {
                        items: [{
                            id: 1,
                            unit: [{
                                stringId: ""1"",
                                name: ""Alpha""
                            }]
                        },{
                            id: 2,
                            unit: []
                        }]
                    }
                }");
        }

        [Test(Description = "FromLoader(...)/Where")]
        public void TestFromLoaderWhere()
        {
            var unitLoader = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.Name);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("unit")
                        .FromLoader<Unit>(unitLoader, (p, u) => p.UnitId == u.Id)
                        .Where(u => u.HeadId.HasValue);
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
                @"
                query t {
                    people {
                        items {
                            unit {
                                id
                                name
                            }
                        }
                    }
                }",
                @"{
                    people: {
                        items: [{
                            unit: [{
                                id: 1,
                                name: ""Alpha""
                            }]
                        }]
                    }
                }");
        }

        [Test(Description = "https://git.epam.com/epm-ppa/epam-graphql/-/issues/3")]
        public void TestFromLoaderAdditionalConditionForChild()
        {
            var unitLoader = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.Name);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("unit")
                        .FromLoader<Unit>(unitLoader, (p, u) => p.UnitId == u.Id && u.HeadId.HasValue);
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
                @"
                query t {
                    people {
                        items {
                            unit {
                                id
                                name
                            }
                        }
                    }
                }",
                @"{
                    people: {
                        items: [{
                            unit: [{
                                id: 1,
                                name: ""Alpha""
                            }]
                        }]
                    }
                }");
        }

        [Test(Description = "https://git.epam.com/epm-ppa/epam-graphql/-/issues/3")]
        public void TestFromLoaderAdditionalConditionForParent()
        {
            var unitLoader = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.Name);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("unit")
                        .FromLoader<Unit>(unitLoader, (p, u) => p.UnitId == u.Id && p.Id > 0);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(1).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQueryError(
                Builder,
                typeof(ArgumentOutOfRangeException),
                "Cannot use expression (p, u) => ((p.UnitId == Convert(u.Id, Nullable`1)) AndAlso (p.Id > 0)) as a relation between Person and Unit types. (Parameter 'condition')",
                @"
                query t {
                    people {
                        items {
                            unit {
                                id
                                name
                            }
                        }
                    }
                }");
        }

        [Test(Description = "FromLoader(...)/TotalCount")]
        public void TestFromLoaderTotalCount()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            var unitLoader = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("people")
                        .FromLoader<Person>(personLoaderType, (u, p) => u.Id == p.UnitId)
                        .AsConnection();
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
                    query t {
                        units {
                            items {
                                id
                                people(first: 1) {
                                    items {
                                        id
                                    }
                                    totalCount
                                }
                            }
                        }
                    }",
                @"{
                    units: {
                        items: [{
                            id: 1,
                            people: {
                                items: [{
                                    id: 1,
                                }],
                                totalCount: 3
                            }
                        },{
                            id: 2,
                            people: {
                                items: [{
                                    id: 4,
                                }],
                                totalCount: 3
                            }
                        }]
                    }
                }",
                null,
                null);
        }

        [Test]
        public void TestFromLoaderIntToIntRelation()
        {
            Type personLoaderType = null;
            personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
                    loader.Field("twin")
                        .FromLoader<Person>(personLoaderType, (p1, p2) => p1.Id == p2.Id);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(1).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query t {
                        people {
                            items {
                                twin {
                                    id
                                    fullName
                                }
                            }
                        }
                    }",
                @"{
                    people: {
                        items: [{
                            twin: [{
                                id: 1,
                                fullName: ""Linoel Livermore""
                            }]
                        }]
                    }
                }",
                null,
                null);
        }

        [Test]
        public void TestFromLoaderIntToNullableIntRelation()
        {
            Type personLoaderType = null;
            personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
                    loader.Field("subordinates")
                        .FromLoader<Person>(personLoaderType, (p1, p2) => p1.Id == p2.ManagerId);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(2).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query t {
                        people {
                            items {
                                id
                                subordinates {
                                    id
                                    fullName
                                }
                            }
                        }
                    }",
                @"{
                    people: {
                        items: [{
                            id: 1,
                            subordinates: [{
                                id: 2,
                                fullName: ""Sophie Gandley""
                            }]
                        },{
                            id: 2,
                            subordinates: []
                        }]
                    }
                }",
                null,
                null);
        }

        [Test(Description = "FromLoader(...)")]
        public void TestFromLoaderConnectionNullableIntToIntRelation()
        {
            var unitLoader = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.Name);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("unit")
                        .FromLoader<Unit>(unitLoader, (p, u) => p.UnitId == u.Id)
                        .AsConnection();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(1).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query t {
                        people {
                            items {
                                unit {
                                    items {
                                        id
                                        name
                                    }
                                }
                            }
                        }
                    }",
                @"{
                    people: {
                        items: [{
                            unit: {
                                items: [{
                                    id: 1,
                                    name: ""Alpha""
                                }]
                            }
                        }]
                    }
                }",
                null,
                null);
        }

        [Test]
        public void TestFromLoaderConnectionIntToIntRelation()
        {
            Type personLoaderType = null;
            personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
                    loader.Field("twin")
                        .FromLoader<Person>(personLoaderType, (p1, p2) => p1.Id == p2.Id)
                        .AsConnection();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(1).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query t {
                        people {
                            items {
                                twin {
                                    items {
                                        id
                                        fullName
                                    }
                                }
                            }
                        }
                    }",
                @"{
                    people: {
                        items: [{
                            twin: {
                                items: [{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            }
                        }]
                    }
                }",
                null,
                null);
        }

        [Test]
        public void TestFromLoaderConnectionIntToNullableIntRelation()
        {
            Type personLoaderType = null;
            personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
                    loader.Field("subordinates")
                        .FromLoader<Person>(personLoaderType, (p1, p2) => p1.Id == p2.ManagerId)
                        .AsConnection();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(2).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query t {
                        people {
                            items {
                                id
                                subordinates {
                                    items {
                                        id
                                        fullName
                                    }
                                }
                            }
                        }
                    }",
                @"{
                    people: {
                        items: [{
                            id: 1,
                            subordinates: {
                                items: [{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            }
                        },{
                            id: 2,
                            subordinates: {
                                items: []
                            }
                        }]
                    }
                }",
                null,
                null);
        }

        [Test(Description = "FromLoader(...)/WithFilter")]
        public void TestFromLoaderWithFilterNullableIntToIntRelation()
        {
            var applyFilter = Substitute.For<Func<TestUserContext, IQueryable<Person>, PersonFilter, IQueryable<Person>>>();

            applyFilter
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IQueryable<Person>>(), Arg.Any<PersonFilter>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<Person>>(1).Where(u => callInfo.ArgAt<PersonFilter>(2).Ids.Contains(u.Id)));

            var personFilter = GraphQLTypeBuilder.CreateLoaderFilterType(applyFilter);

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
                    loader.Field("manager")
                        .FromLoader<Person>(loader.GetType(), (p, m) => p.ManagerId == m.Id)
                        .AsConnection()
                        .WithFilter(personFilter);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query t {
                        people {
                            items {
                                id
                                manager(filter: {ids: [2]}) {
                                    items {
                                        id
                                    }
                                }
                            }
                        }
                    }",
                @"{
                    people: {
                        items: [{
                            id: 1,
                            manager: {
                                items: []
                            }
                        },{
                            id: 2,
                            manager: {
                                items: []
                            }
                        },{
                            id: 3,
                            manager: {
                                items: []
                            }
                        },{
                            id: 4,
                            manager: {
                                items: [{
                                    id: 2
                                }]
                            }
                        },{
                            id: 5,
                            manager: {
                                items: [{
                                    id: 2
                                }]
                            }
                        },{
                            id: 6,
                            manager: {
                                items: []
                            }
                        }]
                    }
                }",
                null,
                null);

            applyFilter
                .Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IQueryable<Person>>(), Arg.Any<PersonFilter>());
        }

        [Test(Description = "FromLoader(...)/WithFilter/Filterable")]
        public void TestFromLoaderWithFilterAndFilterable()
        {
            var applyFilter = Substitute.For<Func<TestUserContext, IQueryable<Person>, PersonFilter, IQueryable<Person>>>();

            applyFilter
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IQueryable<Person>>(), Arg.Any<PersonFilter>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<Person>>(1).Where(u => callInfo.ArgAt<PersonFilter>(2).Ids.Contains(u.Id)));

            var personFilter = GraphQLTypeBuilder.CreateLoaderFilterType(applyFilter);

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName).Filterable();
                    loader.Field("manager")
                        .FromLoader<Person>(loader.GetType(), (p, m) => p.ManagerId == m.Id)
                        .AsConnection()
                        .WithFilter(personFilter);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQueryError(
                Builder,
                typeof(NotSupportedException),
                "Person: Simultaneous use of .WithFilter() and .Filterable() is not supported. Consider to use either .WithFilter() or .Filterable().",
                @"
                query t {
                    people {
                        items {
                            id
                            manager(filter: {ids: [2]}) {
                                items {
                                    id
                                }
                            }
                        }
                    }
                }");
        }

        [Test(Description = "FromLoader(...)/WithFilter/WithSearch")]
        public void TestFromLoaderWithFilterWithSearch()
        {
            var applySearch = Substitute.For<Func<IQueryable<Person>, TestUserContext, string, IQueryable<Person>>>();

            applySearch
                .Invoke(Arg.Any<IQueryable<Person>>(), Arg.Any<TestUserContext>(), Arg.Any<string>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<Person>>(0).Where(u => u.FullName.Contains(callInfo.ArgAt<string>(2))));

            var applyFilter = Substitute.For<Func<TestUserContext, IQueryable<Person>, PersonFilter, IQueryable<Person>>>();

            applyFilter
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IQueryable<Person>>(), Arg.Any<PersonFilter>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<Person>>(1).Where(u => callInfo.ArgAt<PersonFilter>(2).Ids.Contains(u.Id)));

            var personFilter = GraphQLTypeBuilder.CreateLoaderFilterType(applyFilter);
            var personSearch = GraphQLTypeBuilder.CreateSearcherType(applySearch);

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
                    loader.Field("manager")
                        .FromLoader<Person>(loader.GetType(), (p, m) => p.ManagerId == m.Id)
                        .AsConnection()
                        .WithFilter(personFilter)
                        .WithSearch(personSearch);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query t {
                        people {
                            items {
                                id
                                manager(filter: {ids: [1, 2]}, search: ""a"") {
                                    items {
                                        id
                                    }
                                }
                            }
                        }
                    }",
                @"{
                    people: {
                        items: [{
                            id: 1,
                            manager: {
                                items: []
                            }
                        },{
                            id: 2,
                            manager: {
                                items: []
                            }
                        },{
                            id: 3,
                            manager: {
                                items: []
                            }
                        },{
                            id: 4,
                            manager: {
                                items: [{
                                    id: 2
                                }]
                            }
                        },{
                            id: 5,
                            manager: {
                                items: [{
                                    id: 2
                                }]
                            }
                        },{
                            id: 6,
                            manager: {
                                items: []
                            }
                        }]
                    }
                }",
                null,
                null);

            applyFilter
                .Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IQueryable<Person>>(), Arg.Any<PersonFilter>());

            applySearch
                .Received(1)
                .Invoke(Arg.Any<IQueryable<Person>>(), Arg.Any<TestUserContext>(), Arg.Any<string>());
        }

        [Test(Description = "FromLoader(...)/WithFilter/Sortable")]
        public void TestFromLoaderWithFilterSortable()
        {
            var applyFilter = Substitute.For<Func<TestUserContext, IQueryable<Person>, PersonFilter, IQueryable<Person>>>();

            applyFilter
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IQueryable<Person>>(), Arg.Any<PersonFilter>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<Person>>(1).Where(u => callInfo.ArgAt<PersonFilter>(2).Ids.Contains(u.Id)));

            var personFilter = GraphQLTypeBuilder.CreateLoaderFilterType(applyFilter);

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id).Sortable();
                    loader.Field(p => p.FullName);
                    loader.Field("subordinates")
                        .FromLoader<Person>(loader.GetType(), (m, p) => p.ManagerId == m.Id)
                        .AsConnection()
                        .WithFilter(personFilter);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query t {
                        people {
                            items {
                                id
                                subordinates(filter: {ids: [2, 3, 4, 5]}, sorting: [{field: ""id"", direction: DESC}]) {
                                    items {
                                        id
                                    }
                                }
                            }
                        }
                    }",
                @"{
                    people: {
                        items: [{
                            id: 1,
                            subordinates: {
                                items: [{
                                    id: 3
                                },{
                                    id: 2
                                }]
                            }
                        },{
                            id: 2,
                            subordinates: {
                                items: [{
                                    id: 5
                                },{
                                    id: 4
                                }]
                            }
                        },{
                            id: 3,
                            subordinates: {
                                items: []
                            }
                        },{
                            id: 4,
                            subordinates: {
                                items: []
                            }
                        },{
                            id: 5,
                            subordinates: {
                                items: []
                            }
                        },{
                            id: 6,
                            subordinates: {
                                items: []
                            }
                        }]
                    }
                }",
                null,
                null);

            applyFilter
                .Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IQueryable<Person>>(), Arg.Any<PersonFilter>());
        }

        [Test(Description = "FromLoader(...)/WithFilter/WithSearch/Sortable")]
        public void TestFromLoaderWithFilterWithSearchSortable()
        {
            var applySearch = Substitute.For<Func<IQueryable<Person>, TestUserContext, string, IQueryable<Person>>>();

            applySearch
                .Invoke(Arg.Any<IQueryable<Person>>(), Arg.Any<TestUserContext>(), Arg.Any<string>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<Person>>(0).Where(u => u.FullName.Contains(callInfo.ArgAt<string>(2))));

            var applyFilter = Substitute.For<Func<TestUserContext, IQueryable<Person>, PersonFilter, IQueryable<Person>>>();

            applyFilter
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IQueryable<Person>>(), Arg.Any<PersonFilter>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<Person>>(1).Where(u => callInfo.ArgAt<PersonFilter>(2).Ids.Contains(u.Id)));

            var personFilter = GraphQLTypeBuilder.CreateLoaderFilterType(applyFilter);
            var personSearch = GraphQLTypeBuilder.CreateSearcherType(applySearch);

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id).Sortable();
                    loader.Field(p => p.FullName);
                    loader.Field("subordinates")
                        .FromLoader<Person>(loader.GetType(), (m, p) => p.ManagerId == m.Id)
                        .AsConnection()
                        .WithFilter(personFilter)
                        .WithSearch(personSearch);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query t {
                        people {
                            items {
                                id
                                subordinates(filter: {ids: [2, 3, 4, 5]}, search: ""a"", sorting:[{field: ""id"", direction: DESC}]) {
                                    items {
                                        id
                                    }
                                }
                            }
                        }
                    }",
                @"{
                    people: {
                        items: [{
                            id: 1,
                            subordinates: {
                                items: [{
                                    id: 3
                                },{
                                    id: 2
                                }]
                            }
                        },{
                            id: 2,
                            subordinates: {
                                items: [{
                                    id: 4
                                }]
                            }
                        },{
                            id: 3,
                            subordinates: {
                                items: []
                            }
                        },{
                            id: 4,
                            subordinates: {
                                items: []
                            }
                        },{
                            id: 5,
                            subordinates: {
                                items: []
                            }
                        },{
                            id: 6,
                            subordinates: {
                                items: []
                            }
                        }]
                    }
                }",
                null,
                null);

            applyFilter
                .Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IQueryable<Person>>(), Arg.Any<PersonFilter>());

            applySearch
                .Received(1)
                .Invoke(Arg.Any<IQueryable<Person>>(), Arg.Any<TestUserContext>(), Arg.Any<string>());
        }

        [Test(Description = "FromLoader(...)/Filterable()")]
        public void TestFromLoaderFilterable()
        {
            var unitLoader = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id).Filterable();
                    loader.Field(u => u.Name);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("unit")
                        .FromLoader<Unit>(unitLoader, (p, u) => p.UnitId == u.Id)
                        .AsConnection();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query t {
                        people {
                            items {
                                id
                                unit(filter: {id: {in: [2]} }) {
                                    items {
                                        id
                                    }
                                }
                            }
                        }
                    }",
                @"{
                    people: {
                        items: [{
                            id: 1,
                            unit: {
                                items: []
                            }
                        },{
                            id: 2,
                            unit: {
                                items: []
                            }
                        },{
                            id: 3,
                            unit: {
                                items: []
                            }
                        },{
                            id: 4,
                            unit: {
                                items: [{
                                    id: 2
                                }]
                            }
                        },{
                            id: 5,
                            unit: {
                                items: [{
                                    id: 2
                                }]
                            }
                        },{
                            id: 6,
                            unit: {
                                items: [{
                                    id: 2
                                }]
                            }
                        }]
                    }
                }",
                null,
                null);
        }

        [Test(Description = "FromLoader(...)/Filterable()/Sortable()")]
        public void TestFromLoaderFilterableSortable()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id).Filterable().Sortable();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            var unitLoader = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("people")
                        .FromLoader<Person>(personLoaderType, (u, p) => u.Id == p.UnitId)
                        .AsConnection();
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
                    query t {
                        units {
                            items {
                                id
                                people(filter: {id: {in: [1, 3, 4, 5]} }, sorting: {field: ""id"", direction: DESC}) {
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
                            id: 1,
                            people: {
                                items: [{
                                    id: 3
                                },{
                                    id: 1
                                }]
                            }
                        },{
                            id: 2,
                            people: {
                                items: [{
                                    id: 5
                                },{
                                    id: 4
                                }]
                            }
                        }]
                    }
                }",
                null,
                null);
        }

        [Test]
        public void TestFromLoaderWithFilterIntToIntRelation()
        {
            var personFilter = GraphQLTypeBuilder.CreateLoaderFilterType<Person, PersonFilter, TestUserContext>((ctx, q, filter) =>
            {
                return q.Where(u => filter.Ids.Contains(u.Id));
            });

            Type personLoaderType = null;
            personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
                    loader.Field("twin")
                        .FromLoader<Person>(personLoaderType, (p1, p2) => p1.Id == p2.Id)
                        .AsConnection()
                        .WithFilter(personFilter);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(2).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query t {
                        people {
                            items {
                                id
                                twin(filter: {ids: [1]}) {
                                    items {
                                        id
                                        fullName
                                    }
                                }
                            }
                        }
                    }",
                @"{
                    people: {
                        items: [{
                            id: 1,
                            twin: {
                                items: [{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            }
                        },{
                            id: 2,
                            twin: {
                                items: []
                            }
                        }]
                    }
                }",
                null,
                null);
        }

        [Test]
        public void TestFromLoaderWithFilterIntToNullableIntRelation()
        {
            var personFilter = GraphQLTypeBuilder.CreateLoaderFilterType<Person, PersonFilter, TestUserContext>((ctx, q, filter) =>
            {
                return q.Where(u => filter.Ids.Contains(u.Id));
            });

            Type personLoaderType = null;
            personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
                    loader.Field("subordinates")
                        .FromLoader<Person>(personLoaderType, (p1, p2) => p1.Id == p2.ManagerId)
                        .AsConnection()
                        .WithFilter(personFilter);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(3).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query t {
                        people {
                            items {
                                id
                                subordinates(filter: {ids: [2]}) {
                                    items {
                                        id
                                        fullName
                                    }
                                }
                            }
                        }
                    }",
                @"{
                    people: {
                        items: [{
                            id: 1,
                            subordinates: {
                                items: [{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            }
                        },{
                            id: 2,
                            subordinates: {
                                items: []
                            }
                        },{
                            id: 3,
                            subordinates: {
                                items: []
                            }
                        }]
                    }
                }",
                null,
                null);
        }

        [Test(Description = "FromLoader(...)/SingleOrDefault()")]
        public void TestFromLoaderWithSingleOrDefault()
        {
            var unitLoader = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.Name);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("unit")
                        .FromLoader<Unit>(unitLoader, (p, u) => p.UnitId == u.Id)
                        .SingleOrDefault();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(1).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(Builder, "query t { people { items { unit { id name } } } }", "{ people: { items: [{ unit: { id: 1, name: \"Alpha\" } }] } }", null, null);
        }

        [Test]
        public void TestFromLoaderWithSingleOrDefaultExpression()
        {
            var unitLoader = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.Name);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("unit")
                        .FromLoader<Unit>(unitLoader, (p, u) => p.UnitId == u.Id)
                        .SingleOrDefault(u => u.Id > 0);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(1).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(Builder, "query t { people { items { unit { id name } } } }", "{ people: { items: [{ unit: { id: 1, name: \"Alpha\" } }] } }", null, null);
        }

        [Test(Description = "FromLoader(...)/FirstOrDefault()")]
        public void TestFromLoaderWithFirstOrDefault()
        {
            var unitLoader = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.Name);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => Enumerable.Repeat(FakeData.Alpha, 2).AsQueryable());

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("unit")
                        .FromLoader<Unit>(unitLoader, (p, u) => p.UnitId == u.Id)
                        .FirstOrDefault();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(1).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(Builder, "query t { people { items { unit { id name } } } }", "{ people: { items: [{ unit: { id: 1, name: \"Alpha\" } }] } }", null, null);
        }

        [Test]
        public void TestFromLoaderWithFirstOrDefaultExpression()
        {
            var unitLoader = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.Name);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => Enumerable.Repeat(FakeData.Alpha, 2).AsQueryable());

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("unit")
                        .FromLoader<Unit>(unitLoader, (p, u) => p.UnitId == u.Id)
                        .FirstOrDefault(u => u.Id > 0);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(1).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(Builder, "query t { people { items { unit { id name } } } }", "{ people: { items: [{ unit: { id: 1, name: \"Alpha\" } }] } }", null, null);
        }

        [Test(Description = "FromLoader(...)/SingleOrDefault() - self reference int prop")]
        public void TestFromLoaderWithSingleOrDefaultAndSelfReferenceIntProp()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateIdentifiableLoaderType<Person, int, TestUserContext>(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("manager")
                        .FromLoader<Person>(loader.GetType(), (p, m) => p.ManagerId == m.Id)
                        .SingleOrDefault();
                },
                _ => FakeData.People.AsQueryable().Where(p => p.Id <= 2),
                p => p.Id);

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
                }
                ", @"
                {
                    people: {
                        items: [{
                            id: 1,
                            manager: null
                        }, {
                            id: 2,
                            manager: {
                                id: 1
                            }
                        }]
                    }
                }",
                null,
                null);
        }

        [Test(Description = "FromLoader(...)/SingleOrDefault() - self reference long prop")]
        public void TestFromLoaderWithSingleOrDefaultAndSelfReferenceLongProp()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateIdentifiableLoaderType<Person, long, TestUserContext>(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("manager")
                        .FromLoader<Person>(loader.GetType(), (p, m) => p.LongManagerId == m.LongId)
                        .SingleOrDefault();
                },
                _ => FakeData.People.AsQueryable().Where(p => p.Id <= 2),
                p => p.LongId);

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
                    }
                ",
                @"
                {
                    people: {
                        items: [{
                            id: 1,
                            manager: null
                        }, {
                            id: 2,
                            manager: {
                                id: 1
                            }
                        }]
                    }
                }",
                null,
                null);
        }

        [Test(Description = "FromLoader(...) / FirstOrDefault() - metadata(without fk)")]
        public void TestFromLoaderMetadataWithoutFK()
        {
            var unitLoader = GraphQLTypeBuilder.CreateIdentifiableLoaderType<Unit, int, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.Name);
                },
                getBaseQuery: _ => FakeData.Units.AsQueryable(),
                idExpression: p => p.Id);

            var personLoaderType = GraphQLTypeBuilder.CreateIdentifiableLoaderType<Person, int, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.UnitId);
                    loader.Field("unit")
                        .FromLoader<Unit>(unitLoader, (p, u) => p.UnitId == u.Id)
                        .FirstOrDefault();
                },
                getBaseQuery: _ => FakeData.People.AsQueryable(),
                idExpression: p => p.Id);

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        __type(name: ""Person"") {
                            name
                            metadata {
                                primaryKey {
                                    name
                                }
                                foreignKeys {
                                    fromField {
                                        name
                                    }
                                    toType {
                                        name
                                    }
                                    toField {
                                        name
                                    }
                                }
                            }
                        }
                    }
                ", @"
                {
                    __type: {
                        name: ""Person"",
                        metadata: {
                            primaryKey: [{
                                name: ""id""
                            }],
                            foreignKeys: [{
                                fromField: [{
                                    name: ""unitId""
                                }],
                                toType: {
                                    name: ""Unit""
                                },
                                toField: [{
                                    name: ""id""
                                }]
                            }]
                        }
                    }
                }");
        }

        [Test(Description = "FromLoader(...)/FirstOrDefault() - metadata")]
        public void TestFromLoaderMetadata()
        {
            var unitLoader = GraphQLTypeBuilder.CreateIdentifiableLoaderType<Unit, int, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.Name);
                },
                getBaseQuery: _ => FakeData.Units.AsQueryable(),
                idExpression: p => p.Id);

            var personLoaderType = GraphQLTypeBuilder.CreateIdentifiableLoaderType<Person, int, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.UnitId);
                    loader.Field("unit")
                        .FromLoader<Unit>(unitLoader, (p, u) => p.UnitId == u.Id)
                        .FirstOrDefault();
                },
                getBaseQuery: _ => FakeData.People.AsQueryable(),
                idExpression: p => p.Id);

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        __type(name: ""Person"") {
                            name
                            metadata {
                                primaryKey {
                                    name
                                }
                                foreignKeys {
                                    fromField {
                                        name
                                    }
                                    toType {
                                        name
                                    }
                                    toField {
                                        name
                                    }
                                }
                            }
                        }
                    }
                ", @"
                {
                    __type: {
                        name: ""Person"",
                        metadata: {
                            primaryKey: [{
                                name: ""id""
                            }],
                            foreignKeys: [{
                                fromField: [{
                                    name: ""unitId""
                                }],
                                toType: {
                                    name: ""Unit""
                                },
                                toField: [{
                                    name: ""id""
                                }]
                            }]
                        }
                    }
                }");
        }

        [Test(Description = "FromLoader(...)/FirstOrDefault() - metadata (self ref without fk)")]
        public void TestFromLoaderMetadataSelfRefWithoutFK()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateIdentifiableLoaderType<Person, int, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.ManagerId);
                    loader.Field("manager")
                        .FromLoader<Person>(loader.GetType(), (p, m) => p.ManagerId == m.Id)
                        .FirstOrDefault();
                },
                getBaseQuery: _ => FakeData.People.AsQueryable(),
                idExpression: p => p.Id);

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        __type(name: ""Person"") {
                            name
                            metadata {
                                primaryKey {
                                    name
                                }
                                foreignKeys {
                                    fromField {
                                        name
                                    }
                                    toType {
                                        name
                                    }
                                    toField {
                                        name
                                    }
                                }
                            }
                        }
                    }
                ", @"
                {
                    __type: {
                        name: ""Person"",
                        metadata: {
                            primaryKey: [{
                                name: ""id""
                            }],
                            foreignKeys: [{
                                fromField: [{
                                    name: ""managerId""
                                }],
                                toType: {
                                    name: ""Person""
                                },
                                toField: [{
                                    name: ""id""
                                }]
                            }]
                        }
                    }
                }");
        }

        [Test(Description = "FromLoader(...)/FirstOrDefault() - metadata (self ref)")]
        public void TestFromLoaderMetadataSelfRef()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateIdentifiableLoaderType<Person, int, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.ManagerId);
                    loader.Field("manager")
                        .FromLoader<Person>(loader.GetType(), (p, m) => p.ManagerId == m.Id)
                        .FirstOrDefault();
                },
                getBaseQuery: _ => FakeData.People.AsQueryable(),
                idExpression: p => p.Id);

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        __type(name: ""Person"") {
                            name
                            metadata {
                                primaryKey {
                                    name
                                }
                                foreignKeys {
                                    fromField {
                                        name
                                    }
                                    toType {
                                        name
                                    }
                                    toField {
                                        name
                                    }
                                }
                            }
                        }
                    }", @"
                    {
                        __type: {
                            name: ""Person"",
                            metadata: {
                                primaryKey: [{
                                    name: ""id""
                                }],
                                foreignKeys: [{
                                    fromField: [{
                                        name: ""managerId""
                                    }],
                                    toType: {
                                        name: ""Person""
                                    },
                                    toField: [{
                                        name: ""id""
                                    }]
                                }]
                            }
                        }
                    }");
        }

        public class PersonFilter : Input
        {
            public List<int> Ids { get; set; }
        }

        public class UnitFilter : Input
        {
            public List<int> Ids { get; set; }
        }
    }
}
