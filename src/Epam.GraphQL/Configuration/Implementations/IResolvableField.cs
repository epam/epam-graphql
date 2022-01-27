// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;

namespace Epam.GraphQL.Configuration.Implementations
{
    internal interface IResolvableField<TEntity, TExecutionContext>
        where TEntity : class
    {
        void Resolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve, Action<ResolveOptionsBuilder>? optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class;
    }

    internal interface IResolvableField<TEntity, TArgType, TExecutionContext> : IResolvableRootField<TArgType, TExecutionContext>
        where TEntity : class
    {
    }

    internal interface IResolvableField<TEntity, TArgType1, TArgType2, TExecutionContext> : IResolvableRootField<TArgType1, TArgType2, TExecutionContext>
        where TEntity : class
    {
    }

    internal interface IResolvableField<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext> : IResolvableRootField<TArgType1, TArgType2, TArgType3, TExecutionContext>
        where TEntity : class
    {
    }

    internal interface IResolvableField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> : IResolvableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
        where TEntity : class
    {
    }

    internal interface IResolvableField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> : IResolvableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
        where TEntity : class
    {
    }
}
