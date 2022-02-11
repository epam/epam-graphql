// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Helpers;
using GraphQL;
using NSubstitute;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Utils
{
    [TestFixture]
    public class GuardsTests
    {
        [Test]
        public void ThrowIfArgIsNull()
        {
            string arg = null;
            Assert.Throws(
                Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("Value cannot be null. (Parameter 'arg')"),
                () => Guards.ThrowIfNull(arg, nameof(arg)));
        }

        [Test]
        public void ThrowArgException()
        {
            string arg = null;
            Assert.Throws(
                Is.TypeOf<ArgumentException>().And.Message.EqualTo("Message. (Parameter 'arg')"),
                () => Guards.ThrowArgumentExceptionIf(true, "Message.", nameof(arg)));
        }

        [Test]
        public void ThrowIfArgIsEmpty()
        {
            Assert.Throws(
                Is.TypeOf<ArgumentException>().And.Message.EqualTo("String value cannot be empty. (Parameter 'arg')"),
                () => Guards.ThrowIfNullOrEmpty(string.Empty, "arg"));
        }

        [Test]
        public void ShouldHaveOneParameterAtLeast()
        {
            Expression<Func<int>> expression = () => 0;
            Assert.Throws(
                Is.TypeOf<ArgumentException>().And.Message.EqualTo("Expression must have one parameter at least. (Parameter 'arg')"),
                () => expression.ShouldHaveOneParameterAtLeast("arg"));
        }

        [Test]
        public void ShouldHaveOnlyOneParameter()
        {
            Expression<Func<int>> expression = () => 0;
            Assert.Throws(
                Is.TypeOf<ArgumentException>().And.Message.EqualTo("Expression must have only one parameter. (Parameter 'arg')"),
                () => expression.ShouldHaveOnlyOneParameter("arg"));
        }

        [Test]
        public void ThrowInvalidOperation()
        {
            Assert.Throws(
                Is.TypeOf<InvalidOperationException>().And.Message.EqualTo("Exception message"),
                () => Guards.ThrowInvalidOperationIf(true, "Exception message"));
        }

        [Test]
        public void ThrowNotSupported()
        {
            Assert.Throws(
                Is.TypeOf<NotSupportedException>().And.Message.EqualTo("Specified method is not supported."),
                () => Guards.ThrowNotSupportedIf(true));
        }

        [Test]
        public void AssertField()
        {
            var context = Substitute.For<IResolveFieldContext>();

            context.Path.Returns(new[] { "my", "field" });

            Assert.Throws(
                Is.TypeOf<InvalidOperationException>().And.Message.EqualTo("Processing of the field `my.field` failed. This may indicate either a bug (you can create an issue https://epa.ms/gql-bug) or a limitation in Epam.GraphQL."),
                () => Guards.AssertField(true, context));
        }

        [Test]
        public void AssertType()
        {
            Assert.Throws(
                Is.TypeOf<InvalidOperationException>().And.Message.EqualTo("Processing of the type `int` failed. This may indicate either a bug (you can create an issue https://epa.ms/gql-bug) or a limitation in Epam.GraphQL."),
                () => Guards.AssertType<int>(true));
        }
    }
}
