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
    public static class ExecutionResultExtensions
    {
        public static Task<string> WriteToStringAsync(this ExecutionResult executionResult, IGraphQLTextSerializer graphQlTextSerializer)
        {
            Guards.ThrowIfNull(executionResult, nameof(executionResult));
            Guards.ThrowIfNull(graphQlTextSerializer, nameof(graphQlTextSerializer));

            var result = graphQlTextSerializer.Serialize(executionResult);
            return Task.FromResult(result);
        }
    }
}
