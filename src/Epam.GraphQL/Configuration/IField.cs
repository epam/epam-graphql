// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Loaders;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace Epam.GraphQL.Configuration
{
    internal interface IField<TExecutionContext>
    {
        IObjectGraphTypeConfigurator<TExecutionContext> Parent { get; }

        string Name { get; }

        Type FieldType { get; }

        IFieldResolver Resolver { get; }

        MethodCallConfigurationContext ConfigurationContext { get; }

        FieldType AsFieldType();

        void Validate();
    }

    internal interface IField<TEntity, TExecutionContext> : IField<TExecutionContext>
    {
        IFieldEditSettings<TEntity, TExecutionContext>? EditSettings { get; }

        IDataLoader<IFieldChange<TEntity, TExecutionContext>, (bool CanEdit, string DisableReason)> CanEdit(IResolveFieldContext context);
    }
}
