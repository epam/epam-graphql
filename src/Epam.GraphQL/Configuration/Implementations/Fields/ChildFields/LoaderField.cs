// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;
using Epam.GraphQL.Enums;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Loaders;
using GraphQL;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ChildFields
{
    internal class LoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext> : QueryableField<TEntity, TChildEntity, TExecutionContext>
        where TEntity : class
        where TChildEntity : class
        where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
    {
        private readonly Expression<Func<TEntity, TChildEntity, bool>> _condition;

        public LoaderField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType,
            LazyQueryArguments arguments = null)
            : this(
                  registry,
                  parent,
                  name,
                  condition,
                  registry.ResolveLoader<TChildLoader, TChildEntity>(),
                  elementGraphType,
                  arguments)
        {
        }

        protected LoaderField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            TChildLoader loader,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType,
            IQueryableResolver<TEntity, TChildEntity, TExecutionContext> resolver,
            LazyQueryArguments arguments)
            : base(
                  registry,
                  parent,
                  name,
                  resolver,
                  elementGraphType,
                  loader.ObjectGraphTypeConfigurator,
                  arguments)
        {
            _condition = condition;
            Loader = loader ?? throw new ArgumentNullException(nameof(loader));
        }

        private LoaderField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            TChildLoader loader,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType,
            LazyQueryArguments arguments)
            : this(
                  registry,
                  parent,
                  name,
                  condition,
                  loader,
                  elementGraphType,
                  CreateResolver(name, parent, loader, condition),
                  arguments)
        {
        }

        protected TChildLoader Loader { get; }

        public ConnectionLoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext> ApplyConnection()
        {
            var loader = Registry.ResolveLoader<TChildLoader, TChildEntity>();
            return ApplyField(new ConnectionLoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>(Registry, Parent, Name, _condition, Arguments, loader.ApplyNaturalOrderBy, loader.ApplyNaturalThenBy));
        }

        public override QueryableField<TEntity, TChildEntity, TExecutionContext> ApplyConnection(Expression<Func<IQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>>> order)
        {
            var loader = Registry.ResolveLoader<TChildLoader, TChildEntity>();
            return ApplyField(new ConnectionLoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>(Registry, Parent, Name, _condition, Arguments, order.Compile(), order.GetThenBy().Compile()));
        }

        public override QueryableField<TEntity, TChildEntity, TExecutionContext> ApplyFilter<TLoaderFilter, TFilter>()
        {
            if (HasFilter)
            {
                throw new NotSupportedException($"{typeof(TChildEntity).HumanizedName()}: Simultaneous use of .WithFilter() and .Filterable() is not supported. Consider to use either .WithFilter() or .Filterable().");
            }

            return base.ApplyFilter<TLoaderFilter, TFilter>();
        }

        protected static Func<IResolveFieldContext, IQueryable<TChildEntity>> GetQuery(TChildLoader loader)
        {
            return context =>
            {
                var filter = loader.ObjectGraphTypeConfigurator.HasInlineFilters ? loader.ObjectGraphTypeConfigurator.CreateInlineFilters() : null;

                var ctx = context.GetUserContext<TExecutionContext>();
                var listener = context.GetListener();
                var query = loader.DoGetBaseQuery(ctx);

                if (filter != null)
                {
                    query = filter.All(listener, query, ctx, context.GetFilterValue(filter.FilterType), context.GetFilterFieldNames());
                }

                return query;
            };
        }

        private static IQueryableResolver<TEntity, TChildEntity, TExecutionContext> CreateResolver(string fieldName, BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent, TChildLoader loader, Expression<Func<TEntity, TChildEntity, bool>> condition)
        {
            var func = GetQuery(loader);

            // TODO avoid creation in descendands
            if (condition == null)
            {
                return new QueryableFuncResolver<TEntity, TChildEntity, TExecutionContext>(loader.ObjectGraphTypeConfigurator.ProxyAccessor, func, (ctx, query) => loader.DoApplySecurityFilter(ctx.GetUserContext<TExecutionContext>(), query));
            }
            else
            {
                return new QueryableAsyncFuncResolver<TEntity, TChildEntity, TExecutionContext>(fieldName, func, condition, (ctx, query) => loader.DoApplySecurityFilter(ctx.GetUserContext<TExecutionContext>(), query), parent.ProxyAccessor, loader.ObjectGraphTypeConfigurator.ProxyAccessor);
            }
        }
    }

#pragma warning disable CA1501
    internal class LoaderField<TLoader, TChildLoader, TEntity, TChildEntity, TExecutionContext> : LoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>
#pragma warning restore CA1501
        where TLoader : Loader<TEntity, TExecutionContext>, new()
        where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
        where TEntity : class
        where TChildEntity : class
    {
        public LoaderField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            RelationType relationType,
            Expression<Func<TChildEntity, TEntity>> navigationProperty,
            Expression<Func<TEntity, TChildEntity>> reverseNavigationProperty,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType)
            : base(registry, parent, name, condition, elementGraphType)
        {
            registry.Register(typeof(TChildLoader), typeof(TLoader), condition.SwapOperands(), reverseNavigationProperty, navigationProperty, relationType);
        }
    }
}
