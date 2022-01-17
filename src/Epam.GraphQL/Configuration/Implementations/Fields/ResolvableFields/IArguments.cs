// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Loaders;
using GraphQL;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields
{
    internal interface IArguments
    {
        void ApplyTo(IArgumentCollection arguments);
    }

    internal interface IArguments<out TArg1, TExecutionContext> : IArguments
    {
        IArguments<TArg1, TArg2, TExecutionContext> Add<TArg2>(string argName);

        IArguments<TArg1, Expression<Func<TEntity, bool>>, TExecutionContext> AddFilter<TProjection, TEntity>(string argName)
            where TProjection : Projection<TEntity, TExecutionContext>
            where TEntity : class;

        Func<IResolveFieldContext, TResult> GetResolver<TResult>(Func<TExecutionContext, TArg1, TResult> resolve);
    }

    internal interface IArguments<out TArg1, out TArg2, TExecutionContext> : IArguments
    {
        IArguments<TArg1, TArg2, TArg3, TExecutionContext> Add<TArg3>(string argName);

        IArguments<TArg1, TArg2, Expression<Func<TEntity, bool>>, TExecutionContext> AddFilter<TProjection, TEntity>(string argName)
            where TProjection : Projection<TEntity, TExecutionContext>
            where TEntity : class;

        Func<IResolveFieldContext, TResult> GetResolver<TResult>(Func<TExecutionContext, TArg1, TArg2, TResult> resolve);
    }

    internal interface IArguments<out TArg1, out TArg2, out TArg3, TExecutionContext> : IArguments
    {
        IArguments<TArg1, TArg2, TArg3, TArg4, TExecutionContext> Add<TArg4>(string argName);

        IArguments<TArg1, TArg2, TArg3, Expression<Func<TEntity, bool>>, TExecutionContext> AddFilter<TProjection, TEntity>(string argName)
            where TProjection : Projection<TEntity, TExecutionContext>
            where TEntity : class;

        Func<IResolveFieldContext, TResult> GetResolver<TResult>(Func<TExecutionContext, TArg1, TArg2, TArg3, TResult> resolve);
    }

    internal interface IArguments<out TArg1, out TArg2, out TArg3, out TArg4, TExecutionContext> : IArguments
    {
        IArguments<TArg1, TArg2, TArg3, TArg4, TArg5, TExecutionContext> Add<TArg5>(string argName);

        IArguments<TArg1, TArg2, TArg3, TArg4, Expression<Func<TEntity, bool>>, TExecutionContext> AddFilter<TProjection, TEntity>(string argName)
            where TProjection : Projection<TEntity, TExecutionContext>
            where TEntity : class;

        Func<IResolveFieldContext, TResult> GetResolver<TResult>(Func<TExecutionContext, TArg1, TArg2, TArg3, TArg4, TResult> resolve);
    }

    internal interface IArguments<out TArg1, out TArg2, out TArg3, out TArg4, out TArg5, TExecutionContext> : IArguments
    {
        Func<IResolveFieldContext, TResult> GetResolver<TResult>(Func<TExecutionContext, TArg1, TArg2, TArg3, TArg4, TArg5, TResult> resolve);
    }
}
