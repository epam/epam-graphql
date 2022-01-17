// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Builders.Common;
using Epam.GraphQL.Builders.Common.Implementations;
using Epam.GraphQL.Builders.Query;
using Epam.GraphQL.Builders.Query.Implementations;
using Epam.GraphQL.Configuration.Implementations.Fields;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL
{
    public abstract class Query<TExecutionContext> : RootProjection<TExecutionContext>
    {
        protected internal new IQueryFieldBuilder<TExecutionContext> Field(string name, string? deprecationReason = null)
        {
            ThrowIfIsNotConfiguring();
            var field = Configurator.AddField(new QueryField<TExecutionContext>(Registry, Configurator, name), deprecationReason);
            return new QueryFieldBuilder<QueryField<TExecutionContext>, TExecutionContext>(field);
        }

        protected internal IConnectionBuilder Connection<TChildLoader>(string name, string? deprecationReason = null)
            where TChildLoader : class
        {
            return Connection(typeof(TChildLoader), name, deprecationReason);
        }

        protected internal IConnectionBuilder Connection(Type childLoaderType, string name, string? deprecationReason = null)
        {
            var baseLoaderType = TypeHelpers.FindMatchingGenericBaseType(childLoaderType, typeof(Loader<,>));

            if (baseLoaderType == null)
            {
                throw new ArgumentException($"Cannot find the corresponding base type for loader: {childLoaderType}");
            }

            var childEntityType = baseLoaderType.GetGenericArguments().First();

            var addConnectionLoaderFieldMethodInfo = ObjectGraphTypeConfigurator.GetType().GetPublicGenericMethod(
                nameof(ObjectGraphTypeConfigurator.AddConnectionLoaderField),
                new[] { childLoaderType, childEntityType },
                new[] { typeof(string), typeof(string) });

            var field = addConnectionLoaderFieldMethodInfo.InvokeAndHoistBaseException(
                ObjectGraphTypeConfigurator,
                name,
                deprecationReason);

            var projectionBuilder = typeof(ConnectionBuilder<,,>)
                .MakeGenericType(typeof(object), childEntityType, typeof(TExecutionContext));

            return (IConnectionBuilder)projectionBuilder.CreateInstanceAndHoistBaseException(field);
        }

        protected internal IConnectionBuilder Connection<TChildLoader, TEntity>(string name, Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>> order, string? deprecationReason = null)
            where TChildLoader : class
        {
            return Connection(typeof(TChildLoader), name, order, deprecationReason);
        }

        protected internal IConnectionBuilder Connection<TEntity>(Type childLoaderType, string name, Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>> order, string? deprecationReason = null)
        {
            var baseLoaderType = TypeHelpers.FindMatchingGenericBaseType(childLoaderType, typeof(Loader<,>));

            if (baseLoaderType == null)
            {
                throw new ArgumentException($"Cannot find the corresponding base type for loader: {childLoaderType}", nameof(childLoaderType));
            }

            var childEntityType = baseLoaderType.GetGenericArguments().First();

            if (childEntityType != typeof(TEntity))
            {
                throw new ArgumentException($"Entity type mismatch: expected {typeof(TEntity)}, but found {childLoaderType}", nameof(childLoaderType));
            }

            var queryableType = typeof(IQueryable<>).MakeGenericType(childEntityType);
            var orderedQueryableType = typeof(IOrderedQueryable<>).MakeGenericType(childEntityType);
            var orderFuncType = typeof(Func<,>).MakeGenericType(queryableType, orderedQueryableType);
            var orderType = typeof(Expression<>).MakeGenericType(orderFuncType);

            var addConnectionLoaderFieldMethodInfo = ObjectGraphTypeConfigurator.GetType().GetPublicGenericMethod(
                nameof(ObjectGraphTypeConfigurator.AddConnectionLoaderField),
                new[] { childLoaderType, childEntityType },
                new[] { typeof(string), orderType, typeof(string) });

            var field = addConnectionLoaderFieldMethodInfo.InvokeAndHoistBaseException(
                ObjectGraphTypeConfigurator,
                name,
                order,
                deprecationReason);

            var projectionBuilder = typeof(ConnectionBuilder<,,>)
                .MakeGenericType(typeof(object), childEntityType, typeof(TExecutionContext));

            return (IConnectionBuilder)projectionBuilder.CreateInstanceAndHoistBaseException(field);
        }

        protected internal IConnectionBuilder GroupConnection<TChildLoader>(string name, string? deprecationReason = null)
            where TChildLoader : class => GroupConnection(typeof(TChildLoader), name, deprecationReason);

        protected internal IConnectionBuilder GroupConnection(Type childLoaderType, string name, string? deprecationReason = null)
        {
            var baseLoaderType = TypeHelpers.FindMatchingGenericBaseType(childLoaderType, typeof(Loader<,>));

            if (baseLoaderType == null)
            {
                throw new ArgumentException($"Cannot find the corresponding base type for loader: {childLoaderType}");
            }

            var childEntityType = baseLoaderType.GetGenericArguments().First();

            var addGroupConnectionLoaderFieldMethodInfo = ObjectGraphTypeConfigurator.GetType().GetGenericMethod(
                nameof(ObjectGraphTypeConfigurator.AddGroupLoaderField),
                new[] { childLoaderType, childEntityType },
                new[] { typeof(string), typeof(string) });

            var field = addGroupConnectionLoaderFieldMethodInfo.InvokeAndHoistBaseException(
                ObjectGraphTypeConfigurator,
                name,
                deprecationReason);

            var projectionBuilder = typeof(ConnectionBuilder<,,>)
                .MakeGenericType(typeof(object), childEntityType, typeof(TExecutionContext));

            return (IConnectionBuilder)projectionBuilder.CreateInstanceAndHoistBaseException(field);
        }
    }
}
