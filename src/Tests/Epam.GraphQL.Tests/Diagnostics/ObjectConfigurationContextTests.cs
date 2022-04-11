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
    public class ObjectConfigurationContextTests
    {
        [Test]
        public void Empty()
        {
            var context = new ObjectConfigurationContext<object>();

            Assert.AreEqual(
                ConcatLines(
                    "    public override void OnConfigure()",
                    "    {",
                    "    }"),
                context.ToString(1));
        }

        [Test]
        public void OneMethod()
        {
            var context = new ObjectConfigurationContext<object>();

            context.Operation("Method");

            Assert.AreEqual(
                ConcatLines(
                    "    public override void OnConfigure()",
                    "    {",
                    "        Method();",
                    "    }"),
                context.ToString(1));
        }

        [Test]
        public void MethodChain()
        {
            var context = new ObjectConfigurationContext<object>();

            context.Operation("Method")
                .NextOperation("Second");

            Assert.AreEqual(
                ConcatLines(
                    "    public override void OnConfigure()",
                    "    {",
                    "        Method()",
                    "            .Second();",
                    "    }"),
                context.ToString(1));
        }

        [Test]
        public void MarkMethod()
        {
            var context = new ObjectConfigurationContext<object>();

            var method = context.Operation("Method");

            Assert.AreEqual(
                ConcatLines(
                    "    public override void OnConfigure()",
                    "    {",
                    "        Method(); // <-----",
                    "    }"),
                context.ToString(1, method));
        }

        [Test]
        public void MarkFirstMethodInChain()
        {
            var context = new ObjectConfigurationContext<object>();

            var method = context.Operation("Method");
            method.NextOperation("Second");

            Assert.AreEqual(
                ConcatLines(
                    "    public override void OnConfigure()",
                    "    {",
                    "        Method() // <-----",
                    "            .Second();",
                    "    }"),
                context.ToString(1, method));
        }

        [Test]
        public void MarkTwoMethodChains()
        {
            var context = new ObjectConfigurationContext<object>();

            var method1 = context.Operation("First")
                .NextOperation("Next");

            var method2 = context.Operation("Second")
                .NextOperation("Next");

            Assert.AreEqual(
                ConcatLines(
                    "    public override void OnConfigure()",
                    "    {",
                    "        First()",
                    "            .Next(); // <-----",
                    string.Empty,
                    "        Second()",
                    "            .Next(); // <-----",
                    "    }"),
                context.ToString(1, method1, method2));
        }

        [Test]
        public void ShouldPrintPreviousAndNextAroundMarkedMethod()
        {
            var context = new ObjectConfigurationContext<object>();

            context.Operation("Previous");

            var method = context.Operation("Method");

            context.Operation("Next");

            Assert.AreEqual(
                ConcatLines(
                    "    public override void OnConfigure()",
                    "    {",
                    "        Previous();",
                    string.Empty,
                    "        Method(); // <-----",
                    string.Empty,
                    "        Next();",
                    "    }"),
                context.ToString(1, method));
        }

        [Test]
        public void ShouldPrintPreviousAndNextAroundMarkedMethodButNotOthers()
        {
            var context = new ObjectConfigurationContext<object>();

            context.Operation("First");
            context.Operation("Previous");

            var method = context.Operation("Method");

            context.Operation("Next");
            context.Operation("Last");

            Assert.AreEqual(
                ConcatLines(
                    "    public override void OnConfigure()",
                    "    {",
                    "        // ...",
                    string.Empty,
                    "        Previous();",
                    string.Empty,
                    "        Method(); // <-----",
                    string.Empty,
                    "        Next();",
                    string.Empty,
                    "        // ...",
                    "    }"),
                context.ToString(1, method));
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
