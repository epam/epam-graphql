// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Builders.Loader.Implementations;
using Epam.GraphQL.Configuration.Implementations;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Configuration.Implementations.Relations;
using Epam.GraphQL.Enums;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Filters;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Metadata;
using Epam.GraphQL.Search;
using Epam.GraphQL.Types;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;

#nullable enable

namespace Epam.GraphQL.Configuration
{
    internal class RelationRegistry<TExecutionContext> : IServiceProvider, IRegistry
    {
        // TODO It seems like `RelationRegistry` is not good name for this class. Is `ConfigurationRegistry` better choice?
        private static readonly ReadOnlyDictionary<Type, Type> _structTypeMap = new(
            new Dictionary<Type, Type>
            {
                [typeof(int)] = typeof(IntGraphType),
                [typeof(long)] = typeof(IntGraphType),
                [typeof(double)] = typeof(FloatGraphType),
                [typeof(float)] = typeof(FloatGraphType),
                [typeof(decimal)] = typeof(DecimalGraphType),
                [typeof(bool)] = typeof(BooleanGraphType),
                [typeof(DateTime)] = typeof(DateTimeGraphType),
                [typeof(DateTimeOffset)] = typeof(DateTimeOffsetGraphType),
                [typeof(TimeSpan)] = typeof(TimeSpanSecondsGraphType),
                [typeof(Guid)] = typeof(IdGraphType),
            });

        private readonly Dictionary<(Type LoaderType, Type EntityType), Relations> _relationMap = new();
        private readonly Dictionary<Type, Relations> _relationMapPostponedForSave = new();
        private readonly Dictionary<Type, PropertyInfo> _primaryKeys = new();
        private readonly ConcurrentDictionary<(Type Loader, Type Entity), IObjectGraphTypeConfigurator<TExecutionContext>> _loadersToObjectGraphTypeConfiguratorsMap = new();
        private readonly ConcurrentDictionary<(Type Loader, Type Entity), IObjectGraphTypeConfigurator<TExecutionContext>> _loadersToInputObjectGraphTypeConfiguratorsMap = new();
        private readonly ConcurrentDictionary<Type, IObjectGraphTypeConfigurator<TExecutionContext>> _autoEntityTypesToConfiguratorsMap = new();
        private readonly ConcurrentDictionary<Type, IObjectGraphTypeConfigurator<TExecutionContext>> _inputAutoEntityTypesToConfiguratorsMap = new();
        private readonly Dictionary<(Type InlineType, Delegate? Builder, IField<TExecutionContext> Parent, bool IsInput), IInlineGraphTypeResolver<TExecutionContext>> _inlineConfiguratorsToResolversMap = new(
            new ValueTupleEqualityComparer<Type, Delegate?, IField<TExecutionContext>, bool>(thirdItemComparer: ReferenceEqualityComparer.Instance));

        private readonly ConcurrentDictionary<Type, object> _cache = new();
        private readonly Dictionary<string, (Type Entity, Type? Projection)> _typeNamesToTypesMap = new();
        private readonly Dictionary<Type, (Type GraphType, string Name)> _enumTypesMap = new();
        private readonly HashSet<string> _enumTypeNames = new();
        private readonly ConcurrentDictionary<Delegate, Delegate> _funcsToWrappedByContextFuncsMap = new();
        private readonly HashSet<Type> _projectionEntityTypes = new();
        private readonly IServiceProvider _serviceProvider;

        public RelationRegistry(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
        }

        public object GetService(Type type) => _serviceProvider.GetService(type);

        public T GetService<T>() => _serviceProvider.GetService<T>();

        public void Register<TEntity, TChildEntity>(
            Type loaderType,
            Type childLoaderType,
            Expression<Func<TEntity, TChildEntity, bool>> relationCondition,
            Expression<Func<TChildEntity, TEntity>>? navigationProperty,
            Expression<Func<TEntity, TChildEntity>>? childNavigationProperty,
            RelationType relationType)
        {
            RegisterHelper(loaderType, childLoaderType, relationCondition, navigationProperty, childNavigationProperty, relationType);
        }

        public void ConfigureGraphType<TProjection, TEntity>(IObjectGraphType graphType)
            where TProjection : ProjectionBase<TEntity, TExecutionContext>, new()
            where TEntity : class
        {
            InitializeLoader<TProjection, TEntity>();
            var key = (typeof(TProjection), typeof(TEntity));
            _loadersToObjectGraphTypeConfiguratorsMap[key].ConfigureGraphType(graphType);
        }

