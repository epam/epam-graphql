// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using System.Reflection;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Sorters;
using GraphQL;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ExpressionFields
{
    internal class FieldExpression<TEntity, TReturnType, TExecutionContext> : IFieldExpression<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        private readonly ExpressionField<TEntity, TReturnType, TExecutionContext> _field;
        private readonly Expression<Func<TEntity, TReturnType>> _originalExpression;
        private Func<TEntity, TReturnType>? _resolver;

        public FieldExpression(ExpressionField<TEntity, TReturnType, TExecutionContext> field, string name, Expression<Func<TEntity, TReturnType>> expression)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            _originalExpression = expression ?? throw new ArgumentNullException(nameof(expression));
            ContextedExpression = Expression.Lambda<Func<TExecutionContext, TEntity, TReturnType>>(
                _originalExpression.Body,
                Expression.Parameter(typeof(TExecutionContext)),
                _originalExpression.Parameters[0]);

            _field = field ?? throw new ArgumentNullException(nameof(field));
            Name = name;
        }

        public bool IsReadOnly => !_originalExpression.IsProperty() && !typeof(Input).IsAssignableFrom(typeof(TEntity));

        public string Name { get; }

        public PropertyInfo? PropertyInfo => _originalExpression.IsProperty() ? _originalExpression.GetPropertyInfo() : null;

        public bool IsGroupable => _field.IsGroupable;

        public LambdaExpression ContextedExpression { get; }

        public LambdaExpression OriginalExpression => _originalExpression;

        public void ValidateExpression()
        {
            ExpressionValidator.Validate(_originalExpression);
        }

        public TReturnType? Resolve(IResolveFieldContext context, object source)
        {
            // TODO Check for input field (!_field.IsInputField && ...)
            if (context.Source is Proxy<TEntity> proxy)
            {
                var name = _field.Name;
                var func = proxy.GetType().GetPropertyDelegate(name);
                return (TReturnType?)func(proxy);
            }

            if (_resolver == null)
            {
                _resolver = _originalExpression.Compile();
            }

            return _resolver((TEntity)source);
        }

        public TReturnType? Resolve(IResolveFieldContext context)
        {
            return Resolve(context, context.Source);
        }

        object? IFieldResolver.Resolve(IResolveFieldContext context)
        {
            return Resolve(context);
        }

        LambdaExpression ISorter<TExecutionContext>.BuildExpression(TExecutionContext context) => _originalExpression;

        public bool Equals(ISorter<TExecutionContext> other)
        {
            if (other is FieldExpression<TEntity, TReturnType, TExecutionContext> fieldExpression)
            {
                return Name.Equals(fieldExpression.Name, StringComparison.Ordinal)
                    && ExpressionEqualityComparer.Instance.Equals(_originalExpression, fieldExpression._originalExpression);
            }

            return false;
        }
    }
}
