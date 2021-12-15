// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Infrastructure;
using Epam.GraphQL.Options;
using GraphQL.Validation;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Epam.GraphQL
{
    public class SchemaOptionsBuilder<TExecutionContext> : OptionsBuilder<SchemaOptions>
    {
        public SchemaOptionsBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }

        public IServiceCollection Services { get; }

        public SchemaOptionsBuilder<TExecutionContext> UseValidationRule(IValidationRule validationRule) => WithOption(e => e.WithValidationRule(validationRule));

        public SchemaOptionsBuilder<TExecutionContext> UseProfiler(IProfiler profiler) => WithOption(e => e.WithProfiler(profiler));

        public SchemaOptionsBuilder<TExecutionContext> UseLoggerFactory(ILoggerFactory loggerFactory) => WithOption(e => e.WithLoggerFactory(loggerFactory));

        public SchemaOptionsBuilder<TExecutionContext> WithListener(ISchemaExecutionListener listener) => WithOption(e => e.WithListener(listener));

        private SchemaOptionsBuilder<TExecutionContext> WithOption(Func<CoreSchemaOptionsExtension<TExecutionContext>, CoreSchemaOptionsExtension<TExecutionContext>> withFunc)
        {
            AddOrUpdateExtension(withFunc(Options.FindExtension<CoreSchemaOptionsExtension<TExecutionContext>>() ?? new CoreSchemaOptionsExtension<TExecutionContext>()));

            return this;
        }
    }
}
