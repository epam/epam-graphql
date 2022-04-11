// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Epam.GraphQL.Builders.Loader;

namespace Epam.GraphQL.Diagnostics
{
    internal abstract class MethodCallArgumentConfigurationContext : ConfigurationContext, IChildConfigurationContext
    {
        protected MethodCallArgumentConfigurationContext(MethodCallConfigurationContext parent, bool isDefault)
            : base(parent)
        {
            IsDefault = isDefault;
        }

        public bool IsDefault { get; }

        public new MethodCallConfigurationContext Parent => (MethodCallConfigurationContext)base.Parent!;

        IConfigurationContext IChildConfigurationContext.Parent => Parent;

        public static MethodCallArgumentConfigurationContext Create<T>(MethodCallConfigurationContext parent, T[]? array, bool optional)
        {
            return new MethodCallArgumentConfigurationContext<T[]?>(parent, new ArrayPrinter<T>(GenericPrinter<T>.Instance), array, optional && array == null);
        }

        public static MethodCallArgumentConfigurationContext Create(MethodCallConfigurationContext parent, string? str, bool optional)
        {
            return new MethodCallArgumentConfigurationContext<string?>(parent, StringPrinter.Instance, str, optional && str == null);
        }

        public static MethodCallArgumentConfigurationContext Create(MethodCallConfigurationContext parent, LambdaExpression? expression, bool optional)
        {
            return new MethodCallArgumentConfigurationContext<LambdaExpression?>(parent, LambdaExpressionPrinter.Instance, expression, optional && expression == null);
        }

        public static MethodCallArgumentConfigurationContext Create(MethodCallConfigurationContext parent, Delegate? func, bool optional)
        {
            return new MethodCallArgumentConfigurationContext<Delegate?>(parent, DelegatePrinter.Instance, func, optional && func == null);
        }

        public static MethodCallDelegateArgumentConfigurationContext<TReturnType, TExecutionContext> Create<TReturnType, TExecutionContext>(MethodCallConfigurationContext parent, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> func)
            where TReturnType : class
        {
            return new MethodCallDelegateArgumentConfigurationContext<TReturnType, TExecutionContext>(parent, func);
        }
    }

    internal class MethodCallArgumentConfigurationContext<T> : MethodCallArgumentConfigurationContext
    {
        private readonly IPrinter<T> _printer;
        private readonly T _value;

        public MethodCallArgumentConfigurationContext(MethodCallConfigurationContext parent, IPrinter<T> printer, T value, bool isDefault)
            : base(parent, isDefault)
        {
            _printer = printer;
            _value = value;
        }

        public override void Append(StringBuilder stringBuilder, IEnumerable<IConfigurationContext> choosenItems, int indent)
        {
            stringBuilder.Append(' ', 4 * indent);
            stringBuilder.Append(_printer.Print(_value));
        }
    }

    internal class MethodCallDelegateArgumentConfigurationContext<TReturnType, TExecutionContext> : MethodCallArgumentConfigurationContext
        where TReturnType : class
    {
        private readonly Delegate _value;

        public MethodCallDelegateArgumentConfigurationContext(MethodCallConfigurationContext parent, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> value)
            : base(parent, false)
        {
            _value = value;
        }

        public override void Append(StringBuilder stringBuilder, IEnumerable<IConfigurationContext> choosenItems, int indent)
        {
            stringBuilder.Append(' ', 4 * indent);
            stringBuilder.Append(_value.Method.PrintArguments());
            stringBuilder.AppendLine(" =>");
            stringBuilder.Append(' ', 4 * indent);
            stringBuilder.AppendLine("{");

            if (Children.Count == 1 && Children[0] is IObjectConfigurationContext body)
            {
                body.AppendBody(stringBuilder, choosenItems, indent + 1);
            }

            stringBuilder.Append(' ', 4 * indent);
            stringBuilder.Append('}');
        }
    }
}
