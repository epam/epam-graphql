// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using GraphQL.Validation;

#nullable enable

namespace Epam.GraphQL
{
    public class SchemaExecutionOptionsBuilder<TExecutionContext>
    {
        public SchemaExecutionOptions<TExecutionContext> Options { get; } = new SchemaExecutionOptions<TExecutionContext>();

        public SchemaExecutionOptionsBuilder<TExecutionContext> UseValidationRules(IEnumerable<IValidationRule> validationRules) =>
            With(options =>
            {
                if (validationRules == null)
                {
                    throw new ArgumentNullException(nameof(validationRules));
                }

                options.ValidationRules = options.ValidationRules == null
                    ? validationRules
                    : options.ValidationRules.Concat(validationRules);
            });

        public SchemaExecutionOptionsBuilder<TExecutionContext> UseValidationRules(params IValidationRule[] validationRules) =>
            UseValidationRules(validationRules.AsEnumerable());

        public SchemaExecutionOptionsBuilder<TExecutionContext> WithOperationName(string operationName) =>
            With(options => options.OperationName = operationName);

        public SchemaExecutionOptionsBuilder<TExecutionContext> WithVariables(Dictionary<string, object> variables) =>
            With(options => options.Variables = variables);

        public SchemaExecutionOptionsBuilder<TExecutionContext> Query(string query) =>
            With(options => options.Query = query);

        public SchemaExecutionOptionsBuilder<TExecutionContext> EnableMetrics(bool enableMetrics = true) => With(options => options.EnableMetrics = enableMetrics);

        public SchemaExecutionOptionsBuilder<TExecutionContext> ThrowOnUnhandledException(bool throwOnUnhandledException = true) => With(options => options.ThrowOnUnhandledException = throwOnUnhandledException);

        [Obsolete("UseUserContext has been renamed. Use WithExecutionContext instead")]
        public SchemaExecutionOptionsBuilder<TExecutionContext> UseUserContext(TExecutionContext executionContext) => WithExecutionContext(executionContext);

        public SchemaExecutionOptionsBuilder<TExecutionContext> WithExecutionContext(TExecutionContext executionContext) => With(options => options.ExecutionContext = executionContext);

        public SchemaExecutionOptionsBuilder<TExecutionContext> With(Action<SchemaExecutionOptions<TExecutionContext>> configure)
        {
            configure?.Invoke(Options);
            return this;
        }
    }
}