        public void ConfigureGroupGraphType<TProjection, TEntity>(IObjectGraphType graphType)
            where TProjection : ProjectionBase<TEntity, TExecutionContext>, new()
            where TEntity : class
        {
            InitializeLoader<TProjection, TEntity>();
            var key = (typeof(TProjection), typeof(TEntity));
            _loadersToObjectGraphTypeConfiguratorsMap[key].ConfigureGroupGraphType(graphType);
        }

        public void ConfigureInputGraphType<TProjection, TEntity>(IInputObjectGraphType graphType)
            where TProjection : ProjectionBase<TEntity, TExecutionContext>, new()
            where TEntity : class
        {
            InitializeLoader<TProjection, TEntity>();
            var key = (typeof(TProjection), typeof(TEntity));
            _loadersToInputObjectGraphTypeConfiguratorsMap[key].ConfigureGraphType(graphType);
        }

        public IInlineGraphTypeResolver<TReturnType, TExecutionContext> Register<TReturnType>(IField<TExecutionContext> parent, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build, bool isInputType)
            where TReturnType : class
        {
            if (typeof(TReturnType).IsValueType || typeof(TReturnType) == typeof(string))
            {
                throw new NotSupportedException($"Call of Configure method is not supported for a field type `{typeof(TReturnType).Name}`.");
            }

            return (IInlineGraphTypeResolver<TReturnType, TExecutionContext>)_inlineConfiguratorsToResolversMap.GetOrAdd(
                (typeof(TReturnType), build, parent, isInputType),
                key => new InlineObjectBuilder<TReturnType, TExecutionContext>(key.Parent, this, (Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>?)key.Builder, key.IsInput));
        }

        public ObjectGraphTypeConfigurator<TProjection, TEntity, TExecutionContext> Register<TProjection, TEntity>(IField<TExecutionContext>? parent)
            where TProjection : ProjectionBase<TEntity, TExecutionContext>, new()
            where TEntity : class
        {
            var key = (typeof(TProjection), typeof(TEntity));
            return (ObjectGraphTypeConfigurator<TProjection, TEntity, TExecutionContext>)_loadersToObjectGraphTypeConfiguratorsMap.GetOrAdd(
                key,
                _ => new ObjectGraphTypeConfigurator<TProjection, TEntity, TExecutionContext>(parent, this));
        }

        public ObjectGraphTypeConfigurator<TEntity, TExecutionContext> Register<TEntity>(Type projectionType, IField<TExecutionContext>? parent)
            where TEntity : class
        {
            var methodInfo = GetType().GetGenericMethod(nameof(Register), new[] { projectionType, typeof(TEntity) }, new[] { typeof(IField<TExecutionContext>) });
            return methodInfo.InvokeAndHoistBaseException<ObjectGraphTypeConfigurator<TEntity, TExecutionContext>>(this, parent);
        }

        public InputObjectGraphTypeConfigurator<TProjection, TEntity, TExecutionContext> RegisterInput<TProjection, TEntity>(IField<TExecutionContext>? parent)
            where TProjection : ProjectionBase<TEntity, TExecutionContext>, new()
            where TEntity : class
        {
            var key = (typeof(TProjection), typeof(TEntity));
            return (InputObjectGraphTypeConfigurator<TProjection, TEntity, TExecutionContext>)_loadersToInputObjectGraphTypeConfiguratorsMap.GetOrAdd(
                key,
                _ => new InputObjectGraphTypeConfigurator<TProjection, TEntity, TExecutionContext>(parent, this));
        }

        public InputObjectGraphTypeConfigurator<TEntity, TExecutionContext> RegisterInput<TEntity>(Type projectionType, IField<TExecutionContext>? parent)
            where TEntity : class
        {
            var methodInfo = GetType().GetGenericMethod(nameof(RegisterInput), new[] { projectionType, typeof(TEntity) }, new[] { typeof(IField<TExecutionContext>) });
            return methodInfo.InvokeAndHoistBaseException<InputObjectGraphTypeConfigurator<TEntity, TExecutionContext>>(this, parent);
        }

        public IObjectGraphTypeConfigurator<TExecutionContext>? GetObjectGraphTypeConfigurator(Type type, Type? loaderType = null)
        {
            if (loaderType != null)
            {
                InitializeLoader(loaderType, type);
                return _loadersToObjectGraphTypeConfiguratorsMap[(loaderType, type)];
            }
            else if (_autoEntityTypesToConfiguratorsMap.TryGetValue(type, out var result))
            {
                return result;
            }

            return null;
        }

        public void ConfigureAutoObjectGraphType<TEntity>(IObjectGraphType graphType)
            where TEntity : class
        {
            _autoEntityTypesToConfiguratorsMap.GetOrAdd(typeof(TEntity), _ => RegisterAutoObjectGraphType<TEntity>())
                .ConfigureGraphType(graphType);
        }

