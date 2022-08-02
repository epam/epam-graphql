// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NSubstitute;
using NUnit.Framework;

namespace Epam.GraphQL.Tests
{
    [TestFixture]
    public class FromBatchEnumerableTests : BaseTests
    {
        private IDataContext _dataContext;

        [Test]
        public void StructEnumerableTestWithContext()
        {
            var batchFunc = Substitute.For<Func<TestUserContext, IEnumerable<Unit>, IDictionary<Unit, IEnumerable<int>>>>();
            batchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Unit>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Unit>>(1).ToDictionary(unit => unit, unit => FakeData.People.Where(p => p.UnitId == unit.Id).Select(p => p.Id)));

            var onWriteFunc = Substitute.For<Action<TestUserContext, Unit, IEnumerable<int>>>();

            void UnitLoaderBuilder(MutableLoader<Unit, int, TestUserContext> loader)
            {
                loader.Field(u => u.Id);
                loader.Field("empIds")
                    .FromBatch(batchFunc)
                    .OnWrite(onWriteFunc)
                    .Editable();
            }

            var query = CreateQueryMutableBuilder(UnitLoaderBuilder);
            var mutation = CreateMutationMutableBuilder(UnitLoaderBuilder);

            const string Query = @"
                mutation {
                    submit(payload: {
                        units: [{
                            id: 1,
                            empIds: [4, 5, 6]
                        }]
                    }) {
                        units {
                            id
                            payload {
                                empIds
                            }
                        }
                    }
                }";

            const string Expected = @"
                {
                    submit: {
                        units: [{
                            id: 1,
                            payload: {
                                empIds: [1,2,3]
                            }
                        }]
                    }
                }";

            TestHelpers.TestMutation(query, mutation, _dataContext, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Unit>>());

