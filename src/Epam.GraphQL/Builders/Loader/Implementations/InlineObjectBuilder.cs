// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Configuration.Implementations;
using Epam.GraphQL.Configuration.Implementations.Fields;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using GraphQL.Types;

namespace Epam.GraphQL.Builders.Loader.Implementations
{
    internal class InlineObjectBuilder<TSourceType, TExecutionContext> :
        IInlineObjectBuilder<TSourceType, TExecutionContext>,
        IInlineGraphTypeResolver<TSourceType, TExecutionContext>,
        IVoid
        where TSourceType : class
    {
        private readonly RelationRegistry<TExecutionContext> _registry;
        private readonly BaseObjectGraphTypeConfigurator<TSourceType, TExecutionContext> _objectGraphTypeConfigurator;
        private Type? _childProjectionType;

        public InlineObjectBuilder(
            IField<TExecutionContext> parent,
            RelationRegistry<TExecutionContext> registry,
            Action<IInlineObjectBuilder<TSourceType, TExecutionContext>>? build,
            IChainConfigurationContext configurationContext,
            bool isInputType)
        {
            _registry = registry;

            if (build == null)
            {
                var context = configurationContext.New();

                _objectGraphTypeConfigurator = isInputType
                    ? _registry.RegisterInputAutoObjectGraphType<TSourceType>(context)
                    : _registry.RegisterAutoObjectGraphType<TSourceType>(context);
            }
            else
            {
                var context = configurationContext is IInlinedChainConfigurationContext inlinedChainConfigurationContext
                    ? inlinedChainConfigurationContext.Build.New()
                    : configurationContext.New();

                _objectGraphTypeConfigurator = isInputType
                    ? new InputObjectGraphTypeConfigurator<TSourceType, TExecutionContext>(parent, context, registry, isAuto: false)
                    : new ObjectGraphTypeConfigurator<TSourceType, TExecutionContext>(parent, context, registry, isAuto: false);

                build(this);
            }
        }

        public string Name { get => _objectGraphTypeConfigurator.Name; set => _objectGraphTypeConfigurator.Name = value; }

        public IInlineExpressionField<TSourceType, TReturnType, TExecutionContext> Field<TReturnType>(Expression<Func<TSourceType, TReturnType>> expression, string? deprecationReason)
        {
            var field = _objectGraphTypeConfigurator.AddField(
                owner => _objectGraphTypeConfigurator.ConfigurationContext.Chain(owner, nameof(Field))
                    .Argument(expression),
                null,
                expression,
                deprecationReason);
            return field;
        }

        public IInlineExpressionField<TSourceType, TReturnType, TExecutionContext> Field<TReturnType>(string name, Expression<Func<TSourceType, TReturnType>> expression, string? deprecationReason)
        {
            var field = _objectGraphTypeConfigurator.AddField(
                owner => _objectGraphTypeConfigurator.ConfigurationContext.Chain(owner, nameof(Field))
                    .Argument(name)
                    .Argument(expression),
                name,
                expression,
                deprecationReason);
            return field;
        }

        public IInlineExpressionField<TSourceType, TReturnType, TExecutionContext> Field<TReturnType>(string name, Expression<Func<TExecutionContext, TSourceType, TReturnType>> expression, string? deprecationReason)
        {
            var field = _objectGraphTypeConfigurator.AddField(
                owner => _objectGraphTypeConfigurator.ConfigurationContext.Chain(owner, nameof(Field))
                    .Argument(name)
                    .Argument(expression),
                name,
                expression,
                deprecationReason);
            return field;
        }

        public IVoid Field<TReturnType>(Expression<Func<TSourceType, IEnumerable<TReturnType>>> expression, string? deprecationReason)
        {
            _objectGraphTypeConfigurator.AddField(
                owner => _objectGraphTypeConfigurator.ConfigurationContext.Chain(owner, nameof(Field)).Argument(expression),
                null,
                expression,
                deprecationReason);

            return this;
        }

        public IVoid Field<TReturnType>(string name, Expression<Func<TSourceType, IEnumerable<TReturnType>>> expression, string? deprecationReason)
        {
            _objectGraphTypeConfigurator.AddField(
                owner => _objectGraphTypeConfigurator.ConfigurationContext.Chain(owner, nameof(Field))
                    .Argument(name)
                    .Argument(expression),
                name,
                expression,
                deprecationReason);

            return this;
        }

        public IVoid Field<TReturnType>(string name, Expression<Func<TExecutionContext, TSourceType, IEnumerable<TReturnType>>> expression, string? deprecationReason)
        {
            _objectGraphTypeConfigurator.AddField(
                owner => _objectGraphTypeConfigurator.ConfigurationContext.Chain(owner, nameof(Field))
                    .Argument(name)
                    .Argument(expression),
                name,
                expression,
                deprecationReason);

            return this;
        }

        public IInlineObjectFieldBuilder<TSourceType, TExecutionContext> Field(string name, string? deprecationReason = null)
        {
            var fieldType = _objectGraphTypeConfigurator.Field(name, deprecationReason);
            return new InlineObjectFieldBuilder<Field<TSourceType, TExecutionContext>, TSourceType, TExecutionContext>(_registry, fieldType);
        }

        public void ConfigureFrom<TProjection>()
            where TProjection : Projection<TSourceType, TExecutionContext>
        {
            _childProjectionType = typeof(TProjection);
        }

        public (IGraphType? GraphType, Type? Type) Resolve()
        {
            if (_childProjectionType != null)
            {
                var baseLoaderType = ReflectionHelpers.FindMatchingGenericBaseType(_childProjectionType, typeof(ProjectionBase<,>));
                var childEntityType = baseLoaderType.GetGenericArguments().First();
                var graphType = _registry.GetEntityGraphType(_childProjectionType, childEntityType);
                return (null, graphType);
            }

            var resolvedType = _objectGraphTypeConfigurator is InputObjectGraphTypeConfigurator<TSourceType, TExecutionContext>
                ? (IComplexGraphType)new InputObjectGraphType<TSourceType>()
                : new ObjectGraphType<object>
                {
                    IsTypeOf = o => o is TSourceType or Proxy<TSourceType>,
                };

            _objectGraphTypeConfigurator.ConfigureGraphType(resolvedType);

            return (resolvedType, null);
        }

        public void Validate(IConfigurationContext configurationContext)
        {
            if (_childProjectionType != null && _objectGraphTypeConfigurator.Fields.Any())
            {
                configurationContext.AddError($"You must use either {nameof(Field)} or {nameof(ConfigureFrom)} calls, not both at the same time.");
            }

            if (_childProjectionType == null)
            {
                _objectGraphTypeConfigurator.ValidateFields();
            }
        }

        public IObjectGraphTypeConfigurator<TSourceType, TExecutionContext> ResolveConfigurator()
        {
            if (_childProjectionType != null)
            {
                var baseLoaderType = ReflectionHelpers.FindMatchingGenericBaseType(_childProjectionType, typeof(ProjectionBase<,>));
                var childEntityType = baseLoaderType.GetGenericArguments().First();
                var loader = _registry.ResolveLoader(_childProjectionType, childEntityType);

                return _objectGraphTypeConfigurator is InputObjectGraphTypeConfigurator<TSourceType, TExecutionContext>
                    ? (IObjectGraphTypeConfigurator<TSourceType, TExecutionContext>)loader.GetInputObjectGraphTypeConfigurator()
                    : (IObjectGraphTypeConfigurator<TSourceType, TExecutionContext>)loader.GetObjectGraphTypeConfigurator();
            }

            return _objectGraphTypeConfigurator;
        }

        public void OnEntityLoaded<T>(
            Expression<Func<TSourceType, T>> expression,
            Action<TExecutionContext, T> action)
        {
            Guards.ThrowIfNull(expression, nameof(expression));
            Guards.ThrowIfNull(action, nameof(action));

            _objectGraphTypeConfigurator.AddOnEntityLoaded(expression, action);
        }

        public void OnEntityLoaded<TKey, T>(
            Expression<Func<TSourceType, TKey>> keyExpression,
            Func<TExecutionContext, IEnumerable<TKey>, IDictionary<TKey, T>> fetch,
            Action<TExecutionContext, T> action)
        {
            Guards.ThrowIfNull(keyExpression, nameof(keyExpression));
            Guards.ThrowIfNull(fetch, nameof(fetch));
            Guards.ThrowIfNull(action, nameof(action));

            _objectGraphTypeConfigurator.AddOnEntityLoaded(keyExpression, fetch, action);
        }

        public void OnEntityLoaded<TKey, T>(
            Expression<Func<TSourceType, TKey>> keyExpression,
            Func<TExecutionContext, IEnumerable<TKey>, Task<IDictionary<TKey, T>>> fetch,
            Action<TExecutionContext, T> action)
        {
            Guards.ThrowIfNull(keyExpression, nameof(keyExpression));
            Guards.ThrowIfNull(fetch, nameof(fetch));
            Guards.ThrowIfNull(action, nameof(action));

            _objectGraphTypeConfigurator.AddOnEntityLoaded(keyExpression, fetch, action);
        }
    }
}
