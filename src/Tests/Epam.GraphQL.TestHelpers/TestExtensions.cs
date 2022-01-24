// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Builders.Query;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Extensions;

namespace Epam.GraphQL.Tests
{
    public static class TestExtensions
    {
        public static ILoaderField<TChildEntity, TExecutionContext> FromLoader<TChildEntity, TExecutionContext>(this IQueryFieldBuilder<TExecutionContext> builder, Type loaderType)
        {
            var methodInfo = typeof(IQueryFieldBuilder<TExecutionContext>).GetPublicGenericMethod(
                nameof(IQueryFieldBuilder<TExecutionContext>.FromLoader),
                new[] { loaderType, typeof(TChildEntity) },
                Type.EmptyTypes);

            return methodInfo.InvokeAndHoistBaseException<ILoaderField<TChildEntity, TExecutionContext>>(builder);
        }

        public static void WithSearch<TThis, TChildEntity, TExecutionContext>(this ISearchableField<TThis, TChildEntity, TExecutionContext> field, Type searcherType)
        {
            var methodInfo = typeof(ISearchableField<TThis, TChildEntity, TExecutionContext>).GetPublicGenericMethod(
                nameof(ISearchableField<TThis, TChildEntity, TExecutionContext>.WithSearch),
                new[] { searcherType },
                Type.EmptyTypes);

            methodInfo.InvokeAndHoistBaseException(field);
        }

        public static IConnectionField WithFilter(this IConnectionField field, Type filterType)
        {
            var methodInfo = typeof(IConnectionField).GetPublicGenericMethod(
                nameof(IConnectionField.WithFilter),
                new[] { filterType },
                Type.EmptyTypes);

            return methodInfo.InvokeAndHoistBaseException<IConnectionField>(field);
        }

        public static IConnectionField WithSearch(this IConnectionField field, Type searcherType)
        {
            var methodInfo = typeof(IConnectionField).GetPublicGenericMethod(
                nameof(IConnectionField.WithSearch),
                new[] { searcherType },
                Type.EmptyTypes);

            return methodInfo.InvokeAndHoistBaseException<IConnectionField>(field);
        }

        public static IConnectionField Connection<TExecutionContext>(this Query<TExecutionContext> query, Type loaderType, string fieldName)
        {
            var methodInfo = typeof(Query<TExecutionContext>).GetNonPublicGenericMethod(
                nameof(Query<TExecutionContext>.Connection),
                new[] { loaderType },
                new[] { typeof(string), typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IConnectionField>(query, fieldName, null);
        }

        public static IConnectionField Connection<TEntity, TExecutionContext>(this Query<TExecutionContext> query, Type loaderType, string fieldName, Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>> order)
        {
            var methodInfo = typeof(Query<TExecutionContext>).GetNonPublicGenericMethod(
                nameof(Query<TExecutionContext>.Connection),
                new[] { loaderType, typeof(TEntity) },
                new[] { typeof(string), typeof(Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>>), typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IConnectionField>(query, fieldName, order, null);
        }

        public static IConnectionField GroupConnection<TExecutionContext>(this Query<TExecutionContext> query, Type loaderType, string fieldName)
        {
            var methodInfo = typeof(Query<TExecutionContext>).GetNonPublicGenericMethod(
                nameof(Query<TExecutionContext>.GroupConnection),
                new[] { loaderType },
                new[] { typeof(string), typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IConnectionField>(query, fieldName, null);
        }
    }
}
