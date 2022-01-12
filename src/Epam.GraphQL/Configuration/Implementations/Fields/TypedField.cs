// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

#nullable enable

using System;

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal class TypedField<TEntity, TReturnType, TExecutionContext> : FieldBase<TEntity, TExecutionContext>
        where TEntity : class
    {
        protected TypedField(RelationRegistry<TExecutionContext> registry, BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent, string name)
            : base(registry, parent, name)
        {
        }

        public override Type FieldType => typeof(TReturnType);

        public new IFieldEditSettings<TEntity, TReturnType, TExecutionContext>? EditSettings
        {
            get => (IFieldEditSettings<TEntity, TReturnType, TExecutionContext>?)base.EditSettings;
            set => base.EditSettings = value;
        }
    }
}
