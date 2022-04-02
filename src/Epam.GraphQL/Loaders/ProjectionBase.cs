// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Projection;
using Epam.GraphQL.Builders.Projection.Implementations;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Configuration.Implementations;
using Epam.GraphQL.Configuration.Implementations.Fields;
using Epam.GraphQL.Configuration.Implementations.Fields.ExpressionFields;
using Epam.GraphQL.Helpers;

namespace Epam.GraphQL.Loaders
{
    public abstract class ProjectionBase<TExecutionContext> : IEquatable<ProjectionBase<TExecutionContext>>
    {
        internal RelationRegistry<TExecutionContext> Registry { get; set; } = null!;

        internal virtual bool ShouldConfigureInputType => false;

        public override int GetHashCode() => GetType().GetHashCode();

        public override bool Equals(object obj) => Equals(obj as ProjectionBase<TExecutionContext>);

        public bool Equals(ProjectionBase<TExecutionContext>? other)
        {
            if (other == null)
            {
                return false;
            }

            return other.GetType().Equals(GetType());
        }

        internal abstract void Configure();

        internal abstract void ConfigureInput();

        internal abstract void AfterConstruction();

        internal abstract IObjectGraphTypeConfigurator<TExecutionContext> GetObjectGraphTypeConfigurator();

        internal abstract IObjectGraphTypeConfigurator<TExecutionContext> GetInputObjectGraphTypeConfigurator();

        protected abstract void OnConfigure();

        protected virtual void AfterConfigure()
        {
        }
    }

    public abstract class ProjectionBase<TEntity, TExecutionContext> : ProjectionBase<TExecutionContext>
        where TEntity : class
    {
        internal ObjectGraphTypeConfigurator<TEntity, TExecutionContext> ObjectGraphTypeConfigurator { get; private set; } = null!;

        internal InputObjectGraphTypeConfigurator<TEntity, TExecutionContext> InputObjectGraphTypeConfigurator { get; private set; } = null!;

        protected internal string? Name { get; set; }

        protected internal string? InputName { get; set; }

        private protected BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext>? Configurator { get; private set; }

        private protected bool IsConfiguringInputType { get; set; }

        internal override void Configure() => Configure(ObjectGraphTypeConfigurator);

        internal override void ConfigureInput() => ConfigureInput(InputObjectGraphTypeConfigurator);

        internal override void AfterConstruction()
        {
            ObjectGraphTypeConfigurator = Registry.Register<TEntity>(GetType(), null);
            InputObjectGraphTypeConfigurator = Registry.RegisterInput<TEntity>(GetType(), null);
        }

        internal override IObjectGraphTypeConfigurator<TExecutionContext> GetObjectGraphTypeConfigurator()
        {
            return ObjectGraphTypeConfigurator;
        }

        internal override IObjectGraphTypeConfigurator<TExecutionContext> GetInputObjectGraphTypeConfigurator()
        {
            return InputObjectGraphTypeConfigurator;
        }

        protected internal IProjectionFieldBuilder<TEntity, TExecutionContext> Field(string name, string? deprecationReason = null)
        {
            var field = AddField(name, deprecationReason);
            return new ProjectionFieldBuilder<Field<TEntity, TExecutionContext>, TEntity, TExecutionContext>(field);
        }

        [MemberNotNull(nameof(Configurator))]
        private protected void ThrowIfIsNotConfiguring()
        {
            Guards.ThrowInvalidOperationIf(Configurator == null, $"Calling configuring methods are allowed from {nameof(OnConfigure)} method only.");
        }

        private protected ExpressionField<TEntity, TReturnType, TExecutionContext> AddField<TReturnType>(
            Expression<Func<TEntity, TReturnType>> expression,
            string? deprecationReason)
        {
            ThrowIfIsNotConfiguring();
            return Configurator.AddField(
                Configurator.ConfigurationContext.Operation(nameof(Field)).Argument(expression),
                null,
                expression,
                deprecationReason);
        }

        private protected ExpressionField<TEntity, TReturnType, TExecutionContext> AddField<TReturnType>(
            string name,
            Expression<Func<TEntity, TReturnType>> expression,
            string? deprecationReason)
        {
            ThrowIfIsNotConfiguring();
            return Configurator.AddField(
                Configurator.ConfigurationContext.Operation(nameof(Field))
                    .Argument(name)
                    .Argument(expression),
                name,
                expression,
                deprecationReason);
        }

        private protected ExpressionField<TEntity, TReturnType, TExecutionContext> AddField<TReturnType>(
            string name,
            Expression<Func<TExecutionContext, TEntity, TReturnType>> expression,
            string? deprecationReason)
        {
            ThrowIfIsNotConfiguring();
            return Configurator.AddField(
                Configurator.ConfigurationContext.Operation(nameof(Field))
                    .Argument(name)
                    .Argument(expression),
                name,
                expression,
                deprecationReason);
        }

        private protected Field<TEntity, TExecutionContext> AddField(string name, string? deprecationReason)
        {
            ThrowIfIsNotConfiguring();
            return Configurator.Field(name, deprecationReason);
        }

        private protected void AddOnEntityLoaded<T>(Expression<Func<TEntity, T>> proxyExpression, Action<TExecutionContext, T> hook)
        {
            if (!IsConfiguringInputType)
            {
                ThrowIfIsNotConfiguring();
                Configurator.AddOnEntityLoaded(proxyExpression, hook);
            }
        }

        private protected void AddOnEntityLoaded<TKey, T>(
            Expression<Func<TEntity, TKey>> keyExpression,
            Func<TExecutionContext, IEnumerable<TKey>, IDictionary<TKey, T>> batchFunc,
            Action<TExecutionContext, T> hook)
        {
            if (!IsConfiguringInputType)
            {
                ThrowIfIsNotConfiguring();
                Configurator.AddOnEntityLoaded(keyExpression, batchFunc, hook);
            }
        }

        private protected void AddOnEntityLoaded<TKey, T>(
            Expression<Func<TEntity, TKey>> keyExpression,
            Func<TExecutionContext, IEnumerable<TKey>, Task<IDictionary<TKey, T>>> batchFunc,
            Action<TExecutionContext, T> hook)
        {
            if (!IsConfiguringInputType)
            {
                ThrowIfIsNotConfiguring();
                Configurator.AddOnEntityLoaded(keyExpression, batchFunc, hook);
            }
        }

        private void Configure(BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> сonfigurator)
        {
            if (Configurator != null)
            {
                return;
            }

            Configurator = сonfigurator;
            try
            {
                IsConfiguringInputType = false;
                OnConfigure();
                AfterConfigure();
            }
            finally
            {
                Configurator = null;
            }
        }

        private void ConfigureInput(BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> сonfigurator)
        {
            if (ShouldConfigureInputType)
            {
                if (Configurator != null)
                {
                    return;
                }

                Configurator = сonfigurator;
                try
                {
                    IsConfiguringInputType = true;
                    OnConfigure();
                    AfterConfigure();
                }
                finally
                {
                    Configurator = null;
                }
            }
        }
    }
}
