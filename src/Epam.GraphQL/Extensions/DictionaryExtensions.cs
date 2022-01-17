// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using Epam.GraphQL.Helpers;

namespace Epam.GraphQL.Extensions
{
    internal static class DictionaryExtensions
    {
        private static readonly Type _proxyType = typeof(Proxy<>);
        private static readonly Type _proxyTypeGenericParameter = _proxyType.GetTypeInfo().GenericTypeParameters[0];

        private static readonly ConcurrentDictionary<(Type? BaseType, string TypeName, ICollection<KeyValuePair<string, Type>> Properties), Type> _typeCache = new(
            new ValueTupleEqualityComparer<Type?, string, ICollection<KeyValuePair<string, Type>>>(EqualityComparer<Type?>.Default, EqualityComparer<string>.Default, new CollectionEqualityComparer<KeyValuePair<string, Type>>()));

        private static readonly ConcurrentDictionary<ICollection<KeyValuePair<string, Type>>, Type> _proxyTypeCache = new(
            new CollectionEqualityComparer<KeyValuePair<string, Type>>());

        private static long _typeCacheCount;

        private static long _proxyTypeCacheCount;

        public static Type MakeType(this IDictionary<string, Type>? properties, string? typeName, Type? parent = null)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            return _typeCache.GetOrAdd((parent, typeName, properties), key =>
            {
                Interlocked.Increment(ref _typeCacheCount);

                var (baseType, name, props) = key;
                var baseTypeName = baseType == null || baseType == typeof(object) ? string.Empty : baseType.Name;
                var baseTypeIsInterface = baseType != null && baseType.IsInterface;

                var tb = ILUtils.DefineType($"<{baseTypeName}>__{name}`{_typeCacheCount}", baseTypeIsInterface ? null : baseType);

                var propertyList = new List<PropertyBuilder>();

                if (baseTypeIsInterface)
                {
                    tb.AddInterfaceImplementation(baseType!);

                    foreach (var prop in baseType!.GetProperties())
                    {
                        var backingField = tb.DefineBackingField(prop.Name, prop.PropertyType);
                        propertyList.Add(tb.DefineProperty(backingField, prop.Name));
                    }

                    foreach (var baseInterface in baseType.GetInterfaces())
                    {
                        foreach (var prop in baseInterface.GetProperties())
                        {
                            var backingField = tb.DefineBackingField(prop.Name, prop.PropertyType);
                            propertyList.Add(tb.DefineProperty(backingField, prop.Name)); // TODO Check if baseInterface should be passed to DefineProperty for interfaces
                        }
                    }

                    foreach (var method in baseType.GetMethods().Concat(baseType.GetInterfaces().SelectMany(t => t.GetMethods())).Where(m => !m.IsSpecialName && m.IsAbstract))
                    {
                        tb.DefineNotImplementedMethodOverride(method);
                    }
                }
                else if (baseType != null)
                {
                    foreach (var prop in baseType.GetProperties(BindingFlags.Public | BindingFlags.Instance).Where(p => !p.CanWrite))
                    {
                        var backingField = tb.DefineBackingField(prop.Name, prop.PropertyType);
                        propertyList.Add(tb.DefineProperty(backingField, prop.Name));
                    }
                }

                tb.MakeCompilerGenerated();

                foreach (var field in props)
                {
                    var backingField = tb.DefineBackingField(field.Key, field.Value);
                    propertyList.Add(tb.DefineProperty(backingField, field.Key));
                }

                tb.SetDebuggerDisplayProperties(props.Select(p => p.Key));
                tb.DefineEqualsByPublicPropertiesMethod(propertyList);
                tb.DefineGetHashCodeByPublicPropertiesMethod(propertyList);
                tb.DefineToStringByPublicPropertiesMethod(propertyList);

                var objectType = tb.CreateTypeInfo();

                if (objectType == null)
                {
                    // According to its signature, tb.CreateTypeInfo() can return null
                    throw new NotSupportedException();
                }

                return objectType!;
            });
        }

        public static Type MakeProxyType(this IDictionary<string, Type> properties, string typeName)
        {
            if (properties == null)
            {
                throw new ArgumentNullException(nameof(properties));
            }

            if (string.IsNullOrEmpty(typeName))
            {
                throw new ArgumentNullException(nameof(typeName));
            }

            return _proxyTypeCache.GetOrAdd(properties, properties =>
            {
                Interlocked.Increment(ref _proxyTypeCacheCount);

                var tb = ILUtils.DefineType($"<{typeName}>__Proxy`{_proxyTypeCacheCount}", _proxyType);

                tb.DefineNotImplementedVirtualProperty("$original", _proxyTypeGenericParameter).MakeDebuggerBrowsableNever();

                foreach (var prop in properties)
                {
                    tb.DefineNotImplementedVirtualProperty(prop.Key, prop.Value).MakeDebuggerBrowsableNever();
                }

                tb.MakeCompilerGenerated();

                var objectType = tb.CreateTypeInfo();

                if (objectType == null)
                {
                    // According to its signature, tb.CreateTypeInfo() can return null
                    throw new NotSupportedException();
                }

                return objectType;
            });
        }

        public static TValue GetOrAdd<TKey, TValue>(this IDictionary<TKey, TValue> dictionary, TKey key, Func<TKey, TValue> valueFactory)
        {
            if (dictionary.TryGetValue(key, out var value))
            {
                return value;
            }

            value = valueFactory(key);
            dictionary.Add(key, value);
            return value;
        }

        public static string ToFriendlyString(this IReadOnlyDictionary<string, object> dictionary)
        {
            var builder = new StringBuilder();
            ToFriendlyStringImpl(dictionary, builder, 0);
            return builder.ToString();
        }

        private static void ToFriendlyStringImpl(object obj, StringBuilder builder, int indent)
        {
            if (obj == null)
            {
                builder.Append("null");
                return;
            }

            if (obj is IReadOnlyDictionary<string, object> dictionary)
            {
                builder.AppendLine("{");

                var first = true;
                foreach (var kv in dictionary)
                {
                    if (!first)
                    {
                        builder.AppendLine(",");
                    }

                    first = false;
                    builder.Append(' ', indent + 4);
                    builder.Append(kv.Key.ToJsonString());
                    builder.Append(": ");
                    ToFriendlyStringImpl(kv.Value, builder, indent + 4);
                }

                if (dictionary.Count != 0)
                {
                    builder.AppendLine();
                }

                builder.Append(' ', indent);
                builder.Append('}');

                return;
            }

            if (obj is IEnumerable<object> enumerable)
            {
                builder.Append('[');

                var first = true;
                foreach (var item in enumerable)
                {
                    if (!first)
                    {
                        builder.Append(", ");
                    }

                    first = false;
                    ToFriendlyStringImpl(item, builder, indent);
                }

                builder.Append(']');

                return;
            }

            if (obj is string str)
            {
                builder.Append(str.ToJsonString());

                return;
            }

            if (obj is DateTime dateTime)
            {
                builder.Append(dateTime.ToString("yyyy'-'MM'-'dd'T'HH':'mm':'ss.FFFFFFFK", DateTimeFormatInfo.InvariantInfo).ToJsonString());

                return;
            }

            if (obj is bool boolean)
            {
                builder.Append(boolean ? "true" : "false");

                return;
            }

            builder.Append(obj.ToString());
        }
    }
}
