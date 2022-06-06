// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using System.Reflection;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Sorters;
using GraphQL;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ExpressionFields
{
    internal class FieldContextExpression<TEntity, TReturnType, TExecutionContext> : IFieldExpression<TEntity, TReturnType, TExecutionContext>
    {
        private readonly ExpressionField<TEntity, TReturnType, TExecutionContext> _field;
        private readonly Expression<Func<TExecutionContext, TEntity, TReturnType>> _expression;
        private Func<TExecutionContext, TEntity, TReturnType>? _resolver;

        public FieldContextExpression(ExpressionField<TEntity, TReturnType, TExecutionContext> field, string name, Expression<Func<TExecutionContext, TEntity, TReturnType>> expression)
        {
            _expression = expression;
            _field = field;
            Name = name;
        }

        public bool IsGroupable => _field.IsGroupable;

        public LambdaExpression ContextedExpression => _expression;

        public LambdaExpression OriginalExpression => _expression;

        public bool IsReadOnly => true;

        public PropertyInfo? PropertyInfo => null;

        public string Name { get; }

        public IChainConfigurationContext ConfigurationContext => _field.ConfigurationContext;

        public void ValidateExpression()
        {
            ExpressionValidator.Validate(_expression);
        }

        public TReturnType? Resolve(IResolveFieldContext context)
        {
            // TODO Check for input field (!_field.IsInputField && ...)
            if (context.Source is Proxy<TEntity> proxy)
            {
                var name = _field.Name;
                var func = proxy.GetType().GetPropertyDelegate(name);
                return (TReturnType?)func(proxy);
            }

            _resolver ??= _expression.Compile();

            return _resolver(context.GetUserContext<TExecutionContext>(), (TEntity)context.Source);
        }

        object? IFieldResolver.Resolve(IResolveFieldContext context)
        {
            return Resolve(context);
        }

        LambdaExpression ISorter<TExecutionContext>.BuildExpression(TExecutionContext context) => Expression.Lambda<Func<TEntity, TReturnType>>(
            _expression.Body.ReplaceParameter(_expression.Parameters[0], Expression.Constant(context, typeof(TExecutionContext))),
            _expression.Parameters[1]);
    }
}
