// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Epam.GraphQL.Builders.Common;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Builders.Loader
{
    public interface IInlineObjectBuilder<TSourceType, TExecutionContext>
        where TSourceType : class
    {
        string Name { get; set; }

        IHasFilterableAndSortable<TSourceType, TReturnType> Field<TReturnType>(Expression<Func<TSourceType, TReturnType>> expression, string? deprecationReason = null)
            where TReturnType : struct;

        IHasFilterableAndSortable<TSourceType, TReturnType> Field<TReturnType>(string name, Expression<Func<TSourceType, TReturnType>> expression, string? deprecationReason = null)
            where TReturnType : struct;

        IHasFilterableAndSortable<TSourceType, TReturnType> Field<TReturnType>(string name, Expression<Func<TExecutionContext, TSourceType, TReturnType>> expression, string? deprecationReason = null)
            where TReturnType : struct;

        IHasFilterableAndSortable<TSourceType, TReturnType> Field<TReturnType>(Expression<Func<TSourceType, TReturnType?>> expression, string? deprecationReason = null)
            where TReturnType : struct;

        IHasFilterableAndSortable<TSourceType, TReturnType> Field<TReturnType>(string name, Expression<Func<TSourceType, TReturnType?>> expression, string? deprecationReason = null)
            where TReturnType : struct;

        IHasFilterableAndSortable<TSourceType, TReturnType> Field<TReturnType>(string name, Expression<Func<TExecutionContext, TSourceType, TReturnType?>> expression, string? deprecationReason = null)
            where TReturnType : struct;

        IHasFilterableAndSortable<TSourceType, string> Field(Expression<Func<TSourceType, string>> expression, string? deprecationReason = null);

        IHasFilterableAndSortable<TSourceType, string> Field(string name, Expression<Func<TSourceType, string>> expression, string? deprecationReason = null);

        IHasFilterableAndSortable<TSourceType, string> Field(string name, Expression<Func<TExecutionContext, TSourceType, string>> expression, string? deprecationReason = null);

        void Field<TReturnType>(Expression<Func<TSourceType, IEnumerable<TReturnType>>> expression, string? deprecationReason = null)
            where TReturnType : struct;

        void Field<TReturnType>(string name, Expression<Func<TSourceType, IEnumerable<TReturnType>>> expression, string? deprecationReason = null)
            where TReturnType : struct;

        void Field<TReturnType>(string name, Expression<Func<TExecutionContext, TSourceType, IEnumerable<TReturnType>>> expression, string? deprecationReason = null)
            where TReturnType : struct;

        void Field<TReturnType>(Expression<Func<TSourceType, IEnumerable<TReturnType?>>> expression, string? deprecationReason = null)
            where TReturnType : struct;

        void Field<TReturnType>(string name, Expression<Func<TSourceType, IEnumerable<TReturnType?>>> expression, string? deprecationReason = null)
            where TReturnType : struct;

        void Field<TReturnType>(string name, Expression<Func<TExecutionContext, TSourceType, IEnumerable<TReturnType?>>> expression, string? deprecationReason = null)
            where TReturnType : struct;

        void Field(Expression<Func<TSourceType, IEnumerable<string>>> expression, string? deprecationReason = null);

        void Field(string name, Expression<Func<TSourceType, IEnumerable<string>>> expression, string? deprecationReason = null);

        void Field(string name, Expression<Func<TExecutionContext, TSourceType, IEnumerable<string>>> expression, string? deprecationReason = null);

        IInlineObjectFieldBuilder<TSourceType, TExecutionContext> Field(string name, string? deprecationReason = null);

        void OnEntityLoaded<T>(Expression<Func<TSourceType, T>> proxyExpression, Action<TExecutionContext, T> hook);

        void ConfigureFrom<TProjection>()
            where TProjection : Projection<TSourceType, TExecutionContext>;

        void ConfigureFrom(Type loaderType);
    }
}
