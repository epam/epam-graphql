// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Epam.GraphQL.Tests.Common;
using Epam.GraphQL.Tests.Common.Configs;
using Epam.GraphQL.Tests.TestData;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Filtration
{
    [TestFixture(PeopleQueryType.PeopleWithFilter, LoaderType.Loader, PersonLoaderConfigurationType.Basic)]
    [TestFixture(PeopleQueryType.PeopleWithFilter, LoaderType.MutableLoader, PersonLoaderConfigurationType.Basic)]
    [TestFixture(PeopleQueryType.PeopleWithFilter, LoaderType.IdentifiableLoader, PersonLoaderConfigurationType.PersonSortable)]
    [TestFixture(PeopleQueryType.PeopleWithFilter, LoaderType.MutableLoader, PersonLoaderConfigurationType.PersonSortable)]
    [TestFixture(PeopleQueryType.PeopleWithFilterAndSearch, LoaderType.Loader, PersonLoaderConfigurationType.Basic)]
    [TestFixture(PeopleQueryType.PeopleWithFilterAndSearch, LoaderType.MutableLoader, PersonLoaderConfigurationType.Basic)]
    [TestFixture(PeopleQueryType.PeopleWithFilterAndOrderedSearch, LoaderType.IdentifiableLoader, PersonLoaderConfigurationType.PersonSortable)]
    [TestFixture(PeopleQueryType.PeopleWithFilterAndOrderedSearch, LoaderType.MutableLoader, PersonLoaderConfigurationType.PersonSortable)]
    public class FilterTests : BaseTests<Person, PeopleQueryConfigurator, PeopleQueryType, PersonLoaderTypeCreator, PersonLoaderConfigurator, PersonLoaderConfigurationType>
    {
        public FilterTests(PeopleQueryType queryType, LoaderType loaderType, PersonLoaderConfigurationType loaderConfiguration)
            : base(queryType, loaderType, loaderConfiguration)
        {
        }

        [Test]
        public void ShouldFilterByIds()
        {
            TestQuery(
                @"
                query {
                    people(filter: {
                        ids: [1, 2]
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
                        }, {
                            id: 2
                        }]
                    }
                }");
        }
    }
}
