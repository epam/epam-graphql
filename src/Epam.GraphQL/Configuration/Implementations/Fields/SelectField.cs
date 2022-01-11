// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;
using GraphQL;
using GraphQL.Resolvers;

#nullable enable

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal class SelectField<TEntity, TReturnType, TExecutionContext> : TypedField<TEntity, TReturnType, TExecutionContext>,
        IFieldSupportsEditSettings<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        private readonly IGraphTypeDescriptor<TExecutionContext> _graphType;
        private readonly IResolver<TEntity> _resolver;

        public SelectField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IResolver<TEntity> resolver,
            IGraphTypeDescriptor<TExecutionContext> graphType)
            : base(registry, parent, name)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            EditSettings = new FieldEditSettings<TEntity, TReturnType, TExecutionContext>();
            _graphType = graphType;
        }

        public override bool CanResolve => true;

        public override IGraphTypeDescriptor<TExecutionContext> GraphType
        {
            get
            {
                var graphType = _graphType;

                if (Parent is InputObjectGraphTypeConfigurator<TEntity, TExecutionContext> && EditSettings != null && !EditSettings.IsMandatoryForUpdate)
                {
                    graphType = graphType.UnwrapIfNonNullable();
                }

                return graphType;
            }
        }

        IFieldEditSettings<TEntity, TReturnType, TExecutionContext>? IFieldSupportsEditSettings<TEntity, TReturnType, TExecutionContext>.EditSettings => EditSettings;

        public override object Resolve(IResolveFieldContext context)
        {
            return _resolver.Resolve(context);
        }

        protected override IFieldResolver GetResolver()
        {
            return _resolver;
        }
    }
}
