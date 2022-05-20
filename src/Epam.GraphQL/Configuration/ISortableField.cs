// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;

namespace Epam.GraphQL.Configuration
{
    /// <summary>
    /// Provides additional methods for sorting by the field.
    /// </summary>
    /// <typeparam name="TThis">The type of the configured field.</typeparam>
    /// <typeparam name="TEntity">The type of a parent GraphQL field.</typeparam>
    /// <typeparam name="TExecutionContext">The type of execution context.</typeparam>
    public interface ISortableField<out TThis, TEntity, TExecutionContext>
    {
        /// <summary>
        /// Allows to sort a parent GraphQL field by this field.
        /// </summary>
        /// <returns>A new configured field making possible to use a chunk of method calls.</returns>
        /// <seealso cref="Sortable{TValue}(Expression{Func{TEntity, TValue}})"/>
        /// <seealso cref="Sortable{TValue}(Func{TExecutionContext, Expression{Func{TEntity, TValue}}})"/>
        /// <seealso cref="Loaders.Projection{TEntity, TExecutionContext}.Sorter{TValueType}(string, Expression{Func{TEntity, TValueType}})"/>
        /// <seealso cref="Loaders.Projection{TEntity, TExecutionContext}.Sorter{TValueType}(string, Func{TExecutionContext, Expression{Func{TEntity, TValueType}}})"/>
        TThis Sortable();

        /// <summary>
        /// Allows to sort a parent GraphQL field by this field using a calculated value for sorting.
        /// </summary>
        /// <typeparam name="TValue">The type of the calculated value returned by the expression that is represented by <paramref name="sorter"/>.</typeparam>
        /// <param name="sorter">An expression to compute a value for sorting from a <typeparamref name="TEntity"/>.</param>
        /// <returns>A new configured field making possible to use a chunk of method calls.</returns>
        /// <seealso cref="Sortable"/>
        /// <seealso cref="Sortable{TValue}(Func{TExecutionContext, Expression{Func{TEntity, TValue}}})"/>
        /// <seealso cref="Loaders.Projection{TEntity, TExecutionContext}.Sorter{TValueType}(string, Expression{Func{TEntity, TValueType}})"/>
        /// <seealso cref="Loaders.Projection{TEntity, TExecutionContext}.Sorter{TValueType}(string, Func{TExecutionContext, Expression{Func{TEntity, TValueType}}})"/>
        TThis Sortable<TValue>(Expression<Func<TEntity, TValue>> sorter);

        /// <summary>
        /// Allows to sort a parent GraphQL field by this field using a calculated value for sorting.
        /// </summary>
        /// <typeparam name="TValue">The type of the calculated value returned by the expression that is represented by <paramref name="sorterFactory"/>.</typeparam>
        /// <param name="sorterFactory">
        /// A factory function, which returns an expression depending on an execution context.
        /// The expression is used to compute a value for sorting from a <typeparamref name="TEntity"/>.
        /// </param>
        /// <returns>A new configured field making possible to use a chunk of method calls.</returns>
        /// <seealso cref="Sortable"/>
        /// <seealso cref="Sortable{TValue}(Expression{Func{TEntity, TValue}})"/>
        /// <seealso cref="Loaders.Projection{TEntity, TExecutionContext}.Sorter{TValueType}(string, Expression{Func{TEntity, TValueType}})"/>
        /// <seealso cref="Loaders.Projection{TEntity, TExecutionContext}.Sorter{TValueType}(string, Func{TExecutionContext, Expression{Func{TEntity, TValueType}}})"/>
        TThis Sortable<TValue>(Func<TExecutionContext, Expression<Func<TEntity, TValue>>> sorterFactory);
    }
}
