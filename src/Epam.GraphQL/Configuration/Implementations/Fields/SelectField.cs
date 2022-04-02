// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Helpers;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal class SelectField<TEntity, TReturnType, TExecutionContext> : TypedField<TEntity, TReturnType, TExecutionContext>,
        IVoid,
        IFieldSupportsEditSettings<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        private readonly IGraphTypeDescriptor<TExecutionContext> _graphType;

        public SelectField(
            FieldConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IFieldResolver resolver,
            IGraphTypeDescriptor<TExecutionContext> graphType)
            : base(configurationContext, parent, name)
        {
            Resolver = resolver;
            EditSettings = new FieldEditSettings<TEntity, TReturnType, TExecutionContext>();
            _graphType = graphType;
        }

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

        public override IFieldResolver Resolver { get; }
    }
}
