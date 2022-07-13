// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Extensions;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields
{
    internal class PayloadFieldsContext<TExecutionContext>
    {
        private readonly Type _payloadType;

        private readonly object? _payload;

        public PayloadFieldsContext(TExecutionContext executionContext, Type payloadType, object? payload)
        {
            ExecutionContext = executionContext;
            _payloadType = payloadType;
            _payload = payload;
        }

        public TExecutionContext ExecutionContext { get; }

        public TPropertyType? GetPropertyValue<TPropertyType>(string propertyName)
        {
            if (_payload == null)
            {
                return default;
            }

            return _payload.GetPropertyValue<TPropertyType>(_payloadType.GetProperty(propertyName));
        }

        public object? GetPropertyValue(string propertyName)
        {
            if (_payload == null)
            {
                return null;
            }

            return _payload.GetPropertyValue(_payloadType.GetProperty(propertyName));
        }
    }
}
