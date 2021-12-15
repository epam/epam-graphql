// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Collections.Generic;
using System.Linq;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Connection
{
    [TestFixture]
    public class GroupConnectionTests : BaseTests
    {
        [Test(Description = "GroupConnection(...)")]
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
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .GroupConnection(personLoaderType, "peopleGroups");
            }

            TestHelpers.TestQuery(
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
                }",
                null,
                null);
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
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .GroupConnection(personLoaderType, "peopleGroups");
            }

            TestHelpers.TestQuery(
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
                }",
                null,
                null);
        }

        [Test(Description = "GroupConnection(...)/Fragment")]
        public void TestGroupConnectionQueryItemsFragment()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.ManagerId).Groupable();
                    loader.Field(p => p.UnitId).Groupable();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .GroupConnection(personLoaderType, "peopleGroups");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        peopleGroups {
                            items {
                                item {
                                    ...person
                                }
                            }
                        }
                    }
                    fragment person on PersonGrouping {
                        unitId
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
                }",
                null,
                null);
        }

        [Test(Description = "GroupConnection(...)/Count")]
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
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .GroupConnection(personLoaderType, "peopleGroups");
            }

            TestHelpers.TestQuery(
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
                }",
                null,
                null);
        }

        [Test(Description = "GroupConnection(...)/Filterable")]
        public void TestGroupConnectionQueryItemsFilterable()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.ManagerId).Filterable().Groupable();
                    loader.Field(p => p.UnitId).Groupable();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .GroupConnection(personLoaderType, "peopleGroups");
            }

            TestHelpers.TestQuery(
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
                }",
                null,
                null);
        }

        [Test(Description = "GroupConnection(...)")]
        public void TestGroupConnectionQueryEdges()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.ManagerId).Groupable();
                    loader.Field(p => p.UnitId).Groupable();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .GroupConnection(personLoaderType, "peopleGroups");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        peopleGroups {
                            edges {
                                node {
                                    item {
                                        unitId
                                    }
                                }
                            }
                        }
                    }",
                @"{
                    peopleGroups: {
                        edges: [{
                            node: {
                                item: {
                                    unitId: 1
                                }
                            }
                        },{
                            node: {
                                item: {
                                    unitId: 2
                                }
                            }
                        }]
                    }
                }",
                null,
                null);
        }

        [Test(Description = "GroupConnection(...)/Fragment")]
        public void TestGroupConnectionQueryEdgesFragment()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.ManagerId).Groupable();
                    loader.Field(p => p.UnitId).Groupable();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .GroupConnection(personLoaderType, "peopleGroups");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        peopleGroups {
                            edges {
                                node {
                                    item {
                                        ...person
                                    }
                                }
                            }
                        }
                    }
                    fragment person on PersonGrouping {
                        unitId
                    }",
                @"{
                    peopleGroups: {
                        edges: [{
                            node: {
                                item: {
                                    unitId: 1
                                }
                            }
                        },{
                            node: {
                                item: {
                                    unitId: 2
                                }
                            }
                        }]
                    }
                }",
                null,
                null);
        }

        [Test(Description = "GroupConnection(...)/EdgeFragment")]
        public void TestGroupConnectionQueryEdgesEdgeFragment()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.ManagerId).Groupable();
                    loader.Field(p => p.UnitId).Groupable();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .GroupConnection(personLoaderType, "peopleGroups");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        peopleGroups {
                            edges {
                                ...person
                            }
                        }
                    }
                    fragment person on PersonGroupEdge {
                        node {
                            item {
                                unitId
                            }
                        }
                    }",
                @"{
                    peopleGroups: {
                        edges: [{
                            node: {
                                item: {
                                    unitId: 1
                                }
                            }
                        },{
                            node: {
                                item: {
                                    unitId: 2
                                }
                            }
                        }]
                    }
                }",
                null,
                null);
        }

        [Test(Description = "GroupConnection(...)/Count")]
        public void TestGroupConnectionQueryEdgesAndCount()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.ManagerId).Groupable();
                    loader.Field(p => p.UnitId).Groupable();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .GroupConnection(personLoaderType, "peopleGroups");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        peopleGroups {
                            edges {
                                node {
                                    item {
                                        unitId
                                    }
                                    count
                                }
                            }
                        }
                    }",
                @"{
                    peopleGroups: {
                        edges: [{
                            node: {
                                item: {
                                    unitId: 1
                                },
                                count: 3
                            }
                        },{
                            node: {
                                item: {
                                    unitId: 2
                                },
                                count: 3
                            }
                        }]
                    }
                }",
                null,
                null);
        }

        [Test(Description = "GroupConnection(...)")]
        public void TestGroupConnectionQueryBothItemsEdges()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.ManagerId).Groupable();
                    loader.Field(p => p.UnitId).Groupable();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .GroupConnection(personLoaderType, "peopleGroups");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        peopleGroups {
                            items {
                                item {
                                    unitId
                                }
                            }
                            edges {
                                node {
                                    item {
                                        unitId
                                    }
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
                        }],
                        edges: [{
                            node: {
                                item: {
                                    unitId: 1
                                }
                            }
                        },{
                            node: {
                                item: {
                                    unitId: 2
                                }
                            }
                        }]
                    }
                }",
                null,
                null);
        }

        [Test(Description = "GroupConnection(...)/Count")]
        public void TestGroupConnectionQueryBothItemsEdgesAndCount()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.ManagerId).Groupable();
                    loader.Field(p => p.UnitId).Groupable();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .GroupConnection(personLoaderType, "peopleGroups");
            }

            TestHelpers.TestQuery(
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
                            edges {
                                node {
                                    item {
                                        unitId
                                    }
                                    count
                                }
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
                        }],
                        edges: [{
                            node: {
                                item: {
                                    unitId: 1
                                },
                                count: 3
                            }
                        },{
                            node: {
                                item: {
                                    unitId: 2
                                },
                                count: 3
                            }
                        }]
                    }
                }",
                null,
                null);
        }

        [Test(Description = "Connection(...)/Calculated field")]
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
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .GroupConnection(personLoaderType, "peopleGroups");
            }

            TestHelpers.TestQuery(
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
                }",
                null,
                null);
        }

        [Test(Description = "Connection(...)/Calculated field/Same name")]
        public void TestGroupConnectionWithCalculatedFieldSameName()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.ManagerId).Groupable();
                    loader.Field("unitId", p => p.UnitId * 2).Groupable();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .GroupConnection(personLoaderType, "peopleGroups");
            }

            TestHelpers.TestQuery(
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
                                unitId: 2
                            },
                            count: 3
                        },{
                            item: {
                                unitId: 4
                            },
                            count: 3
                        }]
                    }
                }",
                null,
                null);
        }

        [Test(Description = "totalCount without arguments")]
        public void TestGroupConnectionTotalCountWithoutArguments()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.ManagerId).Groupable();
                    loader.Field(p => p.UnitId).Groupable();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .GroupConnection(personLoaderType, "peopleGroups");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        peopleGroups {
                            totalCount
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
                        totalCount: 2,
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
                }",
                null,
                null);
        }

        [Test(Description = "totalCount with arguments")]
        public void TestGroupConnectionTotalCountWithArguments()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.ManagerId).Groupable();
                    loader.Field(p => p.UnitId).Groupable();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .GroupConnection(personLoaderType, "peopleGroups");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        peopleGroups(first: 1, after: ""0"") {
                            totalCount
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
                        totalCount: 2,
                        items: [{
                            item: {
                                unitId: 2
                            },
                            count: 3
                        }]
                    }
                }",
                null,
                null);
        }

        [Test(Description = "Test fix for https://git.epam.com/epm-ppa/epam-graphql/-/issues/49")]
        public void TestIssue49()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.ManagerId).Groupable();
                    loader.Field(p => p.UnitId).Groupable();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .GroupConnection(personLoaderType, "peopleGroups");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        peopleGroups {
                            items {
                                item {
                                    unitId
                                    __typename
                                }
                                count
                                __typename
                            }
                            edges {
                                node {
                                    item {
                                        unitId
                                        __typename
                                    }
                                    count
                                    __typename
                                }
                                __typename
                            }
                            __typename
                        }
                    }",
                @"{
                    peopleGroups: {
                        items: [{
                            item: {
                                unitId: 1,
                                __typename: ""PersonGrouping""
                            },
                            count: 3,
                            __typename: ""PersonGroupResult""
                        },{
                            item: {
                                unitId: 2,
                                __typename: ""PersonGrouping""
                            },
                            count: 3,
                            __typename: ""PersonGroupResult""
                        }],
                        edges: [{
                            node: {
                                item: {
                                    unitId: 1,
                                    __typename: ""PersonGrouping""
                                },
                                count: 3,
                                __typename: ""PersonGroupResult""
                            },
                            __typename: ""PersonGroupEdge""
                        },{
                            node: {
                                item: {
                                    unitId: 2,
                                    __typename: ""PersonGrouping""
                                },
                                count: 3,
                                __typename: ""PersonGroupResult""
                            },
                            __typename: ""PersonGroupEdge""
                        }],
                        __typename: ""PersonGroupConnection""
                    }
                }",
                null,
                null);
        }

        [Test]
        public void TestGroupConnectionQueryItemsWithFilter()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.ManagerId).Groupable();
                    loader.Field(p => p.UnitId).Groupable();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            var filterType = GraphQLTypeBuilder.CreateLoaderFilterType<Person, PersonFilter, TestUserContext>((ctx, query, filter) => query.Where(p => filter.Ids.Contains(p.Id)));

            void Builder(Query<TestUserContext> query)
            {
                query
                    .GroupConnection(personLoaderType, "people")
                    .WithFilter(filterType);
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        people(filter: {
                            ids: [1, 4, 5]
                        }) {
                            items {
                                item {
                                    unitId
                                }
                                count
                            }
                        }
                    }",
                @"{
                    people: {
                        items: [{
                            item: {
                                unitId: 1
                            },
                            count: 1
                        }, {
                            item: {
                                unitId: 2
                            },
                            count: 2
                        }]
                    }
                }",
                null,
                null);
        }

        [Test]
        public void TestGroupConnectionQueryItemsWithSearch()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.ManagerId).Groupable();
                    loader.Field(p => p.UnitId).Groupable();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            var searchType = GraphQLTypeBuilder.CreateSearcherType<Person, TestUserContext>((query, ctx, search) => query.Where(p => p.FullName.Contains(search)));

            void Builder(Query<TestUserContext> query)
            {
                query
                    .GroupConnection(personLoaderType, "people")
                    .WithSearch(searchType);
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        people(search: ""i"") {
                            items {
                                item {
                                    unitId
                                }
                                count
                            }
                        }
                    }",
                @"{
                    people: {
                        items: [{
                            item: {
                                unitId: 1
                            },
                            count: 3
                        }, {
                            item: {
                                unitId: 2
                            },
                            count: 1
                        }]
                    }
                }",
                null,
                null);
        }

        public class PersonFilter : Input
        {
            public List<int> Ids { get; set; }
        }
    }
}
