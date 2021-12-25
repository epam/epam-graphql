// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Helpers;
using GraphQL;

namespace Epam.GraphQL.Sorters.Implementations
{
    internal class CustomSorter<TEntity, TValueType, TExecutionContext> : ISorter<TExecutionContext>
    {
        private readonly Expression<Func<TEntity, TValueType>> _selector;

        public CustomSorter(string name, Expression<Func<TEntity, TValueType>> selector)
        {
            Name = name?.ToCamelCase() ?? throw new ArgumentNullException(nameof(name));
            _selector = selector ?? throw new ArgumentNullException(nameof(selector));
        }

        public string Name { get; }

        public bool IsGroupable => false;

        public LambdaExpression OriginalExpression => _selector;

        public LambdaExpression BuildExpression(TExecutionContext context) => _selector;

        public bool Equals(CustomSorter<TEntity, TValueType, TExecutionContext> other)
        {
            if (other == null)
            {
                return false;
            }

            return Name.Equals(other.Name, StringComparison.Ordinal)
                && ExpressionEqualityComparer.Instance.Equals(_selector, other._selector);
        }

        public bool Equals(ISorter<TExecutionContext> other) => Equals(other as CustomSorter<TEntity, TValueType, TExecutionContext>);

        public override bool Equals(object obj) => Equals(obj as CustomSorter<TEntity, TValueType, TExecutionContext>);

        public override int GetHashCode()
        {
            var hashCode = default(HashCode);
            hashCode.Add(Name, StringComparer.Ordinal);
            hashCode.Add(_selector, ExpressionEqualityComparer.Instance);
            return hashCode.ToHashCode();
        }
    }
}
