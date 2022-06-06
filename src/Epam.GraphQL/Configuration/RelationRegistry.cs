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
using Epam.GraphQL.Diagnostics;
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

namespace Epam.GraphQL.Configuration
{
    internal class RelationRegistry<TExecutionContext> : IServiceProvider, IRegistry<TExecutionContext>
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

        private static MethodInfo? _registerMethodInfo;
        private static MethodInfo? _registerInputMethodInfo;
        private static MethodInfo? _registerLoaderMethodInfo;
        private static MethodInfo? _resolveLoaderMethodInfo;
        private static MethodInfo? _getGraphTypeDescriptorMethodInfo;

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
            _serviceProvider = serviceProvider;
        }

        public object GetService(Type type) => _serviceProvider.GetService(type);

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
        {
            InitializeLoader<TProjection, TEntity>();
            var key = (typeof(TProjection), typeof(TEntity));
            _loadersToObjectGraphTypeConfiguratorsMap[key].ConfigureGraphType(graphType);
        }

        public void ConfigureInputGraphType<TProjection, TEntity>(IInputObjectGraphType graphType)
            where TProjection : ProjectionBase<TEntity, TExecutionContext>, new()
        {
            InitializeLoader<TProjection, TEntity>();
            var key = (typeof(TProjection), typeof(TEntity));
            _loadersToInputObjectGraphTypeConfiguratorsMap[key].ConfigureGraphType(graphType);
        }

        public IInlineGraphTypeResolver<TReturnType, TExecutionContext> Register<TReturnType>(
            IField<TExecutionContext> parent,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build,
            IChainConfigurationContext configurationContext,
            bool isInputType)
        {
            return (IInlineGraphTypeResolver<TReturnType, TExecutionContext>)_inlineConfiguratorsToResolversMap.GetOrAdd(
                (typeof(TReturnType), build, parent, isInputType),
                key => new InlineObjectBuilder<TReturnType, TExecutionContext>(key.Parent, this, (Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>?)key.Builder, configurationContext, key.IsInput));
        }

        public ObjectGraphTypeConfigurator<TProjection, TEntity, TExecutionContext> Register<TProjection, TEntity>()
            where TProjection : ProjectionBase<TEntity, TExecutionContext>, new()
        {
            var key = (typeof(TProjection), typeof(TEntity));
            return (ObjectGraphTypeConfigurator<TProjection, TEntity, TExecutionContext>)_loadersToObjectGraphTypeConfiguratorsMap.GetOrAdd(
                key,
                _ => new ObjectGraphTypeConfigurator<TProjection, TEntity, TExecutionContext>(this));
        }

        public ObjectGraphTypeConfigurator<TEntity, TExecutionContext> Register<TEntity>(Type projectionType)
        {
            _registerMethodInfo ??= ReflectionHelpers.GetMethodInfo(
                Register<DummyMutableLoader<TExecutionContext>, object>);

            var methodInfo = _registerMethodInfo.MakeGenericMethod(projectionType, typeof(TEntity));
            return methodInfo.InvokeAndHoistBaseException<ObjectGraphTypeConfigurator<TEntity, TExecutionContext>>(this);
        }

        IObjectGraphTypeConfigurator<TEntity, TExecutionContext> IRegistry<TExecutionContext>.Register<TEntity>(Type projectionType)
        {
            return Register<TEntity>(projectionType);
        }

        public InputObjectGraphTypeConfigurator<TProjection, TEntity, TExecutionContext> RegisterInput<TProjection, TEntity>()
            where TProjection : ProjectionBase<TEntity, TExecutionContext>, new()
        {
            var key = (typeof(TProjection), typeof(TEntity));
            return (InputObjectGraphTypeConfigurator<TProjection, TEntity, TExecutionContext>)_loadersToInputObjectGraphTypeConfiguratorsMap.GetOrAdd(
                key,
                _ => new InputObjectGraphTypeConfigurator<TProjection, TEntity, TExecutionContext>(this));
        }

        public InputObjectGraphTypeConfigurator<TEntity, TExecutionContext> RegisterInput<TEntity>(Type projectionType)
        {
            _registerInputMethodInfo ??= ReflectionHelpers.GetMethodInfo(
                RegisterInput<DummyMutableLoader<TExecutionContext>, object>);

            var methodInfo = _registerInputMethodInfo.MakeGenericMethod(projectionType, typeof(TEntity));
            return methodInfo.InvokeAndHoistBaseException<InputObjectGraphTypeConfigurator<TEntity, TExecutionContext>>(this);
        }

        IObjectGraphTypeConfigurator<TEntity, TExecutionContext> IRegistry<TExecutionContext>.RegisterInput<TEntity>(Type projectionType)
        {
            return RegisterInput<TEntity>(projectionType);
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
        {
            RegisterAutoObjectGraphType<TEntity>(ConfigurationContext.Create())
                .ConfigureGraphType(graphType);
        }

        public void ConfigureInputAutoObjectGraphType<TEntity>(IInputObjectGraphType graphType)
        {
            RegisterInputAutoObjectGraphType<TEntity>(ConfigurationContext.Create())
                .ConfigureGraphType(graphType);
        }

        public ObjectGraphTypeConfigurator<TEntity, TExecutionContext> RegisterAutoObjectGraphType<TEntity>(IObjectConfigurationContext configurationContext)
        {
            return (ObjectGraphTypeConfigurator<TEntity, TExecutionContext>)_autoEntityTypesToConfiguratorsMap.GetOrAdd(
                typeof(TEntity),
                _ => new ObjectGraphTypeConfigurator<TEntity, TExecutionContext>(null, configurationContext, this, isAuto: true));
        }

        public InputObjectGraphTypeConfigurator<TEntity, TExecutionContext> RegisterInputAutoObjectGraphType<TEntity>(IObjectConfigurationContext configurationContext)
        {
            return (InputObjectGraphTypeConfigurator<TEntity, TExecutionContext>)_inputAutoEntityTypesToConfiguratorsMap.GetOrAdd(
                typeof(TEntity),
                _ => new InputObjectGraphTypeConfigurator<TEntity, TExecutionContext>(null, configurationContext, this, isAuto: true));
        }

        public void Register<TEntity, TProperty>(Expression<Func<TEntity, TProperty>> idExpression)
        {
            if (!_primaryKeys.ContainsKey(typeof(TEntity)))
            {
                _primaryKeys.Add(typeof(TEntity), idExpression.GetPropertyInfo());
            }
        }

        public bool HasFakePropertyValues<TEntity>(Type loaderType, TEntity entity, IDictionary<string, object?> propertyValues)
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
        {
            if (_relationMap.TryGetValue((childLoaderType, typeof(TChildEntity)), out var rel))
            {
                var relations = (Relations<TChildEntity>)rel;
                return relations.CanViewParentAsync(context, entity);
            }

            return Task.FromResult(true);
        }

        public void RegisterRelationPostponedForSave<TEntity, TChildEntity, TChildEntityLoader, TPropertyType, TChildPropertyType>(string propName, Predicate<TPropertyType> isFakePropValue)
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

        public bool HasFakePropertyValuesPostponedForSave<TChildEntity>(TChildEntity entity, IDictionary<string, object?> propertyValues)
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
        {
            if (_cache.ContainsKey(typeof(TLoader)))
            {
                return (TLoader)_cache[typeof(TLoader)];
            }

            var loader = new TLoader();
            _cache[typeof(TLoader)] = loader;
            _projectionEntityTypes.Add(typeof(TEntity));
            loader.Registry = this;
            loader.AfterConstruction();
            loader.Configure();
            loader.ConfigureInput();
            return loader;
        }

        public ProjectionBase<TExecutionContext> ResolveLoader(Type projectionType, Type entityType)
        {
            _resolveLoaderMethodInfo ??= ReflectionHelpers.GetMethodInfo(
                ResolveLoader<DummyMutableLoader<TExecutionContext>, object>);

            var methodInfo = _resolveLoaderMethodInfo.MakeGenericMethod(projectionType, entityType);
            return methodInfo.InvokeAndHoistBaseException<ProjectionBase<TExecutionContext>>(this);
        }

        public IFilter<TEntity, TExecutionContext> ResolveFilter<TEntity>(Type loaderFilterType)
        {
            return (IFilter<TEntity, TExecutionContext>)_cache.GetOrAdd(loaderFilterType, type => loaderFilterType.CreateInstanceAndHoistBaseException());
        }

        public ISearcher<TEntity, TExecutionContext> ResolveSearcher<TSearcher, TEntity>()
            where TSearcher : ISearcher<TEntity, TExecutionContext>
        {
            return (ISearcher<TEntity, TExecutionContext>)_cache.GetOrAdd(typeof(TSearcher), type => type.CreateInstanceAndHoistBaseException());
        }

        public IObjectGraphType ResolveObjectGraphTypeWrapper<TLoader, TEntity>()
            where TLoader : ProjectionBase<TEntity, TExecutionContext>, new()
        {
            return (IObjectGraphType)_serviceProvider.GetRequiredService(GetEntityGraphType<TLoader, TEntity>());
        }

        public string GetGraphQLTypeName<TEntity>(bool isInput, IField<TExecutionContext>? parent)
        {
            return GetGraphQLTypeName(typeof(TEntity), null, isInput, parent);
        }

        public string GetGraphQLTypeName(Type entityType, Type? projectionType, bool isInput, IField<TExecutionContext>? parent, string? name = null)
        {
            name ??= entityType.GraphQLTypeName(isInput);

            if (parent != null)
            {
                name = GetGraphQLTypePrefix(parent);
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

            if (ReflectionHelpers.TryFindMatchingGenericBaseType(projectionType, typeof(Query<>), out var _) || ReflectionHelpers.TryFindMatchingGenericBaseType(projectionType, typeof(Mutation<>), out _))
            {
                possibleName = projectionType.GraphQLTypeName(false);
            }

            return GetGraphQLTypeName(entityType, projectionType, isInput, null, possibleName);
        }

        public void SetGraphQLTypeName<TEntity>(string? oldName, string newName)
        {
            SetGraphQLTypeName(null, typeof(TEntity), oldName, newName);
        }

        public void SetGraphQLTypeName<TProjection, TEntity>(string? oldName, string newName)
            where TProjection : ProjectionBase<TEntity, TExecutionContext>
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
            where TProjection : ProjectionBase<TEntity, TExecutionContext>, new()
        {
            var baseType = GetPropperBaseProjectionType<TProjection, TEntity>();

            if (baseType != typeof(TProjection))
            {
                ResolveLoader<TProjection, TEntity>();
            }

            return typeof(EntityGraphType<,,>).MakeGenericType(baseType, typeof(TEntity), typeof(TExecutionContext));
        }

        public Type GetEntityGraphType(Type projectionType, Type entityType)
        {
            var methodInfo = ReflectionHelpers.GetMethodInfo(GetEntityGraphType<DummyMutableLoader<TExecutionContext>, object>)
                .MakeGenericMethod(projectionType, entityType);

            return methodInfo.InvokeAndHoistBaseException<Type>(this);
        }

        public Type GetInputEntityGraphType<TProjection, TEntity>()
            where TProjection : ProjectionBase<TEntity, TExecutionContext>, new()
        {
            var baseType = GetPropperBaseProjectionType<TProjection, TEntity>();

            if (baseType != typeof(TProjection))
            {
                ResolveLoader<TProjection, TEntity>();
            }

            return typeof(InputEntityGraphType<,,>).MakeGenericType(baseType, typeof(TEntity), typeof(TExecutionContext));
        }

        public Type GetInputEntityGraphType(Type projectionType, Type entityType)
        {
            var methodInfo = ReflectionHelpers.GetMethodInfo(GetInputEntityGraphType<DummyMutableLoader<TExecutionContext>, object>)
                .MakeGenericMethod(projectionType, entityType);

            return methodInfo.InvokeAndHoistBaseException<Type>(this);
        }

        public Type GetSubmitOutputItemGraphType<TProjection, TEntity, TId>()
            where TProjection : ProjectionBase<TEntity, TExecutionContext>, new()
            where TEntity : class
        {
            return typeof(SubmitOutputItemGraphType<,,,>).MakeGenericType(GetPropperBaseProjectionType<TProjection, TEntity>(), typeof(TEntity), typeof(TId), typeof(TExecutionContext));
        }

        public Type GetSubmitOutputItemGraphType(Type projectionType, Type entityType, Type idType)
        {
            var methodInfo = ReflectionHelpers.GetMethodInfo(GetSubmitOutputItemGraphType<DummyMutableLoader<TExecutionContext>, object, object>)
                .MakeGenericMethod(projectionType, entityType, idType);

            return methodInfo.InvokeAndHoistBaseException<Type>(this);
        }

        public Type GetPropperBaseProjectionType<TProjection, TEntity>()
            where TProjection : ProjectionBase<TEntity, TExecutionContext>, new() =>
            GetPropperBaseProjectionType<TProjection, TEntity>((first, second) => first.Equals(second));

        public Type GetPropperBaseProjectionType<TProjection, TEntity>(
            Func<IObjectGraphTypeConfigurator<TExecutionContext>, IObjectGraphTypeConfigurator<TExecutionContext>, bool> equalPredicate)
            where TProjection : ProjectionBase<TEntity, TExecutionContext>, new()
        {
            var foundType = typeof(TProjection);
            var baseType = typeof(TProjection).BaseType;

            while (true)
            {
                Guards.ThrowArgumentExceptionIf(
                    baseType == typeof(object),
                    "Invalid projection type",
                    nameof(TProjection));

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

                var projection = ResolveLoader<TProjection, TEntity>();
                var baseLoader = ResolveLoader(baseType, typeof(TEntity));

                if (equalPredicate(baseLoader.GetObjectGraphTypeConfigurator(), projection.GetObjectGraphTypeConfigurator())
                    && equalPredicate(baseLoader.GetInputObjectGraphTypeConfigurator(), projection.GetInputObjectGraphTypeConfigurator()))
                {
                    foundType = baseType;
                }

                baseType = baseType.BaseType;
            }
        }

        public IGraphTypeDescriptor<TReturnType, TExecutionContext> GetGraphTypeDescriptor<TReturnType>(
            IField<TExecutionContext> parent,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build,
            IInlinedChainConfigurationContext configurationContext)
        {
            if (build == null)
            {
                return GetGraphTypeDescriptor<TReturnType>(parent, configurationContext, false);
            }

            return new ObjectGraphTypeDescriptor<TReturnType, TExecutionContext>(parent, this, build, configurationContext, false);
        }

        public IGraphTypeDescriptor<TReturnType, TExecutionContext> GetInputGraphTypeDescriptor<TReturnType>(
            IField<TExecutionContext> parent,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build,
            IInlinedChainConfigurationContext configurationContext)
        {
            if (build == null)
            {
                return GetGraphTypeDescriptor<TReturnType>(parent, configurationContext, true);
            }

            return new ObjectGraphTypeDescriptor<TReturnType, TExecutionContext>(parent, this, build, configurationContext, true);
        }

        public IGraphTypeDescriptor<TReturnType, TExecutionContext> GetGraphTypeDescriptor<TReturnType>(IField<TExecutionContext> parent)
            => GetGraphTypeDescriptor<TReturnType>(parent, parent.ConfigurationContext, false);

        public IGraphTypeDescriptor<TReturnType, TExecutionContext> GetInputGraphTypeDescriptor<TReturnType>(IField<TExecutionContext> parent)
            => GetGraphTypeDescriptor<TReturnType>(parent, parent.ConfigurationContext, true);

        public IGraphTypeDescriptor<TEntity, TExecutionContext> GetGraphTypeDescriptor<TProjection, TEntity>()
            where TProjection : ProjectionBase<TEntity, TExecutionContext>, new()
        {
            return new EntityGraphTypeDescriptor<TProjection, TEntity, TExecutionContext>(this, false);
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

        public bool IsSimpleType(Type type)
        {
            type = type.UnwrapIfNullable();

            return type == typeof(string) || type.IsEnum || _structTypeMap.ContainsKey(type);
        }

        private static string GetGraphQLTypePrefix(IField<TExecutionContext> parentField)
        {
            return $"{GetGraphQLTypePrefix(parentField.Parent)}{parentField.Name.CapitalizeFirstLetter()}";
        }

        private static string GetGraphQLTypePrefix(IObjectGraphTypeConfigurator<TExecutionContext> parentConfigurator)
        {
            if (parentConfigurator.Parent == null)
            {
                return parentConfigurator.Name;
            }

            return GetGraphQLTypePrefix(parentConfigurator.Parent);
        }

        private IGraphTypeDescriptor<TExecutionContext> GetGraphTypeDescriptor(
            Type type,
            IField<TExecutionContext> parent,
            IConfigurationContext configurationContext,
            bool isInput)
        {
            _getGraphTypeDescriptorMethodInfo ??= ReflectionHelpers.GetMethodInfo<IField<TExecutionContext>, IChainConfigurationContext, bool, IGraphTypeDescriptor<object, TExecutionContext>>(GetGraphTypeDescriptor<object>);

            return _getGraphTypeDescriptorMethodInfo
                .MakeGenericMethod(type)
                .InvokeAndHoistBaseException<IGraphTypeDescriptor<TExecutionContext>>(this, parent, configurationContext, isInput);
        }

        private IGraphTypeDescriptor<TReturnType, TExecutionContext> GetGraphTypeDescriptor<TReturnType>(
            IField<TExecutionContext> parent,
            IChainConfigurationContext configurationContext,
            bool isInput)
        {
            if (parent == null || typeof(TReturnType).IsValueType || typeof(TReturnType) == typeof(string))
            {
                return new GraphTypeDescriptor<TReturnType, TExecutionContext>(this, isInput);
            }

            if (typeof(TReturnType) != typeof(string) && typeof(TReturnType).IsEnumerableType())
            {
                var elementType = typeof(TReturnType).GetEnumerableElementType();
                var elementDescriptor = GetGraphTypeDescriptor(elementType, parent, configurationContext, isInput);

                return (IGraphTypeDescriptor<TReturnType, TExecutionContext>)typeof(ListGraphTypeDescriptor<,,>)
                    .MakeGenericType(elementType, typeof(TReturnType), typeof(TExecutionContext))
                    .CreateInstanceAndHoistBaseException(elementDescriptor);
            }

            return (IGraphTypeDescriptor<TReturnType, TExecutionContext>)typeof(ObjectGraphTypeDescriptor<,>)
                .MakeGenericType(typeof(TReturnType), typeof(TExecutionContext))
                .CreateInstanceAndHoistBaseException(parent, this, null, configurationContext, isInput);
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

                throw new InvalidOperationException($"The type: Nullable<{elementType.HumanizedName()}> cannot be coerced effectively to a GraphQL type.");
            }

            if (type.IsValueType)
            {
                throw new InvalidOperationException(
                    $"The type: {type.HumanizedName()} cannot be coerced effectively to a GraphQL type.");
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
                Guards.ThrowInvalidOperationIf(
                    oldType.Entity != entityType || (oldType.Projection != null && !oldType.Projection.IsAssignableFrom(projectionType)),
                    $"Configuration already contains different type `{oldType.Entity.HumanizedName()}` with name `{newName}`");

                return;
            }

            if (oldName != null && TryGetRegisteredType(oldName, out oldType))
            {
                Guards.ThrowNotSupportedIf((entityType, projectionType) != oldType);

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
            where TLoader : Loader<TEntity, TExecutionContext>, new()
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

            Guards.ThrowArgumentExceptionIf(
                relationInfo.LeftExpression.Parameters[0].Type != typeof(TEntity) || relationInfo.RightExpression.Parameters[0].Type != typeof(TChildEntity),
                "Condition is invalid",
                nameof(relationCondition));

            var leftFuncType = typeof(Func<,>).MakeGenericType(typeof(TEntity), relationInfo.LeftExpression.ReturnType);
            var leftExpressionType = typeof(Expression<>).MakeGenericType(leftFuncType);

            var rightFuncType = typeof(Func<,>).MakeGenericType(typeof(TChildEntity), relationInfo.RightExpression.ReturnType);
            var rightExpressionType = typeof(Expression<>).MakeGenericType(rightFuncType);

            _registerLoaderMethodInfo ??= new Action<Expression<Func<object, object>>, Expression<Func<object, object>>, Expression<Func<object, object>>?, Expression<Func<object, object>>?, RelationType>(Register<object, DummyMutableLoader<TExecutionContext>, object, DummyMutableLoader<TExecutionContext>, object, object>)
                .GetMethodInfo()
                .GetGenericMethodDefinition();

            var registerMethodInfo = _registerLoaderMethodInfo.MakeGenericMethod(
                typeof(TChildEntity), childLoaderType, typeof(TEntity), loaderType, relationInfo.RightExpression.ReturnType, relationInfo.LeftExpression.ReturnType);

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
