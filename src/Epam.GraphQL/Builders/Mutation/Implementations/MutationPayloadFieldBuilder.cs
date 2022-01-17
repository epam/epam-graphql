// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Mutation;

namespace Epam.GraphQL.Builders.Mutation.Implementations
{
    internal class MutationPayloadFieldBuilder<TArgType, TExecutionContext> :
        MutationArgumentBuilderBase<IArgumentedMutationField<TArgType, TExecutionContext>, TArgType, TExecutionContext>,
        IMutationPayloadFieldBuilder<TArgType, TExecutionContext>
    {
        public MutationPayloadFieldBuilder(IArgumentedMutationField<TArgType, TExecutionContext> payloadedField)
            : base(payloadedField)
        {
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, MutationResult<TReturnType>> resolve)
        {
            Field.Resolve(resolve, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<MutationResult<TReturnType>>> resolve)
        {
            Field.Resolve(resolve, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
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

        private MutationPayloadFieldBuilder<TArgType, TType, TExecutionContext> PayloadFieldImpl<TType>(string name)
        {
            var payloadedField = Field.ApplyArgument<TType>(name);
            return new MutationPayloadFieldBuilder<TArgType, TType, TExecutionContext>(payloadedField);
        }

        private MutationPayloadFieldBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadFieldImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var payloadedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new MutationPayloadFieldBuilder<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext>(payloadedField);
        }
    }

    internal class MutationPayloadFieldBuilder<TArgType1, TArgType2, TExecutionContext> :
        MutationArgumentBuilderBase<IArgumentedMutationField<TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext>,
        IMutationPayloadFieldBuilder<TArgType1, TArgType2, TExecutionContext>
    {
        public MutationPayloadFieldBuilder(IArgumentedMutationField<TArgType1, TArgType2, TExecutionContext> payloadedField)
            : base(payloadedField)
        {
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, MutationResult<TReturnType>> resolve)
        {
            Field.Resolve(resolve, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<MutationResult<TReturnType>>> resolve)
        {
            Field.Resolve(resolve, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
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

        private MutationPayloadFieldBuilder<TArgType1, TArgType2, TType, TExecutionContext> PayloadFieldImpl<TType>(string name)
        {
            var payloadedField = Field.ApplyArgument<TType>(name);
            return new MutationPayloadFieldBuilder<TArgType1, TArgType2, TType, TExecutionContext>(payloadedField);
        }

        private MutationPayloadFieldBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadFieldImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var payloadedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new MutationPayloadFieldBuilder<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext>(payloadedField);
        }
    }

    internal class MutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext> :
        MutationArgumentBuilderBase<IArgumentedMutationField<TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext>,
        IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>
    {
        public MutationPayloadFieldBuilder(IArgumentedMutationField<TArgType1, TArgType2, TArgType3, TExecutionContext> payloadedField)
            : base(payloadedField)
        {
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, MutationResult<TReturnType>> resolve)
        {
            Field.Resolve(resolve, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<MutationResult<TReturnType>>> resolve)
        {
            Field.Resolve(resolve, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
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

        private MutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TType, TExecutionContext> PayloadFieldImpl<TType>(string name)
        {
            var payloadedField = Field.ApplyArgument<TType>(name);
            return new MutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TType, TExecutionContext>(payloadedField);
        }

        private MutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadFieldImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var payloadedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new MutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext>(payloadedField);
        }
    }

    internal class MutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> :
        MutationArgumentBuilderBase<IArgumentedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>,
        IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
    {
        public MutationPayloadFieldBuilder(IArgumentedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> payloadedField)
            : base(payloadedField)
        {
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, MutationResult<TReturnType>> resolve)
        {
            Field.Resolve(resolve, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<MutationResult<TReturnType>>> resolve)
        {
            Field.Resolve(resolve, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
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

        private MutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TType, TExecutionContext> PayloadFieldImpl<TType>(string name)
        {
            var payloadedField = Field.ApplyArgument<TType>(name);
            return new MutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TType, TExecutionContext>(payloadedField);
        }

        private MutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadFieldImpl<TProjection, TEntity1>(string name)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var payloadedField = Field.ApplyFilterArgument<TProjection, TEntity1>(name);
            return new MutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext>(payloadedField);
        }
    }

    internal class MutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> :
        MutationArgumentBuilderBase<IArgumentedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>,
        IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
    {
        public MutationPayloadFieldBuilder(IArgumentedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> payloadedField)
            : base(payloadedField)
        {
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, MutationResult<TReturnType>> resolve)
        {
            Field.Resolve(resolve, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<MutationResult<TReturnType>>> resolve)
        {
            Field.Resolve(resolve, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }
    }
}
