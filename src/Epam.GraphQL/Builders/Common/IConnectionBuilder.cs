// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;

namespace Epam.GraphQL.Builders.Common
{
    /// <summary>
    /// <see cref="IConnectionBuilder"/> provides additional methods for filtering and search against data returned from connection.
    /// </summary>
    /// <seealso href="https://facebook.github.io/relay/graphql/connections.htm">Relay Connection</seealso>
    public interface IConnectionBuilder
    {
        /// <summary>
        /// Allows to use a filter parameter on GraphQL node.
        /// </summary>
        /// <typeparam name="TLoaderFilter">Implementation of class inherited from <see cref="Filters.Filter{TEntity,TFilter,TExecutionContext}"/>.</typeparam>
        /// <returns>Builder making possible to use a chunk of method calls.</returns>
        /// <seealso cref="WithFilter(Type)"/>
        /// <seealso cref="WithSearch"/>
        IConnectionBuilder WithFilter<TLoaderFilter>();

        /// <summary>
        /// Allows to use a filter parameter on GraphQL node.
        /// </summary>
        /// <param name="loaderFilterType">Type which implements class inherited from <see cref="Filters.Filter{TEntity,TFilter,TExecutionContext}"/>.</param>
        /// <returns>Builder making possible to use a chunk of method calls.</returns>
        /// <seealso cref="WithFilter{TLoaderFilter}"/>
        /// <seealso cref="WithSearch"/>
        IConnectionBuilder WithFilter(Type loaderFilterType);

        /// <summary>
        /// Allows to use a search string parameter on GraphQL node.
        /// </summary>
        /// <typeparam name="TSearcher">Implementation of class inherited from <see cref="Search.Searcher{TEntity, TExecutionContext}"/>.</typeparam>
        /// <seealso cref="WithFilter{TLoaderFilter}"/>
        /// <seealso cref="WithFilter(Type)"/>
        /// <returns>Builder making possible to use a chunk of method calls.</returns>
        IConnectionBuilder WithSearch<TSearcher>();

        IConnectionBuilder WithSearch(Type searcherType);
    }
}
