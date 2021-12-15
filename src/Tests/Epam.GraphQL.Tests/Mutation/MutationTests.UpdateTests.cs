// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using GraphQL;
using GraphQL.Validation.Errors;
using NSubstitute;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Mutation
{
    public partial class MutationTests
    {
        [Test(Description = "Update (editable)")]
        public void TestUpdateEditable()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Editable();
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
                        }]
                    }
                }",
                null);
        }

        [Test(Description = "Test for https://git.epam.com/epm-ppa/epam-graphql/-/issues/84")]
        public void TestUpdateEditableRenamedField()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("name", p => p.FullName)
                        .Editable();
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

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 1,
                            name: ""Test""
                        }]
                    }) {
                        people {
                            id
                            payload {
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
                                name: ""Test""
                            }
                        }]
                    }
                }",
                null);
        }

        [Test(Description = "Update (absent mandatory field)")]
        public void TestUpdateAbsentMandatoryField()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Editable();
                    builder.Field(p => p.Salary);
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
                                id: 1,
                                fullName: ""Test"",
                                salary: 4015.69
                            }
                        }]
                    }
                }",
                null);
        }

        [Test]
        public void TestUpdateAbsentNotMandatoryField()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Editable();
                    builder.Field(p => p.Salary)
                        .Editable();
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
                                id: 1,
                                fullName: ""Test"",
                                salary: 4015.69
                            }
                        }]
                    }
                }",
                null);
        }

        [Test(Description = "Update (editable if success)")]
        public void TestUpdateEditableIfSuccess()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .EditableIf(change => change.Entity.Id != 1, _ => string.Empty);
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

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 2,
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
                            id: 2,
                            payload: {
                                id: 2,
                                fullName: ""Test""
                            }
                        }]
                    }
                }",
                null);
        }

        [Test]
        public void TestUpdateEditableIfSuccessAbsentNotMandatoryField()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .EditableIf(change => change.Entity.Id != 1, _ => string.Empty);
                    builder.Field(p => p.Salary)
                        .EditableIf(change => change.Entity.Id != 1, _ => string.Empty);
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

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 2,
                            fullName: ""Test""
                        }]
                    }) {
                        people {
                            id
                            payload {
                                id
                                fullName
                                salary
                            }
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: 2,
                            payload: {
                                id: 2,
                                fullName: ""Test"",
                                salary: 2381.91
                            }
                        }]
                    }
                }",
                null);
        }

        [Test(Description = "Update (editable if success, batcher)")]
        public void TestUpdateBatchedEditableIfSuccess()
        {
            Func<IEnumerable<Person>, IDictionary<Person, Unit>> batchFunc = null;
            void BeforeExecute()
            {
                batchFunc = Substitute.For<Func<IEnumerable<Person>, IDictionary<Person, Unit>>>();
                batchFunc
                    .Invoke(Arg.Any<IEnumerable<Person>>())
                    .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(0).ToDictionary(p => p, p => p.Unit));
            }

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .BatchedEditableIf(batchFunc, change => change.BatchEntity.Id == 1);
                    builder.Field(p => p.Salary)
                        .BatchedEditableIf(batchFunc, change => change.BatchEntity.Id == 1);
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

            void Checks(IDataContext context)
            {
                batchFunc.Received(1)
                    .Invoke(Arg.Any<IEnumerable<Person>>());
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 1,
                            fullName: ""Test""
                            salary: 0
                        },{
                            id: 2,
                            fullName: ""Test""
                        }]
                    }) {
                        people {
                            id
                            payload {
                                id
                                fullName
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
                                id: 1,
                                fullName: ""Test"",
                                salary: 0.00
                            }
                        },{
                            id: 2,
                            payload: {
                                id: 2,
                                fullName: ""Test"",
                                salary: 2381.91
                            }
                        }]
                    }
                }",
                Checks,
                BeforeExecute);
        }

        [Test(Description = "Update (editable if success, context, batcher)")]
        public void TestUpdateBatchedEditableIfWIthContextSuccess()
        {
            Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, Unit>> batchFunc = null;
            void BeforeExecute()
            {
                batchFunc = Substitute.For<Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, Unit>>>();
                batchFunc
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>())
                    .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(p => p, p => p.Unit));
            }

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .BatchedEditableIf(batchFunc, change => change.BatchEntity.Id == 1);
                    builder.Field(p => p.Salary)
                        .BatchedEditableIf(batchFunc, change => change.BatchEntity.Id == 1);
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

            void Checks(IDataContext context)
            {
                batchFunc.Received(1)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>());
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 1,
                            fullName: ""Test""
                            salary: 0
                        },{
                            id: 2,
                            fullName: ""Test""
                        }]
                    }) {
                        people {
                            id
                            payload {
                                id
                                fullName
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
                                id: 1,
                                fullName: ""Test"",
                                salary: 0.00
                            }
                        },{
                            id: 2,
                            payload: {
                                id: 2,
                                fullName: ""Test"",
                                salary: 2381.91
                            }
                        }]
                    }
                }",
                Checks,
                BeforeExecute);
        }

        [Test(Description = "Update (absent mandatory for update field)")]
        public void TestUpdateAbsentMandatoryForUpdateField()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Editable();
                    builder.Field(p => p.Salary)
                        .MandatoryForUpdate()
                        .Editable();
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

            TestMutationError(
                QueryBuilder,
                MutationBuilder,
                typeof(ArgumentsOfCorrectTypeError),
                "Argument \"payload\" has invalid value {people: [{id: 1, fullName: \"Test\"}]}.\n" +
                "In field \"people\": In element #1: In field \"salary\": Expected \"Decimal!\", found null.",
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
                }");
        }

        [Test(Description = "Update (item doesn't exist)")]
        public void TestUpdateItemDoesNotExist()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Editable();
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

            TestMutationError(
                QueryBuilder,
                MutationBuilder,
                typeof(ExecutionError),
                "Cannot update entity: Entity was not found (type: Person, id: 8).",
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 8,
                            fullName: ""Test""
                        }]
                    }) {
                        people {
                            id
                        }
                    }
                }");
        }

        [Test(Description = "Update (applySecurityFilter returns empty set)")]
        public void TestUpdateApplySecurityFilterReturnsEmptySet()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Editable();
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>(),
                applySecurityFilter: (context, query) => query.Where(p => false));

            void QueryBuilder(Query<TestUserContext> query)
            {
                query.Connection(personLoader, "people");
            }

            void MutationBuilder(Mutation<TestUserContext> mutation)
            {
                mutation.SubmitField(personLoader, "people");
            }

            TestMutationError(
                QueryBuilder,
                MutationBuilder,
                typeof(ExecutionError),
                "Cannot update entity: Entity was not found (type: Person, id: 1).",
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
                }");
        }

        [Test(Description = "Update (duplicate id)")]
        public void TestUpdateDuplicateId()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Editable();
                },
                getBaseQuery: context =>
                {
                    var dataSet = context.DataContext.GetQueryable<Person>();
                    return dataSet.Concat(dataSet);
                });

            void QueryBuilder(Query<TestUserContext> query)
            {
                query.Connection(personLoader, "people");
            }

            void MutationBuilder(Mutation<TestUserContext> mutation)
            {
                mutation.SubmitField(personLoader, "people");
            }

            TestMutationError(
                QueryBuilder,
                MutationBuilder,
                typeof(ExecutionError),
                "Cannot update entity: More than one entity was found (type: Person: id = 1).",
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
                }");
        }

        [Test(Description = "Update (editable if failure)")]
        public void TestUpdateEditableIfFailure()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .EditableIf(change => change.Entity.Id != 1, _ => "Id != 1.");
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

            TestMutationError(
                QueryBuilder,
                MutationBuilder,
                typeof(ExecutionError),
                "Cannot update entity: Cannot change field `fullName` of entity (type: Person, id: 1): Id != 1.",
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
                }");
        }

        /// <summary>
        /// Test for https://git.epam.com/epm-ppa/epam-graphql/-/issues/11.
        /// Mutation of foreign key of entity with nested identifiable loader throws exception.
        /// </summary>
        [Test]
        public void TestIssue11()
        {
            var unitLoader = GraphQLTypeBuilder.CreateIdentifiableLoaderType<Unit, int, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Unit>(),
                idExpression: u => u.Id);

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.UnitId)
                        .Editable();
                    builder.Field("unit")
                        .FromLoader<Unit>(unitLoader, (p, u) => p.UnitId == u.Id)
                        .SingleOrDefault();
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

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 1,
                            unitId: null
                        }]
                    }) {
                        people {
                            id
                            payload {
                                id
                                unitId
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
                                unitId: null
                            }
                        }]
                    }
                }",
                null);
        }
    }
}
