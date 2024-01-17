// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Builders.Loader
{
    public interface IInlineObjectBuilder<TSourceType, TExecutionContext>
    {
        string Name { get; set; }

        IExpressionField<TSourceType, TReturnType, TExecutionContext> Field<TReturnType>(Expression<Func<TSourceType, TReturnType>> expression, string? deprecationReason = null);

        IExpressionField<TSourceType, TReturnType, TExecutionContext> Field<TReturnType>(string name, Expression<Func<TSourceType, TReturnType>> expression, string? deprecationReason = null);

        IExpressionField<TSourceType, TReturnType, TExecutionContext> Field<TReturnType>(string name, Expression<Func<TExecutionContext, TSourceType, TReturnType>> expression, string? deprecationReason = null);

        IVoid Field<TReturnType>(Expression<Func<TSourceType, IEnumerable<TReturnType>>> expression, string? deprecationReason = null);

        IVoid Field<TReturnType>(string name, Expression<Func<TSourceType, IEnumerable<TReturnType>>> expression, string? deprecationReason = null);

        IVoid Field<TReturnType>(string name, Expression<Func<TExecutionContext, TSourceType, IEnumerable<TReturnType>>> expression, string? deprecationReason = null);

        IInlineObjectFieldBuilder<TSourceType, TExecutionContext> Field(string name, string? deprecationReason = null);

        void OnEntityLoaded<T>(
            Expression<Func<TSourceType, T>> expression,
            Action<TExecutionContext, T> action);

        void OnEntityLoaded<TKey, T>(
            Expression<Func<TSourceType, TKey>> keyExpression,
            Func<TExecutionContext, IEnumerable<TKey>, IDictionary<TKey, T>> fetch,
            Action<TExecutionContext, T> action);

        void OnEntityLoaded<TKey, T>(
            Expression<Func<TSourceType, TKey>> keyExpression,
            Func<TExecutionContext, IEnumerable<TKey>, Task<IDictionary<TKey, T>>> fetch,
            Action<TExecutionContext, T> action);

        void ConfigureFrom<TProjection>()
            where TProjection : Projection<TSourceType, TExecutionContext>;
    }
}
