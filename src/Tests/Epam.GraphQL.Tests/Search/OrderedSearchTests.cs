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
    [TestFixture(PeopleQueryType.PeopleWithOrderedSearch, LoaderType.Loader, PersonLoaderConfigurationType.Basic)]
    [TestFixture(PeopleQueryType.PeopleWithOrderedSearch, LoaderType.MutableLoader, PersonLoaderConfigurationType.PersonFilterableAndSortable)]
    [TestFixture(PeopleQueryType.PeopleWithFilterAndOrderedSearch, LoaderType.IdentifiableLoader, PersonLoaderConfigurationType.Basic)]
    [TestFixture(PeopleQueryType.PeopleWithFilterAndOrderedSearch, LoaderType.MutableLoader, PersonLoaderConfigurationType.PersonSortable)]
    public class OrderedSearchTests : BaseTests<Person, PeopleQueryConfigurator, PeopleQueryType, PersonLoaderTypeCreator, PersonLoaderConfigurator, PersonLoaderConfigurationType>
    {
        public OrderedSearchTests(PeopleQueryType queryType, LoaderType loaderType, PersonLoaderConfigurationType loaderConfiguration)
            : base(queryType, loaderType, loaderConfiguration)
        {
        }

        [Test]
        public void ShouldSearchByString()
        {
            TestQuery(
                @"
                query {
                    people(search: ""a"") {
                        items {
                            id
                            fullName
                        }
                    }
                }",
                @"
                {
                    people: {
                        items: [{
                            id: 5,
                            fullName: ""Aldon Exley""
                        },{
                            id: 6,
                            fullName: ""Walton Alvarez""
                        },{
                            id: 2,
                            fullName: ""Sophie Gandley""
                        },{
                            id: 3,
                            fullName: ""Hannie Everitt""
                        },{
                            id: 4,
                            fullName: ""Florance Goodricke""
                        }]
                    }
                }");
        }
    }
}
