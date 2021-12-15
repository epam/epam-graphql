// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Relay;
using Epam.GraphQL.Types;
using GraphQL;
using Expr = System.Linq.Expressions.Expression;

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
            .Select(GetGroupByQuery(Loader))
            .Reorder(OrderBy(Loader))
            .Select(ToConnection(Loader));

        public override IGraphTypeDescriptor<TExecutionContext> GraphType => _graphType;

        private static Func<IResolveFieldContext, IQueryable<TChildEntity>, IQueryable<Proxy<TChildEntity>>> GetGroupByQuery(TChildLoader loader)
        {
            return (context, query) =>
            {
                var subFields = context.GetGroupConnectionQueriedFields();
                var aggregateQueriedFields = GetAggregateQueriedFields(context).Select(name => $"<>${name}");

                var sourceType = loader.ObjectGraphTypeConfigurator.ProxyAccessor.GetConcreteProxyType(subFields.Concat(aggregateQueriedFields));

                // ApplyGroupBy
                // Actual type of lambda returning value is loader.ObjectGraphTypeConfigurator.ExtendedType, not typeof(TChildEntity)
                // entity => new EntityExt { Country = entity.Country, Language = entity.Language }
                var lambda = loader.ObjectGraphTypeConfigurator.ProxyAccessor.CreateGroupSelectorExpression(sourceType, subFields).BindFirstParameter(context.GetUserContext<TExecutionContext>());

                // .Select(entity => new { Country = entity.Country, Language = entity.Language })
                var grouping = query.ApplySelect(lambda);

                // .GroupBy(g => g)
                var groupBy = grouping.ApplyGroupBy(BuildGroupingIdentityExpression(sourceType));

                // .Select(g => new EntityExt { Country = g.Key.Country, Language = g.Key.Language, Count = g.Count(); })
                var selectKey = groupBy.ApplySelect(BuildGroupKeyExpression(sourceType, loader, subFields, aggregateQueriedFields));

                return CachedReflectionInfo.ForQueryable.Cast(sourceType).InvokeAndHoistBaseException<IQueryable<Proxy<TChildEntity>>>(null, selectKey);
            };
        }

        private static Func<IResolveFieldContext, IQueryable<Proxy<TChildEntity>>, IOrderedQueryable<Proxy<TChildEntity>>> OrderBy(TChildLoader loader)
        {
            return (context, query) =>
            {
                // Apply sort
                var result = ApplySort(
                    context,
                    query,
                    loader);

                return result;
            };
        }

        private static LambdaExpression BuildGroupKeyExpression(Type sourceType, TChildLoader loader, IEnumerable<string> fieldNames, IEnumerable<string> aggregateFieldNames)
        {
            var fields = loader.ObjectGraphTypeConfigurator.Fields.Where(field => fieldNames.Any(name => field.Name.Equals(name, StringComparison.OrdinalIgnoreCase)));

            var groupingType = typeof(IGrouping<,>).MakeGenericType(sourceType, sourceType);
            var param = Expr.Parameter(groupingType);
            var entityExpr = Expr.Property(param, groupingType.GetProperty("Key"));

            var ctor = sourceType.GetConstructors().Single(c => c.GetParameters().Length == 0);
            var newExpr = Expr.New(ctor);
            var initExpr = Expr.MemberInit(
                newExpr,
                fields
                    .Select(f =>
                    {
                        var propInfo = sourceType.GetProperty(f.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                        return Expr.Bind(propInfo, Expr.Property(entityExpr, propInfo));
                    })
                    .Concat(aggregateFieldNames
                        .SafeNull()
                        .Select(f =>
                        {
                            if (f == "<>$count")
                            {
                                var propInfo = sourceType.GetProperty("<>$count", BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                                var countMethodInfo = CachedReflectionInfo.ForEnumerable.Count(sourceType);
                                var methodCallExpr = Expr.Call(null, countMethodInfo, param);

                                return Expr.Bind(propInfo, methodCallExpr);
                            }

                            throw new NotSupportedException();
                        })));

            return Expr.Lambda(initExpr, param);
        }

        private static LambdaExpression BuildGroupingIdentityExpression(Type sourceType)
        {
            var param = Expr.Parameter(sourceType);
            return Expr.Lambda(param, param);
        }

        private static LambdaExpression BuildMemberAccess(TChildLoader loader, string fieldName)
        {
            var param = Expr.Parameter(loader.ObjectGraphTypeConfigurator.ProxyAccessor.ProxyType);
            return Expr.Lambda(
                Expr.Property(param, loader.ObjectGraphTypeConfigurator.ProxyAccessor.ProxyType.GetProperty(fieldName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase)),
                param);
        }

        private static IOrderedQueryable<Proxy<TChildEntity>> ApplyNaturalGroupOrderBy(TChildLoader loader, IEnumerable<IField<TExecutionContext>> fields, IQueryable<Proxy<TChildEntity>> query)
        {
            var result = query.ApplyOrderBy(fields.Select(f => (BuildMemberAccess(loader, f.Name), SortDirection.Asc)));
            return result;
        }

        private static IOrderedQueryable<Proxy<TChildEntity>> ApplyNaturalGroupThenBy(TChildLoader loader, IEnumerable<IField<TExecutionContext>> fields, IOrderedQueryable<Proxy<TChildEntity>> query)
        {
            var result = query.ApplyThenBy(fields.Select(f => (BuildMemberAccess(loader, f.Name), SortDirection.Asc)));
            return result;
        }

        private static IEnumerable<string> GetAggregateQueriedFields(IResolveFieldContext context)
        {
            return context.GetGroupConnectionAggregateQueriedFields();
        }

        private static Func<IResolveFieldContext, IOrderedQueryable<Proxy<TChildEntity>>, Connection<object>> ToConnection(TChildLoader loader)
        {
            return (context, children) =>
            {
                var subFields = context.GetGroupConnectionQueriedFields();
                var aggregateQueriedFields = GetAggregateQueriedFields(context).Select(name => $"<>${name}");

                var sourceType = loader.ObjectGraphTypeConfigurator.ProxyAccessor.GetConcreteProxyType(subFields.Concat(aggregateQueriedFields));

                var first = context.GetFirst();
                var last = context.GetLast();
                var after = context.GetAfter();
                var before = context.GetBefore();

                var shouldComputeCount = context.HasTotalCount();
                var shouldComputeEndOffset = context.HasEndCursor();
                var shouldComputeEdges = context.HasEdges();
                var shouldComputeItems = context.HasItems();

                IOrderedQueryable<object> items;
                if (aggregateQueriedFields.Contains("<>$count"))
                {
                    var propGetter = sourceType.GetPropertyDelegate("<>$count");
                    Func<object, int> countGetter = entity => (int)propGetter(entity);
                    items = (IOrderedQueryable<object>)children.SafeNull().AsQueryable().Select(entity => new GroupResult<Proxy<TChildEntity>>
                    {
                        Item = entity,
                        Count = countGetter(entity),
                    });
                }
                else
                {
                    items = (IOrderedQueryable<object>)children.SafeNull().AsQueryable().Select(entity => new GroupResult<Proxy<TChildEntity>>
                    {
                        Item = entity,
                    });
                }

                return ConnectionUtils.ToConnection(
                    items,
                    () => context.GetPath(),
                    context.GetQueryExecuter(),
                    first,
                    last,
                    before,
                    after,
                    shouldComputeCount,
                    shouldComputeEndOffset,
                    shouldComputeEdges,
                    shouldComputeItems);
            };
        }

        private static IOrderedQueryable<Proxy<TChildEntity>> ApplySort(
            IResolveFieldContext context,
            IQueryable<Proxy<TChildEntity>> queryable,
            TChildLoader loader)
        {
            var sourceType = loader.ObjectGraphTypeConfigurator.ProxyAccessor.ProxyType;

            var sorting = context.GetSorting();
            var search = context.GetSearch();

            var fields = sorting
                .Select(o => (loader.ObjectGraphTypeConfigurator.FindFieldByName(o.Field), o.Direction));

            if (fields.Any(f => !(f.Item1?.IsExpression ?? false)))
            {
                throw new ExecutionError($"Cannot find field(s) for sorting: {string.Join(", ", sorting.Select(o => o.Field))}");
            }

            IOrderedQueryable<Proxy<TChildEntity>> query = null;
            foreach (var f in fields)
            {
                query = query == null
                    ? queryable.ApplyOrderBy(BuildMemberAccess(loader, f.Item1.Name), f.Direction)
                    : query.ApplyThenBy(BuildMemberAccess(loader, f.Item1.Name), f.Direction);
            }

            var subFields = context.GetGroupConnectionQueriedFields().Select(name => loader.ObjectGraphTypeConfigurator.FindFieldByName(name));

            if (query == null)
            {
                query = ApplyNaturalGroupOrderBy(loader, subFields, queryable);
                return (IOrderedQueryable<Proxy<TChildEntity>>)query.Cast<Proxy<TChildEntity>>();
            }

            query = ApplyNaturalGroupThenBy(loader, subFields, query);
            return (IOrderedQueryable<Proxy<TChildEntity>>)query.Cast<Proxy<TChildEntity>>();
        }
    }
}
