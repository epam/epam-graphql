// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NSubstitute;
using NUnit.Framework;

namespace Epam.GraphQL.Tests
{
    [TestFixture]
    public class FromBatchTests : BaseTests
    {
        [Test(Description = "FromBatch(...)/IEnumerable<int?> using key selector/context/select")]
        public void BatchFuncFromNullableIntToNullableIntAndPersonUsingKeySelectorAndSelectTest()
        {
            var batchFunc = Substitute.For<Func<IEnumerable<int?>, IDictionary<int?, Person>>>();
            batchFunc
                .Invoke(Arg.Any<IEnumerable<int?>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int?>>(0).ToDictionary(headId => headId, headId => FakeData.People.FirstOrDefault(p => p.Id == headId)));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("headFullName")
                        .FromBatch(u => u.HeadId, batchFunc, null).Select(p => p.FullName);
                    loader.Field("headId")
                        .FromBatch(u => u.HeadId, batchFunc, null).Select(p => p.Id);
                    loader.Field("headManagerId")
                        .FromBatch(u => u.HeadId, batchFunc, null).Select(p => p.ManagerId);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("headFullName")
                        .FromBatch(u => u.HeadId, batchFunc, null).Select(p => p.FullName);
                    loader.Field("headId")
                        .FromBatch(u => u.HeadId, batchFunc, null).Select(p => p.Id);
                    loader.Field("headManagerId")
                        .FromBatch(u => u.HeadId, batchFunc, null).Select(p => p.ManagerId);
                });

            const string Query = @"
                query {
                    units {
                        items {
                            id
                            headFullName
                            headId
                            headManagerId
                        }
                    }
                }";

            const string Expected = @"
                {
                    units: {
                        items: [{
                            id: 1,
                            headFullName: ""Linoel Livermore"",
                            headId: 1,
                            headManagerId: null
                        }, {
                            id: 2,
                            headFullName: ""Sophie Gandley"",
                            headId: 2,
                            headManagerId: 1
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<IEnumerable<int?>>());
        }

        [Test(Description = "FromBatch(...)/IEnumerable<int?> using key selector/context/select")]
        public void BatchFuncFromContextAndNullableIntToNullableIntAndPersonUsingKeySelectorAndSelectTest()
        {
            var batchFunc = Substitute.For<Func<TestUserContext, IEnumerable<int?>, IDictionary<int?, Person>>>();
            batchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int?>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int?>>(1).ToDictionary(headId => headId, headId => FakeData.People.FirstOrDefault(p => p.Id == headId)));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("headFullName")
                        .FromBatch(u => u.HeadId, batchFunc, null).Select(p => p.FullName);
                    loader.Field("headId")
                        .FromBatch(u => u.HeadId, batchFunc, null).Select(p => p.Id);
                    loader.Field("headManagerId")
                        .FromBatch(u => u.HeadId, batchFunc, null).Select(p => p.ManagerId);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("headFullName")
                        .FromBatch(u => u.HeadId, batchFunc, null).Select(p => p.FullName);
                    loader.Field("headId")
                        .FromBatch(u => u.HeadId, batchFunc, null).Select(p => p.Id);
                    loader.Field("headManagerId")
                        .FromBatch(u => u.HeadId, batchFunc, null).Select(p => p.ManagerId);
                });

            const string Query = @"
                query {
                    units {
                        items {
                            id
                            headFullName
                            headId
                            headManagerId
                        }
                    }
                }";

