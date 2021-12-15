// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Infrastructure;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.TaskBatcher;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using Microsoft.Extensions.Logging.Abstractions;
using NSubstitute;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Loader
{
    [TestFixture]
    public class LoaderSecurityTests : BaseTests
    {
        private IBatcher _batcher;
        private IDataContext _dataContext;
        private GraphQLContext<TestUserContext> _context;
        private RelationRegistry<TestUserContext> _registry;
        private QueryExecuter _queryExecuter;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            var profiler = Substitute.For<IProfiler>();
            _batcher = new Batcher(profiler);
            _dataContext = Substitute.For<IDataContext>();
            _dataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Person>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Person>>>(0).ToAsyncEnumerable());
            _dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Person>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Person>>>(0));
            _queryExecuter = new QueryExecuter(NullLogger.Instance, _dataContext, _dataContext);
            _registry = new RelationRegistry<TestUserContext>(Substitute.For<IServiceProvider>());
            var listener = Substitute.For<ISchemaExecutionListener>();
            _context = new GraphQLContext<TestUserContext>(_dataContext, profiler, _batcher, _registry, NullLogger.Instance, Enumerable.Repeat(listener, 1), new TestUserContext(null));
        }

        [Test]
        public void SelectAllTest()
        {
            var loader = CreatePeopleLoader();
            var people = loader.All(_context.ExecutionContext).ToArray();
            Assert.AreEqual(new[] { FakeData.SophieGandley, FakeData.HannieEveritt }, people);
        }

        [Test]
        public async Task ByPropValuesNegativeTest()
        {
            var loader = CreatePeopleLoader();
            var ids = new[] { 1 };
            var people = await loader
                .All(_context.ExecutionContext)
                .GroupByValues(ids, person => person.Id, FuncConstants<Person>.IdentityExpression, () => string.Empty, _queryExecuter, null)
                .ToListAsync()
                .ConfigureAwait(false);

            Assert.IsEmpty(people);
        }

        [Test]
        public async Task ByPropValuesPositiveTest()
        {
            var loader = CreatePeopleLoader();
            var ids = new[] { 2 };
            var people = await loader
                .All(_context.ExecutionContext)
                .GroupByValues(ids, person => person.Id, FuncConstants<Person>.IdentityExpression, () => string.Empty, _queryExecuter, null)
                .ToListAsync()
                .ConfigureAwait(false);

            Assert.AreEqual(1, people.Count);
            Assert.AreEqual(2, people[0].Key);
            Assert.AreEqual(new[] { FakeData.SophieGandley }, people[0]);
        }

        private Loader<Person, TestUserContext> CreatePeopleLoader()
        {
            return CreatePeopleLoader(FakeData.People, q => q.Where(p => p.ManagerId == 1));
        }

        private Loader<Person, TestUserContext> CreatePeopleLoader(IEnumerable<Person> query, Func<IQueryable<Person>, IQueryable<Person>> filter)
        {
            var loaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(person => person.Id);
                },
                getBaseQuery: _ => query.AsQueryable(),
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.OrderBy(p => p.Id),
                applySecurityFilter: (_, q) => filter(q));

            var result = (Loader<Person, TestUserContext>)_registry.ResolveLoader<Person>(loaderType);
            return result;
        }
    }
}
