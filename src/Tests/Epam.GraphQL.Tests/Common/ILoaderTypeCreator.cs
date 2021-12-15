// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using Epam.Contracts.Models;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Tests.TestData;

namespace Epam.GraphQL.Tests.Common
{
    public interface ILoaderTypeCreator<TEntity>
        where TEntity : class, IHasId<int>
    {
        Type CreateLoaderType(IEnumerable<TEntity> entities, Action<Loader<TEntity, TestUserContext>> configure);

        Type CreateIdentifiableLoaderType(IEnumerable<TEntity> entities, Action<IdentifiableLoader<TEntity, int, TestUserContext>> configure);

        Type CreateMutableLoaderType(IEnumerable<TEntity> entities, Action<MutableLoader<TEntity, int, TestUserContext>> configure);
    }
}
