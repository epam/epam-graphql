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
    public class InlineFiltersDefaultValuesTests : BaseTests
    {
        [Test]
        public void InlineFilterIntDefaultValuesTest()
        {
            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(p => p.Id).Filterable(1, 2, 3);
                    loader.Field(p => p.FullName);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(p => p.Id).Filterable(1, 2, 3);
                    loader.Field(p => p.FullName);
                });

            const string Query = @"
                query {
                    people {
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
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);
        }

        [Test]
        public void InlineFilterIntTest()
        {
            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(p => p.Id).Filterable(1, 2, 3);
                    loader.Field(p => p.FullName);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(p => p.Id).Filterable(1, 2, 3);
                    loader.Field(p => p.FullName);
                });

            const string Query = @"
                query {
                    people(filter: {
                        id: {
                            in: [3, 4]
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
                            id: 3,
                        }, {
                            id: 4,
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);
        }

        [Test]
        public void InlineFilterStringDefaultValuesTest()
        {
            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName)
                        .Filterable(FakeData.LinoelLivermore.FullName, FakeData.SophieGandley.FullName, FakeData.HannieEveritt.FullName);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName)
                        .Filterable(FakeData.LinoelLivermore.FullName, FakeData.SophieGandley.FullName, FakeData.HannieEveritt.FullName);
                });

            const string Query = @"
                query {
                    people {
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
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);
        }

        [Test]
        public void InlineFilterStringTest()
        {
            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName)
                        .Filterable(FakeData.LinoelLivermore.FullName, FakeData.SophieGandley.FullName, FakeData.HannieEveritt.FullName);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName)
                        .Filterable(FakeData.LinoelLivermore.FullName, FakeData.SophieGandley.FullName, FakeData.HannieEveritt.FullName);
                });

            const string Query = @"
                query {
                    people(filter: {
                        fullName: {
                            in: [""Hannie Everitt"", ""Florance Goodricke""]
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
                            id: 3,
                        }, {
                            id: 4,
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);
        }

        [Test]
        public void InlineFilterStringDefaultNotNullTest()
        {
            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("managerName", p => p.Manager != null ? p.Manager.FullName : null)
                        .Filterable(loader.NotNullValues);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("managerName", p => p.Manager != null ? p.Manager.FullName : null)
                        .Filterable(loader.NotNullValues);
                });

            const string Query = @"
                query {
                    people {
                        items {
                            id
                        }
                    }
                }";

            const string Expected = @"
                {
                    people: {
                        items: [{
                            id: 2,
                        }, {
                            id: 3,
                        },{
                            id: 4,
                        }, {
                            id: 5,
                        }, {
                            id: 6,
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);
        }

        [Test]
        public void InlineFilterStringDefaultNotNullWithFilterTest()
        {
            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(p => p.Id).Filterable();
                    loader.Field("managerName", p => p.Manager != null ? p.Manager.FullName : null)
                        .Filterable(loader.NotNullValues);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("managerName", p => p.Manager != null ? p.Manager.FullName : null)
                        .Filterable(loader.NotNullValues);
                });

            const string Query = @"
                query {
                    people(filter: {
                        managerName: {
                            in: [""Linoel Livermore""]
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
                            id: 2,
                        }, {
                            id: 3,
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);
        }

        [Test]
        public void InlineFilterStringDefaultNullTest()
        {
            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("managerName", p => p.Manager != null ? p.Manager.FullName : null)
                        .Filterable(loader.NullValues);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("managerName", p => p.Manager != null ? p.Manager.FullName : null)
                        .Filterable(loader.NullValues);
                });

            const string Query = @"
                query {
                    people {
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

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);
        }

        [Test]
        public void InlineFilterStringDefaultNullWithFilterTest()
        {
            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(p => p.Id).Filterable();
                    loader.Field("managerName", p => p.Manager != null ? p.Manager.FullName : null)
                        .Filterable(loader.NullValues);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("managerName", p => p.Manager != null ? p.Manager.FullName : null)
                        .Filterable(loader.NullValues);
                });

            const string Query = @"
                query {
                    people(filter: {
                        managerName: {
                            in: [""Linoel Livermore""]
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
                            id: 2,
                        }, {
                            id: 3,
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);
        }

        [Test]
        public void InlineFilterNullableIntDefaultValuesTest()
        {
            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.ManagerId)
                        .Filterable(1);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.ManagerId)
                        .Filterable(1);
                });

            const string Query = @"
                query {
                    people {
                        items {
                            id
                        }
                    }
                }";

            const string Expected = @"
                {
                    people: {
                        items: [{
                            id: 2,
                        }, {
                            id: 3,
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);
        }

        [Test]
        public void InlineFilterNullableIntTest()
        {
            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.ManagerId)
                        .Filterable(1);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.ManagerId)
                        .Filterable(1);
                });

            const string Query = @"
                query {
                    people(filter: {
                        managerId: {
                            in: [2]
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
                            id: 4,
                        }, {
                            id: 5,
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);
        }

        [Test]
        public void InlineFilterNullableIntDefaultNotNullTest()
        {
            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.ManagerId)
                        .Filterable(loader.NotNullValues);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.ManagerId)
                        .Filterable(loader.NotNullValues);
                });

            const string Query = @"
                query {
                    people {
                        items {
                            id
                        }
                    }
                }";

            const string Expected = @"
                {
                    people: {
                        items: [{
                            id: 2,
                        }, {
                            id: 3,
                        },{
                            id: 4,
                        }, {
                            id: 5,
                        }, {
                            id: 6,
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);
        }

        [Test]
        public void InlineFilterNullableIntDefaultNotNullWithFilterTest()
        {
            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(p => p.Id).Filterable();
                    loader.Field(p => p.ManagerId)
                        .Filterable(loader.NotNullValues);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.ManagerId)
                        .Filterable(loader.NotNullValues);
                });

            const string Query = @"
                query {
                    people(filter: {
                        managerId: {
                            in: [2]
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
                            id: 4,
                        }, {
                            id: 5,
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);
        }

        [Test]
        public void InlineFilterNullableIntDefaultNullTest()
        {
            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.ManagerId)
                        .Filterable(loader.NullValues);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.ManagerId)
                        .Filterable(loader.NullValues);
                });

            const string Query = @"
                query {
                    people {
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

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);
        }

        [Test]
        public void InlineFilterNullableIntDefaultNullWithFilterTest()
        {
            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(p => p.Id).Filterable();
                    loader.Field(p => p.ManagerId)
                        .Filterable(loader.NullValues);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.ManagerId)
                        .Filterable(loader.NullValues);
                });

            const string Query = @"
                query {
                    people(filter: {
                        managerId: {
                            in: [2]
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
                            id: 4,
                        }, {
                            id: 5,
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);
        }

        [Test]
        public void ShouldThrowIfNullStringWasProvidedAsDefaultValue()
        {
            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(p => p.Id).Filterable();
                    loader.Field(p => p.FullName)
                        .Filterable(null, FakeData.LinoelLivermore.FullName, FakeData.SophieGandley.FullName, FakeData.HannieEveritt.FullName);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(p => p.Id).Filterable();
                    loader.Field(p => p.FullName)
                        .Filterable(null, FakeData.LinoelLivermore.FullName, FakeData.SophieGandley.FullName, FakeData.HannieEveritt.FullName);
                });

            const string Query = @"
                query {
                    people(filter: {
                        fullName: {
                            in: [""Hannie Everitt"", ""Florance Goodricke""]
                        }
                    }) {
                        items {
                            id
                        }
                    }
                }";

            TestHelpers.TestQueryError(builder, typeof(ArgumentException), ".Filterable() does not support nulls as parameters. Consider to use .Filterable(NullValues).", Query);
            TestHelpers.TestQueryError(mutableBuilder, typeof(ArgumentException), ".Filterable() does not support nulls as parameters. Consider to use .Filterable(NullValues).", Query);
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
    }
}
