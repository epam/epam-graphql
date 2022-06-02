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
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Configuration.Implementations.Fields;
using Epam.GraphQL.Configuration.Implementations.Fields.ChildFields;
using Epam.GraphQL.Sorters.Implementations;

namespace Epam.GraphQL.Builders.Projection.Implementations
{
    internal class ProjectionFieldBuilder<TField, TEntity, TExecutionContext> :
        IProjectionField<TEntity, TExecutionContext>
        where TEntity : class
        where TField : FieldBase<TEntity, TExecutionContext>, IUnionableField<TEntity, TExecutionContext>
    {
        public ProjectionFieldBuilder(TField field)
        {
            Field = field;
        }

        protected TField Field { get; }

        public IQueryableField<TEntity, TReturnType, TExecutionContext> FromIQueryable<TReturnType>(
            Func<TExecutionContext, IQueryable<TReturnType>> query,
            Expression<Func<TEntity, TReturnType, bool>> condition,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            var configurationContext = Field.ConfigurationContext.Chain<TReturnType>(nameof(FromIQueryable))
                .Argument(query)
                .Argument(condition)
                .OptionalArgument(build);

            var graphType = Field.Parent.GetGraphQLTypeDescriptor(Field, build, configurationContext);

            var result = new QueryableField<TEntity, TReturnType, TExecutionContext>(
                configurationContext,
                Field.Parent,
                Field.Name,
                query,
                condition,
                graphType,
                searcher: null,
                naturalSorters: SortingHelpers.Empty);

            return Field.Parent.ReplaceField(Field, result);
        }

        public IQueryableField<TEntity, TReturnType, TExecutionContext> FromIQueryable<TReturnType>(
            Func<TExecutionContext, IQueryable<TReturnType>> query,
            Expression<Func<TEntity, TReturnType, bool>> condition)
        {
            return Field.Parent.FromIQueryable(
                Field.ConfigurationContext.Chain<TReturnType>(nameof(FromIQueryable))
                    .Argument(query)
                    .Argument(condition),
                Field,
                query,
                condition);
        }

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
