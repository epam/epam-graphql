// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading.Tasks;
using Epam.Contracts.Models;
using Epam.GraphQL.Filters;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Search;
using Epam.GraphQL.Tests.TestData;

namespace Epam.GraphQL.Tests.Helpers
{
    public static class GraphQLTypeBuilder
    {
        private static readonly Dictionary<Type, Action<object>> _onConfigureActions = new();
        private static readonly Dictionary<Type, Func<object, object>> _getBaseQueryFuncs = new();
        private static readonly Dictionary<Type, Func<object, object, object>> _applySecurityFilterFuncs = new();
        private static readonly Dictionary<Type, Func<object, object>> _applyNaturalOrderByFuncs = new();
        private static readonly Dictionary<Type, Func<object, object>> _applyNaturalThenByFuncs = new();
        private static readonly Dictionary<Type, Func<object, object, object, object>> _applyFilterFuncs = new();
        private static readonly Dictionary<Type, Func<object, object, string, object>> _applySearchFuncs = new();
        private static readonly Dictionary<Type, Func<object, string, object>> _applySearchOrderByFuncs = new();
        private static readonly Dictionary<Type, Func<object, string, object>> _applySearchThenByFuncs = new();
        private static readonly Dictionary<Type, Func<object, object, bool, Task<bool>>> _canSaveAsyncFuncs = new();
        private static readonly Dictionary<Type, Action<object, object>> _beforeCreateFuncs = new();
        private static readonly Dictionary<Type, Action<object, object>> _beforeUpdateFuncs = new();
        private static readonly Dictionary<Type, Func<object, IEnumerable<object>, Task<IEnumerable<object>>>> _afterSaveFuncs = new();
        private static readonly Dictionary<Type, Func<object, bool>> _isFakeIdFuncs = new();
        private static readonly Dictionary<Type, Expression> _idExpressions = new();
        private static readonly AssemblyName _assemblyName = new("GraphQLAssembly");
        private static readonly AssemblyBuilder _assemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(_assemblyName, AssemblyBuilderAccess.RunAndCollect);
        private static readonly ModuleBuilder _moduleBuilder = _assemblyBuilder.DefineDynamicModule("GraphQL.Tests");

        public static void CallOnConfigureAction(Type type, object query)
        {
            GetValueByType(_onConfigureActions, type)(query);
        }

        public static object CallGetBaseQueryFunc(Type type, object context)
        {
            return GetValueByType(_getBaseQueryFuncs, type)(context);
        }

        public static object CallApplySecurityFilterFunc(Type type, object context, object query)
        {
            return _applySecurityFilterFuncs[type](context, query);
        }

        public static object CallApplyNaturalOrderByFunc(Type type, object query)
        {
            return GetValueByType(_applyNaturalOrderByFuncs, type)(query);
        }

        public static object CallApplyNaturalThenByFunc(Type type, object query)
        {
            return _applyNaturalThenByFuncs[type](query);
        }

        public static object CallApplyFilterFunc(Type type, object context, object query, object filter)
        {
            return _applyFilterFuncs[type](context, query, filter);
        }

        public static object CallApplySearchFunc(Type type, object query, object context, string search)
        {
            return _applySearchFuncs[type](query, context, search);
        }

        public static object CallApplySearchOrderByFunc(Type type, object query, string search)
        {
            return _applySearchOrderByFuncs[type](query, search);
        }

        public static object CallApplySearchThenByFunc(Type type, object query, string search)
        {
            return _applySearchThenByFuncs[type](query, search);
        }

        public static Task<bool> CallCanSaveAsync(Type type, object executionContext, object entity, bool isNew)
        {
            return _canSaveAsyncFuncs[type](executionContext, entity, isNew);
        }

        public static void CallBeforeCreate(Type type, object executionContext, object entity)
        {
            _beforeCreateFuncs[type](executionContext, entity);
        }

        public static void CallBeforeUpdate(Type type, object executionContext, object entity)
        {
            _beforeUpdateFuncs[type](executionContext, entity);
        }

        public static Task<IEnumerable<object>> CallAfterSave(Type type, object executionContext, IEnumerable<object> entities)
        {
            return _afterSaveFuncs[type](executionContext, entities);
        }

        public static Expression GetIdExpression(Type type) => GetValueByType(_idExpressions, type);

        public static bool GetIsFakeId(Type type, object id) => GetValueByType(_isFakeIdFuncs, type)(id);

        public static Type CreateQueryType<TExecutionContext>(Action<Query<TExecutionContext>> onConfigure, string typeName = null)
        {
            void OnConfigureWrapper(ProjectionBase<object, TExecutionContext> query)
            {
                onConfigure((Query<TExecutionContext>)query);
            }

            return CreateProjectionBaseType(typeName ?? "Query", typeof(Query<TExecutionContext>), (Action<ProjectionBase<object, TExecutionContext>>)OnConfigureWrapper);
        }

