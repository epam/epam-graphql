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
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Mutation;
using GraphQL;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields
{
    internal class ArgumentedMutationField<TArgType, TExecutionContext> :
        ArgumentedFieldBase<IArguments<TArgType, TExecutionContext>, object, TExecutionContext>,
        IArgumentedMutationField<TArgType, TExecutionContext>,
        IPayloadFieldedMutationField<TArgType, TExecutionContext>
    {
        public ArgumentedMutationField(
            MutationField<TExecutionContext> mutationField,
            IArguments<TArgType, TExecutionContext> arguments)
            : base(mutationField.Registry, mutationField.Parent, mutationField.Name, arguments)
        {
            MutationField = mutationField;
        }

        private MutationField<TExecutionContext> MutationField { get; }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, TReturnType> resolve)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                Parent.GetGraphQLTypeDescriptor<TReturnType>(MutationField),
                ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<TReturnType>> resolve)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                Parent.GetGraphQLTypeDescriptor<TReturnType>(MutationField),
                ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                Parent.GetGraphQLTypeDescriptor(MutationField, build),
                ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                Parent.GetGraphQLTypeDescriptor(MutationField, build),
                ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                Parent.GetGraphQLTypeDescriptor<TReturnType>(this).MakeListDescriptor(),
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                Parent.GetGraphQLTypeDescriptor<TReturnType>(this).MakeListDescriptor(),
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                Parent.GetGraphQLTypeDescriptor(this, build).MakeListDescriptor(),
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                Parent.GetGraphQLTypeDescriptor(this, build).MakeListDescriptor(),
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, IEnumerable<TReturnType>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<IEnumerable<TReturnType>>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public IArgumentedMutationField<TArgType, TArgType2, TExecutionContext> Argument<TArgType2>(string argName)
        {
            return ArgumentImpl<TArgType2>(argName);
        }

        public IArgumentedMutationField<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterArgumentImpl<TProjection, TEntity1>(argName);
        }

        public IPayloadFieldedMutationField<TArgType, TArgType2, TExecutionContext> PayloadField<TArgType2>(string argName)
        {
            return ArgumentImpl<TArgType2>(argName);
        }

        public IPayloadFieldedMutationField<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterArgumentImpl<TProjection, TEntity1>(argName);
        }

        public new IUnionableRootField<TArgType, TExecutionContext> AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            var unionField = new ArgumentedUnionMutationField<TArgType, TExecutionContext>(Registry, Parent, Name, typeof(TLastElementType), UnionField.CreateTypeResolver<object, TLastElementType, TExecutionContext>(build), Arguments);
            return ApplyField(unionField);
        }

        public new IUnionableRootField<TArgType, TExecutionContext> And<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            return AsUnionOf(build);
        }

        public new IUnionableRootField<TArgType, TExecutionContext> AsUnionOf<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class
        {
            return AsUnionOf(build);
        }

        public new IUnionableRootField<TArgType, TExecutionContext> And<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class
        {
            return AsUnionOf(build);
        }

        private Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType, TReturnType> resolve)
        {
            return Arguments.GetResolver(resolve);
        }

        private ArgumentedMutationField<TArgType, TArgType2, TExecutionContext> ArgumentImpl<TArgType2>(string argName)
        {
            var argumentedField = new ArgumentedMutationField<TArgType, TArgType2, TExecutionContext>(MutationField, Arguments.Add<TArgType2>(argName));
            return ApplyField(argumentedField);
        }

        private ArgumentedMutationField<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgumentImpl<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = new ArgumentedMutationField<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext>(MutationField, Arguments.AddFilter<TProjection, TEntity1>(argName));
            return ApplyField(argumentedField);
        }
    }

    internal class ArgumentedMutationField<TArgType1, TArgType2, TExecutionContext> :
        ArgumentedFieldBase<IArguments<TArgType1, TArgType2, TExecutionContext>, object, TExecutionContext>,
        IArgumentedMutationField<TArgType1, TArgType2, TExecutionContext>,
        IPayloadFieldedMutationField<TArgType1, TArgType2, TExecutionContext>
    {
        public ArgumentedMutationField(
            MutationField<TExecutionContext> mutationField,
            IArguments<TArgType1, TArgType2, TExecutionContext> arguments)
            : base(mutationField.Registry, mutationField.Parent, mutationField.Name, arguments)
        {
            MutationField = mutationField;
        }

        private MutationField<TExecutionContext> MutationField { get; }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TReturnType> resolve)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(Parent.GetGraphQLTypeDescriptor<TReturnType>(this), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<TReturnType>> resolve)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(Parent.GetGraphQLTypeDescriptor<TReturnType>(this), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(Parent.GetGraphQLTypeDescriptor(this, build), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(Parent.GetGraphQLTypeDescriptor(this, build), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                Parent.GetGraphQLTypeDescriptor<TReturnType>(this).MakeListDescriptor(),
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                Parent.GetGraphQLTypeDescriptor<TReturnType>(this).MakeListDescriptor(),
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                Parent.GetGraphQLTypeDescriptor(this, build).MakeListDescriptor(),
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                Parent.GetGraphQLTypeDescriptor(this, build).MakeListDescriptor(),
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, IEnumerable<TReturnType>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<IEnumerable<TReturnType>>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TExecutionContext> AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            var unionField = new ArgumentedUnionMutationField<TArgType1, TArgType2, TExecutionContext>(Registry, Parent, Name, typeof(TLastElementType), UnionField.CreateTypeResolver<object, TLastElementType, TExecutionContext>(build), Arguments);
            return ApplyField(unionField);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TExecutionContext> And<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            return AsUnionOf(build);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TExecutionContext> AsUnionOf<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class
        {
            return AsUnionOf(build);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TExecutionContext> And<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class
        {
            return AsUnionOf(build);
        }

        public IArgumentedMutationField<TArgType1, TArgType2, TArgType3, TExecutionContext> Argument<TArgType3>(string argName)
        {
            return ArgumentImpl<TArgType3>(argName);
        }

        public IArgumentedMutationField<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterArgumentImpl<TProjection, TEntity1>(argName);
        }

        public IPayloadFieldedMutationField<TArgType1, TArgType2, TArgType3, TExecutionContext> PayloadField<TArgType3>(string argName)
        {
            return ArgumentImpl<TArgType3>(argName);
        }

        public IPayloadFieldedMutationField<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterArgumentImpl<TProjection, TEntity1>(argName);
        }

        private Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TReturnType> resolve)
        {
            return Arguments.GetResolver(resolve);
        }

        private ArgumentedMutationField<TArgType1, TArgType2, TArgType3, TExecutionContext> ArgumentImpl<TArgType3>(string argName)
        {
            var argumentedField = new ArgumentedMutationField<TArgType1, TArgType2, TArgType3, TExecutionContext>(MutationField, Arguments.Add<TArgType3>(argName));
            return ApplyField(argumentedField);
        }

        private ArgumentedMutationField<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgumentImpl<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = new ArgumentedMutationField<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext>(MutationField, Arguments.AddFilter<TProjection, TEntity1>(argName));
            return ApplyField(argumentedField);
        }
    }

    internal class ArgumentedMutationField<TArgType1, TArgType2, TArgType3, TExecutionContext> :
        ArgumentedFieldBase<IArguments<TArgType1, TArgType2, TArgType3, TExecutionContext>, object, TExecutionContext>,
        IArgumentedMutationField<TArgType1, TArgType2, TArgType3, TExecutionContext>,
        IPayloadFieldedMutationField<TArgType1, TArgType2, TArgType3, TExecutionContext>
    {
        public ArgumentedMutationField(
            MutationField<TExecutionContext> mutationField,
            IArguments<TArgType1, TArgType2, TArgType3, TExecutionContext> arguments)
            : base(mutationField.Registry, mutationField.Parent, mutationField.Name, arguments)
        {
            MutationField = mutationField;
        }

        private MutationField<TExecutionContext> MutationField { get; }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TReturnType> resolve)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(Parent.GetGraphQLTypeDescriptor<TReturnType>(this), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<TReturnType>> resolve)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(Parent.GetGraphQLTypeDescriptor<TReturnType>(this), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(Parent.GetGraphQLTypeDescriptor(this, build), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(Parent.GetGraphQLTypeDescriptor(this, build), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                Parent.GetGraphQLTypeDescriptor<TReturnType>(this).MakeListDescriptor(),
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                Parent.GetGraphQLTypeDescriptor<TReturnType>(this).MakeListDescriptor(),
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                Parent.GetGraphQLTypeDescriptor(this, build).MakeListDescriptor(),
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                Parent.GetGraphQLTypeDescriptor(this, build).MakeListDescriptor(),
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, IEnumerable<TReturnType>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<IEnumerable<TReturnType>>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TArgType3, TExecutionContext> AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            var unionField = new ArgumentedUnionMutationField<TArgType1, TArgType2, TArgType3, TExecutionContext>(Registry, Parent, Name, typeof(TLastElementType), UnionField.CreateTypeResolver<object, TLastElementType, TExecutionContext>(build), Arguments);
            return ApplyField(unionField);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TArgType3, TExecutionContext> And<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            return AsUnionOf(build);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TArgType3, TExecutionContext> AsUnionOf<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class
        {
            return AsUnionOf(build);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TArgType3, TExecutionContext> And<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class
        {
            return AsUnionOf(build);
        }

        public IArgumentedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> Argument<TArgType4>(string argName)
        {
            return ArgumentImpl<TArgType4>(argName);
        }

        public IArgumentedMutationField<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterArgumentImpl<TProjection, TEntity1>(argName);
        }

        public IPayloadFieldedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> PayloadField<TArgType4>(string argName)
        {
            return ArgumentImpl<TArgType4>(argName);
        }

        public IPayloadFieldedMutationField<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterArgumentImpl<TProjection, TEntity1>(argName);
        }

        private Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TReturnType> resolve)
        {
            return Arguments.GetResolver(resolve);
        }

        private ArgumentedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> ArgumentImpl<TArgType4>(string argName)
        {
            var argumentedField = new ArgumentedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>(MutationField, Arguments.Add<TArgType4>(argName));
            return ApplyField(argumentedField);
        }

        private ArgumentedMutationField<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgumentImpl<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = new ArgumentedMutationField<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext>(MutationField, Arguments.AddFilter<TProjection, TEntity1>(argName));
            return ApplyField(argumentedField);
        }
    }

    internal class ArgumentedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> :
        ArgumentedFieldBase<IArguments<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, object, TExecutionContext>,
        IArgumentedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>,
        IPayloadFieldedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
    {
        public ArgumentedMutationField(
            MutationField<TExecutionContext> mutationField,
            IArguments<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> arguments)
            : base(mutationField.Registry, mutationField.Parent, mutationField.Name, arguments)
        {
            MutationField = mutationField;
        }

        private MutationField<TExecutionContext> MutationField { get; }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TReturnType> resolve)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(Parent.GetGraphQLTypeDescriptor<TReturnType>(this), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<TReturnType>> resolve)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(Parent.GetGraphQLTypeDescriptor<TReturnType>(this), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(Parent.GetGraphQLTypeDescriptor(this, build), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(Parent.GetGraphQLTypeDescriptor(this, build), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                Parent.GetGraphQLTypeDescriptor<TReturnType>(this).MakeListDescriptor(),
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                Parent.GetGraphQLTypeDescriptor<TReturnType>(this).MakeListDescriptor(),
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                Parent.GetGraphQLTypeDescriptor(this, build).MakeListDescriptor(),
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                Parent.GetGraphQLTypeDescriptor(this, build).MakeListDescriptor(),
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, IEnumerable<TReturnType>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<IEnumerable<TReturnType>>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            var unionField = new ArgumentedUnionMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>(Registry, Parent, Name, typeof(TLastElementType), UnionField.CreateTypeResolver<object, TLastElementType, TExecutionContext>(build), Arguments);
            return ApplyField(unionField);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> And<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            return AsUnionOf(build);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> AsUnionOf<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class
        {
            return AsUnionOf(build);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> And<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class
        {
            return AsUnionOf(build);
        }

        public IArgumentedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> Argument<TArgType5>(string argName)
        {
            return ArgumentImpl<TArgType5>(argName);
        }

        public IArgumentedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterArgumentImpl<TProjection, TEntity1>(argName);
        }

        public IPayloadFieldedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> PayloadField<TArgType5>(string argName)
        {
            return ArgumentImpl<TArgType5>(argName);
        }

        public IPayloadFieldedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterArgumentImpl<TProjection, TEntity1>(argName);
        }

        private Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TReturnType> resolve)
        {
            return Arguments.GetResolver(resolve);
        }

        private ArgumentedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> ArgumentImpl<TArgType5>(string argName)
        {
            var argumentedField = new ArgumentedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>(MutationField, Arguments.Add<TArgType5>(argName));
            return ApplyField(argumentedField);
        }

        private ArgumentedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgumentImpl<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = new ArgumentedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext>(MutationField, Arguments.AddFilter<TProjection, TEntity1>(argName));
            return ApplyField(argumentedField);
        }
    }

    internal class ArgumentedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> :
        ArgumentedFieldBase<IArguments<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, object, TExecutionContext>,
        IArgumentedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>,
        IPayloadFieldedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
    {
        public ArgumentedMutationField(
            MutationField<TExecutionContext> mutationField,
            IArguments<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> arguments)
            : base(mutationField.Registry, mutationField.Parent, mutationField.Name, arguments)
        {
            MutationField = mutationField;
        }

        private MutationField<TExecutionContext> MutationField { get; }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TReturnType> resolve)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(Parent.GetGraphQLTypeDescriptor<TReturnType>(this), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<TReturnType>> resolve)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(Parent.GetGraphQLTypeDescriptor<TReturnType>(this), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(Parent.GetGraphQLTypeDescriptor(this, build), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(Parent.GetGraphQLTypeDescriptor(this, build), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                Parent.GetGraphQLTypeDescriptor<TReturnType>(this).MakeListDescriptor(),
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                Parent.GetGraphQLTypeDescriptor<TReturnType>(this).MakeListDescriptor(),
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                Parent.GetGraphQLTypeDescriptor(this, build).MakeListDescriptor(),
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                Parent.GetGraphQLTypeDescriptor(this, build).MakeListDescriptor(),
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(
                MutationField,
                ConvertFieldResolver(resolve),
                optionsBuilder);

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, IEnumerable<TReturnType>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<IEnumerable<TReturnType>>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            var unionField = new ArgumentedUnionMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>(Registry, Parent, Name, typeof(TLastElementType), UnionField.CreateTypeResolver<object, TLastElementType, TExecutionContext>(build), Arguments);
            return ApplyField(unionField);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> And<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            return AsUnionOf(build);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> AsUnionOf<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class
        {
            return AsUnionOf(build);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> And<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class
        {
            return AsUnionOf(build);
        }

        private Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TReturnType> resolve)
        {
            return Arguments.GetResolver(resolve);
        }
    }
}
