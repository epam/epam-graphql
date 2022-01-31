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
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Configuration.Implementations.Fields;

namespace Epam.GraphQL.Builders.Projection.Implementations
{
    internal class ProjectionFieldBuilder<TField, TEntity, TExecutionContext> :
        IProjectionFieldBuilder<TEntity, TExecutionContext>
        where TEntity : class
        where TField : FieldBase<TEntity, TExecutionContext>, IUnionableField<TEntity, TExecutionContext>
    {
        public ProjectionFieldBuilder(TField field)
        {
            Field = field;
        }

        protected TField Field { get; }

        public IFromIQueryableBuilder<TReturnType, TExecutionContext> FromIQueryable<TReturnType>(
            Func<TExecutionContext, IQueryable<TReturnType>> query,
            Expression<Func<TEntity, TReturnType, bool>> condition,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class => FromIQueryableBuilder.Create(Field, query, condition, build);

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve)
        {
            Field.Resolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Field.Resolve(resolve, build);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Field.Resolve(resolve, build);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve)
        {
            Field.Resolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve)
        {
            Field.Resolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Field.Resolve(resolve, build);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve)
        {
            Field.Resolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Field.Resolve(resolve, build);
        }

        public IUnionableField<TEntity, TExecutionContext> AsUnionOf<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return Field.AsUnionOf(build);
        }

        public IUnionableField<TEntity, TExecutionContext> AsUnionOf<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TElementType>
            where TElementType : class
        {
            return Field.AsUnionOf(build);
        }

        public IUnionableField<TEntity, TExecutionContext> And<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return Field.And(build);
        }

        public IUnionableField<TEntity, TExecutionContext> And<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TElementType>
            where TElementType : class
        {
            return Field.And(build);
        }
    }
}
