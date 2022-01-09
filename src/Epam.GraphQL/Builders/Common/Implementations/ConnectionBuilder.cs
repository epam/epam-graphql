// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using Epam.GraphQL.Configuration.Implementations.Fields.ChildFields;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Filters;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;

#nullable enable

namespace Epam.GraphQL.Builders.Common.Implementations
{
    internal class ConnectionBuilder<TEntity, TChildEntity, TExecutionContext> : IConnectionBuilder
        where TEntity : class
    {
        private QueryableFieldBase<TEntity, TChildEntity, TExecutionContext> _field;

        public ConnectionBuilder(QueryableFieldBase<TEntity, TChildEntity, TExecutionContext> field)
        {
            _field = field ?? throw new ArgumentNullException(nameof(field));
        }

        public IConnectionBuilder WithFilter<TLoaderFilter>()
        {
            return WithFilter(typeof(TLoaderFilter));
        }

        public IConnectionBuilder WithFilter(Type loaderFilter)
        {
            var filterBaseType = TypeHelpers.FindMatchingGenericBaseType(loaderFilter, typeof(Filter<,,>));

            if (filterBaseType == null)
            {
                throw new ArgumentException($"Cannot find the corresponding base type for filter: {loaderFilter}");
            }

            var filterArgument = filterBaseType.GetGenericArguments().Single(type => typeof(Input).IsAssignableFrom(type));

            var withFilter = typeof(ConnectionBuilder<TEntity, TChildEntity, TExecutionContext>).GetNonPublicGenericMethod(
                nameof(WithFilter),
                new[] { loaderFilter, filterArgument },
                Type.EmptyTypes);

            return withFilter.InvokeAndHoistBaseException<IConnectionBuilder>(this);
        }

        public IConnectionBuilder WithSearch<TSearcher>()
        {
            _field = _field.ApplySearch<TSearcher>();
            return this;
        }

        public IConnectionBuilder WithSearch(Type searcherType)
        {
            var withSearch = typeof(ConnectionBuilder<TEntity, TChildEntity, TExecutionContext>).GetPublicGenericMethod(
                    nameof(WithSearch),
                    new[] { searcherType },
                    Type.EmptyTypes);

            return withSearch.InvokeAndHoistBaseException<IConnectionBuilder>(this);
        }

        private IConnectionBuilder WithFilter<TLoaderFilter, TFilter>()
            where TLoaderFilter : Filter<TChildEntity, TFilter, TExecutionContext>
            where TFilter : Input
        {
            _field = _field.ApplyFilter<TLoaderFilter, TFilter>();
            return this;
        }
    }
}
