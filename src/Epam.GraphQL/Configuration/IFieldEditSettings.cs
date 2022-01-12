// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Threading.Tasks;
using Epam.GraphQL.Loaders;
using GraphQL;
using GraphQL.DataLoader;

namespace Epam.GraphQL.Configuration
{
    internal interface IFieldEditSettings<TExecutionContext>
    {
        bool IsReadOnly { get; }

        bool IsMandatoryForUpdate { get; set; }

        Func<IResolveFieldContext, object, object>? GetDefaultValue { get; set; }

        Action<IResolveFieldContext, object, object?>? OnWrite { get; }

        Func<IResolveFieldContext, object, object?, Task>? OnWriteAsync { get; }
    }

    internal interface IFieldEditSettings<TEntity, TExecutionContext> : IFieldEditSettings<TExecutionContext>
    {
        Func<IResolveFieldContext, IDataLoader<IFieldChange<TEntity, TExecutionContext>, (bool CanEdit, string DisableReason)>>? CanEdit { get; set; }
    }

    internal interface IFieldEditSettings<TEntity, TReturnType, TExecutionContext> : IFieldEditSettings<TEntity, TExecutionContext>
    {
        new Action<IResolveFieldContext, TEntity, TReturnType>? OnWrite { get; set; }

        new Func<IResolveFieldContext, TEntity, TReturnType, Task>? OnWriteAsync { get; set; }
    }
}
