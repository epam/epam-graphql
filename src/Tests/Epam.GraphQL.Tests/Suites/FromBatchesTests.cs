// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Tests.Common;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NSubstitute;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Suites
{
    [TestFixture]
    public class FromBatchesTests : BaseTests
    {
        private static readonly LoaderType[] _loaderTypes =
        {
            LoaderType.Loader,
            LoaderType.MutableLoader,
        };

        private static readonly FromBatchType[] _fromBatchTypes =
        {
            FromBatchType.Entity,
            FromBatchType.EntityContext,
            FromBatchType.EntityTask,
            FromBatchType.EntityContextTask,
            FromBatchType.Key,
            FromBatchType.KeyContext,
            FromBatchType.KeyTask,
            FromBatchType.KeyContextTask,
        };

        [Test]
        public void BatchFuncIntTest(
            [ValueSource(nameof(_loaderTypes))] LoaderType loaderType,
            [ValueSource(nameof(_fromBatchTypes))] FromBatchType batchType)
        {
            var batchUnitFunc = Substitute.For<Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, Unit>>>();
            batchUnitFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(
                    p => p,
                    p => FakeData.Units
                        .AsQueryable()
                        .Where(u => u.Id == p.UnitId)
                        .FirstOrDefault()));

            var batchManagerFunc = Substitute.For<Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, Person>>>();
            batchManagerFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1)
                    .ToDictionary(
                        p => p,
                        p => FakeData.People
                            .AsQueryable()
                            .Where(m => m.Id == p.ManagerId)
                            .FirstOrDefault()));

            var batchCountryFunc = Substitute.For<Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, Country>>>();
            batchCountryFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1)
                    .ToDictionary(
                        p => p,
                        p => FakeData.Countries
                            .AsQueryable()
                            .Where(c => c.Id == p.CountryId)
                            .FirstOrDefault()));

            var builder = loaderType switch
            {
                LoaderType.Loader => CreateQueryBuilder(
                    loader =>
                    {
                        loader.Field(u => u.Id);
                        loader.Field(u => u.FullName);
                        loader.Field("managerOrDepartmentOrCountry")
                            .FromBatch(
                                batchType,
                                person => person,
                                () => new TestUserContext(null),
                                batchManagerFunc,
                                build => build.ConfigureFrom(loader.GetType()))
                            .AndFromBatch(
                                batchType,
                                person => person,
                                () => new TestUserContext(null),
                                batchUnitFunc)
                            .AndFromBatch(
                                batchType,
                                person => person,
                                () => new TestUserContext(null),
                                batchCountryFunc);
                    }),
                LoaderType.MutableLoader => CreateQueryMutableBuilder(
                    loader =>
                    {
                        loader.Field(u => u.Id);
                        loader.Field(u => u.FullName);
                        loader.Field("managerOrDepartmentOrCountry")
                            .FromBatch(
                                batchType,
                                person => person,
                                () => new TestUserContext(null),
                                batchManagerFunc,
                                build => build.ConfigureFrom(loader.GetType()))
                            .AndFromBatch(
                                batchType,
                                person => person,
                                () => new TestUserContext(null),
                                batchUnitFunc)
                            .AndFromBatch(
                                batchType,
                                person => person,
                                () => new TestUserContext(null),
                                batchCountryFunc);
                    }),
                LoaderType.IdentifiableLoader => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };

            const string query = @"
                    query {
                        people {
                            items {
                                id
                                managerOrDepartmentOrCountry {
                                    ... on Unit {
                                        id
                                        name
                                    }
                                    ... on Person {
                                        id
                                        fullName
                                    }
                                    ... on Country {
                                        id
                                    }
                                }
                            }
                        }
                    }";

            const string expected = @"
                    {
                        people: {
                            items: [
                                {
                                    id: 1,
                                    managerOrDepartmentOrCountry: [
                                        {
                                            id: 1,
                                            name: ""Alpha""
                                        },
                                        {
                                            id: 1
                                        }
                                    ]
                                },
                                {
                                    id: 2,
                                    managerOrDepartmentOrCountry: [
                                        {
                                            id: 1,
                                            fullName: ""Linoel Livermore""
                                        },
                                        {
                                            id: 1,
                                            name: ""Alpha""
                                        },
                                        {
                                            id: 1
                                        }
                                    ]
                                },
                                {
                                    id: 3,
                                    managerOrDepartmentOrCountry: [
                                        {
                                            id: 1,
                                            fullName: ""Linoel Livermore""
                                        },
                                        {
                                            id: 1,
                                            name: ""Alpha""
                                        },
                                        {
                                            id: 1
                                        }
                                    ]
                                },
                                {
                                    id: 4,
                                    managerOrDepartmentOrCountry: [
                                        {
                                            id: 2,
                                            fullName: ""Sophie Gandley""
                                        },
                                        {
                                            id: 2,
                                            name: ""Beta""
                                        },
                                        {
                                            id: 1
                                        }
                                    ]
                                },
                                {
                                    id: 5,
                                    managerOrDepartmentOrCountry: [
                                        {
                                            id: 2,
                                            fullName: ""Sophie Gandley""
                                        },
                                        {
                                            id: 2,
                                            name: ""Beta""
                                        },
                                        {
                                            id: 1
                                        }
                                    ]
                                },
                                {
                                    id: 6,
                                    managerOrDepartmentOrCountry: [
                                        {
                                            id: 5,
                                            fullName: ""Aldon Exley""
                                        },
                                        {
                                            id: 2,
                                            name: ""Beta""
                                        },
                                        {
                                            id: 1
                                        }
                                    ]
                                }
                            ]
                        }
                    }";

            TestHelpers.TestQuery(builder, query, expected);

            batchUnitFunc.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>());
            batchManagerFunc.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>());
            batchCountryFunc.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>());
        }

        private Action<Query<TestUserContext>> CreateQueryBuilder(Action<Loader<Person, TestUserContext>> personLoaderBuilder, Action<Query<TestUserContext>> configure = null)
        {
            var peopleLoader = GraphQLTypeBuilder.CreateLoaderType(
                onConfigure: personLoaderBuilder,
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            return query =>
            {
                query
                    .Connection(peopleLoader, "people");
                configure?.Invoke(query);
            };
        }

        private Action<Query<TestUserContext>> CreateQueryMutableBuilder(Action<MutableLoader<Person, int, TestUserContext>> personLoaderBuilder, Action<Query<TestUserContext>> configure = null)
        {
            var peopleLoader = GraphQLTypeBuilder.CreateMutableLoaderType(
                onConfigure: personLoaderBuilder,
                getBaseQuery: _ => FakeData.People.AsQueryable());

            return query =>
            {
                query
                    .Connection(peopleLoader, "people");
                configure?.Invoke(query);
            };
        }
    }
}
