// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epam.GraphQL.EntityFrameworkCore;
using Epam.GraphQL.Mutation;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NSubstitute;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Resolve.Mutation
{
    [TestFixture]
    public class NoArgsTests : ResolveMutationTestBase
    {
        [Test(Description = "Manual mutation/Field/Func<IEnumerable<Person>>")]
        public void TestReturnsEnumerable()
        {
            var manualMutation = FuncSubstitute.For<TestUserContext, IEnumerable<Person>>(ctx =>
                new[]
                {
                    new Person(ctx.DataContext.GetQueryable<Person>().First())
                    {
                        Id = 0,
                        FullName = "Test",
                    },
                });

            var personLoader = CreatePersonLoader();

            TestMutation(
                queryBuilder: query => query
                    .Connection(personLoader, "people"),
                mutationBuilder: mutation =>
                {
                    mutation.SubmitField(personLoader, "people");
                    mutation.Field("manualMutation")
                        .Resolve(manualMutation);
                },
                query: @"
                    mutation {
                        manualMutation {
                            people {
                                id
                                payload {
                                    id
                                    fullName
                                }
                            }
                        }
                    }",
                expected: @"
                    {
                        manualMutation: {
                            people: [{
                                id: 7,
                                payload: {
                                    id: 7,
                                    fullName: ""Test""
                                }
                            }]
                        }
                    }");

            DataContext
                .Received()
                .AddRange(Arg.Is<IEnumerable<Person>>(p => p.First().Id == 7 && p.First().FullName == "Test"));

            DataContext
                .Received(1)
                .SaveChangesAsync();

            manualMutation.HasBeenCalledOnce();
        }

        [Test(Description = "Manual mutation/Field/Func<Task<IEnumerable<Person>>>")]
        public void TestReturnsEnumerableTask()
        {
            var manualMutation = FuncSubstitute.For<TestUserContext, Task<IEnumerable<Person>>>(async ctx =>
            {
                await Task.Yield();
                return new[]
                {
                    new Person(ctx.DataContext.GetQueryable<Person>().First())
                    {
                        Id = 0,
                        FullName = "Test",
                    },
                };
            });

            var personLoader = CreatePersonLoader();

            TestMutation(
                queryBuilder: query => query
                    .Connection(personLoader, "people"),
                mutationBuilder: mutation =>
                {
                    mutation.SubmitField(personLoader, "people");
                    mutation.Field("manualMutation")
                        .Resolve(manualMutation);
                },
                query: @"
                    mutation {
                        manualMutation {
                            people {
                                id
                                payload {
                                    id
                                    fullName
                                }
                            }
                        }
                    }",
                expected: @"
                    {
                        manualMutation: {
                            people: [{
                                id: 7,
                                payload: {
                                    id: 7,
                                    fullName: ""Test""
                                }
                            }]
                        }
                    }");

            DataContext
                .Received(1)
                .AddRange(Arg.Is<IEnumerable<Person>>(p => p.First().Id == 7 && p.First().FullName == "Test"));

            DataContext
                .Received(1)
                .SaveChangesAsync();

            manualMutation.HasBeenCalledOnce();
        }

        [Test(Description = "Manual mutation/Hook/Field/Func<IEnumerable<Person>>")]
        public void TestReturnsEnumerableWithHook()
        {
            var manualMutation = FuncSubstitute.For<TestUserContext, IEnumerable<Person>>(ctx =>
                ctx.DataContext.GetQueryable<Person>().Take(1));

            var personLoader = CreatePersonLoader();

            var afterSave = FuncSubstitute.For<IAfterSaveContext<TestUserContext>, IEnumerable<object>, Task<IEnumerable<object>>>((ctx, items) =>
                Task.FromResult<IEnumerable<object>>(ctx.ExecutionContext.DataContext.GetQueryable<Person>().Skip(1).Take(1)));

            TestMutation(
                queryBuilder: query => query
                    .Connection(personLoader, "people"),
                mutationBuilder: mutation =>
                {
                    mutation.SubmitField(personLoader, "people");
                    mutation.Field("manualMutation")
                        .Resolve(manualMutation);
                },
                query: @"
                    mutation {
                        manualMutation {
                            people {
                                id
                                payload {
                                    id
                                }
                            }
                        }
                    }",
                expected: @"
                    {
                        manualMutation: {
                            people: [{
                                id: 1,
                                payload: {
                                    id: 1
                                }
                            },{
                                id: 2,
                                payload: {
                                    id: 2
                                }
                            }]
                        }
                    }",
                afterSave: afterSave);

            DataContext
                .Received(2)
                .SaveChangesAsync();

            afterSave.HasBeenCalledOnce();
            manualMutation.HasBeenCalledOnce();
        }

        [Test(Description = "Manual mutation/Field/Func<IEnumerable<Person>>")]
        public void TestReturnsEnumerableDoNotSaveChanges()
        {
            var manualMutation = FuncSubstitute.For<TestUserContext, IEnumerable<Person>>(ctx =>
                new[]
                {
                    new Person(ctx.DataContext.GetQueryable<Person>().First())
                    {
                        Id = 7,
                        FullName = "Test",
                    },
                });

            var personLoader = CreatePersonLoader();

            TestMutation(
                queryBuilder: query => query
                    .Connection(personLoader, "people"),
                mutationBuilder: mutation =>
                {
                    mutation.SubmitField(personLoader, "people");
                    mutation.Field("manualMutation")
                        .Resolve(manualMutation, builder => builder.DoNotSaveChanges().DoNotAddNewEntitiesToDbContext());
                },
                query: @"
                    mutation {
                        manualMutation {
                            people {
                                id
                                payload {
                                    id
                                }
                            }
                        }
                    }",
                expected: @"
                    {
                        manualMutation: {
                            people: [{
                                id: 7,
                                payload: {
                                    id: 7
                                }
                            }]
                        }
                    }");

            DataContext
                .DidNotReceive()
                .AddRange(Arg.Any<IEnumerable<Person>>());

            DataContext
                .DidNotReceive()
                .SaveChangesAsync();

            manualMutation.HasBeenCalledOnce();
        }

        [Test(Description = "Manual mutation/Field/Func<Task<IEnumerable<Person>>>")]
        public void TestReturnsEnumerableTaskDoNotSaveChanges()
        {
            var manualMutation = FuncSubstitute.For<TestUserContext, Task<IEnumerable<Person>>>(async ctx =>
            {
                await Task.Yield();
                return new[]
                {
                    new Person(ctx.DataContext.GetQueryable<Person>().First())
                    {
                        Id = 7,
                        FullName = "Test",
                    },
                };
            });

            var personLoader = CreatePersonLoader();

            TestMutation(
                queryBuilder: query => query
                    .Connection(personLoader, "people"),
                mutationBuilder: mutation =>
                {
                    mutation.SubmitField(personLoader, "people");
                    mutation.Field("manualMutation")
                        .Resolve(manualMutation, options => options.DoNotAddNewEntitiesToDbContext().DoNotSaveChanges());
                },
                query: @"
                    mutation {
                        manualMutation {
                            people {
                                id
                                payload {
                                    id
                                }
                            }
                        }
                    }",
                expected: @"
                    {
                        manualMutation: {
                            people: [{
                                id: 7,
                                payload: {
                                    id: 7
                                }
                            }]
                        }
                    }");

            DataContext
                .DidNotReceive()
                .AddRange(Arg.Any<IEnumerable<Person>>());

            DataContext
                .DidNotReceive()
                .SaveChangesAsync();

            manualMutation.HasBeenCalledOnce();
        }

        [Test(Description = "Manual mutation/Hook/Field/Func<IEnumerable<Person>>")]
        public void TestReturnsEnumerableWithHookDoNotSaveChanges()
        {
            var manualMutation = FuncSubstitute.For<TestUserContext, IEnumerable<Person>>(ctx =>
                ctx.DataContext.GetQueryable<Person>().Take(1));

            var personLoader = CreatePersonLoader();

            var afterSave = FuncSubstitute.For<IAfterSaveContext<TestUserContext>, IEnumerable<object>, Task<IEnumerable<object>>>((ctx, items) =>
                Task.FromResult<IEnumerable<object>>(ctx.ExecutionContext.DataContext.GetQueryable<Person>().Skip(1).Take(1)));

            TestMutation(
                queryBuilder: query => query
                    .Connection(personLoader, "people"),
                mutationBuilder: mutation =>
                {
                    mutation.SubmitField(personLoader, "people");
                    mutation.Field("manualMutation")
                        .Resolve(manualMutation, options => options.DoNotAddNewEntitiesToDbContext().DoNotSaveChanges());
                },
                query: @"
                    mutation {
                        manualMutation {
                            people {
                                id
                                payload {
                                    id
                                }
                            }
                        }
                    }",
                expected: @"
                    {
                        manualMutation: {
                            people: [{
                                id: 1,
                                payload: {
                                    id: 1
                                }
                            },{
                                id: 2,
                                payload: {
                                    id: 2
                                }
                            }]
                        }
                    }",
                afterSave: afterSave);

            DataContext
                .DidNotReceive()
                .AddRange(Arg.Any<IEnumerable<Person>>());

            DataContext
                .Received(1)
                .SaveChangesAsync();

            afterSave.HasBeenCalledOnce();
            manualMutation.HasBeenCalledOnce();
        }

        [Test(Description = "Manual mutation/Field/Func<MutationResult<string>>")]
        public void TestReturnsMutationResult()
        {
            var manualMutation = FuncSubstitute.For<TestUserContext, MutationResult<string>>(ctx =>
                MutationResult.Create(ctx.DataContext.GetQueryable<Person>().Take(1), "Test"));

            var personLoader = CreatePersonLoader();

            TestMutation(
                queryBuilder: query => query
                    .Connection(personLoader, "people"),
                mutationBuilder: mutation =>
                {
                    mutation.SubmitField(personLoader, "people");
                    mutation.Field("manualMutation")
                        .Resolve(manualMutation);
                },
                query: @"
                    mutation {
                        manualMutation {
                            payload {
                                people {
                                    id
                                    payload {
                                        id
                                    }
                                }
                            }
                            data
                        }
                    }",
                expected: @"
                    {
                        manualMutation: {
                            payload: {
                                people: [{
                                    id: 1,
                                    payload: {
                                        id: 1
                                    }
                                }]
                            },
                            data: ""Test""
                        }
                    }");

            manualMutation.HasBeenCalledOnce();
        }

        [Test(Description = "Manual mutation/Field/Func<Task<MutationResult<string>>>")]
        public void TestReturnsMutationResultTask()
        {
            var manualMutation = FuncSubstitute.For<TestUserContext, Task<MutationResult<string>>>(async ctx =>
            {
                await Task.Yield();
                return MutationResult.Create(ctx.DataContext.GetQueryable<Person>().Take(1), "Test");
            });

            var personLoader = CreatePersonLoader();

            TestMutation(
                queryBuilder: query => query
                    .Connection(personLoader, "people"),
                mutationBuilder: mutation =>
                {
                    mutation.SubmitField(personLoader, "people");
                    mutation.Field("manualMutation")
                        .Resolve(manualMutation);
                },
                query: @"
                    mutation {
                        manualMutation {
                            payload {
                                people {
                                    id
                                    payload {
                                        id
                                    }
                                }
                            }
                            data
                        }
                    }",
                expected: @"
                    {
                        manualMutation: {
                            payload: {
                                people: [{
                                    id: 1,
                                    payload: {
                                        id: 1
                                    }
                                }]
                            },
                            data: ""Test""
                        }
                    }");

            manualMutation.HasBeenCalledOnce();
        }

        [Test(Description = "Manual mutation/Field/Func<MutationResult<string>>")]
        public void TestReturnsMutationResultDoNotSaveChanges()
        {
            var manualMutation = FuncSubstitute.For<TestUserContext, MutationResult<string>>(ctx =>
                MutationResult.Create(ctx.DataContext.GetQueryable<Person>().Take(1), "Test"));

            var personLoader = CreatePersonLoader();

            TestMutation(
                queryBuilder: query => query
                    .Connection(personLoader, "people"),
                mutationBuilder: mutation =>
                {
                    mutation.SubmitField(personLoader, "people");
                    mutation.Field("manualMutation")
                        .Resolve(manualMutation, optionsBuilder => optionsBuilder.DoNotSaveChanges());
                },
                query: @"
                    mutation {
                        manualMutation {
                            payload {
                                people {
                                    id
                                    payload {
                                        id
                                    }
                                }
                            }
                            data
                        }
                    }",
                expected: @"
                    {
                        manualMutation: {
                            payload: {
                                people: [{
                                    id: 1,
                                    payload: {
                                        id: 1
                                    }
                                }]
                            },
                            data: ""Test""
                        }
                    }");

            manualMutation.HasBeenCalledOnce();

            DataContext
                .DidNotReceive()
                .SaveChangesAsync();

            DataContext
                .DidNotReceive()
                .AddRange(Arg.Any<IEnumerable<Person>>());
        }

        [Test(Description = "Manual mutation/Field/Func<Task<MutationResult<string>>>")]
        public void TestReturnsMutationResultTaskDoNotSaveChanges()
        {
            var manualMutation = FuncSubstitute.For<TestUserContext, Task<MutationResult<string>>>(async ctx =>
            {
                await Task.Yield();
                return MutationResult.Create(ctx.DataContext.GetQueryable<Person>().Take(1), "Test");
            });

            var personLoader = CreatePersonLoader();

            TestMutation(
                queryBuilder: query => query
                    .Connection(personLoader, "people"),
                mutationBuilder: mutation =>
                {
                    mutation.SubmitField(personLoader, "people");
                    mutation.Field("manualMutation")
                        .Resolve(manualMutation, optionsBuilder => optionsBuilder.DoNotSaveChanges());
                },
                query: @"
                    mutation {
                        manualMutation {
                            payload {
                                people {
                                    id
                                    payload {
                                        id
                                    }
                                }
                            }
                            data
                        }
                    }",
                expected: @"
                    {
                        manualMutation: {
                            payload: {
                                people: [{
                                    id: 1,
                                    payload: {
                                        id: 1
                                    }
                                }]
                            },
                            data: ""Test""
                        }
                    }");

            manualMutation.HasBeenCalledOnce();

            DataContext
                .DidNotReceive()
                .SaveChangesAsync();

            DataContext
                .DidNotReceive()
                .AddRange(Arg.Any<IEnumerable<Person>>());
        }

        [Test(Description = "Manual mutation/Field/Func<CustomObject<string>>/Config")]
        public void TestTypeParamReturnsConfigurableResult()
        {
            var manualMutation = FuncSubstitute.For<TestUserContext, CustomObject<string>>(_ =>
                CustomObject.Create("Test"));

            var personLoader = CreatePersonLoader();

            TestMutation(
                queryBuilder: query => query
                    .Connection(personLoader, "people"),
                mutationBuilder: mutation =>
                {
                    mutation.SubmitField(personLoader, "people");
                    mutation.Field("manualMutation")
                        .Resolve(manualMutation, build => build.Field("myField", i => i.TestField));
                },
                query: @"
                    mutation {
                        manualMutation {
                            myField
                        }
                    }",
                expected: @"
                    {
                        manualMutation: {
                            myField: ""Test""
                        }
                    }");

            manualMutation.HasBeenCalledOnce();
        }

        [Test(Description = "Manual mutation/Field/Func<Task<CustomObject<string>>>/Config")]
        public void TestTypeParamReturnsConfigurableResultTask()
        {
            var manualMutation = FuncSubstitute.For<TestUserContext, Task<CustomObject<string>>>(async _ =>
            {
                await Task.Yield();
                return CustomObject.Create("Test");
            });

            var personLoader = CreatePersonLoader();

            TestMutation(
                queryBuilder: query => query
                    .Connection(personLoader, "people"),
                mutationBuilder: mutation =>
                {
                    mutation.SubmitField(personLoader, "people");
                    mutation.Field("manualMutation")
                        .Resolve(manualMutation, build => build.Field("myField", i => i.TestField));
                },
                query: @"
                    mutation {
                        manualMutation {
                            myField
                        }
                    }",
                expected: @"
                    {
                        manualMutation: {
                            myField: ""Test""
                        }
                    }");

            manualMutation.HasBeenCalledOnce();
        }

        [Test]
        public void TestHookReturnsEnumerable()
        {
            var manualMutation = FuncSubstitute.For<TestUserContext, MutationResult<string>>((ctx) =>
                MutationResult.Create(ctx.DataContext.GetQueryable<Person>().Take(1), "Test"));

            var personLoader = CreatePersonLoader();

            var afterSave = FuncSubstitute.For<IAfterSaveContext<TestUserContext>, IEnumerable<object>, Task<IEnumerable<object>>>((ctx, items) =>
                Task.FromResult<IEnumerable<object>>(ctx.ExecutionContext.DataContext.GetQueryable<Person>().Skip(1).Take(1)));

            TestMutation(
                queryBuilder: query => query.Connection(personLoader, "people"),
                mutationBuilder: mutation =>
                {
                    mutation.SubmitField(personLoader, "people");
                    mutation.Field("manualMutation")
                        .Resolve(manualMutation);
                },
                query: @"
                    mutation {
                        manualMutation {
                            payload {
                                people {
                                    id
                                    payload {
                                        id
                                    }
                                }
                            }
                            data
                        }
                    }",
                expected: @"
                    {
                        manualMutation: {
                            payload: {
                                people: [{
                                    id: 1,
                                    payload: {
                                        id: 1
                                    }
                                },{
                                    id: 2,
                                    payload: {
                                        id: 2
                                    }
                                }]
                            },
                            data: ""Test""
                        }
                    }",
                afterSave: afterSave);

            DataContext
                .Received()
                .SaveChangesAsync();

            afterSave.HasBeenCalledOnce();
            manualMutation.HasBeenCalledOnce();
        }

        /// <summary>
        /// Test for https://git.epam.com/epm-ppa/epam-graphql/-/issues/17 (Manual mutation returning MutationResult fails if "payload" field wasn't quieried).
        /// </summary>
        [Test(Description = "Manual mutation/Field/Func<MutationResult<string>>")]
        public void TestReturnsMutationResultNoPayload()
        {
            var manualMutation = FuncSubstitute.For<TestUserContext, MutationResult<string>>(ctx =>
                MutationResult.Create(ctx.DataContext.GetQueryable<Person>().Take(1), "Test"));

            var baseQuery = FuncSubstitute.For<TestUserContext, IQueryable<Person>>(ctx =>
                ctx.DataContext.GetQueryable<Person>());

            var personLoader = CreatePersonLoader(baseQuery);

            TestMutation(
                queryBuilder: query => query.Connection(personLoader, "people"),
                mutationBuilder: mutation =>
                {
                    mutation.SubmitField(personLoader, "people");
                    mutation.Field("manualMutation")
                        .Resolve(manualMutation);
                },
                query: @"
                    mutation {
                        manualMutation {
                            data
                        }
                    }",
                expected: @"
                    {
                        manualMutation: {
                            data: ""Test""
                        }
                    }");

            manualMutation.HasBeenCalledOnce();
            baseQuery.HasNotBeenCalled();
        }

        /// <summary>
        /// Test for https://git.epam.com/epm-ppa/epam-graphql/-/issues/17 (Manual mutation returning MutationResult fails if "payload" field wasn't quieried).
        /// </summary>
        [Test(Description = "Manual mutation/Field/Func<Task<MutationResult<string>>>")]
        public void TestReturnsMutationResultTaskNoPayload()
        {
            var manualMutation = FuncSubstitute.For<TestUserContext, Task<MutationResult<string>>>(async ctx =>
            {
                await Task.Yield();
                return MutationResult.Create(ctx.DataContext.GetQueryable<Person>().Take(1), "Test");
            });

            var baseQuery = FuncSubstitute.For<TestUserContext, IQueryable<Person>>(ctx =>
                ctx.DataContext.GetQueryable<Person>());

            var personLoader = CreatePersonLoader(baseQuery);

            TestMutation(
                queryBuilder: query => query.Connection(personLoader, "people"),
                mutationBuilder: mutation =>
                {
                    mutation.SubmitField(personLoader, "people");
                    mutation.Field("manualMutation")
                        .Resolve(manualMutation);
                },
                query: @"
                    mutation {
                        manualMutation {
                            data
                        }
                    }",
                expected: @"
                    {
                        manualMutation: {
                            data: ""Test""
                        }
                    }");

            manualMutation.HasBeenCalledOnce();
            baseQuery.HasNotBeenCalled();
        }

        /// <summary>
        /// Test for https://git.epam.com/epm-ppa/epam-graphql/-/issues/17 (Manual mutation returning MutationResult fails if "payload" field wasn't quieried).
        /// </summary>
        [Test(Description = "Manual mutation/Field/Func<MutationResult<string>>")]
        public void TestReturnsMutationResultDoNotSaveChangesNoPayload()
        {
            var manualMutation = FuncSubstitute.For<TestUserContext, MutationResult<string>>(ctx =>
                MutationResult.Create(ctx.DataContext.GetQueryable<Person>().Take(1), "Test"));

            var baseQuery = FuncSubstitute.For<TestUserContext, IQueryable<Person>>(ctx =>
                ctx.DataContext.GetQueryable<Person>());

            var personLoader = CreatePersonLoader(baseQuery);

            TestMutation(
                queryBuilder: query => query.Connection(personLoader, "people"),
                mutationBuilder: mutation =>
                {
                    mutation.SubmitField(personLoader, "people");
                    mutation.Field("manualMutation")
                        .Resolve(manualMutation, optionsBuilder => optionsBuilder.DoNotSaveChanges());
                },
                query: @"
                    mutation {
                        manualMutation {
                            data
                        }
                    }",
                expected: @"
                    {
                        manualMutation: {
                            data: ""Test""
                        }
                    }");

            manualMutation.HasBeenCalledOnce();
            baseQuery.HasNotBeenCalled();

            DataContext
                .DidNotReceive()
                .SaveChangesAsync();

            DataContext
                .DidNotReceive()
                .AddRange(Arg.Any<IEnumerable<Person>>());
        }

        /// <summary>
        /// Test for https://git.epam.com/epm-ppa/epam-graphql/-/issues/17 (Manual mutation returning MutationResult fails if "payload" field wasn't quieried).
        /// </summary>
        [Test(Description = "Manual mutation/Field/Func<Task<MutationResult<string>>>")]
        public void TestReturnsMutationResultTaskDoNotSaveChangesNoPayload()
        {
            var manualMutation = FuncSubstitute.For<TestUserContext, Task<MutationResult<string>>>(async ctx =>
            {
                await Task.Yield();
                return MutationResult.Create(ctx.DataContext.GetQueryable<Person>().Take(1), "Test");
            });

            var baseQuery = FuncSubstitute.For<TestUserContext, IQueryable<Person>>(ctx =>
                ctx.DataContext.GetQueryable<Person>());

            var personLoader = CreatePersonLoader(baseQuery);

            TestMutation(
                queryBuilder: query => query.Connection(personLoader, "people"),
                mutationBuilder: mutation =>
                {
                    mutation.SubmitField(personLoader, "people");
                    mutation.Field("manualMutation")
                        .Resolve(manualMutation, optionsBuilder => optionsBuilder.DoNotSaveChanges());
                },
                query: @"
                    mutation {
                        manualMutation {
                            data
                        }
                    }",
                expected: @"
                    {
                        manualMutation: {
                            data: ""Test""
                        }
                    }");

            manualMutation.HasBeenCalledOnce();
            baseQuery.HasNotBeenCalled();

            DataContext
                .DidNotReceive()
                .SaveChangesAsync();

            DataContext
                .DidNotReceive()
                .AddRange(Arg.Any<IEnumerable<Person>>());
        }
    }
}
