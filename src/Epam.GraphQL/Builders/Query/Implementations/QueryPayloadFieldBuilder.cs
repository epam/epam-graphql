// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Builders.Query.Implementations
{
    internal class QueryPayloadFieldBuilder<TArgType, TExecutionContext> :
        QueryArgumentBuilderBase<IArgumentedField<object, TArgType, TExecutionContext>, TArgType, TExecutionContext>,
        IQueryPayloadFieldBuilder<TArgType, TExecutionContext>
    {
        public QueryPayloadFieldBuilder(IArgumentedField<object, TArgType, TExecutionContext> payloadedField)
            : base(payloadedField)
        {
        }

        public IQueryPayloadFieldBuilder<TArgType, TType, TExecutionContext> PayloadField<TType>(string name)
        {
            return PayloadFieldImpl<TType>(name);
        }

        public IQueryPayloadFieldBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterPayloadFieldImpl<TProjection, TEntity1>(name);
        }

        public IQueryPayloadFieldBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TEntity1>(Type projectionType, string name)
            where TEntity1 : class
        {
            var methodInfo = typeof(IQueryPayloadFieldBuilder<TArgType, TExecutionContext>).GetPublicGenericMethod(
                nameof(IQueryPayloadFieldBuilder<TArgType, TExecutionContext>.FilterPayloadField),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IQueryPayloadFieldBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        private QueryPayloadFieldBuilder<TArgType, TType, TExecutionContext> PayloadFieldImpl<TType>(string name)
        {
            var payloadedField = Field.ApplyArgument<TType>(name);
            return new QueryPayloadFieldBuilder<TArgType, TType, TExecutionContext>(payloadedField);
        }

        private QueryPayloadFieldBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadFieldImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var payloadedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new QueryPayloadFieldBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext>(payloadedField);
        }
    }

    internal class QueryPayloadFieldBuilder<TArgType1, TArgType2, TExecutionContext> :
        QueryArgumentBuilderBase<IArgumentedField<object, TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext>,
        IQueryPayloadFieldBuilder<TArgType1, TArgType2, TExecutionContext>
    {
        public QueryPayloadFieldBuilder(IArgumentedField<object, TArgType1, TArgType2, TExecutionContext> payloadedField)
            : base(payloadedField)
        {
        }

        public IQueryPayloadFieldBuilder<TArgType1, TArgType2, TType, TExecutionContext> PayloadField<TType>(string name)
        {
            return PayloadFieldImpl<TType>(name);
        }

        public IQueryPayloadFieldBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterPayloadFieldImpl<TProjection, TEntity1>(name);
        }

        public IQueryPayloadFieldBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TEntity1>(Type projectionType, string name)
            where TEntity1 : class
        {
            var methodInfo = typeof(IQueryPayloadFieldBuilder<TArgType1, TArgType2, TExecutionContext>).GetPublicGenericMethod(
                nameof(IQueryPayloadFieldBuilder<TArgType1, TArgType2, TExecutionContext>.FilterPayloadField),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IQueryPayloadFieldBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        private QueryPayloadFieldBuilder<TArgType1, TArgType2, TType, TExecutionContext> PayloadFieldImpl<TType>(string name)
        {
            var payloadedField = Field.ApplyArgument<TType>(name);
            return new QueryPayloadFieldBuilder<TArgType1, TArgType2, TType, TExecutionContext>(payloadedField);
        }

        private QueryPayloadFieldBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadFieldImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var payloadedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new QueryPayloadFieldBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext>(payloadedField);
        }
    }

    internal class QueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext> :
        QueryArgumentBuilderBase<IArgumentedField<object, TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext>,
        IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>
    {
        public QueryPayloadFieldBuilder(IArgumentedField<object, TArgType1, TArgType2, TArgType3, TExecutionContext> payloadedField)
            : base(payloadedField)
        {
        }

        public IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TType, TExecutionContext> PayloadField<TType>(string name)
        {
            return PayloadFieldImpl<TType>(name);
        }

        public IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterPayloadFieldImpl<TProjection, TEntity1>(name);
        }

        public IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TEntity1>(Type projectionType, string name)
            where TEntity1 : class
        {
            var methodInfo = typeof(IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>).GetPublicGenericMethod(
                nameof(IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>.FilterPayloadField),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        private QueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TType, TExecutionContext> PayloadFieldImpl<TType>(string name)
        {
            var payloadedField = Field.ApplyArgument<TType>(name);
            return new QueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TType, TExecutionContext>(payloadedField);
        }

        private QueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadFieldImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var payloadedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new QueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext>(payloadedField);
        }
    }

    internal class QueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> :
        QueryArgumentBuilderBase<IArgumentedField<object, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>,
        IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
    {
        public QueryPayloadFieldBuilder(IArgumentedField<object, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> payloadedField)
            : base(payloadedField)
        {
        }

        public IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TType, TExecutionContext> PayloadField<TType>(string name)
        {
            return PayloadFieldImpl<TType>(name);
        }

        public IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterPayloadFieldImpl<TProjection, TEntity1>(name);
        }

        public IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TEntity1>(Type projectionType, string name)
            where TEntity1 : class
        {
            var methodInfo = typeof(IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>).GetPublicGenericMethod(
                nameof(IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>.FilterPayloadField),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        private QueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TType, TExecutionContext> PayloadFieldImpl<TType>(string name)
        {
            var payloadedField = Field.ApplyArgument<TType>(name);
            return new QueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TType, TExecutionContext>(payloadedField);
        }

        private QueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadFieldImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var payloadedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new QueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext>(payloadedField);
        }
    }

    internal class QueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> :
        QueryArgumentBuilderBase<IArgumentedField<object, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>,
        IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
    {
        public QueryPayloadFieldBuilder(IArgumentedField<object, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> payloadedField)
            : base(payloadedField)
        {
        }
    }
}
