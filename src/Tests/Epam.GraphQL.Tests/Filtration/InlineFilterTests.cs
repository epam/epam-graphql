// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Epam.GraphQL.Tests.Common;
using Epam.GraphQL.Tests.Common.Configs;
using Epam.GraphQL.Tests.TestData;
using GraphQL.Validation.Errors;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Filtration
{
    [TestFixture(PeopleQueryType.PeopleBasic, LoaderType.Loader, PersonLoaderConfigurationType.PersonFilterable)]
    [TestFixture(PeopleQueryType.PeopleBasic, LoaderType.IdentifiableLoader, PersonLoaderConfigurationType.PersonFilterable)]
    [TestFixture(PeopleQueryType.PeopleBasic, LoaderType.IdentifiableLoader, PersonLoaderConfigurationType.PersonFilterableAndSortable)]
    [TestFixture(PeopleQueryType.PeopleBasic, LoaderType.MutableLoader, PersonLoaderConfigurationType.PersonFilterableAndSortable)]
    [TestFixture(PeopleQueryType.PeopleWithSearch, LoaderType.Loader, PersonLoaderConfigurationType.PersonFilterable)]
    [TestFixture(PeopleQueryType.PeopleWithSearch, LoaderType.MutableLoader, PersonLoaderConfigurationType.PersonFilterable)]
    [TestFixture(PeopleQueryType.PeopleWithSearch, LoaderType.IdentifiableLoader, PersonLoaderConfigurationType.PersonFilterableAndSortable)]
    [TestFixture(PeopleQueryType.PeopleWithSearch, LoaderType.MutableLoader, PersonLoaderConfigurationType.PersonFilterableAndSortable)]
    public class InlineFilterTests : BaseTests<Person, PeopleQueryConfigurator, PeopleQueryType, PersonLoaderTypeCreator, PersonLoaderConfigurator, PersonLoaderConfigurationType>
    {
        public InlineFilterTests(PeopleQueryType queryType, LoaderType loaderType, PersonLoaderConfigurationType loaderConfiguration)
            : base(queryType, loaderType, loaderConfiguration)
        {
        }

        [Test]
        public void ShouldApplyInByInt()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        id: {
                            in: [1]
                        }
                    }) {
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
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyInByIntWithCtx()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        ctxId: {
                            in: [6]
                        }
                    }) {
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
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyNinByInt()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        id: {
                            nin: [1]
                        }
                    }) {
                        items {
                            id
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
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
        public void ShouldApplyInByInts()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        id: {
                            in: [1, 2]
                        }
                    }) {
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
                        },{
                            id: 2
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyNinByInts()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        id: {
                            nin: [1, 2, 3, 4]
                        }
                    }) {
                        items {
                            id
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            id: 5
                        },{
                            id: 6
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyInByIntAndString()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        id: {
                            in: [1, 2]
                        },
                        fullName: {
                            in: [""Linoel Livermore""]
                        }
                    }) {
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
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyInByCalculatedDecimal()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        roundedSalary: {
                            in: [4016]
                        },
                    }) {
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
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldReturnAllDataIfEmptyFieldForFilterProvided()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        id: {
                        }
                    }) {
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
        public void ShouldReturnAllDataIfNullInFieldForFilterProvided()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        id: null
                    }) {
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
        public void ShouldReturnAllDataIfEmptyArrayInInFieldForFilterProvided()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        id: {
                            in: []
                        }
                    }) {
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
        public void ShouldApplyNotByInt()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        not: {
                            id: {
                                in: [1, 2, 3]
                            }
                        }
                    }) {
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
                        },{
                            id: 5
                        },{
                            id: 6
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyAndToSameIntFilters()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        and: [{
                            id: {
                                in: [1, 2, 3]
                            }
                        }, {
                            id: {
                                in: [2]
                            }
                        }]
                    }) {
                        items {
                            id
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            id: 2
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyOrToSameIntFilters()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        or: [{
                            id: {
                                in: [1, 2]
                            }
                        }, {
                            id: {
                                in: [5, 6]
                            }
                        }]
                    }) {
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
                        },{
                            id: 2
                        },{
                            id: 5
                        },{
                            id: 6
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldThrowIfNullForNullableFieldPassedIntoIn()
        {
            TestQueryError(
                typeof(ArgumentsOfCorrectTypeError),
                "Argument \"filter\" has invalid value {managerId: {in: [null]}}.\nIn field \"managerId\": In field \"in\": In element #1: Expected \"Int!\", found null.",
                @"
                query {
                    people(filter: {
                        managerId: {
                            in: [null]
                        }
                    }) {
                        items {
                            id
                        }
                    }
                }");
        }

        [Test]
        public void ShouldApplyInByNullableInt()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        managerId: {
                            in: [1]
                        }
                    }) {
                        items {
                            id
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            id: 2
                        },{
                            id: 3
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyIsNullByyNullableInt()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        managerId: {
                            isNull: true
                        }
                    }) {
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
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyIsNotNullByNullableInt()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        managerId: {
                            isNull: false
                        }
                    }) {
                        items {
                            id
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
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
        public void ShouldApplyIsNullByInt()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        id: {
                            isNull: true
                        }
                    }) {
                        items {
                            id
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: []
                    }
                }");
        }

        [Test]
        public void ShouldApplyIsNotNullByInt()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        id: {
                            isNull: false
                        }
                    }) {
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
        public void ShouldApplyIsNullByString()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        managerName: {
                            isNull: true
                        }
                    }) {
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
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyIsNotNullByString()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        managerName: {
                            isNull: false
                        }
                    }) {
                        items {
                            id
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
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
        public void ShouldApplyEqByInt()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        id: {
                            eq: 1
                        }
                    }) {
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
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyNeqByInt()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        id: {
                            neq: 1
                        }
                    }) {
                        items {
                            id
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
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
        public void ShouldApplyEqByNullableInt()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        managerId: {
                            eq: 1
                        }
                    }) {
                        items {
                            id
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            id: 2
                        },{
                            id: 3
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyNeqByNullableInt()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        managerId: {
                            neq: 1
                        }
                    }) {
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
        public void ShouldApplyEqByString()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        fullName: {
                            eq: ""Hannie Everitt""
                        }
                    }) {
                        items {
                            id
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            id: 3
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyNeqByString()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        fullName: {
                            neq: ""Hannie Everitt""
                        }
                    }) {
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
                        },{
                            id: 2
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
        public void ShouldApplyAndToNestedOrs()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        and: [{
                            or: [{
                                id: {
                                    eq: 1
                                }
                            }, {
                                id: {
                                    eq: 2
                                },
                            }, {
                                id: {
                                    eq: 3
                                }
                            }]
                        }, {
                            not: {
                                id: {
                                    in: [1, 3]
                                }
                            }
                        }]
                    }) {
                        items {
                            id
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            id: 2
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyGtByInt()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        id: {
                            gt: 4
                        }
                    }) {
                        items {
                            id
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            id: 5
                        },{
                            id: 6
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyLtByInt()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        id: {
                            lt: 3
                        }
                    }) {
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
                        },{
                            id: 2
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyGteByInt()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        id: {
                            gte: 5
                        }
                    }) {
                        items {
                            id
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            id: 5
                        },{
                            id: 6
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyLteByInt()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        id: {
                            lte: 2
                        }
                    }) {
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
                        },{
                            id: 2
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyGtByNullableInt()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        managerId: {
                            gt: 1
                        }
                    }) {
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
                        },{
                            id: 5
                        },{
                            id: 6
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyLtByNullableInt()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        managerId: {
                            lt: 2
                        }
                    }) {
                        items {
                            id
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            id: 2
                        },{
                            id: 3
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyGteByNullableInt()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        managerId: {
                            gte: 2
                        }
                    }) {
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
                        },{
                            id: 5
                        },{
                            id: 6
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyLteByNullableInt()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        managerId: {
                            lte: 1
                        }
                    }) {
                        items {
                            id
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            id: 2
                        },{
                            id: 3
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyEqByDateTime()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        hireDate: {
                            eq: ""2000-01-20""
                        }
                    }) {
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
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyGtByDateTime()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        hireDate: {
                            gt: ""2015-03-01""
                        }
                    }) {
                        items {
                            id
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            id: 5
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyLtByDateTime()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        hireDate: {
                            lt: ""2010-06-14""
                        }
                    }) {
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
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyGteByDateTime()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        hireDate: {
                            gte: ""2015-03-01""
                        }
                    }) {
                        items {
                            id
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            id: 3
                        },{
                            id: 5
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyLteByDateTime()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        hireDate: {
                            lte: ""2010-06-14""
                        }
                    }) {
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
                        },{
                            id: 2
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyEqByNullableDateTime()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        terminationDate: {
                            eq: ""2019-10-01""
                        }
                    }) {
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
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyNeqByNullableDateTime()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        terminationDate: {
                            neq: ""2019-10-01""
                        }
                    }) {
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
                        },{
                            id: 2
                        },{
                            id: 3
                        },{
                            id: 5
                        },{
                            id: 6
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyGtByNullableDateTime()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        terminationDate: {
                            gt: ""2017-02-19""
                        }
                    }) {
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
                        },{
                            id: 6
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyLtByNullableDateTime()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        terminationDate: {
                            lt: ""2019-10-01""
                        }
                    }) {
                        items {
                            id
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            id: 5
                        },{
                            id: 6
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyGteByNullableDateTime()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        terminationDate: {
                            gte: ""2017-02-19""
                        }
                    }) {
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
                        },{
                            id: 5
                        },{
                            id: 6
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyLteByNullableDateTime()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        terminationDate: {
                            lte: ""2019-10-01""
                        }
                    }) {
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
                        },{
                            id: 5
                        },{
                            id: 6
                        }]
                    }
                }");
        }

        [Test]
        public void ShouldApplyCustomFilter()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        managerNameFilter: ""Linoel Livermore""
                    }) {
                        items {
                            id
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            id: 2
                        },{
                            id: 3
                        }]
                    }
                }");
        }
    }
}
