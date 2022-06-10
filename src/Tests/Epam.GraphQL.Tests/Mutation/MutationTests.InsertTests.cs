// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using Epam.Contracts.Models;
using Epam.GraphQL.Enums;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using GraphQL;
using NSubstitute;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Mutation
{
    public partial class MutationTests
    {
        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            {
                var master = new List<HasGuidId>();

                DataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<Guid, HasGuidId>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<Guid, HasGuidId>>>(0).ToAsyncEnumerable());
                DataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<Guid, HasGuidIdDetails>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<Guid, HasGuidIdDetails>>>(0).ToAsyncEnumerable());

                DataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<Guid, HasGuidId>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<Guid, HasGuidId>>>(0));
                DataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<Guid, HasGuidIdDetails>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<Guid, HasGuidIdDetails>>>(0));

                DataContext
                    .When(dataContext => dataContext.AddRange(Arg.Any<IEnumerable<HasGuidId>>()))
                    .Do(callInfo =>
                    {
                        var range = callInfo.ArgAt<IEnumerable<HasGuidId>>(0);
                        foreach (var p in range)
                        {
                            if (p.Id == Guid.Empty)
                            {
                                p.Id = new Guid($"00000000-0000-0000-{master.Count + 1:D4}-000000000000");
                                master.Add(p);
                            }
                        }
                    });

                DataContext.GetQueryable<HasGuidId>()
                    .Returns(master.AsQueryable());
            }

            {
                var details = new List<HasGuidIdDetails>();

                DataContext
                    .When(dataContext => dataContext.AddRange(Arg.Any<IEnumerable<HasGuidIdDetails>>()))
                    .Do(callInfo =>
                    {
                        var range = callInfo.ArgAt<IEnumerable<HasGuidIdDetails>>(0);
                        foreach (var p in range)
                        {
                            if (p.Id == Guid.Empty)
                            {
                                p.Id = new Guid($"00000000-0000-0000-{details.Count + 1:D4}-000000000000");
                                details.Add(p);
                            }
                        }
                    });

                DataContext.GetQueryable<HasGuidIdDetails>()
                    .Returns(details.AsQueryable());
            }
        }

        [Test(Description = "Insert")]
        public void TestInsert()
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
                null);
        }

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

            DataContext
                .DidNotReceive()
                .GetQueryable<Person>();
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

        /// <summary>
        /// Test for https://git.epam.com/epm-ppa/epam-graphql/-/issues/18 (Fragments do not work for mutations).
        /// </summary>
        [Test]
        public void TestInsertAndQueryResultViaFragment()
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
                                ...personFragment
                            }
                        }
                    }
                }

                fragment personFragment on Person {
                    id
                    fullName
                }
                ",
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

        [Test(Description = "Insert parent/child")]
        public void TestInsertParentChild()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName);
                    builder.Field(p => p.ManagerId)
                        .ReferencesTo(builder.GetType(), person => person.Id, person => person.Manager, RelationType.Aggregation);
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
                            id: -1,
                            fullName: ""Child"",
                            managerId: -2
                        },{
                            id: -2,
                            fullName: ""Parent""
                        }]
                    }) {
                        people {
                            id
                            payload {
                                id
                                fullName
                                managerId
                            }
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: -2,
                            payload: {
                                id: 7,
                                fullName: ""Parent"",
                                managerId: null
                            }
                        }, {
                            id: -1,
                            payload: {
                                id: 8,
                                fullName: ""Child"",
                                managerId: 7
                            }
                        }]
                    }
                }",
                null);
        }

        [Test(Description = "Insert master/detail")]
        public void TestInsertMasterDetail()
        {
            Type personLoader = null;
            var unitLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Unit, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.Name);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Unit>());

            personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName);
                    builder.Field(p => p.UnitId).ReferencesTo(unitLoader, u => u.Id, p => p.Unit, RelationType.Aggregation);
                    builder.Field("unit")
                        .FromLoader<Person, Unit, TestUserContext>(unitLoader, (p, u) => p.UnitId == u.Id, RelationType.Aggregation);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>());

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
                        units {
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
                            id: -1,
                            payload: {
                                id: 7,
                                fullName: ""Child"",
                                unitId: 3
                            }
                        }],
                        units: [{
                            id: -2,
                            payload: {
                                id: 3,
                                name: ""Parent""
                            }
                        }]
                    }
                }",
                null);
        }

        /// <summary>
        /// Test for https://git.epam.com/epm-ppa/epam-graphql/-/issues/21 (Submit mutation fails if updated entity is not queried).
        /// </summary>
        [Test]
        public void TestInsertMasterDetailDoNotQueryDetails()
        {
            Type personLoader = null;
            var unitLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Unit, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.Name);
                    builder.Field("people")
                        .FromLoader<Unit, Person, TestUserContext>(personLoader, (u, p) => u.Id == p.UnitId, RelationType.Aggregation, p => p.Unit);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Unit>());

            personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName);
                    builder.Field(p => p.UnitId).ReferencesTo(unitLoader, u => u.Id, p => p.Unit, RelationType.Aggregation);
                    builder.Field("unit")
                        .FromLoader<Person, Unit, TestUserContext>(unitLoader, (p, u) => p.UnitId == u.Id, RelationType.Aggregation);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>());

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

            DataContext
                .DidNotReceive()
                .GetQueryable<Unit>();
        }

        [Test(Description = "Insert (should use default value for nullable field)")]
        public void TestInsertShouldUseDefaultValueForNullableField()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Default(p => "Test")
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
                            id: -1
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
                }",
                null);
        }

        [Test(Description = "Insert (should use default value for non-nullable field)")]
        public void TestInsertShouldUseDefaultValueForNonNullableField()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.Salary)
                        .Default(p => 0)
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
                            id: -1
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
                            id: -1,
                            payload: {
                                salary: 0.0
                            }
                        }]
                    }
                }",
                null);
        }

        [Test]
        public void TestInsertManyToOneFromLoaderSelfReference()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Editable();
                    builder.Field(p => p.ManagerId);
                    builder.Field("manager")
                        .FromLoader(builder.GetType(), (p, m) => p.ManagerId == m.Id, RelationType.Association, null, p => p.Manager);
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
                            id: -1,
                            managerId: -2
                            fullName: ""Person""
                        },{
                            id: -2,
                            fullName: ""Manager""
                        }]
                    }) {
                        people {
                            id
                            payload {
                                id
                                managerId
                            }
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: -2,
                            payload: {
                                id: 7,
                                managerId: null
                            }
                        },{
                            id: -1,
                            payload: {
                                id: 8,
                                managerId: 7
                            }
                        }]
                    }
                }",
                null);
        }

        [Test]
        public void TestInsertManyToOneFromLoaderSelfReferenceSingleOrDefault()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Editable();
                    builder.Field(p => p.ManagerId);
                    builder.Field("manager")
                        .FromLoader(builder.GetType(), (p, m) => p.ManagerId == m.Id, RelationType.Association, null, p => p.Manager)
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
                            id: -1,
                            managerId: -2
                        },{
                            id: -2
                        }]
                    }) {
                        people {
                            id
                            payload {
                                id
                                managerId
                            }
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: -2,
                            payload: {
                                id: 7,
                                managerId: null
                            }
                        },{
                            id: -1,
                            payload: {
                                id: 8,
                                managerId: 7
                            }
                        }]
                    }
                }",
                null);
        }

        [Test]
        public void TestInsertManyToOneFromLoader()
        {
            var unitLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Unit, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.Name);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Unit>());

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.UnitId);
                    builder.Field("unit")
                        .FromLoader(unitLoader, (p, u) => p.UnitId == u.Id, RelationType.Association, null, p => p.Unit);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>());

            void QueryBuilder(Query<TestUserContext> query)
            {
                query.Connection(personLoader, "people");
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
                            unitId: -2
                        }]
                        units: [{
                            id: -2
                        }]
                    }) {
                        people {
                            id
                            payload {
                                id
                                unitId
                            }
                        }
                        units {
                            id
                            payload {
                                id
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
                                unitId: 3
                            }
                        }],
                        units: [{
                            id: -2,
                            payload: {
                                id: 3
                            }
                        }]
                    }
                }",
                null);
        }

        [Test]
        public void TestInsertManyToOneFromLoaderSingleOrDefault()
        {
            var unitLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Unit, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.Name);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Unit>());

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.UnitId);
                    builder.Field("unit")
                        .FromLoader(unitLoader, (p, u) => p.UnitId == u.Id, RelationType.Association, null, p => p.Unit)
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
                mutation.SubmitField(unitLoader, "units");
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: -1,
                            unitId: -2
                        }]
                        units: [{
                            id: -2
                        }]
                    }) {
                        people {
                            id
                            payload {
                                id
                                unitId
                            }
                        }
                        units {
                            id
                            payload {
                                id
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
                                unitId: 3
                            }
                        }],
                        units: [{
                            id: -2,
                            payload: {
                                id: 3
                            }
                        }]
                    }
                }",
                null);
        }

        [Test]
        public void TestInsertOneToManyFromLoaderSelfReference()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Editable();
                    builder.Field(p => p.ManagerId);
                    builder.Field("subordinates")
                        .FromLoader<Person, Person, TestUserContext>(builder.GetType(), (manager, subordinate) => manager.Id == subordinate.ManagerId, RelationType.Association, subordinate => subordinate.Manager);
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
                            id: -1,
                            managerId: -2
                            fullName: ""Person""
                        },{
                            id: -2,
                            fullName: ""Manager""
                        }]
                    }) {
                        people {
                            id
                            payload {
                                id
                                managerId
                            }
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: -2,
                            payload: {
                                id: 7,
                                managerId: null
                            }
                        },{
                            id: -1,
                            payload: {
                                id: 8,
                                managerId: 7
                            }
                        }]
                    }
                }",
                null);
        }

        [Test]
        public void TestInsertOneToManyFromLoader()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.UnitId);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>());

            var unitLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Unit, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.Name);
                    builder.Field("people")
                        .FromLoader<Unit, Person, TestUserContext>(personLoader, (u, p) => u.Id == p.UnitId, RelationType.Association, p => p.Unit);
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

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: -1,
                            unitId: -2
                        }]
                        units: [{
                            id: -2
                        }]
                    }) {
                        people {
                            id
                            payload {
                                id
                                unitId
                            }
                        }
                        units {
                            id
                            payload {
                                id
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
                                unitId: 3
                            }
                        }],
                        units: [{
                            id: -2,
                            payload: {
                                id: 3
                            }
                        }]
                    }
                }",
                null);
        }

        [Test]
        public void TestInsertOneToOneFromLoader()
        {
            var personSettingsLoader = GraphQLTypeBuilder.CreateMutableLoaderType<PersonSettings, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<PersonSettings>());

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("settings")
                        .FromLoader<Person, PersonSettings, TestUserContext>(personSettingsLoader, (p, s) => p.Id == s.Id, navigationProperty: s => s.Person);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>());

            void QueryBuilder(Query<TestUserContext> query)
            {
                query.Connection(personLoader, "people");
            }

            void MutationBuilder(Mutation<TestUserContext> mutation)
            {
                mutation.SubmitField(personLoader, "people");
                mutation.SubmitField(personSettingsLoader, "settings");
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: -1
                        }]
                        settings: [{
                            id: -1
                        }]
                    }) {
                        people {
                            id
                            payload {
                                id
                            }
                        }
                        settings {
                            id
                            payload {
                                id
                            }
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: -1,
                            payload: {
                                id: 7
                            }
                        }],
                        settings: [{
                            id: -1,
                            payload: {
                                id: 7
                            }
                        }]
                    }
                }",
                null);
        }

        [Test]
        public void TestInsertOneToOneFromLoaderReverseNavigationProperty()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>());

            var personSettingsLoader = GraphQLTypeBuilder.CreateMutableLoaderType<PersonSettings, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("person")
                        .FromLoader(personLoader, (s, p) => s.Id == p.Id, reverseNavigationProperty: s => s.Person);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<PersonSettings>());

            void QueryBuilder(Query<TestUserContext> query)
            {
                query.Connection(personLoader, "people");
            }

            void MutationBuilder(Mutation<TestUserContext> mutation)
            {
                mutation.SubmitField(personLoader, "people");
                mutation.SubmitField(personSettingsLoader, "settings");
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: -1
                        }]
                        settings: [{
                            id: -1
                        }]
                    }) {
                        people {
                            id
                            payload {
                                id
                            }
                        }
                        settings {
                            id
                            payload {
                                id
                            }
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: -1,
                            payload: {
                                id: 7
                            }
                        }],
                        settings: [{
                            id: -1,
                            payload: {
                                id: 7
                            }
                        }]
                    }
                }",
                null);
        }

        [Test]
        public void TestInsertOneToOneFromLoaderBothNavigationProperties()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>());

            var personSettingsLoader = GraphQLTypeBuilder.CreateMutableLoaderType<PersonSettings, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("person")
                        .FromLoader(personLoader, (s, p) => s.Id == p.Id, navigationProperty: p => p.Settings, reverseNavigationProperty: s => s.Person);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<PersonSettings>());

            void QueryBuilder(Query<TestUserContext> query)
            {
                query.Connection(personLoader, "people");
            }

            void MutationBuilder(Mutation<TestUserContext> mutation)
            {
                mutation.SubmitField(personLoader, "people");
                mutation.SubmitField(personSettingsLoader, "settings");
            }

            TestMutationError(
                QueryBuilder,
                MutationBuilder,
                typeof(InvalidOperationException),
                "Cannot check fakeness of property value. Both navigationProperty and reverseNavigationProperty were provided for identity property p => p.Id of type Epam.GraphQL.Tests.TestData.Person. You must provide either navigationProperty or reverseNavigationProperty: Relation: type = PersonSettings childType = Person prop = Id, childProp Id, childNavigationProp Settings",
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: -1
                        }]
                        settings: [{
                            id: -1
                        }]
                    }) {
                        people {
                            id
                            payload {
                                id
                            }
                        }
                        settings {
                            id
                            payload {
                                id
                            }
                        }
                    }
                }");
        }

        [Test(Description = "Update (not editable)")]
        public void TestUpdateNotEditable()
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

            TestMutationError(
                QueryBuilder,
                MutationBuilder,
                typeof(ExecutionError),
                "Cannot update entity: Cannot change field `fullName` of entity (type: Person, id: 1): The field is not editable. Consider to use `Editable()` or `EditableIf(...)` methods.",
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

        [Test(Description = "Insert (absent mandatory field)")]
        public void TestInsertAbsentMandatoryField()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName);
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

            TestMutationError(
                QueryBuilder,
                MutationBuilder,
                typeof(ExecutionError),
                "Cannot create entity: Field `salary` cannot be null (type: Person, id: -1).",
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

        [Test(Description = "Insert parent/child circular reference")]
        public void TestInsertParentChildCircularReference()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName);
                    builder.Field(p => p.ManagerId)
                        .ReferencesTo(builder.GetType(), person => person.Id, person => person.Manager, RelationType.Aggregation);
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
                "Circular reference detected.",
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: -1,
                            managerId: -2
                        },{
                            id: -2,
                            managerId: -1
                        }]
                    }) {
                        people {
                            id
                        }
                    }
                }");
        }

        [Test(Description = "Insert parent/child circular self-reference")]
        public void TestInsertParentChildCircularSelfReference()
        {
            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName);
                    builder.Field(p => p.ManagerId)
                        .ReferencesTo(builder.GetType(), person => person.Id, person => person.Manager, RelationType.Aggregation);
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
                "Circular reference detected.",
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: -1,
                            managerId: -1
                        }]
                    }) {
                        people {
                            id
                        }
                    }
                }");
        }

        /// <summary>
        /// Test for the issue https://git.epam.com/epm-ppa/epam-graphql/-/issues/1.
        /// </summary>
        [Test]
        public void TestInsertGuidId()
        {
            var loader = GraphQLTypeBuilder.CreateMutableLoaderType<HasGuidId, Guid, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.Name).Editable();
                },
                getBaseQuery: context => context.DataContext.GetQueryable<HasGuidId>(),
                isFakeId: IsFakeGuid);

            void QueryBuilder(Query<TestUserContext> query)
            {
                query.Connection(loader, "parent");
            }

            void MutationBuilder(Mutation<TestUserContext> mutation)
            {
                mutation.SubmitField(loader, "parent");
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        parent: [{
                            id: ""00000000-0000-0000-0000-000000000001"",
                            name: ""Test"",
                        }]
                    }) {
                        parent {
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
                        parent: [{
                            id: ""00000000-0000-0000-0000-000000000001"",
                            payload: {
                                id: ""00000000-0000-0000-0001-000000000000"",
                                name: ""Test""
                            }
                        }]
                    }
                }");
        }

        /// <summary>
        /// Test for the issue https://git.epam.com/epm-ppa/epam-graphql/-/issues/2.
        /// </summary>
        [Test]
        public void TestInsertGuidIdMasterDetailsParentOnly()
        {
            Type childLoader = null;
            var loader = GraphQLTypeBuilder.CreateMutableLoaderType<HasGuidId, Guid, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.Name).Editable();
                    builder.Field("details").FromLoader<HasGuidId, HasGuidIdDetails, TestUserContext>(childLoader, (parent, child) => parent.Id == child.ParentId, RelationType.Aggregation, child => child.Parent);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<HasGuidId>(),
                isFakeId: IsFakeGuid);

            childLoader = GraphQLTypeBuilder.CreateMutableLoaderType<HasGuidIdDetails, Guid, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.Name).Editable();
                    builder.Field(p => p.ParentId)
                        .ReferencesTo(loader, parent => parent.Id, details => details.Parent, RelationType.Aggregation)
                        .Editable();
                },
                getBaseQuery: context => context.DataContext.GetQueryable<HasGuidIdDetails>(),
                isFakeId: IsFakeGuid);

            void QueryBuilder(Query<TestUserContext> query)
            {
                query.Connection(loader, "parent");
            }

            void MutationBuilder(Mutation<TestUserContext> mutation)
            {
                mutation.SubmitField(loader, "parents");
                mutation.SubmitField(childLoader, "children");
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        parents: [{
                            id: ""00000000-0000-0000-0000-000000000001"",
                            name: ""Test"",
                        }]
                    }) {
                        parents {
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
                        parents: [{
                            id: ""00000000-0000-0000-0000-000000000001"",
                            payload: {
                                id: ""00000000-0000-0000-0001-000000000000"",
                                name: ""Test""
                            }
                        }]
                    }
                }");
        }

        /// <summary>
        /// Test for the issue https://git.epam.com/epm-ppa/epam-graphql/-/issues/2.
        /// </summary>
        [Test]
        public void TestInsertGuidIdMasterDetailsParentAndChild()
        {
            Type childLoader = null;
            var loader = GraphQLTypeBuilder.CreateMutableLoaderType<HasGuidId, Guid, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.Name).Editable();
                    builder.Field("details").FromLoader<HasGuidId, HasGuidIdDetails, TestUserContext>(childLoader, (parent, child) => parent.Id == child.ParentId, RelationType.Aggregation, child => child.Parent);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<HasGuidId>(),
                isFakeId: IsFakeGuid);

            childLoader = GraphQLTypeBuilder.CreateMutableLoaderType<HasGuidIdDetails, Guid, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.Name).Editable();
                    builder.Field(p => p.ParentId)
                        .ReferencesTo(loader, parent => parent.Id, details => details.Parent, RelationType.Aggregation)
                        .Editable();
                },
                getBaseQuery: context => context.DataContext.GetQueryable<HasGuidIdDetails>(),
                isFakeId: IsFakeGuid);

            void QueryBuilder(Query<TestUserContext> query)
            {
                query.Connection(loader, "parent");
                query.Connection(childLoader, "details");
            }

            void MutationBuilder(Mutation<TestUserContext> mutation)
            {
                mutation.SubmitField(loader, "parents");
                mutation.SubmitField(childLoader, "children");
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        parents: [{
                            id: ""00000000-0000-0000-0000-000000000001"",
                            name: ""Parent"",
                        }]
                        children: [{
                            id: ""00000000-0000-0000-0000-000000000001"",
                            parentId: ""00000000-0000-0000-0000-000000000001"",
                            name: ""Child"",
                        }]
                    }) {
                        parents {
                            id
                            payload {
                                id
                                name
                            }
                        }
                        children {
                            id
                            payload {
                                id
                                parentId
                                name
                            }
                        }
                    }
                }",
                @"{
                    submit: {
                        parents: [{
                            id: ""00000000-0000-0000-0000-000000000001"",
                            payload: {
                                id: ""00000000-0000-0000-0001-000000000000"",
                                name: ""Parent""
                            }
                        }],
                        children: [{
                            id: ""00000000-0000-0000-0000-000000000001"",
                            payload: {
                                id: ""00000000-0000-0000-0001-000000000000"",
                                parentId: ""00000000-0000-0000-0001-000000000000"",
                                name: ""Child""
                            }
                        }]
                    }
                }");
        }

        /// <summary>
        /// Test for https://github.com/epam/epam-graphql/issues/13.
        /// </summary>
        [Test]
        public void Issue13()
        {
            Type personLoader = null;
            var unitLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Unit, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("people")
                        .FromLoader<Unit, Person, TestUserContext>(personLoader, (u, p) => u.Id == p.UnitId, RelationType.Aggregation, p => p.Unit);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Unit>());

            personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.UnitId);
                    builder.Field(p => p.FullName);
                    builder.Field("unit")
                        .FromLoader<Person, Unit, TestUserContext>(unitLoader, (p, u) => p.UnitId == u.Id, RelationType.Aggregation);
                },
                getBaseQuery: context => context.DataContext.GetQueryable<Person>());

            void QueryBuilder(Query<TestUserContext> query)
            {
                query.Connection(unitLoader, "units");
                query.Connection(personLoader, "people");
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
                             unitId: -2
                        }],
                        units: [{
                            id: -2
                        }]
                    }) {
                        people {
                            id
                            payload {
                                id
                                unitId
                            }
                        }
                        units {
                            id
                                payload {
                                id
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
                                unitId: 3
                            }
                        }],
                        units: [{
                            id: -2,
                            payload: {
                                id: 3
                            }
                        }]
                    }
                }",
                null);
        }

        private static bool IsFakeGuid(Guid guid)
        {
            var bytes = guid.ToByteArray();
            return BitConverter.ToInt64(bytes, 0) == 0 && BitConverter.ToInt16(bytes, 8) == 0 && BitConverter.ToInt64(bytes, 8) != 0;
        }

        public class HasGuidId : IHasId<Guid>
        {
            public Guid Id { get; set; }

            public string Name { get; set; }
        }

        public class HasGuidIdDetails : IHasId<Guid>
        {
            private HasGuidId _parent;

            public Guid Id { get; set; }

            public Guid? ParentId { get; set; }

            public string Name { get; set; }

            public HasGuidId Parent
            {
                get => _parent;
                set
                {
                    _parent = value;
                    ParentId = _parent?.Id;
                }
            }
        }
    }
}
