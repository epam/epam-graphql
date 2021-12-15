// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

#nullable enable

namespace Epam.GraphQL.Extensions
{
    internal static class ObjectExtensions
    {
        private static readonly ConcurrentDictionary<PropertyInfo, Func<object, object?>> _func2Delegates = new();

        private static readonly ConcurrentDictionary<PropertyInfo, Action<object, object?>> _action2Delegates = new();

        private static readonly MethodInfo _func2DelegateHelperMethod = typeof(ObjectExtensions).GetMethod(
            nameof(Func2DelegateHelper),
            BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly MethodInfo _action2DelegateHelperMethod = typeof(ObjectExtensions).GetMethod(
            nameof(Action2DelegateHelper),
            BindingFlags.Static | BindingFlags.NonPublic);

        public static void CopyProperties<T>(this T obj, T value, IEnumerable<PropertyInfo> properties)
        {
            if (obj == null)
            {
                throw new ArgumentNullException(nameof(obj));
            }

            if (value == null)
            {
                throw new ArgumentNullException(nameof(value));
            }

            foreach (var propInfo in properties)
            {
                if (!propInfo.CanRead || !propInfo.CanWrite)
                {
                    throw new ArgumentException($"Cannot read or write property {propInfo.Name}.");
                }

                var val = value.GetPropertyValue(propInfo);
                obj.SetPropertyValue(propInfo, val);
            }
        }

        public static object? GetPropertyValue(this object source, PropertyInfo propertyInfo)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            // We use reflection to create a delegate to access the property
            // Then cache the delegate
            // This is over 10x faster that just using reflection to get the property value
            var sourceType = source.GetType();
            var func = _func2Delegates.GetOrAdd(propertyInfo, CreateFunc2Delegate);

            return func(source);
        }

        public static T? GetPropertyValue<T>(this object source, PropertyInfo propertyInfo)
        {
            return (T?)GetPropertyValue(source, propertyInfo);
        }

        public static void SetPropertyValue(this object source, PropertyInfo propertyInfo, object? value)
        {
            if (source == null)
            {
                throw new ArgumentNullException(nameof(source));
            }

            if (propertyInfo == null)
            {
                throw new ArgumentNullException(nameof(propertyInfo));
            }

            // We use reflection to create a delegate to access the property
            // Then cache the delegate
            // This is over 10x faster that just using reflection to get the property value
            var sourceType = source.GetType();
            var action = _action2Delegates.GetOrAdd(propertyInfo, CreateAction2Delegate);

            action(source, value);
        }

        private static Func<object, object?> CreateFunc2Delegate(PropertyInfo propertyInfo)
        {
            var methodInfo = propertyInfo.GetMethod;

            // Get the property with reflection
            // Use reflection to call the method to generate our delegate
            MethodInfo constructedHelper = _func2DelegateHelperMethod.MakeGenericMethod(
                methodInfo.DeclaringType, methodInfo.ReturnType);

            return constructedHelper.Invoke<Func<object, object?>>(null, methodInfo);
        }

        private static Action<object, object?> CreateAction2Delegate(PropertyInfo propertyInfo)
        {
            var methodInfo = propertyInfo.SetMethod;

            // Set the property with reflection
            // Use reflection to call the method to generate our delegate
            MethodInfo constructedHelper = _action2DelegateHelperMethod.MakeGenericMethod(
                methodInfo.DeclaringType, methodInfo.GetParameters().First().ParameterType);

            return constructedHelper.Invoke<Action<object?, object?>>(null, methodInfo);
        }

        private static Func<object?, object?> Func2DelegateHelper<TTarget, TReturn>(MethodInfo methodInfo)
        {
            // Convert the slow MethodInfo into a fast, strongly typed, open delegate
            Func<TTarget, TReturn> func = (Func<TTarget, TReturn>)Delegate.CreateDelegate(typeof(Func<TTarget, TReturn>), methodInfo);

            // Now create a more weakly typed delegate which will call the strongly typed one
            return (object? target) => func((TTarget)target!);
        }

        private static Action<object?, object?> Action2DelegateHelper<TTarget, TReturn>(MethodInfo methodInfo)
        {
            // Convert the slow MethodInfo into a fast, strongly typed, open delegate
            Action<TTarget, TReturn> action = (Action<TTarget, TReturn>)Delegate.CreateDelegate(typeof(Action<TTarget, TReturn>), methodInfo);

            // Now create a more weakly typed delegate which will call the strongly typed one
            return (object? target, object? value) =>
            {
                action((TTarget)target!, (TReturn)value!);
            };
        }
    }
}
