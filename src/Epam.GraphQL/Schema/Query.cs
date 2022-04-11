// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Configuration.Implementations.Fields;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL
{
    public abstract class Query<TExecutionContext> : RootProjection<TExecutionContext>
    {
        private static MethodInfo? _connectionMethodInfo;
        private static MethodInfo? _groupConnectionMethodInfo;

        protected internal new IQueryField<TExecutionContext> Field(string name, string? deprecationReason = null)
        {
            ThrowIfIsNotConfiguring();

            var field = Configurator.AddField(
                new QueryField<TExecutionContext>(
                    Configurator.ConfigurationContext.Operation(nameof(Field))
                        .Argument(name),
                    Configurator,
                    name),
                deprecationReason);

            return field;
        }

        protected internal IRootLoaderField<TEntity, TExecutionContext> Field<TLoader, TEntity>(string name, string? deprecationReason = null)
            where TLoader : Loader<TEntity, TExecutionContext>, new()
            where TEntity : class
        {
            return Field(name, deprecationReason)
                .FromLoader<TLoader, TEntity>();
        }

        protected internal IConnectionField Connection<TChildLoader>(string name, string? deprecationReason = null)
            where TChildLoader : class
        {
            ThrowIfIsNotConfiguring();

            if (!ReflectionHelpers.TryFindMatchingGenericBaseType(typeof(TChildLoader), typeof(Loader<,>), out var baseLoaderType))
            {
                // TODO Make Dummy IConnectionField implementation
                var configurationContext = Configurator.ConfigurationContext.Operation<TChildLoader>(nameof(Connection))
                    .Argument(name);

                var msg = configurationContext
                    .GetError($"Cannot find the corresponding generic base type `{typeof(Loader<,>).HumanizedName()}` for type `{typeof(TChildLoader).HumanizedName()}`.", configurationContext);

                throw new ConfigurationException(msg);
            }

            _connectionMethodInfo ??= ReflectionHelpers.GetMethodInfo<string, string, IConnectionField>(
                Connection<DummyMutableLoader<TExecutionContext>, object>);

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
            ThrowIfIsNotConfiguring();

            if (!ReflectionHelpers.TryFindMatchingGenericBaseType(typeof(TChildLoader), typeof(Loader<,>), out var baseLoaderType))
            {
                var configurationContext = Configurator.ConfigurationContext.Operation<TChildLoader>(nameof(GroupConnection))
                    .Argument(name);
                var msg = configurationContext
                    .GetError($"Cannot find the corresponding generic base type `{typeof(Loader<,>).HumanizedName()}` for type `{typeof(TChildLoader).HumanizedName()}`.", configurationContext);
                throw new ConfigurationException(msg);
            }

            _groupConnectionMethodInfo ??= ReflectionHelpers.GetMethodInfo<string, string, IConnectionField>(
                GroupConnection<DummyMutableLoader<TExecutionContext>, object>);

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
            return (IConnectionField)Field(name, deprecationReason)
                .FromLoader<TChildLoader, TChildEntity>()
                .AsGroupConnection();
        }
    }
}
