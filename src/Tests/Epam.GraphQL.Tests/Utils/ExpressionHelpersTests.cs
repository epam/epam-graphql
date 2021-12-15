// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Tests.TestData;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Utils
{
    [TestFixture]
    public class ExpressionHelpersTests
    {
        /// <summary>
        /// Test for https://git.epam.com/epm-ppa/epam-graphql/-/issues/95.
        /// </summary>
        [Test]
        public void MakeContainsExpressionResultShouldHaveIEnumerableValueType()
        {
            var expression = ExpressionHelpers.MakeContainsExpression<Person, int>(new[] { 1 }, x => x.Id);
            var methodCallExpression = expression.Body as MethodCallExpression;
            Assert.NotNull(methodCallExpression);
            Assert.NotNull(methodCallExpression.Arguments);
            var firstArgument = methodCallExpression.Arguments.First();
            Assert.NotNull(firstArgument);
            var propertyExpression = firstArgument as MemberExpression;
            Assert.NotNull(propertyExpression);
            Assert.AreEqual(typeof(IEnumerable<int>), propertyExpression.Type);
        }

        /// <summary>
        /// Test for https://git.epam.com/epm-ppa/epam-graphql/-/issues/95.
        /// </summary>
        [Test]
        public void MakeContainsExpressionNullableResultShouldHaveIEnumerableValueType()
        {
            var expression = ExpressionHelpers.MakeContainsExpression<Person, int>(new[] { 1 }, x => x.ManagerId);
            var andExpression = expression.Body as BinaryExpression;
            Assert.NotNull(andExpression);
            var methodCallExpression = andExpression.Right as MethodCallExpression;
            Assert.NotNull(methodCallExpression);
            Assert.NotNull(methodCallExpression.Arguments);
            var firstArgument = methodCallExpression.Arguments.First();
            Assert.NotNull(firstArgument);
            var propertyExpression = firstArgument as MemberExpression;
            Assert.NotNull(propertyExpression);
            Assert.AreEqual(typeof(IEnumerable<int>), propertyExpression.Type);
        }
    }
}
