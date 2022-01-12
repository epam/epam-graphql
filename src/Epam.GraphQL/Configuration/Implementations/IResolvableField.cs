// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Configuration.Implementations
{
    internal interface IResolvableField<TEntity, TExecutionContext>
        where TEntity : class
    {
        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve, Action<ResolveOptionsBuilder>? optionsBuilder);

        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder);

        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class;

        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class;

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder);

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder);

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class;

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class;

        ArgumentedField<TEntity, TArgType, TExecutionContext> ApplyArgument<TArgType>(string argName);

        ArgumentedField<TEntity, Expression<Func<TEntity1, bool>>, TExecutionContext> ApplyFilterArgument<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class;

        ArgumentedField<TEntity, TArgType, TExecutionContext> ApplyPayloadField<TArgType>(string argName);

        ArgumentedField<TEntity, Expression<Func<TEntity1, bool>>, TExecutionContext> ApplyFilterPayloadField<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class;
    }

    internal interface IResolvableField<TEntity, TArgType, TExecutionContext>
        where TEntity : class
    {
        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType, TReturnType> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType, Task<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder = null)
            where TReturnType : class;

        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder = null)
            where TReturnType : class;

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder = null)
            where TReturnType : class;

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder = null)
            where TReturnType : class;
    }

    internal interface IResolvableField<TEntity, TArgType1, TArgType2, TExecutionContext>
        where TEntity : class
    {
        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TReturnType> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder = null)
            where TReturnType : class;

        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder = null)
            where TReturnType : class;

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder = null)
            where TReturnType : class;

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder = null)
            where TReturnType : class;
    }

    internal interface IResolvableField<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext>
        where TEntity : class
    {
        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TReturnType> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder = null)
            where TReturnType : class;

        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder = null)
            where TReturnType : class;

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder = null)
            where TReturnType : class;

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder = null)
            where TReturnType : class;
    }

    internal interface IResolvableField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
        where TEntity : class
    {
        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TReturnType> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder = null)
            where TReturnType : class;

        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder = null)
            where TReturnType : class;

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder = null)
            where TReturnType : class;

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder = null)
            where TReturnType : class;
    }

    internal interface IResolvableField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
        where TEntity : class
    {
        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TReturnType> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder = null)
            where TReturnType : class;

        ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder = null)
            where TReturnType : class;

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder = null)
            where TReturnType : class;

        ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder = null)
            where TReturnType : class;
    }
}
