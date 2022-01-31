// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Security;
using Epam.GraphQL.Helpers;

namespace Epam.GraphQL.Tests.Helpers
{
    internal static class TypeExtensions
    {
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

        public static MethodInfo GetPublicGenericMethod(
            this Type type,
            Action<GenericMethodParameters> configure)
            => GetGenericMethod(type, configure, BindingFlags.Public | BindingFlags.Static | BindingFlags.Instance);

        public static MethodInfo GetNonPublicGenericMethod(
            this Type type,
            Action<GenericMethodParameters> configure)
            => GetGenericMethod(type, configure, BindingFlags.NonPublic | BindingFlags.Static | BindingFlags.Instance);

        private static MethodInfo GetGenericMethod(
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

        private static MethodInfo GetGenericMethod(Type type, Action<GenericMethodParameters> configure, BindingFlags bindingFlags)
        {
            var parameters = new GenericMethodParameters();
            configure(parameters);

            var methodInfo = type.GetMethods(bindingFlags)
                .SingleOrDefault(parameters);

            if (methodInfo != null)
            {
                return methodInfo;
            }

            return type
                .GetTypeInfo()
                .ImplementedInterfaces
                .SelectMany(intf => intf.GetMethods(bindingFlags))
                .Single(parameters);
        }

        public class GenericMethodParameters
        {
            private readonly List<Func<MethodInfo, ParameterInfo, bool>> _parameters = new();
            private string _methodName;
            private int? _genericTypeArgumentCount;

            public static implicit operator Func<MethodInfo, bool>(GenericMethodParameters parameters)
            {
                return parameters.Match;
            }

            public GenericMethodParameters HasName(string methodName)
            {
                _methodName = methodName;
                return this;
            }

            public GenericMethodParameters HasOneGenericTypeParameter()
            {
                _genericTypeArgumentCount = 1;
                return this;
            }

            public GenericMethodParameters HasTwoGenericTypeParameters()
            {
                _genericTypeArgumentCount = 2;
                return this;
            }

            public GenericMethodParameters Parameter<T>()
            {
                return Parameter(typeof(T));
            }

            public GenericMethodParameters Parameter(Type type)
            {
                _parameters.Add((_, param) => param.ParameterType == type);
                return this;
            }

            public GenericMethodParameters AnyParameter()
            {
                _parameters.Add((_, param) => true);
                return this;
            }

            public GenericMethodParameters GenericTypeParameter(Func<Type[], Type> builder)
            {
                _parameters.Add((method, param) =>
                {
                    var type = builder(method.GetGenericArguments());
                    return param.ParameterType == type;
                });

                return this;
            }

            private bool Match(MethodInfo methodInfo)
            {
                Guards.ThrowIfNull(_methodName, "methodName");
                Guards.ThrowIfNull(_genericTypeArgumentCount, "argumentCount");

                var parametersInfo = methodInfo.GetParameters();

                return methodInfo.IsGenericMethodDefinition
                    && methodInfo.Name.Equals(_methodName, StringComparison.OrdinalIgnoreCase)
                    && methodInfo.GetGenericArguments().Length == _genericTypeArgumentCount.Value
                    && _parameters.Select((match, index) => match(methodInfo, parametersInfo[index]))
                        .All(param => param);
            }
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
