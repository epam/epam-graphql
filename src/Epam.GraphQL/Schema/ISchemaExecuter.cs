// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Threading.Tasks;
using GraphQL;

#nullable enable

namespace Epam.GraphQL
{
    /// <summary>
    /// Processes a GraphQL request.
    /// </summary>
    /// <typeparam name="TExecutionContext"> The type of the execution context to be used for queries and mutations. </typeparam>
    public interface ISchemaExecuter<TExecutionContext>
    {
        /// <summary>
        /// Executes a GraphQL request using an execution options <paramref name="schemaExecutionOptions"/>.
        /// </summary>
        /// <returns> A result of GraphQL query execution. </returns>
        Task<ExecutionResult> ExecuteAsync(SchemaExecutionOptions<TExecutionContext> schemaExecutionOptions);
    }

    /// <inheritdoc/>
    /// <typeparam name="TQuery"> The type of the query to be used for GraphQL schema. </typeparam>
    /// <typeparam name="TExecutionContext"> The type of the execution context to be used for queries and mutations. </typeparam>
    public interface ISchemaExecuter<TQuery, TExecutionContext> : ISchemaExecuter<TExecutionContext>
        where TQuery : Query<TExecutionContext>, new()
    {
    }

    /// <inheritdoc/>
    /// <typeparam name="TQuery"> The type of the query to be used for GraphQL schema. </typeparam>
    /// <typeparam name="TMutation"> The type of the mutation to be used for GraphQL schema. </typeparam>
    /// <typeparam name="TExecutionContext"> The type of the execution context to be used for queries and mutations. </typeparam>
    public interface ISchemaExecuter<TQuery, TMutation, TExecutionContext> : ISchemaExecuter<TQuery, TExecutionContext>
        where TQuery : Query<TExecutionContext>, new()
        where TMutation : Mutation<TExecutionContext>, new()
    {
    }
}
