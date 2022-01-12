// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Builders.Loader
{
    public interface IHasSelectAndReferenceTo<TEntity, TReturnType, TExecutionContext> : IHasSelect<TReturnType, TExecutionContext>
    {
        IHasSelect<TReturnType, TExecutionContext> ReferenceTo<TParentEntity, TParentEntityLoader>(Predicate<TReturnType> isFakePropValue)
            where TParentEntity : class
            where TParentEntityLoader : Loader<TParentEntity, TExecutionContext>, IIdentifiableLoader, new();
    }
}
