// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Linq;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using NSubstitute;

namespace Epam.GraphQL.Tests.TestData
{
    public class TestUserContext : ITestUserContext
    {
        public TestUserContext(IDataContext context)
        {
            DataContext = context;

            if (context == null)
            {
                DataContext = Substitute.For<IDataContext>();
                DataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Person>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Person>>>(0).ToAsyncEnumerable());
                DataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Unit>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Unit>>>(0).ToAsyncEnumerable());
                DataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Branch>>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Branch>>>>(0).ToAsyncEnumerable());
                DataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Person>>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Person>>>>(0).ToAsyncEnumerable());
                DataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<long, Proxy<Person>>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<long, Proxy<Person>>>>(0).ToAsyncEnumerable());
                DataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Unit>>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Unit>>>>(0).ToAsyncEnumerable());
                DataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, int>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, int>>>(0).ToAsyncEnumerable());
            }
        }

        public int UserId => 5;

        public IDataContext DataContext { get; }
    }
}
