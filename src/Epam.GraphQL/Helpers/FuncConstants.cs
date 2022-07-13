// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;

namespace Epam.GraphQL.Helpers
{
    internal static class FuncConstants<T>
    {
        public static Func<T, bool> TruePredicate { get; } = arg => true;

        public static Func<T, bool> FalsePredicate { get; } = arg => false;

        public static Func<T, bool> IsNull { get; } = arg => arg == null;

        public static Func<T, T> Identity { get; } = arg => arg;

        public static Expression<Func<T, T>> IdentityExpression { get; } = arg => arg;

        public static Func<T, object?> WeakIdentity { get; } = arg => arg;

        public static Expression<Func<T, bool>> TrueExpression { get; } = f => true;

        public static Expression<Func<T, bool>> FalseExpression { get; } = f => false;
    }

    internal static class FuncConstants<T1, T2>
    {
        public static Func<T1, T2> DefaultResultFunc { get; } = arg => default!;

        public static Func<T1, T2, bool> TruePredicate { get; } = (arg1, arg2) => true;

        public static Func<T1, T2, bool> FalsePredicate { get; } = (arg1, arg2) => false;

        public static Func<T1, T2> Cast { get; } = arg => (T2)(object)arg!;
    }
}
