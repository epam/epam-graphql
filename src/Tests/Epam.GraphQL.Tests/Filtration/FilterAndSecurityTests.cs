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
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Filtration
{
    /// <summary>
    /// Tests for https://git.epam.com/epm-ppa/epam-graphql/-/issues/32.
    /// </summary>
    [TestFixture]
    public class FilterAndSecurityTests : BaseTests
    {
        [Test]
        public void ShouldCallFilterBeforeApplySecurityFilter()
        {
            var expected = new List<Person>(FakeData.People.Where(p => p.Id is 1 or 3 or 6));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
                },
                people => Assert.AreEqual(expected, people));

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
                },
                people => Assert.AreEqual(expected, people));

            var filter = GraphQLTypeBuilder.CreateLoaderFilterType<Person, MyFilter, TestUserContext>((ctx, query, f) => query.Where(p => p.FullName.Contains(f.FullName)));

            const string Query = @"
                query {
                    people(filter:{
                        fullName: ""v""
                    }) {
                        items {
                            id
                        }
                    }
                }";

            const string Expected = @"
                {
                    people: {
                        items: [{
                            id: 1,
                        }]
                    }
                }";

            TestHelpers.TestQuery(query => query.Connection(builder, "people").WithFilter(filter), Query, Expected);
            TestHelpers.TestQuery(query => query.Connection(mutableBuilder, "people").WithFilter(filter), Query, Expected);
        }

        [Test]
        public void ShouldCallSearcherBeforeApplySecurityFilter()
        {
            var expected = new List<Person>(FakeData.People.Where(p => p.Id is 1 or 3 or 6));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
                },
                people => Assert.AreEqual(expected, people));

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
                },
                people => Assert.AreEqual(expected, people));

            var searcherType = GraphQLTypeBuilder.CreateSearcherType<Person, TestUserContext>((query, ctx, search) => query.Where(p => p.FullName.Contains(search)));

            const string Query = @"
                query {
                    people(search: ""v"") {
                        items {
                            id
                        }
                    }
                }";

            const string Expected = @"
                {
                    people: {
                        items: [{
                            id: 1,
                        }]
                    }
                }";

            TestHelpers.TestQuery(query => query.Connection(builder, "people").WithSearch(searcherType), Query, Expected);
            TestHelpers.TestQuery(query => query.Connection(mutableBuilder, "people").WithSearch(searcherType), Query, Expected);
        }

        [Test]
        public void ShouldCallOrderedSearcherBeforeApplySecurityFilter()
        {
            var expected = new List<Person>(FakeData.People.Where(p => p.Id is 1 or 3 or 6));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
                },
                people => Assert.AreEqual(expected, people));

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
                },
                people => Assert.AreEqual(expected, people));

            var searcherType = GraphQLTypeBuilder.CreateOrderedSearcherType<Person, TestUserContext>(
                (query, ctx, search) => query.Where(p => p.FullName.Contains(search)),
                (query, search) => query.OrderBy(p => p.Id),
                (query, search) => query.ThenBy(p => p.Id));

            const string Query = @"
                query {
                    people(search:""v"") {
                        items {
                            id
                        }
                    }
                }";

            const string Expected = @"
                {
                    people: {
                        items: [{
                            id: 1,
                        }]
                    }
                }";

            TestHelpers.TestQuery(query => query.Connection(builder, "people").WithSearch(searcherType), Query, Expected);
            TestHelpers.TestQuery(query => query.Connection(mutableBuilder, "people").WithSearch(searcherType), Query, Expected);
        }

        [Test]
        public void ShouldCallInlineFilterBeforeApplySecurityFilter()
        {
            var expected = new List<Person>(FakeData.People.Where(p => p.Id is 1 or 2 or 3));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(p => p.Id).Filterable();
                    loader.Field(p => p.FullName);
                },
                people => Assert.AreEqual(expected, people));

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(p => p.Id).Filterable();
                    loader.Field(p => p.FullName);
                },
                people => Assert.AreEqual(expected, people));

            const string Query = @"
                query {
                    people(filter:{
                        id: {
                            in: [1, 2, 3]
                        }
                    }) {
                        items {
                            id
                        }
                    }
                }";

            const string Expected = @"
                {
                    people: {
                        items: [{
                            id: 1,
                        }, {
                            id: 2,
                        }]
                    }
                }";

            TestHelpers.TestQuery(query => query.Connection(builder, "people"), Query, Expected);
            TestHelpers.TestQuery(query => query.Connection(mutableBuilder, "people"), Query, Expected);
        }

        [Test]
        public void ShouldCallInlineCustomFilterBeforeApplySecurityFilter()
        {
            var expected = new List<Person>(FakeData.People.Where(p => p.Id is 1 or 3 or 6));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
                    loader.Filter<string>("fullName", name => p => p.FullName.Contains(name));
                },
                people => Assert.AreEqual(expected, people));

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
                    loader.Filter<string>("fullName", name => p => p.FullName.Contains(name));
                },
                people => Assert.AreEqual(expected, people));

            const string Query = @"
                query {
                    people(filter:{
                        fullName: ""v""
                    }) {
                        items {
                            id
                        }
                    }
                }";

            const string Expected = @"
                {
                    people: {
                        items: [{
                            id: 1,
                        }]
                    }
                }";

            TestHelpers.TestQuery(query => query.Connection(builder, "people"), Query, Expected);
            TestHelpers.TestQuery(query => query.Connection(mutableBuilder, "people"), Query, Expected);
        }

        private Type CreateQueryBuilder(Action<Loader<Person, TestUserContext>> personLoaderBuilder, Action<List<Person>> check)
        {
            var personLoader = GraphQLTypeBuilder.CreateLoaderType(
                onConfigure: personLoaderBuilder,
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable(),
                applySecurityFilter: (ctx, q) =>
                {
                    check(q.ToList());
                    return q.Where(p => p.Id == 1 || p.Id == 2);
                });

            return personLoader;
        }

        private Type CreateQueryMutableBuilder(Action<MutableLoader<Person, int, TestUserContext>> personLoaderBuilder, Action<List<Person>> check)
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType(
                onConfigure: personLoaderBuilder,
                getBaseQuery: _ => FakeData.People.AsQueryable(),
                applySecurityFilter: (ctx, q) =>
                {
                    check(q.ToList());
                    return q.Where(p => p.Id == 1 || p.Id == 2);
                });

            return personLoader;
        }

        public class MyFilter : Input
        {
            public string FullName { get; set; }
        }
    }
}
