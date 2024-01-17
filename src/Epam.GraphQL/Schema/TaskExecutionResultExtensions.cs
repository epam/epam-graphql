// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Threading.Tasks;
using Epam.GraphQL.Helpers;
using GraphQL;

namespace Epam.GraphQL
{
    /// <summary>
    /// Extension methods for <see cref="ExecutionResult"/> serialization.
    /// </summary>
    public static class TaskExecutionResultExtensions
    {
        public static async Task<string> ToStringAsync(this Task<ExecutionResult> executionResultTask, IGraphQLTextSerializer graphQlTextSerializer)
        {
            Guards.ThrowIfNull(executionResultTask, nameof(executionResultTask));
            Guards.ThrowIfNull(graphQlTextSerializer, nameof(graphQlTextSerializer));

            var executionResult = await executionResultTask.ConfigureAwait(false);
            var result = await executionResult.WriteToStringAsync(graphQlTextSerializer).ConfigureAwait(false);
            return result;
        }
    }
}
