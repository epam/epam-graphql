// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Epam.GraphQL.Adapters;
using Epam.GraphQL.Infrastructure;
using Epam.GraphQL.Options;
using Epam.GraphQL.TaskBatcher;
using GraphQL;
using GraphQL.Language.AST;
using GraphQL.Validation;
using GraphQL.Validation.Complexity;
using Microsoft.Extensions.Logging.Abstractions;

#nullable enable

namespace Epam.GraphQL
{
    public class SchemaExecutionOptions<TExecutionContext>
    {
        public bool ThrowOnUnhandledException { get; set; }

        public bool EnableMetrics { get; set; }

        public ComplexityConfiguration? ComplexityConfiguration { get; set; }

        public TExecutionContext? ExecutionContext { get; set; }

        [Obsolete("UserContext has been renamed to ExecutionContext. Use ExecutionContext instead")]
        public TExecutionContext? UserContext { get => ExecutionContext; set => ExecutionContext = value; }

        public IEnumerable<IValidationRule>? ValidationRules { get; set; }

        public CancellationToken CancellationToken { get; set; }

#pragma warning disable CA2227 // Collection properties should be read only
        public Dictionary<string, object>? Variables { get; set; }
#pragma warning restore CA2227 // Collection properties should be read only

        public Document? Document { get; set; }

        public string? OperationName { get; set; }

        public string? Query { get; set; }

        public IDataContext? DataContext { get; set; }

        internal ExecutionOptions ToExecutionOptions(SchemaExecuter<TExecutionContext> schema)
        {
            if (schema == null)
            {
                throw new ArgumentNullException(nameof(schema));
            }

            var coreOptionsExtension = schema.Options.FindExtension<CoreSchemaOptionsExtension<TExecutionContext>>();
            var profiler = coreOptionsExtension?.Profiler ?? NullProfiler.Instance;
            var batcher = new Batcher(profiler);
            var logger = coreOptionsExtension?.LoggerFactory?.CreateLogger(Constants.Logging.Category) ?? NullLogger.Instance;
            var listeners = coreOptionsExtension?.Listeners;

            var graphQLContext = new GraphQLContext<TExecutionContext>(
                DataContext,
                profiler,
                batcher,
                schema.Registry,
                logger,
                listeners,
                ExecutionContext);
            var validationRules = coreOptionsExtension?.ValidationRules;

            if (validationRules != null)
            {
                validationRules = DocumentValidator.CoreRules.Concat(validationRules);
            }

            var executionOptions = new ExecutionOptions
            {
                ThrowOnUnhandledException = ThrowOnUnhandledException,
                EnableMetrics = EnableMetrics,
                ComplexityConfiguration = ComplexityConfiguration,
                UserContext = new Dictionary<string, object>
                {
                    ["ctx"] = graphQLContext,
                },
                ValidationRules = validationRules,
                CancellationToken = CancellationToken,
                Inputs = Variables.ToInputs(),
                Document = Document,
                OperationName = OperationName,
                Query = Query,
                Schema = schema.GraphQLSchema,
            };

            executionOptions.Listeners.Add(new DocumentExecutionListener<TExecutionContext>(graphQLContext));

            return executionOptions;
        }
    }
}
