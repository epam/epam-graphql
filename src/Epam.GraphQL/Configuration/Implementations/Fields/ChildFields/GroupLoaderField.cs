// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Search;
using Epam.GraphQL.Sorters.Implementations;
using Epam.GraphQL.Types;
using GraphQL;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ChildFields
{
    internal sealed class GroupLoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext> :
        ConnectionLoaderFieldBase<
            GroupLoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>,
            TEntity,
            TChildLoader,
            TChildEntity,
            TExecutionContext>
        where TEntity : class
        where TChildEntity : class
        where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
    {
        private readonly IGraphTypeDescriptor<TExecutionContext> _graphType;

        public GroupLoaderField(
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType,
            LazyQueryArguments? arguments,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> naturalSorters)
            : base(
                  parent,
                  name,
                  condition: null,
                  elementGraphType,
                  arguments,
                  searcher: null,
                  naturalSorters)
        {
            _graphType = GraphTypeDescriptor.Create<GroupConnectionGraphType<TChildLoader, TChildEntity, TExecutionContext>, TExecutionContext>();
        }

        private GroupLoaderField(
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IQueryableResolver<TEntity, TChildEntity, TExecutionContext> resolver,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType,
            LazyQueryArguments? arguments,
            ISearcher<TChildEntity, TExecutionContext>? searcher,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> naturalSorters)
            : base(
                  parent,
                  name,
                  resolver,
                  elementGraphType,
                  arguments,
                  searcher,
                  naturalSorters)
        {
            _graphType = GraphTypeDescriptor.Create<GroupConnectionGraphType<TChildLoader, TChildEntity, TExecutionContext>, TExecutionContext>();
        }

        public override IResolver<TEntity> FieldResolver => QueryableFieldResolver
            .AsGroupConnection(GetGroupByQuery(Loader.ObjectGraphTypeConfigurator.ProxyAccessor), ApplyGroupSort(Loader.ObjectGraphTypeConfigurator));

        public override IGraphTypeDescriptor<TExecutionContext> GraphType => _graphType;

        protected override GroupLoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext> ReplaceResolver(IQueryableResolver<TEntity, TChildEntity, TExecutionContext> resolver)
        {
            return new GroupLoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>(
                Parent,
                Name,
                resolver,
                ElementGraphType,
                Arguments,
                Searcher,
                NaturalSorters!);
        }

        private static Func<IResolveFieldContext, IQueryable<TChildEntity>, IQueryable<Proxy<TChildEntity>>> GetGroupByQuery(IProxyAccessor<TChildEntity, TExecutionContext> proxyAccessor)
        {
            return (context, query) =>
            {
                var subFields = context.GetGroupConnectionQueriedFields();
                var aggregateQueriedFields = context.GetGroupConnectionAggregateQueriedFields();

                var sourceType = proxyAccessor.GetConcreteProxyType(subFields.Concat(aggregateQueriedFields.Select(field => $"${field}")));

                // ApplyGroupBy
                // Actual type of lambda returning value is loader.ObjectGraphTypeConfigurator.ExtendedType, not typeof(TChildEntity)
                // entity => new EntityExt { Country = entity.Country, Language = entity.Language }
                var lambda = proxyAccessor.CreateGroupSelectorExpression(sourceType, subFields).BindFirstParameter(context.GetUserContext<TExecutionContext>());

                // .Select(entity => new { Country = entity.Country, Language = entity.Language })
                var grouping = query.ApplySelect(lambda);

                // .GroupBy(g => g)
                var groupBy = grouping.ApplyGroupBy(ExpressionHelpers.MakeIdentity(sourceType));

                // .Select(g => new EntityExt { Country = g.Key.Country, Language = g.Key.Language, Count = g.Count(); })
                var selectKey = groupBy.ApplySelect(proxyAccessor.CreateGroupKeySelectorExpression(subFields, aggregateQueriedFields.Select(field => $"${field}")));

                return (IQueryable<Proxy<TChildEntity>>)selectKey;
            };
        }

        private static Func<IResolveFieldContext, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>> ApplyGroupSort(IObjectGraphTypeConfigurator<TChildEntity, TExecutionContext> configurator)
        {
            return (context) =>
            {
                // Apply sort
                var result = SortingHelpers.ApplyGroupSort(
                    context,
                    configurator.Sorters,
                    configurator.ProxyAccessor);

                return result;
            };
        }
    }
}
