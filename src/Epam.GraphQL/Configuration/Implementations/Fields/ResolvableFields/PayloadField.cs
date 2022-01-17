// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields
{
    internal class PayloadField<TArg, TExecutionContext> : IArgument<PayloadFieldsContext<TExecutionContext>>
    {
        public PayloadField(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        public string Name { get; }

        public Type InputType => typeof(TArg);

        public TOut GetValue<TOut>(PayloadFieldsContext<TExecutionContext> context)
        {
            return context.GetPropertyValue<TOut>(Name);
        }
    }
}
