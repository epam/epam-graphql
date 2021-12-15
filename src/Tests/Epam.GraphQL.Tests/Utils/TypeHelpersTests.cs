// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.Contracts.Models;
using Epam.GraphQL.Helpers;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Utils
{
    [TestFixture]
    public class TypeHelpersTests
    {
        private interface IHasIntId : IHasId<int>
        {
        }

        [TestCase(typeof(Class1), typeof(Class2), ExpectedResult = typeof(object))]
        [TestCase(typeof(Class2), typeof(Class1), ExpectedResult = typeof(object))]
        [TestCase(typeof(Class11), typeof(Class1), ExpectedResult = typeof(Class1))]
        [TestCase(typeof(Class1), typeof(Class11), ExpectedResult = typeof(Class1))]
        [TestCase(typeof(Class11), typeof(Class12), ExpectedResult = typeof(Class1))]
        [TestCase(typeof(Class12), typeof(Class11), ExpectedResult = typeof(Class1))]
        [TestCase(typeof(Class3), typeof(Class4), ExpectedResult = typeof(IHasId<int>))]
        [TestCase(typeof(Class4), typeof(Class3), ExpectedResult = typeof(IHasId<int>))]
        [TestCase(typeof(Class3), typeof(Class41), ExpectedResult = typeof(IHasId<int>))]
        [TestCase(typeof(Class41), typeof(Class3), ExpectedResult = typeof(IHasId<int>))]
        [TestCase(typeof(Class3), typeof(Class5), ExpectedResult = typeof(IHasId<int>))]
        [TestCase(typeof(Class5), typeof(Class3), ExpectedResult = typeof(IHasId<int>))]
        [TestCase(typeof(Class5), typeof(Class6), ExpectedResult = typeof(IHasIntId))]
        [TestCase(typeof(Class6), typeof(Class5), ExpectedResult = typeof(IHasIntId))]
        public Type TestGetTheBestCommonBaseType(Type type1, Type type2)
        {
            return TypeHelpers.GetTheBestCommonBaseType(type1, type2);
        }

        private class Class1
        {
        }

        private class Class2
        {
        }

        private class Class11 : Class1
        {
        }

        private class Class12 : Class1
        {
        }

        private class Class3 : IHasId<int>
        {
            public int Id { get; set; }
        }

        private class Class4 : IHasId<int>
        {
            public int Id { get; set; }
        }

        private class Class41 : Class4
        {
        }

        private class Class5 : IHasIntId
        {
            public int Id { get; set; }
        }

        private class Class6 : IHasIntId
        {
            public int Id { get; set; }
        }
    }
}
