// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.Mock;
using Epam.GraphQL.Tests.TestData;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Suites
{
    [TestFixture]
    public partial class FromQueryableTests
    {
        public enum DataSet
        {
            Empty,
            One,
            AllWithManager,
            All,
        }

        public enum Kind
        {
            Root,
            Loader,
            Query,
            Batch,
        }

        public static IEnumerable<TestCaseData> RootClassTestData => GetClassTestData(Kind.Root)
            .Concat(GetClassTestData2(Kind.Root));

        public static IEnumerable<TestCaseData> FromLoaderClassTestData => GetClassTestData(Kind.Loader)
            .Concat(GetClassTestData2(Kind.Loader));

        public static IEnumerable<TestCaseData> FromQueryableClassTestData => GetClassTestData(Kind.Query)
            .Concat(GetClassTestData2(Kind.Query));

        public static IEnumerable<TestCaseData> FromBatchClassTestData => GetClassTestData(Kind.Batch)
            .Concat(GetClassTestData2(Kind.Batch));

        public static IEnumerable<TestCaseData> RootIntTestData => GetRootIntTestData();

        public static IEnumerable<TestCaseData> RootIntClassTestData => GetRootIntClassTestData();

        public static IEnumerable<TestCaseData> IntTestData => GetIntTestData(Kind.Loader)
            .Concat(GetIntTestData(Kind.Query))
            .Concat(GetIntTestData(Kind.Batch));

        public static IEnumerable<TestCaseData> IntClassTestData => GetIntClassTestData(Kind.Loader)
            .Concat(GetIntClassTestData(Kind.Query))
            .Concat(GetIntClassTestData(Kind.Batch));

        public static IEnumerable<TestCaseData> RootNullableIntTestData => GetRootNullableIntTestData();

        public static IEnumerable<TestCaseData> NullableIntTestData => GetNullableIntTestData(Kind.Loader)
            .Concat(GetNullableIntTestData(Kind.Query))
            .Concat(GetNullableIntTestData(Kind.Batch));

        [TestCaseSource(typeof(FromQueryableTests), nameof(RootClassTestData))]
        public void RootFromQueryableClass(
            DataSet dataSet,
            Action<IRootQueryableField<Person, TestUserContext>> configure,
            string queryFragment,
            string resultFragment,
            Action<IInlineObjectBuilder<Person, TestUserContext>> configureQueryable = null)
        {
            var queryFactory = GetDataFactory(dataSet, person => person);

            var query = $@"
                query {{
                    {queryFragment}
                }}";

            var result = $@"{{
                {resultFragment}
            }}";

            TestHelpers.TestQuery(
                query => configure(query.Field("test").FromIQueryable(queryFactory, configureQueryable)),
                query,
                result,
                new DataContextMock());
        }

        [TestCaseSource(typeof(FromQueryableTests), nameof(RootIntTestData))]
        public void RootFromQueryableInt(
            DataSet dataSet,
            Action<IRootQueryableField<int, TestUserContext>> configure,
            string queryFragment,
            string resultFragment)
        {
            RootFromQueryable(
                dataSet,
                configure,
                queryFragment,
                resultFragment,
                person => person.Id);
        }

        [TestCaseSource(typeof(FromQueryableTests), nameof(RootIntClassTestData))]
        public void RootFromQueryableIntClass(
            DataSet dataSet,
            Action<IRootQueryableField<int, TestUserContext>> configure,
            string queryFragment,
            string resultFragment)
        {
            RootFromQueryable(
                dataSet,
                configure,
                queryFragment,
                resultFragment,
                person => person.Id,
                builder => builder.Field("id", id => id).Groupable());
        }

        [TestCaseSource(typeof(FromQueryableTests), nameof(RootNullableIntTestData))]
        public void RootFromQueryableInt(
            DataSet dataSet,
            Action<IRootQueryableField<int?, TestUserContext>> configure,
            string queryFragment,
            string resultFragment)
        {
            RootFromQueryable(
                dataSet,
                configure,
                queryFragment,
                resultFragment,
                person => person.ManagerId);
        }

        [TestCaseSource(typeof(FromQueryableTests), nameof(IntTestData))]
        public void FromQueryableInt(
            Kind kind,
            DataSet dataSet,
            Action<IQueryableField<Country, int, TestUserContext>> configure,
            string queryFragment,
            string resultFragment)
        {
            FromQueryable(
                kind,
                configure,
                queryFragment,
                resultFragment,
                condition: (c, id) => c.Id == id,
                outerQuery: _ => BaseTests.CreateData().Countries.AsQueryable(),
                outerIdExpression: country => country.Id,
                innerQuery: GetDataFactory(dataSet, p => p.Id));
        }

        [TestCaseSource(typeof(FromQueryableTests), nameof(IntClassTestData))]
        public void FromQueryableIntClass(
            Kind kind,
            DataSet dataSet,
            Action<IQueryableField<Country, int, TestUserContext>> configure,
            string queryFragment,
            string resultFragment)
        {
            FromQueryable(
                kind,
                configure,
                queryFragment,
                resultFragment,
                condition: (c, id) => c.Id == id,
                outerQuery: _ => BaseTests.CreateData().Countries.AsQueryable(),
                outerIdExpression: country => country.Id,
                innerQuery: GetDataFactory(dataSet, p => p.Id),
                builder => builder.Field("id", id => id));
        }

        [TestCaseSource(typeof(FromQueryableTests), nameof(NullableIntTestData))]
        public void RootFromQueryableInt(
            Kind kind,
            DataSet dataSet,
            Action<IQueryableField<Country, int?, TestUserContext>> configure,
            string queryFragment,
            string resultFragment)
        {
            FromQueryable(
                kind,
                configure,
                queryFragment,
                resultFragment,
                condition: (c, id) => c.Id == id,
                outerQuery: _ => BaseTests.CreateData().Countries.AsQueryable(),
                outerIdExpression: country => country.Id,
                innerQuery: GetDataFactory(dataSet, p => p.ManagerId));
        }

        [TestCaseSource(typeof(FromQueryableTests), nameof(FromLoaderClassTestData))]
        public void FromLoaderFromQueryableClass(
            DataSet dataSet,
            Action<IQueryableField<Country, Person, TestUserContext>> configure,
            string queryFragment,
            string resultFragment,
            Action<IInlineObjectBuilder<Person, TestUserContext>> configureQueryable = null)
        {
            FromLoaderFromQueryable(
                configure,
                configureQueryable,
                queryFragment,
                resultFragment,
                (c, p) => c.Id == p.CountryId,
                c => c.Id,
                GetDataFactory(dataSet, person => person));

            FromLoaderFromQueryable(
                configure,
                configureQueryable,
                queryFragment,
                resultFragment,
                (c, p) => c.NullableId == p.CountryId,
                c => c.NullableId,
                GetDataFactory(dataSet, person => person));

            FromLoaderFromQueryable(
                configure,
                configureQueryable,
                queryFragment,
                resultFragment,
                (c, p) => c.Id == p.NullableCountryId,
                c => c.Id,
                GetDataFactory(dataSet, person => person));

            FromLoaderFromQueryable(
                configure,
                configureQueryable,
                queryFragment,
                resultFragment,
                (c, p) => c.NullableId == p.NullableCountryId,
                c => c.NullableId,
                GetDataFactory(dataSet, person => person));
        }

        [TestCaseSource(typeof(FromQueryableTests), nameof(FromBatchClassTestData))]
        public void FromBatchFromQueryableClass(
            DataSet dataSet,
            Action<IQueryableField<Country, Person, TestUserContext>> configure,
            string queryFragment,
            string resultFragment,
            Action<IInlineObjectBuilder<Person, TestUserContext>> configureQueryable = null)
        {
            FromLoaderFromBatchFromQueryable(
                configure,
                configureQueryable,
                queryFragment,
                resultFragment,
                (c, p) => c.Id == p.CountryId,
                c => c.Id,
                GetDataFactory(dataSet, person => person));

            FromLoaderFromBatchFromQueryable(
                configure,
                configureQueryable,
                queryFragment,
                resultFragment,
                (c, p) => c.NullableId == p.CountryId,
                c => c.NullableId,
                GetDataFactory(dataSet, person => person));

            FromLoaderFromBatchFromQueryable(
                configure,
                configureQueryable,
                queryFragment,
                resultFragment,
                (c, p) => c.Id == p.NullableCountryId,
                c => c.Id,
                GetDataFactory(dataSet, person => person));

            FromLoaderFromBatchFromQueryable(
                configure,
                configureQueryable,
                queryFragment,
                resultFragment,
                (c, p) => c.NullableId == p.NullableCountryId,
                c => c.NullableId,
                GetDataFactory(dataSet, person => person));
        }

        [TestCaseSource(typeof(FromQueryableTests), nameof(FromQueryableClassTestData))]
        public void FromQueryableFromQueryableClass(
            DataSet dataSet,
            Action<IQueryableField<Country, Person, TestUserContext>> configure,
            string queryFragment,
            string resultFragment,
            Action<IInlineObjectBuilder<Person, TestUserContext>> configureQueryable = null)
        {
            FromQueryableFromQueryable(
                configure,
                configureQueryable,
                queryFragment,
                resultFragment,
                (c, p) => c.Id == p.CountryId,
                GetDataFactory(dataSet, person => person));

            FromQueryableFromQueryable(
                configure,
                configureQueryable,
                queryFragment,
                resultFragment,
                (c, p) => c.NullableId == p.CountryId,
                GetDataFactory(dataSet, person => person));

            FromQueryableFromQueryable(
                configure,
                configureQueryable,
                queryFragment,
                resultFragment,
                (c, p) => c.Id == p.NullableCountryId,
                GetDataFactory(dataSet, person => person));

            FromQueryableFromQueryable(
                configure,
                configureQueryable,
                queryFragment,
                resultFragment,
                (c, p) => c.NullableId == p.NullableCountryId,
                GetDataFactory(dataSet, person => person));
        }

        private static void FromQueryable<TOuter, TOuterId, TInner>(
            Kind kind,
            Action<IQueryableField<TOuter, TInner, TestUserContext>> configure,
            string queryFragment,
            string resultFragment,
            Expression<Func<TOuter, TInner, bool>> condition,
            Func<TestUserContext, IQueryable<TOuter>> outerQuery,
            Expression<Func<TOuter, TOuterId>> outerIdExpression,
            Func<TestUserContext, IQueryable<TInner>> innerQuery,
            Action<IInlineObjectBuilder<TInner, TestUserContext>> configureBuilder = null)
            where TOuter : class
            where TOuterId : struct
        {
            configure ??= _ => { };

            switch (kind)
            {
                case Kind.Loader:
                    var loaderType = GraphQLTypeBuilder.CreateLoaderType(
                        onConfigure: loader =>
                        {
                            loader.Field(outerIdExpression);
                            configure(
                                loader.Field("test")
                                    .FromIQueryable(
                                        innerQuery,
                                        condition,
                                        configureBuilder));
                        },
                        getBaseQuery: outerQuery,
                        applyNaturalOrderBy: q => q.OrderBy(outerIdExpression),
                        applyNaturalThenBy: q => q.ThenBy(outerIdExpression));

                    var query = $@"
                        query {{
                            countries {{
                                items {{
                                    id
                                    {queryFragment}
                                }}
                            }}
                        }}";

                    var result = $@"
                        {{
                            countries: {{
                                items: [{{
                                    id: 1,
                                    {resultFragment}
                                }}]
                            }}
                        }}";

                    TestHelpers.TestQuery(
                        query => query.Connection(loaderType, "countries"),
                        query,
                        result,
                        new DataContextMock());

                    break;
                case Kind.Batch:
                    loaderType = GraphQLTypeBuilder.CreateLoaderType(
                        onConfigure: loader =>
                        {
                            loader.Field(outerIdExpression);
                            loader.Field("batch")
                                .FromBatch(
                                    items => items.ToDictionary(item => item, item => item),
                                    batchConfigure => configure(batchConfigure
                                        .Field("test")
                                        .FromIQueryable(
                                            innerQuery,
                                            condition,
                                            configureBuilder)));
                        },
                        getBaseQuery: outerQuery,
                        applyNaturalOrderBy: q => q.OrderBy(outerIdExpression),
                        applyNaturalThenBy: q => q.ThenBy(outerIdExpression));

                    query = $@"
                        query {{
                            countries {{
                                items {{
                                    id
                                    batch {{
                                        {queryFragment}
                                    }}
                                }}
                            }}
                        }}";

                    result = $@"
                        {{
                            countries: {{
                                items: [{{
                                    id: 1,
                                    batch: {{
                                        {resultFragment}
                                    }}
                                }}]
                            }}
                        }}";

                    TestHelpers.TestQuery(
                        query => query.Connection(loaderType, "countries"),
                        query,
                        result,
                        new DataContextMock());

                    break;
                case Kind.Query:
                    query = $@"
                        query {{
                            test {{
                                {queryFragment}
                            }}
                        }}";

                    result = $@"
                        {{
                            test: [{{
                                {resultFragment}
                            }}]
                        }}";

                    TestHelpers.TestQuery(
                        query => query.Field("test").FromIQueryable(
                            outerQuery,
                            builder => configure(
                                builder
                                    .Field("test")
                                    .FromIQueryable(
                                        innerQuery,
                                        condition,
                                        configureBuilder))),
                        query,
                        result,
                        new DataContextMock());

                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static void FromLoaderFromQueryable<TOuterId, TInner>(
            Action<IQueryableField<Country, TInner, TestUserContext>> configure,
            Action<IInlineObjectBuilder<TInner, TestUserContext>> configureQueryable,
            string queryFragment,
            string resultFragment,
            Expression<Func<Country, TInner, bool>> condition,
            Expression<Func<Country, TOuterId>> outerIdExpression,
            Func<TestUserContext, IQueryable<TInner>> innerQuery)
            where TInner : class
        {
            configure ??= _ => { };

            var loaderType = GraphQLTypeBuilder.CreateLoaderType<Country, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(c => c.Id);
                    configure(
                        loader.Field("test")
                            .FromIQueryable(
                                innerQuery,
                                condition,
                                configureQueryable));
                },
                getBaseQuery: _ => BaseTests.CreateData().Countries.AsQueryable(),
                applyNaturalOrderBy: q => q.OrderBy(outerIdExpression),
                applyNaturalThenBy: q => q.ThenBy(outerIdExpression));

            var query = $@"
                query {{
                    countries {{
                        items {{
                            id
                            {queryFragment}
                        }}
                    }}
                }}";

            var result = $@"
                {{
                    countries: {{
                        items: [{{
                            id: 1,
                            {resultFragment}
                        }}]
                    }}
                }}";

            TestHelpers.TestQuery(
                query => query.Connection(loaderType, "countries"),
                query,
                result,
                new DataContextMock());
        }

        private static void FromLoaderFromBatchFromQueryable<TOuterId, TInner>(
            Action<IQueryableField<Country, TInner, TestUserContext>> configure,
            Action<IInlineObjectBuilder<TInner, TestUserContext>> configureQueryable,
            string queryFragment,
            string resultFragment,
            Expression<Func<Country, TInner, bool>> condition,
            Expression<Func<Country, TOuterId>> outerIdExpression,
            Func<TestUserContext, IQueryable<TInner>> innerQuery)
            where TInner : class
        {
            configure ??= _ => { };

            var loaderType = GraphQLTypeBuilder.CreateLoaderType<Country, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(c => c.Id);
                    loader.Field("batch")
                        .FromBatch(
                            items => items.ToDictionary(item => item, items => items),
                            batchConfigure => configure(batchConfigure
                                .Field("test")
                                .FromIQueryable(
                                    innerQuery,
                                    condition,
                                    configureQueryable)));
                },
                getBaseQuery: _ => BaseTests.CreateData().Countries.AsQueryable(),
                applyNaturalOrderBy: q => q.OrderBy(outerIdExpression),
                applyNaturalThenBy: q => q.ThenBy(outerIdExpression));

            var query = $@"
                query {{
                    countries {{
                        items {{
                            id
                            batch {{
                                {queryFragment}
                            }}
                        }}
                    }}
                }}";

            var result = $@"
                {{
                    countries: {{
                        items: [{{
                            id: 1,
                            batch: {{
                                {resultFragment}
                            }}
                        }}]
                    }}
                }}";

            TestHelpers.TestQuery(
                query => query.Connection(loaderType, "countries"),
                query,
                result,
                new DataContextMock());
        }

        private static void FromQueryableFromQueryable<TInner>(
            Action<IQueryableField<Country, TInner, TestUserContext>> configure,
            Action<IInlineObjectBuilder<TInner, TestUserContext>> configureQueryable,
            string queryFragment,
            string resultFragment,
            Expression<Func<Country, TInner, bool>> condition,
            Func<TestUserContext, IQueryable<TInner>> innerQuery)
            where TInner : class
        {
            configure ??= _ => { };

            var query = $@"
                query {{
                    test {{
                        {queryFragment}
                    }}
                }}";

            var result = $@"
                {{
                    test: [{{
                        {resultFragment}
                    }}]
                }}";

            TestHelpers.TestQuery(
                query => query.Field("test").FromIQueryable(
                    _ => BaseTests.CreateData().Countries.AsQueryable(),
                    builder => configure(
                        builder
                            .Field("test")
                            .FromIQueryable(
                                innerQuery,
                                condition,
                                configureQueryable))),
                query,
                result,
                new DataContextMock());
        }

        private static void RootFromQueryable<T>(
            DataSet dataSet,
            Action<IRootQueryableField<T, TestUserContext>> configure,
            string queryFragment,
            string resultFragment,
            Expression<Func<Person, T>> selector,
            Action<IInlineObjectBuilder<T, TestUserContext>> configureBuilder = null)
        {
            var factory = GetDataFactory(dataSet, selector);

            var query = $@"
                query {{
                    {queryFragment}
                }}";

            var result = $@"{{
                {resultFragment}
            }}";

            configure ??= _ => { };

            TestHelpers.TestQuery(
                query => configure(query.Field("test").FromIQueryable(factory, configureBuilder)),
                query,
                result,
                new DataContextMock());
        }

        private static IEnumerable<TestCaseData> GetClassTestData(Kind kind)
        {
            return Impl(kind).SelectMany(x => x);

            static IEnumerable<IEnumerable<TestCaseData>> Impl(Kind kind)
            {
                yield return Create(
                    kind,
                    DataSet.All,
                    null,
                    null,
                    @"{
                        id
                    }",
                    @"[{
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
                    }]");

                yield return Create(
                    kind,
                    DataSet.All,
                    null,
                    null,
                    @"{
                        id
                    }",
                    @"[{
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
                    }]",
                    configureQueryable: builder => builder.Field(x => x.Id, null));

                yield return Enumerable.Repeat(
                    CreateImpl<IRootQueryableField<Person, TestUserContext>, IQueryableField<Country, Person, TestUserContext>>(
                        kind,
                        DataSet.All,
                        null,
                        null,
                        @"{
                        id
                        }",
                        @"[{
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
                        }]",
                        args: null,
                        configureQueryable: builder => builder.Field("id", (_, x) => x.Id, null),
                        asConnection: false),
                    1);

                yield return Enumerable.Repeat(
                    CreateImpl<IRootQueryableField<Person, TestUserContext>, IQueryableField<Country, Person, TestUserContext>>(
                        kind,
                        DataSet.All,
                        null,
                        null,
                        @"{
                            subordinates {
                                id
                            }
                        }",
                        @"[{
                            subordinates: null
                        }, {
                            subordinates: null
                        }, {
                            subordinates: null
                        }, {
                            subordinates: null
                        }, {
                            subordinates: null
                        }, {
                            subordinates: null
                        }]",
                        args: null,
                        configureQueryable: builder => builder.Field(x => (IEnumerable<Person>)x.Subordinates, null),
                        asConnection: false),
                    1);

                yield return Enumerable.Repeat(
                    CreateImpl<IRootQueryableField<Person, TestUserContext>, IQueryableField<Country, Person, TestUserContext>>(
                        kind,
                        DataSet.All,
                        null,
                        null,
                        @"{
                            subordinates {
                                id
                            }
                        }",
                        @"[{
                            subordinates: null
                        }, {
                            subordinates: null
                        }, {
                            subordinates: null
                        }, {
                            subordinates: null
                        }, {
                            subordinates: null
                        }, {
                            subordinates: null
                        }]",
                        args: null,
                        configureQueryable: builder => builder.Field("subordinates", x => (IEnumerable<Person>)x.Subordinates, null),
                        asConnection: false),
                    1);

                yield return Enumerable.Repeat(
                    CreateImpl<IRootQueryableField<Person, TestUserContext>, IQueryableField<Country, Person, TestUserContext>>(
                        kind,
                        DataSet.All,
                        null,
                        null,
                        @"{
                            subordinates {
                                id
                            }
                        }",
                        @"[{
                            subordinates: null
                        }, {
                            subordinates: null
                        }, {
                            subordinates: null
                        }, {
                            subordinates: null
                        }, {
                            subordinates: null
                        }, {
                            subordinates: null
                        }]",
                        args: null,
                        configureQueryable: builder => builder.Field("subordinates", (_, x) => (IEnumerable<Person>)x.Subordinates, null),
                        asConnection: false),
                    1);

                yield return Create(
                    kind,
                    DataSet.All,
                    null,
                    null,
                    @"{
                        id
                    }",
                    @"[{
                        id: 1
                    }, {
                        id: 2
                    }]",
                    "filter: {id: { in: [1, 2] } }",
                    builder => builder.Field(x => x.Id, null).Filterable());

                yield return Create(
                    kind,
                    DataSet.All,
                    null,
                    null,
                    @"{
                        id
                    }",
                    @"[{
                        id: 1
                    }, {
                        id: 2
                    }]",
                    configureQueryable: builder => builder.Field(x => x.Id, null).Filterable(new[] { 1, 2 }));

                yield return Create(
                    kind,
                    DataSet.All,
                    null,
                    null,
                    @"{
                        id
                    }",
                    @"[{
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
                    }]",
                    @"sorting: { field: ""id"", direction: DESC }",
                    builder => builder.Field(x => x.Id, null).Sortable(),
                    hasSorting: true);

                yield return Create(
                    kind,
                    DataSet.All,
                    null,
                    null,
                    @"{
                        id
                    }",
                    @"[{
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
                    }]",
                    @"sorting: { field: ""id"", direction: DESC }",
