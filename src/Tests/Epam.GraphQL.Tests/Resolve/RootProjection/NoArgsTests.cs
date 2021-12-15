// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using GraphQL.Language.AST;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Resolve.RootProjection
{
    [TestFixtureSource(typeof(OperationFixtureArgCollection))]
    public class NoArgsTests : ResolveRootProjectionTestsBase
    {
        public NoArgsTests(OperationType operationType)
            : base(operationType)
        {
        }

        [Test(Description = "Field<int>(...).Resolve(_ => 10)")]
        public void ShouldResolveIntFieldWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int>(_ => 10);

            Test(
                queryBuilder: query => query
                    .Field("test")
                    .Resolve(resolver),
                mutationBuilder: mutation => mutation
                    .Field("test")
                    .Resolve(resolver),
                query: "test",
                expected: @"
                    {
                        test: 10
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<int>(...).Resolve(_ => Task 10)")]
        public void ShouldResolveTaskIntFieldWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, Task<int>>(async _ =>
            {
                await Task.Yield();
                return 10;
            });

            Test(
                queryBuilder: query => query
                    .Field("test")
                    .Resolve(resolver),
                mutationBuilder: mutation => mutation
                    .Field("test")
                    .Resolve(resolver),
                query: "test",
                expected: @"
                    {
                        test: 10
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<int?>(...).Resolve(_ => 10)")]
        public void ShouldResolveNullableIntFieldWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int?>(_ => 10);

            Test(
                queryBuilder: query => query
                    .Field("test")
                    .Resolve(resolver),
                mutationBuilder: mutation => mutation
                    .Field("test")
                    .Resolve(resolver),
                query: "test",
                expected: @"
                    {
                        test: 10
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<int?>(...).Resolve(_ => null)")]
        public void ShouldResolveNullableIntFieldReturningNullWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int?>(_ => null);

            Test(
                queryBuilder: query => query
                    .Field("test")
                    .Resolve(resolver),
                mutationBuilder: mutation => mutation
                    .Field("test")
                    .Resolve(resolver),
                query: "test",
                expected: @"
                    {
                        test: null
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<string>(...).Resolve(_ => \"test\")")]
        public void ShouldResolveStringWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, string>(_ => "Foo");

            Test(
                queryBuilder: query => query
                    .Field("test")
                    .Resolve(resolver),
                mutationBuilder: mutation => mutation
                    .Field("test")
                    .Resolve(resolver),
                query: "test",
                expected: @"
                    {
                        test: ""Foo""
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<CustomObject<int>>(...).Resolve(_ => new CustomObject<int> { TestField = 10 })")]
        public void ShouldResolveCustomObjectWithIntFieldWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, CustomObject<int>>(_ => CustomObject.Create(10));

            Test(
                queryBuilder: query => query
                    .Field("test")
                    .Resolve(resolver),
                mutationBuilder: mutation => mutation
                    .Field("test")
                    .Resolve(resolver),
                query: @"
                    test { 
                        testField
                    }",
                expected: @"
                    {
                        test: {
                            testField: 10 
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<CustomObject<int?>>(...).Resolve(_ => new CustomObject<int?> { TestField = 10 })")]
        public void ShouldResolveCustomObjectWithNullableIntFieldWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, CustomObject<int?>>(_ => CustomObject.Create<int?>(10));

            Test(
                queryBuilder: query => query
                    .Field("test")
                    .Resolve(resolver),
                mutationBuilder: mutation => mutation
                    .Field("test")
                    .Resolve(resolver),
                query: @"
                    test { 
                        testField
                    }",
                expected: @"
                    {
                        test: {
                            testField: 10
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<CustomObject<int?>>(...).Resolve(_ => new CustomObject<int?> { TestField = null })")]
        public void ShouldResolveCustomObjectWithNullableIntFieldReturningNullWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, CustomObject<int?>>(_ => CustomObject.Create<int?>(null));

            Test(
                queryBuilder: query => query
                    .Field("test")
                    .Resolve(resolver),
                mutationBuilder: mutation => mutation
                    .Field("test")
                    .Resolve(resolver),
                query: @"
                    test { 
                        testField
                    }",
                expected: @"
                    {
                        test: {
                            testField: null
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<CustomObject<string>>(...).Resolve(_ => new CustomObject<string> { TestField = \"test\" })")]
        public void ShouldResolveCustomObjectWithStringFieldWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, CustomObject<string>>(_ => CustomObject.Create("test"));

            Test(
                queryBuilder: query => query
                    .Field("test")
                    .Resolve(resolver),
                mutationBuilder: mutation => mutation
                    .Field("test")
                    .Resolve(resolver),
                query: @"
                    test { 
                        testField
                    }",
                expected: @"
                    {
                        test: {
                            testField: ""test""
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<CustomObject<int[]>>(...).Resolve(_ => new CustomObject<int[]> { ... })")]
        public void ShouldResolveCustomObjectWithIntArrayFieldWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, CustomObject<int[]>>(_ => CustomObject.Create(new[] { 10, 20 }));

            Test(
                queryBuilder: query => query
                    .Field("test")
                    .Resolve(resolver),
                mutationBuilder: mutation => mutation
                    .Field("test")
                    .Resolve(resolver),
                query: @"
                    test { 
                        testField
                    }",
                expected: @"
                    {
                        test: {
                            testField: [10, 20]
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<CustomObject<List<int>>>(...).Resolve(_ => new CustomObject<List<int>> { ... })")]
        public void ShouldResolveCustomObjectWithIntListFieldWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, CustomObject<List<int>>>(_ =>
                CustomObject.Create(new List<int> { 10, 20 }));

            Test(
                queryBuilder: query => query
                    .Field("test")
                    .Resolve(resolver),
                mutationBuilder: mutation => mutation
                    .Field("test")
                    .Resolve(resolver),
                query: @"
                    test { 
                        testField
                    }",
                expected: @"
                    {
                        test: {
                            testField: [10, 20]
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<IEnumerable<CustomObject<int>>>(...).Resolve(...)")]
        public void ShouldResolveClassWithIntFieldArrayResultWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, IEnumerable<CustomObject<int>>>(
                _ => new[] { 1, 2, 100500, 10 }.Select(n => new CustomObject<int> { TestField = n }));

            Test(
                queryBuilder: query => query
                    .Field("test")
                    .Resolve(resolver),
                mutationBuilder: mutation => mutation
                    .Field("test")
                    .Resolve(resolver),
                query: @"
                    test { 
                        testField
                    }",
                expected: @"
                    {
                        test: [{
                            testField: 1
                        },{
                            testField: 2
                        },{
                            testField: 100500
                        }, {
                            testField: 10
                        }]
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<IEnumerable<CustomObject<int>>>(...).Resolve(...) Task")]
        public void ShouldResolveTaskClassWithIntFieldArrayResultWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, Task<IEnumerable<CustomObject<int>>>>(async _ =>
            {
                await Task.Yield();
                return new[] { 1, 2, 100500, 10 }.Select(n => new CustomObject<int> { TestField = n });
            });

            Test(
                queryBuilder: query => query
                    .Field("test")
                    .Resolve(resolver),
                mutationBuilder: mutation => mutation
                    .Field("test")
                    .Resolve(resolver),
                query: @"
                    test { 
                        testField
                    }",
                expected: @"
                    {
                        test: [{
                            testField: 1
                        },{
                            testField: 2
                        },{
                            testField: 100500
                        }, {
                            testField: 10
                        }]
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<IEnumerable<CustomObject<CustomOnject<int>>>>(...).Resolve(...)")]
        public void ShouldResolveClassWithClassIntFieldArrayResultWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, IEnumerable<CustomObject<CustomObject<int>>>>(_ =>
                new[] { 1, 2, 100500, 10 }
                    .Select(n => new CustomObject<int> { TestField = n })
                    .Select(obj => new CustomObject<CustomObject<int>> { TestField = obj }));

            Test(
                queryBuilder: query => query
                    .Field("test")
                    .Resolve(resolver),
                mutationBuilder: mutation => mutation
                    .Field("test")
                    .Resolve(resolver),
                query: @"
                    test {
                        testField {
                            testField
                        }
                    }",
                expected: @"
                    {
                        test: [{
                            testField: {
                                testField: 1
                            }
                        },{
                            testField: {
                                testField: 2
                            }
                        },{
                            testField: {
                                testField: 100500
                            }
                        }, {
                            testField: {
                                testField: 10
                            }
                        }]
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<IEnumerable<int>>(...).Resolve(...)")]
        public void ShouldResolveIntArrayResultWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, IEnumerable<int>>(_ =>
                new[] { 1, 2, 100500, 10 });

            Test(
                queryBuilder: query => query
                    .Field("test")
                    .Resolve(resolver),
                mutationBuilder: mutation => mutation
                    .Field("test")
                    .Resolve(resolver),
                query: "test",
                expected: @"
                    {
                        test: [1, 2, 100500, 10]
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<IEnumerable<int>>(...).Resolve(...)")]
        public void ShouldResolveTaskIntArrayResultWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, Task<IEnumerable<int>>>(async _ =>
            {
                await Task.Yield();
                return new[] { 1, 2, 100500, 10 }.AsEnumerable();
            });

            Test(
                queryBuilder: query => query
                    .Field("test")
                    .Resolve(resolver),
                mutationBuilder: mutation => mutation
                    .Field("test")
                    .Resolve(resolver),
                query: "test",
                expected: @"
                    {
                        test: [1, 2, 100500, 10]
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<Geometry>(...).AsUnionOf<Line>(...).And<Circle>(...).Resolve(...)")]
        public void ShouldResolveUnionOfTwoTypesWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, Geometry>(_ => new Line
            {
                Id = 1,
                Length = 10,
            });

            Test(
                queryBuilder: query =>
                {
                    query.Field("test")
                        .AsUnionOf<Line>()
                            .And<Circle>()
                        .Resolve(resolver);
                },
                mutationBuilder: mutation =>
                {
                    mutation.Field("test")
                        .AsUnionOf<Line>()
                            .And<Circle>()
                        .Resolve(resolver);
                },
                query: @"
                    test {
                        ... on Line {
                            id
                            length
                        }
                    }",
                expected: @"
                    {
                        test: {
                            id: 1,
                            length: 10.0
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<Geometry>(...).AsUnionOf<Line>(...).And<Circle>(...).Resolve(...)")]
        public void ShouldResolveUnionOfTwoGenericTypesWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, BaseCustomObject<int>>(_ => CustomObject.Create("test"));

            Test(
                queryBuilder: query =>
                {
                    query.Field("test")
                        .AsUnionOf<CustomObject<string>>()
                            .And<CustomObject<string, int>>()
                        .Resolve(resolver);
                },
                mutationBuilder: mutation =>
                {
                    mutation.Field("test")
                        .AsUnionOf<CustomObject<string>>()
                            .And<CustomObject<string, int>>()
                        .Resolve(resolver);
                },
                query: @"
                    test {
                        ... on CustomObjectOfString {
                            testField
                        }
                    }",
                expected: @"
                    {
                        test: {
                            testField: ""test""
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<IEnumerable<Geometry>>(...).AsUnionOf<IEnumerable<Line>, Line>(...).And<IEnumerable<Circle>, Circle>(...).Resolve(...)")]
        public void ShouldResolveArrayOfUnionOfTwoTypesWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, IEnumerable<Geometry>>(_ => new Geometry[]
            {
                new Line
                {
                    Id = 1,
                    Length = 10,
                },
                new Circle
                {
                    Id = 2,
                    Radius = 5,
                },
            });

            Test(
                queryBuilder: query =>
                {
                    query.Field("test")
                        .AsUnionOf<IEnumerable<Line>, Line>()
                            .And<IEnumerable<Circle>, Circle>()
                        .Resolve(resolver);
                },
                mutationBuilder: mutation =>
                {
                    mutation.Field("test")
                        .AsUnionOf<IEnumerable<Line>, Line>()
                            .And<IEnumerable<Circle>, Circle>()
                        .Resolve(resolver);
                },
                query: @"
                    test {
                        ... on Line {
                            id
                            length
                        }
                        ... on Circle {
                            id
                            radius
                        }
                    }",
                expected: @"
                    {
                        test: [{
                            id: 1,
                            length: 10.0
                        },{
                            id: 2,
                            radius: 5.0
                        }]
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<IEnumerable<BaseCustomObject<int>>>(...).AsUnionOf<IEnumerable<CustomObject<string>>, CustomObject<string>>(...).And<IEnumerable<CustomObject<string, int>>, CustomObject<string, int>>(...).Resolve(...)")]
        public void ShouldResolveArrayOfUnionOfTwoGenericTypesWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, IEnumerable<BaseCustomObject<int>>>(_ => new BaseCustomObject<int>[]
            {
                CustomObject.Create("test"),
                CustomObject.Create("test", 1),
            });

            Test(
                queryBuilder: query =>
                {
                    query.Field("test")
                        .AsUnionOf<IEnumerable<CustomObject<string>>, CustomObject<string>>()
                            .And<IEnumerable<CustomObject<string, int>>, CustomObject<string, int>>()
                        .Resolve(resolver);
                },
                mutationBuilder: mutation =>
                {
                    mutation.Field("test")
                        .AsUnionOf<IEnumerable<CustomObject<string>>, CustomObject<string>>()
                            .And<IEnumerable<CustomObject<string, int>>, CustomObject<string, int>>()
                        .Resolve(resolver);
                },
                query: @"
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
                    }",
                expected: @"
                    {
                        test: [{
                            id: 0,
                            testField: ""test""
                        },{
                            id: 0,
                            firstField: ""test"",
                            secondField: 1
                        }]
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<IEnumerable<BaseCustomObject<int>>>(...).AsUnionOf<IEnumerable<CustomObject<string>>, CustomObject<string>>(...).And<IEnumerable<CustomObject<string, int>>, CustomObject<string, int>>(...).Resolve(...)")]
        public void ShouldResolveArrayOfUnionOfTwoGenericTypesWithBuildersWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, IEnumerable<BaseCustomObject<int>>>(_ => new BaseCustomObject<int>[]
            {
                CustomObject.Create("test"),
                CustomObject.Create("test", 1),
            });

            Test(
                queryBuilder: query =>
                {
                    query.Field("test")
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
                        .Resolve(resolver);
                },
                mutationBuilder: mutation =>
                {
                    mutation.Field("test")
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
                        .Resolve(resolver);
                },
                query: @"
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
                    }",
                expected: @"
                    {
                        test: [{
                            id: 0,
                            myField: ""test""
                        },{
                            id: 0,
                            firstField: ""test"",
                            secondField: 1
                        }]
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test]
        public void ShouldResolveClassWithIntFieldWithBuilder()
        {
            var resolver = FuncSubstitute.For<TestUserContext, CustomObject<int>>(_ => new CustomObject<int> { TestField = 100500 });

            Test(
                queryBuilder: query => query
                    .Field("test")
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                mutationBuilder: mutation => mutation
                    .Field("test")
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                query: @"
                    test {
                        myField
                    }",
                expected: @"
                    {
                        test: {
                            myField: 100500
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test]
        public void ShouldResolveTaskClassWithIntFieldWithBuilder()
        {
            var resolver = FuncSubstitute.For<TestUserContext, Task<CustomObject<int>>>(async _ =>
            {
                await Task.Yield();
                return new CustomObject<int> { TestField = 100500 };
            });

            Test(
                queryBuilder: query => query
                    .Field("test")
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                mutationBuilder: mutation => mutation
                    .Field("test")
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                query: @"
                    test {
                        myField
                    }",
                expected: @"
                    {
                        test: {
                            myField: 100500
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test]
        public void ShouldResolveClassWithIntFieldArrayResultWithBuilder()
        {
            var resolver = FuncSubstitute.For<TestUserContext, IEnumerable<CustomObject<int>>>(_ =>
                new[] { 1, 2, 100500, 10 }.Select(n => new CustomObject<int> { TestField = n }));

            Test(
                queryBuilder: query => query
                    .Field("test")
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                mutationBuilder: mutation => mutation
                    .Field("test")
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                query: @"
                    test {
                        myField
                    }",
                expected: @"
                    {
                        test: [{
                            myField: 1
                        },{
                            myField: 2
                        },{
                            myField: 100500
                        }, {
                            myField: 10
                        }]
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test]
        public void ShouldResolveTaskClassWithIntFieldArrayResultWithBuilder()
        {
            var resolver = FuncSubstitute.For<TestUserContext, Task<IEnumerable<CustomObject<int>>>>(async _ =>
            {
                await Task.Yield();
                return new[] { 1, 2, 100500, 10 }.Select(n => new CustomObject<int> { TestField = n });
            });

            Test(
                queryBuilder: query => query
                    .Field("test")
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                mutationBuilder: mutation => mutation
                    .Field("test")
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                query: @"
                    test {
                        myField
                    }",
                expected: @"
                    {
                        test: [{
                            myField: 1
                        },{
                            myField: 2
                        },{
                            myField: 100500
                        }, {
                            myField: 10
                        }]
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test]
        public void ShouldResolveFieldFromInterfacePropConfiguredFromLoader()
        {
            var loader = GraphQLTypeBuilder.CreateLoaderType<ITestUserContext, TestUserContext>(
                onConfigure: builder => builder.Field("id", _ => _.UserId),
                getBaseQuery: context => new[] { context }.AsQueryable(),
                applyNaturalOrderBy: q => q.OrderBy(c => c.UserId),
                applyNaturalThenBy: q => q.ThenBy(c => c.UserId));

            var resolver = FuncSubstitute.For<TestUserContext, ITestUserContext>(ctx => ctx);

            Test(
                queryBuilder: query => query
                    .Field("test")
                    .Resolve(resolver, b => b.ConfigureFrom(loader)),
                mutationBuilder: mutation => mutation
                    .Field("test")
                    .Resolve(resolver, b => b.ConfigureFrom(loader)),
                query: @"
                    test {
                        id
                    }",
                expected: @"
                    {
                        test: {
                            id: 5
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test]
        public void ShouldResolveFieldFromInterfacePropAutoConfigured()
        {
            var resolver = FuncSubstitute.For<TestUserContext, ITestUserContext>(ctx => ctx);

            Test(
                queryBuilder: query => query
                    .Field("test")
                    .Resolve(resolver),
                mutationBuilder: mutation => mutation
                    .Field("test")
                    .Resolve(resolver),
                query: @"
                    test {
                        userId
                    }",
                expected: @"
                    {
                        test: {
                            userId: 5
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test]
        public void ShouldResolveFieldFromInterfacePropConfiguredByHands()
        {
            var resolver = FuncSubstitute.For<TestUserContext, ITestUserContext>(ctx => ctx);

            Test(
                queryBuilder: query => query
                    .Field("test")
                    .Resolve(resolver, b => b.Field("id", ctx => ctx.UserId)),
                mutationBuilder: mutation => mutation
                    .Field("test")
                    .Resolve(resolver, b => b.Field("id", ctx => ctx.UserId)),
                query: @"
                    test {
                        id
                    }",
                expected: @"
                    {
                        test: {
                            id: 5
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }
    }
}
