// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Diagnostics;
using GraphQL;

namespace Epam.GraphQL.Sorters.Implementations
{
    internal class CustomSorter<TEntity, TValueType, TExecutionContext> : ISorter<TExecutionContext>, IChainConfigurationContextOwner
    {
        private readonly Func<TExecutionContext, Expression<Func<TEntity, TValueType>>> _selectorFactory;

        public CustomSorter(
            Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory,
            string name,
            Expression<Func<TEntity, TValueType>> selector)
            : this(configurationContextFactory, name, _ => selector)
        {
        }

        public CustomSorter(
            Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory,
            string name,
            Func<TExecutionContext, Expression<Func<TEntity, TValueType>>> selectorFactory)
        {
            ConfigurationContext = configurationContextFactory(this);
            Name = name.ToCamelCase();
            _selectorFactory = selectorFactory;
        }

        public IChainConfigurationContext ConfigurationContext { get; set; }

        public string Name { get; }

        public bool IsGroupable => false;

        public LambdaExpression BuildExpression(TExecutionContext context) => _selectorFactory(context);

        public LambdaExpression BuildOriginalExpression(TExecutionContext context) => _selectorFactory(context);
    }
}
