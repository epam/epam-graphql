// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Builders.Loader.Implementations;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Configuration.Implementations.Fields;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Builders.Query.Implementations
{
    internal class QueryFieldBuilder<TField, TExecutionContext> : QueryFieldBuilderBase<TField, TExecutionContext>,
        IQueryFieldBuilder<TExecutionContext>
        where TField : FieldBase<object, TExecutionContext>, IQueryField<TExecutionContext>
    {
        public QueryFieldBuilder(TField field)
            : base(field)
        {
        }

        public IQueryArgumentBuilder<TType, TExecutionContext> Argument<TType>(string name)
        {
            return ArgumentImpl<TType>(name);
        }

        public IQueryArgumentBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterArgumentImpl<TProjection, TEntity1>(name);
        }

        public IQueryArgumentBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TEntity1>(Type projectionType, string name)
            where TEntity1 : class
        {
            var methodInfo = typeof(IQueryFieldBuilder<TExecutionContext>).GetPublicGenericMethod(
                nameof(IQueryFieldBuilder<TExecutionContext>.FilterArgument),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IQueryArgumentBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        public IQueryPayloadFieldBuilder<TType, TExecutionContext> PayloadField<TType>(string name)
        {
            return PayloadFieldImpl<TType>(name);
        }

        public IQueryPayloadFieldBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterPayloadFieldImpl<TProjection, TEntity1>(name);
        }

        public IQueryPayloadFieldBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TEntity1>(Type projectionType, string name)
            where TEntity1 : class
        {
            var methodInfo = typeof(IQueryFieldBuilder<TExecutionContext>).GetPublicGenericMethod(
                nameof(IQueryFieldBuilder<TExecutionContext>.FilterPayloadField),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IQueryPayloadFieldBuilder<Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        public IFromIQueryableBuilder<TReturnType, TExecutionContext> FromIQueryable<TReturnType>(
            Func<TExecutionContext, IQueryable<TReturnType>> query,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? configure)
            where TReturnType : class
        {
            return new FromIQueryableBuilder<object, TReturnType, TExecutionContext>(Field.FromIQueryable(query, configure));
        }

        private QueryArgumentBuilder<object, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgumentImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new QueryArgumentBuilder<object, Expression<Func<TEntity1, bool>>, TExecutionContext>(argumentedField);
        }

        private QueryArgumentBuilder<object, TType, TExecutionContext> ArgumentImpl<TType>(string name)
        {
            var argumentedField = Field.ApplyArgument<TType>(name);
            return new QueryArgumentBuilder<object, TType, TExecutionContext>(argumentedField);
        }

        private QueryPayloadFieldBuilder<object, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadFieldImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var payloadedField = Field.ApplyFilterPayloadField<TProjection, TEntity1>(name);
            return new QueryPayloadFieldBuilder<object, Expression<Func<TEntity1, bool>>, TExecutionContext>(payloadedField);
        }

        private QueryPayloadFieldBuilder<object, TType, TExecutionContext> PayloadFieldImpl<TType>(string name)
        {
            var payloadedField = Field.ApplyPayloadField<TType>(name);
            return new QueryPayloadFieldBuilder<object, TType, TExecutionContext>(payloadedField);
        }
    }
}
