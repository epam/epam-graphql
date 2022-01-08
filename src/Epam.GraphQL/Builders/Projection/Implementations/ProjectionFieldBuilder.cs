// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Builders.Loader.Implementations;
using Epam.GraphQL.Configuration.Implementations;
using Epam.GraphQL.Configuration.Implementations.Fields;

namespace Epam.GraphQL.Builders.Projection.Implementations
{
    internal class ProjectionFieldBuilder<TField, TEntity, TExecutionContext> :
        IProjectionFieldBuilder<TEntity, TExecutionContext>
        where TEntity : class
        where TField : FieldBase<TEntity, TExecutionContext>, IFieldSupportsApplyResolve<TEntity, TExecutionContext>
    {
        public ProjectionFieldBuilder(TField field)
        {
            Field = field ?? throw new ArgumentNullException(nameof(field));
        }

        protected TField Field { get; }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve)
        {
            Field.ApplyResolve(resolve, true, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Field.ApplyResolve(resolve, build, true, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Field.ApplyResolve(resolve, build, true, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve)
        {
            Field.ApplyResolve(resolve, true, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.ApplyResolve(resolve, true, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class
        {
            Field.ApplyResolve(resolve, build, true, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.ApplyResolve(resolve, true, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class
        {
            Field.ApplyResolve(resolve, build, true, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TEntity, TReturnType> resolve)
        {
            Field.ApplyResolve((ctx, entity) => resolve(entity), true, null);
        }

        public void Resolve<TReturnType>(Func<TEntity, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Field.ApplyResolve((ctx, entity) => resolve(entity), build, true, null);
        }

        public void Resolve<TReturnType>(Func<TEntity, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Field.ApplyResolve((ctx, entity) => resolve(entity), build, true, null);
        }

        public void Resolve<TReturnType>(Func<TEntity, Task<TReturnType>> resolve)
        {
            Field.ApplyResolve((ctx, entity) => resolve(entity), true, null);
        }

        public void Resolve<TReturnType>(Func<TEntity, IEnumerable<TReturnType>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>)null);
        }

        public void Resolve<TReturnType>(Func<TEntity, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(Func<TEntity, Task<IEnumerable<TReturnType>>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>)null);
        }

        public void Resolve<TReturnType>(Func<TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(Func<TEntity, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.ApplyResolve((ctx, entity) => resolve(entity), true, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TEntity, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class
        {
            Field.ApplyResolve((ctx, entity) => resolve(entity), build, true, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.ApplyResolve((ctx, entity) => resolve(entity), true, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class
        {
            Field.ApplyResolve((ctx, entity) => resolve(entity), build, true, optionsBuilder);
        }

        public IProjectionFieldBuilder<TEntity, TExecutionContext> AsUnionOf<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        public IProjectionFieldBuilder<TEntity, TExecutionContext> AsUnionOf<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        public IProjectionFieldBuilder<TEntity, TExecutionContext> And<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AndImpl(build);
        }

        public IProjectionFieldBuilder<TEntity, TExecutionContext> And<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AndImpl<TEnumerable, TElementType>(build);
        }

        public IFromIQueryableBuilder<TReturnType, TExecutionContext> FromIQueryable<TReturnType>(Func<TExecutionContext, IQueryable<TReturnType>> query, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> configure)
            where TReturnType : class
        {
            return new FromIQueryableBuilder<TEntity, TReturnType, TExecutionContext>(Field.Parent.FromIQueryableClass(Field, query, null, configure));
        }

        public IFromIQueryableBuilder<TReturnType, TExecutionContext> FromIQueryable<TReturnType>(Func<TExecutionContext, IQueryable<TReturnType>> query, Expression<Func<TEntity, TReturnType, bool>> condition, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class => FromIQueryableBuilder.Create(Field, query, condition, build);

        private ProjectionFieldBuilder<UnionField<TEntity, TExecutionContext>, TEntity, TExecutionContext> AsUnionOfImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return new ProjectionFieldBuilder<UnionField<TEntity, TExecutionContext>, TEntity, TExecutionContext>(Field.ApplyUnion(build, false));
        }

        private ProjectionFieldBuilder<UnionField<TEntity, TExecutionContext>, TEntity, TExecutionContext> AsUnionOfImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return new ProjectionFieldBuilder<UnionField<TEntity, TExecutionContext>, TEntity, TExecutionContext>(Field.ApplyUnion(build, true));
        }

        private ProjectionFieldBuilder<UnionField<TEntity, TExecutionContext>, TEntity, TExecutionContext> AndImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        private ProjectionFieldBuilder<UnionField<TEntity, TExecutionContext>, TEntity, TExecutionContext> AndImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOfImpl(build);
        }
    }
}
