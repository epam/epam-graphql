// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Linq;

namespace Epam.GraphQL.Search
{
    public abstract class Searcher<TEntity, TExecutionContext> : ISearcher<TEntity, TExecutionContext>
    {
        public abstract IQueryable<TEntity> All(IQueryable<TEntity> query, TExecutionContext context, string? search);

        public override int GetHashCode() => GetType().GetHashCode();

        public override bool Equals(object obj) => Equals(obj as ISearcher<TEntity, TExecutionContext>);

        public bool Equals(ISearcher<TEntity, TExecutionContext>? other) => other != null
            && other.GetType() == GetType();
    }
}
