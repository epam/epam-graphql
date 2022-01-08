// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using Epam.GraphQL.Builders.Loader;

#nullable enable

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal class TypedField<TEntity, TExecutionContext> : FieldBase<TEntity, TExecutionContext>
        where TEntity : class
    {
        public TypedField(RelationRegistry<TExecutionContext> registry, BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent, string name, Type fieldType)
            : base(registry, parent, name)
        {
            FieldType = fieldType;
        }

        public override UnionField<TEntity, TExecutionContext> ApplyUnion<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>> build, bool isList)
            where TLastElementType : class
        {
            if (FieldType == null || (!FieldType.IsAssignableFrom(typeof(TLastElementType)) && !FieldType.IsAssignableFrom(typeof(IEnumerable<TLastElementType>))))
            {
                // TODO Throw exception with meaningful message
                throw new NotSupportedException();
            }

            return base.ApplyUnion(build, isList);
        }
    }

    internal class TypedField<TEntity, TReturnType, TExecutionContext> : TypedField<TEntity, TExecutionContext>
        where TEntity : class
    {
        protected TypedField(RelationRegistry<TExecutionContext> registry, BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent, string name)
            : base(registry, parent, name, typeof(TReturnType))
        {
        }

        public new IFieldEditSettings<TEntity, TReturnType, TExecutionContext>? EditSettings
        {
            get => (IFieldEditSettings<TEntity, TReturnType, TExecutionContext>?)base.EditSettings;
            set => base.EditSettings = value;
        }
    }
}
