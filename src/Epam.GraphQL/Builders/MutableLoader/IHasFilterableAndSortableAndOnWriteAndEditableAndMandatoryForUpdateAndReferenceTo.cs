// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Enums;
using Epam.GraphQL.Loaders;

#nullable enable

namespace Epam.GraphQL.Builders.MutableLoader
{
    public interface IHasFilterableAndSortableAndOnWriteAndEditableAndMandatoryForUpdateAndReferenceTo<TEntity, TReturnType, TFilterValueType, TExecutionContext> :
        IHasFilterableAndSortableAndOnWriteAndEditableAndMandatoryForUpdate<TEntity, TReturnType, TFilterValueType, TExecutionContext>
    {
        IHasFilterableAndSortableAndOnWriteAndEditableAndMandatoryForUpdate<TEntity, TReturnType, TFilterValueType, TExecutionContext> ReferencesTo<TParentEntity, TParentEntityLoader>(Expression<Func<TParentEntity, TReturnType>> parentProperty, Expression<Func<TEntity, TParentEntity>> navigationProperty, RelationType relationType)
            where TParentEntity : class
            where TParentEntityLoader : Loader<TParentEntity, TExecutionContext>, IIdentifiableLoader, new();

        IHasFilterableAndSortableAndOnWriteAndEditableAndMandatoryForUpdate<TEntity, TReturnType, TFilterValueType, TExecutionContext> ReferencesTo<TParentEntity>(Type parentLoaderType, Expression<Func<TParentEntity, TReturnType>> parentProperty, Expression<Func<TEntity, TParentEntity>> navigationProperty, RelationType relationType)
            where TParentEntity : class;
    }
}
