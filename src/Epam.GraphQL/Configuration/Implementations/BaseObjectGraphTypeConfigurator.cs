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
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;
using Epam.GraphQL.Configuration.Implementations.Fields;
using Epam.GraphQL.Configuration.Implementations.Fields.BatchFields;
using Epam.GraphQL.Configuration.Implementations.Fields.ChildFields;
using Epam.GraphQL.Configuration.Implementations.Fields.ExpressionFields;
using Epam.GraphQL.Configuration.Implementations.Fields.Helpers;
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
using GraphQL.Types;
using TypeExtensions = Epam.GraphQL.Extensions.TypeExtensions;

namespace Epam.GraphQL.Configuration.Implementations
{
#pragma warning disable CA1506
    internal abstract class BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> : IObjectGraphTypeConfigurator<TEntity, TExecutionContext>, IEquatable<BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext>>
#pragma warning restore CA1506
        where TEntity : class
    {
        private readonly List<IField<TEntity, TExecutionContext>> _fields = new();
        private readonly List<IInlineFilter<TExecutionContext>> _inlineFilters = new();
        private readonly List<ISorter<TExecutionContext>> _sorters = new();
        private IInlineFilters<TEntity, TExecutionContext> _filters;

        private string _name;

        protected BaseObjectGraphTypeConfigurator(IField<TExecutionContext> parent, RelationRegistry<TExecutionContext> registry, bool isAuto)
        {
            Registry = registry ?? throw new ArgumentNullException(nameof(registry));
            Parent = parent;
            ProxyAccessor = new ProxyAccessor<TEntity, TExecutionContext>(_fields);

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
            get => _name;

            set
            {
                if (value != _name)
                {
                    if (value == null)
                    {
                        throw new ArgumentNullException(nameof(value));
                    }

                    SetGraphQLTypeName(_name, value);

                    _name = value;
                }
            }
        }

        public bool HasInlineFilters => _inlineFilters.Any() || _fields.Any(f => f.IsFilterable);

        public IReadOnlyList<IField<TExecutionContext>> Fields => _fields;

        public ProxyAccessor<TEntity, TExecutionContext> ProxyAccessor { get; }

        IProxyAccessor<TEntity, TExecutionContext> IObjectGraphTypeConfigurator<TEntity, TExecutionContext>.ProxyAccessor => ProxyAccessor;

        IProxyAccessor<TExecutionContext> IObjectGraphTypeConfigurator<TExecutionContext>.ProxyAccessor => ProxyAccessor;

        IReadOnlyList<IField> IObjectGraphTypeConfigurator.Fields => Fields;

        public IField<TExecutionContext> Parent { get; }

        public IReadOnlyList<ISorter<TExecutionContext>> Sorters => _sorters;

        protected RelationRegistry<TExecutionContext> Registry { get; }

        public IField<TEntity, TExecutionContext> FindFieldByName(string name) => _fields.Find(field => field.Name.Equals(name, StringComparison.OrdinalIgnoreCase));

        public TField ReplaceField<TField>(Field<TEntity, TExecutionContext> oldField, TField newField)
            where TField : Field<TEntity, TExecutionContext>
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

        public Field<TEntity, TExecutionContext> AddField(string name, string deprecationReason)
        {
            var field = new Field<TEntity, TExecutionContext>(Registry, this, name);
            return InternalAddField(field, deprecationReason);
        }

        public TypedField<TEntity, TReturnType, TExecutionContext> AddField<TReturnType>(string name, string deprecationReason)
        {
            var field = new TypedField<TEntity, TReturnType, TExecutionContext>(
                Registry,
                this,
                name,
                GetGraphQLTypeDescriptor<TReturnType>(null));

            return InternalAddField(field, deprecationReason);
        }

        public TypedField<TEntity, TReturnType, TExecutionContext> AddField<TProjection, TReturnType>(string name, string deprecationReason)
            where TProjection : ProjectionBase<TReturnType, TExecutionContext>, new()
            where TReturnType : class
        {
            var field = new TypedField<TEntity, TProjection, TReturnType, TExecutionContext>(
                Registry,
                this,
                name);

            return InternalAddField(field, deprecationReason);
        }

        public StructExpressionField<TEntity, TReturnType, TExecutionContext> AddField<TReturnType>(string name, Expression<Func<TEntity, TReturnType>> expression, string deprecationReason)
            where TReturnType : struct => InternalAddField(new StructExpressionField<TEntity, TReturnType, TExecutionContext>(Registry, this, expression, name), deprecationReason);

        public StructExpressionField<TEntity, TReturnType, TExecutionContext> AddField<TReturnType>(string name, Expression<Func<TExecutionContext, TEntity, TReturnType>> expression, string deprecationReason)
            where TReturnType : struct => InternalAddField(new StructExpressionField<TEntity, TReturnType, TExecutionContext>(Registry, this, expression, name), deprecationReason);

        public NullableExpressionField<TEntity, TReturnType, TExecutionContext> AddField<TReturnType>(string name, Expression<Func<TEntity, TReturnType?>> expression, string deprecationReason)
            where TReturnType : struct => InternalAddField(new NullableExpressionField<TEntity, TReturnType, TExecutionContext>(Registry, this, expression, name), deprecationReason);

        public NullableExpressionField<TEntity, TReturnType, TExecutionContext> AddField<TReturnType>(string name, Expression<Func<TExecutionContext, TEntity, TReturnType?>> expression, string deprecationReason)
            where TReturnType : struct => InternalAddField(new NullableExpressionField<TEntity, TReturnType, TExecutionContext>(Registry, this, expression, name), deprecationReason);

        public StringExpressionField<TEntity, TExecutionContext> AddField(string name, Expression<Func<TEntity, string>> expression, string deprecationReason)
            => InternalAddField(new StringExpressionField<TEntity, TExecutionContext>(Registry, this, expression, name), deprecationReason);

        public StringExpressionField<TEntity, TExecutionContext> AddField(string name, Expression<Func<TExecutionContext, TEntity, string>> expression, string deprecationReason)
            => InternalAddField(new StringExpressionField<TEntity, TExecutionContext>(Registry, this, expression, name), deprecationReason);

        public ExpressionField<TEntity, IEnumerable<TReturnType>, TExecutionContext> AddField<TReturnType>(string name, Expression<Func<TEntity, IEnumerable<TReturnType>>> expression, string deprecationReason)
            where TReturnType : struct => InternalAddField(new ExpressionField<TEntity, IEnumerable<TReturnType>, TExecutionContext>(Registry, this, expression, name), deprecationReason);

        public ExpressionField<TEntity, IEnumerable<TReturnType>, TExecutionContext> AddField<TReturnType>(string name, Expression<Func<TExecutionContext, TEntity, IEnumerable<TReturnType>>> expression, string deprecationReason)
            where TReturnType : struct => InternalAddField(new ExpressionField<TEntity, IEnumerable<TReturnType>, TExecutionContext>(Registry, this, expression, name), deprecationReason);

        public ExpressionField<TEntity, IEnumerable<TReturnType?>, TExecutionContext> AddField<TReturnType>(string name, Expression<Func<TEntity, IEnumerable<TReturnType?>>> expression, string deprecationReason)
            where TReturnType : struct => InternalAddField(new ExpressionField<TEntity, IEnumerable<TReturnType?>, TExecutionContext>(Registry, this, expression, name), deprecationReason);

        public ExpressionField<TEntity, IEnumerable<TReturnType?>, TExecutionContext> AddField<TReturnType>(string name, Expression<Func<TExecutionContext, TEntity, IEnumerable<TReturnType?>>> expression, string deprecationReason)
            where TReturnType : struct => InternalAddField(new ExpressionField<TEntity, IEnumerable<TReturnType?>, TExecutionContext>(Registry, this, expression, name), deprecationReason);

        public ExpressionField<TEntity, IEnumerable<string>, TExecutionContext> AddField(string name, Expression<Func<TEntity, IEnumerable<string>>> expression, string deprecationReason)
            => InternalAddField(new ExpressionField<TEntity, IEnumerable<string>, TExecutionContext>(Registry, this, expression, name), deprecationReason);

        public ExpressionField<TEntity, IEnumerable<string>, TExecutionContext> AddField(string name, Expression<Func<TExecutionContext, TEntity, IEnumerable<string>>> expression, string deprecationReason)
            => InternalAddField(new ExpressionField<TEntity, IEnumerable<string>, TExecutionContext>(Registry, this, expression, name), deprecationReason);

        public ExpressionField<TEntity, TReturnType, TExecutionContext> AddObjectField<TReturnType>(string name, Expression<Func<TEntity, TReturnType>> expression, string deprecationReason)
           where TReturnType : class => InternalAddField(new ExpressionField<TEntity, TReturnType, TExecutionContext>(Registry, this, expression, name), deprecationReason);

        public ExpressionField<TEntity, IEnumerable<TReturnType>, TExecutionContext> AddObjectField<TReturnType>(string name, Expression<Func<TEntity, IEnumerable<TReturnType>>> expression, string deprecationReason)
           where TReturnType : class => InternalAddField(new ExpressionField<TEntity, IEnumerable<TReturnType>, TExecutionContext>(Registry, this, expression, name), deprecationReason);

        public ConnectionLoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext> AddConnectionLoaderField<TChildLoader, TChildEntity>(
            string name,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> graphResultType,
            string deprecationReason)
            where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
            where TChildEntity : class
        {
            var field = new LoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>(Registry, this, name, null, graphResultType);
            var connectionField = field.ApplyConnection();
            return InternalAddField(connectionField, deprecationReason);
        }

        public QueryableField<TEntity, TChildEntity, TExecutionContext> AddConnectionLoaderField<TChildLoader, TChildEntity>(
            string name,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> graphResultType,
            Expression<Func<IQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>>> order,
            string deprecationReason)
            where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
            where TChildEntity : class
        {
            var field = new LoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>(Registry, this, name, null, graphResultType);
            var connectionField = field.ApplyConnection(order);
            return InternalAddField(connectionField, deprecationReason);
        }

        public GroupLoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext> AddGroupLoaderField<TChildLoader, TChildEntity>(string name, string deprecationReason = null)
            where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
            where TChildEntity : class
        {
            var loader = Registry.ResolveLoader<TChildLoader, TChildEntity>();
            var field = new GroupLoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>(Registry, this, name, null, loader.ApplyNaturalOrderBy, loader.ApplyNaturalThenBy);
            return InternalAddField(field, deprecationReason);
        }

        public SubmitField<TEntity, TExecutionContext> AddSubmitField(string name, IGraphTypeDescriptor<TExecutionContext> returnGraphType, string argName, Type graphType, Func<IResolveFieldContext, Dictionary<string, object>, Task<object>> resolve, Type fieldType, string deprecationReason = null)
        {
            var field = new SubmitField<TEntity, TExecutionContext>(Registry, this, name, returnGraphType, argName, graphType, resolve, fieldType);
            return InternalAddField(field, deprecationReason);
        }

        public void AddFilter<TValueType>(string name, Func<TValueType, Expression<Func<TEntity, bool>>> filterPredicateFactory)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (filterPredicateFactory == null)
            {
                throw new ArgumentNullException(nameof(filterPredicateFactory));
            }

            _inlineFilters.Add(new CustomInlineFilter<TEntity, TValueType, TExecutionContext>(name, filterPredicateFactory));
        }

