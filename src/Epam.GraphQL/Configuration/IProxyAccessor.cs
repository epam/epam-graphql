// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Relay;
using Epam.GraphQL.Sorters;
using GraphQL;

namespace Epam.GraphQL.Configuration
{
    internal interface IProxyAccessor<TEntity, TTransformedEntity, TExecutionContext>
    {
        bool HasHooks { get; }

        Expression<Func<TTransformedEntity, T>> Rewrite<T>(Expression<Func<TEntity, T>> originalExpression);

        LambdaExpression Rewrite(LambdaExpression expression, LambdaExpression originalExpression);

        IReadOnlyList<(LambdaExpression SortExpression, SortDirection SortDirection)> GetGroupSort(
            IResolveFieldContext context,
            IEnumerable<ISorter<TExecutionContext>> sorters);

        Expression<Func<TExecutionContext, TEntity, TTransformedEntity>> CreateSelectorExpression(IEnumerable<string> fieldNames);

        IQueryable<IGroupResult<TTransformedEntity>> GroupBy(IResolveFieldContext context, IQueryable<TEntity> query);

        ILoaderHooksExecuter<TTransformedEntity>? CreateHooksExecuter(IResolveFieldContext context);

        void AddMembers(IEnumerable<LambdaExpression> members);

        void AddMember<TResult>(Expression<Func<TEntity, TResult>> member);

        void RemoveMember<TResult>(Expression<Func<TEntity, TResult>> member);
    }

    internal interface IProxyAccessor<TEntity, TExecutionContext> : IProxyAccessor<TEntity, Proxy<TEntity>, TExecutionContext>
    {
        void AddMember<TResult>(string childFieldName, Expression<Func<TEntity, TResult>> member);

        void AddMembers<TChildEntity, TTransformedChildEntity>(
            string childFieldName,
            IProxyAccessor<TChildEntity, TTransformedChildEntity, TExecutionContext>? childProxyAccessor,
            ExpressionFactorizationResult factorizationResult);

        void AddMembers(string childFieldName, IEnumerable<LambdaExpression> members);
    }
}
