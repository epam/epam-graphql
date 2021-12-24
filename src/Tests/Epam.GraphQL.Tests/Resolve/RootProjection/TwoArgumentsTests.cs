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
    public class TwoArgumentsTests : ResolveRootProjectionTestsBase
    {
        private readonly ArgumentType _argumentType;

        public TwoArgumentsTests(OperationType operationType, ArgumentType argumentType)
            : base(operationType)
        {
            _argumentType = argumentType;
        }

        [Test(Description = "Field<int>(...).Resolve(_ => 10)")]
        public void ShouldResolveIntFieldWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, int>((_, arg1, arg2) =>
                arg1 + arg2);

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int>(mutation)
                    .Resolve(resolver),
                query: $"test({BuildArguments(arg1: 10, arg2: 20)})",
                expected: @"
                    {
                        test: 30
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<int>(...).Resolve(_ => Task 10)")]
        public void ShouldResolveTaskIntFieldWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, Task<int>>(async (_, arg1, arg2) =>
            {
                await Task.Yield();
                return arg1 + arg2;
            });

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int>(mutation)
                    .Resolve(resolver),
                query: $"test({BuildArguments(arg1: 10, arg2: 20)})",
                expected: @"
                    {
                        test: 30
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<int?>(...).Resolve(_ => 10)")]
        public void ShouldResolveNullableIntFieldWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, int?>((_, arg1, arg2) =>
                arg1 + arg2);

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int>(mutation)
                    .Resolve(resolver),
                query: $"test({BuildArguments(arg1: 10, arg2: 20)})",
                expected: @"
                    {
                        test: 30
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<int?>(...).Resolve(_ => null)")]
        public void ShouldResolveNullableIntFieldReturningNullWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, int?>((_, arg1, arg2) => null);

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int>(mutation)
                    .Resolve(resolver),
                query: $"test({BuildArguments(arg1: 10, arg2: 20)})",
                expected: @"
                    {
                        test: null
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<string>(...).Resolve(_ => \"test\")")]
        public void ShouldResolveStringWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, string, string, string>((_, arg1, arg2) =>
                arg1 + arg2);

            Test(
                queryBuilder: query => CreateArgumentBuilder<string, string>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<string, string>(mutation)
                    .Resolve(resolver),
                query: $"test({BuildArguments(arg1: "Foo", arg2: "Bar")})",
                expected: @"
                    {
                        test: ""FooBar""
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<CustomObject<int>>(...).Resolve(_ => new CustomObject<int> { TestField = 10 })")]
        public void ShouldResolveCustomObjectWithIntFieldWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, CustomObject<int>>((_, arg1, arg2) =>
                CustomObject.Create(arg1 + arg2));

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int>(mutation)
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(arg1: 10, arg2: 20)}) {{
                        testField
                    }}",
                expected: @"
                    {
                        test: {
                            testField: 30 
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<CustomObject<int?>>(...).Resolve(_ => new CustomObject<int?> { TestField = 10 })")]
        public void ShouldResolveCustomObjectWithNullableIntFieldWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, CustomObject<int?>>((_, arg1, arg2) =>
                CustomObject.Create<int?>(arg1 + arg2));

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int>(mutation)
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(arg1: 10, arg2: 20)}) {{
                        testField
                    }}",
                expected: @"
                    {
                        test: {
                            testField: 30
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<CustomObject<int?>>(...).Resolve(_ => new CustomObject<int?> { TestField = null })")]
        public void ShouldResolveCustomObjectWithNullableIntFieldReturningNullWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, CustomObject<int?>>((_, arg1, arg2) =>
                CustomObject.Create<int?>(null));

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int>(mutation)
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(arg1: 10, arg2: 20)}) {{
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
            var resolver = FuncSubstitute.For<TestUserContext, string, string, CustomObject<string>>((_, arg1, arg2) =>
                CustomObject.Create(arg1 + arg2));

            Test(
                queryBuilder: query => CreateArgumentBuilder<string, string>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<string, string>(mutation)
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(arg1: "test", arg2: "test2")}) {{
                        testField
                    }}",
                expected: @"
                    {
                        test: {
                            testField: ""testtest2""
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<CustomObject<int[]>>(...).Resolve(_ => new CustomObject<int[]> { ... })")]
        public void ShouldResolveCustomObjectWithIntArrayFieldWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, CustomObject<int[]>>((_, arg1, arg2) =>
                CustomObject.Create(new[] { 10, arg1 + arg2 }));

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int>(mutation)
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(arg1: 20, arg2: 40)}) {{
                        testField
                    }}",
                expected: @"
                    {
                        test: {
                            testField: [10, 60]
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<CustomObject<List<int>>>(...).Resolve(_ => new CustomObject<List<int>> { ... })")]
        public void ShouldResolveCustomObjectWithIntListFieldWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, CustomObject<List<int>>>((_, arg1, arg2) =>
                CustomObject.Create(new List<int> { 10, arg1 + arg2 }));

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int>(mutation)
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(arg1: 20, arg2: 40)}) {{
                        testField
                    }}",
                expected: @"
                    {
                        test: {
                            testField: [10, 60]
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<IEnumerable<CustomObject<int>>>(...).Resolve(...)")]
        public void ShouldResolveClassWithIntFieldArrayResultWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, IEnumerable<CustomObject<int>>>((_, arg1, arg2) =>
                new[] { arg1 + arg2, 2, 100500, 10 }.Select(n => new CustomObject<int> { TestField = n }));

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int>(mutation)
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(arg1: 1, arg2: 2)}) {{
                        testField
                    }}",
                expected: @"
                    {
                        test: [{
                            testField: 3
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
            var resolver = FuncSubstitute.For<TestUserContext, int, int, Task<IEnumerable<CustomObject<int>>>>(async (_, arg1, arg2) =>
            {
                await Task.Yield();
                return new[] { arg1 + arg2, 2, 100500, 10 }.Select(n => new CustomObject<int> { TestField = n });
            });

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int>(mutation)
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(arg1: 1, arg2: 2)}) {{
                        testField
                    }}",
                expected: @"
                    {
                        test: [{
                            testField: 3
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
            var resolver = FuncSubstitute.For<TestUserContext, int, int, IEnumerable<CustomObject<CustomObject<int>>>>((_, arg1, arg2) =>
                new[] { arg1 + arg2, 2, 100500, 10 }
                .Select(n => new CustomObject<int> { TestField = n })
                .Select(obj => new CustomObject<CustomObject<int>> { TestField = obj }));

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int>(mutation)
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(arg1: 1, arg2: 2)}) {{
                        testField {{
                            testField
                        }}
                    }}",
                expected: @"
                    {
                        test: [{
                            testField: {
                                testField: 3
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
            var resolver = FuncSubstitute.For<TestUserContext, int, int, IEnumerable<int>>((_, arg1, arg2) =>
                new[] { arg1 + arg2, 2, 100500, 10 });

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int>(mutation)
                    .Resolve(resolver),
                query: $"test({BuildArguments(arg1: 1, arg2: 2)})",
                expected: @"
                    {
                        test: [3, 2, 100500, 10]
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<IEnumerable<int>>(...).Resolve(...)")]
        public void ShouldResolveTaskIntArrayResultWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, Task<IEnumerable<int>>>(async (_, arg1, arg2) =>
            {
                await Task.Yield();
                return new[] { arg1 + arg2, 2, 100500, 10 }.AsEnumerable();
            });

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int>(mutation)
                    .Resolve(resolver),
                query: $"test({BuildArguments(arg1: 1, arg2: 2)})",
                expected: @"
                    {
                        test: [3, 2, 100500, 10]
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<Geometry>(...).AsUnionOf<Line>(...).And<Circle>(...).Resolve(...)")]
        public void ShouldResolveUnionOfTwoTypesWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, Geometry>((_, arg1, arg2) =>
                new Line
                {
                    Id = 1,
                    Length = arg1 + arg2,
                });

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int>(query)
                    .AsUnionOf<Line>()
                        .And<Circle>()
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int>(mutation)
                    .AsUnionOf<Line>()
                        .And<Circle>()
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(arg1: 10, arg2: 20)}) {{
                        ... on Line {{
                            id
                            length
                        }}
                    }}",
                expected: @"
                    {
                        test: {
                            id: 1,
                            length: 30.0
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<Geometry>(...).AsUnionOf<Line>(...).And<Circle>(...).Resolve(...)")]
        public void ShouldResolveUnionOfTwoGenericTypesWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, string, string, BaseCustomObject<int>>((_, arg1, arg2) =>
                CustomObject.Create(arg1 + arg2));

            Test(
                queryBuilder: query => CreateArgumentBuilder<string, string>(query)
                    .AsUnionOf<CustomObject<string>>()
                        .And<CustomObject<string, int>>()
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<string, string>(mutation)
                    .AsUnionOf<CustomObject<string>>()
                        .And<CustomObject<string, int>>()
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(arg1: "test", arg2: "test2")}) {{
                        ... on CustomObjectOfString {{
                            testField
                        }}
                    }}",
                expected: @"
                    {
                        test: {
                            testField: ""testtest2""
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<IEnumerable<Geometry>>(...).AsUnionOf<IEnumerable<Line>, Line>(...).And<IEnumerable<Circle>, Circle>(...).Resolve(...)")]
        public void ShouldResolveArrayOfUnionOfTwoTypesWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, IEnumerable<Geometry>>((_, arg1, arg2) =>
                new Geometry[]
                {
                    new Line
                    {
                        Id = 1,
                        Length = arg1 + arg2,
                    },
                    new Circle
                    {
                        Id = 2,
                        Radius = arg1 + arg2,
                    },
                });

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int>(query)
                    .AsUnionOf<IEnumerable<Line>, Line>()
                        .And<IEnumerable<Circle>, Circle>()
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int>(mutation)
                    .AsUnionOf<IEnumerable<Line>, Line>()
                        .And<IEnumerable<Circle>, Circle>()
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(arg1: 10, arg2: 20)}) {{
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
                            length: 30.0
                        },{
                            id: 2,
                            radius: 30.0
                        }]
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<IEnumerable<BaseCustomObject<int>>>(...).AsUnionOf<IEnumerable<CustomObject<string>>, CustomObject<string>>(...).And<IEnumerable<CustomObject<string, int>>, CustomObject<string, int>>(...).Resolve(...)")]
        public void ShouldResolveArrayOfUnionOfTwoGenericTypesWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, string, string, IEnumerable<BaseCustomObject<int>>>((_, arg1, arg2) =>
                new BaseCustomObject<int>[]
                {
                    CustomObject.Create(arg1 + arg2),
                    CustomObject.Create(arg1 + arg2, 1),
                });

            Test(
                queryBuilder: query => CreateArgumentBuilder<string, string>(query)
                    .AsUnionOf<IEnumerable<CustomObject<string>>, CustomObject<string>>()
                        .And<IEnumerable<CustomObject<string, int>>, CustomObject<string, int>>()
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<string, string>(mutation)
                    .AsUnionOf<IEnumerable<CustomObject<string>>, CustomObject<string>>()
                        .And<IEnumerable<CustomObject<string, int>>, CustomObject<string, int>>()
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(arg1: "test", arg2: "test2")}) {{
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
                            testField: ""testtest2""
                        },{
                            id: 0,
                            firstField: ""testtest2"",
                            secondField: 1
                        }]
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<IEnumerable<BaseCustomObject<int>>>(...).AsUnionOf<IEnumerable<CustomObject<string>>, CustomObject<string>>(...).And<IEnumerable<CustomObject<string, int>>, CustomObject<string, int>>(...).Resolve(...)")]
        public void ShouldResolveArrayOfUnionOfTwoGenericTypesWithBuildersWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, string, string, IEnumerable<BaseCustomObject<int>>>((_, arg1, arg2) =>
                new BaseCustomObject<int>[]
                {
                    CustomObject.Create(arg1 + arg2),
                    CustomObject.Create(arg1 + arg2, 1),
                });

            Test(
                queryBuilder: query => CreateArgumentBuilder<string, string>(query)
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
                mutationBuilder: mutation => CreateArgumentBuilder<string, string>(mutation)
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
                    test({BuildArguments(arg1: "test", arg2: "test2")}) {{
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
                            myField: ""testtest2""
                        },{
                            id: 0,
                            firstField: ""testtest2"",
                            secondField: 1
                        }]
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test]
        public void ShouldResolveClassWithIntFieldWithBuilder()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, CustomObject<int>>((_, arg1, arg2) =>
                new CustomObject<int> { TestField = arg1 + arg2 });

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int>(query)
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int>(mutation)
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                query: $@"
                    test({BuildArguments(arg1: 100500, arg2: 1)}) {{
                        myField
                    }}",
                expected: @"
                    {
                        test: {
                            myField: 100501
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test]
        public void ShouldResolveTaskClassWithIntFieldWithBuilder()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, Task<CustomObject<int>>>(async (_, arg1, arg2) =>
            {
                await Task.Yield();
                return new CustomObject<int> { TestField = arg1 + arg2 };
            });

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int>(query)
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int>(mutation)
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                query: $@"
                    test({BuildArguments(arg1: 100500, arg2: 1)}) {{
                        myField
                    }}",
                expected: @"
                    {
                        test: {
                            myField: 100501
                        }
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test]
        public void ShouldResolveClassWithIntFieldArrayResultWithBuilder()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int, IEnumerable<CustomObject<int>>>((_, arg1, arg2) =>
                new[] { arg1 + arg2, 2, 100500, 10 }.Select(n => new CustomObject<int> { TestField = n }));

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int>(query)
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int>(mutation)
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                query: $@"
                    test({BuildArguments(arg1: 100500, arg2: 1)}) {{
                        myField
                    }}",
                expected: @"
                    {
                        test: [{
                            myField: 100501
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
            var resolver = FuncSubstitute.For<TestUserContext, int, int, Task<IEnumerable<CustomObject<int>>>>(async (_, arg1, arg2) =>
            {
                await Task.Yield();
                return new[] { arg1 + arg2, 2, 100500, 10 }.Select(n => new CustomObject<int> { TestField = n });
            });

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, int>(query)
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                mutationBuilder: mutation => CreateArgumentBuilder<int, int>(mutation)
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                query: $@"
                    test({BuildArguments(arg1: 100500, arg2: 1)}) {{
                        myField
                    }}",
                expected: @"
                    {
                        test: [{
                            myField: 100501
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

            var resolver = FuncSubstitute.For<TestUserContext, int, Expression<Func<Person, bool>>, int>((_, arg1, filter) => 10);

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, Person>(loader, query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, Person>(loader, mutation)
                    .Resolve(resolver),
                query: $"test({BuildFilterArguments(1)})",
                expected: @"
                    {
                        test: 10
                    }");

            resolver.HasBeenCalledOnce(arg3: filter => ExpressionEqualityComparer.Instance.Equals(filter, FuncConstants<Person>.TrueExpression));
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

            var resolver = FuncSubstitute.For<TestUserContext, int, Expression<Func<Person, bool>>, int>((_, arg1, filter) => 10);

            Test(
                queryBuilder: query => CreateArgumentBuilder<int, Person>(loader, query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int, Person>(loader, mutation)
                    .Resolve(resolver),
                query: $"test({BuildFilterArguments(1, "{ or: [{ id: { eq: 1} }, { fullName: { isNull: true } }] }")})",
                expected: @"
                    {
                        test: 10
                    }");

            Expression<Func<Person, bool>> expected = p => p.Id == 1 || p.FullName == null;
            resolver.HasBeenCalledOnce(arg3: filter => ExpressionEqualityComparer.Instance.Equals(filter, expected));
        }

        private string BuildArguments(int arg1, int arg2)
        {
            return _argumentType switch
            {
                ArgumentType.Argument => $"arg1: {arg1}, arg2: {arg2}",
                ArgumentType.PayloadField => $"payload: {{ arg1: {arg1}, arg2: {arg2} }}",
                _ => throw new NotSupportedException(),
            };
        }

        private string BuildArguments(string arg1, string arg2)
        {
            return _argumentType switch
            {
                ArgumentType.Argument => $"arg1: \"{arg1}\", arg2: \"{arg2}\"",
                ArgumentType.PayloadField => $"payload: {{ arg1: \"{arg1}\", arg2: \"{arg2}\" }}",
                _ => throw new NotSupportedException(),
            };
        }

        private string BuildFilterArguments(int arg1, string filter = null)
        {
            var arg2 = filter == null ? string.Empty : $", arg2: {filter}";

            return _argumentType switch
            {
                ArgumentType.Argument => $"arg1: {arg1}{arg2}",
                ArgumentType.PayloadField => $"payload: {{ arg1: {arg1}{arg2} }}",
                _ => throw new NotSupportedException(),
            };
        }

        private IQueryFieldBuilder<IRootProjectionFieldBuilder<TArg1, TArg2, TestUserContext>, TArg1, TArg2, TestUserContext> CreateArgumentBuilder<TArg1, TArg2>(Query<TestUserContext> query)
        {
            return _argumentType switch
            {
                ArgumentType.Argument => query.Field("test")
                    .Argument<TArg1>("arg1")
                    .Argument<TArg2>("arg2"),
                ArgumentType.PayloadField => query.Field("test")
                    .PayloadField<TArg1>("arg1")
                    .PayloadField<TArg2>("arg2"),
                _ => throw new NotSupportedException(),
            };
        }

        private IMutationFieldBuilder<IMutationFieldBuilderBase<TArg1, TArg2, TestUserContext>, TArg1, TArg2, TestUserContext> CreateArgumentBuilder<TArg1, TArg2>(Mutation<TestUserContext> mutation)
        {
            return _argumentType switch
            {
                ArgumentType.Argument => mutation.Field("test")
                    .Argument<TArg1>("arg1")
                    .Argument<TArg2>("arg2"),
                ArgumentType.PayloadField => mutation.Field("test")
                    .PayloadField<TArg1>("arg1")
                    .PayloadField<TArg2>("arg2"),
                _ => throw new NotSupportedException(),
            };
        }

        private IQueryFieldBuilder<IRootProjectionFieldBuilder<TArg1, Expression<Func<TEntity, bool>>, TestUserContext>, TArg1, Expression<Func<TEntity, bool>>, TestUserContext> CreateArgumentBuilder<TArg1, TEntity>(Type loaderType, Query<TestUserContext> query)
            where TEntity : class
        {
            return _argumentType switch
            {
                ArgumentType.Argument => query.Field("test")
                    .Argument<TArg1>("arg1")
                    .FilterArgument<TEntity>(loaderType, "arg2"),
                ArgumentType.PayloadField => query.Field("test")
                    .PayloadField<TArg1>("arg1")
                    .FilterPayloadField<TEntity>(loaderType, "arg2"),
                _ => throw new NotSupportedException(),
            };
        }

        private IMutationFieldBuilder<IMutationFieldBuilderBase<TArg1, Expression<Func<TEntity, bool>>, TestUserContext>, TArg1, Expression<Func<TEntity, bool>>, TestUserContext> CreateArgumentBuilder<TArg1, TEntity>(Type loaderType, Mutation<TestUserContext> mutation)
            where TEntity : class
        {
            return _argumentType switch
            {
                ArgumentType.Argument => mutation.Field("test")
                    .Argument<TArg1>("arg1")
                    .FilterArgument<TEntity>(loaderType, "arg2"),
                ArgumentType.PayloadField => mutation.Field("test")
                    .PayloadField<TArg1>("arg1")
                    .FilterPayloadField<TEntity>(loaderType, "arg2"),
                _ => throw new NotSupportedException(),
            };
        }
    }
}
