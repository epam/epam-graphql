// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Helpers;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Utils
{
    [TestFixture]
    public class ConditionFactorizationTests
    {
        [Test]
        public void NotEqualExpression()
        {
            Expression<Func<int, int, bool>> expr = (left, right) => left != right;
            Assert.IsFalse(ExpressionHelpers.TryFactorizeCondition(expr, out var result));
        }

        [Test]
        public void OrExpression()
        {
            Expression<Func<int, int, bool>> expr = (left, right) => left == right || right == 0;
            Assert.IsFalse(ExpressionHelpers.TryFactorizeCondition(expr, out var result));
        }

        [Test]
        public void AndOrExpression()
        {
            Expression<Func<int, int, bool>> expr = (left, right) => (left == right && right == 0) || left == 0;
            Assert.IsFalse(ExpressionHelpers.TryFactorizeCondition(expr, out var result));
        }

        [Test]
        public void EqualIndependentExpression()
        {
            Expression<Func<int, int, bool>> expr = (left, right) => true;
            Assert.IsFalse(ExpressionHelpers.TryFactorizeCondition(expr, out var result));
        }

        [Test]
        public void EqualLeftIndependentExpression()
        {
#pragma warning disable SA1131 // Use readable conditions
            Expression<Func<int, int, bool>> expr = (left, right) => 1 == right;
#pragma warning restore SA1131 // Use readable conditions
            Assert.IsFalse(ExpressionHelpers.TryFactorizeCondition(expr, out var result));
        }

        [Test]
        public void EqualRightIndependentExpression()
        {
            Expression<Func<int, int, bool>> expr = (left, right) => left == 1;
            Assert.IsFalse(ExpressionHelpers.TryFactorizeCondition(expr, out var result));
        }

        [Test]
        public void EqualExpression()
        {
            Expression<Func<int, int, bool>> expr = (left, right) => left == right;
            Assert.IsTrue(ExpressionHelpers.TryFactorizeCondition(expr, out var result));

            Expression<Func<int, int>> expected = i => i;
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expected, result.LeftExpression));
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expected, result.RightExpression));
            Assert.IsNull(result.RightCondition);
        }

        [Test]
        public void EqualNullableExpression()
        {
            Expression<Func<int, int?, bool>> expr = (left, right) => left == right;
            Assert.IsTrue(ExpressionHelpers.TryFactorizeCondition(expr, out var result));

            Expression<Func<int, int?>> leftExpected = i => i;
            Expression<Func<int?, int?>> rightExpected = i => i;
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(leftExpected, result.LeftExpression));
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(rightExpected, result.RightExpression));
            Assert.IsNull(result.RightCondition);
        }

        [Test]
        public void EqualNullableReverseOrderExpression()
        {
            Expression<Func<int, int?, bool>> expr = (left, right) => right == left;
            Assert.IsTrue(ExpressionHelpers.TryFactorizeCondition(expr, out var result));

            Expression<Func<int, int?>> leftExpected = i => i;
            Expression<Func<int?, int?>> rightExpected = i => i;
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(leftExpected, result.LeftExpression));
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(rightExpected, result.RightExpression));
            Assert.IsNull(result.RightCondition);
        }

        [Test]
        public void EqualAndLeftConditionExpression()
        {
            Expression<Func<int, int, bool>> expr = (left, right) => left == right && left == 1;
            Assert.IsFalse(ExpressionHelpers.TryFactorizeCondition(expr, out var result));
        }

        [Test]
        public void EqualAndRightConditionExpression()
        {
            Expression<Func<int, int, bool>> expr = (left, right) => left == right && right == 1;
            Assert.IsTrue(ExpressionHelpers.TryFactorizeCondition(expr, out var result));

            Expression<Func<int, int>> expected = i => i;
            Expression<Func<int, bool>> expectedCondition = i => i == 1;
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expected, result.LeftExpression));
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expected, result.RightExpression));
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expectedCondition, result.RightCondition));
        }

        [Test]
        public void RightConditionAndEqualExpression()
        {
            Expression<Func<int, int, bool>> expr = (left, right) => right == 1 && left == right;
            Assert.IsTrue(ExpressionHelpers.TryFactorizeCondition(expr, out var result));

            Expression<Func<int, int>> expected = i => i;
            Expression<Func<int, bool>> expectedCondition = i => i == 1;
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expected, result.LeftExpression));
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expected, result.RightExpression));
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expectedCondition, result.RightCondition));
        }

        [Test]
        public void EqualAndTwoRightConditionsExpression()
        {
            Expression<Func<int, int, bool>> expr = (left, right) => left == right && right == 1 && right != 5;
            Assert.IsTrue(ExpressionHelpers.TryFactorizeCondition(expr, out var result));

            Expression<Func<int, int>> expected = i => i;
            Expression<Func<int, bool>> expectedCondition = i => i == 1 && i != 5;
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expected, result.LeftExpression));
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expected, result.RightExpression));
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expectedCondition, result.RightCondition));
        }

        [Test]
        public void RightConditionAndEqualAndRightConditionExpression()
        {
            Expression<Func<int, int, bool>> expr = (left, right) => right == 1 && left == right && right != 5;
            Assert.IsTrue(ExpressionHelpers.TryFactorizeCondition(expr, out var result));

            Expression<Func<int, int>> expected = i => i;
            Expression<Func<int, bool>> expectedCondition = i => i == 1 && i != 5;
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expected, result.LeftExpression));
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expected, result.RightExpression));
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expectedCondition, result.RightCondition));
        }
    }
}
