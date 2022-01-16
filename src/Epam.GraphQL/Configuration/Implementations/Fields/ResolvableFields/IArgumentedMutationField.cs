// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Mutation;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields
{
    internal interface IArgumentedMutationField<TExecutionContext> : IUnionableMutationField<TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder);

        IArgumentedMutationField<TArgType, TExecutionContext> Argument<TArgType>(string argName);

        IArgumentedMutationField<Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class;

        IArgumentedMutationField<TArgType, TExecutionContext> PayloadField<TArgType>(string argName);

        IArgumentedMutationField<Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class;
    }

    internal interface IArgumentedMutationField<TArgType, TExecutionContext> : IUnionableMutationField<TArgType, TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder);

        IArgumentedMutationField<TArgType, TArgType2, TExecutionContext> ApplyArgument<TArgType2>(string argName);

        IArgumentedMutationField<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> ApplyFilterArgument<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class;
    }

    internal interface IArgumentedMutationField<TArgType1, TArgType2, TExecutionContext> : IUnionableMutationField<TArgType1, TArgType2, TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder);

        IArgumentedMutationField<TArgType1, TArgType2, TArgType3, TExecutionContext> ApplyArgument<TArgType3>(string argName);

        IArgumentedMutationField<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> ApplyFilterArgument<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class;
    }

    internal interface IArgumentedMutationField<TArgType1, TArgType2, TArgType3, TExecutionContext> : IUnionableMutationField<TArgType1, TArgType2, TArgType3, TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder);

        IArgumentedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> ApplyArgument<TArgType4>(string argName);

        IArgumentedMutationField<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> ApplyFilterArgument<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class;
    }

    internal interface IArgumentedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> : IUnionableMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder);

        IArgumentedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> ApplyArgument<TArgType5>(string argName);

        IArgumentedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> ApplyFilterArgument<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class;
    }

    internal interface IArgumentedMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> : IUnionableMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder);
    }
}