        public static Type CreateMutationType<TExecutionContext>(Action<Mutation<TExecutionContext>> onConfigure, Func<TExecutionContext, IEnumerable<object>, Task<IEnumerable<object>>> afterSave = null, string typeName = null)
        {
            void OnConfigureWrapper(ProjectionBase<object, TExecutionContext> query)
            {
                onConfigure((Mutation<TExecutionContext>)query);
            }

            var type = CreateProjectionBaseType(typeName ?? "Mutation", typeof(Mutation<TExecutionContext>), (Action<ProjectionBase<object, TExecutionContext>>)OnConfigureWrapper, typeBuilder =>
            {
                if (afterSave != null)
                {
                    var afterSaveMethodInfo = typeof(Mutation<TExecutionContext>).GetMethod("AfterSaveAsync", BindingFlags.Instance | BindingFlags.NonPublic);
                    var afterSaveMethodBuilder = typeBuilder.DefineMethod(
                        "AfterSaveAsync",
                        MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                        CallingConventions.Standard,
                        afterSaveMethodInfo.ReturnType,
                        afterSaveMethodInfo.GetParameters().Select(p => p.ParameterType).ToArray());
                    var afterSaveGenerator = afterSaveMethodBuilder.GetILGenerator();

                    afterSaveGenerator.Emit(OpCodes.Ldarg_0); // this
                    afterSaveGenerator.Emit(OpCodes.Call, typeof(object).GetMethod("GetType"));
                    afterSaveGenerator.Emit(OpCodes.Ldarg_1); // context
                    afterSaveGenerator.Emit(OpCodes.Ldarg_2); // entities
                    afterSaveGenerator.Emit(OpCodes.Call, typeof(GraphQLTypeBuilder).GetMethod(nameof(GraphQLTypeBuilder.CallAfterSave), BindingFlags.Static | BindingFlags.Public));
                    afterSaveGenerator.Emit(OpCodes.Ret);

                    typeBuilder.DefineMethodOverride(afterSaveMethodBuilder, afterSaveMethodInfo);
                }
            });

            if (afterSave != null)
            {
                _afterSaveFuncs[type] = (context, entities) => afterSave((TExecutionContext)context, entities);
            }

            return type;
        }

        public static Type CreateInheritedLoaderType<TEntity, TExecutionContext>(
            Type baseType,
            string typeName = null,
            Action<Loader<TEntity, TExecutionContext>> onConfigure = null,
            Func<TExecutionContext, IQueryable<TEntity>> getBaseQuery = null)
            where TEntity : class
        {
            var typeBuilder = GetTypeBuilder(typeName, baseType);

            if (getBaseQuery != null)
            {
                OverrideGetBaseQuery<TEntity, TExecutionContext>(typeBuilder);
            }

            if (onConfigure != null)
            {
                OverrideOnConfigure<TExecutionContext>(typeBuilder);
            }

            var type = typeBuilder.CreateTypeInfo();

            if (getBaseQuery != null)
            {
                _getBaseQueryFuncs[type] = context => getBaseQuery((TExecutionContext)context);
            }

            if (onConfigure != null)
            {
                _onConfigureActions[type] = obj => onConfigure((Loader<TEntity, TExecutionContext>)obj);
            }

            return type;
        }

        public static Type CreateLoaderType<TEntity, TExecutionContext>(
            Action<Loader<TEntity, TExecutionContext>> onConfigure,
            Func<TExecutionContext, IQueryable<TEntity>> getBaseQuery,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> applyNaturalOrderBy = null,
            Func<IOrderedQueryable<TEntity>, IOrderedQueryable<TEntity>> applyNaturalThenBy = null,
            Func<TExecutionContext, IQueryable<TEntity>, IQueryable<TEntity>> applySecurityFilter = null,
            string typeName = null)
            where TEntity : class
        {
            return CreateLoaderType(typeName ?? $"{typeof(TEntity).Name}Loader", typeof(Loader<TEntity, TExecutionContext>), onConfigure, getBaseQuery, applyNaturalOrderBy, applyNaturalThenBy, applySecurityFilter);
        }

        public static Type CreateProjectionType<TEntity, TExecutionContext>(
            Action<Projection<TEntity, TExecutionContext>> onConfigure)
            where TEntity : class
        {
            void OnConfigureWrapper(ProjectionBase<TEntity, TExecutionContext> query)
            {
                onConfigure((Projection<TEntity, TExecutionContext>)query);
            }

            return CreateProjectionBaseType($"{typeof(TEntity).Name}Projection", typeof(Projection<TEntity, TExecutionContext>), (Action<ProjectionBase<TEntity, TExecutionContext>>)OnConfigureWrapper);
        }

        public static Type CreateIdentifiableLoaderType<TEntity, TId, TExecutionContext>(
            Action<IdentifiableLoader<TEntity, TId, TExecutionContext>> onConfigure,
            Func<TExecutionContext, IQueryable<TEntity>> getBaseQuery,
            Expression<Func<TEntity, TId>> idExpression,
            Func<TExecutionContext, IQueryable<TEntity>, IQueryable<TEntity>> applySecurityFilter = null,
            string typeName = null)
            where TEntity : class
        {
            void OnConfigureWrapper(Loader<TEntity, TExecutionContext> query)
            {
                onConfigure((IdentifiableLoader<TEntity, TId, TExecutionContext>)query);
            }

            var type = CreateLoaderType(typeName ?? $"{typeof(TEntity).Name}Loader", typeof(IdentifiableLoader<TEntity, TId, TExecutionContext>), OnConfigureWrapper, getBaseQuery, null, null, applySecurityFilter, OverrideIdExpression<TEntity, TId, TExecutionContext>);

            _idExpressions[type] = idExpression;

            return type;
        }

