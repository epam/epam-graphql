// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;
using Epam.GraphQL.Configuration.Implementations.Fields.Helpers;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Sorters.Implementations;
using Epam.GraphQL.Types;
using GraphQL;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ChildFields
{
#pragma warning disable CA1501
    internal class GroupLoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext> : OrderedLoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>
#pragma warning restore CA1501
        where TEntity : class
        where TChildEntity : class
        where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
    {
        private readonly IGraphTypeDescriptor<TExecutionContext> _graphType;

        public GroupLoaderField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            LazyQueryArguments arguments,
            Func<IQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>> orderBy,
            Func<IOrderedQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>> thenBy)
            : base(
                  registry,
                  parent,
                  name,
                  null,
                  registry.ResolveLoader<TChildLoader, TChildEntity>(),
                  null,
                  arguments,
                  orderBy,
                  thenBy)
        {
            _graphType = GraphTypeDescriptor.Create<GroupConnectionGraphType<TChildLoader, TChildEntity, TExecutionContext>, TExecutionContext>();

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

        public override IResolver<TEntity> FieldResolver => OrderedQueryableFieldResolver
            .Select(GetGroupByQuery(Loader.ObjectGraphTypeConfigurator.ProxyAccessor), OrderBy(Loader.ObjectGraphTypeConfigurator))
            .Select(Resolvers.ToGroupConnection(Loader.ObjectGraphTypeConfigurator.ProxyAccessor));

        public override IGraphTypeDescriptor<TExecutionContext> GraphType => _graphType;

        private static Func<IResolveFieldContext, IQueryable<TChildEntity>, IQueryable<Proxy<TChildEntity>>> GetGroupByQuery(IProxyAccessor<TChildEntity, TExecutionContext> proxyAccessor)
        {
            return (context, query) =>
            {
                var subFields = context.GetGroupConnectionQueriedFields();
                var aggregateQueriedFields = context.GetGroupConnectionAggregateQueriedFields().Select(name => $"<>${name}");

                var sourceType = proxyAccessor.GetConcreteProxyType(subFields.Concat(aggregateQueriedFields));

                // ApplyGroupBy
                // Actual type of lambda returning value is loader.ObjectGraphTypeConfigurator.ExtendedType, not typeof(TChildEntity)
                // entity => new EntityExt { Country = entity.Country, Language = entity.Language }
                var lambda = proxyAccessor.CreateGroupSelectorExpression(sourceType, subFields).BindFirstParameter(context.GetUserContext<TExecutionContext>());

                // .Select(entity => new { Country = entity.Country, Language = entity.Language })
                var grouping = query.ApplySelect(lambda);

                // .GroupBy(g => g)
                var groupBy = grouping.ApplyGroupBy(ExpressionHelpers.MakeIdentity(sourceType));

                // .Select(g => new EntityExt { Country = g.Key.Country, Language = g.Key.Language, Count = g.Count(); })
                var selectKey = groupBy.ApplySelect(proxyAccessor.CreateGroupKeySelectorExpression(subFields, aggregateQueriedFields));

                return (IQueryable<Proxy<TChildEntity>>)selectKey;
            };
        }

        private static Func<IResolveFieldContext, IQueryable<Proxy<TChildEntity>>, IOrderedQueryable<Proxy<TChildEntity>>> OrderBy(IObjectGraphTypeConfigurator<TChildEntity, TExecutionContext> configurator)
        {
            return (context, query) =>
            {
                // Apply sort
                var result = SortingHelpers.ApplyGroupSort(
                    context,
                    query,
                    configurator.Sorters,
                    configurator.ProxyAccessor);

                return result;
            };
        }
    }
}
