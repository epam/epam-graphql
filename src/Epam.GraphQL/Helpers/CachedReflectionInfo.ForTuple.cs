// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Reflection;

namespace Epam.GraphQL.Helpers
{
    internal partial class CachedReflectionInfo
    {
        public static class ForTuple<T>
        {
            private static PropertyInfo? _item1;

            public static PropertyInfo Item1 => _item1 ??= typeof(Tuple<T>).GetProperty(nameof(Tuple<T>.Item1));
        }
    }
}