        public static Type CreateMutableLoaderType<TEntity, TExecutionContext>(
            Action<MutableLoader<TEntity, int, TExecutionContext>> onConfigure,
            Func<TExecutionContext, IQueryable<TEntity>> getBaseQuery,
            Func<TExecutionContext, IQueryable<TEntity>, IQueryable<TEntity>> applySecurityFilter = null,
            Func<IExecutionContextAccessor<TExecutionContext>, TEntity, bool, Task<bool>> canSaveAsync = null,
            Action<TExecutionContext, TEntity> beforeCreate = null,
            Action<TExecutionContext, TEntity> beforeUpdate = null,
            string typeName = null)
            where TEntity : class, IHasId<int>
        {
            var type = CreateMutableLoaderType<TEntity, int, TExecutionContext>(
                onConfigure,
                getBaseQuery,
                id => id < 0,
                applySecurityFilter,
                canSaveAsync,
                beforeCreate,
                beforeUpdate,
                typeName);

            return type;
        }

        public static Type CreateMutableLoaderType<TEntity, TId, TExecutionContext>(
            Action<MutableLoader<TEntity, TId, TExecutionContext>> onConfigure,
            Func<TExecutionContext, IQueryable<TEntity>> getBaseQuery,
            Func<TId, bool> isFakeId,
            Func<TExecutionContext, IQueryable<TEntity>, IQueryable<TEntity>> applySecurityFilter = null,
            Func<IExecutionContextAccessor<TExecutionContext>, TEntity, bool, Task<bool>> canSaveAsync = null,
            Action<TExecutionContext, TEntity> beforeCreate = null,
            Action<TExecutionContext, TEntity> beforeUpdate = null,
            string typeName = null)
            where TEntity : class, IHasId<TId>
        {
            var type = CreateMutableLoaderType(
                typeof(MutableLoader<TEntity, TId, TExecutionContext>),
                onConfigure,
                getBaseQuery,
                applySecurityFilter,
                canSaveAsync,
                beforeCreate,
                beforeUpdate,
                typeName,
                typeBuilder =>
                {
                    var isFakeIdMethodInfo = typeof(MutableLoader<TEntity, TId, TExecutionContext>).GetMethod("IsFakeId", BindingFlags.Instance | BindingFlags.Public);
                    var isFakeIdMethodBuilder = typeBuilder.DefineMethod(
                        "IsFakeId",
                        MethodAttributes.Public | MethodAttributes.Virtual,
                        CallingConventions.Standard,
                        isFakeIdMethodInfo.ReturnType,
                        isFakeIdMethodInfo.GetParameters().Select(p => p.ParameterType).ToArray());
                    var isFakeIdGenerator = isFakeIdMethodBuilder.GetILGenerator();

                    isFakeIdGenerator.Emit(OpCodes.Ldarg_0); // this
                    isFakeIdGenerator.Emit(OpCodes.Call, typeof(object).GetMethod("GetType"));
                    isFakeIdGenerator.Emit(OpCodes.Ldarg_1); // id

                    if (typeof(TId).IsValueType)
                    {
                        isFakeIdGenerator.Emit(OpCodes.Box, typeof(TId));
                    }

                    isFakeIdGenerator.Emit(OpCodes.Call, typeof(GraphQLTypeBuilder).GetMethod(nameof(GraphQLTypeBuilder.GetIsFakeId), BindingFlags.Static | BindingFlags.Public));
                    isFakeIdGenerator.Emit(OpCodes.Ret);

                    typeBuilder.DefineMethodOverride(isFakeIdMethodBuilder, isFakeIdMethodInfo);
                });

            Expression<Func<TEntity, TId>> idExpression = e => e.Id;
            _idExpressions[type] = idExpression;
            _isFakeIdFuncs[type] = id => isFakeId((TId)id);

            return type;
        }

