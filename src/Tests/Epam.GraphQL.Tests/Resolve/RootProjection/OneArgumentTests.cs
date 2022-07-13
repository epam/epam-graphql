// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using GraphQL.Language.AST;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Resolve.RootProjection
{
    [TestFixtureSource(typeof(OperationAndArgumentFixtureArgCollection))]
    public class OneArgumentTests : ResolveRootProjectionTestsBase
    {
        private readonly ArgumentType _argumentType;

        public OneArgumentTests(OperationType operationType, ArgumentType argumentType)
            : base(operationType)
        {
            _argumentType = argumentType;
        }

        [Test(Description = "Field<int>(...).Resolve(_ => 10)")]
        public void ShouldResolveIntFieldWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int>((_, arg1) =>
                arg1);

            Test(
                queryBuilder: query => CreateArgumentBuilder<int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int>(mutation)
                    .Resolve(resolver),
                query: $"test({BuildArguments(10)})",
                expected: @"
                    {
                        test: 10
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<int>(...).Resolve(_ => Task 10)")]
        public void ShouldResolveTaskIntFieldWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, Task<int>>(async (_, arg1) =>
            {
                await Task.Yield();
                return arg1;
            });

            Test(
                queryBuilder: query => CreateArgumentBuilder<int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int>(mutation)
                    .Resolve(resolver),
                query: $"test({BuildArguments(10)})",
                expected: @"
                    {
                        test: 10
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<int?>(...).Resolve(_ => 10)")]
        public void ShouldResolveNullableIntFieldWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int?>((_, arg1) =>
                arg1);

            Test(
                queryBuilder: query => CreateArgumentBuilder<int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int>(mutation)
                    .Resolve(resolver),
                query: $"test({BuildArguments(10)})",
                expected: @"
                    {
                        test: 10
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<int?>(...).Resolve(_ => null)")]
        public void ShouldResolveNullableIntFieldReturningNullWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, int?>((_, arg1) => null);

            Test(
                queryBuilder: query => CreateArgumentBuilder<int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int>(mutation)
                    .Resolve(resolver),
                query: $"test({BuildArguments(10)})",
                expected: @"
                    {
                        test: null
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<string>(...).Resolve(_ => \"test\")")]
        public void ShouldResolveStringWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, string, string>((_, arg1) =>
                arg1);

            Test(
                queryBuilder: query => CreateArgumentBuilder<string>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<string>(mutation)
                    .Resolve(resolver),
                query: $"test({BuildArguments("Foo")})",
                expected: @"
                    {
                        test: ""Foo""
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<CustomObject<int>>(...).Resolve(_ => new CustomObject<int> { TestField = 10 })")]
        public void ShouldResolveCustomObjectWithIntFieldWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, CustomObject<int>>((_, arg1) =>
                CustomObject.Create(arg1));

            Test(
                queryBuilder: query => CreateArgumentBuilder<int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int>(mutation)
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(10)}) {{
                        testField
                    }}",
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
            var resolver = FuncSubstitute.For<TestUserContext, int, CustomObject<int?>>((_, arg1) =>
                CustomObject.Create<int?>(arg1));

            Test(
                queryBuilder: query => CreateArgumentBuilder<int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int>(mutation)
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(10)}) {{
                        testField
                    }}",
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
            var resolver = FuncSubstitute.For<TestUserContext, int, CustomObject<int?>>((_, arg1) =>
                CustomObject.Create<int?>(null));

            Test(
                queryBuilder: query => CreateArgumentBuilder<int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int>(mutation)
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(10)}) {{
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
            var resolver = FuncSubstitute.For<TestUserContext, string, CustomObject<string>>((_, arg1) =>
                CustomObject.Create(arg1));

            Test(
                queryBuilder: query => CreateArgumentBuilder<string>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<string>(mutation)
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments("test")}) {{
                        testField
                    }}",
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
            var resolver = FuncSubstitute.For<TestUserContext, int, CustomObject<int[]>>((_, arg1) =>
                CustomObject.Create(new[] { 10, arg1 }));

            Test(
                queryBuilder: query => CreateArgumentBuilder<int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int>(mutation)
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(20)}) {{
                        testField
                    }}",
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
            var resolver = FuncSubstitute.For<TestUserContext, int, CustomObject<List<int>>>((_, arg1) =>
                CustomObject.Create(new List<int> { 10, arg1 }));

            Test(
                queryBuilder: query => CreateArgumentBuilder<int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int>(mutation)
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(20)}) {{
                        testField
                    }}",
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
            var resolver = FuncSubstitute.For<TestUserContext, int, IEnumerable<CustomObject<int>>>((_, arg1) =>
                new[] { arg1, 2, 100500, 10 }.Select(n => new CustomObject<int> { TestField = n }));

            Test(
                queryBuilder: query => CreateArgumentBuilder<int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int>(mutation)
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(1)}) {{
                        testField
                    }}",
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
            var resolver = FuncSubstitute.For<TestUserContext, int, Task<IEnumerable<CustomObject<int>>>>(async (_, arg1) =>
            {
                await Task.Yield();
                return new[] { arg1, 2, 100500, 10 }.Select(n => new CustomObject<int> { TestField = n });
            });

            Test(
                queryBuilder: query => CreateArgumentBuilder<int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int>(mutation)
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(1)}) {{
                        testField
                    }}",
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
            var resolver = FuncSubstitute.For<TestUserContext, int, IEnumerable<CustomObject<CustomObject<int>>>>((_, arg1) =>
                new[] { arg1, 2, 100500, 10 }
                    .Select(n => new CustomObject<int> { TestField = n })
                    .Select(obj => new CustomObject<CustomObject<int>> { TestField = obj }));

            Test(
                queryBuilder: query => CreateArgumentBuilder<int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int>(mutation)
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(1)}) {{
                        testField {{
                            testField
                        }}
                    }}",
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
            var resolver = FuncSubstitute.For<TestUserContext, int, IEnumerable<int>>((_, arg1) =>
                new[] { arg1, 2, 100500, 10 });

            Test(
                queryBuilder: query => CreateArgumentBuilder<int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int>(mutation)
                    .Resolve(resolver),
                query: $"test({BuildArguments(1)})",
                expected: @"
                    {
                        test: [1, 2, 100500, 10]
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<IEnumerable<int>>(...).Resolve(...)")]
        public void ShouldResolveTaskIntArrayResultWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, Task<IEnumerable<int>>>(async (_, arg1) =>
            {
                await Task.Yield();
                return new[] { arg1, 2, 100500, 10 }.AsEnumerable();
            });

            Test(
                queryBuilder: query => CreateArgumentBuilder<int>(query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int>(mutation)
                    .Resolve(resolver),
                query: $"test({BuildArguments(1)})",
                expected: @"
                    {
                        test: [1, 2, 100500, 10]
                    }");

            resolver.HasBeenCalledTimes(1);
        }

        [Test(Description = "Field<Geometry>(...).AsUnionOf<Line>(...).And<Circle>(...).Resolve(...)")]
        public void ShouldResolveUnionOfTwoTypesWithTypeParam()
        {
            var resolver = FuncSubstitute.For<TestUserContext, int, Geometry>((_, arg1) =>
                new Line
                {
                    Id = 1,
                    Length = arg1,
                });

            Test(
                queryBuilder: query => CreateArgumentBuilder<int>(query)
                    .AsUnionOf<Line>()
                        .And<Circle>()
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int>(mutation)
                    .AsUnionOf<Line>()
                        .And<Circle>()
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(10)}) {{
                        ... on Line {{
                            id
                            length
                        }}
                    }}",
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
            var resolver = FuncSubstitute.For<TestUserContext, string, BaseCustomObject<int>>((_, arg1) =>
                CustomObject.Create(arg1));

            Test(
                queryBuilder: query => CreateArgumentBuilder<string>(query)
                    .AsUnionOf<CustomObject<string>>()
                        .And<CustomObject<string, int>>()
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<string>(mutation)
                    .AsUnionOf<CustomObject<string>>()
                        .And<CustomObject<string, int>>()
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments("test")}) {{
                        ... on CustomObjectOfString {{
                            testField
                        }}
                    }}",
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
            var resolver = FuncSubstitute.For<TestUserContext, int, IEnumerable<Geometry>>((_, arg1) =>
                new Geometry[]
                {
                    new Line
                    {
                        Id = 1,
                        Length = arg1,
                    },
                    new Circle
                    {
                        Id = 2,
                        Radius = arg1 / 2,
                    },
                });

            Test(
#pragma warning disable CS0618 // Type or member is obsolete
                queryBuilder: query => CreateArgumentBuilder<int>(query)
                    .AsUnionOf<IEnumerable<Line>, Line>()
                        .And<IEnumerable<Circle>, Circle>()
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<int>(mutation)
                    .AsUnionOf<IEnumerable<Line>, Line>()
                        .And<IEnumerable<Circle>, Circle>()
#pragma warning restore CS0618 // Type or member is obsolete
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments(10)}) {{
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
            var resolver = FuncSubstitute.For<TestUserContext, string, IEnumerable<BaseCustomObject<int>>>((_, arg1) =>
                new BaseCustomObject<int>[]
                {
                    CustomObject.Create(arg1),
                    CustomObject.Create(arg1, 1),
                });

            Test(
#pragma warning disable CS0618 // Type or member is obsolete
                queryBuilder: query => CreateArgumentBuilder<string>(query)
                    .AsUnionOf<IEnumerable<CustomObject<string>>, CustomObject<string>>()
                        .And<IEnumerable<CustomObject<string, int>>, CustomObject<string, int>>()
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<string>(mutation)
                    .AsUnionOf<IEnumerable<CustomObject<string>>, CustomObject<string>>()
                        .And<IEnumerable<CustomObject<string, int>>, CustomObject<string, int>>()
#pragma warning restore CS0618 // Type or member is obsolete
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments("test")}) {{
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
            var resolver = FuncSubstitute.For<TestUserContext, string, IEnumerable<BaseCustomObject<int>>>((_, arg1) =>
                new BaseCustomObject<int>[]
                {
                    CustomObject.Create(arg1),
                    CustomObject.Create(arg1, 1),
                });

            Test(
#pragma warning disable CS0618 // Type or member is obsolete
                queryBuilder: query => CreateArgumentBuilder<string>(query)
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
                mutationBuilder: mutation => CreateArgumentBuilder<string>(mutation)
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
                    .Resolve(resolver),
                query: $@"
                    test({BuildArguments("test")}) {{
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
            var resolver = FuncSubstitute.For<TestUserContext, int, CustomObject<int>>((_, arg1) =>
                new CustomObject<int> { TestField = arg1 });

            Test(
                queryBuilder: query => CreateArgumentBuilder<int>(query)
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                mutationBuilder: mutation => CreateArgumentBuilder<int>(mutation)
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                query: $@"
                    test({BuildArguments(100500)}) {{
                        myField
                    }}",
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
            var resolver = FuncSubstitute.For<TestUserContext, int, Task<CustomObject<int>>>(async (_, arg1) =>
            {
                await Task.Yield();
                return new CustomObject<int> { TestField = arg1 };
            });

            Test(
                queryBuilder: query => CreateArgumentBuilder<int>(query)
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                mutationBuilder: mutation => CreateArgumentBuilder<int>(mutation)
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                query: $@"
                    test({BuildArguments(100500)}) {{
                        myField
                    }}",
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
            var resolver = FuncSubstitute.For<TestUserContext, int, IEnumerable<CustomObject<int>>>((_, arg1) =>
                new[] { arg1, 2, 100500, 10 }.Select(n => new CustomObject<int> { TestField = n }));

            Test(
                queryBuilder: query => CreateArgumentBuilder<int>(query)
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                mutationBuilder: mutation => CreateArgumentBuilder<int>(mutation)
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                query: $@"
                    test({BuildArguments(1)}) {{
                        myField
                    }}",
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
            var resolver = FuncSubstitute.For<TestUserContext, int, Task<IEnumerable<CustomObject<int>>>>(async (_, arg1) =>
            {
                await Task.Yield();
                return new[] { arg1, 2, 100500, 10 }.Select(n => new CustomObject<int> { TestField = n });
            });

            Test(
                queryBuilder: query => CreateArgumentBuilder<int>(query)
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                mutationBuilder: mutation => CreateArgumentBuilder<int>(mutation)
                    .Resolve(resolver, b => b.Field("myField", o => o.TestField)),
                query: $@"
                    test({BuildArguments(1)}) {{
                        myField
                    }}",
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

            var resolver = FuncSubstitute.For<TestUserContext, Expression<Func<Person, bool>>, int>((_, filter) => 10);

            Test(
                queryBuilder: query => CreateArgumentBuilder<Person>(loader, query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<Person>(loader, mutation)
                    .Resolve(resolver),
                query: "test",
                expected: @"
                    {
                        test: 10
                    }");

            resolver.HasBeenCalledOnce(arg2: filter => ExpressionEqualityComparer.Instance.Equals(filter, FuncConstants<Person>.TrueExpression));
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

            var resolver = FuncSubstitute.For<TestUserContext, Expression<Func<Person, bool>>, int>((_, filter) => 10);

            Test(
                queryBuilder: query => CreateArgumentBuilder<Person>(loader, query)
                    .Resolve(resolver),
                mutationBuilder: mutation => CreateArgumentBuilder<Person>(loader, mutation)
                    .Resolve(resolver),
                query: $"test({BuildFilterArguments("{ or: [{ id: { eq: 1} }, { fullName: { isNull: true } }] }")})",
                expected: @"
                    {
                        test: 10
                    }");

            Expression<Func<Person, bool>> expected = p => p.Id == 1 || p.FullName == null;
            resolver.HasBeenCalledOnce(arg2: filter => ExpressionEqualityComparer.Instance.Equals(filter, expected));
        }

        private string BuildArguments(int arg1)
        {
            return _argumentType switch
            {
                ArgumentType.Argument => $"arg1: {arg1}",
                ArgumentType.PayloadField => $"payload: {{ arg1: {arg1} }}",
                _ => throw new NotSupportedException(),
            };
        }

        private string BuildArguments(string arg1)
        {
            return _argumentType switch
            {
                ArgumentType.Argument => $"arg1: \"{arg1}\"",
                ArgumentType.PayloadField => $"payload: {{ arg1: \"{arg1}\" }}",
                _ => throw new NotSupportedException(),
            };
        }

        private string BuildFilterArguments(string arg1)
        {
            return _argumentType switch
            {
                ArgumentType.Argument => $"arg1: {arg1}",
                ArgumentType.PayloadField => $"payload: {{ arg1: {arg1} }}",
                _ => throw new NotSupportedException(),
            };
        }

        private IUnionableRootField<TArg, TestUserContext> CreateArgumentBuilder<TArg>(Query<TestUserContext> query)
        {
            return _argumentType switch
            {
                ArgumentType.Argument => query.Field("test")
                    .Argument<TArg>("arg1"),
                ArgumentType.PayloadField => query.Field("test")
                    .PayloadField<TArg>("arg1"),
                _ => throw new NotSupportedException(),
            };
        }

        private IUnionableRootField<TArg, TestUserContext> CreateArgumentBuilder<TArg>(Mutation<TestUserContext> mutation)
        {
            return _argumentType switch
            {
                ArgumentType.Argument => mutation.Field("test")
                    .Argument<TArg>("arg1"),
                ArgumentType.PayloadField => mutation.Field("test")
                    .PayloadField<TArg>("arg1"),
                _ => throw new NotSupportedException(),
            };
        }

        private IUnionableRootField<Expression<Func<TEntity, bool>>, TestUserContext> CreateArgumentBuilder<TEntity>(Type loaderType, Query<TestUserContext> query)
            where TEntity : class
        {
            return _argumentType switch
            {
                ArgumentType.Argument => query.Field("test")
                    .FilterArgument<TEntity, TestUserContext>(loaderType, "arg1"),
                ArgumentType.PayloadField => query.Field("test")
                    .FilterPayloadField<TEntity, TestUserContext>(loaderType, "arg1"),
                _ => throw new NotSupportedException(),
            };
        }

        private IUnionableRootField<Expression<Func<TEntity, bool>>, TestUserContext> CreateArgumentBuilder<TEntity>(Type loaderType, Mutation<TestUserContext> mutation)
            where TEntity : class
        {
            return _argumentType switch
            {
                ArgumentType.Argument => mutation.Field("test")
                    .FilterArgument<TEntity, TestUserContext>(loaderType, "arg1"),
                ArgumentType.PayloadField => mutation.Field("test")
                    .FilterPayloadField<TEntity, TestUserContext>(loaderType, "arg1"),
                _ => throw new NotSupportedException(),
            };
        }
    }
}
