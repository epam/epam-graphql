// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Infrastructure;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.TaskBatcher;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Language.AST;

#nullable enable

namespace Epam.GraphQL.Extensions
{
    internal static class ResolveFieldContextExtensions
    {
        public static IEnumerable<TEntity> ExecuteQuery<TEntity>(this IResolveFieldContext context, Func<IResolveFieldContext, IQueryable<TEntity>> queryFactory)
        {
            return context.Bind(ctx => ctx.GetQueryExecuter().ToEnumerable(() => ctx.GetPath(), queryFactory(ctx)));
        }

        public static TReturnType ExecuteQuery<TEntity, TReturnType>(this IResolveFieldContext context, Func<IResolveFieldContext, IQueryable<TEntity>> queryFactory, Func<IQueryable<TEntity>, TReturnType> transform, string transformName)
        {
            return context.Bind(ctx => ctx.GetQueryExecuter().Execute(() => ctx.GetPath(), queryFactory(ctx), transform, transformName));
        }

        public static TExecutionContext GetUserContext<TExecutionContext>(this IResolveFieldContext context) => ((GraphQLContext<TExecutionContext>)context.UserContext["ctx"]).ExecutionContext;

        public static ISchemaExecutionListener GetListener(this IResolveFieldContext context) => ((GraphQLContext)context.UserContext["ctx"]).Listener;

        public static IQueryExecuter GetQueryExecuter(this IResolveFieldContext context) => ((GraphQLContext)context.UserContext["ctx"]).QueryExecuter;

        public static TResult Bind<TResult>(this IResolveFieldContext context, Func<IResolveFieldContext, TResult> func) => ((GraphQLContext)context.UserContext["ctx"]).ResolveFieldContextBinder.Bind(context, func);

        public static Func<T, TResult> Bind<T, TResult>(this IResolveFieldContext context, Func<IResolveFieldContext, T, TResult> func) => ((GraphQLContext)context.UserContext["ctx"]).ResolveFieldContextBinder.Bind(context, func);

        public static IBatcher GetBatcher(this IResolveFieldContext context) => ((GraphQLContext)context.UserContext["ctx"]).Batcher;

        public static IProfiler GetProfiler(this IResolveFieldContext context) => ((GraphQLContext)context.UserContext["ctx"]).Profiler;

        public static IDataContext? GetDataContext(this IResolveFieldContext context) => ((GraphQLContext)context.UserContext["ctx"]).DataContext;

        public static bool HasTotalCount(this IResolveFieldContext context) => context.SubFields.ContainsKey("totalCount");

        public static bool HasItems(this IResolveFieldContext context) => context.SubFields.ContainsKey("items");

        public static bool HasEdges(this IResolveFieldContext context) => context.SubFields.ContainsKey("edges");

        public static bool HasEndCursor(this IResolveFieldContext context)
        {
            if (!context.SubFields.TryGetValue("pageInfo", out var pageInfo))
            {
                return false;
            }

            return pageInfo.GetSubFieldsNames(context.Fragments, c => !c.Name.Equals("endCursor", StringComparison.OrdinalIgnoreCase)).Any();
        }

        public static string GetPath(this IResolveFieldContext context) => string.Join(".", context.Path.SafeNull());

        public static IEnumerable<string>? GetFilterFieldNames(this IResolveFieldContext context) => GetArgument(context, "filter", new Dictionary<string, object>())?.Keys;

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

        public static IEnumerable<SortingOption>? GetSorting(this IResolveFieldContext context) => GetArgument(context, "sorting", Enumerable.Empty<SortingOption>());

        public static IEnumerable<string> GetQueriedFields(this IResolveFieldContext context)
        {
            var result = context.FieldAst.GetSubFieldsNames(context.Fragments, _ => true);
            return result;
        }