        public static Type CreateLoaderFilterType<TEntity, TFilter, TExecutionContext>(Func<TExecutionContext, IQueryable<TEntity>, TFilter, IQueryable<TEntity>> applyFilter)
            where TEntity : class
            where TFilter : Input
        {
            var loaderFilterType = typeof(Filter<,,>).MakeGenericType(typeof(TEntity), typeof(TFilter), typeof(TExecutionContext));
            var typeBuilder = GetTypeBuilder($"{typeof(TEntity).Name}LoaderFilter", loaderFilterType);

            var applyFilterMethodInfo = loaderFilterType.GetMethod("ApplyFilter", BindingFlags.Instance | BindingFlags.NonPublic);
            var applyFilterMethodBuilder = typeBuilder.DefineMethod(
                "ApplyFilter",
                MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                CallingConventions.Standard,
                applyFilterMethodInfo.ReturnType,
                applyFilterMethodInfo.GetParameters().Select(p => p.ParameterType).ToArray());
            var applyFilterGenerator = applyFilterMethodBuilder.GetILGenerator();
            applyFilterGenerator.Emit(OpCodes.Ldarg_0); // this
            applyFilterGenerator.Emit(OpCodes.Call, typeof(object).GetMethod("GetType"));
            applyFilterGenerator.Emit(OpCodes.Ldarg_1); // context
            applyFilterGenerator.Emit(OpCodes.Ldarg_2); // query
            applyFilterGenerator.Emit(OpCodes.Ldarg_3); // filter
            applyFilterGenerator.Emit(OpCodes.Call, typeof(GraphQLTypeBuilder).GetMethod(nameof(GraphQLTypeBuilder.CallApplyFilterFunc), BindingFlags.Static | BindingFlags.Public));
            applyFilterGenerator.Emit(OpCodes.Castclass, typeof(IQueryable<>).MakeGenericType(typeof(TEntity)));
            applyFilterGenerator.Emit(OpCodes.Ret);
            typeBuilder.DefineMethodOverride(applyFilterMethodBuilder, applyFilterMethodInfo);

            var type = typeBuilder.CreateTypeInfo();
            _applyFilterFuncs[type] = (context, query, filter) => applyFilter((TExecutionContext)context, (IQueryable<TEntity>)query, (TFilter)filter);

            return type;
        }

        public static Type CreateSearcherType<TEntity, TExecutionContext>(Func<IQueryable<TEntity>, TExecutionContext, string, IQueryable<TEntity>> all)
            where TEntity : class
        {
            return CreateSearcherTypeBuilder($"{nameof(Person)}Searcher", typeof(Searcher<TEntity, TExecutionContext>), all);
        }

        public static Type CreateOrderedSearcherType<TEntity, TExecutionContext>(
            Func<IQueryable<TEntity>, TExecutionContext, string, IQueryable<TEntity>> all,
            Func<IQueryable<TEntity>, string, IOrderedQueryable<TEntity>> applySearchOrderBy,
            Func<IOrderedQueryable<TEntity>, string, IOrderedQueryable<TEntity>> applySearchThenBy)
            where TEntity : class
        {
            var type = CreateSearcherTypeBuilder($"{nameof(Person)}Searcher", typeof(OrderedSearcher<TEntity, TExecutionContext>), all, typeBuilder =>
            {
                var applySearchOrderByMethodInfo = typeof(OrderedSearcher<TEntity, TExecutionContext>).GetMethod("ApplySearchOrderBy", BindingFlags.Instance | BindingFlags.Public);
                var applySearchOrderByMethodBuilder = typeBuilder.DefineMethod(
                    "ApplySearchOrderBy",
                    MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                    CallingConventions.Standard,
                    applySearchOrderByMethodInfo.ReturnType,
                    applySearchOrderByMethodInfo.GetParameters().Select(p => p.ParameterType).ToArray());
                var applySearchOrderByGenerator = applySearchOrderByMethodBuilder.GetILGenerator();
                applySearchOrderByGenerator.Emit(OpCodes.Ldarg_0); // this
                applySearchOrderByGenerator.Emit(OpCodes.Call, typeof(object).GetMethod("GetType"));
                applySearchOrderByGenerator.Emit(OpCodes.Ldarg_1); // query
                applySearchOrderByGenerator.Emit(OpCodes.Ldarg_2); // search
                applySearchOrderByGenerator.Emit(OpCodes.Call, typeof(GraphQLTypeBuilder).GetMethod(nameof(GraphQLTypeBuilder.CallApplySearchOrderByFunc), BindingFlags.Static | BindingFlags.Public));
                applySearchOrderByGenerator.Emit(OpCodes.Castclass, typeof(IOrderedQueryable<>).MakeGenericType(typeof(TEntity)));
                applySearchOrderByGenerator.Emit(OpCodes.Ret);
                typeBuilder.DefineMethodOverride(applySearchOrderByMethodBuilder, applySearchOrderByMethodInfo);

                var applySearchThenByMethodInfo = typeof(OrderedSearcher<TEntity, TExecutionContext>).GetMethod("ApplySearchThenBy", BindingFlags.Instance | BindingFlags.Public);
                var applySearchThenByMethodBuilder = typeBuilder.DefineMethod(
                    "ApplySearchThenBy",
                    MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                    CallingConventions.Standard,
                    applySearchThenByMethodInfo.ReturnType,
                    applySearchThenByMethodInfo.GetParameters().Select(p => p.ParameterType).ToArray());
                var applySearchThenByGenerator = applySearchThenByMethodBuilder.GetILGenerator();
                applySearchThenByGenerator.Emit(OpCodes.Ldarg_0); // this
                applySearchThenByGenerator.Emit(OpCodes.Call, typeof(object).GetMethod("GetType"));
                applySearchThenByGenerator.Emit(OpCodes.Ldarg_1); // query
                applySearchThenByGenerator.Emit(OpCodes.Ldarg_2); // search
                applySearchThenByGenerator.Emit(OpCodes.Call, typeof(GraphQLTypeBuilder).GetMethod(nameof(GraphQLTypeBuilder.CallApplySearchThenByFunc), BindingFlags.Static | BindingFlags.Public));
                applySearchThenByGenerator.Emit(OpCodes.Castclass, typeof(IOrderedQueryable<>).MakeGenericType(typeof(TEntity)));
                applySearchThenByGenerator.Emit(OpCodes.Ret);
                typeBuilder.DefineMethodOverride(applySearchThenByMethodBuilder, applySearchThenByMethodInfo);
            });

            _applySearchOrderByFuncs[type] = (query, search) => applySearchOrderBy((IQueryable<TEntity>)query, search);
            _applySearchThenByFuncs[type] = (query, search) => applySearchThenBy((IOrderedQueryable<TEntity>)query, search);

            return type;
        }

