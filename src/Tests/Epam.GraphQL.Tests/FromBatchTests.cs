// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
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
        [Test(Description = "FromBatch(...)/int/context")]
        public void BatchFuncFromContextAndUnitToUnitAndIntTest()
        {
            var batchFunc = Substitute.For<Func<TestUserContext, IEnumerable<Unit>, IDictionary<Unit, int>>>();
            batchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Unit>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Unit>>(1).ToDictionary(u => u, u => FakeData.People.Count(p => p.UnitId == u.Id)));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("empCount")
                        .FromBatch(batchFunc);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("empCount")
                        .FromBatch(batchFunc);
                });

            const string Query = @"
                query {
                    units {
                        items {
                            id
                            empCount
                        }
                    }
                }";

            const string Expected = @"
                {
                    units: {
                        items: [{
                            id: 1,
                            empCount: 3
                        }, {
                            id: 2,
                            empCount: 3
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);
            batchFunc.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Unit>>());
        }

        [Test(Description = "FromBatch(...)/int")]
        public void BatchFuncFromUnitToUnitAndIntTest()
        {
            var batchFunc = Substitute.For<Func<IEnumerable<Unit>, IDictionary<Unit, int>>>();
            batchFunc
                .Invoke(Arg.Any<IEnumerable<Unit>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Unit>>(0).ToDictionary(u => u, u => FakeData.People.Count(p => p.UnitId == u.Id)));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("empCount")
                        .FromBatch(batchFunc);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("empCount")
                        .FromBatch(batchFunc);
                });

            const string Query = @"
                query {
                    units {
                        items {
                            id
                            empCount
                        }
                    }
                }";

            const string Expected = @"
                {
                    units: {
                        items: [{
                            id: 1,
                            empCount: 3
                        }, {
                            id: 2,
                            empCount: 3
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);
            batchFunc.Received(2)
                .Invoke(Arg.Any<IEnumerable<Unit>>());
        }

        [Test(Description = "FromBatch(...)/int using key selector/context")]
        public void BatchFuncFromContextAndIntToIntAndIntUsingKeySelectorTest()
        {
            var batchFunc = Substitute.For<Func<TestUserContext, IEnumerable<int>, IDictionary<int, int>>>();
            batchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(1).ToDictionary(id => id, id => FakeData.People.Count(p => p.UnitId == id)));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("empCount")
                        .FromBatch(u => u.Id, batchFunc);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("empCount")
                        .FromBatch(u => u.Id, batchFunc);
                });

            const string Query = @"
                query {
                    units {
                        items {
                            id
                            empCount
                        }
                    }
                }";

            const string Expected = @"
                {
                    units: {
                        items: [{
                            id: 1,
                            empCount: 3
                        }, {
                            id: 2,
                            empCount: 3
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);
            batchFunc.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>());
        }

        [Test(Description = "FromBatch(...)/int using key selector")]
        public void BatchFuncFromIntToIntAndIntUsingKeySelectorTest()
        {
            var batchFunc = Substitute.For<Func<IEnumerable<int>, IDictionary<int, int>>>();
            batchFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(0).ToDictionary(id => id, id => FakeData.People.Count(p => p.UnitId == id)));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("empCount")
                        .FromBatch(u => u.Id, batchFunc);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("empCount")
                        .FromBatch(u => u.Id, batchFunc);
                });

            const string Query = @"
                query {
                    units {
                        items {
                            id
                            empCount
                        }
                    }
                }";

            const string Expected = @"
                {
                    units: {
                        items: [{
                            id: 1,
                            empCount: 3
                        }, {
                            id: 2,
                            empCount: 3
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);
            batchFunc.Received(2)
                .Invoke(Arg.Any<IEnumerable<int>>());
        }

        [Test(Description = "FromBatch(...)/IEnumerable<int>/context")]
        public void BatchFuncFromContextAndUnitToUnitAndEnumerableOfIntTest()
        {
            var batchFunc = Substitute.For<Func<TestUserContext, IEnumerable<Unit>, IDictionary<Unit, IEnumerable<int>>>>();
            batchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Unit>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Unit>>(1).ToDictionary(u => u, u => FakeData.People.Where(p => p.UnitId == u.Id).Select(p => p.Id)));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("empIds")
                        .FromBatch(batchFunc);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("empIds")
                        .FromBatch(batchFunc);
                });

            const string Query = @"
                query {
                    units {
                        items {
                            id
                            empIds
                        }
                    }
                }";

            const string Expected = @"
                {
                    units: {
                        items: [{
                            id: 1,
                            empIds: [1,2,3]
                        }, {
                            id: 2,
                            empIds: [4,5,6]
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);
            batchFunc.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Unit>>());
        }

        [Test(Description = "FromBatch(...)/IEnumerable<int>")]
        public void BatchFuncFromUnitToUnitAndEnumerableOfIntTest()
        {
            var batchFunc = Substitute.For<Func<IEnumerable<Unit>, IDictionary<Unit, IEnumerable<int>>>>();
            batchFunc
                .Invoke(Arg.Any<IEnumerable<Unit>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Unit>>(0).ToDictionary(u => u, u => FakeData.People.Where(p => p.UnitId == u.Id).Select(p => p.Id)));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("empIds")
                        .FromBatch(batchFunc);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("empIds")
                        .FromBatch(batchFunc);
                });

            const string Query = @"
                query {
                    units {
                        items {
                            id
                            empIds
                        }
                    }
                }";

            const string Expected = @"
                {
                    units: {
                        items: [{
                            id: 1,
                            empIds: [1,2,3]
                        }, {
                            id: 2,
                            empIds: [4,5,6]
                        }]
                    }
                }";

            TestHelpers.TestQuery(mutableBuilder, Query, Expected);
            TestHelpers.TestQuery(builder, Query, Expected);
            batchFunc.Received(2)
                .Invoke(Arg.Any<IEnumerable<Unit>>());
        }

        [Test(Description = "FromBatch(...)/IEnumerable<int>")]
        public void BatchFuncFromUnitToUnitAndArrayOfIntTest()
        {
            var batchFunc = Substitute.For<Func<IEnumerable<Unit>, IDictionary<Unit, int[]>>>();
            batchFunc
                .Invoke(Arg.Any<IEnumerable<Unit>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Unit>>(0).ToDictionary(u => u, u => FakeData.People.Where(p => p.UnitId == u.Id).Select(p => p.Id).ToArray()));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("empIds")
                        .FromBatch(batchFunc);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("empIds")
                        .FromBatch(batchFunc);
                });

            const string Query = @"
                query {
                    units {
                        items {
                            id
                            empIds
                        }
                    }
                }";

            const string Expected = @"
                {
                    units: {
                        items: [{
                            id: 1,
                            empIds: [1,2,3]
                        }, {
                            id: 2,
                            empIds: [4,5,6]
                        }]
                    }
                }";

            TestHelpers.TestQuery(mutableBuilder, Query, Expected);
            TestHelpers.TestQuery(builder, Query, Expected);
            batchFunc.Received(2)
                .Invoke(Arg.Any<IEnumerable<Unit>>());
        }

        [Test(Description = "FromBatch(...)/IEnumerable<int> using key selector/context")]
        public void BatchFuncFromContextAndIntToIntAndEnumerableOfIntUsingKeySelectorTest()
        {
            var batchFunc = Substitute.For<Func<TestUserContext, IEnumerable<int>, IDictionary<int, IEnumerable<int>>>>();
            batchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(1).ToDictionary(id => id, id => FakeData.People.Where(p => p.UnitId == id).Select(p => p.Id)));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("empIds")
                        .FromBatch(u => u.Id, batchFunc);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("empIds")
                        .FromBatch(u => u.Id, batchFunc);
                });

            const string Query = @"
                query {
                    units {
                        items {
                            id
                            empIds
                        }
                    }
                }";

            const string Expected = @"
                {
                    units: {
                        items: [{
                            id: 1,
                            empIds: [1,2,3]
                        }, {
                            id: 2,
                            empIds: [4,5,6]
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>());
        }

        [Test(Description = "FromBatch(...)/IEnumerable<int> using key selector")]
        public void BatchFuncFromIntToIntAndEnumerableOfIntUsingKeySelectorTest()
        {
            var batchFunc = Substitute.For<Func<IEnumerable<int>, IDictionary<int, IEnumerable<int>>>>();
            batchFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(0).ToDictionary(id => id, id => FakeData.People.Where(p => p.UnitId == id).Select(p => p.Id)));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("empIds")
                        .FromBatch(u => u.Id, batchFunc);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("empIds")
                        .FromBatch(u => u.Id, batchFunc);
                });

            const string Query = @"
                query {
                    units {
                        items {
                            id
                            empIds
                        }
                    }
                }";

            const string Expected = @"
                {
                    units: {
                        items: [{
                            id: 1,
                            empIds: [1,2,3]
                        }, {
                            id: 2,
                            empIds: [4,5,6]
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<IEnumerable<int>>());
        }

        [Test(Description = "FromBatch(...)/IEnumerable<int?> using key selector/context")]
        public void BatchFuncFromContextAndNullableIntToNullableIntAndPersonUsingKeySelectorTest()
        {
            var batchFunc = Substitute.For<Func<TestUserContext, IEnumerable<int?>, IDictionary<int?, Person>>>();
            batchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int?>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int?>>(1).ToDictionary(headId => headId, headId => FakeData.People.FirstOrDefault(p => p.Id == headId)));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("head")
                        .FromBatch(u => u.HeadId, batchFunc, null);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("head")
                        .FromBatch(u => u.HeadId, batchFunc, null);
                });

            const string Query = @"
                query {
                    units {
                        items {
                            id
                            head {
                                id
                            }
                        }
                    }
                }";

            const string Expected = @"
                {
                    units: {
                        items: [{
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
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int?>>());
        }

        [Test(Description = "FromBatch(...)/IEnumerable<int?> using key selector")]
        public void BatchFuncFromNullableIntToNullableIntAndPersonUsingKeySelectorTest()
        {
            var batchFunc = Substitute.For<Func<IEnumerable<int?>, IDictionary<int?, Person>>>();
            batchFunc
                .Invoke(Arg.Any<IEnumerable<int?>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int?>>(0).ToDictionary(headId => headId, headId => FakeData.People.FirstOrDefault(p => p.Id == headId)));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("head")
                        .FromBatch(u => u.HeadId, batchFunc, null);
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("head")
                        .FromBatch(u => u.HeadId, batchFunc, null);
                });

            const string Query = @"
                query {
                    units {
                        items {
                            id
                            head {
                                id
                            }
                        }
                    }
                }";

            const string Expected = @"
                {
                    units: {
                        items: [{
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
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<IEnumerable<int?>>());
        }

        [Test(Description = "FromBatch(..., builder)/int/context")]
        public void BatchFuncFromContextAndUnitToUnitAndTupleOfUnitAndIntUsingBuilderTest()
        {
            var batchFunc = Substitute.For<Func<TestUserContext, IEnumerable<Unit>, IDictionary<Unit, Tuple<Unit, int>>>>();
            batchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Unit>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Unit>>(1).ToDictionary(u => u, u => Tuple.Create(u, FakeData.People.Count(p => p.UnitId == u.Id))));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field("unitInfo")
                        .FromBatch(
                            batchFunc,
                            b =>
                            {
                                b.Field("id", u => u.Item1.Id);
                                b.Field("count", u => u.Item2);
                            });
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field("unitInfo")
                        .FromBatch(
                            batchFunc,
                            b =>
                            {
                                b.Field("id", u => u.Item1.Id);
                                b.Field("count", u => u.Item2);
                            });
                });

            const string Query = @"
                query {
                    units {
                        items {
                            unitInfo {
                                id
                                count
                            }
                        }
                    }
                }";

            const string Expected = @"
                {
                    units: {
                        items: [{
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
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Unit>>());
        }

        [Test(Description = "FromBatch(..., builder)/int")]
        public void BatchFuncFromUnitToUnitAndTupleOfUnitAndIntUsingBuilderTest()
        {
            var batchFunc = Substitute.For<Func<IEnumerable<Unit>, IDictionary<Unit, Tuple<Unit, int>>>>();
            batchFunc
                .Invoke(Arg.Any<IEnumerable<Unit>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Unit>>(0).ToDictionary(u => u, u => Tuple.Create(u, FakeData.People.Count(p => p.UnitId == u.Id))));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field("unitInfo")
                        .FromBatch(
                            batchFunc,
                            b =>
                            {
                                b.Field("id", u => u.Item1.Id);
                                b.Field("count", u => u.Item2);
                            });
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field("unitInfo")
                        .FromBatch(
                            batchFunc,
                            b =>
                            {
                                b.Field("id", u => u.Item1.Id);
                                b.Field("count", u => u.Item2);
                            });
                });

            const string Query = @"
                query {
                    units {
                        items {
                            unitInfo {
                                id
                                count
                            }
                        }
                    }
                }";

            const string Expected = @"
                {
                    units: {
                        items: [{
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
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<IEnumerable<Unit>>());
        }

        [Test(Description = "FromBatch(..., builder)/int using key selector/context")]
        public void BatchFuncFromContextAndIntToIntAndTupleOfIntAndIntUsingBuilderAndKeySelectorTest()
        {
            var batchFunc = Substitute.For<Func<TestUserContext, IEnumerable<int>, IDictionary<int, Tuple<int, int>>>>();
            batchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(1).ToDictionary(id => id, id => Tuple.Create(id, FakeData.People.Count(p => p.UnitId == id))));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field("unitInfo")
                        .FromBatch(
                            u => u.Id,
                            batchFunc,
                            b =>
                            {
                                b.Field("id", u => u.Item1);
                                b.Field("count", u => u.Item2);
                            });
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field("unitInfo")
                        .FromBatch(
                            u => u.Id,
                            batchFunc,
                            b =>
                            {
                                b.Field("id", u => u.Item1);
                                b.Field("count", u => u.Item2);
                            });
                });

            const string Query = @"
                query {
                    units {
                        items {
                            unitInfo {
                                id
                                count
                            }
                        }
                    }
                }";

            const string Expected = @"
                {
                    units: {
                        items: [{
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
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>());
        }

        [Test(Description = "FromBatch(..., builder)/int using key selector")]
        public void BatchFuncFromIntToIntAndTupleOfIntAndIntUsingBuilderAndKeySelectorTest()
        {
            var batchFunc = Substitute.For<Func<IEnumerable<int>, IDictionary<int, Tuple<int, int>>>>();
            batchFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(0).ToDictionary(id => id, id => Tuple.Create(id, FakeData.People.Count(p => p.UnitId == id))));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field("unitInfo")
                        .FromBatch(
                            u => u.Id,
                            batchFunc,
                            b =>
                            {
                                b.Field("id", u => u.Item1);
                                b.Field("count", u => u.Item2);
                            });
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field("unitInfo")
                        .FromBatch(
                            u => u.Id,
                            batchFunc,
                            b =>
                            {
                                b.Field("id", u => u.Item1);
                                b.Field("count", u => u.Item2);
                            });
                });

            const string Query = @"
                query {
                    units {
                        items {
                            unitInfo {
                                id
                                count
                            }
                        }
                    }
                }";

            const string Expected = @"
                {
                    units: {
                        items: [{
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
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<IEnumerable<int>>());
        }

        [Test(Description = "FromBatch(..., builder)/IEnumerable<int>/context")]
        public void BatchFuncFromContextAndUnitToUnitAndEnumerableOfTupleOfIntAndIntUsingBuilderTest()
        {
            var batchFunc = Substitute.For<Func<TestUserContext, IEnumerable<Unit>, IDictionary<Unit, IEnumerable<Tuple<int, int>>>>>();
            batchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Unit>>())
                .Returns(callInfo =>
                    callInfo
                        .ArgAt<IEnumerable<Unit>>(1)
                        .GroupBy(u => u.Id)
                        .ToDictionary(u => u.First(), u => FakeData.People.Where(p => p.UnitId == u.Key).Select(p => Tuple.Create(u.Key, p.Id))));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field("unitInfo")
                        .FromBatch(
                            batchFunc,
                            b =>
                            {
                                b.Field("id", u => u.Item1);
                                b.Field("peopleId", u => u.Item2);
                            });
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field("unitInfo")
                        .FromBatch(
                            batchFunc,
                            b =>
                            {
                                b.Field("id", u => u.Item1);
                                b.Field("peopleId", u => u.Item2);
                            });
                });

            const string Query = @"
                query {
                    units {
                        items {
                            unitInfo {
                                id,
                                peopleId
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
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Unit>>());
        }

        [Test(Description = "FromBatch(..., builder)/IEnumerable<int>")]
        public void BatchFuncFromUnitToUnitAndEnumerableOfTipleOfIntAndIntUsingBuilderTest()
        {
            var batchFunc = Substitute.For<Func<IEnumerable<Unit>, IDictionary<Unit, IEnumerable<Tuple<int, int>>>>>();
            batchFunc
                .Invoke(Arg.Any<IEnumerable<Unit>>())
                .Returns(callInfo =>
                    callInfo
                        .ArgAt<IEnumerable<Unit>>(0)
                        .GroupBy(u => u.Id)
                        .ToDictionary(u => u.First(), u => FakeData.People.Where(p => p.UnitId == u.Key).Select(p => Tuple.Create(u.Key, p.Id))));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field("unitInfo")
                        .FromBatch(
                            batchFunc,
                            b =>
                            {
                                b.Field("id", u => u.Item1);
                                b.Field("peopleId", u => u.Item2);
                            });
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field("unitInfo")
                        .FromBatch(
                            batchFunc,
                            b =>
                            {
                                b.Field("id", u => u.Item1);
                                b.Field("peopleId", u => u.Item2);
                            });
                });

            const string Query = @"
                query {
                    units {
                        items {
                            unitInfo {
                                id,
                                peopleId
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
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<IEnumerable<Unit>>());
        }

        [Test(Description = "FromBatch(..., builder)/IEnumerable<int> using key selector/context")]
        public void BatchFuncFromContextAndIntToIntAndEnumerableOfTupleOfIntAndIntUsingBuilderAndKeySelectorTest()
        {
            var batchFunc = Substitute.For<Func<TestUserContext, IEnumerable<int>, IDictionary<int, IEnumerable<Tuple<int, int>>>>>();
            batchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>())
                .Returns(callInfo =>
                    callInfo
                        .ArgAt<IEnumerable<int>>(1)
                        .GroupBy(id => id)
                        .ToDictionary(u => u.First(), u => FakeData.People.Where(p => p.UnitId == u.Key).Select(p => Tuple.Create(u.Key, p.Id))));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field("unitInfo")
                        .FromBatch(
                            u => u.Id,
                            batchFunc,
                            b =>
                            {
                                b.Field("id", u => u.Item1);
                                b.Field("peopleId", u => u.Item2);
                            });
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field("unitInfo")
                        .FromBatch(
                            u => u.Id,
                            batchFunc,
                            b =>
                            {
                                b.Field("id", u => u.Item1);
                                b.Field("peopleId", u => u.Item2);
                            });
                });

            const string Query = @"
                query {
                    units {
                        items {
                            unitInfo {
                                id,
                                peopleId
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
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>());
        }

        [Test(Description = "FromBatch(..., builder)/IEnumerable<int> using key selector")]
        public void BatchFuncFromIntToIntAndEnumerableOfTupleOfIntAndIntUsingBuilderAndKeySelectorTest()
        {
            var batchFunc = Substitute.For<Func<IEnumerable<int>, IDictionary<int, IEnumerable<Tuple<int, int>>>>>();
            batchFunc
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
                            batchFunc,
                            b =>
                            {
                                b.Field("id", u => u.Item1);
                                b.Field("peopleId", u => u.Item2);
                            });
                });

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field("unitInfo")
                        .FromBatch(
                            u => u.Id,
                            batchFunc,
                            b =>
                            {
                                b.Field("id", u => u.Item1);
                                b.Field("peopleId", u => u.Item2);
                            });
                });

            const string Query = @"
                query {
                    units {
                        items {
                            unitInfo {
                                id,
                                peopleId
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
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<IEnumerable<int>>());
        }

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
        public void BatchFuncFromContextAndUnitToUnitAndPersonUsingSelectTest()
        {
            var batchFunc = Substitute.For<Func<TestUserContext, IEnumerable<Unit>, IDictionary<Unit, Person>>>();
            batchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Unit>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Unit>>(1).ToDictionary(
                    unit => unit,
                    unit => FakeData.People.FirstOrDefault(p => p.Id == unit.HeadId)));

            var childrenBatchFunc = Substitute.For<Func<TestUserContext, IEnumerable<Unit>, IDictionary<Unit, Unit>>>();
            childrenBatchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Unit>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Unit>>(1).ToDictionary(
                    unit => unit,
                    unit => FakeData.Units.FirstOrDefault(c => c.ParentId == unit.Id)));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("headId")
                        .FromBatch(batchFunc).Select(p => p.Id);
                    loader.Field("child")
                        .FromBatch(childrenBatchFunc, build => build.ConfigureFrom(loader.GetType()));
                },
                filter: unit => unit.Id == 1);

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field("headId")
                        .FromBatch(batchFunc).Select(p => p.Id);
                    loader.Field("child")
                        .FromBatch(childrenBatchFunc, build => build.ConfigureFrom(loader.GetType()));
                },
                filter: unit => unit.Id == 1);

            const string Query = @"
                query {
                    units {
                        items {
                            id
                            headId
                            child {
                                id
                                headId
                            }
                        }
                    }
                }";

            const string Expected = @"
                {
                    units: {
                        items: [{
                            id: 1,
                            headId: 1,
                            child: {
                                id: 2,
                                headId: 2
                            }
                        }]
                    }
                }";

            TestHelpers.TestQuery(builder, Query, Expected);
            TestHelpers.TestQuery(mutableBuilder, Query, Expected);

            batchFunc.Received(4)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Unit>>());

            childrenBatchFunc.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Unit>>());
        }

        [Test]
        public void BatchFromBatchTest()
        {
            var batchUnitPeopleFunc = Substitute.For<Func<IEnumerable<int>, IDictionary<int, IEnumerable<Tuple<int, int>>>>>();
            batchUnitPeopleFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo =>
                    callInfo
                        .ArgAt<IEnumerable<int>>(0)
                        .GroupBy(id => id)
                        .ToDictionary(u => u.First(), u => FakeData.People.Where(p => p.UnitId == u.Key).Select(p => Tuple.Create(u.Key, p.Id))));

            var batchManagerFunc = Substitute.For<Func<IEnumerable<int>, IDictionary<int, Person>>>();
            batchManagerFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo =>
                    callInfo
                        .ArgAt<IEnumerable<int>>(0)
                        .ToDictionary(id => id, id => FakeData.People.Where(p2 => p2.Id == id).Select(p2 => p2.Manager).FirstOrDefault()));

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
                                    .FromBatch(p => p.Item2, batchManagerFunc, null);
                            });
                });

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
                                    .FromBatch(p => p.Item2, batchManagerFunc, null);
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
            batchManagerFunc.Received(2)
                .Invoke(Arg.Any<IEnumerable<int>>());
        }

        [Test]
        public void BatchFromBatchTaskTest()
        {
            var batchUnitPeopleFunc = Substitute.For<Func<IEnumerable<int>, Task<IDictionary<int, IEnumerable<Tuple<int, int>>>>>>();
            batchUnitPeopleFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo =>
                    Task.FromResult<IDictionary<int, IEnumerable<Tuple<int, int>>>>(
                        callInfo
                            .ArgAt<IEnumerable<int>>(0)
                            .GroupBy(id => id)
                            .ToDictionary(u => u.First(), u => FakeData.People.Where(p => p.UnitId == u.Key).Select(p => Tuple.Create(u.Key, p.Id)))));

            var batchManagerFunc = Substitute.For<Func<IEnumerable<int>, Task<IDictionary<int, Person>>>>();
            batchManagerFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo =>
                    Task.FromResult<IDictionary<int, Person>>(
                    callInfo
                        .ArgAt<IEnumerable<int>>(0)
                        .ToDictionary(id => id, id => FakeData.People.Where(p2 => p2.Id == id).Select(p2 => p2.Manager).FirstOrDefault())));

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
                                    .FromBatch(p => p.Item2, batchManagerFunc, null);
                            });
                });

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
                                    .FromBatch(p => p.Item2, batchManagerFunc, null);
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
            batchManagerFunc.Received(2)
                .Invoke(Arg.Any<IEnumerable<int>>());
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
