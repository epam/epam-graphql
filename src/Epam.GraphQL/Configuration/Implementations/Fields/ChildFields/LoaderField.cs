// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;
using Epam.GraphQL.Enums;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Search;
using Epam.GraphQL.Sorters.Implementations;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ChildFields
{
    internal class LoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext> :
        LoaderFieldBase<
            LoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>,
            ILoaderField<TChildEntity, TExecutionContext>,
            TEntity,
            TChildLoader,
            TChildEntity,
            TExecutionContext>,
        ILoaderField<TChildEntity, TExecutionContext>
        where TEntity : class
        where TChildEntity : class
        where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
    {
        public LoaderField(
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
        }

        private LoaderField(
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IQueryableResolver<TEntity, TChildEntity, TExecutionContext> resolver,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType,
            TChildLoader loader,
            LazyQueryArguments? arguments,
            ISearcher<TChildEntity, TExecutionContext>? searcher,
            IEnumerable<(LambdaExpression SortExpression, SortDirection SortDirection)> naturalSorters)
            : base(
                  parent,
                  name,
                  resolver,
                  elementGraphType,
                  loader,
                  arguments,
                  searcher,
                  naturalSorters)
        {
        }

        public IVoid AsConnection(Expression<Func<IQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>>> naturalOrder)
        {
            var connectionField = new ConnectionLoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>(
                Parent,
                Name,
                QueryableFieldResolver,
                ElementGraphType,
                Arguments,
                Searcher,
                naturalOrder.GetSorters());
            return ApplyField(connectionField);
        }

        public IVoid AsConnection()
        {
            var connectionField = new ConnectionLoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>(
                Parent,
                Name,
                QueryableFieldResolver,
                ElementGraphType,
                Arguments,
                Searcher,
                Loader.ApplyNaturalOrderBy(Enumerable.Empty<TChildEntity>().AsQueryable()).GetSorters());
            return ApplyField(connectionField);
        }

        protected override LoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext> ReplaceResolver(IQueryableResolver<TEntity, TChildEntity, TExecutionContext> resolver)
        {
            var queryableField = new LoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>(
                Parent,
                Name,
                resolver,
                ElementGraphType,
                Loader,
                Arguments,
                Searcher,
                NaturalSorters);

            return queryableField;
        }
    }

#pragma warning disable CA1501
    internal sealed class LoaderField<TLoader, TChildLoader, TEntity, TChildEntity, TExecutionContext> : LoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>
#pragma warning restore CA1501
        where TLoader : Loader<TEntity, TExecutionContext>, new()
        where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
        where TEntity : class
        where TChildEntity : class
    {
        public LoaderField(
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            RelationType relationType,
            Expression<Func<TChildEntity, TEntity>> navigationProperty,
            Expression<Func<TEntity, TChildEntity>> reverseNavigationProperty,
            IGraphTypeDescriptor<TChildEntity, TExecutionContext> elementGraphType)
            : base(parent, name, condition, elementGraphType, arguments: null, searcher: null, naturalSorters: SortingHelpers.Empty)
        {
            parent.Registry.Register(typeof(TChildLoader), typeof(TLoader), condition.SwapOperands(), reverseNavigationProperty, navigationProperty, relationType);
        }
    }
}
