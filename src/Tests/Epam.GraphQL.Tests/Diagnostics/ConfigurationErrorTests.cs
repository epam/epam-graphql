// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Text;
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
            Assert.Throws(
                Is.TypeOf<ConfigurationException>().And.Message.EqualTo(
                    ConcatLines(
                        "Error during Query.OnConfigure() call.",
                        "OnConfigure() method must have a declaration of one field at least.",
                        "Details:",
                        "public override void OnConfigure()",
                        "{",
                        "}")),
                () =>
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
            Assert.Throws(
                Is.TypeOf<ConfigurationException>().And.Message.EqualTo(
                    ConcatLines(
                        "Error during Query.OnConfigure() call.",
                        "Field `test` must have resolver.",
                        "Details:",
                        "public override void OnConfigure()",
                        "{",
                        "    Field(\"test\"); // <-----",
                        "}")),
                () =>
                {
                    var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(query => query
                        .Field("test"));
                    ExecuteHelpers.CreateSchemaExecuter<TestUserContext>(queryType, null);
                });
        }

        [Test]
        public void ThrowOnNullQueryFieldName()
        {
            Assert.Throws(
                Is.TypeOf<ConfigurationException>().And.Message.EqualTo(
                    ConcatLines(
                        "Error during Query.OnConfigure() call.",
                        "Field name cannot be null or empty.",
                        "Details:",
                        "public override void OnConfigure()",
                        "{",
                        "    Field(null)",
                        "        .Resolve<int>(_ => ...); // <-----",
                        "}")),
                () =>
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
            Assert.Throws(
                Is.TypeOf<ConfigurationException>().And.Message.EqualTo(
                    ConcatLines(
                        "Error during Query.OnConfigure() call.",
                        "Field name cannot be null or empty.",
                        "Details:",
                        "public override void OnConfigure()",
                        "{",
                        "    Field(\"\")",
                        "        .Resolve<int>(_ => ...); // <-----",
                        "}")),
                () =>
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

            Assert.Throws(
                Is.TypeOf<ConfigurationException>().And.Message.EqualTo(
                    ConcatLines(
                        "Error during Query.OnConfigure() call.",
                        "A field with the name `test` is already registered.",
                        "Details:",
                        "public override void OnConfigure()",
                        "{",
                        "    Field(\"test\")",
                        "        .Resolve<int>(_ => ...); // <-----",
                        string.Empty,
                        "    Field(\"test\")",
                        "        .Resolve<int>(_ => ...); // <-----",
                        "}")),
                () =>
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

            Assert.Throws(
                Is.TypeOf<ConfigurationException>().And.Message.EqualTo(
                    ConcatLines(
                        "Error during Query.OnConfigure() call.",
                        "Cannot find the corresponding generic base type `Loader<,>` for type `object`.",
                        "Details:",
                        "public override void OnConfigure()",
                        "{",
                        "    Connection<object>(\"test\"); // <-----",
                        "}")),
                () =>
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

            Assert.Throws(
                Is.TypeOf<ConfigurationException>().And.Message.EqualTo(
                    ConcatLines(
                        "Error during Query.OnConfigure() call.",
                        "Cannot find the corresponding generic base type `Loader<,>` for type `object`.",
                        "Details:",
                        "public override void OnConfigure()",
                        "{",
                        "    GroupConnection<object>(\"test\"); // <-----",
                        "}")),
                () =>
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

            Assert.Throws(
                Is.TypeOf<ConfigurationException>().And.Message.EqualTo(
                    ConcatLines(
                        "Error during PersonLoader.OnConfigure() call.",
                        "Field `test` must have resolver.",
                        "Details:",
                        "public override void OnConfigure()",
                        "{",
                        "    Field(\"test\"); // <-----",
                        "}")),
                () =>
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

            Assert.Throws(
                Is.TypeOf<ConfigurationException>().And.Message.EqualTo(
                    ConcatLines(
                        "Error during PersonLoader.OnConfigure() call.",
                        "Expression (p => (2 * p.Id)), provided for field is not a property. Consider giving a name to the field explicitly.",
                        "Details:",
                        "public override void OnConfigure()",
                        "{",
                        "    Field(p => 2 * p.Id); // <-----",
                        "}")),
                () =>
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

            Assert.Throws(
                Is.TypeOf<ConfigurationException>().And.Message.EqualTo(
                    ConcatLines(
                        "Error during Query.OnConfigure() call.",
                        "The type: ValueTuple<string, string> cannot be coerced effectively to a GraphQL type.",
                        "Details:",
                        "public override void OnConfigure()",
                        "{",
                        "    Field(\"people\")",
                        "        .Resolve<ValueTuple<string, string>>(_ => ...); // <-----",
                        "}")),
                () =>
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

            Assert.Throws(
                Is.TypeOf<ConfigurationException>().And.Message.EqualTo(
                    ConcatLines(
                        "Error during PersonLoader.OnConfigure() call.",
                        "Client projection (value(Epam.GraphQL.Tests.Diagnostics.ConfigurationErrorTests).GetPersonName(p)) contains a call of instance method 'GetPersonName' of type 'ConfigurationErrorTests'. This could potentially cause memory leak. Consider making the method static so that it does not capture constant in the instance.",
                        "Details:",
                        "public override void OnConfigure()",
                        "{",
                        "    Field(",
                        "        \"instance\",",
                        "        p => ConfigurationErrorTests.GetPersonName(p)); // <-----",
                        "}")),
                () =>
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

            Assert.Throws(
                Is.TypeOf<ConfigurationException>().And.Message.EqualTo(
                    ConcatLines(
                        "Error during Query.OnConfigure() call.",
                        "Cannot find the corresponding generic base type `Filter<,,>` for type `object`.",
                        "Details:",
                        "public override void OnConfigure()",
                        "{",
                        "    Field(\"people\")",
                        "        .FromLoader<PersonLoader, Person>()",
                        "        .AsConnection()",
                        "        .WithFilter<object>(); // <-----",
                        "}")),
                () =>
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

            Assert.Throws(
                Is.TypeOf<ConfigurationException>().And.Message.EqualTo(
                    ConcatLines(
                        "Error during PersonLoader.OnConfigure() call.",
                        "Type `Empty` must have a declaration of one field at least.",
                        "Details:",
                        "public override void OnConfigure()",
                        "{",
                        "    Field(\"error\")",
                        "        .FromBatch<Empty>(people => ...); // <-----",
                        "}")),
                () =>
                {
                    var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(query => query
                        .Connection(personLoaderType, "people"));
                    ExecuteHelpers.CreateSchemaExecuter<TestUserContext>(queryType, null);
                });
        }

        [Test]
        public void ThrowIfFromBatchConfigureContainsDuplicates()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field("error")
                        .FromBatch(
                            people => people.ToDictionary(p => p, p => new Empty()),
                            builder =>
                            {
                                builder.Field("test", x => 1);
                                builder.Field("test", x => 1);
                            });
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable(),
                applySecurityFilter: (_, __) => Array.Empty<Person>().AsQueryable());

            Assert.Throws(
                Is.TypeOf<ConfigurationException>().And.Message.EqualTo(
                    ConcatLines(
                        "Error during PersonLoader.OnConfigure() call.",
                        "A field with the name `test` is already registered.",
                        "Details:",
                        "public override void OnConfigure()",
                        "{",
                        "    Field(\"error\")",
                        "        .FromBatch<Empty>(",
                        "            people => ...,",
                        "            builder =>",
                        "            {",
                        "                Field(",
                        "                    \"test\",",
                        "                    x => 1); // <-----",
                        string.Empty,
                        "                Field(",
                        "                    \"test\",",
                        "                    x => 1); // <-----",
                        "            });",
                        "}")),
                () =>
                {
                    var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(query => query
                        .Connection(personLoaderType, "people"));
                    ExecuteHelpers.CreateSchemaExecuter<TestUserContext>(queryType, null);
                });
        }

        [Test]
        public void ThrowIfCustomFilterHasTheSameNameWithFilterableFieldTest()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName).Filterable();
                    loader.Filter<string>("fullName", name => p => p.FullName.Contains(name));
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable(),
                applySecurityFilter: (_, __) => Array.Empty<Person>().AsQueryable());

            Assert.Throws(
                Is.TypeOf<ConfigurationException>().And.Message.EqualTo(
                    ConcatLines(
                        "Error during PersonLoader.OnConfigure() call.",
                        "A filter for field with the name `fullName` is already registered.",
                        "Details:",
                        "public override void OnConfigure()",
                        "{",
                        "    Field(p => p.Id);",
                        string.Empty,
                        "    Field(p => p.FullName)",
                        "        .Filterable(); // <-----",
                        string.Empty,
                        "    Filter<string>(",
                        "        \"fullName\",",
                        "        name => ...); // <-----",
                        "}")),
                () =>
                {
                    var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);
                    ExecuteHelpers.CreateSchemaExecuter<TestUserContext>(queryType, null);
                });

            var personMutableLoaderType = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName).Editable().Filterable();
                    loader.Filter<string>("fullName", name => p => p.FullName.Contains(name));
                },
                getBaseQuery: _ => FakeData.People.AsQueryable(),
                applySecurityFilter: (_, __) => Array.Empty<Person>().AsQueryable());

            Assert.Throws(
                Is.TypeOf<ConfigurationException>().And.Message.EqualTo(
                    ConcatLines(
                        "Error during PersonLoader.OnConfigure() call.",
                        "A filter for field with the name `fullName` is already registered.",
                        "Details:",
                        "public override void OnConfigure()",
                        "{",
                        "    Field(p => p.Id);",
                        string.Empty,
                        "    Field(p => p.FullName)",
                        "        .Editable()",
                        "        .Filterable(); // <-----",
                        string.Empty,
                        "    Filter<string>(",
                        "        \"fullName\",",
                        "        name => ...); // <-----",
                        "}")),
                () =>
                {
                    var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(MutableBuilder);
                    ExecuteHelpers.CreateSchemaExecuter<TestUserContext>(queryType, null);
                });

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            void MutableBuilder(Query<TestUserContext> query)
            {
                query
                    .Connection(personMutableLoaderType, "people");
            }
        }

        [Test]
        public void ThrowIfCustomSorterHasTheSameNameWithSortableFieldTest()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName).Sortable();
                    loader.Sorter("fullName", p => p.FullName);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable(),
                applySecurityFilter: (_, __) => Array.Empty<Person>().AsQueryable());

            Assert.Throws(
                Is.TypeOf<ConfigurationException>().And.Message.EqualTo(
                    ConcatLines(
                        "Error during PersonLoader.OnConfigure() call.",
                        "A sorter with the name `fullName` is already registered.",
                        "Details:",
                        "public override void OnConfigure()",
                        "{",
                        "    Field(p => p.Id);",
                        string.Empty,
                        "    Field(p => p.FullName)",
                        "        .Sortable(); // <-----",
                        string.Empty,
                        "    Sorter(",
                        "        \"fullName\",",
                        "        p => p.FullName); // <-----",
                        "}")),
                () =>
                {
                    var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);
                    ExecuteHelpers.CreateSchemaExecuter<TestUserContext>(queryType, null);
                });

            var personMutableLoaderType = GraphQLTypeBuilder.CreateMutableLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Field(p => p.FullName).Editable().Sortable();
                    loader.Sorter("fullName", p => p.FullName);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable(),
                applySecurityFilter: (_, __) => Array.Empty<Person>().AsQueryable());

            Assert.Throws(
                Is.TypeOf<ConfigurationException>().And.Message.EqualTo(
                    ConcatLines(
                        "Error during PersonLoader.OnConfigure() call.",
                        "A sorter with the name `fullName` is already registered.",
                        "Details:",
                        "public override void OnConfigure()",
                        "{",
                        "    Field(p => p.Id);",
                        string.Empty,
                        "    Field(p => p.FullName)",
                        "        .Editable()",
                        "        .Sortable(); // <-----",
                        string.Empty,
                        "    Sorter(",
                        "        \"fullName\",",
                        "        p => p.FullName); // <-----",
                        "}")),
                () =>
                {
                    var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(MutableBuilder);
                    ExecuteHelpers.CreateSchemaExecuter<TestUserContext>(queryType, null);
                });

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            void MutableBuilder(Query<TestUserContext> query)
            {
                query
                    .Connection(personMutableLoaderType, "people");
            }
        }

        [Test]
        public void ThrowIfFromBatchSourceTypeContainsIndexedFieldOnly()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field("error")
                        .FromBatch(people => people.ToDictionary(p => p, p => new WithIndexedProperty()), null);
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => FakeData.People.AsQueryable(),
                applySecurityFilter: (_, __) => Array.Empty<Person>().AsQueryable());

            Assert.Throws(
                Is.TypeOf<ConfigurationException>().And.Message.EqualTo(
                    ConcatLines(
                        "Error during PersonLoader.OnConfigure() call.",
                        "Type `WithIndexedProperty` must have a declaration of one field at least.",
                        "Details:",
                        "public override void OnConfigure()",
                        "{",
                        "    Field(\"error\")",
                        "        .FromBatch<WithIndexedProperty>(people => ...); // <-----",
                        "}")),
                () =>
                {
                    var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);
                    ExecuteHelpers.CreateSchemaExecuter<TestUserContext>(queryType, null);
                });

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }
        }

        [Test]
        public void ThrowIfCustomSorterHasFieldName()
        {
            var personLoader = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id).Sortable();
                    loader.Sorter("id", p => p.Id);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable(),
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id));

            Assert.Throws(
                Is.TypeOf<ConfigurationException>().And.Message.EqualTo(
                    ConcatLines(
                        "Error during PersonLoader.OnConfigure() call.",
                        "A sorter with the name `id` is already registered.",
                        "Details:",
                        "public override void OnConfigure()",
                        "{",
                        "    Field(p => p.Id)",
                        "        .Sortable(); // <-----",
                        string.Empty,
                        "    Sorter(",
                        "        \"id\",",
                        "        p => p.Id); // <-----",
                        "}")),
                () =>
                {
                    var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);
                    ExecuteHelpers.CreateSchemaExecuter<TestUserContext>(queryType, null);
                });

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoader, "people");
            }
        }

        [Test]
        public void ThrowIfCustomSorterRegisteredTwice()
        {
            var personLoader = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id);
                    loader.Sorter("customSorter", p => p.Id);
                    loader.Sorter("customSorter", p => p.Id);
                },
                getBaseQuery: _ => FakeData.People.AsQueryable(),
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id));

            Assert.Throws(
                Is.TypeOf<ConfigurationException>().And.Message.EqualTo(
                    ConcatLines(
                        "Error during PersonLoader.OnConfigure() call.",
                        "A sorter with the name `customSorter` is already registered.",
                        "Details:",
                        "public override void OnConfigure()",
                        "{",
                        "    Field(p => p.Id);",
                        string.Empty,
                        "    Sorter(",
                        "        \"customSorter\",",
                        "        p => p.Id); // <-----",
                        string.Empty,
                        "    Sorter(",
                        "        \"customSorter\",",
                        "        p => p.Id); // <-----",
                        "}")),
                () =>
                {
                    var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);
                    ExecuteHelpers.CreateSchemaExecuter<TestUserContext>(queryType, null);
                });

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoader, "people");
            }
        }

        private static string ConcatLines(params string[] lines)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < lines.Length; i++)
            {
                if (i > 0)
                {
                    sb.AppendLine();
                }

                sb.Append(lines[i]);
            }

            return sb.ToString();
        }

#pragma warning disable CA1822 // Mark members as static
        private string GetPersonName(Person person) => person.FullName;
#pragma warning restore CA1822 // Mark members as static

        public class Empty
        {
        }

        public class WithIndexedProperty
        {
#pragma warning disable CA1065 // Do not raise exceptions in unexpected locations
            public int this[int index] => throw new NotImplementedException();
#pragma warning restore CA1065 // Do not raise exceptions in unexpected locations
        }
    }
}
