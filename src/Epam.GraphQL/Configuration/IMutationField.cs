// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Mutation;

namespace Epam.GraphQL.Configuration
{
    public interface IMutationField<TExecutionContext> : IUnionableRootField<TExecutionContext>
    {
        void Resolve<TReturnType>(Func<TExecutionContext, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        void Resolve<TReturnType>(Func<TExecutionContext, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder = null);

        void Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;

        void Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder);

        void Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class;

        IArgumentedMutationField<TArgType, TExecutionContext> Argument<TArgType>(string argName);

        IArgumentedMutationField<Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class;

        IPayloadFieldedMutationField<TArgType, TExecutionContext> PayloadField<TArgType>(string argName);

        IPayloadFieldedMutationField<Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class;
    }
}
