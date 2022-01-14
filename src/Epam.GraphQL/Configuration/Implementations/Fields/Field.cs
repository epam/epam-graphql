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

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal class Field<TEntity, TExecutionContext> :
        FieldBase<TEntity, TExecutionContext>,
        IArgumentedField<TEntity, TExecutionContext>
        where TEntity : class
    {
        public Field(RelationRegistry<TExecutionContext> registry, BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent, string name)
            : base(registry, parent, name)
        {
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var graphType = Parent.GetGraphQLTypeDescriptor<TReturnType>(this);
            Parent.ApplyResolvedField(this, graphType, Resolvers.ConvertFieldResolver(resolve), optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var graphType = Parent.GetGraphQLTypeDescriptor<TReturnType>(this);
            Parent.ApplyResolvedField(this, graphType, Resolvers.ConvertFieldResolver(resolve), optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build);
            Parent.ApplyResolvedField(this, graphType, Resolvers.ConvertFieldResolver(resolve), optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build);
            Parent.ApplyResolvedField(this, graphType, Resolvers.ConvertFieldResolver(resolve), optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var graphType = Parent.GetGraphQLTypeDescriptor<TReturnType>(this).MakeListDescriptor();
            Parent.ApplyResolvedField(this, graphType, Resolvers.ConvertFieldResolver(resolve), optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var graphType = Parent.GetGraphQLTypeDescriptor<TReturnType>(this).MakeListDescriptor();
            Parent.ApplyResolvedField(this, graphType, Resolvers.ConvertFieldResolver(resolve), optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build).MakeListDescriptor();
            Parent.ApplyResolvedField(this, graphType, Resolvers.ConvertFieldResolver(resolve), optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build).MakeListDescriptor();
            Parent.ApplyResolvedField(this, graphType, Resolvers.ConvertFieldResolver(resolve), optionsBuilder);
        }

        public IUnionableField<TEntity, TExecutionContext> AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
            => Parent.ApplyUnion(this, build);

        public IArgumentedField<TEntity, TArgType, TExecutionContext> Argument<TArgType>(string argName)
            => Parent.ApplyArgument<TArgType>(this, argName);

        public IArgumentedField<TEntity, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
            => Parent.ApplyFilterArgument<TProjection, TEntity1>(this, argName);

        public IArgumentedField<TEntity, TArgType, TExecutionContext> PayloadField<TArgType>(string argName)
            => Parent.ApplyPayloadField<TArgType>(this, argName);

        public IArgumentedField<TEntity, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
            => Parent.ApplyFilterPayloadField<TProjection, TEntity1>(this, argName);
    }
}
