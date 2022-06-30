// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Filtration
{
    [TestFixture]
    public class GetExpressionByFilterValueTests
    {
        public enum Issue83
        {
            InProgress,
        }

        [Test]
        public void GetExpressionByFilterValue()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id).Filterable();
                    loader.Field(p => p.FullName).Filterable();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => Enumerable.Empty<Person>().AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);
            var schemaExecuter = ExecuteHelpers.CreateSchemaExecuter<TestUserContext>(queryType, null);
            var filterValue = new Dictionary<string, object>
            {
                ["id"] = new Dictionary<string, object>
                {
                    ["eq"] = 100500,
                },
            };

            var filterExpression = schemaExecuter.GetExpressionByFilterValue<Person, TestUserContext>(personLoaderType, new TestUserContext(null), filterValue);
            Expression<Func<Person, bool>> expected = p => p.Id == 100500;

            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(filterExpression, expected));
        }

        [Test]
        public void Issue83Test()
        {
            var personLoaderType = GraphQLTypeBuilder.CreateLoaderType<Person, TestUserContext>(
                onConfigure: loader =>
                {
                    loader.Field(p => p.Id).Filterable();
                    loader.Field("test", p => Issue83.InProgress).Filterable();
                },
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                getBaseQuery: _ => Enumerable.Empty<Person>().AsQueryable());

            void Builder(Query<TestUserContext> query)
            {
                query
                    .Connection(personLoaderType, "people");
            }

            var queryType = GraphQLTypeBuilder.CreateQueryType<TestUserContext>(Builder);
            var schemaExecuter = ExecuteHelpers.CreateSchemaExecuter<TestUserContext>(queryType, null);
            var filterValue = new Dictionary<string, object>
            {
                ["test"] = new Dictionary<string, object>
                {
                    ["eq"] = "IN_PROGRESS",
                },
            };

            var filterExpression = schemaExecuter.GetExpressionByFilterValue<Person, TestUserContext>(personLoaderType, new TestUserContext(null), filterValue);
#pragma warning disable CS1718 // Comparison made to same variable
            Expression<Func<Person, Issue83>> expected = p => Issue83.InProgress;
#pragma warning restore CS1718 // Comparison made to same variable

            Assert.IsTrue(filterExpression.Body.NodeType == ExpressionType.Equal);
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(((BinaryExpression)filterExpression.Body).Left, expected.Body));
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(((BinaryExpression)filterExpression.Body).Right, expected.Body));
        }
    }
}
