// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Mutation;

namespace Epam.GraphQL.Builders.Mutation
{
    public interface IMutationFieldBuilder<TExecutionContext> :
        IMutationFieldBuilderBase<TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, MutationResult<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, Task<MutationResult<TReturnType>>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        IMutationPayloadFieldBuilder<TType, TExecutionContext> PayloadField<TType>(string name);

        IMutationPayloadFieldBuilder<Expression<Func<TEntity, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity>(string name)
            where TProjection : Projection<TEntity, TExecutionContext>
            where TEntity : class;

        IMutationPayloadFieldBuilder<Expression<Func<TEntity, bool>>, TExecutionContext> FilterPayloadField<TEntity>(Type projectionType, string name)
            where TEntity : class;

        IMutationArgumentBuilder<TType, TExecutionContext> Argument<TType>(string name);

        IMutationArgumentBuilder<Expression<Func<TEntity, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity>(string name)
            where TProjection : Projection<TEntity, TExecutionContext>
            where TEntity : class;

        IMutationArgumentBuilder<Expression<Func<TEntity, bool>>, TExecutionContext> FilterArgument<TEntity>(Type projectionType, string name)
            where TEntity : class;
    }

    public interface IMutationFieldBuilder<out TThisType, out TArgType, TExecutionContext> :
        IMutationFieldBuilderBase<TArgType, TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, MutationResult<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<MutationResult<TReturnType>>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);
    }

    public interface IMutationFieldBuilder<out TThisType, out TArgType1, out TArgType2, TExecutionContext> :
        IMutationFieldBuilderBase<TArgType1, TArgType2, TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, MutationResult<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<MutationResult<TReturnType>>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);
    }

    public interface IMutationFieldBuilder<out TThisType, out TArgType1, out TArgType2, out TArgType3, TExecutionContext> :
        IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, MutationResult<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<MutationResult<TReturnType>>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);
    }

    public interface IMutationFieldBuilder<out TThisType, out TArgType1, out TArgType2, out TArgType3, out TArgType4, TExecutionContext> :
        IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, MutationResult<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<MutationResult<TReturnType>>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);
    }

    public interface IMutationFieldBuilder<out TThisType, out TArgType1, out TArgType2, out TArgType3, out TArgType4, out TArgType5, TExecutionContext> :
        IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, MutationResult<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<MutationResult<TReturnType>>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);
    }
}
