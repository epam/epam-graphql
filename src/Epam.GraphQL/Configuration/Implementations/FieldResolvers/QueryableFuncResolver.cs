// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Configuration.Implementations.Fields.Helpers;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Relay;
using Epam.GraphQL.Sorters;
using Epam.GraphQL.TaskBatcher;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal class QueryableFuncResolver<TReturnType, TTransformedReturnType, TExecutionContext> : IRootQueryableResolver<TReturnType, TExecutionContext>
    {
        private readonly Func<IResolveFieldContext, IQueryable<TReturnType>> _resolver;
        private readonly IProxyAccessor<TReturnType, TTransformedReturnType, TExecutionContext> _proxyAccessor;
        private readonly Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> _transform;
        private readonly Func<IResolveFieldContext, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>> _sorters;
        private readonly Func<IResolveFieldContext, IEnumerable<string>> _getQueriedFields;

        public QueryableFuncResolver(
            IProxyAccessor<TReturnType, TTransformedReturnType, TExecutionContext> proxyAccessor,
            Func<IResolveFieldContext, IQueryable<TReturnType>> resolver,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> transform,
            Func<IResolveFieldContext, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>> sorters)
            : this(
                  proxyAccessor,
                  resolver,
                  transform,
                  sorters,
                  GetQueriedFields)
        {
        }

        private QueryableFuncResolver(
            IProxyAccessor<TReturnType, TTransformedReturnType, TExecutionContext> proxyAccessor,
            Func<IResolveFieldContext, IQueryable<TReturnType>> resolver,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> transform,
            Func<IResolveFieldContext, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>> sorters,
            Func<IResolveFieldContext, IEnumerable<string>> getQueriedFields)
        {
            _resolver = resolver;
            _transform = transform;
            _sorters = sorters;
            _proxyAccessor = proxyAccessor;
            _getQueriedFields = getQueriedFields;
        }

        private Func<IResolveFieldContext, IQueryable<TReturnType>> Resolver => ctx =>
        {
            var result = _transform(ctx, _resolver(ctx));
            var sorters = _sorters(ctx);

            if (sorters.Any())
            {
                return result.ApplyOrderBy(sorters);
            }

            return result;
        };

        public ValueTask<object?> ResolveAsync(IResolveFieldContext context)
        {
            if (_proxyAccessor.HasHooks)
            {
                var hooksExecuter = _proxyAccessor.CreateHooksExecuter(context);
                return new ValueTask<object?>(hooksExecuter!
                    .Execute(FuncConstants<TTransformedReturnType>.Identity)
                    .LoadAsync(context.ExecuteQuery(ctx => Resolver(ctx).Select(Transform(ctx)))));
            }

            return new ValueTask<object?>(context.ExecuteQuery(ctx => Resolver(ctx).Select(Transform(ctx))));
        }

        public IRootQueryableResolver<TReturnType, TExecutionContext> Select(Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> selector)
        {
            return Create(_proxyAccessor, ctx => selector(ctx, _resolver(ctx)), _transform, _sorters);
        }

        public IRootQueryableResolver<TSelectType, TExecutionContext> Select<TSelectType>(
            Expression<Func<TReturnType, TSelectType>> selector,
            IProxyAccessor<TSelectType, TExecutionContext>? selectTypeProxyAccessor)
        {
            if (selectTypeProxyAccessor != null)
            {
                return Create(
                    selectTypeProxyAccessor,
                    ctx => Resolver(ctx).Select(selector),
                    (ctx, query) => query,
                    ctx => Enumerable.Empty<(LambdaExpression SortExpression, SortDirection SortDirection)>());
            }

            return new QueryableFuncResolver<TSelectType, TSelectType, TExecutionContext>(
                IdentityProxyAccessor<TSelectType, TExecutionContext>.Instance,
                ctx => Resolver(ctx).Select(selector),
                (ctx, query) => query,
                ctx => Enumerable.Empty<(LambdaExpression SortExpression, SortDirection SortDirection)>());
        }

        public IFieldResolver SingleOrDefault()
        {
            return new FuncResolver<TReturnType, TTransformedReturnType, TExecutionContext>(
                _proxyAccessor,
                ctx => ctx.ExecuteQuery(ctx => Resolver(ctx).Select(Transform(ctx)), query => query.SingleOrDefault(), nameof(Queryable.SingleOrDefault)));
        }

        public IFieldResolver FirstOrDefault()
        {
            return new FuncResolver<TReturnType, TTransformedReturnType, TExecutionContext>(
                _proxyAccessor,
                ctx => ctx.ExecuteQuery(ctx => Resolver(ctx).Select(Transform(ctx)), query => query.FirstOrDefault(), nameof(Queryable.FirstOrDefault)));
        }

        public IRootQueryableResolver<TReturnType, TExecutionContext> Reorder(Func<IResolveFieldContext, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>> sorters)
        {
            return Create(_proxyAccessor, _resolver, _transform, sorters);
        }

        public IFieldResolver AsGroupConnection(IEnumerable<ISorter<TExecutionContext>> sorters)
        {
            var resolver = new QueryableFuncResolver<IGroupResult<TTransformedReturnType>, IGroupResult<TTransformedReturnType>, TExecutionContext>(
                IdentityProxyAccessor<IGroupResult<TTransformedReturnType>, TExecutionContext>.Instance,
                ctx => _proxyAccessor.GroupBy(ctx, _transform(ctx, _resolver(ctx))),
                (ctx, query) => query,
                Sort);

            return new FuncResolver<Connection<object>, Connection<object>, TExecutionContext>(
                IdentityProxyAccessor<Connection<object>, TExecutionContext>.Instance,
                ctx =>
                {
                    var resolved = resolver.Resolver(ctx);
                    var selected = Resolvers.Resolve(
                         ctx,
                         (IQueryable<object>)resolved);

                    return selected;
                });

            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> Sort(IResolveFieldContext context)
            {
                // Apply sort
                var result = _proxyAccessor.GetGroupSort(
                    context,
                    sorters);

                return result;
            }
        }

        public IRootQueryableResolver<TReturnType, TExecutionContext> Where(Expression<Func<TReturnType, bool>> predicate)
        {
            return Create(_proxyAccessor, ctx => _resolver(ctx).Where(predicate), _transform, _sorters);
        }

        public IFieldResolver AsConnection()
        {
            var resolver = new QueryableFuncResolver<TReturnType, TTransformedReturnType, TExecutionContext>(_proxyAccessor, _resolver, _transform, _sorters, GetConnectionQueriedFields);

            if (_proxyAccessor.HasHooks)
            {
                return new FuncResolver<
                    Connection<IDataLoaderResult<IEnumerable<TTransformedReturnType>>, IDataLoaderResult<global::GraphQL.Types.Relay.DataObjects.Edge<TTransformedReturnType>[]>>,
                    Connection<IDataLoaderResult<IEnumerable<TTransformedReturnType>>, IDataLoaderResult<global::GraphQL.Types.Relay.DataObjects.Edge<TTransformedReturnType>[]>>,
                    TExecutionContext>(
                    IdentityProxyAccessor<Connection<IDataLoaderResult<IEnumerable<TTransformedReturnType>>, IDataLoaderResult<global::GraphQL.Types.Relay.DataObjects.Edge<TTransformedReturnType>[]>>, TExecutionContext>.Instance,
                    context =>
                    {
                        var resolved = resolver.Resolver(context).Select(resolver.Transform(context));
                        var selected = Resolvers.Resolve(context, resolved);

                        var connection = new Connection<IDataLoaderResult<IEnumerable<TTransformedReturnType>>, IDataLoaderResult<global::GraphQL.Types.Relay.DataObjects.Edge<TTransformedReturnType>[]>>
                        {
                            PageInfo = selected.PageInfo,
                            TotalCount = selected.TotalCount,
                        };

                        if (selected.Edges != null)
                        {
                            var hooksExecuter = _proxyAccessor.CreateHooksExecuter(context);
                            connection.Edges = hooksExecuter!
                                .Execute<global::GraphQL.Types.Relay.DataObjects.Edge<TTransformedReturnType>>(edge => edge.Node!)
                                .LoadAsync(selected.Edges);

                            if (selected.Items != null)
                            {
                                connection.Items = connection.Edges.Then(edges => edges.Select(edge => edge.Node!));
                            }

                            return connection;
                        }

                        if (selected.Items != null)
                        {
                            var hooksExecuter = _proxyAccessor.CreateHooksExecuter(context);
                            connection.Items = hooksExecuter!
                                .Execute(FuncConstants<TTransformedReturnType>.Identity)
                                .LoadAsync(selected.Items)
                                .Then(items => items.AsEnumerable());
                        }

                        return connection;
                    });
            }

            return new FuncResolver<Connection<TTransformedReturnType>, Connection<TTransformedReturnType>, TExecutionContext>(
                IdentityProxyAccessor<Connection<TTransformedReturnType>, TExecutionContext>.Instance,
                ctx =>
                {
                    var resolved = resolver.Resolver(ctx).Select(resolver.Transform(ctx));
                    var selected = Resolvers.Resolve(ctx, resolved);

                    return selected;
                });

            static IEnumerable<string> GetConnectionQueriedFields(IResolveFieldContext context)
            {
                return context.GetConnectionQueriedFields();
            }
        }

        private static IEnumerable<string> GetQueriedFields(IResolveFieldContext context)
        {
            return context.GetQueriedFields();
        }

        private QueryableFuncResolver<TAnotherReturnType, TAnotherTransformedReturnType, TExecutionContext> Create<TAnotherReturnType, TAnotherTransformedReturnType>(
            IProxyAccessor<TAnotherReturnType, TAnotherTransformedReturnType, TExecutionContext> proxyAccessor,
            Func<IResolveFieldContext, IQueryable<TAnotherReturnType>> resolver,
            Func<IResolveFieldContext, IQueryable<TAnotherReturnType>, IQueryable<TAnotherReturnType>> transform,
            Func<IResolveFieldContext, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>> sorters)
        {
            return new QueryableFuncResolver<TAnotherReturnType, TAnotherTransformedReturnType, TExecutionContext>(proxyAccessor, resolver, transform, sorters, _getQueriedFields);
        }

        private Expression<Func<TReturnType, TTransformedReturnType>> Transform(IResolveFieldContext context)
        {
            var fieldNames = _getQueriedFields(context);
            var lambda = _proxyAccessor.CreateSelectorExpression(fieldNames);

            var ctx = context.GetUserContext<TExecutionContext>();
            return lambda.BindFirstParameter(ctx);
        }
    }
}
