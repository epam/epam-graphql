// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.TaskBatcher;
using GraphQL;
using GraphQL.DataLoader;

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal class BatchTaskKeyResolver<TEntity, TKey, TReturnType, TExecutionContext> : BatchResolverBase<TEntity, TReturnType>
        where TEntity : class
    {
        public BatchTaskKeyResolver(
            string fieldName,
            Expression<Func<TEntity, TKey>> keySelector,
            Func<TExecutionContext, IEnumerable<TKey>, Task<IDictionary<TKey, TReturnType>>> batchFunc,
            ProxyAccessor<TEntity, TExecutionContext> proxyAccessor)
            : base(CreateResolver(keySelector, batchFunc), CreateProxiedResolver(keySelector, batchFunc, proxyAccessor))
        {
            proxyAccessor.AddMember(fieldName, keySelector);
        }

        private static Func<IResolveFieldContext, IDataLoader<TEntity, TReturnType?>> CreateResolver(
            Expression<Func<TEntity, TKey>> keySelector,
            Func<TExecutionContext, IEnumerable<TKey>, Task<IDictionary<TKey, TReturnType>>> batchFunc)
        {
            var compiledKeySelector = keySelector.Compile();

            return context =>
            {
                var batcher = context.GetBatcher();

                var result = compiledKeySelector.Then(
                    FuncConstants<TKey>.IsNull,
                    BatchLoader.FromResult(FuncConstants<TKey, TReturnType?>.DefaultResultFunc),
                    batcher.Get(context.GetPath, context.GetUserContext<TExecutionContext>(), batchFunc));

                return result;
            };
        }

        private static Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, TReturnType?>> CreateProxiedResolver(
            Expression<Func<TEntity, TKey>> keySelector,
            Func<TExecutionContext, IEnumerable<TKey>, Task<IDictionary<TKey, TReturnType>>> batchFunc,
            IProxyAccessor<TEntity, TExecutionContext> proxyAccessor)
        {
            var compiledProxyKeySelector = new Lazy<Func<Proxy<TEntity>, TKey>>(() =>
            {
                var proxyKeySelector = proxyAccessor.Rewrite(keySelector);
                return proxyKeySelector.Compile();
            });

            return context =>
            {
                var batcher = context.GetBatcher();

                var result = compiledProxyKeySelector.Value.Then(
                    FuncConstants<TKey>.IsNull,
                    BatchLoader.FromResult(FuncConstants<TKey, TReturnType?>.DefaultResultFunc),
                    batcher.Get(context.GetPath, context.GetUserContext<TExecutionContext>(), batchFunc));

                return result;
            };
        }
    }
}
