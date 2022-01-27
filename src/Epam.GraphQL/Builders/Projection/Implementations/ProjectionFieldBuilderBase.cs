// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration;

namespace Epam.GraphQL.Builders.Projection.Implementations
{
    internal class ProjectionFieldBuilderBase<TField, TEntity, TExecutionContext> :
        IProjectionFieldBuilderBase<TEntity, TExecutionContext>
        where TEntity : class
        where TField : IUnionableField<TEntity, TExecutionContext>
    {
        public ProjectionFieldBuilderBase(TField field)
        {
            Field = field ?? throw new ArgumentNullException(nameof(field));
        }

        protected TField Field { get; }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve)
        {
            Field.Resolve(resolve, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Field.Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Field.Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve)
        {
            Field.Resolve(resolve, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }

        public void Resolve<TReturnType>(
            Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build,
            Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            Field.Resolve(resolve, build, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }

        public void Resolve<TReturnType>(
            Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build,
            Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            Field.Resolve(resolve, build, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TEntity, TReturnType> resolve)
        {
            Field.Resolve((ctx, entity) => resolve(entity), null);
        }

        public void Resolve<TReturnType>(Func<TEntity, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Field.Resolve((ctx, entity) => resolve(entity), build, null);
        }

        public void Resolve<TReturnType>(Func<TEntity, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Field.Resolve((ctx, entity) => resolve(entity), build, null);
        }

        public void Resolve<TReturnType>(Func<TEntity, Task<TReturnType>> resolve)
        {
            Field.Resolve((ctx, entity) => resolve(entity), null);
        }

        public void Resolve<TReturnType>(Func<TEntity, IEnumerable<TReturnType>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public void Resolve<TReturnType>(Func<TEntity, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(Func<TEntity, Task<IEnumerable<TReturnType>>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public void Resolve<TReturnType>(Func<TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(Func<TEntity, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            Field.Resolve((ctx, entity) => resolve(entity), optionsBuilder);
        }

        public void Resolve<TReturnType>(
            Func<TEntity, IEnumerable<TReturnType>> resolve,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build,
            Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            Field.Resolve((ctx, entity) => resolve(entity), build, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            Field.Resolve((ctx, entity) => resolve(entity), optionsBuilder);
        }

        public void Resolve<TReturnType>(
            Func<TEntity, Task<IEnumerable<TReturnType>>> resolve,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build,
            Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            Field.Resolve((ctx, entity) => resolve(entity), build, optionsBuilder);
        }

        public IProjectionFieldBuilderBase<TEntity, TExecutionContext> AsUnionOf<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        public IProjectionFieldBuilderBase<TEntity, TExecutionContext> AsUnionOf<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>>? build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOf(build);
        }

        public IProjectionFieldBuilderBase<TEntity, TExecutionContext> And<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return AndImpl(build);
        }

        public IProjectionFieldBuilderBase<TEntity, TExecutionContext> And<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>>? build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return And(build);
        }

        private ProjectionFieldBuilderBase<IUnionableField<TEntity, TExecutionContext>, TEntity, TExecutionContext> AsUnionOfImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return new ProjectionFieldBuilderBase<IUnionableField<TEntity, TExecutionContext>, TEntity, TExecutionContext>(Field.AsUnionOf(build));
        }

        private ProjectionFieldBuilderBase<IUnionableField<TEntity, TExecutionContext>, TEntity, TExecutionContext> AndImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }
    }
}
