// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Linq;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Configuration.Implementations;
using Epam.GraphQL.Configuration.Implementations.Fields;
using Epam.GraphQL.Configuration.Implementations.Fields.ExpressionFields;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Tests.TestData;
using NSubstitute;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Internals
{
    [TestFixture]
    public class ProxyAccessorTests : BaseTests
    {
        private IRegistry<TestUserContext> _registry;

        [SetUp]
        public void Setup()
        {
            _registry = Substitute.For<IRegistry<TestUserContext>>();

            _registry.GetGraphQLTypeName<Person>(Arg.Any<bool>(), Arg.Any<IField<TestUserContext>>())
                .Returns("Test");
        }

        [Test]
        public void ExpressionField()
        {
            var configurator = new ObjectGraphTypeConfigurator<Person, TestUserContext>(
                parent: null,
                registry: _registry,
                isAuto: false);

            configurator.AddField(
                new ExpressionField<Person, int, TestUserContext>(
                    parent: configurator,
                    expression: person => person.Id,
                    name: null),
                null);

            var proxyAccessor = new ProxyAccessor<Person, TestUserContext>(configurator);
            var expression = proxyAccessor.CreateSelectorExpression(new[] { "id" })
                .Compile();
            var context = new TestUserContext(null);

            var proxy = expression(context, FakeData.SophieGandley);
            Assert.AreEqual(FakeData.SophieGandley.Id, proxy.GetPropertyValue(proxy.GetType().GetProperty("id")));
        }

        [Test]
        public void ExpressionFieldsDuplication()
        {
            var configurator = new ObjectGraphTypeConfigurator<Person, TestUserContext>(
                parent: null,
                registry: _registry,
                isAuto: false);

            configurator.AddField(
                new ExpressionField<Person, int, TestUserContext>(
                    parent: configurator,
                    expression: person => person.Id,
                    name: null),
                null);
            configurator.AddField(
                new ExpressionField<Person, int, TestUserContext>(
                    parent: configurator,
                    expression: person => person.Id,
                    name: "duplicateId"),
                null);

            var proxyAccessor = new ProxyAccessor<Person, TestUserContext>(configurator);
            var expression = proxyAccessor.CreateSelectorExpression(new[] { "id", "duplicateId" })
                .Compile();
            var context = new TestUserContext(null);

            var proxy = expression(context, FakeData.SophieGandley);
            Assert.AreEqual(FakeData.SophieGandley.Id, proxy.GetPropertyValue(proxy.GetType().GetProperty("id")));
            Assert.AreEqual(FakeData.SophieGandley.Id, proxy.GetPropertyValue(proxy.GetType().GetProperty("duplicateId")));
        }

        [Test]
        public void ConditionMemberWithExistingExpressionField()
        {
            var configurator = new ObjectGraphTypeConfigurator<Person, TestUserContext>(
                parent: null,
                registry: _registry,
                isAuto: false);

            configurator.AddField(
                new ExpressionField<Person, int, TestUserContext>(
                    parent: configurator,
                    expression: person => person.Id,
                    name: null),
                null);

            var proxyAccessor = new ProxyAccessor<Person, TestUserContext>(configurator);
            proxyAccessor.AddMember("children", person => person.Id);

            var proxyType = proxyAccessor.ProxyGenericType;

            var props = proxyType.GetProperties().Select(propInfo => propInfo.Name).OrderBy(name => name);
            Assert.AreEqual(new[] { "id" }, props);
        }

        [Test]
        public void ConditionMemberWithExistingNonExpressionField()
        {
            var configurator = new ObjectGraphTypeConfigurator<Person, TestUserContext>(
                parent: null,
                registry: _registry,
                isAuto: false);

            configurator.AddField(
                new ExpressionField<Person, int, TestUserContext>(
                    parent: configurator,
                    expression: person => person.Id,
                    name: null),
                null);
            configurator.AddField(
                new TypedField<Person, int, TestUserContext>(
                    parent: configurator,
                    name: "unitId"),
                null);

            var proxyAccessor = new ProxyAccessor<Person, TestUserContext>(configurator);
            proxyAccessor.AddMember("unit", person => person.UnitId);

            var proxyType = proxyAccessor.ProxyGenericType;

            var props = proxyType.GetProperties().Select(propInfo => propInfo.Name).OrderBy(name => name);
            Assert.AreEqual(new[] { "id", "unit$1", "unitId" }, props);
        }

        [Test]
        public void ConditionMemberWithNonExistingExpressionField()
        {
            var configurator = new ObjectGraphTypeConfigurator<Person, TestUserContext>(
                parent: null,
                registry: _registry,
                isAuto: false);

            configurator.AddField(
                new ExpressionField<Person, int, TestUserContext>(
                    parent: configurator,
                    expression: person => person.Id,
                    name: null),
                null);

            var proxyAccessor = new ProxyAccessor<Person, TestUserContext>(configurator);
            proxyAccessor.AddMember("unit", person => person.UnitId);

            var proxyType = proxyAccessor.ProxyGenericType;

            var props = proxyType.GetProperties().Select(propInfo => propInfo.Name).OrderBy(name => name);
            Assert.AreEqual(new[] { "id", "unitId" }, props);
        }
    }
}
