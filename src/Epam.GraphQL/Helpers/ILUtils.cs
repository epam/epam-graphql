// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;

namespace Epam.GraphQL.Helpers
{
    internal static class ILUtils
    {
        private const TypeAttributes DefaultAttributes = TypeAttributes.NotPublic | TypeAttributes.Class | TypeAttributes.AutoClass | TypeAttributes.AnsiClass | TypeAttributes.BeforeFieldInit | TypeAttributes.AutoLayout;
        private const string AssemblyName = "Epam.GraphQL";
        private static readonly AssemblyName _assemblyName = new(AssemblyName);
        private static readonly AssemblyBuilder _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(_assemblyName, AssemblyBuilderAccess.RunAndCollect);
        private static readonly ModuleBuilder _moduleBuilder = _assemblyBuilder.DefineDynamicModule(AssemblyName);

        public static TypeBuilder DefineType(string typeName, Type? parent = null)
        {
            if (parent != null && parent.IsInterface)
            {
                throw new ArgumentException($"Interfaces currently are not supported as `{nameof(parent)}` argument.");
            }

            lock (_moduleBuilder)
            {
                var tb = _moduleBuilder.DefineType(
                    typeName,
                    DefaultAttributes,
                    parent);

                if (parent != null && parent.IsGenericTypeDefinition)
                {
                    var parameters = tb.DefineGenericParameters(parent.GetTypeInfo().GenericTypeParameters.Select(type => type.Name).ToArray());
                    var parentType = parent.MakeGenericType(parameters);
                    tb.SetParent(parentType);
                }

                return tb;
            }
        }
    }
}
