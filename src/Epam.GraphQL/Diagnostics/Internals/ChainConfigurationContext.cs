// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Extensions;

namespace Epam.GraphQL.Diagnostics.Internals
{
    internal class ChainConfigurationContext : ConfigurationContext,
        IChainConfigurationContext
    {
        private readonly IChainConfigurationContextOwner _owner;

        public ChainConfigurationContext(
            IChainConfigurationContextOwner owner,
            ObjectConfigurationContextBase parent,
            ChainConfigurationContext? previous,
            string operation)
            : base(parent)
        {
            _owner = owner;
            Operation = operation;
            Previous = previous;
        }

        protected ChainConfigurationContext(ChainConfigurationContext toClone, Func<ConfigurationContext, IConfigurationContext> additionalChildFactory)
            : base(toClone, additionalChildFactory)
        {
            _owner = toClone._owner;
            Operation = toClone.Operation;
            Previous = toClone.Previous;

            _owner.ConfigurationContext = this;
        }

        public new ObjectConfigurationContextBase Parent => (ObjectConfigurationContextBase)base.Parent!;

        IConfigurationContext IChildConfigurationContext.Parent => Parent;

        protected string Operation { get; }

        protected IChainConfigurationContext? Previous { get; }

        protected IChainConfigurationContext? Next { get; private set; }

        public override bool Contains(IConfigurationContext item)
        {
            return (Previous != null && (ReferenceEquals(Previous, item) || Previous.Contains(item)))
                || base.Contains(item);
        }

        public IChainConfigurationContext Chain(string operation)
        {
            var newContext = new ChainConfigurationContext(_owner, Parent, this, operation);
            Parent.ReplaceChild(this, newContext);
            _owner.ConfigurationContext = newContext;
            Next = newContext;

            return newContext;
        }

        public IChainConfigurationContext Chain<T>(string operation)
        {
            return Chain($"{operation}<{typeof(T).HumanizedName()}>");
        }

        public IChainConfigurationContext Chain<T1, T2>(string operation)
        {
            return Chain($"{operation}<{typeof(T1).HumanizedName()}, {typeof(T2).HumanizedName()}>");
        }

        public IChainConfigurationContext Argument(string arg)
        {
            AddChild(ChainArgumentConfigurationContext.Create(this, arg, optional: false));
            return this;
        }

        public IChainConfigurationContext Argument(Delegate arg)
        {
            AddChild(ChainArgumentConfigurationContext.Create(this, arg, optional: false));
            return this;
        }

        public IChainConfigurationContext OptionalArgument(Delegate? arg)
        {
            AddChild(ChainArgumentConfigurationContext.Create(this, arg, optional: true));
            return this;
        }

        public IChainConfigurationContext OptionalArgument<T>(T[]? arg)
        {
            AddChild(ChainArgumentConfigurationContext.Create(this, arg, optional: true));
            return this;
        }

        public IInlinedChainConfigurationContext Argument<TReturnType, TExecutionContext>(Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> arg)
        {
            return new InlinedChainConfigurationContext(this, parent => ChainArgumentConfigurationContext.Create((ChainConfigurationContext)parent, arg));
        }

        public IInlinedChainConfigurationContext OptionalArgument<TReturnType, TExecutionContext>(Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? arg)
        {
            return new InlinedChainConfigurationContext(this, parent => arg == null
                ? ChainArgumentConfigurationContext.Create((ChainConfigurationContext)parent, arg, optional: true)
                : ChainArgumentConfigurationContext.Create((ChainConfigurationContext)parent, arg));
        }

        public IChainConfigurationContext Argument(LambdaExpression arg)
        {
            AddChild(ChainArgumentConfigurationContext.Create(this, arg, optional: false));
            return this;
        }

        public IChainConfigurationContext OptionalArgument(LambdaExpression? arg)
        {
            AddChild(ChainArgumentConfigurationContext.Create(this, arg, optional: true));
            return this;
        }

        public IResolvedChainConfigurationContext Argument<TKey, TValue>(Func<IEnumerable<TKey>, IEnumerable<KeyValuePair<TKey, TValue>>> arg)
        {
            return new ResolvedChainConfigurationContext(this, parent => ChainArgumentConfigurationContext.Create((ChainConfigurationContext)parent, arg, optional: false));
        }

        public IResolvedChainConfigurationContext Argument<TContext, TKey, TValue>(Func<TContext, IEnumerable<TKey>, IEnumerable<KeyValuePair<TKey, TValue>>> arg)
        {
            return new ResolvedChainConfigurationContext(this, parent => ChainArgumentConfigurationContext.Create((ChainConfigurationContext)parent, arg, optional: false));
        }

        public IResolvedChainConfigurationContext Argument<TKey, TValue>(Func<IEnumerable<TKey>, Task<IDictionary<TKey, TValue>>> arg)
        {
            return new ResolvedChainConfigurationContext(this, parent => ChainArgumentConfigurationContext.Create((ChainConfigurationContext)parent, arg, optional: false));
        }

        public IResolvedChainConfigurationContext Argument<TContext, TKey, TValue>(Func<TContext, IEnumerable<TKey>, Task<IDictionary<TKey, TValue>>> arg)
        {
            return new ResolvedChainConfigurationContext(this, parent => ChainArgumentConfigurationContext.Create((ChainConfigurationContext)parent, arg, optional: false));
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
            var arguments = Children.OfType<ChainArgumentConfigurationContext>().ToList();
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

                    if (choosenItems.Contains(arguments[i]))
                    {
                        builder.Append(" // <-----");
                    }
                }
            }

            builder.Append(')');

            if (Next == null)
            {
                builder.Append(';');
            }

            if (lastIndex >= 0 && choosenItems.Contains(arguments[lastIndex]))
            {
                builder.Append(" // <-----");
            }
        }
    }
}
