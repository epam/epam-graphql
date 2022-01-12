// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Configuration.Implementations;
using Epam.GraphQL.Loaders;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Types;

namespace Epam.GraphQL.Configuration
{
    internal interface IField
    {
        string Name { get; }

        Type FieldType { get; }

        bool CanResolve { get; }

        FieldType AsFieldType();

        void ValidateField();

        string GetGraphQLTypePrefix();

        object? Resolve(IResolveFieldContext context);
    }

    internal interface IField<TExecutionContext> : IField
    {
        IFieldEditSettings<TExecutionContext>? EditSettings { get; }

        IGraphTypeDescriptor<TExecutionContext> GraphType { get; }
    }

    internal interface IField<TEntity, TExecutionContext> : IField<TExecutionContext>
    {
        new IFieldEditSettings<TEntity, TExecutionContext>? EditSettings { get; }

        IDataLoader<IFieldChange<TEntity, TExecutionContext>, (bool CanEdit, string DisableReason)> CanEdit(IResolveFieldContext context);
    }
}
