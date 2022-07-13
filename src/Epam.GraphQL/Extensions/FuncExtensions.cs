// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Helpers;

namespace Epam.GraphQL.Extensions
{
    internal static class FuncExtensions
    {
        public static Func<T, TResult> Safe<T, TResult>(this Func<T, TResult>? func)
        {
            return func ?? FuncConstants<T, TResult>.DefaultResultFunc;
        }

        public static Func<T1, Func<T2, TResult>> Curry<T1, T2, TResult>(this Func<T1, T2, TResult> function)
        {
            return a => b => function(a, b);
        }
    }
}
