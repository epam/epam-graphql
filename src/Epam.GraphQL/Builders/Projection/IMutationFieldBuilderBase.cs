// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Threading.Tasks;
using Epam.GraphQL.Mutation;

namespace Epam.GraphQL.Builders.Projection
{
    public interface IMutationFieldBuilderBase<TExecutionContext> :
        IRootProjectionFieldBuilder<TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, MutationResult<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, Task<MutationResult<TReturnType>>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);
    }

    public interface IMutationFieldBuilderBase<out TArgType, TExecutionContext> :
        IRootProjectionFieldBuilder<TArgType, TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, MutationResult<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<MutationResult<TReturnType>>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);
    }

    public interface IMutationFieldBuilderBase<out TArgType1, out TArgType2, TExecutionContext> :
        IRootProjectionFieldBuilder<TArgType1, TArgType2, TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, MutationResult<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<MutationResult<TReturnType>>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);
    }

    public interface IMutationFieldBuilderBase<out TArgType1, out TArgType2, out TArgType3, TExecutionContext> :
        IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, MutationResult<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<MutationResult<TReturnType>>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);
    }

    public interface IMutationFieldBuilderBase<out TArgType1, out TArgType2, out TArgType3, out TArgType4, TExecutionContext> :
        IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, MutationResult<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<MutationResult<TReturnType>>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);
    }

    public interface IMutationFieldBuilderBase<out TArgType1, out TArgType2, out TArgType3, out TArgType4, out TArgType5, TExecutionContext> :
        IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, MutationResult<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<MutationResult<TReturnType>>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);
    }
}