        public static IEnumerable<string> GetConnectionQueriedFields(this IResolveFieldContext context)
        {
            context.SubFields.TryGetValue("items", out var items);
            if (context.SubFields.TryGetValue("edges", out var edges))
            {
                edges = GetSubField(context, edges, "node");
            }

            return items.GetSubFieldsNames(context.Fragments, f => true).Concat(edges.GetSubFieldsNames(context.Fragments, f => true)).Distinct();
        }

        public static IEnumerable<string> GetGroupConnectionQueriedFields(this IResolveFieldContext context)
        {
            if (context.SubFields.TryGetValue("items", out var items))
            {
                items = GetSubField(context, items, "item");
            }

            if (context.SubFields.TryGetValue("edges", out var edges))
            {
                edges = GetSubField(context, edges, "node");
                if (edges != null)
                {
                    edges = GetSubField(context, edges, "item");
                }
            }

            return items.GetSubFieldsNames(context.Fragments, f => true).Concat(edges.GetSubFieldsNames(context.Fragments, f => true)).Distinct();
        }

        public static IEnumerable<string> GetGroupConnectionAggregateQueriedFields(this IResolveFieldContext context)
        {
            context.SubFields.TryGetValue("items", out var items);

            if (context.SubFields.TryGetValue("edges", out var edges))
            {
                edges = GetSubField(context, edges, "node");
            }

            return items.GetSubFieldsNames(context.Fragments, c => !c.Name.Equals("item", StringComparison.OrdinalIgnoreCase))
                .Concat(edges.GetSubFieldsNames(context.Fragments, c => !c.Name.Equals("item", StringComparison.OrdinalIgnoreCase))).Distinct();
        }

        public static object? GetFilterValue(this IResolveFieldContext context, Type filterType)
        {
            return GetArgument(context, filterType, "filter", new Dictionary<string, object>());
        }

        public static IDataLoader<TOuterEntity, IGrouping<TOuterEntity, TTransformedInnerEntity>> Get<TOuterEntity, TInnerEntity, TTransformedInnerEntity>(
            this IResolveFieldContext context,
            Func<IResolveFieldContext, IQueryable<TInnerEntity>> queryFactory,
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
                    transform,
                    outerExpression,
                    innerExpression,
                    hooksExecuter);

            return result;
        }

        private static Field? GetSubField(IResolveFieldContext context, Field parent, string subFieldName)
        {
            if (parent == null)
            {
                return null;
            }

            var result = parent.SelectionSet.Children.OfType<Field>().FirstOrDefault(f => f.Name == subFieldName);
            if (result != null)
            {
                return result;
            }

            var fragmentNames = parent.SelectionSet.Children.OfType<FragmentSpread>().Select(fragment => fragment.Name);

            return context.Fragments
                .Where(fragment => fragmentNames.Contains(fragment.Name))
                .SelectMany(f => f.SelectionSet.Children.OfType<Field>())
                .FirstOrDefault();
        }

        private static TType? GetArgument<TType>(IResolveFieldContext context, string name, TType? defaultValue = default)
        {
            return (TType?)GetArgument(context, typeof(TType), name, defaultValue);
        }

        private static object? GetArgument(IResolveFieldContext context, Type argumentType, string name, object? defaultValue)
        {
            if (!HasArgument(context, name) || context.Arguments[name] == null)
            {
                if (defaultValue is IDictionary<string, object> dict)
                {
                    return dict.ToObject(argumentType);
                }

                return defaultValue;
            }

            var arg = context.Arguments[name];
            if (arg is IDictionary<string, object> inputObject)
            {
                if (typeof(IDictionary<string, object>).IsAssignableFrom(argumentType))
                {
                    return inputObject;
                }

                return inputObject.ToObject(argumentType);
            }

            return arg.GetPropertyValue(argumentType);
        }

        private static bool HasArgument(IResolveFieldContext context, string argumentName)
        {
            return context.Arguments?.ContainsKey(argumentName) ?? false;
        }
    }
}
