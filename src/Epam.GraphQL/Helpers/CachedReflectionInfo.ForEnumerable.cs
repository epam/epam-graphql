// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Epam.GraphQL.Helpers
{
    internal static partial class CachedReflectionInfo
    {
        public static class ForEnumerable
        {
            private static MethodInfo? _any;
            private static MethodInfo? _cast;
            private static MethodInfo? _contains;
            private static MethodInfo? _count;
            private static MethodInfo? _distinct;
            private static MethodInfo? _select;
            private static MethodInfo? _toArray;
            private static MethodInfo? _where;

            public static MethodInfo Any(Type source)
            {
                return (_any ??= new Func<IEnumerable<object>, bool>(Enumerable.Any).GetMethodInfo().GetGenericMethodDefinition()).MakeGenericMethod(source);
            }

            public static MethodInfo Any<TSource>()
            {
                return Any(typeof(TSource));
            }

            public static MethodInfo Cast(Type result)
            {
                return (_cast ??= new Func<IEnumerable<object>, IEnumerable<object>>(Enumerable.Cast<object>).GetMethodInfo().GetGenericMethodDefinition()).MakeGenericMethod(result);
            }

            public static MethodInfo Count(Type source)
            {
                return (_count ??= new Func<IEnumerable<object>, int>(Enumerable.Count).GetMethodInfo().GetGenericMethodDefinition()).MakeGenericMethod(source);
            }

            public static MethodInfo Contains(Type source)
            {
                return (_contains ??= new Func<IEnumerable<object>, object, bool>(Enumerable.Contains).GetMethodInfo().GetGenericMethodDefinition()).MakeGenericMethod(source);
            }

            public static MethodInfo Distinct(Type source)
            {
                return (_distinct ??= new Func<IEnumerable<object>, IEnumerable<object>>(Enumerable.Distinct).GetMethodInfo().GetGenericMethodDefinition()).MakeGenericMethod(source);
            }

            public static MethodInfo Select(Type source, Type result)
            {
                return (_select ??= new Func<IEnumerable<object>, Func<object, object>, IEnumerable<object>>(Enumerable.Select).GetMethodInfo().GetGenericMethodDefinition()).MakeGenericMethod(source, result);
            }

            public static MethodInfo ToArray(Type source)
            {
                return (_toArray ??= new Func<IEnumerable<object>, object[]>(Enumerable.ToArray).GetMethodInfo().GetGenericMethodDefinition()).MakeGenericMethod(source);
            }

            public static MethodInfo Where(Type source)
            {
                return (_where ??= new Func<IEnumerable<object>, Func<object, bool>, IEnumerable<object>>(Enumerable.Where).GetMethodInfo().GetGenericMethodDefinition()).MakeGenericMethod(source);
            }

            public static MethodInfo Where<TSource>()
            {
                return Where(typeof(TSource));
            }
        }

        public static class ForEnumerable<TSource>
        {
            private static MethodInfo? _contains;

            public static MethodInfo Contains => _contains ??= new Func<IEnumerable<TSource>, TSource, bool>(Enumerable.Contains).GetMethodInfo();
        }
    }
}