// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Configuration.Implementations;
using Epam.GraphQL.Configuration.Implementations.Fields;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Builders.Loader.Implementations
{
    internal class SelectAndReferenceToBuilder<TField, TSourceType, TReturnType, TExecutionContext> : SelectBuilder<TField, TSourceType, TReturnType, TExecutionContext>,
        IHasSelectAndReferenceTo<TSourceType, TReturnType, TExecutionContext>
        where TSourceType : class
        where TField : FieldBase<TSourceType, TExecutionContext>, IFieldSupportsApplySelect<TSourceType, TReturnType, TExecutionContext>
    {
        internal SelectAndReferenceToBuilder(RelationRegistry<TExecutionContext> registry, TField field)
            : base(field)
        {
            Registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }

        protected RelationRegistry<TExecutionContext> Registry { get; }

        public IHasSelect<TReturnType, TExecutionContext> ReferenceTo<TChildEntity, TChildEntityLoader>(Predicate<TReturnType> isFakePropValue)
            where TChildEntity : class
            where TChildEntityLoader : Loader<TChildEntity, TExecutionContext>, IIdentifiableLoader, new()
        {
            Registry.RegisterRelationPostponedForSave<TSourceType, TChildEntity, TChildEntityLoader, TReturnType, TReturnType>(Field.Name, isFakePropValue);
            return this;
        }
    }
}
