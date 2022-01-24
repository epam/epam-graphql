// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Linq.Expressions;

namespace Epam.GraphQL.Configuration
{
    public interface IConnectableField<out TThis>
    {
        TThis AsConnection();
    }

    public interface IConnectableField<out TThis, TEntity>
    {
        TThis AsConnection(Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>> naturalOrder);
    }
}