#pragma warning disable CA1305 // Specify IFormatProvider
                    builder => builder.Field(x => x.Id, null).Sortable(x => x.Id.ToString()),
                    hasSorting: true);
#pragma warning restore CA1305 // Specify IFormatProvider

                yield return Create(
                    kind,
                    DataSet.All,
                    null,
                    null,
                    @"{
                        id
                    }",
                    @"[{
                        id: 2
                    }, {
                        id: 1
                    }]",
                    @"filter: {id: { in: [1, 2] } }, sorting: { field: ""id"", direction: DESC }",
                    builder => builder.Field(x => x.Id, null).Filterable().Sortable(),
                    hasSorting: true);

                yield return Create(
                    kind,
                    DataSet.All,
                    builder => builder
                        .Where(x => x.UnitId == 1),
                    builder => builder
                        .Where(x => x.UnitId == 1),
                    @"{
                        id
                    }",
                    @"[{
                        id: 1
                    }, {
                        id: 2
                    }, {
                        id: 3
                    }]");

                yield return Create(
                    kind,
                    DataSet.All,
                    builder => builder
                        .Where(x => x.UnitId == 1)
                        .Where(x => x.Id == 1),
                    builder => builder
                        .Where(x => x.UnitId == 1)
                        .Where(x => x.Id == 1),
                    @"{
                        id
                    }",
                    @"[{
                        id: 1
                    }]");

                yield return Create(
                    kind,
                    DataSet.All,
                    builder => builder
                        .Where(x => x.UnitId == -1),
                    builder => builder
                        .Where(x => x.UnitId == -1),
                    @"{
                        id
                    }",
                    "[]");
            }

            static IEnumerable<TestCaseData> Create(
                Kind kind,
                DataSet dataSet,
                Expression<Func<IRootQueryableField<Person, TestUserContext>, IRootQueryableField<Person, TestUserContext>>> rootConfigure,
                Expression<Func<IQueryableField<Country, Person, TestUserContext>, IQueryableField<Country, Person, TestUserContext>>> configure,
                string query,
                string result,
                string args = null,
                Expression<Action<IInlineObjectBuilder<Person, TestUserContext>>> configureQueryable = null,
                bool hasSorting = false)
            {
                yield return CreateImpl(
                    kind,
                    dataSet,
                    rootConfigure,
                    configure,
                    query,
                    result,
                    args,
                    configureQueryable,
                    false);

                if (!hasSorting)
                {
                    yield return CreateImpl(
                        kind,
                        dataSet,
                        rootConfigure == null
                            ? field => field.Select(x => x, null)
                            : ExpressionHelpers.Compose(rootConfigure, field => field.Select(x => x, null)),
                        configure == null
                            ? field => field.Select(x => x, null)
                            : ExpressionHelpers.Compose(configure, field => field.Select(x => x, null)),
                        query,
                        result,
                        args,
                        configureQueryable,
                        false);

                    yield return CreateImpl(
                        kind,
                        dataSet,
                        rootConfigure == null
                            ? field => field.Select(x => x, null).Select(x => x, null)
                            : ExpressionHelpers.Compose(rootConfigure, field => field.Select(x => x, null).Select(x => x, null)),
                        configure == null
                            ? field => field.Select(x => x, null).Select(x => x, null)
                            : ExpressionHelpers.Compose(configure, field => field.Select(x => x, null).Select(x => x, null)),
                        query,
                        result,
                        args,
                        configureQueryable,
                        false);

                    yield return CreateImpl(
                        kind,
                        dataSet,
                        rootConfigure == null
                            ? field => field.Select(x => x, configure => configure.Field(x => x.Id, null))
                            : ExpressionHelpers.Compose(
                                rootConfigure,
                                field => field.Select(x => x, configure => configure.Field(x => x.Id, null))),
                        configure == null
                            ? field => field.Select(x => x, configure => configure.Field(x => x.Id, null))
                            : ExpressionHelpers.Compose(
                                configure,
                                field => field.Select(x => x, configure => configure.Field(x => x.Id, null))),
                        query,
                        result,
                        args,
                        configureQueryable,
                        false);

                    yield return CreateImpl(
                        kind,
                        dataSet,
                        rootConfigure == null
                            ? field => field
                                .Select(x => x, configure => configure.Field(x => x.Id, null))
                                .Select(x => x, null)
                            : ExpressionHelpers.Compose(
                                rootConfigure,
                                field => field
                                    .Select(x => x, configure => configure.Field(x => x.Id, null))
                                    .Select(x => x, null)),
                        configure == null
                            ? field => field
                                .Select(x => x, configure => configure.Field(x => x.Id, null))
                                .Select(x => x, null)
                            : ExpressionHelpers.Compose(
                                configure,
                                field => field
                                    .Select(x => x, configure => configure.Field(x => x.Id, null))
                                    .Select(x => x, null)),
                        query,
                        result,
                        args,
                        configureQueryable,
                        false);

                    yield return CreateImpl(
                        kind,
                        dataSet,
                        rootConfigure == null
                            ? field => field
                                .Select(x => x, configure => configure.Field(x => x.Id, null))
                                .Select(x => x, configure => configure.Field(x => x.Id, null))
                            : ExpressionHelpers.Compose(
                                rootConfigure,
                                field => field
                                    .Select(x => x, configure => configure.Field(x => x.Id, null))
                                    .Select(x => x, configure => configure.Field(x => x.Id, null))),
                        configure == null
                            ? field => field
                                .Select(x => x, configure => configure.Field(x => x.Id, null))
                                .Select(x => x, configure => configure.Field(x => x.Id, null))
                            : ExpressionHelpers.Compose(
                                configure,
                                field => field
                                    .Select(x => x, configure => configure.Field(x => x.Id, null))
                                    .Select(x => x, configure => configure.Field(x => x.Id, null))),
                        query,
                        result,
                        args,
                        configureQueryable,
                        false);

                    if (kind != Kind.Root)
                    {
                        yield return CreateNonRootImpl(
                            kind,
                            dataSet,
                            configure == null
                                ? field => field.Select((c, x) => c != null ? x : default, null)
                                : ExpressionHelpers.Compose(configure, field => field.Select((c, x) => c != null ? x : default, null)),
                            query,
                            result,
                            args,
                            configureQueryable,
                            false);

                        yield return CreateNonRootImpl(
                            kind,
                            dataSet,
                            configure == null
                                ? field => field
                                    .Select((c, x) => c != null ? x : default, null)
                                    .Select(x => x, null)
                                : ExpressionHelpers.Compose(
                                    configure,
                                    field => field
                                        .Select((c, x) => c != null ? x : default, null)
                                        .Select(x => x, null)),
                            query,
                            result,
                            args,
                            configureQueryable,
                            false);

                        yield return CreateNonRootImpl(
                            kind,
                            dataSet,
                            configure == null
                                ? field => field
                                    .Select(x => x, configure => configure.Field(x => x.Id, null))
                                    .Select((c, x) => c != null ? x : default, null)
                                : ExpressionHelpers.Compose(
                                    configure,
                                    field => field
                                        .Select(x => x, configure => configure.Field(x => x.Id, null))
                                        .Select((c, x) => c != null ? x : default, null)),
                            query,
                            result,
                            args,
                            configureQueryable,
                            false);

                        yield return CreateNonRootImpl(
                            kind,
                            dataSet,
                            configure == null
                                ? field => field
                                    .Select((c, x) => c != null ? x : default, null)
                                    .Select((c, x) => c != null ? x : default, null)
                                : ExpressionHelpers.Compose(
                                    configure,
                                    field => field
                                        .Select((c, x) => c != null ? x : default, null)
                                        .Select((c, x) => c != null ? x : default, null)),
                            query,
                            result,
                            args,
                            configureQueryable,
                            false);
                    }
                }

                yield return CreateImpl(
                    kind,
                    dataSet,
                    rootConfigure == null
                        ? field => field.AsConnection(query => query.OrderBy(person => person.Id))
                        : ExpressionHelpers.Compose(rootConfigure, field => field.AsConnection(query => query.OrderBy(person => person.Id))),
                    configure == null
                        ? field => field.AsConnection(query => query.OrderBy(person => person.Id))
                        : ExpressionHelpers.Compose(configure, field => field.AsConnection(query => query.OrderBy(person => person.Id))),
                    query,
                    result,
                    args,
                    configureQueryable,
                    true);
            }

            static TestCaseData CreateImpl<TField1, TField2>(
                Kind kind,
                DataSet dataSet,
                Expression<Func<IRootQueryableField<Person, TestUserContext>, TField1>> rootConfigure,
                Expression<Func<IQueryableField<Country, Person, TestUserContext>, TField2>> configure,
                string query,
                string result,
                string args,
                Expression<Action<IInlineObjectBuilder<Person, TestUserContext>>> configureQueryable,
                bool asConnection)
            {
                query = GetQuery(query, args, asConnection);
                result = GetResult(result, asConnection);
                var name = GetTestName("Class", kind, dataSet, configure, configureQueryable);

                if (kind == Kind.Root)
                {
                    return new TestCaseData(
                        dataSet,
                        ToAction(rootConfigure),
                        query,
                        result,
                        configureQueryable?.Compile()).SetName(name);
                }

                return new TestCaseData(
                    dataSet,
                    ToAction(configure),
                    query,
                    result,
                    configureQueryable?.Compile()).SetName(name);
            }

            static TestCaseData CreateNonRootImpl<TField>(
                Kind kind,
                DataSet dataSet,
                Expression<Func<IQueryableField<Country, Person, TestUserContext>, TField>> configure,
                string query,
                string result,
                string args,
                Expression<Action<IInlineObjectBuilder<Person, TestUserContext>>> configureQueryable,
                bool asConnection)
            {
                query = GetQuery(query, args, asConnection);
                result = GetResult(result, asConnection);
                var name = GetTestName("Class", kind, dataSet, configure, configureQueryable);

                return new TestCaseData(
                    dataSet,
                    ToAction(configure),
                    query,
                    result,
                    configureQueryable?.Compile()).SetName(name);
            }
        }

        private static IEnumerable<TestCaseData> GetRootIntTestData()
        {
            return Impl().SelectMany(x => x);

            static IEnumerable<IEnumerable<TestCaseData>> Impl()
            {
                yield return Create<int>(
                    DataSet.All,
                    null,
                    string.Empty,
                    @"[1, 2, 3, 4, 5, 6]");

                var searcher = GraphQLTypeBuilder.CreateSearcherType<int, TestUserContext>((query, user, search) => query.Where(x => x.ToString(CultureInfo.InvariantCulture) == search));

                yield return Create<int>(
                    DataSet.All,
                    builder => builder.WithSearch(searcher),
                    string.Empty,
                    @"[1]",
                    "search: \"1\"");

                yield return CreateRootOnly<int, IConnectionField<int, TestUserContext>>(
                    DataSet.All,
                    builder => ((IConnectionField<int, TestUserContext>)builder.AsConnection(query => query.OrderBy(x => x))).WithSearch(searcher),
                    @"{
                        items
                    }",
                    @"{
                        items: [1]
                    }",
                    "search: \"1\"");

                yield return CreateRootOnly<int, IVoid>(
                    DataSet.All,
                    builder => builder.AsGroupConnection(),
                    @"{
                        items {
                            item
                            count
                        }
                    }",
                    @"{
                        items:[{
                            item: 1,
                            count: 1
                        },{
                            item: 2,
                            count:1
                        },{
                            item: 3,
                            count:1
                        },{
                            item: 4,
                            count:1
                        },{
                            item: 5,
                            count:1
                        },{
                            item: 6,
                            count:1
                        }]
                    }");

                yield return CreateRootOnly<int, IVoid>(
                    DataSet.All,
                    builder => builder.AsGroupConnection(),
                    @"{
                        items {
                            item
                        }
                    }",
                    @"{
                        items:[{
                            item: 1
                        },{
                            item: 2
                        },{
                            item: 3
                        },{
                            item: 4
                        },{
                            item: 5
                        },{
                            item: 6
                        }]
                    }");

                yield return CreateRootOnly<int, IVoid>(
                    DataSet.All,
                    builder => builder.AsGroupConnection(),
                    @"{
                        items {
                            count
                        }
                    }",
                    @"{
                        items:[{
                            count: 6
                        }]
                    }");

                yield return CreateRootOnly<int, IConnectionField<int, TestUserContext>>(
                    DataSet.All,
                    builder => ((IConnectionField<int, TestUserContext>)builder.AsGroupConnection()).WithSearch(searcher),
                    @"{
                    items {
                            item
                            count
                        }
                    }",
                    @"{
                        items:[{
                            item: 1,
                            count: 1
                        }]
                    }",
                    "search: \"1\"");
            }
        }

        private static IEnumerable<TestCaseData> GetRootIntClassTestData()
        {
            return Impl().SelectMany(x => x);

            static IEnumerable<IEnumerable<TestCaseData>> Impl()
            {
                yield return Create<int>(
                    DataSet.All,
                    null,
                    @"{
                        id
                    }",
                    @"[{
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
                    }]",
                    hasInlineBuilder: true);

                var searcher = GraphQLTypeBuilder.CreateSearcherType<int, TestUserContext>((query, user, search) => query.Where(x => x.ToString(CultureInfo.InvariantCulture) == search));

                yield return Create<int>(
                    DataSet.All,
                    builder => builder.WithSearch(searcher),
                    @"{
                        id
                    }",
                    @"[{
                        id: 1
                    }]",
                    "search: \"1\"",
                    hasInlineBuilder: true);

                yield return CreateRootOnly<int, IConnectionField<int, TestUserContext>>(
                    DataSet.All,
                    builder => ((IConnectionField<int, TestUserContext>)builder.AsConnection(query => query.OrderBy(x => x))).WithSearch(searcher),
                    @"{
                        items {
                            id
                        }
                    }",
                    @"{
                        items: [{
                            id: 1
                        }]
                    }",
                    "search: \"1\"");

                yield return CreateRootOnly<int, IVoid>(
                    DataSet.All,
                    builder => builder.AsGroupConnection(),
                    @"{
                        items {
                            item {
                                id
                            }
                            count
                        }
                    }",
                    @"{
                        items:[{
                            item: {
                                id: 1
                            },
                            count: 1
                        },{
                            item: {
                                id: 2
                            },
                            count:1
                        },{
                            item: {
                                id: 3
                            },
                            count:1
                        },{
                            item: {
                                id: 4
                            },
                            count:1
                        },{
                            item: {
                                id: 5
                            },
                            count:1
                        },{
                            item: {
                                id: 6
                            },
                            count:1
                        }]
                    }");

                yield return CreateRootOnly<int, IVoid>(
                    DataSet.All,
                    builder => builder.AsGroupConnection(),
                    @"{
                        items {
                            item {
                                id
                            }
                        }
                    }",
                    @"{
                        items:[{
                            item: {
                                id: 1
                            }
                        },{
                            item: {
                                id: 2
                            }
                        },{
                            item: {
                                id: 3
                            }
                        },{
                            item: {
                                id: 4
                            }
                        },{
                            item: {
                                id: 5
                            }
                        },{
                            item: {
                                id: 6
                            }
                        }]
                    }");

                yield return CreateRootOnly<int, IVoid>(
                    DataSet.All,
                    builder => builder.AsGroupConnection(),
                    @"{
                        items {
                            count
                        }
                    }",
                    @"{
                        items:[{
                            count: 6
                        }]
                    }");

                yield return CreateRootOnly<int, IConnectionField<int, TestUserContext>>(
                    DataSet.All,
                    builder => ((IConnectionField<int, TestUserContext>)builder.AsGroupConnection()).WithSearch(searcher),
                    @"{
                    items {
                            item {
                                id
                            }
                            count
                        }
                    }",
                    @"{
                        items:[{
                            item: {
                                id: 1
                            },
                            count: 1
                        }]
                    }",
                    "search: \"1\"");
            }
        }

        private static IEnumerable<TestCaseData> GetIntTestData(Kind kind)
        {
            return Impl().SelectMany(x => x);

            IEnumerable<IEnumerable<TestCaseData>> Impl()
            {
                yield return CreateNonRootSuite<int>(
                    kind,
                    DataSet.All,
                    null,
                    string.Empty,
                    @"[1]");

                var searcher = GraphQLTypeBuilder.CreateSearcherType<int, TestUserContext>((query, user, search) => query.Where(x => x.ToString(CultureInfo.InvariantCulture) == search));

                yield return CreateNonRootSuite<int>(
                    kind,
                    DataSet.All,
                    builder => builder.WithSearch(searcher),
                    string.Empty,
                    @"[1]",
                    "search: \"1\"");

                yield return Enumerable.Repeat(
                    CreateNonRoot<int, IEnumerableField<Country, int, TestUserContext>>(
                        kind,
                        DataSet.All,
                        builder => builder
                            .Select((country, id) => id, null)
                            .Where(id => true),
                        string.Empty,
                        @"[1]"),
                    1);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int, IEnumerableField<Country, Tuple<int>, TestUserContext>>(
                        kind,
                        DataSet.All,
                        builder => builder
                            .Select(x => Tuple.Create(x), config => config.Field("item", x => x.Item1, null)),
                        "{item}",
                        @"[{item: 1}]"),
                    1);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int, IEnumerableField<Country, Tuple<int>, TestUserContext>>(
                        kind,
                        DataSet.All,
                        builder => builder
                            .Select(x => Tuple.Create(x), config => config.Field("item", x => x.Item1, null))
                            .Select(x => Tuple.Create(x.Item1), config => config.Field("item", x => x.Item1, null)),
                        "{item}",
                        @"[{item: 1}]"),
                    1);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int, IEnumerableField<Country, int, TestUserContext>>(
                        kind,
                        DataSet.All,
                        builder => builder
                            .Select(x => Tuple.Create(x), config => config.Field("item", x => x.Item1, null))
                            .Select((c, x) => c.Id + x.Item1, null),
                        string.Empty,
                        @"[2]"),
                    1);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int, IEnumerableField<Country, int, TestUserContext>>(
                        kind,
                        DataSet.All,
                        builder => builder
                            .Select(x => x, null)
                            .Select((c, x) => c.Id + x, null),
                        string.Empty,
                        @"[2]"),
                    1);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int, IVoid>(
                        kind,
                        DataSet.All,
                        builder => builder.FirstOrDefault(null),
                        string.Empty,
                        @"1"),
                    1);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int, IVoid>(
                        kind,
                        DataSet.All,
                        builder => builder.SingleOrDefault(null),
                        string.Empty,
                        @"1"),
                    1);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int, IVoid>(
                        kind,
                        DataSet.All,
                        builder => builder.FirstOrDefault(x => true),
                        string.Empty,
                        @"1"),
                    1);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int, IVoid>(
                        kind,
                        DataSet.All,
                        builder => builder.SingleOrDefault(x => true),
                        string.Empty,
                        @"1"),
                    1);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int, IVoid>(
                        kind,
                        DataSet.All,
                        builder => builder
                            .Select(x => x, null)
                            .FirstOrDefault(null),
                        string.Empty,
                        @"1"),
                    1);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int, IVoid>(
                        kind,
                        DataSet.All,
                        builder => builder
                            .Select(x => x, null)
                            .SingleOrDefault(null),
                        string.Empty,
                        @"1"),
                    1);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int, IVoid>(
                        kind,
                        DataSet.All,
                        builder => builder
                            .Select(x => x, null)
                            .FirstOrDefault(x => true),
                        string.Empty,
                        @"1"),
                    1);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int, IVoid>(
                        kind,
                        DataSet.All,
                        builder => builder
                            .Select(x => x, null)
                            .SingleOrDefault(x => true),
                        string.Empty,
                        @"1"),
                    1);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int, IVoid>(
                        kind,
                        DataSet.All,
                        builder => builder
                            .Select(x => Tuple.Create(x), config => config.Field("item", x => x.Item1, null))
                            .SingleOrDefault(null),
                        @"{item}",
                        @"{item: 1}"),
                    1);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int, IVoid>(
                        kind,
                        DataSet.All,
                        builder => builder
                            .Select(x => Tuple.Create(x), config => config.Field("item", x => x.Item1, null))
                            .FirstOrDefault(x => true),
                        @"{item}",
                        @"{item: 1}"),
                    1);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int, IVoid>(
                        kind,
                        DataSet.All,
                        builder => builder
                            .Select(x => Tuple.Create(x), config => config.Field("item", x => x.Item1, null))
                            .SingleOrDefault(x => true),
                        @"{item}",
                        @"{item: 1}"),
                    1);
            }
        }

        private static IEnumerable<TestCaseData> GetIntClassTestData(Kind kind)
        {
            return Impl().SelectMany(x => x);

            IEnumerable<IEnumerable<TestCaseData>> Impl()
            {
                yield return CreateNonRootSuite<int>(
                    kind,
                    DataSet.All,
                    null,
                    @"{
                        id
                    }",
                    @"[{
                        id: 1
                    }]",
                    hasInlineBuilder: true);

                var searcher = GraphQLTypeBuilder.CreateSearcherType<int, TestUserContext>((query, user, search) => query.Where(x => x.ToString(CultureInfo.InvariantCulture) == search));

                yield return CreateNonRootSuite<int>(
                    kind,
                    DataSet.All,
                    builder => builder.WithSearch(searcher),
                    @"{
                        id
                    }",
                    @"[{
                        id: 1
                    }]",
                    "search: \"1\"",
                    hasInlineBuilder: true);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int, IVoid>(
                        kind,
                        DataSet.All,
                        builder => builder.FirstOrDefault(null),
                        @"{
                            id
                        }",
                        @"{
                            id: 1
                        }"),
                    1);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int, IVoid>(
                        kind,
                        DataSet.All,
                        builder => builder.SingleOrDefault(null),
                        @"{
                            id
                        }",
                        @"{
                            id: 1
                        }"),
                    1);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int, IVoid>(
                        kind,
                        DataSet.All,
                        builder => builder.FirstOrDefault(x => true),
                        @"{
                            id
                        }",
                        @"{
                            id: 1
                        }"),
                    1);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int, IVoid>(
                        kind,
                        DataSet.All,
                        builder => builder.SingleOrDefault(x => true),
                        @"{
                            id
                        }",
                        @"{
                            id: 1
                        }"),
                    1);
            }
        }

        private static IEnumerable<TestCaseData> GetRootNullableIntTestData()
        {
            return Impl().SelectMany(x => x);

            static IEnumerable<IEnumerable<TestCaseData>> Impl()
            {
                yield return Create<int?>(
                    DataSet.All,
                    null,
                    string.Empty,
                    @"[null, 1, 1, 2, 2, 5]");

                var searcher = GraphQLTypeBuilder.CreateSearcherType<int?, TestUserContext>((query, user, search) => query.Where(x => x.ToString() == search));

                yield return Create<int?>(
                    DataSet.All,
                    builder => builder.WithSearch(searcher),
                    string.Empty,
                    @"[1, 1]",
                    "search: \"1\"");

                yield return CreateRootOnly<int?, IConnectionField<int?, TestUserContext>>(
                    DataSet.All,
                    builder => ((IConnectionField<int?, TestUserContext>)builder.AsConnection(query => query.OrderBy(x => x))).WithSearch(searcher),
                    @"{
                        items
                    }",
                    @"{
                        items: [1, 1]
                    }",
                    "search: \"1\"");

                yield return CreateRootOnly<int?, IVoid>(
                    DataSet.All,
                    builder => builder.AsGroupConnection(),
                    @"{
                        items {
                            item
                            count
                        }
                    }",
                    @"{
                        items:[{
                            item: null,
                            count: 1
                        },{
                            item: 1,
                            count:2
                        },{
                            item: 2,
                            count:2
                        },{
                            item: 5,
                            count:1
                        }]
                    }");

                yield return CreateRootOnly<int?, IVoid>(
                    DataSet.All,
                    builder => builder.AsGroupConnection(),
                    @"{
                        items {
                            item
                        }
                    }",
                    @"{
                        items:[{
                            item: null
                        },{
                            item: 1
                        },{
                            item: 2
                        },{
                            item: 5
                        }]
                    }");

                yield return CreateRootOnly<int?, IVoid>(
                    DataSet.All,
                    builder => builder.AsGroupConnection(),
                    @"{
                        items {
                            count
                        }
                    }",
                    @"{
                        items:[{
                            count: 6
                        }]
                    }");
            }
        }

        private static IEnumerable<TestCaseData> GetNullableIntTestData(Kind kind)
        {
            return Impl().SelectMany(x => x);

            IEnumerable<IEnumerable<TestCaseData>> Impl()
            {
                yield return CreateNonRootSuite<int?>(
                    kind,
                    DataSet.All,
                    null,
                    string.Empty,
                    @"[1, 1]");

                var searcher = GraphQLTypeBuilder.CreateSearcherType<int?, TestUserContext>((query, user, search) => query.Where(x => x.ToString() == search));

                yield return CreateNonRootSuite<int?>(
                    kind,
                    DataSet.All,
                    builder => builder.WithSearch(searcher),
                    string.Empty,
                    @"[1, 1]",
                    "search: \"1\"");

                yield return Enumerable.Repeat(
                    CreateNonRoot<int?, IEnumerableField<Country, int?, TestUserContext>>(
                        kind,
                        DataSet.All,
                        builder => builder
                            .Select((country, id) => id, null)
                            .Where(id => true),
                        string.Empty,
                        @"[1,1]"),
                    1);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int?, IEnumerableField<Country, Tuple<int?>, TestUserContext>>(
                        kind,
                        DataSet.All,
                        builder => builder
                            .Select(x => Tuple.Create(x), config => config.Field("item", x => x.Item1, null)),
                        "{item}",
                        @"[{item: 1}, {item: 1}]"),
                    1);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int?, IEnumerableField<Country, Tuple<int?>, TestUserContext>>(
                        kind,
                        DataSet.All,
                        builder => builder
                            .Select(x => Tuple.Create(x), config => config.Field("item", x => x.Item1, null))
                            .Select(x => Tuple.Create(x.Item1), config => config.Field("item", x => x.Item1, null)),
                        "{item}",
                        @"[{item: 1}, {item: 1}]"),
                    1);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int?, IEnumerableField<Country, int?, TestUserContext>>(
                        kind,
                        DataSet.All,
                        builder => builder
                            .Select(x => Tuple.Create(x), config => config.Field("item", x => x.Item1, null))
                            .Select((c, x) => c.NullableId + x.Item1, null),
                        string.Empty,
                        @"[2, 2]"),
                    1);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int?, IVoid>(
                        kind,
                        DataSet.All,
                        builder => builder.FirstOrDefault(null),
                        string.Empty,
                        @"1"),
                    1);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int?, IVoid>(
                        kind,
                        DataSet.All,
                        builder => builder.FirstOrDefault(x => true),
                        string.Empty,
                        @"1"),
                    1);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int?, IVoid>(
                        kind,
                        DataSet.All,
                        builder => builder
                            .Select(x => x, null)
                            .FirstOrDefault(null),
                        string.Empty,
                        @"1"),
                    1);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int?, IVoid>(
                        kind,
                        DataSet.All,
                        builder => builder
                            .Select(x => x, null)
                            .FirstOrDefault(x => true),
                        string.Empty,
                        @"1"),
                    1);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int?, IVoid>(
                        kind,
                        DataSet.All,
                        builder => builder
                            .Select(x => Tuple.Create(x), config => config.Field("item", x => x.Item1, null))
                            .FirstOrDefault(null),
                        @"{item}",
                        @"{item: 1}"),
                    1);

                yield return Enumerable.Repeat(
                    CreateNonRoot<int?, IVoid>(
                        kind,
                        DataSet.All,
                        builder => builder
                            .Select(x => Tuple.Create(x), config => config.Field("item", x => x.Item1, null))
                            .FirstOrDefault(x => true),
                        @"{item}",
                        @"{item: 1}"),
                    1);
            }
        }

        private static IEnumerable<TestCaseData> GetClassTestData2(Kind kind)
        {
            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.Id, null),
                builder => builder
                    .Select(x => x.Id, null),
                string.Empty,
                "[1, 2, 3, 4, 5, 6]");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.ManagerId, null),
                builder => builder
                    .Select(x => x.ManagerId, null),
                string.Empty,
                "[null, 1, 1, 2, 2, 5]");

            if (kind != Kind.Root)
            {
                yield return CreateNonRoot<Person, IEnumerableField<Country, int?, TestUserContext>>(
                    kind,
                    DataSet.All,
                    builder => builder
                        .Select(x => x.ManagerId, null)
                        .Select((c, managerId) => c.Id + managerId, null),
                    string.Empty,
                    "[null, 2, 2, 3, 3, 6]",
                    null,
                    null);
            }

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.FullName, null),
                builder => builder
                    .Select(x => x.FullName, null),
                string.Empty,
                @"[""Linoel Livermore"", ""Sophie Gandley"", ""Hannie Everitt"", ""Florance Goodricke"", ""Aldon Exley"", ""Walton Alvarez""]");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.Unit, null),
                builder => builder
                    .Select(x => x.Unit, null),
                @"{
                    id
                }",
                @"[{
                    id: 1
                }, {
                    id: 1
                }, {
                    id: 1
                },{
                    id: 2
                }, {
                    id: 2
                }, {
                    id: 2
                }]");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.Unit, inlineBuilder => inlineBuilder.Field(x => x.Id, null)),
                builder => builder
                    .Select(x => x.Unit, inlineBuilder => inlineBuilder.Field(x => x.Id, null)),
                @"{
                    id
                }",
                @"[{
                    id: 1
                }, {
                    id: 1
                }, {
                    id: 1
                },{
                    id: 2
                }, {
                    id: 2
                }, {
                    id: 2
                }]");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(
                        x => new Person
                        {
                            Id = x.Id,
                        },
                        null),
                builder => builder
                    .Select(
                        x => new Person
                        {
                            Id = x.Id,
                        },
                        null),
                @"{
                    id
                }",
                @"[{
                    id: 1
                }, {
                    id: 2
                }, {
                    id: 3
                },{
                    id: 4
                }, {
                    id: 5
                }, {
                    id: 6
                }]");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(
                        x => new
                        {
                            x.Id,
                        },
                        null),
                builder => builder
                    .Select(
                        x => new
                        {
                            x.Id,
                        },
                        null),
                @"{
                    id
                }",
                @"[{
                    id: 1
                }, {
                    id: 2
                }, {
                    id: 3
                },{
                    id: 4
                }, {
                    id: 5
                }, {
                    id: 6
                }]");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder.Select(x => x, null),
                builder => builder.Select(x => x, null),
                @"{
                    id
                }",
                @"[{
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
                }]",
                @"sorting: { field: ""id"", direction: DESC }",
                builder => builder.Field(x => x.Id, null).Sortable(x => x.Id));

            if (kind != Kind.Root)
            {
                yield return CreateNonRoot<Person, IEnumerableField<Country, int, TestUserContext>>(
                    kind,
                    DataSet.All,
                    builder => builder.Select((c, x) => x.Id + c.Id, null),
                    string.Empty,
                    @"[7, 6, 5, 4, 3, 2]",
                    @"sorting: { field: ""id"", direction: DESC }",
                    builder => builder.Field(x => x.Id, null).Sortable(x => x.Id));
            }

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.Id, null)
                    .Select(id => 2 * id, null),
                builder => builder
                    .Select(x => x.Id, null)
                    .Select(id => 2 * id, null),
                string.Empty,
                "[2, 4, 6, 8, 10, 12]");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.ManagerId, null)
                    .Select(managerId => 2 * managerId, null),
                builder => builder
                    .Select(x => x.ManagerId, null)
                    .Select(managerId => 2 * managerId, null),
                string.Empty,
                "[null, 2, 2, 4, 4, 10]");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.FullName, null)
                    .Select(fullName => $"*{fullName}", null),
                builder => builder
                    .Select(x => x.FullName, null)
                    .Select(fullName => $"*{fullName}", null),
                string.Empty,
                @"[""*Linoel Livermore"", ""*Sophie Gandley"", ""*Hannie Everitt"", ""*Florance Goodricke"", ""*Aldon Exley"", ""*Walton Alvarez""]");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.Unit, null)
                    .Select(u => u.Id, null),
                builder => builder
                    .Select(x => x.Unit, null)
                    .Select(u => u.Id, null),
                string.Empty,
                "[1, 1, 1, 2, 2, 2]");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.Unit, null)
                    .Select(u => u.ParentId, null),
                builder => builder
                    .Select(x => x.Unit, null)
                    .Select(u => u.ParentId, null),
                string.Empty,
                "[null, null, null, 1, 1, 1]");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.Unit, null)
                    .Select(u => u.Name, null),
                builder => builder
                    .Select(x => x.Unit, null)
                    .Select(u => u.Name, null),
                string.Empty,
                @"[""Alpha"", ""Alpha"", ""Alpha"", ""Beta"", ""Beta"", ""Beta""]");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.Unit, null)
                    .Select(
                        u => new Person
                        {
                            Id = u.Id,
                        },
                        null),
                builder => builder
                    .Select(x => x.Unit, null)
                    .Select(
                        u => new Person
                        {
                            Id = u.Id,
                        },
                        null),
                @"{
                    id
                }",
                @"[{
                    id: 1
                }, {
                    id: 1
                }, {
                    id: 1
                },{
                    id: 2
                }, {
                    id: 2
                }, {
                    id: 2
                }]");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.Unit, null)
                    .Select(
                        u => new Person
                        {
                            Id = u.Id,
                        },
                        inlineBuilder => inlineBuilder.Field(p => p.Id, null)),
                builder => builder
                    .Select(x => x.Unit, null)
                    .Select(
                        u => new Person
                        {
                            Id = u.Id,
                        },
                        inlineBuilder => inlineBuilder.Field(p => p.Id, null)),
                @"{
                    id
                }",
                @"[{
                    id: 1
                }, {
                    id: 1
                }, {
                    id: 1
                },{
                    id: 2
                }, {
                    id: 2
                }, {
                    id: 2
                }]");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.Unit, null)
                    .Select(
                        u => new
                        {
                            u.Id,
                        },
                        null),
                builder => builder
                    .Select(x => x.Unit, null)
                    .Select(
                        u => new
                        {
                            u.Id,
                        },
                        null),
                @"{
                    id
                }",
                @"[{
                    id: 1
                }, {
                    id: 1
                }, {
                    id: 1
                },{
                    id: 2
                }, {
                    id: 2
                }, {
                    id: 2
                }]");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.Id, null)
                    .Select(x => Tuple.Create(x), config => config.Field("id", x => x.Item1, null)),
                builder => builder
                    .Select(x => x.Id, null)
                    .Select(x => Tuple.Create(x), config => config.Field("id", x => x.Item1, null)),
                @"{
                    id
                }",
                @"[{
                    id: 1
                }, {
                    id: 2
                }, {
                    id: 3
                },{
                    id: 4
                }, {
                    id: 5
                }, {
                    id: 6
                }]");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .FirstOrDefault(null),
                builder => builder
                    .FirstOrDefault(null),
                @"{
                    id
                }",
                @"{
                    id: 1
                }");

            yield return Create(
                kind,
                DataSet.One,
                builder => builder
                    .SingleOrDefault(null),
                builder => builder
                    .SingleOrDefault(null),
                @"{
                    id
                }",
                @"{
                    id: 1
                }");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .FirstOrDefault(x => x.Id == 1),
                builder => builder
                    .FirstOrDefault(x => x.Id == 1),
                @"{
                    id
                }",
                @"{
                    id: 1
                }");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .SingleOrDefault(x => x.Id == 1),
                builder => builder
                    .SingleOrDefault(x => x.Id == 1),
                @"{
                    id
                }",
                @"{
                    id: 1
                }");

            yield return Create(
                kind,
                DataSet.Empty,
                builder => builder
                    .FirstOrDefault(null),
                builder => builder
                    .FirstOrDefault(null),
                @"{
                    id
                }",
                "null");

            yield return Create(
                kind,
                DataSet.Empty,
                builder => builder
                    .SingleOrDefault(null),
                builder => builder
                    .SingleOrDefault(null),
                @"{
                    id
                }",
                "null");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.Id, null)
                    .FirstOrDefault(null),
                builder => builder
                    .Select(x => x.Id, null)
                    .FirstOrDefault(null),
                string.Empty,
                "1");

            yield return Create(
                kind,
                DataSet.One,
                builder => builder
                    .Select(x => x.Id, null)
                    .SingleOrDefault(null),
                builder => builder
                    .Select(x => x.Id, null)
                    .SingleOrDefault(null),
                string.Empty,
                "1");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.Id, null)
                    .FirstOrDefault(i => i == 1),
                builder => builder
                    .Select(x => x.Id, null)
                    .FirstOrDefault(i => i == 1),
                string.Empty,
                "1");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.Id, null)
                    .SingleOrDefault(i => i == 1),
                builder => builder
                    .Select(x => x.Id, null)
                    .SingleOrDefault(i => i == 1),
                string.Empty,
                "1");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.ManagerId, null)
                    .FirstOrDefault(m => m == null),
                builder => builder
                    .Select(x => x.ManagerId, null)
                    .FirstOrDefault(m => m == null),
                string.Empty,
                "null");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.ManagerId, null)
                    .SingleOrDefault(m => m == null),
                builder => builder
                    .Select(x => x.ManagerId, null)
                    .SingleOrDefault(m => m == null),
                string.Empty,
                "null");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.FullName, null)
                    .FirstOrDefault(null),
                builder => builder
                    .Select(x => x.FullName, null)
                    .FirstOrDefault(null),
                string.Empty,
                @"""Linoel Livermore""");

            yield return Create(
                kind,
                DataSet.One,
                builder => builder
                    .Select(x => x.FullName, null)
                    .SingleOrDefault(null),
                builder => builder
                    .Select(x => x.FullName, null)
                    .SingleOrDefault(null),
                string.Empty,
                @"""Linoel Livermore""");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.FullName, null)
                    .FirstOrDefault(n => n == "Linoel Livermore"),
                builder => builder
                    .Select(x => x.FullName, null)
                    .FirstOrDefault(n => n == "Linoel Livermore"),
                string.Empty,
                @"""Linoel Livermore""");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.FullName, null)
                    .SingleOrDefault(n => n == "Linoel Livermore"),
                builder => builder
                    .Select(x => x.FullName, null)
                    .SingleOrDefault(n => n == "Linoel Livermore"),
                string.Empty,
                @"""Linoel Livermore""");

            yield return Create(
                kind,
                DataSet.AllWithManager,
                builder => builder
                    .Select(x => x.Manager, null)
                    .FirstOrDefault(m => m.Id == 1),
                builder => builder
                    .Select(x => x.Manager, null)
                    .FirstOrDefault(m => m.Id == 1),
                @"{
                    id
                }",
                @"{
                    id: 1
                }");

            yield return Create(
                kind,
                DataSet.AllWithManager,
                builder => builder
                    .Select(x => x.Manager, null)
                    .SingleOrDefault(m => m.Id == 5),
                builder => builder
                    .Select(x => x.Manager, null)
                    .SingleOrDefault(m => m.Id == 5),
                @"{
                    id
                }",
                @"{
                    id: 5
                }");

            yield return Create(
                kind,
                DataSet.AllWithManager,
                builder => builder
                    .Select(x => x.Manager, inlineBuilder => inlineBuilder.Field(x => x.Id, null))
                    .FirstOrDefault(m => m.Id == 1),
                builder => builder
                    .Select(x => x.Manager, inlineBuilder => inlineBuilder.Field(x => x.Id, null))
                    .FirstOrDefault(m => m.Id == 1),
                @"{
                    id
                }",
                @"{
                    id: 1
                }");

            yield return Create(
                kind,
                DataSet.AllWithManager,
                builder => builder
                    .Select(x => x.Manager, inlineBuilder => inlineBuilder.Field(x => x.Id, null))
                    .SingleOrDefault(m => m.Id == 5),
                builder => builder
                    .Select(x => x.Manager, inlineBuilder => inlineBuilder.Field(x => x.Id, null))
                    .SingleOrDefault(m => m.Id == 5),
                @"{
                    id
                }",
                @"{
                    id: 5
                }");

            static TestCaseData Create(
                Kind kind,
                DataSet dataSet,
                Expression<Action<IRootQueryableField<Person, TestUserContext>>> rootConfigure,
                Expression<Action<IQueryableField<Country, Person, TestUserContext>>> configure,
                string query,
                string result,
                string args = null,
                Expression<Action<IInlineObjectBuilder<Person, TestUserContext>>> configureQueryable = null)
            {
                query = GetQuery(query, args, false);
                result = GetResult(result, false);
                var name = GetTestName("Class", kind, dataSet, configure, configureQueryable);

                if (kind == Kind.Root)
                {
                    return new TestCaseData(
                        dataSet,
                        rootConfigure?.Compile(),
                        query,
                        result,
                        configureQueryable?.Compile()).SetName(name);
                }

                return new TestCaseData(
                    dataSet,
                    configure?.Compile() ?? (builder => { }),
                    query,
                    result,
                    configureQueryable?.Compile()).SetName(name);
            }
        }

        private static string GetTestName(
            string prefix,
            Kind kind,
            DataSet dataSet,
            Expression configure,
            Expression configureQueryable)
        {
            var cardinality = string.Empty;
            switch (dataSet)
            {
                case DataSet.Empty:
                    cardinality = "Empty";
                    break;
                case DataSet.One:
                    cardinality = "One";
                    break;
                case DataSet.AllWithManager:
                    cardinality = "AllWithManager";
                    break;
                case DataSet.All:
                    cardinality = "All";
                    break;
            }

            var name = configureQueryable == null
                ? $"FromIQueryable({cardinality})"
                : $"FromIQueryable({cardinality}, {configureQueryable.ToTestName()})";

            if (configure != null)
            {
                name = $"{name}․{configure.ToTestName().Replace("builder ⇒ builder․", string.Empty, StringComparison.Ordinal)}";
            }

            switch (kind)
            {
                case Kind.Loader:
                    name = $"FromLoader/{name}";
                    break;
                case Kind.Query:
                    name = $"FromIQueryable/{name}";
                    break;
                case Kind.Batch:
                    name = $"FromBatch/{name}";
                    break;
            }

            return $"{prefix}/{name}";
        }

        private static string GetResult(string result, bool asConnection)
        {
            result = asConnection ? $"{{items: {result}}}" : result;
            result = $"test: {result}";
            return result;
        }

        private static string GetQuery(string query, string args, bool asConnection)
        {
            query = asConnection ? $"{{items {query}}}" : query;
            var test = string.IsNullOrEmpty(args) ? "test" : $"test({args})";
            query = string.IsNullOrEmpty(query) ? test : $"{test} {query}";
            return query;
        }

        private static Action<IRootQueryableField<T, TestUserContext>> ToAction<T, TField>(
            Expression<Func<IRootQueryableField<T, TestUserContext>, TField>> expression)
        {
            return builder =>
            {
                var compiledConfigure = expression?.Compile();
                compiledConfigure?.Invoke(builder);
            };
        }

        private static Action<IQueryableField<T1, T2, TestUserContext>> ToAction<T1, T2, TField>(
            Expression<Func<IQueryableField<T1, T2, TestUserContext>, TField>> expression)
        {
            return builder =>
            {
                var compiledConfigure = expression?.Compile();
                compiledConfigure?.Invoke(builder);
            };
        }

        private static Func<TestUserContext, IQueryable<T>> GetDataFactory<T>(DataSet dataSet, Expression<Func<Person, T>> selector)
        {
            return dataSet switch
            {
                DataSet.Empty => _ => Enumerable.Empty<T>().AsQueryable(),
                DataSet.One => _ => BaseTests.CreateData().People.Take(1).AsQueryable().Select(selector),
                DataSet.AllWithManager => _ => BaseTests.CreateData().People.AsQueryable().Where(x => x.Manager != null).Select(selector),
                DataSet.All => _ => BaseTests.CreateData().People.AsQueryable().Select(selector),
                _ => throw new NotImplementedException(),
            };
        }

        private static IEnumerable<TestCaseData> CreateRootOnly<T, TField>(
            DataSet dataSet,
            Expression<Func<IRootQueryableField<T, TestUserContext>, TField>> configure,
            string query,
            string result,
            string args = null,
            bool asConnection = false)
        {
            query = GetQuery(query, args, asConnection);
            result = GetResult(result, asConnection);
            var name = GetTestName(typeof(T).GraphQLTypeName(false), Kind.Root, dataSet, configure, null);

            return Enumerable.Repeat(
                new TestCaseData(
                    dataSet,
                    ToAction(configure),
                    query,
                    result).SetName(name),
                1);
        }

        private static TestCaseData CreateNonRoot<T, TField>(
            Kind kind,
            DataSet dataSet,
            Expression<Func<IQueryableField<Country, T, TestUserContext>, TField>> configure,
            string query,
            string result,
            string args = null,
            bool asConnection = false)
        {
            query = GetQuery(query, args, asConnection);
            result = GetResult(result, asConnection);
            var testName = GetTestName(typeof(T).GraphQLTypeName(false), kind, dataSet, configure, null);

            return new TestCaseData(
                kind,
                dataSet,
                ToAction(configure),
                query,
                result).SetName(testName);
        }

        private static TestCaseData CreateNonRoot<T, TField>(
            Kind kind,
            DataSet dataSet,
            Expression<Func<IQueryableField<Country, T, TestUserContext>, TField>> configure,
            string query,
            string result,
            string args = null,
            Expression<Action<IInlineObjectBuilder<T, TestUserContext>>> configureQueryable = null)
            where T : class
        {
            query = GetQuery(query, args, false);
            result = GetResult(result, false);
            var testName = GetTestName(typeof(T).GraphQLTypeName(false), kind, dataSet, configure, null);

            return new TestCaseData(
                dataSet,
                ToAction(configure),
                query,
                result,
                configureQueryable?.Compile()).SetName(testName);
        }

        private static IEnumerable<TestCaseData> Create<T>(
            DataSet dataSet,
            Expression<Func<IRootQueryableField<T, TestUserContext>, IRootQueryableField<T, TestUserContext>>> configure,
            string query,
            string result,
            string args = null,
            bool hasInlineBuilder = false)
        {
            yield return CreateRootOnly(
                dataSet,
                configure,
                query,
                result,
                args).Single();

            yield return CreateRootOnly(
                dataSet,
                configure == null
                    ? builder => builder.AsConnection(query => query.OrderBy(x => x))
                    : ExpressionHelpers.Compose(configure, builder => builder.AsConnection(query => query.OrderBy(x => x))),
                query,
                result,
                args,
                asConnection: true).Single();

            if (!hasInlineBuilder)
            {
                yield return CreateRootOnly(
                    dataSet,
                    configure == null
                        ? field => field.Select(x => x, null)
                        : ExpressionHelpers.Compose(configure, field => field.Select(x => x, null)),
                    query,
                    result,
                    args).Single();

                yield return CreateRootOnly(
                    dataSet,
                    configure == null
                        ? field => field
                            .Select(x => x, null)
                            .Select(x => x, null)
                        : ExpressionHelpers.Compose(
                            configure,
                            field => field
                                .Select(x => x, null)
                                .Select(x => x, null)),
                    query,
                    result,
                    args).Single();

                yield return CreateRootOnly(
                    dataSet,
                    configure == null
                        ? field => field
                            .Select(x => Tuple.Create(x), b => b.Field(x => x.Item1, null))
                            .Select(x => x.Item1, null)
                        : ExpressionHelpers.Compose(
                            configure,
                            field => field
                                .Select(x => Tuple.Create(x), b => b.Field(x => x.Item1, null))
                                .Select(x => x.Item1, null)),
                    query,
                    result,
                    args).Single();

                yield return CreateRootOnly(
                    dataSet,
                    configure == null
                        ? field => field
                            .Select(x => Tuple.Create(x), null)
                            .Select(x => x.Item1, null)
                        : ExpressionHelpers.Compose(
                            configure,
                            field => field
                                .Select(x => Tuple.Create(x), null)
                                .Select(x => x.Item1, null)),
                    query,
                    result,
                    args).Single();

                yield return CreateRootOnly(
                    dataSet,
                    configure == null
                        ? field => field
                            .Select(x => x, null)
                            .Select(x => Tuple.Create(x), b => b.Field(x => x.Item1, null))
                            .Select(x => x.Item1, null)
                        : ExpressionHelpers.Compose(
                            configure,
                            field => field
                                .Select(x => x, null)
                                .Select(x => Tuple.Create(x), b => b.Field(x => x.Item1, null))
                                .Select(x => x.Item1, null)),
                    query,
                    result,
                    args).Single();

                yield return CreateRootOnly(
                    dataSet,
                    configure == null
                        ? field => field
                            .Select(x => x, null)
                            .Select(x => Tuple.Create(x), null)
                            .Select(x => x.Item1, null)
                        : ExpressionHelpers.Compose(
                            configure,
                            field => field
                                .Select(x => x, null)
                                .Select(x => Tuple.Create(x), null)
                                .Select(x => x.Item1, null)),
                    query,
                    result,
                    args).Single();
            }
        }

        private static IEnumerable<TestCaseData> CreateNonRootSuite<T>(
            Kind kind,
            DataSet dataSet,
            Expression<Func<IQueryableField<Country, T, TestUserContext>, IQueryableField<Country, T, TestUserContext>>> configure,
            string query,
            string result,
            string args = null,
            bool hasInlineBuilder = false)
        {
            yield return CreateNonRoot(
                kind,
                dataSet,
                configure,
                query,
                result,
                args);

            yield return CreateNonRoot(
                kind,
                dataSet,
                configure == null
                    ? builder => builder.AsConnection(query => query.OrderBy(x => x))
                    : ExpressionHelpers.Compose(configure, builder => builder.AsConnection(query => query.OrderBy(x => x))),
                query,
                result,
                args,
                asConnection: true);

            if (!hasInlineBuilder)
            {
                yield return CreateNonRoot(
                    kind,
                    dataSet,
                    configure == null
                        ? field => field.Select(x => x, null)
                        : ExpressionHelpers.Compose(configure, field => field.Select(x => x, null)),
                    query,
                    result,
                    args);

                yield return CreateNonRoot(
                    kind,
                    dataSet,
                    configure == null
                        ? field => field
                            .Select(x => x, null)
                            .Select(x => x, null)
                        : ExpressionHelpers.Compose(
                            configure,
                            field => field
                                .Select(x => x, null)
                                .Select(x => x, null)),
                    query,
                    result,
                    args);

                yield return CreateNonRoot(
                    kind,
                    dataSet,
                    configure == null
                        ? field => field
                            .Select(x => Tuple.Create(x), b => b.Field(x => x.Item1, null))
                            .Select(x => x.Item1, null)
                        : ExpressionHelpers.Compose(
                            configure,
                            field => field
                                .Select(x => Tuple.Create(x), b => b.Field(x => x.Item1, null))
                                .Select(x => x.Item1, null)),
                    query,
                    result,
                    args);

                yield return CreateNonRoot(
                    kind,
                    dataSet,
                    configure == null
                        ? field => field
                            .Select(x => Tuple.Create(x), null)
                            .Select(x => x.Item1, null)
                        : ExpressionHelpers.Compose(
                            configure,
                            field => field
                                .Select(x => Tuple.Create(x), null)
                                .Select(x => x.Item1, null)),
                    query,
                    result,
                    args);

                yield return CreateNonRoot(
                    kind,
                    dataSet,
                    configure == null
                        ? field => field
                            .Select(x => x, null)
                            .Select(x => Tuple.Create(x), b => b.Field(x => x.Item1, null))
                            .Select(x => x.Item1, null)
                        : ExpressionHelpers.Compose(
                            configure,
                            field => field
                                .Select(x => x, null)
                                .Select(x => Tuple.Create(x), b => b.Field(x => x.Item1, null))
                                .Select(x => x.Item1, null)),
                    query,
                    result,
                    args);

                yield return CreateNonRoot(
                    kind,
                    dataSet,
                    configure == null
                        ? field => field
                            .Select(x => x, null)
                            .Select(x => Tuple.Create(x), null)
                            .Select(x => x.Item1, null)
                        : ExpressionHelpers.Compose(
                            configure,
                            field => field
                                .Select(x => x, null)
                                .Select(x => Tuple.Create(x), null)
                                .Select(x => x.Item1, null)),
                    query,
                    result,
                    args);

                yield return CreateNonRoot(
                    kind,
                    dataSet,
                    configure == null
                        ? field => field.Select((c, x) => c != null ? x : default, null)
                        : ExpressionHelpers.Compose(configure, field => field.Select((c, x) => c != null ? x : default, null)),
                    query,
                    result,
                    args);

                yield return CreateNonRoot(
                    kind,
                    dataSet,
                    configure == null
                        ? field => field
                            .Select((c, x) => c != null ? x : default, null)
                            .Select(x => x, null)
                        : ExpressionHelpers.Compose(configure, field => field
                            .Select((c, x) => c != null ? x : default, null)
                            .Select(x => x, null)),
                    query,
                    result,
                    args);

                yield return CreateNonRoot(
                    kind,
                    dataSet,
                    configure == null
                        ? field => field
                            .Select((c, x) => c != null ? x : default, null)
                            .Select(x => Tuple.Create(x), b => b.Field(x => x.Item1, null))
                            .Select(x => x.Item1, null)
                        : ExpressionHelpers.Compose(configure, field => field
                            .Select((c, x) => c != null ? x : default, null)
                            .Select(x => Tuple.Create(x), b => b.Field(x => x.Item1, null))
                            .Select(x => x.Item1, null)),
                    query,
                    result,
                    args);

                yield return CreateNonRoot(
                    kind,
                    dataSet,
                    configure == null
                        ? field => field
                            .Select((c, x) => c != null ? x : default, null)
                            .Select(x => Tuple.Create(x), null)
                            .Select(x => x.Item1, null)
                        : ExpressionHelpers.Compose(configure, field => field
                            .Select((c, x) => c != null ? x : default, null)
                            .Select(x => Tuple.Create(x), null)
                            .Select(x => x.Item1, null)),
                    query,
                    result,
                    args);
            }
        }
    }
}
