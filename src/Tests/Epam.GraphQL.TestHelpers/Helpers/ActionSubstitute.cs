// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using NSubstitute;

#nullable enable

namespace Epam.GraphQL.Tests.Helpers
{
    public static class ActionSubstitute
    {
        public static Action<T1, T2> For<T1, T2>()
        {
            var result = Substitute.For<Action<T1, T2>>();

            return result;
        }

        public static Action<T1, T2> For<T1, T2>(Action<T1, T2> action)
        {
            var result = Substitute.For<Action<T1, T2>>();

            result.When(act => act.Invoke(Arg.Any<T1>(), Arg.Any<T2>()))
                .Do(x => action(x.ArgAt<T1>(0), x.ArgAt<T2>(1)));

            return result;
        }

        public static void HasBeenCalledTimes<T1, T2>(this Action<T1, T2> substitute, int count)
        {
            SubstituteExtensions.ReceivedWithAnyArgs(substitute, count)
                .Invoke(Arg.Any<T1>(), Arg.Any<T2>());
        }

        public static void HasBeenCalledOnce<T1, T2>(this Action<T1, T2> substitute) => HasBeenCalledTimes(substitute, 1);

#pragma warning disable NS1004 // Argument matcher used with a non-virtual member of a class.
        public static void HasBeenCalledOnce<T1, T2>(this Action<T1, T2> substitute, Expression<Predicate<T1>>? arg1 = null, Expression<Predicate<T2>>? arg2 = null)
        {
            SubstituteExtensions.Received(substitute, 1)
                .Invoke(
                    arg1 == null ? Arg.Any<T1>() : Arg.Is(arg1),
                    arg2 == null ? Arg.Any<T2>() : Arg.Is(arg2));
        }
    }
#pragma warning restore NS1004 // Argument matcher used with a non-virtual member of a class.
}
