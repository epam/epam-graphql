// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Builders.Mutation.Implementations
{
    internal class MutationFieldBuilder<TField, TExecutionContext> : MutationFieldBuilderBase<TField, TExecutionContext>,
        IMutationFieldBuilder<TExecutionContext>
        where TField : IArgumentedField<object, TExecutionContext>
    {
        public MutationFieldBuilder(TField field)
            : base(field)
        {
        }

        public IMutationArgumentBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterArgumentImpl<TProjection, TEntity1>(name);
        }

        public IMutationArgumentBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TEntity1>(Type projectionType, string name)
            where TEntity1 : class
        {
            var methodInfo = typeof(IMutationFieldBuilder<TExecutionContext>).GetPublicGenericMethod(
                nameof(IMutationFieldBuilder<TExecutionContext>.FilterArgument),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IMutationArgumentBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        public IMutationArgumentBuilder<TType, TExecutionContext> Argument<TType>(string name)
        {
            return ArgumentImpl<TType>(name);
        }

        public IMutationPayloadFieldBuilder<TType, TExecutionContext> PayloadField<TType>(string name)
        {
            return PayloadFieldImpl<TType>(name);
        }

        public IMutationPayloadFieldBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterPayloadFieldImpl<TProjection, TEntity1>(name);
        }

        public IMutationPayloadFieldBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TEntity1>(Type projectionType, string name)
            where TEntity1 : class
        {
            var methodInfo = typeof(IMutationFieldBuilder<TExecutionContext>).GetPublicGenericMethod(
                nameof(IMutationFieldBuilder<TExecutionContext>.FilterPayloadField),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IMutationPayloadFieldBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        private MutationArgumentBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgumentImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = Field.FilterArgument<TProjection, TEntity1>(name);
            return new MutationArgumentBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext>(argumentedField);
        }

        private MutationArgumentBuilder<TType, TExecutionContext> ArgumentImpl<TType>(string name)
        {
            var argumentedField = Field.Argument<TType>(name);
            return new MutationArgumentBuilder<TType, TExecutionContext>(argumentedField);
        }

        private MutationPayloadFieldBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadFieldImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var payloadedField = Field.FilterPayloadField<TProjection, TEntity1>(name);
            return new MutationPayloadFieldBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext>(payloadedField);
        }

        private MutationPayloadFieldBuilder<TType, TExecutionContext> PayloadFieldImpl<TType>(string name)
        {
            var payloadedField = Field.PayloadField<TType>(name);
            return new MutationPayloadFieldBuilder<TType, TExecutionContext>(payloadedField);
        }
    }
}
