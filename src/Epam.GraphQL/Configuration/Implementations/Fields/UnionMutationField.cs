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
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Mutation;

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal static class UnionMutationField
    {
        public static UnionMutationField<TExecutionContext> Create<TLastElementType, TExecutionContext>(
            FieldConfigurationContext configurationContext,
            MutationField<TExecutionContext> mutationField,
            Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            return new UnionMutationField<TExecutionContext>(configurationContext, mutationField, typeof(TLastElementType), CreateTypeResolver(build));
        }

        public static Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> CreateTypeResolver<TLastElementType, TExecutionContext>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            return field => field.Parent.GetGraphQLTypeDescriptor(field, build);
        }
    }

    internal class UnionMutationField<TExecutionContext> :
        UnionFieldBase<object, TExecutionContext>,
        IUnionableRootField<TExecutionContext>
    {
        public UnionMutationField(
            FieldConfigurationContext configurationContext,
            MutationField<TExecutionContext> mutationField,
            Type unionType,
            Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> graphTypeFactory)
            : base(configurationContext, mutationField.Parent, mutationField.Name, unionType, graphTypeFactory)
        {
            MutationField = mutationField;
        }

        private UnionMutationField(
            FieldConfigurationContext configurationContext,
            MutationField<TExecutionContext> mutationField,
            Type unionType,
            Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> graphTypeFactory,
            List<Type> unionTypes,
            UnionGraphTypeDescriptor<TExecutionContext> unionGraphType)
            : base(
                configurationContext,
                mutationField.Parent,
                mutationField.Name,
                unionType,
                graphTypeFactory,
                unionTypes,
                unionGraphType)
        {
            MutationField = mutationField;
        }

        private MutationField<TExecutionContext> MutationField { get; }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TReturnType> resolve)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                GraphType,
                Resolvers.ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve)).Argument(resolve),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, Task<TReturnType>> resolve)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                GraphType,
                Resolvers.ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve)).Argument(resolve),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                GraphType,
                Resolvers.ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve)).Argument(resolve),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                GraphType,
                Resolvers.ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve)).Argument(resolve),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                GraphType.MakeListDescriptor(),
                Resolvers.ConvertFieldResolver(resolve),
                null);

            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve)).Argument(resolve),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                GraphType.MakeListDescriptor(),
                Resolvers.ConvertFieldResolver(resolve),
                null);

            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve)).Argument(resolve),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                GraphType.MakeListDescriptor(),
                Resolvers.ConvertFieldResolver(resolve),
                null);

            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve)).Argument(resolve),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                GraphType.MakeListDescriptor(),
                Resolvers.ConvertFieldResolver(resolve),
                null);

            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve)).Argument(resolve),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                Resolvers.ConvertFieldResolver(resolve),
                optionsBuilder);

            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve)).Argument(resolve),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                Resolvers.ConvertFieldResolver(resolve),
                optionsBuilder);

            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve)).Argument(resolve),
                this,
                graphType,
                resolver);
        }

        public IUnionableRootField<TExecutionContext> AsUnionOf<TLastElementType2>(Action<IInlineObjectBuilder<TLastElementType2, TExecutionContext>>? build)
            where TLastElementType2 : class
        {
            return And(build);
        }

        public IUnionableRootField<TExecutionContext> And<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            var unionField = new UnionMutationField<TExecutionContext>(
                ConfigurationContext.NextOperation<TLastElementType>(nameof(And)).OptionalArgument(build),
                MutationField,
                typeof(TLastElementType),
                UnionMutationField.CreateTypeResolver(build),
                UnionTypes,
                UnionGraphType);
            return ApplyField(unionField);
        }

        public IUnionableRootField<TExecutionContext> AsUnionOf<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build = null)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class
        {
            return And<TEnumerable, TLastElementType>(build);
        }

        public IUnionableRootField<TExecutionContext> And<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build = null)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class
        {
            return And(build);
        }
    }
}
