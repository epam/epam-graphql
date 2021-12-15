// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Tests.Common;
using Epam.GraphQL.Tests.Common.Configs;
using Epam.GraphQL.Tests.TestData;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Filtration
{
    [TestFixture(PeopleQueryType.PeopleWithFilter, LoaderType.Loader, PersonLoaderConfigurationType.PersonFilterable)]
    [TestFixture(PeopleQueryType.PeopleWithFilter, LoaderType.MutableLoader, PersonLoaderConfigurationType.PersonFilterable)]
    [TestFixture(PeopleQueryType.PeopleWithFilter, LoaderType.IdentifiableLoader, PersonLoaderConfigurationType.PersonFilterableAndSortable)]
    [TestFixture(PeopleQueryType.PeopleWithFilter, LoaderType.MutableLoader, PersonLoaderConfigurationType.PersonFilterableAndSortable)]
    [TestFixture(PeopleQueryType.PeopleWithFilterAndSearch, LoaderType.Loader, PersonLoaderConfigurationType.PersonFilterable)]
    [TestFixture(PeopleQueryType.PeopleWithFilterAndSearch, LoaderType.MutableLoader, PersonLoaderConfigurationType.PersonFilterable)]
    [TestFixture(PeopleQueryType.PeopleWithFilterAndOrderedSearch, LoaderType.IdentifiableLoader, PersonLoaderConfigurationType.PersonFilterable)]
    [TestFixture(PeopleQueryType.PeopleWithFilterAndOrderedSearch, LoaderType.MutableLoader, PersonLoaderConfigurationType.PersonFilterable)]
    public class FilterAndInlineFiltersTests : BaseTests<Person, PeopleQueryConfigurator, PeopleQueryType, PersonLoaderTypeCreator, PersonLoaderConfigurator, PersonLoaderConfigurationType>
    {
        public FilterAndInlineFiltersTests(PeopleQueryType queryType, LoaderType loaderType, PersonLoaderConfigurationType loaderConfiguration)
            : base(queryType, loaderType, loaderConfiguration)
        {
        }

        [Test]
        public void ShouldThrowIfBothWithFilterAndInlineFiltersExposed()
        {
            TestQueryError(
                typeof(NotSupportedException),
                "Person: Simultaneous use of .WithFilter() and .Filterable() is not supported. Consider to use either .WithFilter() or .Filterable().",
                @"
                query {
                    people(filter: {
                        ids: [1, 2]
                    }) {
                        items {
                            id
                        }
                    }
                }");
        }
    }
}
