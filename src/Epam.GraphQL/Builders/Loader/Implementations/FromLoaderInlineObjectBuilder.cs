// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Builders.Common;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Configuration.Implementations.Fields.ChildFields;
using Epam.GraphQL.Loaders;

#nullable enable

namespace Epam.GraphQL.Builders.Loader.Implementations
{
    internal class FromLoaderInlineObjectBuilder<TEntity, TChildLoader, TChildEntity, TResult, TExecutionContext> : IFromLoaderInlineObjectBuilder<TEntity, TChildEntity, TResult>
        where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
        where TEntity : class
        where TChildEntity : class
    {
        internal FromLoaderInlineObjectBuilder(RelationRegistry<TExecutionContext> registry, EnumerableFieldBase<TEntity, TResult, TExecutionContext> field)
        {
            Registry = registry ?? throw new ArgumentNullException(nameof(registry));
            Field = field;
        }

        protected EnumerableFieldBase<TEntity, TResult, TExecutionContext> Field { get; private set; }

        protected RelationRegistry<TExecutionContext> Registry { get; }

        public IFromLoaderInlineObjectBuilder<TEntity, TChildEntity, T> Select<T>(Expression<Func<TResult, T>> selector)
        {
            return new FromLoaderInlineObjectBuilder<TEntity, TChildLoader, TChildEntity, T, TExecutionContext>(Registry, Field.ApplySelect(selector));
        }

        public IFromLoaderInlineObjectBuilder<TEntity, TChildEntity, T> Select<T>(Expression<Func<TEntity, TResult, T>> selector)
        {
            return new FromLoaderInlineObjectBuilder<TEntity, TChildLoader, TChildEntity, T, TExecutionContext>(Registry, Field.ApplySelect(selector));
        }

        public virtual void SingleOrDefault(Expression<Func<TResult, bool>>? predicate)
        {
            Field.ApplySingleOrDefault(predicate);
        }

        public virtual void FirstOrDefault(Expression<Func<TResult, bool>>? predicate)
        {
            Field.ApplyFirstOrDefault(predicate);
        }

        public IFromLoaderInlineObjectBuilder<TEntity, TChildEntity, TResult> Where(Expression<Func<TResult, bool>> predicate)
        {
            Field = Field.ApplyWhere(predicate);
            return this;
        }
    }
}
