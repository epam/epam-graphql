// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Enums;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Builders.Common
{
    public interface IHasFromLoader<TEntity, TExecutionContext>
    {
        IFromLoaderBuilder<TEntity, TChildEntity, TChildEntity, TExecutionContext> FromLoader<TChildLoader, TChildEntity>(
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            RelationType relationType = RelationType.Association,
            Expression<Func<TChildEntity, TEntity>>? navigationProperty = null,
            Expression<Func<TEntity, TChildEntity>>? reverseNavigationProperty = null)
            where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
            where TChildEntity : class;

        IFromLoaderBuilder<TEntity, TChildEntity, TChildEntity, TExecutionContext> FromLoader<TChildEntity>(
            Type childLoaderType,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            RelationType relationType = RelationType.Association,
            Expression<Func<TChildEntity, TEntity>>? navigationProperty = null,
            Expression<Func<TEntity, TChildEntity>>? reverseNavigationProperty = null)
            where TChildEntity : class;
    }
}
