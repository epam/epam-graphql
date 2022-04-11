// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Tests.TestData;
using GraphQL;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json.Linq;
using NSubstitute;

namespace Epam.GraphQL.Tests.Helpers
{
    public static class ExecuteHelpers
    {
        public static ExecutionResult ExecuteQuery(Type queryType, Type mutationType, string request, IDataContext dataContext = null, Action<SchemaOptionsBuilder<TestUserContext>> configure = null)
        {
            var executeQueryMethodInfo = typeof(ExecuteHelpers).GetNonPublicGenericMethod(
                nameof(ExecuteQuery),
                new[] { queryType, mutationType },
                new[] { typeof(string), typeof(IDataContext), typeof(Action<SchemaOptionsBuilder<TestUserContext>>) });
            return executeQueryMethodInfo.InvokeAndHoistBaseException<ExecutionResult>(null, request, dataContext, configure);
        }

        public static ExecutionResult ExecuteQuery<TExecutionContext>(Type queryType, Type mutationType, Action<SchemaOptionsBuilder<TExecutionContext>> configure, Action<SchemaExecutionOptionsBuilder<TExecutionContext>> configureExecutionOptions)
        {
            var executeQueryMethodInfo = typeof(ExecuteHelpers).GetNonPublicGenericMethod(
                nameof(ExecuteQuery),
                new[] { queryType, mutationType, typeof(TExecutionContext) },
                new[] { typeof(Action<SchemaOptionsBuilder<TExecutionContext>>), typeof(Action<SchemaExecutionOptionsBuilder<TExecutionContext>>) });
            return executeQueryMethodInfo.InvokeAndHoistBaseException<ExecutionResult>(null, configure, configureExecutionOptions);
        }

        public static Dictionary<string, object> Deserialize(string json)
        {
            return (Dictionary<string, object>)ToObject(JToken.Parse(json));
        }

        public static ISchemaExecuter<TExecutionContext> CreateSchemaExecuter<TExecutionContext>(
            Type queryType,
            Action<SchemaOptionsBuilder<TExecutionContext>> configure)
        {
            var createSchemaExecuterMethodInfo = typeof(ExecuteHelpers).GetNonPublicGenericMethod(
                nameof(CreateSchemaExecuter),
                new[] { queryType, typeof(TExecutionContext) },
                new[] { typeof(Action<SchemaOptionsBuilder<TExecutionContext>>) });

            return createSchemaExecuterMethodInfo.InvokeAndHoistBaseException<ISchemaExecuter<TExecutionContext>>(null, configure);
        }

        private static ISchemaExecuter<TQuery, TExecutionContext> CreateSchemaExecuter<TQuery, TExecutionContext>(
            Action<SchemaOptionsBuilder<TExecutionContext>> configure)
            where TQuery : Query<TExecutionContext>, new()
        {
            var services = new ServiceCollection();
            services.AddEpamGraphQLSchema<TQuery, TExecutionContext>(configure);
            var serviceProvider = services.BuildServiceProvider();

            var schemaExecuter = serviceProvider.GetRequiredService<ISchemaExecuter<TQuery, TExecutionContext>>();
            return schemaExecuter;
        }

        private static ExecutionResult ExecuteQuery<TQuery, TMutation, TExecutionContext>(Action<SchemaOptionsBuilder<TExecutionContext>> configure, Action<SchemaExecutionOptionsBuilder<TExecutionContext>> configureExecutionOptions)
            where TQuery : Query<TExecutionContext>, new()
            where TMutation : Mutation<TExecutionContext>, new()
        {
            var services = new ServiceCollection();
            services.AddEpamGraphQLSchema<TQuery, TMutation, TExecutionContext>(configure);
            var serviceProvider = services.BuildServiceProvider();

            var schemaExecuter = serviceProvider.GetRequiredService<ISchemaExecuter<TQuery, TMutation, TExecutionContext>>();
            var result = schemaExecuter.ExecuteAsync(configureExecutionOptions).Result;
            return result;
        }

