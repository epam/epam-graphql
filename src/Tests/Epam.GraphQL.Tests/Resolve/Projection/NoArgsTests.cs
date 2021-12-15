// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NSubstitute;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Resolve.Projection
{
    [TestFixture]
    public class NoArgsTests
    {
        public interface IGeometry
        {
            int Id { get; set; }
        }

        public interface IGeometry2 : IGeometry
        {
            string ReadOnlyProperty { get; }
        }

        [Test]
        public void ShouldResolveFieldReturningInterface()
        {
            var resolver = Substitute.For<Func<TestUserContext, Projection, IGeometry>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Projection>())
                .Returns(callInfo => new Geometry { Id = 1 });

            var geometryLoader = GraphQLTypeBuilder.CreateLoaderType<IGeometry, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field("id1", g => g.Id);
                    loader.Field("prop", g => g.Id * 2);
                },
                getBaseQuery: ctx => new[] { new Geometry { Id = 1 } }.AsQueryable(),
                applyNaturalOrderBy: q => q.OrderBy(g => g.Id),
                applyNaturalThenBy: q => q.OrderBy(g => g.Id));

            var projection = GraphQLTypeBuilder.CreateProjectionType<Projection, TestUserContext>(
                onConfigure: prj => prj.Field("geometry").Resolve(resolver, builder => builder.ConfigureFrom(geometryLoader)));

            TestHelpers.TestQuery(
                builder: builder =>
                {
                    builder.Field("test")
                        .Resolve(ctx => new Projection(), builder => builder.ConfigureFrom(projection));
                },
                query: @"
                    query {
                        test {
                            geometry {
                                id1
                                prop
                            }
                        }
                    }",
                expected: @"
                    {
                        test: {
                            geometry: {
                                id1: 1,
                                prop: 2
                            }
                        }
                    }");

            resolver.Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Projection>());
        }

        [Test]
        public void ShouldResolveFieldViaExpression()
        {
            var projection = GraphQLTypeBuilder.CreateProjectionType<Projection, TestUserContext>(
                onConfigure: prj => prj.Field("stringField", x => x.StringField + "1"));

            TestHelpers.TestQuery(
                builder: builder =>
                {
                    builder.Field("test")
                        .Resolve(
                            ctx => new Projection
                            {
                                StringField = "Test",
                            },
                            builder => builder.ConfigureFrom(projection));
                },
                query: @"
                    query {
                        test {
                            stringField
                        }
                    }",
                expected: @"
                    {
                        test: {
                            stringField: ""Test1""
                        }
                    }");
        }

        [Test]
        public void ShouldResolveFieldReturningInterfaceWithReadOnlyProperty()
        {
            var resolver = Substitute.For<Func<TestUserContext, Projection, IGeometry2>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Projection>())
                .Returns(callInfo => new Geometry { Id = 1 });

            var geometryLoader = GraphQLTypeBuilder.CreateLoaderType<IGeometry2, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field("id1", g => g.Id);
                    loader.Field("prop", g => g.Id * 2);
                    loader.Field(g => g.ReadOnlyProperty);
                    loader.Field("prop2", g => g.ReadOnlyProperty + "2");
                },
                getBaseQuery: ctx => new[] { new Geometry { Id = 1 } }.AsQueryable(),
                applyNaturalOrderBy: q => q.OrderBy(g => g.Id),
                applyNaturalThenBy: q => q.OrderBy(g => g.Id));

            var projection = GraphQLTypeBuilder.CreateProjectionType<Projection, TestUserContext>(
                onConfigure: prj => prj.Field("geometry").Resolve(resolver, builder => builder.ConfigureFrom(geometryLoader)));

            TestHelpers.TestQuery(
                builder: builder =>
                {
                    builder.Field("test")
                        .Resolve(ctx => new Projection(), builder => builder.ConfigureFrom(projection));
                },
                query: @"
                    query {
                        test {
                            geometry {
                                id1
                                prop
                                readOnlyProperty
                                prop2
                            }
                        }
                    }",
                expected: @"
                    {
                        test: {
                            geometry: {
                                id1: 1,
                                prop: 2,
                                readOnlyProperty: ""Test"",
                                prop2: ""Test2""
                            }
                        }
                    }");

            resolver.Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Projection>());
        }

        [Test]
        public void ShouldResolveFieldReturningClass()
        {
            var resolver = Substitute.For<Func<TestUserContext, Projection, Geometry>>();
            resolver
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Projection>())
                .Returns(callInfo => new Geometry { Id = 1 });

            var geometryLoader = GraphQLTypeBuilder.CreateLoaderType<Geometry, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field("id1", g => g.Id);
                    loader.Field("prop", g => g.Id * 2);
                    loader.Field(g => g.ReadOnlyProperty);
                    loader.Field("prop2", g => g.ReadOnlyProperty + "2");
                },
                getBaseQuery: ctx => new[] { new Geometry { Id = 1 } }.AsQueryable(),
                applyNaturalOrderBy: q => q.OrderBy(g => g.Id),
                applyNaturalThenBy: q => q.OrderBy(g => g.Id));

            var projection = GraphQLTypeBuilder.CreateProjectionType<Projection, TestUserContext>(
                onConfigure: prj => prj.Field("geometry").Resolve(resolver, builder => builder.ConfigureFrom(geometryLoader)));

            TestHelpers.TestQuery(
                builder: builder =>
                {
                    builder.Field("test")
                        .Resolve(ctx => new Projection(), builder => builder.ConfigureFrom(projection));
                },
                query: @"
                    query {
                        test {
                            geometry {
                                id1
                                prop
                                readOnlyProperty
                                prop2
                            }
                        }
                    }",
                expected: @"
                    {
                        test: {
                            geometry: {
                                id1: 1,
                                prop: 2,
                                readOnlyProperty: ""Test"",
                                prop2: ""Test2""
                            }
                        }
                    }");

            resolver.Received(1)
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Projection>());
        }

        [Test]
        public void ShouldResolveFieldReturningEnumerable()
        {
            var resolver = Substitute.For<Func<TestUserContext, IEnumerable<Person>>>();

            resolver
                .Invoke(Arg.Any<TestUserContext>())
                .Returns(callInfo => new[]
                    {
                        new Person
                        {
                            Id = 7,
                            FullName = "Test",
                        },
                    });

            var personLoader = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName);
                },
                getBaseQuery: context => Enumerable.Empty<Person>().AsQueryable(),
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id));

            TestHelpers.TestQuery(
                query => query.Field("field").Resolve(resolver),
                @"query {
                    field {
                        id
                        fullName
                    }
                }",
                @"{
                    field: [{
                        id: 7,
                        fullName: ""Test""
                    }]
                }");

            resolver.Received(1)
                .Invoke(Arg.Any<TestUserContext>());
        }

        [Test]
        public void ShouldResolveFieldReturningArray()
        {
            var resolver = Substitute.For<Func<TestUserContext, Person[]>>();

            resolver
                .Invoke(Arg.Any<TestUserContext>())
                .Returns(callInfo => new[]
                    {
                        new Person
                        {
                            Id = 7,
                            FullName = "Test",
                        },
                    });

            var personLoader = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName);
                },
                getBaseQuery: context => Enumerable.Empty<Person>().AsQueryable(),
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id));

            TestHelpers.TestQuery(
                query => query.Field("field").Resolve(resolver),
                @"query {
                    field {
                        id
                        fullName
                    }
                }",
                @"{
                    field: [{
                        id: 7,
                        fullName: ""Test""
                    }]
                }");

            resolver.Received(1)
                .Invoke(Arg.Any<TestUserContext>());
        }

        public class Geometry : IGeometry, IGeometry2
        {
            public int Id { get; set; }

            public string ReadOnlyProperty => "Test";
        }

        public class Projection
        {
            public string StringField { get; set; }
        }
    }
}
