// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Linq;

namespace Epam.GraphQL.Search
{
#pragma warning disable CA1040 // Avoid empty interfaces
    public interface ISearcher
#pragma warning restore CA1040 // Avoid empty interfaces
    {
    }

    public interface ISearcher<TEntity, TExecutionContext> : ISearcher
    {
        IQueryable<TEntity> All(IQueryable<TEntity> query, TExecutionContext context, string? search);
    }
}
