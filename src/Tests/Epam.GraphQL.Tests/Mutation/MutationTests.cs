// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Collections.Generic;
using Epam.GraphQL.Loaders;
using NUnit.Framework;

namespace Epam.GraphQL.Tests.Mutation
{
    [TestFixture]
    public partial class MutationTests : BaseMutationTests
    {
        public class PersonFilter : Input
        {
            public List<int> Ids { get; set; }
        }
    }
}
