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
    internal class QueryArgumentBuilder<TArgType, TExecutionContext> :
        QueryArgumentBuilderBase<IArgumentedField<object, TArgType, TExecutionContext>, TArgType, TExecutionContext>,
        IQueryArgumentBuilder<TArgType, TExecutionContext>
    {
        public QueryArgumentBuilder(IArgumentedField<object, TArgType, TExecutionContext> argumentedField)
            : base(argumentedField)
        {
        }

        public IQueryArgumentBuilder<TArgType, TType, TExecutionContext> Argument<TType>(string name)
        {
            return ArgumentImpl<TType>(name);
        }

        public IQueryArgumentBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterArgumentImpl<TProjection, TEntity1>(name);
        }

        public IQueryArgumentBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TEntity1>(Type projectionType, string name)
            where TEntity1 : class
        {
            var methodInfo = typeof(IQueryArgumentBuilder<TArgType, TExecutionContext>).GetPublicGenericMethod(
                nameof(IQueryArgumentBuilder<TArgType, TExecutionContext>.FilterArgument),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IQueryArgumentBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        private QueryArgumentBuilder<TArgType, TType, TExecutionContext> ArgumentImpl<TType>(string name)
        {
            var argumentedField = Field.ApplyArgument<TType>(name);
            return new QueryArgumentBuilder<TArgType, TType, TExecutionContext>(argumentedField);
        }

        private QueryArgumentBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgumentImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new QueryArgumentBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext>(argumentedField);
        }
    }

    internal class QueryArgumentBuilder<TArgType1, TArgType2, TExecutionContext> :
        QueryArgumentBuilderBase<IArgumentedField<object, TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext>,
        IQueryArgumentBuilder<TArgType1, TArgType2, TExecutionContext>
    {
        public QueryArgumentBuilder(IArgumentedField<object, TArgType1, TArgType2, TExecutionContext> argumentedField)
            : base(argumentedField)
        {
        }

        public IQueryArgumentBuilder<TArgType1, TArgType2, TType, TExecutionContext> Argument<TType>(string name)
        {
            return ArgumentImpl<TType>(name);
        }

        public IQueryArgumentBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterArgumentImpl<TProjection, TEntity1>(name);
        }

        public IQueryArgumentBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TEntity1>(Type projectionType, string name)
            where TEntity1 : class
        {
            var methodInfo = typeof(IQueryArgumentBuilder<TArgType1, TArgType2, TExecutionContext>).GetPublicGenericMethod(
                nameof(IQueryArgumentBuilder<TArgType1, TArgType2, TExecutionContext>.FilterArgument),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IQueryArgumentBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        private QueryArgumentBuilder<TArgType1, TArgType2, TType, TExecutionContext> ArgumentImpl<TType>(string name)
        {
            var argumentedField = Field.ApplyArgument<TType>(name);
            return new QueryArgumentBuilder<TArgType1, TArgType2, TType, TExecutionContext>(argumentedField);
        }

        private QueryArgumentBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgumentImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new QueryArgumentBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext>(argumentedField);
        }
    }

    internal class QueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext> :
        QueryArgumentBuilderBase<IArgumentedField<object, TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext>,
        IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>
    {
        public QueryArgumentBuilder(IArgumentedField<object, TArgType1, TArgType2, TArgType3, TExecutionContext> argumentedField)
            : base(argumentedField)
        {
        }

        public IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TType, TExecutionContext> Argument<TType>(string name)
        {
            return ArgumentImpl<TType>(name);
        }

        public IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterArgumentImpl<TProjection, TEntity1>(name);
        }

        public IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TEntity1>(Type projectionType, string name)
            where TEntity1 : class
        {
            var methodInfo = typeof(IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>).GetPublicGenericMethod(
                nameof(IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>.FilterArgument),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        private QueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TType, TExecutionContext> ArgumentImpl<TType>(string name)
        {
            var argumentedField = Field.ApplyArgument<TType>(name);
            return new QueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TType, TExecutionContext>(argumentedField);
        }

        private QueryArgumentBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgumentImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new QueryArgumentBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext>(argumentedField);
        }
    }

    internal class QueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> :
        QueryArgumentBuilderBase<IArgumentedField<object, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>,
        IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
    {
        public QueryArgumentBuilder(IArgumentedField<object, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> argumentedField)
            : base(argumentedField)
        {
        }

        public IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TType, TExecutionContext> Argument<TType>(string name)
        {
            return ArgumentImpl<TType>(name);
        }

        public IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterArgumentImpl<TProjection, TEntity1>(name);
        }

        public IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TEntity1>(Type projectionType, string name)
            where TEntity1 : class
        {
            var methodInfo = typeof(IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>).GetPublicGenericMethod(
                nameof(IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>.FilterArgument),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        private QueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TType, TExecutionContext> ArgumentImpl<TType>(string name)
        {
            var argumentedField = Field.ApplyArgument<TType>(name);
            return new QueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TType, TExecutionContext>(argumentedField);
        }

        private QueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgumentImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new QueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext>(argumentedField);
        }
    }

    internal class QueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> :
        QueryArgumentBuilderBase<IArgumentedField<object, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>,
        IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
    {
        public QueryArgumentBuilder(IArgumentedField<object, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> argumentedField)
            : base(argumentedField)
        {
        }
    }
}
