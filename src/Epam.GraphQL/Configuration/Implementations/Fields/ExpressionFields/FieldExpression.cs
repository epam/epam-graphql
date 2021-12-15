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
using Expr = System.Linq.Expressions.Expression;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ExpressionFields
{
    internal class FieldExpression<TEntity, TReturnType, TExecutionContext> : IFieldExpression<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        private readonly Expression<Func<TEntity, TReturnType>> _expression;
        private readonly ExpressionField<TEntity, TReturnType, TExecutionContext> _field;
        private Func<TEntity, TReturnType> _resolver;

        public FieldExpression(ExpressionField<TEntity, TReturnType, TExecutionContext> field, string name, Expression<Func<TEntity, TReturnType>> expression)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            _expression = expression ?? throw new ArgumentNullException(nameof(expression));
            Expression = Expr.Lambda<Func<TExecutionContext, TEntity, TReturnType>>(
                _expression.Body,
                Expr.Parameter(typeof(TExecutionContext)),
                _expression.Parameters[0]);

            _field = field ?? throw new ArgumentNullException(nameof(field));
            Name = name;
        }

        public bool IsReadOnly => !_expression.IsProperty() && !typeof(Input).IsAssignableFrom(typeof(TEntity));

        public string Name { get; }

        public PropertyInfo PropertyInfo => _expression.IsProperty() ? _expression.GetPropertyInfo() : null;

        public Expression<Func<TExecutionContext, TEntity, TReturnType>> Expression { get; }

        public void ValidateExpression()
        {
            ExpressionValidator.Validate(_expression);
        }

        public TReturnType Resolve(IResolveFieldContext context, object source)
        {
            // TODO Check for input field (!_field.IsInputField && ...)
            if (context.Source is Proxy<TEntity> proxy)
            {
                var name = _field.Name;
                var func = proxy.GetType().GetPropertyDelegate(name);
                return (TReturnType)func(proxy);
            }

            if (_resolver == null)
            {
                _resolver = _expression.Compile();
            }

            return _resolver((TEntity)source);
        }

        public TReturnType Resolve(IResolveFieldContext context)
        {
            return Resolve(context, context.Source);
        }

        object IFieldResolver.Resolve(IResolveFieldContext context)
        {
            return Resolve(context);
        }

        LambdaExpression ISorter<TExecutionContext>.BuildExpression(TExecutionContext context) => _expression;

        public bool Equals(ISorter<TExecutionContext> other)
        {
            if (other is FieldExpression<TEntity, TReturnType, TExecutionContext> fieldExpression)
            {
                return Name.Equals(fieldExpression.Name, StringComparison.Ordinal)
                    && ExpressionEqualityComparer.Instance.Equals(_expression, fieldExpression._expression);
            }

            return false;
        }
    }
}
