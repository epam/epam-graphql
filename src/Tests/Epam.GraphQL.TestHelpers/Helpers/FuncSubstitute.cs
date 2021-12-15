// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using NSubstitute;

namespace Epam.GraphQL.Tests.Helpers
{
    public static class FuncSubstitute
    {
        public static Func<T, TResult> For<T, TResult>(Func<T, TResult> func)
        {
            var result = Substitute.For<Func<T, TResult>>();
            result
                .Invoke(Arg.Any<T>())
                .ReturnsForAnyArgs(callInfo => func(callInfo.ArgAt<T>(0)));

            return result;
        }

        public static Func<T1, T2, TResult> For<T1, T2, TResult>(Func<T1, T2, TResult> func)
        {
            var result = Substitute.For<Func<T1, T2, TResult>>();
            result
                .Invoke(Arg.Any<T1>(), Arg.Any<T2>())
                .ReturnsForAnyArgs(callInfo => func(callInfo.ArgAt<T1>(0), callInfo.ArgAt<T2>(1)));

            return result;
        }

        public static Func<T1, T2, T3, TResult> For<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> func)
        {
            var result = Substitute.For<Func<T1, T2, T3, TResult>>();
            result
                .Invoke(Arg.Any<T1>(), Arg.Any<T2>(), Arg.Any<T3>())
                .ReturnsForAnyArgs(callInfo => func(callInfo.ArgAt<T1>(0), callInfo.ArgAt<T2>(1), callInfo.ArgAt<T3>(2)));

            return result;
        }

        public static Func<T1, T2, T3, T4, TResult> For<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> func)
        {
            var result = Substitute.For<Func<T1, T2, T3, T4, TResult>>();
            result
                .Invoke(Arg.Any<T1>(), Arg.Any<T2>(), Arg.Any<T3>(), Arg.Any<T4>())
                .ReturnsForAnyArgs(callInfo => func(callInfo.ArgAt<T1>(0), callInfo.ArgAt<T2>(1), callInfo.ArgAt<T3>(2), callInfo.ArgAt<T4>(3)));

            return result;
        }

        public static Func<T1, T2, T3, T4, T5, TResult> For<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> func)
        {
            var result = Substitute.For<Func<T1, T2, T3, T4, T5, TResult>>();
            result
                .Invoke(Arg.Any<T1>(), Arg.Any<T2>(), Arg.Any<T3>(), Arg.Any<T4>(), Arg.Any<T5>())
                .ReturnsForAnyArgs(callInfo => func(callInfo.ArgAt<T1>(0), callInfo.ArgAt<T2>(1), callInfo.ArgAt<T3>(2), callInfo.ArgAt<T4>(3), callInfo.ArgAt<T5>(4)));

            return result;
        }

        public static Func<T1, T2, T3, T4, T5, T6, TResult> For<T1, T2, T3, T4, T5, T6, TResult>(Func<T1, T2, T3, T4, T5, T6, TResult> func)
        {
            var result = Substitute.For<Func<T1, T2, T3, T4, T5, T6, TResult>>();
            result
                .Invoke(Arg.Any<T1>(), Arg.Any<T2>(), Arg.Any<T3>(), Arg.Any<T4>(), Arg.Any<T5>(), Arg.Any<T6>())
                .ReturnsForAnyArgs(callInfo => func(callInfo.ArgAt<T1>(0), callInfo.ArgAt<T2>(1), callInfo.ArgAt<T3>(2), callInfo.ArgAt<T4>(3), callInfo.ArgAt<T5>(4), callInfo.ArgAt<T6>(5)));

            return result;
        }

        public static void HasBeenCalledTimes<T, TResult>(this Func<T, TResult> substitute, int count)
        {
            SubstituteExtensions.ReceivedWithAnyArgs(substitute, count)
                .Invoke(Arg.Any<T>());
        }

        public static void HasBeenCalledTimes<T1, T2, TResult>(this Func<T1, T2, TResult> substitute, int count)
        {
            SubstituteExtensions.ReceivedWithAnyArgs(substitute, count)
                .Invoke(Arg.Any<T1>(), Arg.Any<T2>());
        }

        public static void HasBeenCalledTimes<T1, T2, T3, TResult>(this Func<T1, T2, T3, TResult> substitute, int count)
        {
            SubstituteExtensions.ReceivedWithAnyArgs(substitute, count)
                .Invoke(Arg.Any<T1>(), Arg.Any<T2>(), Arg.Any<T3>());
        }

        public static void HasBeenCalledTimes<T1, T2, T3, T4, TResult>(this Func<T1, T2, T3, T4, TResult> substitute, int count)
        {
            SubstituteExtensions.ReceivedWithAnyArgs(substitute, count)
                .Invoke(Arg.Any<T1>(), Arg.Any<T2>(), Arg.Any<T3>(), Arg.Any<T4>());
        }

        public static void HasBeenCalledTimes<T1, T2, T3, T4, T5, TResult>(this Func<T1, T2, T3, T4, T5, TResult> substitute, int count)
        {
            SubstituteExtensions.ReceivedWithAnyArgs(substitute, count)
                .Invoke(Arg.Any<T1>(), Arg.Any<T2>(), Arg.Any<T3>(), Arg.Any<T4>(), Arg.Any<T5>());
        }

        public static void HasBeenCalledTimes<T1, T2, T3, T4, T5, T6, TResult>(this Func<T1, T2, T3, T4, T5, T6, TResult> substitute, int count)
        {
            SubstituteExtensions.ReceivedWithAnyArgs(substitute, count)
                .Invoke(Arg.Any<T1>(), Arg.Any<T2>(), Arg.Any<T3>(), Arg.Any<T4>(), Arg.Any<T5>(), Arg.Any<T6>());
        }

        public static void HasNotBeenCalled<T, TResult>(this Func<T, TResult> substitute)
        {
            SubstituteExtensions.DidNotReceive(substitute)
                .Invoke(Arg.Any<T>());
        }

