// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Collections.Generic;
using System.Linq;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Sorting
{
    [TestFixture]
    public class Issue10Tests : BaseTests
    {
        [Test]
        public void DifferentSortingFields()
        {
            var firstPersonLoader = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName).Sortable();
                },
                getBaseQuery: _ => FakeData.People.AsQueryable());
            var secondPersonLoader = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.Salary).Sortable();
                },
                getBaseQuery: _ => FakeData.People.AsQueryable());

            TestHelpers.TestQuery(
                query =>
                {
                    query.Field("firstPeople").FromLoader<Person, TestUserContext>(firstPersonLoader);
                    query.Field("secondPeople").FromLoader<Person, TestUserContext>(secondPersonLoader);
                },
                @"
                query test {
                    firstPeople(sorting: { field: ""fullName""}) {
                        id
                    }
                    secondPeople(sorting: { field: ""salary""}) {
                        id
                    }
                }",
                @"{
                    firstPeople: [{
                        id: 5
                    },{
                        id: 4
                    },{
                        id: 3
                    },{
                        id: 1
                    },{
                        id: 2
                    },{
                        id: 6
                    }],
                    secondPeople: [{
                        id: 4
                    },{
                        id: 3
                    },{
                        id: 2
                    },{
                        id: 5
                    },{
                        id: 6
                    },{
                        id: 1
                    }]
                }");
        }

        [Test]
        public void DifferentSortingFieldsWithVariables()
        {
            var firstPersonLoader = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName).Sortable();
                },
                getBaseQuery: _ => FakeData.People.AsQueryable());
            var secondPersonLoader = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.Salary).Sortable();
                },
                getBaseQuery: _ => FakeData.People.AsQueryable());

            TestHelpers.TestQuery(
                query =>
                {
                    query.Field("firstPeople").FromLoader<Person, TestUserContext>(firstPersonLoader);
                    query.Field("secondPeople").FromLoader<Person, TestUserContext>(secondPersonLoader);
                },
                @"
                query test($firstSorting: [PersonSortingOption], $secondSorting: [Person1SortingOption]) {
                    firstPeople(sorting: $firstSorting) {
                        id
                    }
                    secondPeople(sorting: $secondSorting) {
                        id
                    }
                }",
                @"{
                    firstPeople: [{
                        id: 5
                    },{
                        id: 4
                    },{
                        id: 3
                    },{
                        id: 1
                    },{
                        id: 2
                    },{
                        id: 6
                    }],
                    secondPeople: [{
                        id: 4
                    },{
                        id: 3
                    },{
                        id: 2
                    },{
                        id: 5
                    },{
                        id: 6
                    },{
                        id: 1
                    }]
                }",
                executionOptionsBuilder: builder => builder.WithVariables(new Dictionary<string, object>
                {
                    ["firstSorting"] = new[]
                    {
                        new Dictionary<string, object>
                        {
                            ["field"] = "fullName",
                        },
                    },
                    ["secondSorting"] = new[]
                    {
                        new Dictionary<string, object>
                        {
                            ["field"] = "salary",
                        },
                    },
                }));
        }
    }
}