        private static void OverrideIdExpression<TEntity, TId, TExecutionContext>(TypeBuilder typeBuilder)
            where TEntity : class
        {
            var getIdExpressionMethodInfo = typeof(IdentifiableLoader<TEntity, TId, TExecutionContext>).GetMethod("get_IdExpression", BindingFlags.Instance | BindingFlags.NonPublic);
            var getIdExpressionMethodBuilder = typeBuilder.DefineMethod(
                "get_IdExpression",
                MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                CallingConventions.Standard,
                getIdExpressionMethodInfo.ReturnType,
                getIdExpressionMethodInfo.GetParameters().Select(p => p.ParameterType).ToArray());
            var generator = getIdExpressionMethodBuilder.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0); // this
            generator.Emit(OpCodes.Call, typeof(object).GetMethod("GetType"));
            generator.Emit(OpCodes.Call, typeof(GraphQLTypeBuilder).GetMethod(nameof(GraphQLTypeBuilder.GetIdExpression), BindingFlags.Static | BindingFlags.Public));
            generator.Emit(OpCodes.Castclass, typeof(Expression<Func<TEntity, TId>>));
            generator.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(getIdExpressionMethodBuilder, getIdExpressionMethodInfo);
        }

        private static Type CreateMutableLoaderType<TEntity, TId, TExecutionContext>(
            Type loaderType,
            Action<MutableLoader<TEntity, TId, TExecutionContext>> onConfigure,
            Func<TExecutionContext, IQueryable<TEntity>> getBaseQuery,
            Func<TExecutionContext, IQueryable<TEntity>, IQueryable<TEntity>> applySecurityFilter = null,
            Func<IExecutionContextAccessor<TExecutionContext>, TEntity, bool, Task<bool>> canSaveAsync = null,
            Action<TExecutionContext, TEntity> beforeCreate = null,
            Action<TExecutionContext, TEntity> beforeUpdate = null,
            string typeName = null,
            Action<TypeBuilder> afterBuild = null)
            where TEntity : class, IHasId<TId>
        {
            void OnConfigureWrapper(Loader<TEntity, TExecutionContext> query)
            {
                onConfigure((MutableLoader<TEntity, TId, TExecutionContext>)query);
            }

            var type = CreateLoaderType(typeName ?? $"{typeof(TEntity).Name}Loader", loaderType, OnConfigureWrapper, getBaseQuery, null, null, applySecurityFilter, typeBuilder =>
            {
                if (canSaveAsync != null)
                {
                    var canSaveAsyncMethodInfo = typeof(MutableLoader<TEntity, TId, TExecutionContext>).GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                        .Single(m => m.Name == "CanSaveAsync" && m.GetParameters().First().ParameterType == typeof(IExecutionContextAccessor<TExecutionContext>));

                    var canSaveAsyncMethodBuilder = typeBuilder.DefineMethod(
                        "CanSaveAsync",
                        MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                        CallingConventions.Standard,
                        canSaveAsyncMethodInfo.ReturnType,
                        canSaveAsyncMethodInfo.GetParameters().Select(p => p.ParameterType).ToArray());
                    var canSaveAsyncGenerator = canSaveAsyncMethodBuilder.GetILGenerator();

                    canSaveAsyncGenerator.Emit(OpCodes.Ldarg_0); // this
                    canSaveAsyncGenerator.Emit(OpCodes.Call, typeof(object).GetMethod("GetType"));
                    canSaveAsyncGenerator.Emit(OpCodes.Ldarg_1); // context
                    canSaveAsyncGenerator.Emit(OpCodes.Ldarg_2); // entity
                    canSaveAsyncGenerator.Emit(OpCodes.Ldarg_3); // isNew
                    canSaveAsyncGenerator.Emit(OpCodes.Call, typeof(GraphQLTypeBuilder).GetMethod(nameof(GraphQLTypeBuilder.CallCanSaveAsync), BindingFlags.Static | BindingFlags.Public));
                    canSaveAsyncGenerator.Emit(OpCodes.Ret);

                    typeBuilder.DefineMethodOverride(canSaveAsyncMethodBuilder, canSaveAsyncMethodInfo);
                }

                if (beforeCreate != null)
                {
                    var beforeCreateMethodInfo = loaderType.GetMethod("BeforeCreate", BindingFlags.Instance | BindingFlags.NonPublic);
                    var beforeCreateMethodBuilder = typeBuilder.DefineMethod(
                        "BeforeCreate",
                        MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                        CallingConventions.Standard,
                        beforeCreateMethodInfo.ReturnType,
                        beforeCreateMethodInfo.GetParameters().Select(p => p.ParameterType).ToArray());
                    var beforeCreateGenerator = beforeCreateMethodBuilder.GetILGenerator();

                    beforeCreateGenerator.Emit(OpCodes.Ldarg_0); // this
                    beforeCreateGenerator.Emit(OpCodes.Call, typeof(object).GetMethod("GetType"));
                    beforeCreateGenerator.Emit(OpCodes.Ldarg_1); // context
                    beforeCreateGenerator.Emit(OpCodes.Ldarg_2); // entity
                    beforeCreateGenerator.Emit(OpCodes.Call, typeof(GraphQLTypeBuilder).GetMethod(nameof(GraphQLTypeBuilder.CallBeforeCreate), BindingFlags.Static | BindingFlags.Public));
                    beforeCreateGenerator.Emit(OpCodes.Ret);

                    typeBuilder.DefineMethodOverride(beforeCreateMethodBuilder, beforeCreateMethodInfo);
                }

                if (beforeUpdate != null)
                {
                    var beforeUpdateMethodInfo = loaderType.GetMethod("BeforeUpdate", BindingFlags.Instance | BindingFlags.NonPublic);
                    var beforeUpdateMethodBuilder = typeBuilder.DefineMethod(
                        "BeforeUpdate",
                        MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                        CallingConventions.Standard,
                        beforeUpdateMethodInfo.ReturnType,
                        beforeUpdateMethodInfo.GetParameters().Select(p => p.ParameterType).ToArray());
                    var beforeUpdateGenerator = beforeUpdateMethodBuilder.GetILGenerator();

                    beforeUpdateGenerator.Emit(OpCodes.Ldarg_0); // this
                    beforeUpdateGenerator.Emit(OpCodes.Call, typeof(object).GetMethod("GetType"));
                    beforeUpdateGenerator.Emit(OpCodes.Ldarg_1); // context
                    beforeUpdateGenerator.Emit(OpCodes.Ldarg_2); // entity
                    beforeUpdateGenerator.Emit(OpCodes.Call, typeof(GraphQLTypeBuilder).GetMethod(nameof(GraphQLTypeBuilder.CallBeforeUpdate), BindingFlags.Static | BindingFlags.Public));
                    beforeUpdateGenerator.Emit(OpCodes.Ret);

                    typeBuilder.DefineMethodOverride(beforeUpdateMethodBuilder, beforeUpdateMethodInfo);
                }

                OverrideIdExpression<TEntity, TId, TExecutionContext>(typeBuilder);

                afterBuild?.Invoke(typeBuilder);
            });

            Expression<Func<TEntity, TId>> idExpression = e => e.Id;
            _idExpressions[type] = idExpression;

            if (canSaveAsync != null)
            {
                _canSaveAsyncFuncs[type] = (context, entity, isNew) => canSaveAsync((IExecutionContextAccessor<TExecutionContext>)context, (TEntity)entity, isNew);
            }

            if (beforeCreate != null)
            {
                _beforeCreateFuncs[type] = (context, entity) => beforeCreate((TExecutionContext)context, (TEntity)entity);
            }

            if (beforeUpdate != null)
            {
                _beforeUpdateFuncs[type] = (context, entity) => beforeUpdate((TExecutionContext)context, (TEntity)entity);
            }

            return type;
        }

