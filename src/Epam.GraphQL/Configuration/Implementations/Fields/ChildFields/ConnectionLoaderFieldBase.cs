// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Filters;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Search;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ChildFields
{
    internal abstract class ConnectionLoaderFieldBase<TThis, TEntity, TChildLoader, TChildEntity, TExecutionContext> :
        LoaderFieldBase<
            TThis,
            IConnectionField<TChildEntity, TExecutionContext>,
            TEntity,
            TChildLoader,
            TChildEntity,
            TExecutionContext>,
        IConnectionField<TChildEntity, TExecutionContext>,
        IConnectionField,
        IVoid
        where TEntity : class
        where TChildEntity : class
        where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
        where TThis : ConnectionLoaderFieldBase<TThis, TEntity, TChildLoader, TChildEntity, TExecutionContext>
    {
        private static MethodInfo? _withFilterMethodInfo;
        private static MethodInfo? _withSearchMethodInfo;

        protected ConnectionLoaderFieldBase(
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
            Initialize();
        }

        protected ConnectionLoaderFieldBase(
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TChildEntity, bool>>? condition,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType,
            LazyQueryArguments? arguments,
            ISearcher<TChildEntity, TExecutionContext>? searcher,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> naturalSorters)
            : base(
                  parent,
                  name,
                  condition,
                  elementGraphType,
                  arguments,
                  searcher,
                  naturalSorters)
        {
            Initialize();
        }

        public IConnectionField WithFilter<TFilter>()
        {
            var filterBaseType = ReflectionHelpers.FindMatchingGenericBaseType(typeof(TFilter), typeof(Filter<,,>));
            var filterArgument = filterBaseType.GetGenericArguments().Single(type => typeof(Input).IsAssignableFrom(type));

            _withFilterMethodInfo ??= ReflectionHelpers.GetMethodInfo(
                WithFilter<Filter<TChildEntity, Input, TExecutionContext>, Input>);

            var withFilter = _withFilterMethodInfo.MakeGenericMethod(typeof(TFilter), filterArgument);
            return withFilter.InvokeAndHoistBaseException<IConnectionField>(this);
        }

        IConnectionField IConnectionField.WithSearch<TSearcher>()
        {
            var baseType = ReflectionHelpers.FindMatchingGenericBaseType(typeof(TSearcher), typeof(ISearcher<,>));

            _withSearchMethodInfo ??= ReflectionHelpers.GetMethodInfo(
                WithSearch<ISearcher<TChildEntity, TExecutionContext>>);

            var withSearcher = _withSearchMethodInfo.MakeGenericMethod(typeof(TSearcher));
            return withSearcher.InvokeAndHoistBaseException<IConnectionField>(this);
        }

        private void Initialize()
        {
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
    }
}
