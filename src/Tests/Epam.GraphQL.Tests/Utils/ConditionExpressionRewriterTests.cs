// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.Contracts.Models;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Tests.TestData;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Utils
{
    [TestFixture]
    public class ConditionExpressionRewriterTests : BaseTests
    {
        private static IEnumerable<TestCaseData> NotDependentTestData => GetNotDependedExpressions()
            .Select((e, index) => new[] { new TestCaseData(e, Array.Empty<Unit>()).SetName($"NotDependendEmpty #{index}"), new TestCaseData(e, Data.CreateUnits()).SetName($"NotDependendNotEmpty #{index}") })
            .SelectMany(e => e);

        private static IEnumerable<TestCaseData> TestData => GetTestData()
            .Select(data => new TestCaseData(data.Expression, data.UnitIds, data.PersonIds).SetName(data.Name));

        private static IEnumerable<TestCaseData> SelfReferencedTestData => GetSelfReferencedTestData()
            .Select(data => new TestCaseData(data.Expression, data.LeftIds, data.RightIds).SetName(data.Name));

        private static IEnumerable<TestCaseData> NotSupportedTestData => GetNotSupportedExpressions()
            .Select(data => new TestCaseData(data.Expression).SetName(data.Name));

        [TestCaseSource(nameof(NotDependentTestData))]
        public void TestNoDependencies(Expression<Func<Unit, Person, bool>> expression, IEnumerable<Unit> units)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            var rewrittenExpressionFactory = ConditionExpressionRewriter.Rewrite(expression);
            var rewrittenExpression = rewrittenExpressionFactory(units);

            // Should be equal by references
            Assert.AreEqual(expression.Body, rewrittenExpression.Body);
        }

        [TestCaseSource(nameof(TestData))]
        public void TestRewrite(Expression<Func<Unit, Person, bool>> expression, int[] unitIds, int[] expectedPersonIds)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            TestRewrite(FakeData.Units, FakeData.People, expression, unitIds, expectedPersonIds);
        }

        [TestCaseSource(nameof(SelfReferencedTestData))]
        public void TestRewrite(Expression<Func<Person, Person, bool>> expression, int[] leftIds, int[] expectedPersonIds)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            TestRewrite(FakeData.People, FakeData.People.Where(p => p.ManagerId != null), expression, leftIds, expectedPersonIds);
        }

        [TestCaseSource(nameof(NotSupportedTestData))]
        public void TestNotSupported(Expression<Func<Unit, Person, bool>> expression)
        {
            Assert.Throws(Is.TypeOf<NotSupportedException>(), () => ConditionExpressionRewriter.Rewrite(expression)(Array.Empty<Unit>()));
        }

        private static void TestRewrite<T1, T2>(IEnumerable<T1> leftValues, IEnumerable<T2> rightValues, Expression<Func<T1, T2, bool>> expression, int[] unitIds, int[] expectedPersonIds)
            where T1 : IHasId<int>
            where T2 : IHasId<int>
        {
            var rewrittenExpressionFactory = ConditionExpressionRewriter.Rewrite(expression);
            var rewrittenExpression = rewrittenExpressionFactory(leftValues.Where(u => unitIds.Contains(u.Id)));
            var people = rightValues.AsQueryable().Where(rewrittenExpression);

            var actualPersonIds = people.Select(p => p.Id).ToArray();

            // Should be equal by references
            Assert.AreEqual(expectedPersonIds, actualPersonIds);
            Assert.AreEqual(GetExpectedResult(leftValues, rightValues, expression.Compile(), unitIds), actualPersonIds);
        }

        private static IEnumerable<Expression<Func<Unit, Person, bool>>> GetNotDependedExpressions()
        {
            yield return (u, p) => true;
            yield return (u, p) => p.Id == 1;
#pragma warning disable SA1131 // Use readable conditions
            yield return (u, p) => 1 == p.Id;
#pragma warning restore SA1131 // Use readable conditions
            yield return (u, p) => p.Id == p.ManagerId;
            yield return (u, p) => p.Id == p.ManagerId && p.Id == 1;
        }

        private static IEnumerable<(Expression<Func<Unit, Person, bool>> Expression, string Name)> GetNotSupportedExpressions()
        {
            yield return ((u, p) => u.Id + 1 == p.UnitId, "EqualityExpr2");
            yield return ((u, p) => p.UnitId + 1 == u.Id + 1, "EqualityExpr3");
            yield return ((u, p) => u.Id + 1 == p.UnitId + 1, "EqualityExpr4");
            yield return ((u, p) => u.Id - p.UnitId == 0, "EqualityExpr5");
        }

        private static IEnumerable<(Expression<Func<Unit, Person, bool>> Expression, int[] UnitIds, int[] PersonIds, string Name)> GetTestData()
        {
            yield return ((u, p) => p.UnitId == u.Id, new[] { 1 }, new[] { 1, 2, 3 }, "Equality1");
            yield return ((u, p) => u.Id == p.UnitId, new[] { 1 }, new[] { 1, 2, 3 }, "Equality2");
            yield return ((u, p) => p.Unit.Id == u.Id, new[] { 1 }, new[] { 1, 2, 3 }, "Equality3");
            yield return ((u, p) => u.Id == p.Unit.Id, new[] { 1 }, new[] { 1, 2, 3 }, "Equality4");

            yield return ((u, p) => p.UnitId + 1 == u.Id, new[] { 2 }, new[] { 1, 2, 3 }, "EqualityExpr1");

            yield return ((u, p) => p.UnitId != u.Id, new[] { 1 }, new[] { 4, 5, 6 }, "Inequality1");
            yield return ((u, p) => u.Id != p.UnitId, new[] { 1 }, new[] { 4, 5, 6 }, "Inequality2");

            yield return ((u, p) => u.Id == 1, new[] { 1, 2 }, new[] { 1, 2, 3, 4, 5, 6 }, "IndependendOnly1");
            yield return ((u, p) => u.Id == 0, new[] { 1, 2 }, Array.Empty<int>(), "IndependendOnly2");

            yield return ((u, p) => p.UnitId == u.Id && u.Id == 1, new[] { 1, 2 }, new[] { 1, 2, 3 }, "And1");
            yield return ((u, p) => u.Id == 1 && p.UnitId == u.Id, new[] { 1, 2 }, new[] { 1, 2, 3 }, "And2");
            yield return ((u, p) => p.UnitId == u.Id && p.Unit.Id == u.Id, new[] { 1, 2 }, new[] { 1, 2, 3, 4, 5, 6 }, "And3");
            yield return ((u, p) => u.Id == 1 && u.Id == 1, new[] { 1, 2 }, new[] { 1, 2, 3, 4, 5, 6 }, "And4");
            yield return ((u, p) => u.Id == 1 && u.Id == 0, new[] { 1, 2 }, Array.Empty<int>(), "And5");

            yield return ((u, p) => p.UnitId == u.Id || u.Id == 1, new[] { 1, 2 }, new[] { 1, 2, 3, 4, 5, 6 }, "Or1");
            yield return ((u, p) => u.Id == 1 || p.UnitId == u.Id, new[] { 1, 2 }, new[] { 1, 2, 3, 4, 5, 6 }, "Or2");
            yield return ((u, p) => p.UnitId == u.Id || p.Unit.Id == u.Id, new[] { 1, 2 }, new[] { 1, 2, 3, 4, 5, 6 }, "Or3");
            yield return ((u, p) => u.Id == 1 || u.Id == 1, new[] { 1, 2 }, new[] { 1, 2, 3, 4, 5, 6 }, "Or4");
            yield return ((u, p) => u.Id == 1 || u.Id == 0, new[] { 1, 2 }, new[] { 1, 2, 3, 4, 5, 6 }, "Or5");

            yield return ((u, p) => !(p.UnitId == u.Id), new[] { 1 }, new[] { 4, 5, 6 }, "Not1");
            yield return ((u, p) => !(u.Id == p.UnitId), new[] { 1 }, new[] { 4, 5, 6 }, "Not2");
        }

        private static IEnumerable<(Expression<Func<Person, Person, bool>> Expression, int[] LeftIds, int[] RightIds, string Name)> GetSelfReferencedTestData()
        {
            yield return ((l, r) => r.ManagerId == l.Id, new[] { 1 }, new[] { 2, 3 }, "SelfReferenceEquality1");
            yield return ((l, r) => l.Id == r.ManagerId, new[] { 1 }, new[] { 2, 3 }, "SelfReferenceEquality2");
            yield return ((l, r) => r.Manager.Id == l.Id, new[] { 1 }, new[] { 2, 3 }, "SelfReferenceEquality3");
            yield return ((l, r) => l.Id == r.Manager.Id, new[] { 1 }, new[] { 2, 3 }, "SelfReferenceEquality4");

            yield return ((l, r) => r.ManagerId + 1 == l.Id, new[] { 2 }, new[] { 2, 3 }, "SelfReferenceEqualityExpr1");

            yield return ((l, r) => r.ManagerId != l.Id, new[] { 1 }, new[] { 4, 5, 6 }, "SelfReferenceInequality1");
            yield return ((l, r) => l.Id != r.ManagerId, new[] { 1 }, new[] { 4, 5, 6 }, "SelfReferenceInequality2");

            yield return ((l, r) => l.Id == 1, new[] { 1, 2 }, new[] { 2, 3, 4, 5, 6 }, "SelfReferenceIndependendOnly1");
            yield return ((l, r) => l.Id == 0, new[] { 1, 2 }, Array.Empty<int>(), "SelfReferenceIndependendOnly2");

            yield return ((l, r) => r.ManagerId == l.Id && l.Id == 1, new[] { 1, 2 }, new[] { 2, 3 }, "SelfReferenceAnd1");
            yield return ((l, r) => l.Id == 1 && r.ManagerId == l.Id, new[] { 1, 2 }, new[] { 2, 3 }, "SelfReferenceAnd2");
            yield return ((l, r) => r.ManagerId == l.Id && r.Manager.Id == l.Id, new[] { 1, 2 }, new[] { 2, 3, 4, 5 }, "SelfReferenceAnd3");
            yield return ((l, r) => l.Id == 1 && l.Id == 1, new[] { 1, 2 }, new[] { 2, 3, 4, 5, 6 }, "SelfReferenceAnd4");
            yield return ((l, r) => l.Id == 1 && l.Id == 0, new[] { 1, 2 }, Array.Empty<int>(), "SelfReferenceAnd5");

            yield return ((l, r) => r.ManagerId == l.Id || l.Id == 1, new[] { 1, 2 }, new[] { 2, 3, 4, 5, 6 }, "SelfReferenceOr1");
            yield return ((l, r) => l.Id == 1 || r.ManagerId == l.Id, new[] { 1, 2 }, new[] { 2, 3, 4, 5, 6 }, "SelfReferenceOr2");
            yield return ((l, r) => r.ManagerId == l.Id || r.Manager.Id == l.Id, new[] { 1, 2 }, new[] { 2, 3, 4, 5 }, "SelfReferenceOr3");
            yield return ((l, r) => l.Id == 1 || l.Id == 1, new[] { 1, 2 }, new[] { 2, 3, 4, 5, 6 }, "SelfReferenceOr4");
            yield return ((l, r) => l.Id == 1 || l.Id == 0, new[] { 1, 2 }, new[] { 2, 3, 4, 5, 6 }, "SelfReferenceOr5");

            yield return ((l, r) => !(r.ManagerId == l.Id), new[] { 1 }, new[] { 4, 5, 6 }, "SelfReferenceNot1");
            yield return ((l, r) => !(l.Id == r.ManagerId), new[] { 1 }, new[] { 4, 5, 6 }, "SelfReferenceNot2");
        }

        private static int[] GetExpectedResult<T1, T2>(IEnumerable<T1> leftValues, IEnumerable<T2> rightValues, Func<T1, T2, bool> func, int[] leftIds)
            where T1 : IHasId<int>
            where T2 : IHasId<int>
        {
            return leftValues
               .Where(u => leftIds.Contains(u.Id))
               .Select(u => rightValues.Where(p => func(u, p)).Select(p => p.Id))
               .SelectMany(i => i)
               .Distinct()
               .ToArray();
        }
    }
}
