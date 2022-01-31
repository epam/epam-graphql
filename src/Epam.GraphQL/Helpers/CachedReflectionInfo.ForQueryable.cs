// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Epam.GraphQL.Helpers
{
    internal static partial class CachedReflectionInfo
    {
        public static class ForQueryable
        {
            private static MethodInfo? _groupBy;
            private static MethodInfo? _orderBy;
            private static MethodInfo? _orderByDescending;
            private static MethodInfo? _select;
            private static MethodInfo? _thenBy;
            private static MethodInfo? _thenByDescending;

            private static MethodInfo? _orderByWithComparer;
            private static MethodInfo? _orderByDescendingWithComparer;
            private static MethodInfo? _thenByWithComparer;
            private static MethodInfo? _thenByDescendingWithComparer;

            public static MethodInfo GenericOrderBy => _orderBy ??= ReflectionHelpers.GetMethodInfo<IQueryable<object>, Expression<Func<object, object>>, IOrderedQueryable<object>>(Queryable.OrderBy);

            public static MethodInfo GenericOrderByDescending => _orderByDescending ??= ReflectionHelpers.GetMethodInfo<IQueryable<object>, Expression<Func<object, object>>, IOrderedQueryable<object>>(Queryable.OrderByDescending);

            public static MethodInfo GenericThenBy => _thenBy ??= ReflectionHelpers.GetMethodInfo<IOrderedQueryable<object>, Expression<Func<object, object>>, IOrderedQueryable<object>>(Queryable.ThenBy);

            public static MethodInfo GenericThenByDescending => _thenByDescending ??= ReflectionHelpers.GetMethodInfo<IOrderedQueryable<object>, Expression<Func<object, object>>, IOrderedQueryable<object>>(Queryable.ThenByDescending);

            public static MethodInfo GenericOrderByWithComparer => _orderByWithComparer ??= ReflectionHelpers.GetMethodInfo<IQueryable<object>, Expression<Func<object, object>>, IComparer<object>, IOrderedQueryable<object>>(Queryable.OrderBy);

            public static MethodInfo GenericOrderByDescendingWithComparer => _orderByDescendingWithComparer ??= ReflectionHelpers.GetMethodInfo<IQueryable<object>, Expression<Func<object, object>>, IComparer<object>, IOrderedQueryable<object>>(Queryable.OrderByDescending);

            public static MethodInfo GenericThenByWithComparer => _thenByWithComparer ??= ReflectionHelpers.GetMethodInfo<IOrderedQueryable<object>, Expression<Func<object, object>>, IComparer<object>, IOrderedQueryable<object>>(Queryable.ThenBy);

            public static MethodInfo GenericThenByDescendingWithComparer => _thenByDescendingWithComparer ??= ReflectionHelpers.GetMethodInfo<IOrderedQueryable<object>, Expression<Func<object, object>>, IComparer<object>, IOrderedQueryable<object>>(Queryable.ThenByDescending);

            public static MethodInfo GroupBy(Type source, Type key)
            {
                return (_groupBy ??= ReflectionHelpers.GetMethodInfo<IQueryable<object>, Expression<Func<object, object>>, IQueryable<IGrouping<object, object>>>(Queryable.GroupBy))
                    .MakeGenericMethod(source, key);
            }

            public static MethodInfo OrderBy(Type source, Type key)
            {
                return GenericOrderBy.MakeGenericMethod(source, key);
            }

            public static MethodInfo OrderByDescending(Type source, Type key)
            {
                return GenericOrderByDescending.MakeGenericMethod(source, key);
            }

            public static MethodInfo Select(Type source, Type result)
            {
                return (_select ??= ReflectionHelpers.GetMethodInfo<IQueryable<object>, Expression<Func<object, object>>, IQueryable<object>>(Queryable.Select))
                    .MakeGenericMethod(source, result);
            }

            public static MethodInfo ThenBy(Type source, Type key)
            {
                return GenericThenBy.MakeGenericMethod(source, key);
            }

            public static MethodInfo ThenByDescending(Type source, Type key)
            {
                return GenericThenByDescending.MakeGenericMethod(source, key);
            }
        }
    }
}
