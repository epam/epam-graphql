// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.TaskBatcher;
using GraphQL;
using GraphQL.DataLoader;

namespace Epam.GraphQL.Configuration.Implementations.FieldResolvers
{
    internal class SelectEnumerableAsyncFuncResolver<TEntity, TChildEntity, TChildEntityTransformedType, TReturnType, TTransformedReturnType, TExecutionContext> :
        EnumerableAsyncFuncResolverBase<IEnumerableResolver<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TTransformedReturnType, TExecutionContext>,
        IEnumerableResolver<TEntity, TReturnType, TExecutionContext>
    {
        private readonly Func<IResolveFieldContext, IDataLoader<TEntity, IGrouping<TEntity, TTransformedReturnType>>> _batchTaskResolver;
        private readonly Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, IGrouping<Proxy<TEntity>, TTransformedReturnType>>> _proxiedBatchTaskResolver;
        private readonly Func<IResolveFieldContext, IQueryable<TChildEntity>> _resolver;
        private readonly Expression<Func<TEntity, TChildEntity, bool>> _condition;
        private readonly Expression<Func<TChildEntity, TReturnType>> _transform;

        public SelectEnumerableAsyncFuncResolver(
            string fieldName,
            Func<IResolveFieldContext, IQueryable<TChildEntity>> resolver,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            Expression<Func<TChildEntity, TReturnType>> transform,
            IProxyAccessor<TEntity, TExecutionContext> outerProxyAccessor,
            IProxyAccessor<TChildEntity, TChildEntityTransformedType, TExecutionContext> innerProxyAccessor,
            IProxyAccessor<TReturnType, TTransformedReturnType, TExecutionContext> returnTypeProxyAccessor)
            : base(fieldName, outerProxyAccessor, returnTypeProxyAccessor)
        {
            _resolver = resolver;
            _condition = condition;
            _transform = transform;
            InnerProxyAccessor = innerProxyAccessor;
            _batchTaskResolver = GetBatchTaskResolver<TEntity>(resolver, condition, transform, FuncConstants<LambdaExpression>.Identity);
            _proxiedBatchTaskResolver = GetBatchTaskResolver<Proxy<TEntity>>(resolver, condition, transform, leftExpression => OuterProxyAccessor.Rewrite(leftExpression, leftExpression));

            innerProxyAccessor?.AddMember(transform);
        }

        protected IProxyAccessor<TChildEntity, TChildEntityTransformedType, TExecutionContext> InnerProxyAccessor { get; }

        protected override Func<IResolveFieldContext, IDataLoader<TEntity, IEnumerable<TTransformedReturnType>>> Resolver => ctx => _batchTaskResolver(ctx).Then(grouping => grouping.SafeNull());

        protected override Func<IResolveFieldContext, IDataLoader<Proxy<TEntity>, IEnumerable<TTransformedReturnType>>> ProxiedResolver => ctx => _proxiedBatchTaskResolver(ctx).Then(grouping => grouping.SafeNull());

        public override IEnumerableResolver<TEntity, TSelectType, TExecutionContext> Select<TSelectType>(
            Expression<Func<TReturnType, TSelectType>> selector,
            IProxyAccessor<TSelectType, TExecutionContext>? selectTypeProxyAccessor)
        {
            InnerProxyAccessor.RemoveMember(_transform);

            if (selectTypeProxyAccessor == null)
            {
                return new SelectEnumerableAsyncFuncResolver<TEntity, TChildEntity, TChildEntityTransformedType, TSelectType, TSelectType, TExecutionContext>(
                    FieldName,
                    _resolver,
                    _condition,
                    ExpressionHelpers.Compose(_transform, selector),
                    OuterProxyAccessor,
                    InnerProxyAccessor,
                    IdentityProxyAccessor<TSelectType, TExecutionContext>.Instance);
            }

            return new SelectEnumerableAsyncFuncResolver<TEntity, TChildEntity, TChildEntityTransformedType, TSelectType, Proxy<TSelectType>, TExecutionContext>(
                FieldName,
                _resolver,
                _condition,
                ExpressionHelpers.Compose(_transform, selector),
                OuterProxyAccessor,
                InnerProxyAccessor,
                selectTypeProxyAccessor);
        }

        public override IEnumerableResolver<TEntity, TReturnType, TExecutionContext> Where(Expression<Func<TReturnType, bool>> predicate)
        {
            InnerProxyAccessor.RemoveMember(_transform);

            return new SelectEnumerableAsyncFuncResolver<TEntity, TChildEntity, TChildEntityTransformedType, TReturnType, TTransformedReturnType, TExecutionContext>(
                FieldName,
                ctx => _resolver(ctx).Where(ExpressionHelpers.Compose(_transform, predicate)),
                _condition,
                _transform,
                OuterProxyAccessor,
                InnerProxyAccessor,
                ReturnTypeProxyAccessor);
        }

        private Expression<Func<TExecutionContext, TChildEntity, TTransformedReturnType>> Transform(
            IEnumerable<string> fieldNames,
            Expression<Func<TChildEntity, TReturnType>> expression)
        {
            var transformedReturnLambda = ReturnTypeProxyAccessor.CreateSelectorExpression(fieldNames);

            var proxiedSelector = InnerProxyAccessor.Rewrite(expression);
            var transformedLambda = InnerProxyAccessor.CreateSelectorExpression(fieldNames);
            var transformedResult = ExpressionRewriter.Rewrite(ExpressionHelpers.Compose(transformedLambda, proxiedSelector));

            var result = ExpressionHelpers.SafeCompose(transformedResult, transformedReturnLambda);

            return result;
        }

        private Expression<Func<TExecutionContext, TChildEntity, Tuple<TChildEntityTransformedType, TTransformedReturnType>>> Transform2(
            IEnumerable<string> fieldNames,
            Expression<Func<TChildEntity, TReturnType>> transform)
        {
            var childEntityLambda = InnerProxyAccessor.CreateSelectorExpression(fieldNames);
            var resultLambda = Transform(fieldNames, transform);

            var executionContextParam = Expression.Parameter(typeof(TExecutionContext));
            var entityParam = Expression.Parameter(typeof(TChildEntity));

            var childEntityProxyExpr = childEntityLambda.Body
                .ReplaceParameter(childEntityLambda.Parameters[0], executionContextParam)
                .ReplaceParameter(childEntityLambda.Parameters[1], entityParam);

            var transformedReturnExpr = resultLambda.Body
                .ReplaceParameter(resultLambda.Parameters[0], executionContextParam)
                .ReplaceParameter(resultLambda.Parameters[1], entityParam);

            var newExpr = Expression.New(
                typeof(Tuple<TChildEntityTransformedType, TTransformedReturnType>).GetConstructor(new[] { typeof(TChildEntityTransformedType), typeof(TTransformedReturnType) }),
                childEntityProxyExpr,
                transformedReturnExpr);

            var result = ExpressionRewriter.Rewrite(
                Expression.Lambda<Func<TExecutionContext, TChildEntity, Tuple<TChildEntityTransformedType, TTransformedReturnType>>>(
                    newExpr,
                    executionContextParam,
                    entityParam));

            return result;
        }

        private Func<IResolveFieldContext, IDataLoader<TOuterEntity, IGrouping<TOuterEntity, TTransformedReturnType>>> GetBatchTaskResolver<TOuterEntity>(
            Func<IResolveFieldContext, IQueryable<TChildEntity>> resolver,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            Expression<Func<TChildEntity, TReturnType>> transform,
            Func<LambdaExpression, LambdaExpression> leftExpressionConverter)
        {
            var factorizationResult = ExpressionHelpers.FactorizeCondition(condition);

            var outerExpression = new Lazy<LambdaExpression>(() => leftExpressionConverter(factorizationResult.LeftExpression));
            var innerExpression = factorizationResult.RightExpression;
            var rightCondition = factorizationResult.RightCondition;

            if (InnerProxyAccessor.HasHooks)
            {
                return context =>
                {
                    var result = context
                        .Get<TOuterEntity, TChildEntity, Tuple<TChildEntityTransformedType, TTransformedReturnType>>(
                            ctx => resolver(ctx).SafeWhere(rightCondition),
                            sorters: null,
                            ctx => Transform2(ctx.GetQueriedFields(), transform).BindFirstParameter(ctx.GetUserContext<TExecutionContext>()),
                            outerExpression.Value,
                            innerExpression,
                            new LoaderHooksExecuter<TChildEntity, TChildEntityTransformedType, TTransformedReturnType, TExecutionContext>(context, InnerProxyAccessor))
                        .Then(group => Grouping.Create(group.Key, group.Select(g => g.Item2)));

                    return result;
                };
            }

            return context =>
            {
                var result = context
                    .Get<TOuterEntity, TChildEntity, TTransformedReturnType>(
                        ctx => resolver(ctx).SafeWhere(rightCondition),
                        sorters: null,
                        ctx => Transform(ctx.GetQueriedFields(), transform).BindFirstParameter(ctx.GetUserContext<TExecutionContext>()),
                        outerExpression.Value,
                        innerExpression,
                        CreateHooksExecuter(context));

                return result;
            };
        }
    }
}