        public void ConfigureInputAutoObjectGraphType<TEntity>(IInputObjectGraphType graphType)
            where TEntity : class
        {
            _inputAutoEntityTypesToConfiguratorsMap.GetOrAdd(typeof(TEntity), _ => RegisterInputAutoObjectGraphType<TEntity>())
                .ConfigureGraphType(graphType);
        }

        public ObjectGraphTypeConfigurator<TEntity, TExecutionContext> RegisterAutoObjectGraphType<TEntity>()
            where TEntity : class
        {
            return (ObjectGraphTypeConfigurator<TEntity, TExecutionContext>)_autoEntityTypesToConfiguratorsMap.GetOrAdd(
                typeof(TEntity),
                _ => new ObjectGraphTypeConfigurator<TEntity, TExecutionContext>(null, this, true, true));
        }

        public InputObjectGraphTypeConfigurator<TEntity, TExecutionContext> RegisterInputAutoObjectGraphType<TEntity>()
            where TEntity : class
        {
            return (InputObjectGraphTypeConfigurator<TEntity, TExecutionContext>)_inputAutoEntityTypesToConfiguratorsMap.GetOrAdd(
                typeof(TEntity),
                _ => new InputObjectGraphTypeConfigurator<TEntity, TExecutionContext>(null, this, true, true));
        }

        public void Register<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> idExpression)
        {
            if (!_primaryKeys.ContainsKey(typeof(TEntity)))
            {
                _primaryKeys.Add(typeof(TEntity), idExpression.GetPropertyInfo());
            }
        }

        public bool HasFakePropertyValues<TEntity>(Type loaderType, TEntity entity, IDictionary<string, object> propertyValues)
            where TEntity : class
        {
            if (_relationMap.TryGetValue((loaderType, typeof(TEntity)), out var childRels))
            {
                var relations = (Relations<TEntity>)childRels;
                if (relations.HasFakePropertyValues(entity, propertyValues))
                {
                    return true;
                }
            }

            return false;
        }

        public void UpdateFakePropertyValues(object? parent, object? childEntity, IDictionary<string, object?> childPropertyValues, object? fakePropertyValue, Type childLoaderType, Type childEntityType)
        {
            if (_relationMap.TryGetValue((childLoaderType, childEntityType), out var childRelations))
            {
                childRelations.UpdateFakeProperties(parent, childEntity, childPropertyValues, fakePropertyValue);
            }
        }

        public Task<bool> CanViewParentAsync<TChildEntity>(Type childLoaderType, GraphQLContext<TExecutionContext> context, TChildEntity entity)
            where TChildEntity : class
        {
            if (_relationMap.TryGetValue((childLoaderType, typeof(TChildEntity)), out var rel))
            {
                var relations = (Relations<TChildEntity>)rel;
                return relations.CanViewParentAsync(context, entity);
            }

            return Task.FromResult(true);
        }

        public void RegisterRelationPostponedForSave<TEntity, TChildEntity, TChildEntityLoader, TPropertyType, TChildPropertyType>(string propName, Predicate<TPropertyType> isFakePropValue)
            where TChildEntity : class
            where TChildEntityLoader : Loader<TChildEntity, TExecutionContext>, IIdentifiableLoader, new()
        {
            Relations<TEntity> childRelations;
            if (_relationMapPostponedForSave.TryGetValue(typeof(TEntity), out var childRel))
            {
                childRelations = (Relations<TEntity>)childRel;
            }
            else
            {
                childRelations = new Relations<TEntity>();
                _relationMapPostponedForSave.Add(typeof(TEntity), childRelations);
            }

            var loader = ResolveLoader<TChildEntityLoader, TChildEntity>();
            childRelations.Register<TPropertyType, TChildPropertyType>(propName, isFakePropValue, loader.GetId);
        }

        public bool HasFakePropertyValuesPostponedForSave<TChildEntity>(TChildEntity entity, IDictionary<string, object> propertyValues)
            where TChildEntity : class
        {
            if (_relationMapPostponedForSave.TryGetValue(typeof(TChildEntity), out var rel))
            {
                var relations = (Relations<TChildEntity>)rel;
                return relations.HasFakePropertyValues(entity, propertyValues);
            }

            return false;
        }

        public void UpdatePostponedFakePropertyValues(object? parent, object? childEntity, IDictionary<string, object?> childPropertyValues, object? fakePropertyValue, Type childEntityType)
        {
            if (_relationMapPostponedForSave.TryGetValue(childEntityType, out var relations))
            {
                relations.UpdateFakeProperties(parent, childEntity, childPropertyValues, fakePropertyValue);
            }
        }

