// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Reflection;
using System.Reflection.Emit;

#nullable enable

namespace Epam.GraphQL.Extensions
{
    internal static class ILGeneratorExtensions
    {
        public static ILGenerator Box(this ILGenerator generator, Type type)
        {
            generator.Emit(OpCodes.Box, type);
            return generator;
        }

        public static ILGenerator Brfalse(this ILGenerator generator, Label label)
        {
            generator.Emit(OpCodes.Brfalse, label);
            return generator;
        }

        public static ILGenerator BrS(this ILGenerator generator, Label label)
        {
            generator.Emit(OpCodes.Br_S, label);
            return generator;
        }

        public static ILGenerator Call(this ILGenerator generator, MethodInfo method)
        {
            generator.Emit(method.IsVirtual ? OpCodes.Callvirt : OpCodes.Call, method);
            return generator;
        }

        public static ILGenerator DeclareLocal<T>(this ILGenerator generator)
        {
            generator.DeclareLocal(typeof(T));
            return generator;
        }

        public static ILGenerator Dup(this ILGenerator generator)
        {
            generator.Emit(OpCodes.Dup);
            return generator;
        }

        public static ILGenerator Initobj<T>(this ILGenerator generator)
        {
            generator.Emit(OpCodes.Initobj, typeof(T));
            return generator;
        }

        public static ILGenerator Isinst(this ILGenerator generator, Type type)
        {
            generator.Emit(OpCodes.Isinst, type);
            return generator;
        }

        public static ILGenerator Ldarg(this ILGenerator generator, ushort index)
        {
            switch (index)
            {
                case 0:
                    generator.Emit(OpCodes.Ldarg_0);
                    break;
                case 1:
                    generator.Emit(OpCodes.Ldarg_1);
                    break;
                case 2:
                    generator.Emit(OpCodes.Ldarg_2);
                    break;
                case 3:
                    generator.Emit(OpCodes.Ldarg_3);
                    break;
                default:
                    if (index <= byte.MaxValue)
                    {
                        generator.Emit(OpCodes.Ldarg_S, (byte)index);
                    }
                    else
                    {
                        generator.Emit(OpCodes.Ldarg, index);
                    }

                    break;
            }

            return generator;
        }

        public static ILGenerator LdcI4(this ILGenerator generator, int value)
        {
            switch (value)
            {
                case -1:
                    generator.Emit(OpCodes.Ldc_I4_M1);
                    break;

                case 0:
                    generator.Emit(OpCodes.Ldc_I4_0);
                    break;

                case 1:
                    generator.Emit(OpCodes.Ldc_I4_1);
                    break;

                case 2:
                    generator.Emit(OpCodes.Ldc_I4_2);
                    break;

                case 3:
                    generator.Emit(OpCodes.Ldc_I4_3);
                    break;

                case 4:
                    generator.Emit(OpCodes.Ldc_I4_4);
                    break;

                case 5:
                    generator.Emit(OpCodes.Ldc_I4_5);
                    break;

                case 6:
                    generator.Emit(OpCodes.Ldc_I4_6);
                    break;

                case 7:
                    generator.Emit(OpCodes.Ldc_I4_7);
                    break;

                case 8:
                    generator.Emit(OpCodes.Ldc_I4_8);
                    break;

                default:
                    if (value is >= byte.MinValue and <= byte.MaxValue)
                    {
                        generator.Emit(OpCodes.Ldc_I4_S, value);
                    }
                    else
                    {
                        generator.Emit(OpCodes.Ldc_I4, value);
                    }

                    break;
            }

            return generator;
        }

        public static ILGenerator Ldfld(this ILGenerator generator, FieldInfo field)
        {
            generator.Emit(OpCodes.Ldfld, field);
            return generator;
        }

        public static ILGenerator Ldloc(this ILGenerator generator, ushort index)
        {
            switch (index)
            {
                case 0:
                    generator.Emit(OpCodes.Ldloc_0);
                    break;
                case 1:
                    generator.Emit(OpCodes.Ldloc_1);
                    break;
                case 2:
                    generator.Emit(OpCodes.Ldloc_2);
                    break;
                case 3:
                    generator.Emit(OpCodes.Ldloc_3);
                    break;
                default:
                    if (index <= byte.MaxValue)
                    {
                        generator.Emit(OpCodes.Ldloc_S, (byte)index);
                    }
                    else
                    {
                        generator.Emit(OpCodes.Ldloc, index);
                    }

                    break;
            }

            return generator;
        }

        public static ILGenerator Ldloca(this ILGenerator generator, ushort index)
        {
            if (index <= byte.MaxValue)
            {
                generator.Emit(OpCodes.Ldloca_S, (byte)index);
            }
            else
            {
                generator.Emit(OpCodes.Ldloca, index);
            }

            return generator;
        }

        public static ILGenerator Ldstr(this ILGenerator generator, string value)
        {
            generator.Emit(OpCodes.Ldstr, value);
            return generator;
        }

        public static ILGenerator Mul(this ILGenerator generator)
        {
            generator.Emit(OpCodes.Mul);
            return generator;
        }

        public static ILGenerator Newarr<T>(this ILGenerator generator)
        {
            generator.Emit(OpCodes.Newarr, typeof(T));
            return generator;
        }

        public static ILGenerator Newobj(this ILGenerator generator, ConstructorInfo ctor)
        {
            generator.Emit(OpCodes.Newobj, ctor);
            return generator;
        }

        public static ILGenerator PutLabel(this ILGenerator generator, Label label)
        {
            generator.MarkLabel(label);
            return generator;
        }

        public static ILGenerator Ret(this ILGenerator generator)
        {
            generator.Emit(OpCodes.Ret);
            return generator;
        }

        public static ILGenerator Stelem<T>(this ILGenerator generator)
        {
            generator.Emit(OpCodes.Stelem, typeof(T));
            return generator;
        }

        public static ILGenerator Stfld(this ILGenerator generator, FieldInfo field)
        {
            generator.Emit(OpCodes.Stfld, field);
            return generator;
        }

        public static ILGenerator Stloc(this ILGenerator generator, ushort index)
        {
            switch (index)
            {
                case 0:
                    generator.Emit(OpCodes.Stloc_0);
                    break;
                case 1:
                    generator.Emit(OpCodes.Stloc_1);
                    break;
                case 2:
                    generator.Emit(OpCodes.Stloc_2);
                    break;
                case 3:
                    generator.Emit(OpCodes.Stloc_3);
                    break;
                default:
                    if (index <= byte.MaxValue)
                    {
                        generator.Emit(OpCodes.Stloc_S, (byte)index);
                    }
                    else
                    {
                        generator.Emit(OpCodes.Stloc, index);
                    }

                    break;
            }

            return generator;
        }

        public static ILGenerator Throw(this ILGenerator generator)
        {
            generator.Emit(OpCodes.Throw);
            return generator;
        }
    }
}
