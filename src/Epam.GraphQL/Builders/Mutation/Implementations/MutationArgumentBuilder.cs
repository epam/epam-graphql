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
    internal class MutationArgumentBuilder<TArgType, TExecutionContext> :
        MutationArgumentBuilderBase<IArgumentedField<object, TArgType, TExecutionContext>, TArgType, TExecutionContext>,
        IMutationArgumentBuilder<TArgType, TExecutionContext>
    {
        public MutationArgumentBuilder(IArgumentedField<object, TArgType, TExecutionContext> argumentedField)
            : base(argumentedField)
        {
        }

        public IMutationArgumentBuilder<TArgType, TType, TExecutionContext> Argument<TType>(string name)
        {
            return ArgumentImpl<TType>(name);
        }

        public IMutationArgumentBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterArgumentImpl<TProjection, TEntity1>(name);
        }

        public IMutationArgumentBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TEntity1>(Type projectionType, string name)
            where TEntity1 : class
        {
            var methodInfo = typeof(IMutationArgumentBuilder<TArgType, TExecutionContext>).GetPublicGenericMethod(
                nameof(IMutationArgumentBuilder<TArgType, TExecutionContext>.FilterArgument),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IMutationArgumentBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        private MutationArgumentBuilder<TArgType, TType, TExecutionContext> ArgumentImpl<TType>(string name)
        {
            var argumentedField = Field.ApplyArgument<TType>(name);
            return new MutationArgumentBuilder<TArgType, TType, TExecutionContext>(argumentedField);
        }

        private MutationArgumentBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgumentImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new MutationArgumentBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext>(argumentedField);
        }
    }

    internal class MutationArgumentBuilder<TArgType1, TArgType2, TExecutionContext> :
        MutationArgumentBuilderBase<IArgumentedField<object, TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext>,
        IMutationArgumentBuilder<TArgType1, TArgType2, TExecutionContext>
    {
        public MutationArgumentBuilder(IArgumentedField<object, TArgType1, TArgType2, TExecutionContext> argumentedField)
            : base(argumentedField)
        {
        }

        public IMutationArgumentBuilder<TArgType1, TArgType2, TType, TExecutionContext> Argument<TType>(string name)
        {
            return ArgumentImpl<TType>(name);
        }

        public IMutationArgumentBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterArgumentImpl<TProjection, TEntity1>(name);
        }

        public IMutationArgumentBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TEntity1>(Type projectionType, string name)
            where TEntity1 : class
        {
            var methodInfo = typeof(IMutationArgumentBuilder<TArgType1, TArgType2, TExecutionContext>).GetPublicGenericMethod(
                nameof(IMutationArgumentBuilder<TArgType1, TArgType2, TExecutionContext>.FilterArgument),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IMutationArgumentBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        private MutationArgumentBuilder<TArgType1, TArgType2, TType, TExecutionContext> ArgumentImpl<TType>(string name)
        {
            var argumentedField = Field.ApplyArgument<TType>(name);
            return new MutationArgumentBuilder<TArgType1, TArgType2, TType, TExecutionContext>(argumentedField);
        }

        private MutationArgumentBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgumentImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new MutationArgumentBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext>(argumentedField);
        }
    }

    internal class MutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext> :
        MutationArgumentBuilderBase<IArgumentedField<object, TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext>,
        IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>
    {
        public MutationArgumentBuilder(IArgumentedField<object, TArgType1, TArgType2, TArgType3, TExecutionContext> argumentedField)
            : base(argumentedField)
        {
        }

        public IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TType, TExecutionContext> Argument<TType>(string name)
        {
            return ArgumentImpl<TType>(name);
        }

        public IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterArgumentImpl<TProjection, TEntity1>(name);
        }

        public IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TEntity1>(Type projectionType, string name)
            where TEntity1 : class
        {
            var methodInfo = typeof(IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>).GetPublicGenericMethod(
                nameof(IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>.FilterArgument),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        private MutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TType, TExecutionContext> ArgumentImpl<TType>(string name)
        {
            var argumentedField = Field.ApplyArgument<TType>(name);
            return new MutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TType, TExecutionContext>(argumentedField);
        }

        private MutationArgumentBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgumentImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new MutationArgumentBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext>(argumentedField);
        }
    }

    internal class MutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> :
        MutationArgumentBuilderBase<IArgumentedField<object, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>,
        IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
    {
        public MutationArgumentBuilder(IArgumentedField<object, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> argumentedField)
            : base(argumentedField)
        {
        }

        public IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TType, TExecutionContext> Argument<TType>(string name)
        {
            return ArgumentImpl<TType>(name);
        }

        public IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterArgumentImpl<TProjection, TEntity1>(name);
        }

        public IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TEntity1>(Type projectionType, string name)
            where TEntity1 : class
        {
            var methodInfo = typeof(IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>).GetPublicGenericMethod(
                nameof(IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>.FilterArgument),
                new[] { projectionType, typeof(TEntity1) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext>>(this, name);
        }

        private MutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TType, TExecutionContext> ArgumentImpl<TType>(string name)
        {
            var argumentedField = Field.ApplyArgument<TType>(name);
            return new MutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TType, TExecutionContext>(argumentedField);
        }

        private MutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgumentImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new MutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext>(argumentedField);
        }
    }

    internal class MutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> :
        MutationArgumentBuilderBase<IArgumentedField<object, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>,
        IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
    {
        public MutationArgumentBuilder(IArgumentedField<object, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> argumentedField)
            : base(argumentedField)
        {
        }
    }
}
