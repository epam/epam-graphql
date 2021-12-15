// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using System.Reflection;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Sorters;
using GraphQL;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ExpressionFields
{
    internal class FieldContextExpression<TEntity, TReturnType, TExecutionContext> : IFieldExpression<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        private readonly ExpressionField<TEntity, TReturnType, TExecutionContext> _field;
        private Func<TExecutionContext, TEntity, TReturnType> _resolver;

        public FieldContextExpression(ExpressionField<TEntity, TReturnType, TExecutionContext> field, string name, Expression<Func<TExecutionContext, TEntity, TReturnType>> expression)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            Expression = expression ?? throw new ArgumentNullException(nameof(expression));
            _field = field ?? throw new ArgumentNullException(nameof(field));
            Name = name;
        }

        public Expression<Func<TExecutionContext, TEntity, TReturnType>> Expression { get; }

        public bool IsReadOnly => true;

        public PropertyInfo PropertyInfo => null;

        public string Name { get; }

        public void ValidateExpression()
        {
            ExpressionValidator.Validate(Expression);
        }

        public TReturnType Resolve(IResolveFieldContext context, object source)
        {
            // TODO Check for input field (!_field.IsInputField && ...)
            if (source is Proxy<TEntity> proxy)
            {
                var name = _field.Name;
                var func = proxy.GetType().GetPropertyDelegate(name);
                return (TReturnType)func(proxy);
            }

            if (_resolver == null)
            {
                _resolver = Expression.Compile();
            }

            return _resolver(context.GetUserContext<TExecutionContext>(), (TEntity)source);
        }

        public TReturnType Resolve(IResolveFieldContext context)
        {
            return Resolve(context, context.Source);
        }

        object IFieldResolver.Resolve(IResolveFieldContext context)
        {
            return Resolve(context);
        }

        public bool Equals(ISorter<TExecutionContext> other)
        {
            if (other is FieldContextExpression<TEntity, TReturnType, TExecutionContext> fieldExpression)
            {
                return Name.Equals(fieldExpression.Name, StringComparison.Ordinal)
                    && ExpressionEqualityComparer.Instance.Equals(Expression, fieldExpression.Expression);
            }

            return false;
        }

        LambdaExpression ISorter<TExecutionContext>.BuildExpression(TExecutionContext context) => System.Linq.Expressions.Expression.Lambda<Func<TEntity, TReturnType>>(
            Expression.Body.ReplaceParameter(Expression.Parameters[0], System.Linq.Expressions.Expression.Constant(context, typeof(TExecutionContext))),
            Expression.Parameters[1]);
    }
}
