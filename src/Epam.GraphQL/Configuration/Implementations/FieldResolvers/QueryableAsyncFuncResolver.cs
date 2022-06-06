// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Implementations.Fields.Helpers;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Relay;
using Epam.GraphQL.TaskBatcher;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal class QueryableAsyncFuncResolver<TEntity, TReturnType, TTransformedReturnType, TExecutionContext> :
        EnumerableAsyncFuncResolverBase<IQueryableResolver<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TTransformedReturnType, TExecutionContext>,
        IQueryableResolver<TEntity, TReturnType, TExecutionContext>
    {
        private readonly Expression<Func<TEntity, TReturnType, bool>> _condition;
        private readonly Func<IResolveFieldContext, IQueryable<TReturnType>> _resolver;
        private readonly Lazy<Func<IResolveFieldContext, IDataLoader<TEntity, IGrouping<TEntity, TTransformedReturnType>>>> _batchTaskResolver;
        private readonly Lazy<Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, IGrouping<Proxy<TEntity>, TTransformedReturnType>>>> _proxiedBatchTaskResolver;
        private readonly Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> _transform;
        private readonly Func<IResolveFieldContext, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>> _sorters;
        private readonly Func<IResolveFieldContext, IEnumerable<string>> _getQueriedFields;

        public QueryableAsyncFuncResolver(
            string fieldName,
            Func<IResolveFieldContext, IQueryable<TReturnType>> resolver,
            Expression<Func<TEntity, TReturnType, bool>> condition,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> transform,
            Func<IResolveFieldContext, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>> sorters,
            IProxyAccessor<TEntity, TExecutionContext> outerProxyAccessor,
            IProxyAccessor<TReturnType, TTransformedReturnType, TExecutionContext> innerProxyAccessor)
            : this(
                  fieldName,
                  resolver,
                  condition,
                  transform,
                  sorters,
                  outerProxyAccessor,
                  innerProxyAccessor,
                  GetQueriedFields)
        {
        }

        private QueryableAsyncFuncResolver(
            string fieldName,
            Func<IResolveFieldContext, IQueryable<TReturnType>> resolver,
            Expression<Func<TEntity, TReturnType, bool>> condition,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> transform,
            Func<IResolveFieldContext, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>> sorters,
            IProxyAccessor<TEntity, TExecutionContext> outerProxyAccessor,
            IProxyAccessor<TReturnType, TTransformedReturnType, TExecutionContext> innerProxyAccessor,
            Func<IResolveFieldContext, IEnumerable<string>> getQueriedFields)
            : base(fieldName, outerProxyAccessor, innerProxyAccessor)
        {
            _resolver = resolver;
            _condition = condition;
            _transform = transform;
            _sorters = sorters;
            _getQueriedFields = getQueriedFields;

            _batchTaskResolver = new Lazy<Func<IResolveFieldContext, IDataLoader<TEntity, IGrouping<TEntity, TTransformedReturnType>>>>(
                () => GetBatchTaskResolver<TEntity>(_resolver, _condition, _transform, _sorters, FuncConstants<LambdaExpression>.Identity));
            _proxiedBatchTaskResolver = new Lazy<Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, IGrouping<Proxy<TEntity>, TTransformedReturnType>>>>(
                () => GetBatchTaskResolver<Proxy<TEntity>>(_resolver, _condition, _transform, _sorters, leftExpression => outerProxyAccessor.Rewrite(leftExpression)));

            var factorizationResults = ExpressionHelpers.Factorize(condition);
            outerProxyAccessor.AddMembers(fieldName, innerProxyAccessor, factorizationResults);
        }

        protected override Func<IResolveFieldContext, IDataLoader<TEntity, IEnumerable<TTransformedReturnType>>> Resolver => ctx => _batchTaskResolver.Value(ctx).Then(items => items.SafeNull());

        protected override Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, IEnumerable<TTransformedReturnType>>> ProxiedResolver => ctx => _proxiedBatchTaskResolver.Value(ctx).Then(items => items.SafeNull());

        public IQueryableResolver<TEntity, TReturnType, TExecutionContext> Reorder(
            Func<IResolveFieldContext, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>> sorters)
        {
            return Create(_resolver, sorters);
        }

        public override IQueryableResolver<TEntity, TReturnType, TExecutionContext> Where(Expression<Func<TReturnType, bool>> predicate)
        {
            return Create(ctx => _resolver(ctx).Where(predicate), _sorters);
        }

        public IQueryableResolver<TEntity, TReturnType, TExecutionContext> Select(Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> selector)
        {
            return Create(ctx => selector(ctx, _resolver(ctx)), _sorters);
        }

        public override IEnumerableResolver<TEntity, TSelectType, TExecutionContext> Select<TSelectType>(
            Expression<Func<TReturnType, TSelectType>> selector,
            IProxyAccessor<TSelectType, TExecutionContext>? selectTypeProxyAccessor)
        {
            if (selectTypeProxyAccessor == null)
            {
                return new SelectEnumerableAsyncFuncResolver<TEntity, TReturnType, TTransformedReturnType, TSelectType, TSelectType, TExecutionContext>(
                    FieldName,
                    QueryFactory,
                    _condition,
                    selector,
                    OuterProxyAccessor,
                    ReturnTypeProxyAccessor,
                    IdentityProxyAccessor<TSelectType, TExecutionContext>.Instance);
            }

            return new SelectEnumerableAsyncFuncResolver<TEntity, TReturnType, TTransformedReturnType, TSelectType, Proxy<TSelectType>, TExecutionContext>(
                FieldName,
                QueryFactory,
                _condition,
                selector,
                OuterProxyAccessor,
                ReturnTypeProxyAccessor,
                selectTypeProxyAccessor);

            IQueryable<TReturnType> QueryFactory(IResolveFieldContext ctx)
            {
                var sorters = _sorters(ctx);
                var result = _transform(ctx, _resolver(ctx));

                if (sorters.Any())
                {
                    return result.ApplyOrderBy(sorters);
                }

                return result;
            }
        }

        public IFieldResolver AsConnection()
        {
            var resolver = new QueryableAsyncFuncResolver<TEntity, TReturnType, TTransformedReturnType, TExecutionContext>(FieldName, _resolver, _condition, _transform, _sorters, OuterProxyAccessor, ReturnTypeProxyAccessor, GetConnectionQueriedFields);
            return new AsyncFuncResolver<TEntity, Connection<TTransformedReturnType>>(
                ctx => resolver.Resolver(ctx).Then(items => Resolvers.Resolve(ctx, items.AsQueryable())),
                ctx => resolver.ProxiedResolver(ctx).Then(items => Resolvers.Resolve(ctx, items.AsQueryable())));

            static IEnumerable<string> GetConnectionQueriedFields(IResolveFieldContext context)
            {
                return context.GetConnectionQueriedFields();
            }
        }

        private static IEnumerable<string> GetQueriedFields(IResolveFieldContext context)
        {
            return context.GetQueriedFields();
        }

        private IQueryableResolver<TEntity, TReturnType, TExecutionContext> Create(
            Func<IResolveFieldContext, IQueryable<TReturnType>> resolver,
            Func<IResolveFieldContext, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>> sorters)
        {
            return new QueryableAsyncFuncResolver<TEntity, TReturnType, TTransformedReturnType, TExecutionContext>(FieldName, resolver, _condition, _transform, sorters, OuterProxyAccessor, ReturnTypeProxyAccessor, _getQueriedFields);
        }

        private Func<IResolveFieldContext, IDataLoader<TOuterEntity, IGrouping<TOuterEntity, TTransformedReturnType>>> GetBatchTaskResolver<TOuterEntity>(
            Func<IResolveFieldContext, IQueryable<TReturnType>> resolver,
            Expression<Func<TEntity, TReturnType, bool>> condition,
            Func<IResolveFieldContext, IQueryable<TReturnType>, IQueryable<TReturnType>> queryTransform,
            Func<IResolveFieldContext, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>> sorters,
            Func<LambdaExpression, LambdaExpression> leftExpressionConverter)
        {
            var factorizationResult = ExpressionHelpers.FactorizeCondition(condition);

            var outerExpression = new Lazy<LambdaExpression>(() => leftExpressionConverter(factorizationResult.LeftExpression));
            var innerExpression = factorizationResult.RightExpression;
            var rightCondition = factorizationResult.RightCondition;

            if (ReturnTypeProxyAccessor.HasHooks)
            {
                return context =>
                {
                    var hooksExecuter = ReturnTypeProxyAccessor.CreateHooksExecuter(context);

                    var result = context
                        .Get<TOuterEntity, TReturnType, TTransformedReturnType>(
                            ctx => queryTransform(ctx, resolver(ctx)).SafeWhere(rightCondition),
                            sorters: sorters,
                            Transform,
                            outerExpression.Value,
                            innerExpression,
                            hooksExecuter);

                    return result;
                };
            }

            return context =>
            {
                var result = context
                    .Get<TOuterEntity, TReturnType, TTransformedReturnType>(
                        ctx => queryTransform(ctx, resolver(ctx)).SafeWhere(rightCondition),
                        sorters: sorters,
                        Transform,
                        outerExpression.Value,
                        innerExpression,
                        null);

                return result;
            };
        }

        private Expression<Func<TReturnType, TTransformedReturnType>> Transform(IResolveFieldContext context)
        {
            var fieldNames = _getQueriedFields(context);
            var lambda = ReturnTypeProxyAccessor.CreateSelectorExpression(fieldNames);

            var ctx = context.GetUserContext<TExecutionContext>();
            return lambda.BindFirstParameter(ctx);
        }
    }
}
