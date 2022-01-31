// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using Epam.GraphQL.Helpers;

namespace Epam.GraphQL.Extensions
{
    internal static class TypeBuilderExtensions
    {
        private static readonly ConcurrentDictionary<Type, MethodInfo> _equalsMethodsInfo = new();
        private static readonly ConcurrentDictionary<Type, MethodInfo> _getHashCodeMethodsInfo = new();
        private static readonly ConcurrentDictionary<Type, MethodInfo> _toStringMethodsInfo = new();
        private static readonly ConcurrentDictionary<Type, MethodInfo> _equalityComparerDefaultGetters = new();
        private static readonly ConcurrentDictionary<Type, MethodInfo> _equalityComparerEqualsMethods = new();
        private static readonly ConcurrentDictionary<Type, MethodInfo> _hashCodeAddMethodsInfo = new();

        private static readonly MethodInfo _stringFormat = ReflectionHelpers.GetMethodInfo<string, object[], string>(string.Format);
        private static readonly MethodInfo _hashCodeToHashCodeMethodInfo = ReflectionHelpers.GetMethodInfo(default(HashCode).ToHashCode);
        private static MethodInfo? _hashCodeAddMethodInfo;

        public static FieldBuilder DefineBackingField(this TypeBuilder tb, string propertyName, Type propertyType)
        {
            var fieldBuilder = tb.DefineField($"<{propertyName}>k__BackingField", propertyType, FieldAttributes.Private);
            return fieldBuilder;
        }

        public static PropertyBuilder DefineProperty(this TypeBuilder tb, FieldInfo backingField, string propertyName)
        {
            var propertyBuilder = tb.DefineReadOnlyProperty(backingField, propertyName);
            var setterMethodBuilder = tb.DefineSetter(backingField, propertyName);
            propertyBuilder.SetSetMethod(setterMethodBuilder);

            return propertyBuilder;
        }

        public static PropertyBuilder DefineReadOnlyProperty(this TypeBuilder tb, FieldInfo backingField, string propertyName)
        {
            var propertyType = backingField.FieldType;
            var propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);
            var getterMethodBuilder = tb.DefineGetter(backingField, propertyName);

            propertyBuilder.SetGetMethod(getterMethodBuilder);

            return propertyBuilder;
        }

        public static MethodBuilder DefineGetter(this TypeBuilder tb, FieldInfo backingField, string propertyName)
        {
            var propertyType = backingField.FieldType;
            var methodName = $"get_{propertyName}";
            var flags = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;

            var getterMethodBuilder = tb.DefineMethod(
                methodName,
                flags,
                propertyType,
                Type.EmptyTypes);

            getterMethodBuilder.GetILGenerator()
                .Ldarg(0)
                .Ldfld(backingField)
                .Ret();

            if (tb.BaseType != null)
            {
                var methodInfo = tb.GetBaseType().GetMethod(methodName);
                if (methodInfo != null && methodInfo.IsVirtual && !methodInfo.IsFinal)
                {
                    if (tb.BaseType.IsGenericType && !tb.BaseType.IsGenericTypeDefinition)
                    {
                        methodInfo = TypeBuilder.GetMethod(tb.BaseType, methodInfo);
                    }

                    tb.DefineMethodOverride(getterMethodBuilder, methodInfo);
                }
            }

            return getterMethodBuilder;
        }

        public static PropertyBuilder DefineNotImplementedVirtualProperty(this TypeBuilder tb, string propertyName, Type propertyType)
        {
            var propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, propertyType, null);

            var getterMethodBuilder = tb.DefineNotImplementedVirtualGetter(propertyName, propertyType);
            propertyBuilder.SetGetMethod(getterMethodBuilder);

            var setterMethodBuilder = tb.DefineNotImplementedVirtualSetter(propertyName, propertyType);
            propertyBuilder.SetSetMethod(setterMethodBuilder);

