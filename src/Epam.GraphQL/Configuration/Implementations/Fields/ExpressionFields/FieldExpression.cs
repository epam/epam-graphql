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
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Sorters;
using GraphQL;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ExpressionFields
{
    internal class FieldExpression<TEntity, TReturnType, TExecutionContext> : IFieldExpression<TEntity, TReturnType, TExecutionContext>
    {
        private readonly ExpressionField<TEntity, TReturnType, TExecutionContext> _field;
        private readonly Expression<Func<TEntity, TReturnType>> _originalExpression;
        private readonly Lazy<Func<TEntity, TReturnType>> _resolver;
        private readonly Lazy<Func<object, object?>> _proxyResolver;

        public FieldExpression(ExpressionField<TEntity, TReturnType, TExecutionContext> field, Expression<Func<TEntity, TReturnType>> expression)
        {
            _originalExpression = expression;
            ContextedExpression = Expression.Lambda<Func<TExecutionContext, TEntity, TReturnType>>(
                _originalExpression.Body,
                Expression.Parameter(typeof(TExecutionContext)),
                _originalExpression.Parameters[0]);

            _field = field;
            _resolver = new Lazy<Func<TEntity, TReturnType>>(_originalExpression.Compile);
            _proxyResolver = new Lazy<Func<object, object?>>(() => _field.Parent.ProxyAccessor.ProxyType.GetPropertyDelegate(Name));
        }

        public bool IsReadOnly => !_originalExpression.IsProperty() && !typeof(Input).IsAssignableFrom(typeof(TEntity));

        public string Name => _field.Name;

        public IChainConfigurationContext ConfigurationContext => _field.ConfigurationContext;

        public PropertyInfo? PropertyInfo => _originalExpression.IsProperty() ? _originalExpression.GetPropertyInfo() : null;

        public bool IsGroupable => _field.IsGroupable;

        public LambdaExpression ContextedExpression { get; }

        public LambdaExpression OriginalExpression => _originalExpression;

        public void ValidateExpression()
        {
            ExpressionValidator.Validate(_originalExpression);
        }

        public TReturnType? Resolve(IResolveFieldContext context)
        {
            try
            {
                return context.Source is Proxy<TEntity> proxy
                    ? (TReturnType?)_proxyResolver.Value(proxy)
                    : _resolver.Value((TEntity)context.Source);
            }
            catch (Exception e)
            {
                throw new ExecutionError(ConfigurationContext.GetRuntimeError($"Error during resolving field. See an inner exception for details.", ConfigurationContext), e);
            }
        }

        object? IFieldResolver.Resolve(IResolveFieldContext context)
        {
            return Resolve(context);
        }

        LambdaExpression ISorter<TExecutionContext>.BuildExpression(TExecutionContext context) => _originalExpression;
    }
}
