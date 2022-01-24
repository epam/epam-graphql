// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Epam.GraphQL.Builders.Query;
using Epam.GraphQL.Builders.Query.Implementations;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Configuration.Implementations.Fields;
using Epam.GraphQL.Configuration.Implementations.Fields.ChildFields;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Sorters.Implementations;

namespace Epam.GraphQL
{
    public abstract class Query<TExecutionContext> : RootProjection<TExecutionContext>
    {
        private static MethodInfo? _connectionMethodInfo;
        private static MethodInfo? _groupConnectionMethodInfo;

        protected internal new IQueryFieldBuilder<TExecutionContext> Field(string name, string? deprecationReason = null)
        {
            ThrowIfIsNotConfiguring();
            var field = Configurator.AddField(new QueryField<TExecutionContext>(Registry, Configurator, name), deprecationReason);
            return new QueryFieldBuilder<QueryField<TExecutionContext>, TExecutionContext>(field);
        }

        protected internal ILoaderField<TEntity, TExecutionContext> Field<TLoader, TEntity>(string name, string? deprecationReason = null)
            where TLoader : Loader<TEntity, TExecutionContext>, new()
            where TEntity : class
        {
            return Field(name, deprecationReason)
                .FromLoader<TLoader, TEntity>();
        }

        protected internal IConnectionField Connection<TChildLoader>(string name, string? deprecationReason = null)
            where TChildLoader : class
        {
            var baseLoaderType = TypeHelpers.FindMatchingGenericBaseType(typeof(TChildLoader), typeof(Loader<,>));
            if (baseLoaderType == null)
            {
                throw new ArgumentException($"Cannot find the corresponding base type for loader: {typeof(TChildLoader)}");
            }

            _connectionMethodInfo ??= typeof(Query<TExecutionContext>).GetNonPublicGenericMethod(method => method
                .HasName(nameof(Connection))
                .HasTwoGenericTypeParameters()
                .Parameter<string>()
                .Parameter<string?>());

            var field = _connectionMethodInfo
                .MakeGenericMethod(typeof(TChildLoader), baseLoaderType.GenericTypeArguments[0])
                .InvokeAndHoistBaseException<IConnectionField>(
                    this,
                    name,
                    deprecationReason);

            return field;
        }

        protected internal IConnectionField Connection<TChildLoader, TChildEntity>(
            string name,
            Expression<Func<IQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>>> order,
            string? deprecationReason = null)
            where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
            where TChildEntity : class
        {
            return (IConnectionField)Field(name, deprecationReason)
                .FromLoader<TChildLoader, TChildEntity>()
                .AsConnection(order);
        }

        protected internal IConnectionField GroupConnection<TChildLoader>(string name, string? deprecationReason = null)
            where TChildLoader : class
        {
            var baseLoaderType = TypeHelpers.FindMatchingGenericBaseType(typeof(TChildLoader), typeof(Loader<,>));

            if (baseLoaderType == null)
            {
                throw new ArgumentException($"Cannot find the corresponding base type for loader: {typeof(TChildLoader)}");
            }

            _groupConnectionMethodInfo ??= typeof(Query<TExecutionContext>).GetNonPublicGenericMethod(method => method
                .HasName(nameof(GroupConnection))
                .HasTwoGenericTypeParameters()
                .Parameter<string>()
                .Parameter<string?>());

            var field = _groupConnectionMethodInfo
                .MakeGenericMethod(typeof(TChildLoader), baseLoaderType.GenericTypeArguments[0])
                .InvokeAndHoistBaseException<IConnectionField>(
                    this,
                    name,
                    deprecationReason);

            return field;
        }

        private IConnectionField Connection<TChildLoader, TChildEntity>(
            string name,
            string deprecationReason)
            where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
            where TChildEntity : class
        {
            return (IConnectionField)Field(name, deprecationReason)
                .FromLoader<TChildLoader, TChildEntity>()
                .AsConnection();
        }

        private IConnectionField GroupConnection<TChildLoader, TChildEntity>(string name, string? deprecationReason)
            where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
            where TChildEntity : class
        {
            ThrowIfIsNotConfiguring();
            var graphResultType = Configurator.GetGraphQLTypeDescriptor<TChildLoader, TChildEntity>();
            var field = new GroupLoaderField<object, TChildLoader, TChildEntity, TExecutionContext>(
                Registry,
                Configurator,
                name,
                graphResultType,
                arguments: null,
                naturalSorters: SortingHelpers.Empty);

            return Configurator.AddField(field, deprecationReason);
        }
    }
}
