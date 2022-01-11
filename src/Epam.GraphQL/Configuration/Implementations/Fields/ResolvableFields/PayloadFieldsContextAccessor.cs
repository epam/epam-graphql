// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Loaders;
using GraphQL;

#nullable enable

namespace Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields
{
    internal class PayloadFieldsContextAccessor<TExecutionContext>
    {
        private const string PayloadSuffix = "Payload";
        private const string PayloadArgumentName = "payload";

        private readonly Lazy<Type> _payloadType;

        public PayloadFieldsContextAccessor(string fieldName, IEnumerable<IArgument<PayloadFieldsContext<TExecutionContext>>> items)
        {
            FieldName = fieldName;
            _payloadType = new Lazy<Type>(() =>
            {
                var fields = items.ToDictionary(arg => arg.Name, arg => arg.InputType);
                return fields.MakeType($"{fieldName.CapitalizeFirstLetter()}{PayloadSuffix}", typeof(Input));
            });
        }

        public string FieldName { get; }

        public void ApplyTo(IArgumentCollection arguments)
        {
            arguments.Argument(PayloadArgumentName, _payloadType.Value);
        }

        public PayloadFieldsContext<TExecutionContext> GetContext(IResolveFieldContext context) =>
            new(
                context.GetUserContext<TExecutionContext>(),
                _payloadType.Value,
                context.GetArgument(_payloadType.Value, PayloadArgumentName));
    }
}
