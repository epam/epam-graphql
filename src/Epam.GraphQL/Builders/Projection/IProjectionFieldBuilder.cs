// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;

namespace Epam.GraphQL.Builders.Projection
{
    public interface IProjectionFieldBuilder<TEntity, TExecutionContext> :
        IUnionableProjectionFieldBuilder<IProjectionFieldBuilder<TEntity, TExecutionContext>, TExecutionContext>,
        IHasFromIQueryable<TEntity, TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TEntity, TReturnType> resolve);

        void Resolve<TReturnType>(Func<TEntity, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TEntity, Task<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TEntity, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TEntity, IEnumerable<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TEntity, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TEntity, Task<IEnumerable<TReturnType>>> resolve);

        void Resolve<TReturnType>(Func<TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve);

        void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class;
    }
}
