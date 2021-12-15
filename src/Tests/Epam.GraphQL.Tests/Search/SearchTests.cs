// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Epam.GraphQL.Tests.Common;
using Epam.GraphQL.Tests.Common.Configs;
using Epam.GraphQL.Tests.TestData;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Search
{
    [TestFixture(PeopleQueryType.PeopleWithSearch, LoaderType.Loader, PersonLoaderConfigurationType.Basic)]
    [TestFixture(PeopleQueryType.PeopleWithSearch, LoaderType.IdentifiableLoader, PersonLoaderConfigurationType.Basic)]
    [TestFixture(PeopleQueryType.PeopleWithSearch, LoaderType.MutableLoader, PersonLoaderConfigurationType.Basic)]
    [TestFixture(PeopleQueryType.PeopleWithSearch, LoaderType.Loader, PersonLoaderConfigurationType.PersonFilterable)]
    [TestFixture(PeopleQueryType.PeopleWithSearch, LoaderType.IdentifiableLoader, PersonLoaderConfigurationType.PersonFilterable)]
    [TestFixture(PeopleQueryType.PeopleWithSearch, LoaderType.MutableLoader, PersonLoaderConfigurationType.PersonFilterable)]
    [TestFixture(PeopleQueryType.PeopleWithSearch, LoaderType.Loader, PersonLoaderConfigurationType.PersonFilterableAndSortable)]
    [TestFixture(PeopleQueryType.PeopleWithSearch, LoaderType.IdentifiableLoader, PersonLoaderConfigurationType.PersonFilterableAndSortable)]
    [TestFixture(PeopleQueryType.PeopleWithSearch, LoaderType.MutableLoader, PersonLoaderConfigurationType.PersonFilterableAndSortable)]
    [TestFixture(PeopleQueryType.PeopleWithSearch, LoaderType.Loader, PersonLoaderConfigurationType.PersonSortable)]
    [TestFixture(PeopleQueryType.PeopleWithSearch, LoaderType.IdentifiableLoader, PersonLoaderConfigurationType.PersonSortable)]
    [TestFixture(PeopleQueryType.PeopleWithSearch, LoaderType.MutableLoader, PersonLoaderConfigurationType.PersonSortable)]
    [TestFixture(PeopleQueryType.PeopleWithFilterAndSearch, LoaderType.Loader, PersonLoaderConfigurationType.Basic)]
    [TestFixture(PeopleQueryType.PeopleWithFilterAndSearch, LoaderType.IdentifiableLoader, PersonLoaderConfigurationType.Basic)]
    [TestFixture(PeopleQueryType.PeopleWithFilterAndSearch, LoaderType.MutableLoader, PersonLoaderConfigurationType.Basic)]
    [TestFixture(PeopleQueryType.PeopleWithFilterAndSearch, LoaderType.Loader, PersonLoaderConfigurationType.PersonSortable)]
    [TestFixture(PeopleQueryType.PeopleWithFilterAndSearch, LoaderType.IdentifiableLoader, PersonLoaderConfigurationType.PersonSortable)]
    [TestFixture(PeopleQueryType.PeopleWithFilterAndSearch, LoaderType.MutableLoader, PersonLoaderConfigurationType.PersonSortable)]
    public class SearchTests : BaseTests<Person, PeopleQueryConfigurator, PeopleQueryType, PersonLoaderTypeCreator, PersonLoaderConfigurator, PersonLoaderConfigurationType>
    {
        public SearchTests(PeopleQueryType queryType, LoaderType loaderType, PersonLoaderConfigurationType loaderConfiguration)
            : base(queryType, loaderType, loaderConfiguration)
        {
        }

        [Test]
        public void ShouldSearchByString()
        {
            TestQuery(
                @"
                query {
                    people(search: ""alv"") {
                        items {
                            id,
                            fullName
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            id: 6,
                            fullName: ""Walton Alvarez""
                        }]
                    }
                }");
        }
    }
}