        public TypeMetadata? GetMetadata(IGraphType graphType)
        {
            if (graphType is IHasGetEntityType p and IObjectGraphType og)
            {
                var loaderType = p.GetProjectionType();
                var entityType = p.GetEntityType();
                FieldType[]? primaryKeys = null;
                IEnumerable<ForeignKeyMetadata>? foreignKeys = null;
                if (_primaryKeys.TryGetValue(entityType, out var propertyInfo))
                {
                    var primaryKeyField = og.Fields.SingleOrDefault(field => field.Name.Equals(propertyInfo.Name, StringComparison.OrdinalIgnoreCase));
                    if (primaryKeyField != null)
                    {
                        primaryKeys = new[] { primaryKeyField };
                    }
                }

                if (_relationMap.TryGetValue((loaderType, entityType), out var relations))
                {
                    foreignKeys = relations.Items.Select(rel => rel.GetForeignKeyMetadata(og)).Where(fk => fk != null)!;
                }

                return new TypeMetadata(primaryKeys, foreignKeys);
            }

            return null;
        }

        public TLoader ResolveLoader<TLoader, TEntity>()
            where TLoader : ProjectionBase<TEntity, TExecutionContext>, new()
            where TEntity : class
        {
            return (TLoader)ResolveLoader<TEntity>(typeof(TLoader));
        }

        public ProjectionBase<TExecutionContext> ResolveLoader(Type projectionType, Type entityType)
        {
            var methodInfo = GetType().GetGenericMethod(
                nameof(ResolveLoader),
                new[] { projectionType, entityType },
                Type.EmptyTypes);

            return methodInfo.InvokeAndHoistBaseException<ProjectionBase<TExecutionContext>>(this);
        }

        public ProjectionBase<TEntity, TExecutionContext> ResolveLoader<TEntity>(Type type)
            where TEntity : class
        {
            if (_cache.ContainsKey(type))
            {
                return (ProjectionBase<TEntity, TExecutionContext>)_cache[type];
            }

            var loader = (ProjectionBase<TEntity, TExecutionContext>)type.CreateInstanceAndHoistBaseException();
            _cache[type] = loader;
            _projectionEntityTypes.Add(typeof(TEntity));
            loader.Registry = this;
            loader.AfterConstruction();
            loader.Configure();
            loader.ConfigureInput();
            return loader;
        }

        public IMutableLoader<TExecutionContext> ResolveLoader(Type type)
        {
            if (_cache.ContainsKey(type))
            {
                return (IMutableLoader<TExecutionContext>)_cache[type];
            }

            var loader = (ProjectionBase<TExecutionContext>)type.CreateInstanceAndHoistBaseException();
            _cache[type] = loader;

            var baseType = TypeHelpers.FindMatchingGenericBaseType(type, typeof(Projection<,>));

            if (baseType == null)
            {
                throw new ArgumentException($"Cannot resolve loader of type {type}", nameof(type));
            }

            _projectionEntityTypes.Add(baseType.GenericTypeArguments[0]);
            loader.Registry = this;
            loader.AfterConstruction();
            loader.Configure();
            loader.ConfigureInput();
            return (IMutableLoader<TExecutionContext>)loader;
        }

        public IFilter<TEntity, TExecutionContext> ResolveFilter<TEntity>(Type loaderFilterType)
        {
            return (IFilter<TEntity, TExecutionContext>)_cache.GetOrAdd(loaderFilterType, type => loaderFilterType.CreateInstanceAndHoistBaseException());
        }

        public ISearcher<TEntity, TExecutionContext> ResolveSearcher<TEntity>(Type loaderSearcherType)
        {
            return (ISearcher<TEntity, TExecutionContext>)_cache.GetOrAdd(loaderSearcherType, type => loaderSearcherType.CreateInstanceAndHoistBaseException());
        }

        public IObjectGraphType ResolveObjectGraphTypeWrapper<TLoader, TEntity>()
            where TLoader : ProjectionBase<TEntity, TExecutionContext>, new()
            where TEntity : class
        {
            return (IObjectGraphType)_serviceProvider.GetRequiredService(GetEntityGraphType<TLoader, TEntity>());
        }

        public string GetGraphQLTypeName<TEntity>(bool isInput, IField<TExecutionContext>? parent)
            where TEntity : class
        {
            return GetGraphQLTypeName(typeof(TEntity), null, isInput, parent);
        }

