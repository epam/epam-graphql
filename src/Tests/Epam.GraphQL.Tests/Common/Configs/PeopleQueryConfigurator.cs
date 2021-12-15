// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;

namespace Epam.GraphQL.Tests.Common.Configs
{
    public class PeopleQueryConfigurator : IQueryConfigurator<PeopleQueryType>
    {
#pragma warning disable CA1304 // Specify CultureInfo
        public void ConfigureQuery(PeopleQueryType queryType, Query<TestUserContext> configure, Type personLoaderType)
        {
            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            switch (queryType)
            {
                case PeopleQueryType.PeopleBasic:
                    configure
                        .Connection(personLoaderType, "people");
                    break;

                case PeopleQueryType.PeopleWithFilter:
                    var filterType = GraphQLTypeBuilder.CreateLoaderFilterType<Person, PersonDerivedFilter, TestUserContext>(
                        applyFilter: (context, query, filter) => filter.Ids != null ? query = query.Where(a => filter.Ids.Contains(a.Id)) : query);

                    configure
                        .Connection(personLoaderType, "people")
                        .WithFilter(filterType);
                    break;

                case PeopleQueryType.PeopleWithFilterAndSearch:
                    filterType = GraphQLTypeBuilder.CreateLoaderFilterType<Person, PersonDerivedFilter, TestUserContext>(
                        applyFilter: (context, query, filter) => filter.Ids != null ? query = query.Where(a => filter.Ids.Contains(a.Id)) : query);

                    var searcherType = GraphQLTypeBuilder.CreateSearcherType<Person, TestUserContext>(
                        all: (query, context, search) => query.Where(p => p.FullName.ToLower().Contains(search)));

                    configure
                        .Connection(personLoaderType, "people")
                        .WithFilter(filterType)
                        .WithSearch(searcherType);
                    break;

                case PeopleQueryType.PeopleWithFilterAndOrderedSearch:
                    filterType = GraphQLTypeBuilder.CreateLoaderFilterType<Person, PersonDerivedFilter, TestUserContext>(
                        applyFilter: (context, query, filter) => filter.Ids != null ? query = query.Where(a => filter.Ids.Contains(a.Id)) : query);

                    searcherType = GraphQLTypeBuilder.CreateOrderedSearcherType<Person, TestUserContext>(
                        all: (query, context, search) => query.Where(p => p.FullName.ToLower().Contains(search)),
                        applySearchOrderBy: (query, search) => string.IsNullOrEmpty(search) ? query.OrderBy(p => p.Id) : query.OrderBy(p => p.FullName.ToLower().StartsWith(search) ? 0 : p.FullName.ToLower().Contains(" " + search) ? 1 : 2),
                        applySearchThenBy: (query, search) => string.IsNullOrEmpty(search) ? query.ThenBy(p => p.Id) : query.ThenBy(p => p.FullName.ToLower().StartsWith(search) ? 0 : p.FullName.ToLower().Contains(" " + search) ? 1 : 2));

                    configure
                        .Connection(personLoaderType, "people")
                        .WithFilter(filterType)
                        .WithSearch(searcherType);
                    break;

                case PeopleQueryType.PeopleWithSearch:
                    searcherType = GraphQLTypeBuilder.CreateSearcherType<Person, TestUserContext>(
                        all: (query, context, search) => query.Where(p => p.FullName.ToLower().Contains(search)));

                    configure
                        .Connection(personLoaderType, "people")
                        .WithSearch(searcherType);
                    break;

                case PeopleQueryType.PeopleWithOrderedSearch:
                    searcherType = GraphQLTypeBuilder.CreateOrderedSearcherType<Person, TestUserContext>(
                        all: (query, context, search) => query.Where(p => p.FullName.ToLower().Contains(search)),
                        applySearchOrderBy: (query, search) => string.IsNullOrEmpty(search) ? query.OrderBy(p => p.Id) : query.OrderBy(p => p.FullName.ToLower().StartsWith(search) ? 0 : p.FullName.ToLower().Contains(" " + search) ? 1 : 2),
                        applySearchThenBy: (query, search) => string.IsNullOrEmpty(search) ? query.ThenBy(p => p.Id) : query.ThenBy(p => p.FullName.ToLower().StartsWith(search) ? 0 : p.FullName.ToLower().Contains(" " + search) ? 1 : 2));

                    configure
                        .Connection(personLoaderType, "people")
                        .WithSearch(searcherType);
                    break;

                default:
                    throw new NotSupportedException();
            }
        }
#pragma warning restore CA1304 // Specify CultureInfo

        public class PersonFilter : Input
        {
            public List<int> Ids { get; set; }
        }

        public class PersonDerivedFilter : PersonFilter
        {
        }
    }
}
