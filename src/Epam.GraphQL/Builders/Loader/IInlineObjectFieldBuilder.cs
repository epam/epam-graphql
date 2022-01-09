// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Builders.Common;
using Epam.GraphQL.Loaders;

#nullable enable

namespace Epam.GraphQL.Builders.Loader
{
    public interface IInlineObjectFieldBuilder<TEntity, TExecutionContext> : IHasFromIQueryable<TEntity, TExecutionContext>, IHasFromBatch<TEntity, TExecutionContext>
        where TEntity : class
    {
        IFromLoaderInlineObjectBuilder<TEntity, TChildEntity, TChildEntity> FromLoader<TChildLoader, TChildEntity>(Expression<Func<TEntity, TChildEntity, bool>> condition)
            where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
            where TChildEntity : class;

        IFromLoaderInlineObjectBuilder<TEntity, TChildEntity, TChildEntity> FromLoader<TChildEntity>(Type childLoader, Expression<Func<TEntity, TChildEntity, bool>> condition)
            where TChildEntity : class;
    }
}
