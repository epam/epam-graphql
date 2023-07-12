// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Infrastructure;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.TaskBatcher;
using GraphQL;
using GraphQL.DataLoader;
using GraphQLParser.AST;
using Microsoft.Extensions.Logging;

namespace Epam.GraphQL.Extensions
{
    internal static class ResolveFieldContextExtensions
    {
        public static IEnumerable<TEntity> ExecuteQuery<TEntity>(this IResolveFieldContext context, Func<IResolveFieldContext, IQueryable<TEntity>> queryFactory)
        {
            return context.Bind(ctx => ctx.GetQueryExecuter().ToEnumerable(context.GetFieldConfigurationContext(), () => ctx.GetPath(), queryFactory(ctx)));
        }

        public static TReturnType ExecuteQuery<TEntity, TReturnType>(this IResolveFieldContext context, Func<IResolveFieldContext, IQueryable<TEntity>> queryFactory, Func<IQueryable<TEntity>, TReturnType> transform, string transformName)
        {
            return context.Bind(ctx => ctx.GetQueryExecuter().Execute(context.GetFieldConfigurationContext(), () => ctx.GetPath(), queryFactory(ctx), transform, transformName));
        }

        public static TExecutionContext GetUserContext<TExecutionContext>(this IResolveFieldContext context) => GetGraphQLContext<TExecutionContext>(context).ExecutionContext;

        public static ISchemaExecutionListener GetListener(this IResolveFieldContext context) => GetGraphQLContext(context).Listener;

        public static IQueryExecuter GetQueryExecuter(this IResolveFieldContext context) => GetGraphQLContext(context).QueryExecuter;

        public static ILogger GetLogger(this IResolveFieldContext context) => GetGraphQLContext(context).Logger;

        public static TResult Bind<TResult>(this IResolveFieldContext context, Func<IResolveFieldContext, TResult> func) => GetGraphQLContext(context).ResolveFieldContextBinder.Bind(context, func);

        public static IBatcher GetBatcher(this IResolveFieldContext context) => GetGraphQLContext(context).Batcher;

        public static IProfiler GetProfiler(this IResolveFieldContext context) => GetGraphQLContext(context).Profiler;

        public static IDataContext GetDataContext(this IResolveFieldContext context)
        {
            return GetGraphQLContext(context).DataContext ?? throw new NotSupportedException();
        }

        public static bool HasTotalCount(this IResolveFieldContext context) => context.SubFields?.ContainsKey("totalCount") ?? false;

        public static bool HasItems(this IResolveFieldContext context) => context.SubFields?.ContainsKey("items") ?? false;

        public static bool HasEdges(this IResolveFieldContext context) => context.SubFields?.ContainsKey("edges") ?? false;

        public static bool HasEndCursor(this IResolveFieldContext context)
        {
            if (context.SubFields != null && context.SubFields.TryGetValue("pageInfo", out var pageInfo))
            {
                return pageInfo.Field?.GetSubFieldsNames(context.Document.Definitions.OfType<GraphQLFragmentDefinition>(), c => !c.Name.StringValue.Equals("endCursor", StringComparison.OrdinalIgnoreCase)).Any() ?? false;
            }

            return false;
        }

        public static string GetPath(this IResolveFieldContext context) => string.Join(".", context.Path.SafeNull());

        public static IChainConfigurationContext GetFieldConfigurationContext(this IResolveFieldContext context)
        {
            var result = context.FieldDefinition.Metadata["CONFIGURATION_CONTEXT"];
            Guards.AssertIfNull(result);

            return (IChainConfigurationContext)result;
        }

        public static ExecutionError CreateFieldExecutionError(this IResolveFieldContext context, Exception innerException)
        {
            var configurationContext = context.GetFieldConfigurationContext();
            var path = context.GetPath();
            throw new ExecutionError(configurationContext.GetRuntimeError($"Error during resolving field `{path}`. See an inner exception for details.", configurationContext), innerException);
        }

        public static void LogFieldExecutionError(this IResolveFieldContext context, Exception innerException)
        {
            var configurationContext = context.GetFieldConfigurationContext();
            var path = context.GetPath();
            var logger = context.GetLogger();
            logger.Log(
                Constants.Logging.ExecutionError.Level,
                Constants.Logging.ExecutionError.EventId,
                innerException,
                configurationContext.GetRuntimeError($"Error during resolving field `{path}`.", configurationContext));
        }

