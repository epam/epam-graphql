// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Reflection;

namespace Epam.GraphQL.Tests.Helpers
{
    public static class ObjectExtensions
    {
        public static T Clone<T>(this T obj) => CloneUtil<T>.Clone(obj);

        private static class CloneUtil<T>
        {
            private static readonly MethodInfo _cloneMethod = typeof(T).GetMethod("MemberwiseClone", BindingFlags.Instance | BindingFlags.NonPublic);
            private static readonly Func<T, object> clone = (Func<T, object>)_cloneMethod.CreateDelegate(typeof(Func<T, object>));

            public static T Clone(T obj) => (T)clone(obj);
        }
    }
}