        public void AddFilter<TValueType>(string name, Func<TExecutionContext, TValueType, Expression<Func<TEntity, bool>>> filterPredicateFactory)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (filterPredicateFactory == null)
            {
                throw new ArgumentNullException(nameof(filterPredicateFactory));
            }

            _inlineFilters.Add(new CustomInlineFilter<TEntity, TValueType, TExecutionContext>(name, filterPredicateFactory));
        }

        public void AddSorter<TValueType>(string name, Expression<Func<TEntity, TValueType>> selector)
        {
            if (string.IsNullOrEmpty(name))
            {
                throw new ArgumentNullException(nameof(name));
            }

            if (selector == null)
            {
                throw new ArgumentNullException(nameof(selector));
            }

            _sorters.Add(new CustomSorter<TEntity, TValueType, TExecutionContext>(name, selector));
        }

        public void AddSorter(ISorter<TExecutionContext> sorter)
        {
            if (sorter == null)
            {
                throw new ArgumentNullException(nameof(sorter));
            }

            _sorters.Add(sorter);
        }

        public void AddOnEntityLoaded<T>(Expression<Func<TEntity, T>> proxyExpression, Action<TExecutionContext, T> hook)
        {
            ProxyAccessor.AddLoadHook(proxyExpression, hook);
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
                            .Where(f => f.IsFilterable)
                            .Select(f => f.CreateInlineFilter())
                            .Concat(_inlineFilters),
                        $"{Name}Filter");
            }

            return _filters;
        }

        IInlineFilters IObjectGraphTypeConfigurator.CreateInlineFilters() => CreateInlineFilters();

        public abstract void ConfigureGraphType(IComplexGraphType graphType);

        public virtual void ConfigureGroupGraphType(IObjectGraphType graphType)
        {
            ValidateFields();
            ProxyAccessor.Configure();

            // TODO should be generated by RelationRegistry
            graphType.Name = $"{Name}Grouping";
            foreach (var field in _fields.Where(field => field.IsGroupable))
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

        public bool FilterEquals(BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> other)
        {
            if (other == null)
            {
                return false;
            }

            return _fields.Where(field => field.IsFilterable).SequenceEqual(other._fields.Where(field => field.IsFilterable))
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

        public abstract IGraphTypeDescriptor<TReturnType, TExecutionContext> GetGraphQLTypeDescriptor<TReturnType>(IField<TExecutionContext> parent, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        public abstract IGraphTypeDescriptor<TReturnType, TExecutionContext> GetGraphQLTypeDescriptor<TReturnType>(IField<TExecutionContext> parent);

        public abstract IGraphTypeDescriptor<TReturnType, TExecutionContext> GetGraphQLTypeDescriptor<TProjection, TReturnType>()
            where TReturnType : class
            where TProjection : ProjectionBase<TReturnType, TExecutionContext>, new();

        public abstract string GetGraphQLTypeName(Type entityType, Type projectionType, IField<TExecutionContext> field);

        public abstract Type GenerateGraphType();

        public BatchClassField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Field<TEntity, TExecutionContext> field, Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var graphType = GetGraphQLTypeDescriptor(field, build);
            var result = new BatchClassField<TEntity, TReturnType, TExecutionContext>(Registry, this, field.Name, batchFunc, graphType);
            return ReplaceField(field, result);
        }

        public BatchEnumerableClassField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Field<TEntity, TExecutionContext> field, Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var graphType = GetGraphQLTypeDescriptor(field, build);
            var result = new BatchEnumerableClassField<TEntity, TReturnType, TExecutionContext>(Registry, this, field.Name, batchFunc, graphType);
            return ReplaceField(field, result);
        }

        public BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Field<TEntity, TExecutionContext> field, Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            var graphType = GetGraphQLTypeDescriptor(field, build);
            var result = new BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext>(Registry, this, field.Name, keySelector, batchFunc, graphType);
            return ReplaceField(field, result);
        }

        public BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Field<TEntity, TExecutionContext> field, Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            var graphType = GetGraphQLTypeDescriptor(field, build);
            var result = new BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext>(Registry, this, field.Name, keySelector, batchFunc, graphType);
            return ReplaceField(field, result);
        }

        public BatchField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Field<TEntity, TExecutionContext> field, Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc)
        {
            var graphType = GetGraphQLTypeDescriptor<TReturnType>(field);
            var result = new BatchField<TEntity, TReturnType, TExecutionContext>(Registry, this, field.Name, batchFunc, graphType);
            return ReplaceField(field, result);
        }

        public BatchEnumerableField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Field<TEntity, TExecutionContext> field, Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc)
        {
            var graphType = GetGraphQLTypeDescriptor<TReturnType>(field);
            var result = new BatchEnumerableField<TEntity, TReturnType, TExecutionContext>(Registry, this, field.Name, batchFunc, graphType);
            return ReplaceField(field, result);
        }

        public BatchField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Field<TEntity, TExecutionContext> field, Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc)
        {
            var graphType = GetGraphQLTypeDescriptor<TReturnType>(field);
            var result = new BatchField<TEntity, TKeyType, TReturnType, TExecutionContext>(Registry, this, field.Name, keySelector, batchFunc, graphType);
            return ReplaceField(field, result);
        }

        public BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Field<TEntity, TExecutionContext> field, Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc)
        {
            var graphType = GetGraphQLTypeDescriptor<TReturnType>(field);
            var result = new BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext>(Registry, this, field.Name, keySelector, batchFunc, graphType);
            return ReplaceField(field, result);
        }

        public BatchClassField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Field<TEntity, TExecutionContext> field, Func<IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            return FromBatch(field, Registry.WrapFuncByUnusedContext(batchFunc), build);
        }

        public BatchEnumerableClassField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Field<TEntity, TExecutionContext> field, Func<IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            return FromBatch(field, Registry.WrapFuncByUnusedContext(batchFunc), build);
        }

        public BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Field<TEntity, TExecutionContext> field, Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return FromBatch(field, keySelector, Registry.WrapFuncByUnusedContext(batchFunc), build);
        }

        public BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Field<TEntity, TExecutionContext> field, Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return FromBatch(field, keySelector, Registry.WrapFuncByUnusedContext(batchFunc), build);
        }

        public BatchField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Field<TEntity, TExecutionContext> field, Func<IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc)
        {
            return FromBatch(field, Registry.WrapFuncByUnusedContext(batchFunc));
        }

        public BatchEnumerableField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Field<TEntity, TExecutionContext> field, Func<IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc)
        {
            return FromBatch(field, Registry.WrapFuncByUnusedContext(batchFunc));
        }

        public BatchField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Field<TEntity, TExecutionContext> field, Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc)
        {
            return FromBatch(field, keySelector, Registry.WrapFuncByUnusedContext(batchFunc));
        }

        public BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Field<TEntity, TExecutionContext> field, Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc)
        {
            return FromBatch(field, keySelector, Registry.WrapFuncByUnusedContext(batchFunc));
        }

        public BatchClassField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Field<TEntity, TExecutionContext> field, Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var graphType = GetGraphQLTypeDescriptor(field, build);
            var result = new BatchClassField<TEntity, TReturnType, TExecutionContext>(Registry, this, field.Name, batchFunc, graphType);
            return ReplaceField(field, result);
        }

        public BatchEnumerableClassField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Field<TEntity, TExecutionContext> field, Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var graphType = GetGraphQLTypeDescriptor(field, build);
            var result = new BatchEnumerableClassField<TEntity, TReturnType, TExecutionContext>(Registry, this, field.Name, batchFunc, graphType);
            return ReplaceField(field, result);
        }

        public BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Field<TEntity, TExecutionContext> field, Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            var graphType = GetGraphQLTypeDescriptor(field, build);
            var result = new BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext>(Registry, this, field.Name, keySelector, batchFunc, graphType);
            return ReplaceField(field, result);
        }

        public BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Field<TEntity, TExecutionContext> field, Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            var graphType = GetGraphQLTypeDescriptor(field, build);
            var result = new BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext>(Registry, this, field.Name, keySelector, batchFunc, graphType);
            return ReplaceField(field, result);
        }

        public BatchField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Field<TEntity, TExecutionContext> field, Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc)
        {
            var graphType = GetGraphQLTypeDescriptor<TReturnType>(field);
            var result = new BatchField<TEntity, TReturnType, TExecutionContext>(Registry, this, field.Name, batchFunc, graphType);
            return ReplaceField(field, result);
        }

        public BatchEnumerableField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Field<TEntity, TExecutionContext> field, Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc)
        {
            var graphType = GetGraphQLTypeDescriptor<TReturnType>(field);
            var result = new BatchEnumerableField<TEntity, TReturnType, TExecutionContext>(Registry, this, field.Name, batchFunc, graphType);
            return ReplaceField(field, result);
        }

        public BatchField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Field<TEntity, TExecutionContext> field, Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc)
        {
            var graphType = GetGraphQLTypeDescriptor<TReturnType>(field);
            var result = new BatchField<TEntity, TKeyType, TReturnType, TExecutionContext>(Registry, this, field.Name, keySelector, batchFunc, graphType);
            return ReplaceField(field, result);
        }

        public BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Field<TEntity, TExecutionContext> field, Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc)
        {
            var graphType = GetGraphQLTypeDescriptor<TReturnType>(field);
            var result = new BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext>(Registry, this, field.Name, keySelector, batchFunc, graphType);
            return ReplaceField(field, result);
        }

        public BatchClassField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Field<TEntity, TExecutionContext> field, Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            return FromBatch(field, Registry.WrapFuncByUnusedContext(batchFunc), build);
        }

        public BatchEnumerableClassField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Field<TEntity, TExecutionContext> field, Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            return FromBatch(field, Registry.WrapFuncByUnusedContext(batchFunc), build);
        }

        public BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Field<TEntity, TExecutionContext> field, Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return FromBatch(field, keySelector, Registry.WrapFuncByUnusedContext(batchFunc), build);
        }

        public BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Field<TEntity, TExecutionContext> field, Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return FromBatch(field, keySelector, Registry.WrapFuncByUnusedContext(batchFunc), build);
        }

        public BatchField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Field<TEntity, TExecutionContext> field, Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc)
        {
            return FromBatch(field, Registry.WrapFuncByUnusedContext(batchFunc));
        }

        public BatchEnumerableField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Field<TEntity, TExecutionContext> field, Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc)
        {
            return FromBatch(field, Registry.WrapFuncByUnusedContext(batchFunc));
        }

        public BatchField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Field<TEntity, TExecutionContext> field, Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc)
        {
            return FromBatch(field, keySelector, Registry.WrapFuncByUnusedContext(batchFunc));
        }

        public BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Field<TEntity, TExecutionContext> field, Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc)
        {
            return FromBatch(field, keySelector, Registry.WrapFuncByUnusedContext(batchFunc));
        }

        public QueryableField<TEntity, TSelectType, TExecutionContext> FromIQueryableClass<TSelectType>(
            Field<TEntity, TExecutionContext> field,
            Func<TExecutionContext, IQueryable<TSelectType>> query,
            Expression<Func<TEntity, TSelectType, bool>> condition,
            Action<IInlineObjectBuilder<TSelectType, TExecutionContext>> build)
            where TSelectType : class
        {
            var graphType = GetGraphQLTypeDescriptor(field, build);
            var result = new QueryableField<TEntity, TSelectType, TExecutionContext>(Registry, this, field.Name, query, condition, graphType);
            return ReplaceField(field, result);
        }

        public EnumerableField<TEntity, TChildEntity, TExecutionContext> FromLoader<TLoader, TChildLoader, TChildEntity>(
            Field<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            RelationType relationType,
            Expression<Func<TChildEntity, TEntity>> navigationProperty,
            Expression<Func<TEntity, TChildEntity>> reverseNavigationProperty,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> graphResultType)
            where TChildEntity : class
            where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
            where TLoader : Loader<TEntity, TExecutionContext>, new()
        {
            var factorizationResult = ExpressionHelpers.FactorizeCondition(condition);
            var equality = Expression.Equal(factorizationResult.LeftExpression.Body, factorizationResult.RightExpression.Body);
            var lambda = Expression.Lambda<Func<TEntity, TChildEntity, bool>>(equality, condition.Parameters);

            var result = ReplaceField(field, new LoaderField<TLoader, TChildLoader, TEntity, TChildEntity, TExecutionContext>(Registry, this, field.Name, lambda, relationType, navigationProperty, reverseNavigationProperty, graphResultType));

            if (factorizationResult.RightCondition != null)
            {
                return result.ApplyWhere(factorizationResult.RightCondition);
            }

            return result;
        }

        public LoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext> FromLoader<TChildLoader, TChildEntity>(
            Field<TEntity, TExecutionContext> field,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> graphResultType)
            where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
            where TChildEntity : class
        {
            var result = new LoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>(Registry, this, field.Name, condition, graphResultType);
            return ReplaceField(field, result);
        }

        public UnionField<TEntity, TExecutionContext> ApplyUnion<TLastElementType>(Field<TEntity, TExecutionContext> field, Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>> build, bool isList)
            where TLastElementType : class
        {
            var unionField = UnionField.Create(Registry, this, field.Name, build, isList);
            return ReplaceField(field, unionField);
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Field<TEntity, TExecutionContext> field, Func<TExecutionContext, TEntity, TReturnType> resolve, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            var graphType = GetGraphQLTypeDescriptor<TReturnType>(field);
            var resolvedField = ResolvedField.Create(Registry, this, field.Name, graphType, Resolvers.ConvertFieldResolver(resolve, doesDependOnAllFields), field.Arguments, doesDependOnAllFields, optionsBuilder);
            return ReplaceField(field, resolvedField);
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Field<TEntity, TExecutionContext> field, Func<TExecutionContext, TEntity, Task<TReturnType>> resolve, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            var graphType = GetGraphQLTypeDescriptor<TReturnType>(field);
            var resolvedField = ResolvedField.Create(Registry, this, field.Name, graphType, Resolvers.ConvertFieldResolver(resolve, doesDependOnAllFields), field.Arguments, doesDependOnAllFields, optionsBuilder);
            return ReplaceField(field, resolvedField);
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Field<TEntity, TExecutionContext> field, Func<TExecutionContext, TEntity, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class
        {
            var graphType = GetGraphQLTypeDescriptor(field, build);
            var resolvedField = ResolvedField.Create(Registry, this, field.Name, graphType, Resolvers.ConvertFieldResolver(resolve, doesDependOnAllFields), field.Arguments, doesDependOnAllFields, optionsBuilder);
            return ReplaceField(field, resolvedField);
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Field<TEntity, TExecutionContext> field, Func<TExecutionContext, TEntity, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class
        {
            var graphType = GetGraphQLTypeDescriptor(field, build);
            var resolvedField = ResolvedField.Create(Registry, this, field.Name, graphType, Resolvers.ConvertFieldResolver(resolve, doesDependOnAllFields), field.Arguments, doesDependOnAllFields, optionsBuilder);
            return ReplaceField(field, resolvedField);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Field<TEntity, TExecutionContext> field, Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            var graphType = GetGraphQLTypeDescriptor<TReturnType>(field).MakeListDescriptor();
            var resolvedField = ResolvedField.Create(Registry, this, field.Name, graphType, Resolvers.ConvertFieldResolver(resolve, doesDependOnAllFields), field.Arguments, doesDependOnAllFields, optionsBuilder);
            return ReplaceField(field, resolvedField);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Field<TEntity, TExecutionContext> field, Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            var graphType = GetGraphQLTypeDescriptor<TReturnType>(field).MakeListDescriptor();
            var resolvedField = ResolvedField.Create(Registry, this, field.Name, graphType, Resolvers.ConvertFieldResolver(resolve, doesDependOnAllFields), field.Arguments, doesDependOnAllFields, optionsBuilder);
            return ReplaceField(field, resolvedField);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Field<TEntity, TExecutionContext> field, Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class
        {
            var graphType = GetGraphQLTypeDescriptor(field, build).MakeListDescriptor();
            var resolvedField = ResolvedField.Create(Registry, this, field.Name, graphType, Resolvers.ConvertFieldResolver(resolve, doesDependOnAllFields), field.Arguments, doesDependOnAllFields, optionsBuilder);
            return ReplaceField(field, resolvedField);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Field<TEntity, TExecutionContext> field, Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class
        {
            // TODO throw if TReturnType is IEnumerable
            var graphType = GetGraphQLTypeDescriptor(field, build).MakeListDescriptor();
            var resolvedField = ResolvedField.Create(Registry, this, field.Name, graphType, Resolvers.ConvertFieldResolver(resolve, doesDependOnAllFields), field.Arguments, doesDependOnAllFields, optionsBuilder);
            return ReplaceField(field, resolvedField);
        }

        public ArgumentedField<TEntity, TArgType, TExecutionContext> ApplyArgument<TArgType>(Field<TEntity, TExecutionContext> field, string argName)
        {
            var argumentedField = new ArgumentedField<TEntity, TArgType, TExecutionContext>(Registry, this, field.Name, new Arguments<TArgType, TExecutionContext>(Registry, argName));
            return ReplaceField(field, argumentedField);
        }

        public ArgumentedField<TEntity, Expression<Func<TEntity1, bool>>, TExecutionContext> ApplyFilterArgument<TProjection, TEntity1>(Field<TEntity, TExecutionContext> field, string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = new ArgumentedField<TEntity, Expression<Func<TEntity1, bool>>, TExecutionContext>(Registry, this, field.Name, new Arguments<Expression<Func<TEntity1, bool>>, TExecutionContext>(Registry, argName, typeof(TProjection), typeof(TEntity1)));
            return ReplaceField(field, argumentedField);
        }

        public ArgumentedField<TEntity, TArgType, TExecutionContext> ApplyPayloadField<TArgType>(Field<TEntity, TExecutionContext> field, string argName)
        {
            var payloadedField = new ArgumentedField<TEntity, TArgType, TExecutionContext>(Registry, this, field.Name, new PayloadFields<TArgType, TExecutionContext>(field.Name, Registry, argName));
            return ReplaceField(field, payloadedField);
        }

        public ArgumentedField<TEntity, Expression<Func<TEntity1, bool>>, TExecutionContext> ApplyFilterPayloadField<TProjection, TEntity1>(Field<TEntity, TExecutionContext> field, string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = new ArgumentedField<TEntity, Expression<Func<TEntity1, bool>>, TExecutionContext>(Registry, this, field.Name, new PayloadFields<Expression<Func<TEntity1, bool>>, TExecutionContext>(field.Name, Registry, argName, typeof(TProjection), typeof(TEntity1)));
            return ReplaceField(field, argumentedField);
        }

        public SelectField<TEntity, TReturnType, TExecutionContext> ApplySelect<TReturnType>(Field<TEntity, TExecutionContext> field, IResolver<TEntity> resolver)
        {
            var graphType = GetGraphQLTypeDescriptor<TReturnType>(field);
            return ReplaceField(field, new SelectField<TEntity, TReturnType, TExecutionContext>(Registry, this, field.Name, resolver, graphType, field.Arguments));
        }

        public SelectField<TEntity, TReturnType, TExecutionContext> ApplySelect<TReturnType>(Field<TEntity, TExecutionContext> field, IResolver<TEntity> resolver, IGraphTypeDescriptor<TExecutionContext> graphType)
        {
            return ReplaceField(field, new SelectField<TEntity, TReturnType, TExecutionContext>(Registry, this, field.Name, resolver, graphType, field.Arguments));
        }

        public SelectField<TEntity, TReturnType, TExecutionContext> ApplySelect<TReturnType>(Field<TEntity, TExecutionContext> field, IResolver<TEntity> resolver, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var graphType = GetGraphQLTypeDescriptor(field, build);
            return ReplaceField(field, new SelectField<TEntity, TReturnType, TExecutionContext>(Registry, this, field.Name, resolver, graphType, field.Arguments));
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TFromType, TAnotherReturnType>(
            Field<TEntity, TExecutionContext> field,
            IBatchResolver<TEntity, TFromType> firstResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>> build)
            where TAnotherReturnType : class
        {
            var batchResolver = new BatchResolver<TEntity, TAnotherReturnType, TExecutionContext>(field.Name, batchFunc, ProxyAccessor);

            return ApplyBatchUnion(field, firstResolver, firstGraphType, batchResolver, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TFromType, TAnotherReturnType>(
            Field<TEntity, TExecutionContext> field,
            IBatchResolver<TEntity, TFromType> firstResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            Func<IEnumerable<TEntity>, IDictionary<TEntity, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>> build)
            where TAnotherReturnType : class
        {
            var batchResolver = new BatchResolver<TEntity, TAnotherReturnType, TExecutionContext>(field.Name, (ctx, e) => batchFunc(e), ProxyAccessor);

            return ApplyBatchUnion(field, firstResolver, firstGraphType, batchResolver, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TFromType, TKeyType, TAnotherReturnType>(
            Field<TEntity, TExecutionContext> field,
            IBatchResolver<TEntity, TFromType> firstResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>> build)
            where TAnotherReturnType : class
        {
            var batchResolver = new BatchKeyResolver<TEntity, TKeyType, TAnotherReturnType, TExecutionContext>(field.Name, keySelector, batchFunc, ProxyAccessor);

            return ApplyBatchUnion(field, firstResolver, firstGraphType, batchResolver, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TFromType, TKeyType, TAnotherReturnType>(
            Field<TEntity, TExecutionContext> field,
            IBatchResolver<TEntity, TFromType> firstResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, IDictionary<TKeyType, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>> build)
            where TAnotherReturnType : class
        {
            var batchResolver = new BatchKeyResolver<TEntity, TKeyType, TAnotherReturnType, TExecutionContext>(field.Name, keySelector, (ctx, items) => batchFunc(items), ProxyAccessor);

            return ApplyBatchUnion(field, firstResolver, firstGraphType, batchResolver, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TFromType, TAnotherReturnType>(
            Field<TEntity, TExecutionContext> field,
            IBatchResolver<TEntity, TFromType> firstResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>> build)
            where TAnotherReturnType : class
        {
            var batchResolver = new BatchTaskResolver<TEntity, TAnotherReturnType, TExecutionContext>(field.Name, batchFunc, ProxyAccessor);

            return ApplyBatchUnion(field, firstResolver, firstGraphType, batchResolver, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TFromType, TAnotherReturnType>(
            Field<TEntity, TExecutionContext> field,
            IBatchResolver<TEntity, TFromType> firstResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>> build)
            where TAnotherReturnType : class
        {
            var batchResolver = new BatchTaskResolver<TEntity, TAnotherReturnType, TExecutionContext>(field.Name, (ctx, e) => batchFunc(e), ProxyAccessor);

            return ApplyBatchUnion(field, firstResolver, firstGraphType, batchResolver, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TFromType, TKeyType, TAnotherReturnType>(
            Field<TEntity, TExecutionContext> field,
            IBatchResolver<TEntity, TFromType> firstResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>> build)
            where TAnotherReturnType : class
        {
            var batchResolver = new BatchTaskKeyResolver<TEntity, TKeyType, TAnotherReturnType, TExecutionContext>(field.Name, keySelector, batchFunc, ProxyAccessor);

            return ApplyBatchUnion(field, firstResolver, firstGraphType, batchResolver, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TFromType, TKeyType, TAnotherReturnType>(
            Field<TEntity, TExecutionContext> field,
            IBatchResolver<TEntity, TFromType> firstResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>> build)
            where TAnotherReturnType : class
        {
            var batchResolver = new BatchTaskKeyResolver<TEntity, TKeyType, TAnotherReturnType, TExecutionContext>(field.Name, keySelector, (ctx, items) => batchFunc(items), ProxyAccessor);

            return ApplyBatchUnion(field, firstResolver, firstGraphType, batchResolver, build);
        }

        public BatchUnionField<TEntity, TExecutionContext> ApplyBatchUnion<TFromType, TAnotherReturnType>(
            Field<TEntity, TExecutionContext> field,
            IBatchResolver<TEntity, TFromType> firstResolver,
            IGraphTypeDescriptor<TExecutionContext> firstGraphType,
            IBatchResolver<TEntity, TAnotherReturnType> secondResolver,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>> build)
            where TAnotherReturnType : class
        {
            var secondGraphType = GetGraphQLTypeDescriptor(field, build);

            return ReplaceField(field, new BatchUnionField<TEntity, TExecutionContext>(
                Registry,
                this,
                field.Name,
                firstResolver,
                secondResolver,
                firstGraphType,
                typeof(TFromType),
                secondGraphType,
                typeof(TAnotherReturnType)));
        }

        protected virtual void OnConfigure()
        {
        }

        protected virtual void SetGraphQLTypeName(string oldName, string newName)
        {
            Registry.SetGraphQLTypeName<TEntity>(oldName, newName);
        }

        protected virtual BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> GetBaseObjectGraphTypeConfiguratorForFilters() => null;

        private protected abstract void ValidateFields();

        private IField<TExecutionContext> AddField(PropertyInfo propertyInfo)
        {
            var propertyType = propertyInfo.PropertyType;
            var expressionType = typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(typeof(TEntity), propertyType));
            LambdaExpression expression = propertyInfo.MakePropertyLambdaExpression();
            MethodInfo addFieldMethodInfo = null;
            if (propertyType == typeof(string))
            {
                addFieldMethodInfo = typeof(BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext>).GetMethod(
                    nameof(AddField),
                    new[] { typeof(string), expressionType, typeof(string) });
            }
            else if (propertyType.IsEnumerableOfT())
            {
                var underlyingType = TypeExtensions.GetEnumerableElementType(propertyType);
                var enumerableType = typeof(IEnumerable<>).MakeGenericType(underlyingType);
                expressionType = typeof(Expression<>).MakeGenericType(typeof(Func<,>).MakeGenericType(typeof(TEntity), enumerableType));
                expression = propertyInfo.MakePropertyLambdaExpression(enumerableType);

                if (underlyingType == typeof(string))
                {
                    addFieldMethodInfo = GetType().GetMethod(
                        nameof(AddField),
                        new[] { typeof(string), expressionType, typeof(string) });
                }
                else if (underlyingType.IsValueType)
                {
                    addFieldMethodInfo = GetType().GetGenericMethod(
                        nameof(AddField),
                        new[] { underlyingType.UnwrapIfNullable() },
                        new[] { typeof(string), expressionType, typeof(string) });
                }
                else
                {
                    addFieldMethodInfo = GetType().GetGenericMethod(
                        nameof(AddObjectField),
                        new[] { underlyingType },
                        new[] { typeof(string), expressionType, typeof(string) });
                }
            }
            else if (propertyType.IsValueType)
            {
                addFieldMethodInfo = GetType().GetGenericMethod(
                    nameof(AddField),
                    new[] { propertyType.UnwrapIfNullable() },
                    new[] { typeof(string), expressionType, typeof(string) });
            }
            else
            {
                addFieldMethodInfo = GetType().GetGenericMethod(
                    nameof(AddObjectField),
                    new[] { propertyType },
                    new[] { typeof(string), expressionType, typeof(string) });
            }

            var name = propertyInfo.Name;

            return addFieldMethodInfo.InvokeAndHoistBaseException<IField<TExecutionContext>>(this, name, expression, null);
        }

        private TField InternalAddField<TField>(TField field, string deprecationReason)
            where TField : Field<TEntity, TExecutionContext>
        {
            _fields.Add(field);
            field.DeprecationReason = deprecationReason;
            return field;
        }
    }
}