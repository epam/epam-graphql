// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Builders.Projection.Implementations
{
    internal class ProjectionPayloadFieldBuilder<TEntity, TArgType, TExecutionContext> :
        ProjectionArgumentBuilderBase<IArgumentedField<TEntity, TArgType, TExecutionContext>, TEntity, TArgType, TExecutionContext>,
        IQueryPayloadFieldBuilder<TArgType, TExecutionContext>,
        IMutationPayloadFieldBuilder<TArgType, TExecutionContext>,
        IRootProjectionPayloadFieldBuilder<TArgType, TExecutionContext>
        where TEntity : class
    {
        internal ProjectionPayloadFieldBuilder(IArgumentedField<TEntity, TArgType, TExecutionContext> payloadedField)
            : base(payloadedField)
        {
        }

        IQueryPayloadFieldBuilder<TArgType, TType, TExecutionContext> IQueryPayloadFieldBuilder<TArgType, TExecutionContext>.PayloadField<TType>(string name)
        {
            return PayloadFieldImpl<TType>(name);
        }

        IQueryPayloadFieldBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> IQueryPayloadFieldBuilder<TArgType, TExecutionContext>.FilterPayloadField<TProjection, TEntity1>(string name)
        {
            return FilterPayloadFieldImpl<TProjection, TEntity1>(name);
        }

        IQueryPayloadFieldBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> IQueryPayloadFieldBuilder<TArgType, TExecutionContext>.FilterPayloadField<TEntity1>(Type projectionType, string name)
        {
            var methodInfo = typeof(IQueryPayloadFieldBuilder<TArgType, TExecutionContext>).GetPublicGenericMethod(
                nameof(IQueryPayloadFieldBuilder<TArgType, TExecutionContext>.FilterPayloadField),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IQueryPayloadFieldBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType, TExecutionContext>, TArgType, TExecutionContext> IUnionableProjectionFieldBuilder<IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType, TExecutionContext>, TArgType, TExecutionContext>, TExecutionContext>.AsUnionOf<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
        {
            return AsUnionOfImpl(build);
        }

        IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType, TExecutionContext>, TArgType, TExecutionContext> IUnionableProjectionFieldBuilder<IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType, TExecutionContext>, TArgType, TExecutionContext>, TExecutionContext>.AsUnionOf<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType, TExecutionContext>, TArgType, TExecutionContext> IUnionableProjectionFieldBuilder<IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType, TExecutionContext>, TArgType, TExecutionContext>, TExecutionContext>.And<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
        {
            return AndImpl(build);
        }

        IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType, TExecutionContext>, TArgType, TExecutionContext> IUnionableProjectionFieldBuilder<IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType, TExecutionContext>, TArgType, TExecutionContext>, TExecutionContext>.And<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
        {
            return AndImpl<TEnumerable, TElementType>(build);
        }

        IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType, TExecutionContext>, TArgType, TExecutionContext> IUnionableProjectionFieldBuilder<IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType, TExecutionContext>, TArgType, TExecutionContext>, TExecutionContext>.AsUnionOf<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
        {
            return AsUnionOfImpl(build);
        }

        IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType, TExecutionContext>, TArgType, TExecutionContext> IUnionableProjectionFieldBuilder<IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType, TExecutionContext>, TArgType, TExecutionContext>, TExecutionContext>.AsUnionOf<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType, TExecutionContext>, TArgType, TExecutionContext> IUnionableProjectionFieldBuilder<IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType, TExecutionContext>, TArgType, TExecutionContext>, TExecutionContext>.And<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
        {
            return AndImpl(build);
        }

        IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType, TExecutionContext>, TArgType, TExecutionContext> IUnionableProjectionFieldBuilder<IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType, TExecutionContext>, TArgType, TExecutionContext>, TExecutionContext>.And<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
        {
            return AndImpl<TEnumerable, TElementType>(build);
        }

        IMutationPayloadFieldBuilder<TArgType, TType, TExecutionContext> IMutationPayloadFieldBuilder<TArgType, TExecutionContext>.PayloadField<TType>(string name)
        {
            return PayloadFieldImpl<TType>(name);
        }

        IMutationPayloadFieldBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> IMutationPayloadFieldBuilder<TArgType, TExecutionContext>.FilterPayloadField<TProjection, TEntity1>(string name)
        {
            return FilterPayloadFieldImpl<TProjection, TEntity1>(name);
        }

        IMutationPayloadFieldBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> IMutationPayloadFieldBuilder<TArgType, TExecutionContext>.FilterPayloadField<TEntity1>(Type projectionType, string name)
        {
            var methodInfo = typeof(IMutationPayloadFieldBuilder<TArgType, TExecutionContext>).GetPublicGenericMethod(
                nameof(IMutationPayloadFieldBuilder<TArgType, TExecutionContext>.FilterPayloadField),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IMutationPayloadFieldBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        protected ProjectionPayloadFieldBuilder<TEntity, TArgType, TExecutionContext> AsUnionOfImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            Field = Field.ApplyUnion(build, false);
            return this;
        }

        protected ProjectionPayloadFieldBuilder<TEntity, TArgType, TExecutionContext> AsUnionOfImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            Field = Field.ApplyUnion(build, true);
            return this;
        }

        protected ProjectionPayloadFieldBuilder<TEntity, TArgType, TExecutionContext> AndImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        protected ProjectionPayloadFieldBuilder<TEntity, TArgType, TExecutionContext> AndImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        private IRootProjectionPayloadFieldBuilder<TArgType, TType, TExecutionContext> PayloadFieldImpl<TType>(string name)
        {
            var payloadedField = Field.ApplyArgument<TType>(name);
            return new ProjectionPayloadFieldBuilder<TEntity, TArgType, TType, TExecutionContext>(payloadedField);
        }

        private IRootProjectionPayloadFieldBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadFieldImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var payloadedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new ProjectionPayloadFieldBuilder<TEntity, TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext>(payloadedField);
        }
    }

    internal class ProjectionPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TExecutionContext> :
        ProjectionArgumentBuilderBase<IArgumentedField<TEntity, TArgType1, TArgType2, TExecutionContext>, TEntity, TArgType1, TArgType2, TExecutionContext>,
        IQueryPayloadFieldBuilder<TArgType1, TArgType2, TExecutionContext>,
        IMutationPayloadFieldBuilder<TArgType1, TArgType2, TExecutionContext>,
        IRootProjectionPayloadFieldBuilder<TArgType1, TArgType2, TExecutionContext>
        where TEntity : class
    {
        internal ProjectionPayloadFieldBuilder(IArgumentedField<TEntity, TArgType1, TArgType2, TExecutionContext> payloadedField)
            : base(payloadedField)
        {
        }

        IQueryPayloadFieldBuilder<TArgType1, TArgType2, TType, TExecutionContext> IQueryPayloadFieldBuilder<TArgType1, TArgType2, TExecutionContext>.PayloadField<TType>(string name)
        {
            return PayloadFieldImpl<TType>(name);
        }

        IQueryPayloadFieldBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> IQueryPayloadFieldBuilder<TArgType1, TArgType2, TExecutionContext>.FilterPayloadField<TProjection, TEntity1>(string name)
        {
            return FilterPayloadFieldImpl<TProjection, TEntity1>(name);
        }

        IQueryPayloadFieldBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> IQueryPayloadFieldBuilder<TArgType1, TArgType2, TExecutionContext>.FilterPayloadField<TEntity1>(Type projectionType, string name)
        {
            var methodInfo = typeof(IQueryPayloadFieldBuilder<TArgType1, TArgType2, TExecutionContext>).GetPublicGenericMethod(
                nameof(IQueryPayloadFieldBuilder<TArgType1, TArgType2, TExecutionContext>.FilterPayloadField),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IQueryPayloadFieldBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext> IUnionableProjectionFieldBuilder<IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext>, TExecutionContext>.AsUnionOf<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
        {
            return AsUnionOfImpl(build);
        }

        IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext> IUnionableProjectionFieldBuilder<IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext>, TExecutionContext>.AsUnionOf<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext> IUnionableProjectionFieldBuilder<IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext>, TExecutionContext>.And<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
        {
            return AndImpl(build);
        }

        IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext> IUnionableProjectionFieldBuilder<IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext>, TExecutionContext>.And<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
        {
            return AndImpl<TEnumerable, TElementType>(build);
        }

        IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext> IUnionableProjectionFieldBuilder<IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext>, TExecutionContext>.AsUnionOf<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
        {
            return AsUnionOfImpl(build);
        }

        IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext> IUnionableProjectionFieldBuilder<IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext>, TExecutionContext>.AsUnionOf<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext> IUnionableProjectionFieldBuilder<IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext>, TExecutionContext>.And<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
        {
            return AndImpl(build);
        }

        IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext> IUnionableProjectionFieldBuilder<IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext>, TExecutionContext>.And<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
        {
            return AndImpl<TEnumerable, TElementType>(build);
        }

        IMutationPayloadFieldBuilder<TArgType1, TArgType2, TType, TExecutionContext> IMutationPayloadFieldBuilder<TArgType1, TArgType2, TExecutionContext>.PayloadField<TType>(string name)
        {
            return PayloadFieldImpl<TType>(name);
        }

        IMutationPayloadFieldBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> IMutationPayloadFieldBuilder<TArgType1, TArgType2, TExecutionContext>.FilterPayloadField<TProjection, TEntity1>(string name)
        {
            return FilterPayloadFieldImpl<TProjection, TEntity1>(name);
        }

        IMutationPayloadFieldBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> IMutationPayloadFieldBuilder<TArgType1, TArgType2, TExecutionContext>.FilterPayloadField<TEntity1>(Type projectionType, string name)
        {
            var methodInfo = typeof(IMutationPayloadFieldBuilder<TArgType1, TArgType2, TExecutionContext>).GetPublicGenericMethod(
                nameof(IMutationPayloadFieldBuilder<TArgType1, TArgType2, TExecutionContext>.FilterPayloadField),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IMutationPayloadFieldBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        protected ProjectionPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TExecutionContext> AsUnionOfImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            Field = Field.ApplyUnion(build, false);
            return this;
        }

        protected ProjectionPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TExecutionContext> AsUnionOfImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            Field = Field.ApplyUnion(build, true);
            return this;
        }

        protected ProjectionPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TExecutionContext> AndImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        protected ProjectionPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TExecutionContext> AndImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        private IRootProjectionPayloadFieldBuilder<TArgType1, TArgType2, TType, TExecutionContext> PayloadFieldImpl<TType>(string name)
        {
            var payloadedField = Field.ApplyArgument<TType>(name);
            return new ProjectionPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TType, TExecutionContext>(payloadedField);
        }

        private IRootProjectionPayloadFieldBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadFieldImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var payloadedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new ProjectionPayloadFieldBuilder<TEntity, TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext>(payloadedField);
        }
    }

    internal class ProjectionPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext> :
        ProjectionArgumentBuilderBase<IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext>, TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext>,
        IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>,
        IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>,
        IRootProjectionPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>
        where TEntity : class
    {
        internal ProjectionPayloadFieldBuilder(IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext> payloadedField)
            : base(payloadedField)
        {
        }

        IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TType, TExecutionContext> IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>.PayloadField<TType>(string name)
        {
            return PayloadFieldImpl<TType>(name);
        }

        IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>.FilterPayloadField<TProjection, TEntity1>(string name)
        {
            return FilterPayloadFieldImpl<TProjection, TEntity1>(name);
        }

        IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>.FilterPayloadField<TEntity1>(Type projectionType, string name)
        {
            var methodInfo = typeof(IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>).GetPublicGenericMethod(
                nameof(IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>.FilterPayloadField),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext> IUnionableProjectionFieldBuilder<IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext>, TExecutionContext>.AsUnionOf<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
        {
            return AsUnionOfImpl(build);
        }

        IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext> IUnionableProjectionFieldBuilder<IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext>, TExecutionContext>.AsUnionOf<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext> IUnionableProjectionFieldBuilder<IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext>, TExecutionContext>.And<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
        {
            return AndImpl(build);
        }

        IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext> IUnionableProjectionFieldBuilder<IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext>, TExecutionContext>.And<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
        {
            return AndImpl<TEnumerable, TElementType>(build);
        }

        IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext> IUnionableProjectionFieldBuilder<IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext>, TExecutionContext>.AsUnionOf<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
        {
            return AsUnionOfImpl(build);
        }

        IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext> IUnionableProjectionFieldBuilder<IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext>, TExecutionContext>.AsUnionOf<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext> IUnionableProjectionFieldBuilder<IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext>, TExecutionContext>.And<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
        {
            return AndImpl(build);
        }

        IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext> IUnionableProjectionFieldBuilder<IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext>, TExecutionContext>.And<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
        {
            return AndImpl<TEnumerable, TElementType>(build);
        }

        IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TType, TExecutionContext> IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>.PayloadField<TType>(string name)
        {
            return PayloadFieldImpl<TType>(name);
        }

        IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>.FilterPayloadField<TProjection, TEntity1>(string name)
        {
            return FilterPayloadFieldImpl<TProjection, TEntity1>(name);
        }

        IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>.FilterPayloadField<TEntity1>(Type projectionType, string name)
        {
            var methodInfo = typeof(IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>).GetPublicGenericMethod(
                nameof(IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>.FilterPayloadField),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        protected ProjectionPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext> AsUnionOfImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            Field = Field.ApplyUnion(build, false);
            return this;
        }

        protected ProjectionPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext> AsUnionOfImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            Field = Field.ApplyUnion(build, true);
            return this;
        }

        protected ProjectionPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext> AndImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        protected ProjectionPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext> AndImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        private IRootProjectionPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TType, TExecutionContext> PayloadFieldImpl<TType>(string name)
        {
            var payloadedField = Field.ApplyArgument<TType>(name);
            return new ProjectionPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TType, TExecutionContext>(payloadedField);
        }

        private IRootProjectionPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadFieldImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var payloadedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new ProjectionPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext>(payloadedField);
        }
    }

    internal class ProjectionPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> :
        ProjectionArgumentBuilderBase<IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>,
        IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>,
        IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>,
        IRootProjectionPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
        where TEntity : class
    {
        internal ProjectionPayloadFieldBuilder(IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> payloadedField)
            : base(payloadedField)
        {
        }

        IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TType, TExecutionContext> IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>.PayloadField<TType>(string name)
        {
            return PayloadFieldImpl<TType>(name);
        }

        IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>.FilterPayloadField<TProjection, TEntity1>(string name)
        {
            return FilterPayloadFieldImpl<TProjection, TEntity1>(name);
        }

        IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>.FilterPayloadField<TEntity1>(Type projectionType, string name)
        {
            var methodInfo = typeof(IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>).GetPublicGenericMethod(
                nameof(IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>.FilterPayloadField),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> IUnionableProjectionFieldBuilder<IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TExecutionContext>.AsUnionOf<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
        {
            return AsUnionOfImpl(build);
        }

        IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> IUnionableProjectionFieldBuilder<IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TExecutionContext>.AsUnionOf<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> IUnionableProjectionFieldBuilder<IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TExecutionContext>.And<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
        {
            return AndImpl(build);
        }

        IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> IUnionableProjectionFieldBuilder<IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TExecutionContext>.And<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
        {
            return AndImpl<TEnumerable, TElementType>(build);
        }

        IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> IUnionableProjectionFieldBuilder<IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TExecutionContext>.AsUnionOf<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
        {
            return AsUnionOfImpl(build);
        }

        IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> IUnionableProjectionFieldBuilder<IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TExecutionContext>.AsUnionOf<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> IUnionableProjectionFieldBuilder<IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TExecutionContext>.And<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
        {
            return AndImpl(build);
        }

        IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> IUnionableProjectionFieldBuilder<IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TExecutionContext>.And<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
        {
            return AndImpl<TEnumerable, TElementType>(build);
        }

        IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TType, TExecutionContext> IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>.PayloadField<TType>(string name)
        {
            return PayloadFieldImpl<TType>(name);
        }

        IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>.FilterPayloadField<TProjection, TEntity1>(string name)
        {
            return FilterPayloadFieldImpl<TProjection, TEntity1>(name);
        }

        IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>.FilterPayloadField<TEntity1>(Type projectionType, string name)
        {
            var methodInfo = typeof(IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>).GetPublicGenericMethod(
                nameof(IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>.FilterPayloadField),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        protected ProjectionPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> AsUnionOfImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            Field = Field.ApplyUnion(build, false);
            return this;
        }

        protected ProjectionPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> AsUnionOfImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            Field = Field.ApplyUnion(build, true);
            return this;
        }

        protected ProjectionPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> AndImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        protected ProjectionPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> AndImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        private IRootProjectionPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TType, TExecutionContext> PayloadFieldImpl<TType>(string name)
        {
            var payloadedField = Field.ApplyArgument<TType>(name);
            return new ProjectionPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TType, TExecutionContext>(payloadedField);
        }

        private IRootProjectionPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadFieldImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var payloadedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new ProjectionPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext>(payloadedField);
        }
    }

    internal class ProjectionPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> :
        ProjectionArgumentBuilderBase<IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>,
        IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>,
        IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>,
        IRootProjectionPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
        where TEntity : class
    {
        internal ProjectionPayloadFieldBuilder(IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> payloadedField)
            : base(payloadedField)
        {
        }

        IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> IUnionableProjectionFieldBuilder<IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TExecutionContext>.AsUnionOf<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
        {
            return AsUnionOfImpl(build);
        }

        IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> IUnionableProjectionFieldBuilder<IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TExecutionContext>.AsUnionOf<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> IUnionableProjectionFieldBuilder<IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TExecutionContext>.And<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
        {
            return AndImpl(build);
        }

        IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> IUnionableProjectionFieldBuilder<IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TExecutionContext>.And<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
        {
            return AndImpl<TEnumerable, TElementType>(build);
        }

        IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> IUnionableProjectionFieldBuilder<IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TExecutionContext>.AsUnionOf<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
        {
            return AsUnionOfImpl(build);
        }

        IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> IUnionableProjectionFieldBuilder<IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TExecutionContext>.AsUnionOf<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> IUnionableProjectionFieldBuilder<IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TExecutionContext>.And<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
        {
            return AndImpl(build);
        }

        IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> IUnionableProjectionFieldBuilder<IQueryFieldBuilder<IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TExecutionContext>.And<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
        {
            return AndImpl<TEnumerable, TElementType>(build);
        }

        protected ProjectionPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> AsUnionOfImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            Field = Field.ApplyUnion(build, false);
            return this;
        }

        protected ProjectionPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> AsUnionOfImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            Field = Field.ApplyUnion(build, true);
            return this;
        }

        protected ProjectionPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> AndImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        protected ProjectionPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> AndImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }
    }
}
