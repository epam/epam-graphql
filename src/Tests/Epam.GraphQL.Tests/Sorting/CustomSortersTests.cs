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
        public void ShouldShouldSortByCustomSorter()
        {
            var personLoader = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName).Sortable(p => p.Id);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable(),
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id));

            TestHelpers.TestQuery(
                query => query.Connection(personLoader, "people"),
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
                        items: [{
                            id: 1
                        },{
                            id: 2
                        },{
                            id: 3
                        },{
                            id: 4
                        },{
                            id: 5
                        },{
                            id: 6
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldShouldSortByNullableCustomSorter()
        {
            var personLoader = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(nameof(Person.TerminationDate))
                        .Resolve((ctx, person) => person.TerminationDate);
                    loader.Sorter(nameof(Person.TerminationDate), person => person.TerminationDate ?? DateTime.MaxValue);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable(),
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id));

            TestHelpers.TestQuery(
                query => query.Connection(personLoader, "people"),
                @"
                query {
                    people(sorting: {field: ""terminationDate""}) {
                        items {
                            terminationDate
                        }
                    }
                }",
                @"{
                    people: {
                        items: [{
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
                    }
                }");
        }
    }
}
