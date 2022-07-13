// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL;

namespace Microsoft.Extensions.DependencyInjection
{
    /// <summary>
    /// Extension methods for setting up Epam.GraphQL schemas in an <see cref="IServiceCollection"/>.
    /// </summary>
    public static class SchemaServiceCollectionExtensions
    {
        /// <summary>
        /// Adds the <see cref="SchemaExecuter{TQuery, TMutation, TExecutionContext}"/> and related services to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TQuery"> The type of the query to be used for GraphQL schema. </typeparam>
        /// <typeparam name="TMutation"> The type of the mutation to be used for GraphQL schema. </typeparam>
        /// <typeparam name="TExecutionContext"> The type of the user context to be used for queries and mutations. </typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="configure"> An action used to create or modify options for this schema. </param>
        /// <returns>The <see cref="IServiceCollection"/>  so that additional calls can be chained.</returns>
        public static IServiceCollection AddEpamGraphQLSchema<TQuery, TMutation, TExecutionContext>(this IServiceCollection services, Action<SchemaOptionsBuilder<TExecutionContext>>? configure = null)
            where TQuery : Query<TExecutionContext>, new()
            where TMutation : Mutation<TExecutionContext>, new()
        {
            services.AddSingleton<ISchemaExecuter<TQuery, TMutation, TExecutionContext>>(serviceProvider =>
            {
                var schemaOptionsBuilder = new SchemaOptionsBuilder<TExecutionContext>(services);
                configure?.Invoke(schemaOptionsBuilder);

                return new SchemaExecuter<TQuery, TMutation, TExecutionContext>(schemaOptionsBuilder.Options);
            });

            return services;
        }

        /// <summary>
        /// Adds the <see cref="SchemaExecuter{TQuery, TExecutionContext}"/> and related services to the <see cref="IServiceCollection"/>.
        /// </summary>
        /// <typeparam name="TQuery"> The type of the query to be used for GraphQL schema. </typeparam>
        /// <typeparam name="TExecutionContext"> The type of the user context to be used for queries and mutations. </typeparam>
        /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
        /// <param name="configure"> An action used to create or modify options for this schema. </param>
        /// <returns>The <see cref="IServiceCollection"/>  so that additional calls can be chained.</returns>
        public static IServiceCollection AddEpamGraphQLSchema<TQuery, TExecutionContext>(this IServiceCollection services, Action<SchemaOptionsBuilder<TExecutionContext>>? configure = null)
            where TQuery : Query<TExecutionContext>, new()
        {
            services.AddSingleton<ISchemaExecuter<TQuery, TExecutionContext>>(serviceProvider =>
            {
                var schemaOptionsBuilder = new SchemaOptionsBuilder<TExecutionContext>(services);
                configure?.Invoke(schemaOptionsBuilder);

                return new SchemaExecuter<TQuery, TExecutionContext>(schemaOptionsBuilder.Options);
            });

            return services;
        }
    }
}
