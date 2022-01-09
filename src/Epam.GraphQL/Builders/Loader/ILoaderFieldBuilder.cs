// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Epam.GraphQL.Builders.Common;
using Epam.GraphQL.Builders.Projection;

#nullable enable

namespace Epam.GraphQL.Builders.Loader
{
    public interface ILoaderFieldBuilder<TEntity, TExecutionContext> :
        IProjectionFieldBuilder<TEntity, TExecutionContext>,
        IHasFromBatch<TEntity, TExecutionContext>,
        IHasFromLoader<TEntity, TExecutionContext>
        where TEntity : class
    {
    }
}
