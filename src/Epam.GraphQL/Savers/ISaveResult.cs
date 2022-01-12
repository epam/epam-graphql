// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using Epam.GraphQL.Infrastructure;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Savers
{
    [InternalApi]
    public interface ISaveResult<TExecutionContext>
    {
        IList<ISaveResultItem> ProcessedItems { get; }

        IList<ISaveResultItem> PendingItems { get; }

        IList<ISaveResultItem> PostponedItems { get; }

        IMutableLoader<TExecutionContext> Loader { get; }

        string FieldName { get; }

        Type MutationType { get; }

        Type EntityType { get; }

        ISaveResult<TExecutionContext> CloneAndMovePostponedToPending();

        ISaveResult<TExecutionContext> Merge(ISaveResult<TExecutionContext> otherSaveResult);
    }
}
