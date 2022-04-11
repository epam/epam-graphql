// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Text;
using Epam.GraphQL.Diagnostics;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Diagnostics
{
    [TestFixture]
    public class MethodCallConfigurationContextTests
    {
        [Test]
        public void OneOperationNoArgs()
        {
            var context = new ObjectConfigurationContext(null).Operation("Name");
            Assert.AreEqual("Name()", context.ToString());
        }

        [Test]
        public void OneOperationOneArg()
        {
            var context = new ObjectConfigurationContext(null).Operation("Name")
                .Argument("test");

            Assert.AreEqual("Name(\"test\")", context.ToString());
        }

        [Test]
        public void OneOperationTwoArgs()
        {
            var context = new ObjectConfigurationContext(null).Operation("Name")
                .Argument("test")
                .Argument("test");

            Assert.AreEqual(
                ConcatLines(
                    "Name(",
                    "    \"test\",",
                    "    \"test\")"),
                context.ToString());
        }

        [Test]
        public void TwoOperationsOneArgEach()
        {
            var context = new ObjectConfigurationContext(null).Operation("First")
                    .Argument("test")
                .NextOperation("Second")
                    .Argument("test");

            Assert.AreEqual(
                ConcatLines(
                    "First(\"test\")",
                    "    .Second(\"test\")"),
                context.ToString());
        }

        [Test]
        public void TwoOperationsOneArgAndTwoArgs()
        {
            var context = new ObjectConfigurationContext(null).Operation("First")
                    .Argument("test")
                .NextOperation("Second")
                    .Argument("test")
                    .Argument("test");

            Assert.AreEqual(
                ConcatLines(
                    "    First(\"test\")",
                    "        .Second(",
                    "            \"test\",",
                    "            \"test\")"),
                context.ToString(1));
        }

        [Test]
        public void TwoOperationsTwoArgsEach()
        {
            var context = new ObjectConfigurationContext(null).Operation("First")
                    .Argument("test1")
                    .Argument("test2")
                .NextOperation("Second")
                    .Argument("first")
                    .Argument("second");

            Assert.AreEqual(
                ConcatLines(
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
            var context = new ObjectConfigurationContext(null).Operation("First")
                    .Argument("test")
                .NextOperation("Second")
                    .Argument("test")
                .NextOperation("Third")
                    .Argument("test");

            Assert.AreEqual(
                ConcatLines(
                    "First(\"test\")",
                    "    .Second(\"test\")",
                    "    .Third(\"test\")"),
                context.ToString());
        }

        [Test]
        public void ThreeOperationsTwoArgsEach()
        {
            var context = new ObjectConfigurationContext(null).Operation("First")
                    .Argument("test1")
                    .Argument("test2")
                .NextOperation("Second")
                    .Argument("first")
                    .Argument("second")
                .NextOperation("Third")
                    .Argument("first")
                    .Argument("second");

            Assert.AreEqual(
                ConcatLines(
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
            var context = new ObjectConfigurationContext(null).Operation("First")
                    .Argument("test1")
                    .Argument("test2")
                .NextOperation("Second")
                    .Argument("first")
                    .Argument("second")
                .NextOperation("Third")
                    .Argument("first")
                    .Argument("second");

            Assert.AreEqual(
                ConcatLines(
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

        private static string ConcatLines(params string[] lines)
        {
            var sb = new StringBuilder();

            for (int i = 0; i < lines.Length; i++)
            {
                if (i > 0)
                {
                    sb.AppendLine();
                }

                sb.Append(lines[i]);
            }

            return sb.ToString();
        }
    }
}
