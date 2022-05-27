// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epam.GraphQL.Extensions;

namespace Epam.GraphQL.Diagnostics.Internals
{
    internal abstract class ObjectConfigurationContextBase : ConfigurationContext, IRootConfigurationContext
    {
        private readonly List<ConfigurationException> _errors = new();

        protected ObjectConfigurationContextBase(ConfigurationContext? parent)
            : base(parent)
        {
        }

        public void AppendBody(StringBuilder stringBuilder, IEnumerable<IConfigurationContext> choosenItems, int indent)
        {
            var indicies = new List<int>();
            var choosenItemList = choosenItems.ToList();

            if (choosenItemList.Count == 0)
            {
                indicies.AddRange(Enumerable.Range(0, Children.Count));
            }
            else
            {
                for (var i = 0; i < Children.Count; i++)
                {
                    var index = choosenItemList.FindIndex(choosenItem => ReferenceEquals(choosenItem, Children[i]) || Children[i].Contains(choosenItem));

                    if (index >= 0)
                    {
                        if (i > 0)
                        {
                            indicies.Add(i - 1);
                        }

                        indicies.Add(i);

                        if (i < Children.Count - 1)
                        {
                            indicies.Add(i + 1);
                        }
                    }
                }

                indicies = indicies
                    .Distinct()
                    .OrderBy(index => index)
                    .ToList();
            }

            var nextIndex = 0;

            foreach (var index in indicies)
            {
                if (index != nextIndex)
                {
                    stringBuilder.Append(' ', 4 * indent);
                    stringBuilder.AppendLine("// ...");
                    stringBuilder.AppendLine();
                }

                if (nextIndex > 0)
                {
                    stringBuilder.AppendLine();
                }

                var child = Children[index];
                child.Append(stringBuilder, choosenItems, indent);
                stringBuilder.Append(';');

                if (choosenItems.Contains(child))
                {
                    stringBuilder.Append(" // <-----");
                }

                stringBuilder.AppendLine();
                nextIndex = index + 1;
            }

            if (nextIndex != Children.Count)
            {
                stringBuilder.AppendLine();
                stringBuilder.Append(' ', 4 * indent);
                stringBuilder.AppendLine("// ...");
            }
        }

        public override void Clear()
        {
            base.Clear();
            _errors.Clear();
        }

        public IChainConfigurationContext Chain(IChainConfigurationContextOwner owner, string operation)
        {
            return AddChild(new ChainConfigurationContext(owner, this, null, operation));
        }

        public IChainConfigurationContext Chain<T>(IChainConfigurationContextOwner owner, string operation)
        {
            return Chain(owner, $"{operation}<{typeof(T).HumanizedName()}>");
        }

        public void AddError(string message, params IConfigurationContext[] invalidItems)
        {
            _errors.Add(new ConfigurationException(GetError(message, invalidItems)));
        }

        public string GetError(string message, params IConfigurationContext[] invalidItems)
        {
            var builder = new StringBuilder();

            builder.Append(DoGetError(message));
            builder.AppendLine();
            builder.AppendLine("Details:");
            this.GetRoot().Append(builder, invalidItems, 0);

            return builder.ToString();
        }

        public string GetRuntimeError(string message, params IConfigurationContext[] invalidItems)
        {
            var builder = new StringBuilder();

            builder.Append(DoGetRuntimeError(message));
            builder.AppendLine();
            this.GetRoot().Append(builder, invalidItems, 0);

            return builder.ToString();
        }

        public void ThrowErrors()
        {
            if (_errors.Count > 0)
            {
                throw new AggregateException(_errors);
            }
        }

        protected virtual string DoGetError(string message)
        {
            return message;
        }

        protected virtual string DoGetRuntimeError(string message)
        {
            return message;
        }
    }
}
