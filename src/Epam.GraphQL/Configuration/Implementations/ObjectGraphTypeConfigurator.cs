// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Diagnostics.CodeAnalysis;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Loaders;
using GraphQL.Types;

namespace Epam.GraphQL.Configuration.Implementations
{
    internal class ObjectGraphTypeConfigurator<TEntity, TExecutionContext> : BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext>
        where TEntity : class
    {
        public ObjectGraphTypeConfigurator(IField<TExecutionContext>? parent, IObjectConfigurationContext context, IRegistry<TExecutionContext> registry, bool isAuto)
            : base(context, parent, registry, isAuto)
        {
            Name = isAuto ? Registry.GetGraphQLAutoTypeName<TEntity>(false) : Registry.GetGraphQLTypeName<TEntity>(false, parent);
        }

        protected ObjectGraphTypeConfigurator(
            ObjectConfigurationContextBase configurationContext,
            IField<TExecutionContext>? parent,
            IRegistry<TExecutionContext> registry)
            : base(configurationContext, parent, registry, isAuto: false)
        {
        }

        public override string GetGraphQLTypeName(Type entityType, Type? projectionType, IField<TExecutionContext> field)
        {
            return Registry.GetGraphQLTypeName(entityType, null, false, field);
        }

        public override IGraphTypeDescriptor<TReturnType, TExecutionContext> GetGraphQLTypeDescriptor<TReturnType>(
            IField<TExecutionContext> parent,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build,
            IChildConfigurationContext configurationContext)
        {
            return Registry.GetGraphTypeDescriptor(parent, build, configurationContext);
        }

        public override IGraphTypeDescriptor<TReturnType, TExecutionContext> GetGraphQLTypeDescriptor<TReturnType>(IField<TExecutionContext> parent)
        {
            return Registry.GetGraphTypeDescriptor<TReturnType>(parent);
        }

        public override Type GenerateGraphType() => Registry.GenerateGraphType(typeof(TEntity));

        public override void ConfigureGraphType(IComplexGraphType graphType)
        {
            graphType.Name = Name;

            foreach (var field in Fields)
            {
                graphType.AddField(field.AsFieldType());
            }
        }
    }

    internal class ObjectGraphTypeConfigurator<TProjection, TEntity, TExecutionContext> : ObjectGraphTypeConfigurator<TEntity, TExecutionContext>
        where TProjection : ProjectionBase<TEntity, TExecutionContext>, new()
        where TEntity : class
    {
        private TProjection? _projection;
        private IObjectGraphTypeConfigurator<TEntity, TExecutionContext>? _baseConfigurator;

        public ObjectGraphTypeConfigurator(IRegistry<TExecutionContext> registry)
            : base(new ObjectConfigurationContext<TProjection>(), parent: null, registry)
        {
            Name = Registry.GetProjectionTypeName<TProjection, TEntity>(false);
        }

        public override void ConfigureGraphType(IComplexGraphType graphType)
        {
            Configure();
            base.ConfigureGraphType(graphType);
        }

        public override Type GenerateGraphType() => Registry.GetEntityGraphType<TProjection, TEntity>();

        protected override void OnConfigure()
        {
            var projection = GetProjection();
            _projection.Configure();

            if (projection.Name != null)
            {
                Name = projection.Name;
                return;
            }

            var baseConfigurator = GetBaseObjectGraphTypeConfigurator((first, second) => first.Equals(second));
            if (baseConfigurator != null)
            {
                Registry.SetGraphQLTypeName<TProjection, TEntity>(Name, baseConfigurator.Name);
                Name = baseConfigurator.Name;
            }
        }

        protected override IObjectGraphTypeConfigurator<TEntity, TExecutionContext>? GetBaseObjectGraphTypeConfiguratorForFilters()
        {
            return GetBaseObjectGraphTypeConfigurator((first, second) => first.FilterEquals(second));
        }

        protected IObjectGraphTypeConfigurator<TEntity, TExecutionContext>? GetBaseObjectGraphTypeConfigurator(Func<IObjectGraphTypeConfigurator<TExecutionContext>, IObjectGraphTypeConfigurator<TExecutionContext>, bool> predicate)
        {
            if (_baseConfigurator == null)
            {
                var baseProjectionType = Registry.GetPropperBaseProjectionType<TProjection, TEntity>(predicate);

                if (baseProjectionType != typeof(TProjection))
                {
                    _baseConfigurator = Registry.Register<TEntity>(baseProjectionType);
                    _baseConfigurator.Configure();

                    return _baseConfigurator;
                }
            }

            return null;
        }

        protected override void SetGraphQLTypeName(string? oldName, string newName)
        {
            Registry.SetGraphQLTypeName<TProjection, TEntity>(oldName, newName);
        }

        private protected override void DoValidateFields()
        {
            if (!typeof(Mutation<TExecutionContext>).IsAssignableFrom(typeof(TProjection)))
            {
                base.DoValidateFields();
            }
        }

        [MemberNotNull(nameof(_projection))]
        private TProjection GetProjection()
        {
            if (_projection == null)
            {
                _projection = Registry.ResolveLoader<TProjection, TEntity>();
            }

            return _projection;
        }
    }
}
