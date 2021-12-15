﻿// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using System.Reflection;
using Epam.GraphQL.Sorters;
using GraphQL;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal interface IFieldExpression<TEntity, TReturnType, TExecutionContext> : IFieldResolver<TReturnType>, ISorter<TExecutionContext>
    {
        bool IsReadOnly { get; }

        PropertyInfo PropertyInfo { get; }

        public Expression<Func<TExecutionContext, TEntity, TReturnType>> Expression { get; }

        TReturnType Resolve(IResolveFieldContext context, object source);

        void ValidateExpression();
    }
}