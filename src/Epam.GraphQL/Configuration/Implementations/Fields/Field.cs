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

#nullable enable

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal class Field<TEntity, TExecutionContext> :
        FieldBase<TEntity, TExecutionContext>,
        IFieldSupportsApplyResolve<TEntity, TExecutionContext>,
        IFieldSupportsApplyUnion<TEntity, TExecutionContext>
        where TEntity : class
    {
        public Field(RelationRegistry<TExecutionContext> registry, BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent, string name)
            : base(registry, parent, name)
        {
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            var graphType = Parent.GetGraphQLTypeDescriptor<TReturnType>(this);
            var resolvedField = ResolvedField.Create(Registry, Parent, Name, graphType, Resolvers.ConvertFieldResolver(resolve, doesDependOnAllFields), Arguments, doesDependOnAllFields, optionsBuilder);
            return Parent.ReplaceField(this, resolvedField);
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            var graphType = Parent.GetGraphQLTypeDescriptor<TReturnType>(this);
            var resolvedField = ResolvedField.Create(Registry, Parent, Name, graphType, Resolvers.ConvertFieldResolver(resolve, doesDependOnAllFields), Arguments, doesDependOnAllFields, optionsBuilder);
            return Parent.ReplaceField(this, resolvedField);
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class
        {
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build);
            var resolvedField = ResolvedField.Create(Registry, Parent, Name, graphType, Resolvers.ConvertFieldResolver(resolve, doesDependOnAllFields), Arguments, doesDependOnAllFields, optionsBuilder);
            return Parent.ReplaceField(this, resolvedField);
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class
        {
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build);
            var resolvedField = ResolvedField.Create(Registry, Parent, Name, graphType, Resolvers.ConvertFieldResolver(resolve, doesDependOnAllFields), Arguments, doesDependOnAllFields, optionsBuilder);
            return Parent.ReplaceField(this, resolvedField);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            var graphType = Parent.GetGraphQLTypeDescriptor<TReturnType>(this).MakeListDescriptor();
            var resolvedField = ResolvedField.Create(Registry, Parent, Name, graphType, Resolvers.ConvertFieldResolver(resolve, doesDependOnAllFields), Arguments, doesDependOnAllFields, optionsBuilder);
            return Parent.ReplaceField(this, resolvedField);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            var graphType = Parent.GetGraphQLTypeDescriptor<TReturnType>(this).MakeListDescriptor();
            var resolvedField = ResolvedField.Create(Registry, Parent, Name, graphType, Resolvers.ConvertFieldResolver(resolve, doesDependOnAllFields), Arguments, doesDependOnAllFields, optionsBuilder);
            return Parent.ReplaceField(this, resolvedField);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class
        {
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build).MakeListDescriptor();
            var resolvedField = ResolvedField.Create(Registry, Parent, Name, graphType, Resolvers.ConvertFieldResolver(resolve, doesDependOnAllFields), Arguments, doesDependOnAllFields, optionsBuilder);
            return Parent.ReplaceField(this, resolvedField);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class
        {
            // TODO throw if TReturnType is IEnumerable
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build).MakeListDescriptor();
            var resolvedField = ResolvedField.Create(Registry, Parent, Name, graphType, Resolvers.ConvertFieldResolver(resolve, doesDependOnAllFields), Arguments, doesDependOnAllFields, optionsBuilder);
            return Parent.ReplaceField(this, resolvedField);
        }

        public UnionField<TEntity, TExecutionContext> ApplyUnion<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>> build, bool isList)
            where TLastElementType : class
            => Parent.ApplyUnion(this, build, isList);

        public ArgumentedField<TEntity, TArgType, TExecutionContext> ApplyArgument<TArgType>(string argName)
            => Parent.ApplyArgument<TArgType>(this, argName);

        public ArgumentedField<TEntity, Expression<Func<TEntity1, bool>>, TExecutionContext> ApplyFilterArgument<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
            => Parent.ApplyFilterArgument<TProjection, TEntity1>(this, argName);

        public ArgumentedField<TEntity, TArgType, TExecutionContext> ApplyPayloadField<TArgType>(string argName)
            => Parent.ApplyPayloadField<TArgType>(this, argName);

        public ArgumentedField<TEntity, Expression<Func<TEntity1, bool>>, TExecutionContext> ApplyFilterPayloadField<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
            => Parent.ApplyFilterPayloadField<TProjection, TEntity1>(this, argName);
    }
}
