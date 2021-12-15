// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Collections.Generic;
using System.Runtime.CompilerServices;

#nullable enable

namespace Epam.GraphQL.Helpers
{
    internal class ReferenceEqualityComparer : IEqualityComparer<object?>
    {
        public static ReferenceEqualityComparer Instance { get; } = new ReferenceEqualityComparer();

        public new bool Equals(object? x, object? y) => ReferenceEquals(x, y);

        public int GetHashCode(object? obj) => RuntimeHelpers.GetHashCode(obj);
    }
}
