// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;
using Epam.GraphQL.Configuration.Implementations.Fields;
using Epam.GraphQL.Configuration.Implementations.Fields.BatchFields;
using Epam.GraphQL.Configuration.Implementations.Fields.ChildFields;
using Epam.GraphQL.Configuration.Implementations.Fields.ExpressionFields;
using Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Enums;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Filters;
using Epam.GraphQL.Filters.Implementations;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Sorters;
using Epam.GraphQL.Sorters.Implementations;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;
using TypeExtensions = Epam.GraphQL.Extensions.TypeExtensions;

namespace Epam.GraphQL.Configuration.Implementations
{
#pragma warning disable CA1506
    internal abstract class BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> :
        IObjectGraphTypeConfigurator<TEntity, TExecutionContext>, IEquatable<BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext>>
#pragma warning restore CA1506
    {
        private static MethodInfo? _addFieldMethodInfo;
        private static MethodInfo? _addEnumerableFieldMethodInfo;
        private static MethodInfo? _addEnumerableStringExpressionFieldMethodInfo;
        private static MethodInfo? _addEnumerableExpressionFieldMethodInfo;
        private static MethodInfo? _addEnumerableNullableExpressionFieldMethodInfo;
        private static MethodInfo? _addEnumerableObjectExpressionFieldMethodInfo;
        private static MethodInfo? _addExpressionFieldMethodInfo;
        private static MethodInfo? _addExpressionNullableFieldMethodInfo;
        private static MethodInfo? _addExpressionObjectFieldMethodInfo;
        private static MethodInfo? _addExpressionStringFieldMethodInfo;
        private static MethodInfo? _addContextEnumerableStringExpressionFieldMethodInfo;
        private static MethodInfo? _addContextEnumerableExpressionFieldMethodInfo;
        private static MethodInfo? _addContextEnumerableNullableExpressionFieldMethodInfo;
        private static MethodInfo? _addContextEnumerableObjectExpressionFieldMethodInfo;
        private static MethodInfo? _addContextExpressionFieldMethodInfo;
        private static MethodInfo? _addContextExpressionNullableFieldMethodInfo;
        private static MethodInfo? _addContextExpressionObjectFieldMethodInfo;
        private static MethodInfo? _addContextExpressionStringFieldMethodInfo;

        private readonly List<IField<TEntity, TExecutionContext>> _fields = new();
        private readonly List<IInlineFilter<TExecutionContext>> _inlineFilters = new();
        private readonly List<ISorter<TExecutionContext>> _sorters = new();
        private IInlineFilters<TEntity, TExecutionContext>? _filters;

        private string? _name;
        private bool isConfigured;

        protected BaseObjectGraphTypeConfigurator(
            IObjectConfigurationContext configurationContext,
            IField<TExecutionContext>? parent,
            IRegistry<TExecutionContext> registry,
            bool isAuto)
        {
            Registry = registry;
            Parent = parent;
            ProxyAccessor = new ProxyAccessor<TEntity, TExecutionContext>(this);
            ConfigurationContext = configurationContext;
            IsAuto = isAuto;

            if (isAuto)
            {
                var properties = typeof(TEntity)
                    .GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(propInfo => propInfo.CanRead && propInfo.GetIndexParameters().Length == 0);

                foreach (var propertyInfo in properties)
                {
                    AddField(propertyInfo);
                }
            }
        }

        public string Name
        {
            get => _name ?? throw new NotSupportedException();

            set
            {
                if (value != _name)
                {
                    Guards.ThrowIfNullOrEmpty(value, nameof(value));
                    SetGraphQLTypeName(_name, value);

                    _name = value;
                }
            }
        }

        public bool HasInlineFilters => InlineFilters.Any();

        public IReadOnlyList<IField<TEntity, TExecutionContext>> Fields => _fields;

        public ProxyAccessor<TEntity, TExecutionContext> ProxyAccessor { get; }

        IProxyAccessor<TEntity, TExecutionContext> IObjectGraphTypeConfigurator<TEntity, TExecutionContext>.ProxyAccessor => ProxyAccessor;

        public IField<TExecutionContext>? Parent { get; }

        public IReadOnlyList<ISorter<TExecutionContext>> Sorters => _sorters;

        public IRegistry<TExecutionContext> Registry { get; }

        public IObjectConfigurationContext ConfigurationContext { get; }

        protected bool IsAuto { get; }

        private IEnumerable<IInlineFilter<TExecutionContext>> InlineFilters => Fields
            .OfType<IExpressionFieldConfiguration<TEntity, TExecutionContext>>()
            .Where(f => f.IsFilterable)
            .Select(f => f.CreateInlineFilter())
            .Concat(_inlineFilters);

        public TField ReplaceField<TField>(FieldBase<TEntity, TExecutionContext> oldField, TField newField)
            where TField : FieldBase<TEntity, TExecutionContext>
        {
            var index = _fields.FindIndex(field => ReferenceEquals(oldField, field));
            Guards.ThrowInvalidOperationIf(index < 0, $"Internal error: {nameof(ReplaceField)}");

            _fields.RemoveAt(index);
            _fields.Insert(index, newField);

            ProxyAccessor.ReplaceField(oldField, newField);

            return newField;
        }

        public Field<TEntity, TExecutionContext> Field(string name, string? deprecationReason)
        {
            var field = new Field<TEntity, TExecutionContext>(
                owner => ConfigurationContext.Chain(owner, nameof(Field))
                    .Argument(name),
                this,
                name);

            return InternalAddField(field, deprecationReason);
        }

        public ExpressionField<TEntity, TReturnType, TExecutionContext> AddField<TReturnType>(
            Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory,
            string? name,
            Expression<Func<TEntity, TReturnType>> expression,
            string? deprecationReason)
        {
            MethodInfo? addFieldMethodInfo = null;
            if (typeof(TReturnType) == typeof(string))
            {
                _addExpressionStringFieldMethodInfo = ReflectionHelpers.GetMethodInfo<Func<IChainConfigurationContextOwner, IChainConfigurationContext>, string, Expression<Func<TEntity, string>>, string?, ExpressionField<TEntity, string, TExecutionContext>>(
                    AddExpressionField);

                addFieldMethodInfo = _addExpressionStringFieldMethodInfo;
            }
            else if (typeof(TReturnType).IsValueType)
            {
                var unwrappedUnderlyingType = typeof(TReturnType).UnwrapIfNullable();
                var isNullable = typeof(TReturnType) != unwrappedUnderlyingType;

                if (isNullable)
                {
                    _addExpressionNullableFieldMethodInfo ??= ReflectionHelpers.GetMethodInfo<Func<IChainConfigurationContextOwner, IChainConfigurationContext>, string?, Expression<Func<TEntity, int?>>, string?, ExpressionField<TEntity, int?, TExecutionContext>>(
                        AddExpressionField);

                    addFieldMethodInfo = _addExpressionNullableFieldMethodInfo.MakeGenericMethod(unwrappedUnderlyingType);
                }
                else
                {
                    _addExpressionFieldMethodInfo ??= ReflectionHelpers.GetMethodInfo<Func<IChainConfigurationContextOwner, IChainConfigurationContext>, string?, Expression<Func<TEntity, int>>, string?, ExpressionField<TEntity, int, TExecutionContext>>(
                        AddExpressionField);

                    addFieldMethodInfo = _addExpressionFieldMethodInfo.MakeGenericMethod(unwrappedUnderlyingType);
                }
            }
            else
            {
                _addExpressionObjectFieldMethodInfo ??= ReflectionHelpers.GetMethodInfo<Func<IChainConfigurationContextOwner, IChainConfigurationContext>, string, Expression<Func<TEntity, object>>, string?, ExpressionField<TEntity, object, TExecutionContext>>(
                    AddObjectExpressionField);

                addFieldMethodInfo = _addExpressionObjectFieldMethodInfo.MakeGenericMethod(typeof(TReturnType));
            }

            return addFieldMethodInfo.InvokeAndHoistBaseException<ExpressionField<TEntity, TReturnType, TExecutionContext>>(this, configurationContextFactory, name, expression, deprecationReason);
        }

        public ExpressionField<TEntity, IEnumerable<TReturnType>, TExecutionContext> AddField<TReturnType>(
            Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory,
            string? name,
            Expression<Func<TEntity, IEnumerable<TReturnType>>> expression,
            string? deprecationReason)
        {
            MethodInfo? addFieldMethodInfo = null;

            var underlyingType = typeof(TReturnType);
            var unwrappedUnderlyingType = underlyingType.UnwrapIfNullable();
            var isNullable = underlyingType != unwrappedUnderlyingType;

            if (underlyingType == typeof(string))
            {
                _addEnumerableStringExpressionFieldMethodInfo ??= ReflectionHelpers.GetMethodInfo<Func<IChainConfigurationContextOwner, IChainConfigurationContext>, string?, Expression<Func<TEntity, IEnumerable<string>>>, string?, ExpressionField<TEntity, IEnumerable<string>, TExecutionContext>>(
                    AddEnumerableExpressionField);

                addFieldMethodInfo = _addEnumerableStringExpressionFieldMethodInfo;
            }
            else if (underlyingType.IsValueType)
            {
                if (isNullable)
                {
                    _addEnumerableNullableExpressionFieldMethodInfo ??= ReflectionHelpers.GetMethodInfo<Func<IChainConfigurationContextOwner, IChainConfigurationContext>, string?, Expression<Func<TEntity, IEnumerable<int?>>>, string?, ExpressionField<TEntity, IEnumerable<int?>, TExecutionContext>>(
                        AddEnumerableExpressionField);
                    addFieldMethodInfo = _addEnumerableNullableExpressionFieldMethodInfo
                        .MakeGenericMethod(unwrappedUnderlyingType);
                }
                else
                {
                    _addEnumerableExpressionFieldMethodInfo ??= ReflectionHelpers.GetMethodInfo<Func<IChainConfigurationContextOwner, IChainConfigurationContext>, string?, Expression<Func<TEntity, IEnumerable<int>>>, string?, ExpressionField<TEntity, IEnumerable<int>, TExecutionContext>>(
                        AddEnumerableExpressionField);

                    addFieldMethodInfo = _addEnumerableExpressionFieldMethodInfo
                        .MakeGenericMethod(underlyingType.UnwrapIfNullable());
                }
            }
            else
            {
                _addEnumerableObjectExpressionFieldMethodInfo ??= ReflectionHelpers.GetMethodInfo<Func<IChainConfigurationContextOwner, IChainConfigurationContext>, string, Expression<Func<TEntity, IEnumerable<object>>>, string?, ExpressionField<TEntity, IEnumerable<object>, TExecutionContext>>(
                    AddEnumerableObjectExpressionField);

                addFieldMethodInfo = _addEnumerableObjectExpressionFieldMethodInfo
                    .MakeGenericMethod(underlyingType);
            }

            return addFieldMethodInfo.InvokeAndHoistBaseException<ExpressionField<TEntity, IEnumerable<TReturnType>, TExecutionContext>>(this, configurationContextFactory, name, expression, deprecationReason);
        }

        public ExpressionField<TEntity, TReturnType, TExecutionContext> AddField<TReturnType>(
            Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory,
            string? name,
            Expression<Func<TExecutionContext, TEntity, TReturnType>> expression,
            string? deprecationReason)
        {
            MethodInfo? addFieldMethodInfo = null;
            if (typeof(TReturnType) == typeof(string))
            {
                _addContextExpressionStringFieldMethodInfo = ReflectionHelpers.GetMethodInfo<Func<IChainConfigurationContextOwner, IChainConfigurationContext>, string, Expression<Func<TExecutionContext, TEntity, string>>, string?, ExpressionField<TEntity, string, TExecutionContext>>(
                    AddExpressionField);

                addFieldMethodInfo = _addContextExpressionStringFieldMethodInfo;
            }
            else if (typeof(TReturnType).IsValueType)
            {
                var unwrappedUnderlyingType = typeof(TReturnType).UnwrapIfNullable();
                var isNullable = typeof(TReturnType) != unwrappedUnderlyingType;

                if (isNullable)
                {
                    _addContextExpressionNullableFieldMethodInfo ??= ReflectionHelpers.GetMethodInfo<Func<IChainConfigurationContextOwner, IChainConfigurationContext>, string, Expression<Func<TExecutionContext, TEntity, int?>>, string?, ExpressionField<TEntity, int?, TExecutionContext>>(
                        AddExpressionField);

                    addFieldMethodInfo = _addContextExpressionNullableFieldMethodInfo.MakeGenericMethod(unwrappedUnderlyingType);
                }
                else
                {
                    _addContextExpressionFieldMethodInfo ??= ReflectionHelpers.GetMethodInfo<Func<IChainConfigurationContextOwner, IChainConfigurationContext>, string, Expression<Func<TExecutionContext, TEntity, int>>, string?, ExpressionField<TEntity, int, TExecutionContext>>(
                        AddExpressionField);

                    addFieldMethodInfo = _addContextExpressionFieldMethodInfo.MakeGenericMethod(unwrappedUnderlyingType);
                }
            }
            else
            {
                _addContextExpressionObjectFieldMethodInfo ??= ReflectionHelpers.GetMethodInfo<Func<IChainConfigurationContextOwner, IChainConfigurationContext>, string, Expression<Func<TExecutionContext, TEntity, object>>, string?, ExpressionField<TEntity, object, TExecutionContext>>(
                    AddObjectExpressionField);

                addFieldMethodInfo = _addContextExpressionObjectFieldMethodInfo.MakeGenericMethod(typeof(TReturnType));
            }

            return addFieldMethodInfo.InvokeAndHoistBaseException<ExpressionField<TEntity, TReturnType, TExecutionContext>>(this, configurationContextFactory, name, expression, deprecationReason);
        }

        public ExpressionField<TEntity, IEnumerable<TReturnType>, TExecutionContext> AddField<TReturnType>(
            Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory,
            string name,
            Expression<Func<TExecutionContext, TEntity, IEnumerable<TReturnType>>> expression,
            string? deprecationReason)
        {
            MethodInfo? addFieldMethodInfo = null;
            var underlyingType = typeof(TReturnType);
            var unwrappedUnderlyingType = underlyingType.UnwrapIfNullable();
            var isNullable = underlyingType != unwrappedUnderlyingType;

            if (underlyingType == typeof(string))
            {
                _addContextEnumerableStringExpressionFieldMethodInfo ??= ReflectionHelpers.GetMethodInfo<Func<IChainConfigurationContextOwner, IChainConfigurationContext>, string, Expression<Func<TExecutionContext, TEntity, IEnumerable<string>>>, string?, ExpressionField<TEntity, IEnumerable<string>, TExecutionContext>>(
                    AddEnumerableExpressionField);

                addFieldMethodInfo = _addContextEnumerableStringExpressionFieldMethodInfo;
            }
            else if (underlyingType.IsValueType)
            {
                if (isNullable)
                {
                    _addContextEnumerableNullableExpressionFieldMethodInfo ??= ReflectionHelpers.GetMethodInfo<Func<IChainConfigurationContextOwner, IChainConfigurationContext>, string, Expression<Func<TExecutionContext, TEntity, IEnumerable<int?>>>, string?, ExpressionField<TEntity, IEnumerable<int?>, TExecutionContext>>(
                        AddEnumerableExpressionField);
                    addFieldMethodInfo = _addContextEnumerableNullableExpressionFieldMethodInfo
                        .MakeGenericMethod(unwrappedUnderlyingType);
                }
                else
                {
                    _addContextEnumerableExpressionFieldMethodInfo ??= ReflectionHelpers.GetMethodInfo<Func<IChainConfigurationContextOwner, IChainConfigurationContext>, string, Expression<Func<TExecutionContext, TEntity, IEnumerable<int>>>, string?, ExpressionField<TEntity, IEnumerable<int>, TExecutionContext>>(
                        AddEnumerableExpressionField);

                    addFieldMethodInfo = _addContextEnumerableExpressionFieldMethodInfo
                        .MakeGenericMethod(underlyingType.UnwrapIfNullable());
                }
            }
            else
            {
                _addContextEnumerableObjectExpressionFieldMethodInfo ??= ReflectionHelpers.GetMethodInfo<Func<IChainConfigurationContextOwner, IChainConfigurationContext>, string, Expression<Func<TExecutionContext, TEntity, IEnumerable<object>>>, string?, ExpressionField<TEntity, IEnumerable<object>, TExecutionContext>>(
                    AddEnumerableObjectExpressionField);

                addFieldMethodInfo = _addContextEnumerableObjectExpressionFieldMethodInfo
                    .MakeGenericMethod(underlyingType);
            }

            return addFieldMethodInfo.InvokeAndHoistBaseException<ExpressionField<TEntity, IEnumerable<TReturnType>, TExecutionContext>>(this, configurationContextFactory, name, expression, deprecationReason);
        }

        public SubmitField<TEntity, TExecutionContext> AddSubmitField(
            string name,
            IGraphTypeDescriptor<TExecutionContext> returnGraphType,
            string argName,
            IInputObjectGraphType graphType,
            Func<IResolveFieldContext, Dictionary<string, object>, ValueTask<object?>> resolve,
            Type fieldType,
            string? deprecationReason = null)
        {
            var field = new SubmitField<TEntity, TExecutionContext>(
                owner => ConfigurationContext.Chain(owner, "NoOp"),
                this,
                name,
                returnGraphType,
                argName,
                graphType,
                resolve,
                fieldType);
            return InternalAddField(field, deprecationReason);
        }

        public void Filter<TValueType>(string name, Func<TValueType, Expression<Func<TEntity, bool>>> filterPredicateFactory)
        {
            Guards.ThrowIfNullOrEmpty(name, nameof(name));

            _inlineFilters.Add(new CustomInlineFilter<TEntity, TValueType, TExecutionContext>(
                owner => ConfigurationContext
                    .Chain<TValueType>(owner, nameof(Filter))
                    .Argument(name)
                    .Argument(filterPredicateFactory),
                name,
                filterPredicateFactory));
        }

        public void Filter<TValueType>(string name, Func<TExecutionContext, TValueType, Expression<Func<TEntity, bool>>> filterPredicateFactory)
        {
            Guards.ThrowIfNullOrEmpty(name, nameof(name));

            _inlineFilters.Add(new CustomInlineFilter<TEntity, TValueType, TExecutionContext>(
                owner => ConfigurationContext
                    .Chain<TValueType>(owner, nameof(Filter))
                    .Argument(name)
                    .Argument(filterPredicateFactory),
                name,
                filterPredicateFactory));
        }

        public void Sorter<TValueType>(string name, Expression<Func<TEntity, TValueType>> selector)
        {
            Guards.ThrowIfNullOrEmpty(name, nameof(name));

            _sorters.Add(new CustomSorter<TEntity, TValueType, TExecutionContext>(
                owner => ConfigurationContext
                    .Chain(owner, nameof(Sorter))
                    .Argument(name)
                    .Argument(selector),
                name,
                selector));
        }

        public void Sorter<TValueType>(string name, Func<TExecutionContext, Expression<Func<TEntity, TValueType>>> selectorFactory)
        {
            Guards.ThrowIfNullOrEmpty(name, nameof(name));

            _sorters.Add(new CustomSorter<TEntity, TValueType, TExecutionContext>(
                owner => ConfigurationContext
                    .Chain(owner, nameof(Sorter))
                    .Argument(name)
                    .Argument(selectorFactory),
                name,
                selectorFactory));
        }

        public void Sorter(ISorter<TExecutionContext> sorter)
        {
            _sorters.Add(sorter);
        }

        public void OnEntityLoaded<T>(Expression<Func<TEntity, T>> proxyExpression, Action<TExecutionContext, T> hook)
        {
            ProxyAccessor.AddLoadHook(
                owner => ConfigurationContext
                    .Chain(owner, nameof(OnEntityLoaded))
                    .Argument(proxyExpression)
                    .Argument(hook),
                proxyExpression,
                hook);
        }

        public void OnEntityLoaded<TKey, T>(
            Expression<Func<TEntity, TKey>> keyExpression,
            Func<TExecutionContext, IEnumerable<TKey>, IDictionary<TKey, T>> batchFunc,
            Action<TExecutionContext, T> hook)
        {
            ProxyAccessor.AddLoadHook(
                owner => ConfigurationContext.Chain(owner, nameof(OnEntityLoaded))
                    .Argument(keyExpression)
                    .Argument(batchFunc)
                    .Argument(hook),
                keyExpression,
                batchFunc,
                hook);
        }

        public void OnEntityLoaded<TKey, T>(
            Expression<Func<TEntity, TKey>> keyExpression,
            Func<TExecutionContext, IEnumerable<TKey>, Task<IDictionary<TKey, T>>> batchFunc,
            Action<TExecutionContext, T> hook)
        {
            ProxyAccessor.AddLoadHook(
                owner => ConfigurationContext.Chain(owner, nameof(OnEntityLoaded))
                    .Argument(keyExpression)
                    .Argument(batchFunc)
                    .Argument(hook),
                keyExpression,
                batchFunc,
                hook);
        }

        public IInlineFilters<TEntity, TExecutionContext> CreateInlineFilters()
        {
            if (_filters == null)
            {
                var baseConfigurator = GetBaseObjectGraphTypeConfiguratorForFilters();

                _filters = baseConfigurator != null
                    ? baseConfigurator.CreateInlineFilters()
                    : new InlineFilters<TEntity, TExecutionContext>(
                        InlineFilters,
                        $"{Name}Filter");
            }

            return _filters;
        }

        IInlineFilters<TExecutionContext> IObjectGraphTypeConfigurator<TExecutionContext>.CreateInlineFilters() => CreateInlineFilters();

        public abstract void ConfigureGraphType(IComplexGraphType graphType);

        public virtual void ConfigureGroupGraphType(IObjectGraphType graphType)
        {
            // TODO should be generated by RelationRegistry
            graphType.Name = $"{Name}Grouping";

            foreach (var field in _fields
                .OfType<IExpressionFieldConfiguration<TEntity, TExecutionContext>>()
                .Where(IsFieldGroupable))
            {
                graphType.AddField(field.AsFieldType());
            }
        }

        public override int GetHashCode()
        {
            var res = default(HashCode);

            res.Add(_fields, CollectionEqualityComparer<IField<TEntity, TExecutionContext>>.Default);
            res.Add(typeof(TEntity));
            res.Add(typeof(TExecutionContext));
            return res.ToHashCode();
        }

        public bool Equals(BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> other)
        {
            if (other == null)
            {
                return false;
            }

            return _fields.SequenceEqual(other._fields);
        }

        public bool FilterEquals(BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext>? other)
        {
            if (other == null)
            {
                return false;
            }

            return _fields
                .OfType<IExpressionFieldConfiguration<TEntity, TExecutionContext>>()
                .Where(field => field.IsFilterable).SequenceEqual(other._fields.OfType<IExpressionFieldConfiguration<TEntity, TExecutionContext>>().Where(field => field.IsFilterable))
                && _inlineFilters.SequenceEqual(other._inlineFilters);
        }

        bool IObjectGraphTypeConfigurator<TExecutionContext>.FilterEquals(IObjectGraphTypeConfigurator<TExecutionContext> other) => FilterEquals(other as BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext>);

        public override bool Equals(object obj)
        {
            if (obj is BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> other)
            {
                return Equals(other);
            }

            return false;
        }

        public void Configure()
        {
            if (!isConfigured)
            {
                _fields.Clear();
                _inlineFilters.Clear();
                _sorters.Clear();
                ConfigurationContext.Clear();

                OnConfigure();
                isConfigured = true;

                // ValidateFields();
            }
        }

        public abstract IGraphTypeDescriptor<TReturnType, TExecutionContext> GetGraphQLTypeDescriptor<TReturnType>(
            IField<TExecutionContext> parent,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build,
            IChainConfigurationContext configurationContext);

        public abstract string GetGraphQLTypeName(Type entityType, Type? projectionType, IField<TExecutionContext> field);

        public BatchField<TEntity, TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var graphType = GetGraphQLTypeDescriptor(field, build, configurationContext);
            var result = new BatchField<TEntity, TEntity, TReturnType, TExecutionContext>(
                configurationContext,
                this,
                field.Name,
                FuncConstants<TEntity>.IdentityExpression,
                batchFunc,
                graphType);
            return ReplaceField(field, result);
        }

