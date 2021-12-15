// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Reflection;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Utils
{
    [TestFixture]
    public class ILUtilsTests
    {
        public interface ITest
        {
            string Property { get; set; }

            string ReadOnlyProperty { get; }
        }

        public interface ITest2
        {
            string Property { get; set; }

            void Method();
        }

        public interface ITest3 : ITest2
        {
            string Property2 { get; set; }

            void Method2();
        }

        [Test]
        public void DefineTypeInterfaceAsABaseType()
        {
            Assert.Throws(Is.TypeOf<ArgumentException>().And.Message.EqualTo("Interfaces currently are not supported as `parent` argument."), () => ILUtils.DefineType("test", typeof(ITest)));
        }

        [Test]
        public void MakeTypeEmptyBaseType()
        {
            var props = new Dictionary<string, Type>()
            {
                ["prop"] = typeof(string),
            };

            props.MakeType("Test");
        }

        [Test]
        public void MakeTypeInterfaceAsABaseType()
        {
            var props = new Dictionary<string, Type>();

            props.MakeType("Test", typeof(ITest));
        }

        [Test]
        public void MakeTypeInterfaceWithMethodAsABaseType()
        {
            var props = new Dictionary<string, Type>();

            props.MakeType("Test", typeof(ITest2));
        }

        [Test]
        public void MakeTypeInterfaceWithBaseInterfaceAsABaseType()
        {
            var props = new Dictionary<string, Type>();

            props.MakeType("Test", typeof(ITest3));
        }

        [Test]
        public void MakeTypeClassInheritedFromInterface()
        {
            var props = new Dictionary<string, Type>();

            props.MakeType("Test", typeof(Test));
        }

        [Test]
        public void MakeTypeGenericBaseType()
        {
            var props = new Dictionary<string, Type>()
            {
                [nameof(Test<object>.Property)] = typeof(string),
                [nameof(Test<object>.GenericProperty)] = typeof(Test<>).GetTypeInfo().GenericTypeParameters[0],
            };

            var type = props.MakeType("Test", typeof(Test<>));
            type.MakeGenericType(new[] { typeof(int) });
        }

        [Test]
        public void MakeProxyTypeForAnonymousType()
        {
            var props = new Dictionary<string, Type>()
            {
                [nameof(Test<object>.Property)] = typeof(string),
            };

            var anonymous = new { Id = 1 };
            var type = anonymous.GetType();
            var newType = props.MakeProxyType("test");
            newType.MakeGenericType(type);
        }

        public class Test : ITest
        {
            public string Property { get; set; }

            public string ReadOnlyProperty => "Test";
        }

        public class Test<T>
        {
            public string Property { get; set; }

            public T GenericProperty { get; set; }
        }
    }
}
