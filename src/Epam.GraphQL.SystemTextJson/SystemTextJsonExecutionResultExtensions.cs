// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Text.Json;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Execution;
using GraphQL.SystemTextJson;

namespace Epam.GraphQL.SystemTextJson
{
    /// <summary>
    /// Extension methods for <see cref="ExecutionResult"/> serialization using System.Text.Json.
    /// </summary>
    public static class SystemTextJsonExecutionResultExtensions
    {
        public static Task<string> WriteToStringAsync(this ExecutionResult executionResult)
        {
            return executionResult.WriteToStringAsync(new GraphQLSerializer());
        }

        public static Task<string> WriteToStringAsync(this ExecutionResult executionResult, bool indent)
        {
            return executionResult.WriteToStringAsync(new GraphQLSerializer(indent: indent));
        }

        public static Task<string> WriteToStringAsync(this ExecutionResult executionResult, IErrorInfoProvider errorInfoProvider)
        {
            return executionResult.WriteToStringAsync(new GraphQLSerializer(errorInfoProvider: errorInfoProvider));
        }

        public static Task<string> WriteToStringAsync(this ExecutionResult executionResult, JsonSerializerOptions serializerOptions)
        {
            return executionResult.WriteToStringAsync(new GraphQLSerializer(serializerOptions: serializerOptions));
        }

        public static Task<string> WriteToStringAsync(this ExecutionResult executionResult, Action<JsonSerializerOptions> configureSerializerOptions)
        {
            return executionResult.WriteToStringAsync(new GraphQLSerializer(configureSerializerOptions: configureSerializerOptions));
        }

        public static Task<string> WriteToStringAsync(this ExecutionResult executionResult, bool indent, IErrorInfoProvider errorInfoProvider)
        {
            return executionResult.WriteToStringAsync(new GraphQLSerializer(indent: indent, errorInfoProvider: errorInfoProvider));
        }

        public static Task<string> WriteToStringAsync(this ExecutionResult executionResult, JsonSerializerOptions serializerOptions, IErrorInfoProvider errorInfoProvider)
        {
            return executionResult.WriteToStringAsync(new GraphQLSerializer(serializerOptions: serializerOptions, errorInfoProvider: errorInfoProvider));
        }

        public static Task<string> WriteToStringAsync(this ExecutionResult executionResult, Action<JsonSerializerOptions> configureSerializerOptions, IErrorInfoProvider errorInfoProvider)
        {
            return executionResult.WriteToStringAsync(new GraphQLSerializer(configureSerializerOptions: configureSerializerOptions, errorInfoProvider: errorInfoProvider));
        }
    }
}
