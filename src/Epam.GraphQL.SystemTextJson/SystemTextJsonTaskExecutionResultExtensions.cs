// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Text.Json;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Execution;

namespace Epam.GraphQL.SystemTextJson
{
    /// <summary>
    /// Extension methods for <see cref="ExecutionResult"/> serialization using System.Text.Json.
    /// </summary>
    public static class SystemTextJsonTaskExecutionResultExtensions
    {
        public static async Task<string> ToStringAsync(this Task<ExecutionResult> executionResultTask)
        {
            if (executionResultTask == null)
            {
                throw new ArgumentNullException(nameof(executionResultTask));
            }

            var executionResult = await executionResultTask.ConfigureAwait(false);
            var result = await executionResult.WriteToStringAsync().ConfigureAwait(false);
            return result;
        }

        public static async Task<string> ToStringAsync(this Task<ExecutionResult> executionResultTask, bool indent)
        {
            if (executionResultTask == null)
            {
                throw new ArgumentNullException(nameof(executionResultTask));
            }

            var executionResult = await executionResultTask.ConfigureAwait(false);
            var result = await executionResult.WriteToStringAsync(indent: indent).ConfigureAwait(false);
            return result;
        }

        public static async Task<string> ToStringAsync(this Task<ExecutionResult> executionResultTask, IErrorInfoProvider errorInfoProvider)
        {
            if (executionResultTask == null)
            {
                throw new ArgumentNullException(nameof(executionResultTask));
            }

            var executionResult = await executionResultTask.ConfigureAwait(false);
            var result = await executionResult.WriteToStringAsync(errorInfoProvider: errorInfoProvider).ConfigureAwait(false);
            return result;
        }

        public static async Task<string> ToStringAsync(this Task<ExecutionResult> executionResultTask, JsonSerializerOptions serializerOptions)
        {
            if (executionResultTask == null)
            {
                throw new ArgumentNullException(nameof(executionResultTask));
            }

            var executionResult = await executionResultTask.ConfigureAwait(false);
            var result = await executionResult.WriteToStringAsync(serializerOptions: serializerOptions).ConfigureAwait(false);
            return result;
        }

        public static async Task<string> ToStringAsync(this Task<ExecutionResult> executionResultTask, Action<JsonSerializerOptions> configureSerializerOptions)
        {
            if (executionResultTask == null)
            {
                throw new ArgumentNullException(nameof(executionResultTask));
            }

            var executionResult = await executionResultTask.ConfigureAwait(false);
            var result = await executionResult.WriteToStringAsync(configureSerializerOptions: configureSerializerOptions).ConfigureAwait(false);
            return result;
        }

        public static async Task<string> ToStringAsync(this Task<ExecutionResult> executionResultTask, bool indent, IErrorInfoProvider errorInfoProvider)
        {
            if (executionResultTask == null)
            {
                throw new ArgumentNullException(nameof(executionResultTask));
            }

            var executionResult = await executionResultTask.ConfigureAwait(false);
            var result = await executionResult.WriteToStringAsync(indent: indent, errorInfoProvider: errorInfoProvider).ConfigureAwait(false);
            return result;
        }

        public static async Task<string> ToStringAsync(this Task<ExecutionResult> executionResultTask, JsonSerializerOptions serializerOptions, IErrorInfoProvider errorInfoProvider)
        {
            if (executionResultTask == null)
            {
                throw new ArgumentNullException(nameof(executionResultTask));
            }

            var executionResult = await executionResultTask.ConfigureAwait(false);
            var result = await executionResult.WriteToStringAsync(serializerOptions: serializerOptions, errorInfoProvider: errorInfoProvider).ConfigureAwait(false);
            return result;
        }

        public static async Task<string> ToStringAsync(this Task<ExecutionResult> executionResultTask, Action<JsonSerializerOptions> configureSerializerOptions, IErrorInfoProvider errorInfoProvider)
        {
            if (executionResultTask == null)
            {
                throw new ArgumentNullException(nameof(executionResultTask));
            }

            var executionResult = await executionResultTask.ConfigureAwait(false);
            var result = await executionResult.WriteToStringAsync(configureSerializerOptions: configureSerializerOptions, errorInfoProvider: errorInfoProvider).ConfigureAwait(false);
            return result;
        }
    }
}
