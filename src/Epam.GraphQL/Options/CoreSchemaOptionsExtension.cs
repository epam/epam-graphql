// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using Epam.GraphQL.Infrastructure;
using GraphQL.Validation;
using Microsoft.Extensions.Logging;

#nullable enable

namespace Epam.GraphQL.Options
{
    internal class CoreSchemaOptionsExtension<TExecutionContext>
    {
        private readonly List<IValidationRule> _validationRules = new();
        private readonly List<ISchemaExecutionListener> _listeners = new();

        public CoreSchemaOptionsExtension()
        {
        }

        public CoreSchemaOptionsExtension(CoreSchemaOptionsExtension<TExecutionContext> copyFrom)
        {
            if (copyFrom == null)
            {
                throw new ArgumentNullException(nameof(copyFrom));
            }

            _validationRules.AddRange(copyFrom._validationRules);
            _listeners.AddRange(copyFrom._listeners);
            Profiler = copyFrom.Profiler;
            LoggerFactory = copyFrom.LoggerFactory;
        }

        public IEnumerable<IValidationRule> ValidationRules => _validationRules;

        public IProfiler? Profiler { get; private set; }

        public IEnumerable<ISchemaExecutionListener> Listeners => _listeners;

        public ILoggerFactory? LoggerFactory { get; private set; }

        public CoreSchemaOptionsExtension<TExecutionContext> WithValidationRule(IValidationRule validationRule)
        {
            var extension = new CoreSchemaOptionsExtension<TExecutionContext>(this);

            extension._validationRules.Add(validationRule);

            return extension;
        }

        public CoreSchemaOptionsExtension<TExecutionContext> WithProfiler(IProfiler profiler)
        {
            return new CoreSchemaOptionsExtension<TExecutionContext>(this)
            {
                Profiler = profiler ?? throw new ArgumentNullException(nameof(profiler)),
            };
        }

        public CoreSchemaOptionsExtension<TExecutionContext> WithListener(ISchemaExecutionListener listener)
        {
            var extension = new CoreSchemaOptionsExtension<TExecutionContext>(this);

            extension._listeners.Add(listener);

            return extension;
        }

        public CoreSchemaOptionsExtension<TExecutionContext> WithLoggerFactory(ILoggerFactory loggerFactory)
        {
            var extension = new CoreSchemaOptionsExtension<TExecutionContext>(this)
            {
                LoggerFactory = loggerFactory,
            };

            return extension;
        }
    }
}
