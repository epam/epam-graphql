// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using GraphQL;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Diagnostics
{
    [TestFixture]
    public class ConfigurationErrorTests : BaseTests
    {
        [Test]
        public void ThrowOnQueryEmptyConfig()
        {
            Assert.Throws(Is.TypeOf<ConfigurationException>().And.Message.EqualTo("Error during Query.OnConfigure() call.\r\nOnConfigure() method must have a declaration of one field at least."), () =>
            {
                var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(query =>
                {
                });
                ExecuteHelpers.CreateSchemaExecuter<TestUserContext>(queryType, null);
            });
        }

        [Test]
        public void ThrowOnQueryFieldWithoutResolver()
        {
            Assert.Throws(Is.TypeOf<ConfigurationException>().And.Message.EqualTo("Error during Query.OnConfigure() call.\r\nField: Field(\"test\")\r\nField `test` must have resolver."), () =>
            {
                var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(query => query
                    .Field("test"));
                ExecuteHelpers.CreateSchemaExecuter<TestUserContext>(queryType, null);
            });
        }

        [Test]
        public void ThrowOnNullQueryFieldName()
        {
            Assert.Throws(Is.TypeOf<ConfigurationException>().And.Message.EqualTo("Error during Query.OnConfigure() call.\r\nField: Field(null).Resolve<int>(_ => ...)\r\nField name cannot be null or empty."), () =>
            {
                var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(query => query
                    .Field(null)
                    .Resolve(_ => 0));
                ExecuteHelpers.CreateSchemaExecuter<TestUserContext>(queryType, null);
            });
        }

        [Test]
        public void ThrowOnEmptyQueryFieldName()
        {
            Assert.Throws(Is.TypeOf<ConfigurationException>().And.Message.EqualTo("Error during Query.OnConfigure() call.\r\nField: Field(\"\").Resolve<int>(_ => ...)\r\nField name cannot be null or empty."), () =>
            {
                var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(query => query
                    .Field(string.Empty)
                    .Resolve(_ => 0));
                ExecuteHelpers.CreateSchemaExecuter<TestUserContext>(queryType, null);
            });
        }

        [Test]
        public void ThrowOnDuplicateQueryField()
        {
            static void Builder(Query<TestUserContext> query)
            {
                query.Field("test")
                    .Resolve(_ => 0);
                query.Field("test")
                    .Resolve(_ => 0);
            }

            Assert.Throws(Is.TypeOf<ConfigurationException>().And.Message.EqualTo("Error during Query.OnConfigure() call.\r\nA field with the name `test` is already registered."), () =>
            {
                var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);
                ExecuteHelpers.CreateSchemaExecuter<TestUserContext>(queryType, null);
            });
        }

        [Test]
        public void ThrowOnQueryConnectionFromNonLoader()
        {
            static void Builder(Query<TestUserContext> query)
            {
                query.Connection<object>("test");
            }

            Assert.Throws(Is.TypeOf<ConfigurationException>().And.Message.EqualTo("Error during Query.OnConfigure() call.\r\nField: Connection<object>(\"test\")\r\nCannot find the corresponding generic base type `Loader<,>` for type `object`."), () =>
            {
                var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);
                ExecuteHelpers.CreateSchemaExecuter<TestUserContext>(queryType, null);
            });
        }

        [Test]
        public void ThrowOnQueryGroupConnectionFromNonLoader()
        {
            static void Builder(Query<TestUserContext> query)
            {
                query.GroupConnection<object>("test");
            }

            Assert.Throws(Is.TypeOf<ConfigurationException>().And.Message.EqualTo("Error during Query.OnConfigure() call.\r\nField: GroupConnection<object>(\"test\")\r\nCannot find the corresponding generic base type `Loader<,>` for type `object`."), () =>
            {
                var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);
                ExecuteHelpers.CreateSchemaExecuter<TestUserContext>(queryType, null);
            });
        }

        [Test]
        public void ThrowOnLoaderFieldWithoutResolver()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field("test"),
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            Assert.Throws(Is.TypeOf<ConfigurationException>().And.Message.EqualTo("Error during PersonLoader.OnConfigure() call.\r\nField: Field(\"test\")\r\nField `test` must have resolver."), () =>
            {
                var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);
                ExecuteHelpers.CreateSchemaExecuter<TestUserContext>(queryType, null);
            });
        }

        [Test]
        public void ThrowOnLoaderFieldWithExpressionAndNoName()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field(p => 2 * p.Id),
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            Assert.Throws(Is.TypeOf<ConfigurationException>().And.Message.EqualTo("Error during PersonLoader.OnConfigure() call.\r\nField: Field(p => 2 * p.Id)\r\nExpression (p => (2 * p.Id)), provided for field is not a property. Consider giving a name to the field explicitly."), () =>
            {
                var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);
                ExecuteHelpers.CreateSchemaExecuter<TestUserContext>(queryType, null);
            });
        }

        [Test]
        public void ThrowOnFieldReturningValueTypeWithoutConfig()
        {
            static void Builder(Query<TestUserContext> query)
            {
                query
                    .Field("people")
                    .Resolve(_ => ("abc", "abc"));
            }

            Assert.Throws(Is.TypeOf<ConfigurationException>().And.Message.EqualTo("Error during Query.OnConfigure() call.\r\nField: Field(\"people\").Resolve<ValueTuple<string, string>>(_ => ...)\r\nThe type: ValueTuple<string, string> cannot be coerced effectively to a GraphQL type."), () =>
            {
                var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);
                ExecuteHelpers.CreateSchemaExecuter<TestUserContext>(queryType, null);
            });
        }

        [Test]
        public void ThrowOnFieldThatUsesInstanceMember()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field("instance", p => GetPersonName(p)),
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            Assert.Throws(Is.TypeOf<ConfigurationException>().And.Message.EqualTo("Error during PersonLoader.OnConfigure() call.\r\nField: Field(\"instance\", p => ConfigurationErrorTests.GetPersonName(p))\r\nClient projection (value(Epam.GraphQL.Tests.Diagnostics.ConfigurationErrorTests).GetPersonName(p)) contains a call of instance method 'GetPersonName' of type 'ConfigurationErrorTests'. This could potentially cause memory leak. Consider making the method static so that it does not capture constant in the instance."), () =>
            {
                var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);
                ExecuteHelpers.CreateSchemaExecuter<TestUserContext>(queryType, null);
            });
        }

        [Test]
        public void ThrowOnQueryConnectionWithFilterFromNonFilter()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field(p => p.Id),
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people")
                    .WithFilter<object>();
            }

            Assert.Throws(Is.TypeOf<ConfigurationException>().And.Message.EqualTo("Error during Query.OnConfigure() call.\r\nField: Field(\"people\").FromLoader<PersonLoader, Person>().AsConnection().WithFilter<object>()\r\nCannot find the corresponding generic base type `Filter<,,>` for type `object`."), () =>
            {
                var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);
                ExecuteHelpers.CreateSchemaExecuter<TestUserContext>(queryType, null);
            });
        }

        [Test(Description = "ApplySecurityFilter() based on another query")]
        public void ThrowIfApplySecurityFilterReturnsQueryNotBasedPassedQuery()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field(p => p.Id),
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable(),
                applySecurityFilter: (_, __) => Array.Empty<Person>().AsQueryable());

            TestHelpers.TestQueryError(
                query => query
                    .Connection(personLoaderType, "people"),
                typeof(ExecutionError),
                "PersonLoader.ApplySecurityFilter:  You cannot query data from anywhere (e.g. using context), please use passed `query` parameter.",
                @"
                query {
                    people {
                        items {
                            id
                        }
                    }
                }
                ");
        }

        [Test]
        public void ThrowIfFromBatchSourceTypeDoesNotContainFields()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field("error")
                        .FromBatch(people => people.ToDictionary(p => p, p => new Empty()), null);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable(),
                applySecurityFilter: (_, __) => Array.Empty<Person>().AsQueryable());

            Assert.Throws(Is.TypeOf<ConfigurationException>().And.Message.EqualTo("Type `Empty` must have a declaration of one field at least."), () =>
            {
                var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(query => query
                    .Connection(personLoaderType, "people"));
                ExecuteHelpers.CreateSchemaExecuter<TestUserContext>(queryType, null);
            });
        }

#pragma warning disable CA1822 // Mark members as static
        private string GetPersonName(Person person) => person.FullName;
#pragma warning restore CA1822 // Mark members as static

        public class Empty
        {
        }
    }
}
