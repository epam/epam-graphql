// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.ExceptionServices;
using System.Security;
using Epam.GraphQL.Helpers;

namespace Epam.GraphQL.Extensions
{
    internal static class TypeExtensions
    {
        private const BindingFlags Flags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase;

        private static readonly ConcurrentDictionary<(Type, string), Func<object, object?>> _propertyDelegates
            = new();

        private static readonly MethodInfo _delegateHelperMethod = typeof(TypeExtensions).GetMethod(
            nameof(DelegateHelper),
            BindingFlags.Static | BindingFlags.NonPublic);

        private static readonly ReadOnlyDictionary<Type, string> _typeNameMap = new(
            new Dictionary<Type, string>
            {
                [typeof(object)] = "object",
                [typeof(byte)] = "byte",
                [typeof(sbyte)] = "sbyte",
                [typeof(short)] = "short",
                [typeof(ushort)] = "ushort",
                [typeof(int)] = "int",
                [typeof(uint)] = "uint",
                [typeof(long)] = "long",
                [typeof(ulong)] = "ulong",
                [typeof(double)] = "double",
                [typeof(float)] = "float",
                [typeof(decimal)] = "decimal",
                [typeof(bool)] = "bool",
                [typeof(string)] = "string",
                [typeof(char)] = "char",
            });

        private static readonly ConcurrentDictionary<(Type Type, bool WithOriginal, ICollection<string> Properties), Type> _instantiatedProxyGenericTypeCache = new(
            new ValueTupleEqualityComparer<Type, bool, ICollection<string>>(EqualityComparer<Type>.Default, EqualityComparer<bool>.Default, new CollectionEqualityComparer<string>()));

        private static readonly ConcurrentDictionary<(Type ProxyGenericType, Type EntityType, bool WithOriginal, ICollection<string> Properties), Type> _instantiatedProxyTypeCache = new(
            new ValueTupleEqualityComparer<Type, Type, bool, ICollection<string>>(EqualityComparer<Type>.Default, EqualityComparer<Type>.Default, EqualityComparer<bool>.Default, new CollectionEqualityComparer<string>()));

        private static readonly object _proxyTypeCountLock = new();
        private static long _proxyTypeCount;

