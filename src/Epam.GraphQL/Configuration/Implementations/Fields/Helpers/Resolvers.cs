// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Relay;
using GraphQL;

namespace Epam.GraphQL.Configuration.Implementations.Fields.Helpers
{
    internal static class Resolvers
    {
        public static Connection<TReturnType> Resolve<TReturnType>(IResolveFieldContext context, IQueryable<TReturnType> children)
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

        public static Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TEntity, TReturnType, TExecutionContext>(
            string fieldName,
            Func<TExecutionContext, TEntity, TReturnType> func,
            IProxyAccessor<TEntity, TExecutionContext> proxyAccessor)
            where TEntity : class
        {
            proxyAccessor.AddMember(fieldName, FuncConstants<TEntity>.IdentityExpression);
            var converter = new Lazy<Func<Proxy<TEntity>, TEntity>>(() =>
                proxyAccessor.Rewrite(FuncConstants<TEntity>.IdentityExpression).Compile());

            return Resolver;

            TReturnType Resolver(IResolveFieldContext ctx)
            {
                var context = ctx.GetUserContext<TExecutionContext>();
                if (ctx.Source is Proxy<TEntity> proxy)
                {
                    return func(context, converter.Value(proxy));
                }

                return func(context, (TEntity)ctx.Source);
            }
        }

        public static Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TReturnType, TExecutionContext>(Func<TExecutionContext, TReturnType> func)
        {
            return ctx => func(ctx.GetUserContext<TExecutionContext>());
        }
    }
}
