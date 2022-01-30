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
using Epam.GraphQL.Mutation;

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal static class UnionMutationField
    {
        public static UnionMutationField<TExecutionContext> Create<TLastElementType, TExecutionContext>(
            MutationField<TExecutionContext> mutationField,
            Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            return new UnionMutationField<TExecutionContext>(mutationField, typeof(TLastElementType), CreateTypeResolver(build));
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
            MutationField<TExecutionContext> mutationField,
            Type unionType,
            Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> graphTypeFactory)
            : base(mutationField.Parent, mutationField.Name, unionType, graphTypeFactory)
        {
            MutationField = mutationField;
        }

        private UnionMutationField(
            MutationField<TExecutionContext> mutationField,
            Type unionType,
            Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> graphTypeFactory,
            List<Type> unionTypes,
            UnionGraphTypeDescriptor<TExecutionContext> unionGraphType)
            : base(
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

        public void Resolve<TReturnType>(Func<TExecutionContext, TReturnType> resolve)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                GraphType,
                Resolvers.ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<TReturnType>> resolve)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                GraphType,
                Resolvers.ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                GraphType,
                Resolvers.ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                GraphType,
                Resolvers.ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                GraphType.MakeListDescriptor(),
                Resolvers.ConvertFieldResolver(resolve),
                null);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                GraphType.MakeListDescriptor(),
                Resolvers.ConvertFieldResolver(resolve),
                null);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                GraphType.MakeListDescriptor(),
                Resolvers.ConvertFieldResolver(resolve),
                null);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                GraphType.MakeListDescriptor(),
                Resolvers.ConvertFieldResolver(resolve),
                null);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                Resolvers.ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                Resolvers.ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public IUnionableRootField<TExecutionContext> AsUnionOf<TLastElementType2>(Action<IInlineObjectBuilder<TLastElementType2, TExecutionContext>>? build)
            where TLastElementType2 : class
        {
            var unionField = new UnionMutationField<TExecutionContext>(MutationField, typeof(TLastElementType2), UnionMutationField.CreateTypeResolver(build), UnionTypes, UnionGraphType);
            return ApplyField(unionField);
        }

        public IUnionableRootField<TExecutionContext> And<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            return AsUnionOf(build);
        }

        public IUnionableRootField<TExecutionContext> AsUnionOf<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build = null)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class
        {
            return AsUnionOf(build);
        }

        public IUnionableRootField<TExecutionContext> And<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build = null)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class
        {
            return AsUnionOf(build);
        }
    }
}
