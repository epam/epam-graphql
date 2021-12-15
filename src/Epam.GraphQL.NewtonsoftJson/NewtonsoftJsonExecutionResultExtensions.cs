// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Execution;
using GraphQL.NewtonsoftJson;
using Newtonsoft.Json;

namespace Epam.GraphQL.SystemTextJson
{
    /// <summary>
    /// Extension methods for <see cref="ExecutionResult"/> serialization using NewtonsoftJson.
    /// </summary>
    public static class NewtonsoftJsonExecutionResultExtensions
    {
        public static Task<string> WriteToStringAsync(this ExecutionResult executionResul)
        {
            return executionResul.WriteToStringAsync(new DocumentWriter());
        }

        public static Task<string> WriteToStringAsync(this ExecutionResult executionResul, bool indent)
        {
            return executionResul.WriteToStringAsync(new DocumentWriter(indent: indent));
        }

        public static Task<string> WriteToStringAsync(this ExecutionResult executionResul, IErrorInfoProvider errorInfoProvider)
        {
            return executionResul.WriteToStringAsync(new DocumentWriter(errorInfoProvider: errorInfoProvider));
        }

        public static Task<string> WriteToStringAsync(this ExecutionResult executionResul, JsonSerializerSettings serializerSettings)
        {
            return executionResul.WriteToStringAsync(new DocumentWriter(serializerSettings: serializerSettings));
        }

        public static Task<string> WriteToStringAsync(this ExecutionResult executionResul, Action<JsonSerializerSettings> configureSerializerSettings)
        {
            return executionResul.WriteToStringAsync(new DocumentWriter(configureSerializerSettings: configureSerializerSettings));
        }

        public static Task<string> WriteToStringAsync(this ExecutionResult executionResul, bool indent, IErrorInfoProvider errorInfoProvider)
        {
            return executionResul.WriteToStringAsync(new DocumentWriter(indent: indent, errorInfoProvider: errorInfoProvider));
        }

        public static Task<string> WriteToStringAsync(this ExecutionResult executionResul, JsonSerializerSettings serializerSettings, IErrorInfoProvider errorInfoProvider)
        {
            return executionResul.WriteToStringAsync(new DocumentWriter(serializerSettings: serializerSettings, errorInfoProvider: errorInfoProvider));
        }

        public static Task<string> WriteToStringAsync(this ExecutionResult executionResul, Action<JsonSerializerSettings> configureSerializerSettings, IErrorInfoProvider errorInfoProvider)
        {
            return executionResul.WriteToStringAsync(new DocumentWriter(configureSerializerSettings: configureSerializerSettings, errorInfoProvider: errorInfoProvider));
        }
    }
}
