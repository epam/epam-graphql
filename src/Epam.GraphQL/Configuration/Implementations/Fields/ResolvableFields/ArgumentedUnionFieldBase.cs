// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Diagnostics;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields
{
    internal class ArgumentedUnionFieldBase<TArguments, TEntity, TExecutionContext> :
        UnionFieldBase<TEntity, TExecutionContext>
        where TArguments : IArguments
    {
        protected ArgumentedUnionFieldBase(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<TEntity, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> typeResolver,
            TArguments arguments)
            : base(configurationContext, parent, name, unionType, typeResolver)
        {
            Arguments = arguments;
            Arguments.ApplyTo(this);
        }

        protected ArgumentedUnionFieldBase(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<TEntity, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> typeResolver,
            List<Type> unionTypes,
            UnionGraphTypeDescriptor<TExecutionContext> unionGraphType,
            TArguments arguments)
            : base(configurationContext, parent, name, unionType, typeResolver, unionTypes, unionGraphType)
        {
            Arguments = arguments;
            Arguments.ApplyTo(this);
        }

        protected new TArguments Arguments { get; }
    }
}
