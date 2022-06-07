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
            IInlinedChainConfigurationContext configurationContext,
            MutationField<TExecutionContext> mutationField,
            Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
        {
            return new UnionMutationField<TExecutionContext>(configurationContext, mutationField, typeof(TLastElementType), CreateTypeResolver(build, configurationContext));
        }

        public static Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> CreateTypeResolver<TLastElementType, TExecutionContext>(
            Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build,
            IInlinedChainConfigurationContext configurationContext)
        {
            return field => field.Parent.GetGraphQLTypeDescriptor(field, build, configurationContext);
        }
    }

    internal class UnionMutationField<TExecutionContext> :
        UnionFieldBase<object, TExecutionContext>,
        IUnionableRootField<TExecutionContext>
    {
        public UnionMutationField(
            IChainConfigurationContext configurationContext,
            MutationField<TExecutionContext> mutationField,
            Type unionType,
            Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> graphTypeFactory)
            : base(configurationContext, mutationField.Parent, mutationField.Name, unionType, graphTypeFactory)
        {
            MutationField = mutationField;
        }

        private UnionMutationField(
            IChainConfigurationContext configurationContext,
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

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                GraphType,
                Resolvers.ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                GraphType,
                Resolvers.ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                GraphType.MakeListDescriptor(),
                Resolvers.ConvertFieldResolver(resolve),
                null);

            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                GraphType.MakeListDescriptor(),
                Resolvers.ConvertFieldResolver(resolve),
                null);

            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build),
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
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve)).Argument(resolve),
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
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve)).Argument(resolve),
                this,
                graphType,
                resolver);
        }

        public IUnionableRootField<TExecutionContext> AsUnionOf<TLastElementType2>(Action<IInlineObjectBuilder<TLastElementType2, TExecutionContext>>? build)
        {
            return And(build);
        }

        public IUnionableRootField<TExecutionContext> And<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext
                .Chain<TLastElementType>(nameof(And))
                .OptionalArgument(build);

            var unionField = new UnionMutationField<TExecutionContext>(
                configurationContext,
                MutationField,
                typeof(TLastElementType),
                UnionMutationField.CreateTypeResolver(build, configurationContext),
                UnionTypes,
                UnionGraphType);
            return ApplyField(unionField);
        }

        public IUnionableRootField<TExecutionContext> AsUnionOf<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build = null)
            where TEnumerable : IEnumerable<TLastElementType>
        {
            return And<TEnumerable, TLastElementType>(build);
        }

        public IUnionableRootField<TExecutionContext> And<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build = null)
            where TEnumerable : IEnumerable<TLastElementType>
        {
            return And(build);
        }
    }
}
