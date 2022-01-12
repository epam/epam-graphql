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
    public class ExpressionRewriterTests
    {
        public interface IOther
        {
            public int Id { get; set; }
        }

        [Test]
        public void ShouldRewriteAnonymousObjectPropertyAccess()
        {
            Expression<Func<int, int>> originalExpression = x => new { Id = x, Id2 = 10 }.Id;
            Expression<Func<int, int>> expectedResult = x => x;
            var actualResult = ExpressionRewriter.Rewrite(originalExpression);

            Assert.That(actualResult, Is.EqualTo(expectedResult).Using(ExpressionEqualityComparer.Instance));
        }

        [Test]
        public void ShouldRewritePropertyAccessViaConvertedToBaseTypeObject()
        {
            Expression<Func<int, int>> originalExpression = x => new Derived { Id = x }.Id;
            Expression<Func<int, int>> expectedResult = x => x;
            var actualResult = ExpressionRewriter.Rewrite(originalExpression);

            Assert.That(actualResult, Is.EqualTo(expectedResult).Using(ExpressionEqualityComparer.Instance));
        }

        [Test]
        public void ShouldRewritePropertyAccessViaConvertedToNonConvertibleTypeObject()
        {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            Expression<Func<int, int>> originalExpression = x => (new Derived { Id = x } as IOther).Id;
            Expression<Func<int, int>> expectedResult = x => (new Derived { Id = x } as IOther).Id;
#pragma warning restore CS8602 // Dereference of a possibly null reference.
            var actualResult = ExpressionRewriter.Rewrite(originalExpression);

            Assert.That(actualResult, Is.EqualTo(expectedResult).Using(ExpressionEqualityComparer.Instance));
        }

        [Test]
        public void ShouldNotRewritePropertyAccessFromNotInitializedProperty()
        {
            Expression<Func<int, int>> originalExpression = x => new Derived().Id;
            Expression<Func<int, int>> expectedResult = x => new Derived().Id;
            var actualResult = ExpressionRewriter.Rewrite(originalExpression);

            Assert.That(actualResult, Is.EqualTo(expectedResult).Using(ExpressionEqualityComparer.Instance));
        }

        [Test]
        public void ShouldRewriteNestedNewExpressions()
        {
            Expression<Func<int, Base>> originalExpression = x => new Base { Id = new Base { Id = x }.Id };
            Expression<Func<int, Base>> expectedResult = x => new Base { Id = x };
            var actualResult = ExpressionRewriter.Rewrite(originalExpression);

            Assert.That(actualResult, Is.EqualTo(expectedResult).Using(ExpressionEqualityComparer.Instance));
        }

        [Test]
        public void ShouldRewriteOperandAndAlsoTrue()
        {
            Expression<Func<int, bool>> originalExpression = x => x == 1 && true;
            Expression<Func<int, bool>> expectedResult = x => x == 1;
            var actualResult = ExpressionRewriter.Rewrite(originalExpression);

            Assert.That(actualResult, Is.EqualTo(expectedResult).Using(ExpressionEqualityComparer.Instance));
        }

        [Test]
        public void ShouldRewriteTrueAndAlsoOperand()
        {
            Expression<Func<int, bool>> originalExpression = x => true && x == 1;
            Expression<Func<int, bool>> expectedResult = x => x == 1;
            var actualResult = ExpressionRewriter.Rewrite(originalExpression);

            Assert.That(actualResult, Is.EqualTo(expectedResult).Using(ExpressionEqualityComparer.Instance));
        }

        [Test]
        public void ShouldRewriteOperandAndAlsoFalse()
        {
            Expression<Func<int, bool>> originalExpression = x => x == 1 && false;
            Expression<Func<int, bool>> expectedResult = x => false;
            var actualResult = ExpressionRewriter.Rewrite(originalExpression);

            Assert.That(actualResult, Is.EqualTo(expectedResult).Using(ExpressionEqualityComparer.Instance));
        }

        [Test]
        public void ShouldRewriteFalseAndAlsoOperand()
        {
            Expression<Func<int, bool>> originalExpression = x => false && x == 1;
            Expression<Func<int, bool>> expectedResult = x => false;
            var actualResult = ExpressionRewriter.Rewrite(originalExpression);

            Assert.That(actualResult, Is.EqualTo(expectedResult).Using(ExpressionEqualityComparer.Instance));
        }

        [Test]
        public void ShouldRewriteOperandOrElseTrue()
        {
            Expression<Func<int, bool>> originalExpression = x => x == 1 || true;
            Expression<Func<int, bool>> expectedResult = x => true;
            var actualResult = ExpressionRewriter.Rewrite(originalExpression);

            Assert.That(actualResult, Is.EqualTo(expectedResult).Using(ExpressionEqualityComparer.Instance));
        }

        [Test]
        public void ShouldRewriteTrueOrElseOperand()
        {
            Expression<Func<int, bool>> originalExpression = x => true || x == 1;
            Expression<Func<int, bool>> expectedResult = x => true;
            var actualResult = ExpressionRewriter.Rewrite(originalExpression);

            Assert.That(actualResult, Is.EqualTo(expectedResult).Using(ExpressionEqualityComparer.Instance));
        }

        [Test]
        public void ShouldRewriteOperandOrElseFalse()
        {
            Expression<Func<int, bool>> originalExpression = x => x == 1 || false;
            Expression<Func<int, bool>> expectedResult = x => x == 1;
            var actualResult = ExpressionRewriter.Rewrite(originalExpression);

            Assert.That(actualResult, Is.EqualTo(expectedResult).Using(ExpressionEqualityComparer.Instance));
        }

        [Test]
        public void ShouldRewriteFalseOrElseOperand()
        {
            Expression<Func<int, bool>> originalExpression = x => false || x == 1;
            Expression<Func<int, bool>> expectedResult = x => x == 1;
            var actualResult = ExpressionRewriter.Rewrite(originalExpression);

            Assert.That(actualResult, Is.EqualTo(expectedResult).Using(ExpressionEqualityComparer.Instance));
        }

        public class Base
        {
            public int Id { get; set; }
        }

        public class Derived : Base
        {
        }
    }
}
