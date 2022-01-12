// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Builders.Loader.Implementations;
using Epam.GraphQL.Configuration.Enums;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using GraphQL;
using GraphQL.DataLoader;

namespace Epam.GraphQL.Loaders
{
    public abstract class Loader<TEntity, TExecutionContext> : Projection<TEntity, TExecutionContext>, ILoader<TEntity, TExecutionContext>
        where TEntity : class
    {
        protected internal NullOption NullValues => NullOption.NullValues;

        protected internal NullOption NotNullValues => NullOption.NotNullValues;

        public IQueryable<TEntity> All(TExecutionContext context)
        {
            var query = DoGetBaseQuery(context);
            query = DoApplySecurityFilter(context, query);
            return query;
        }

        public virtual IOrderedQueryable<TEntity> ApplyNaturalOrderBy(IQueryable<TEntity> query)
        {
            throw new NotImplementedException($"You must override {nameof(ApplyNaturalOrderBy)} method or pass order expression to Connection()/AsConnection() call.");
        }

        public virtual IOrderedQueryable<TEntity> ApplyNaturalThenBy(IOrderedQueryable<TEntity> query)
        {
            throw new NotImplementedException($"You must override {nameof(ApplyNaturalThenBy)} method or pass order expression to Connection()/AsConnection() call.");
        }

        internal IQueryable<TEntity> DoGetBaseQuery(TExecutionContext context) => GetBaseQuery(context);

        internal IQueryable<TEntity> DoApplySecurityFilter(TExecutionContext context, IQueryable<TEntity> query)
        {
            var result = ApplySecurityFilter(context, query);
            if (!QueryableChecker.IsSubExpression(result, query))
            {
                throw new ExecutionError($"{GetType().HumanizedName()}.{nameof(ApplySecurityFilter)}:  You cannot query data from anywhere (e.g. using context), please use passed `{nameof(query)}` parameter.");
            }

            return result;
        }

        internal IDataLoaderResult<bool> CanViewAsync(GraphQLContext<TExecutionContext> context, TEntity entity)
        {
            if (!DoApplySecurityFilter(context.ExecutionContext, Enumerable.Repeat(entity, 1).AsQueryable()).Contains(entity))
            {
                return new DataLoaderResult<bool>(false);
            }

            return new DataLoaderResult<bool>(Registry.CanViewParentAsync(GetType(), context, entity));
        }

        protected internal new ILoaderFieldBuilder<TEntity, TExecutionContext> Field(string name, string? deprecationReason = null)
        {
            var fieldType = AddField(name, deprecationReason);
            var fieldBuilderType = typeof(LoaderFieldBuilder<,,>).MakeGenericType(typeof(TEntity), GetType(), typeof(TExecutionContext));
            return (ILoaderFieldBuilder<TEntity, TExecutionContext>)fieldBuilderType.CreateInstanceAndHoistBaseException(Registry, fieldType);
        }

        protected internal void OnEntityLoaded<T>(Expression<Func<TEntity, T>> proxyExpression, Action<TExecutionContext, T> hook) =>
            AddOnEntityLoaded(proxyExpression, hook);

        protected abstract IQueryable<TEntity> GetBaseQuery(TExecutionContext context);

        protected virtual IQueryable<TEntity> ApplySecurityFilter(TExecutionContext context, IQueryable<TEntity> query) => query;
    }
}
