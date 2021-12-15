// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

#nullable enable

namespace Epam.GraphQL.Helpers
{
    internal static partial class CachedReflectionInfo
    {
        public static class ForQueryable
        {
            private static MethodInfo? _cast;
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

            public static MethodInfo GenericOrderBy => _orderBy ??= new Func<IQueryable<object>, Expression<Func<object, object>>, IOrderedQueryable<object>>(Queryable.OrderBy).GetMethodInfo().GetGenericMethodDefinition();

            public static MethodInfo GenericOrderByDescending => _orderByDescending ??= new Func<IQueryable<object>, Expression<Func<object, object>>, IOrderedQueryable<object>>(Queryable.OrderByDescending).GetMethodInfo().GetGenericMethodDefinition();

            public static MethodInfo GenericThenBy => _thenBy ??= new Func<IOrderedQueryable<object>, Expression<Func<object, object>>, IOrderedQueryable<object>>(Queryable.ThenBy).GetMethodInfo()!.GetGenericMethodDefinition();

            public static MethodInfo GenericThenByDescending => _thenByDescending ??= new Func<IOrderedQueryable<object>, Expression<Func<object, object>>, IOrderedQueryable<object>>(Queryable.ThenByDescending).GetMethodInfo().GetGenericMethodDefinition();

            public static MethodInfo GenericOrderByWithComparer => _orderByWithComparer ??= new Func<IQueryable<object>, Expression<Func<object, object>>, IComparer<object>, IOrderedQueryable<object>>(Queryable.OrderBy).GetMethodInfo().GetGenericMethodDefinition();

            public static MethodInfo GenericOrderByDescendingWithComparer => _orderByDescendingWithComparer ??= new Func<IQueryable<object>, Expression<Func<object, object>>, IComparer<object>, IOrderedQueryable<object>>(Queryable.OrderByDescending).GetMethodInfo().GetGenericMethodDefinition();

            public static MethodInfo GenericThenByWithComparer => _thenByWithComparer ??= new Func<IOrderedQueryable<object>, Expression<Func<object, object>>, IComparer<object>, IOrderedQueryable<object>>(Queryable.ThenBy).GetMethodInfo().GetGenericMethodDefinition();

            public static MethodInfo GenericThenByDescendingWithComparer => _thenByDescendingWithComparer ??= new Func<IOrderedQueryable<object>, Expression<Func<object, object>>, IComparer<object>, IOrderedQueryable<object>>(Queryable.ThenByDescending).GetMethodInfo().GetGenericMethodDefinition();

            public static MethodInfo Cast(Type result)
            {
                return (_cast ??= new Func<IQueryable, IQueryable<object>>(Queryable.Cast<object>).GetMethodInfo().GetGenericMethodDefinition()).MakeGenericMethod(result);
            }

            public static MethodInfo GroupBy(Type source, Type key)
            {
                return (_groupBy ??= new Func<IQueryable<object>, Expression<Func<object, object>>, IQueryable<IGrouping<object, object>>>(Queryable.GroupBy).GetMethodInfo().GetGenericMethodDefinition()).MakeGenericMethod(source, key);
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
                return (_select ??= new Func<IQueryable<object>, Expression<Func<object, object>>, IQueryable<object>>(Queryable.Select).GetMethodInfo().GetGenericMethodDefinition()).MakeGenericMethod(source, result);
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
