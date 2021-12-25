// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Epam.GraphQL.Helpers;

#nullable enable

namespace Epam.GraphQL.Configuration
{
    internal interface IProxyAccessor<TExecutionContext>
    {
        bool HasHooks { get; }

        Type ProxyType { get; }

        Type GetConcreteProxyType(IEnumerable<string> fieldNames);

        LambdaExpression GetProxyExpression(LambdaExpression originalExpression);

        void Configure();

        LambdaExpression CreateGroupSelectorExpression(Type entityType, IEnumerable<string> fieldNames);

        LambdaExpression CreateGroupKeySelectorExpression(IEnumerable<string> subFields, IEnumerable<string> aggregateQueriedFields);
    }

    internal interface IProxyAccessor<TEntity, TExecutionContext> : IProxyAccessor<TExecutionContext>
    {
        IReadOnlyList<IField<TEntity, TExecutionContext>> Fields { get; }

        Expression<Func<TExecutionContext, TEntity, Proxy<TEntity>>> CreateSelectorExpression(IEnumerable<string> fieldNames);

        ILoaderHooksExecuter<Proxy<TEntity>>? CreateHooksExecuter(TExecutionContext executionContext);

        void AddMember<TResult>(string childFieldName, Expression<Func<TEntity, TResult>> member);

        void AddMember<TResult>(Expression<Func<TEntity, TResult>> member);

        void RemoveMember<TResult>(Expression<Func<TEntity, TResult>> member);

        void AddMembers<TChildEntity>(string childFieldName, IProxyAccessor<TChildEntity, TExecutionContext> childProxyAccessor, ExpressionFactorizationResult factorizationResult);

        void AddAllMembers(string childFieldName);
    }
}
