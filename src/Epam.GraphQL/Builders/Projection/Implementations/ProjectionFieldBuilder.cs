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
using Epam.GraphQL.Configuration.Implementations.Fields;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Mutation;

namespace Epam.GraphQL.Builders.Projection.Implementations
{
    internal class ProjectionFieldBuilder<TEntity, TExecutionContext> :
        IQueryFieldBuilder<TExecutionContext>,
        IProjectionFieldBuilder<TEntity, TExecutionContext>,
        IMutationFieldBuilder<TExecutionContext>
        where TEntity : class
    {
        public ProjectionFieldBuilder(Field<TEntity, TExecutionContext> field)
        {
            Field = field ?? throw new ArgumentNullException(nameof(field));
        }

        protected Field<TEntity, TExecutionContext> Field { get; set; }

        public void Resolve<TReturnType>(Func<TExecutionContext, TReturnType> resolve)
        {
            Field.ApplyResolve((ctx, entity) => resolve(ctx), false, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Field.ApplyResolve((ctx, entity) => resolve(ctx), build, false, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Field.ApplyResolve((ctx, entity) => resolve(ctx), build, false, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<TReturnType>> resolve)
        {
            Field.ApplyResolve((ctx, entity) => resolve(ctx), false, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.ApplyResolve((ctx, entity) => resolve(ctx), false, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class
        {
            Field.ApplyResolve((ctx, entity) => resolve(ctx), build, false, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.ApplyResolve((ctx, entity) => resolve(ctx), false, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class
        {
            Field.ApplyResolve((ctx, entity) => resolve(ctx), build, false, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, MutationResult<TReturnType>> resolve)
        {
            Field.ApplyResolve((ctx, entity) => resolve(ctx), false, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<MutationResult<TReturnType>>> resolve)
        {
            Field.ApplyResolve((ctx, entity) => resolve(ctx), false, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.ApplyResolve((ctx, entity) => resolve(ctx), false, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.ApplyResolve((ctx, entity) => resolve(ctx), false, optionsBuilder);
        }

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

        IQueryFieldBuilder<TExecutionContext> IUnionableProjectionFieldBuilder<IQueryFieldBuilder<TExecutionContext>, TExecutionContext>.AsUnionOf<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
        {
            return AsUnionOfImpl(build);
        }

        IQueryFieldBuilder<TExecutionContext> IUnionableProjectionFieldBuilder<IQueryFieldBuilder<TExecutionContext>, TExecutionContext>.AsUnionOf<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        IQueryFieldBuilder<TExecutionContext> IUnionableProjectionFieldBuilder<IQueryFieldBuilder<TExecutionContext>, TExecutionContext>.And<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
        {
            return AndImpl(build);
        }

        IQueryFieldBuilder<TExecutionContext> IUnionableProjectionFieldBuilder<IQueryFieldBuilder<TExecutionContext>, TExecutionContext>.And<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
        {
            return AndImpl<TEnumerable, TElementType>(build);
        }

        IMutationFieldBuilder<TExecutionContext> IUnionableProjectionFieldBuilder<IMutationFieldBuilder<TExecutionContext>, TExecutionContext>.AsUnionOf<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
        {
            return AsUnionOfImpl(build);
        }

        IMutationFieldBuilder<TExecutionContext> IUnionableProjectionFieldBuilder<IMutationFieldBuilder<TExecutionContext>, TExecutionContext>.AsUnionOf<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        IMutationFieldBuilder<TExecutionContext> IUnionableProjectionFieldBuilder<IMutationFieldBuilder<TExecutionContext>, TExecutionContext>.And<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
        {
            return AndImpl(build);
        }

        IMutationFieldBuilder<TExecutionContext> IUnionableProjectionFieldBuilder<IMutationFieldBuilder<TExecutionContext>, TExecutionContext>.And<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
        {
            return AndImpl<TEnumerable, TElementType>(build);
        }

        public IQueryArgumentBuilder<TType, TExecutionContext> Argument<TType>(string name)
        {
            return ArgumentImpl<TType>(name);
        }

        IQueryArgumentBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext> IQueryFieldBuilder<TExecutionContext>.FilterArgument<TProjection, TEntity1>(string name)
        {
            return FilterArgumentImpl<TProjection, TEntity1>(name);
        }

        IQueryArgumentBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext> IQueryFieldBuilder<TExecutionContext>.FilterArgument<TEntity1>(Type projectionType, string name)
        {
            var methodInfo = typeof(IQueryFieldBuilder<TExecutionContext>).GetPublicGenericMethod(
                nameof(IQueryFieldBuilder<TExecutionContext>.FilterArgument),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IQueryArgumentBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        IMutationArgumentBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext> IMutationFieldBuilder<TExecutionContext>.FilterArgument<TProjection, TEntity1>(string name)
        {
            return FilterArgumentImpl<TProjection, TEntity1>(name);
        }

        IMutationArgumentBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext> IMutationFieldBuilder<TExecutionContext>.FilterArgument<TEntity1>(Type projectionType, string name)
        {
            var methodInfo = typeof(IMutationFieldBuilder<TExecutionContext>).GetPublicGenericMethod(
                nameof(IMutationFieldBuilder<TExecutionContext>.FilterArgument),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IMutationArgumentBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        IMutationArgumentBuilder<TType, TExecutionContext> IMutationFieldBuilder<TExecutionContext>.Argument<TType>(string name)
        {
            return ArgumentImpl<TType>(name);
        }

        public IQueryPayloadFieldBuilder<TType, TExecutionContext> PayloadField<TType>(string name)
        {
            return PayloadFieldImpl<TType>(name);
        }

        IMutationPayloadFieldBuilder<TType, TExecutionContext> IMutationFieldBuilder<TExecutionContext>.PayloadField<TType>(string name)
        {
            return PayloadFieldImpl<TType>(name);
        }

        IQueryPayloadFieldBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext> IQueryFieldBuilder<TExecutionContext>.FilterPayloadField<TProjection, TEntity1>(string name)
        {
            return FilterPayloadFieldImpl<TProjection, TEntity1>(name);
        }

        IQueryPayloadFieldBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext> IQueryFieldBuilder<TExecutionContext>.FilterPayloadField<TEntity1>(Type projectionType, string name)
        {
            var methodInfo = typeof(IQueryFieldBuilder<TExecutionContext>).GetPublicGenericMethod(
                nameof(IQueryFieldBuilder<TExecutionContext>.FilterPayloadField),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IQueryPayloadFieldBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        IMutationPayloadFieldBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext> IMutationFieldBuilder<TExecutionContext>.FilterPayloadField<TProjection, TEntity1>(string name)
        {
            return FilterPayloadFieldImpl<TProjection, TEntity1>(name);
        }

        IMutationPayloadFieldBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext> IMutationFieldBuilder<TExecutionContext>.FilterPayloadField<TEntity1>(Type projectionType, string name)
        {
            var methodInfo = typeof(IMutationFieldBuilder<TExecutionContext>).GetPublicGenericMethod(
                nameof(IMutationFieldBuilder<TExecutionContext>.FilterPayloadField),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IMutationPayloadFieldBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
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

        protected ProjectionFieldBuilder<TEntity, TExecutionContext> AsUnionOfImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            Field = Field.ApplyUnion(build, false);
            return this;
        }

        protected ProjectionFieldBuilder<TEntity, TExecutionContext> AsUnionOfImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            Field = Field.ApplyUnion(build, true);
            return this;
        }

        protected ProjectionFieldBuilder<TEntity, TExecutionContext> AndImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        protected ProjectionFieldBuilder<TEntity, TExecutionContext> AndImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOfImpl(build);
        }

        private IRootProjectionArgumentBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgumentImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new ProjectionArgumentBuilder<TEntity, Expression<Func<TEntity1, bool>>, TExecutionContext>(argumentedField);
        }

        private IRootProjectionArgumentBuilder<TType, TExecutionContext> ArgumentImpl<TType>(string name)
        {
            var argumentedField = Field.ApplyArgument<TType>(name);
            return new ProjectionArgumentBuilder<TEntity, TType, TExecutionContext>(argumentedField);
        }

        private IRootProjectionPayloadFieldBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadFieldImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var payloadedField = Field.ApplyFilterPayloadField<TProjection, TEntity1>(name);
            return new ProjectionPayloadFieldBuilder<TEntity, Expression<Func<TEntity1, bool>>, TExecutionContext>(payloadedField);
        }

        private IRootProjectionPayloadFieldBuilder<TType, TExecutionContext> PayloadFieldImpl<TType>(string name)
        {
            var payloadedField = Field.ApplyPayloadField<TType>(name);
            return new ProjectionPayloadFieldBuilder<TEntity, TType, TExecutionContext>(payloadedField);
        }
    }
}
