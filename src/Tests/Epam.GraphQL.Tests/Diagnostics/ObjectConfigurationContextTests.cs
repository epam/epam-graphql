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
    public class ObjectConfigurationContextTests : IChainConfigurationContextOwner
    {
        IChainConfigurationContext IChainConfigurationContextOwner.ConfigurationContext { get; set; }

        [Test]
        public void Empty()
        {
            var context = ConfigurationContext.Create<object>();

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
            var context = ConfigurationContext.Create<object>();

            context.Chain(this, "Method");

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
            var context = ConfigurationContext.Create<object>();

            context.Chain(this, "Method")
                .Chain("Second");

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
            var context = ConfigurationContext.Create<object>();

            var method = context.Chain(this, "Method");

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
            var context = ConfigurationContext.Create<object>();

            var method = context.Chain(this, "Method");
            method.Chain("Second");

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
            var context = ConfigurationContext.Create<object>();

            var method1 = context.Chain(this, "First")
                .Chain("Next");

            var method2 = context.Chain(this, "Second")
                .Chain("Next");

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
            var context = ConfigurationContext.Create<object>();

            context.Chain(this, "Previous");

            var method = context.Chain(this, "Method");

            context.Chain(this, "Next");

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
            var context = ConfigurationContext.Create<object>();

            context.Chain(this, "First");
            context.Chain(this, "Previous");

            var method = context.Chain(this, "Method");

            context.Chain(this, "Next");
            context.Chain(this, "Last");

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