            onWriteFunc.Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Unit>(), Arg.Is<IEnumerable<int>>(value => Enumerable.SequenceEqual(value, new[] { 4, 5, 6 })));
        }

        [Test]
        public void StructEnumerableTestWithContextAndKey()
        {
            var batchFunc = Substitute.For<Func<TestUserContext, IEnumerable<int>, IDictionary<int, IEnumerable<int>>>>();
            batchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(1).ToDictionary(id => id, id => FakeData.People.Where(p => p.UnitId == id).Select(p => p.Id)));

            var onWriteFunc = Substitute.For<Action<TestUserContext, Unit, IEnumerable<int>>>();

            void UnitLoaderBuilder(MutableLoader<Unit, int, TestUserContext> loader)
            {
                loader.Field(u => u.Id);
                loader.Field("empIds")
                    .FromBatch(unit => unit.Id, batchFunc)
                    .OnWrite(onWriteFunc)
                    .Editable();
            }

            var query = CreateQueryMutableBuilder(UnitLoaderBuilder);
            var mutation = CreateMutationMutableBuilder(UnitLoaderBuilder);

            const string Query = @"
                mutation {
                    submit(payload: {
                        units: [{
                            id: 1,
                            empIds: [4, 5, 6]
                        }]
                    }) {
                        units {
                            id
                            payload {
                                empIds
                            }
                        }
                    }
                }";

            const string Expected = @"
                {
                    submit: {
                        units: [{
                            id: 1,
                            payload: {
                                empIds: [1,2,3]
                            }
                        }]
                    }
                }";

            TestHelpers.TestMutation(query, mutation, _dataContext, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>());

            onWriteFunc.Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Unit>(), Arg.Is<IEnumerable<int>>(value => Enumerable.SequenceEqual(value, new[] { 4, 5, 6 })));
        }

        [Test]
        public void StructEnumerableTest()
        {
            var batchFunc = Substitute.For<Func<IEnumerable<Unit>, IDictionary<Unit, IEnumerable<int>>>>();
            batchFunc
                .Invoke(Arg.Any<IEnumerable<Unit>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Unit>>(0).ToDictionary(unit => unit, unit => FakeData.People.Where(p => p.UnitId == unit.Id).Select(p => p.Id)));

            var onWriteFunc = Substitute.For<Action<TestUserContext, Unit, IEnumerable<int>>>();

            void UnitLoaderBuilder(MutableLoader<Unit, int, TestUserContext> loader)
            {
                loader.Field(u => u.Id);
                loader.Field("empIds")
                    .FromBatch(batchFunc)
                    .OnWrite(onWriteFunc)
                    .Editable();
            }

            var query = CreateQueryMutableBuilder(UnitLoaderBuilder);
            var mutation = CreateMutationMutableBuilder(UnitLoaderBuilder);

            const string Query = @"
                mutation {
                    submit(payload: {
                        units: [{
                            id: 1,
                            empIds: [4, 5, 6]
                        }]
                    }) {
                        units {
                            id
                            payload {
                                empIds
                            }
                        }
                    }
                }";

            const string Expected = @"
                {
                    submit: {
                        units: [{
                            id: 1,
                            payload: {
                                empIds: [1,2,3]
                            }
                        }]
                    }
                }";

            TestHelpers.TestMutation(query, mutation, _dataContext, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<IEnumerable<Unit>>());

            onWriteFunc.Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Unit>(), Arg.Is<IEnumerable<int>>(value => Enumerable.SequenceEqual(value, new[] { 4, 5, 6 })));
        }

        [Test]
        public void StructEnumerableTestWithKey()
        {
            var batchFunc = Substitute.For<Func<IEnumerable<int>, IDictionary<int, IEnumerable<int>>>>();
            batchFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(0).ToDictionary(id => id, id => FakeData.People.Where(p => p.UnitId == id).Select(p => p.Id)));

            var onWriteFunc = Substitute.For<Action<TestUserContext, Unit, IEnumerable<int>>>();

            void UnitLoaderBuilder(MutableLoader<Unit, int, TestUserContext> loader)
            {
                loader.Field(u => u.Id);
                loader.Field("empIds")
                    .FromBatch(unit => unit.Id, batchFunc)
                    .OnWrite(onWriteFunc)
                    .Editable();
            }

            var query = CreateQueryMutableBuilder(UnitLoaderBuilder);
            var mutation = CreateMutationMutableBuilder(UnitLoaderBuilder);

            const string Query = @"
                mutation {
                    submit(payload: {
                        units: [{
                            id: 1,
                            empIds: [4, 5, 6]
                        }]
                    }) {
                        units {
                            id
                            payload {
                                empIds
                            }
                        }
                    }
                }";

            const string Expected = @"
                {
                    submit: {
                        units: [{
                            id: 1,
                            payload: {
                                empIds: [1,2,3]
                            }
                        }]
                    }
                }";

            TestHelpers.TestMutation(query, mutation, _dataContext, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<IEnumerable<int>>());

            onWriteFunc.Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Unit>(), Arg.Is<IEnumerable<int>>(value => Enumerable.SequenceEqual(value, new[] { 4, 5, 6 })));
        }

        [Test]
        public void NullableStructEnumerableTestWithContext()
        {
            var batchFunc = Substitute.For<Func<TestUserContext, IEnumerable<Unit>, IDictionary<Unit, IEnumerable<int?>>>>();
            batchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Unit>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Unit>>(1).ToDictionary(unit => unit, unit => FakeData.People.Where(p => p.UnitId == unit.Id).Select(p => (int?)p.Id)));

            var onWriteFunc = Substitute.For<Action<TestUserContext, Unit, IEnumerable<int?>>>();

            void UnitLoaderBuilder(MutableLoader<Unit, int, TestUserContext> loader)
            {
                loader.Field(u => u.Id);
                loader.Field("empIds")
                    .FromBatch(batchFunc)
                    .OnWrite(onWriteFunc)
                    .Editable();
            }

            var query = CreateQueryMutableBuilder(UnitLoaderBuilder);
            var mutation = CreateMutationMutableBuilder(UnitLoaderBuilder);

            const string Query = @"
                mutation {
                    submit(payload: {
                        units: [{
                            id: 1,
                            empIds: [4, 5, 6]
                        }]
                    }) {
                        units {
                            id
                            payload {
                                empIds
                            }
                        }
                    }
                }";

            const string Expected = @"
                {
                    submit: {
                        units: [{
                            id: 1,
                            payload: {
                                empIds: [1,2,3]
                            }
                        }]
                    }
                }";

            TestHelpers.TestMutation(query, mutation, _dataContext, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Unit>>());

            onWriteFunc.Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Unit>(), Arg.Is<IEnumerable<int?>>(value => Enumerable.SequenceEqual(value, new int?[] { 4, 5, 6 })));
        }

        [Test]
        public void NullableStructEnumerableTestWithContextAndKey()
        {
            var batchFunc = Substitute.For<Func<TestUserContext, IEnumerable<int>, IDictionary<int, IEnumerable<int?>>>>();
            batchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(1).ToDictionary(id => id, id => FakeData.People.Where(p => p.UnitId == id).Select(p => (int?)p.Id)));

            var onWriteFunc = Substitute.For<Action<TestUserContext, Unit, IEnumerable<int?>>>();

            void UnitLoaderBuilder(MutableLoader<Unit, int, TestUserContext> loader)
            {
                loader.Field(u => u.Id);
                loader.Field("empIds")
                    .FromBatch(unit => unit.Id, batchFunc)
                    .OnWrite(onWriteFunc)
                    .Editable();
            }

            var query = CreateQueryMutableBuilder(UnitLoaderBuilder);
            var mutation = CreateMutationMutableBuilder(UnitLoaderBuilder);

            const string Query = @"
                mutation {
                    submit(payload: {
                        units: [{
                            id: 1,
                            empIds: [4, 5, 6]
                        }]
                    }) {
                        units {
                            id
                            payload {
                                empIds
                            }
                        }
                    }
                }";

            const string Expected = @"
                {
                    submit: {
                        units: [{
                            id: 1,
                            payload: {
                                empIds: [1,2,3]
                            }
                        }]
                    }
                }";

            TestHelpers.TestMutation(query, mutation, _dataContext, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>());

            onWriteFunc.Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Unit>(), Arg.Is<IEnumerable<int?>>(value => Enumerable.SequenceEqual(value, new int?[] { 4, 5, 6 })));
        }

        [Test]
        public void NullableStructEnumerableTest()
        {
            var batchFunc = Substitute.For<Func<IEnumerable<Unit>, IDictionary<Unit, IEnumerable<int?>>>>();
            batchFunc
                .Invoke(Arg.Any<IEnumerable<Unit>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Unit>>(0).ToDictionary(unit => unit, unit => FakeData.People.Where(p => p.UnitId == unit.Id).Select(p => (int?)p.Id)));

            var onWriteFunc = Substitute.For<Action<TestUserContext, Unit, IEnumerable<int?>>>();

            void UnitLoaderBuilder(MutableLoader<Unit, int, TestUserContext> loader)
            {
                loader.Field(u => u.Id);
                loader.Field("empIds")
                    .FromBatch(batchFunc)
                    .OnWrite(onWriteFunc)
                    .Editable();
            }

            var query = CreateQueryMutableBuilder(UnitLoaderBuilder);
            var mutation = CreateMutationMutableBuilder(UnitLoaderBuilder);

            const string Query = @"
                mutation {
                    submit(payload: {
                        units: [{
                            id: 1,
                            empIds: [4, 5, 6]
                        }]
                    }) {
                        units {
                            id
                            payload {
                                empIds
                            }
                        }
                    }
                }";

            const string Expected = @"
                {
                    submit: {
                        units: [{
                            id: 1,
                            payload: {
                                empIds: [1,2,3]
                            }
                        }]
                    }
                }";

            TestHelpers.TestMutation(query, mutation, _dataContext, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<IEnumerable<Unit>>());

            onWriteFunc.Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Unit>(), Arg.Is<IEnumerable<int?>>(value => Enumerable.SequenceEqual(value, new int?[] { 4, 5, 6 })));
        }

        [Test]
        public void NullableStructEnumerableTestWithKey()
        {
            var batchFunc = Substitute.For<Func<IEnumerable<int>, IDictionary<int, IEnumerable<int?>>>>();
            batchFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(0).ToDictionary(id => id, id => FakeData.People.Where(p => p.UnitId == id).Select(p => (int?)p.Id)));

            var onWriteFunc = Substitute.For<Action<TestUserContext, Unit, IEnumerable<int?>>>();

            void UnitLoaderBuilder(MutableLoader<Unit, int, TestUserContext> loader)
            {
                loader.Field(u => u.Id);
                loader.Field("empIds")
                    .FromBatch(unit => unit.Id, batchFunc)
                    .OnWrite(onWriteFunc)
                    .Editable();
            }

            var query = CreateQueryMutableBuilder(UnitLoaderBuilder);
            var mutation = CreateMutationMutableBuilder(UnitLoaderBuilder);

            const string Query = @"
                mutation {
                    submit(payload: {
                        units: [{
                            id: 1,
                            empIds: [4, 5, 6]
                        }]
                    }) {
                        units {
                            id
                            payload {
                                empIds
                            }
                        }
                    }
                }";

            const string Expected = @"
                {
                    submit: {
                        units: [{
                            id: 1,
                            payload: {
                                empIds: [1,2,3]
                            }
                        }]
                    }
                }";

            TestHelpers.TestMutation(query, mutation, _dataContext, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<IEnumerable<int>>());

            onWriteFunc.Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Unit>(), Arg.Is<IEnumerable<int?>>(value => Enumerable.SequenceEqual(value, new int?[] { 4, 5, 6 })));
        }

