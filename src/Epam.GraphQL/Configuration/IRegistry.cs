// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration.Implementations;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Enums;
using Epam.GraphQL.Filters;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Search;

namespace Epam.GraphQL.Configuration
{
    internal interface IRegistry
    {
    }

    internal interface IRegistry<TExecutionContext> : IRegistry, IServiceProvider
    {
        Type GenerateGraphType(Type type);

        Type GenerateInputGraphType(Type type);

        Func<TExecutionContext, TArgType, TResultType> WrapFuncByUnusedContext<TArgType, TResultType>(Func<TArgType, TResultType> func);

        InputObjectGraphTypeConfigurator<TEntity, TExecutionContext> RegisterInputAutoObjectGraphType<TEntity>(IObjectConfigurationContext configurationContext);

        TLoader ResolveLoader<TLoader, TEntity>()
            where TLoader : ProjectionBase<TEntity, TExecutionContext>, new();

        IFilter<TEntity, TExecutionContext> ResolveFilter<TEntity>(Type loaderFilterType);

        ISearcher<TEntity, TExecutionContext> ResolveSearcher<TSearcher, TEntity>()
            where TSearcher : ISearcher<TEntity, TExecutionContext>;

        IObjectGraphTypeConfigurator<TEntity, TExecutionContext> Register<TEntity>(Type projectionType);

        IObjectGraphTypeConfigurator<TEntity, TExecutionContext> RegisterInput<TEntity>(Type projectionType);

        void Register<TEntity, TChildEntity>(
            Type loaderType,
            Type childLoaderType,
            Expression<Func<TEntity, TChildEntity, bool>> relationCondition,
            Expression<Func<TChildEntity, TEntity>>? navigationProperty,
            Expression<Func<TEntity, TChildEntity>>? childNavigationProperty,
            RelationType relationType);

        void SetGraphQLTypeName<TEntity>(string? oldName, string newName);

        void SetGraphQLTypeName<TProjection, TEntity>(string? oldName, string newName)
            where TProjection : ProjectionBase<TEntity, TExecutionContext>;

        string GetGraphQLAutoTypeName<TEntity>(bool isInput);

        string GetGraphQLTypeName<TEntity>(bool isInput, IField<TExecutionContext>? parent);

        string GetGraphQLTypeName(Type entityType, Type? projectionType, bool isInput, IField<TExecutionContext>? parent, string? name = null);

        string GetProjectionTypeName<TProjection, TEntity>(bool isInput)
            where TProjection : ProjectionBase<TEntity, TExecutionContext>, new();

        IGraphTypeDescriptor<TReturnType, TExecutionContext> GetGraphTypeDescriptor<TReturnType>(
            IField<TExecutionContext> parent,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build,
            IChainConfigurationContext configurationContext);

        IGraphTypeDescriptor<TEntity, TExecutionContext> GetGraphTypeDescriptor<TProjection, TEntity>()
            where TProjection : ProjectionBase<TEntity, TExecutionContext>, new();

        IGraphTypeDescriptor<TReturnType, TExecutionContext> GetInputGraphTypeDescriptor<TReturnType>(
            IField<TExecutionContext> parent,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build,
            IChainConfigurationContext configurationContext);

        Type GetEntityGraphType<TProjection, TEntity>()
            where TProjection : ProjectionBase<TEntity, TExecutionContext>, new();

        Type GetInputEntityGraphType<TProjection, TEntity>()
            where TProjection : ProjectionBase<TEntity, TExecutionContext>, new();

        Type GetPropperBaseProjectionType<TProjection, TEntity>(
            Func<IObjectGraphTypeConfigurator<TExecutionContext>, IObjectGraphTypeConfigurator<TExecutionContext>, bool> equalPredicate)
            where TProjection : ProjectionBase<TEntity, TExecutionContext>, new();

        IObjectGraphTypeConfigurator<TExecutionContext>? GetObjectGraphTypeConfigurator(Type type, Type? loaderType = null);

        bool IsSimpleType(Type type);
    }
}
