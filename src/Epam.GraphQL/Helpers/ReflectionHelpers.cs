// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Epam.GraphQL.Extensions;

namespace Epam.GraphQL.Helpers
{
    internal static class ReflectionHelpers
    {
        public static bool TryFindMatchingGenericBaseType(Type sourceType, Type matchingType, [NotNullWhen(true)] out Type? result)
        {
            result = sourceType;

            while (result != null)
            {
                if (result.IsGenericType && result.GetGenericTypeDefinition() == matchingType)
                {
                    return true;
                }

                result = result.BaseType;
            }

            foreach (var intf in sourceType.GetInterfaces())
            {
                if (TryFindMatchingGenericBaseType(intf, matchingType, out result))
                {
                    return true;
                }
            }

            result = null;
            return false;
        }

        public static Type FindMatchingGenericBaseType(Type sourceType, Type matchingType)
        {
            if (TryFindMatchingGenericBaseType(sourceType, matchingType, out var result))
            {
                return result;
            }

            throw new InvalidOperationException($"Cannot find the corresponding generic base type `{matchingType.HumanizedName()}` for type `{sourceType.HumanizedName()}`.");
        }

        public static Type GetTheBestCommonBaseType(IEnumerable<Type> types)
        {
            return GetTheBestCommonBaseType(types.ToArray());
        }

        public static Type GetTheBestCommonBaseType(params Type[] types)
        {
            if (types.Length == 0)
            {
                throw new ArgumentException("You must pass one parameter at least.");
            }

            if (types.Length == 1)
            {
                return types[0];
            }

            return types.Aggregate(GetTheBestCommonBaseType);
        }

        public static MethodInfo GetMethodInfo<TResult>(Func<TResult> method)
        {
            return GetMethodInfoImpl(method);
        }

        public static MethodInfo GetMethodInfo<T, TResult>(Func<T, TResult> method)
        {
            return GetMethodInfoImpl(method);
        }

        public static MethodInfo GetMethodInfo<T1, T2, TResult>(Func<T1, T2, TResult> method)
        {
            return GetMethodInfoImpl(method);
        }

        public static MethodInfo GetMethodInfo<T1, T2, T3, TResult>(Func<T1, T2, T3, TResult> method)
        {
            return GetMethodInfoImpl(method);
        }

        public static MethodInfo GetMethodInfo<T1, T2, T3, T4, TResult>(Func<T1, T2, T3, T4, TResult> method)
        {
            return GetMethodInfoImpl(method);
        }

        public static MethodInfo GetMethodInfo<T1, T2, T3, T4, T5, TResult>(Func<T1, T2, T3, T4, T5, TResult> method)
        {
            return GetMethodInfoImpl(method);
        }

        public static MethodInfo GetMethodInfo<T>(Action<T> method)
        {
            return GetMethodInfoImpl(method);
        }

        public static MethodInfo GetMethodInfo<T1, T2>(Action<T1, T2> method)
        {
            return GetMethodInfoImpl(method);
        }

        private static MethodInfo GetMethodInfoImpl(Delegate method)
        {
            var methodInfo = method.GetMethodInfo();

            if (methodInfo.IsGenericMethod)
            {
                return methodInfo.GetGenericMethodDefinition();
            }

            return methodInfo;
        }

        private static Type GetTheBestCommonBaseType(Type firstType, Type secondType)
        {
            var result = GetTheBestCommonBaseTypeAsymmetric(firstType, secondType);
            if (result != typeof(object))
            {
                return result;
            }

            return GetTheBestCommonBaseTypeAsymmetric(secondType, firstType);
        }

        private static Type GetTheBestCommonBaseTypeAsymmetric(Type firstType, Type secondType)
        {
            if (firstType.IsAssignableFrom(secondType))
            {
                return firstType;
            }

            Type result = typeof(object);

            if (firstType.BaseType != null)
            {
                result = GetTheBestCommonBaseType(firstType.BaseType, secondType);
                if (result != typeof(object))
                {
                    return result;
                }
            }

            foreach (var interfaceType in firstType.GetInterfaces())
            {
                result = GetTheBestCommonBaseType(interfaceType, secondType);
                if (result != typeof(object))
                {
                    return result;
                }
            }

            return result;
        }
    }
}
