// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Mutation;

namespace Epam.GraphQL.Configuration
{
    public interface IResolvableMutationFieldMixin<TArgType, TExecutionContext>
        : IResolvableRootField<TArgType, TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;
    }

    public interface IResolvableMutationFieldMixin<TArgType1, TArgType2, TExecutionContext>
        : IResolvableRootField<TArgType1, TArgType2, TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;
    }

    public interface IResolvableMutationFieldMixin<TArgType1, TArgType2, TArgType3, TExecutionContext>
        : IResolvableRootField<TArgType1, TArgType2, TArgType3, TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;
    }

    public interface IResolvableMutationFieldMixin<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
        : IResolvableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;
    }

    public interface IResolvableMutationFieldMixin<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
        : IResolvableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;
    }
}
