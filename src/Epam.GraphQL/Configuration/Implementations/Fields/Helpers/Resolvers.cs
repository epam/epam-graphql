// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Relay;
using GraphQL;
using DataObjects = GraphQL.Types.Relay.DataObjects;

namespace Epam.GraphQL.Configuration.Implementations.Fields.Helpers
{
    internal static class Resolvers
    {
        public static Connection<Proxy<TReturnType>> Resolve<TReturnType>(IResolveFieldContext context, IOrderedQueryable<Proxy<TReturnType>> children)
        {
            var first = context.GetFirst();
            var last = context.GetLast();
            var after = context.GetAfter();
            var before = context.GetBefore();

            var shouldComputeCount = context.HasTotalCount();
            var shouldComputeEndOffset = context.HasEndCursor();
            var shouldComputeEdges = context.HasEdges();
            var shouldComputeItems = context.HasItems();

            var connection = ConnectionUtils.ToConnection(
                children,
                () => context.GetPath(),
                context.GetQueryExecuter(),
                first,
                last,
                before,
                after,
                shouldComputeCount,
                shouldComputeEndOffset,
                shouldComputeEdges,
                shouldComputeItems);

            return connection;
        }

        public static Func<IResolveFieldContext, IOrderedQueryable<Proxy<TReturnType>>, Connection<Proxy<TReturnType>>> ToConnection<TChildEntity, TReturnType, TExecutionContext>(IProxyAccessor<TReturnType, TExecutionContext> proxyAccessor)
        {
            return (context, children) =>
            {
                var connection = Resolve(context, children);

                var executer = proxyAccessor.CreateHooksExecuter(context.GetUserContext<TExecutionContext>());

                if (connection.Items != null)
                {
                    connection.Items = executer.ExecuteHooks(connection.Items);
                }
                else if (connection.Edges != null)
                {
                    connection.Edges = executer.ExecuteHooks(connection.Edges);
                }

                return connection;
            };
        }

        public static Func<IResolveFieldContext, IOrderedQueryable<Proxy<TChildEntity>>, Connection<object>> ToGroupConnection<TChildEntity, TExecutionContext>(IProxyAccessor<TChildEntity, TExecutionContext> proxyAccessor)
        {
            return (context, children) =>
            {
                var subFields = context.GetGroupConnectionQueriedFields();
                var aggregateQueriedFields = context.GetGroupConnectionAggregateQueriedFields().Select(name => $"<>${name}");

                var sourceType = proxyAccessor.GetConcreteProxyType(subFields.Concat(aggregateQueriedFields));

                var first = context.GetFirst();
                var last = context.GetLast();
                var after = context.GetAfter();
                var before = context.GetBefore();

                var shouldComputeCount = context.HasTotalCount();
                var shouldComputeEndOffset = context.HasEndCursor();
                var shouldComputeEdges = context.HasEdges();
                var shouldComputeItems = context.HasItems();

                IOrderedQueryable<object> items;
                if (aggregateQueriedFields.Contains("<>$count"))
                {
                    var propGetter = sourceType.GetPropertyDelegate("<>$count");
                    Func<object, int> countGetter = entity => (int)propGetter(entity);
                    items = (IOrderedQueryable<object>)children.SafeNull().AsQueryable().Select(entity => new GroupResult<Proxy<TChildEntity>>
                    {
                        Item = entity,
                        Count = countGetter(entity),
                    });
                }
                else
                {
                    items = (IOrderedQueryable<object>)children.SafeNull().AsQueryable().Select(entity => new GroupResult<Proxy<TChildEntity>>
                    {
                        Item = entity,
                    });
                }

                return ConnectionUtils.ToConnection(
                    items,
                    () => context.GetPath(),
                    context.GetQueryExecuter(),
                    first,
                    last,
                    before,
                    after,
                    shouldComputeCount,
                    shouldComputeEndOffset,
                    shouldComputeEdges,
                    shouldComputeItems);
            };
        }

        public static Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TEntity, TReturnType, TExecutionContext>(Func<TExecutionContext, TEntity, TReturnType> func, bool doesDependOnAllFields)
            where TEntity : class
        {
            if (doesDependOnAllFields)
            {
                return ctx => func(ctx.GetUserContext<TExecutionContext>(), ctx.Source is Proxy<TEntity> proxy ? proxy.GetOriginal() : (TEntity)ctx.Source);
            }

            return ctx => func(ctx.GetUserContext<TExecutionContext>(), null);
        }

        public static IEnumerable<T> ExecuteHooks<T>(this ILoaderHooksExecuter<T> executer, IEnumerable<T> items)
        {
            if (executer == null)
            {
                return items;
            }

            return Impl(executer, items);

            static IEnumerable<T> Impl(ILoaderHooksExecuter<T> executer, IEnumerable<T> items)
            {
                foreach (var item in items)
                {
                    executer.Execute(item);
                    yield return item;
                }
            }
        }

        public static T ExecuteHooks<T>(this ILoaderHooksExecuter<T> executer, T item)
        {
            if (executer != null)
            {
                executer.Execute(item);
            }

            return item;
        }

        private static IEnumerable<DataObjects.Edge<T>> ExecuteHooks<T>(this ILoaderHooksExecuter<T> executer, IEnumerable<DataObjects.Edge<T>> items)
        {
            if (executer == null)
            {
                return items;
            }

            return Impl(executer, items);

            static IEnumerable<DataObjects.Edge<T>> Impl(ILoaderHooksExecuter<T> executer, IEnumerable<DataObjects.Edge<T>> items)
            {
                foreach (var edge in items)
                {
                    executer.Execute(edge.Node);
                    yield return edge;
                }
            }
        }
    }
}