        public BatchEnumerableField<TEntity, TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var graphType = GetGraphQLTypeDescriptor(field, build, configurationContext);
            var result = new BatchEnumerableField<TEntity, TEntity, TReturnType, TExecutionContext>(
                configurationContext,
                this,
                field.Name,
                FuncConstants<TEntity>.IdentityExpression,
                batchFunc,
                graphType);
            return ReplaceField(field, result);
        }

        public BatchField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var graphType = GetGraphQLTypeDescriptor(field, build, configurationContext);
            var result = new BatchField<TEntity, TKeyType, TReturnType, TExecutionContext>(configurationContext, this, field.Name, keySelector, batchFunc, graphType);
            return ReplaceField(field, result);
        }

        public BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var graphType = GetGraphQLTypeDescriptor(field, build, configurationContext);
            var result = new BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext>(configurationContext, this, field.Name, keySelector, batchFunc, graphType);
            return ReplaceField(field, result);
        }

        public BatchField<TEntity, TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            Func<IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return FromBatch(configurationContext, field, Registry.WrapFuncByUnusedContext(batchFunc), build);
        }

        public BatchEnumerableField<TEntity, TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            Func<IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return FromBatch(configurationContext, field, Registry.WrapFuncByUnusedContext(batchFunc), build);
        }

        public BatchField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return FromBatch(configurationContext, field, keySelector, Registry.WrapFuncByUnusedContext(batchFunc), build);
        }

        public BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return FromBatch(configurationContext, field, keySelector, Registry.WrapFuncByUnusedContext(batchFunc), build);
        }

        public BatchField<TEntity, TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var graphType = GetGraphQLTypeDescriptor(field, build, configurationContext);
            var result = new BatchField<TEntity, TEntity, TReturnType, TExecutionContext>(
                configurationContext,
                this,
                field.Name,
                FuncConstants<TEntity>.IdentityExpression,
                batchFunc,
                graphType);
            return ReplaceField(field, result);
        }

        public BatchEnumerableField<TEntity, TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var graphType = GetGraphQLTypeDescriptor(field, build, configurationContext);
            var result = new BatchEnumerableField<TEntity, TEntity, TReturnType, TExecutionContext>(
                configurationContext,
                this,
                field.Name,
                FuncConstants<TEntity>.IdentityExpression,
                batchFunc,
                graphType);
            return ReplaceField(field, result);
        }

        public BatchField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var graphType = GetGraphQLTypeDescriptor(field, build, configurationContext);
            var result = new BatchField<TEntity, TKeyType, TReturnType, TExecutionContext>(
                configurationContext,
                this,
                field.Name,
                keySelector,
                batchFunc,
                graphType);
            return ReplaceField(field, result);
        }

        public BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var graphType = GetGraphQLTypeDescriptor(field, build, configurationContext);
            var result = new BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext>(
                configurationContext,
                this,
                field.Name,
                keySelector,
                batchFunc,
                graphType);
            return ReplaceField(field, result);
        }

        public BatchField<TEntity, TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return FromBatch(configurationContext, field, Registry.WrapFuncByUnusedContext(batchFunc), build);
        }

        public BatchEnumerableField<TEntity, TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return FromBatch(configurationContext, field, Registry.WrapFuncByUnusedContext(batchFunc), build);
        }

        public BatchField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return FromBatch(configurationContext, field, keySelector, Registry.WrapFuncByUnusedContext(batchFunc), build);
        }

        public BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return FromBatch(configurationContext, field, keySelector, Registry.WrapFuncByUnusedContext(batchFunc), build);
        }

        public LoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext> FromLoader<TLoader, TChildLoader, TChildEntity>(
            IChainConfigurationContext configurationContext,
            Field<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            RelationType relationType,
            Expression<Func<TChildEntity, TEntity>>? navigationProperty,
            Expression<Func<TEntity, TChildEntity>>? reverseNavigationProperty,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> graphResultType)
            where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
            where TLoader : Loader<TEntity, TExecutionContext>, new()
        {
            var factorizationResult = ExpressionHelpers.FactorizeCondition(condition);
            var equality = Expression.Equal(factorizationResult.LeftExpression.Body, factorizationResult.RightExpression.Body);
            var lambda = Expression.Lambda<Func<TEntity, TChildEntity, bool>>(equality, condition.Parameters);

            var result = ReplaceField(field, new LoaderField<TLoader, TChildLoader, TEntity, TChildEntity, TExecutionContext>(
                configurationContext,
                this,
                field.Name,
                lambda,
                relationType,
                navigationProperty,
                reverseNavigationProperty,
                graphResultType));

            if (factorizationResult.RightCondition != null)
            {
                return result.ApplyWhere(result.ConfigurationContext, factorizationResult.RightCondition);
            }

            return result;
        }

        public SelectField<TEntity, TReturnType, TExecutionContext> ApplySelect<TReturnType>(
            IChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            IFieldResolver resolver,
            IGraphTypeDescriptor<TExecutionContext> graphType)
        {
            return ReplaceField(field, new SelectField<TEntity, TReturnType, TExecutionContext>(configurationContext, this, field.Name, resolver, graphType));
        }

        public SelectField<TEntity, TReturnType, TExecutionContext> ApplySelect<TReturnType>(
            IInlinedChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            IFieldResolver resolver,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var graphType = GetGraphQLTypeDescriptor(field, build, configurationContext);
            return ReplaceField(field, new SelectField<TEntity, TReturnType, TExecutionContext>(
                configurationContext,
                this,
                field.Name,
                resolver,
                graphType));
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TFromType, TAnotherReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            IBatchResolver<TEntity, TFromType> firstResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
        {
            var batchResolver = new BatchKeyResolver<TEntity, TEntity, TAnotherReturnType, TExecutionContext>(configurationContext, field.Name, FuncConstants<TEntity>.IdentityExpression, batchFunc, ProxyAccessor);

            return ApplyBatchUnion(configurationContext, field, firstResolver, firstGraphType, batchResolver, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TFromType, TAnotherReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            IBatchResolver<TEntity, TFromType> firstResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            Func<IEnumerable<TEntity>, IDictionary<TEntity, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
        {
            var batchResolver = new BatchKeyResolver<TEntity, TEntity, TAnotherReturnType, TExecutionContext>(configurationContext, field.Name, FuncConstants<TEntity>.IdentityExpression, (ctx, e) => batchFunc(e), ProxyAccessor);

            return ApplyBatchUnion(configurationContext, field, firstResolver, firstGraphType, batchResolver, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TFromType, TKeyType, TAnotherReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            IBatchResolver<TEntity, TFromType> firstResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
        {
            var batchResolver = new BatchKeyResolver<TEntity, TKeyType, TAnotherReturnType, TExecutionContext>(configurationContext, field.Name, keySelector, batchFunc, ProxyAccessor);

            return ApplyBatchUnion(configurationContext, field, firstResolver, firstGraphType, batchResolver, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TFromType, TKeyType, TAnotherReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            IBatchResolver<TEntity, TFromType> firstResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, IDictionary<TKeyType, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
        {
            var batchResolver = new BatchKeyResolver<TEntity, TKeyType, TAnotherReturnType, TExecutionContext>(configurationContext, field.Name, keySelector, (ctx, items) => batchFunc(items), ProxyAccessor);

            return ApplyBatchUnion(configurationContext, field, firstResolver, firstGraphType, batchResolver, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TFromType, TAnotherReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            IBatchResolver<TEntity, TFromType> firstResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
        {
            var batchResolver = new BatchTaskKeyResolver<TEntity, TEntity, TAnotherReturnType, TExecutionContext>(configurationContext, field.Name, FuncConstants<TEntity>.IdentityExpression, batchFunc, ProxyAccessor);

            return ApplyBatchUnion(configurationContext, field, firstResolver, firstGraphType, batchResolver, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TFromType, TAnotherReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            IBatchResolver<TEntity, TFromType> firstResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
        {
            var batchResolver = new BatchTaskKeyResolver<TEntity, TEntity, TAnotherReturnType, TExecutionContext>(configurationContext, field.Name, FuncConstants<TEntity>.IdentityExpression, (ctx, e) => batchFunc(e), ProxyAccessor);

            return ApplyBatchUnion(configurationContext, field, firstResolver, firstGraphType, batchResolver, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TFromType, TKeyType, TAnotherReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            IBatchResolver<TEntity, TFromType> firstResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
        {
            var batchResolver = new BatchTaskKeyResolver<TEntity, TKeyType, TAnotherReturnType, TExecutionContext>(configurationContext, field.Name, keySelector, batchFunc, ProxyAccessor);

            return ApplyBatchUnion(configurationContext, field, firstResolver, firstGraphType, batchResolver, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TFromType, TKeyType, TAnotherReturnType>(
            IInlinedResolvedChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            IBatchResolver<TEntity, TFromType> firstResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
        {
            var batchResolver = new BatchTaskKeyResolver<TEntity, TKeyType, TAnotherReturnType, TExecutionContext>(configurationContext, field.Name, keySelector, (ctx, items) => batchFunc(items), ProxyAccessor);

            return ApplyBatchUnion(configurationContext, field, firstResolver, firstGraphType, batchResolver, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TFromType, TAnotherReturnType>(
            IInlinedChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            IBatchResolver<TEntity, TFromType> firstResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            IBatchResolver<TEntity, TAnotherReturnType> secondResolver,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
        {
            var secondGraphType = GetGraphQLTypeDescriptor(field, build, configurationContext);

            return ReplaceField(field, new BatchUnionField<TEntity, TExecutionContext>(
                configurationContext,
                this,
                field.Name,
                firstResolver,
                secondResolver,
                firstGraphType,
                typeof(TFromType),
                secondGraphType,
                typeof(TAnotherReturnType)));
        }

        public IVoid ApplyResolvedField<TReturnType>(
            IChainConfigurationContext configurationContext,
            FieldBase<TEntity, TExecutionContext> field,
            IGraphTypeDescriptor<TExecutionContext> graphType,
            IFieldResolver resolver)
        {
            var resolvedField = new ResolvedField<TEntity, TReturnType, TExecutionContext>(
                configurationContext,
                this,
                field.Name,
                graphType,
                resolver,
                field.Arguments);

            return ReplaceField(field, resolvedField);
        }

        public void ValidateFields()
        {
            DoValidateFields();
            ConfigurationContext.ThrowErrors();
        }

        internal TField AddField<TField>(TField field, string? deprecationReason)
            where TField : FieldBase<TEntity, TExecutionContext> => InternalAddField(field, deprecationReason);

        protected virtual void OnConfigure()
        {
        }

        protected virtual void SetGraphQLTypeName(string? oldName, string newName)
        {
            Registry.SetGraphQLTypeName<TEntity>(oldName, newName);
        }

        protected virtual IObjectGraphTypeConfigurator<TEntity, TExecutionContext>? GetBaseObjectGraphTypeConfiguratorForFilters() => null;

        protected virtual bool IsFieldGroupable(IExpressionFieldConfiguration<TEntity, TExecutionContext> field)
        {
            return field.IsGroupable;
        }

        private protected virtual void DoValidateFields()
        {
            if (!Fields.Any())
            {
                ConfigurationContext.AddError(
                    $"{(IsAuto ? $"Type `{typeof(TEntity).HumanizedName()}`" : "OnConfigure() method")} must have a declaration of one field at least.",
                    ConfigurationContext.Parent == null ? Array.Empty<IConfigurationContext>() : new[] { ConfigurationContext.Parent });
            }

            // Find dulicates and add errors for them
            foreach (var group in Fields
                .GroupBy(field => field.Name)
                .Where(group => group.Count() > 1))
            {
                ConfigurationContext.AddError(
                    $"A field with the name `{group.Key}` is already registered.",
                    group.Select(field => field.ConfigurationContext).ToArray());
            }

            foreach (var group in InlineFilters
                .GroupBy(filter => filter.FieldName)
                .Where(group => group.Count() > 1))
            {
                ConfigurationContext.AddError(
                    $"A filter for field with the name `{group.Key}` is already registered.",
                    group.Select(filter => filter.ConfigurationContext).ToArray());
            }

            foreach (var group in Sorters
                .GroupBy(sorter => sorter.Name)
                .Where(group => group.Count() > 1))
            {
                ConfigurationContext.AddError(
                    $"A sorter with the name `{group.Key}` is already registered.",
                    group.Select(sorter => sorter.ConfigurationContext).ToArray());
            }

            if (!IsAuto)
            {
                Fields.ForEach(f => f.Validate());
            }
        }

        private IField<TExecutionContext> AddField(PropertyInfo propertyInfo)
        {
            if (propertyInfo.PropertyType != typeof(string) && propertyInfo.PropertyType.IsEnumerableType())
            {
                _addEnumerableFieldMethodInfo ??= ReflectionHelpers.GetMethodInfo<Func<IChainConfigurationContextOwner, IChainConfigurationContext>, string, Expression<Func<TEntity, IEnumerable<object>>>, string?, ExpressionField<TEntity, IEnumerable<object>, TExecutionContext>>(
                    AddField<object>);

                var itemPropertyType = TypeExtensions.GetEnumerableElementType(propertyInfo.PropertyType);
                var enumerablePropertyType = typeof(IEnumerable<>).MakeGenericType(itemPropertyType);
                var enumerableExpression = propertyInfo.MakePropertyLambdaExpression(enumerablePropertyType);

                Func<IChainConfigurationContextOwner, IChainConfigurationContext> enumerableConfigurationContextFactory = owner =>
                    ConfigurationContext.Chain(owner, nameof(Field))
                        .Argument(propertyInfo.Name)
                        .Argument(enumerableExpression);

                return _addEnumerableFieldMethodInfo.MakeGenericMethod(itemPropertyType).InvokeAndHoistBaseException<IField<TExecutionContext>>(
                    this,
                    enumerableConfigurationContextFactory,
                    propertyInfo.Name,
                    enumerableExpression,
                    null);
            }

            _addFieldMethodInfo ??= ReflectionHelpers.GetMethodInfo<Func<IChainConfigurationContextOwner, IChainConfigurationContext>, string, Expression<Func<TEntity, object>>, string?, ExpressionField<TEntity, object, TExecutionContext>>(
                AddField<object>);

            var propertyType = propertyInfo.PropertyType;
            var expressionType = typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(typeof(TEntity), propertyType));
            var expression = propertyInfo.MakePropertyLambdaExpression();

            Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory = owner =>
                ConfigurationContext.Chain(owner, nameof(Field))
                    .Argument(propertyInfo.Name)
                    .Argument(expression);

            return _addFieldMethodInfo.MakeGenericMethod(propertyType).InvokeAndHoistBaseException<IField<TExecutionContext>>(
                this,
                configurationContextFactory,
                propertyInfo.Name,
                expression,
                null);
        }

        private TField InternalAddField<TField>(TField field, string? deprecationReason)
            where TField : FieldBase<TEntity, TExecutionContext>
        {
            _fields.Add(field);
            field.DeprecationReason = deprecationReason;
            return field;
        }

        private StructExpressionField<TEntity, TReturnType, TExecutionContext> AddExpressionField<TReturnType>(
            Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory,
            string? name,
            Expression<Func<TEntity, TReturnType>> expression,
            string? deprecationReason)
            where TReturnType : struct => InternalAddField(new StructExpressionField<TEntity, TReturnType, TExecutionContext>(configurationContextFactory, this, expression, name), deprecationReason);

        private StructExpressionField<TEntity, TReturnType, TExecutionContext> AddExpressionField<TReturnType>(
            Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory,
            string name,
            Expression<Func<TExecutionContext, TEntity, TReturnType>> expression,
            string? deprecationReason)
            where TReturnType : struct => InternalAddField(new StructExpressionField<TEntity, TReturnType, TExecutionContext>(configurationContextFactory, this, expression, name), deprecationReason);

        private NullableExpressionField<TEntity, TReturnType, TExecutionContext> AddExpressionField<TReturnType>(
            Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory,
            string? name,
            Expression<Func<TEntity, TReturnType?>> expression,
            string? deprecationReason)
            where TReturnType : struct => InternalAddField(new NullableExpressionField<TEntity, TReturnType, TExecutionContext>(configurationContextFactory, this, expression, name), deprecationReason);

        private NullableExpressionField<TEntity, TReturnType, TExecutionContext> AddExpressionField<TReturnType>(
            Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory,
            string name,
            Expression<Func<TExecutionContext, TEntity, TReturnType?>> expression,
            string? deprecationReason)
            where TReturnType : struct => InternalAddField(new NullableExpressionField<TEntity, TReturnType, TExecutionContext>(configurationContextFactory, this, expression, name), deprecationReason);

        private StringExpressionField<TEntity, TExecutionContext> AddExpressionField(
            Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory,
            string? name,
            Expression<Func<TEntity, string>> expression,
            string? deprecationReason)
            => InternalAddField(new StringExpressionField<TEntity, TExecutionContext>(configurationContextFactory, this, expression, name), deprecationReason);

        private StringExpressionField<TEntity, TExecutionContext> AddExpressionField(
            Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory,
            string name,
            Expression<Func<TExecutionContext, TEntity, string>> expression,
            string? deprecationReason)
            => InternalAddField(new StringExpressionField<TEntity, TExecutionContext>(configurationContextFactory, this, expression, name), deprecationReason);

        private ExpressionField<TEntity, IEnumerable<TReturnType>, TExecutionContext> AddEnumerableExpressionField<TReturnType>(
            Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory,
            string? name,
            Expression<Func<TEntity, IEnumerable<TReturnType>>> expression,
            string? deprecationReason)
            where TReturnType : struct => InternalAddField(new ExpressionField<TEntity, IEnumerable<TReturnType>, TExecutionContext>(configurationContextFactory, this, expression, name), deprecationReason);

        private ExpressionField<TEntity, IEnumerable<TReturnType>, TExecutionContext> AddEnumerableExpressionField<TReturnType>(
            Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory,
            string name,
            Expression<Func<TExecutionContext, TEntity, IEnumerable<TReturnType>>> expression,
            string? deprecationReason)
            where TReturnType : struct => InternalAddField(new ExpressionField<TEntity, IEnumerable<TReturnType>, TExecutionContext>(configurationContextFactory, this, expression, name), deprecationReason);

        private ExpressionField<TEntity, IEnumerable<TReturnType?>, TExecutionContext> AddEnumerableExpressionField<TReturnType>(
            Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory,
            string? name,
            Expression<Func<TEntity, IEnumerable<TReturnType?>>> expression,
            string? deprecationReason)
            where TReturnType : struct => InternalAddField(new ExpressionField<TEntity, IEnumerable<TReturnType?>, TExecutionContext>(configurationContextFactory, this, expression, name), deprecationReason);

        private ExpressionField<TEntity, IEnumerable<TReturnType?>, TExecutionContext> AddEnumerableExpressionField<TReturnType>(
            Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory,
            string name,
            Expression<Func<TExecutionContext, TEntity, IEnumerable<TReturnType?>>> expression,
            string? deprecationReason)
            where TReturnType : struct => InternalAddField(new ExpressionField<TEntity, IEnumerable<TReturnType?>, TExecutionContext>(configurationContextFactory, this, expression, name), deprecationReason);

        private ExpressionField<TEntity, IEnumerable<string>, TExecutionContext> AddEnumerableExpressionField(
            Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory,
            string? name,
            Expression<Func<TEntity, IEnumerable<string>>> expression,
            string? deprecationReason)
            => InternalAddField(new ExpressionField<TEntity, IEnumerable<string>, TExecutionContext>(configurationContextFactory, this, expression, name), deprecationReason);

        private ExpressionField<TEntity, IEnumerable<string>, TExecutionContext> AddEnumerableExpressionField(
            Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory,
            string name,
            Expression<Func<TExecutionContext, TEntity, IEnumerable<string>>> expression,
            string? deprecationReason)
            => InternalAddField(new ExpressionField<TEntity, IEnumerable<string>, TExecutionContext>(configurationContextFactory, this, expression, name), deprecationReason);

        private ExpressionField<TEntity, TReturnType, TExecutionContext> AddObjectExpressionField<TReturnType>(
            Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory,
            string? name,
            Expression<Func<TEntity, TReturnType>> expression,
            string? deprecationReason)
           where TReturnType : class => InternalAddField(new ExpressionField<TEntity, TReturnType, TExecutionContext>(configurationContextFactory, this, expression, name), deprecationReason);

        private ExpressionField<TEntity, TReturnType, TExecutionContext> AddObjectExpressionField<TReturnType>(
            Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory,
            string name,
            Expression<Func<TExecutionContext, TEntity, TReturnType>> expression,
            string? deprecationReason)
           where TReturnType : class => InternalAddField(new ExpressionField<TEntity, TReturnType, TExecutionContext>(configurationContextFactory, this, expression, name), deprecationReason);

        private ExpressionField<TEntity, IEnumerable<TReturnType>, TExecutionContext> AddEnumerableObjectExpressionField<TReturnType>(
            Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory,
            string name,
            Expression<Func<TEntity, IEnumerable<TReturnType>>> expression,
            string? deprecationReason)
           where TReturnType : class => InternalAddField(new ExpressionField<TEntity, IEnumerable<TReturnType>, TExecutionContext>(configurationContextFactory, this, expression, name), deprecationReason);

        private ExpressionField<TEntity, IEnumerable<TReturnType>, TExecutionContext> AddEnumerableObjectExpressionField<TReturnType>(
            Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory,
            string name,
            Expression<Func<TExecutionContext, TEntity, IEnumerable<TReturnType>>> expression,
            string? deprecationReason)
           where TReturnType : class => InternalAddField(new ExpressionField<TEntity, IEnumerable<TReturnType>, TExecutionContext>(configurationContextFactory, this, expression, name), deprecationReason);
    }
}
