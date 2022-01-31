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
    internal abstract class BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> : IObjectGraphTypeConfigurator<TEntity, TExecutionContext>, IEquatable<BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext>>
#pragma warning restore CA1506
        where TEntity : class
    {
        private static MethodInfo? _addEnumerableStringExpressionFieldMethodInfo;
        private static MethodInfo? _addEnumerableExpressionFieldMethodInfo;
        private static MethodInfo? _addEnumerableNullableExpressionFieldMethodInfo;
        private static MethodInfo? _addEnumerableObjectExpressionFieldMethodInfo;
        private static MethodInfo? _addExpressionFieldMethodInfo;
        private static MethodInfo? _addExpressionNullableFieldMethodInfo;
        private static MethodInfo? _addExpressionObjectFieldMethodInfo;
        private static MethodInfo? _addExpressionStringFieldMethodInfo;
        private readonly List<IField<TEntity, TExecutionContext>> _fields = new();
        private readonly List<IInlineFilter<TExecutionContext>> _inlineFilters = new();
        private readonly List<ISorter<TExecutionContext>> _sorters = new();
        private IInlineFilters<TEntity, TExecutionContext>? _filters;

        private string? _name;

        protected BaseObjectGraphTypeConfigurator(IField<TExecutionContext>? parent, IRegistry<TExecutionContext> registry, bool isAuto)
        {
            Registry = registry;
            Parent = parent;
            ProxyAccessor = new ProxyAccessor<TEntity, TExecutionContext>(this);

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

        public bool HasInlineFilters => _inlineFilters.Any()
            || _fields
                .OfType<IExpressionField<TEntity, TExecutionContext>>()
                .Any(f => f.IsFilterable);

        public IReadOnlyList<IField<TEntity, TExecutionContext>> Fields => _fields;

        public ProxyAccessor<TEntity, TExecutionContext> ProxyAccessor { get; }

        IProxyAccessor<TEntity, TExecutionContext> IObjectGraphTypeConfigurator<TEntity, TExecutionContext>.ProxyAccessor => ProxyAccessor;

        IProxyAccessor<TExecutionContext> IObjectGraphTypeConfigurator<TExecutionContext>.ProxyAccessor => ProxyAccessor;

        public IField<TExecutionContext>? Parent { get; }

        public IReadOnlyList<ISorter<TExecutionContext>> Sorters => _sorters;

        public IRegistry<TExecutionContext> Registry { get; }

        public TField ReplaceField<TField>(FieldBase<TEntity, TExecutionContext> oldField, TField newField)
            where TField : FieldBase<TEntity, TExecutionContext>
        {
            var index = _fields.IndexOf(oldField);
            if (index >= 0)
            {
                _fields.RemoveAt(index);
                _fields.Insert(index, newField);

                ProxyAccessor.ReplaceField(oldField, newField);
            }

            return newField;
        }

        public Field<TEntity, TExecutionContext> AddField(string name, string? deprecationReason)
        {
            var field = new Field<TEntity, TExecutionContext>(this, name);
            return InternalAddField(field, deprecationReason);
        }

        public StructExpressionField<TEntity, TReturnType, TExecutionContext> AddExpressionField<TReturnType>(string? name, Expression<Func<TEntity, TReturnType>> expression, string? deprecationReason)
            where TReturnType : struct => InternalAddField(new StructExpressionField<TEntity, TReturnType, TExecutionContext>(this, expression, name), deprecationReason);

        public StructExpressionField<TEntity, TReturnType, TExecutionContext> AddExpressionField<TReturnType>(string name, Expression<Func<TExecutionContext, TEntity, TReturnType>> expression, string? deprecationReason)
            where TReturnType : struct => InternalAddField(new StructExpressionField<TEntity, TReturnType, TExecutionContext>(this, expression, name), deprecationReason);

        public NullableExpressionField<TEntity, TReturnType, TExecutionContext> AddExpressionField<TReturnType>(string? name, Expression<Func<TEntity, TReturnType?>> expression, string? deprecationReason)
            where TReturnType : struct => InternalAddField(new NullableExpressionField<TEntity, TReturnType, TExecutionContext>(this, expression, name), deprecationReason);

        public NullableExpressionField<TEntity, TReturnType, TExecutionContext> AddExpressionField<TReturnType>(string name, Expression<Func<TExecutionContext, TEntity, TReturnType?>> expression, string? deprecationReason)
            where TReturnType : struct => InternalAddField(new NullableExpressionField<TEntity, TReturnType, TExecutionContext>(this, expression, name), deprecationReason);

        public StringExpressionField<TEntity, TExecutionContext> AddExpressionField(string? name, Expression<Func<TEntity, string>> expression, string? deprecationReason)
            => InternalAddField(new StringExpressionField<TEntity, TExecutionContext>(this, expression, name), deprecationReason);

        public StringExpressionField<TEntity, TExecutionContext> AddExpressionField(string name, Expression<Func<TExecutionContext, TEntity, string>> expression, string? deprecationReason)
            => InternalAddField(new StringExpressionField<TEntity, TExecutionContext>(this, expression, name), deprecationReason);

        public ExpressionField<TEntity, IEnumerable<TReturnType>, TExecutionContext> AddEnumerableExpressionField<TReturnType>(string? name, Expression<Func<TEntity, IEnumerable<TReturnType>>> expression, string? deprecationReason)
            where TReturnType : struct => InternalAddField(new ExpressionField<TEntity, IEnumerable<TReturnType>, TExecutionContext>(this, expression, name), deprecationReason);

        public ExpressionField<TEntity, IEnumerable<TReturnType>, TExecutionContext> AddEnumerableExpressionField<TReturnType>(string name, Expression<Func<TExecutionContext, TEntity, IEnumerable<TReturnType>>> expression, string? deprecationReason)
            where TReturnType : struct => InternalAddField(new ExpressionField<TEntity, IEnumerable<TReturnType>, TExecutionContext>(this, expression, name), deprecationReason);

        public ExpressionField<TEntity, IEnumerable<TReturnType?>, TExecutionContext> AddEnumerableExpressionField<TReturnType>(string? name, Expression<Func<TEntity, IEnumerable<TReturnType?>>> expression, string? deprecationReason)
            where TReturnType : struct => InternalAddField(new ExpressionField<TEntity, IEnumerable<TReturnType?>, TExecutionContext>(this, expression, name), deprecationReason);

        public ExpressionField<TEntity, IEnumerable<TReturnType?>, TExecutionContext> AddEnumerableExpressionField<TReturnType>(string name, Expression<Func<TExecutionContext, TEntity, IEnumerable<TReturnType?>>> expression, string? deprecationReason)
            where TReturnType : struct => InternalAddField(new ExpressionField<TEntity, IEnumerable<TReturnType?>, TExecutionContext>(this, expression, name), deprecationReason);

        public ExpressionField<TEntity, IEnumerable<string>, TExecutionContext> AddEnumerableExpressionField(string? name, Expression<Func<TEntity, IEnumerable<string>>> expression, string? deprecationReason)
            => InternalAddField(new ExpressionField<TEntity, IEnumerable<string>, TExecutionContext>(this, expression, name), deprecationReason);

        public ExpressionField<TEntity, IEnumerable<string>, TExecutionContext> AddEnumerableExpressionField(string name, Expression<Func<TExecutionContext, TEntity, IEnumerable<string>>> expression, string? deprecationReason)
            => InternalAddField(new ExpressionField<TEntity, IEnumerable<string>, TExecutionContext>(this, expression, name), deprecationReason);

        public ExpressionField<TEntity, TReturnType, TExecutionContext> AddObjectExpressionField<TReturnType>(string name, Expression<Func<TEntity, TReturnType>> expression, string? deprecationReason)
           where TReturnType : class => InternalAddField(new ExpressionField<TEntity, TReturnType, TExecutionContext>(this, expression, name), deprecationReason);

        public ExpressionField<TEntity, IEnumerable<TReturnType>, TExecutionContext> AddEnumerableObjectExpressionField<TReturnType>(string name, Expression<Func<TEntity, IEnumerable<TReturnType>>> expression, string? deprecationReason)
           where TReturnType : class => InternalAddField(new ExpressionField<TEntity, IEnumerable<TReturnType>, TExecutionContext>(this, expression, name), deprecationReason);

        public SubmitField<TEntity, TExecutionContext> AddSubmitField(string name, IGraphTypeDescriptor<TExecutionContext> returnGraphType, string argName, Type graphType, Func<IResolveFieldContext, Dictionary<string, object>, Task<object>> resolve, Type fieldType, string? deprecationReason = null)
        {
            var field = new SubmitField<TEntity, TExecutionContext>(this, name, returnGraphType, argName, graphType, resolve, fieldType);
            return InternalAddField(field, deprecationReason);
        }

        public void AddFilter<TValueType>(string name, Func<TValueType, Expression<Func<TEntity, bool>>> filterPredicateFactory)
        {
            Guards.ThrowIfNullOrEmpty(name, nameof(name));

            _inlineFilters.Add(new CustomInlineFilter<TEntity, TValueType, TExecutionContext>(name, filterPredicateFactory));
        }

        public void AddFilter<TValueType>(string name, Func<TExecutionContext, TValueType, Expression<Func<TEntity, bool>>> filterPredicateFactory)
        {
            Guards.ThrowIfNullOrEmpty(name, nameof(name));

            _inlineFilters.Add(new CustomInlineFilter<TEntity, TValueType, TExecutionContext>(name, filterPredicateFactory));
        }

        public void AddSorter<TValueType>(string name, Expression<Func<TEntity, TValueType>> selector)
        {
            Guards.ThrowIfNullOrEmpty(name, nameof(name));

            _sorters.Add(new CustomSorter<TEntity, TValueType, TExecutionContext>(name, selector));
        }

        public void AddSorter(ISorter<TExecutionContext> sorter)
        {
            _sorters.Add(sorter);
        }

        public void AddOnEntityLoaded<T>(Expression<Func<TEntity, T>> proxyExpression, Action<TExecutionContext, T> hook)
        {
            ProxyAccessor.AddLoadHook(proxyExpression, hook);
        }

        public void AddOnEntityLoaded<TKey, T>(
            Expression<Func<TEntity, TKey>> keyExpression,
            Func<TExecutionContext, IEnumerable<TKey>, IDictionary<TKey, T>> batchFunc,
            Action<TExecutionContext, T> hook)
        {
            ProxyAccessor.AddLoadHook(keyExpression, batchFunc, hook);
        }

        public void AddOnEntityLoaded<TKey, T>(
            Expression<Func<TEntity, TKey>> keyExpression,
            Func<TExecutionContext, IEnumerable<TKey>, Task<IDictionary<TKey, T>>> batchFunc,
            Action<TExecutionContext, T> hook)
        {
            ProxyAccessor.AddLoadHook(keyExpression, batchFunc, hook);
        }

        public IInlineFilters<TEntity, TExecutionContext> CreateInlineFilters()
        {
            if (_filters == null)
            {
                var baseConfigurator = GetBaseObjectGraphTypeConfiguratorForFilters();

                _filters = baseConfigurator != null
                    ? baseConfigurator.CreateInlineFilters()
                    : new InlineFilters<TEntity, TExecutionContext>(
                        Fields
                            .OfType<IExpressionField<TEntity, TExecutionContext>>()
                            .Where(f => f.IsFilterable)
                            .Select(f => f.CreateInlineFilter())
                            .Concat(_inlineFilters),
                        $"{Name}Filter");
            }

            return _filters;
        }

        IInlineFilters<TExecutionContext> IObjectGraphTypeConfigurator<TExecutionContext>.CreateInlineFilters() => CreateInlineFilters();

        public abstract void ConfigureGraphType(IComplexGraphType graphType);

        public virtual void ConfigureGroupGraphType(IObjectGraphType graphType)
        {
            ValidateFields();
            ProxyAccessor.Configure();

            // TODO should be generated by RelationRegistry
            graphType.Name = $"{Name}Grouping";
            foreach (var field in _fields
                .OfType<IExpressionField<TEntity, TExecutionContext>>()
                .Where(field => field.IsGroupable))
            {
                graphType.AddField(field.AsFieldType());
            }
        }

        public string GetGraphQLTypePrefix()
        {
            if (Parent == null)
            {
                return Name;
            }

            return Parent.GetGraphQLTypePrefix();
        }

        public override int GetHashCode()
        {
            var res = default(HashCode);

            res.Add(_fields as ICollection<IField<TExecutionContext>>, CollectionEqualityComparer<IField<TExecutionContext>>.Default);
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
                .OfType<IExpressionField<TEntity, TExecutionContext>>()
                .Where(field => field.IsFilterable).SequenceEqual(other._fields.OfType<IExpressionField<TEntity, TExecutionContext>>().Where(field => field.IsFilterable))
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
            _fields.Clear();
            _inlineFilters.Clear();
            _sorters.Clear();

            OnConfigure();

            ValidateFields();
            ProxyAccessor.Configure();
        }

        public abstract IGraphTypeDescriptor<TReturnType, TExecutionContext> GetGraphQLTypeDescriptor<TReturnType>(IField<TExecutionContext> parent, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class;

        public abstract IGraphTypeDescriptor<TReturnType, TExecutionContext> GetGraphQLTypeDescriptor<TReturnType>(IField<TExecutionContext> parent);

        public abstract IGraphTypeDescriptor<TReturnType, TExecutionContext> GetGraphQLTypeDescriptor<TProjection, TReturnType>()
            where TReturnType : class
            where TProjection : ProjectionBase<TReturnType, TExecutionContext>, new();

        public abstract string GetGraphQLTypeName(Type entityType, Type? projectionType, IField<TExecutionContext> field);

        public abstract Type GenerateGraphType();

        public BatchClassField<TEntity, TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            var graphType = GetGraphQLTypeDescriptor(field, build);
            var result = new BatchClassField<TEntity, TEntity, TReturnType, TExecutionContext>(
                this,
                field.Name,
                FuncConstants<TEntity>.IdentityExpression,
                batchFunc,
                graphType);
            return ReplaceField(field, result);
        }

        public BatchEnumerableClassField<TEntity, TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            var graphType = GetGraphQLTypeDescriptor(field, build);
            var result = new BatchEnumerableClassField<TEntity, TEntity, TReturnType, TExecutionContext>(
                this,
                field.Name,
                FuncConstants<TEntity>.IdentityExpression,
                batchFunc,
                graphType);
            return ReplaceField(field, result);
        }

        public BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build = null)
            where TReturnType : class
        {
            var graphType = GetGraphQLTypeDescriptor(field, build);
            var result = new BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext>(this, field.Name, keySelector, batchFunc, graphType);
            return ReplaceField(field, result);
        }

        public BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build = null)
            where TReturnType : class
        {
            var graphType = GetGraphQLTypeDescriptor(field, build);
            var result = new BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext>(this, field.Name, keySelector, batchFunc, graphType);
            return ReplaceField(field, result);
        }

        public BatchField<TEntity, TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc)
        {
            var graphType = GetGraphQLTypeDescriptor<TReturnType>(field);
            var result = new BatchField<TEntity, TEntity, TReturnType, TExecutionContext>(
                this,
                field.Name,
                FuncConstants<TEntity>.IdentityExpression,
                batchFunc,
                graphType);
            return ReplaceField(field, result);
        }

        public BatchEnumerableField<TEntity, TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc)
        {
            var graphType = GetGraphQLTypeDescriptor<TReturnType>(field);
            var result = new BatchEnumerableField<TEntity, TEntity, TReturnType, TExecutionContext>(
                this,
                field.Name,
                FuncConstants<TEntity>.IdentityExpression,
                batchFunc,
                graphType);
            return ReplaceField(field, result);
        }

        public BatchField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc)
        {
            var graphType = GetGraphQLTypeDescriptor<TReturnType>(field);
            var result = new BatchField<TEntity, TKeyType, TReturnType, TExecutionContext>(this, field.Name, keySelector, batchFunc, graphType);
            return ReplaceField(field, result);
        }

        public BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc)
        {
            var graphType = GetGraphQLTypeDescriptor<TReturnType>(field);
            var result = new BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext>(this, field.Name, keySelector, batchFunc, graphType);
            return ReplaceField(field, result);
        }

        public BatchClassField<TEntity, TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Func<IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return FromBatch(field, Registry.WrapFuncByUnusedContext(batchFunc), build);
        }

        public BatchEnumerableClassField<TEntity, TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Func<IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return FromBatch(field, Registry.WrapFuncByUnusedContext(batchFunc), build);
        }

        public BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build = null)
            where TReturnType : class
        {
            return FromBatch(field, keySelector, Registry.WrapFuncByUnusedContext(batchFunc), build);
        }

        public BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build = null)
            where TReturnType : class
        {
            return FromBatch(field, keySelector, Registry.WrapFuncByUnusedContext(batchFunc), build);
        }

        public BatchField<TEntity, TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Func<IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc)
        {
            return FromBatch(field, Registry.WrapFuncByUnusedContext(batchFunc));
        }

        public BatchEnumerableField<TEntity, TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Func<IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc)
        {
            return FromBatch(field, Registry.WrapFuncByUnusedContext(batchFunc));
        }

        public BatchField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc)
        {
            return FromBatch(field, keySelector, Registry.WrapFuncByUnusedContext(batchFunc));
        }

        public BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc)
        {
            return FromBatch(field, keySelector, Registry.WrapFuncByUnusedContext(batchFunc));
        }

        public BatchClassField<TEntity, TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            var graphType = GetGraphQLTypeDescriptor(field, build);
            var result = new BatchClassField<TEntity, TEntity, TReturnType, TExecutionContext>(
                this,
                field.Name,
                FuncConstants<TEntity>.IdentityExpression,
                batchFunc,
                graphType);
            return ReplaceField(field, result);
        }

        public BatchEnumerableClassField<TEntity, TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            var graphType = GetGraphQLTypeDescriptor(field, build);
            var result = new BatchEnumerableClassField<TEntity, TEntity, TReturnType, TExecutionContext>(
                this,
                field.Name,
                FuncConstants<TEntity>.IdentityExpression,
                batchFunc,
                graphType);
            return ReplaceField(field, result);
        }

        public BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>,
                Task<IDictionary<TKeyType, TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build = null)
            where TReturnType : class
        {
            var graphType = GetGraphQLTypeDescriptor(field, build);
            var result = new BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext>(this, field.Name, keySelector, batchFunc, graphType);
            return ReplaceField(field, result);
        }

        public BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build = null)
            where TReturnType : class
        {
            var graphType = GetGraphQLTypeDescriptor(field, build);
            var result = new BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext>(this, field.Name, keySelector, batchFunc, graphType);
            return ReplaceField(field, result);
        }

        public BatchField<TEntity, TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc)
        {
            var graphType = GetGraphQLTypeDescriptor<TReturnType>(field);
            var result = new BatchField<TEntity, TEntity, TReturnType, TExecutionContext>(
                this,
                field.Name,
                FuncConstants<TEntity>.IdentityExpression,
                batchFunc,
                graphType);
            return ReplaceField(field, result);
        }

        public BatchEnumerableField<TEntity, TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc)
        {
            var graphType = GetGraphQLTypeDescriptor<TReturnType>(field);
            var result = new BatchEnumerableField<TEntity, TEntity, TReturnType, TExecutionContext>(
                this,
                field.Name,
                FuncConstants<TEntity>.IdentityExpression,
                batchFunc,
                graphType);
            return ReplaceField(field, result);
        }

        public BatchField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc)
        {
            var graphType = GetGraphQLTypeDescriptor<TReturnType>(field);
            var result = new BatchField<TEntity, TKeyType, TReturnType, TExecutionContext>(this, field.Name, keySelector, batchFunc, graphType);
            return ReplaceField(field, result);
        }

        public BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc)
        {
            var graphType = GetGraphQLTypeDescriptor<TReturnType>(field);
            var result = new BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext>(this, field.Name, keySelector, batchFunc, graphType);
            return ReplaceField(field, result);
        }

        public BatchClassField<TEntity, TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return FromBatch(field, Registry.WrapFuncByUnusedContext(batchFunc), build);
        }

        public BatchEnumerableClassField<TEntity, TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return FromBatch(field, Registry.WrapFuncByUnusedContext(batchFunc), build);
        }

        public BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build = null)
            where TReturnType : class
        {
            return FromBatch(field, keySelector, Registry.WrapFuncByUnusedContext(batchFunc), build);
        }

        public BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build = null)
            where TReturnType : class
        {
            return FromBatch(field, keySelector, Registry.WrapFuncByUnusedContext(batchFunc), build);
        }

        public BatchField<TEntity, TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc)
        {
            return FromBatch(field, Registry.WrapFuncByUnusedContext(batchFunc));
        }

        public BatchEnumerableField<TEntity, TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc)
        {
            return FromBatch(field, Registry.WrapFuncByUnusedContext(batchFunc));
        }

        public BatchField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc)
        {
            return FromBatch(field, keySelector, Registry.WrapFuncByUnusedContext(batchFunc));
        }

        public BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc)
        {
            return FromBatch(field, keySelector, Registry.WrapFuncByUnusedContext(batchFunc));
        }

        public QueryableField<TEntity, TSelectType, TExecutionContext> FromIQueryableClass<TSelectType>(
            FieldBase<TEntity, TExecutionContext> field,
            Func<TExecutionContext, IQueryable<TSelectType>> query,
            Expression<Func<TEntity, TSelectType, bool>>? condition,
            Action<IInlineObjectBuilder<TSelectType, TExecutionContext>>? build)
            where TSelectType : class
        {
            var graphType = GetGraphQLTypeDescriptor(field, build);
            var result = new QueryableField<TEntity, TSelectType, TExecutionContext>(
                this,
                field.Name,
                query,
                condition,
                graphType,
                searcher: null,
                naturalSorters: SortingHelpers.Empty);

            return ReplaceField(field, result);
        }

        public LoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext> FromLoader<TLoader, TChildLoader, TChildEntity>(
            Field<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            RelationType relationType,
            Expression<Func<TChildEntity, TEntity>>? navigationProperty,
            Expression<Func<TEntity, TChildEntity>>? reverseNavigationProperty,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> graphResultType)
            where TChildEntity : class
            where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
            where TLoader : Loader<TEntity, TExecutionContext>, new()
        {
            var factorizationResult = ExpressionHelpers.FactorizeCondition(condition);
            var equality = Expression.Equal(factorizationResult.LeftExpression.Body, factorizationResult.RightExpression.Body);
            var lambda = Expression.Lambda<Func<TEntity, TChildEntity, bool>>(equality, condition.Parameters);

            var result = ReplaceField(field, new LoaderField<TLoader, TChildLoader, TEntity, TChildEntity, TExecutionContext>(this, field.Name, lambda, relationType, navigationProperty, reverseNavigationProperty, graphResultType));

            if (factorizationResult.RightCondition != null)
            {
                return result.ApplyWhere(factorizationResult.RightCondition);
            }

            return result;
        }

        public LoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext> FromLoader<TChildLoader, TChildEntity>(
            FieldBase<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> graphResultType)
            where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
            where TChildEntity : class
        {
            var result = new LoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>(
                this,
                field.Name,
                condition,
                graphResultType,
                arguments: null,
                searcher: null,
                naturalSorters: SortingHelpers.Empty);

            return ReplaceField(field, result);
        }

        public SelectField<TEntity, TReturnType, TExecutionContext> ApplySelect<TReturnType>(FieldBase<TEntity, TExecutionContext> field, IResolver<TEntity> resolver)
        {
            var graphType = GetGraphQLTypeDescriptor<TReturnType>(field);
            return ReplaceField(field, new SelectField<TEntity, TReturnType, TExecutionContext>(this, field.Name, resolver, graphType));
        }

        public SelectField<TEntity, TReturnType, TExecutionContext> ApplySelect<TReturnType>(FieldBase<TEntity, TExecutionContext> field, IResolver<TEntity> resolver, IGraphTypeDescriptor<TExecutionContext> graphType)
        {
            return ReplaceField(field, new SelectField<TEntity, TReturnType, TExecutionContext>(this, field.Name, resolver, graphType));
        }

        public SelectField<TEntity, TReturnType, TExecutionContext> ApplySelect<TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            IResolver<TEntity> resolver,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            var graphType = GetGraphQLTypeDescriptor(field, build);
            return ReplaceField(field, new SelectField<TEntity, TReturnType, TExecutionContext>(this, field.Name, resolver, graphType));
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TFromType, TAnotherReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            IBatchResolver<TEntity, TFromType> firstResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
            where TAnotherReturnType : class
        {
            var batchResolver = new BatchKeyResolver<TEntity, TEntity, TAnotherReturnType, TExecutionContext>(field.Name, FuncConstants<TEntity>.IdentityExpression, batchFunc, ProxyAccessor);

            return ApplyBatchUnion(field, firstResolver, firstGraphType, batchResolver, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TFromType, TAnotherReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            IBatchResolver<TEntity, TFromType> firstResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            Func<IEnumerable<TEntity>, IDictionary<TEntity, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
            where TAnotherReturnType : class
        {
            var batchResolver = new BatchKeyResolver<TEntity, TEntity, TAnotherReturnType, TExecutionContext>(field.Name, FuncConstants<TEntity>.IdentityExpression, (ctx, e) => batchFunc(e), ProxyAccessor);

            return ApplyBatchUnion(field, firstResolver, firstGraphType, batchResolver, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TFromType, TKeyType, TAnotherReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            IBatchResolver<TEntity, TFromType> firstResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
            where TAnotherReturnType : class
        {
            var batchResolver = new BatchKeyResolver<TEntity, TKeyType, TAnotherReturnType, TExecutionContext>(field.Name, keySelector, batchFunc, ProxyAccessor);

            return ApplyBatchUnion(field, firstResolver, firstGraphType, batchResolver, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TFromType, TKeyType, TAnotherReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            IBatchResolver<TEntity, TFromType> firstResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, IDictionary<TKeyType, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
            where TAnotherReturnType : class
        {
            var batchResolver = new BatchKeyResolver<TEntity, TKeyType, TAnotherReturnType, TExecutionContext>(field.Name, keySelector, (ctx, items) => batchFunc(items), ProxyAccessor);

            return ApplyBatchUnion(field, firstResolver, firstGraphType, batchResolver, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TFromType, TAnotherReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            IBatchResolver<TEntity, TFromType> firstResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
            where TAnotherReturnType : class
        {
            var batchResolver = new BatchTaskKeyResolver<TEntity, TEntity, TAnotherReturnType, TExecutionContext>(field.Name, FuncConstants<TEntity>.IdentityExpression, batchFunc, ProxyAccessor);

            return ApplyBatchUnion(field, firstResolver, firstGraphType, batchResolver, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TFromType, TAnotherReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            IBatchResolver<TEntity, TFromType> firstResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
            where TAnotherReturnType : class
        {
            var batchResolver = new BatchTaskKeyResolver<TEntity, TEntity, TAnotherReturnType, TExecutionContext>(field.Name, FuncConstants<TEntity>.IdentityExpression, (ctx, e) => batchFunc(e), ProxyAccessor);

            return ApplyBatchUnion(field, firstResolver, firstGraphType, batchResolver, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TFromType, TKeyType, TAnotherReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            IBatchResolver<TEntity, TFromType> firstResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
            where TAnotherReturnType : class
        {
            var batchResolver = new BatchTaskKeyResolver<TEntity, TKeyType, TAnotherReturnType, TExecutionContext>(field.Name, keySelector, batchFunc, ProxyAccessor);

            return ApplyBatchUnion(field, firstResolver, firstGraphType, batchResolver, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TFromType, TKeyType, TAnotherReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            IBatchResolver<TEntity, TFromType> firstResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
            where TAnotherReturnType : class
        {
            var batchResolver = new BatchTaskKeyResolver<TEntity, TKeyType, TAnotherReturnType, TExecutionContext>(field.Name, keySelector, (ctx, items) => batchFunc(items), ProxyAccessor);

            return ApplyBatchUnion(field, firstResolver, firstGraphType, batchResolver, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TFromType, TAnotherReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            IBatchResolver<TEntity, TFromType> firstResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            IBatchResolver<TEntity, TAnotherReturnType> secondResolver,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
            where TAnotherReturnType : class
        {
            var secondGraphType = GetGraphQLTypeDescriptor(field, build);

            return ReplaceField(field, new BatchUnionField<TEntity, TExecutionContext>(
                this,
                field.Name,
                firstResolver,
                secondResolver,
                firstGraphType,
                typeof(TFromType),
                secondGraphType,
                typeof(TAnotherReturnType)));
        }

        public void ApplyResolvedField<TReturnType>(
            FieldBase<TEntity, TExecutionContext> field,
            IGraphTypeDescriptor<TExecutionContext> graphType,
            IFieldResolver resolver)
        {
            var resolvedField = new ResolvedField<TEntity, TReturnType, TExecutionContext>(
                this,
                field.Name,
                graphType,
                resolver,
                field.Arguments);

            ReplaceField(field, resolvedField);
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

        private protected abstract void ValidateFields();

        private IField<TExecutionContext> AddField(PropertyInfo propertyInfo)
        {
            var propertyType = propertyInfo.PropertyType;
            var expressionType = typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(typeof(TEntity), propertyType));
            LambdaExpression expression = propertyInfo.MakePropertyLambdaExpression();

            MethodInfo? addFieldMethodInfo = null;
            if (propertyType == typeof(string))
            {
                _addExpressionStringFieldMethodInfo = ReflectionHelpers.GetMethodInfo<string, Expression<Func<TEntity, string>>, string?, ExpressionField<TEntity, string, TExecutionContext>>(
                    AddExpressionField);

                addFieldMethodInfo = _addExpressionStringFieldMethodInfo;
            }
            else if (propertyType.IsEnumerableOfT())
            {
                var underlyingType = TypeExtensions.GetEnumerableElementType(propertyType);
                var unwrappedUnderlyingType = underlyingType.UnwrapIfNullable();
                var isNullable = underlyingType != unwrappedUnderlyingType;
                var enumerableType = typeof(IEnumerable<>).MakeGenericType(underlyingType);
                expressionType = typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(typeof(TEntity), enumerableType));
                expression = propertyInfo.MakePropertyLambdaExpression(enumerableType);

                if (underlyingType == typeof(string))
                {
                    _addEnumerableStringExpressionFieldMethodInfo ??= ReflectionHelpers.GetMethodInfo<string?, Expression<Func<TEntity, IEnumerable<string>>>, string?, ExpressionField<TEntity, IEnumerable<string>, TExecutionContext>>(
                        AddEnumerableExpressionField);

                    addFieldMethodInfo = _addEnumerableStringExpressionFieldMethodInfo;
                }
                else if (underlyingType.IsValueType)
                {
                    if (isNullable)
                    {
                        _addEnumerableNullableExpressionFieldMethodInfo ??= ReflectionHelpers.GetMethodInfo<string?, Expression<Func<TEntity, IEnumerable<int?>>>, string?, ExpressionField<TEntity, IEnumerable<int?>, TExecutionContext>>(
                            AddEnumerableExpressionField);
                        addFieldMethodInfo = _addEnumerableNullableExpressionFieldMethodInfo
                            .MakeGenericMethod(unwrappedUnderlyingType);
                    }
                    else
                    {
                        _addEnumerableExpressionFieldMethodInfo ??= ReflectionHelpers.GetMethodInfo<string?, Expression<Func<TEntity, IEnumerable<int>>>, string?, ExpressionField<TEntity, IEnumerable<int>, TExecutionContext>>(
                            AddEnumerableExpressionField);

                        addFieldMethodInfo = _addEnumerableExpressionFieldMethodInfo
                            .MakeGenericMethod(underlyingType.UnwrapIfNullable());
                    }
                }
                else
                {
                    _addEnumerableObjectExpressionFieldMethodInfo ??= ReflectionHelpers.GetMethodInfo<string, Expression<Func<TEntity, IEnumerable<object>>>, string?, ExpressionField<TEntity, IEnumerable<object>, TExecutionContext>>(
                        AddEnumerableObjectExpressionField);

                    addFieldMethodInfo = _addEnumerableObjectExpressionFieldMethodInfo
                        .MakeGenericMethod(underlyingType);
                }
            }
            else if (propertyType.IsValueType)
            {
                var unwrappedUnderlyingType = propertyType.UnwrapIfNullable();
                var isNullable = propertyType != unwrappedUnderlyingType;

                if (isNullable)
                {
                    _addExpressionNullableFieldMethodInfo ??= ReflectionHelpers.GetMethodInfo<string?, Expression<Func<TEntity, int?>>, string?, ExpressionField<TEntity, int?, TExecutionContext>>(
                        AddExpressionField);

                    addFieldMethodInfo = _addExpressionNullableFieldMethodInfo.MakeGenericMethod(unwrappedUnderlyingType);
                }
                else
                {
                    _addExpressionFieldMethodInfo ??= ReflectionHelpers.GetMethodInfo<string?, Expression<Func<TEntity, int>>, string?, ExpressionField<TEntity, int, TExecutionContext>>(
                        AddExpressionField);

                    addFieldMethodInfo = _addExpressionFieldMethodInfo.MakeGenericMethod(unwrappedUnderlyingType);
                }
            }
            else
            {
                _addExpressionObjectFieldMethodInfo ??= ReflectionHelpers.GetMethodInfo<string, Expression<Func<TEntity, object>>, string?, ExpressionField<TEntity, object, TExecutionContext>>(
                    AddObjectExpressionField);

                addFieldMethodInfo = _addExpressionObjectFieldMethodInfo.MakeGenericMethod(propertyType);
            }

            var name = propertyInfo.Name;

            return addFieldMethodInfo.InvokeAndHoistBaseException<IField<TExecutionContext>>(this, name, expression, null);
        }

        private TField InternalAddField<TField>(TField field, string? deprecationReason)
            where TField : FieldBase<TEntity, TExecutionContext>
        {
            _fields.Add(field);
            field.DeprecationReason = deprecationReason;
            return field;
        }
    }
}