            const string Expected = @"
                {
                    units: {
                        items: [{
                            id: 1,
                            headFullName: ""Linoel Livermore"",
                            headId: 1,
                            headManagerId: null
                        }, {
                            id: 2,
                            headFullName: ""Sophie Gandley"",
                            headId: 2,
                            headManagerId: 1
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int?>>());
        }

        [Test]
        public void IQueryableFromBatchTest()
        {
            var batchUnitPeopleFunc = Substitute.For<Func<IEnumerable<int>, IDictionary<int, IEnumerable<Tuple<int, int>>>>>();
            batchUnitPeopleFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo =>
                    callInfo
                        .ArgAt<IEnumerable<int>>(0)
                        .GroupBy(id => id)
                        .ToDictionary(u => u.First(), u => FakeData.People.Where(p => p.UnitId == u.Key).Select(p => Tuple.Create(u.Key, p.Id))));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field("unitInfo")
                        .FromBatch(
                            u => u.Id,
                            batchUnitPeopleFunc,
                            b =>
                            {
                                b.Field("id", u => u.Item1);
                                b.Field("peopleId", u => u.Item2);
                                b.Field("manager")
                                    .FromIQueryable(
                                        _ => FakeData.People.AsQueryable(),
                                        (t, p) => t.Item2 == p.Id,
                                        b2 =>
                                        {
                                            b2.Field("id", m => m.Id);
                                            b2.Field("fullName", m => m.FullName);
                                        })
                                    .Select(m => m.Manager)
                                    .FirstOrDefault();
                            });
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(x => x.Name).Editable();
                    loader.Field("unitInfo")
                        .FromBatch(
                            u => u.Id,
                            batchUnitPeopleFunc,
                            b =>
                            {
                                b.Field("id", u => u.Item1);
                                b.Field("peopleId", u => u.Item2);
                                b.Field("manager")
                                    .FromIQueryable(
                                        _ => FakeData.People.AsQueryable(),
                                        (t, p) => t.Item2 == p.Id,
                                        b2 =>
                                        {
                                            b2.Field("id", m => m.Id);
                                            b2.Field("fullName", m => m.FullName);
                                        })
                                    .Select(m => m.Manager) // TODO it should be not possible to Select from IQueryable with builder passed
                                    .FirstOrDefault();
                            });
                });

