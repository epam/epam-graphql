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

namespace Epam.GraphQL.Tests.Connection
{
    [TestFixture]
    public class ConnectionTests : BaseTests
    {
        [Test(Description = "Connection(...)")]
        public void TestConnectionQueryItems()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field(p => p.Id),
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
                            { id: 1 },
                            { id: 2 },
                            { id: 3 },
                            { id: 4 },
                            { id: 5 },
                            { id: 6 }
                        ]
                    }
                }",
                null,
                null);
        }

        [Test(Description = "Test for https://git.epam.com/epm-ppa/epam-graphql/-/issues/62")]
        public void TestConnectionQueryItemsDisableTracking()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field(p => p.Id),
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            var dataAdapter = Substitute.For<IDataContext>();
            dataAdapter.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<Proxy<Person>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<Proxy<Person>>>(0));

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
                            { id: 1 },
                            { id: 2 },
                            { id: 3 },
                            { id: 4 },
                            { id: 5 },
                            { id: 6 }
                        ]
                    }
                }",
                dataAdapter);

            dataAdapter.Received(1).QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<Proxy<Person>>>());
        }

        [Test(Description = "Connection(...)")]
        public void TestConnectionQueryItemsAndPageInfo()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field(p => p.Id),
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
                    query {
                        people {
                            items {
                                id
                            }
                            pageInfo {
                                hasNextPage
                                hasPreviousPage
                                startCursor
                                endCursor
                            }
                        }
                    }",
                @"{
                    people: {
                        items: [
                            { id: 1 },
                            { id: 2 },
                            { id: 3 },
                            { id: 4 },
                            { id: 5 },
                            { id: 6 }
                        ],
                        pageInfo: {
                            hasNextPage: false,
                            hasPreviousPage: false,
                            startCursor: ""0"",
                            endCursor: ""5""
                        }
                    }
                }",
                null,
                null);
        }

        [Test(Description = "Connection(...)/Fragment")]
        public void TestConnectionQueryItemsFragment()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field(p => p.Id),
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
                    query {
                        people {
                            items {
                                ...person
                            }
                        }
                    }
                    fragment person on Person {
                        id
                    }",
                @"{
                    people: {
                        items: [
                            { id: 1 },
                            { id: 2 },
                            { id: 3 },
                            { id: 4 },
                            { id: 5 },
                            { id: 6 }
                        ]
                    }
                }",
                null,
                null);
        }

        [Test(Description = "Connection(...)")]
        public void TestConnectionQueryEdges()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field(p => p.Id),
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
                    query {
                        people {
                            edges {
                                node {
                                    id
                                }
                            }
                        }
                    }",
                @"{
                    people: {
                        edges: [
                            { node: { id: 1 } },
                            { node: { id: 2 } },
                            { node: { id: 3 } },
                            { node: { id: 4 } },
                            { node: { id: 5 } },
                            { node: { id: 6 } }
                        ]
                    }
                }",
                null,
                null);
        }

        [Test(Description = "Connection(...)/Fragment")]
        public void TestConnectionQueryEdgesFragment()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field(p => p.Id),
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
                    query {
                        people {
                            edges {
                                node {
                                    ...person
                                }
                            }
                        }
                    }
                    fragment person on Person {
                        id
                    }",
                @"{
                    people: {
                        edges: [
                            { node: { id: 1 } },
                            { node: { id: 2 } },
                            { node: { id: 3 } },
                            { node: { id: 4 } },
                            { node: { id: 5 } },
                            { node: { id: 6 } }
                        ]
                    }
                }",
                null,
                null);
        }

        [Test(Description = "Connection(...)/EdgeFragment")]
        public void TestConnectionQueryEdgesEdgeFragment()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field(p => p.Id),
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
                    query {
                        people {
                            edges {
                                ...personEdge
                            }
                        }
                    }
                    fragment personEdge on PersonEdge {
                        node {
                            id
                        }
                    }",
                @"{
                    people: {
                        edges: [
                            { node: { id: 1 } },
                            { node: { id: 2 } },
                            { node: { id: 3 } },
                            { node: { id: 4 } },
                            { node: { id: 5 } },
                            { node: { id: 6 } }
                        ]
                    }
                }",
                null,
                null);
        }

        [Test(Description = "Connection(...)/EdgeFragment")]
        public void TestConnectionQueryEdgesEdgeFragmentRecursive()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field(p => p.Id),
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
                    query {
                        people {
                            edges {
                                ...personEdge
                            }
                        }
                    }
                    fragment person on Person {
                        id
                    }
                    fragment personEdge on PersonEdge {
                        node {
                            ...person
                        }
                    }",
                @"{
                    people: {
                        edges: [
                            { node: { id: 1 } },
                            { node: { id: 2 } },
                            { node: { id: 3 } },
                            { node: { id: 4 } },
                            { node: { id: 5 } },
                            { node: { id: 6 } }
                        ]
                    }
                }",
                null,
                null);
        }

        [Test(Description = "Connection(...)")]
        public void TestConnectionQueryBothItemsEdges()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.ManagerId);
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
                            }
                            edges {
                                node {
                                    managerId
                                }
                            }
                        }
                    }",
                @"{
                    people: {
                        items: [
                            { id: 1 },
                            { id: 2 },
                            { id: 3 },
                            { id: 4 },
                            { id: 5 },
                            { id: 6 }
                        ],
                        edges: [
                            { node: { managerId: null } },
                            { node: { managerId: 1 } },
                            { node: { managerId: 1 } },
                            { node: { managerId: 2 } },
                            { node: { managerId: 2 } },
                            { node: { managerId: 5 } }
                        ]
                    }
                }",
                null,
                null);
        }

        [Test(Description = "Connection(...)/Calculated field")]
        public void TestConnectionWithCalculatedField()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field("id", p => 2 * p.Id),
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
                                }
                            }
                        }",
                @"{
                            people: {
                                items: [
                                    { id: 2 },
                                    { id: 4 },
                                    { id: 6 },
                                    { id: 8 },
                                    { id: 10 },
                                    { id: 12 }
                                ]
                            }
                        }",
                null,
                null);
        }

        [Test]
        public void TestConnectionWithRenamedField()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field("identity", p => p.Id),
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
                                    identity
                                }
                            }
                        }",
                @"{
                            people: {
                                items: [
                                    { identity: 1 },
                                    { identity: 2 },
                                    { identity: 3 },
                                    { identity: 4 },
                                    { identity: 5 },
                                    { identity: 6 }
                                ]
                            }
                        }",
                null,
                null);
        }

        [Test(Description = "totalCount without arguments")]
        public void TestConnectionTotalCountWithoutArguments()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
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
                    query t {
                        people {
                            totalCount
                            items {
                                id
                                fullName
                            }
                        }
                    }",
                @"{
                        people: {
                            totalCount: 6,
                            items: [
                            {
                                id: 1,
                                fullName: ""Linoel Livermore""
                            },
                            {
                                id: 2,
                                fullName: ""Sophie Gandley""
                            },
                            {
                                id: 3,
                                fullName: ""Hannie Everitt""
                            },
                            {
                                id: 4,
                                fullName: ""Florance Goodricke""
                            },
                            {
                                id: 5,
                                fullName: ""Aldon Exley""
                            },
                            {
                                id: 6,
                                fullName: ""Walton Alvarez""
                            }]
                        }
                    }",
                null,
                null);
        }

        [Test(Description = "totalCount with arguments")]
        public void TestConnectionTotalCountWithArguments()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
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
                    query t {
                        people(first: 1, after: ""1"") {
                            totalCount
                            items {
                                id
                                fullName
                            }
                        }
                    }",
                @"{
                        people: {
                            totalCount: 6,
                            items: [
                            {
                                id: 3,
                                fullName: ""Hannie Everitt""
                            }]
                        }
                    }",
                null,
                null);
        }

        [Test]
        public void TestConnectionQueryItemsWithFilter()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field(p => p.Id),
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            var filterType = GraphQLTypeBuilder.CreateLoaderFilterType<Person, PersonFilter, TestUserContext>((ctx, query, filter) => query.Where(p => filter.Ids.Contains(p.Id)));

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people")
                    .WithFilter(filterType);
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        people(filter: {
                            ids: [1, 2, 3]
                        }) {
                            items {
                                id
                            }
                        }
                    }",
                @"{
                    people: {
                        items: [
                            { id: 1 },
                            { id: 2 },
                            { id: 3 }
                        ]
                    }
                }",
                null,
                null);
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
