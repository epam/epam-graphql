// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NSubstitute;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Loader
{
    [TestFixture]
    public class InheritanceTests : BaseTests
    {
        private IDataContext _dataContext;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            _dataContext = Substitute.For<IDataContext>();

            _dataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Person>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Person>>>(0).ToAsyncEnumerable());
            _dataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Unit>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Unit>>>(0).ToAsyncEnumerable());

            _dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Person>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Person>>>(0));
            _dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Unit>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Unit>>>(0));

            _dataContext.ExecuteInTransactionAsync(Arg.Any<Func<Task>>())
                .Returns(callInfo => callInfo.ArgAt<Func<Task>>(0)());

            var people = FakeData.People.Select(p => p.Clone()).ToList();

            _dataContext
                .When(dataContext => dataContext.AddRange(Arg.Any<IEnumerable<Person>>()))
                .Do(callInfo =>
                {
                    var range = callInfo.ArgAt<IEnumerable<Person>>(0);
                    foreach (var p in range)
                    {
                        if (p.Id == 0)
                        {
                            p.Id = people.Max(person => person.Id) + 1;
                            people.Add(p);
                        }
                    }
                });

            _dataContext.GetQueryable<Person>()
                .Returns(people.AsQueryable());
        }

        [Test]
        public void NoOverridenOnConfigure()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(3));

            var person2LoaderType = GraphQLTypeBuilder.CreateInheritedLoaderType<Person, TestUserContext>(
                personLoaderType,
                typeName: "InheritedLoader",
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2));

            var person3LoaderType = GraphQLTypeBuilder.CreateInheritedLoaderType<Person, TestUserContext>(
                person2LoaderType,
                typeName: "InheritedLoader2",
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(1));

            void Builder(Query<TestUserContext> query)
            {
                query.Connection(personLoaderType, "people");
                query.Connection(person2LoaderType, "people2");
                query.Connection(person3LoaderType, "people3");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        people {
                            items {
                                id
                            }
                        }
                        people2 {
                            items {
                                id
                            }
                        }
                        people3 {
                            items {
                                id
                            }
                        }
                    }
                ",
                @"{
                    people: {
                        items: [{
                            id: 1
                        },{
                            id: 2
                        },{
                            id: 3
                        }]
                    },
                    people2: {
                        items: [{
                            id: 1
                        },{
                            id: 2
                        }]
                    },
                    people3: {
                        items: [{
                            id: 1
                        }]
                    }
                }");
        }

        [Test]
        public void MutableLoaderNoOverridenOnConfigureSubmitByGrandparent()
        {
            TestMutableLoaderNoOverridenOnConfigure(
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 1,
                            fullName: ""Test1""
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
                                fullName: ""Test1""
                            }
                        }]
                    }
                }");
        }

        [Test]
        public void MutableLoaderNoOverridenOnConfigureSubmitByChild()
        {
            TestMutableLoaderNoOverridenOnConfigure(
                @"mutation {
                    submit(payload: {
                        people2: [{
                            id: 1,
                            fullName: ""Test1""
                        }]
                    }) {
                        people2 {
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
                        people2: [{
                            id: 1,
                            payload: {
                                id: 1,
                                fullName: ""Test1""
                            }
                        }]
                    }
                }");
        }

        [Test]
        public void MutableLoaderNoOverridenOnConfigureSubmitByGrandchild()
        {
            TestMutableLoaderNoOverridenOnConfigure(
                @"mutation {
                    submit(payload: {
                        people3: [{
                            id: 1,
                            fullName: ""Test1""
                        }]
                    }) {
                        people3 {
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
                        people3: [{
                            id: 1,
                            payload: {
                                id: 1,
                                fullName: ""Test1""
                            }
                        }]
                    }
                }");
        }

        [Ignore("TBD: How it should behaive? Maybe current behaviour is right...")]
        [Test]
        public void MutableLoaderNoOverridenOnConfigureMultipleSubmit()
        {
            // TODO Fix inherited mutable loaders submit
            TestMutableLoaderNoOverridenOnConfigure(
                @"mutation {
                    submit(payload: {
                        people: [{
                            id: 1,
                            fullName: ""Test1""
                        }],
                        people2: [{
                            id: 2,
                            fullName: ""Test2""
                        }],
                        people3: [{
                            id: 3,
                            fullName: ""Test3""
                        }]
                    }) {
                        people {
                            id
                            payload {
                                id
                                fullName
                            }
                        }
                        people2 {
                            id
                            payload {
                                id
                                fullName
                            }
                        }
                        people3 {
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
                                fullName: ""Test1""
                            }
                        }],
                        people2: [{
                            id: 2,
                            payload: {
                                id: 2,
                                fullName: ""Test2""
                            }
                        }],
                        people3: [{
                            id: 3,
                            payload: {
                                id: 3,
                                fullName: ""Test3""
                            }
                        }]
                    }
                }");
        }

        [Test]
        public void OverridenAbstractClass()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: null,
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable().Take(2));

            var person2LoaderType = GraphQLTypeBuilder.CreateInheritedLoaderType<Person, TestUserContext>(
                personLoaderType,
                typeName: "InheritedLoader",
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                });

            var person3LoaderType = GraphQLTypeBuilder.CreateInheritedLoaderType<Person, TestUserContext>(
                person2LoaderType,
                typeName: "InheritedLoader2");

            void Builder(Query<TestUserContext> query)
            {
                query.Connection(person2LoaderType, "people2");
                query.Connection(person3LoaderType, "people3");
            }

            TestHelpers.TestQuery(
                Builder,
                @"
                    query {
                        people2 {
                            items {
                                id
                            }
                        }
                        people3 {
                            items {
                                id
                            }
                        }
                    }
                ",
                @"{
                    people2: {
                        items: [{
                            id: 1
                        },{
                            id: 2
                        }]
                    },
                    people3: {
                        items: [{
                            id: 1
                        },{
                            id: 2
                        }]
                    }
                }");
        }

        private void TestMutableLoaderNoOverridenOnConfigure(string query, string expectedResult)
        {
            var personLoaderType = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName).Editable();
                },
                getBaseQuery: ctx => ctx.DataContext.GetQueryable<Person>());

            var person2LoaderType = GraphQLTypeBuilder.CreateInheritedLoaderType<Person, TestUserContext>(
                personLoaderType,
                typeName: "InheritedLoader");

            var person3LoaderType = GraphQLTypeBuilder.CreateInheritedLoaderType<Person, TestUserContext>(
                person2LoaderType,
                typeName: "InheritedLoader2");

            void Builder(Query<TestUserContext> q)
            {
                q.Connection(personLoaderType, "people");
                q.Connection(person2LoaderType, "people2");
                q.Connection(person2LoaderType, "people3");
            }

            void MutationBuilder(Mutation<TestUserContext> q)
            {
                q.SubmitField(personLoaderType, "people");
                q.SubmitField(person2LoaderType, "people2");
                q.SubmitField(person3LoaderType, "people3");
            }

            TestHelpers.TestMutation(
                Builder,
                MutationBuilder,
                _dataContext,
                query,
                expectedResult);
        }
    }
}
