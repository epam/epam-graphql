// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Tests.Common;
using Epam.GraphQL.Tests.Common.Configs;
using Epam.GraphQL.Tests.TestData;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Sorting
{
    [TestFixture(PeopleQueryType.PeopleWithSearch, LoaderType.Loader, PersonLoaderConfigurationType.PersonFilterableAndSortable)]
    [TestFixture(PeopleQueryType.PeopleWithSearch, LoaderType.MutableLoader, PersonLoaderConfigurationType.PersonFilterableAndSortable)]
    [TestFixture(PeopleQueryType.PeopleWithSearch, LoaderType.IdentifiableLoader, PersonLoaderConfigurationType.PersonSortable)]
    [TestFixture(PeopleQueryType.PeopleWithSearch, LoaderType.MutableLoader, PersonLoaderConfigurationType.PersonSortable)]
    [TestFixture(PeopleQueryType.PeopleWithFilterAndSearch, LoaderType.Loader, PersonLoaderConfigurationType.PersonSortable)]
    [TestFixture(PeopleQueryType.PeopleWithFilterAndSearch, LoaderType.MutableLoader, PersonLoaderConfigurationType.PersonSortable)]
    [TestFixture(PeopleQueryType.PeopleWithFilterAndOrderedSearch, LoaderType.IdentifiableLoader, PersonLoaderConfigurationType.PersonSortable)]
    [TestFixture(PeopleQueryType.PeopleWithFilterAndOrderedSearch, LoaderType.MutableLoader, PersonLoaderConfigurationType.PersonSortable)]
    public class SortTests : BaseTests<Person, PeopleQueryConfigurator, PeopleQueryType, PersonLoaderTypeCreator, PersonLoaderConfigurator, PersonLoaderConfigurationType>
    {
        public SortTests(PeopleQueryType queryType, LoaderType loaderType, PersonLoaderConfigurationType loaderConfiguration)
            : base(queryType, loaderType, loaderConfiguration)
        {
        }

        [Test]
        public void ShouldSortEntitiesInNaturalOrderWithoutSorting()
        {
            TestQuery(
                @"
                query {
                    people {
                        items {
                            id
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            id: 1
                        }, {
                            id: 2
                        }, {
                            id: 3
                        }, {
                            id: 4
                        }, {
                            id: 5
                        }, {
                            id: 6
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldBeAbleToSortByStringField()
        {
            TestQuery(
                @"
                query {
                    people(sorting: [{
                        field: ""fullName""
                    }]) {
                        items {
                            fullName
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            fullName: ""Aldon Exley""
                        }, {
                            fullName: ""Florance Goodricke""
                        }, {
                            fullName: ""Hannie Everitt""
                        }, {
                            fullName: ""Linoel Livermore""
                        }, {
                            fullName: ""Sophie Gandley""
                        }, {
                            fullName: ""Walton Alvarez""
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldBeAbleToSortByIntFieldAsc()
        {
            TestQuery(
                @"
                query {
                    people(sorting: [{
                        field: ""id"",
                        direction: ASC
                    }]) {
                        items {
                            id
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            id: 1
                        }, {
                            id: 2
                        }, {
                            id: 3
                        }, {
                            id: 4
                        }, {
                            id: 5
                        }, {
                            id: 6
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldBeAbleToSortByIntFieldAscWithContext()
        {
            TestQuery(
                @"
                query {
                    people(sorting: [{
                        field: ""ctxId"",
                        direction: ASC
                    }]) {
                        items {
                            id
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            id: 1
                        }, {
                            id: 2
                        }, {
                            id: 3
                        }, {
                            id: 4
                        }, {
                            id: 5
                        }, {
                            id: 6
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldBeAbleToSortByIntFieldAscDirectionAsString()
        {
            TestQuery(
                @"
                query {
                    people(sorting: [{
                        field: ""id"",
                        direction: ""asc""
                    }]) {
                        items {
                            id
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            id: 1
                        }, {
                            id: 2
                        }, {
                            id: 3
                        }, {
                            id: 4
                        }, {
                            id: 5
                        }, {
                            id: 6
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldBeAbleToSortByIntFieldDesc()
        {
            TestQuery(
                @"
                query {
                    people(sorting: [{
                        field: ""id"",
                        direction: DESC
                    }]) {
                        items {
                            id
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            id: 6
                        }, {
                            id: 5
                        }, {
                            id: 4
                        }, {
                            id: 3
                        }, {
                            id: 2
                        }, {
                            id: 1
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldBeAbleToSortByIntFieldDescDirectionAsString()
        {
            TestQuery(
                @"
                query {
                    people(sorting: [{
                        field: ""id"",
                        direction: ""desc""
                    }]) {
                        items {
                            id
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            id: 6
                        }, {
                            id: 5
                        }, {
                            id: 4
                        }, {
                            id: 3
                        }, {
                            id: 2
                        }, {
                            id: 1
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldBeAbleToSortByNullableIntFieldAscAndThenByStringFieldAsc()
        {
            TestQuery(
                @"
                query {
                    people(sorting: [{
                        field: ""managerId"",
                        direction: ASC
                    }, {
                        field: ""fullName"",
                        direction: ASC
                    }]) {
                        items {
                            managerId,
                            fullName
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            managerId: null,
                            fullName: ""Linoel Livermore""
                        }, {
                            managerId: 1,
                            fullName: ""Hannie Everitt""
                        }, {
                            managerId: 1,
                            fullName: ""Sophie Gandley""
                        }, {
                            managerId: 2,
                            fullName: ""Aldon Exley""
                        }, {
                            managerId: 2,
                            fullName: ""Florance Goodricke""
                        }, {
                            managerId: 5,
                            fullName: ""Walton Alvarez""
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldBeAbleToSortByNullableIntFieldDescAndThenByStringFieldAsc()
        {
            TestQuery(
                @"
                query {
                    people(sorting: [{
                        field: ""managerId"",
                        direction: DESC
                    }, {
                        field: ""fullName"",
                        direction: ASC
                    }]) {
                        items {
                            managerId,
                            fullName
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            managerId: 5,
                            fullName: ""Walton Alvarez""
                        }, {
                            managerId: 2,
                            fullName: ""Aldon Exley""
                        }, {
                            managerId: 2,
                            fullName: ""Florance Goodricke""
                        }, {
                            managerId: 1,
                            fullName: ""Hannie Everitt""
                        }, {
                            managerId: 1,
                            fullName: ""Sophie Gandley""
                        }, {
                            managerId: null,
                            fullName: ""Linoel Livermore""
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldBeAbleToSortByNullableIntFieldAscAndThenByStringFieldDesc()
        {
            TestQuery(
                @"
                query {
                    people(sorting: [{
                        field: ""managerId"",
                        direction: ASC
                    }, {
                        field: ""fullName"",
                        direction: DESC
                    }]) {
                        items {
                            managerId,
                            fullName
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            managerId: null,
                            fullName: ""Linoel Livermore""
                        }, {
                            managerId: 1,
                            fullName: ""Sophie Gandley""
                        }, {
                            managerId: 1,
                            fullName: ""Hannie Everitt""
                        }, {
                            managerId: 2,
                            fullName: ""Florance Goodricke""
                        }, {
                            managerId: 2,
                            fullName: ""Aldon Exley""
                        }, {
                            managerId: 5,
                            fullName: ""Walton Alvarez""
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldBeAbleToSortByNullableIntFieldDescAndThenByStringFieldDesc()
        {
            TestQuery(
                @"
                query {
                    people(sorting: [{
                        field: ""managerId"",
                        direction: DESC
                    }, {
                        field: ""fullName"",
                        direction: DESC
                    }]) {
                        items {
                            managerId,
                            fullName
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            managerId: 5,
                            fullName: ""Walton Alvarez""
                        }, {
                            managerId: 2,
                            fullName: ""Florance Goodricke""
                        }, {
                            managerId: 2,
                            fullName: ""Aldon Exley""
                        }, {
                            managerId: 1,
                            fullName: ""Sophie Gandley""
                        }, {
                            managerId: 1,
                            fullName: ""Hannie Everitt""
                        }, {
                            managerId: null,
                            fullName: ""Linoel Livermore""
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldBeAbleToSortByCalculatedField()
        {
            TestQuery(
                @"
                query {
                    people(sorting: [{
                        field: ""roundedSalary"",
                        direction: ASC
                    }]) {
                        items {
                            id,
                            roundedSalary
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            id: 4,
                            roundedSalary: 549
                        }, {
                            id: 3,
                            roundedSalary: 1393
                        }, {
                            id: 2,
                            roundedSalary: 2382
                        }, {
                            id: 5,
                            roundedSalary: 3389
                        }, {
                            id: 6,
                            roundedSalary: 3437
                        }, {
                            id: 1,
                            roundedSalary: 4016
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldBeAbleToSortByCustomFieldAsc()
        {
            TestQuery(
                @"
                query {
                    people(sorting: [{
                        field: ""managerNameSorter"",
                        direction: ASC
                    }]) {
                        items {
                            id
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            id: 1
                        }, {
                            id: 6
                        }, {
                            id: 2
                        }, {
                            id: 3
                        }, {
                            id: 4
                        }, {
                            id: 5
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldBeAbleToSortByCustomFieldDesc()
        {
            TestQuery(
                @"
                query {
                    people(sorting: [{
                        field: ""managerNameSorter"",
                        direction: DESC
                    }]) {
                        items {
                            id
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            id: 4
                        }, {
                            id: 5
                        }, {
                            id: 2
                        }, {
                            id: 3
                        }, {
                            id: 6
                        }, {
                            id: 1
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldThrowIfSortByUnknownField()
        {
            TestQueryError(
                typeof(FormatException),
                "Failed to parse PersonFieldName from input 'unknown'. Input should be a string ('id', 'ctxId', 'managerId', 'fullName', 'roundedSalary', 'managerName', 'hireDate', 'terminationDate', 'managerNameSorter').",
                @"
                query {
                    people(sorting: [{
                        field: ""unknown""
                    }]) {
                        items {
                            fullName
                        }
                    }
                }");
        }

        [Test]
        public void ShouldThrowIfSortByInvalidDirection()
        {
            TestQueryError(
                typeof(FormatException),
                "Failed to parse SortDirection from input 'invalid'. Input should be a string (\"asc\", \"desc\") or enum (ASC, DESC).",
                @"
                query {
                    people(sorting: [{
                        field: ""fullName"",
                        direction: ""invalid""
                    }]) {
                        items {
                            fullName
                        }
                    }
                }");
        }
    }
}