        public string GetGraphQLTypeName(Type entityType, Type? projectionType, bool isInput, IField<TExecutionContext>? parent, string? name = null)
        {
            name ??= entityType.GraphQLTypeName(isInput);

            if (parent != null)
            {
                name = parent.GetGraphQLTypePrefix();
                UnregisterProjectionType(name);
                RegisterProjectionType(name, entityType, projectionType);
                return name;
            }

            if (!IsTypeNameRegistered(name))
            {
                RegisterProjectionType(name, entityType, projectionType);
                return name;
            }

            var counter = 1;
            while (IsTypeNameRegistered($"{name}{counter}"))
            {
                counter++;
            }

            name = $"{name}{counter}";
            RegisterProjectionType(name, entityType, projectionType);

            return name;
        }

        public string GetGraphQLAutoTypeName<TEntity>(bool isInput)
            where TEntity : class
        {
            return GetGraphQLAutoTypeName(typeof(TEntity), isInput);
        }

        public string GetGraphQLAutoTypeName(Type type, bool isInput)
        {
            var prefix = string.Empty;

            if (_projectionEntityTypes.Contains(type))
            {
                prefix = "Auto";
            }

            var name = $"{prefix}{type.GraphQLTypeName(isInput)}";

            if (!IsTypeNameRegistered(name))
            {
                RegisterProjectionType(name, type, null);
                return name;
            }

            if (TryGetRegisteredType(name, out var existingType))
            {
                if (existingType.Entity == type)
                {
                    return name;
                }
            }

            var counter = 1;
            while (IsTypeNameRegistered($"{name}{counter}"))
            {
                if (TryGetRegisteredType($"{name}{counter}", out existingType) && existingType.Entity == type)
                {
                    return $"{name}{counter}";
                }

                counter++;
            }

            name = $"{name}{counter}";
            RegisterProjectionType(name, type, null);

            return name;
        }

        public string GetProjectionTypeName<TProjection, TEntity>(bool isInput)
            where TProjection : ProjectionBase<TEntity, TExecutionContext>, new()
            where TEntity : class
        {
            var projectionType = typeof(TProjection);
            var entityType = typeof(TEntity);

            var typeName = projectionType.Name.Split('`')[0];
            const string LoaderString = "Loader";

            if (typeName.EndsWith(LoaderString, StringComparison.Ordinal))
            {
                var suffix = entityType.IsGenericType ? $"Of{string.Join("And", entityType.GetGenericArguments().Select(t => t.GraphQLTypeName(false)))}" : string.Empty;
                var prefix = isInput ? "Input" : string.Empty;
                typeName = typeName.Substring(0, typeName.Length - LoaderString.Length);
                var name = $"{prefix}{typeName}{suffix}";

                if (TryGetRegisteredType(name, out var types))
                {
                    if (types.Entity == entityType && types.Projection == projectionType)
                    {
                        return name;
                    }
                }
                else if (!IsTypeNameRegistered(name))
                {
                    return name;
                }

                var counter = 1;
                while (IsTypeNameRegistered($"{name}{counter}"))
                {
                    counter++;
                }

                name = $"{name}{counter}";

                return name;
            }

            string? possibleName = null;

            if (TypeHelpers.FindMatchingGenericBaseType(projectionType, typeof(Query<>)) != null || TypeHelpers.FindMatchingGenericBaseType(projectionType, typeof(Mutation<>)) != null)
            {
                possibleName = projectionType.GraphQLTypeName(false);
            }

            return GetGraphQLTypeName(entityType, projectionType, isInput, null, possibleName);
        }

        public void SetGraphQLTypeName<TEntity>(string? oldName, string newName)
            where TEntity : class
        {
            SetGraphQLTypeName(null, typeof(TEntity), oldName, newName);
        }

        public void SetGraphQLTypeName<TProjection, TEntity>(string? oldName, string newName)
            where TProjection : ProjectionBase<TEntity, TExecutionContext>
            where TEntity : class
        {
            SetGraphQLTypeName(typeof(TProjection), typeof(TEntity), oldName, newName);
        }

        public Func<TExecutionContext, TArgType, TResultType> WrapFuncByUnusedContext<TArgType, TResultType>(Func<TArgType, TResultType> func)
        {
            return (Func<TExecutionContext, TArgType, TResultType>)_funcsToWrappedByContextFuncsMap.GetOrAdd(func, key =>
            {
                Func<TExecutionContext, TArgType, TResultType> result = (context, arg) => ((Func<TArgType, TResultType>)key)(arg);
                return result;
            });
        }

        public Type GetEntityGraphType<TProjection, TEntity>()
            where TProjection : ProjectionBase<TEntity, TExecutionContext>
            where TEntity : class
        {
            return GetEntityGraphType(typeof(TProjection), typeof(TEntity));
        }

