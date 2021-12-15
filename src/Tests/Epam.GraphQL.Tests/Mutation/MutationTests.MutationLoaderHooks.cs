// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NSubstitute;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Mutation
{
    public partial class MutationTests
    {
        [Test(Description = "Insert (should call BeforeCreate and should not call BeforeUpdate)")]
        public void TestMutationHookInsertShouldCallBeforeCreateAndShouldNotCallBeforeUpdate()
        {
            var beforeCreate = Substitute.For<Action<TestUserContext, Person>>();
            var beforeUpdate = Substitute.For<Action<TestUserContext, Person>>();

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Editable();
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>(),
                beforeCreate: beforeCreate,
                beforeUpdate: beforeUpdate);

            void QueryBuilder(Query<TestUserContext> query)
            {
                query.Connection(personLoader, "people");
            }

            void MutationBuilder(Mutation<TestUserContext> mutation)
            {
                mutation.SubmitField(personLoader, "people");
            }

            void Checks(IDataContext dataContext)
            {
                beforeUpdate.DidNotReceive().Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
                beforeCreate.Received().Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: -1,
                            fullName: ""Test""
                        }]
                    }) {
                        people {
                            id
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: -1,
                        }]
                    }
                }",
                Checks);
        }

        [Test(Description = "https://git.epam.com/epm-ppa/epam-graphql/-/issues/28")]
        [Ignore("TBD: https://git.epam.com/epm-ppa/epam-graphql/-/issues/28")]
        public void TestMutationHookBeforeCreateChangeNonNullableField()
        {
            var beforeCreate = Substitute.For<Action<TestUserContext, Person>>();
            beforeCreate.When(a => a(Arg.Any<TestUserContext>(), Arg.Any<Person>()))
                .Do(x => x.ArgAt<Person>(1).HireDate = DateTime.Now);

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.HireDate)
                        .Editable();
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>(),
                beforeCreate: beforeCreate);

            void QueryBuilder(Query<TestUserContext> query)
            {
                query.Connection(personLoader, "people");
            }

            void MutationBuilder(Mutation<TestUserContext> mutation)
            {
                mutation.SubmitField(personLoader, "people");
            }

            void Checks(IDataContext dataContext)
            {
                beforeCreate.Received().Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: -1,
                        }]
                    }) {
                        people {
                            id 
                            payload {
                                hireDate
                            }
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: -1,
                        }]
                    }
                }",
                Checks);
        }

        [Test(Description = "Update (should not call BeforeCreate and should call BeforeUpdate)")]
        public void TestMutationHookUpdateShouldNotCallBeforeCreateAndShouldCallBeforeUpdate()
        {
            var beforeCreate = Substitute.For<Action<TestUserContext, Person>>();
            var beforeUpdate = Substitute.For<Action<TestUserContext, Person>>();

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Editable();
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>(),
                beforeCreate: beforeCreate,
                beforeUpdate: beforeUpdate);

            void QueryBuilder(Query<TestUserContext> query)
            {
                query.Connection(personLoader, "people");
            }

            void MutationBuilder(Mutation<TestUserContext> mutation)
            {
                mutation.SubmitField(personLoader, "people");
            }

            void Checks(IDataContext dataContext)
            {
                beforeUpdate.Received().Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
                beforeCreate.DidNotReceive().Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 1,
                            fullName: ""Test""
                        }]
                    }) {
                        people {
                            id
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: 1,
                        }]
                    }
                }",
                Checks);
        }

        [Test(Description = "Update (should not call BeforeCreate and should call BeforeUpdate and update salary)")]
        public void TestMutationHookUpdateShouldNotCallBeforeCreateAndShouldCallBeforeUpdateAndUpdateField()
        {
            var beforeCreate = Substitute.For<Action<TestUserContext, Person>>();

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType(
                builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName).Editable();
                    builder.Field(p => p.Salary).Default(x => x.Salary);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>(),
                beforeCreate: beforeCreate,
                beforeUpdate: (executionContext, person) => { person.Salary = 10; });

            void QueryBuilder(Query<TestUserContext> query)
            {
                query.Connection(personLoader, "people");
            }

            void MutationBuilder(Mutation<TestUserContext> mutation)
            {
                mutation.SubmitField(personLoader, "people");
            }

            void Checks(IDataContext dataContext)
            {
                beforeCreate.DidNotReceive().Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>());
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 1,
                            fullName: ""Test""
                        }]
                    }) {
                        people {
                            id
                            payload {
                                salary
                            }
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: 1,
                            payload: {
                                salary: 10.0
                            }
                        }]
                    }
                }",
                Checks);
        }

        [Test(Description = "Insert should call AfterSave method")]
        public void TestMutationHookInsertShouldCallAfterSaveMethod()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>());

            void QueryBuilder(Query<TestUserContext> query)
            {
                query.Connection(personLoader, "people");
            }

            void MutationBuilder(Mutation<TestUserContext> mutation)
            {
                mutation.SubmitField(personLoader, "people");
            }

            var afterSave = Substitute.For<Func<TestUserContext, IEnumerable<object>, Task<IEnumerable<object>>>>();

            afterSave
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<object>>())
                .Returns(Task.FromResult(Enumerable.Empty<object>()));

            void Checks(IDataContext dataContext)
            {
                afterSave
                    .Received()
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Is<IEnumerable<object>>(entities =>
                        entities.Count() == 1 && typeof(Person).IsAssignableFrom(entities.First().GetType())
                        && ((Person)entities.First()).FullName == "Test"
                        && ((Person)entities.First()).Id == 7));
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: -1,
                            fullName: ""Test""
                        }]
                    }) {
                        people {
                            id
                            payload {
                                id
                                fullName
                            }
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: -1,
                            payload: {
                                id: 7,
                                fullName: ""Test""
                            }
                        }]
                    }
                }",
                Checks,
                null,
                afterSave);
        }

        [Test(Description = "Insert should return entities from AfterSave method")]
        public void TestMutationHookInsertShouldReturnEntitiesFromAfterSaveMethod()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>());

            var unitLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Unit, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.Name);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Unit>());

            void QueryBuilder(Query<TestUserContext> query)
            {
                query.Connection(personLoader, "people");
            }

            void MutationBuilder(Mutation<TestUserContext> mutation)
            {
                mutation.SubmitField(personLoader, "people");
                mutation.SubmitField(unitLoader, "units");
            }

            var afterSave = Substitute.For<Func<TestUserContext, IEnumerable<object>, Task<IEnumerable<object>>>>();

            afterSave
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<object>>())
                .Returns(callInfo =>
                    Task.FromResult<IEnumerable<object>>(new Unit[]
                    {
                            new Unit
                            {
                                Id = 0,
                                Name = "New Unit",
                            },
                    }));

            void Checks(IDataContext dataContext)
            {
                afterSave
                    .Received()
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Is<IEnumerable<object>>(entities =>
                        entities.Count() == 1 && typeof(Person).IsAssignableFrom(entities.First().GetType())
                        && ((Person)entities.First()).FullName == "Test"
                        && ((Person)entities.First()).Id == 7));

                dataContext
                    .Received()
                    .AddRange(Arg.Is<IEnumerable<Unit>>(u => u.Count() == 1 && u.First().Id == 3 && u.First().Name == "New Unit"));
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: -1,
                            fullName: ""Test""
                        }]
                    }) {
                        people {
                            id
                            payload {
                                id
                                fullName
                            }
                        }
                        units {
                            id
                            payload{
                                id
                                name
                            }
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: -1,
                            payload: {
                                id: 7,
                                fullName: ""Test""
                            }
                        }],
                        units: [{
                            id: 3,
                            payload: {
                                id: 3,
                                name: ""New Unit""
                            }
                        }]
                    }
                }",
                Checks,
                null,
                afterSave);
        }

        [Test(Description = "Update should return entities from AfterSave method")]
        public void TestMutationHookUpdateShouldReturnEntitiesFromAfterSaveMethod()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName).Editable();
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>());

            var unitLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Unit, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.Name);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Unit>());

            void QueryBuilder(Query<TestUserContext> query)
            {
                query.Connection(personLoader, "people");
            }

            void MutationBuilder(Mutation<TestUserContext> mutation)
            {
                mutation.SubmitField(personLoader, "people");
                mutation.SubmitField(unitLoader, "units");
            }

            var afterSave = Substitute.For<Func<TestUserContext, IEnumerable<object>, Task<IEnumerable<object>>>>();

            afterSave
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<object>>())
                .Returns(callInfo =>
                {
                    var person = callInfo.ArgAt<IEnumerable<object>>(1)
                        .Cast<Person>()
                        .First();

                    var unit = callInfo.ArgAt<TestUserContext>(0).DataContext.GetQueryable<Unit>()
                        .Where(u => u.Id == person.UnitId)
                        .First();
                    unit.Name = "New Unit";
                    return Task.FromResult<IEnumerable<object>>(new[] { unit });
                });

            void Checks(IDataContext dataContext)
            {
                afterSave
                    .Received()
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Is<IEnumerable<object>>(entities =>
                        entities.Count() == 1 && typeof(Person).IsAssignableFrom(entities.First().GetType())
                        && ((Person)entities.First()).FullName == "Test"
                        && ((Person)entities.First()).Id == 1));
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 1,
                            fullName: ""Test""
                        }]
                    }) {
                        people {
                            id
                            payload {
                                id
                                fullName
                            }
                        }
                        units {
                            id
                            payload{
                                id
                                name
                            }
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: 1,
                            payload: {
                                id: 1,
                                fullName: ""Test""
                            }
                        }],
                        units: [{
                            id: 1,
                            payload: {
                                id: 1,
                                name: ""New Unit""
                            }
                        }]
                    }
                }",
                Checks,
                null,
                afterSave);
        }
    }
}