            const string Query = @"
                query {
                    units {
                        items {
                            unitInfo {
                                id,
                                peopleId
                                manager {
                                    id
                                }
                            }
                        }
                    }
                }";
            const string Expected = @"
                {
                    units: {
                        items: [{
                            unitInfo: [{
                                id: 1,
                                peopleId: 1,
                                manager: null
                            }, {
                                id: 1,
                                peopleId: 2,
                                manager: {
                                    id: 1
                                }
                            }, {
                                id: 1,
                                peopleId: 3,
                                manager: {
                                    id: 1
                                }
                            }]
                        }, {
                            unitInfo: [{
                                id: 2,
                                peopleId: 4,
                                manager: {
                                    id: 2
                                }
                            }, {
                                id: 2,
                                peopleId: 5,
                                manager: {
                                    id: 2
                                }
                            }, {
                                id: 2,
                                peopleId: 6,
                                manager: {
                                    id: 5
                                }
                            }]
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);

            batchUnitPeopleFunc.Received(2)
                .Invoke(Arg.Any<IEnumerable<int>>());
        }

        [Test]
        public void LoaderFromBatchTest()
        {
            var batchUnitPeopleFunc = Substitute.For<Func<IEnumerable<int>, IDictionary<int, IEnumerable<Tuple<int, int>>>>>();
            batchUnitPeopleFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo =>
                    callInfo
                        .ArgAt<IEnumerable<int>>(0)
                        .GroupBy(id => id)
                        .ToDictionary(u => u.First(), u => FakeData.People.Where(p => p.UnitId == u.Key).Select(p => Tuple.Create(u.Key, p.Id))));

            var personLoader = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable(),
                applyNaturalOrderBy: query => query.OrderBy(p => p.Id),
                applyNaturalThenBy: query => query.ThenBy(p => p.Id));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field("unitInfo")
                        .FromBatch(
                            u => u.Id,
                            batchUnitPeopleFunc,
                            b =>
                            {
                                b.Field("id", u => u.Item1);
                                b.Field("peopleId", u => u.Item2);
                                b.Field("manager")
                                    .FromLoader<Tuple<int, int>, Person, TestUserContext>(personLoader, (t, p) => t.Item2 == p.Id)
                                    .Select(p => p.Manager)
                                    .FirstOrDefault();
                            });
                },
                query => query.Connection(personLoader, "people"));

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(x => x.Name).Editable();
                    loader.Field("unitInfo")
                        .FromBatch(
                            u => u.Id,
                            batchUnitPeopleFunc,
                            b =>
                            {
                                b.Field("id", u => u.Item1);
                                b.Field("peopleId", u => u.Item2);
                                b.Field("manager")
                                    .FromLoader<Tuple<int, int>, Person, TestUserContext>(personLoader, (t, p) => t.Item2 == p.Id)
                                    .Select(p => p.Manager)
                                    .FirstOrDefault();
                            });
                },
                query => query.Connection(personLoader, "people"));

            const string Query = @"
                query {
                    units {
                        items {
                            unitInfo {
                                id,
                                peopleId
                                manager {
                                    id
                                }
                            }
                        }
                    }
                }";
            const string Expected = @"
                {
                    units: {
                        items: [{
                            unitInfo: [{
                                id: 1,
                                peopleId: 1,
                                manager: null
                            }, {
                                id: 1,
                                peopleId: 2,
                                manager: {
                                    id: 1
                                }
                            }, {
                                id: 1,
                                peopleId: 3,
                                manager: {
                                    id: 1
                                }
                            }]
                        }, {
                            unitInfo: [{
                                id: 2,
                                peopleId: 4,
                                manager: {
                                    id: 2
                                }
                            }, {
                                id: 2,
                                peopleId: 5,
                                manager: {
                                    id: 2
                                }
                            }, {
                                id: 2,
                                peopleId: 6,
                                manager: {
                                    id: 5
                                }
                            }]
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);

            batchUnitPeopleFunc.Received(2)
                .Invoke(Arg.Any<IEnumerable<int>>());
        }

        [Test(Description = "https://git.epam.com/epm-ppa/epam-graphql/-/issues/3")]
        public void LoaderFromBatchTestAdditionalConditionForChild()
        {
            var batchUnitPeopleFunc = Substitute.For<Func<IEnumerable<int>, IDictionary<int, IEnumerable<Tuple<int, int>>>>>();
            batchUnitPeopleFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo =>
                    callInfo
                        .ArgAt<IEnumerable<int>>(0)
                        .GroupBy(id => id)
                        .ToDictionary(u => u.First(), u => FakeData.People.Where(p => p.UnitId == u.Key).Select(p => Tuple.Create(u.Key, p.Id))));

            var personLoader = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable(),
                applyNaturalOrderBy: query => query.OrderBy(p => p.Id),
                applyNaturalThenBy: query => query.ThenBy(p => p.Id));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field("unitInfo")
                        .FromBatch(
                            u => u.Id,
                            batchUnitPeopleFunc,
                            b =>
                            {
                                b.Field("id", u => u.Item1);
                                b.Field("peopleId", u => u.Item2);
                                b.Field("manager")
                                    .FromLoader<Tuple<int, int>, Person, TestUserContext>(personLoader, (t, p) => t.Item2 == p.Id && p.Id > 0)
                                    .Select(p => p.Manager)
                                    .FirstOrDefault();
                            });
                },
                query => query.Connection(personLoader, "people"));

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(x => x.Name).Editable();
                    loader.Field("unitInfo")
                        .FromBatch(
                            u => u.Id,
                            batchUnitPeopleFunc,
                            b =>
                            {
                                b.Field("id", u => u.Item1);
                                b.Field("peopleId", u => u.Item2);
                                b.Field("manager")
                                    .FromLoader<Tuple<int, int>, Person, TestUserContext>(personLoader, (t, p) => t.Item2 == p.Id && p.Id > 0)
                                    .Select(p => p.Manager)
                                    .FirstOrDefault();
                            });
                },
                query => query.Connection(personLoader, "people"));

            const string Query = @"
                query {
                    units {
                        items {
                            unitInfo {
                                id,
                                peopleId
                                manager {
                                    id
                                }
                            }
                        }
                    }
                }";
            const string Expected = @"
                {
                    units: {
                        items: [{
                            unitInfo: [{
                                id: 1,
                                peopleId: 1,
                                manager: null
                            }, {
                                id: 1,
                                peopleId: 2,
                                manager: {
                                    id: 1
                                }
                            }, {
                                id: 1,
                                peopleId: 3,
                                manager: {
                                    id: 1
                                }
                            }]
                        }, {
                            unitInfo: [{
                                id: 2,
                                peopleId: 4,
                                manager: {
                                    id: 2
                                }
                            }, {
                                id: 2,
                                peopleId: 5,
                                manager: {
                                    id: 2
                                }
                            }, {
                                id: 2,
                                peopleId: 6,
                                manager: {
                                    id: 5
                                }
                            }]
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);

            batchUnitPeopleFunc.Received(2)
                .Invoke(Arg.Any<IEnumerable<int>>());
        }

        [Test(Description = "https://git.epam.com/epm-ppa/epam-graphql/-/issues/3")]
        public void LoaderFromBatchTestAdditionalConditionForParent()
        {
            var batchUnitPeopleFunc = Substitute.For<Func<IEnumerable<int>, IDictionary<int, IEnumerable<Tuple<int, int>>>>>();
            batchUnitPeopleFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo =>
                    callInfo
                        .ArgAt<IEnumerable<int>>(0)
                        .GroupBy(id => id)
                        .ToDictionary(u => u.First(), u => FakeData.People.Where(p => p.UnitId == u.Key).Select(p => Tuple.Create(u.Key, p.Id))));

            var personLoader = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable(),
                applyNaturalOrderBy: query => query.OrderBy(p => p.Id),
                applyNaturalThenBy: query => query.ThenBy(p => p.Id));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field("unitInfo")
                        .FromBatch(
                            u => u.Id,
                            batchUnitPeopleFunc,
                            b =>
                            {
                                b.Field("id", u => u.Item1);
                                b.Field("peopleId", u => u.Item2);
                                b.Field("manager")
                                    .FromLoader<Tuple<int, int>, Person, TestUserContext>(personLoader, (t, p) => t.Item2 == p.Id && t.Item1 > 0)
                                    .Select(p => p.Manager)
                                    .FirstOrDefault();
                            });
                },
                query => query.Connection(personLoader, "people"));

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field("unitInfo")
                        .FromBatch(
                            u => u.Id,
                            batchUnitPeopleFunc,
                            b =>
                            {
                                b.Field("id", u => u.Item1);
                                b.Field("peopleId", u => u.Item2);
                                b.Field("manager")
                                    .FromLoader<Tuple<int, int>, Person, TestUserContext>(personLoader, (t, p) => t.Item2 == p.Id && t.Item1 > 0)
                                    .Select(p => p.Manager)
                                    .FirstOrDefault();
                            });
                },
                query => query.Connection(personLoader, "people"));

            const string Query = @"
                query {
                    units {
                        items {
                            unitInfo {
                                id,
                                peopleId
                                manager {
                                    id
                                }
                            }
                        }
                    }
                }";

            const string Message = "Cannot use expression (t, p) => ((t.Item2 == p.Id) AndAlso (t.Item1 > 0)) as a relation between Tuple<int, int> and Person types. (Parameter 'condition')";

            TestHelpers.TestQueryError(builder, typeof(ArgumentOutOfRangeException), Message, Query);
            TestHelpers.TestQueryError(mutableBuilder, typeof(ArgumentOutOfRangeException), Message, Query);
        }

        [Test]
        public void BatchConfigureFromTheSameLoaderTest()
        {
            var childrenBatchFunc = Substitute.For<Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, Person>>>();
            childrenBatchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(
                    p => p,
                    p => FakeData.People.FirstOrDefault(c => c.ManagerId == p.Id)));

            var branchLoader = GraphQLTypeBuilder.CreateLoaderType<Branch, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(b => b.Id);
                },
                getBaseQuery: _ => FakeData.Branches.AsQueryable(),
                applyNaturalOrderBy: query => query.OrderBy(p => p.Id),
                applyNaturalThenBy: query => query.ThenBy(p => p.Id));

            var unitLoader = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field("branches")
                        .FromLoader<Unit, Branch, TestUserContext>(branchLoader, (u, b) => u.BranchId == b.Id);
                },
                getBaseQuery: _ => FakeData.Units.AsQueryable(),
                applyNaturalOrderBy: query => query.OrderBy(p => p.Id),
                applyNaturalThenBy: query => query.ThenBy(p => p.Id));

            var peopleLoader = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("units")
                        .FromLoader<Person, Unit, TestUserContext>(unitLoader, (p, u) => p.UnitId == u.Id);
                    loader.Field("subordinate")
                        .FromBatch(childrenBatchFunc, build => build.ConfigureFrom(loader.GetType()));
                },
                getBaseQuery: _ => FakeData.People.AsQueryable(),
                applyNaturalOrderBy: query => query.OrderBy(p => p.Id),
                applyNaturalThenBy: query => query.ThenBy(p => p.Id));

            const string Query = @"
                query {
                    people {
                        items {
                            id
                            units {
                                branches {
                                    id
                                }
                            }
                            subordinate {
                                id
                                units {
                                    branches {
                                        id
                                    }
                                }
                            }
                        }
                    }
                }";

            const string Expected = @"
                {
                    people: {
                        items: [{
                            id: 1,
                            units: [{ branches: [{ id: 2 }] }],
                            subordinate: {
                                id: 2,
                                units: [{ branches: [{ id: 2 }] }]
                            }
                        },{
                            id: 2,
                            units: [{ branches: [{ id: 2 }] }],
                            subordinate: {
                                id: 4,
                                units: [{ branches: [{ id: 1 }] }]
                            }
                        },{
                            id: 3,
                            units: [{ branches: [{ id: 2 }] }],
                            subordinate: null
                        },{
                            id: 4,
                            units: [{ branches: [{ id: 1 }] }],
                            subordinate: null
                        },{
                            id: 5,
                            units: [{ branches: [{ id: 1 }] }],
                            subordinate: {
                                id: 6,
                                units: [{ branches: [{ id: 1 }] }]
                            }
                        },{
                            id: 6,
                            units: [{ branches: [{ id: 1 }] }],
                            subordinate: null
                        }]
                    }
                }";

            TestHelpers.TestQuery(query => query.Connection(peopleLoader, "people"), Query, Expected);

            childrenBatchFunc.Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>());
        }

        private Action<Query<TestUserContext>> CreateQueryBuilder(Action<Loader<Unit, TestUserContext>> userLoaderBuilder, Action<Query<TestUserContext>> configure = null, Expression<Func<Unit, bool>> filter = null)
        {
            var unitLoader = GraphQLTypeBuilder.CreateLoaderType(
                onConfigure: userLoaderBuilder,
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => filter == null
                    ? FakeData.Units.AsQueryable()
                    : FakeData.Units.AsQueryable().Where(filter));

            return query =>
            {
                query
                    .Connection(unitLoader, "units");
                configure?.Invoke(query);
            };
        }

        private Action<Query<TestUserContext>> CreateQueryMutableBuilder(Action<MutableLoader<Unit, int, TestUserContext>> userLoaderBuilder, Action<Query<TestUserContext>> configure = null, Expression<Func<Unit, bool>> filter = null)
        {
            var unitLoader = GraphQLTypeBuilder.CreateMutableLoaderType(
                onConfigure: userLoaderBuilder,
                getBaseQuery: _ => filter == null
                    ? FakeData.Units.AsQueryable()
                    : FakeData.Units.AsQueryable().Where(filter));

            return query =>
            {
                query
                    .Connection(unitLoader, "units");
                configure?.Invoke(query);
            };
        }
    }
}
