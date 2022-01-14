// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using Epam.GraphQL.Configuration.Implementations.Descriptors;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields
{
    internal class ArgumentedUnionFieldBase<TArguments, TEntity, TExecutionContext> :
        UnionFieldBase<TEntity, TExecutionContext>
        where TArguments : IArguments
        where TEntity : class
    {
        protected ArgumentedUnionFieldBase(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<TEntity, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> typeResolver,
            bool isList,
            TArguments arguments)
            : base(registry, parent, name, unionType, typeResolver, isList)
        {
            Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            Arguments.ApplyTo(this);
        }

        protected ArgumentedUnionFieldBase(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<TEntity, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> typeResolver,
            List<Type> unionTypes,
            UnionGraphTypeDescriptor<TExecutionContext> unionGraphType,
            bool isList,
            TArguments arguments)
            : base(registry, parent, name, unionType, typeResolver, unionTypes, unionGraphType, isList)
        {
            Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            Arguments.ApplyTo(this);
        }

        protected ArgumentedUnionFieldBase(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            List<Type> unionTypes,
            UnionGraphTypeDescriptor<TExecutionContext> unionGraphType,
            bool isList,
            TArguments arguments)
            : base(registry, parent, name, unionTypes, unionGraphType, isList)
        {
            Arguments = arguments ?? throw new ArgumentNullException(nameof(arguments));
            Arguments.ApplyTo(this);
        }

        protected new TArguments Arguments { get; }
    }
}