#pragma warning disable CA1305 // Specify IFormatProvider
        [Test]
        public void StringEnumerableTestWithContext()
        {
            var batchFunc = Substitute.For<Func<TestUserContext, IEnumerable<Unit>, IDictionary<Unit, IEnumerable<string>>>>();
            batchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Unit>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Unit>>(1).ToDictionary(unit => unit, unit => FakeData.People.Where(p => p.UnitId == unit.Id).Select(p => p.Id.ToString())));

            var onWriteFunc = Substitute.For<Action<TestUserContext, Unit, IEnumerable<string>>>();

            void UnitLoaderBuilder(MutableLoader<Unit, int, TestUserContext> loader)
            {
                loader.Field(u => u.Id);
                loader.Field("empIds")
                    .FromBatch(batchFunc)
                    .OnWrite(onWriteFunc)
                    .Editable();
            }

            var query = CreateQueryMutableBuilder(UnitLoaderBuilder);
            var mutation = CreateMutationMutableBuilder(UnitLoaderBuilder);

            const string Query = @"
                mutation {
                    submit(payload: {
                        units: [{
                            id: 1,
                            empIds: [""4"", ""5"", ""6""]
                        }]
                    }) {
                        units {
                            id
                            payload {
                                empIds
                            }
                        }
                    }
                }";

            const string Expected = @"
                {
                    submit: {
                        units: [{
                            id: 1,
                            payload: {
                                empIds: [""1"", ""2"", ""3""]
                            }
                        }]
                    }
                }";

            TestHelpers.TestMutation(query, mutation, _dataContext, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Unit>>());

            onWriteFunc.Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Unit>(), Arg.Is<IEnumerable<string>>(value => Enumerable.SequenceEqual(value, new[] { "4", "5", "6" })));
        }

        [Test]
        public void StringEnumerableTestWithContextAndKey()
        {
            var batchFunc = Substitute.For<Func<TestUserContext, IEnumerable<int>, IDictionary<int, IEnumerable<string>>>>();
            batchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(1).ToDictionary(id => id, id => FakeData.People.Where(p => p.UnitId == id).Select(p => p.Id.ToString())));

            var onWriteFunc = Substitute.For<Action<TestUserContext, Unit, IEnumerable<string>>>();

            void UnitLoaderBuilder(MutableLoader<Unit, int, TestUserContext> loader)
            {
                loader.Field(u => u.Id);
                loader.Field("empIds")
                    .FromBatch(unit => unit.Id, batchFunc)
                    .OnWrite(onWriteFunc)
                    .Editable();
            }

            var query = CreateQueryMutableBuilder(UnitLoaderBuilder);
            var mutation = CreateMutationMutableBuilder(UnitLoaderBuilder);

            const string Query = @"
                mutation {
                    submit(payload: {
                        units: [{
                            id: 1,
                            empIds: [""4"", ""5"", ""6""]
                        }]
                    }) {
                        units {
                            id
                            payload {
                                empIds
                            }
                        }
                    }
                }";

            const string Expected = @"
                {
                    submit: {
                        units: [{
                            id: 1,
                            payload: {
                                empIds: [""1"", ""2"", ""3""]
                            }
                        }]
                    }
                }";

            TestHelpers.TestMutation(query, mutation, _dataContext, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>());

            onWriteFunc.Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Unit>(), Arg.Is<IEnumerable<string>>(value => Enumerable.SequenceEqual(value, new[] { "4", "5", "6" })));
        }

        [Test]
        public void StringEnumerableTest()
        {
            var batchFunc = Substitute.For<Func<IEnumerable<Unit>, IDictionary<Unit, IEnumerable<string>>>>();
            batchFunc
                .Invoke(Arg.Any<IEnumerable<Unit>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Unit>>(0).ToDictionary(unit => unit, unit => FakeData.People.Where(p => p.UnitId == unit.Id).Select(p => p.Id.ToString())));

            var onWriteFunc = Substitute.For<Action<TestUserContext, Unit, IEnumerable<string>>>();

            void UnitLoaderBuilder(MutableLoader<Unit, int, TestUserContext> loader)
            {
                loader.Field(u => u.Id);
                loader.Field("empIds")
                    .FromBatch(batchFunc)
                    .OnWrite(onWriteFunc)
                    .Editable();
            }

            var query = CreateQueryMutableBuilder(UnitLoaderBuilder);
            var mutation = CreateMutationMutableBuilder(UnitLoaderBuilder);

            const string Query = @"
                mutation {
                    submit(payload: {
                        units: [{
                            id: 1,
                            empIds: [""4"", ""5"", ""6""]
                        }]
                    }) {
                        units {
                            id
                            payload {
                                empIds
                            }
                        }
                    }
                }";

            const string Expected = @"
                {
                    submit: {
                        units: [{
                            id: 1,
                            payload: {
                                empIds: [""1"", ""2"", ""3""]
                            }
                        }]
                    }
                }";

            TestHelpers.TestMutation(query, mutation, _dataContext, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<IEnumerable<Unit>>());

            onWriteFunc.Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Unit>(), Arg.Is<IEnumerable<string>>(value => Enumerable.SequenceEqual(value, new[] { "4", "5", "6" })));
        }

        [Test]
        public void StringEnumerableTestWithKey()
        {
            var batchFunc = Substitute.For<Func<IEnumerable<int>, IDictionary<int, IEnumerable<string>>>>();
            batchFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(0).ToDictionary(id => id, id => FakeData.People.Where(p => p.UnitId == id).Select(p => p.Id.ToString())));

            var onWriteFunc = Substitute.For<Action<TestUserContext, Unit, IEnumerable<string>>>();

            void UnitLoaderBuilder(MutableLoader<Unit, int, TestUserContext> loader)
            {
                loader.Field(u => u.Id);
                loader.Field("empIds")
                    .FromBatch(unit => unit.Id, batchFunc)
                    .OnWrite(onWriteFunc)
                    .Editable();
            }

            var query = CreateQueryMutableBuilder(UnitLoaderBuilder);
            var mutation = CreateMutationMutableBuilder(UnitLoaderBuilder);

            const string Query = @"
                mutation {
                    submit(payload: {
                        units: [{
                            id: 1,
                            empIds: [""4"", ""5"", ""6""]
                        }]
                    }) {
                        units {
                            id
                            payload {
                                empIds
                            }
                        }
                    }
                }";

            const string Expected = @"
                {
                    submit: {
                        units: [{
                            id: 1,
                            payload: {
                                empIds: [""1"", ""2"", ""3""]
                            }
                        }]
                    }
                }";

            TestHelpers.TestMutation(query, mutation, _dataContext, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<IEnumerable<int>>());

            onWriteFunc.Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Unit>(), Arg.Is<IEnumerable<string>>(value => Enumerable.SequenceEqual(value, new[] { "4", "5", "6" })));
        }
