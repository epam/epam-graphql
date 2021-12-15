// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Builders.Common.Implementations
{
    internal class DummyConnectionBuilder<TLoader, TEntity, TExecutionContext> : IConnectionBuilder
        where TLoader : Loader<TEntity, TExecutionContext>
        where TEntity : class
    {
        public IConnectionBuilder WithFilter<TLoaderFilter>()
        {
            return this;
        }

        public IConnectionBuilder WithFilter(Type loaderFilter)
        {
            return this;
        }

        public IConnectionBuilder WithSearch<TSearcher>()
        {
            return this;
        }

        public IConnectionBuilder WithSearch(Type searcherType)
        {
            return this;
        }
    }
}
