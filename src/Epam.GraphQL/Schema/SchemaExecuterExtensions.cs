// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Utilities;

namespace Epam.GraphQL
{
    public static class SchemaExecuterExtensions
    {
        public static Task<ExecutionResult> ExecuteAsync<TQuery, TExecutionContext>(this ISchemaExecuter<TQuery, TExecutionContext> schemaExecuter, Action<SchemaExecutionOptionsBuilder<TExecutionContext>> configure)
            where TQuery : Query<TExecutionContext>, new()
        {
            if (schemaExecuter == null)
            {
                throw new ArgumentNullException(nameof(schemaExecuter));
            }

            if (configure == null)
            {
                throw new ArgumentNullException(nameof(configure));
            }

            var builder = new SchemaExecutionOptionsBuilder<TExecutionContext>();
            configure(builder);

            return schemaExecuter.ExecuteAsync(builder.Options);
        }

        public static Task<ExecutionResult> ExecuteAsync<TQuery, TMutation, TExecutionContext>(this ISchemaExecuter<TQuery, TMutation, TExecutionContext> schemaExecuter, Action<SchemaExecutionOptionsBuilder<TExecutionContext>> configure)
            where TQuery : Query<TExecutionContext>, new()
            where TMutation : Mutation<TExecutionContext>, new()
        {
            if (schemaExecuter == null)
            {
                throw new ArgumentNullException(nameof(schemaExecuter));
            }

            return ExecuteAsync((ISchemaExecuter<TQuery, TExecutionContext>)schemaExecuter, configure);
        }

        public static string Print<TExecutionContext>(this ISchemaExecuter<TExecutionContext> schemaExecuter, SchemaPrinterOptions? options = null)
        {
            if (schemaExecuter == null)
            {
                throw new ArgumentNullException(nameof(schemaExecuter));
            }

            var printer = new SchemaPrinter(((SchemaExecuter<TExecutionContext>)schemaExecuter).GraphQLSchema, options);
            return printer.Print();
        }
    }
}
