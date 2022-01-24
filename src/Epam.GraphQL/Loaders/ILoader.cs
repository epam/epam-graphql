// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Linq;

namespace Epam.GraphQL.Loaders
{
#pragma warning disable CA1040 // Avoid empty interfaces
    public interface ILoader<TExecutionContext>
#pragma warning restore CA1040 // Avoid empty interfaces
    {
    }

    public interface ILoader<TEntity, TExecutionContext> : ILoader<TExecutionContext>
    {
        IQueryable<TEntity> All(TExecutionContext context);

        IOrderedQueryable<TEntity> ApplyNaturalOrderBy(IQueryable<TEntity> query);

        IOrderedQueryable<TEntity> ApplyNaturalThenBy(IOrderedQueryable<TEntity> query);
    }
}