        private static ExecutionResult ExecuteQuery<TQuery, TMutation>(string request, IDataContext dataContext, Action<SchemaOptionsBuilder<TestUserContext>> configure)
            where TQuery : Query<TestUserContext>, new()
            where TMutation : Mutation<TestUserContext>, new()
        {
            if (dataContext == null)
            {
                dataContext = Substitute.For<IDataContext>();

                dataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<Proxy<Person>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<Proxy<Person>>>(0).ToAsyncEnumerable());
                dataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<Proxy<Unit>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<Proxy<Unit>>>(0).ToAsyncEnumerable());
                dataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<Proxy<Branch>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<Proxy<Branch>>>(0).ToAsyncEnumerable());
                dataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Person>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Person>>>(0).ToAsyncEnumerable());
                dataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Unit>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Unit>>>(0).ToAsyncEnumerable());
                dataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Branch>>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Branch>>>>(0).ToAsyncEnumerable());
                dataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Person>>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Person>>>>(0).ToAsyncEnumerable());
                dataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<long, Proxy<Person>>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<long, Proxy<Person>>>>(0).ToAsyncEnumerable());
                dataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Unit>>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Unit>>>>(0).ToAsyncEnumerable());
                dataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<string, Proxy<Unit>>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<string, Proxy<Unit>>>>(0).ToAsyncEnumerable());
                dataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, int>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, int>>>(0).ToAsyncEnumerable());

                dataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Tuple<Proxy<Unit>, int>>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Tuple<Proxy<Unit>, int>>>>(0).ToAsyncEnumerable());
                dataContext.QueryableToAsyncEnumerable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Tuple<Proxy<Unit>, Proxy<Branch>>>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Tuple<Proxy<Unit>, Proxy<Branch>>>>>(0).ToAsyncEnumerable());

                dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<Proxy<Person>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<Proxy<Person>>>(0));
                dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<Proxy<Unit>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<Proxy<Unit>>>(0));
                dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<Proxy<Branch>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<Proxy<Branch>>>(0));
                dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<object>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<object>>(0));
                dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Person>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Person>>>(0));
                dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Unit>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Unit>>>(0));
                dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Branch>>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Branch>>>>(0));
                dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Person>>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Person>>>>(0));
                dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<long, Proxy<Person>>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<long, Proxy<Person>>>>(0));
                dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Unit>>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Proxy<Unit>>>>(0));
                dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<string, Proxy<Unit>>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<string, Proxy<Unit>>>>(0));
                dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, int>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, int>>>(0));

                dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Tuple<Proxy<Unit>, int>>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Tuple<Proxy<Unit>, int>>>>(0));
                dataContext.QueryableToAsNoTrackingQueryable(Arg.Any<IQueryable<QueryableExtensions.KeyValue<int, Tuple<Proxy<Unit>, Proxy<Branch>>>>>())
                    .Returns(callInfo => callInfo.ArgAt<IQueryable<QueryableExtensions.KeyValue<int, Tuple<Proxy<Unit>, Proxy<Branch>>>>>(0));

                dataContext.ExecuteInTransactionAsync(Arg.Any<Func<Task>>())
                    .Returns(callInfo => callInfo.ArgAt<Func<Task>>(0)());
            }

            return ExecuteQuery<TQuery, TMutation, TestUserContext>(configure, executionBuilder =>
            {
                executionBuilder.Options.DataContext = dataContext;
                executionBuilder
                    .ThrowOnUnhandledException()
                    .WithExecutionContext(new TestUserContext(dataContext))
                    .Query(request);
            });
        }

        private static object ToObject(JToken token)
        {
#pragma warning disable IDE0072 // Add missing cases
            return token.Type switch
#pragma warning restore IDE0072 // Add missing cases
            {
                JTokenType.Object => token.Children<JProperty>()
                                       .ToDictionary(
                                           prop => prop.Name,
                                           prop => ToObject(prop.Value)),
                JTokenType.Array => token.Select(ToObject).ToList(),
                _ => ((JValue)token).Value,
            };
        }
    }
}
