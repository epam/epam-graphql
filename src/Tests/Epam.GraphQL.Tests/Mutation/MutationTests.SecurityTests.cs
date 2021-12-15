// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Linq;
using System.Threading.Tasks;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using GraphQL;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Mutation
{
    public partial class MutationTests
    {
        [Test(Description = "Insert/FromLoader/SingleOrDefault (unathorized to view associated parent item)")]
        public void TestSecurityInsertFromLoaderSingleOrDefaultUnathorizedToViewAssociatedParentItem()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Editable();
                    builder.Field(p => p.ManagerId);
                    builder.Field("manager")
                        .FromLoader<Person>(builder.GetType(), (p, m) => p.ManagerId == m.Id, Enums.RelationType.Association)
                        .SingleOrDefault();
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>(),
                applySecurityFilter: (_, query) => query.Where(p => p.Id != 1));

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
                            id: -1,
                            managerId: 1
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
                null);
        }

        [Test(Description = "Update/FromLoader/SingleOrDefault (unathorized to view associated parent item)")]
        public void TestSecurityUpdateFromLoaderSingleOrDefaultUnathorizedToViewAssociatedParentItem()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Editable();
                    builder.Field(p => p.ManagerId);
                    builder.Field("manager")
                        .FromLoader<Person>(builder.GetType(), (p, m) => p.ManagerId == m.Id, Enums.RelationType.Association)
                        .SingleOrDefault();
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>(),
                applySecurityFilter: (_, query) => query.Where(p => p.Id != 1));

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

        [Test(Description = "Insert/FromLoader/SingleOrDefault (unathorized to view aggregate parent item)")]
        public void TestSecurityInsertFromLoaderSingleOrDefaultUnathorizedToViewAggregatedParentItem()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Editable();
                    builder.Field(p => p.ManagerId);
                    builder.Field("manager")
                        .FromLoader<Person>(builder.GetType(), (p, m) => p.ManagerId == m.Id, Enums.RelationType.Aggregation)
                        .SingleOrDefault();
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>(),
                applySecurityFilter: (_, query) => query.Where(p => p.Id != 1));

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
                "Cannot create entity: Unauthorized.",
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: -1,
                            managerId: 1
                            fullName: ""Test""
                        }]
                    }) {
                        people {
                            id
                        }
                    }
                }");
        }

        [Test(Description = "Update/FromLoader/SingleOrDefault (unathorized to view aggregate parent item)")]
        public void TestSecurityUpdateFromLoaderSingleOrDefaultUnathorizedToViewAggregatedParentItem()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Editable();
                    builder.Field(p => p.ManagerId);
                    builder.Field("manager")
                        .FromLoader<Person>(builder.GetType(), (p, m) => p.ManagerId == m.Id, Enums.RelationType.Aggregation)
                        .SingleOrDefault();
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>(),
                applySecurityFilter: (_, query) => query.Where(p => p.Id != 1));

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
                "Cannot update entity: Unauthorized.",
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 2,
                            fullName: ""Test""
                        }]
                    }) {
                        people {
                            id
                        }
                    }
                }");
        }

        [Test(Description = "Insert/FromLoader (unathorized to view associated parent item)")]
        public void TestSecurityInsertFromLoaderUnathorizedToViewAssociatedParentItem()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Editable();
                    builder.Field(p => p.ManagerId);
                    builder.Field("subordinates")
                        .FromLoader<Person>(builder.GetType(), (p, s) => p.Id == s.ManagerId, Enums.RelationType.Association);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>(),
                applySecurityFilter: (_, query) => query.Where(p => p.Id != 1));

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
                            id: -1,
                            managerId: 1
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
                null);
        }

        [Test(Description = "Update/FromLoader (unathorized to view associated parent item)")]
        public void TestSecurityUpdateFromLoaderUnathorizedToViewAssociatedParentItem()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Editable();
                    builder.Field(p => p.ManagerId);
                    builder.Field("subordinates")
                        .FromLoader<Person>(builder.GetType(), (p, s) => p.Id == s.ManagerId, Enums.RelationType.Association);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>(),
                applySecurityFilter: (_, query) => query.Where(p => p.Id != 1));

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

        [Test(Description = "Insert/FromLoader (unathorized to view aggregate parent item)")]
        public void TestSecurityInsertFromLoaderUnathorizedToViewAggregatedParentItem()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Editable();
                    builder.Field(p => p.ManagerId);
                    builder.Field("subordinates")
                        .FromLoader<Person>(builder.GetType(), (p, s) => p.Id == s.ManagerId, Enums.RelationType.Aggregation);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>(),
                applySecurityFilter: (_, query) => query.Where(p => p.Id != 1));

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
                "Cannot create entity: Unauthorized.",
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: -1,
                            managerId: 1
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
                }");
        }

        [Test(Description = "Update/FromLoader (unathorized to view aggregate parent item)")]
        public void TestSecurityUpdateFromLoaderUnathorizedToViewAggregatedParentItem()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Editable();
                    builder.Field(p => p.ManagerId);
                    builder.Field("subordinates")
                        .FromLoader<Person>(builder.GetType(), (p, s) => p.Id == s.ManagerId, Enums.RelationType.Aggregation);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>(),
                applySecurityFilter: (_, query) => query.Where(p => p.Id != 1));

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
                "Cannot update entity: Unauthorized.",
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 2,
                            fullName: ""Test""
                        }]
                    }) {
                        people {
                            id
                        }
                    }
                }");
        }

        [Test(Description = "Insert/FromLoader/AsConnection (unathorized to view associated parent item)")]
        public void TestSecurityInsertFromLoaderAsConnectionUnathorizedToViewAssociatedParentItem()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Editable();
                    builder.Field(p => p.ManagerId);
                    builder.Field("subordinates")
                        .FromLoader<Person>(builder.GetType(), (p, s) => p.Id == s.ManagerId, Enums.RelationType.Association)
                        .AsConnection();
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>(),
                applySecurityFilter: (_, query) => query.Where(p => p.Id != 1));

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
                            id: -1,
                            managerId: 1
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
                null);
        }

        [Test(Description = "Update/FromLoader/AsConnection (unathorized to view associated parent item)")]
        public void TestSecurityUpdateFromLoaderAsConnectionUnathorizedToViewAssociatedParentItem()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Editable();
                    builder.Field(p => p.ManagerId);
                    builder.Field("subordinates")
                        .FromLoader<Person>(builder.GetType(), (p, s) => p.Id == s.ManagerId, Enums.RelationType.Association)
                        .AsConnection();
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>(),
                applySecurityFilter: (_, query) => query.Where(p => p.Id != 1));

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

        [Test(Description = "Insert/FromLoader/AsConnection (unathorized to view aggregate parent item)")]
        public void TestSecurityInsertFromLoaderAsConnectionUnathorizedToViewAggregatedParentItem()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Editable();
                    builder.Field(p => p.ManagerId);
                    builder.Field("subordinates")
                        .FromLoader<Person>(builder.GetType(), (p, s) => p.Id == s.ManagerId, Enums.RelationType.Aggregation)
                        .AsConnection();
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>(),
                applySecurityFilter: (_, query) => query.Where(p => p.Id != 1));

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
                "Cannot create entity: Unauthorized.",
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: -1,
                            managerId: 1
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
                }");
        }

        [Test(Description = "Update/FromLoader/AsConnection (unathorized to view aggregate parent item)")]
        public void TestSecurityUpdateFromLoaderAsConnectionUnathorizedToViewAggregatedParentItem()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Editable();
                    builder.Field(p => p.ManagerId);
                    builder.Field("subordinates")
                        .FromLoader<Person>(builder.GetType(), (p, s) => p.Id == s.ManagerId, Enums.RelationType.Aggregation)
                        .AsConnection();
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>(),
                applySecurityFilter: (_, query) => query.Where(p => p.Id != 1));

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
                "Cannot update entity: Unauthorized.",
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 2,
                            fullName: ""Test""
                        }]
                    }) {
                        people {
                            id
                        }
                    }
                }");
        }

        [Test(Description = "Insert (canSaveAsync returns false)")]
        public void TestSecurityInsertCanSaveAsyncReturnsFalse()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Editable();
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>(),
                canSaveAsync: (_, entity, isNew) => Task.FromResult(false));

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
                "Cannot create entity: Unauthorized.",
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
                }");
        }

        [Test(Description = "Update (canSaveAsync returns false)")]
        public void TestSecurityUpdateCanSaveAsyncReturnsFalse()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Editable();
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>(),
                canSaveAsync: (_, entity, isNew) => Task.FromResult(false));

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
                "Cannot update entity: Unauthorized.",
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
    }
}
