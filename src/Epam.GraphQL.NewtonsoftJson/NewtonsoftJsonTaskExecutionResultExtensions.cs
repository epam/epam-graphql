// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Execution;
using GraphQL.NewtonsoftJson;

namespace Epam.GraphQL.SystemTextJson
{
    /// <summary>
    /// Extension methods for <see cref="ExecutionResult"/> serialization using NewtonsoftJson.
    /// </summary>
    public static class NewtonsoftJsonTaskExecutionResultExtensions
    {
        public static async Task<string> ToStringAsync(this Task<ExecutionResult> executionResultTask)
        {
            if (executionResultTask == null)
            {
                throw new ArgumentNullException(nameof(executionResultTask));
            }

            var executionResult = await executionResultTask.ConfigureAwait(false);
            var result = await executionResult.WriteToStringAsync(new GraphQLSerializer()).ConfigureAwait(false);
            return result;
        }

        public static async Task<string> ToStringAsync(this Task<ExecutionResult> executionResultTask, bool indent)
        {
            if (executionResultTask == null)
            {
                throw new ArgumentNullException(nameof(executionResultTask));
            }

            var executionResult = await executionResultTask.ConfigureAwait(false);
            var result = await executionResult.WriteToStringAsync(new GraphQLSerializer(indent: indent)).ConfigureAwait(false);
            return result;
        }

        public static async Task<string> ToStringAsync(this Task<ExecutionResult> executionResultTask, IErrorInfoProvider errorInfoProvider)
        {
            if (executionResultTask == null)
            {
                throw new ArgumentNullException(nameof(executionResultTask));
            }

            var executionResult = await executionResultTask.ConfigureAwait(false);
            var result = await executionResult.WriteToStringAsync(new GraphQLSerializer(errorInfoProvider: errorInfoProvider)).ConfigureAwait(false);
            return result;
        }

        public static async Task<string> ToStringAsync(this Task<ExecutionResult> executionResultTask, JsonSerializerSettings serializerSettings)
        {
            if (executionResultTask == null)
            {
                throw new ArgumentNullException(nameof(executionResultTask));
            }

            var executionResult = await executionResultTask.ConfigureAwait(false);
            var result = await executionResult.WriteToStringAsync(new GraphQLSerializer(serializerSettings: serializerSettings)).ConfigureAwait(false);
            return result;
        }

        public static async Task<string> ToStringAsync(this Task<ExecutionResult> executionResultTask, Action<JsonSerializerSettings> configureSerializerSettings)
        {
            if (executionResultTask == null)
            {
                throw new ArgumentNullException(nameof(executionResultTask));
            }

            var executionResult = await executionResultTask.ConfigureAwait(false);
            var result = await executionResult.WriteToStringAsync(new GraphQLSerializer(configureSerializerSettings: configureSerializerSettings)).ConfigureAwait(false);
            return result;
        }

        public static async Task<string> ToStringAsync(this Task<ExecutionResult> executionResultTask, bool indent, IErrorInfoProvider errorInfoProvider)
        {
            if (executionResultTask == null)
            {
                throw new ArgumentNullException(nameof(executionResultTask));
            }

            var executionResult = await executionResultTask.ConfigureAwait(false);
            var result = await executionResult.WriteToStringAsync(new GraphQLSerializer(indent: indent, errorInfoProvider: errorInfoProvider)).ConfigureAwait(false);
            return result;
        }

        public static async Task<string> ToStringAsync(this Task<ExecutionResult> executionResultTask, JsonSerializerSettings serializerSettings, IErrorInfoProvider errorInfoProvider)
        {
            if (executionResultTask == null)
            {
                throw new ArgumentNullException(nameof(executionResultTask));
            }

            var executionResult = await executionResultTask.ConfigureAwait(false);
            var result = await executionResult.WriteToStringAsync(new GraphQLSerializer(serializerSettings: serializerSettings, errorInfoProvider: errorInfoProvider)).ConfigureAwait(false);
            return result;
        }

        public static async Task<string> ToStringAsync(this Task<ExecutionResult> executionResultTask, Action<JsonSerializerSettings> configureSerializerSettings, IErrorInfoProvider errorInfoProvider)
        {
            if (executionResultTask == null)
            {
                throw new ArgumentNullException(nameof(executionResultTask));
            }

            var executionResult = await executionResultTask.ConfigureAwait(false);
            var result = await executionResult.WriteToStringAsync(new GraphQLSerializer(configureSerializerSettings: configureSerializerSettings, errorInfoProvider: errorInfoProvider)).ConfigureAwait(false);
            return result;
        }
    }
}
