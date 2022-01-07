// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
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
    public class LoaderTests : BaseTests
    {
        private Loader<Person, TestUserContext> _loader;
        private IBatcher _batcher;
        private IDataContext _dataContext;
        private QueryExecuter _queryExecuter;
        private GraphQLContext<TestUserContext> _context;
        private RelationRegistry<TestUserContext> _registry;

        [SetUp]
        public new void SetUp()
        {
            base.SetUp();
            _registry = new RelationRegistry<TestUserContext>(Substitute.For<IServiceProvider>());
            var profiler = Substitute.For<IProfiler>();
            _batcher = new Batcher(profiler);
            _dataContext = Substitute.For<IDataContext>();
            _dataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Person>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Person>>>(0).ToAsyncEnumerable());
            _dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Person>>>())
                .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Person>>>(0));

            _queryExecuter = new QueryExecuter(NullLogger.Instance, _dataContext, _dataContext);

            var listener = Substitute.For<ISchemaExecutionListener>();
            _context = new GraphQLContext<TestUserContext>(_dataContext, profiler, _batcher, _registry, NullLogger.Instance, Enumerable.Repeat(listener, 1), new TestUserContext(null));

            var loaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(person => person.Id);
                    loader.Field(person => person.FullName);
                    loader.Field(person => person.Salary);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable(),
                applyNaturalOrderBy: query => query.OrderBy(p => p.Id),
                applyNaturalThenBy: query => query.ThenBy(p => p.Id));

            _loader = (Loader<Person, TestUserContext>)_registry.ResolveLoader<Person>(loaderType);
        }

        [Test]
        public void SelectAllTest()
        {
            var people = _loader.All(_context.ExecutionContext).ToArray();
            Assert.AreEqual(FakeData.People, people);
        }

        [Test]
        public async Task ByPropValuesTest()
        {
            var ids = new[] { 2, 3 };
            var people = await _loader
                .All(_context.ExecutionContext)
                .GroupByValues(ids, person => person.Id, FuncConstants<Person>.IdentityExpression, () => string.Empty, _queryExecuter, null, null)
                .ToListAsync()
                .ConfigureAwait(false);

            Assert.AreEqual(new[] { new[] { FakeData.SophieGandley }, new[] { FakeData.HannieEveritt } }, people);
        }

        [Test]
        public async Task ByNestedPropValuesTest()
        {
            var ids = new[] { 1 };
            var people = await _loader
                .All(_context.ExecutionContext)
                .GroupByValues(ids, person => person.Unit.Id, FuncConstants<Person>.IdentityExpression, () => string.Empty, _queryExecuter, null, null)
                .ToListAsync()
                .ConfigureAwait(false);

            Assert.AreEqual(new[] { new[] { FakeData.LinoelLivermore, FakeData.SophieGandley, FakeData.HannieEveritt } }, people);
        }
    }
}
