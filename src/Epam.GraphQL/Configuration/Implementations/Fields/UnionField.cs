// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Configuration.Implementations.Fields.Helpers;
using Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields;
using Epam.GraphQL.Diagnostics;

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal static class UnionField
    {
        public static UnionField<TEntity, TExecutionContext> Create<TEntity, TLastElementType, TExecutionContext>(
            MethodCallArgumentConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEntity : class
            where TLastElementType : class
        {
            return new UnionField<TEntity, TExecutionContext>(
                configurationContext.Parent,
                parent,
                name,
                typeof(TLastElementType),
                CreateTypeResolver<TEntity, TLastElementType, TExecutionContext>(build, configurationContext));
        }

        public static Func<UnionFieldBase<TEntity, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> CreateTypeResolver<TEntity, TLastElementType, TExecutionContext>(
            Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build,
            MethodCallArgumentConfigurationContext configurationContext)
            where TEntity : class
            where TLastElementType : class
        {
            return field => field.Parent.GetGraphQLTypeDescriptor(field, build, configurationContext);
        }
    }

    internal class UnionField<TEntity, TExecutionContext> :
        UnionFieldBase<TEntity, TExecutionContext>,
        IUnionableField<TEntity, TExecutionContext>
        where TEntity : class
    {
        public UnionField(
            MethodCallConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<TEntity, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> graphTypeFactory)
            : base(configurationContext, parent, name, unionType, graphTypeFactory)
        {
        }

        private UnionField(
            MethodCallConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<TEntity, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> graphTypeFactory,
            List<Type> unionTypes,
            UnionGraphTypeDescriptor<TExecutionContext> unionGraphType)
            : base(
                configurationContext,
                parent,
                name,
                unionType,
                graphTypeFactory,
                unionTypes,
                unionGraphType)
        {
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve)
        {
            Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve)).Argument(resolve),
                this,
                GraphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(Name, resolve, Parent.ProxyAccessor)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve)
        {
            Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve)).Argument(resolve),
                this,
                GraphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(Name, resolve, Parent.ProxyAccessor)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            // TODO Argument build is not used there
            Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build)
                    .Parent,
                this,
                GraphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(Name, resolve, Parent.ProxyAccessor)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build)
                    .Parent,
                this,
                GraphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(Name, resolve, Parent.ProxyAccessor)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve)
        {
            var graphType = GraphType.MakeListDescriptor();
            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve)).Argument(resolve),
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(Name, resolve, Parent.ProxyAccessor)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve)
        {
            var graphType = GraphType.MakeListDescriptor();
            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve)).Argument(resolve),
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(Name, resolve, Parent.ProxyAccessor)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            var graphType = GraphType.MakeListDescriptor();
            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build)
                    .Parent,
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(Name, resolve, Parent.ProxyAccessor)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            var graphType = GraphType.MakeListDescriptor();
            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build)
                    .Parent,
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(Name, resolve, Parent.ProxyAccessor)));
        }

        public IUnionableField<TEntity, TExecutionContext> AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            return And(build);
        }

        public IUnionableField<TEntity, TExecutionContext> And<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            var configurationContext = ConfigurationContext
                .NextOperation<TLastElementType>(nameof(And))
                .OptionalArgument(build);

            var unionField = new UnionField<TEntity, TExecutionContext>(
                configurationContext.Parent,
                Parent,
                Name,
                typeof(TLastElementType),
                UnionField.CreateTypeResolver<TEntity, TLastElementType, TExecutionContext>(build, configurationContext),
                UnionTypes,
                UnionGraphType);
            return ApplyField(unionField);
        }

        public IUnionableField<TEntity, TExecutionContext> AsUnionOf<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class
        {
            return And<TEnumerable, TLastElementType>(build);
        }

        public IUnionableField<TEntity, TExecutionContext> And<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class
        {
            return And(build);
        }
    }
}
