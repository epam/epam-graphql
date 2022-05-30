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
    public class RootQueryFieldConfigurationContextTests : BaseTests
    {
        [Test]
        public void FieldFromIQueryable()
        {
            CreateSchemaExecuter(Builder);

            void Builder(Query<TestUserContext> query)
            {
                var field = query
                    .Field("people")
                    .FromIQueryable(_ => FakeData.People.AsQueryable());

                Assert.AreEqual(
                    TestHelpers.ConcatLines(
                        "Field(\"people\")",
                        "    .FromIQueryable<Person>(_ => ...);"),
                    field.ToString());
            }
        }

        [Test]
        public void FieldFromIQueryableWithConfigure()
        {
            CreateSchemaExecuter(Builder);

            void Builder(Query<TestUserContext> query)
            {
                var field = query
                    .Field("people")
                    .FromIQueryable(_ => FakeData.People.AsQueryable(), configure => configure.Field(p => p.Id));

                Assert.AreEqual(
                    TestHelpers.ConcatLines(
                        "Field(\"people\")",
                        "    .FromIQueryable<Person>(",
                        "        _ => ...,",
                        "        configure =>",
                        "        {",
                        "            Field(p => p.Id);",
                        "        });"),
                    field.ToString());
            }
        }

        [Test]
        public void FieldFromIQueryableAsConnection()
        {
            CreateSchemaExecuter(Builder);

            void Builder(Query<TestUserContext> query)
            {
                var field = query
                    .Field("people")
                    .FromIQueryable(_ => FakeData.People.AsQueryable())
                    .AsConnection(query => query.OrderBy(person => person.Id));

                Assert.AreEqual(
                    TestHelpers.ConcatLines(
                        "Field(\"people\")",
                        "    .FromIQueryable<Person>(_ => ...)",
                        "    .AsConnection(query => query.OrderBy(person => person.Id));"),
                    field.ToString());
            }
        }

        [Test]
        public void FieldFromIQueryableAsGroupConnection()
        {
            CreateSchemaExecuter(Builder);

            void Builder(Query<TestUserContext> query)
            {
                var field = query
                    .Field("people")
                    .FromIQueryable(_ => FakeData.People.AsQueryable())
                    .AsGroupConnection();

                Assert.AreEqual(
                    TestHelpers.ConcatLines(
                        "Field(\"people\")",
                        "    .FromIQueryable<Person>(_ => ...)",
                        "    .AsGroupConnection();"),
                    field.ToString());
            }
        }

        [Test]
        public void FieldResolve()
        {
            CreateSchemaExecuter(Builder);

            static void Builder(Query<TestUserContext> query)
            {
                var field = query
                    .Field("people")
                    .Resolve(_ => 100500);

                Assert.AreEqual(
                    TestHelpers.ConcatLines(
                        "Field(\"people\")",
                        "    .Resolve<int>(_ => ...);"),
                    field.ToString());
            }
        }

        [Test]
        public void FieldOneArgResolve()
        {
            CreateSchemaExecuter(Builder);

            static void Builder(Query<TestUserContext> query)
            {
                var field = query
                    .Field("people")
                    .Argument<string>("arg1")
                    .Resolve((_, arg1) => 100500);

                Assert.AreEqual(
                    TestHelpers.ConcatLines(
                        "Field(\"people\")",
                        "    .Argument<string>(\"arg1\")",
                        "    .Resolve<int>((_, arg1) => ...);"),
                    field.ToString());
            }
        }

        private static ISchemaExecuter<TestUserContext> CreateSchemaExecuter(Action<Query<TestUserContext>> queryBuilder)
        {
            var queryType = GraphQLTypeBuilder.CreateQueryType(queryBuilder);
            return ExecuteHelpers.CreateSchemaExecuter<TestUserContext>(queryType, null);
        }

        public class Empty
        {
        }
    }
}