        private static Type CreateLoaderType<TEntity, TExecutionContext>(
            string typeName,
            Type loaderType,
            Action<Loader<TEntity, TExecutionContext>> onConfigure,
            Func<TExecutionContext, IQueryable<TEntity>> getBaseQuery,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> applyNaturalOrderBy,
            Func<IOrderedQueryable<TEntity>, IOrderedQueryable<TEntity>> applyNaturalThenBy,
            Func<TExecutionContext, IQueryable<TEntity>, IQueryable<TEntity>> applySecurityFilter,
            Action<TypeBuilder> afterBuild = null)
            where TEntity : class
        {
            void OnConfigureWrapper(ProjectionBase<TEntity, TExecutionContext> query)
            {
                onConfigure((Loader<TEntity, TExecutionContext>)query);
            }

            var type = CreateProjectionBaseType(typeName, loaderType, onConfigure == null ? null : (Action<ProjectionBase<TEntity, TExecutionContext>>)OnConfigureWrapper, typeBuilder =>
            {
                OverrideGetBaseQuery<TEntity, TExecutionContext>(typeBuilder);

                if (applySecurityFilter != null)
                {
                    var applySecurityFilterMethodInfo = typeof(Loader<TEntity, TExecutionContext>).GetMethod("ApplySecurityFilter", BindingFlags.Instance | BindingFlags.NonPublic);
                    var applySecurityFilterMethodBuilder = typeBuilder.DefineMethod(
                        "ApplySecurityFilter",
                        MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                        CallingConventions.Standard,
                        applySecurityFilterMethodInfo.ReturnType,
                        applySecurityFilterMethodInfo.GetParameters().Select(p => p.ParameterType).ToArray());
                    var applySecurityFilterGenerator = applySecurityFilterMethodBuilder.GetILGenerator();

                    applySecurityFilterGenerator.Emit(OpCodes.Ldarg_0); // this
                    applySecurityFilterGenerator.Emit(OpCodes.Call, typeof(object).GetMethod("GetType"));
                    applySecurityFilterGenerator.Emit(OpCodes.Ldarg_1); // context
                    applySecurityFilterGenerator.Emit(OpCodes.Ldarg_2); // query
                    applySecurityFilterGenerator.Emit(OpCodes.Call, typeof(GraphQLTypeBuilder).GetMethod(nameof(GraphQLTypeBuilder.CallApplySecurityFilterFunc), BindingFlags.Static | BindingFlags.Public));
                    applySecurityFilterGenerator.Emit(OpCodes.Castclass, typeof(IQueryable<>).MakeGenericType(typeof(TEntity)));
                    applySecurityFilterGenerator.Emit(OpCodes.Ret);

                    typeBuilder.DefineMethodOverride(applySecurityFilterMethodBuilder, applySecurityFilterMethodInfo);
                }

                if (applyNaturalOrderBy != null)
                {
                    var applyNaturalOrderByMethodInfo = typeof(Loader<TEntity, TExecutionContext>).GetMethod(
                        "ApplyNaturalOrderBy",
                        BindingFlags.Instance | BindingFlags.Public);

                    var applyNaturalOrderByMethodBuilder = typeBuilder.DefineMethod(
                        "ApplyNaturalOrderBy",
                        MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                        CallingConventions.Standard,
                        applyNaturalOrderByMethodInfo.ReturnType,
                        applyNaturalOrderByMethodInfo.GetParameters().Select(p => p.ParameterType).ToArray());
                    var applyNaturalOrderByGenerator = applyNaturalOrderByMethodBuilder.GetILGenerator();

                    applyNaturalOrderByGenerator.Emit(OpCodes.Ldarg_0); // this
                    applyNaturalOrderByGenerator.Emit(OpCodes.Call, typeof(object).GetMethod("GetType"));
                    applyNaturalOrderByGenerator.Emit(OpCodes.Ldarg_1); // query
                    applyNaturalOrderByGenerator.Emit(OpCodes.Call, typeof(GraphQLTypeBuilder).GetMethod(nameof(GraphQLTypeBuilder.CallApplyNaturalOrderByFunc), BindingFlags.Static | BindingFlags.Public));
                    applyNaturalOrderByGenerator.Emit(OpCodes.Castclass, typeof(IOrderedQueryable<>).MakeGenericType(typeof(TEntity)));
                    applyNaturalOrderByGenerator.Emit(OpCodes.Ret);

                    typeBuilder.DefineMethodOverride(applyNaturalOrderByMethodBuilder, applyNaturalOrderByMethodInfo);
                }

                if (applyNaturalThenBy != null)
                {
                    var applyNaturalThenByMethodInfo = typeof(Loader<TEntity, TExecutionContext>).GetMethod(
                        "ApplyNaturalThenBy",
                        BindingFlags.Instance | BindingFlags.Public);

                    var applyNaturalThenByMethodBuilder = typeBuilder.DefineMethod(
                        "ApplyNaturalThenBy",
                        MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                        CallingConventions.Standard,
                        applyNaturalThenByMethodInfo.ReturnType,
                        applyNaturalThenByMethodInfo.GetParameters().Select(p => p.ParameterType).ToArray());
                    var applyNaturalThenByGenerator = applyNaturalThenByMethodBuilder.GetILGenerator();

                    applyNaturalThenByGenerator.Emit(OpCodes.Ldarg_0); // this
                    applyNaturalThenByGenerator.Emit(OpCodes.Call, typeof(object).GetMethod("GetType"));
                    applyNaturalThenByGenerator.Emit(OpCodes.Ldarg_1); // query
                    applyNaturalThenByGenerator.Emit(OpCodes.Call, typeof(GraphQLTypeBuilder).GetMethod(nameof(GraphQLTypeBuilder.CallApplyNaturalThenByFunc), BindingFlags.Static | BindingFlags.Public));
                    applyNaturalThenByGenerator.Emit(OpCodes.Castclass, typeof(IOrderedQueryable<>).MakeGenericType(typeof(TEntity)));
                    applyNaturalThenByGenerator.Emit(OpCodes.Ret);

                    typeBuilder.DefineMethodOverride(applyNaturalThenByMethodBuilder, applyNaturalThenByMethodInfo);
                }

                afterBuild?.Invoke(typeBuilder);
            });

            _getBaseQueryFuncs[type] = context => getBaseQuery((TExecutionContext)context);

            if (applySecurityFilter != null)
            {
                _applySecurityFilterFuncs[type] = (context, query) => applySecurityFilter((TExecutionContext)context, (IQueryable<TEntity>)query);
            }

            if (applyNaturalOrderBy != null)
            {
                _applyNaturalOrderByFuncs[type] = query => applyNaturalOrderBy((IQueryable<TEntity>)query);
            }

            if (applyNaturalThenBy != null)
            {
                _applyNaturalThenByFuncs[type] = query => applyNaturalThenBy((IOrderedQueryable<TEntity>)query);
            }

            return type;
        }

