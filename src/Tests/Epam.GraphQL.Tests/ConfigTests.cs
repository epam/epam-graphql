// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Threading.Tasks;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using GraphQL.Language.AST;
using GraphQL.Validation;
using NUnit.Framework;

namespace Epam.GraphQL.Tests
{
    [TestFixture]
    public class ConfigTests
    {
        public interface ITest
        {
            int Id { get; }
        }

        [Test]
        public void InterfaceAsEntityForLoader()
        {
            var loaderType = GraphQLTypeBuilder.CreateLoaderType<ITest, TestUserContext>(
                onConfigure: loader => loader.Field(person => person.Id),
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => Enumerable.Empty<ITest>().AsQueryable());

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(onConfigure: query => query.Connection(loaderType, "test"));
            TestHelpers.TestQuery(
                query => query.Connection(loaderType, "test"),
                @"
                    query {
                        test {
                            items {
                                id
                            }
                        }
                    }
                ",
                @"{
                    test: {
                        items: []
                    }
                }");
        }

        [Test]
        public void InterfaceAsEntityForLoaderWithCalculatedField()
        {
            var loaderType = GraphQLTypeBuilder.CreateLoaderType<ITest, TestUserContext>(
                onConfigure: loader => loader.Field("id", person => person.Id + 1),
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => Enumerable.Empty<ITest>().AsQueryable());

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(onConfigure: query => query.Connection(loaderType, "test"));
            TestHelpers.TestQuery(
                query => query.Connection(loaderType, "test"),
                @"
                    query {
                        test {
                            items {
                                id
                            }
                        }
                    }
                ",
                @"{
                    test: {
                        items: []
                    }
                }");
        }

        [Test]
        public void ValidationRule()
        {
            var loaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader => loader.Field(person => person.Id),
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => Enumerable.Empty<Person>().AsQueryable());

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(onConfigure: query => query.Connection(loaderType, "test"));
            TestHelpers.TestQueryError(
                query => query.Connection(loaderType, "test"),
                typeof(ValidationError),
                "Permissions denied",
                @"
                    query {
                        test {
                            items {
                                id
                            }
                        }
                    }
                ",
                optionsBuilder: builder =>
                    builder
                        .UseValidationRule(new CustomValidationRule()));
        }

        public class CustomValidationRule : IValidationRule
        {
            public Task<INodeVisitor> ValidateAsync(ValidationContext context)
            {
                return Task.FromResult<INodeVisitor>(new EnterLeaveListener(x =>
                {
                    x.Match<Field>(
                        field => context.ReportError(new ValidationError(
                            context.OriginalQuery,
                            "403",
                            "Permissions denied",
                            new UnauthorizedAccessException(),
                            field)));
                }));
            }
        }
    }
}
