// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Diagnostics
{
    [TestFixture]
    public class ExpressionFieldErrorTests : BaseTests
    {
        [Test]
        public void ExpressionFieldThrowsError()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field("id", (ctx, p) => 1 / (ctx.UserId - 5)),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("people")
                    .FromLoader<Person, TestUserContext>(personLoaderType);
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);
            var mutationType = GraphQLTypeBuilder.CreateMutationType<TestUserContext>(_ => { });

            var result = ExecuteHelpers.ExecuteQuery(
                queryType,
                mutationType,
                @"
                query {
                    people {
                        id
                    }
                }");

            Assert.That(
                result.Errors,
                Is.Not.Empty.And.All.Message.EqualTo(TestHelpers.ConcatLines(
                    "Error during resolving field `people`. See an inner exception for details.",
                    "Query:",
                    "public override void OnConfigure()",
                    "{",
                    "    Field(\"people\")",
                    "        .FromLoader<PersonLoader, Person>(); // <-----",
                    "}"))
                    .And.InnerException.TypeOf<DivideByZeroException>());
        }

        [Test]
        public void NestedLoaderExpressionFieldThrowsError()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field("id", (ctx, p) => 1 / (ctx.UserId - 5)),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            var unitLoaderType = GraphQLTypeBuilder.CreateLoaderType<Unit, TestUserContext>(
                onConfigure: loader => loader.Field("people").FromLoader<Unit, Person, TestUserContext>(personLoaderType, (u, p) => u.Id == p.UnitId),
                getBaseQuery: _ => FakeData.Units.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("units")
                    .FromLoader<Unit, TestUserContext>(unitLoaderType);
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);
            var mutationType = GraphQLTypeBuilder.CreateMutationType<TestUserContext>(_ => { });

            var result = ExecuteHelpers.ExecuteQuery(
                queryType,
                mutationType,
                @"
                query {
                    units {
                        people {
                            id
                        }
                    }
                }");

            Assert.That(
                result.Errors,
                Is.Not.Empty.And.All.Message.EqualTo(TestHelpers.ConcatLines(
                    "Error during resolving field `units.0.people`. See an inner exception for details.",
                    "UnitLoader:",
                    "public override void OnConfigure()",
                    "{",
                    "    Field(\"people\")",
                    "        .FromLoader<PersonLoader, Person>((u, p) => (int?)u.Id == p.UnitId); // <-----",
                    "}"))
                    .And.InnerException.TypeOf<DivideByZeroException>());
        }
    }
}
