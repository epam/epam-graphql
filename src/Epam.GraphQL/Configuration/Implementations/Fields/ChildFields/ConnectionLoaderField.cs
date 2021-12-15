// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Types;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ChildFields
{
#pragma warning disable CA1501
    internal class ConnectionLoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext> : OrderedLoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>
#pragma warning restore CA1501
        where TEntity : class
        where TChildEntity : class
        where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
    {
        private readonly IGraphTypeDescriptor<TExecutionContext> _graphType;

        public ConnectionLoaderField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            LazyQueryArguments arguments,
            Func<IQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>> orderBy,
            Func<IOrderedQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>> thenBy)
            : this(
                  registry,
                  parent,
                  name,
                  condition,
                  registry.ResolveLoader<TChildLoader, TChildEntity>(),
                  arguments,
                  orderBy,
                  thenBy)
        {
        }

        private ConnectionLoaderField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            TChildLoader loader,
            LazyQueryArguments arguments,
            Func<IQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>> orderBy,
            Func<IOrderedQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>> thenBy)
            : base(
                  registry,
                  parent,
                  name,
                  condition,
                  loader,
                  null, // TODO elementGraphType was typeof(ConnectionGraphType<TChildLoader, TChildEntity, TExecutionContext>),
                  CreateResolver(name, parent, condition, loader, orderBy, thenBy),
                  arguments,
                  orderBy,
                  thenBy)
        {
            _graphType = GraphTypeDescriptor.Create<ConnectionGraphType<TChildLoader, TChildEntity, TExecutionContext>, TExecutionContext>();

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

        private static IOrderedQueryableResolver<TEntity, TChildEntity, TExecutionContext> CreateResolver(
            string fieldName,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            TChildLoader loader,
            Func<IQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>> orderBy,
            Func<IOrderedQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>> thenBy)
        {
            return condition == null
                ? new ConnectionFuncResolver<TEntity, TChildEntity, TExecutionContext>(loader.ObjectGraphTypeConfigurator.ProxyAccessor, GetQuery(loader), (ctx, query) => loader.DoApplySecurityFilter(ctx.GetUserContext<TExecutionContext>(), query), ApplySort(loader, null, orderBy, thenBy))
                : new ConnectionAsyncFuncResolver<TEntity, TChildEntity, TExecutionContext>(fieldName, GetQuery(loader), condition, (ctx, query) => loader.DoApplySecurityFilter(ctx.GetUserContext<TExecutionContext>(), query), ApplySort(loader, null, orderBy, thenBy), parent.ProxyAccessor, loader.ObjectGraphTypeConfigurator.ProxyAccessor);
        }
    }
}
