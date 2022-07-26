// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Tests.Common;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NSubstitute;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Suites
{
    [TestFixture]
    public class FromBatchTests : BaseTests
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
            var batchFunc = Substitute.For<Func<TestUserContext, IEnumerable<int>, IDictionary<int, int>>>();

            batchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(1)
                    .ToDictionary(
                        u => u,
                        u => FakeData.People.Count(p => p.UnitId == u)));

            var builder = loaderType switch
            {
                LoaderType.Loader => CreateQueryBuilder(
                    loader =>
                    {
                        loader.Field(u => u.Id);
                        loader.Field("empCount")
                            .FromBatch(
                                batchType,
                                unit => unit.Id,
                                () => new TestUserContext(null),
                                batchFunc);
                    }),
                LoaderType.MutableLoader => CreateQueryMutableBuilder(
                    loader =>
                    {
                        loader.Field(u => u.Id);
                        loader.Field("empCount")
                            .FromBatch(
                                batchType,
                                unit => unit.Id,
                                () => new TestUserContext(null),
                                batchFunc);
                    }),
                LoaderType.IdentifiableLoader => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };

            const string query = @"
                query {
                    units {
                        id
                        empCount
                    }
                }";

            const string expected = @"
                {
                    units: [{
                        id: 1,
                        empCount: 3
                    }, {
                        id: 2,
                        empCount: 3
                    }]
                }";

            TestHelpers.TestQuery(builder, query, expected);
            batchFunc.Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>());
        }

        [Test]
        public void BatchFuncEnumerableOfIntTest(
            [ValueSource(nameof(_loaderTypes))] LoaderType loaderType,
            [ValueSource(nameof(_fromBatchTypes))] FromBatchType batchType)
        {
            var batchFunc = Substitute.For<Func<TestUserContext, IEnumerable<int>, IDictionary<int, IEnumerable<int>>>>();
            batchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(1).ToDictionary(u => u, u => FakeData.People.Where(p => p.UnitId == u).Select(p => p.Id)));

            var builder = loaderType switch
            {
                LoaderType.Loader => CreateQueryBuilder(
                    loader =>
                    {
                        loader.Field(u => u.Id);
                        loader.Field("empIds")
                            .FromBatch(
                                batchType,
                                unit => unit.Id,
                                () => new TestUserContext(null),
                                batchFunc);
                    }),
                LoaderType.MutableLoader => CreateQueryMutableBuilder(
                    loader =>
                    {
                        loader.Field(u => u.Id);
                        loader.Field("empIds")
                            .FromBatch(
                                batchType,
                                unit => unit.Id,
                                () => new TestUserContext(null),
                                batchFunc);
                    }),
                LoaderType.IdentifiableLoader => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };

            const string query = @"
                query {
                    units {
                        id
                        empIds
                    }
                }";

            const string expected = @"
                {
                    units: [{
                        id: 1,
                        empIds: [1,2,3]
                    }, {
                        id: 2,
                        empIds: [4,5,6]
                    }]
                }";

            TestHelpers.TestQuery(builder, query, expected);
            batchFunc.Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>());
        }

        [Test]
        public void BatchFuncPersonUsingNullableKeySelectorTest(
            [ValueSource(nameof(_loaderTypes))] LoaderType loaderType,
            [ValueSource(nameof(_fromBatchTypes))] FromBatchType batchType)
        {
            var batchFunc = Substitute.For<Func<TestUserContext, IEnumerable<int?>, IDictionary<int?, Person>>>();
            batchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int?>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int?>>(1).ToDictionary(headId => headId, headId => FakeData.People.FirstOrDefault(p => p.Id == headId)));

            var builder = loaderType switch
            {
                LoaderType.Loader => CreateQueryBuilder(
                    loader =>
                    {
                        loader.Field(u => u.Id);
                        loader.Field("head")
                            .FromBatch(
                                batchType,
                                unit => unit.HeadId,
                                () => new TestUserContext(null),
                                batchFunc);
                    }),
                LoaderType.MutableLoader => CreateQueryMutableBuilder(
                    loader =>
                    {
                        loader.Field(u => u.Id);
                        loader.Field("head")
                            .FromBatch(
                                batchType,
                                unit => unit.HeadId,
                                () => new TestUserContext(null),
                                batchFunc);
                    }),
                LoaderType.IdentifiableLoader => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };

            const string query = @"
                query {
                    units {
                        id
                        head {
                            id
                        }
                    }
                }";

            const string expected = @"
                {
                    units: [{
                        id: 1,
                        head: {
                            id: 1
                        }
                    }, {
                        id: 2,
                        head: {
                            id: 2
                        }
                    }]
                }";

            TestHelpers.TestQuery(builder, query, expected);

            batchFunc.Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int?>>());
        }

        [Test]
        public void BatchFuncTupleOfUnitAndIntUsingBuilderTest(
            [ValueSource(nameof(_loaderTypes))] LoaderType loaderType,
            [ValueSource(nameof(_fromBatchTypes))] FromBatchType batchType)
        {
            var batchFunc = Substitute.For<Func<TestUserContext, IEnumerable<Unit>, IDictionary<Unit, Tuple<Unit, int>>>>();
            batchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Unit>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Unit>>(1).ToDictionary(u => u, u => Tuple.Create(u, FakeData.People.Count(p => p.UnitId == u.Id))));

            var builder = loaderType switch
            {
                LoaderType.Loader => CreateQueryBuilder(
                    loader =>
                    {
                        loader.Field("unitInfo")
                            .FromBatch(
                                batchType,
                                unit => unit,
                                () => new TestUserContext(null),
                                batchFunc,
                                b =>
                                {
                                    b.Field("id", u => u.Item1.Id);
                                    b.Field("count", u => u.Item2);
                                });
                    }),
                LoaderType.MutableLoader => CreateQueryMutableBuilder(
                    loader =>
                    {
                        loader.Field(x => x.Name).Editable();
                        loader.Field("unitInfo")
                            .FromBatch(
                                batchType,
                                unit => unit,
                                () => new TestUserContext(null),
                                batchFunc,
                                b =>
                                {
                                    b.Field("id", u => u.Item1.Id);
                                    b.Field("count", u => u.Item2);
                                });
                    }),
                LoaderType.IdentifiableLoader => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };

            const string query = @"
                query {
                    units {
                        unitInfo {
                            id
                            count
                        }
                    }
                }";

            const string expected = @"
                {
                    units: [{
                        unitInfo: {
                            id: 1,
                            count: 3
                        }
                        }, {
                            unitInfo: {
                                id: 2,
                                count: 3
                        }
                    }]
                }";

            TestHelpers.TestQuery(builder, query, expected);

            batchFunc.Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Unit>>());
        }

        [Test]
        public void BatchFuncTupleOfIntAndIntUsingBuilderTest(
            [ValueSource(nameof(_loaderTypes))] LoaderType loaderType,
            [ValueSource(nameof(_fromBatchTypes))] FromBatchType batchType)
        {
            var batchFunc = Substitute.For<Func<TestUserContext, IEnumerable<int>, IDictionary<int, IEnumerable<Tuple<int, int>>>>>();
            batchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>())
                .Returns(callInfo =>
                    callInfo
                        .ArgAt<IEnumerable<int>>(1)
                        .ToDictionary(u => u, u => FakeData.People.Where(p => p.UnitId == u).Select(p => Tuple.Create(u, p.Id))));

            var builder = loaderType switch
            {
                LoaderType.Loader => CreateQueryBuilder(
                    loader =>
                    {
                        loader.Field("unitInfo")
                            .FromBatch(
                                batchType,
                                unit => unit.Id,
                                () => new TestUserContext(null),
                                batchFunc,
                                b =>
                                {
                                    b.Field("id", u => u.Item1);
                                    b.Field("peopleId", u => u.Item2);
                                });
                    }),
                LoaderType.MutableLoader => CreateQueryMutableBuilder(
                    loader =>
                    {
                        loader.Field(x => x.Name).Editable();
                        loader.Field("unitInfo")
                            .FromBatch(
                                batchType,
                                unit => unit.Id,
                                () => new TestUserContext(null),
                                batchFunc,
                                b =>
                                {
                                    b.Field("id", u => u.Item1);
                                    b.Field("peopleId", u => u.Item2);
                                });
                    }),
                LoaderType.IdentifiableLoader => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };

            const string query = @"
                query {
                    units {
                        unitInfo {
                            id,
                            peopleId
                        }
                    }
                }";
            const string expected = @"
                {
                    units: [{
                        unitInfo: [{
                            id: 1,
                            peopleId: 1
                        }, {
                            id: 1,
                            peopleId: 2
                        }, {
                            id: 1,
                            peopleId: 3
                        }]
                    }, {
                        unitInfo: [{
                            id: 2,
                            peopleId: 4
                        }, {
                            id: 2,
                            peopleId: 5
                        }, {
                            id: 2,
                            peopleId: 6
                        }]
                    }]
                }";

            TestHelpers.TestQuery(builder, query, expected);

            batchFunc.Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>());
        }

        [Test]
        public void BatchUnitAndPersonUsingSelectTest(
            [ValueSource(nameof(_loaderTypes))] LoaderType loaderType,
            [ValueSource(nameof(_fromBatchTypes))] FromBatchType batchType,
            [ValueSource(nameof(_fromBatchTypes))] FromBatchType childBatchType)
        {
            var batchFunc = Substitute.For<Func<TestUserContext, IEnumerable<int?>, IDictionary<int?, Person>>>();
            batchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int?>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int?>>(1).ToDictionary(
                    headId => headId,
                    headId => FakeData.People.FirstOrDefault(p => p.Id == headId)));

            var childrenBatchFunc = Substitute.For<Func<TestUserContext, IEnumerable<int>, IDictionary<int, Unit>>>();
            childrenBatchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(1).ToDictionary(
                    unit => unit,
                    unit => FakeData.Units.FirstOrDefault(c => c.ParentId == unit)));

            var builder = loaderType switch
            {
                LoaderType.Loader => CreateQueryBuilder(
                    loader =>
                    {
                        loader.Field(u => u.Id);
                        loader.Field("headId")
                            .FromBatch(
                                batchType,
                                u => u.HeadId,
                                () => new TestUserContext(null),
                                batchFunc)
                            .Select(p => p.Id);
                        loader.Field("child")
                            .FromBatch(
                                childBatchType,
                                u => u.Id,
                                () => new TestUserContext(null),
                                childrenBatchFunc,
                                build => build.ConfigureFrom(loader.GetType()));
                    },
                    filter: unit => unit.Id == 1),
                LoaderType.MutableLoader => CreateQueryMutableBuilder(
                    loader =>
                    {
                        loader.Field(u => u.Id);
                        loader.Field("headId")
                            .FromBatch(
                                batchType,
                                u => u.HeadId,
                                () => new TestUserContext(null),
                                batchFunc)
                            .Select(p => p.Id);
                        loader.Field("child")
                            .FromBatch(
                                childBatchType,
                                u => u.Id,
                                () => new TestUserContext(null),
                                childrenBatchFunc,
                                build => build.ConfigureFrom(loader.GetType()));
                    },
                    filter: unit => unit.Id == 1),
                LoaderType.IdentifiableLoader => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };

            const string query = @"
                query {
                    units {
                        id
                        headId
                        child {
                            id
                            headId
                        }
                    }
                }";

            const string expected = @"
                {
                    units: [{
                        id: 1,
                        headId: 1,
                        child: {
                            id: 2,
                            headId: 2
                        }
                    }]
                }";

            TestHelpers.TestQuery(builder, query, expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int?>>());

            childrenBatchFunc.Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>());
        }

        [Test]
        public void BatchFromBatchTest(
            [ValueSource(nameof(_loaderTypes))] LoaderType loaderType,
            [ValueSource(nameof(_fromBatchTypes))] FromBatchType batchType)
        {
            var batchUnitPeopleFunc = Substitute.For<Func<IEnumerable<int>, IDictionary<int, IEnumerable<Tuple<int, int>>>>>();
            batchUnitPeopleFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo =>
                    callInfo
                        .ArgAt<IEnumerable<int>>(0)
                        .GroupBy(id => id)
                        .ToDictionary(u => u.First(), u => FakeData.People.Where(p => p.UnitId == u.Key).Select(p => Tuple.Create(u.Key, p.Id))));

            var batchManagerFunc = Substitute.For<Func<TestUserContext, IEnumerable<int>, IDictionary<int, Person>>>();
            batchManagerFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>())
                .Returns(callInfo =>
                    callInfo
                        .ArgAt<IEnumerable<int>>(1)
                        .ToDictionary(id => id, id => FakeData.People.Where(p2 => p2.Id == id).Select(p2 => p2.Manager).FirstOrDefault()));

            var builder = loaderType switch
            {
                LoaderType.Loader => CreateQueryBuilder(
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
                                        .FromBatch(
                                            batchType,
                                            p => p.Item2,
                                            () => new TestUserContext(null),
                                            batchManagerFunc);
                                });
                    }),
                LoaderType.MutableLoader => CreateQueryMutableBuilder(
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
                                        .FromBatch(
                                            batchType,
                                            p => p.Item2,
                                            () => new TestUserContext(null),
                                            batchManagerFunc);
                                });
                    }),
                LoaderType.IdentifiableLoader => throw new NotImplementedException(),
                _ => throw new NotImplementedException(),
            };

            const string query = @"
                query {
                    units {
                        unitInfo {
                            id,
                            peopleId
                            manager {
                                id
                            }
                        }
                    }
                }";
            const string expected = @"
                {
                    units: [{
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
                }";

            TestHelpers.TestQuery(builder, query, expected);

            batchUnitPeopleFunc.Received(1)
                .Invoke(Arg.Any<IEnumerable<int>>());
            batchManagerFunc.Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>());
        }

        private Action<Query<TestUserContext>> CreateQueryBuilder(Action<Loader<Unit, TestUserContext>> userLoaderBuilder, Action<Query<TestUserContext>> configure = null, Expression<Func<Unit, bool>> filter = null)
        {
            var unitLoader = GraphQLTypeBuilder.CreateLoaderType(
                onConfigure: userLoaderBuilder,
                getBaseQuery: _ => filter == null
                    ? FakeData.Units.AsQueryable()
                    : FakeData.Units.AsQueryable().Where(filter));

            return query =>
            {
                query.Field("units").FromLoader<Unit, TestUserContext>(unitLoader);
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
                query.Field("units").FromLoader<Unit, TestUserContext>(unitLoader);
                configure?.Invoke(query);
            };
        }
    }
}
