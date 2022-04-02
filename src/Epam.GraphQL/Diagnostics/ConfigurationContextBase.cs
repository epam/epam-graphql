// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using Epam.GraphQL.Extensions;

namespace Epam.GraphQL.Diagnostics
{
    internal class ConfigurationContextBase : IConfigurationContext
    {
        private readonly List<ConfigurationException> _errors = new();

        public void Clear()
        {
            _errors.Clear();
        }

        public FieldConfigurationContext Operation(string operation)
        {
            return new FieldConfigurationContext(this, null, operation);
        }

        public FieldConfigurationContext Operation<T>(string operation)
        {
            return Operation($"{operation}<{typeof(T).HumanizedName()}>");
        }

        public void AddError(string message)
        {
            _errors.Add(new ConfigurationException(GetError(message)));
        }

        public virtual string GetError(string message) => message;

        public void ThrowErrors()
        {
            if (_errors.Count > 0)
            {
                throw new AggregateException(_errors);
            }
        }
    }
}
