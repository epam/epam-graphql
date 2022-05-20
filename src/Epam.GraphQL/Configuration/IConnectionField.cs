// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Epam.GraphQL.Filters;
using Epam.GraphQL.Search;

namespace Epam.GraphQL.Configuration
{
    /// <summary>
    /// Provides additional methods for filtering and search against data returned from connection.
    /// </summary>
    /// <seealso href="https://facebook.github.io/relay/graphql/connections.htm">Relay Connection</seealso>
    public interface IConnectionField
    {
        /// <summary>
        /// Allows to use a filter parameter on GraphQL node.
        /// </summary>
        /// <typeparam name="TFilter">Implementation of class inherited from <see cref="Filter{TEntity,TFilter,TExecutionContext}"/>.</typeparam>
        /// <returns>A new configured field making possible to use a chunk of method calls.</returns>
        /// <seealso cref="WithSearch"/>
        IConnectionField WithFilter<TFilter>();

        /// <summary>
        /// Allows to use a search string parameter on GraphQL node.
        /// </summary>
        /// <typeparam name="TSearcher">Implementation of class inherited from <see cref="Searcher{TEntity, TExecutionContext}"/>.</typeparam>
        /// <seealso cref="WithFilter{TLoaderFilter}"/>
        /// <returns>A new configured field making possible to use a chunk of method calls.</returns>
        IConnectionField WithSearch<TSearcher>()
            where TSearcher : ISearcher;
    }

    public interface IConnectionField<TEntity, TExecutionContext> :
        ILegacyFilterableField<IConnectionField<TEntity, TExecutionContext>, TEntity, TExecutionContext>,
        ISearchableField<IConnectionField<TEntity, TExecutionContext>, TEntity, TExecutionContext>
    {
    }
}
