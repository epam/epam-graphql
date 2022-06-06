// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using Epam.GraphQL.Builders.Loader;

namespace Epam.GraphQL.Diagnostics.Internals
{
    internal abstract class ChainArgumentConfigurationContext : ConfigurationContext, IChainArgumentConfigurationContext
    {
        protected ChainArgumentConfigurationContext(ChainConfigurationContext parent, bool isDefault)
            : base(parent)
        {
            IsDefault = isDefault;
        }

        public bool IsDefault { get; }

        public new IChainConfigurationContext Parent => (IChainConfigurationContext)base.Parent!;

        IConfigurationContext IChildConfigurationContext.Parent => Parent;

        public static ChainArgumentConfigurationContext Create<T>(ChainConfigurationContext parent, T[]? array, bool optional)
        {
            return new ChainArgumentConfigurationContext<T[]?>(parent, new ArrayPrinter<T>(GenericPrinter<T>.Instance), array, optional && array == null);
        }

        public static ChainArgumentConfigurationContext Create(ChainConfigurationContext parent, string? str, bool optional)
        {
            return new ChainArgumentConfigurationContext<string?>(parent, StringPrinter.Instance, str, optional && str == null);
        }

        public static ChainArgumentConfigurationContext Create(ChainConfigurationContext parent, LambdaExpression? expression, bool optional)
        {
            return new ChainArgumentConfigurationContext<LambdaExpression?>(parent, LambdaExpressionPrinter.Instance, expression, optional && expression == null);
        }

        public static ChainArgumentConfigurationContext Create(ChainConfigurationContext parent, Delegate? func, bool optional)
        {
            return new ChainArgumentConfigurationContext<Delegate?>(parent, DelegatePrinter.Instance, func, optional && func == null);
        }

        public static ChainDelegateArgumentConfigurationContext Create<TReturnType, TExecutionContext>(ChainConfigurationContext parent, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> func)
        {
            return new ChainDelegateArgumentConfigurationContext(parent, func);
        }
    }

    internal class ChainArgumentConfigurationContext<T> : ChainArgumentConfigurationContext
    {
        private readonly IPrinter<T> _printer;
        private readonly T _value;

        public ChainArgumentConfigurationContext(ChainConfigurationContext parent, IPrinter<T> printer, T value, bool isDefault)
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

    internal class ChainDelegateArgumentConfigurationContext : ChainArgumentConfigurationContext
    {
        private readonly Delegate _value;

        public ChainDelegateArgumentConfigurationContext(ChainConfigurationContext parent, Delegate value)
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
