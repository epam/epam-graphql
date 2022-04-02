// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Diagnostics;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Diagnostics
{
    [TestFixture]
    public class DiagnosticsExtensionsTests
    {
        [Test]
        public void InlineLambda()
        {
            Assert.AreEqual("_ => ...", new Action<int>(_ => { }).Method.Print());
        }

        [Test]
        public void InlineLambdaTwoArgs()
        {
            Assert.AreEqual("(_, arg1) => ...", new Action<int, string>((_, arg1) => { }).Method.Print());
        }

        [Test]
        public void LocalMethod()
        {
            Assert.AreEqual("Method", new Action<int>(Method).Method.Print());

#pragma warning disable IDE0062 // Make local function 'static'
            void Method(int value)
#pragma warning restore IDE0062 // Make local function 'static'
            {
            }
        }

        [Test]
        public void LocalStaticMethod()
        {
            Assert.AreEqual("Method", new Action<int>(Method).Method.Print());

            static void Method(int value)
            {
            }
        }
    }
}
