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
    internal class ProjectionArgumentBuilder<TEntity, TArgType, TExecutionContext> :
        ProjectionArgumentBuilderBase<IArgumentedField<TEntity, TArgType, TExecutionContext>, TEntity, TArgType, TExecutionContext>,
        IQueryArgumentBuilder<TArgType, TExecutionContext>,
        IMutationArgumentBuilder<TArgType, TExecutionContext>,
        IRootProjectionArgumentBuilder<TArgType, TExecutionContext>
        where TEntity : class
    {
        internal ProjectionArgumentBuilder(IArgumentedField<TEntity, TArgType, TExecutionContext> argumentedField)
            : base(argumentedField)
        {
        }

        IQueryArgumentBuilder<TArgType, TType, TExecutionContext> IQueryArgumentBuilder<TArgType, TExecutionContext>.Argument<TType>(string name)
        {
            return ArgumentImpl<TType>(name);
        }

        IQueryArgumentBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> IQueryArgumentBuilder<TArgType, TExecutionContext>.FilterArgument<TProjection, TEntity1>(string name)
        {
            return FilterArgumentImpl<TProjection, TEntity1>(name);
        }

        IQueryArgumentBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> IQueryArgumentBuilder<TArgType, TExecutionContext>.FilterArgument<TEntity1>(Type projectionType, string name)
        {
            var methodInfo = typeof(IQueryArgumentBuilder<TArgType, TExecutionContext>).GetPublicGenericMethod(
                nameof(IQueryArgumentBuilder<TArgType, TExecutionContext>.FilterArgument),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IQueryArgumentBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
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

        IMutationArgumentBuilder<TArgType, TType, TExecutionContext> IMutationArgumentBuilder<TArgType, TExecutionContext>.Argument<TType>(string name)
        {
            return ArgumentImpl<TType>(name);
        }

        IMutationArgumentBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> IMutationArgumentBuilder<TArgType, TExecutionContext>.FilterArgument<TProjection, TEntity1>(string name)
        {
            return FilterArgumentImpl<TProjection, TEntity1>(name);
        }

        IMutationArgumentBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> IMutationArgumentBuilder<TArgType, TExecutionContext>.FilterArgument<TEntity1>(Type projectionType, string name)
        {
            var methodInfo = typeof(IMutationArgumentBuilder<TArgType, TExecutionContext>).GetPublicGenericMethod(
                nameof(IMutationArgumentBuilder<TArgType, TExecutionContext>.FilterArgument),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IMutationArgumentBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        protected ProjectionArgumentBuilder<TEntity, TArgType, TExecutionContext> AsUnionOfImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            Field = Field.ApplyUnion(build, false);
            return this;
        }

        protected ProjectionArgumentBuilder<TEntity, TArgType, TExecutionContext> AsUnionOfImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            Field = Field.ApplyUnion(build, true);
            return this;
        }

        protected ProjectionArgumentBuilder<TEntity, TArgType, TExecutionContext> AndImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        protected ProjectionArgumentBuilder<TEntity, TArgType, TExecutionContext> AndImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        private IRootProjectionArgumentBuilder<TArgType, TType, TExecutionContext> ArgumentImpl<TType>(string name)
        {
            var argumentedField = Field.ApplyArgument<TType>(name);
            return new ProjectionArgumentBuilder<TEntity, TArgType, TType, TExecutionContext>(argumentedField);
        }

        private IRootProjectionArgumentBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgumentImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new ProjectionArgumentBuilder<TEntity, TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext>(argumentedField);
        }
    }

    internal class ProjectionArgumentBuilder<TEntity, TArgType1, TArgType2, TExecutionContext> :
        ProjectionArgumentBuilderBase<IArgumentedField<TEntity, TArgType1, TArgType2, TExecutionContext>, TEntity, TArgType1, TArgType2, TExecutionContext>,
        IQueryArgumentBuilder<TArgType1, TArgType2, TExecutionContext>,
        IMutationArgumentBuilder<TArgType1, TArgType2, TExecutionContext>,
        IRootProjectionArgumentBuilder<TArgType1, TArgType2, TExecutionContext>
        where TEntity : class
    {
        internal ProjectionArgumentBuilder(IArgumentedField<TEntity, TArgType1, TArgType2, TExecutionContext> argumentedField)
            : base(argumentedField)
        {
        }

        IQueryArgumentBuilder<TArgType1, TArgType2, TType, TExecutionContext> IQueryArgumentBuilder<TArgType1, TArgType2, TExecutionContext>.Argument<TType>(string name)
        {
            return ArgumentImpl<TType>(name);
        }

        IQueryArgumentBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> IQueryArgumentBuilder<TArgType1, TArgType2, TExecutionContext>.FilterArgument<TProjection, TEntity1>(string name)
        {
            return FilterArgumentImpl<TProjection, TEntity1>(name);
        }

        IQueryArgumentBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> IQueryArgumentBuilder<TArgType1, TArgType2, TExecutionContext>.FilterArgument<TEntity1>(Type projectionType, string name)
        {
            var methodInfo = typeof(IQueryArgumentBuilder<TArgType1, TArgType2, TExecutionContext>).GetPublicGenericMethod(
                nameof(IQueryArgumentBuilder<TArgType1, TArgType2, TExecutionContext>.FilterArgument),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IQueryArgumentBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
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

        IMutationArgumentBuilder<TArgType1, TArgType2, TType, TExecutionContext> IMutationArgumentBuilder<TArgType1, TArgType2, TExecutionContext>.Argument<TType>(string name)
        {
            return ArgumentImpl<TType>(name);
        }

        IMutationArgumentBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> IMutationArgumentBuilder<TArgType1, TArgType2, TExecutionContext>.FilterArgument<TProjection, TEntity1>(string name)
        {
            return FilterArgumentImpl<TProjection, TEntity1>(name);
        }

        IMutationArgumentBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> IMutationArgumentBuilder<TArgType1, TArgType2, TExecutionContext>.FilterArgument<TEntity1>(Type projectionType, string name)
        {
            var methodInfo = typeof(IMutationArgumentBuilder<TArgType1, TArgType2, TExecutionContext>).GetPublicGenericMethod(
                nameof(IMutationArgumentBuilder<TArgType1, TArgType2, TExecutionContext>.FilterArgument),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IMutationArgumentBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        protected ProjectionArgumentBuilder<TEntity, TArgType1, TArgType2, TExecutionContext> AsUnionOfImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            Field = Field.ApplyUnion(build, false);
            return this;
        }

        protected ProjectionArgumentBuilder<TEntity, TArgType1, TArgType2, TExecutionContext> AsUnionOfImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            Field = Field.ApplyUnion(build, true);
            return this;
        }

        protected ProjectionArgumentBuilder<TEntity, TArgType1, TArgType2, TExecutionContext> AndImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        protected ProjectionArgumentBuilder<TEntity, TArgType1, TArgType2, TExecutionContext> AndImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        private IRootProjectionArgumentBuilder<TArgType1, TArgType2, TType, TExecutionContext> ArgumentImpl<TType>(string name)
        {
            var argumentedField = Field.ApplyArgument<TType>(name);
            return new ProjectionArgumentBuilder<TEntity, TArgType1, TArgType2, TType, TExecutionContext>(argumentedField);
        }

        private IRootProjectionArgumentBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgumentImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new ProjectionArgumentBuilder<TEntity, TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext>(argumentedField);
        }
    }

    internal class ProjectionArgumentBuilder<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext> :
        ProjectionArgumentBuilderBase<IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext>, TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext>,
        IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>,
        IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>,
        IRootProjectionArgumentBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>
        where TEntity : class
    {
        internal ProjectionArgumentBuilder(IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext> argumentedField)
            : base(argumentedField)
        {
        }

        IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TType, TExecutionContext> IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>.Argument<TType>(string name)
        {
            return ArgumentImpl<TType>(name);
        }

        IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>.FilterArgument<TProjection, TEntity1>(string name)
        {
            return FilterArgumentImpl<TProjection, TEntity1>(name);
        }

        IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>.FilterArgument<TEntity1>(Type projectionType, string name)
        {
            var methodInfo = typeof(IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>).GetPublicGenericMethod(
                nameof(IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>.FilterArgument),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
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

        IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TType, TExecutionContext> IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>.Argument<TType>(string name)
        {
            return ArgumentImpl<TType>(name);
        }

        IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>.FilterArgument<TProjection, TEntity1>(string name)
        {
            return FilterArgumentImpl<TProjection, TEntity1>(name);
        }

        IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>.FilterArgument<TEntity1>(Type projectionType, string name)
        {
            var methodInfo = typeof(IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>).GetPublicGenericMethod(
                nameof(IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>.FilterArgument),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        protected ProjectionArgumentBuilder<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext> AsUnionOfImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            Field = Field.ApplyUnion(build, false);
            return this;
        }

        protected ProjectionArgumentBuilder<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext> AsUnionOfImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            Field = Field.ApplyUnion(build, true);
            return this;
        }

        protected ProjectionArgumentBuilder<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext> AndImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        protected ProjectionArgumentBuilder<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext> AndImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        private IRootProjectionArgumentBuilder<TArgType1, TArgType2, TArgType3, TType, TExecutionContext> ArgumentImpl<TType>(string name)
        {
            var argumentedField = Field.ApplyArgument<TType>(name);
            return new ProjectionArgumentBuilder<TEntity, TArgType1, TArgType2, TArgType3, TType, TExecutionContext>(argumentedField);
        }

        private IRootProjectionArgumentBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgumentImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new ProjectionArgumentBuilder<TEntity, TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext>(argumentedField);
        }
    }

    internal class ProjectionArgumentBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> :
        ProjectionArgumentBuilderBase<IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>,
        IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>,
        IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>,
        IRootProjectionArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
        where TEntity : class
    {
        internal ProjectionArgumentBuilder(IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> argumentedField)
            : base(argumentedField)
        {
        }

        IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TType, TExecutionContext> IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>.Argument<TType>(string name)
        {
            return ArgumentImpl<TType>(name);
        }

        IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>.FilterArgument<TProjection, TEntity1>(string name)
        {
            return FilterArgumentImpl<TProjection, TEntity1>(name);
        }

        IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>.FilterArgument<TEntity1>(Type projectionType, string name)
        {
            var methodInfo = typeof(IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>).GetPublicGenericMethod(
                nameof(IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>.FilterArgument),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
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

        IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TType, TExecutionContext> IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>.Argument<TType>(string name)
        {
            return ArgumentImpl<TType>(name);
        }

        IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>.FilterArgument<TProjection, TEntity1>(string name)
        {
            return FilterArgumentImpl<TProjection, TEntity1>(name);
        }

        IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>.FilterArgument<TEntity1>(Type projectionType, string name)
        {
            var methodInfo = typeof(IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>).GetPublicGenericMethod(
                nameof(IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>.FilterArgument),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        protected ProjectionArgumentBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> AsUnionOfImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            Field = Field.ApplyUnion(build, false);
            return this;
        }

        protected ProjectionArgumentBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> AsUnionOfImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            Field = Field.ApplyUnion(build, true);
            return this;
        }

        protected ProjectionArgumentBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> AndImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        protected ProjectionArgumentBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> AndImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        private IRootProjectionArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TType, TExecutionContext> ArgumentImpl<TType>(string name)
        {
            var argumentedField = Field.ApplyArgument<TType>(name);
            return new ProjectionArgumentBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TType, TExecutionContext>(argumentedField);
        }

        private IRootProjectionArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgumentImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new ProjectionArgumentBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext>(argumentedField);
        }
    }

    internal class ProjectionArgumentBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> :
        ProjectionArgumentBuilderBase<IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>,
        IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>,
        IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>,
        IRootProjectionArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
        where TEntity : class
    {
        internal ProjectionArgumentBuilder(IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> argumentedField)
            : base(argumentedField)
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

        protected ProjectionArgumentBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> AsUnionOfImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            Field = Field.ApplyUnion(build, false);
            return this;
        }

        protected ProjectionArgumentBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> AsUnionOfImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            Field = Field.ApplyUnion(build, true);
            return this;
        }

        protected ProjectionArgumentBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> AndImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        protected ProjectionArgumentBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> AndImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }
    }
}
