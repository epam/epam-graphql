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
    }
}
