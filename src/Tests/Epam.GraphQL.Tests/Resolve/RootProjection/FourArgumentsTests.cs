// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Mutation;
using Epam.GraphQL.Builders.Query;
using Epam.GraphQL.Builders.RootProjection;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using GraphQL.Language.AST;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Resolve.RootProjection
{
    [TestFixtureSource(typeof(OperationAndArgumentFixtureArgCollection))]
    public class FourArgumentsTests : ResolveRootProjectionTestsBase
    {
        private readonly ArgumentType _argumentType;

        public FourArgumentsTests(OperationType operationType, ArgumentType argumentType)
            : base(operationType)
        {
            _argumentType = argumentType;
        }

        [Test(Description = "Field<int>(...).Resolve(_ => 10)")]
        public void ShouldResolveIntFieldWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, int, int, int>((_, arg1, arg2, arg3, arg4) =>
                arg1 + arg2 + arg3 + arg4);

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int, int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int, int, int>(mutation)
                    .Resolve(resolver),
                query: $"test({BuildArguments(arg1: 10, arg2: 20, arg3: 30, arg4: 40)})",
                expected: @"
                    {
                        test: 100
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<int>(...).Resolve(_ => Task 10)")]
        public void ShouldResolveTaskIntFieldWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, int, int, Task<int>>(async (_, arg1, arg2, arg3, arg4) =>
            {
                await Task.Yield();
                return arg1 + arg2 + arg3 + arg4;
            });

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int, int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int, int, int>(mutation)
                    .Resolve(resolver),
                query: $"test({BuildArguments(arg1: 10, arg2: 20, arg3: 30, arg4: 40)})",
                expected: @"
                    {
                        test: 100
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<int?>(...).Resolve(_ => 10)")]
        public void ShouldResolveNullableIntFieldWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, int, int, int?>((_, arg1, arg2, arg3, arg4) =>
                arg1 + arg2 + arg3 + arg4);

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int, int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int, int, int>(mutation)
                    .Resolve(resolver),
                query: $"test({BuildArguments(arg1: 10, arg2: 20, arg3: 30, arg4: 40)})",
                expected: @"
                    {
                        test: 100
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<int?>(...).Resolve(_ => null)")]
        public void ShouldResolveNullableIntFieldReturningNullWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, int, int, int?>((_, arg1, arg2, arg3, arg4) => null);

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int, int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int, int, int>(mutation)
                    .Resolve(resolver),
                query: $"test({BuildArguments(arg1: 10, arg2: 20, arg3: 30, arg4: 40)})",
                expected: @"
                    {
                        test: null
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<string>(...).Resolve(_ => \"test\")")]
        public void ShouldResolveStringWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, string, string, string, string, string>((_, arg1, arg2, arg3, arg4) =>
                arg1 + arg2 + arg3 + arg4);

            Test(
                queryBuilder: query => CreateArgumentBuilder<string, string, string, string>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<string, string, string, string>(mutation)
                    .Resolve(resolver),
                query: $"test({BuildArguments(arg1: "Foo", arg2: "Bar", arg3: "Baz", arg4: "Enterprise")})",
                expected: @"
                    {
                        test: ""FooBarBazEnterprise""
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<CustomObject<int>>(...).Resolve(_ => new CustomObject<int> { TestField = 10 })")]
        public void ShouldResolveCustomObjectWithIntFieldWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, int, int, CustomObject<int>>((_, arg1, arg2, arg3, arg4) =>
                CustomObject.Create(arg1 + arg2 + arg3 + arg4));

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int, int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int, int, int>(mutation)
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(arg1: 10, arg2: 20, arg3: 30, arg4: 40)}) {{
                        testField
                    }}",
                expected: @"
                    {
                        test: {
                            testField: 100 
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<CustomObject<int?>>(...).Resolve(_ => new CustomObject<int?> { TestField = 10 })")]
        public void ShouldResolveCustomObjectWithNullableIntFieldWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, int, int, CustomObject<int?>>((_, arg1, arg2, arg3, arg4) =>
                CustomObject.Create<int?>(arg1 + arg2 + arg3 + arg4));

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int, int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int, int, int>(mutation)
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(arg1: 10, arg2: 20, arg3: 30, arg4: 40)}) {{
                        testField
                    }}",
                expected: @"
                    {
                        test: {
                            testField: 100
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<CustomObject<int?>>(...).Resolve(_ => new CustomObject<int?> { TestField = null })")]
        public void ShouldResolveCustomObjectWithNullableIntFieldReturningNullWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, int, int, CustomObject<int?>>((_, arg1, arg2, arg3, arg4) =>
                CustomObject.Create<int?>(null));

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int, int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int, int, int>(mutation)
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(arg1: 10, arg2: 20, arg3: 30, arg4: 40)}) {{
                        testField
                    }}",
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
            var resolver = FuncSubstitute.For<TestUserContext, string, string, string, string, CustomObject<string>>((_, arg1, arg2, arg3, arg4) =>
                CustomObject.Create(arg1 + arg2 + arg3 + arg4));

            Test(
                queryBuilder: query => CreateArgumentBuilder<string, string, string, string>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<string, string, string, string>(mutation)
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(arg1: "test", arg2: "test2", arg3: "test3", arg4: "test4")}) {{
                        testField
                    }}",
                expected: @"
                    {
                        test: {
                            testField: ""testtest2test3test4""
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<CustomObject<int[]>>(...).Resolve(_ => new CustomObject<int[]> { ... })")]
        public void ShouldResolveCustomObjectWithIntArrayFieldWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, int, int, CustomObject<int[]>>((_, arg1, arg2, arg3, arg4) =>
                CustomObject.Create(new[] { 10, arg1 + arg2 + arg3 + arg4 }));

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int, int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int, int, int>(mutation)
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(arg1: 20, arg2: 40, arg3: 60, arg4: 100)}) {{
                        testField
                    }}",
                expected: @"
                    {
                        test: {
                            testField: [10, 220]
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<CustomObject<List<int>>>(...).Resolve(_ => new CustomObject<List<int>> { ... })")]
        public void ShouldResolveCustomObjectWithIntListFieldWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, int, int, CustomObject<List<int>>>((_, arg1, arg2, arg3, arg4) =>
                CustomObject.Create(new List<int> { 10, arg1 + arg2 + arg3 + arg4 }));

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int, int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int, int, int>(mutation)
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(arg1: 20, arg2: 40, arg3: 60, arg4: 100)}) {{
                        testField
                    }}",
                expected: @"
                    {
                        test: {
                            testField: [10, 220]
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<IEnumerable<CustomObject<int>>>(...).Resolve(...)")]
        public void ShouldResolveClassWithIntFieldArrayResultWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, int, int, IEnumerable<CustomObject<int>>>((_, arg1, arg2, arg3, arg4) =>
                new[] { arg1 + arg2 + arg3 + arg4, 2, 100500, 10 }.Select(n => new CustomObject<int> { TestField = n }));

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int, int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int, int, int>(mutation)
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(arg1: 1, arg2: 2, arg3: 3, arg4: 4)}) {{
                        testField
                    }}",
                expected: @"
                    {
                        test: [{
                            testField: 10
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
            var resolver = FuncSubstitute.For<TestUserContext, int, int, int, int, Task<IEnumerable<CustomObject<int>>>>(async (_, arg1, arg2, arg3, arg4) =>
            {
                await Task.Yield();
                return new[] { arg1 + arg2 + arg3 + arg4, 2, 100500, 10 }.Select(n => new CustomObject<int> { TestField = n });
            });

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int, int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int, int, int>(mutation)
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(arg1: 1, arg2: 2, arg3: 3, arg4: 4)}) {{
                        testField
                    }}",
                expected: @"
                    {
                        test: [{
                            testField: 10
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
            var resolver = FuncSubstitute.For<TestUserContext, int, int, int, int, IEnumerable<CustomObject<CustomObject<int>>>>((_, arg1, arg2, arg3, arg4) =>
                new[] { arg1 + arg2 + arg3 + arg4, 2, 100500, 10 }
                    .Select(n => new CustomObject<int> { TestField = n })
                    .Select(obj => new CustomObject<CustomObject<int>> { TestField = obj }));

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int, int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int, int, int>(mutation)
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(arg1: 1, arg2: 2, arg3: 3, arg4: 4)}) {{
                        testField {{
                            testField
                        }}
                    }}",
                expected: @"
                    {
                        test: [{
                            testField: {
                                testField: 10
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
            var resolver = FuncSubstitute.For<TestUserContext, int, int, int, int, IEnumerable<int>>((_, arg1, arg2, arg3, arg4) =>
                new[] { arg1 + arg2 + arg3 + arg4, 2, 100500, 10 });

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int, int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int, int, int>(mutation)
                    .Resolve(resolver),
                query: $"test({BuildArguments(arg1: 1, arg2: 2, arg3: 3, arg4: 4)})",
                expected: @"
                    {
                        test: [10, 2, 100500, 10]
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<IEnumerable<int>>(...).Resolve(...)")]
        public void ShouldResolveTaskIntArrayResultWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, int, int, Task<IEnumerable<int>>>(async (_, arg1, arg2, arg3, arg4) =>
            {
                await Task.Yield();
                return new[] { arg1 + arg2 + arg3 + arg4, 2, 100500, 10 }.AsEnumerable();
            });

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int, int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int, int, int>(mutation)
                    .Resolve(resolver),
                query: $"test({BuildArguments(arg1: 1, arg2: 2, arg3: 3, arg4: 4)})",
                expected: @"
                    {
                        test: [10, 2, 100500, 10]
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<Geometry>(...).AsUnionOf<Line>(...).And<Circle>(...).Resolve(...)")]
        public void ShouldResolveUnionOfTwoTypesWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, int, int, Geometry>((_, arg1, arg2, arg3, arg4) =>
                new Line
                {
                    Id = 1,
                    Length = arg1 + arg2 + arg3 + arg4,
                });

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int, int, int>(query)
                    .AsUnionOf<Line>()
                        .And<Circle>()
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int, int, int>(mutation)
                    .AsUnionOf<Line>()
                        .And<Circle>()
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(arg1: 10, arg2: 20, arg3: 30, arg4: 40)}) {{
                        ... on Line {{
                            id
                            length
                        }}
                    }}",
                expected: @"
                    {
                        test: {
                            id: 1,
                            length: 100.0
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<Geometry>(...).AsUnionOf<Line>(...).And<Circle>(...).Resolve(...)")]
        public void ShouldResolveUnionOfTwoGenericTypesWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, string, string, string, string, BaseCustomObject<int>>((_, arg1, arg2, arg3, arg4) =>
                CustomObject.Create(arg1 + arg2 + arg3 + arg4));

            Test(
                queryBuilder: query => CreateArgumentBuilder<string, string, string, string>(query)
                    .AsUnionOf<CustomObject<string>>()
                        .And<CustomObject<string, int>>()
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<string, string, string, string>(mutation)
                    .AsUnionOf<CustomObject<string>>()
                        .And<CustomObject<string, int>>()
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(arg1: "test", arg2: "test2", arg3: "test3", arg4: "test4")}) {{
                        ... on CustomObjectOfString {{
                            testField
                        }}
                    }}",
                expected: @"
                    {
                        test: {
                            testField: ""testtest2test3test4""
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<IEnumerable<Geometry>>(...).AsUnionOf<IEnumerable<Line>, Line>(...).And<IEnumerable<Circle>, Circle>(...).Resolve(...)")]
        public void ShouldResolveArrayOfUnionOfTwoTypesWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, int, int, IEnumerable<Geometry>>((_, arg1, arg2, arg3, arg4) =>
                new Geometry[]
                {
                    new Line
                    {
                        Id = 1,
                        Length = arg1 + arg2 + arg3 + arg4,
                    },
                    new Circle
                    {
                        Id = 2,
                        Radius = arg1 + arg2 + arg3 + arg4,
                    },
                });

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int, int, int>(query)
                    .AsUnionOf<IEnumerable<Line>, Line>()
                        .And<IEnumerable<Circle>, Circle>()
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int, int, int>(mutation)
                    .AsUnionOf<IEnumerable<Line>, Line>()
                        .And<IEnumerable<Circle>, Circle>()
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(arg1: 10, arg2: 20, arg3: 30, arg4: 40)}) {{
                        ... on Line {{
                            id
                            length
                        }}
                        ... on Circle {{
                            id
                            radius
                        }}
                    }}",
                expected: @"
                    {
                        test: [{
                            id: 1,
                            length: 100.0
                        },{
                            id: 2,
                            radius: 100.0
                        }]
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<IEnumerable<BaseCustomObject<int>>>(...).AsUnionOf<IEnumerable<CustomObject<string>>, CustomObject<string>>(...).And<IEnumerable<CustomObject<string, int>>, CustomObject<string, int>>(...).Resolve(...)")]
        public void ShouldResolveArrayOfUnionOfTwoGenericTypesWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, string, string, string, string, IEnumerable<BaseCustomObject<int>>>((_, arg1, arg2, arg3, arg4) =>
                new BaseCustomObject<int>[]
                {
                    CustomObject.Create(arg1 + arg2 + arg3 + arg4),
                    CustomObject.Create(arg1 + arg2 + arg3 + arg4, 1),
                });

            Test(
                queryBuilder: query => CreateArgumentBuilder<string, string, string, string>(query)
                    .AsUnionOf<IEnumerable<CustomObject<string>>, CustomObject<string>>()
                        .And<IEnumerable<CustomObject<string, int>>, CustomObject<string, int>>()
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<string, string, string, string>(mutation)
                    .AsUnionOf<IEnumerable<CustomObject<string>>, CustomObject<string>>()
                        .And<IEnumerable<CustomObject<string, int>>, CustomObject<string, int>>()
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(arg1: "test", arg2: "test2", arg3: "test3", arg4: "test4")}) {{
                        ... on CustomObjectOfString {{
                            id
                            testField
                        }}
                        ... on CustomObjectOfStringAndInt {{
                            id
                            firstField
                            secondField
                        }}
                    }}",
                expected: @"
                    {
                        test: [{
                            id: 0,
                            testField: ""testtest2test3test4""
                        },{
                            id: 0,
                            firstField: ""testtest2test3test4"",
                            secondField: 1
                        }]
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<IEnumerable<BaseCustomObject<int>>>(...).AsUnionOf<IEnumerable<CustomObject<string>>, CustomObject<string>>(...).And<IEnumerable<CustomObject<string, int>>, CustomObject<string, int>>(...).Resolve(...)")]
        public void ShouldResolveArrayOfUnionOfTwoGenericTypesWithBuildersWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, string, string, string, string, IEnumerable<BaseCustomObject<int>>>((_, arg1, arg2, arg3, arg4) =>
                new BaseCustomObject<int>[]
                {
                    CustomObject.Create(arg1 + arg2 + arg3 + arg4),
                    CustomObject.Create(arg1 + arg2 + arg3 + arg4, 1),
                });

            Test(
                queryBuilder: query => CreateArgumentBuilder<string, string, string, string>(query)
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
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<string, string, string, string>(mutation)
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
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(arg1: "test", arg2: "test2", arg3: "test3", arg4: "test4")}) {{
                        ... on CustomObject1 {{
                            id
                            myField
                        }}
                        ... on CustomObject2 {{
                            id
                            firstField
                            secondField
                        }}
                    }}",
                expected: @"
                    {
                        test: [{
                            id: 0,
                            myField: ""testtest2test3test4""
                        },{
                            id: 0,
                            firstField: ""testtest2test3test4"",
                            secondField: 1
                        }]
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test]
        public void ShouldResolveClassWithIntFieldWithBuilder()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, int, int, CustomObject<int>>((_, arg1, arg2, arg3, arg4) =>
                new CustomObject<int> { TestField = arg1 + arg2 + arg3 + arg4 });

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int, int, int>(query)
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int, int, int>(mutation)
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                query: $@"
                    test({BuildArguments(arg1: 100500, arg2: 1, arg3: 2, arg4: 3)}) {{
                        myField
                    }}",
                expected: @"
                    {
                        test: {
                            myField: 100506
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test]
        public void ShouldResolveTaskClassWithIntFieldWithBuilder()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, int, int, Task<CustomObject<int>>>(async (_, arg1, arg2, arg3, arg4) =>
            {
                await Task.Yield();
                return new CustomObject<int> { TestField = arg1 + arg2 + arg3 + arg4 };
            });

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int, int, int>(query)
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int, int, int>(mutation)
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                query: $@"
                    test({BuildArguments(arg1: 100500, arg2: 1, arg3: 2, arg4: 3)}) {{
                        myField
                    }}",
                expected: @"
                    {
                        test: {
                            myField: 100506
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test]
        public void ShouldResolveClassWithIntFieldArrayResultWithBuilder()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, int, int, IEnumerable<CustomObject<int>>>((_, arg1, arg2, arg3, arg4) =>
                new[] { arg1 + arg2 + arg3 + arg4, 2, 100500, 10 }.Select(n => new CustomObject<int> { TestField = n }));

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int, int, int>(query)
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int, int, int>(mutation)
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                query: $@"
                    test({BuildArguments(arg1: 100500, arg2: 1, arg3: 2, arg4: 3)}) {{
                        myField
                    }}",
                expected: @"
                    {
                        test: [{
                            myField: 100506
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
            var resolver = FuncSubstitute.For<TestUserContext, int, int, int, int, Task<IEnumerable<CustomObject<int>>>>(async (_, arg1, arg2, arg3, arg4) =>
            {
                await Task.Yield();
                return new[] { arg1 + arg2 + arg3 + arg4, 2, 100500, 10 }.Select(n => new CustomObject<int> { TestField = n });
            });

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int, int, int>(query)
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int, int, int>(mutation)
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                query: $@"
                    test({BuildArguments(arg1: 100500, arg2: 1, arg3: 2, arg4: 3)}) {{
                        myField
                    }}",
                expected: @"
                    {
                        test: [{
                            myField: 100506
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

        /// <summary>
        /// Test for https://git.epam.com/epm-ppa/epam-graphql/-/issues/100.
        /// </summary>
        [Test]
        public void ShouldResolveWithEmptyFilterArg()
        {
            var loader = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id).Filterable();
                    loader.Field(p => p.FullName).Filterable();
                },
                getBaseQuery: _ => throw new NotImplementedException(),
                applyNaturalOrderBy: _ => throw new NotImplementedException(),
                applyNaturalThenBy: _ => throw new NotImplementedException());

            var resolver = FuncSubstitute.For<TestUserContext, int, int, int, Expression<Func<Person, bool>>, int>((_, arg1, arg2, arg3, filter) => 10);

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int, int, Person>(loader, query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int, int, Person>(loader, mutation)
                    .Resolve(resolver),
                query: $"test({BuildFilterArguments(1, 2, 3)})",
                expected: @"
                    {
                        test: 10
                    }");

            resolver.HasBeenCalledOnce(arg5: filter => ExpressionEqualityComparer.Instance.Equals(filter, FuncConstants<Person>.TrueExpression));
        }

        /// <summary>
        /// Test for https://git.epam.com/epm-ppa/epam-graphql/-/issues/100.
        /// </summary>
        [Test]
        public void ShouldResolveWithFilterArg()
        {
            var loader = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id).Filterable();
                    loader.Field(p => p.FullName).Filterable();
                },
                getBaseQuery: _ => throw new NotImplementedException(),
                applyNaturalOrderBy: _ => throw new NotImplementedException(),
                applyNaturalThenBy: _ => throw new NotImplementedException());

            var resolver = FuncSubstitute.For<TestUserContext, int, int, int, Expression<Func<Person, bool>>, int>((_, arg1, arg2, arg3, filter) => 10);

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int, int, Person>(loader, query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int, int, Person>(loader, mutation)
                    .Resolve(resolver),
                query: $"test({BuildFilterArguments(1, 2, 3, "{ or: [{ id: { eq: 1} }, { fullName: { isNull: true } }] }")})",
                expected: @"
                    {
                        test: 10
                    }");

            Expression<Func<Person, bool>> expected = p => p.Id == 1 || p.FullName == null;
            resolver.HasBeenCalledOnce(arg5: filter => ExpressionEqualityComparer.Instance.Equals(filter, expected));
        }

        private string BuildArguments(int arg1, int arg2, int arg3, int arg4)
        {
            return _argumentType switch
            {
                ArgumentType.Argument => $"arg1: {arg1}, arg2: {arg2}, arg3: {arg3}, arg4: {arg4}",
                ArgumentType.PayloadField => $"payload: {{ arg1: {arg1}, arg2: {arg2}, arg3: {arg3}, arg4: {arg4} }}",
                _ => throw new NotSupportedException(),
            };
        }

        private string BuildArguments(string arg1, string arg2, string arg3, string arg4)
        {
            return _argumentType switch
            {
                ArgumentType.Argument => $"arg1: \"{arg1}\", arg2: \"{arg2}\", arg3: \"{arg3}\", arg4: \"{arg4}\"",
                ArgumentType.PayloadField => $"payload: {{ arg1: \"{arg1}\", arg2: \"{arg2}\", arg3: \"{arg3}\", arg4: \"{arg4}\" }}",
                _ => throw new NotSupportedException(),
            };
        }

        private string BuildFilterArguments(int arg1, int arg2, int arg3, string filter = null)
        {
            var arg4 = filter == null ? string.Empty : $", arg4: {filter}";

            return _argumentType switch
            {
                ArgumentType.Argument => $"arg1: {arg1}, arg2: {arg2}, arg3: {arg3}{arg4}",
                ArgumentType.PayloadField => $"payload: {{ arg1: {arg1}, arg2: {arg2}, arg3: {arg3}{arg4} }}",
                _ => throw new NotSupportedException(),
            };
        }

        private IQueryFieldBuilder<IRootProjectionFieldBuilder<TArg1, TArg2, TArg3, TArg4, TestUserContext>, TArg1, TArg2, TArg3, TArg4, TestUserContext> CreateArgumentBuilder<TArg1, TArg2, TArg3, TArg4>(Query<TestUserContext> query)
        {
            return _argumentType switch
            {
                ArgumentType.Argument => query.Field("test")
                    .Argument<TArg1>("arg1")
                    .Argument<TArg2>("arg2")
                    .Argument<TArg3>("arg3")
                    .Argument<TArg4>("arg4"),
                ArgumentType.PayloadField => query.Field("test")
                    .PayloadField<TArg1>("arg1")
                    .PayloadField<TArg2>("arg2")
                    .PayloadField<TArg3>("arg3")
                    .PayloadField<TArg4>("arg4"),
                _ => throw new NotSupportedException(),
            };
        }

        private IMutationFieldBuilder<IMutationFieldBuilderBase<TArg1, TArg2, TArg3, TArg4, TestUserContext>, TArg1, TArg2, TArg3, TArg4, TestUserContext> CreateArgumentBuilder<TArg1, TArg2, TArg3, TArg4>(Mutation<TestUserContext> mutation)
        {
            return _argumentType switch
            {
                ArgumentType.Argument => mutation.Field("test")
                    .Argument<TArg1>("arg1")
                    .Argument<TArg2>("arg2")
                    .Argument<TArg3>("arg3")
                    .Argument<TArg4>("arg4"),
                ArgumentType.PayloadField => mutation.Field("test")
                    .PayloadField<TArg1>("arg1")
                    .PayloadField<TArg2>("arg2")
                    .PayloadField<TArg3>("arg3")
                    .PayloadField<TArg4>("arg4"),
                _ => throw new NotSupportedException(),
            };
        }

        private IQueryFieldBuilder<IRootProjectionFieldBuilder<TArg1, TArg2, TArg3, Expression<Func<TEntity, bool>>, TestUserContext>, TArg1, TArg2, TArg3, Expression<Func<TEntity, bool>>, TestUserContext> CreateArgumentBuilder<TArg1, TArg2, TArg3, TEntity>(Type loaderType, Query<TestUserContext> query)
            where TEntity : class
        {
            return _argumentType switch
            {
                ArgumentType.Argument => query.Field("test")
                    .Argument<TArg1>("arg1")
                    .Argument<TArg2>("arg2")
                    .Argument<TArg3>("arg3")
                    .FilterArgument<TEntity>(loaderType, "arg4"),
                ArgumentType.PayloadField => query.Field("test")
                    .PayloadField<TArg1>("arg1")
                    .PayloadField<TArg2>("arg2")
                    .PayloadField<TArg3>("arg3")
                    .FilterPayloadField<TEntity>(loaderType, "arg4"),
                _ => throw new NotSupportedException(),
            };
        }

        private IMutationFieldBuilder<IMutationFieldBuilderBase<TArg1, TArg2, TArg3, Expression<Func<TEntity, bool>>, TestUserContext>, TArg1, TArg2, TArg3, Expression<Func<TEntity, bool>>, TestUserContext> CreateArgumentBuilder<TArg1, TArg2, TArg3, TEntity>(Type loaderType, Mutation<TestUserContext> mutation)
            where TEntity : class
        {
            return _argumentType switch
            {
                ArgumentType.Argument => mutation.Field("test")
                    .Argument<TArg1>("arg1")
                    .Argument<TArg2>("arg2")
                    .Argument<TArg3>("arg3")
                    .FilterArgument<TEntity>(loaderType, "arg4"),
                ArgumentType.PayloadField => mutation.Field("test")
                    .PayloadField<TArg1>("arg1")
                    .PayloadField<TArg2>("arg2")
                    .PayloadField<TArg3>("arg3")
                    .FilterPayloadField<TEntity>(loaderType, "arg4"),
                _ => throw new NotSupportedException(),
            };
        }
    }
}
