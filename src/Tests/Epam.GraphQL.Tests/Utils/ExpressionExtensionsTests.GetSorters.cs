// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Tests.TestData;
using NUnit.Framework;

#nullable enable

namespace Epam.GraphQL.Tests.Utils
{
    [TestFixture]
    public partial class ExpressionExtensionsTests
    {
        private static readonly Expression<Func<Person, int>> _idExpression = x => x.Id;
        private static readonly Expression<Func<Person, string>> _fullNameExpression = x => x.FullName;
        private static readonly Expression<Func<Person, int>> _getHashCodeExpression = x => x.GetHashCode();

        [Test]
        public void GetSortersOrderBy()
        {
            var result = ExpressionExtensions.GetSorters<Person>(query => query
                .OrderBy(x => x.Id));

            var expected = new List<(LambdaExpression, SortDirection)>()
            {
                (_idExpression, SortDirection.Asc),
            };

            Assert.That(result, Is.EqualTo(expected).Using(new ValueTupleEqualityComparer<LambdaExpression, SortDirection>(ExpressionEqualityComparer.Instance)));
        }

        [Test]
        public void GetSortersOrderByWithComparer()
        {
            var result = ExpressionExtensions.GetSorters<Person>(query => query
                .OrderBy(x => x.Id, Comparer<int>.Default));

            var expected = new List<(LambdaExpression, SortDirection)>()
            {
                (_idExpression, SortDirection.Asc),
            };

            Assert.That(result, Is.EqualTo(expected).Using(new ValueTupleEqualityComparer<LambdaExpression, SortDirection>(ExpressionEqualityComparer.Instance)));
        }

        [Test]
        public void GetSortersOrderByMethodCall()
        {
            var result = ExpressionExtensions.GetSorters<Person>(query => query
                .OrderBy(x => x.GetHashCode()));

            var expected = new List<(LambdaExpression, SortDirection)>()
            {
                (_getHashCodeExpression, SortDirection.Asc),
            };

            Assert.That(result, Is.EqualTo(expected).Using(new ValueTupleEqualityComparer<LambdaExpression, SortDirection>(ExpressionEqualityComparer.Instance)));
        }

        [Test]
        public void GetSortersOrderByDesc()
        {
            var result = ExpressionExtensions.GetSorters<Person>(query => query
                .OrderByDescending(x => x.Id));

            var expected = new List<(LambdaExpression, SortDirection)>()
            {
                (_idExpression, SortDirection.Desc),
            };

            Assert.That(result, Is.EqualTo(expected).Using(new ValueTupleEqualityComparer<LambdaExpression, SortDirection>(ExpressionEqualityComparer.Instance)));
        }

        [Test]
        public void GetSortersOrderByDescWithComparer()
        {
            var result = ExpressionExtensions.GetSorters<Person>(query => query
                .OrderByDescending(x => x.Id, Comparer<int>.Default));

            var expected = new List<(LambdaExpression, SortDirection)>()
            {
                (_idExpression, SortDirection.Desc),
            };

            Assert.That(result, Is.EqualTo(expected).Using(new ValueTupleEqualityComparer<LambdaExpression, SortDirection>(ExpressionEqualityComparer.Instance)));
        }

        [Test]
        public void GetSortersOrderByAndOrderBy()
        {
            var result = ExpressionExtensions.GetSorters<Person>(query => query
                .OrderBy(x => x.Id)
                .OrderBy(x => x.FullName));

            var expected = new List<(LambdaExpression, SortDirection)>()
            {
                (_fullNameExpression, SortDirection.Asc),
            };

            Assert.That(result, Is.EqualTo(expected).Using(new ValueTupleEqualityComparer<LambdaExpression, SortDirection>(ExpressionEqualityComparer.Instance)));
        }

        [Test]
        public void GetSortersOrderByDescAndOrderByDesc()
        {
            var result = ExpressionExtensions.GetSorters<Person>(query => query
                .OrderByDescending(x => x.Id)
                .OrderByDescending(x => x.FullName));

            var expected = new List<(LambdaExpression, SortDirection)>()
            {
                (_fullNameExpression, SortDirection.Desc),
            };

            Assert.That(result, Is.EqualTo(expected).Using(new ValueTupleEqualityComparer<LambdaExpression, SortDirection>(ExpressionEqualityComparer.Instance)));
        }

        [Test]
        public void GetSortersOrderByAndThenBy()
        {
            var result = ExpressionExtensions.GetSorters<Person>(query => query
                .OrderBy(x => x.Id)
                .ThenBy(x => x.FullName));

            var expected = new List<(LambdaExpression, SortDirection)>()
            {
                (_idExpression, SortDirection.Asc),
                (_fullNameExpression, SortDirection.Asc),
            };

            Assert.That(result, Is.EqualTo(expected).Using(new ValueTupleEqualityComparer<LambdaExpression, SortDirection>(ExpressionEqualityComparer.Instance)));
        }

        [Test]
        public void GetSortersOrderByAndThenByWithComparer()
        {
            var result = ExpressionExtensions.GetSorters<Person>(query => query
                .OrderBy(x => x.Id)
                .ThenBy(x => x.FullName, StringComparer.Ordinal));

            var expected = new List<(LambdaExpression, SortDirection)>()
            {
                (_idExpression, SortDirection.Asc),
                (_fullNameExpression, SortDirection.Asc),
            };

            Assert.That(result, Is.EqualTo(expected).Using(new ValueTupleEqualityComparer<LambdaExpression, SortDirection>(ExpressionEqualityComparer.Instance)));
        }

        [Test]
        public void GetSortersOrderByAndThenByDesc()
        {
            var result = ExpressionExtensions.GetSorters<Person>(query => query
                .OrderBy(x => x.Id)
                .ThenByDescending(x => x.FullName));

            var expected = new List<(LambdaExpression, SortDirection)>()
            {
                (_idExpression, SortDirection.Asc),
                (_fullNameExpression, SortDirection.Desc),
            };

            Assert.That(result, Is.EqualTo(expected).Using(new ValueTupleEqualityComparer<LambdaExpression, SortDirection>(ExpressionEqualityComparer.Instance)));
        }

        [Test]
        public void GetSortersOrderByAndThenByDescWithComparer()
        {
            var result = ExpressionExtensions.GetSorters<Person>(query => query
                .OrderBy(x => x.Id)
                .ThenByDescending(x => x.FullName, StringComparer.Ordinal));

            var expected = new List<(LambdaExpression, SortDirection)>()
            {
                (_idExpression, SortDirection.Asc),
                (_fullNameExpression, SortDirection.Desc),
            };

            Assert.That(result, Is.EqualTo(expected).Using(new ValueTupleEqualityComparer<LambdaExpression, SortDirection>(ExpressionEqualityComparer.Instance)));
        }

        [Test]
        public void GetSortersWhereAndOrderBy()
        {
            Assert.Throws(
                Is.TypeOf<ArgumentException>()
                    .And.Message.EqualTo("Expression should not contain method calls except OrderBy, OrderByDescending, ThenBy and ThenByDescending. Call of Where method was found."),
                () => ExpressionExtensions.GetSorters<Person>(query => query
                    .Where(x => x.Id > 0)
                    .OrderBy(x => x.Id)));
        }
    }
}