            return propertyBuilder;
        }

        public static PropertyBuilder OverrideProperty(this TypeBuilder tb, string propertyName)
        {
            var baseProperty = tb.GetBaseType().GetProperty(propertyName);
            var backingField = tb.DefineBackingField(propertyName, baseProperty.PropertyType);
            return tb.OverrideProperty(backingField, propertyName);
        }

        public static PropertyBuilder OverrideProperty(this TypeBuilder tb, FieldBuilder backingField, string propertyName)
        {
            var propertyBuilder = tb.DefineProperty(propertyName, PropertyAttributes.HasDefault, backingField.FieldType, null);

            var getterMethodBuilder = tb.DefineGetter(backingField, propertyName);
            propertyBuilder.SetGetMethod(getterMethodBuilder);

            var setterMethodBuilder = tb.DefineSetter(backingField, propertyName);
            propertyBuilder.SetSetMethod(setterMethodBuilder);

            return propertyBuilder;
        }

        public static MethodBuilder DefineNotImplementedVirtualGetter(this TypeBuilder tb, string propertyName, Type propertyType)
        {
            var methodName = $"get_{propertyName}";
            var flags = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;

            var getterMethodBuilder = tb.DefineMethod(
                methodName,
                flags,
                propertyType,
                Type.EmptyTypes);

            getterMethodBuilder.GetILGenerator()
                .Newobj(typeof(NotImplementedException).GetConstructor(Type.EmptyTypes))
                .Throw();

            foreach (var implementedInterface in tb.GetInterfaces())
            {
                var methodInfo = implementedInterface.GetMethod(methodName, Type.EmptyTypes);
                if (methodInfo != null)
                {
                    tb.DefineMethodOverride(getterMethodBuilder, methodInfo);
                }
            }

            return getterMethodBuilder;
        }

        public static MethodBuilder DefineSetter(this TypeBuilder tb, FieldInfo backingField, string propertyName)
        {
            var propertyType = backingField.FieldType;
            var methodName = $"set_{propertyName}";
            var flags = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;

            var setterMethodBuilder = tb.DefineMethod(
                methodName,
                flags,
                null,
                new[] { propertyType });

            setterMethodBuilder.GetILGenerator()
                .Ldarg(0)
                .Ldarg(1)
                .Stfld(backingField)
                .Ret();

            if (tb.BaseType != null)
            {
                var methodInfo = tb.GetBaseType().GetMethod(methodName, new[] { propertyType });
                if (methodInfo != null && methodInfo.IsVirtual && !methodInfo.IsFinal)
                {
                    if (tb.BaseType.IsGenericType && !tb.BaseType.IsGenericTypeDefinition)
                    {
                        methodInfo = TypeBuilder.GetMethod(tb.BaseType, methodInfo);
                    }

                    tb.DefineMethodOverride(setterMethodBuilder, methodInfo);
                }
            }

            return setterMethodBuilder;
        }

        public static MethodBuilder DefineNotImplementedVirtualSetter(this TypeBuilder tb, string propertyName, Type propertyType)
        {
            var methodName = $"set_{propertyName}";
            var flags = MethodAttributes.Public | MethodAttributes.SpecialName | MethodAttributes.HideBySig | MethodAttributes.NewSlot | MethodAttributes.Virtual;

            var setterMethodBuilder = tb.DefineMethod(
                methodName,
                flags,
                null,
                new[] { propertyType });

            setterMethodBuilder.GetILGenerator()
                .Newobj(typeof(NotImplementedException).GetConstructor(Type.EmptyTypes))
                .Throw();

            foreach (var implementedInterface in tb.GetInterfaces())
            {
                var methodInfo = implementedInterface.GetMethod(methodName, new[] { propertyType });
                if (methodInfo != null)
                {
                    tb.DefineMethodOverride(setterMethodBuilder, methodInfo);
                }
            }

            return setterMethodBuilder;
        }

        public static void MakeCompilerGenerated(this TypeBuilder tb)
        {
            tb.SetCustomAttribute(new CustomAttributeBuilder(typeof(CompilerGeneratedAttribute).GetConstructor(Type.EmptyTypes), Array.Empty<object>()));
        }

        public static void MakeDebuggerBrowsableNever(this PropertyBuilder tb)
        {
            tb.SetCustomAttribute(new CustomAttributeBuilder(typeof(DebuggerBrowsableAttribute).GetConstructor(new[] { typeof(DebuggerBrowsableState) }), new object[] { DebuggerBrowsableState.Never }));
        }

        public static void MakeDebuggerBrowsableCollapsed(this PropertyBuilder tb)
        {
            tb.SetCustomAttribute(new CustomAttributeBuilder(typeof(DebuggerBrowsableAttribute).GetConstructor(new[] { typeof(DebuggerBrowsableState) }), new object[] { DebuggerBrowsableState.Collapsed }));
        }

        public static void SetDebuggerDisplayProperties(this TypeBuilder tb, IEnumerable<string> propertyNames)
        {
            var debuggerDisplay = $"\\{{ {string.Join(", ", propertyNames.Select(n => $"{n} = {{{n}}}"))} }}";

            tb.SetCustomAttribute(new CustomAttributeBuilder(typeof(DebuggerDisplayAttribute).GetConstructor(new[] { typeof(string) }), new object[] { debuggerDisplay }));
        }

        public static void DefineEqualsByPublicPropertiesMethod(this TypeBuilder typeBuilder, IEnumerable<PropertyInfo> properties)
        {
            var equalsMethodInfo = _equalsMethodsInfo.GetOrAdd(typeBuilder.GetBaseType(), baseType => baseType.GetMethod(
                nameof(object.Equals),
                BindingFlags.Public | BindingFlags.Instance,
                null,
                new[] { typeof(object) },
                null));

            var equals = typeBuilder.DefineMethod(
                nameof(object.Equals),
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                typeof(bool),
                new[] { typeof(object) });

            typeBuilder.DefineMethodOverride(equals, equalsMethodInfo);

            var il = equals.GetILGenerator();
            il.DeclareLocal(typeBuilder);
            var retFalse = il.DefineLabel();
            var ret = il.DefineLabel();

            il
                .Ldarg(1)
                .Isinst(typeBuilder)
                .Stloc(0) // local = argument as the constructed type
                .Ldloc(0); // push result of the "as" operator

            foreach (var property in properties)
            {
                var defaultGetter = _equalityComparerDefaultGetters.GetOrAdd(
                    property.PropertyType,
                    type => typeof(EqualityComparer<>).MakeGenericType(type).GetProperty("Default", BindingFlags.Public | BindingFlags.Static).GetGetMethod());

                var equalsMethod = _equalityComparerEqualsMethods.GetOrAdd(
                    property.PropertyType,
                    type => typeof(EqualityComparer<>).MakeGenericType(type).GetMethod(
                        "Equals",
                        BindingFlags.Public | BindingFlags.Instance,
                        null,
                        new[] { property.PropertyType, property.PropertyType },
                        null));

                var getMethodInfo = property.GetGetMethod();
                il
                    .Brfalse(retFalse) // check if the result of the previous check is false
                    .Call(defaultGetter)
                    .Ldarg(0)
                    .Call(getMethodInfo)
                    .Ldloc(0)
                    .Call(getMethodInfo)
                    .Call(equalsMethod); // push EqualityComparer<FieldType>.Default.Equals(this.property, other.property)
            }

            il
                .BrS(ret) // jump to the end with what was the last result
                .PutLabel(retFalse)
                .LdcI4(0) // push false
                .PutLabel(ret)
                .Ret();
        }

        public static void DefineGetHashCodeByPublicPropertiesMethod(this TypeBuilder typeBuilder, IEnumerable<PropertyInfo> properties)
        {
            var getHasCodeMethodInfo = _getHashCodeMethodsInfo.GetOrAdd(
                typeBuilder.GetBaseType(),
                baseType => baseType.GetMethod(
                    nameof(object.GetHashCode),
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    Type.EmptyTypes,
                    null));

            var getHashCode = typeBuilder.DefineMethod(
                nameof(object.GetHashCode),
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                typeof(int),
                Type.EmptyTypes);

            typeBuilder.DefineMethodOverride(getHashCode, getHasCodeMethodInfo);

            var il = getHashCode.GetILGenerator();

            il
                .DeclareLocal<HashCode>()
                .DeclareLocal<int>()
                .Ldloca(0)
                .Initobj<HashCode>();

            foreach (var property in properties)
            {
                var comparer = typeof(EqualityComparer<>).MakeGenericType(property.PropertyType);
                var getMethodInfo = property.GetGetMethod();

                var addMethod = _hashCodeAddMethodsInfo.GetOrAdd(property.PropertyType, type =>
                {
                    _hashCodeAddMethodInfo ??= ReflectionHelpers.GetMethodInfo<object>(default(HashCode).Add<object>);
                    return _hashCodeAddMethodInfo.MakeGenericMethod(property.PropertyType);
                });

                il
                    .Ldloca(0)
                    .Ldarg(0)
                    .Call(getMethodInfo)
                    .Call(addMethod);
            }

            il
                .Ldloca(0)
                .Call(_hashCodeToHashCodeMethodInfo)
                .Stloc(1)
                .Ldloc(1)
                .Ret();
        }

        public static void DefineToStringByPublicPropertiesMethod(this TypeBuilder typeBuilder, IEnumerable<PropertyInfo> properties)
        {
            var toStringMethodInfo = _toStringMethodsInfo.GetOrAdd(
                typeBuilder.GetBaseType(),
                baseType => baseType.GetMethod(
                    nameof(object.ToString),
                    BindingFlags.Public | BindingFlags.Instance,
                    null,
                    Type.EmptyTypes,
                    null));

            var toString = typeBuilder.DefineMethod(
                nameof(object.ToString),
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                typeof(string),
                Type.EmptyTypes);

            typeBuilder.DefineMethodOverride(toString, toStringMethodInfo);
            var propertyArray = properties.ToArray();

            var template = $"{{{{ {string.Join(", ", propertyArray.Select((p, i) => $"{p.Name} = {{{i}}}"))} }}}}";

            var il = toString.GetILGenerator();

            il
                .Ldstr(template) // push template
                .LdcI4(propertyArray.Length) // push new array
                .Newarr<object>();

            var index = 0;
            foreach (var property in propertyArray)
            {
                var getMethodInfo = property.GetGetMethod();

                il
                    .Dup() // duplicate array ref
                    .LdcI4(index) // push array index
                    .Ldarg(0)
                    .Call(getMethodInfo)
                    .Box(property.PropertyType)
                    .Stelem<object>();

                index++;
            }

            il
                .Call(_stringFormat)
                .Ret();
        }

        public static void OverrideMethod(this TypeBuilder typeBuilder, MethodInfo methodToOverride, Action<ILGenerator> builder)
        {
            var method = typeBuilder.DefineMethod(
                methodToOverride.Name,
                (methodToOverride.Attributes | MethodAttributes.Virtual) & ~MethodAttributes.Abstract,
                methodToOverride.ReturnType,
                methodToOverride.GetParameters().Select(p => p.ParameterType).ToArray());

            typeBuilder.DefineMethodOverride(method, methodToOverride);

            var il = method.GetILGenerator();
            builder(il);
        }

        public static void DefineNotImplementedMethodOverride(this TypeBuilder typeBuilder, MethodInfo methodToOverride)
        {
            typeBuilder.OverrideMethod(
                methodToOverride,
                il =>
                {
                    var ctor = typeof(NotImplementedException).GetConstructor(Type.EmptyTypes);
                    il
                        .Newobj(ctor)
                        .Throw();
                });
        }

        public static Type GetBaseType(this TypeBuilder typeBuilder)
        {
            var baseType = typeBuilder.BaseType;

            if (baseType == null)
            {
                throw new ArgumentException("TypeBuilder must have non-null BaseType", nameof(typeBuilder));
            }

            if (baseType.IsGenericType && !baseType.IsGenericTypeDefinition)
            {
                return baseType.GetGenericTypeDefinition();
            }

            return baseType;
        }
    }
}
