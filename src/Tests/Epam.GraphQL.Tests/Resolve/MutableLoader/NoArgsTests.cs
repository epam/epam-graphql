// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NSubstitute;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Resolve.MutableLoader
{
    [TestFixture]
    public class NoArgsTests : BaseTests
    {
        [Test]
        public void ShouldResolveIntFieldWithTypeParam()
        {
            var resolver = Substitute.For<Func<TestUserContext, Person, int>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>())
                .Returns(callInfo => callInfo.ArgAt<Person>(1).Id + 10);

            TestHelpers.TestMutableLoader(
                builder: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("test")
                        .Resolve(resolver);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2),
                connectionName: "people",
                query: @"
                    query {
                        people {
                            items {
                                id
                                test
                            }
                        }
                    }",
                expected: @"
                    {
                        people: {
                            items: [{
                                id: 1,
                                test: 11
                            },{
                                id: 2,
                                test: 12
                            }]
                        }
                    }");

            resolver.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
        }

        [Test]
        public void ShouldResolveIntFieldWithTypeParamNoContext()
        {
            var resolver = Substitute.For<Func<Person, int>>();
            resolver
                .Invoke(Arg.Any<Person>())
                .Returns(callInfo => callInfo.ArgAt<Person>(0).Id);

            TestHelpers.TestMutableLoader(
                builder: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("test")
                        .Resolve(resolver);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2),
                connectionName: "people",
                query: @"
                    query {
                        people {
                            items {
                                id
                                test
                            }
                        }
                    }",
                expected: @"
                    {
                        people: {
                            items: [{
                                id: 1,
                                test: 1
                            },{
                                id: 2,
                                test: 2
                            }]
                        }
                    }");

            resolver.Received(2)
                .Invoke(Arg.Any<Person>());
        }

        [Test(Description = "Field<int>(...).Resolve(_ => Task 10)")]
        public void ShouldResolveTaskIntFieldWithTypeParam()
        {
            var resolver = Substitute.For<Func<TestUserContext, Person, Task<int>>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>())
                .Returns(callInfo => Task<int>.Factory.StartNew(() => callInfo.ArgAt<Person>(1).Id + 10));

            TestHelpers.TestMutableLoader(
                builder: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("test")
                        .Resolve(resolver);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2),
                connectionName: "people",
                query: @"
                    query {
                        people {
                            items {
                                id
                                test
                            }
                        }
                    }",
                expected: @"
                    {
                        people: {
                            items: [{
                                id: 1,
                                test: 11
                            },{
                                id: 2,
                                test: 12
                            }]
                        }
                    }");

            resolver.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
        }

        [Test(Description = "Field<int?>(...).Resolve(_ => 10)")]
        public void ShouldResolveNullableIntFieldWithTypeParam()
        {
            var resolver = Substitute.For<Func<TestUserContext, Person, int?>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>())
                .Returns(callInfo => callInfo.ArgAt<Person>(1).Id + 10);

            TestHelpers.TestMutableLoader(
                builder: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("test")
                        .Resolve(resolver);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2),
                connectionName: "people",
                query: @"
                    query {
                        people {
                            items {
                                id
                                test
                            }
                        }
                    }",
                expected: @"
                    {
                        people: {
                            items: [{
                                id: 1,
                                test: 11
                            },{
                                id: 2,
                                test: 12
                            }]
                        }
                    }");

            resolver.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
        }

        [Test(Description = "Field<int?>(...).Resolve(_ => null)")]
        public void ShouldResolveNullableIntFieldReturningNullWithTypeParam()
        {
            var resolver = Substitute.For<Func<TestUserContext, Person, int?>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>())
                .Returns(callInfo => callInfo.ArgAt<Person>(1).ManagerId + 10);

            TestHelpers.TestMutableLoader(
                builder: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("test")
                        .Resolve(resolver);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2),
                connectionName: "people",
                query: @"
                    query {
                        people {
                            items {
                                id
                                test
                            }
                        }
                    }",
                expected: @"
                    {
                        people: {
                            items: [{
                                id: 1,
                                test: null
                            },{
                                id: 2,
                                test: 11
                            }]
                        }
                    }");

            resolver.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
        }

        [Test(Description = "Field<string>(...).Resolve(_ => \"test\")")]
        public void ShouldResolveStringWithTypeParam()
        {
            var resolver = Substitute.For<Func<TestUserContext, Person, string>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>())
                .Returns(callInfo => callInfo.ArgAt<Person>(1).FullName + " Foo");

            TestHelpers.TestMutableLoader(
                builder: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("test")
                        .Resolve(resolver);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2),
                connectionName: "people",
                query: @"
                    query {
                        people {
                            items {
                                id
                                test
                            }
                        }
                    }",
                expected: @"
                    {
                        people: {
                            items: [{
                                id: 1,
                                test: ""Linoel Livermore Foo""
                            },{
                                id: 2,
                                test: ""Sophie Gandley Foo""
                            }]
                        }
                    }");

            resolver.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
        }

        [Test(Description = "Field<CustomObject<int>>(...).Resolve(_ => new CustomObject<int> { TestField = 10 })")]
        public void ShouldResolveCustomObjectWithIntFieldWithTypeParam()
        {
            var resolver = Substitute.For<Func<TestUserContext, Person, CustomObject<int>>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>())
                .Returns(callInfo => CustomObject.Create(callInfo.ArgAt<Person>(1).Id + 10));

            TestHelpers.TestMutableLoader(
                builder: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("test")
                        .Resolve(resolver);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2),
                connectionName: "people",
                query: @"
                    query {
                        people {
                            items {
                                id
                                test { 
                                    testField
                                }
                            }
                        }
                    }",
                expected: @"
                    {
                        people: {
                            items: [{
                                id: 1,
                                test: {
                                    testField: 11
                                }
                            },{
                                id: 2,
                                test: {
                                    testField: 12
                                }
                            }]
                        }
                    }");

            resolver.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
        }

        [Test(Description = "Field<CustomObject<int?>>(...).Resolve(_ => new CustomObject<int?> { TestField = 10 })")]
        public void ShouldResolveCustomObjectWithNullableIntFieldWithTypeParam()
        {
            var resolver = Substitute.For<Func<TestUserContext, Person, CustomObject<int?>>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>())
                .Returns(callInfo => CustomObject.Create<int?>(callInfo.ArgAt<Person>(1).Id + 10));

            TestHelpers.TestMutableLoader(
                builder: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("test")
                        .Resolve(resolver);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2),
                connectionName: "people",
                query: @"
                    query {
                        people {
                            items {
                                id
                                test { 
                                    testField
                                }
                            }
                        }
                    }",
                expected: @"
                    {
                        people: {
                            items: [{
                                id: 1,
                                test: {
                                    testField: 11
                                }
                            },{
                                id: 2,
                                test: {
                                    testField: 12
                                }
                            }]
                        }
                    }");

            resolver.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
        }

        [Test(Description = "Field<CustomObject<int?>>(...).Resolve(_ => new CustomObject<int?> { TestField = null })")]
        public void ShouldResolveCustomObjectWithNullableIntFieldReturningNullWithTypeParam()
        {
            var resolver = Substitute.For<Func<TestUserContext, Person, CustomObject<int?>>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>())
                .Returns(callInfo => CustomObject.Create(callInfo.ArgAt<Person>(1).ManagerId + 10));

            TestHelpers.TestMutableLoader(
                builder: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("test")
                        .Resolve(resolver);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2),
                connectionName: "people",
                query: @"
                    query {
                        people {
                            items {
                                id
                                test { 
                                    testField
                                }
                            }
                        }
                    }",
                expected: @"
                    {
                        people: {
                            items: [{
                                id: 1,
                                test: {
                                    testField: null
                                }
                            },{
                                id: 2,
                                test: {
                                    testField: 11
                                }
                            }]
                        }
                    }");

            resolver.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
        }

        [Test(Description = "Field<CustomObject<string>>(...).Resolve(_ => new CustomObject<string> { TestField = \"test\" })")]
        public void ShouldResolveCustomObjectWithStringFieldWithTypeParam()
        {
            var resolver = Substitute.For<Func<TestUserContext, Person, CustomObject<string>>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>())
                .Returns(callInfo => CustomObject.Create(callInfo.ArgAt<Person>(1).FullName + " test"));

            TestHelpers.TestMutableLoader(
                builder: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("test")
                        .Resolve(resolver);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2),
                connectionName: "people",
                query: @"
                    query {
                        people {
                            items {
                                id
                                test { 
                                    testField
                                }
                            }
                        }
                    }",
                expected: @"
                    {
                        people: {
                            items: [{
                                id: 1,
                                test: {
                                    testField: ""Linoel Livermore test""
                                }
                            },{
                                id: 2,
                                test: {
                                    testField: ""Sophie Gandley test""
                                }
                            }]
                        }
                    }");

            resolver.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
        }

        [Test(Description = "Field<CustomObject<int[]>>(...).Resolve(_ => new CustomObject<int[]> { ... })")]
        public void ShouldResolveCustomObjectWithIntArrayFieldWithTypeParam()
        {
            var resolver = Substitute.For<Func<TestUserContext, Person, CustomObject<int[]>>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>())
                .Returns(callInfo => CustomObject.Create(new[] { callInfo.ArgAt<Person>(1).Id + 10, 20 }));

            TestHelpers.TestMutableLoader(
                builder: builder =>
                {
                    builder.Field("test")
                        .Resolve(resolver);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2),
                connectionName: "people",
                query: @"
                    query {
                        people {
                            items {
                                test { 
                                    testField
                                }
                            }
                        }
                    }",
                expected: @"
                    {
                        people: {
                            items: [{
                                test: {
                                    testField: [11, 20]
                                }
                            },{
                                test: {
                                    testField: [12, 20]
                                }
                            }]
                        }
                    }");

            resolver.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
        }

        [Test(Description = "Field<CustomObject<List<int>>>(...).Resolve(_ => new CustomObject<List<int>> { ... })")]
        public void ShouldResolveCustomObjectWithIntListFieldWithTypeParam()
        {
            var resolver = Substitute.For<Func<TestUserContext, Person, CustomObject<List<int>>>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>())
                .Returns(callInfo => CustomObject.Create(new List<int> { callInfo.ArgAt<Person>(1).Id + 10, 20 }));

            TestHelpers.TestMutableLoader(
                builder: builder =>
                {
                    builder.Field("test")
                        .Resolve(resolver);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2),
                connectionName: "people",
                query: @"
                    query {
                        people {
                            items {
                                test { 
                                    testField
                                }
                            }
                        }
                    }",
                expected: @"
                    {
                        people: {
                            items: [{
                                test: {
                                    testField: [11, 20]
                                }
                            },{
                                test: {
                                    testField: [12, 20]
                                }
                            }]
                        }
                    }");

            resolver.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
        }

        [Test(Description = "Field<IEnumerable<CustomObject<int>>>(...).Resolve(...)")]
        public void ShouldResolveClassWithIntFieldArrayResultWithTypeParam()
        {
            var resolver = Substitute.For<Func<TestUserContext, Person, IEnumerable<CustomObject<int>>>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>())
                .Returns(callInfo => new[] { 1, 2, 100500, 10 }.Select(n => new CustomObject<int> { TestField = callInfo.ArgAt<Person>(1).Id + n }));

            TestHelpers.TestMutableLoader(
                builder: builder =>
                {
                    builder.Field("test")
                        .Resolve(resolver);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2),
                connectionName: "people",
                query: @"
                    query {
                        people {
                            items {
                                test { 
                                    testField
                                }
                            }
                        }
                    }",
                expected: @"
                    {
                        people: {
                            items: [{
                                test: [{
                                    testField: 2
                                },{
                                    testField: 3
                                },{
                                    testField: 100501
                                }, {
                                    testField: 11
                                }]
                            },{
                                test: [{
                                    testField: 3
                                },{
                                    testField: 4
                                },{
                                    testField: 100502
                                }, {
                                    testField: 12
                                }]
                            }]
                        }
                    }");

            resolver.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
        }

        [Test(Description = "Field<IEnumerable<CustomObject<int>>>(...).Resolve(...) Task")]
        public void ShouldResolveTaskClassWithIntFieldArrayResultWithTypeParam()
        {
            var resolver = Substitute.For<Func<TestUserContext, Person, Task<IEnumerable<CustomObject<int>>>>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>())
                .Returns(async callInfo =>
                {
                    await Task.Yield();
                    return new[] { 1, 2, 100500, 10 }.Select(n => new CustomObject<int> { TestField = callInfo.ArgAt<Person>(1).Id + n });
                });

            TestHelpers.TestMutableLoader(
                builder: builder =>
                {
                    builder.Field("test")
                        .Resolve(resolver);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2),
                connectionName: "people",
                query: @"
                    query {
                        people {
                            items {
                                test { 
                                    testField
                                }
                            }
                        }
                    }",
                expected: @"
                    {
                        people: {
                            items: [{
                                test: [{
                                    testField: 2
                                },{
                                    testField: 3
                                },{
                                    testField: 100501
                                }, {
                                    testField: 11
                                }]
                            },{
                                test: [{
                                    testField: 3
                                },{
                                    testField: 4
                                },{
                                    testField: 100502
                                }, {
                                    testField: 12
                                }]
                            }]
                        }
                    }");

            resolver.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
        }

        [Test(Description = "Field<IEnumerable<CustomObject<CustomOnject<int>>>>(...).Resolve(...)")]
        public void ShouldResolveClassWithClassIntFieldArrayResultWithTypeParam()
        {
            var resolver = Substitute.For<Func<TestUserContext, Person, IEnumerable<CustomObject<CustomObject<int>>>>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>())
                .Returns(callInfo => new[] { 1, 2, 100500, 10 }
                    .Select(n => new CustomObject<int> { TestField = callInfo.ArgAt<Person>(1).Id + n })
                    .Select(obj => new CustomObject<CustomObject<int>> { TestField = obj }));

            TestHelpers.TestMutableLoader(
                builder: builder =>
                {
                    builder.Field("test")
                        .Resolve(resolver);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2),
                connectionName: "people",
                query: @"
                    query {
                        people {
                            items {
                                test { 
                                    testField {
                                        testField
                                    }
                                }
                            }
                        }
                    }",
                expected: @"
                    {
                        people: {
                            items: [{
                                test: [{
                                    testField: {
                                        testField: 2
                                    }
                                },{
                                    testField: {
                                        testField: 3
                                    }
                                },{
                                    testField: {
                                        testField: 100501
                                    }
                                }, {
                                    testField: {
                                        testField: 11
                                    }
                                },]
                            },{
                                test: [{
                                    testField: {
                                        testField: 3
                                    }
                                },{
                                    testField: {
                                        testField: 4
                                    }
                                },{
                                    testField: {
                                        testField: 100502
                                    }
                                }, {
                                    testField: {
                                        testField: 12
                                    }
                                },]
                            }]
                        }
                    }");

            resolver.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
        }

        [Test(Description = "Field<IEnumerable<int>>(...).Resolve(...)")]
        public void ShouldResolveIntArrayResultWithTypeParam()
        {
            var resolver = Substitute.For<Func<TestUserContext, Person, IEnumerable<int>>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>())
                .Returns(callInfo => new[] { 1, 2, 100500, callInfo.ArgAt<Person>(1).Id + 10 });

            TestHelpers.TestMutableLoader(
                builder: builder =>
                {
                    builder.Field("test")
                        .Resolve(resolver);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2),
                connectionName: "people",
                query: @"
                    query {
                        people {
                            items {
                                test
                            }
                        }
                    }",
                expected: @"
                    {
                        people: {
                            items: [{
                                test: [1, 2, 100500, 11]
                            },{
                                test: [1, 2, 100500, 12]
                            }]
                        }
                    }");

            resolver.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
        }

        [Test(Description = "Field<IEnumerable<int>>(...).Resolve(...)")]
        public void ShouldResolveTaskIntArrayResultWithTypeParam()
        {
            var resolver = Substitute.For<Func<TestUserContext, Person, Task<IEnumerable<int>>>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>())
                .Returns(async callInfo =>
                {
                    await Task.Yield();
                    return new[] { 1, 2, 100500, callInfo.ArgAt<Person>(1).Id + 10 }.AsEnumerable();
                });

            TestHelpers.TestMutableLoader(
                builder: builder =>
                {
                    builder.Field("test")
                        .Resolve(resolver);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2),
                connectionName: "people",
                query: @"
                    query {
                        people {
                            items {
                                test
                            }
                        }
                    }",
                expected: @"
                    {
                        people: {
                            items: [{
                                test: [1, 2, 100500, 11]
                            },{
                                test: [1, 2, 100500, 12]
                            }]
                        }
                    }");

            resolver.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
        }

        [Test(Description = "Field<Geometry>(...).AsUnionOf<Line>(...).And<Circle>(...).Resolve(...)")]
        public void ShouldResolveUnionOfTwoTypesWithTypeParam()
        {
            var resolver = Substitute.For<Func<TestUserContext, Person, Geometry>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>())
                .Returns(callInfo => new Line
                {
                    Id = callInfo.ArgAt<Person>(1).Id,
                    Length = 10,
                });

            TestHelpers.TestMutableLoader(
                builder: builder =>
                {
                    builder.Field("test")
                        .AsUnionOf<Line>()
                            .And<Circle>()
                        .Resolve(resolver);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2),
                connectionName: "people",
                query: @"
                    query {
                        people {
                            items {
                                test {
                                    ... on Line {
                                        id
                                        length
                                    }
                                }
                            }
                        }
                    }",
                expected: @"
                    {
                        people: {
                            items: [{
                                test: {
                                    id: 1,
                                    length: 10.0
                                }
                            },{
                                test: {
                                    id: 2,
                                    length: 10.0
                                }
                            }]
                        }
                    }");

            resolver.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
        }

        [Test(Description = "Field<Geometry>(...).AsUnionOf<Line>(...).And<Circle>(...).Resolve(...)")]
        public void ShouldResolveUnionOfTwoGenericTypesWithTypeParam()
        {
            var resolver = Substitute.For<Func<TestUserContext, Person, BaseCustomObject<int>>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>())
                .Returns(callInfo => CustomObject.Create(callInfo.ArgAt<Person>(1).FullName + " test"));

            TestHelpers.TestMutableLoader(
                builder: builder =>
                {
                    builder.Field("test")
                        .AsUnionOf<CustomObject<string>>()
                            .And<CustomObject<string, int>>()
                        .Resolve(resolver);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2),
                connectionName: "people",
                query: @"
                    query {
                        people {
                            items {
                                test {
                                    ... on CustomObjectOfString {
                                        testField
                                    }
                                }
                            }
                        }
                    }",
                expected: @"
                    {
                        people: {
                            items: [{
                                test: {
                                    testField: ""Linoel Livermore test""
                                }
                            },{
                                test: {
                                    testField: ""Sophie Gandley test""
                                }
                            }]
                        }
                    }");

            resolver.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
        }

        [Test(Description = "Field<IEnumerable<Geometry>>(...).AsUnionOf<IEnumerable<Line>, Line>(...).And<IEnumerable<Circle>, Circle>(...).Resolve(...)")]
        public void ShouldResolveArrayOfUnionOfTwoTypesWithTypeParam()
        {
            var resolver = Substitute.For<Func<TestUserContext, Person, IEnumerable<Geometry>>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>())
                .Returns(callInfo => new Geometry[]
                {
                    new Line
                    {
                        Id = callInfo.ArgAt<Person>(1).Id,
                        Length = 10,
                    },
                    new Circle
                    {
                        Id = callInfo.ArgAt<Person>(1).Id,
                        Radius = 5,
                    },
                });

            TestHelpers.TestMutableLoader(
                builder: builder =>
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    builder.Field("test")
                        .AsUnionOf<IEnumerable<Line>, Line>()
                            .And<IEnumerable<Circle>, Circle>()
                        .Resolve(resolver);
#pragma warning restore CS0618 // Type or member is obsolete
                },
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2),
                connectionName: "people",
                query: @"
                    query {
                        people {
                            items {
                                test {
                                    ... on Line {
                                        id
                                        length
                                    }
                                    ... on Circle {
                                        id
                                        radius
                                    }
                                }
                            }
                        }
                    }",
                expected: @"
                    {
                        people: {
                            items: [{
                                test: [{
                                    id: 1,
                                    length: 10.0
                                },{
                                    id: 1,
                                    radius: 5.0
                                }]
                            },{
                                test: [{
                                    id: 2,
                                    length: 10.0
                                },{
                                    id: 2,
                                    radius: 5.0
                                }]
                            }]
                        }
                    }");

            resolver.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
        }

        [Test(Description = "Field<IEnumerable<BaseCustomObject<int>>>(...).AsUnionOf<IEnumerable<CustomObject<string>>, CustomObject<string>>(...).And<IEnumerable<CustomObject<string, int>>, CustomObject<string, int>>(...).Resolve(...)")]
        public void ShouldResolveArrayOfUnionOfTwoGenericTypesWithTypeParam()
        {
            var resolver = Substitute.For<Func<TestUserContext, Person, IEnumerable<BaseCustomObject<int>>>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>())
                .Returns(callInfo => new BaseCustomObject<int>[]
                {
                    CustomObject.Create("test"),
                    CustomObject.Create("test", callInfo.ArgAt<Person>(1).Id),
                });

            TestHelpers.TestMutableLoader(
                builder: builder =>
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    builder.Field("test")
                        .AsUnionOf<IEnumerable<CustomObject<string>>, CustomObject<string>>()
                            .And<IEnumerable<CustomObject<string, int>>, CustomObject<string, int>>()
#pragma warning restore CS0618 // Type or member is obsolete
                        .Resolve(resolver);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2),
                connectionName: "people",
                query: @"
                    query {
                        people {
                            items {
                                test {
                                    ... on CustomObjectOfString {
                                        id
                                        testField
                                    }
                                    ... on CustomObjectOfStringAndInt {
                                        id
                                        firstField
                                        secondField
                                    }
                                }
                            }
                        }
                    }",
                expected: @"
                    {
                        people: {
                            items: [{
                                test: [{
                                    id: 0,
                                    testField: ""test""
                                },{
                                    id: 0,
                                    firstField: ""test"",
                                    secondField: 1
                                }]
                            },{
                                test: [{
                                    id: 0,
                                    testField: ""test""
                                },{
                                    id: 0,
                                    firstField: ""test"",
                                    secondField: 2
                                }]
                            }]
                        }
                    }");

            resolver.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
        }

        [Test(Description = "Field<IEnumerable<BaseCustomObject<int>>>(...).AsUnionOf<IEnumerable<CustomObject<string>>, CustomObject<string>>(...).And<IEnumerable<CustomObject<string, int>>, CustomObject<string, int>>(...).Resolve(...)")]
        public void ShouldResolveArrayOfUnionOfTwoGenericTypesWithBuildersWithTypeParam()
        {
            var resolver = Substitute.For<Func<TestUserContext, Person, IEnumerable<BaseCustomObject<int>>>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>())
                .Returns(callInfo => new BaseCustomObject<int>[]
                {
                    CustomObject.Create("test"),
                    CustomObject.Create("test", callInfo.ArgAt<Person>(1).Id + 1),
                });

            TestHelpers.TestMutableLoader(
                builder: builder =>
                {
#pragma warning disable CS0618 // Type or member is obsolete
                    builder.Field("test")
                        .AsUnionOf<IEnumerable<CustomObject<string>>, CustomObject<string>>(b =>
                        {
                            b.Name = "CustomObject1";
                            b.Field(o => o.Id);
                            b.Field("myField", o => o.TestField);
                        })
                            .And<IEnumerable<CustomObject<string, int>>, CustomObject<string, int>>(b =>
                            {
                                b.Name = "CustomObject2";
                                b.Field(o => o.Id);
                                b.Field(o => o.FirstField);
                                b.Field(o => o.SecondField);
                            })
#pragma warning restore CS0618 // Type or member is obsolete
                        .Resolve(resolver);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2),
                connectionName: "people",
                query: @"
                    query {
                        people {
                            items {
                                test {
                                    ... on CustomObject1 {
                                        id
                                        myField
                                    }
                                    ... on CustomObject2 {
                                        id
                                        firstField
                                        secondField
                                    }
                                }
                            }
                        }
                    }",
                expected: @"
                    {
                        people: {
                            items: [{
                                test: [{
                                    id: 0,
                                    myField: ""test""
                                },{
                                    id: 0,
                                    firstField: ""test"",
                                    secondField: 2
                                }]
                            },{
                                test: [{
                                    id: 0,
                                    myField: ""test""
                                },{
                                    id: 0,
                                    firstField: ""test"",
                                    secondField: 3
                                }]
                            }]
                        }
                    }");

            resolver.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
        }

        [Test]
        public void ShouldResolveClassWithIntFieldWithBuilder()
        {
            var resolver = Substitute.For<Func<TestUserContext, Person, CustomObject<int>>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>())
                .Returns(callInfo => new CustomObject<int> { TestField = callInfo.ArgAt<Person>(1).Id + 100500 });

            TestHelpers.TestMutableLoader(
                builder: builder =>
                {
                    builder.Field("test")
                        .Resolve(resolver, b => b.Field("myField", o => o.TestField));
                },
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2),
                connectionName: "people",
                query: @"
                    query {
                        people {
                            items {
                                test {
                                    myField
                                }
                            }
                        }
                    }",
                expected: @"
                    {
                        people: {
                            items: [{
                                test: {
                                    myField: 100501
                                }
                            },{
                                test: {
                                    myField: 100502
                                }
                            }]
                        }
                    }");

            resolver.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
        }

        [Test]
        public void ShouldResolveTaskClassWithIntFieldWithBuilder()
        {
            var resolver = Substitute.For<Func<TestUserContext, Person, Task<CustomObject<int>>>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>())
                .Returns(async callInfo =>
                {
                    await Task.Yield();
                    return new CustomObject<int> { TestField = callInfo.ArgAt<Person>(1).Id + 100500 };
                });

            TestHelpers.TestMutableLoader(
                builder: builder =>
                {
                    builder.Field("test")
                        .Resolve(resolver, b => b.Field("myField", o => o.TestField));
                },
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2),
                connectionName: "people",
                query: @"
                    query {
                        people {
                            items {
                                test {
                                    myField
                                }
                            }
                        }
                    }",
                expected: @"
                    {
                        people: {
                            items: [{
                                test: {
                                    myField: 100501
                                }
                            },{
                                test: {
                                    myField: 100502
                                }
                            }]
                        }
                    }");

            resolver.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
        }

        [Test]
        public void ShouldResolveClassWithIntFieldArrayResultWithBuilder()
        {
            var resolver = Substitute.For<Func<TestUserContext, Person, IEnumerable<CustomObject<int>>>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>())
                .Returns(callInfo => new[] { 1, 2, 100500, callInfo.ArgAt<Person>(1).Id + 10 }.Select(n => new CustomObject<int> { TestField = n }));

            TestHelpers.TestMutableLoader(
                builder: builder =>
                {
                    builder.Field("test")
                        .Resolve(resolver, b => b.Field("myField", o => o.TestField));
                },
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2),
                connectionName: "people",
                query: @"
                    query {
                        people {
                            items {
                                test {
                                    myField
                                }
                            }
                        }
                    }",
                expected: @"
                    {
                        people: {
                            items: [{
                                test: [{
                                    myField: 1
                                },{
                                    myField: 2
                                },{
                                    myField: 100500
                                }, {
                                    myField: 11
                                }]
                            },{
                                test: [{
                                    myField: 1
                                },{
                                    myField: 2
                                },{
                                    myField: 100500
                                }, {
                                    myField: 12
                                }]
                            }]
                        }
                    }");

            resolver.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
        }

        [Test]
        public void ShouldResolveTaskClassWithIntFieldArrayResultWithBuilder()
        {
            var resolver = Substitute.For<Func<TestUserContext, Person, Task<IEnumerable<CustomObject<int>>>>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>())
                .Returns(async callInfo =>
                {
                    await Task.Yield();
                    return new[] { 1, 2, 100500, callInfo.ArgAt<Person>(1).Id + 10 }.Select(n => new CustomObject<int> { TestField = n });
                });

            TestHelpers.TestMutableLoader(
                builder: builder =>
                {
                    builder.Field("test")
                        .Resolve(resolver, b => b.Field("myField", o => o.TestField));
                },
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2),
                connectionName: "people",
                query: @"
                    query {
                        people {
                            items {
                                test {
                                    myField
                                }
                            }
                        }
                    }",
                expected: @"
                    {
                        people: {
                            items: [{
                                test: [{
                                    myField: 1
                                },{
                                    myField: 2
                                },{
                                    myField: 100500
                                }, {
                                    myField: 11
                                }]
                            },{
                                test: [{
                                    myField: 1
                                },{
                                    myField: 2
                                },{
                                    myField: 100500
                                }, {
                                    myField: 12
                                }]
                            }]
                        }
                    }");

            resolver.Received(2)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
        }

        public static class CustomObject
        {
            public static CustomObject<T> Create<T>(T fieldValue) => new() { TestField = fieldValue };

            public static CustomObject<T, T2> Create<T, T2>(T firstField, T2 secondField) => new() { FirstField = firstField, SecondField = secondField };
        }

        public class BaseCustomObject<TId>
        {
            public TId Id { get; set; }
        }

        public class CustomObject<T> : BaseCustomObject<int>
        {
            public T TestField { get; set; }
        }

        public class CustomObject<T, T2> : BaseCustomObject<int>
        {
            public T FirstField { get; set; }

            public T2 SecondField { get; set; }
        }

        public class Geometry
        {
            public int Id { get; set; }
        }

        public class Line : Geometry
        {
            public double Length { get; set; }
        }

        public class Circle : Geometry
        {
            public double Radius { get; set; }
        }
    }
}
