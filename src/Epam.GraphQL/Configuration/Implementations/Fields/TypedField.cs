﻿// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration.Implementations.Fields.Helpers;
using Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields;
using Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields.Helpers;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal class TypedField<TEntity, TExecutionContext> : Field<TEntity, TExecutionContext>
        where TEntity : class
    {
        public TypedField(RelationRegistry<TExecutionContext> registry, BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent, string name, Type fieldType)
            : base(registry, parent, name)
        {
            FieldType = fieldType;
        }

        public override ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            return ResolvableTypedFieldHelpers.ApplyResolve(this, Resolvers.ConvertFieldResolver(resolve, doesDependOnAllFields), doesDependOnAllFields, optionsBuilder);
        }

        public override ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            return ResolvableTypedFieldHelpers.ApplyResolve(this, Resolvers.ConvertFieldResolver(resolve, doesDependOnAllFields), doesDependOnAllFields, optionsBuilder);
        }

        public override ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class
        {
            return ResolvableTypedFieldHelpers.ApplyResolve(this, Resolvers.ConvertFieldResolver(resolve, doesDependOnAllFields), build, doesDependOnAllFields, optionsBuilder);
        }

        public override ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class
        {
            return ResolvableTypedFieldHelpers.ApplyResolve(this, Resolvers.ConvertFieldResolver(resolve, doesDependOnAllFields), build, doesDependOnAllFields, optionsBuilder);
        }

        public override ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            return ResolvableTypedFieldHelpers.ApplyResolve(this, Resolvers.ConvertFieldResolver(resolve, doesDependOnAllFields), doesDependOnAllFields, optionsBuilder);
        }

        public override ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            return ResolvableTypedFieldHelpers.ApplyResolve(this, Resolvers.ConvertFieldResolver(resolve, doesDependOnAllFields), doesDependOnAllFields, optionsBuilder);
        }

        public override ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class
        {
            return ResolvableTypedFieldHelpers.ApplyResolve(this, Resolvers.ConvertFieldResolver(resolve, doesDependOnAllFields), build, doesDependOnAllFields, optionsBuilder);
        }

        public override ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class
        {
            return ResolvableTypedFieldHelpers.ApplyResolve(this, Resolvers.ConvertFieldResolver(resolve, doesDependOnAllFields), build, doesDependOnAllFields, optionsBuilder);
        }

        public override UnionField<TEntity, TExecutionContext> ApplyUnion<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>> build, bool isList)
            where TLastElementType : class
        {
            if (!FieldType.IsAssignableFrom(typeof(TLastElementType)) && !FieldType.IsAssignableFrom(typeof(IEnumerable<TLastElementType>)))
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
        private readonly IGraphTypeDescriptor<TExecutionContext> _graphType;

        public TypedField(RelationRegistry<TExecutionContext> registry, BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent, string name)
            : base(registry, parent, name, typeof(TReturnType))
        {
        }

        public TypedField(RelationRegistry<TExecutionContext> registry, BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent, string name, IGraphTypeDescriptor<TExecutionContext> graphType)
            : this(registry, parent, name)
        {
            _graphType = graphType ?? throw new ArgumentNullException(nameof(graphType));
        }

        public override IGraphTypeDescriptor<TExecutionContext> GraphType => _graphType;

        public new IFieldEditSettings<TEntity, TReturnType, TExecutionContext> EditSettings
        {
            get => (IFieldEditSettings<TEntity, TReturnType, TExecutionContext>)base.EditSettings;
            set => base.EditSettings = value;
        }
    }

    internal class TypedField<TEntity, TProjection, TReturnType, TExecutionContext> : TypedField<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
        where TReturnType : class
        where TProjection : ProjectionBase<TReturnType, TExecutionContext>, new()
    {
        public TypedField(RelationRegistry<TExecutionContext> registry, BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent, string name)
            : base(registry, parent, name, parent.GetGraphQLTypeDescriptor<TProjection, TReturnType>())
        {
        }
    }
}