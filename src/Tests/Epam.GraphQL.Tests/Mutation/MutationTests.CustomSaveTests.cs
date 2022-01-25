// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epam.GraphQL.Loaders;
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
        [Test(Description = "Insert (custom save from batch)")]
        public void TestInsertCustomSaveFromBatch()
        {
            var save = Substitute.For<Action<TestUserContext, Person, string>>();
            var batch = Substitute.For<Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, string>>>();

            batch
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(p => p, p => FakeData.Units.FirstOrDefault(u => u.Id == p.UnitId)?.Name));

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.UnitId);
                    builder.Field(p => p.FullName);
                    builder.Field("departmentName")
                        .FromBatch(batch)
                        .OnWrite(save);
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

            void Checks(IDataContext dataContext)
            {
                save.Received().Invoke(Arg.Any<TestUserContext>(), Arg.Is<Person>(p => p.FullName == "Ilya"), Arg.Is<string>(value => value == "Test"));
                batch.Received().Invoke(Arg.Any<TestUserContext>(), Arg.Is<IEnumerable<Person>>(p => p.Single().FullName == "Ilya"));
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: -1,
                            departmentName: ""Test"",
                            unitId: 1,
                            fullName: ""Ilya""
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
                                departmentName: ""Alpha""
                            }
                        }]
                    }
                }", Checks);
        }

        [Test(Description = "Insert (custom save from batch)")]
        public void TestInsertCustomSaveFromBatchTask()
        {
            var save = Substitute.For<Func<TestUserContext, Person, string, Task>>();
            save
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>(), Arg.Any<string>())
                .Returns(_ => Task.CompletedTask);

            var batch = Substitute.For<Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, string>>>();
            batch
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(p => p, p => FakeData.Units.FirstOrDefault(u => u.Id == p.UnitId)?.Name));

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.UnitId);
                    builder.Field(p => p.FullName);
                    builder.Field("departmentName")
                        .FromBatch(batch)
                        .OnWrite(save);
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

            void Checks(IDataContext dataContext)
            {
                save.Received().Invoke(Arg.Any<TestUserContext>(), Arg.Is<Person>(p => p.FullName == "Ilya"), Arg.Is<string>(value => value == "Test"));
                batch.Received().Invoke(Arg.Any<TestUserContext>(), Arg.Is<IEnumerable<Person>>(p => p.Single().FullName == "Ilya"));
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: -1,
                            departmentName: ""Test"",
                            unitId: 1,
                            fullName: ""Ilya""
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
                                departmentName: ""Alpha""
                            }
                        }]
                    }
                }", Checks);
        }

        [Test(Description = "Update (custom save from batch)")]
        public void TestUpdateCustomSaveFromBatch()
        {
            Action<TestUserContext, Person, string> saveDepartmentName = null;
            Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, string>> departmentNameBatch = null;
            Action<TestUserContext, Person, int> saveDepartmentId = null;
            Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, int>> departmentIdBatch = null;

            void BeforeExecute()
            {
                saveDepartmentName = Substitute.For<Action<TestUserContext, Person, string>>();
                departmentNameBatch = Substitute.For<Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, string>>>();

                departmentNameBatch
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>())
                    .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(p => p, p => FakeData.Units.FirstOrDefault(u => u.Id == p.UnitId)?.Name));

                saveDepartmentId = Substitute.For<Action<TestUserContext, Person, int>>();
                departmentIdBatch = Substitute.For<Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, int>>>();

                departmentIdBatch
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>())
                    .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(p => p, p => p.UnitId ?? -1));
            }

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("departmentName")
                        .FromBatch(departmentNameBatch)
                        .OnWrite(saveDepartmentName)
                        .Editable(); // TODO test without Editable call
                    builder.Field("departmentId")
                        .FromBatch(departmentIdBatch)
                        .OnWrite(saveDepartmentId)
                        .Editable(); // TODO test without Editable call
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

            void Checks(IDataContext dataContext)
            {
                saveDepartmentName
                    .Received(1)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Is<Person>(p => p.Id == 1), Arg.Is<string>(value => value == "Test"));

                saveDepartmentName
                    .Received(1)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Is<Person>(p => p.Id == 2), Arg.Is<string>(value => value == "Test"));

                saveDepartmentName
                    .Received(2)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>(), Arg.Any<string>());

                departmentNameBatch
                    .Received(2)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Is<IEnumerable<Person>>(p => p.Count(item => item.Id == 1 || item.Id == 2) == 2));

                departmentNameBatch
                    .Received(2)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>());

                saveDepartmentId
                    .Received(1)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Is<Person>(p => p.Id == 1), Arg.Is<int>(value => value == 2));

                saveDepartmentId
                    .Received(1)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>(), Arg.Any<int>());

                departmentIdBatch
                    .Received(1)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Is<IEnumerable<Person>>(p => p.Count(item => item.Id == 1 || item.Id == 2) == 2));

                departmentIdBatch
                    .Received(1)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Is<IEnumerable<Person>>(p => p.Count(item => item.Id == 1 || item.Id == 2) == 1));

                departmentIdBatch
                    .Received(2)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>());
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 1,
                            departmentName: ""Test"",
                            departmentId: 2
                        },{
                            id: 2,
                            departmentName: ""Test""
                        }]
                    }) {
                        people {
                            id
                            payload {
                                departmentName
                                departmentId
                            }
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: 1,
                            payload: {
                                departmentName: ""Alpha"",
                                departmentId: 1
                            }
                        },{
                            id: 2,
                            payload: {
                                departmentName: ""Alpha"",
                                departmentId: 1
                            }
                        }]
                    }
                }",
                Checks,
                BeforeExecute);
        }

        [Test(Description = "Update (custom save from batch)")]
        public void TestUpdateCustomSaveFromBatchTask()
        {
            Func<TestUserContext, Person, string, Task> saveDepartmentName = null;
            Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, string>> departmentNameBatch = null;
            Action<TestUserContext, Person, int> saveDepartmentId = null;
            Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, int>> departmentIdBatch = null;

            void BeforeExecute()
            {
                saveDepartmentName = Substitute.For<Func<TestUserContext, Person, string, Task>>();
                saveDepartmentName
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>(), Arg.Any<string>())
                    .Returns(_ => Task.CompletedTask);

                departmentNameBatch = Substitute.For<Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, string>>>();
                departmentNameBatch
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>())
                    .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(p => p, p => FakeData.Units.FirstOrDefault(u => u.Id == p.UnitId)?.Name));

                saveDepartmentId = Substitute.For<Action<TestUserContext, Person, int>>();
                departmentIdBatch = Substitute.For<Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, int>>>();

                departmentIdBatch
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>())
                    .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(p => p, p => p.UnitId ?? -1));
            }

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("departmentName")
                        .FromBatch(departmentNameBatch)
                        .OnWrite(saveDepartmentName)
                        .Editable(); // TODO test without Editable call
                    builder.Field("departmentId")
                        .FromBatch(departmentIdBatch)
                        .OnWrite(saveDepartmentId)
                        .Editable(); // TODO test without Editable call
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

            void Checks(IDataContext dataContext)
            {
                saveDepartmentName
                    .Received(1)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Is<Person>(p => p.Id == 1), Arg.Is<string>(value => value == "Test"));

                saveDepartmentName
                    .Received(1)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Is<Person>(p => p.Id == 2), Arg.Is<string>(value => value == "Test"));

                saveDepartmentName
                    .Received(2)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>(), Arg.Any<string>());

                departmentNameBatch
                    .Received(2)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Is<IEnumerable<Person>>(p => p.Count(item => item.Id == 1 || item.Id == 2) == 2));

                departmentNameBatch
                    .Received(2)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>());

                saveDepartmentId
                    .Received(1)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Is<Person>(p => p.Id == 1), Arg.Is<int>(value => value == 2));

                saveDepartmentId
                    .Received(1)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>(), Arg.Any<int>());

                departmentIdBatch
                    .Received(1)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Is<IEnumerable<Person>>(p => p.Count(item => item.Id == 1 || item.Id == 2) == 2));

                departmentIdBatch
                    .Received(1)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Is<IEnumerable<Person>>(p => p.Count(item => item.Id == 1 || item.Id == 2) == 1));

                departmentIdBatch
                    .Received(2)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>());
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 1,
                            departmentName: ""Test"",
                            departmentId: 2
                        },{
                            id: 2,
                            departmentName: ""Test""
                        }]
                    }) {
                        people {
                            id
                            payload {
                                departmentName
                                departmentId
                            }
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: 1,
                            payload: {
                                departmentName: ""Alpha"",
                                departmentId: 1
                            }
                        },{
                            id: 2,
                            payload: {
                                departmentName: ""Alpha"",
                                departmentId: 1
                            }
                        }]
                    }
                }",
                Checks,
                BeforeExecute);
        }

        [Test]
        public void TestUpdateCustomSaveFromBatchAbsentMandatoryForUpdate()
        {
            Action<TestUserContext, Person, string> saveDepartmentName = null;
            Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, string>> departmentNameBatch = null;
            Action<TestUserContext, Person, int> saveDepartmentId = null;
            Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, int>> departmentIdBatch = null;

            void BeforeExecute()
            {
                saveDepartmentName = Substitute.For<Action<TestUserContext, Person, string>>();
                departmentNameBatch = Substitute.For<Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, string>>>();

                departmentNameBatch
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>())
                    .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(p => p, p => FakeData.Units.FirstOrDefault(u => u.Id == p.UnitId)?.Name));

                saveDepartmentId = Substitute.For<Action<TestUserContext, Person, int>>();
                departmentIdBatch = Substitute.For<Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, int>>>();

                departmentIdBatch
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>())
                    .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(p => p, p => p.UnitId ?? -1));
            }

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("departmentName")
                        .FromBatch(departmentNameBatch)
                        .OnWrite(saveDepartmentName)
                        .Editable(); // TODO test without Editable call
                    builder.Field("departmentId")
                        .FromBatch(departmentIdBatch)
                        .MandatoryForUpdate()
                        .OnWrite(saveDepartmentId)
                        .Editable(); // TODO test without Editable call
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

            void Checks(IDataContext dataContext)
            {
                saveDepartmentName
                    .DidNotReceive()
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>(), Arg.Any<string>());

                departmentNameBatch
                    .DidNotReceive()
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>());

                saveDepartmentId
                    .DidNotReceive()
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>(), Arg.Any<int>());

                departmentIdBatch
                    .DidNotReceive()
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>());
            }

            TestMutationError(
                QueryBuilder,
                MutationBuilder,
                typeof(ArgumentsOfCorrectTypeError),
                "Argument 'payload' has invalid value. In field 'people': [In element #1: [Missing required field 'departmentId' of type 'Int'.]]",
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 1,
                            departmentName: ""Test"",
                        }]
                    }) {
                        people {
                            id
                        }
                    }
                }",
                Checks,
                BeforeExecute);
        }

        [Test(Description = "Update (custom save from batch & select)")]
        public void TestUpdateCustomSaveFromBatchAndSelect()
        {
            Action<TestUserContext, Person, string> saveDepartmentName = null;
            Action<TestUserContext, Person, int> saveDepartmentId = null;
            Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, Unit>> batch = null;

            void BeforeExecute()
            {
                saveDepartmentName = Substitute.For<Action<TestUserContext, Person, string>>();
                saveDepartmentId = Substitute.For<Action<TestUserContext, Person, int>>();
                batch = Substitute.For<Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, Unit>>>();

                batch
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>())
                    .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(p => p, p => p.Unit));
            }

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("departmentName")
                        .FromBatch(batch, null)
                        .Select(u => u.Name)
                        .OnWrite(saveDepartmentName)
                        .Editable(); // TODO test without Editable call
                    builder.Field("departmentId")
                        .FromBatch(batch, null)
                        .Select(u => u.Id)
                        .OnWrite(saveDepartmentId)
                        .Editable(); // TODO test without Editable call
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

            void Checks(IDataContext dataContext)
            {
                saveDepartmentName
                    .Received(1)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Is<Person>(p => p.FullName == FakeData.LinoelLivermore.FullName), Arg.Is<string>(value => value == "Test"));

                saveDepartmentName
                    .Received(1)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>(), Arg.Any<string>());

                saveDepartmentId
                    .Received(1)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Is<Person>(p => p.FullName == FakeData.SophieGandley.FullName), Arg.Is<int>(value => value == 2));

                saveDepartmentId
                    .Received(1)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>(), Arg.Any<int>());

                batch
                    .Received(2)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Is<IEnumerable<Person>>(p => p.Count(item => item.Id == 1 || item.Id == 2) == 2));

                batch
                    .Received(2)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>());
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 1,
                            departmentName: ""Test""
                        },{
                            id: 2,
                            departmentId: 2
                        }]
                    }) {
                        people {
                            id
                            payload {
                                departmentName
                                departmentId
                            }
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: 1,
                            payload: {
                                departmentName: ""Alpha"",
                                departmentId: 1
                            }
                        },{
                            id: 2,
                            payload: {
                                departmentName: ""Alpha"",
                                departmentId: 1
                            }
                        }]
                    }
                }",
                Checks,
                BeforeExecute);
        }

        [Test(Description = "Update (custom save from batch, EditableIf returns true)")]
        public void TestUpdateCustomSaveFromBatchAndEditableIfReturnsTrue()
        {
            Action<TestUserContext, Person, string> save = null;
            Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, string>> batch = null;
            Func<IFieldChange<Person, string, TestUserContext>, bool> editableIf = null;

            void BeforeExecute()
            {
                save = Substitute.For<Action<TestUserContext, Person, string>>();
                batch = Substitute.For<Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, string>>>();
                editableIf = Substitute.For<Func<IFieldChange<Person, string, TestUserContext>, bool>>();

                batch
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>())
                    .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(p => p, p => FakeData.Units.FirstOrDefault(u => u.Id == p.UnitId)?.Name));

                editableIf
                    .Invoke(Arg.Any<IFieldChange<Person, string, TestUserContext>>())
                    .Returns(callInfo => true);
            }

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("departmentName")
                        .FromBatch(batch)
                        .MandatoryForUpdate()
                        .OnWrite(save)
                        .EditableIf(editableIf);
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

            void Checks(IDataContext dataContext)
            {
                save
                    .Received(1)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Is<Person>(p => p.FullName == FakeData.LinoelLivermore.FullName), Arg.Is<string>(value => value == "Test"));

                batch
                    .Received(2)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Is<IEnumerable<Person>>(p => p.Single().Id == 1));

                editableIf
                    .Received(1)
                    .Invoke(Arg.Is<IFieldChange<Person, string, TestUserContext>>(change => change.Entity.FullName == FakeData.LinoelLivermore.FullName && change.PreviousValue == "Alpha" && change.NextValue == "Test"));
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 1,
                            departmentName: ""Test""
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
                                departmentName: ""Alpha""
                            }
                        }]
                    }
                }",
                Checks,
                BeforeExecute);
        }

        [Test(Description = "Update (custom save from batch, EditableIf returns false)")]
        public void TestUpdateCustomSaveFromBatchAndEditableIfReturnsFalse()
        {
            Action<TestUserContext, Person, string> save = null;
            Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, string>> batch = null;
            Func<IFieldChange<Person, string, TestUserContext>, bool> editableIf = null;

            void BeforeExecute()
            {
                save = Substitute.For<Action<TestUserContext, Person, string>>();
                batch = Substitute.For<Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, string>>>();
                editableIf = Substitute.For<Func<IFieldChange<Person, string, TestUserContext>, bool>>();

                batch
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>())
                    .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(p => p, p => FakeData.Units.FirstOrDefault(u => u.Id == p.UnitId)?.Name));

                editableIf
                    .Invoke(Arg.Any<IFieldChange<Person, string, TestUserContext>>())
                    .Returns(callInfo => false);
            }

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("departmentName")
                        .FromBatch(batch)
                        .OnWrite(save)
                        .EditableIf(editableIf, change => "prohibited");
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

            void Checks(IDataContext dataContext)
            {
                save
                    .DidNotReceive()
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>(), Arg.Any<string>());

                batch
                    .Received(1)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Is<IEnumerable<Person>>(p => p.Single().Id == 1));

                editableIf
                    .Received(1)
                    .Invoke(Arg.Is<IFieldChange<Person, string, TestUserContext>>(change => change.Entity.FullName == FakeData.LinoelLivermore.FullName && change.PreviousValue == "Alpha" && change.NextValue == "Test"));
            }

            TestMutationError(
                QueryBuilder,
                MutationBuilder,
                typeof(ExecutionError),
                "Cannot update entity: Cannot change field `departmentName` of entity (type: Person, id: 1): prohibited",
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 1,
                            departmentName: ""Test""
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
                Checks,
                BeforeExecute);
        }

        [Test(Description = "Update (custom save from batch, batched EditableIf returns true)")]
        public void TestUpdateCustomSaveFromBatchAndBatchedEditableIfReturnsTrue()
        {
            Action<TestUserContext, Person, string> save = null;
            Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, string>> batch = null;
            Func<IBatchFieldChange<Person, string, Unit, TestUserContext>, bool> editableIf = null;
            Func<IEnumerable<Person>, IEnumerable<KeyValuePair<Person, Unit>>> batchForEditableIf = null;

            void BeforeExecute()
            {
                save = Substitute.For<Action<TestUserContext, Person, string>>();
                batch = Substitute.For<Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, string>>>();
                editableIf = Substitute.For<Func<IFieldChange<Person, string, TestUserContext>, bool>>();
                batchForEditableIf = Substitute.For<Func<IEnumerable<Person>, IEnumerable<KeyValuePair<Person, Unit>>>>();

                batch
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>())
                    .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(p => p, p => FakeData.Units.FirstOrDefault(u => u.Id == p.UnitId)?.Name));

                editableIf
                    .Invoke(Arg.Any<IBatchFieldChange<Person, string, Unit, TestUserContext>>())
                    .Returns(callInfo => callInfo.ArgAt<IBatchFieldChange<Person, string, Unit, TestUserContext>>(0).BatchEntity.Id == 1);

                batchForEditableIf
                    .Invoke(Arg.Any<IEnumerable<Person>>())
                    .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(0).ToDictionary(p => p, p => p.Unit));
            }

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("departmentName")
                        .FromBatch(batch)
                        .OnWrite(save)
                        .BatchedEditableIf(batchForEditableIf, editableIf);
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

            void Checks(IDataContext dataContext)
            {
                save
                    .Received(1)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Is<Person>(p => p.FullName == FakeData.LinoelLivermore.FullName), Arg.Is<string>(value => value == "Test"));

                save
                    .Received(1)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Is<Person>(p => p.FullName == FakeData.SophieGandley.FullName), Arg.Is<string>(value => value == "Test"));

                editableIf
                    .Received(1)
                    .Invoke(Arg.Is<IBatchFieldChange<Person, string, Unit, TestUserContext>>(change => change.Entity.FullName == FakeData.LinoelLivermore.FullName && change.PreviousValue == "Alpha" && change.NextValue == "Test" && change.BatchEntity.Name == "Alpha"));

                editableIf
                    .Received(1)
                    .Invoke(Arg.Is<IBatchFieldChange<Person, string, Unit, TestUserContext>>(change => change.Entity.FullName == FakeData.SophieGandley.FullName && change.PreviousValue == "Alpha" && change.NextValue == "Test" && change.BatchEntity.Name == "Alpha"));

                editableIf
                    .Received(1)
                    .Invoke(Arg.Is<IBatchFieldChange<Person, string, Unit, TestUserContext>>(change => change.Entity.FullName == FakeData.HannieEveritt.FullName && change.PreviousValue == "Alpha" && change.NextValue == "Test" && change.BatchEntity.Name == "Alpha"));

                batch
                    .Received(2)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Is<IEnumerable<Person>>(people => people.Select(p => p.Id).SequenceEqual(new[] { 1, 2, 3 })));

                batchForEditableIf
                    .Received(1)
                    .Invoke(Arg.Is<IEnumerable<Person>>(people => people.Select(p => p.Id).SequenceEqual(new[] { 1, 2, 3 })));
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 1,
                            departmentName: ""Test""
                        },{
                            id: 2,
                            departmentName: ""Test"",
                        },{
                            id: 3,
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
                                departmentName: ""Alpha""
                            }
                        },{
                            id: 2,
                            payload: {
                                departmentName: ""Alpha""
                            }
                        },{
                            id: 3,
                            payload: {
                                departmentName: ""Alpha""
                            }
                        }]
                    }
                }",
                Checks,
                BeforeExecute);
        }

        [Test]
        public void TestUpdateTwoFieldsWithCustomSaveFromBatchAndSelect()
        {
            Action<TestUserContext, Person, string> saveName = null;
            Action<TestUserContext, Person, int> saveId = null;
            Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, Unit>> batch = null;

            void BeforeExecute()
            {
                saveName = Substitute.For<Action<TestUserContext, Person, string>>();
                saveId = Substitute.For<Action<TestUserContext, Person, int>>();
                batch = Substitute.For<Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, Unit>>>();

                batch
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>())
                    .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(p => p, p => p.Unit.Clone()));
            }

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("departmentName")
                        .FromBatch(batch)
                        .Select(u => u.Name)
                        .OnWrite(saveName)
                        .Editable();
                    builder.Field("departmentId")
                        .FromBatch(batch)
                        .Select(u => u.Id)
                        .OnWrite(saveId)
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

            void Checks(IDataContext dataContext)
            {
                saveName
                    .Received(1)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Is<Person>(p => p.FullName == FakeData.LinoelLivermore.FullName), Arg.Is<string>(value => value == "Test"));

                saveId
                    .Received(1)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Is<Person>(p => p.FullName == FakeData.LinoelLivermore.FullName), Arg.Is<int>(value => value == 100));

                batch
                    .Received(2)
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Is<IEnumerable<Person>>(p => p.Single().Id == 1));
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 1,
                            departmentName: ""Test"",
                            departmentId: 100

                        }]
                    }) {
                        people {
                            id
                            payload {
                                departmentName
                                departmentId
                            }
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: 1,
                            payload: {
                                departmentName: ""Alpha"",
                                departmentId: 1
                            }
                        }]
                    }
                }",
                Checks,
                BeforeExecute);
        }

        [Test(Description = "Insert (custom save)")]
        public void TestInsertCustomSave()
        {
            Action<TestUserContext, Person, string> save = null;
            void BeforeExecute()
            {
                save = Substitute.For<Action<TestUserContext, Person, string>>();
            }

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .OnWrite(save);
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
                save.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Is<Person>(p => p.FullName == "Test"), Arg.Is<string>(value => value == "Test"));
                context.Received(1).GetQueryable<Person>();
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
                }",
                Checks,
                BeforeExecute);
        }

        [Test(Description = "Insert (custom save, calculated field)")]
        public void TestInsertCustomSaveCalculatedField()
        {
            Action<TestUserContext, Person, decimal> save = null;
            void BeforeExecute()
            {
                save = Substitute.For<Action<TestUserContext, Person, decimal>>();
                save.When(s => s(Arg.Any<TestUserContext>(), Arg.Any<Person>(), Arg.Any<decimal>()))
                    .Do(callInfo => callInfo.ArgAt<Person>(1).Salary = decimal.Round(callInfo.ArgAt<decimal>(2) / 0.87m));
            }

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("netSalary", p => decimal.Round(p.Salary * 0.87m))
                        .OnWrite(save);
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
                save.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Is<Person>(p => p.Salary == 115m), Arg.Is<decimal>(value => value == 100m));
                context.Received(1).GetQueryable<Person>();
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: -1,
                            netSalary: 100
                        }]
                    }) {
                        people {
                            id
                            payload {
                                netSalary
                            }
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: -1,
                            payload: {
                                netSalary: 100.00
                            }
                        }]
                    }
                }",
                Checks,
                BeforeExecute);
        }

        [Test(Description = "Insert (no custom save, calculated field)")]
        public void TestInsertNoCustomSaveCalculatedField()
        {
            Action<TestUserContext, Person, decimal> save = null;
            void BeforeExecute()
            {
                save = Substitute.For<Action<TestUserContext, Person, decimal>>();
            }

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("netSalary", p => decimal.Round(p.Salary * 0.87m));
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
                save
                    .DidNotReceive()
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>(), Arg.Any<decimal>());

                context
                    .DidNotReceive()
                    .GetQueryable<Person>();
            }

            TestMutationError(
                QueryBuilder,
                MutationBuilder,
                typeof(ArgumentsOfCorrectTypeError),
                "Argument 'payload' has invalid value. In field 'people': [In element #1: [In field 'netSalary': Unknown field.]]",
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: -1,
                            netSalary: 100
                        }]
                    }) {
                        people {
                            id
                            payload {
                                netSalary
                            }
                        }
                    }
                }",
                Checks,
                BeforeExecute);
        }

        [Test(Description = "Update (custom save)")]
        public void TestUpdateCustomSave()
        {
            Action<TestUserContext, Person, string> save = null;
            void BeforeExecute()
            {
                save = Substitute.For<Action<TestUserContext, Person, string>>();
            }

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .Editable()
                        .OnWrite(save);
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
                save.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Is<Person>(p => p.FullName == FakeData.LinoelLivermore.FullName), Arg.Is<string>(value => value == "Test"));
                context.Received(2).GetQueryable<Person>();
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
                                fullName: ""Linoel Livermore""
                            }
                        }]
                    }
                }",
                Checks,
                BeforeExecute);
        }

        [Test(Description = "Update (custom save, no Editable call)")]
        public void TestUpdateCustomSaveAndNoEditableCall()
        {
            Action<TestUserContext, Person, string> save = null;
            void BeforeExecute()
            {
                save = Substitute.For<Action<TestUserContext, Person, string>>();
            }

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.FullName)
                        .OnWrite(save);
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
                save
                    .DidNotReceive()
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>(), Arg.Any<string>());

                context
                    .Received(1)
                    .GetQueryable<Person>();
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
                            payload {
                                fullName
                            }
                        }
                    }
                }",
                Checks,
                BeforeExecute);
        }

        [Test(Description = "Update (custom save, calculated field)")]
        public void TestUpdateCustomSaveCalculatedField()
        {
            Action<TestUserContext, Person, decimal> save = null;
            void BeforeExecute()
            {
                save = Substitute.For<Action<TestUserContext, Person, decimal>>();
                save.When(s => s(Arg.Any<TestUserContext>(), Arg.Any<Person>(), Arg.Any<decimal>()))
                    .Do(callInfo => callInfo.ArgAt<Person>(1).Salary = decimal.Round(callInfo.ArgAt<decimal>(2) / 0.87m));
            }

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("netSalary", p => decimal.Round(p.Salary * 0.87m))
                        .Editable()
                        .OnWrite(save);
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
                save.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Is<Person>(p => p.Salary == 115m), Arg.Is<decimal>(value => value == 100m));
                context.Received(2).GetQueryable<Person>();
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 1,
                            netSalary: 100
                        }]
                    }) {
                        people {
                            id
                            payload {
                                netSalary
                            }
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: 1,
                            payload: {
                                netSalary: 100.00
                            }
                        }]
                    }
                }",
                Checks,
                BeforeExecute);
        }

        [Test(Description = "Update (custom save, calculated field, EditableIf returns true)")]
        public void TestCustomSaveCalculatedFieldAndEditableIfReturnsTrue()
        {
            Action<TestUserContext, Person, decimal> save = null;
            void BeforeExecute()
            {
                save = Substitute.For<Action<TestUserContext, Person, decimal>>();
                save.When(s => s(Arg.Any<TestUserContext>(), Arg.Any<Person>(), Arg.Any<decimal>()))
                    .Do(callInfo => callInfo.ArgAt<Person>(1).Salary = decimal.Round(callInfo.ArgAt<decimal>(2) / 0.87m));
            }

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("netSalary", p => decimal.Round(p.Salary * 0.87m))
                        .EditableIf(_ => true, _ => string.Empty)
                        .OnWrite(save);
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
                save.Received(1).Invoke(Arg.Any<TestUserContext>(), Arg.Is<Person>(p => p.Salary == 115m), Arg.Is<decimal>(value => value == 100m));
                context.Received(2).GetQueryable<Person>();
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 1,
                            netSalary: 100
                        }]
                    }) {
                        people {
                            id
                            payload {
                                netSalary
                            }
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: 1,
                            payload: {
                                netSalary: 100.00
                            }
                        }]
                    }
                }",
                Checks,
                BeforeExecute);
        }

        [Test(Description = "Update (custom save, calculated field, EditableIf returns false)")]
        public void TestCustomSaveCalculatedFieldAndEditableIfReturnsFalse()
        {
            Action<TestUserContext, Person, decimal> save = null;
            void BeforeExecute()
            {
                save = Substitute.For<Action<TestUserContext, Person, decimal>>();
            }

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field("salary", p => p.Salary)
                        .EditableIf(_ => false, _ => "Cannot edit salary")
                        .OnWrite(save);
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
                save
                    .DidNotReceive()
                    .Invoke(Arg.Any<TestUserContext>(), Arg.Any<Person>(), Arg.Any<decimal>());

                context
                    .Received(1)
                    .GetQueryable<Person>();
            }

            TestMutationError(
                QueryBuilder,
                MutationBuilder,
                typeof(ExecutionError),
                "Cannot update entity: Cannot change field `salary` of entity (type: Person, id: 1): Cannot edit salary",
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 1,
                            salary: 100
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
                Checks,
                BeforeExecute);
        }

        [Test]
        public void TestInsertClassCustomSaveFromBatch()
        {
            var save = Substitute.For<Action<TestUserContext, Person, TestClass>>();
            var batch = Substitute.For<Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, TestClass>>>();

            batch
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(
                    p => p,
                    p => new TestClass { UnitId = p.UnitId, ManagerId = p.ManagerId }));

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.UnitId);
                    builder.Field(p => p.ManagerId);
                    builder.Field("test")
                        .FromBatch(batch)
                        .OnWrite(save);
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

            void Checks(IDataContext dataContext)
            {
                save.Received(1).Invoke(
                    Arg.Any<TestUserContext>(),
                    Arg.Is<Person>(p => p.UnitId == 2 && p.ManagerId == 5),
                    Arg.Is<TestClass>(c => c.UnitId == 10 && c.ManagerId == 10));

                batch.Received(1).Invoke(
                    Arg.Any<TestUserContext>(),
                    Arg.Is<IEnumerable<Person>>(p => p.Single().UnitId == 2 && p.Single().ManagerId == 5));
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: -1,
                            unitId: 2,
                            managerId: 5,
                            test: {
                                unitId: 10,
                                managerId: 10
                            }
                        }]
                    }) {
                        people {
                            id
                            payload {
                                test {
                                    unitId
                                    managerId
                                }
                            }
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: -1,
                            payload: {
                                test: {
                                    unitId: 2,
                                    managerId: 5
                                }
                            }
                        }]
                    }
                }", Checks);
        }

        [Test]
        public void TestUpdateClassCustomSaveFromBatch()
        {
            var save = Substitute.For<Action<TestUserContext, Person, TestClass>>();
            var batch = Substitute.For<Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, TestClass>>>();

            batch
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(
                    p => p,
                    p => new TestClass { UnitId = p.UnitId, ManagerId = p.ManagerId }));

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.UnitId).Editable();
                    builder.Field(p => p.ManagerId).Editable();
                    builder.Field("test")
                        .FromBatch(batch)
                        .OnWrite(save)
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

            void Checks(IDataContext dataContext)
            {
                save.Received(1).Invoke(
                    Arg.Any<TestUserContext>(),
                    Arg.Is<Person>(p => p.UnitId == 2 && p.ManagerId == 5),
                    Arg.Is<TestClass>(c => c.UnitId == 10 && c.ManagerId == 10));

                batch.Received(2).Invoke(
                    Arg.Any<TestUserContext>(),
                    Arg.Is<IEnumerable<Person>>(p => p.Single().UnitId == 2 && p.Single().ManagerId == 5));
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 1,
                            unitId: 2,
                            managerId: 5,
                            test: {
                                unitId: 10,
                                managerId: 10
                            }
                        }]
                    }) {
                        people {
                            id
                            payload {
                                test {
                                    unitId
                                    managerId
                                }
                            }
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: 1,
                            payload: {
                                test: {
                                    unitId: 2,
                                    managerId: 5
                                }
                            }
                        }]
                    }
                }", Checks);
        }

        [Test]
        public void TestInsertArrayOfClassCustomSaveFromBatch()
        {
            var save = Substitute.For<Action<TestUserContext, Person, TestClass[]>>();
            var batch = Substitute.For<Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, TestClass[]>>>();

            batch
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(
                    p => p,
                    p => new[] { new TestClass { UnitId = p.UnitId, ManagerId = p.ManagerId } }));

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.UnitId);
                    builder.Field(p => p.ManagerId);
                    builder.Field("test")
                        .FromBatch(batch)
                        .OnWrite(save);
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

            void Checks(IDataContext dataContext)
            {
                save.Received(1).Invoke(
                    Arg.Any<TestUserContext>(),
                    Arg.Is<Person>(p => p.UnitId == 2 && p.ManagerId == 5),
                    Arg.Is<TestClass[]>(c => c.Single().UnitId == 10 && c.Single().ManagerId == 10));

                batch.Received(1).Invoke(
                    Arg.Any<TestUserContext>(),
                    Arg.Is<IEnumerable<Person>>(p => p.Single().UnitId == 2 && p.Single().ManagerId == 5));
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: -1,
                            unitId: 2,
                            managerId: 5,
                            test: [{
                                unitId: 10,
                                managerId: 10
                            }]
                        }]
                    }) {
                        people {
                            id
                            payload {
                                test {
                                    unitId
                                    managerId
                                }
                            }
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: -1,
                            payload: {
                                test: [{
                                    unitId: 2,
                                    managerId: 5
                                }]
                            }
                        }]
                    }
                }", Checks);
        }

        [Test]
        public void TestUpdateArrayOfClassCustomSaveFromBatch()
        {
            var save = Substitute.For<Action<TestUserContext, Person, TestClass[]>>();
            var batch = Substitute.For<Func<TestUserContext, IEnumerable<Person>, IDictionary<Person, TestClass[]>>>();

            batch
                .Invoke(Arg.Any<TestUserContext>(), Arg.Any<IEnumerable<Person>>())
                .Returns(callInfo => callInfo.ArgAt<IEnumerable<Person>>(1).ToDictionary(
                    p => p,
                    p => new[] { new TestClass { UnitId = p.UnitId, ManagerId = p.ManagerId } }));

            var personLoader = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: builder =>
                {
                    builder.Field(p => p.Id);
                    builder.Field(p => p.UnitId).Editable();
                    builder.Field(p => p.ManagerId).Editable();
                    builder.Field("test")
                        .FromBatch(batch)
                        .OnWrite(save)
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

            void Checks(IDataContext dataContext)
            {
                save.Received(1).Invoke(
                    Arg.Any<TestUserContext>(),
                    Arg.Is<Person>(p => p.UnitId == 2 && p.ManagerId == 5),
                    Arg.Is<TestClass[]>(c => c.Single().UnitId == 10 && c.Single().ManagerId == 10));

                batch.Received(2).Invoke(
                    Arg.Any<TestUserContext>(),
                    Arg.Is<IEnumerable<Person>>(p => p.Single().UnitId == 2 && p.Single().ManagerId == 5));
            }

            TestMutation(
                QueryBuilder,
                MutationBuilder,
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 1,
                            unitId: 2,
                            managerId: 5,
                            test: [{
                                unitId: 10,
                                managerId: 10
                            }]
                        }]
                    }) {
                        people {
                            id
                            payload {
                                test {
                                    unitId
                                    managerId
                                }
                            }
                        }
                    }
                }",
                @"{
                    submit: {
                        people: [{
                            id: 1,
                            payload: {
                                test: [{
                                    unitId: 2,
                                    managerId: 5
                                }]
                            }
                        }]
                    }
                }", Checks);
        }

        public class TestClass
        {
            public int? UnitId { get; set; }

            public int? ManagerId { get; set; }
        }
    }
}
