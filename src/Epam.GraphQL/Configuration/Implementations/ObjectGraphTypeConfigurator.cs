// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Loaders;
using GraphQL.Types;

namespace Epam.GraphQL.Configuration.Implementations
{
    internal class ObjectGraphTypeConfigurator<TEntity, TExecutionContext> : BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext>
        where TEntity : class
    {
        public ObjectGraphTypeConfigurator(IField<TExecutionContext>? parent, IRegistry<TExecutionContext> registry, bool isAuto, bool shouldSetNames = true)
            : base(new ConfigurationContextBase(), parent, registry, isAuto)
        {
            if (shouldSetNames)
            {
                Name = isAuto ? Registry.GetGraphQLAutoTypeName<TEntity>(false) : Registry.GetGraphQLTypeName<TEntity>(false, Parent);
            }
        }

        protected ObjectGraphTypeConfigurator(
            ConfigurationContextBase configurationContext,
            IField<TExecutionContext>? parent,
            IRegistry<TExecutionContext> registry)
            : base(configurationContext, parent, registry, isAuto: false)
        {
        }

        public override string GetGraphQLTypeName(Type entityType, Type? projectionType, IField<TExecutionContext> field)
        {
            return Registry.GetGraphQLTypeName(entityType, null, false, field);
        }

        public override IGraphTypeDescriptor<TReturnType, TExecutionContext> GetGraphQLTypeDescriptor<TReturnType>(IField<TExecutionContext> parent, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return Registry.GetGraphTypeDescriptor(parent, build);
        }

        public override IGraphTypeDescriptor<TReturnType, TExecutionContext> GetGraphQLTypeDescriptor<TReturnType>(IField<TExecutionContext> parent)
        {
            return Registry.GetGraphTypeDescriptor<TReturnType>(parent);
        }

        public override IGraphTypeDescriptor<TReturnType, TExecutionContext> GetGraphQLTypeDescriptor<TProjection, TReturnType>()
        {
            return Registry.GetGraphTypeDescriptor<TProjection, TReturnType>();
        }

        public override Type GenerateGraphType() => Registry.GenerateGraphType(typeof(TEntity));

        public override void ConfigureGraphType(IComplexGraphType graphType)
        {
            ValidateFields();

            graphType.Name = Name;

            foreach (var field in Fields)
            {
                graphType.AddField(field.AsFieldType());
            }
        }

        private protected override void DoValidateFields()
        {
            base.DoValidateFields();

            var duplicateSorterName = Sorters
                .GroupBy(sorter => sorter.Name)
                .Where(group => group.Count() > 1)
                .Select(group => group.Key)
                .FirstOrDefault();

            if (duplicateSorterName != null)
            {
                throw new InvalidOperationException($"A sorter with the name `{duplicateSorterName}` is already registered.");
            }
        }
    }

    internal class ObjectGraphTypeConfigurator<TProjection, TEntity, TExecutionContext> : ObjectGraphTypeConfigurator<TEntity, TExecutionContext>
        where TProjection : ProjectionBase<TEntity, TExecutionContext>, new()
        where TEntity : class
    {
        private TProjection? _projection;
        private IObjectGraphTypeConfigurator<TEntity, TExecutionContext>? _baseConfigurator;

        public ObjectGraphTypeConfigurator(IField<TExecutionContext>? parent, IRegistry<TExecutionContext> registry)
            : base(new ConfigurationContext<TProjection, TEntity, TExecutionContext>(), parent, registry)
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
                    _baseConfigurator = Registry.Register<TEntity>(baseProjectionType, null);
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
