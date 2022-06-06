// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Loaders;
using GraphQL.Types;

namespace Epam.GraphQL.Configuration.Implementations
{
    internal class InputObjectGraphTypeConfigurator<TEntity, TExecutionContext> : BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext>
    {
        public InputObjectGraphTypeConfigurator(IField<TExecutionContext>? parent, IObjectConfigurationContext context, IRegistry<TExecutionContext> registry, bool isAuto)
            : base(context, parent, registry, isAuto)
        {
            Name = isAuto ? Registry.GetGraphQLAutoTypeName<TEntity>(true) : Registry.GetGraphQLTypeName<TEntity>(true, parent);
        }

        protected InputObjectGraphTypeConfigurator(
            IObjectConfigurationContext configurationContext,
            IField<TExecutionContext>? parent,
            IRegistry<TExecutionContext> registry)
            : base(configurationContext, parent, registry, isAuto: false)
        {
        }

        public override string GetGraphQLTypeName(Type entityType, Type? projectionType, IField<TExecutionContext> field)
        {
            return Registry.GetGraphQLTypeName(entityType, null, true, field);
        }

        public override IGraphTypeDescriptor<TReturnType, TExecutionContext> GetGraphQLTypeDescriptor<TReturnType>(
            IField<TExecutionContext> parent,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build,
            IChainConfigurationContext configurationContext)
        {
            return Registry.GetInputGraphTypeDescriptor(parent, build, configurationContext);
        }

        public override void ConfigureGraphType(IComplexGraphType graphType)
        {
            if (graphType is not IInputObjectGraphType inputGraphType)
            {
                throw new InvalidOperationException();
            }

            inputGraphType.Name = Name;
            foreach (var field in Fields.Where(f => f.EditSettings != null && !f.EditSettings.IsReadOnly))
            {
                inputGraphType.AddField(field.AsFieldType());
            }
        }

        private protected override void DoValidateFields()
        {
            base.DoValidateFields();

            if (Fields.All(field => field.EditSettings != null && field.EditSettings.IsReadOnly))
            {
                throw new InvalidOperationException($"{(IsAuto ? $"Type `{typeof(TEntity).HumanizedName()}`" : "OnConfigure() method")} must have one writable field at least. Consider calling Editable() or EditableIf(...) during fields' configuration one time at least --or-- consider inheriting loader from {typeof(Loader<,>).HumanizedName()}, not from {typeof(MutableLoader<,,>).HumanizedName()}");
            }
        }
    }

    internal class InputObjectGraphTypeConfigurator<TProjection, TEntity, TExecutionContext> : InputObjectGraphTypeConfigurator<TEntity, TExecutionContext>
        where TProjection : ProjectionBase<TEntity, TExecutionContext>, new()
    {
        private TProjection? _projection;
        private IObjectGraphTypeConfigurator<TEntity, TExecutionContext>? _baseConfigurator;

        public InputObjectGraphTypeConfigurator(IRegistry<TExecutionContext> registry)
            : base(Diagnostics.ConfigurationContext.Create<TProjection>(), parent: null, registry)
        {
            Name = Registry.GetProjectionTypeName<TProjection, TEntity>(true);
        }

        public override void ConfigureGraphType(IComplexGraphType graphType)
        {
            Configure();
            base.ConfigureGraphType(graphType);
        }

        protected override void OnConfigure()
        {
            var projection = GetProjection();
            _projection.ConfigureInput();

            if (projection.InputName != null)
            {
                Name = projection.InputName;
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
                    _baseConfigurator = Registry.RegisterInput<TEntity>(baseProjectionType);
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
