// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.TaskBatcher;
using GraphQL;
using GraphQL.DataLoader;

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal class BatchResolver<TEntity, TReturnType, TExecutionContext> : BatchResolverBase<TEntity, TReturnType>
        where TEntity : class
    {
        private static readonly Func<Proxy<TEntity>, TEntity> _proxyKeySelector = p => p.GetOriginal();

        public BatchResolver(
            string fieldName,
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc,
            ProxyAccessor<TEntity, TExecutionContext> proxyAccessor)
            : base(CreateResolver(batchFunc), CreateProxiedResolver(batchFunc, proxyAccessor))
        {
            proxyAccessor.AddAllMembers(fieldName);
        }

        private static Func<IResolveFieldContext, IDataLoader<TEntity, TReturnType?>> CreateResolver(
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc)
        {
            return context =>
            {
                var batcher = context.GetBatcher();

                var result = FuncConstants<TEntity>.Identity.Then(
                    FuncConstants<TEntity>.IsNull,
                    BatchLoader.FromResult(FuncConstants<TEntity, TReturnType?>.DefaultResultFunc),
                    batcher.Get(context.GetPath, context.GetUserContext<TExecutionContext>(), batchFunc));

                return result;
            };
        }

        private static Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, TReturnType?>> CreateProxiedResolver(
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc,
            IProxyAccessor<TEntity, TExecutionContext> proxyAccessor)
        {
            if (proxyAccessor == null)
            {
                throw new ArgumentNullException(nameof(proxyAccessor));
            }

            return context =>
            {
                var batcher = context.GetBatcher();

                var result = _proxyKeySelector.Then(
                    FuncConstants<TEntity>.IsNull,
                    BatchLoader.FromResult(FuncConstants<TEntity, TReturnType?>.DefaultResultFunc),
                    batcher.Get(context.GetPath, context.GetUserContext<TExecutionContext>(), batchFunc));

                return result;
            };
        }
    }
}
