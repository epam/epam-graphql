// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Collections.Generic;
using System.Linq;
using System.Text;
using Epam.GraphQL.Helpers;

namespace Epam.GraphQL.Diagnostics
{
    internal abstract class ConfigurationContext : IConfigurationContext
    {
        private readonly List<IConfigurationContext> _children = new();

        public ConfigurationContext(ConfigurationContext? parent)
        {
            Parent = parent;
        }

        public ConfigurationContext? Parent { get; }

        IConfigurationContext? IConfigurationContext.Parent => Parent;

        protected IReadOnlyList<IConfigurationContext> Children => _children;

        public abstract void Append(StringBuilder stringBuilder, IEnumerable<IConfigurationContext> choosenItems, int indent);

        public virtual bool Contains(IConfigurationContext item)
        {
            return Children.Any(child => ReferenceEquals(child, item) || child.Contains(item));
        }

        public virtual void Clear()
        {
            _children.Clear();
        }

        public override string ToString()
        {
            return this.ToString(0);
        }

        public IObjectConfigurationContext New()
        {
            return AddChild(new ObjectConfigurationContext(this));
        }

        public T ReplaceChild<T>(IConfigurationContext oldChild, T newChild)
            where T : IConfigurationContext
        {
            var index = _children.FindIndex(item => ReferenceEquals(item, oldChild));

            Guards.ThrowInvalidOperationIf(index < 0, "Internal error");

            _children[index] = newChild;

            return newChild;
        }

        public T AddChild<T>(T child)
            where T : IConfigurationContext
        {
            _children.Add(child);
            return child;
        }
    }
}
