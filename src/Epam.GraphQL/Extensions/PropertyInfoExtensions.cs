// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using System.Reflection;
using Epam.GraphQL.Helpers;

namespace Epam.GraphQL.Extensions
{
    internal static class PropertyInfoExtensions
    {
        public static LambdaExpression MakePropertyLambdaExpression(this PropertyInfo propertyInfo, Type? type = null)
        {
            var entityType = propertyInfo.ReflectedType;
            var propertyType = propertyInfo.PropertyType;

            Guards.ThrowInvalidOperationIf(type != null && !type.IsAssignableFrom(propertyType), $"Cannot cast {propertyType} to {type}");

            propertyType = type ?? propertyType;

            var parameter = Expression.Parameter(entityType, "entity");
            var property = Expression.Property(parameter, propertyInfo);
            var funcType = typeof(Func<,>).MakeGenericType(entityType, propertyType);
            return Expression.Lambda(funcType, property, parameter);
        }
    }
}
