// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Collections.Generic;

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal interface IBatchCompoundResolver<TEntity, TExecutionContext> : IBatchResolver<TEntity, IEnumerable<object>>
    {
        IBatchCompoundResolver<TEntity, TExecutionContext> Add(IBatchResolver<TEntity> resolver);
    }
}
