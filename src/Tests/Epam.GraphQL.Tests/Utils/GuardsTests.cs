// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Helpers;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Utils
{
    [TestFixture]
    public class GuardsTests
    {
        [Test]
        public void ThrowIfArgIsNull()
        {
            string arg = null;
            Assert.Throws<ArgumentNullException>(
                () => Guards.ThrowIfNull(arg, nameof(arg)),
                "Argument is null");
        }

        [Test]
        public void ThrowIfArgIsEmpty()
        {
            Assert.Throws<ArgumentException>(
                () => Guards.ThrowIfNullOrEmpty(string.Empty, "arg"),
                "String argument is empty");
        }
    }
}
