// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NSubstitute;
using NUnit.Framework;

namespace Epam.GraphQL.Tests
{
    public class BaseMutationTests : BaseTests
    {
        protected IDataContext DataContext { get; set; }

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            DataContext = Substitute.For<IDataContext>();

            DataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Person>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Person>>>(0).ToAsyncEnumerable());
            DataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, PersonSettings>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, PersonSettings>>>(0).ToAsyncEnumerable());
            DataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Unit>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Unit>>>(0).ToAsyncEnumerable());
            DataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Branch>>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Branch>>>>(0).ToAsyncEnumerable());
            DataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Person>>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Person>>>>(0).ToAsyncEnumerable());
            DataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<long, Proxy<Person>>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<long, Proxy<Person>>>>(0).ToAsyncEnumerable());
            DataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Proxy<PersonSettings>>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Proxy<PersonSettings>>>>(0).ToAsyncEnumerable());
            DataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Unit>>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Unit>>>>(0).ToAsyncEnumerable());
            DataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, int>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, int>>>(0).ToAsyncEnumerable());

            DataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Person>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Person>>>(0));
            DataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, PersonSettings>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, PersonSettings>>>(0));
            DataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Unit>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Unit>>>(0));
            DataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Branch>>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Branch>>>>(0));
            DataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Person>>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Person>>>>(0));
            DataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<long, Proxy<Person>>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<long, Proxy<Person>>>>(0));
            DataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Proxy<PersonSettings>>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Proxy<PersonSettings>>>>(0));
            DataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Unit>>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Unit>>>>(0));
            DataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, int>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, int>>>(0));

            DataContext.ExecuteInTransactionAsync(Arg.Any<Func<Task>>())
                .Returns(callInfo => callInfo.ArgAt<Func<Task>>(0)());

            DataContext
                .When(dataContext => dataContext.AddRange(Arg.Any<IEnumerable<Person>>()))
                .Do(callInfo =>
                {
                    var range = callInfo.ArgAt<IEnumerable<Person>>(0);
                    foreach (var p in range)
                    {
                        FakeData.AddPerson(p);
                    }
                });

            DataContext
                .When(dataContext => dataContext.AddRange(Arg.Any<IEnumerable<PersonSettings>>()))
                .Do(callInfo =>
                {
                    var range = callInfo.ArgAt<IEnumerable<PersonSettings>>(0);
                    foreach (var p in range)
                    {
                        FakeData.AddPersonSettings(p);
                    }
                });

            DataContext
                .When(dataContext => dataContext.AddRange(Arg.Any<IEnumerable<Unit>>()))
                .Do(callInfo =>
                {
                    var range = callInfo.ArgAt<IEnumerable<Unit>>(0);
                    foreach (var p in range)
                    {
                        FakeData.AddUnit(p);
                    }
                });

            DataContext
                .When(dataContext => dataContext.SaveChangesAsync())
                .Do(callInfo =>
                {
                    FakeData.UpdateRelations();
                });

            DataContext.GetQueryable<Person>()
                .Returns(callInfo =>
                {
                    return FakeData.People.AsQueryable();
                });

            DataContext.GetQueryable<PersonSettings>()
                .Returns(callInfo =>
                {
                    return FakeData.PeopleSettings.AsQueryable();
                });

            DataContext.GetQueryable<Unit>()
                .Returns(callInfo =>
                {
                    return FakeData.Units.AsQueryable();
                });
        }

        protected void TestMutationError(Action<Query<TestUserContext>> queryBuilder, Action<Mutation<TestUserContext>> mutationBuilder, Type exceptionType, string message, string query, Action<IDataContext> checks = null, Action beforeExecute = null)
        {
            beforeExecute?.Invoke();
            Assert.Throws(
                Is.TypeOf(exceptionType).And.Message.EqualTo(message),
                () =>
                {
                    var queryType = GraphQLTypeBuilder.CreateQueryType(queryBuilder);
                    var mutationType = GraphQLTypeBuilder.CreateMutationType(mutationBuilder);
                    var result = ExecuteHelpers.ExecuteQuery(queryType, mutationType, query, DataContext);
                    if (result.Errors != null && result.Errors.Any())
                    {
                        throw result.Errors.First();
                    }

                    checks?.Invoke(DataContext);
                });
        }

        protected void TestMutation(Action<Query<TestUserContext>> queryBuilder, Action<Mutation<TestUserContext>> mutationBuilder, string query, string expected, Action<IDataContext> checks = null, Action beforeExecute = null, Func<TestUserContext, IEnumerable<object>, Task<IEnumerable<object>>> afterSave = null)
        {
            TestHelpers.TestMutation(queryBuilder, mutationBuilder, DataContext, query, expected, checks, beforeExecute, afterSave);
        }

        public static class CustomObject
        {
            public static CustomObject<T> Create<T>(T fieldValue) => new() { TestField = fieldValue };
        }

        public class CustomObject<T>
        {
            public T TestField { get; set; }
        }
    }
}
