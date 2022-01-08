// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields;

#nullable enable

namespace Epam.GraphQL.Configuration.Implementations
{
    internal interface IFieldSupportsApplyResolve<TEntity, TExecutionContext>
        where TEntity : class
    {
        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder);

        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder);

        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;

        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder);

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder);

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;
    }
}