        private static void OverrideGetBaseQuery<TEntity, TExecutionContext>(TypeBuilder typeBuilder)
            where TEntity : class
        {
            var methodInfo = typeof(Loader<TEntity, TExecutionContext>).GetMethod("GetBaseQuery", BindingFlags.Instance | BindingFlags.NonPublic);
            var methodBuilder = typeBuilder.DefineMethod(
                "GetBaseQuery",
                MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                CallingConventions.Standard,
                methodInfo.ReturnType,
                methodInfo.GetParameters().Select(p => p.ParameterType).ToArray());
            var generator = methodBuilder.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0); // this
            generator.Emit(OpCodes.Call, typeof(object).GetMethod("GetType"));
            generator.Emit(OpCodes.Ldarg_1); // context
            generator.Emit(OpCodes.Call, typeof(GraphQLTypeBuilder).GetMethod(nameof(GraphQLTypeBuilder.CallGetBaseQueryFunc), BindingFlags.Static | BindingFlags.Public));
            generator.Emit(OpCodes.Castclass, typeof(IQueryable<>).MakeGenericType(typeof(TEntity)));
            generator.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);
        }

        private static Type CreateSearcherTypeBuilder<TEntity, TExecutionContext>(string typeName, Type baseType, Func<IQueryable<TEntity>, TExecutionContext, string, IQueryable<TEntity>> all, Action<TypeBuilder> afterBuild = null)
            where TEntity : class
        {
            var typeBuilder = GetTypeBuilder(typeName, baseType);

            var methodInfo = baseType.GetMethod("All", BindingFlags.Instance | BindingFlags.Public);
            var methodBuilder = typeBuilder.DefineMethod(
                "All",
                MethodAttributes.Public | MethodAttributes.HideBySig | MethodAttributes.Virtual,
                CallingConventions.Standard,
                methodInfo.ReturnType,
                methodInfo.GetParameters().Select(p => p.ParameterType).ToArray());

            var generator = methodBuilder.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0); // this
            generator.Emit(OpCodes.Call, typeof(object).GetMethod("GetType"));
            generator.Emit(OpCodes.Ldarg_1); // query
            generator.Emit(OpCodes.Ldarg_2); // context
            generator.Emit(OpCodes.Ldarg_3); // search
            generator.Emit(OpCodes.Call, typeof(GraphQLTypeBuilder).GetMethod(nameof(GraphQLTypeBuilder.CallApplySearchFunc), BindingFlags.Static | BindingFlags.Public));
            generator.Emit(OpCodes.Castclass, typeof(IQueryable<>).MakeGenericType(typeof(TEntity)));
            generator.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);

            afterBuild?.Invoke(typeBuilder);

            var type = typeBuilder.CreateTypeInfo();
            _applySearchFuncs[type] = (query, context, search) => all((IQueryable<TEntity>)query, (TExecutionContext)context, search);

            return type;
        }

        private static Type CreateProjectionBaseType<TEntity, TExecutionContext>(string typeName, Type baseType, Action<ProjectionBase<TEntity, TExecutionContext>> onConfigure, Action<TypeBuilder> afterBuild = null)
            where TEntity : class
        {
            var typeBuilder = GetTypeBuilder(typeName, baseType, onConfigure == null);

            OverrideOnConfigure<TExecutionContext>(typeBuilder);

            afterBuild?.Invoke(typeBuilder);

            var type = typeBuilder.CreateTypeInfo();
            _onConfigureActions[type] = query => onConfigure((ProjectionBase<TEntity, TExecutionContext>)query);

            return type;
        }

        private static void OverrideOnConfigure<TExecutionContext>(TypeBuilder typeBuilder)
        {
            var methodInfo = typeof(ProjectionBase<TExecutionContext>).GetMethod("OnConfigure", BindingFlags.Instance | BindingFlags.NonPublic);
            var methodBuilder = typeBuilder.DefineMethod("OnConfigure", MethodAttributes.Family | MethodAttributes.HideBySig | MethodAttributes.Virtual);
            var generator = methodBuilder.GetILGenerator();

            generator.Emit(OpCodes.Ldarg_0); // this
            generator.Emit(OpCodes.Call, typeof(object).GetMethod("GetType"));
            generator.Emit(OpCodes.Ldarg_0); // this
            generator.Emit(OpCodes.Call, typeof(GraphQLTypeBuilder).GetMethod(nameof(GraphQLTypeBuilder.CallOnConfigureAction), BindingFlags.Static | BindingFlags.Public));
            generator.Emit(OpCodes.Ret);

            typeBuilder.DefineMethodOverride(methodBuilder, methodInfo);
        }

        private static TypeBuilder GetTypeBuilder(string typeName, Type baseType, bool isAbstract = false)
        {
            var attributes = TypeAttributes.Public |
                TypeAttributes.Class |
                TypeAttributes.AutoClass |
                TypeAttributes.AnsiClass |
                TypeAttributes.BeforeFieldInit |
                TypeAttributes.AutoLayout;

            if (isAbstract)
            {
                attributes |= TypeAttributes.Abstract;
            }

            var tb = _moduleBuilder.DefineType($"{Guid.NewGuid()}.{typeName}", attributes, baseType);
            return tb;
        }

        private static T GetValueByType<T>(Dictionary<Type, T> dictionary, Type type)
        {
            T result;

            while (!dictionary.TryGetValue(type, out result) && type != null)
            {
                type = type.BaseType;
            }

            return result;
        }
    }
}
