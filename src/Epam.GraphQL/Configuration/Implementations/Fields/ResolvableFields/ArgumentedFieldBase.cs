// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields
{
    internal class ArgumentedFieldBase<TArguments, TEntity, TExecutionContext> : Field<TEntity, TExecutionContext>
        where TArguments : IArguments
        where TEntity : class
    {
        protected ArgumentedFieldBase(
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            TArguments arguments)
            : base(parent, name)
        {
            Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            Arguments.ApplyTo(this);
        }

        protected new TArguments Arguments { get; }
    }
}
