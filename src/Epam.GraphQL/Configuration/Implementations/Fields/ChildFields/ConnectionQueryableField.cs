// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;
using Epam.GraphQL.Types;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ChildFields
{
#pragma warning disable CA1501
    internal class ConnectionQueryableField<TEntity, TReturnType, TExecutionContext> : OrderedQueryableField<TEntity, TReturnType, TExecutionContext>
#pragma warning restore CA1501
        where TEntity : class
    {
        private readonly IGraphTypeDescriptor<TExecutionContext> _graphType;

        public ConnectionQueryableField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Func<TExecutionContext, IQueryable<TReturnType>> query,
            Expression<Func<TEntity, TReturnType, bool>> condition,
            IObjectGraphTypeConfigurator<TReturnType, TExecutionContext> configurator,
            LazyQueryArguments arguments,
            Func<IQueryable<TReturnType>, IOrderedQueryable<TReturnType>> orderBy,
            Func<IOrderedQueryable<TReturnType>, IOrderedQueryable<TReturnType>> thenBy)
            : base(
                  registry,
                  parent,
                  name,
                  null, // TODO elementGraphType was typeof(ConnectionGraphType<TChildLoader, TChildEntity, TExecutionContext>),
                  configurator,
                  arguments,
                  CreateResolver(name, parent, query, condition, configurator, orderBy, thenBy),
                  orderBy,
                  thenBy)
        {
            _graphType = new GraphTypeDescriptor<TReturnType, TExecutionContext>(
                type: null,
                graphTypeFactory: () => new ConnectionGraphType<TReturnType, TExecutionContext>(configurator),
                configurator);

            Argument<string>(
                "after",
                "Only look at connected edges with cursors greater than the value of `after`.");

            Argument<int?>(
                "first",
                "Specifies the number of edges to return starting from `after` or the first entry if `after` is not specified.");

            Argument<string>(
                "before",
                "Only look at connected edges with cursors smaller than the value of `before`.");

            Argument<int?>(
                "last",
                "Specifies the number of edges to return counting reversely from `before`, or the last entry if `before` is not specified.");
        }

        public override IGraphTypeDescriptor<TExecutionContext> GraphType => _graphType;

        public override IResolver<TEntity> FieldResolver => OrderedQueryableFieldResolver.AsConnection();

        private static IOrderedQueryableResolver<TEntity, TReturnType, TExecutionContext> CreateResolver(
            string fieldName,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            Func<TExecutionContext, IQueryable<TReturnType>> query,
            Expression<Func<TEntity, TReturnType, bool>> condition,
            IObjectGraphTypeConfigurator<TReturnType, TExecutionContext> configurator,
            Func<IQueryable<TReturnType>, IOrderedQueryable<TReturnType>> orderBy,
            Func<IOrderedQueryable<TReturnType>, IOrderedQueryable<TReturnType>> thenBy)
        {
            return condition == null
                ? new ConnectionFuncResolver<TEntity, TReturnType, TExecutionContext>(configurator.ProxyAccessor, GetQuery(configurator, query), (ctx, query) => query, ApplySort(configurator, null, orderBy, thenBy))
                : new ConnectionAsyncFuncResolver<TEntity, TReturnType, TExecutionContext>(fieldName, GetQuery(configurator, query), condition, (ctx, query) => query, ApplySort(configurator, null, orderBy, thenBy), parent.ProxyAccessor, configurator.ProxyAccessor);
        }
    }
}
