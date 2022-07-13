// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Collections.Generic;
using Epam.GraphQL.Filters;
using Epam.GraphQL.Sorters;
using GraphQL.Types;

namespace Epam.GraphQL.Configuration
{
    internal interface IObjectGraphTypeConfigurator<TExecutionContext>
    {
        IField<TExecutionContext>? Parent { get; }

        string Name { get; }

        bool HasInlineFilters { get; }

        IReadOnlyList<ISorter<TExecutionContext>> Sorters { get; }

        void Configure();

        void ConfigureGraphType(IComplexGraphType graphType);

        void ConfigureGroupGraphType(IObjectGraphType graphType);

        IInlineFilters<TExecutionContext> CreateInlineFilters();

        bool FilterEquals(IObjectGraphTypeConfigurator<TExecutionContext> other);
    }

    internal interface IObjectGraphTypeConfigurator<TEntity, TExecutionContext> : IObjectGraphTypeConfigurator<TExecutionContext>
    {
        IReadOnlyList<IField<TEntity, TExecutionContext>> Fields { get; }

        IProxyAccessor<TEntity, TExecutionContext> ProxyAccessor { get; }

        new IInlineFilters<TEntity, TExecutionContext> CreateInlineFilters();
    }
}
