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
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Utils
{
    [TestFixture]
    public class ExpressionEqualityComparerTests
    {
        private static readonly IEnumerable<TestCaseData> DifferentExpressions = GetDifferentExpressions()
            .Select(e => new TestCaseData(e.First, e.Second)
                .SetName($"AreNotEqual({e.First.ToTestName()}, {e.Second.ToTestName()})"));

        private static readonly IEnumerable<TestCaseData> SameExpressions = GetTheSameExpressions()
            .Select(e => new TestCaseData(e.First, e.Second)
                .SetName($"AreEqual({e.First.ToTestName()}, {e.Second.ToTestName()})"));

        public static int MinValue => int.MinValue;

        public static int MaxValue => int.MaxValue;

        [Test]
        [TestCaseSource(nameof(DifferentExpressions))]
        public void TestExpressionAreNotEqual(Expression first, Expression second)
        {
            var firstHash = ExpressionEqualityComparer.Instance.GetHashCode(first);
            var secondHash = ExpressionEqualityComparer.Instance.GetHashCode(second);
            var areEqual = ExpressionEqualityComparer.Instance.Equals(first, second);

            Assert.AreNotEqual(firstHash, secondHash);
            Assert.IsFalse(areEqual);
        }

        [Test]
        [TestCaseSource(nameof(SameExpressions))]
        public void TestExpressionAreEqual(Expression first, Expression second)
        {
            var firstHash = ExpressionEqualityComparer.Instance.GetHashCode(first);
            var secondHash = ExpressionEqualityComparer.Instance.GetHashCode(second);
            var areEqual = ExpressionEqualityComparer.Instance.Equals(first, second);

            Assert.AreEqual(firstHash, secondHash);
            Assert.IsTrue(areEqual);
        }

        private static IEnumerable<(Expression First, Expression Second)> GetDifferentExpressions()
        {
            {
                Expression<Func<string, string>> first = s => s;
                Expression<Func<object, object>> second = o => o;
                yield return (first, second);
            }

            {
                Expression<Func<int, int, int>> first = (x, y) => x + y;
                Expression<Func<int, int, int>> second = (x, y) => y + x;
                yield return (first, second);
            }

            {
                Expression<Func<object, bool>> first = o => o is string;
                Expression<Func<object, bool>> second = o => o is int;
                yield return (first, second);
            }

            {
#pragma warning disable IDE0075 // Simplify conditional expression
                Expression<Func<object, bool>> first = o => o != null ? true : false;
                Expression<Func<object, bool>> second = o => o != null ? false : true;
#pragma warning restore IDE0075 // Simplify conditional expression
                yield return (first, second);
            }

            {
                Expression<Func<int>> first = () => MaxValue;
                Expression<Func<int>> second = () => MinValue;
                yield return (first, second);
            }

            {
                Expression<Func<object>> first = () => new { Property = "Test1" };
                Expression<Func<object>> second = () => new { Property = "Test2" };
                yield return (first, second);
            }

            {
                Expression<Func<object>> first = () => new { Property1 = "Test" };
                Expression<Func<object>> second = () => new { Property2 = "Test" };
                yield return (first, second);
            }

            {
                Expression<Func<object>> first = () => new Test { Property1 = "Test" };
                Expression<Func<object>> second = () => new Test { Property2 = "Test" };
                yield return (first, second);
            }

            {
                Expression<Func<IEnumerable<int>>> first = () => new List<int> { 1, 2, 3 };
                Expression<Func<IEnumerable<int>>> second = () => new List<int> { 1, 2, 4 };
                yield return (first, second);
            }

            {
                Expression<Func<object>> first = () => new Test
                {
                    Children = { new Test { Property1 = "Test1" } }, // Member List Binding
                };
                Expression<Func<object>> second = () => new Test
                {
                    Children = { new Test { Property1 = "Test2" } },
                };
                yield return (first, second);
            }

            {
                Expression<Func<object>> first = () => new Test
                {
                    Parent = { Property1 = "Test1" }, // Member Member Binding
                };
                Expression<Func<object>> second = () => new Test
                {
                    Parent = { Property1 = "Test2" },
                };
                yield return (first, second);
            }
        }

        private static IEnumerable<(Expression First, Expression Second)> GetTheSameExpressions()
        {
            {
                Expression<Func<string, string>> first = s1 => s1;
                Expression<Func<string, string>> second = s2 => s2;
                yield return (first, second);
            }

            {
                Expression<Func<IQueryable<int>, int>> first = q => q.First(p => p == 0);
                Expression<Func<IQueryable<int>, int>> second = q => q.First(r => r == 0);
                yield return (first, second);
            }

            {
                Expression<Func<int, int, int>> first = (x1, y1) => x1 + y1;
                Expression<Func<int, int, int>> second = (y2, x2) => y2 + x2;
                yield return (first, second);
            }

            {
#pragma warning disable IDE0075 // Simplify conditional expression
                Expression<Func<object, bool>> first = o => o is object ? true : false;
                Expression<Func<object, bool>> second = o => o is object ? true : false;
#pragma warning restore IDE0075 // Simplify conditional expression
                yield return (first, second);
            }

            {
                Expression<Func<string, int>> first = s => s.Length;
                Expression<Func<string, int>> second = s => s.Length;
                yield return (first, second);
            }

            {
                Expression<Func<object>> first = () => new { Property = "Test" };
                Expression<Func<object>> second = () => new { Property = "Test" };
                yield return (first, second);
            }

            {
                Expression<Func<object>> first = () => new Test
                {
                    Property1 = "Test", // Assignment
                    Children = { new Test { Property1 = "Test" } }, // Member List Binding
                    Parent = { Property1 = "Test" }, // Member Member Binding
                };
                Expression<Func<object>> second = () => new Test
                {
                    Property1 = "Test",
                    Children = { new Test { Property1 = "Test" } },
                    Parent = { Property1 = "Test" },
                };
                yield return (first, second);
            }

            {
                Expression<Func<IEnumerable<int>>> first = () => new List<int> { 1, 2, 3 };
                Expression<Func<IEnumerable<int>>> second = () => new List<int> { 1, 2, 3 };
                yield return (first, second);
            }

            {
                Expression first = Expression.Default(typeof(int));
                Expression second = Expression.Default(typeof(int));
                yield return (first, second);
            }

            {
                Expression<Func<Func<int, int>, int>> first = func => func(0);
                Expression<Func<Func<int, int>, int>> second = func => func(0);
                yield return (first, second);
            }

            {
                Expression<Func<int[]>> first = () => Array.Empty<int>();
                Expression<Func<int[]>> second = () => Array.Empty<int>();
                yield return (first, second);
            }

            {
                Expression<Func<int[,], int, int>> first = (arr, index) => arr[index, index];
                Expression<Func<int[,], int, int>> second = (arr, index) => arr[index, index];
                yield return (first, second);
            }
        }

        private class Test
        {
            public string Property1 { get; set; }

            public string Property2 { get; set; }

            public List<Test> Children { get; set; }

            public Test Parent { get; set; }
        }
    }
}