#pragma warning restore CA1305 // Specify IFormatProvider

        [Test]
        public void ClassEnumerableTestWithContext()
        {
            var batchFunc = Substitute.For<Func<TestUserContext, IEnumerable<Unit>, IDictionary<Unit, IEnumerable<Person>>>>();
            batchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Unit>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Unit>>(1).ToDictionary(unit => unit, unit => FakeData.People.Where(p => p.UnitId == unit.Id)));

            var onWriteFunc = Substitute.For<Action<TestUserContext, Unit, IEnumerable<Person>>>();

            void UnitLoaderBuilder(MutableLoader<Unit, int, TestUserContext> loader)
            {
                loader.Field(u => u.Id);
                loader.Field("empIds")
                    .FromBatch(batchFunc)
                    .OnWrite(onWriteFunc)
                    .Editable();
            }

            var query = CreateQueryMutableBuilder(UnitLoaderBuilder);
            var mutation = CreateMutationMutableBuilder(UnitLoaderBuilder);

            const string Query = @"
                mutation {
                    submit(payload: {
                        units: [{
                            id: 1,
                            empIds: [{
                                id: 4
                            }, {
                                id: 5
                            }, {
                                id: 6
                            }]
                        }]
                    }) {
                        units {
                            id
                            payload {
                                empIds {
                                    id
                                }
                            }
                        }
                    }
                }";

            const string Expected = @"
                {
                    submit: {
                        units: [{
                            id: 1,
                            payload: {
                                empIds: [{
                                    id: 1
                                }, {
                                    id: 2
                                }, {
                                    id: 3
                                }]
                            }
                        }]
                    }
                }";

            TestHelpers.TestMutation(query, mutation, _dataContext, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Unit>>());

            onWriteFunc.Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Unit>(), Arg.Is<IEnumerable<Person>>(value => Enumerable.SequenceEqual(value.Select(v => v.Id), new[] { 4, 5, 6 })));
        }

        [Test]
        public void ClassEnumerableTestWithContextAndKey()
        {
            var batchFunc = Substitute.For<Func<TestUserContext, IEnumerable<int>, IDictionary<int, IEnumerable<Person>>>>();
            batchFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(1).ToDictionary(id => id, id => FakeData.People.Where(p => p.UnitId == id)));

            var onWriteFunc = Substitute.For<Action<TestUserContext, Unit, IEnumerable<Person>>>();

            void UnitLoaderBuilder(MutableLoader<Unit, int, TestUserContext> loader)
            {
                loader.Field(u => u.Id);
                loader.Field("empIds")
                    .FromBatch(unit => unit.Id, batchFunc)
                    .OnWrite(onWriteFunc)
                    .Editable();
            }

            var query = CreateQueryMutableBuilder(UnitLoaderBuilder);
            var mutation = CreateMutationMutableBuilder(UnitLoaderBuilder);

            const string Query = @"
                mutation {
                    submit(payload: {
                        units: [{
                            id: 1,
                            empIds: [{
                                id: 4
                            }, {
                                id: 5
                            }, {
                                id: 6
                            }]
                        }]
                    }) {
                        units {
                            id
                            payload {
                                empIds {
                                    id
                                }
                            }
                        }
                    }
                }";

            const string Expected = @"
                {
                    submit: {
                        units: [{
                            id: 1,
                            payload: {
                                empIds: [{
                                    id: 1
                                }, {
                                    id: 2
                                }, {
                                    id: 3
                                }]
                            }
                        }]
                    }
                }";

            TestHelpers.TestMutation(query, mutation, _dataContext, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>());

            onWriteFunc.Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Unit>(), Arg.Is<IEnumerable<Person>>(value => Enumerable.SequenceEqual(value.Select(v => v.Id), new[] { 4, 5, 6 })));
        }

        [Test]
        public void ClassEnumerableTest()
        {
            var batchFunc = Substitute.For<Func<IEnumerable<Unit>, IDictionary<Unit, IEnumerable<Person>>>>();
            batchFunc
                .Invoke(Arg.Any<IEnumerable<Unit>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Unit>>(0).ToDictionary(unit => unit, unit => FakeData.People.Where(p => p.UnitId == unit.Id)));

            var onWriteFunc = Substitute.For<Action<TestUserContext, Unit, IEnumerable<Person>>>();

            void UnitLoaderBuilder(MutableLoader<Unit, int, TestUserContext> loader)
            {
                loader.Field(u => u.Id);
                loader.Field("empIds")
                    .FromBatch(batchFunc)
                    .OnWrite(onWriteFunc)
                    .Editable();
            }

            var query = CreateQueryMutableBuilder(UnitLoaderBuilder);
            var mutation = CreateMutationMutableBuilder(UnitLoaderBuilder);

            const string Query = @"
                mutation {
                    submit(payload: {
                        units: [{
                            id: 1,
                            empIds: [{
                                id: 4
                            }, {
                                id: 5
                            }, {
                                id: 6
                            }]
                        }]
                    }) {
                        units {
                            id
                            payload {
                                empIds {
                                    id
                                }
                            }
                        }
                    }
                }";

            const string Expected = @"
                {
                    submit: {
                        units: [{
                            id: 1,
                            payload: {
                                empIds: [{
                                    id: 1
                                }, {
                                    id: 2
                                }, {
                                    id: 3
                                }]
                            }
                        }]
                    }
                }";

            TestHelpers.TestMutation(query, mutation, _dataContext, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<IEnumerable<Unit>>());

            onWriteFunc.Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Unit>(), Arg.Is<IEnumerable<Person>>(value => Enumerable.SequenceEqual(value.Select(v => v.Id), new[] { 4, 5, 6 })));
        }

        [Test]
        public void ClassEnumerableTestWithKey()
        {
            var batchFunc = Substitute.For<Func<IEnumerable<int>, IDictionary<int, IEnumerable<Person>>>>();
            batchFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(0).ToDictionary(id => id, id => FakeData.People.Where(p => p.UnitId == id)));

            var onWriteFunc = Substitute.For<Action<TestUserContext, Unit, IEnumerable<Person>>>();

            void UnitLoaderBuilder(MutableLoader<Unit, int, TestUserContext> loader)
            {
                loader.Field(u => u.Id);
                loader.Field("empIds")
                    .FromBatch(unit => unit.Id, batchFunc)
                    .OnWrite(onWriteFunc)
                    .Editable();
            }

            var query = CreateQueryMutableBuilder(UnitLoaderBuilder);
            var mutation = CreateMutationMutableBuilder(UnitLoaderBuilder);

            const string Query = @"
                mutation {
                    submit(payload: {
                        units: [{
                            id: 1,
                            empIds: [{
                                id: 4
                            }, {
                                id: 5
                            }, {
                                id: 6
                            }]
                        }]
                    }) {
                        units {
                            id
                            payload {
                                empIds {
                                    id
                                }
                            }
                        }
                    }
                }";

            const string Expected = @"
                {
                    submit: {
                        units: [{
                            id: 1,
                            payload: {
                                empIds: [{
                                    id: 1
                                }, {
                                    id: 2
                                }, {
                                    id: 3
                                }]
                            }
                        }]
                    }
                }";

            TestHelpers.TestMutation(query, mutation, _dataContext, Query, Expected);

            batchFunc.Received(2)
                .Invoke(Arg.Any<IEnumerable<int>>());

            onWriteFunc.Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Unit>(), Arg.Is<IEnumerable<Person>>(value => Enumerable.SequenceEqual(value.Select(v => v.Id), new[] { 4, 5, 6 })));
        }

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            _dataContext = Substitute.For<IDataContext>();

            _dataContext.Convert(Arg.Any<IQueryable<Proxy<Person>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<Proxy<Person>>>(0).ToAsyncEnumerable());
            _dataContext.Convert(Arg.Any<IQueryable<Proxy<Unit>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<Proxy<Unit>>>(0).ToAsyncEnumerable());
            _dataContext.Convert(Arg.Any<IQueryable<Proxy<Branch>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<Proxy<Branch>>>(0).ToAsyncEnumerable());
            _dataContext.Convert(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Person>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Person>>>(0).ToAsyncEnumerable());
            _dataContext.Convert(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Unit>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Unit>>>(0).ToAsyncEnumerable());
            _dataContext.Convert(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Branch>>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Branch>>>>(0).ToAsyncEnumerable());
            _dataContext.Convert(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Person>>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Person>>>>(0).ToAsyncEnumerable());
            _dataContext.Convert(Arg.Any<IQueryable<QueryableExtensions.KeyValue<long, Proxy<Person>>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<long, Proxy<Person>>>>(0).ToAsyncEnumerable());
            _dataContext.Convert(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Unit>>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Unit>>>>(0).ToAsyncEnumerable());
            _dataContext.Convert(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, int>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, int>>>(0).ToAsyncEnumerable());

            _dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<Proxy<Person>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<Proxy<Person>>>(0));
            _dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<Proxy<Unit>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<Proxy<Unit>>>(0));
            _dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<Proxy<Branch>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<Proxy<Branch>>>(0));
            _dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Person>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Person>>>(0));
            _dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Unit>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Unit>>>(0));
            _dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Branch>>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Branch>>>>(0));
            _dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Person>>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Person>>>>(0));
            _dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<long, Proxy<Person>>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<long, Proxy<Person>>>>(0));
            _dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Unit>>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Unit>>>>(0));
            _dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, int>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, int>>>(0));

            _dataContext.ExecuteInTransactionAsync(Arg.Any<Func<Task>>())
                .Returns(callInfo => callInfo.ArgAt<Func<Task>>(0)());

            _dataContext
                .When(dataContext => dataContext.AddRange(Arg.Any<IEnumerable<Person>>()))
                .Do(callInfo =>
                {
                    var range = callInfo.ArgAt<IEnumerable<Person>>(0);
                    foreach (var p in range)
                    {
                        FakeData.AddPerson(p);
                    }
                });

            _dataContext
                .When(dataContext => dataContext.AddRange(Arg.Any<IEnumerable<Unit>>()))
                .Do(callInfo =>
                {
                    var range = callInfo.ArgAt<IEnumerable<Unit>>(0);
                    foreach (var p in range)
                    {
                        FakeData.AddUnit(p);
                    }
                });

            _dataContext
                .When(dataContext => dataContext.SaveChangesAsync())
                .Do(callInfo =>
                {
                    FakeData.UpdateRelations();
                });

            _dataContext.GetQueryable<Person>()
                .Returns(callInfo =>
                {
                    return FakeData.People.AsQueryable();
                });

            _dataContext.GetQueryable<Unit>()
                .Returns(callInfo =>
                {
                    return FakeData.Units.AsQueryable();
                });
        }

        private Action<Query<TestUserContext>> CreateQueryMutableBuilder(Action<MutableLoader<Unit, int, TestUserContext>> userLoaderBuilder, Action<Query<TestUserContext>> configure = null)
        {
            var unitLoader = GraphQLTypeBuilder.CreateMutableLoaderType(
                onConfigure: userLoaderBuilder,
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            return query =>
            {
                query
                    .Connection(unitLoader, "units");
                configure?.Invoke(query);
            };
        }

        private Action<Mutation<TestUserContext>> CreateMutationMutableBuilder(Action<MutableLoader<Unit, int, TestUserContext>> userLoaderBuilder)
        {
            var unitLoader = GraphQLTypeBuilder.CreateMutableLoaderType(
                onConfigure: userLoaderBuilder,
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            return query =>
            {
                query
                    .SubmitField(unitLoader, "units");
            };
        }
    }
}