#pragma warning disable NS1004 // Argument matcher used with a non-virtual member of a class.
        public static void HasBeenCalledOnce<T, TResult>(this Func<T, TResult> substitute, Expression<Predicate<T>> arg1 = null)
        {
            SubstituteExtensions.Received(substitute, 1)
                .Invoke(arg1 == null ? Arg.Any<T>() : Arg.Is(arg1));
        }

        public static void HasBeenCalledOnce<T1, T2, TResult>(this Func<T1, T2, TResult> substitute, Expression<Predicate<T1>> arg1 = null, Expression<Predicate<T2>> arg2 = null)
        {
            SubstituteExtensions.Received(substitute, 1)
                .Invoke(
                    arg1 == null ? Arg.Any<T1>() : Arg.Is(arg1),
                    arg2 == null ? Arg.Any<T2>() : Arg.Is(arg2));
        }

        public static void HasBeenCalledOnce<T1, T2, T3, TResult>(
            this Func<T1, T2, T3, TResult> substitute,
            Expression<Predicate<T1>> arg1 = null,
            Expression<Predicate<T2>> arg2 = null,
            Expression<Predicate<T3>> arg3 = null)
        {
            SubstituteExtensions.Received(substitute, 1)
                .Invoke(
                    arg1 == null ? Arg.Any<T1>() : Arg.Is(arg1),
                    arg2 == null ? Arg.Any<T2>() : Arg.Is(arg2),
                    arg3 == null ? Arg.Any<T3>() : Arg.Is(arg3));
        }

        public static void HasBeenCalledOnce<T1, T2, T3, T4, TResult>(
            this Func<T1, T2, T3, T4, TResult> substitute,
            Expression<Predicate<T1>> arg1 = null,
            Expression<Predicate<T2>> arg2 = null,
            Expression<Predicate<T3>> arg3 = null,
            Expression<Predicate<T4>> arg4 = null)
        {
            SubstituteExtensions.Received(substitute, 1)
                .Invoke(
                    arg1 == null ? Arg.Any<T1>() : Arg.Is(arg1),
                    arg2 == null ? Arg.Any<T2>() : Arg.Is(arg2),
                    arg3 == null ? Arg.Any<T3>() : Arg.Is(arg3),
                    arg4 == null ? Arg.Any<T4>() : Arg.Is(arg4));
        }

        public static void HasBeenCalledOnce<T1, T2, T3, T4, T5, TResult>(
            this Func<T1, T2, T3, T4, T5, TResult> substitute,
            Expression<Predicate<T1>> arg1 = null,
            Expression<Predicate<T2>> arg2 = null,
            Expression<Predicate<T3>> arg3 = null,
            Expression<Predicate<T4>> arg4 = null,
            Expression<Predicate<T5>> arg5 = null)
        {
            SubstituteExtensions.Received(substitute, 1)
                .Invoke(
                    arg1 == null ? Arg.Any<T1>() : Arg.Is(arg1),
                    arg2 == null ? Arg.Any<T2>() : Arg.Is(arg2),
                    arg3 == null ? Arg.Any<T3>() : Arg.Is(arg3),
                    arg4 == null ? Arg.Any<T4>() : Arg.Is(arg4),
                    arg5 == null ? Arg.Any<T5>() : Arg.Is(arg5));
        }

        public static void HasBeenCalledOnce<T1, T2, T3, T4, T5, T6, TResult>(
            this Func<T1, T2, T3, T4, T5, T6, TResult> substitute,
            Expression<Predicate<T1>> arg1 = null,
            Expression<Predicate<T2>> arg2 = null,
            Expression<Predicate<T3>> arg3 = null,
            Expression<Predicate<T4>> arg4 = null,
            Expression<Predicate<T5>> arg5 = null,
            Expression<Predicate<T6>> arg6 = null)
        {
            SubstituteExtensions.Received(substitute, 1)
                .Invoke(
                    arg1 == null ? Arg.Any<T1>() : Arg.Is(arg1),
                    arg2 == null ? Arg.Any<T2>() : Arg.Is(arg2),
                    arg3 == null ? Arg.Any<T3>() : Arg.Is(arg3),
                    arg4 == null ? Arg.Any<T4>() : Arg.Is(arg4),
                    arg5 == null ? Arg.Any<T5>() : Arg.Is(arg5),
                    arg6 == null ? Arg.Any<T6>() : Arg.Is(arg6));
        }
#pragma warning restore NS1004 // Argument matcher used with a non-virtual member of a class.
    }
}
