// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Tests.Helpers;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Diagnostics
{
    [TestFixture]
    public class MethodCallConfigurationContextTests : IChainConfigurationContextOwner
    {
        IChainConfigurationContext IChainConfigurationContextOwner.ConfigurationContext { get; set; }

        [Test]
        public void OneOperationNoArgs()
        {
            var context = ConfigurationContext.Create().Chain(this, "Name");
            Assert.AreEqual("Name()", context.ToString());
        }

        [Test]
        public void OneOperationOneArg()
        {
            var context = ConfigurationContext.Create().Chain(this, "Name")
                .Argument("test");

            Assert.AreEqual("Name(\"test\")", context.ToString());
        }

        [Test]
        public void OneOperationTwoArgs()
        {
            var context = ConfigurationContext.Create().Chain(this, "Name")
                .Argument("test")
                .Argument("test");

            Assert.AreEqual(
                TestHelpers.ConcatLines(
                    "Name(",
                    "    \"test\",",
                    "    \"test\")"),
                context.ToString());
        }

        [Test]
        public void TwoOperationsOneArgEach()
        {
            var context = ConfigurationContext.Create().Chain(this, "First")
                    .Argument("test")
                .Chain("Second")
                    .Argument("test");

            Assert.AreEqual(
                TestHelpers.ConcatLines(
                    "First(\"test\")",
                    "    .Second(\"test\")"),
                context.ToString());
        }

        [Test]
        public void TwoOperationsOneArgAndTwoArgs()
        {
            var context = ConfigurationContext.Create().Chain(this, "First")
                    .Argument("test")
                .Chain("Second")
                    .Argument("test")
                    .Argument("test");

            Assert.AreEqual(
                TestHelpers.ConcatLines(
                    "    First(\"test\")",
                    "        .Second(",
                    "            \"test\",",
                    "            \"test\")"),
                context.ToString(1));
        }

        [Test]
        public void TwoOperationsTwoArgsEach()
        {
            var context = ConfigurationContext.Create().Chain(this, "First")
                    .Argument("test1")
                    .Argument("test2")
                .Chain("Second")
                    .Argument("first")
                    .Argument("second");

            Assert.AreEqual(
                TestHelpers.ConcatLines(
                    "First(",
                    "    \"test1\",",
                    "    \"test2\")",
                    "    .Second(",
                    "        \"first\",",
                    "        \"second\")"),
                context.ToString());
        }

        [Test]
        public void ThreeOperationsOneArgEach()
        {
            var context = ConfigurationContext.Create().Chain(this, "First")
                    .Argument("test")
                .Chain("Second")
                    .Argument("test")
                .Chain("Third")
                    .Argument("test");

            Assert.AreEqual(
                TestHelpers.ConcatLines(
                    "First(\"test\")",
                    "    .Second(\"test\")",
                    "    .Third(\"test\")"),
                context.ToString());
        }

        [Test]
        public void ThreeOperationsTwoArgsEach()
        {
            var context = ConfigurationContext.Create().Chain(this, "First")
                    .Argument("test1")
                    .Argument("test2")
                .Chain("Second")
                    .Argument("first")
                    .Argument("second")
                .Chain("Third")
                    .Argument("first")
                    .Argument("second");

            Assert.AreEqual(
                TestHelpers.ConcatLines(
                    "First(",
                    "    \"test1\",",
                    "    \"test2\")",
                    "    .Second(",
                    "        \"first\",",
                    "        \"second\")",
                    "    .Third(",
                    "        \"first\",",
                    "        \"second\")"),
                context.ToString());
        }

        [Test]
        public void IndentThreeOperationsTwoArgsEach()
        {
            var context = ConfigurationContext.Create().Chain(this, "First")
                    .Argument("test1")
                    .Argument("test2")
                .Chain("Second")
                    .Argument("first")
                    .Argument("second")
                .Chain("Third")
                    .Argument("first")
                    .Argument("second");

            Assert.AreEqual(
                TestHelpers.ConcatLines(
                    "        First(",
                    "            \"test1\",",
                    "            \"test2\")",
                    "            .Second(",
                    "                \"first\",",
                    "                \"second\")",
                    "            .Third(",
                    "                \"first\",",
                    "                \"second\")"),
                context.ToString(2));
        }
    }
}
