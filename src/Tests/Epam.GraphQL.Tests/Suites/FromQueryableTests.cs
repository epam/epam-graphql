// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Builders.Loader;
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
        }

        public static IEnumerable<TestCaseData> TestData => GetTestData(Kind.Root)
            .Concat(GetTestData(Kind.Loader))
            .Concat(GetTestData(Kind.Query))
            .Concat(GetTestData2(Kind.Root))
            .Concat(GetTestData2(Kind.Loader))
            .Concat(GetTestData2(Kind.Query));

        [TestCaseSource(typeof(FromQueryableTests), nameof(TestData))]
        public void FromQueryable(
            Kind kind,
            DataSet dataSet,
            Action<IFromIQueryableBuilder<Person, TestUserContext>> configure,
            string queryFragment,
            string resultFragment,
            Action<IInlineObjectBuilder<Person, TestUserContext>> configureQueryable = null)
        {
            Func<TestUserContext, IQueryable<Person>> queryFactory = null;
            switch (dataSet)
            {
                case DataSet.Empty:
                    queryFactory = _ => Enumerable.Empty<Person>().AsQueryable();
                    break;
                case DataSet.One:
                    queryFactory = _ => BaseTests.CreateData().People.Take(1).AsQueryable();
                    break;
                case DataSet.AllWithManager:
                    queryFactory = _ => BaseTests.CreateData().People.Where(x => x.Manager != null).AsQueryable();
                    break;
                case DataSet.All:
                    queryFactory = _ => BaseTests.CreateData().People.AsQueryable();
                    break;
            }

            switch (kind)
            {
                case Kind.Root:
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
                    break;

                case Kind.Loader:
                    var loaderType = GraphQLTypeBuilder.CreateLoaderType<Country, TestUserContext>(
                        onConfigure: loader =>
                        {
                            loader.Field(c => c.Id);
                            configure(
                                loader.Field("test")
                                    .FromIQueryable(
                                        queryFactory,
                                        (c, p) => c.Id == p.CountryId,
                                        configureQueryable));
                        },
                        getBaseQuery: _ => BaseTests.CreateData().Countries.AsQueryable(),
                        applyNaturalOrderBy: q => q.OrderBy(c => c.Id),
                        applyNaturalThenBy: q => q.ThenBy(c => c.Id));

                    query = $@"
                        query {{
                            countries {{
                                items {{
                                    id
                                    {queryFragment}
                                }}
                            }}
                        }}";

                    result = $@"
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
                            _ => BaseTests.CreateData().Countries.AsQueryable(),
                            builder => configure(
                                builder
                                    .Field("test")
                                    .FromIQueryable(
                                        queryFactory,
                                        (c, p) => c.Id == p.CountryId,
                                        configureQueryable))),
                        query,
                        result,
                        new DataContextMock());

                    break;
                default:
                    throw new NotImplementedException();
            }
        }

        private static IEnumerable<TestCaseData> GetTestData(Kind kind)
        {
            return Impl(kind).SelectMany(x => x);

            static IEnumerable<IEnumerable<TestCaseData>> Impl(Kind kind)
            {
                yield return Create(
                    kind,
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
                    }]");

                yield return Create(
                    kind,
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
                    configureQueryable: builder => builder.Field(x => x.Id, null));

                yield return Create(
                    kind,
                    DataSet.All,
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
                    builder => builder.Field(x => x.Id, null).Sortable());

                yield return Create(
                    kind,
                    DataSet.All,
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
                    builder => builder.Field(x => x.Id, null).Sortable(x => x.Id.ToString()));
#pragma warning restore CA1305 // Specify IFormatProvider

                yield return Create(
                    kind,
                    DataSet.All,
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
                    builder => builder.Field(x => x.Id, null).Filterable().Sortable());

                yield return Create(
                    kind,
                    DataSet.All,
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
                    @"{
                        id
                    }",
                    "[]");
            }

            static IEnumerable<TestCaseData> Create(
                Kind kind,
                DataSet dataSet,
                Expression<Func<IFromIQueryableBuilder<Person, TestUserContext>, IFromIQueryableBuilder<Person, TestUserContext>>> configure,
                string query,
                string result,
                string args = null,
                Expression<Action<IInlineObjectBuilder<Person, TestUserContext>>> configureQueryable = null)
            {
                yield return CreateImpl(kind, dataSet, configure, query, result, args, configureQueryable, false);
                yield return CreateImpl(kind, dataSet, configure, query, result, args, configureQueryable, true);
            }

            static TestCaseData CreateImpl(
                Kind kind,
                DataSet dataSet,
                Expression<Func<IFromIQueryableBuilder<Person, TestUserContext>, IFromIQueryableBuilder<Person, TestUserContext>>> configure,
                string query,
                string result,
                string args,
                Expression<Action<IInlineObjectBuilder<Person, TestUserContext>>> configureQueryable,
                bool asConnection)
            {
                query = asConnection ? $"{{items {query}}}" : query;
                result = asConnection ? $"{{items: {result}}}" : result;

                var test = string.IsNullOrEmpty(args) ? "test" : $"test({args})";
                query = string.IsNullOrEmpty(query) ? test : $"{test} {query}";
                result = $"test: {result}";

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

                if (asConnection)
                {
                    name = $"{name}․AsConnection(query => query.OrderBy(p => p.Id))";
                }

                switch (kind)
                {
                    case Kind.Loader:
                        name = $"FromLoader/{name}";
                        break;
                    case Kind.Query:
                        name = $"FromIQueryable/{name}";
                        break;
                }

                Action<IFromIQueryableBuilder<Person, TestUserContext>> configureAction = builder =>
                {
                    var compiledConfigure = configure?.Compile();
                    var result = compiledConfigure?.Invoke(builder);

                    if (asConnection)
                    {
                        if (result == null)
                        {
                            result = builder;
                        }

                        result.AsConnection(query => query.OrderBy(p => p.Id));
                    }
                };

                return new TestCaseData(kind, dataSet, configureAction, query, result, configureQueryable?.Compile()).SetName(name);
            }
        }

        private static IEnumerable<TestCaseData> GetTestData2(Kind kind)
        {
            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.Id),
                string.Empty,
                "[1, 2, 3, 4, 5, 6]");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.ManagerId),
                string.Empty,
                "[null, 1, 1, 2, 2, 5]");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.FullName),
                string.Empty,
                @"[""Linoel Livermore"", ""Sophie Gandley"", ""Hannie Everitt"", ""Florance Goodricke"", ""Aldon Exley"", ""Walton Alvarez""]");

            yield return Create(
                kind,
                DataSet.All,
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
                    .Select(x => x.Id)
                    .Select(id => 2 * id),
                string.Empty,
                "[2, 4, 6, 8, 10, 12]");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.ManagerId)
                    .Select(managerId => 2 * managerId),
                string.Empty,
                "[null, 2, 2, 4, 4, 10]");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.FullName)
                    .Select(fullName => $"*{fullName}"),
                string.Empty,
                @"[""*Linoel Livermore"", ""*Sophie Gandley"", ""*Hannie Everitt"", ""*Florance Goodricke"", ""*Aldon Exley"", ""*Walton Alvarez""]");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.Unit, null)
                    .Select(u => u.Id),
                string.Empty,
                "[1, 1, 1, 2, 2, 2]");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.Unit, null)
                    .Select(u => u.ParentId),
                string.Empty,
                "[null, null, null, 1, 1, 1]");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.Unit, null)
                    .Select(u => u.Name),
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
                @"{
                    id
                }",
                "null");

            yield return Create(
                kind,
                DataSet.Empty,
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
                    .Select(x => x.Id)
                    .FirstOrDefault(null),
                string.Empty,
                "1");

            yield return Create(
                kind,
                DataSet.One,
                builder => builder
                    .Select(x => x.Id)
                    .SingleOrDefault(null),
                string.Empty,
                "1");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.Id)
                    .FirstOrDefault(i => i == 1),
                string.Empty,
                "1");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.Id)
                    .SingleOrDefault(i => i == 1),
                string.Empty,
                "1");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.ManagerId)
                    .FirstOrDefault(m => m == null),
                string.Empty,
                "null");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.ManagerId)
                    .SingleOrDefault(m => m == null),
                string.Empty,
                "null");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.FullName)
                    .FirstOrDefault(null),
                string.Empty,
                @"""Linoel Livermore""");

            yield return Create(
                kind,
                DataSet.One,
                builder => builder
                    .Select(x => x.FullName)
                    .SingleOrDefault(null),
                string.Empty,
                @"""Linoel Livermore""");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.FullName)
                    .FirstOrDefault(n => n == "Linoel Livermore"),
                string.Empty,
                @"""Linoel Livermore""");

            yield return Create(
                kind,
                DataSet.All,
                builder => builder
                    .Select(x => x.FullName)
                    .SingleOrDefault(n => n == "Linoel Livermore"),
                string.Empty,
                @"""Linoel Livermore""");

            yield return Create(
                kind,
                DataSet.AllWithManager,
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
                @"{
                    id
                }",
                @"{
                    id: 5
                }");

            static TestCaseData Create(
                Kind kind,
                DataSet dataSet,
                Expression<Action<IFromIQueryableBuilder<Person, TestUserContext>>> configure,
                string query,
                string result,
                string args = null,
                Expression<Action<IInlineObjectBuilder<Person, TestUserContext>>> configureQueryable = null)
            {
                var test = string.IsNullOrEmpty(args) ? "test" : $"test({args})";
                query = string.IsNullOrEmpty(query) ? test : $"{test} {query}";
                result = $"test: {result}";

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
                }

                return new TestCaseData(kind, dataSet, configure?.Compile() ?? (builder => { }), query, result, configureQueryable?.Compile()).SetName(name);
            }
        }
    }
}
