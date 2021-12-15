// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Helpers;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Utils
{
    [TestFixture]
    public class ExpressionFactorizationTests
    {
        [Test]
        public void TestNotDependent()
        {
            Expression<Func<int, string, int>> expr = (p1, p2) => 0;

            var result = ExpressionHelpers.Factorize(expr);
            Assert.IsEmpty(result.LeftExpressions);
            Assert.IsEmpty(result.RightExpressions);

            Expression<Func<int>> expected = () => 0;
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expected, result.Expression));
        }

        [Test]
        public void DependentOnLeftOnly()
        {
            Expression<Func<int, string, int>> expr = (p1, p2) => p1 + 5;

            var result = ExpressionHelpers.Factorize(expr);
            Assert.AreEqual(1, result.LeftExpressions.Count);

            Expression<Func<int, int>> expectedLeftExpr = p1 => p1 + 5;

            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expectedLeftExpr, result.LeftExpressions[0]));
            Assert.IsEmpty(result.RightExpressions);

            Expression<Func<int, int>> expected = p1 => p1;
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expected, result.Expression));
        }

#pragma warning disable CA1305 // Specify IFormatProvider
        [Test]
        public void DependentOnRightOnly()
        {
            Expression<Func<int, string, int>> expr = (p1, p2) => Convert.ToInt32(p2);

            var result = ExpressionHelpers.Factorize(expr);
            Assert.AreEqual(1, result.RightExpressions.Count);

            Expression<Func<string, int>> expectedRightExpr = p2 => Convert.ToInt32(p2);

            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expectedRightExpr, result.RightExpressions[0]));
            Assert.IsEmpty(result.LeftExpressions);

            Expression<Func<int, int>> expected = p1 => p1;
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expected, result.Expression));
        }

        [Test]
        public void DependentOnBoth()
        {
            Expression<Func<int, string, int>> expr = (p1, p2) => Convert.ToInt32(p2) + p1 * 2;

            var result = ExpressionHelpers.Factorize(expr);

            Assert.AreEqual(1, result.LeftExpressions.Count);
            Assert.AreEqual(1, result.RightExpressions.Count);

            Expression<Func<int, int>> expectedLeftExpr = p1 => p1 * 2;
            Expression<Func<string, int>> expectedRightExpr = p2 => Convert.ToInt32(p2);

            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expectedLeftExpr, result.LeftExpressions[0]));
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expectedRightExpr, result.RightExpressions[0]));

            Expression<Func<int, int, int>> expected = (p1, p2) => p2 + p1;
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expected, result.Expression));
        }

        [Test]
        public void DependentOnBothMultiple()
        {
            Expression<Func<int, string, bool>> expr = (p1, p2) => Convert.ToInt32(p2) + p1 * 2 == Convert.ToInt32(p2) && p2 + "1" == p1.ToString();

            var result = ExpressionHelpers.Factorize(expr);

            Assert.AreEqual(2, result.LeftExpressions.Count);
            Assert.AreEqual(3, result.RightExpressions.Count);

            Expression<Func<int, int>> expectedLeftExpr1 = p1 => p1 * 2;
            Expression<Func<int, string>> expectedLeftExpr2 = p1 => p1.ToString();

            Expression<Func<string, int>> expectedRightExpr1 = p2 => Convert.ToInt32(p2);
            Expression<Func<string, string>> expectedRightExpr3 = p2 => p2 + "1";

            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expectedLeftExpr1, result.LeftExpressions[0]));
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expectedLeftExpr2, result.LeftExpressions[1]));
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expectedRightExpr1, result.RightExpressions[0]));
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expectedRightExpr1, result.RightExpressions[1]));
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expectedRightExpr3, result.RightExpressions[2]));

            Expression<Func<int, string, int, int, string, bool>> expected = (l1, l2, r1, r2, r3) => r1 + l1 == r2 && r3 == l2;
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expected, result.Expression));
        }
#pragma warning restore CA1305 // Specify IFormatProvider

        [Test]
        public void TestFuncLambdaExpression()
        {
            Expression<Func<int[], int, int[]>> expr = (ids, id) => ids.Where(i => i > id).ToArray();

            var result = ExpressionHelpers.Factorize(expr);

            Assert.AreEqual(1, result.LeftExpressions.Count);
            Assert.AreEqual(1, result.RightExpressions.Count);

            Expression<Func<int[], int[]>> expectedLeftExpr = id => id;

            Expression<Func<int, Func<int, bool>>> expectedRightExpr = id => i => i > id;

            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expectedLeftExpr, result.LeftExpressions[0]));
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expectedRightExpr, result.RightExpressions[0]));

            Expression<Func<int[], Func<int, bool>, int[]>> expected = (l1, r1) => l1.Where(r1).ToArray();
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expected, result.Expression));
        }

        [Test]
        public void TestExpressionLambdaExpression()
        {
            Expression<Func<IQueryable<int>, int, IQueryable<int>>> expr = (ids, id) => ids.Where(i => i > id);

            var result = ExpressionHelpers.Factorize(expr);

            Assert.AreEqual(1, result.LeftExpressions.Count);
            Assert.AreEqual(1, result.RightExpressions.Count);

            Expression<Func<IQueryable<int>, IQueryable<int>>> expectedLeftExpr = id => id;

            Expression<Func<int, Expression<Func<int, bool>>>> expectedRightExpr = id => i => i > id;

            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expectedLeftExpr, result.LeftExpressions[0]));
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expectedRightExpr, result.RightExpressions[0]));

            Expression<Func<IQueryable<int>, Expression<Func<int, bool>>, IQueryable<int>>> expected = (l1, r1) => l1.Where(r1);
            Assert.IsTrue(ExpressionEqualityComparer.Instance.Equals(expected, result.Expression));
        }
    }
}
