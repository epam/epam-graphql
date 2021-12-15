// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Configuration.Implementations.Fields.ExpressionFields;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Utils
{
    /// <summary>
    /// Test for https://git.epam.com/epm-ppa/epam-graphql/-/issues/22 (EF Query fails for loaders having instance methods in field expressions).
    /// </summary>
    [TestFixture]
    public class ExpressionValidatorTests
    {
#pragma warning disable CA1822 // Mark members as static
        private string InstanceProperty => "Test";
#pragma warning restore CA1822 // Mark members as static

        [Test]
        public void ShouldThrowOnInstanceMethodCall1()
        {
            Assert.Throws(
                Is.TypeOf<InvalidOperationException>().And.Message.EqualTo("Client projection (value(Epam.GraphQL.Tests.Utils.ExpressionValidatorTests).GetEntityProperty(e)) contains a call of instance method 'GetEntityProperty' of type 'ExpressionValidatorTests'. This could potentially cause memory leak. Consider making the method static so that it does not capture constant in the instance."),
                () => ExpressionValidator.Validate<Entity, string>(e => GetEntityProperty(e)));
        }

        [Test]
        public void ShouldThrowOnInstanceMethodCall2()
        {
            Assert.Throws(
                Is.TypeOf<InvalidOperationException>().And.Message.EqualTo("Client projection (value(Epam.GraphQL.Tests.Utils.ExpressionValidatorTests).GetEntityProperty(e)) contains a call of instance method 'GetEntityProperty' of type 'ExpressionValidatorTests'. This could potentially cause memory leak. Consider making the method static so that it does not capture constant in the instance."),
                () => ExpressionValidator.Validate<Context, Entity, string>((_, e) => GetEntityProperty(e)));
        }

        [Test]
        public void ShouldThrowOnInstanceMemberAccess()
        {
            Assert.Throws(
                Is.TypeOf<InvalidOperationException>().And.Message.EqualTo("Client projection (value(Epam.GraphQL.Tests.Utils.ExpressionValidatorTests).InstanceProperty) contains an access to a property/field that doesn't match any parameter. This could potentially cause memory leak. Consider making the method static so that it does not capture constant in the instance."),
                () => ExpressionValidator.Validate<Entity, string>(e => InstanceProperty));
        }

        [Test]
        public void ShouldNotThrowOnMethodCallOfParameter()
        {
            ExpressionValidator.Validate<Entity, int>(e => e.GetHashCode());
#pragma warning disable CA1305 // Specify IFormatProvider
            ExpressionValidator.Validate<Entity, string>(e => e.Age.ToString());
#pragma warning restore CA1305 // Specify IFormatProvider
        }

        [Test]
        public void ShouldNotThrowOnStaticProperty()
        {
            ExpressionValidator.Validate<Entity, string>(e => string.Empty);
        }

        [Test]
        public void ShouldNotThrowOnConstant()
        {
            ExpressionValidator.Validate<Entity, string>(e => "Test");
        }

#pragma warning disable CA1822 // Mark members as static
        private string GetEntityProperty(Entity entity) => entity.Name;
#pragma warning restore CA1822 // Mark members as static

        private class Context
        {
        }

        private class Entity
        {
            public string Name { get; set; }

            public int Age { get; set; }
        }
    }
}
