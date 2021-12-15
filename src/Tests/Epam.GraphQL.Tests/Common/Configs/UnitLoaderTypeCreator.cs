// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Tests.Helpers;
using Epam.GraphQL.Tests.TestData;

namespace Epam.GraphQL.Tests.Common.Configs
{
    public class UnitLoaderTypeCreator : ILoaderTypeCreator<Unit>
    {
        public Type CreateIdentifiableLoaderType(IEnumerable<Unit> entities, Action<IdentifiableLoader<Unit, int, TestUserContext>> configure)
        {
            return GraphQLTypeBuilder.CreateIdentifiableLoaderType(
                onConfigure: configure,
                getBaseQuery: _ => entities.AsQueryable(),
                idExpression: p => p.Id);
        }

        public Type CreateLoaderType(IEnumerable<Unit> entities, Action<Loader<Unit, TestUserContext>> configure)
        {
            return GraphQLTypeBuilder.CreateLoaderType(
                onConfigure: configure,
                applyNaturalOrderBy: q => q.OrderBy(p => p.Id),
                applyNaturalThenBy: q => q.ThenBy(p => p.Id),
                getBaseQuery: _ => entities.AsQueryable());
        }

        public Type CreateMutableLoaderType(IEnumerable<Unit> entities, Action<MutableLoader<Unit, int, TestUserContext>> configure)
        {
            return GraphQLTypeBuilder.CreateMutableLoaderType(
                onConfigure: configure,
                getBaseQuery: _ => entities.AsQueryable());
        }
    }
}
