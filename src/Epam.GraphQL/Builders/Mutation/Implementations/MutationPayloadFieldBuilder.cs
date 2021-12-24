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

namespace Epam.GraphQL.Builders.Mutation.Implementations
{
    internal class MutationPayloadFieldBuilder<TEntity, TArgType, TExecutionContext> :
        MutationArgumentBuilderBase<IArgumentedField<TEntity, TArgType, TExecutionContext>, TEntity, TArgType, TExecutionContext>,
        IMutationPayloadFieldBuilder<TArgType, TExecutionContext>
        where TEntity : class
    {
        public MutationPayloadFieldBuilder(IArgumentedField<TEntity, TArgType, TExecutionContext> payloadedField)
            : base(payloadedField)
        {
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType, TExecutionContext>, TArgType, TExecutionContext> AsUnionOf<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType, TExecutionContext>, TArgType, TExecutionContext> AsUnionOf<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType, TExecutionContext>, TArgType, TExecutionContext> And<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AndImpl(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType, TExecutionContext>, TArgType, TExecutionContext> And<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AndImpl<TEnumerable, TElementType>(build);
        }

        public IMutationPayloadFieldBuilder<TArgType, TType, TExecutionContext> PayloadField<TType>(string name)
        {
            return PayloadFieldImpl<TType>(name);
        }

        public IMutationPayloadFieldBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterPayloadFieldImpl<TProjection, TEntity1>(name);
        }

        public IMutationPayloadFieldBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TEntity1>(Type projectionType, string name)
            where TEntity1 : class
        {
            var methodInfo = typeof(IMutationPayloadFieldBuilder<TArgType, TExecutionContext>).GetPublicGenericMethod(
                nameof(IMutationPayloadFieldBuilder<TArgType, TExecutionContext>.FilterPayloadField),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IMutationPayloadFieldBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType, TExecutionContext> AsUnionOfImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            Field = Field.ApplyUnion(build, false);
            return this;
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType, TExecutionContext> AsUnionOfImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            Field = Field.ApplyUnion(build, true);
            return this;
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType, TExecutionContext> AndImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType, TExecutionContext> AndImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType, TType, TExecutionContext> PayloadFieldImpl<TType>(string name)
        {
            var payloadedField = Field.ApplyArgument<TType>(name);
            return new MutationPayloadFieldBuilder<TEntity, TArgType, TType, TExecutionContext>(payloadedField);
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadFieldImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var payloadedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new MutationPayloadFieldBuilder<TEntity, TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext>(payloadedField);
        }
    }

    internal class MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TExecutionContext> :
        MutationArgumentBuilderBase<IArgumentedField<TEntity, TArgType1, TArgType2, TExecutionContext>, TEntity, TArgType1, TArgType2, TExecutionContext>,
        IMutationPayloadFieldBuilder<TArgType1, TArgType2, TExecutionContext>
        where TEntity : class
    {
        public MutationPayloadFieldBuilder(IArgumentedField<TEntity, TArgType1, TArgType2, TExecutionContext> payloadedField)
            : base(payloadedField)
        {
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext> AsUnionOf<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext> AsUnionOf<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext> And<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AndImpl(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext> And<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AndImpl<TEnumerable, TElementType>(build);
        }

        public IMutationPayloadFieldBuilder<TArgType1, TArgType2, TType, TExecutionContext> PayloadField<TType>(string name)
        {
            return PayloadFieldImpl<TType>(name);
        }

        public IMutationPayloadFieldBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterPayloadFieldImpl<TProjection, TEntity1>(name);
        }

        public IMutationPayloadFieldBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TEntity1>(Type projectionType, string name)
            where TEntity1 : class
        {
            var methodInfo = typeof(IMutationPayloadFieldBuilder<TArgType1, TArgType2, TExecutionContext>).GetPublicGenericMethod(
                nameof(IMutationPayloadFieldBuilder<TArgType1, TArgType2, TExecutionContext>.FilterPayloadField),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IMutationPayloadFieldBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TExecutionContext> AsUnionOfImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            Field = Field.ApplyUnion(build, false);
            return this;
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TExecutionContext> AsUnionOfImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            Field = Field.ApplyUnion(build, true);
            return this;
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TExecutionContext> AndImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TExecutionContext> AndImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TType, TExecutionContext> PayloadFieldImpl<TType>(string name)
        {
            var payloadedField = Field.ApplyArgument<TType>(name);
            return new MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TType, TExecutionContext>(payloadedField);
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadFieldImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var payloadedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext>(payloadedField);
        }
    }

    internal class MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext> :
        MutationArgumentBuilderBase<IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext>, TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext>,
        IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>
        where TEntity : class
    {
        public MutationPayloadFieldBuilder(IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext> payloadedField)
            : base(payloadedField)
        {
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext> AsUnionOf<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext> AsUnionOf<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext> And<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AndImpl(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext> And<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AndImpl<TEnumerable, TElementType>(build);
        }

        public IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TType, TExecutionContext> PayloadField<TType>(string name)
        {
            return PayloadFieldImpl<TType>(name);
        }

        public IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterPayloadFieldImpl<TProjection, TEntity1>(name);
        }

        public IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TEntity1>(Type projectionType, string name)
            where TEntity1 : class
        {
            var methodInfo = typeof(IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>).GetPublicGenericMethod(
                nameof(IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>.FilterPayloadField),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext> AsUnionOfImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            Field = Field.ApplyUnion(build, false);
            return this;
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext> AsUnionOfImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            Field = Field.ApplyUnion(build, true);
            return this;
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext> AndImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext> AndImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TType, TExecutionContext> PayloadFieldImpl<TType>(string name)
        {
            var payloadedField = Field.ApplyArgument<TType>(name);
            return new MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TType, TExecutionContext>(payloadedField);
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadFieldImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var payloadedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext>(payloadedField);
        }
    }

    internal class MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> :
        MutationArgumentBuilderBase<IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>,
        IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
        where TEntity : class
    {
        public MutationPayloadFieldBuilder(IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> payloadedField)
            : base(payloadedField)
        {
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> AsUnionOf<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> AsUnionOf<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> And<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AndImpl(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> And<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AndImpl<TEnumerable, TElementType>(build);
        }

        public IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TType, TExecutionContext> PayloadField<TType>(string name)
        {
            return PayloadFieldImpl<TType>(name);
        }

        public IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterPayloadFieldImpl<TProjection, TEntity1>(name);
        }

        public IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TEntity1>(Type projectionType, string name)
            where TEntity1 : class
        {
            var methodInfo = typeof(IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>).GetPublicGenericMethod(
                nameof(IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>.FilterPayloadField),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> AsUnionOfImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            Field = Field.ApplyUnion(build, false);
            return this;
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> AsUnionOfImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            Field = Field.ApplyUnion(build, true);
            return this;
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> AndImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> AndImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TType, TExecutionContext> PayloadFieldImpl<TType>(string name)
        {
            var payloadedField = Field.ApplyArgument<TType>(name);
            return new MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TType, TExecutionContext>(payloadedField);
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadFieldImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var payloadedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext>(payloadedField);
        }
    }

    internal class MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> :
        MutationArgumentBuilderBase<IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>,
        IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
        where TEntity : class
    {
        public MutationPayloadFieldBuilder(IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> payloadedField)
            : base(payloadedField)
        {
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> AsUnionOf<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> AsUnionOf<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> And<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AndImpl(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> And<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AndImpl<TEnumerable, TElementType>(build);
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> AsUnionOfImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            Field = Field.ApplyUnion(build, false);
            return this;
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> AsUnionOfImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            Field = Field.ApplyUnion(build, true);
            return this;
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> AndImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        private MutationPayloadFieldBuilder<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> AndImpl<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOfImpl<TEnumerable, TElementType>(build);
        }
    }
}