        public Type GetEntityGraphType(Type projectionType, Type entityType)
        {
            var baseType = GetPropperBaseProjectionType(projectionType, entityType);

            if (baseType != projectionType)
            {
                var loader = ResolveLoader(projectionType, entityType);
                loader.GetObjectGraphTypeConfigurator().ProxyAccessor.Configure();
            }

            return typeof(EntityGraphType<,,>).MakeGenericType(baseType, entityType, typeof(TExecutionContext));
        }

        public Type GetInputEntityGraphType<TProjection, TEntity>()
            where TProjection : ProjectionBase<TEntity, TExecutionContext>
            where TEntity : class
        {
            return GetInputEntityGraphType(typeof(TProjection), typeof(TEntity));
        }

        public Type GetInputEntityGraphType(Type projectionType, Type entityType)
        {
            var baseType = GetPropperBaseProjectionType(projectionType, entityType);

            if (baseType != projectionType)
            {
                var loader = ResolveLoader(projectionType, entityType);
                loader.GetInputObjectGraphTypeConfigurator().ProxyAccessor.Configure();
            }

            return typeof(InputEntityGraphType<,,>).MakeGenericType(baseType, entityType, typeof(TExecutionContext));
        }

        public Type GetSubmitOutputItemGraphType(Type projectionType, Type entityType, Type idType)
        {
            return typeof(SubmitOutputItemGraphType<,,,>).MakeGenericType(GetPropperBaseProjectionType(projectionType, entityType), entityType, idType, typeof(TExecutionContext));
        }

        public Type GetPropperBaseProjectionType(Type projectionType, Type entityType) =>
            GetPropperBaseProjectionType(projectionType, entityType, (first, second) => first.Equals(second));

        public Type GetPropperBaseProjectionType(
            Type projectionType,
            Type entityType,
            Func<IObjectGraphTypeConfigurator<TExecutionContext>, IObjectGraphTypeConfigurator<TExecutionContext>, bool> equalPredicate)
        {
            var foundType = projectionType;
            var baseType = projectionType.BaseType;

            while (true)
            {
                if (baseType == typeof(object))
                {
                    throw new ArgumentOutOfRangeException(nameof(projectionType));
                }

                if (baseType.IsGenericType)
                {
                    var genericTypeDefinition = baseType.GetGenericTypeDefinition();
                    if (genericTypeDefinition.Assembly == GetType().Assembly)
                    {
                        return foundType;
                    }
                }

                if (baseType.IsAbstract)
                {
                    return foundType;
                }

                var projection = ResolveLoader(projectionType, entityType);
                var baseLoader = ResolveLoader(baseType, entityType);

                if (equalPredicate(baseLoader.GetObjectGraphTypeConfigurator(), projection.GetObjectGraphTypeConfigurator())
                    && equalPredicate(baseLoader.GetInputObjectGraphTypeConfigurator(), projection.GetInputObjectGraphTypeConfigurator()))
                {
                    foundType = baseType;
                }

                baseType = baseType.BaseType;
            }
        }

        public IGraphTypeDescriptor<TReturnType, TExecutionContext> GetGraphTypeDescriptor<TReturnType>(IField<TExecutionContext> parent, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new ObjectGraphTypeDescriptor<TReturnType, TExecutionContext>(parent, this, build, false);
        }

        public IGraphTypeDescriptor<TReturnType, TExecutionContext> GetInputGraphTypeDescriptor<TReturnType>(IField<TExecutionContext> parent, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new ObjectGraphTypeDescriptor<TReturnType, TExecutionContext>(parent, this, build, true);
        }

        public IGraphTypeDescriptor<TReturnType, TExecutionContext> GetGraphTypeDescriptor<TReturnType>(IField<TExecutionContext> parent) => GetGraphTypeDescriptor<TReturnType>(parent, false);

        public IGraphTypeDescriptor<TReturnType, TExecutionContext> GetInputGraphTypeDescriptor<TReturnType>(IField<TExecutionContext> parent) => GetGraphTypeDescriptor<TReturnType>(parent, true);

        public IGraphTypeDescriptor<TEntity, TExecutionContext> GetGraphTypeDescriptor<TProjection, TEntity>()
            where TProjection : ProjectionBase<TEntity, TExecutionContext>, new()
            where TEntity : class
        {
            return new EntityGraphTypeDescriptor<TProjection, TEntity, TExecutionContext>(this, false);
        }

        public IGraphTypeDescriptor<TEntity, TExecutionContext> GetInputGraphTypeDescriptor<TProjection, TEntity>()
            where TProjection : ProjectionBase<TEntity, TExecutionContext>, new()
            where TEntity : class
        {
            return new EntityGraphTypeDescriptor<TProjection, TEntity, TExecutionContext>(this, true);
        }

