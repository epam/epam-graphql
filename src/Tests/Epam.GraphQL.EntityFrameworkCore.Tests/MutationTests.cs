// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using Epam.GraphQL.Enums;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NUnit.Framework;

namespace Epam.GraphQL.EntityFrameworkCore.Tests
{
    [TestFixture]
    public class MutationTests : BaseTests
    {
        /// <summary>
        /// Test for https://git.epam.com/epm-ppa/epam-graphql/-/issues/21 (Submit mutation fails if updated entity is not queried).
        /// </summary>
        [Test]
        public void TestInsertDoNotQueryPayload()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName);
                },
                getBaseQuery: context => context.DbContext.People);

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
                }");
        }

        /// <summary>
        /// Test for https://git.epam.com/epm-ppa/epam-graphql/-/issues/21 (Submit mutation fails if updated entity is not queried).
        /// </summary>
        [Test]
        public void TestInsertDoNotQueryIdField()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName);
                },
                getBaseQuery: context => context.DbContext.People);

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
                            fullName: ""Test""
                        }]
                    }) {
                        people {
                            id
                            payload {
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
                                fullName: ""Test""
                            }
                        }]
                    }
                }");
        }

        [Test]
        public void TestInsertCustomSaveNavigationProperty()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("departmentName", p => p.Unit.Name)
                        .OnWrite((ctx, person, value) =>
                        {
                            person.Unit = new Unit
                            {
                                Name = value,
                            };
                        });
                },
                getBaseQuery: context => context.DbContext.People);

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
                            departmentName: ""Test"",
                        }]
                    }) {
                        people {
                            id
                            payload {
                                departmentName
                            }
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: -1,
                            payload: {
                                departmentName: ""Test""
                            }
                        }]
                    }
                }");
        }

        [Test]
        [Ignore("TODO: figure out and fix why Reload doesn't reload entities' id from db")]
        public void TestInsertCustomSaveNavigationPropertyWithBatcher()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("departmentName")
                        .FromBatch(p => p.Id, (ctx, ids) => Queryable.Where(ctx.DbContext.People, u => ids.Contains(u.Id))
                            .Select(x => new
                            {
                                x.Unit.Name,
                                x.Id,
                            })
                            .ToDictionary(u => u.Id, u => u.Name))
                        .OnWrite((ctx, person, value) =>
                        {
                            person.Unit = new Unit
                            {
                                Name = value,
                            };
                        });
                },
                getBaseQuery: context => context.DbContext.People);

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
                            departmentName: ""Test"",
                        }]
                    }) {
                        people {
                            id
                            payload {
                                departmentName
                            }
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: -1,
                            payload: {
                                departmentName: ""Test""
                            }
                        }]
                    }
                }");
        }

        /// <summary>
        /// Test for https://git.epam.com/epm-ppa/epam-graphql/-/issues/21 (Submit mutation fails if updated entity is not queried).
        /// </summary>
        [Test]
        [Ignore("TODO: figure out and fix why Reload doesn't reload entities' id from db")]
        public void TestInsertMasterDetailDoNotQueryDetails()
        {
            Type personLoader = null;
            var unitLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Unit, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.Name);
                    builder.Field("people")
                        .FromLoader<Person>(personLoader, (u, p) => u.Id == p.UnitId, RelationType.Aggregation, p => p.Unit);
                },
                getBaseQuery: context => context.DbContext.Units);

            personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName);
                    builder.Field(p => p.UnitId).ReferencesTo(unitLoader, u => u.Id, p => p.Unit, RelationType.Aggregation);
                    builder.Field("unit")
                        .FromLoader<Unit>(unitLoader, (p, u) => p.UnitId == u.Id, RelationType.Aggregation);
                },
                getBaseQuery: context => context.DbContext.People);

            void QueryBuilder(Query<TestUserContext> query)
            {
                query.Connection(personLoader, "people");
                query.Connection(unitLoader, "units");
            }

            void MutationBuilder(Mutation<TestUserContext> mutation)
            {
                mutation.SubmitField(personLoader, "people");
                mutation.SubmitField(unitLoader, "units");
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: -1,
                            fullName: ""Child"",
                            unitId: -2
                        }],
                        units: [{
                            id: -2,
                            name: ""Parent""
                        }]
                    }) {
                        people {
                            id
                            payload {
                                id
                                fullName
                                unitId
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
                                fullName: ""Child"",
                                unitId: 3
                            }
                        }]
                    }
                }");
        }

        [Test]
        [Ignore("Switch on after MutableLoader rework")]
        public void TestUpdateCustomSaveNavigationProperty()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("departmentName", p => p.Unit.Name)
                        .Editable()
                        .OnWrite((ctx, person, value) => person.Unit.Name = value); // TODO test without Editable call
                },
                getBaseQuery: context => context.DbContext.People);

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
                            departmentName: ""Test"",
                        }]
                    }) {
                        people {
                            id
                            payload {
                                departmentName
                            }
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: 1,
                            payload: {
                                departmentName: ""Test"",
                            }
                        }]
                    }
                }");
        }

        /// <summary>
        /// Test for https://git.epam.com/epm-ppa/epam-graphql/-/issues/104 (Submit mutation for two entities with 1-2-1 relationship fails).
        /// </summary>
        [Test]
        public void TestInsertEntitiesWithOneToOneRelationshipDirectNavigationProperty()
        {
            var peopleSettingsLoader = GraphQLTypeBuilder.CreateMutableLoaderType<PersonSettings, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.NickName);
                },
                getBaseQuery: context => context.DbContext.PeopleSettings);

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName);
                    builder.Field("settings")
                        .FromLoader<PersonSettings>(peopleSettingsLoader, (p, s) => p.Id == s.Id, RelationType.Association, s => s.Person);
                },
                getBaseQuery: context => context.DbContext.People);

            void QueryBuilder(Query<TestUserContext> query)
            {
                query.Connection(personLoader, "people");
                query.Connection(peopleSettingsLoader, "settings");
            }

            void MutationBuilder(Mutation<TestUserContext> mutation)
            {
                mutation.SubmitField(personLoader, "people");
                mutation.SubmitField(peopleSettingsLoader, "settings");
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: -1,
                            fullName: ""Child"",
                        }],
                        settings: [{
                            id: -1,
                            nickName: ""Test""
                        }]
                    }) {
                        people {
                            id
                        }
                        settings {
                            id
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: -1,
                        }],
                        settings: [{
                            id: -1,
                        }]
                    }
                }");
        }

        /// <summary>
        /// Test for https://git.epam.com/epm-ppa/epam-graphql/-/issues/104 (Submit mutation for two entities with 1-2-1 relationship fails).
        /// </summary>
        [Test]
        public void TestInsertEntitiesWithOneToOneRelationshipReverseNavigationProperty()
        {
            var peopleSettingsLoader = GraphQLTypeBuilder.CreateMutableLoaderType<PersonSettings, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.NickName);
                },
                getBaseQuery: context => context.DbContext.PeopleSettings);

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName);
                    builder.Field("settings")
                        .FromLoader<PersonSettings>(
                            peopleSettingsLoader,
                            (p, s) => p.Id == s.Id,
                            RelationType.Association,
                            s => s.Person);
                },
                getBaseQuery: context => context.DbContext.People);

            void QueryBuilder(Query<TestUserContext> query)
            {
                query.Connection(personLoader, "people");
                query.Connection(peopleSettingsLoader, "settings");
            }

            void MutationBuilder(Mutation<TestUserContext> mutation)
            {
                mutation.SubmitField(personLoader, "people");
                mutation.SubmitField(peopleSettingsLoader, "settings");
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: -1,
                            fullName: ""Child"",
                        }],
                        settings: [{
                            id: -1,
                            nickName: ""Test""
                        }]
                    }) {
                        people {
                            id
                        }
                        settings {
                            id
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: -1,
                        }],
                        settings: [{
                            id: -1,
                        }]
                    }
                }");
        }
    }
}
