// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Builders.Common;
using Epam.GraphQL.Configuration.Implementations.Fields.ChildFields;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Builders.Loader.Implementations
{
    internal class FromLoaderInlineObjectBuilder<TField, TEntity, TChildLoader, TChildEntity, TResult, TExecutionContext> : IFromLoaderInlineObjectBuilder<TEntity, TChildEntity, TResult>
        where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
        where TEntity : class
        where TChildEntity : class
        where TField : EnumerableFieldBase<TEntity, TResult, TExecutionContext>
    {
        internal FromLoaderInlineObjectBuilder(TField field)
        {
            Field = field;
        }

        protected TField Field { get; private set; }

        public IFromLoaderInlineObjectBuilder<TEntity, TChildEntity, T> Select<T>(Expression<Func<TResult, T>> selector)
        {
            return new FromLoaderInlineObjectBuilder<EnumerableFieldBase<TEntity, T, TExecutionContext>, TEntity, TChildLoader, TChildEntity, T, TExecutionContext>(Field.ApplySelect(selector));
        }

        public IFromLoaderInlineObjectBuilder<TEntity, TChildEntity, T> Select<T>(Expression<Func<TEntity, TResult, T>> selector)
        {
            return new FromLoaderInlineObjectBuilder<EnumerableFieldBase<TEntity, T, TExecutionContext>, TEntity, TChildLoader, TChildEntity, T, TExecutionContext>(Field.ApplySelect(selector));
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
            return new FromLoaderInlineObjectBuilder<EnumerableFieldBase<TEntity, TResult, TExecutionContext>, TEntity, TChildLoader, TChildEntity, TResult, TExecutionContext>(
                Field.ApplyWhere(predicate));
        }
    }
}