        public Type GenerateGraphType(Type type)
            => GenerateGraphType(type, false);

        public Type GenerateInputGraphType(Type type)
            => GenerateGraphType(type, true);

        public Type GenerateGraphType<TType>(bool isInputType)
            => GenerateGraphType(typeof(TType), isInputType);

        public string GetEnumTypeName<TEnumType>()
        {
            if (_enumTypesMap.TryGetValue(typeof(TEnumType), out var result))
            {
                return result.Name;
            }

            throw new ArgumentOutOfRangeException(nameof(TEnumType));
        }

        private IGraphTypeDescriptor<TReturnType, TExecutionContext> GetGraphTypeDescriptor<TReturnType>(IField<TExecutionContext> parent, bool isInput)
        {
            if (parent == null || typeof(TReturnType).IsValueType || typeof(TReturnType) == typeof(string))
            {
                return new GraphTypeDescriptor<TReturnType, TExecutionContext>(this, isInput);
            }

            return (IGraphTypeDescriptor<TReturnType, TExecutionContext>)typeof(ObjectGraphTypeDescriptor<,>).MakeGenericType(typeof(TReturnType), typeof(TExecutionContext)).CreateInstanceAndHoistBaseException(parent, this, null, isInput);
        }

        private bool TryGetGraphValueType(Type type, out Type graphType)
        {
            if (_structTypeMap.TryGetValue(type, out graphType))
            {
                return true;
            }

            if (type.IsEnum)
            {
                RegisterEnumType(type, out graphType);
                return true;
            }

            return false;
        }

