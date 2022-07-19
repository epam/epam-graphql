// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epam.Contracts.Models;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NSubstitute;
using NUnit.Framework;

namespace Epam.GraphQL.Tests
{
    [TestFixture]
    public class FromBatchesTests : BaseTests
    {
        [Test]
        public void BatchUnionFuncsFromContextAndPersonToUnitFromContextAndPersonToPerson()
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
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(
                    p => p,
                    p => FakeData.People
                        .AsQueryable()
                        .Where(m => m.Id == p.ManagerId)
                        .FirstOrDefault()));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.FullName);
                    loader.Field("managerOrDepartment")
                        .FromBatch(batchUnitFunc)
                        .AndFromBatch(batchManagerFunc, build => build.ConfigureFrom(loader.GetType()));
                });

            const string Query = @"
                    query {
                        people {
                            items {
                                id
                                managerOrDepartment {
                                    ... on Unit {
                                        id
                                        name
                                    }
                                    ... on Person {
                                        id
                                        fullName
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
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                }]
                            },{
                                id: 2,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 3,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 4,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 5,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 6,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 5,
                                    fullName: ""Aldon Exley""
                                }]
                            }]
                        }
                    }";

            TestHelpers.TestQuery(builder, Query, Expected);

            batchUnitFunc.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>());
            batchManagerFunc.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>());
        }

        [Test]
        public void BatchUnionTaskFuncsFromContextAndPersonToUnitFromContextAndPersonToPerson()
        {
            var batchUnitFunc = Substitute.For<Func<TestUserContext, IEnumerable<Person>, Task<IDictionary<Person, Unit>>>>();
            batchUnitFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(
                    p => p,
                    p => FakeData.Units
                        .AsQueryable()
                        .Where(u => u.Id == p.UnitId)
                        .FirstOrDefault()));

            var batchManagerFunc = Substitute.For<Func<TestUserContext, IEnumerable<Person>, Task<IDictionary<Person, Person>>>>();
            batchManagerFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(
                    p => p,
                    p => FakeData.People
                        .AsQueryable()
                        .Where(m => m.Id == p.ManagerId)
                        .FirstOrDefault()));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.FullName);
                    loader.Field("managerOrDepartment")
                        .FromBatch(batchUnitFunc)
                        .AndFromBatch(batchManagerFunc, build => build.ConfigureFrom(loader.GetType()));
                });

            const string Query = @"
                    query {
                        people {
                            items {
                                id
                                managerOrDepartment {
                                    ... on Unit {
                                        id
                                        name
                                    }
                                    ... on Person {
                                        id
                                        fullName
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
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                }]
                            },{
                                id: 2,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 3,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 4,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 5,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 6,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 5,
                                    fullName: ""Aldon Exley""
                                }]
                            }]
                        }
                    }";

            TestHelpers.TestQuery(builder, Query, Expected);

            batchUnitFunc.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>());
            batchManagerFunc.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>());
        }

        [Test]
        public void BatchUnionMutableLoaderFuncsFromContextAndPersonToUnitFromContextAndPersonToPerson()
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
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(
                    p => p,
                    p => FakeData.People
                        .AsQueryable()
                        .Where(m => m.Id == p.ManagerId)
                        .FirstOrDefault()));

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.FullName);
                    loader.Field("managerOrDepartment")
                        .FromBatch(batchUnitFunc)
                        .AndFromBatch(batchManagerFunc, build => build.ConfigureFrom(loader.GetType()));
                });

            const string Query = @"
                    query {
                        people {
                            items {
                                id
                                managerOrDepartment {
                                    ... on Unit {
                                        id
                                        name
                                    }
                                    ... on Person {
                                        id
                                        fullName
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
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                }]
                            },{
                                id: 2,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 3,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 4,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 5,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 6,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 5,
                                    fullName: ""Aldon Exley""
                                }]
                            }]
                        }
                    }";

            TestHelpers.TestQuery(mutableBuilder, Query, Expected);

            batchUnitFunc.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>());
            batchManagerFunc.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>());
        }

        [Test]
        public void BatchUnionFuncsFromContextAndPersonToUnitFromContextAndPersonToPersonAndSelectString()
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
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(
                    p => p,
                    p => FakeData.People
                        .AsQueryable()
                        .Where(m => m.Id == p.ManagerId)
                        .FirstOrDefault()));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.FullName);
                    loader.Field("managerOrDepartmentIds")
                        .FromBatch(batchUnitFunc)
                        .AndFromBatch(batchManagerFunc)
                        .Select(x => x.Cast<IHasId<int>>().Select(o => o.Id));
                });

            const string Query = @"
                    query {
                        people {
                            items {
                                id
                                managerOrDepartmentIds
                            }
                        }
                    }";

            const string Expected = @"
                    {
                        people: {
                            items: [{
                                id: 1,
                                managerOrDepartmentIds: [1]
                            },{
                                id: 2,
                                managerOrDepartmentIds: [1, 1]
                            },{
                                id: 3,
                                managerOrDepartmentIds: [1, 1]
                            },{
                                id: 4,
                                managerOrDepartmentIds: [2, 2]
                            },{
                                id: 5,
                                managerOrDepartmentIds: [2, 2]
                            },{
                                id: 6,
                                managerOrDepartmentIds: [2, 5]
                            }]
                        }
                    }";

            TestHelpers.TestQuery(builder, Query, Expected);

            batchUnitFunc.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>());
            batchManagerFunc.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>());
        }

        [Test]
        public void BatchUnionTaskFuncsFromContextAndPersonToUnitFromContextAndPersonToPersonAndSelectString()
        {
            var batchUnitFunc = Substitute.For<Func<TestUserContext, IEnumerable<Person>, Task<IDictionary<Person, Unit>>>>();
            batchUnitFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(
                    p => p,
                    p => FakeData.Units
                        .AsQueryable()
                        .Where(u => u.Id == p.UnitId)
                        .FirstOrDefault()));

            var batchManagerFunc = Substitute.For<Func<TestUserContext, IEnumerable<Person>, Task<IDictionary<Person, Person>>>>();
            batchManagerFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(
                    p => p,
                    p => FakeData.People
                        .AsQueryable()
                        .Where(m => m.Id == p.ManagerId)
                        .FirstOrDefault()));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.FullName);
                    loader.Field("managerOrDepartmentIds")
                        .FromBatch(batchUnitFunc)
                        .AndFromBatch(batchManagerFunc)
                        .Select(x => x.Cast<IHasId<int>>().Select(o => o.Id));
                });

            const string Query = @"
                    query {
                        people {
                            items {
                                id
                                managerOrDepartmentIds
                            }
                        }
                    }";

            const string Expected = @"
                    {
                        people: {
                            items: [{
                                id: 1,
                                managerOrDepartmentIds: [1]
                            },{
                                id: 2,
                                managerOrDepartmentIds: [1, 1]
                            },{
                                id: 3,
                                managerOrDepartmentIds: [1, 1]
                            },{
                                id: 4,
                                managerOrDepartmentIds: [2, 2]
                            },{
                                id: 5,
                                managerOrDepartmentIds: [2, 2]
                            },{
                                id: 6,
                                managerOrDepartmentIds: [2, 5]
                            }]
                        }
                    }";

            TestHelpers.TestQuery(builder, Query, Expected);

            batchUnitFunc.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>());
            batchManagerFunc.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>());
        }

        [Test]
        public void BatchUnionMutableLoaderFuncsFromContextAndPersonToUnitFromContextAndPersonToPersonAndSelectString()
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
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(
                    p => p,
                    p => FakeData.People
                        .AsQueryable()
                        .Where(m => m.Id == p.ManagerId)
                        .FirstOrDefault()));

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.FullName);
                    loader.Field("managerOrDepartmentIds")
                        .FromBatch(batchUnitFunc)
                        .AndFromBatch(batchManagerFunc)
                        .Select(x => x.Cast<IHasId<int>>().Select(o => o.Id));
                });

            const string Query = @"
                    query {
                        people {
                            items {
                                id
                                managerOrDepartmentIds
                            }
                        }
                    }";

            const string Expected = @"
                    {
                        people: {
                            items: [{
                                id: 1,
                                managerOrDepartmentIds: [1]
                            },{
                                id: 2,
                                managerOrDepartmentIds: [1, 1]
                            },{
                                id: 3,
                                managerOrDepartmentIds: [1, 1]
                            },{
                                id: 4,
                                managerOrDepartmentIds: [2, 2]
                            },{
                                id: 5,
                                managerOrDepartmentIds: [2, 2]
                            },{
                                id: 6,
                                managerOrDepartmentIds: [2, 5]
                            }]
                        }
                    }";

            TestHelpers.TestQuery(mutableBuilder, Query, Expected);

            batchUnitFunc.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>());
            batchManagerFunc.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>());
        }

        [Test]
        public void BatchUnionFuncsFromPersonToUnitFromPersonToPerson()
        {
            var batchUnitFunc = Substitute.For<Func<IEnumerable<Person>, IDictionary<Person, Unit>>>();
            batchUnitFunc
                .Invoke(Arg.Any<IEnumerable<Person>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(0).ToDictionary(
                    p => p,
                    p => FakeData.Units
                        .AsQueryable()
                        .Where(u => u.Id == p.UnitId)
                        .FirstOrDefault()));

            var batchManagerFunc = Substitute.For<Func<IEnumerable<Person>, IDictionary<Person, Person>>>();
            batchManagerFunc
                .Invoke(Arg.Any<IEnumerable<Person>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(0).ToDictionary(
                    p => p,
                    p => FakeData.People
                        .AsQueryable()
                        .Where(m => m.Id == p.ManagerId)
                        .FirstOrDefault()));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.FullName);
                    loader.Field("managerOrDepartment")
                        .FromBatch(batchUnitFunc)
                        .AndFromBatch(batchManagerFunc, build => build.ConfigureFrom(loader.GetType()));
                });

            const string Query = @"
                    query {
                        people {
                            items {
                                id
                                managerOrDepartment {
                                    ... on Unit {
                                        id
                                        name
                                    }
                                    ... on Person {
                                        id
                                        fullName
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
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                }]
                            },{
                                id: 2,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 3,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 4,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 5,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 6,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 5,
                                    fullName: ""Aldon Exley""
                                }]
                            }]
                        }
                    }";

            TestHelpers.TestQuery(builder, Query, Expected);

            batchUnitFunc.Received(1).Invoke(Arg.Any<IEnumerable<Person>>());
            batchManagerFunc.Received(1).Invoke(Arg.Any<IEnumerable<Person>>());
        }

        [Test]
        public void BatchUnionTaskFuncsFromPersonToUnitFromPersonToPerson()
        {
            var batchUnitFunc = Substitute.For<Func<IEnumerable<Person>, Task<IDictionary<Person, Unit>>>>();
            batchUnitFunc
                .Invoke(Arg.Any<IEnumerable<Person>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(0).ToDictionary(
                    p => p,
                    p => FakeData.Units
                        .AsQueryable()
                        .Where(u => u.Id == p.UnitId)
                        .FirstOrDefault()));

            var batchManagerFunc = Substitute.For<Func<IEnumerable<Person>, Task<IDictionary<Person, Person>>>>();
            batchManagerFunc
                .Invoke(Arg.Any<IEnumerable<Person>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(0).ToDictionary(
                    p => p,
                    p => FakeData.People
                        .AsQueryable()
                        .Where(m => m.Id == p.ManagerId)
                        .FirstOrDefault()));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.FullName);
                    loader.Field("managerOrDepartment")
                        .FromBatch(batchUnitFunc)
                        .AndFromBatch(batchManagerFunc, build => build.ConfigureFrom(loader.GetType()));
                });

            const string Query = @"
                    query {
                        people {
                            items {
                                id
                                managerOrDepartment {
                                    ... on Unit {
                                        id
                                        name
                                    }
                                    ... on Person {
                                        id
                                        fullName
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
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                }]
                            },{
                                id: 2,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 3,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 4,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 5,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 6,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 5,
                                    fullName: ""Aldon Exley""
                                }]
                            }]
                        }
                    }";

            TestHelpers.TestQuery(builder, Query, Expected);

            batchUnitFunc.Received(1).Invoke(Arg.Any<IEnumerable<Person>>());
            batchManagerFunc.Received(1).Invoke(Arg.Any<IEnumerable<Person>>());
        }

        [Test]
        public void BatchUnionMutableLoaderFuncsFromPersonToUnitFromPersonToPerson()
        {
            var batchUnitFunc = Substitute.For<Func<IEnumerable<Person>, IDictionary<Person, Unit>>>();
            batchUnitFunc
                .Invoke(Arg.Any<IEnumerable<Person>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(0).ToDictionary(
                    p => p,
                    p => FakeData.Units
                        .AsQueryable()
                        .Where(u => u.Id == p.UnitId)
                        .FirstOrDefault()));

            var batchManagerFunc = Substitute.For<Func<IEnumerable<Person>, IDictionary<Person, Person>>>();
            batchManagerFunc
                .Invoke(Arg.Any<IEnumerable<Person>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(0).ToDictionary(
                    p => p,
                    p => FakeData.People
                        .AsQueryable()
                        .Where(m => m.Id == p.ManagerId)
                        .FirstOrDefault()));

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.FullName);
                    loader.Field("managerOrDepartment")
                        .FromBatch(batchUnitFunc)
                        .AndFromBatch(batchManagerFunc, build => build.ConfigureFrom(loader.GetType()));
                });

            const string Query = @"
                    query {
                        people {
                            items {
                                id
                                managerOrDepartment {
                                    ... on Unit {
                                        id
                                        name
                                    }
                                    ... on Person {
                                        id
                                        fullName
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
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                }]
                            },{
                                id: 2,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 3,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 4,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 5,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 6,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 5,
                                    fullName: ""Aldon Exley""
                                }]
                            }]
                        }
                    }";

            TestHelpers.TestQuery(mutableBuilder, Query, Expected);

            batchUnitFunc.Received(1).Invoke(Arg.Any<IEnumerable<Person>>());
            batchManagerFunc.Received(1).Invoke(Arg.Any<IEnumerable<Person>>());
        }

        [Test]
        public void BatchUnionFuncsFromPersonToUnitFromPersonToPersonAndSelectString()
        {
            var batchUnitFunc = Substitute.For<Func<IEnumerable<Person>, IDictionary<Person, Unit>>>();
            batchUnitFunc
                .Invoke(Arg.Any<IEnumerable<Person>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(0).ToDictionary(
                    p => p,
                    p => FakeData.Units
                        .AsQueryable()
                        .Where(u => u.Id == p.UnitId)
                        .FirstOrDefault()));

            var batchManagerFunc = Substitute.For<Func<IEnumerable<Person>, IDictionary<Person, Person>>>();
            batchManagerFunc
                .Invoke(Arg.Any<IEnumerable<Person>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(0).ToDictionary(
                    p => p,
                    p => FakeData.People
                        .AsQueryable()
                        .Where(m => m.Id == p.ManagerId)
                        .FirstOrDefault()));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.FullName);
                    loader.Field("managerOrDepartmentIds")
                        .FromBatch(batchUnitFunc)
                        .AndFromBatch(batchManagerFunc)
                        .Select(x => x.Cast<IHasId<int>>().Select(o => o.Id));
                });

            const string Query = @"
                    query {
                        people {
                            items {
                                id
                                managerOrDepartmentIds
                            }
                        }
                    }";

            const string Expected = @"
                    {
                        people: {
                            items: [{
                                id: 1,
                                managerOrDepartmentIds: [1]
                            },{
                                id: 2,
                                managerOrDepartmentIds: [1, 1]
                            },{
                                id: 3,
                                managerOrDepartmentIds: [1, 1]
                            },{
                                id: 4,
                                managerOrDepartmentIds: [2, 2]
                            },{
                                id: 5,
                                managerOrDepartmentIds: [2, 2]
                            },{
                                id: 6,
                                managerOrDepartmentIds: [2, 5]
                            }]
                        }
                    }";

            TestHelpers.TestQuery(builder, Query, Expected);

            batchUnitFunc.Received(1).Invoke(Arg.Any<IEnumerable<Person>>());
            batchManagerFunc.Received(1).Invoke(Arg.Any<IEnumerable<Person>>());
        }

        [Test]
        public void BatchUnionTaskFuncsFromPersonToUnitFromPersonToPersonAndSelectString()
        {
            var batchUnitFunc = Substitute.For<Func<IEnumerable<Person>, Task<IDictionary<Person, Unit>>>>();
            batchUnitFunc
                .Invoke(Arg.Any<IEnumerable<Person>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(0).ToDictionary(
                    p => p,
                    p => FakeData.Units
                        .AsQueryable()
                        .Where(u => u.Id == p.UnitId)
                        .FirstOrDefault()));

            var batchManagerFunc = Substitute.For<Func<IEnumerable<Person>, Task<IDictionary<Person, Person>>>>();
            batchManagerFunc
                .Invoke(Arg.Any<IEnumerable<Person>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(0).ToDictionary(
                    p => p,
                    p => FakeData.People
                        .AsQueryable()
                        .Where(m => m.Id == p.ManagerId)
                        .FirstOrDefault()));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.FullName);
                    loader.Field("managerOrDepartmentIds")
                        .FromBatch(batchUnitFunc)
                        .AndFromBatch(batchManagerFunc)
                        .Select(x => x.Cast<IHasId<int>>().Select(o => o.Id));
                });

            const string Query = @"
                    query {
                        people {
                            items {
                                id
                                managerOrDepartmentIds
                            }
                        }
                    }";

            const string Expected = @"
                    {
                        people: {
                            items: [{
                                id: 1,
                                managerOrDepartmentIds: [1]
                            },{
                                id: 2,
                                managerOrDepartmentIds: [1, 1]
                            },{
                                id: 3,
                                managerOrDepartmentIds: [1, 1]
                            },{
                                id: 4,
                                managerOrDepartmentIds: [2, 2]
                            },{
                                id: 5,
                                managerOrDepartmentIds: [2, 2]
                            },{
                                id: 6,
                                managerOrDepartmentIds: [2, 5]
                            }]
                        }
                    }";

            TestHelpers.TestQuery(builder, Query, Expected);

            batchUnitFunc.Received(1).Invoke(Arg.Any<IEnumerable<Person>>());
            batchManagerFunc.Received(1).Invoke(Arg.Any<IEnumerable<Person>>());
        }

        [Test]
        public void BatchUnionMutableLoaderFuncsFromPersonToUnitFromPersonToPersonAndSelectString()
        {
            var batchUnitFunc = Substitute.For<Func<IEnumerable<Person>, IDictionary<Person, Unit>>>();
            batchUnitFunc
                .Invoke(Arg.Any<IEnumerable<Person>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(0).ToDictionary(
                    p => p,
                    p => FakeData.Units
                        .AsQueryable()
                        .Where(u => u.Id == p.UnitId)
                        .FirstOrDefault()));

            var batchManagerFunc = Substitute.For<Func<IEnumerable<Person>, IDictionary<Person, Person>>>();
            batchManagerFunc
                .Invoke(Arg.Any<IEnumerable<Person>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(0).ToDictionary(
                    p => p,
                    p => FakeData.People
                        .AsQueryable()
                        .Where(m => m.Id == p.ManagerId)
                        .FirstOrDefault()));

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.FullName);
                    loader.Field("managerOrDepartmentIds")
                        .FromBatch(batchUnitFunc)
                        .AndFromBatch(batchManagerFunc)
                        .Select(x => x.Cast<IHasId<int>>().Select(o => o.Id));
                });

            const string Query = @"
                    query {
                        people {
                            items {
                                id
                                managerOrDepartmentIds
                            }
                        }
                    }";

            const string Expected = @"
                    {
                        people: {
                            items: [{
                                id: 1,
                                managerOrDepartmentIds: [1]
                            },{
                                id: 2,
                                managerOrDepartmentIds: [1, 1]
                            },{
                                id: 3,
                                managerOrDepartmentIds: [1, 1]
                            },{
                                id: 4,
                                managerOrDepartmentIds: [2, 2]
                            },{
                                id: 5,
                                managerOrDepartmentIds: [2, 2]
                            },{
                                id: 6,
                                managerOrDepartmentIds: [2, 5]
                            }]
                        }
                    }";

            TestHelpers.TestQuery(mutableBuilder, Query, Expected);

            batchUnitFunc.Received(1).Invoke(Arg.Any<IEnumerable<Person>>());
            batchManagerFunc.Received(1).Invoke(Arg.Any<IEnumerable<Person>>());
        }

        [Test]
        public void BatchUnionFuncsFromContextAndPersonToUnitFromContextAndPersonToPersonUsingKeySelector()
        {
            var batchUnitFunc = Substitute.For<Func<TestUserContext, IEnumerable<int>, IDictionary<int, Unit>>>();
            batchUnitFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(1).ToDictionary(
                    id => id,
                    id => FakeData.People
                        .AsQueryable()
                        .Where(p => p.Id == id)
                        .Select(p => p.Unit)
                        .FirstOrDefault()));

            var batchManagerFunc = Substitute.For<Func<TestUserContext, IEnumerable<int>, IDictionary<int, Person>>>();
            batchManagerFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(1).ToDictionary(
                    id => id,
                    id => FakeData.People
                        .AsQueryable()
                        .Where(p => p.Id == id)
                        .Select(p => p.Manager)
                        .FirstOrDefault()));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.FullName);
                    loader.Field("managerOrDepartment")
                        .FromBatch(p => p.Id, batchUnitFunc)
                        .AndFromBatch(p => p.Id, batchManagerFunc, build => build.ConfigureFrom(loader.GetType()));
                });

            const string Query = @"
                    query {
                        people {
                            items {
                                id
                                managerOrDepartment {
                                    ... on Unit {
                                        id
                                        name
                                    }
                                    ... on Person {
                                        id
                                        fullName
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
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                }]
                            },{
                                id: 2,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 3,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 4,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 5,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 6,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 5,
                                    fullName: ""Aldon Exley""
                                }]
                            }]
                        }
                    }";

            TestHelpers.TestQuery(builder, Query, Expected);

            batchUnitFunc.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>());
            batchManagerFunc.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>());
        }

        [Test]
        public void BatchUnionTaskFuncsFromContextAndPersonToUnitFromContextAndPersonToPersonUsingKeySelector()
        {
            var batchUnitFunc = Substitute.For<Func<TestUserContext, IEnumerable<int>, Task<IDictionary<int, Unit>>>>();
            batchUnitFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(1).ToDictionary(
                    id => id,
                    id => FakeData.People
                        .AsQueryable()
                        .Where(p => p.Id == id)
                        .Select(p => p.Unit)
                        .FirstOrDefault()));

            var batchManagerFunc = Substitute.For<Func<TestUserContext, IEnumerable<int>, Task<IDictionary<int, Person>>>>();
            batchManagerFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(1).ToDictionary(
                    id => id,
                    id => FakeData.People
                        .AsQueryable()
                        .Where(p => p.Id == id)
                        .Select(p => p.Manager)
                        .FirstOrDefault()));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.FullName);
                    loader.Field("managerOrDepartment")
                        .FromBatch(p => p.Id, batchUnitFunc)
                        .AndFromBatch(p => p.Id, batchManagerFunc, build => build.ConfigureFrom(loader.GetType()));
                });

            const string Query = @"
                    query {
                        people {
                            items {
                                id
                                managerOrDepartment {
                                    ... on Unit {
                                        id
                                        name
                                    }
                                    ... on Person {
                                        id
                                        fullName
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
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                }]
                            },{
                                id: 2,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 3,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 4,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 5,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 6,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 5,
                                    fullName: ""Aldon Exley""
                                }]
                            }]
                        }
                    }";

            TestHelpers.TestQuery(builder, Query, Expected);

            batchUnitFunc.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>());
            batchManagerFunc.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>());
        }

        [Test]
        public void BatchUnionMutableLoaderFuncsFromContextAndPersonToUnitFromContextAndPersonToPersonUsingKeySelector()
        {
            var batchUnitFunc = Substitute.For<Func<TestUserContext, IEnumerable<int>, IDictionary<int, Unit>>>();
            batchUnitFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(1).ToDictionary(
                    id => id,
                    id => FakeData.People
                        .AsQueryable()
                        .Where(p => p.Id == id)
                        .Select(p => p.Unit)
                        .FirstOrDefault()));

            var batchManagerFunc = Substitute.For<Func<TestUserContext, IEnumerable<int>, IDictionary<int, Person>>>();
            batchManagerFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(1).ToDictionary(
                    id => id,
                    id => FakeData.People
                        .AsQueryable()
                        .Where(p => p.Id == id)
                        .Select(p => p.Manager)
                        .FirstOrDefault()));

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.FullName);
                    loader.Field("managerOrDepartment")
                        .FromBatch(p => p.Id, batchUnitFunc)
                        .AndFromBatch(p => p.Id, batchManagerFunc, build => build.ConfigureFrom(loader.GetType()));
                });

            const string Query = @"
                    query {
                        people {
                            items {
                                id
                                managerOrDepartment {
                                    ... on Unit {
                                        id
                                        name
                                    }
                                    ... on Person {
                                        id
                                        fullName
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
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                }]
                            },{
                                id: 2,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 3,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 4,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 5,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 6,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 5,
                                    fullName: ""Aldon Exley""
                                }]
                            }]
                        }
                    }";

            TestHelpers.TestQuery(mutableBuilder, Query, Expected);

            batchUnitFunc.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>());
            batchManagerFunc.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>());
        }

        [Test]
        public void BatchUnionLoaderTaskFuncsFromContextAndPersonToUnitFromContextAndPersonToPersonUsingKeySelector()
        {
            var batchUnitFunc = Substitute.For<Func<TestUserContext, IEnumerable<int>, Task<IDictionary<int, Unit>>>>();
            batchUnitFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(1).ToDictionary(
                    id => id,
                    id => FakeData.People
                        .AsQueryable()
                        .Where(p => p.Id == id)
                        .Select(p => p.Unit)
                        .FirstOrDefault()));

            var batchManagerFunc = Substitute.For<Func<TestUserContext, IEnumerable<int>, Task<IDictionary<int, Person>>>>();
            batchManagerFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(1).ToDictionary(
                    id => id,
                    id => FakeData.People
                        .AsQueryable()
                        .Where(p => p.Id == id)
                        .Select(p => p.Manager)
                        .FirstOrDefault()));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.FullName);
                    loader.Field("managerOrDepartment")
                        .FromBatch(p => p.Id, batchUnitFunc)
                        .AndFromBatch(p => p.Id, batchManagerFunc, build => build.ConfigureFrom(loader.GetType()));
                });

            const string Query = @"
                    query {
                        people {
                            items {
                                id
                                managerOrDepartment {
                                    ... on Unit {
                                        id
                                        name
                                    }
                                    ... on Person {
                                        id
                                        fullName
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
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                }]
                            },{
                                id: 2,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 3,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 4,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 5,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 6,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 5,
                                    fullName: ""Aldon Exley""
                                }]
                            }]
                        }
                    }";

            TestHelpers.TestQuery(builder, Query, Expected);

            batchUnitFunc.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>());
            batchManagerFunc.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>());
        }

        [Test]
        public void BatchUnionFuncsFromContextAndPersonToUnitFromContextAndPersonToPersonAndSelectStringUsingKeySelector()
        {
            var batchUnitFunc = Substitute.For<Func<TestUserContext, IEnumerable<int>, IDictionary<int, Unit>>>();
            batchUnitFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(1).ToDictionary(
                    id => id,
                    id => FakeData.People
                        .AsQueryable()
                        .Where(p => p.Id == id)
                        .Select(p => p.Unit)
                        .FirstOrDefault()));

            var batchManagerFunc = Substitute.For<Func<TestUserContext, IEnumerable<int>, IDictionary<int, Person>>>();
            batchManagerFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(1).ToDictionary(
                    id => id,
                    id => FakeData.People
                        .AsQueryable()
                        .Where(p => p.Id == id)
                        .Select(p => p.Manager)
                        .FirstOrDefault()));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.FullName);
                    loader.Field("managerOrDepartmentIds")
                        .FromBatch(p => p.Id, batchUnitFunc)
                        .AndFromBatch(p => p.Id, batchManagerFunc)
                        .Select(x => x.Cast<IHasId<int>>().Select(o => o.Id));
                });

            const string Query = @"
                    query {
                        people {
                            items {
                                id
                                managerOrDepartmentIds
                            }
                        }
                    }";

            const string Expected = @"
                    {
                        people: {
                            items: [{
                                id: 1,
                                managerOrDepartmentIds: [1]
                            },{
                                id: 2,
                                managerOrDepartmentIds: [1, 1]
                            },{
                                id: 3,
                                managerOrDepartmentIds: [1, 1]
                            },{
                                id: 4,
                                managerOrDepartmentIds: [2, 2]
                            },{
                                id: 5,
                                managerOrDepartmentIds: [2, 2]
                            },{
                                id: 6,
                                managerOrDepartmentIds: [2, 5]
                            }]
                        }
                    }";

            TestHelpers.TestQuery(builder, Query, Expected);

            batchUnitFunc.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>());
            batchManagerFunc.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>());
        }

        [Test]
        public void BatchUnionMutableLoaderFuncsFromContextAndPersonToUnitFromContextAndPersonToPersonAndSelectStringUsingKeySelector()
        {
            var batchUnitFunc = Substitute.For<Func<TestUserContext, IEnumerable<int>, IDictionary<int, Unit>>>();
            batchUnitFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(1).ToDictionary(
                    id => id,
                    id => FakeData.People
                        .AsQueryable()
                        .Where(p => p.Id == id)
                        .Select(p => p.Unit)
                        .FirstOrDefault()));

            var batchManagerFunc = Substitute.For<Func<TestUserContext, IEnumerable<int>, IDictionary<int, Person>>>();
            batchManagerFunc
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(1).ToDictionary(
                    id => id,
                    id => FakeData.People
                        .AsQueryable()
                        .Where(p => p.Id == id)
                        .Select(p => p.Manager)
                        .FirstOrDefault()));

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.FullName);
                    loader.Field("managerOrDepartmentIds")
                        .FromBatch(p => p.Id, batchUnitFunc)
                        .AndFromBatch(p => p.Id, batchManagerFunc)
                        .Select(x => x.Cast<IHasId<int>>().Select(o => o.Id));
                });

            const string Query = @"
                    query {
                        people {
                            items {
                                id
                                managerOrDepartmentIds
                            }
                        }
                    }";

            const string Expected = @"
                    {
                        people: {
                            items: [{
                                id: 1,
                                managerOrDepartmentIds: [1]
                            },{
                                id: 2,
                                managerOrDepartmentIds: [1, 1]
                            },{
                                id: 3,
                                managerOrDepartmentIds: [1, 1]
                            },{
                                id: 4,
                                managerOrDepartmentIds: [2, 2]
                            },{
                                id: 5,
                                managerOrDepartmentIds: [2, 2]
                            },{
                                id: 6,
                                managerOrDepartmentIds: [2, 5]
                            }]
                        }
                    }";

            TestHelpers.TestQuery(mutableBuilder, Query, Expected);

            batchUnitFunc.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>());
            batchManagerFunc.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<int>>());
        }

        [Test]
        public void BatchUnionFuncsFromPersonToUnitFromPersonToPersonUsingKeySelector()
        {
            var batchUnitFunc = Substitute.For<Func<IEnumerable<int>, IDictionary<int, Unit>>>();
            batchUnitFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(0).ToDictionary(
                    id => id,
                    id => FakeData.People
                        .AsQueryable()
                        .Where(p => p.Id == id)
                        .Select(p => p.Unit)
                        .FirstOrDefault()));

            var batchManagerFunc = Substitute.For<Func<IEnumerable<int>, IDictionary<int, Person>>>();
            batchManagerFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(0).ToDictionary(
                    id => id,
                    id => FakeData.People
                        .AsQueryable()
                        .Where(p => p.Id == id)
                        .Select(p => p.Manager)
                        .FirstOrDefault()));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.FullName);
                    loader.Field("managerOrDepartment")
                        .FromBatch(p => p.Id, batchUnitFunc)
                        .AndFromBatch(p => p.Id, batchManagerFunc, build => build.ConfigureFrom(loader.GetType()));
                });

            const string Query = @"
                    query {
                        people {
                            items {
                                id
                                managerOrDepartment {
                                    ... on Unit {
                                        id
                                        name
                                    }
                                    ... on Person {
                                        id
                                        fullName
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
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                }]
                            },{
                                id: 2,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 3,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 4,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 5,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 6,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 5,
                                    fullName: ""Aldon Exley""
                                }]
                            }]
                        }
                    }";

            TestHelpers.TestQuery(builder, Query, Expected);

            batchUnitFunc.Received(1).Invoke(Arg.Any<IEnumerable<int>>());
            batchManagerFunc.Received(1).Invoke(Arg.Any<IEnumerable<int>>());
        }

        [Test]
        public void BatchUnionTaskFuncsFromPersonToUnitFromPersonToPersonUsingKeySelector()
        {
            var batchUnitFunc = Substitute.For<Func<IEnumerable<int>, Task<IDictionary<int, Unit>>>>();
            batchUnitFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(0).ToDictionary(
                    id => id,
                    id => FakeData.People
                        .AsQueryable()
                        .Where(p => p.Id == id)
                        .Select(p => p.Unit)
                        .FirstOrDefault()));

            var batchManagerFunc = Substitute.For<Func<IEnumerable<int>, Task<IDictionary<int, Person>>>>();
            batchManagerFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(0).ToDictionary(
                    id => id,
                    id => FakeData.People
                        .AsQueryable()
                        .Where(p => p.Id == id)
                        .Select(p => p.Manager)
                        .FirstOrDefault()));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.FullName);
                    loader.Field("managerOrDepartment")
                        .FromBatch(p => p.Id, batchUnitFunc)
                        .AndFromBatch(p => p.Id, batchManagerFunc, build => build.ConfigureFrom(loader.GetType()));
                });

            const string Query = @"
                    query {
                        people {
                            items {
                                id
                                managerOrDepartment {
                                    ... on Unit {
                                        id
                                        name
                                    }
                                    ... on Person {
                                        id
                                        fullName
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
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                }]
                            },{
                                id: 2,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 3,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 4,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 5,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 6,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 5,
                                    fullName: ""Aldon Exley""
                                }]
                            }]
                        }
                    }";

            TestHelpers.TestQuery(builder, Query, Expected);

            batchUnitFunc.Received(1).Invoke(Arg.Any<IEnumerable<int>>());
            batchManagerFunc.Received(1).Invoke(Arg.Any<IEnumerable<int>>());
        }

        [Test]
        public void BatchUnionMutableLoaderFuncsFromPersonToUnitFromPersonToPersonUsingKeySelector()
        {
            var batchUnitFunc = Substitute.For<Func<IEnumerable<int>, IDictionary<int, Unit>>>();
            batchUnitFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(0).ToDictionary(
                    id => id,
                    id => FakeData.People
                        .AsQueryable()
                        .Where(p => p.Id == id)
                        .Select(p => p.Unit)
                        .FirstOrDefault()));

            var batchManagerFunc = Substitute.For<Func<IEnumerable<int>, IDictionary<int, Person>>>();
            batchManagerFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(0).ToDictionary(
                    id => id,
                    id => FakeData.People
                        .AsQueryable()
                        .Where(p => p.Id == id)
                        .Select(p => p.Manager)
                        .FirstOrDefault()));

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.FullName);
                    loader.Field("managerOrDepartment")
                        .FromBatch(p => p.Id, batchUnitFunc)
                        .AndFromBatch(p => p.Id, batchManagerFunc, build => build.ConfigureFrom(loader.GetType()));
                });

            const string Query = @"
                    query {
                        people {
                            items {
                                id
                                managerOrDepartment {
                                    ... on Unit {
                                        id
                                        name
                                    }
                                    ... on Person {
                                        id
                                        fullName
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
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                }]
                            },{
                                id: 2,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 3,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 4,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 5,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 6,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 5,
                                    fullName: ""Aldon Exley""
                                }]
                            }]
                        }
                    }";

            TestHelpers.TestQuery(mutableBuilder, Query, Expected);

            batchUnitFunc.Received(1).Invoke(Arg.Any<IEnumerable<int>>());
            batchManagerFunc.Received(1).Invoke(Arg.Any<IEnumerable<int>>());
        }

        [Test]
        public void BatchUnionLoaderTaskFuncsFromPersonToUnitFromPersonToPersonUsingKeySelector()
        {
            var batchUnitFunc = Substitute.For<Func<IEnumerable<int>, Task<IDictionary<int, Unit>>>>();
            batchUnitFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(0).ToDictionary(
                    id => id,
                    id => FakeData.People
                        .AsQueryable()
                        .Where(p => p.Id == id)
                        .Select(p => p.Unit)
                        .FirstOrDefault()));

            var batchManagerFunc = Substitute.For<Func<IEnumerable<int>, Task<IDictionary<int, Person>>>>();
            batchManagerFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(0).ToDictionary(
                    id => id,
                    id => FakeData.People
                        .AsQueryable()
                        .Where(p => p.Id == id)
                        .Select(p => p.Manager)
                        .FirstOrDefault()));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.FullName);
                    loader.Field("managerOrDepartment")
                        .FromBatch(p => p.Id, batchUnitFunc)
                        .AndFromBatch(p => p.Id, batchManagerFunc, build => build.ConfigureFrom(loader.GetType()));
                });

            const string Query = @"
                    query {
                        people {
                            items {
                                id
                                managerOrDepartment {
                                    ... on Unit {
                                        id
                                        name
                                    }
                                    ... on Person {
                                        id
                                        fullName
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
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                }]
                            },{
                                id: 2,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 3,
                                managerOrDepartment: [{
                                    id: 1,
                                    name: ""Alpha""
                                },{
                                    id: 1,
                                    fullName: ""Linoel Livermore""
                                }]
                            },{
                                id: 4,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 5,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 2,
                                    fullName: ""Sophie Gandley""
                                }]
                            },{
                                id: 6,
                                managerOrDepartment: [{
                                    id: 2,
                                    name: ""Beta""
                                },{
                                    id: 5,
                                    fullName: ""Aldon Exley""
                                }]
                            }]
                        }
                    }";

            TestHelpers.TestQuery(builder, Query, Expected);

            batchUnitFunc.Received(1).Invoke(Arg.Any<IEnumerable<int>>());
            batchManagerFunc.Received(1).Invoke(Arg.Any<IEnumerable<int>>());
        }

        [Test]
        public void BatchUnionFuncsFromPersonToUnitFromPersonToPersonAndSelectStringUsingKeySelector()
        {
            var batchUnitFunc = Substitute.For<Func<IEnumerable<int>, IDictionary<int, Unit>>>();
            batchUnitFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(0).ToDictionary(
                    id => id,
                    id => FakeData.People
                        .AsQueryable()
                        .Where(p => p.Id == id)
                        .Select(p => p.Unit)
                        .FirstOrDefault()));

            var batchManagerFunc = Substitute.For<Func<IEnumerable<int>, IDictionary<int, Person>>>();
            batchManagerFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(0).ToDictionary(
                    id => id,
                    id => FakeData.People
                        .AsQueryable()
                        .Where(p => p.Id == id)
                        .Select(p => p.Manager)
                        .FirstOrDefault()));

            var builder = CreateQueryBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.FullName);
                    loader.Field("managerOrDepartmentIds")
                        .FromBatch(p => p.Id, batchUnitFunc)
                        .AndFromBatch(p => p.Id, batchManagerFunc)
                        .Select(x => x.Cast<IHasId<int>>().Select(o => o.Id));
                });

            const string Query = @"
                    query {
                        people {
                            items {
                                id
                                managerOrDepartmentIds
                            }
                        }
                    }";

            const string Expected = @"
                    {
                        people: {
                            items: [{
                                id: 1,
                                managerOrDepartmentIds: [1]
                            },{
                                id: 2,
                                managerOrDepartmentIds: [1, 1]
                            },{
                                id: 3,
                                managerOrDepartmentIds: [1, 1]
                            },{
                                id: 4,
                                managerOrDepartmentIds: [2, 2]
                            },{
                                id: 5,
                                managerOrDepartmentIds: [2, 2]
                            },{
                                id: 6,
                                managerOrDepartmentIds: [2, 5]
                            }]
                        }
                    }";

            TestHelpers.TestQuery(builder, Query, Expected);

            batchUnitFunc.Received(1).Invoke(Arg.Any<IEnumerable<int>>());
            batchManagerFunc.Received(1).Invoke(Arg.Any<IEnumerable<int>>());
        }

        [Test]
        public void BatchUnionMutableLoaderFuncsFromPersonToUnitFromPersonToPersonAndSelectStringUsingKeySelector()
        {
            var batchUnitFunc = Substitute.For<Func<IEnumerable<int>, IDictionary<int, Unit>>>();
            batchUnitFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(0).ToDictionary(
                    id => id,
                    id => FakeData.People
                        .AsQueryable()
                        .Where(p => p.Id == id)
                        .Select(p => p.Unit)
                        .FirstOrDefault()));

            var batchManagerFunc = Substitute.For<Func<IEnumerable<int>, IDictionary<int, Person>>>();
            batchManagerFunc
                .Invoke(Arg.Any<IEnumerable<int>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<int>>(0).ToDictionary(
                    id => id,
                    id => FakeData.People
                        .AsQueryable()
                        .Where(p => p.Id == id)
                        .Select(p => p.Manager)
                        .FirstOrDefault()));

            var mutableBuilder = CreateQueryMutableBuilder(
                loader =>
                {
                    loader.Field(u => u.Id);
                    loader.Field(u => u.FullName);
                    loader.Field("managerOrDepartmentIds")
                        .FromBatch(p => p.Id, batchUnitFunc)
                        .AndFromBatch(p => p.Id, batchManagerFunc)
                        .Select(x => x.Cast<IHasId<int>>().Select(o => o.Id));
                });

            const string Query = @"
                    query {
                        people {
                            items {
                                id
                                managerOrDepartmentIds
                            }
                        }
                    }";

            const string Expected = @"
                    {
                        people: {
                            items: [{
                                id: 1,
                                managerOrDepartmentIds: [1]
                            },{
                                id: 2,
                                managerOrDepartmentIds: [1, 1]
                            },{
                                id: 3,
                                managerOrDepartmentIds: [1, 1]
                            },{
                                id: 4,
                                managerOrDepartmentIds: [2, 2]
                            },{
                                id: 5,
                                managerOrDepartmentIds: [2, 2]
                            },{
                                id: 6,
                                managerOrDepartmentIds: [2, 5]
                            }]
                        }
                    }";

            TestHelpers.TestQuery(mutableBuilder, Query, Expected);

            batchUnitFunc.Received(1).Invoke(Arg.Any<IEnumerable<int>>());
            batchManagerFunc.Received(1).Invoke(Arg.Any<IEnumerable<int>>());
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
