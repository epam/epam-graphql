// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Builders.Common;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Builders.MutableLoader;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Enums;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Tests.Helpers;

namespace Epam.GraphQL.Tests
{
    public static class TestExtensions
    {
        public static ILoaderField<TChildEntity, TExecutionContext> Field<TChildEntity, TExecutionContext>(this Query<TExecutionContext> query, string name, Type loaderType)
        {
            var methodInfo = typeof(Query<TExecutionContext>).GetNonPublicGenericMethod(
                nameof(Query<TExecutionContext>.Field),
                new[] { loaderType, typeof(TChildEntity) },
                new[] { typeof(string), typeof(string) });

            return methodInfo.InvokeAndHoistBaseException<ILoaderField<TChildEntity, TExecutionContext>>(query, name, null);
        }

        public static ILoaderField<TChildEntity, TExecutionContext> FromLoader<TChildEntity, TExecutionContext>(this IQueryField<TExecutionContext> builder, Type loaderType)
        {
            var methodInfo = typeof(IQueryField<TExecutionContext>).GetPublicGenericMethod(
                nameof(IQueryField<TExecutionContext>.FromLoader),
                new[] { loaderType, typeof(TChildEntity) },
                Type.EmptyTypes);

            return methodInfo.InvokeAndHoistBaseException<ILoaderField<TChildEntity, TExecutionContext>>(builder);
        }

        public static void WithSearch<TThis, TChildEntity, TExecutionContext>(this ISearchableField<TThis, TChildEntity, TExecutionContext> field, Type searcherType)
        {
            var methodInfo = typeof(ISearchableField<TThis, TChildEntity, TExecutionContext>).GetPublicGenericMethod(
                nameof(ISearchableField<TThis, TChildEntity, TExecutionContext>.WithSearch),
                new[] { searcherType },
                Type.EmptyTypes);

            methodInfo.InvokeAndHoistBaseException(field);
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

        public static IFromLoaderBuilder<TEntity, TChildEntity, TChildEntity, TExecutionContext> FromLoader<TEntity, TChildEntity, TExecutionContext>(
            this IHasFromLoader<TEntity, TExecutionContext> builder,
            Type childLoaderType,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            RelationType relationType = RelationType.Association,
            Expression<Func<TChildEntity, TEntity>> navigationProperty = null,
            Expression<Func<TEntity, TChildEntity>> reverseNavigationProperty = null)
            where TEntity : class
            where TChildEntity : class
        {
            var methodInfo = typeof(IHasFromLoader<TEntity, TExecutionContext>).GetPublicGenericMethod(
                nameof(IHasFromLoader<TEntity, TExecutionContext>.FromLoader),
                new[] { childLoaderType, typeof(TChildEntity) },
                new[] { typeof(Expression<Func<TEntity, TChildEntity, bool>>), typeof(RelationType), typeof(Expression<Func<TChildEntity, TEntity>>), typeof(Expression<Func<TEntity, TChildEntity>>) });

            return methodInfo.InvokeAndHoistBaseException<IFromLoaderBuilder<TEntity, TChildEntity, TChildEntity, TExecutionContext>>(
                builder,
                condition,
                relationType,
                navigationProperty,
                reverseNavigationProperty);
        }

        public static IFromLoaderInlineObjectBuilder<TEntity, TChildEntity, TChildEntity> FromLoader<TEntity, TChildEntity, TExecutionContext>(
            this IInlineObjectFieldBuilder<TEntity, TExecutionContext> builder,
            Type childLoaderType,
            Expression<Func<TEntity, TChildEntity, bool>> condition)
            where TEntity : class
            where TChildEntity : class
        {
            var methodInfo = typeof(IInlineObjectFieldBuilder<TEntity, TExecutionContext>).GetPublicGenericMethod(
                nameof(IInlineObjectFieldBuilder<TEntity, TExecutionContext>.FromLoader),
                new[] { childLoaderType, typeof(TChildEntity) },
                new[] { typeof(Expression<Func<TEntity, TChildEntity, bool>>) });

            return methodInfo.InvokeAndHoistBaseException<IFromLoaderInlineObjectBuilder<TEntity, TChildEntity, TChildEntity>>(
                builder,
                condition);
        }

        public static void ConfigureFrom<TEntity, TExecutionContext>(
            this IInlineObjectBuilder<TEntity, TExecutionContext> builder,
            Type projectionType)
            where TEntity : class
        {
            var methodInfo = typeof(IInlineObjectBuilder<TEntity, TExecutionContext>).GetPublicGenericMethod(
                nameof(IInlineObjectBuilder<TEntity, TExecutionContext>.ConfigureFrom),
                new[] { projectionType },
                Type.EmptyTypes);

            methodInfo.InvokeAndHoistBaseException(builder);
        }

        public static IHasFilterableAndSortableAndOnWriteAndEditableAndMandatoryForUpdate<TEntity, TReturnType, TFilterValueType, TExecutionContext> ReferencesTo<TParentEntity, TEntity, TReturnType, TFilterValueType, TExecutionContext>(
            this IHasFilterableAndSortableAndOnWriteAndEditableAndMandatoryForUpdateAndReferenceTo<TEntity, TReturnType, TFilterValueType, TExecutionContext> builder,
            Type parentLoaderType,
            Expression<Func<TParentEntity, TReturnType>> parentProperty,
            Expression<Func<TEntity, TParentEntity>> navigationProperty,
            RelationType relationType)
            where TParentEntity : class
        {
            var foreignKeyMethodInfo = typeof(IHasFilterableAndSortableAndOnWriteAndEditableAndMandatoryForUpdateAndReferenceTo<TEntity, TReturnType, TFilterValueType, TExecutionContext>).GetPublicGenericMethod(
                nameof(ReferencesTo),
                new[] { typeof(TParentEntity), parentLoaderType },
                new[] { typeof(Expression<Func<TParentEntity, TReturnType>>), typeof(Expression<Func<TEntity, TParentEntity>>), typeof(RelationType) });
            return (IHasFilterableAndSortableAndOnWriteAndEditableAndMandatoryForUpdate<TEntity, TReturnType, TFilterValueType, TExecutionContext>)foreignKeyMethodInfo.InvokeAndHoistBaseException(builder, parentProperty, navigationProperty, relationType);
        }
    }
}
