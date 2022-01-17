// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;

namespace Epam.GraphQL.Helpers
{
    internal static class TypeHelpers
    {
        public static Type? FindMatchingGenericBaseType(Type sourceType, Type matchingType)
        {
            var baseType = sourceType;

            while (baseType != null)
            {
                if (baseType.IsGenericType && baseType.GetGenericTypeDefinition() == matchingType)
                {
                    return baseType;
                }

                baseType = baseType.BaseType;
            }

            return null;
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