        public static bool IsEnumerableOfT(this Type type)
        {
            return (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
                || type.GetInterfaces().Any(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        public static Type GetEnumerableElementType(this Type type)
        {
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
            {
                return type.GenericTypeArguments[0];
            }

            return type.GetInterfaces().First(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>)).GenericTypeArguments[0];
        }

        public static Func<object, object?> GetPropertyDelegate(this Type type, string propertyName)
        {
            // We use reflection to create a delegate to access the property
            // Then cache the delegate
            // This is over 10x faster that just using reflection to get the property value
            return _propertyDelegates.GetOrAdd((type, propertyName), t => CreatePropertyDelegate(t.Item1, t.Item2));
        }

        public static string GraphQLTypeName(this Type type, bool isInput)
        {
            var prefix = isInput ? "Input" : string.Empty;

            if (_typeNameMap.TryGetValue(type, out var result))
            {
                return $"{prefix}{result.CapitalizeFirstLetter()}";
            }

            if (type != typeof(string) && type.IsEnumerableType())
            {
                var elementTypeName = type.GetEnumerableElementType().GraphQLTypeName(false);
                return $"{prefix}ListOf{elementTypeName}";
            }

            var typeName = type.IsAnonymousType()
                ? "AnonymousType"
                : SanitizeTypeName(type.GetTypeName());

            if (type.IsGenericType)
            {
                return $"{prefix}{typeName}Of{string.Join("And", type.GetGenericArguments().Select(t => t.GraphQLTypeName(false)))}";
            }

            return $"{prefix}{typeName}";
        }

        public static string HumanizedName(this Type type)
        {
            if (_typeNameMap.TryGetValue(type, out var result))
            {
                return result;
            }

            if (type.IsNullable())
            {
                return $"{type.UnwrapIfNullable().HumanizedName()}?";
            }

            var typeName = type.GetTypeName();

            if (type.IsGenericTypeDefinition)
            {
                if (type.GenericTypeArguments.Length <= 1)
                {
                    return $"{typeName}<>";
                }

                return $"{typeName}<{string.Join(string.Empty, Enumerable.Repeat(",", type.GenericTypeArguments.Length - 1))}>";
            }

            if (type.IsGenericType)
            {
                return $"{typeName}<{string.Join(", ", type.GetGenericArguments().Select(t => t.HumanizedName()))}>";
            }

            return type.Name;
        }

        public static Type MakeInstantiatedProxyGenericType(this Type proxyGenericType, IEnumerable<string> propertyNames, bool withOriginal)
        {
            if (propertyNames.Any(name => string.IsNullOrEmpty(name)))
            {
                throw new ArgumentException("Property names must not contain null or empty strings.", nameof(propertyNames));
            }

            var invalidPropNames = propertyNames.Where(name => proxyGenericType.GetProperty(name) == null);

            if (invalidPropNames.Any())
            {
                throw new ArgumentException($"Cannot find a properties ({string.Join(", ", invalidPropNames)})  on type {proxyGenericType}.", nameof(propertyNames));
            }

            if (proxyGenericType.BaseType.Name != typeof(Proxy<>).Name || proxyGenericType.BaseType.Assembly != typeof(Proxy<>).Assembly)
            {
                throw new ArgumentException("Type must be inherited from Proxy<>", nameof(proxyGenericType));
            }

            return _instantiatedProxyGenericTypeCache.GetOrAdd((proxyGenericType, withOriginal, propertyNames.ToList()), key =>
            {
                var (proxyGenericType, withOriginal, propertyNames) = key;

                var tb = ILUtils.DefineType(GenerateInstantiatedProxyGenericTypeName(), proxyGenericType);

                if (withOriginal)
                {
                    var baseProperty = tb.GetBaseType().GetProperty("$original");
                    var backingField = tb.DefineBackingField("$original", baseProperty.PropertyType);
                    tb.OverrideProperty(backingField, "$original").MakeDebuggerBrowsableCollapsed();

                    var getOriginalMethodInfo = proxyGenericType.BaseType.GetMethod(nameof(Proxy<object>.GetOriginal));
                    tb.OverrideMethod(
                        getOriginalMethodInfo,
                        il => il
                            .Ldarg(0)
                            .Ldfld(backingField)
                            .Ret());
                }

                var propertyList = new List<PropertyInfo>();
                foreach (var prop in propertyNames)
                {
                    var propertyInfo = tb.OverrideProperty(prop);
                    propertyInfo.MakeDebuggerBrowsableCollapsed();
                    propertyList.Add(propertyInfo);
                }

                tb.MakeCompilerGenerated();

                tb.SetDebuggerDisplayProperties(propertyNames);
                tb.DefineEqualsByPublicPropertiesMethod(propertyList);
                tb.DefineGetHashCodeByPublicPropertiesMethod(propertyList);
                tb.DefineToStringByPublicPropertiesMethod(propertyList);

                var objectType = tb.CreateTypeInfo();

                if (objectType == null)
                {
                    // According to its signature, tb.CreateTypeInfo() can return null
                    throw new NotSupportedException();
                }

                return objectType;
            });
        }

        public static Type MakeInstantiatedProxyType<TEntity>(this Type proxyGenericType, IEnumerable<string> propertyNames, bool withOriginal)
        {
            return _instantiatedProxyTypeCache.GetOrAdd((proxyGenericType, typeof(TEntity), withOriginal, propertyNames.ToList()), key =>
            {
                var genericType = key.ProxyGenericType.MakeInstantiatedProxyGenericType(key.Properties, key.WithOriginal);

                return genericType.MakeGenericType(key.EntityType);
            });
        }

        public static MethodInfo GetPublicGenericMethod(
            this Type type,
            string name,
            Type[] genericTypes,
            Type[] parameterTypes) => GetGenericMethod(type, name, genericTypes, parameterTypes);

        public static MethodInfo GetNonPublicGenericMethod(
            this Type type,
            string name,
            Type[] genericTypes,
            Type[] parameterTypes) => GetGenericMethod(type, name, genericTypes, parameterTypes, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

        public static MethodInfo GetGenericMethod(
            this Type type,
            string name,
            Type[] genericTypes,
            Type[] parameterTypes,
            BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance)
        {
            var methods = type.GetMethods(bindingFlags);
            foreach (var genericMethod in methods.Where(m =>
                m.Name == name && m.IsGenericMethodDefinition && m.GetGenericArguments().Length == genericTypes.Length))
            {
                try
                {
                    var method = genericMethod.MakeGenericMethod(genericTypes);
                    var methodParameterTypes = method.GetParameters().Select(p => p.ParameterType);

                    if (methodParameterTypes.SequenceEqual(parameterTypes, new SimpleTypeComparer()))
                    {
                        return method;
                    }
                }
                catch (ArgumentException ex)
                {
                    // TODO Try to come up with a better check for proper candidate
                    if (ex.InnerException is not VerificationException)
                    {
                        throw;
                    }

                    // Do nothing if VerificationException occurs. Try to discover another candidate
                }
            }

            throw new ArgumentException(
                $"Type `{type.Name}` does not have a generic method `{name}` with {genericTypes.Length} type parameter(s) that can be called with parameters of type(s) [{string.Join<Type>(", ", parameterTypes)}]");
        }

        public static bool IsEnumerableType(this Type type)
        {
            return type.GetTypeInfo().ImplementedInterfaces.Contains(typeof(IEnumerable)) ||
                   type == typeof(IEnumerable);
        }

        public static bool IsNullable(this Type type)
        {
            return type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>);
        }

        public static Type UnwrapIfNullable(this Type type)
        {
            return type.IsNullable() ? type.GetGenericArguments()[0] : type;
        }

        public static object CreateInstanceAndHoistBaseException(this Type type, params object?[] args)
        {
            try
            {
                var bindingFlags = BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance;
                return Activator.CreateInstance(type, bindingFlags, null, args, null);
            }
            catch (TargetInvocationException e)
            {
                ExceptionDispatchInfo.Capture(e.GetBaseException()).Throw();
                throw;
            }
        }

        public static object GetDefault(this Type type)
        {
            var method = typeof(TypeExtensions).GetGenericMethod(nameof(GetDefault), new[] { type }, Type.EmptyTypes);
            return method.Invoke(null, null);
        }

        public static T GetDefault<T>() => default!;

        public static bool IsAnonymousType(this Type type)
        {
            return Attribute.IsDefined(type, typeof(CompilerGeneratedAttribute), false)
                && type.IsGenericType && type.Name.Contains("AnonymousType", StringComparison.InvariantCulture)
                && (type.Name.StartsWith("<>", StringComparison.Ordinal) || type.Name.StartsWith("VB$", StringComparison.Ordinal))
                && type.Attributes.HasFlag(TypeAttributes.NotPublic);
        }

        public static bool IsSupportComparisons(this Type type)
        {
            type = type.UnwrapIfNullable();

            if (type == typeof(byte) || type == typeof(sbyte) || type == typeof(short) || type == typeof(ushort)
                || type == typeof(int) || type == typeof(uint) || type == typeof(long) || type == typeof(ulong))
            {
                // Integer types doesn't expose op_XXX operators...
                return true;
            }

            return type.GetMethod("op_LessThan", BindingFlags.Static | BindingFlags.Public) != null
                && type.GetMethod("op_LessThanOrEqual", BindingFlags.Static | BindingFlags.Public) != null
                && type.GetMethod("op_GreaterThan", BindingFlags.Static | BindingFlags.Public) != null
                && type.GetMethod("op_GreaterThanOrEqual", BindingFlags.Static | BindingFlags.Public) != null;
        }

        private static string GenerateInstantiatedProxyGenericTypeName()
        {
            lock (_proxyTypeCountLock)
            {
                return $"<>InstantiatedProxy`{++_proxyTypeCount}";
            }
        }

        private static string SanitizeTypeName(string name)
        {
            var result = name.Replace("<", string.Empty, StringComparison.InvariantCulture).Replace(">__", string.Empty, StringComparison.InvariantCulture);

            var apoIndex = result.IndexOf('`', StringComparison.InvariantCulture);

            if (apoIndex > 0)
            {
                result = result.Substring(0, apoIndex);
            }

            if (result.StartsWith("Input", StringComparison.Ordinal))
            {
                result = result[5..];
            }

            return result;
        }

        private static string GetTypeName(this Type type)
        {
            if (type.IsGenericType || type.IsGenericTypeDefinition)
            {
                return type.Name.Split('`')[0];
            }

            return type.Name;
        }

        private static Func<object, object?> CreatePropertyDelegate(Type target, string name)
        {
            // Get the property with reflection
            var property = target.GetProperties(Flags)
                .OrderBy(p => p.Name)
                .ThenBy(p => p.DeclaringType != target)
                .FirstOrDefault(p => p.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

            if (property == null)
            {
                throw new InvalidOperationException($"Expected to find property {name} on {target.HumanizedName()} but it does not exist.");
            }

            // Use reflection to call the method to generate our delegate
            MethodInfo constructedHelper = _delegateHelperMethod.MakeGenericMethod(
                property.DeclaringType, property.GetMethod.ReturnType);

            return (Func<object, object?>)constructedHelper.Invoke(null, new object[] { property });
        }

        private static Func<object, object?> DelegateHelper<TTarget, TReturn>(PropertyInfo property)
        {
            // Convert the slow MethodInfo into a fast, strongly typed, open delegate
            Func<TTarget, TReturn> func = (Func<TTarget, TReturn>)Delegate.CreateDelegate(typeof(Func<TTarget, TReturn>), property.GetMethod);

            // Now create a more weakly typed delegate which will call the strongly typed one
            return (object target) => func((TTarget)target);
        }

        private class SimpleTypeComparer : IEqualityComparer<Type>
        {
            public bool Equals(Type x, Type y)
            {
                return x.Assembly == y.Assembly &&
                       x.Namespace == y.Namespace &&
                       x.Name == y.Name
                       && x.GetGenericArguments().SequenceEqual(y.GetGenericArguments(), new SimpleTypeComparer());
            }

            public int GetHashCode(Type obj)
            {
                throw new NotImplementedException();
            }
        }
    }
}
