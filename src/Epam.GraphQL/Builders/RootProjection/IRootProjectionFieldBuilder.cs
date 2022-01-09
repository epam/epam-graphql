// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;

#nullable enable

namespace Epam.GraphQL.Builders.RootProjection
{
    public interface IRootProjectionFieldBuilder<TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, TReturnType> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, Task<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;
    }

    public interface IRootProjectionFieldBuilder<out TArgType, TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, TReturnType> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, IEnumerable<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<IEnumerable<TReturnType>>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;
    }

    public interface IRootProjectionFieldBuilder<out TArgType1, out TArgType2, TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TReturnType> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, IEnumerable<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<IEnumerable<TReturnType>>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;
    }

    public interface IRootProjectionFieldBuilder<out TArgType1, out TArgType2, out TArgType3, TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TReturnType> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, IEnumerable<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<IEnumerable<TReturnType>>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;
    }

    public interface IRootProjectionFieldBuilder<out TArgType1, out TArgType2, out TArgType3, out TArgType4, TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TReturnType> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, IEnumerable<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<IEnumerable<TReturnType>>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;
    }

    public interface IRootProjectionFieldBuilder<out TArgType1, out TArgType2, out TArgType3, out TArgType4, out TArgType5, TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TReturnType> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, IEnumerable<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<IEnumerable<TReturnType>>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;
    }
}