        private Type GenerateGraphType(Type type, bool generateInputType)
        {
            if (TryGetGraphValueType(type, out var graphType))
            {
                return typeof(NonNullGraphType<>).MakeGenericType(graphType);
            }

            if (type == typeof(string))
            {
                return typeof(StringGraphType);
            }

            if (type.IsEnumerableOfT())
            {
                var elementType = GenerateGraphType(type.GetEnumerableElementType(), generateInputType);
                var listGraphType = typeof(ListGraphType<>);
                return listGraphType.MakeGenericType(elementType);
            }

            if (type.IsNullable())
            {
                var elementType = type.UnwrapIfNullable();
                if (TryGetGraphValueType(elementType, out graphType))
                {
                    return graphType;
                }

                throw new ArgumentOutOfRangeException(
                    nameof(type),
                    $"The type: Nullable<{elementType.Name}> cannot be coerced effectively to a GraphQL type");
            }

            if (type.IsValueType)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(type),
                    $"The type: {type.Name} cannot be coerced effectively to a GraphQL type");
            }

            return generateInputType
                ? typeof(InputAutoObjectGraphType<,>).MakeGenericType(type, typeof(TExecutionContext))
                : typeof(AutoObjectGraphType<,>).MakeGenericType(type, typeof(TExecutionContext));
        }

        private bool IsTypeNameRegistered(string typeName)
        {
            return _typeNamesToTypesMap.ContainsKey(typeName) || _enumTypeNames.Contains(typeName);
        }

        private void RegisterProjectionType(string typeName, Type entityType, Type? projectionType)
        {
            _typeNamesToTypesMap.Add(typeName, (entityType, projectionType));
        }

        private void UnregisterProjectionType(string typeName)
        {
            _typeNamesToTypesMap.Remove(typeName);
        }

        private bool TryGetRegisteredType(string typeName, out (Type Entity, Type? Projection) result)
        {
            return _typeNamesToTypesMap.TryGetValue(typeName, out result);
        }

        private void RegisterEnumType(Type enumType, out Type graphType)
        {
            if (_enumTypesMap.TryGetValue(enumType, out var result))
            {
                graphType = result.GraphType;
                return;
            }

            var name = enumType.Name;
            var counter = 1;

            while (IsTypeNameRegistered(name))
            {
                name = $"{name}{counter++}";
            }

            graphType = typeof(EnumerationGraphType<,>).MakeGenericType(enumType, typeof(TExecutionContext));
            _enumTypesMap.Add(enumType, (graphType, name));
            _enumTypeNames.Add(name);
        }

        private void SetGraphQLTypeName(Type? projectionType, Type entityType, string? oldName, string newName)
        {
            if (TryGetRegisteredType(newName, out var oldType))
            {
                if (oldType.Entity != entityType || (oldType.Projection != null && !oldType.Projection.IsAssignableFrom(projectionType)))
                {
                    throw new InvalidOperationException($"Configuration already contains different type `{oldType.Entity.HumanizedName()}` with name `{newName}`");
                }

                return;
            }

            if (oldName != null && TryGetRegisteredType(oldName, out oldType))
            {
                if ((entityType, projectionType) != oldType)
                {
                    throw new NotSupportedException();
                }

                UnregisterProjectionType(oldName);
            }

            RegisterProjectionType(newName, entityType, projectionType);
        }

        private void Register<TEntity, TLoader, TChildEntity, TChildLoader, TProperty, TChildProperty>(
            Expression<Func<TEntity, TProperty>> property,
            Expression<Func<TChildEntity, TChildProperty>> childProperty,
            Expression<Func<TEntity, TChildEntity>>? navigationProperty,
            Expression<Func<TChildEntity, TEntity>>? childNavigationProperty,
            RelationType relationType)
            where TEntity : class
            where TLoader : Loader<TEntity, TExecutionContext>, new()
            where TChildEntity : class
            where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
        {
            var childRelations = GetOrAddChildRelations<TChildLoader, TChildEntity>();
            childRelations.Register<TEntity, TLoader, TProperty, TChildProperty, TExecutionContext>(
                this,
                property,
                navigationProperty,
                childProperty,
                childNavigationProperty,
                GetPrimaryKeyPropertyInfo,
                relationType);

            var relations = GetOrAddChildRelations<TLoader, TEntity>();
            relations.Register<TChildEntity, TChildLoader, TChildProperty, TProperty, TExecutionContext>(
                this,
                childProperty,
                childNavigationProperty,
                property,
                navigationProperty,
                GetPrimaryKeyPropertyInfo,
                relationType);
        }

        private Relations<TChildEntity> GetOrAddChildRelations<TChildLoader, TChildEntity>()
        {
            Relations<TChildEntity> childRelations;
            if (_relationMap.TryGetValue((typeof(TChildLoader), typeof(TChildEntity)), out var childRel))
            {
                childRelations = (Relations<TChildEntity>)childRel;
            }
            else
            {
                childRelations = new Relations<TChildEntity>();
                _relationMap.Add((typeof(TChildLoader), typeof(TChildEntity)), childRelations);
            }

            return childRelations;
        }

        private void RegisterHelper<TEntity, TChildEntity>(
            Type loaderType,
            Type childLoaderType,
            Expression<Func<TEntity, TChildEntity, bool>> relationCondition,
            Expression<Func<TChildEntity, TEntity>>? navigationProperty,
            Expression<Func<TEntity, TChildEntity>>? childNavigationProperty,
            RelationType relationType)
        {
            var relationInfo = relationCondition.GetExpressionInfo();
            if (relationInfo.LeftExpression.Parameters[0].Type != typeof(TEntity) || relationInfo.RightExpression.Parameters[0].Type != typeof(TChildEntity))
            {
                throw new ArgumentException(null, nameof(relationCondition));
            }

            var leftFuncType = typeof(Func<,>).MakeGenericType(typeof(TEntity), relationInfo.LeftExpression.ReturnType);
            var leftExpressionType = typeof(Expression<>).MakeGenericType(leftFuncType);

            var rightFuncType = typeof(Func<,>).MakeGenericType(typeof(TChildEntity), relationInfo.RightExpression.ReturnType);
            var rightExpressionType = typeof(Expression<>).MakeGenericType(rightFuncType);

            var registerMethodInfo = GetType().GetGenericMethod(
                nameof(Register),
                new[] { typeof(TChildEntity), childLoaderType, typeof(TEntity), loaderType, relationInfo.RightExpression.ReturnType, relationInfo.LeftExpression.ReturnType },
                new[] { rightExpressionType, leftExpressionType, typeof(Expression<Func<TChildEntity, TEntity>>), typeof(Expression<Func<TEntity, TChildEntity>>), typeof(RelationType) },
                BindingFlags.NonPublic | BindingFlags.Instance);

            registerMethodInfo.InvokeAndHoistBaseException(
                this,
                relationInfo.RightExpression,
                relationInfo.LeftExpression,
                navigationProperty,
                childNavigationProperty,
                relationType);
        }

        private void InitializeLoader<TProjection, TEntity>()
            where TProjection : ProjectionBase<TEntity, TExecutionContext>, new()
            where TEntity : class
        {
            InitializeLoader(typeof(TProjection), typeof(TEntity));
        }

        private void InitializeLoader(Type projectionType, Type entityType)
        {
            if (!_loadersToObjectGraphTypeConfiguratorsMap.ContainsKey((projectionType, entityType)))
            {
                // Force loader registration by its instance creation
                ResolveLoader(projectionType, entityType);
            }
        }

        private PropertyInfo? GetPrimaryKeyPropertyInfo(Type type)
        {
            if (_primaryKeys.TryGetValue(type, out var result))
            {
                return result;
            }

            return null;
        }
    }
}
