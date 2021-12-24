// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Diagnostics;
using System.Reflection;
using System.Runtime.ExceptionServices;

#nullable enable

namespace Epam.GraphQL.Extensions
{
    internal static class MethodBaseExtensions
    {
        [DebuggerStepThrough]
        public static TResult Invoke<TResult>(this MethodBase methodBase, object? obj, params object?[] parameters) =>
            (TResult)methodBase.Invoke(obj, parameters);

        [DebuggerStepThrough]
        public static object InvokeAndHoistBaseException(this MethodBase methodBase, object? obj, params object?[] parameters)
        {
            try
            {
                return methodBase.Invoke(obj, parameters);
            }
            catch (TargetInvocationException e)
            {
                ExceptionDispatchInfo.Capture(e.GetBaseException()).Throw();
                throw;
            }
        }

        [DebuggerStepThrough]
        public static TResult InvokeAndHoistBaseException<TResult>(this MethodBase methodBase, object? obj, params object?[] parameters)
        {
            try
            {
                return (TResult)methodBase.Invoke(obj, parameters);
            }
            catch (TargetInvocationException e)
            {
                ExceptionDispatchInfo.Capture(e.GetBaseException()).Throw();
                throw;
            }
        }
    }
}
