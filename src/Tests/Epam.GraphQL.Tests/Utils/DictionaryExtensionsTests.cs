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
    public class DictionaryExtensionsTests
    {
        [Test]
        public void MakeProxyTypeForObject()
        {
            var properties = new Dictionary<string, Type>
            {
                ["stringProp"] = typeof(string),
            };

            var proxyType = properties.MakeProxyType(nameof(Object)).MakeGenericType(typeof(object));

            Assert.AreEqual(typeof(Proxy<object>), proxyType.BaseType);

            var propInfo = proxyType.GetProperty("stringProp");
            Assert.NotNull(propInfo);
            Assert.AreEqual(typeof(string), propInfo.PropertyType);
            Assert.NotNull(propInfo.GetGetMethod());
            Assert.NotNull(propInfo.GetSetMethod());

            var originalPropInfo = proxyType.GetProperty("$original");
            Assert.NotNull(originalPropInfo);
            Assert.AreEqual(typeof(object), originalPropInfo.PropertyType);
            Assert.NotNull(originalPropInfo.GetGetMethod());
            Assert.NotNull(originalPropInfo.GetSetMethod());

            var instance = (Proxy<object>)Activator.CreateInstance(proxyType);

            Assert.Throws<NotImplementedException>(() => instance.GetOriginal());

            Assert.Throws<NotImplementedException>(() => propInfo.GetGetMethod().InvokeAndHoistBaseException(instance));
            Assert.Throws<NotImplementedException>(() => propInfo.GetSetMethod().InvokeAndHoistBaseException(instance, "test"));

            Assert.Throws<NotImplementedException>(() => originalPropInfo.GetGetMethod().InvokeAndHoistBaseException(instance));
            Assert.Throws<NotImplementedException>(() => originalPropInfo.GetSetMethod().InvokeAndHoistBaseException(instance, (object)null));
        }

        [Test]
        public void MakeProxyTypeForSimpleType()
        {
            var properties = new Dictionary<string, Type>
            {
                ["stringProp"] = typeof(string),
            };

            var proxyType = properties.MakeProxyType(nameof(SimpleType)).MakeGenericType(typeof(SimpleType));

            Assert.AreEqual(typeof(Proxy<SimpleType>), proxyType.BaseType);

            var propInfo = proxyType.GetProperty("stringProp");
            Assert.NotNull(propInfo);
            Assert.AreEqual(typeof(string), propInfo.PropertyType);
            Assert.NotNull(propInfo.GetGetMethod());
            Assert.NotNull(propInfo.GetSetMethod());

            var originalPropInfo = proxyType.GetProperty("$original");
            Assert.NotNull(originalPropInfo);
            Assert.AreEqual(typeof(SimpleType), originalPropInfo.PropertyType);
            Assert.NotNull(originalPropInfo.GetGetMethod());
            Assert.NotNull(originalPropInfo.GetSetMethod());

            var instance = (Proxy<SimpleType>)Activator.CreateInstance(proxyType);

            Assert.Throws<NotImplementedException>(() => instance.GetOriginal());

            Assert.Throws<NotImplementedException>(() => propInfo.GetGetMethod().InvokeAndHoistBaseException(instance));
            Assert.Throws<NotImplementedException>(() => propInfo.GetSetMethod().InvokeAndHoistBaseException(instance, "test"));

            Assert.Throws<NotImplementedException>(() => originalPropInfo.GetGetMethod().InvokeAndHoistBaseException(instance));
            Assert.Throws<NotImplementedException>(() => originalPropInfo.GetSetMethod().InvokeAndHoistBaseException(instance, (SimpleType)null));
        }

        public class SimpleType
        {
        }
    }
}