        public static string? GetSearch(this IResolveFieldContext context) => GetArgument<string>(context, "search");

        public static int? GetFirst(this IResolveFieldContext context) => GetArgument<int?>(context, "first");

        public static int? GetLast(this IResolveFieldContext context) => GetArgument<int?>(context, "last");

        public static int? GetAfter(this IResolveFieldContext context)
        {
            var arg = GetArgument<string>(context, "after");
            if (int.TryParse(arg, out var result))
            {
                return result;
            }

            return null;
        }

        public static int? GetBefore(this IResolveFieldContext context)
        {
            var arg = GetArgument<string>(context, "before");
            if (int.TryParse(arg, out var result))
            {
                return result;
            }

            return null;
        }

        public static IEnumerable<SortingOption> GetSorting(this IResolveFieldContext context) => GetArgument(context, "sorting", Enumerable.Empty<SortingOption>());

        public static IEnumerable<string> GetQueriedFields(this IResolveFieldContext context)
        {
            var result = context.FieldAst.GetSubFieldsNames(context.Document.Definitions.OfType<GraphQLFragmentDefinition>(), _ => true);
            return result;
        }

        public static IEnumerable<string> GetConnectionQueriedFields(this IResolveFieldContext context)
        {
            GraphQLField? itemsField = null;
            GraphQLField? edgesField = null;

            if (context.SubFields != null && context.SubFields.TryGetValue("items", out var items))
            {
                itemsField = items.Field;
            }

            if (context.SubFields != null && context.SubFields.TryGetValue("edges", out var edges))
            {
                edgesField = GetSubField(context, edges.Field, "node");
            }

            return itemsField.GetSubFieldsNames(context.Document.Definitions.OfType<GraphQLFragmentDefinition>(), f => true)
                .Concat(edgesField.GetSubFieldsNames(context.Document.Definitions.OfType<GraphQLFragmentDefinition>(), f => true)).Distinct(StringComparer.Ordinal);
        }

        public static IEnumerable<string> GetGroupConnectionQueriedFields(this IResolveFieldContext context)
        {
            GraphQLField? itemsField = null;
            GraphQLField? edgesField = null;

            if (context.SubFields != null && context.SubFields.TryGetValue("items", out var items))
            {
                itemsField = GetSubField(context, items.Field, "item");
            }

            if (context.SubFields != null && context.SubFields.TryGetValue("edges", out var edges))
            {
                edgesField = GetSubField(context, edges.Field, "node");
                if (edgesField != null)
                {
                    edgesField = GetSubField(context, edgesField, "item");
                }
            }

            return itemsField.GetSubFieldsNames(context.Document.Definitions.OfType<GraphQLFragmentDefinition>(), f => true)
                .Concat(edgesField.GetSubFieldsNames(context.Document.Definitions.OfType<GraphQLFragmentDefinition>(), f => true)).Distinct(StringComparer.Ordinal);
        }

        public static bool HasGroupConnectionItemField(this IResolveFieldContext context)
        {
            GraphQLField? itemsField = null;
            GraphQLField? edgesField = null;

            if (context.SubFields != null && context.SubFields.TryGetValue("items", out var items))
            {
                itemsField = GetSubField(context, items.Field, "item");
            }

            if (context.SubFields != null && context.SubFields.TryGetValue("edges", out var edges))
            {
                edgesField = GetSubField(context, edges.Field, "node");
                if (edgesField != null)
                {
                    edgesField = GetSubField(context, edgesField, "item");
                }
            }

            return itemsField != null || edgesField != null;
        }

