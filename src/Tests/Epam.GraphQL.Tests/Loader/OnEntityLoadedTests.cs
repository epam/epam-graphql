// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Linq;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Loader
{
    [TestFixture]
    public class OnEntityLoadedTests : BaseTests
    {
        [Test]
        public void ShouldCallHookForConnection()
        {
            var hook = ActionSubstitute.For<TestUserContext, ShouldCallHookProxy>();
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.OnEntityLoaded(p => new ShouldCallHookProxy { Id = p.Id }, hook);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(2).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                builder: Builder,
                query: @"
                    query {
                        people {
                            items {
                                id
                            }
                        }
                    }",
                expected: @"{
                    people: {
                        items: [
                            { id: 1 },
                            { id: 2 }
                        ]
                    }
                }");

            hook.HasBeenCalledTimes(2);
            hook.HasBeenCalledOnce(arg2: person => person.Id == 1);
            hook.HasBeenCalledOnce(arg2: person => person.Id == 2);
        }

        [Test]
        public void ShouldCallHookForConnectionWithLegacyFilter()
        {
            var hook = ActionSubstitute.For<TestUserContext, ShouldCallHookProxy>();
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.OnEntityLoaded(p => new ShouldCallHookProxy { Id = p.Id }, hook);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            var filterType = GraphQLTypeBuilder.CreateLoaderFilterType<Person, PersonFilter, TestUserContext>((ctx, people, filter) => people.Where(p => p.Id <= filter.Id));

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people")
                    .WithFilter(filterType);
            }

            TestHelpers.TestQuery(
                builder: Builder,
                query: @"
                    query {
                        people(filter: { id: 2 }) {
                            items {
                                id
                            }
                        }
                    }",
                expected: @"{
                    people: {
                        items: [
                            { id: 1 },
                            { id: 2 }
                        ]
                    }
                }");

            hook.HasBeenCalledTimes(2);
            hook.HasBeenCalledOnce(arg2: person => person.Id == 1);
            hook.HasBeenCalledOnce(arg2: person => person.Id == 2);
        }

        [Test]
        public void ShouldCallHookForConnectionWithFilter()
        {
            var hook = ActionSubstitute.For<TestUserContext, ShouldCallHookProxy>();
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id).Filterable();
                    loader.OnEntityLoaded(p => new ShouldCallHookProxy { Id = p.Id }, hook);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                builder: Builder,
                query: @"
                    query {
                        people(filter: { id: { lte: 2 } }) {
                            items {
                                id
                            }
                        }
                    }",
                expected: @"{
                    people: {
                        items: [
                            { id: 1 },
                            { id: 2 }
                        ]
                    }
                }");

            hook.HasBeenCalledTimes(2);
            hook.HasBeenCalledOnce(arg2: person => person.Id == 1);
            hook.HasBeenCalledOnce(arg2: person => person.Id == 2);
        }

        [Test]
        public void ShouldCallHookForConnectionWithSearch()
        {
            var hook = ActionSubstitute.For<TestUserContext, ShouldCallHookProxy>();
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.OnEntityLoaded(p => new ShouldCallHookProxy { Id = p.Id }, hook);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            var searchType = GraphQLTypeBuilder.CreateSearcherType<Person, TestUserContext>((people, ctx, search) => people.Where(p => p.FullName.Contains(search)));

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people")
                    .WithSearch(searchType);
            }

            TestHelpers.TestQuery(
                builder: Builder,
                query: @"
                    query {
                        people(search: ""on"") {
                            items {
                                id
                            }
                        }
                    }",
                expected: @"{
                    people: {
                        items: [
                            { id: 5 },
                            { id: 6 }
                        ]
                    }
                }");

            hook.HasBeenCalledTimes(2);
            hook.HasBeenCalledOnce(arg2: person => person.Id == 5);
            hook.HasBeenCalledOnce(arg2: person => person.Id == 6);
        }

        [Test]
        public void ShouldCallHookForNestedConnection()
        {
            var hook = ActionSubstitute.For<TestUserContext, ShouldCallHookProxy>();
            var unitLoaderType = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.OnEntityLoaded(p => new ShouldCallHookProxy { Id = p.Id }, hook);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("units")
                        .FromLoader<Unit>(unitLoaderType, (p, u) => u.Id == p.UnitId)
                        .AsConnection();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(2).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                builder: Builder,
                query: @"
                    query {
                        people {
                            items {
                                id
                                units {
                                    items {
                                        id
                                    }
                                }
                            }
                        }
                    }",
                expected: @"{
                    people: {
                        items: [{
                            id: 1,
                            units: {
                                items: [{
                                    id: 1
                                }]
                            }
                        }, {
                            id: 2,
                            units: {
                                items: [{
                                    id: 1
                                }]
                            }
                        }]
                    }
                }");

            hook.HasBeenCalledOnce();

            hook.HasBeenCalledOnce(arg2: unit => unit.Id == 1);
        }

        [Test]
        public void ShouldCallHookForNestedLoader()
        {
            var hook = ActionSubstitute.For<TestUserContext, ShouldCallHookProxy>();
            var unitLoaderType = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.OnEntityLoaded(p => new ShouldCallHookProxy { Id = p.Id }, hook);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("units")
                        .FromLoader<Unit>(unitLoaderType, (p, u) => u.Id == p.UnitId);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(2).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                builder: Builder,
                query: @"
                    query {
                        people {
                            items {
                                id
                                units {
                                    id
                                }
                            }
                        }
                    }",
                expected: @"{
                    people: {
                        items: [{
                            id: 1,
                            units: [{
                                id: 1
                            }]
                        }, {
                            id: 2,
                            units: [{
                                id: 1
                            }]
                        }]
                    }
                }");

            hook.HasBeenCalledOnce();

            hook.HasBeenCalledOnce(arg2: unit => unit.Id == 1);
        }

        [Test]
        public void ShouldCallHookForNestedLoaderFirstOrDefault()
        {
            var hook = ActionSubstitute.For<TestUserContext, ShouldCallHookProxy>();
            var unitLoaderType = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.OnEntityLoaded(p => new ShouldCallHookProxy { Id = p.Id }, hook);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("units")
                        .FromLoader<Unit>(unitLoaderType, (p, u) => u.Id == p.UnitId)
                        .FirstOrDefault();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(2).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                builder: Builder,
                query: @"
                    query {
                        people {
                            items {
                                id
                                units {
                                    id
                                }
                            }
                        }
                    }",
                expected: @"{
                    people: {
                        items: [{
                            id: 1,
                            units: {
                                id: 1
                            }
                        }, {
                            id: 2,
                            units: {
                                id: 1
                            }
                        }]
                    }
                }");

            hook.HasBeenCalledOnce();

            hook.HasBeenCalledOnce(arg2: unit => unit.Id == 1);
        }

        [Test]
        public void ShouldCallHookForNestedLoaderWithWhere()
        {
            var hook = ActionSubstitute.For<TestUserContext, ShouldCallHookProxy>();
            var unitLoaderType = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.OnEntityLoaded(p => new ShouldCallHookProxy { Id = p.Id }, hook);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("units")
                        .FromLoader<Unit>(unitLoaderType, (p, u) => u.Id == p.UnitId)
                        .Where(u => u.Id == 1);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(2).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                builder: Builder,
                query: @"
                    query {
                        people {
                            items {
                                id
                                units {
                                    id
                                }
                            }
                        }
                    }",
                expected: @"{
                    people: {
                        items: [{
                            id: 1,
                            units: [{
                                id: 1
                            }]
                        }, {
                            id: 2,
                            units: [{
                                id: 1
                            }]
                        }]
                    }
                }");

            hook.HasBeenCalledOnce();

            hook.HasBeenCalledOnce(arg2: unit => unit.Id == 1);
        }

        [Test]
        public void ShouldCallHookForNestedLoaderWithWhereFirstOrDefault()
        {
            var hook = ActionSubstitute.For<TestUserContext, ShouldCallHookProxy>();
            var unitLoaderType = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.OnEntityLoaded(p => new ShouldCallHookProxy { Id = p.Id }, hook);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("units")
                        .FromLoader<Unit>(unitLoaderType, (p, u) => u.Id == p.UnitId)
                        .Where(u => u.Id == 1)
                        .FirstOrDefault();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(2).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                builder: Builder,
                query: @"
                    query {
                        people {
                            items {
                                id
                                units {
                                    id
                                }
                            }
                        }
                    }",
                expected: @"{
                    people: {
                        items: [{
                            id: 1,
                            units: {
                                id: 1
                            }
                        }, {
                            id: 2,
                            units: {
                                id: 1
                            }
                        }]
                    }
                }");

            hook.HasBeenCalledOnce();

            hook.HasBeenCalledOnce(arg2: unit => unit.Id == 1);
        }

        [Test]
        public void ShouldCallHookForNestedLoaderWithSelect()
        {
            var hook = ActionSubstitute.For<TestUserContext, ShouldCallHookProxy>();
            var unitLoaderType = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.OnEntityLoaded(p => new ShouldCallHookProxy { Id = p.Id }, hook);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("unitIds")
                        .FromLoader<Unit>(unitLoaderType, (p, u) => u.Id == p.UnitId)
                        .Select(u => u.Id);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(2).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                builder: Builder,
                query: @"
                    query {
                        people {
                            items {
                                id
                                unitIds
                            }
                        }
                    }",
                expected: @"{
                    people: {
                        items: [{
                            id: 1,
                            unitIds: [1]
                        }, {
                            id: 2,
                            unitIds: [1]
                        }]
                    }
                }");

            hook.HasBeenCalledOnce();

            hook.HasBeenCalledOnce(arg2: unit => unit.Id == 1);
        }

        [Test]
        public void ShouldCallHookForNestedLoaderWithSelectFirstOrDefault()
        {
            var hook = ActionSubstitute.For<TestUserContext, ShouldCallHookProxy>();
            var unitLoaderType = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.OnEntityLoaded(p => new ShouldCallHookProxy { Id = p.Id }, hook);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("unitIds")
                        .FromLoader<Unit>(unitLoaderType, (p, u) => u.Id == p.UnitId)
                        .Select(u => u.Id)
                        .FirstOrDefault();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(2).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                builder: Builder,
                query: @"
                    query {
                        people {
                            items {
                                id
                                unitIds
                            }
                        }
                    }",
                expected: @"{
                    people: {
                        items: [{
                            id: 1,
                            unitIds: 1
                        }, {
                            id: 2,
                            unitIds: 1
                        }]
                    }
                }");

            hook.HasBeenCalledOnce();

            hook.HasBeenCalledOnce(arg2: unit => unit.Id == 1);
        }

        [Test]
        public void ShouldCallHookForNestedLoaderWithSelectAndWhere()
        {
            var hook = ActionSubstitute.For<TestUserContext, ShouldCallHookProxy>();
            var unitLoaderType = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.OnEntityLoaded(p => new ShouldCallHookProxy { Id = p.Id }, hook);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("unitIds")
                        .FromLoader<Unit>(unitLoaderType, (p, u) => u.Id == p.UnitId)
                        .Select(u => u.Id)
                        .Where(id => id == 1);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(2).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                builder: Builder,
                query: @"
                    query {
                        people {
                            items {
                                id
                                unitIds
                            }
                        }
                    }",
                expected: @"{
                    people: {
                        items: [{
                            id: 1,
                            unitIds: [1]
                        }, {
                            id: 2,
                            unitIds: [1]
                        }]
                    }
                }");

            hook.HasBeenCalledOnce();

            hook.HasBeenCalledOnce(arg2: unit => unit.Id == 1);
        }

        [Test]
        public void ShouldCallHookForNestedLoaderWithSelectAndWhereFirstOrDefault()
        {
            var hook = ActionSubstitute.For<TestUserContext, ShouldCallHookProxy>();
            var unitLoaderType = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.OnEntityLoaded(p => new ShouldCallHookProxy { Id = p.Id }, hook);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("unitIds")
                        .FromLoader<Unit>(unitLoaderType, (p, u) => u.Id == p.UnitId)
                        .Select(u => u.Id)
                        .Where(id => id == 1)
                        .FirstOrDefault();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(2).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                builder: Builder,
                query: @"
                    query {
                        people {
                            items {
                                id
                                unitIds
                            }
                        }
                    }",
                expected: @"{
                    people: {
                        items: [{
                            id: 1,
                            unitIds: 1
                        }, {
                            id: 2,
                            unitIds: 1
                        }]
                    }
                }");

            hook.HasBeenCalledOnce();

            hook.HasBeenCalledOnce(arg2: unit => unit.Id == 1);
        }

        [Test]
        public void ShouldCallHookForNestedLoaderWithSelectClass()
        {
            var hook = ActionSubstitute.For<TestUserContext, ShouldCallHookProxy>();
            var unitLoaderType = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.OnEntityLoaded(p => new ShouldCallHookProxy { Id = p.Id }, hook);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("units")
                        .FromLoader<Unit>(unitLoaderType, (p, u) => u.Id == p.UnitId)
                        .Select(u => new Branch
                        {
                            Id = u.Id,
                        });
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(2).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                builder: Builder,
                query: @"
                    query {
                        people {
                            items {
                                id
                                units {
                                    id
                                }
                            }
                        }
                    }",
                expected: @"{
                    people: {
                        items: [{
                            id: 1,
                            units: [{
                                id: 1
                            }]
                        }, {
                            id: 2,
                            units: [{
                                id: 1
                            }]
                        }]
                    }
                }");

            hook.HasBeenCalledOnce();

            hook.HasBeenCalledOnce(arg2: unit => unit.Id == 1);
        }

        [Test]
        public void ShouldCallHookForNestedLoaderWithSelectClassFirstOrDefault()
        {
            var hook = ActionSubstitute.For<TestUserContext, ShouldCallHookProxy>();
            var unitLoaderType = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.OnEntityLoaded(p => new ShouldCallHookProxy { Id = p.Id }, hook);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("units")
                        .FromLoader<Unit>(unitLoaderType, (p, u) => u.Id == p.UnitId)
                        .Select(u => new Branch
                        {
                            Id = u.Id,
                        })
                        .FirstOrDefault();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(2).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                builder: Builder,
                query: @"
                    query {
                        people {
                            items {
                                id
                                units {
                                    id
                                }
                            }
                        }
                    }",
                expected: @"{
                    people: {
                        items: [{
                            id: 1,
                            units: {
                                id: 1
                            }
                        }, {
                            id: 2,
                            units: {
                                id: 1
                            }
                        }]
                    }
                }");

            hook.HasBeenCalledOnce();

            hook.HasBeenCalledOnce(arg2: unit => unit.Id == 1);
        }

        [Test]
        public void ShouldCallHookForQueryable()
        {
            var hook = ActionSubstitute.For<TestUserContext, ShouldCallHookProxy>();

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("people")
                    .FromIQueryable(
                        ctx => FakeData.People.Take(2).AsQueryable(),
                        builder =>
                        {
                            builder.Field(p => p.Id);
                            builder.OnEntityLoaded(p => new ShouldCallHookProxy { Id = p.Id }, hook);
                        });
            }

            TestHelpers.TestQuery(
                builder: Builder,
                query: @"
                    query {
                        people {
                            id
                        }
                    }",
                expected: @"{
                    people: [
                        { id: 1 },
                        { id: 2 }
                    ]
                }");

            hook.HasBeenCalledTimes(2);
            hook.HasBeenCalledOnce(arg2: person => person.Id == 1);
            hook.HasBeenCalledOnce(arg2: person => person.Id == 2);
        }

        [Test]
        public void ShouldCallHookForQueryableFirstOrDefault()
        {
            var hook = ActionSubstitute.For<TestUserContext, ShouldCallHookProxy>();

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("people")
                    .FromIQueryable(
                        ctx => FakeData.People.Take(2).AsQueryable(),
                        builder =>
                        {
                            builder.Field(p => p.Id);
                            builder.OnEntityLoaded(p => new ShouldCallHookProxy { Id = p.Id }, hook);
                        })
                    .FirstOrDefault();
            }

            TestHelpers.TestQuery(
                builder: Builder,
                query: @"
                    query {
                        people {
                            id
                        }
                    }",
                expected: @"{
                    people: {
                        id: 1
                    }
                }");

            hook.HasBeenCalledTimes(1);
            hook.HasBeenCalledOnce(arg2: person => person.Id == 1);
        }

        [Test]
        public void ShouldCallHookForNestedQueryable()
        {
            var hook = ActionSubstitute.For<TestUserContext, ShouldCallHookProxy>();

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("units")
                        .FromIQueryable<Unit>(
                            ctx => FakeData.Units.AsQueryable(),
                            (p, u) => u.Id == p.UnitId,
                            builder =>
                            {
                                builder.Field(p => p.Id);
                                builder.OnEntityLoaded(p => new ShouldCallHookProxy { Id = p.Id }, hook);
                            });
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(2).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                builder: Builder,
                query: @"
                    query {
                        people {
                            items {
                                id
                                units {
                                    id
                                }
                            }
                        }
                    }",
                expected: @"{
                    people: {
                        items: [{
                            id: 1,
                            units: [{
                                id: 1
                            }]
                        }, {
                            id: 2,
                            units: [{
                                id: 1
                            }]
                        }]
                    }
                }");

            hook.HasBeenCalledOnce();

            hook.HasBeenCalledOnce(arg2: unit => unit.Id == 1);
        }

        [Test]
        public void ShouldCallHookForNestedQueryableFirstOrDefault()
        {
            var hook = ActionSubstitute.For<TestUserContext, ShouldCallHookProxy>();

            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field("units")
                        .FromIQueryable<Unit>(
                            ctx => FakeData.Units.AsQueryable(),
                            (p, u) => u.Id == p.UnitId,
                            builder =>
                            {
                                builder.Field(p => p.Id);
                                builder.OnEntityLoaded(p => new ShouldCallHookProxy { Id = p.Id }, hook);
                            })
                        .FirstOrDefault();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.Take(2).AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            TestHelpers.TestQuery(
                builder: Builder,
                query: @"
                    query {
                        people {
                            items {
                                id
                                units {
                                    id
                                }
                            }
                        }
                    }",
                expected: @"{
                    people: {
                        items: [{
                            id: 1,
                            units: {
                                id: 1
                            }
                        }, {
                            id: 2,
                            units: {
                                id: 1
                            }
                        }]
                    }
                }");

            hook.HasBeenCalledOnce();

            hook.HasBeenCalledOnce(arg2: unit => unit.Id == 1);
        }

        public class PersonFilter : Input
        {
            public int Id { get; set; }
        }

        private class ShouldCallHookProxy
        {
            public int Id { get; set; }
        }
    }
}
