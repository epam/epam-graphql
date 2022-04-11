// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Helpers;

namespace Epam.GraphQL.Loaders
{
    public static class EpamGraphQLExtensions
    {
        public static void Resolve<TEntity, TReturnType, TExecutionContext>(
            this IResolvableField<TEntity, TExecutionContext> field,
            Func<TEntity, TReturnType> resolve)
            where TEntity : class
        {
            Guards.ThrowIfNull(field, nameof(field));
            field.Resolve((ctx, entity) => resolve(entity));
        }

        public static void Resolve<TEntity, TReturnType, TExecutionContext>(
            this IResolvableField<TEntity, TExecutionContext> field,
            Func<TEntity, TReturnType> resolve,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TEntity : class
            where TReturnType : class
        {
            Guards.ThrowIfNull(field, nameof(field));
            field.Resolve((ctx, entity) => resolve(entity), build);
        }

        public static void Resolve<TEntity, TReturnType, TExecutionContext>(
            this IResolvableField<TEntity, TExecutionContext> field,
            Func<TEntity, Task<TReturnType>> resolve)
            where TEntity : class
        {
            Guards.ThrowIfNull(field, nameof(field));
            field.Resolve((ctx, entity) => resolve(entity));
        }

        public static void Resolve<TEntity, TReturnType, TExecutionContext>(
            this IResolvableField<TEntity, TExecutionContext> field,
            Func<TEntity, Task<TReturnType>> resolve,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TEntity : class
            where TReturnType : class
        {
            Guards.ThrowIfNull(field, nameof(field));
            field.Resolve((ctx, entity) => resolve(entity), build);
        }

        public static void Resolve<TEntity, TReturnType, TExecutionContext>(
            this IResolvableField<TEntity, TExecutionContext> field,
            Func<TEntity, IEnumerable<TReturnType>> resolve)
            where TEntity : class
        {
            Guards.ThrowIfNull(field, nameof(field));
            field.Resolve((ctx, entity) => resolve(entity));
        }

        public static void Resolve<TEntity, TReturnType, TExecutionContext>(
            this IResolvableField<TEntity, TExecutionContext> field,
            Func<TEntity, IEnumerable<TReturnType>> resolve,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TEntity : class
            where TReturnType : class
        {
            Guards.ThrowIfNull(field, nameof(field));
            field.Resolve((ctx, entity) => resolve(entity), build);
        }

        public static void Resolve<TEntity, TReturnType, TExecutionContext>(
            this IResolvableField<TEntity, TExecutionContext> field,
            Func<TEntity, Task<IEnumerable<TReturnType>>> resolve)
            where TEntity : class
        {
            Guards.ThrowIfNull(field, nameof(field));
            field.Resolve((ctx, entity) => resolve(entity));
        }

        public static void Resolve<TEntity, TReturnType, TExecutionContext>(
            this IResolvableField<TEntity, TExecutionContext> field,
            Func<TEntity, Task<IEnumerable<TReturnType>>> resolve,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TEntity : class
            where TReturnType : class
        {
            Guards.ThrowIfNull(field, nameof(field));
            field.Resolve((ctx, entity) => resolve(entity), build);
        }
    }
}
