// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration;

namespace Epam.GraphQL.Loaders
{
    public abstract class Projection<TEntity, TExecutionContext> : ProjectionBase<TEntity, TExecutionContext>
        where TEntity : class
    {
        protected internal IExpressionField<TEntity, TReturnType, TExecutionContext> Field<TReturnType>(Expression<Func<TEntity, TReturnType>> expression, string? deprecationReason = null)
           => AddField(expression, deprecationReason);

        protected internal IExpressionField<TEntity, TReturnType, TExecutionContext> Field<TReturnType>(string name, Expression<Func<TEntity, TReturnType>> expression, string? deprecationReason = null)
            => AddField(name, expression, deprecationReason);

        protected internal IExpressionField<TEntity, TReturnType, TExecutionContext> Field<TReturnType>(string name, Expression<Func<TExecutionContext, TEntity, TReturnType>> expression, string? deprecationReason = null)
            => AddField(name, expression, deprecationReason);

        protected internal void Filter<TValueType>(string name, Func<TValueType, Expression<Func<TEntity, bool>>> filterPredicateFactory)
        {
            ThrowIfIsNotConfiguring();
            if (!IsConfiguringInputType)
            {
                Configurator.Filter(name, filterPredicateFactory);
            }
        }

        protected internal void Filter<TValueType>(string name, Func<TExecutionContext, TValueType, Expression<Func<TEntity, bool>>> filterPredicateFactory)
        {
            ThrowIfIsNotConfiguring();
            if (!IsConfiguringInputType)
            {
                Configurator.Filter(name, filterPredicateFactory);
            }
        }

        /// <summary>
        /// Configures custom sorter for a loader (<see cref="Loader{TEntity, TExecutionContext}"/>).
        /// If a loader has one configured custom sorter at least, an argument `sorting` is exposed for all GraphQL fields,
        /// which are configured using the loader (for example, by calling <c>Field("fieldName").FromLoader&lt;ModelLoader, Model&gt;()</c>).
        /// If a GraphQL query passes <paramref name="name"/> to a `sorting` argument of this field,
        /// then result will be sorted using a <paramref name="selector"/> expression.
        /// </summary>
        /// <typeparam name="TValueType">The type of the calculated value returned by the expression that is represented by <paramref name="selector"/>.</typeparam>
        /// <param name="name">A name of a field in a GraphQL argument `sorting`.</param>
        /// <param name="selector">An expression to compute a value for sorting from a <typeparamref name="TEntity"/>.</param>
        protected internal void Sorter<TValueType>(string name, Expression<Func<TEntity, TValueType>> selector)
        {
            if (!IsConfiguringInputType)
            {
                ThrowIfIsNotConfiguring();
                Configurator.Sorter(name, selector);
            }
        }

        /// <summary>
        /// Configures custom sorter for a loader (<see cref="Loader{TEntity, TExecutionContext}"/>).
        /// If a loader has one configured custom sorter at least, an argument `sorting` is exposed for all GraphQL fields,
        /// which are configured using the loader (for example, by calling <c>Field("fieldName").FromLoader&lt;ModelLoader, Model&gt;()</c>).
        /// If a GraphQL query passes <paramref name="name"/> to a `sorting` argument of this field,
        /// then result will be sorted using a <paramref name="selectorFactory"/> expression.
        /// </summary>
        /// <typeparam name="TValueType">The type of the calculated value returned by the expression that is represented by <paramref name="selectorFactory"/>.</typeparam>
        /// <param name="name">A name of a field in a GraphQL argument `sorting`.</param>
        /// <param name="selectorFactory">
        /// A factory function, which returns an expression depending on an execution context.
        /// The expression is used to compute a value for sorting from a <typeparamref name="TEntity"/>.
        /// </param>
        protected internal void Sorter<TValueType>(string name, Func<TExecutionContext, Expression<Func<TEntity, TValueType>>> selectorFactory)
        {
            if (!IsConfiguringInputType)
            {
                ThrowIfIsNotConfiguring();
                Configurator.Sorter(name, selectorFactory);
            }
        }
    }
}
