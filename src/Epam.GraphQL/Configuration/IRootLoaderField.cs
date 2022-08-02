// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Epam.GraphQL.Helpers;

namespace Epam.GraphQL.Configuration
{
    public interface IRootLoaderField<TEntity, TExecutionContext> :
        IRootEnumerableField<IRootLoaderField<TEntity, TExecutionContext>, TEntity, TExecutionContext>,
        IWhereableField<IRootLoaderField<TEntity, TExecutionContext>, TEntity>,
        IConnectableField<IVoid, TEntity>,
        IConnectableField<IVoid>,
        IGroupConnectableField<IVoid, TEntity>,
        ISearchableField<IRootLoaderField<TEntity, TExecutionContext>, TEntity, TExecutionContext>
    {
    }
}
