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
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Mutation;

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal class MutationField<TExecutionContext> :
        FieldBase<object, TExecutionContext>,
        IMutationField<TExecutionContext>
    {
        public MutationField(
            FieldConfigurationContext configurationContext,
            Mutation<TExecutionContext> mutation,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name)
            : base(configurationContext, parent, name)
        {
            Mutation = mutation;
        }

        internal Mutation<TExecutionContext> Mutation { get; }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TReturnType> resolve)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                Parent.GetGraphQLTypeDescriptor<TReturnType>(this),
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
                Parent.GetGraphQLTypeDescriptor<TReturnType>(this),
                Resolvers.ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve)).Argument(resolve),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                Parent.GetGraphQLTypeDescriptor(this, build),
                Resolvers.ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .Argument(build),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                Parent.GetGraphQLTypeDescriptor(this, build),
                Resolvers.ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .Argument(build),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                this,
                Parent.GetGraphQLTypeDescriptor<TReturnType>(this).MakeListDescriptor(),
                Resolvers.ConvertFieldResolver(resolve),
                optionsBuilder);

            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(optionsBuilder),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                this,
                Parent.GetGraphQLTypeDescriptor<TReturnType>(this).MakeListDescriptor(),
                Resolvers.ConvertFieldResolver(resolve),
                optionsBuilder);

            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(optionsBuilder),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                this,
                Parent.GetGraphQLTypeDescriptor(this, build).MakeListDescriptor(),
                Resolvers.ConvertFieldResolver(resolve),
                optionsBuilder);

            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .Argument(build)
                    .OptionalArgument(optionsBuilder),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                this,
                Parent.GetGraphQLTypeDescriptor(this, build).MakeListDescriptor(),
                Resolvers.ConvertFieldResolver(resolve),
                optionsBuilder);

            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .Argument(build)
                    .OptionalArgument(optionsBuilder),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                this,
                Resolvers.ConvertFieldResolver(resolve),
                optionsBuilder);

            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(optionsBuilder),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                this,
                Resolvers.ConvertFieldResolver(resolve),
                optionsBuilder);

            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(optionsBuilder),
                this,
                graphType,
                resolver);
        }

        public IUnionableRootField<TExecutionContext> AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            return And(build);
        }

        public IUnionableRootField<TExecutionContext> And<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            var unionField = UnionMutationField.Create(ConfigurationContext, this, build);
            return Parent.ReplaceField(this, unionField);
        }

        public IUnionableRootField<TExecutionContext> AsUnionOf<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class
        {
            return And<TEnumerable, TLastElementType>(build);
        }

        public IUnionableRootField<TExecutionContext> And<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class
        {
            return And(build);
        }

        public IArgumentedMutationField<TArgType, TExecutionContext> Argument<TArgType>(string argName)
        {
            var argumentedField = new ArgumentedMutationField<TArgType, TExecutionContext>(
                ConfigurationContext.NextOperation<TArgType>(nameof(Argument)).Argument(argName),
                this,
                new Arguments<TArgType, TExecutionContext>(Registry, argName));
            return Parent.ReplaceField(this, argumentedField);
        }

        public IArgumentedMutationField<Expression<Func<TEntity, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity>(string argName)
            where TProjection : Projection<TEntity, TExecutionContext>
            where TEntity : class
        {
            var argumentedField = new ArgumentedMutationField<Expression<Func<TEntity, bool>>, TExecutionContext>(
                ConfigurationContext.NextOperation<TProjection, TEntity>(nameof(FilterArgument)).Argument(argName),
                this,
                new Arguments<Expression<Func<TEntity, bool>>, TExecutionContext>(Registry, argName, typeof(TProjection), typeof(TEntity)));
            return Parent.ReplaceField(this, argumentedField);
        }

        public IPayloadFieldedMutationField<TArgType, TExecutionContext> PayloadField<TArgType>(string argName)
        {
            var payloadedField = new ArgumentedMutationField<TArgType, TExecutionContext>(
                ConfigurationContext.NextOperation<TArgType>(nameof(PayloadField)).Argument(argName),
                this,
                new PayloadFields<TArgType, TExecutionContext>(Name, Registry, argName));
            return Parent.ReplaceField(this, payloadedField);
        }

        public IPayloadFieldedMutationField<Expression<Func<TEntity, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity>(string argName)
            where TProjection : Projection<TEntity, TExecutionContext>
            where TEntity : class
        {
            var argumentedField = new ArgumentedMutationField<Expression<Func<TEntity, bool>>, TExecutionContext>(
                ConfigurationContext.NextOperation<TProjection, TEntity>(nameof(FilterPayloadField)).Argument(argName),
                this,
                new PayloadFields<Expression<Func<TEntity, bool>>, TExecutionContext>(Name, Registry, argName, typeof(TProjection), typeof(TEntity)));
            return Parent.ReplaceField(this, argumentedField);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve)
        {
            return Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve)
        {
            return Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            return Resolve(resolve, build, null);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            return Resolve(resolve, build, null);
        }
    }
}
