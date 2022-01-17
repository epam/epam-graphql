// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Configuration.Implementations.Fields.Helpers;
using Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Mutation;

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal class MutationField<TExecutionContext> :
        FieldBase<object, TExecutionContext>,
        IArgumentedMutationField<TExecutionContext>
    {
        public MutationField(Mutation<TExecutionContext> mutation, RelationRegistry<TExecutionContext> registry, BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent, string name)
            : base(registry, parent, name)
        {
            Mutation = mutation;
        }

        internal Mutation<TExecutionContext> Mutation { get; }

        public void Resolve<TReturnType>(Func<TExecutionContext, object, TReturnType> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                Parent.GetGraphQLTypeDescriptor<TReturnType>(this),
                Resolvers.ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, object, Task<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                Parent.GetGraphQLTypeDescriptor<TReturnType>(this),
                Resolvers.ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, object, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                Parent.GetGraphQLTypeDescriptor(this, build),
                Resolvers.ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, object, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                Parent.GetGraphQLTypeDescriptor(this, build),
                Resolvers.ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, object, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                this,
                Parent.GetGraphQLTypeDescriptor<TReturnType>(this).MakeListDescriptor(),
                Resolvers.ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, object, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                this,
                Parent.GetGraphQLTypeDescriptor<TReturnType>(this).MakeListDescriptor(),
                Resolvers.ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, object, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                this,
                Parent.GetGraphQLTypeDescriptor(this, build).MakeListDescriptor(),
                Resolvers.ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, object, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                this,
                Parent.GetGraphQLTypeDescriptor(this, build).MakeListDescriptor(),
                Resolvers.ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                this,
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
                this,
                Resolvers.ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public IUnionableMutationField<TExecutionContext> AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            var unionField = UnionMutationField.Create(this, build);
            return Parent.ReplaceField(this, unionField);
        }

        public IArgumentedMutationField<TArgType, TExecutionContext> Argument<TArgType>(string argName)
        {
            var argumentedField = new ArgumentedMutationField<TArgType, TExecutionContext>(this, new Arguments<TArgType, TExecutionContext>(Registry, argName));
            return Parent.ReplaceField(this, argumentedField);
        }

        public IArgumentedMutationField<Expression<Func<TEntity, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity>(string argName)
            where TProjection : Projection<TEntity, TExecutionContext>
            where TEntity : class
        {
            var argumentedField = new ArgumentedMutationField<Expression<Func<TEntity, bool>>, TExecutionContext>(this, new Arguments<Expression<Func<TEntity, bool>>, TExecutionContext>(Registry, argName, typeof(TProjection), typeof(TEntity)));
            return Parent.ReplaceField(this, argumentedField);
        }

        public IArgumentedMutationField<TArgType, TExecutionContext> PayloadField<TArgType>(string argName)
        {
            var payloadedField = new ArgumentedMutationField<TArgType, TExecutionContext>(this, new PayloadFields<TArgType, TExecutionContext>(Name, Registry, argName));
            return Parent.ReplaceField(this, payloadedField);
        }

        public IArgumentedMutationField<Expression<Func<TEntity, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity>(string argName)
            where TProjection : Projection<TEntity, TExecutionContext>
            where TEntity : class
        {
            var argumentedField = new ArgumentedMutationField<Expression<Func<TEntity, bool>>, TExecutionContext>(this, new PayloadFields<Expression<Func<TEntity, bool>>, TExecutionContext>(Name, Registry, argName, typeof(TProjection), typeof(TEntity)));
            return Parent.ReplaceField(this, argumentedField);
        }
    }
}
