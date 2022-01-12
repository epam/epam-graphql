// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Epam.GraphQL.Infrastructure;

namespace Epam.GraphQL.Loaders
{
    [InternalApi]
    public interface IIdentifiableLoader
    {
        object? GetId(object entity);
    }

    [InternalApi]
    public interface IIdentifiableLoader<TEntity, TId> : IIdentifiableLoader
    {
        TId GetId(TEntity entity);
    }
}
