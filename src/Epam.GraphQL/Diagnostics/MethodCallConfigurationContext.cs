// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Extensions;

namespace Epam.GraphQL.Diagnostics
{
    internal class MethodCallConfigurationContext : ConfigurationContext, IChildConfigurationContext
    {
        public MethodCallConfigurationContext(
            ObjectConfigurationContextBase parent,
            MethodCallConfigurationContext? previous,
            string operation)
            : base(parent)
        {
            Operation = operation;
            Previous = previous;
        }

        public new ObjectConfigurationContextBase Parent => (ObjectConfigurationContextBase)base.Parent!;

        IConfigurationContext IChildConfigurationContext.Parent => Parent;

        protected string Operation { get; }

        protected MethodCallConfigurationContext? Previous { get; }

        public override bool Contains(IConfigurationContext item)
        {
            return (Previous != null && (ReferenceEquals(Previous, item) || Previous.Contains(item)))
                || base.Contains(item);
        }

        public MethodCallConfigurationContext NextOperation(string operation)
        {
            return Parent.ReplaceChild(this, new MethodCallConfigurationContext(Parent, this, operation));
        }

        public MethodCallConfigurationContext NextOperation<T>(string operation)
        {
            return NextOperation($"{operation}<{typeof(T).HumanizedName()}>");
        }

        public MethodCallConfigurationContext NextOperation<T1, T2>(string operation)
        {
            return NextOperation($"{operation}<{typeof(T1).HumanizedName()}, {typeof(T2).HumanizedName()}>");
        }

        public MethodCallConfigurationContext Argument(string arg)
        {
            AddChild(MethodCallArgumentConfigurationContext.Create(this, arg, optional: false));
            return this;
        }

        public MethodCallConfigurationContext Argument(Delegate arg)
        {
            AddChild(MethodCallArgumentConfigurationContext.Create(this, arg, optional: false));
            return this;
        }

        public MethodCallConfigurationContext OptionalArgument(Delegate? arg)
        {
            AddChild(MethodCallArgumentConfigurationContext.Create(this, arg, optional: true));
            return this;
        }

        public MethodCallConfigurationContext OptionalArgument<T>(T[]? arg)
        {
            AddChild(MethodCallArgumentConfigurationContext.Create(this, arg, optional: true));
            return this;
        }

        public MethodCallArgumentConfigurationContext Argument<TReturnType, TExecutionContext>(Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? arg)
            where TReturnType : class
        {
            if (arg == null)
            {
                return AddChild(MethodCallArgumentConfigurationContext.Create(this, arg, optional: false));
            }

            return AddChild(MethodCallArgumentConfigurationContext.Create(this, arg));
        }

        public MethodCallArgumentConfigurationContext OptionalArgument<TReturnType, TExecutionContext>(Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? arg)
            where TReturnType : class
        {
            if (arg == null)
            {
                return AddChild(MethodCallArgumentConfigurationContext.Create(this, arg, optional: true));
            }

            return AddChild(MethodCallArgumentConfigurationContext.Create(this, arg));
        }

        public MethodCallConfigurationContext Argument(LambdaExpression arg)
        {
            AddChild(MethodCallArgumentConfigurationContext.Create(this, arg, optional: false));
            return this;
        }

        public MethodCallConfigurationContext OptionalArgument(LambdaExpression? arg)
        {
            AddChild(MethodCallArgumentConfigurationContext.Create(this, arg, optional: true));
            return this;
        }

        public override void Append(StringBuilder builder, IEnumerable<IConfigurationContext> choosenItems, int indent)
        {
            if (Previous == null)
            {
                builder.Append(' ', indent * 4);
            }
            else
            {
                Previous.Append(builder, choosenItems, indent);

                if (choosenItems.Contains(Previous))
                {
                    builder.Append(" // <-----");
                }

                builder.AppendLine();
                builder.Append(' ', (indent + 1) * 4);
                builder.Append('.');
            }

            builder.Append(Operation);
            var arguments = Children.OfType<MethodCallArgumentConfigurationContext>().ToList();
            var lastIndex = arguments.Count - 1;

            while (lastIndex >= 0 && arguments[lastIndex].IsDefault)
            {
                lastIndex -= 1;
            }

            builder.Append('(');

            for (var i = 0; i <= lastIndex; i++)
            {
                if (lastIndex == 0)
                {
                    arguments[i].Append(builder, choosenItems, 0);
                }
                else
                {
                    builder.AppendLine();
                    var newIndent = indent + 1;

                    if (Previous != null)
                    {
                        newIndent += 1;
                    }

                    arguments[i].Append(builder, choosenItems, newIndent);
                }

                if (i < lastIndex)
                {
                    builder.Append(',');
                }
            }

            builder.Append(')');
        }
    }
}
