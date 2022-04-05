// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Filtration
{
    [TestFixture]
    public class InlineCustomFilterTests : BaseTests
    {
        [Test]
        public void InlineCustomFilterTest()
        {
            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
                    loader.Filter<string>("fullName", name => p => p.FullName.Contains(name));
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
                    loader.Filter<string>("fullName", name => p => p.FullName.Contains(name));
                });

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
                        }, {
                            id: 3,
                        }, {
                            id: 6,
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);
        }

        [Test]
        public void InlineCustomFilterUsingContextTest()
        {
            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
                    loader.Filter<string>("fullName", (context, name) => p => p.FullName.Contains(name) && p.Id == context.UserId); // context.UserId = 5
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
                    loader.Filter<string>("fullName", (context, name) => p => p.FullName.Contains(name) && p.Id == context.UserId); // context.UserId = 5
                });

            const string Query = @"
                query {
                    people(filter:{
                        fullName: ""e""
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
                            id: 5,
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);
        }

        [Test]
        public void InlineCustomFilterOrInlineFilterTest()
        {
            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(p => p.Id).Filterable();
                    loader.Field(p => p.FullName);
                    loader.Filter<string>("fullNameContains", name => p => p.FullName.Contains(name));
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(p => p.Id).Filterable();
                    loader.Field(p => p.FullName);
                    loader.Filter<string>("fullNameContains", name => p => p.FullName.Contains(name));
                });

            const string Query = @"
                query {
                    people(filter:{
                        or: [{
                            fullNameContains: ""v""
                        }, {
                            id: {
                                eq: 2
                            }
                        }]
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
                        }, {
                            id: 3,
                        }, {
                            id: 6,
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);
        }

        [Test]
        public void InlineCustomFilterClassTest()
        {
            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
                    loader.Filter<Filter>("fullNameContains", filter => p => p.FullName.Contains(filter.Like));
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
                    loader.Filter<Filter>("fullNameContains", filter => p => p.FullName.Contains(filter.Like));
                });

            const string Query = @"
                query {
                    people(filter:{
                        fullNameContains: {
                            like: ""v""
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
                            id: 3,
                        }, {
                            id: 6,
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);
        }

        [Test]
        public void InlineCustomFilterOrInlineFilterClassTest()
        {
            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(p => p.Id).Filterable();
                    loader.Field(p => p.FullName);
                    loader.Filter<Filter>("fullNameContains", filter => p => p.FullName.Contains(filter.Like));
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(p => p.Id).Filterable();
                    loader.Field(p => p.FullName);
                    loader.Filter<Filter>("fullNameContains", filter => p => p.FullName.Contains(filter.Like));
                });

            const string Query = @"
                query {
                    people(filter:{
                        or: [{
                            fullNameContains: {
                                like: ""v""
                            }
                        }, {
                            id: {
                                eq: 2
                            }
                        }]
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
                        }, {
                            id: 3,
                        }, {
                            id: 6,
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);
        }

        private Action<Query<TestUserContext>> CreateQueryBuilder(Action<Loader<Person, TestUserContext>> personLoaderBuilder, Action<Query<TestUserContext>> configure = null)
        {
            var personLoader = GraphQLTypeBuilder.CreateLoaderType(
                onConfigure: personLoaderBuilder,
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            return query =>
            {
                query
                    .Connection(personLoader, "people");
                configure?.Invoke(query);
            };
        }

        private Action<Query<TestUserContext>> CreateQueryMutableBuilder(Action<MutableLoader<Person, int, TestUserContext>> personLoaderBuilder, Action<Query<TestUserContext>> configure = null)
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType(
                onConfigure: personLoaderBuilder,
                getBaseQuery: _ => FakeData.People.AsQueryable());

            return query =>
            {
                query
                    .Connection(personLoader, "people");
                configure?.Invoke(query);
            };
        }

        public class Filter
        {
            public string Like { get; set; }
        }
    }
}
