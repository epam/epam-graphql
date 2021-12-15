// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Utils
{
    [TestFixture]
    public class TypeExtensionsTests
    {
        [Test]
        public void MakeConcreteProxyTest()
        {
            var props = new Dictionary<string, Type>
            {
                ["stringProp"] = typeof(string),
                ["intProp"] = typeof(int),
            };

            var proxyType = props.MakeProxyType(nameof(Object));
            var concreteProxyType = proxyType.MakeInstantiatedProxyGenericType(new[] { "intProp" }, false).MakeGenericType(typeof(object));
            Assert.AreNotEqual(proxyType, concreteProxyType);

            var proxy = Activator.CreateInstance(concreteProxyType);
            var intPropInfo = concreteProxyType.GetProperty("intProp");
            Assert.IsNotNull(intPropInfo);
            var intPropGetter = intPropInfo.GetGetMethod();
            Assert.IsNotNull(intPropGetter);
            var intPropSetter = intPropInfo.GetSetMethod();
            Assert.IsNotNull(intPropSetter);

            intPropSetter.InvokeAndHoistBaseException(proxy, 100500);
            Assert.AreEqual(100500, intPropGetter.InvokeAndHoistBaseException<int>(proxy));

            var stringPropInfo = concreteProxyType.GetProperty("stringProp");
            Assert.IsNotNull(stringPropInfo);
            var stringPropGetter = stringPropInfo.GetGetMethod();
            Assert.IsNotNull(stringPropGetter);
            var stringPropSetter = stringPropInfo.GetSetMethod();
            Assert.IsNotNull(stringPropSetter);

            Assert.Throws<NotImplementedException>(() => stringPropGetter.InvokeAndHoistBaseException(proxy));
            Assert.Throws<NotImplementedException>(() => stringPropSetter.InvokeAndHoistBaseException(proxy, "test"));

            var originalPropInfo = concreteProxyType.GetProperty("$original");
            Assert.IsNotNull(originalPropInfo);
            var originalPropGetter = originalPropInfo.GetGetMethod();
            Assert.IsNotNull(originalPropGetter);
            var originalPropSetter = originalPropInfo.GetSetMethod();
            Assert.IsNotNull(originalPropSetter);

            Assert.Throws<NotImplementedException>(() => originalPropGetter.InvokeAndHoistBaseException(proxy));
            Assert.Throws<NotImplementedException>(() => originalPropSetter.InvokeAndHoistBaseException(proxy, "test"));
            Assert.Throws<NotImplementedException>(() => ((Proxy<object>)proxy).GetOriginal());
        }

        [Test]
        public void MakeConcreteProxyTestWithOriginal()
        {
            var props = new Dictionary<string, Type>
            {
                ["stringProp"] = typeof(string),
                ["intProp"] = typeof(int),
            };

            var proxyType = props.MakeProxyType(nameof(Object));
            var concreteProxyType = proxyType.MakeInstantiatedProxyGenericType(new[] { "intProp" }, true).MakeGenericType(typeof(object));
            Assert.AreNotEqual(proxyType, concreteProxyType);

            var proxy = Activator.CreateInstance(concreteProxyType);
            var intPropInfo = concreteProxyType.GetProperty("intProp");
            Assert.IsNotNull(intPropInfo);
            var intPropGetter = intPropInfo.GetGetMethod();
            Assert.IsNotNull(intPropGetter);
            var intPropSetter = intPropInfo.GetSetMethod();
            Assert.IsNotNull(intPropSetter);

            intPropSetter.InvokeAndHoistBaseException(proxy, 100500);
            Assert.AreEqual(100500, intPropGetter.InvokeAndHoistBaseException<int>(proxy));

            var stringPropInfo = concreteProxyType.GetProperty("stringProp");
            Assert.IsNotNull(stringPropInfo);
            var stringPropGetter = stringPropInfo.GetGetMethod();
            Assert.IsNotNull(stringPropGetter);
            var stringPropSetter = stringPropInfo.GetSetMethod();
            Assert.IsNotNull(stringPropSetter);

            Assert.Throws<NotImplementedException>(() => stringPropGetter.InvokeAndHoistBaseException(proxy));
            Assert.Throws<NotImplementedException>(() => stringPropSetter.InvokeAndHoistBaseException(proxy, "test"));

            var originalPropInfo = concreteProxyType.GetProperty("$original");
            Assert.IsNotNull(originalPropInfo);
            var originalPropGetter = originalPropInfo.GetGetMethod();
            Assert.IsNotNull(originalPropGetter);
            var originalPropSetter = originalPropInfo.GetSetMethod();
            Assert.IsNotNull(originalPropSetter);

            originalPropSetter.InvokeAndHoistBaseException(proxy, "test");
            Assert.AreEqual("test", originalPropGetter.InvokeAndHoistBaseException(proxy));
            Assert.AreEqual("test", ((Proxy<object>)proxy).GetOriginal());
        }
    }
}
