// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Sorting
{
    [TestFixture]
    public class CustomSortersTests : BaseTests
    {
        [Test]
        public void ShouldSortByCustomSortable()
        {
            var personLoader = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName).Sortable(p => p.Id);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable());

            TestHelpers.TestQuery(
                query => query.Field("people").FromLoader<Person, TestUserContext>(personLoader),
                @"
                query {
                    people(sorting: {field: ""fullName"", direction: DESC}) {
                        id
                    }
                }",
                @"{
                    people: [{
                        id: 6
                    },{
                        id: 5
                    },{
                        id: 4
                    },{
                        id: 3
                    },{
                        id: 2
                    },{
                        id: 1
                    }]
                }");
        }

        [Test]
        public void ShouldSortByCustomSortableWithContext()
        {
            var personLoader = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName).Sortable<int>(ctx => p => p.Id + ctx.UserId);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable());

            TestHelpers.TestQuery(
                query => query.Field("people").FromLoader<Person, TestUserContext>(personLoader),
                @"
                query {
                    people(sorting: {field: ""fullName"", direction: DESC}) {
                        id
                    }
                }",
                @"{
                    people: [{
                        id: 6
                    },{
                        id: 5
                    },{
                        id: 4
                    },{
                        id: 3
                    },{
                        id: 2
                    },{
                        id: 1
                    }]
                }");
        }

        [Test]
        public void ShouldSortByCustomSorter()
        {
            var personLoader = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
                    loader.Sorter("fullName", p => p.Id);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable());

            TestHelpers.TestQuery(
                query => query.Field("people").FromLoader<Person, TestUserContext>(personLoader),
                @"
                query {
                    people(sorting: {field: ""fullName"", direction: DESC}) {
                        id
                    }
                }",
                @"{
                    people: [{
                        id: 6
                    },{
                        id: 5
                    },{
                        id: 4
                    },{
                        id: 3
                    },{
                        id: 2
                    },{
                        id: 1
                    }]
                }");
        }

        [Test]
        public void ShouldSortByNullableCustomSorter()
        {
            var personLoader = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(nameof(Person.TerminationDate))
                        .Resolve((ctx, person) => person.TerminationDate);
                    loader.Sorter(nameof(Person.TerminationDate), person => person.TerminationDate ?? DateTime.MaxValue);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable());

            TestHelpers.TestQuery(
                query => query.Field("people").FromLoader<Person, TestUserContext>(personLoader),
                @"
                query {
                    people(sorting: {field: ""terminationDate""}) {
                        terminationDate
                    }
                }",
                @"{
                    people: [{
                        terminationDate: ""2017-02-19T00:00:00""
                    },{
                        terminationDate: ""2018-05-10T00:00:00""
                    },{
                        terminationDate: ""2019-10-01T00:00:00""
                    },{
                        terminationDate: null
                    },{
                        terminationDate: null
                    },{
                        terminationDate: null
                    }]
                }");
        }

        [Test]
        public void ShouldSortByCustomSorterWithContext()
        {
            var personLoader = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
                    loader.Sorter<int>("fullName", ctx => p => p.Id + ctx.UserId);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable());

            TestHelpers.TestQuery(
                query => query.Field("people").FromLoader<Person, TestUserContext>(personLoader),
                @"
                query {
                    people(sorting: {field: ""fullName"", direction: DESC}) {
                        id
                    }
                }",
                @"{
                    people: [{
                        id: 6
                    },{
                        id: 5
                    },{
                        id: 4
                    },{
                        id: 3
                    },{
                        id: 2
                    },{
                        id: 1
                    }]
                }");
        }
    }
}
