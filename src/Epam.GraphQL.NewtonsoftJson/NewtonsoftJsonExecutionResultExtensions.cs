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
    public static class NewtonsoftJsonExecutionResultExtensions
    {
        public static Task<string> WriteToStringAsync(this ExecutionResult executionResul)
        {
            return executionResul.WriteToStringAsync(new GraphQLSerializer());
        }

        public static Task<string> WriteToStringAsync(this ExecutionResult executionResul, bool indent)
        {
            return executionResul.WriteToStringAsync(new GraphQLSerializer(indent: indent));
        }

        public static Task<string> WriteToStringAsync(this ExecutionResult executionResul, IErrorInfoProvider errorInfoProvider)
        {
            return executionResul.WriteToStringAsync(new GraphQLSerializer(errorInfoProvider: errorInfoProvider));
        }

        public static Task<string> WriteToStringAsync(this ExecutionResult executionResul, JsonSerializerSettings serializerSettings)
        {
            return executionResul.WriteToStringAsync(new GraphQLSerializer(serializerSettings: serializerSettings));
        }

        public static Task<string> WriteToStringAsync(this ExecutionResult executionResul, Action<JsonSerializerSettings> configureSerializerSettings)
        {
            return executionResul.WriteToStringAsync(new GraphQLSerializer(configureSerializerSettings: configureSerializerSettings));
        }

        public static Task<string> WriteToStringAsync(this ExecutionResult executionResul, bool indent, IErrorInfoProvider errorInfoProvider)
        {
            return executionResul.WriteToStringAsync(new GraphQLSerializer(indent: indent, errorInfoProvider: errorInfoProvider));
        }

        public static Task<string> WriteToStringAsync(this ExecutionResult executionResul, JsonSerializerSettings serializerSettings, IErrorInfoProvider errorInfoProvider)
        {
            return executionResul.WriteToStringAsync(new GraphQLSerializer(serializerSettings: serializerSettings, errorInfoProvider: errorInfoProvider));
        }

        public static Task<string> WriteToStringAsync(this ExecutionResult executionResul, Action<JsonSerializerSettings> configureSerializerSettings, IErrorInfoProvider errorInfoProvider)
        {
            return executionResul.WriteToStringAsync(new GraphQLSerializer(configureSerializerSettings: configureSerializerSettings, errorInfoProvider: errorInfoProvider));
        }
    }
}
