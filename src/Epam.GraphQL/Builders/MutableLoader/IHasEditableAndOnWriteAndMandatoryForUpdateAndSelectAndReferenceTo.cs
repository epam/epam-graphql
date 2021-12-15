// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Builders.MutableLoader
{
    public interface IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, TReturnType, TExecutionContext> :
        IHasEditableAndOnWrite<TEntity, TReturnType, TExecutionContext>,
        IHasEditableAndOnWriteAndMandatoryForUpdate<TEntity, TReturnType, TExecutionContext>,
        IHasEditableAndOnWriteAndMandatoryForUpdateAndSelect<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        IHasEditableAndOnWriteAndMandatoryForUpdate<TEntity, TReturnType, TExecutionContext> ReferenceTo<TParentEntity, TParentEntityLoader>(Predicate<TReturnType> isFakePropValue)
            where TParentEntity : class
            where TParentEntityLoader : Loader<TParentEntity, TExecutionContext>, IIdentifiableLoader, new();
    }
}
