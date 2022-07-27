// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Common;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Builders.MutableLoader;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Enums;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Tests.Helpers;

namespace Epam.GraphQL.Tests
{
    public static class TestExtensions
    {
        public static IRootLoaderField<TChildEntity, TExecutionContext> Field<TChildEntity, TExecutionContext>(this Query<TExecutionContext> query, string name, Type loaderType)
        {
            var methodInfo = typeof(Query<TExecutionContext>).GetNonPublicGenericMethod(
                nameof(Query<TExecutionContext>.Field),
                new[] { loaderType, typeof(TChildEntity) },
                new[] { typeof(string), typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IRootLoaderField<TChildEntity, TExecutionContext>>(query, name, null);
        }

        public static IRootLoaderField<TChildEntity, TExecutionContext> FromLoader<TChildEntity, TExecutionContext>(this IQueryField<TExecutionContext> builder, Type loaderType)
        {
            var methodInfo = typeof(IQueryField<TExecutionContext>).GetPublicGenericMethod(
                nameof(IQueryField<TExecutionContext>.FromLoader),
                new[] { loaderType, typeof(TChildEntity) },
                Type.EmptyTypes);

            return methodInfo.InvokeAndHoistBaseException<IRootLoaderField<TChildEntity, TExecutionContext>>(builder);
        }

        public static TThis WithSearch<TThis, TChildEntity, TExecutionContext>(this ISearchableField<TThis, TChildEntity, TExecutionContext> field, Type searcherType)
        {
            var methodInfo = typeof(ISearchableField<TThis, TChildEntity, TExecutionContext>).GetPublicGenericMethod(
                nameof(ISearchableField<TThis, TChildEntity, TExecutionContext>.WithSearch),
                new[] { searcherType },
                Type.EmptyTypes);

            return methodInfo.InvokeAndHoistBaseException<TThis>(field);
        }

        public static IConnectionField WithFilter(this IConnectionField field, Type filterType)
        {
            var methodInfo = typeof(IConnectionField).GetPublicGenericMethod(
                nameof(IConnectionField.WithFilter),
                new[] { filterType },
                Type.EmptyTypes);

            return methodInfo.InvokeAndHoistBaseException<IConnectionField>(field);
        }

        public static IConnectionField WithSearch(this IConnectionField field, Type searcherType)
        {
            var methodInfo = typeof(IConnectionField).GetPublicGenericMethod(
                nameof(IConnectionField.WithSearch),
                new[] { searcherType },
                Type.EmptyTypes);

            return methodInfo.InvokeAndHoistBaseException<IConnectionField>(field);
        }

        public static IConnectionField Connection<TExecutionContext>(this Query<TExecutionContext> query, Type loaderType, string fieldName)
        {
            var methodInfo = typeof(Query<TExecutionContext>).GetNonPublicGenericMethod(
                nameof(Query<TExecutionContext>.Connection),
                new[] { loaderType },
                new[] { typeof(string), typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IConnectionField>(query, fieldName, null);
        }

        public static IConnectionField Connection<TEntity, TExecutionContext>(this Query<TExecutionContext> query, Type loaderType, string fieldName, Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>> order)
        {
            var methodInfo = typeof(Query<TExecutionContext>).GetNonPublicGenericMethod(
                nameof(Query<TExecutionContext>.Connection),
                new[] { loaderType, typeof(TEntity) },
                new[] { typeof(string), typeof(Expression<Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>>), typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IConnectionField>(query, fieldName, order, null);
        }

        public static IConnectionField GroupConnection<TExecutionContext>(this Query<TExecutionContext> query, Type loaderType, string fieldName)
        {
            var methodInfo = typeof(Query<TExecutionContext>).GetNonPublicGenericMethod(
                nameof(Query<TExecutionContext>.GroupConnection),
                new[] { loaderType },
                new[] { typeof(string), typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IConnectionField>(query, fieldName, null);
        }

        public static IArgumentedQueryField<Expression<Func<TEntity, bool>>, TExecutionContext> FilterArgument<TEntity, TExecutionContext>(this IQueryField<TExecutionContext> builder, Type loaderType, string argName)
        {
            var methodInfo = typeof(IQueryField<TExecutionContext>).GetPublicGenericMethod(
                nameof(IQueryField<TExecutionContext>.FilterArgument),
                new[] { loaderType, typeof(TEntity) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IArgumentedQueryField<Expression<Func<TEntity, bool>>, TExecutionContext>>(builder, argName);
        }

        public static IArgumentedQueryField<TArg1, Expression<Func<TEntity, bool>>, TExecutionContext> FilterArgument<TArg1, TEntity, TExecutionContext>(this IArgumentedQueryField<TArg1, TExecutionContext> field, Type loaderType, string argName)
        {
            var methodInfo = typeof(IArgumentedQueryField<TArg1, TExecutionContext>).GetPublicGenericMethod(
                nameof(IArgumentedQueryField<TArg1, TExecutionContext>.FilterArgument),
                new[] { loaderType, typeof(TEntity) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IArgumentedQueryField<TArg1, Expression<Func<TEntity, bool>>, TExecutionContext>>(field, argName);
        }

        public static IArgumentedQueryField<TArg1, TArg2, Expression<Func<TEntity, bool>>, TExecutionContext> FilterArgument<TArg1, TArg2, TEntity, TExecutionContext>(this IArgumentedQueryField<TArg1, TArg2, TExecutionContext> field, Type loaderType, string argName)
        {
            var methodInfo = typeof(IArgumentedQueryField<TArg1, TArg2, TExecutionContext>).GetPublicGenericMethod(
                nameof(IArgumentedQueryField<TArg1, TArg2, TExecutionContext>.FilterArgument),
                new[] { loaderType, typeof(TEntity) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IArgumentedQueryField<TArg1, TArg2, Expression<Func<TEntity, bool>>, TExecutionContext>>(field, argName);
        }

        public static IArgumentedQueryField<TArg1, TArg2, TArg3, Expression<Func<TEntity, bool>>, TExecutionContext> FilterArgument<TArg1, TArg2, TArg3, TEntity, TExecutionContext>(this IArgumentedQueryField<TArg1, TArg2, TArg3, TExecutionContext> field, Type loaderType, string argName)
        {
            var methodInfo = typeof(IArgumentedQueryField<TArg1, TArg2, TArg3, TExecutionContext>).GetPublicGenericMethod(
                nameof(IArgumentedQueryField<TArg1, TArg2, TArg3, TExecutionContext>.FilterArgument),
                new[] { loaderType, typeof(TEntity) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IArgumentedQueryField<TArg1, TArg2, TArg3, Expression<Func<TEntity, bool>>, TExecutionContext>>(field, argName);
        }

        public static IUnionableRootField<TArg1, TArg2, TArg3, TArg4, Expression<Func<TEntity, bool>>, TExecutionContext> FilterArgument<TArg1, TArg2, TArg3, TArg4, TEntity, TExecutionContext>(this IArgumentedQueryField<TArg1, TArg2, TArg3, TArg4, TExecutionContext> field, Type loaderType, string argName)
        {
            var methodInfo = typeof(IArgumentedQueryField<TArg1, TArg2, TArg3, TArg4, TExecutionContext>).GetPublicGenericMethod(
                nameof(IArgumentedQueryField<TArg1, TArg2, TArg3, TArg4, TExecutionContext>.FilterArgument),
                new[] { loaderType, typeof(TEntity) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IUnionableRootField<TArg1, TArg2, TArg3, TArg4, Expression<Func<TEntity, bool>>, TExecutionContext>>(field, argName);
        }

        public static IPayloadFieldedQueryField<Expression<Func<TEntity, bool>>, TExecutionContext> FilterPayloadField<TEntity, TExecutionContext>(this IQueryField<TExecutionContext> builder, Type loaderType, string argName)
        {
            var methodInfo = typeof(IQueryField<TExecutionContext>).GetPublicGenericMethod(
                nameof(IQueryField<TExecutionContext>.FilterPayloadField),
                new[] { loaderType, typeof(TEntity) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IPayloadFieldedQueryField<Expression<Func<TEntity, bool>>, TExecutionContext>>(builder, argName);
        }

        public static IPayloadFieldedQueryField<TArg1, Expression<Func<TEntity, bool>>, TExecutionContext> FilterPayloadField<TArg1, TEntity, TExecutionContext>(this IPayloadFieldedQueryField<TArg1, TExecutionContext> field, Type loaderType, string argName)
        {
            var methodInfo = typeof(IPayloadFieldedQueryField<TArg1, TExecutionContext>).GetPublicGenericMethod(
                nameof(IPayloadFieldedQueryField<TArg1, TExecutionContext>.FilterPayloadField),
                new[] { loaderType, typeof(TEntity) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IPayloadFieldedQueryField<TArg1, Expression<Func<TEntity, bool>>, TExecutionContext>>(field, argName);
        }

        public static IPayloadFieldedQueryField<TArg1, TArg2, Expression<Func<TEntity, bool>>, TExecutionContext> FilterPayloadField<TArg1, TArg2, TEntity, TExecutionContext>(this IPayloadFieldedQueryField<TArg1, TArg2, TExecutionContext> field, Type loaderType, string argName)
        {
            var methodInfo = typeof(IPayloadFieldedQueryField<TArg1, TArg2, TExecutionContext>).GetPublicGenericMethod(
                nameof(IPayloadFieldedQueryField<TArg1, TArg2, TExecutionContext>.FilterPayloadField),
                new[] { loaderType, typeof(TEntity) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IPayloadFieldedQueryField<TArg1, TArg2, Expression<Func<TEntity, bool>>, TExecutionContext>>(field, argName);
        }

        public static IPayloadFieldedQueryField<TArg1, TArg2, TArg3, Expression<Func<TEntity, bool>>, TExecutionContext> FilterPayloadField<TArg1, TArg2, TArg3, TEntity, TExecutionContext>(this IPayloadFieldedQueryField<TArg1, TArg2, TArg3, TExecutionContext> field, Type loaderType, string argName)
        {
            var methodInfo = typeof(IPayloadFieldedQueryField<TArg1, TArg2, TArg3, TExecutionContext>).GetPublicGenericMethod(
                nameof(IPayloadFieldedQueryField<TArg1, TArg2, TArg3, TExecutionContext>.FilterPayloadField),
                new[] { loaderType, typeof(TEntity) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IPayloadFieldedQueryField<TArg1, TArg2, TArg3, Expression<Func<TEntity, bool>>, TExecutionContext>>(field, argName);
        }

        public static IUnionableRootField<TArg1, TArg2, TArg3, TArg4, Expression<Func<TEntity, bool>>, TExecutionContext> FilterPayloadField<TArg1, TArg2, TArg3, TArg4, TEntity, TExecutionContext>(this IPayloadFieldedQueryField<TArg1, TArg2, TArg3, TArg4, TExecutionContext> field, Type loaderType, string argName)
        {
            var methodInfo = typeof(IPayloadFieldedQueryField<TArg1, TArg2, TArg3, TArg4, TExecutionContext>).GetPublicGenericMethod(
                nameof(IPayloadFieldedQueryField<TArg1, TArg2, TArg3, TArg4, TExecutionContext>.FilterPayloadField),
                new[] { loaderType, typeof(TEntity) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IUnionableRootField<TArg1, TArg2, TArg3, TArg4, Expression<Func<TEntity, bool>>, TExecutionContext>>(field, argName);
        }

        public static IArgumentedMutationField<Expression<Func<TEntity, bool>>, TExecutionContext> FilterArgument<TEntity, TExecutionContext>(this IMutationField<TExecutionContext> builder, Type loaderType, string argName)
        {
            var methodInfo = typeof(IMutationField<TExecutionContext>).GetPublicGenericMethod(
                nameof(IMutationField<TExecutionContext>.FilterArgument),
                new[] { loaderType, typeof(TEntity) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IArgumentedMutationField<Expression<Func<TEntity, bool>>, TExecutionContext>>(builder, argName);
        }

        public static IArgumentedMutationField<TArg1, Expression<Func<TEntity, bool>>, TExecutionContext> FilterArgument<TArg1, TEntity, TExecutionContext>(this IArgumentedMutationField<TArg1, TExecutionContext> field, Type loaderType, string argName)
        {
            var methodInfo = typeof(IArgumentedMutationField<TArg1, TExecutionContext>).GetPublicGenericMethod(
                nameof(IArgumentedMutationField<TArg1, TExecutionContext>.FilterArgument),
                new[] { loaderType, typeof(TEntity) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IArgumentedMutationField<TArg1, Expression<Func<TEntity, bool>>, TExecutionContext>>(field, argName);
        }

        public static IArgumentedMutationField<TArg1, TArg2, Expression<Func<TEntity, bool>>, TExecutionContext> FilterArgument<TArg1, TArg2, TEntity, TExecutionContext>(this IArgumentedMutationField<TArg1, TArg2, TExecutionContext> field, Type loaderType, string argName)
        {
            var methodInfo = typeof(IArgumentedMutationField<TArg1, TArg2, TExecutionContext>).GetPublicGenericMethod(
                nameof(IArgumentedMutationField<TArg1, TArg2, TExecutionContext>.FilterArgument),
                new[] { loaderType, typeof(TEntity) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IArgumentedMutationField<TArg1, TArg2, Expression<Func<TEntity, bool>>, TExecutionContext>>(field, argName);
        }

        public static IArgumentedMutationField<TArg1, TArg2, TArg3, Expression<Func<TEntity, bool>>, TExecutionContext> FilterArgument<TArg1, TArg2, TArg3, TEntity, TExecutionContext>(this IArgumentedMutationField<TArg1, TArg2, TArg3, TExecutionContext> field, Type loaderType, string argName)
        {
            var methodInfo = typeof(IArgumentedMutationField<TArg1, TArg2, TArg3, TExecutionContext>).GetPublicGenericMethod(
                nameof(IArgumentedMutationField<TArg1, TArg2, TArg3, TExecutionContext>.FilterArgument),
                new[] { loaderType, typeof(TEntity) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IArgumentedMutationField<TArg1, TArg2, TArg3, Expression<Func<TEntity, bool>>, TExecutionContext>>(field, argName);
        }

        public static IUnionableRootField<TArg1, TArg2, TArg3, TArg4, Expression<Func<TEntity, bool>>, TExecutionContext> FilterArgument<TArg1, TArg2, TArg3, TArg4, TEntity, TExecutionContext>(this IArgumentedMutationField<TArg1, TArg2, TArg3, TArg4, TExecutionContext> field, Type loaderType, string argName)
        {
            var methodInfo = typeof(IArgumentedMutationField<TArg1, TArg2, TArg3, TArg4, TExecutionContext>).GetPublicGenericMethod(
                nameof(IArgumentedMutationField<TArg1, TArg2, TArg3, TArg4, TExecutionContext>.FilterArgument),
                new[] { loaderType, typeof(TEntity) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IUnionableRootField<TArg1, TArg2, TArg3, TArg4, Expression<Func<TEntity, bool>>, TExecutionContext>>(field, argName);
        }

        public static IPayloadFieldedMutationField<Expression<Func<TEntity, bool>>, TExecutionContext> FilterPayloadField<TEntity, TExecutionContext>(this IMutationField<TExecutionContext> builder, Type loaderType, string argName)
        {
            var methodInfo = typeof(IMutationField<TExecutionContext>).GetPublicGenericMethod(
                nameof(IMutationField<TExecutionContext>.FilterPayloadField),
                new[] { loaderType, typeof(TEntity) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IPayloadFieldedMutationField<Expression<Func<TEntity, bool>>, TExecutionContext>>(builder, argName);
        }

        public static IPayloadFieldedMutationField<TArg1, Expression<Func<TEntity, bool>>, TExecutionContext> FilterPayloadField<TArg1, TEntity, TExecutionContext>(this IPayloadFieldedMutationField<TArg1, TExecutionContext> field, Type loaderType, string argName)
        {
            var methodInfo = typeof(IPayloadFieldedMutationField<TArg1, TExecutionContext>).GetPublicGenericMethod(
                nameof(IPayloadFieldedMutationField<TArg1, TExecutionContext>.FilterPayloadField),
                new[] { loaderType, typeof(TEntity) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IPayloadFieldedMutationField<TArg1, Expression<Func<TEntity, bool>>, TExecutionContext>>(field, argName);
        }

        public static IPayloadFieldedMutationField<TArg1, TArg2, Expression<Func<TEntity, bool>>, TExecutionContext> FilterPayloadField<TArg1, TArg2, TEntity, TExecutionContext>(this IPayloadFieldedMutationField<TArg1, TArg2, TExecutionContext> field, Type loaderType, string argName)
        {
            var methodInfo = typeof(IPayloadFieldedMutationField<TArg1, TArg2, TExecutionContext>).GetPublicGenericMethod(
                nameof(IPayloadFieldedMutationField<TArg1, TArg2, TExecutionContext>.FilterPayloadField),
                new[] { loaderType, typeof(TEntity) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IPayloadFieldedMutationField<TArg1, TArg2, Expression<Func<TEntity, bool>>, TExecutionContext>>(field, argName);
        }

        public static IPayloadFieldedMutationField<TArg1, TArg2, TArg3, Expression<Func<TEntity, bool>>, TExecutionContext> FilterPayloadField<TArg1, TArg2, TArg3, TEntity, TExecutionContext>(this IPayloadFieldedMutationField<TArg1, TArg2, TArg3, TExecutionContext> field, Type loaderType, string argName)
        {
            var methodInfo = typeof(IPayloadFieldedMutationField<TArg1, TArg2, TArg3, TExecutionContext>).GetPublicGenericMethod(
                nameof(IPayloadFieldedMutationField<TArg1, TArg2, TArg3, TExecutionContext>.FilterPayloadField),
                new[] { loaderType, typeof(TEntity) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IPayloadFieldedMutationField<TArg1, TArg2, TArg3, Expression<Func<TEntity, bool>>, TExecutionContext>>(field, argName);
        }

        public static IUnionableRootField<TArg1, TArg2, TArg3, TArg4, Expression<Func<TEntity, bool>>, TExecutionContext> FilterPayloadField<TArg1, TArg2, TArg3, TArg4, TEntity, TExecutionContext>(this IPayloadFieldedMutationField<TArg1, TArg2, TArg3, TArg4, TExecutionContext> field, Type loaderType, string argName)
        {
            var methodInfo = typeof(IPayloadFieldedMutationField<TArg1, TArg2, TArg3, TArg4, TExecutionContext>).GetPublicGenericMethod(
                nameof(IPayloadFieldedMutationField<TArg1, TArg2, TArg3, TArg4, TExecutionContext>.FilterPayloadField),
                new[] { loaderType, typeof(TEntity) },
                new[] { typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<IUnionableRootField<TArg1, TArg2, TArg3, TArg4, Expression<Func<TEntity, bool>>, TExecutionContext>>(field, argName);
        }

        public static ILoaderField<TEntity, TChildEntity, TExecutionContext> FromLoader<TEntity, TChildEntity, TExecutionContext>(
            this IHasFromLoader<TEntity, TExecutionContext> builder,
            Type childLoaderType,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            RelationType relationType = RelationType.Association,
            Expression<Func<TChildEntity, TEntity>> navigationProperty = null,
            Expression<Func<TEntity, TChildEntity>> reverseNavigationProperty = null)
        {
            var methodInfo = typeof(IHasFromLoader<TEntity, TExecutionContext>).GetPublicGenericMethod(
                nameof(IHasFromLoader<TEntity, TExecutionContext>.FromLoader),
                new[] { childLoaderType, typeof(TChildEntity) },
                new[] { typeof(Expression<Func<TEntity, TChildEntity, bool>>), typeof(RelationType), typeof(Expression<Func<TChildEntity, TEntity>>), typeof(Expression<Func<TEntity, TChildEntity>>) });

            return methodInfo.InvokeAndHoistBaseException<ILoaderField<TEntity, TChildEntity, TExecutionContext>>(
                builder,
                condition,
                relationType,
                navigationProperty,
                reverseNavigationProperty);
        }

        public static IInlineLoaderField<TEntity, TChildEntity, TExecutionContext> FromLoader<TEntity, TChildEntity, TExecutionContext>(
            this IInlineObjectFieldBuilder<TEntity, TExecutionContext> builder,
            Type childLoaderType,
            Expression<Func<TEntity, TChildEntity, bool>> condition)
        {
            var methodInfo = typeof(IInlineObjectFieldBuilder<TEntity, TExecutionContext>).GetPublicGenericMethod(
                nameof(IInlineObjectFieldBuilder<TEntity, TExecutionContext>.FromLoader),
                new[] { childLoaderType, typeof(TChildEntity) },
                new[] { typeof(Expression<Func<TEntity, TChildEntity, bool>>) });

            return methodInfo.InvokeAndHoistBaseException<IInlineLoaderField<TEntity, TChildEntity, TExecutionContext>>(
                builder,
                condition);
        }

        public static void ConfigureFrom<TEntity, TExecutionContext>(
            this IInlineObjectBuilder<TEntity, TExecutionContext> builder,
            Type projectionType)
        {
            var methodInfo = typeof(IInlineObjectBuilder<TEntity, TExecutionContext>).GetPublicGenericMethod(
                nameof(IInlineObjectBuilder<TEntity, TExecutionContext>.ConfigureFrom),
                new[] { projectionType },
                Type.EmptyTypes);

            methodInfo.InvokeAndHoistBaseException(builder);
        }

        public static IHasFilterableAndSortableAndOnWriteAndEditableAndMandatoryForUpdate<TEntity, TReturnType, TExecutionContext> ReferencesTo<TParentEntity, TEntity, TReturnType, TExecutionContext>(
            this IHasFilterableAndSortableAndOnWriteAndEditableAndMandatoryForUpdateAndReferenceTo<TEntity, TReturnType, TExecutionContext> builder,
            Type parentLoaderType,
            Expression<Func<TParentEntity, TReturnType>> parentProperty,
            Expression<Func<TEntity, TParentEntity>> navigationProperty,
            RelationType relationType)
        {
            var foreignKeyMethodInfo = typeof(IHasFilterableAndSortableAndOnWriteAndEditableAndMandatoryForUpdateAndReferenceTo<TEntity, TReturnType, TExecutionContext>).GetPublicGenericMethod(
                nameof(ReferencesTo),
                new[] { typeof(TParentEntity), parentLoaderType },
                new[] { typeof(Expression<Func<TParentEntity, TReturnType>>), typeof(Expression<Func<TEntity, TParentEntity>>), typeof(RelationType) });
            return (IHasFilterableAndSortableAndOnWriteAndEditableAndMandatoryForUpdate<TEntity, TReturnType, TExecutionContext>)foreignKeyMethodInfo.InvokeAndHoistBaseException(builder, parentProperty, navigationProperty, relationType);
        }

        public static Expression<Func<TEntity, bool>> GetExpressionByFilterValue<TEntity, TExecutionContext>(
            this ISchemaExecuter<TExecutionContext> schemaExecuter,
            Type projectionType,
            TExecutionContext executionContext,
            Dictionary<string, object> filterValue)
        {
            var methodInfo = typeof(ISchemaExecuter<TExecutionContext>).GetPublicGenericMethod(
                nameof(ISchemaExecuter<TExecutionContext>.GetExpressionByFilterValue),
                new[] { projectionType, typeof(TEntity) },
                new[] { typeof(TExecutionContext), typeof(Dictionary<string, object>) });

            return methodInfo.InvokeAndHoistBaseException<Expression<Func<TEntity, bool>>>(schemaExecuter, executionContext, filterValue);
        }

        public static IHasSelectAndReferenceToAndAndFromBatch<TEntity, TResult, TExecutionContext> FromBatch<TEntity, TKey, TResult, TExecutionContext>(
            this Builders.Loader.IHasFromBatch<TEntity, TExecutionContext> builder,
            FromBatchType fromBatchType,
            Expression<Func<TEntity, TKey>> keySelector,
            Func<TExecutionContext> contextFactory,
            Func<TExecutionContext, IEnumerable<TKey>, IDictionary<TKey, TResult>> batchFunc,
            Action<IInlineObjectBuilder<TResult, TExecutionContext>> resultBuilder = null)
        {
            Guards.ThrowIfNull(builder, nameof(builder));

            var result = fromBatchType switch
            {
                FromBatchType.KeyContext => builder.FromBatch(keySelector, batchFunc, resultBuilder),
                FromBatchType.KeyContextTask => builder.FromBatch(
                    keySelector,
                    (ctx, items) => Task.FromResult(batchFunc(ctx, items)),
                    resultBuilder),
                FromBatchType.Key => builder.FromBatch(
                    keySelector,
                    items => batchFunc(contextFactory(), items),
                    resultBuilder),
                FromBatchType.KeyTask => builder.FromBatch(
                    keySelector,
                    items => Task.FromResult(batchFunc(contextFactory(), items)),
                    resultBuilder),

                FromBatchType.EntityContext => builder.FromBatch(
                    (ctx, items) =>
                    {
                        var selector = keySelector.Compile();
                        var dict = items.ToDictionary(selector);
                        return batchFunc(ctx, dict.Keys)
                            .ToDictionary(
                                pair => dict[pair.Key],
                                pair => pair.Value);
                    },
                    resultBuilder),
                FromBatchType.EntityContextTask => builder.FromBatch(
                    (ctx, items) =>
                    {
                        var selector = keySelector.Compile();
                        var dict = items.ToDictionary(selector);
                        return Task.FromResult<IDictionary<TEntity, TResult>>(
                            batchFunc(ctx, dict.Keys)
                                .ToDictionary(
                                    pair => dict[pair.Key],
                                    pair => pair.Value));
                    },
                    resultBuilder),
                FromBatchType.Entity => builder.FromBatch(
                    items =>
                    {
                        var selector = keySelector.Compile();
                        var dict = items.ToDictionary(selector);
                        return batchFunc(contextFactory(), dict.Keys)
                            .ToDictionary(
                                pair => dict[pair.Key],
                                pair => pair.Value);
                    },
                    resultBuilder),
                FromBatchType.EntityTask => builder.FromBatch(
                    items =>
                    {
                        var selector = keySelector.Compile();
                        var dict = items.ToDictionary(selector);
                        return Task.FromResult<IDictionary<TEntity, TResult>>(
                            batchFunc(contextFactory(), dict.Keys)
                                .ToDictionary(
                                    pair => dict[pair.Key],
                                    pair => pair.Value));
                    },
                    resultBuilder),
                _ => throw new NotImplementedException(),
            };

            return result;
        }

        public static IHasSelectAndAndFromBatch<TEntity, IEnumerable<TResult>, TExecutionContext> FromBatch<TEntity, TKey, TResult, TExecutionContext>(
            this Builders.Loader.IHasFromBatch<TEntity, TExecutionContext> builder,
            FromBatchType fromBatchType,
            Expression<Func<TEntity, TKey>> keySelector,
            Func<TExecutionContext> contextFactory,
            Func<TExecutionContext, IEnumerable<TKey>, IDictionary<TKey, IEnumerable<TResult>>> batchFunc,
            Action<IInlineObjectBuilder<TResult, TExecutionContext>> resultBuilder = null)
        {
            Guards.ThrowIfNull(builder, nameof(builder));

            var result = fromBatchType switch
            {
                FromBatchType.KeyContext => builder.FromBatch(keySelector, batchFunc, resultBuilder),
                FromBatchType.KeyContextTask => builder.FromBatch(
                    keySelector,
                    (ctx, items) => Task.FromResult(batchFunc(ctx, items)),
                    resultBuilder),
                FromBatchType.Key => builder.FromBatch(
                    keySelector,
                    items => batchFunc(contextFactory(), items),
                    resultBuilder),
                FromBatchType.KeyTask => builder.FromBatch(
                    keySelector,
                    items => Task.FromResult(batchFunc(contextFactory(), items)),
                    resultBuilder),

                FromBatchType.EntityContext => builder.FromBatch(
                    (ctx, items) =>
                    {
                        var selector = keySelector.Compile();
                        var dict = items.ToDictionary(selector);
                        return batchFunc(ctx, dict.Keys)
                            .ToDictionary(
                                pair => dict[pair.Key],
                                pair => pair.Value);
                    },
                    resultBuilder),
                FromBatchType.EntityContextTask => builder.FromBatch(
                    (ctx, items) =>
                    {
                        var selector = keySelector.Compile();
                        var dict = items.ToDictionary(selector);
                        return Task.FromResult<IDictionary<TEntity, IEnumerable<TResult>>>(
                            batchFunc(ctx, dict.Keys)
                                .ToDictionary(
                                    pair => dict[pair.Key],
                                    pair => pair.Value));
                    },
                    resultBuilder),
                FromBatchType.Entity => builder.FromBatch(
                    items =>
                    {
                        var selector = keySelector.Compile();
                        var dict = items.ToDictionary(selector);
                        return batchFunc(contextFactory(), dict.Keys)
                            .ToDictionary(
                                pair => dict[pair.Key],
                                pair => pair.Value);
                    },
                    resultBuilder),
                FromBatchType.EntityTask => builder.FromBatch(
                    items =>
                    {
                        var selector = keySelector.Compile();
                        var dict = items.ToDictionary(selector);
                        return Task.FromResult<IDictionary<TEntity, IEnumerable<TResult>>>(
                            batchFunc(contextFactory(), dict.Keys)
                                .ToDictionary(
                                    pair => dict[pair.Key],
                                    pair => pair.Value));
                    },
                    resultBuilder),
                _ => throw new NotImplementedException(),
            };

            return result;
        }

        public static IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, TResult, TExecutionContext> FromBatch<TEntity, TKey, TResult, TExecutionContext>(
            this IMutableLoaderFieldBuilder<TEntity, TExecutionContext> builder,
            FromBatchType fromBatchType,
            Expression<Func<TEntity, TKey>> keySelector,
            Func<TExecutionContext> contextFactory,
            Func<TExecutionContext, IEnumerable<TKey>, IDictionary<TKey, TResult>> batchFunc,
            Action<IInlineObjectBuilder<TResult, TExecutionContext>> resultBuilder = null)
        {
            Guards.ThrowIfNull(builder, nameof(builder));

            var result = fromBatchType switch
            {
                FromBatchType.KeyContext => builder.FromBatch(keySelector, batchFunc, resultBuilder),
                FromBatchType.KeyContextTask => builder.FromBatch(
                    keySelector,
                    (ctx, items) => Task.FromResult(batchFunc(ctx, items)),
                    resultBuilder),
                FromBatchType.Key => builder.FromBatch(
                    keySelector,
                    items => batchFunc(contextFactory(), items),
                    resultBuilder),
                FromBatchType.KeyTask => builder.FromBatch(
                    keySelector,
                    items => Task.FromResult(batchFunc(contextFactory(), items)),
                    resultBuilder),

                FromBatchType.EntityContext => builder.FromBatch(
                    (ctx, items) =>
                    {
                        var selector = keySelector.Compile();
                        var dict = items.ToDictionary(selector);
                        return batchFunc(ctx, dict.Keys)
                            .ToDictionary(
                                pair => dict[pair.Key],
                                pair => pair.Value);
                    },
                    resultBuilder),
                FromBatchType.EntityContextTask => builder.FromBatch(
                    (ctx, items) =>
                    {
                        var selector = keySelector.Compile();
                        var dict = items.ToDictionary(selector);
                        return Task.FromResult<IDictionary<TEntity, TResult>>(
                            batchFunc(ctx, dict.Keys)
                                .ToDictionary(
                                    pair => dict[pair.Key],
                                    pair => pair.Value));
                    },
                    resultBuilder),
                FromBatchType.Entity => builder.FromBatch(
                    items =>
                    {
                        var selector = keySelector.Compile();
                        var dict = items.ToDictionary(selector);
                        return batchFunc(contextFactory(), dict.Keys)
                            .ToDictionary(
                                pair => dict[pair.Key],
                                pair => pair.Value);
                    },
                    resultBuilder),
                FromBatchType.EntityTask => builder.FromBatch(
                    items =>
                    {
                        var selector = keySelector.Compile();
                        var dict = items.ToDictionary(selector);
                        return Task.FromResult<IDictionary<TEntity, TResult>>(
                            batchFunc(contextFactory(), dict.Keys)
                                .ToDictionary(
                                    pair => dict[pair.Key],
                                    pair => pair.Value));
                    },
                    resultBuilder),
                _ => throw new NotImplementedException(),
            };

            return result;
        }

        public static IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndAndFromBatch<TEntity, IEnumerable<TResult>, TExecutionContext> FromBatch<TEntity, TKey, TResult, TExecutionContext>(
            this IMutableLoaderFieldBuilder<TEntity, TExecutionContext> builder,
            FromBatchType fromBatchType,
            Expression<Func<TEntity, TKey>> keySelector,
            Func<TExecutionContext> contextFactory,
            Func<TExecutionContext, IEnumerable<TKey>, IDictionary<TKey, IEnumerable<TResult>>> batchFunc,
            Action<IInlineObjectBuilder<TResult, TExecutionContext>> resultBuilder = null)
        {
            Guards.ThrowIfNull(builder, nameof(builder));

            var result = fromBatchType switch
            {
                FromBatchType.KeyContext => builder.FromBatch(keySelector, batchFunc, resultBuilder),
                FromBatchType.KeyContextTask => builder.FromBatch(
                    keySelector,
                    (ctx, items) => Task.FromResult(batchFunc(ctx, items)),
                    resultBuilder),
                FromBatchType.Key => builder.FromBatch(
                    keySelector,
                    items => batchFunc(contextFactory(), items),
                    resultBuilder),
                FromBatchType.KeyTask => builder.FromBatch(
                    keySelector,
                    items => Task.FromResult(batchFunc(contextFactory(), items)),
                    resultBuilder),

                FromBatchType.EntityContext => builder.FromBatch(
                    (ctx, items) =>
                    {
                        var selector = keySelector.Compile();
                        var dict = items.ToDictionary(selector);
                        return batchFunc(ctx, dict.Keys)
                            .ToDictionary(
                                pair => dict[pair.Key],
                                pair => pair.Value);
                    },
                    resultBuilder),
                FromBatchType.EntityContextTask => builder.FromBatch(
                    (ctx, items) =>
                    {
                        var selector = keySelector.Compile();
                        var dict = items.ToDictionary(selector);
                        return Task.FromResult<IDictionary<TEntity, IEnumerable<TResult>>>(
                            batchFunc(ctx, dict.Keys)
                                .ToDictionary(
                                    pair => dict[pair.Key],
                                    pair => pair.Value));
                    },
                    resultBuilder),
                FromBatchType.Entity => builder.FromBatch(
                    items =>
                    {
                        var selector = keySelector.Compile();
                        var dict = items.ToDictionary(selector);
                        return batchFunc(contextFactory(), dict.Keys)
                            .ToDictionary(
                                pair => dict[pair.Key],
                                pair => pair.Value);
                    },
                    resultBuilder),
                FromBatchType.EntityTask => builder.FromBatch(
                    items =>
                    {
                        var selector = keySelector.Compile();
                        var dict = items.ToDictionary(selector);
                        return Task.FromResult<IDictionary<TEntity, IEnumerable<TResult>>>(
                            batchFunc(contextFactory(), dict.Keys)
                                .ToDictionary(
                                    pair => dict[pair.Key],
                                    pair => pair.Value));
                    },
                    resultBuilder),
                _ => throw new NotImplementedException(),
            };

            return result;
        }

        public static IHasSelectAndAndFromBatch<TEntity, IEnumerable<object>, TExecutionContext> AndFromBatch<TEntity, TKey, TResult, TPreviousBatchResult, TExecutionContext>(
            this IHasSelectAndAndFromBatch<TEntity, TPreviousBatchResult, TExecutionContext> builder,
            FromBatchType fromBatchType,
            Expression<Func<TEntity, TKey>> keySelector,
            Func<TExecutionContext> contextFactory,
            Func<TExecutionContext, IEnumerable<TKey>, IDictionary<TKey, TResult>> batchFunc,
            Action<IInlineObjectBuilder<TResult, TExecutionContext>> resultBuilder = null)
        {
            Guards.ThrowIfNull(builder, nameof(builder));

            var result = fromBatchType switch
            {
                FromBatchType.KeyContext => builder.AndFromBatch(
                    keySelector,
                    batchFunc,
                    resultBuilder),
                FromBatchType.KeyContextTask => builder.AndFromBatch(
                    keySelector,
                    (ctx, items) => Task.FromResult(batchFunc(ctx, items)),
                    resultBuilder),
                FromBatchType.Key => builder.AndFromBatch(
                    keySelector,
                    items => batchFunc(contextFactory(), items),
                    resultBuilder),
                FromBatchType.KeyTask => builder.AndFromBatch(
                    keySelector,
                    items => Task.FromResult(batchFunc(contextFactory(), items)),
                    resultBuilder),

                FromBatchType.EntityContext => builder.AndFromBatch(
                    (ctx, items) =>
                    {
                        var selector = keySelector.Compile();
                        var dict = items.ToDictionary(selector);
                        return batchFunc(ctx, dict.Keys)
                            .ToDictionary(
                                pair => dict[pair.Key],
                                pair => pair.Value);
                    },
                    resultBuilder),
                FromBatchType.EntityContextTask => builder.AndFromBatch(
                    (ctx, items) =>
                    {
                        var selector = keySelector.Compile();
                        var dict = items.ToDictionary(selector);
                        return Task.FromResult<IDictionary<TEntity, TResult>>(
                            batchFunc(ctx, dict.Keys)
                                .ToDictionary(
                                    pair => dict[pair.Key],
                                    pair => pair.Value));
                    },
                    resultBuilder),
                FromBatchType.Entity => builder.AndFromBatch(
                    items =>
                    {
                        var selector = keySelector.Compile();
                        var dict = items.ToDictionary(selector);
                        return batchFunc(contextFactory(), dict.Keys)
                            .ToDictionary(
                                pair => dict[pair.Key],
                                pair => pair.Value);
                    },
                    resultBuilder),
                FromBatchType.EntityTask => builder.AndFromBatch(
                    items =>
                    {
                        var selector = keySelector.Compile();
                        var dict = items.ToDictionary(selector);
                        return Task.FromResult<IDictionary<TEntity, TResult>>(
                            batchFunc(contextFactory(), dict.Keys)
                                .ToDictionary(
                                    pair => dict[pair.Key],
                                    pair => pair.Value));
                    },
                    resultBuilder),
                _ => throw new NotImplementedException(),
            };

            return result;
        }

        public static IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndAndFromBatch<TEntity, IEnumerable<object>, TExecutionContext> AndFromBatch<TEntity, TKey, TResult, TPreviousBatchResult, TExecutionContext>(
           this IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndAndFromBatch<TEntity, TPreviousBatchResult, TExecutionContext> builder,
           FromBatchType fromBatchType,
           Expression<Func<TEntity, TKey>> keySelector,
           Func<TExecutionContext> contextFactory,
           Func<TExecutionContext, IEnumerable<TKey>, IDictionary<TKey, TResult>> batchFunc,
           Action<IInlineObjectBuilder<TResult, TExecutionContext>> resultBuilder = null)
        {
            Guards.ThrowIfNull(builder, nameof(builder));

            var result = fromBatchType switch
            {
                FromBatchType.KeyContext => builder.AndFromBatch(
                    keySelector,
                    batchFunc,
                    resultBuilder),
                FromBatchType.KeyContextTask => builder.AndFromBatch(
                    keySelector,
                    (ctx, items) => Task.FromResult(batchFunc(ctx, items)),
                    resultBuilder),
                FromBatchType.Key => builder.AndFromBatch(
                    keySelector,
                    items => batchFunc(contextFactory(), items),
                    resultBuilder),
                FromBatchType.KeyTask => builder.AndFromBatch(
                    keySelector,
                    items => Task.FromResult(batchFunc(contextFactory(), items)),
                    resultBuilder),

                FromBatchType.EntityContext => builder.AndFromBatch(
                    (ctx, items) =>
                    {
                        var selector = keySelector.Compile();
                        var dict = items.ToDictionary(selector);
                        return batchFunc(ctx, dict.Keys)
                            .ToDictionary(
                                pair => dict[pair.Key],
                                pair => pair.Value);
                    },
                    resultBuilder),
                FromBatchType.EntityContextTask => builder.AndFromBatch(
                    (ctx, items) =>
                    {
                        var selector = keySelector.Compile();
                        var dict = items.ToDictionary(selector);
                        return Task.FromResult<IDictionary<TEntity, TResult>>(
                            batchFunc(ctx, dict.Keys)
                                .ToDictionary(
                                    pair => dict[pair.Key],
                                    pair => pair.Value));
                    },
                    resultBuilder),
                FromBatchType.Entity => builder.AndFromBatch(
                    items =>
                    {
                        var selector = keySelector.Compile();
                        var dict = items.ToDictionary(selector);
                        return batchFunc(contextFactory(), dict.Keys)
                            .ToDictionary(
                                pair => dict[pair.Key],
                                pair => pair.Value);
                    },
                    resultBuilder),
                FromBatchType.EntityTask => builder.AndFromBatch(
                    items =>
                    {
                        var selector = keySelector.Compile();
                        var dict = items.ToDictionary(selector);
                        return Task.FromResult<IDictionary<TEntity, TResult>>(
                            batchFunc(contextFactory(), dict.Keys)
                                .ToDictionary(
                                    pair => dict[pair.Key],
                                    pair => pair.Value));
                    },
                    resultBuilder),
                _ => throw new NotImplementedException(),
            };

            return result;
        }

        internal static ProjectionBase<TEntity, TExecutionContext> ResolveLoader<TEntity, TExecutionContext>(
            this RelationRegistry<TExecutionContext> registry,
            Type loaderType)
        {
            var registryResolveLoaderMethodInfo = ReflectionHelpers.GetMethodInfo(registry.ResolveLoader<DummyMutableLoader<TExecutionContext>, object>)
                .MakeGenericMethod(loaderType, typeof(TEntity));

            return registryResolveLoaderMethodInfo.InvokeAndHoistBaseException<ProjectionBase<TEntity, TExecutionContext>>(registry);
        }
    }
}