        public static IEnumerable<string> GetGroupConnectionAggregateQueriedFields(this IResolveFieldContext context)
        {
            GraphQLField? itemsField = null;
            GraphQLField? edgesField = null;

            if (context.SubFields != null && context.SubFields.TryGetValue("items", out var items))
            {
                itemsField = items.Field;
            }

            if (context.SubFields != null && context.SubFields.TryGetValue("edges", out var edges))
            {
                edgesField = GetSubField(context, edges.Field, "node");
            }

            return itemsField.GetSubFieldsNames(context.Document.Definitions.OfType<GraphQLFragmentDefinition>(), c => !c.Name.StringValue.Equals("item", StringComparison.OrdinalIgnoreCase))
                .Concat(edgesField.GetSubFieldsNames(context.Document.Definitions.OfType<GraphQLFragmentDefinition>(), c => !c.Name.StringValue.Equals("item", StringComparison.OrdinalIgnoreCase))).Distinct(StringComparer.Ordinal);
        }

        public static object GetFilterValue(this IResolveFieldContext context, Type filterType)
        {
            return GetArgument(context, filterType, "filter", new Dictionary<string, object>());
        }

        public static IDataLoader<TOuterEntity, IGrouping<TOuterEntity, TTransformedInnerEntity>> Get<TOuterEntity, TInnerEntity, TTransformedInnerEntity>(
            this IResolveFieldContext context,
            Func<IResolveFieldContext, IQueryable<TInnerEntity>> queryFactory,
            Func<IResolveFieldContext, IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)>>? sorters,
            Func<IResolveFieldContext, Expression<Func<TInnerEntity, TTransformedInnerEntity>>> transform,
            LambdaExpression outerExpression,
            LambdaExpression innerExpression,
            ILoaderHooksExecuter<TTransformedInnerEntity>? hooksExecuter)
        {
            var batcher = context.GetBatcher();

            var result = batcher
                .Get<TOuterEntity, TInnerEntity, TTransformedInnerEntity>(
                    context,
                    queryFactory,
                    sorters,
                    transform,
                    outerExpression,
                    innerExpression,
                    hooksExecuter);

            return result;
        }

        private static GraphQLField? GetSubField(IResolveFieldContext context, GraphQLField? parent, string subFieldName)
        {
            if (parent == null)
            {
                return null;
            }

            var result = parent.SelectionSet?.Selections.OfType<GraphQLField>().FirstOrDefault(f => f.Name.StringValue == subFieldName);

            if (result != null)
            {
                return result;
            }

            var fragmentNames = parent.SelectionSet?.Selections.OfType<GraphQLFragmentSpread>().Select(fragment => fragment.FragmentName.Name.StringValue);

            return context.Document.Definitions.OfType<GraphQLFragmentDefinition>()
                .Where(fragment => fragmentNames?.Contains(fragment.FragmentName.Name.StringValue) ?? false)
                .SelectMany(f => f.SelectionSet.Selections.OfType<GraphQLField>())
                .FirstOrDefault();
        }

        [return: NotNullIfNotNull("defaultValue")]
        private static TType? GetArgument<TType>(IResolveFieldContext context, string name, TType? defaultValue = default)
        {
            return (TType?)GetArgument(context, typeof(TType), name, defaultValue);
        }

        [return: NotNullIfNotNull("defaultValue")]
        private static object? GetArgument(IResolveFieldContext context, Type argumentType, string name, object? defaultValue)
        {
            if (!HasArgument(context, name) || context.Arguments![name].Value == null)
            {
                if (defaultValue is IDictionary<string, object?> dict)
                {
                    return dict.ToObject(argumentType);
                }

                return defaultValue;
            }

            var arg = context.Arguments[name];
            if (arg.Value is IDictionary<string, object?> inputObject)
            {
                if (typeof(IDictionary<string, object?>).IsAssignableFrom(argumentType))
                {
                    return inputObject;
                }

                return inputObject.ToObject(argumentType);
            }

            return arg.Value.GetPropertyValue(argumentType);
        }

        private static bool HasArgument(IResolveFieldContext context, string argumentName)
        {
            return context.Arguments?.ContainsKey(argumentName) ?? false;
        }

        private static GraphQLContext GetGraphQLContext(this IResolveFieldContext context)
        {
            var result = (GraphQLContext?)context.UserContext["ctx"];

            Guards.AssertIfNull(result);

            return result;
        }

        private static GraphQLContext<TExecutionContext> GetGraphQLContext<TExecutionContext>(this IResolveFieldContext context)
        {
            var result = (GraphQLContext<TExecutionContext>?)context.UserContext["ctx"];

            Guards.AssertIfNull(result);

            return result;
        }
    }
}
