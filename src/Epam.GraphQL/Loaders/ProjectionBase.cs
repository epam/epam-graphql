// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Builders.Projection;
using Epam.GraphQL.Builders.Projection.Implementations;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Configuration.Implementations;
using Epam.GraphQL.Configuration.Implementations.Fields;
using Epam.GraphQL.Configuration.Implementations.Fields.ExpressionFields;

namespace Epam.GraphQL.Loaders
{
    public abstract class ProjectionBase<TExecutionContext> : IEquatable<ProjectionBase<TExecutionContext>>
    {
        internal RelationRegistry<TExecutionContext> Registry { get; set; }

        internal virtual bool ShouldConfigureInputType => false;

        public override int GetHashCode() => GetType().GetHashCode();

        public override bool Equals(object obj) => Equals(obj as ProjectionBase<TExecutionContext>);

        public bool Equals(ProjectionBase<TExecutionContext> other)
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
        internal ObjectGraphTypeConfigurator<TEntity, TExecutionContext> ObjectGraphTypeConfigurator { get; private set; }

        internal InputObjectGraphTypeConfigurator<TEntity, TExecutionContext> InputObjectGraphTypeConfigurator { get; private set; }

        protected internal string Name { get; set; }

        protected internal string InputName { get; set; }

        private protected BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> Configurator { get; private set; }

        private protected bool IsConfiguringInputType { get; set; }

        internal void Configure(BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> сonfigurator)
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

        internal void ConfigureInput(BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> сonfigurator)
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

        protected internal IProjectionFieldBuilder<TEntity, TExecutionContext> Field(string name, string deprecationReason = null)
        {
            var field = AddField(name, deprecationReason);
            return new ProjectionFieldBuilder<TEntity, TExecutionContext>(field);
        }

        private protected void ThrowIfIsNotConfiguring()
        {
            if (Configurator == null)
            {
                throw new InvalidOperationException($"Calling configuring methods are allowed from {nameof(OnConfigure)} method only.");
            }
        }

        private protected StructExpressionField<TEntity, TReturnType, TExecutionContext> AddField<TReturnType>(string name, Expression<Func<TEntity, TReturnType>> expression, string deprecationReason = null)
            where TReturnType : struct
        {
            ThrowIfIsNotConfiguring();
            return Configurator.AddField(name, expression, deprecationReason);
        }

        private protected StructExpressionField<TEntity, TReturnType, TExecutionContext> AddField<TReturnType>(string name, Expression<Func<TExecutionContext, TEntity, TReturnType>> expression, string deprecationReason = null)
            where TReturnType : struct
        {
            ThrowIfIsNotConfiguring();
            return Configurator.AddField(name, expression, deprecationReason);
        }

        private protected NullableExpressionField<TEntity, TReturnType, TExecutionContext> AddField<TReturnType>(string name, Expression<Func<TEntity, TReturnType?>> expression, string deprecationReason = null)
            where TReturnType : struct
        {
            ThrowIfIsNotConfiguring();
            return Configurator.AddField(name, expression, deprecationReason);
        }

        private protected NullableExpressionField<TEntity, TReturnType, TExecutionContext> AddField<TReturnType>(string name, Expression<Func<TExecutionContext, TEntity, TReturnType?>> expression, string deprecationReason = null)
            where TReturnType : struct
        {
            ThrowIfIsNotConfiguring();
            return Configurator.AddField(name, expression, deprecationReason);
        }

        private protected StringExpressionField<TEntity, TExecutionContext> AddField(string name, Expression<Func<TEntity, string>> expression, string deprecationReason = null)
        {
            ThrowIfIsNotConfiguring();
            return Configurator.AddField(name, expression, deprecationReason);
        }

        private protected StringExpressionField<TEntity, TExecutionContext> AddField(string name, Expression<Func<TExecutionContext, TEntity, string>> expression, string deprecationReason = null)
        {
            ThrowIfIsNotConfiguring();
            return Configurator.AddField(name, expression, deprecationReason);
        }

        private protected Field<TEntity, TExecutionContext> AddField(string name, string deprecationReason)
        {
            ThrowIfIsNotConfiguring();
            return Configurator.AddField(name, deprecationReason);
        }

        private protected TypedField<TEntity, TReturnType, TExecutionContext> AddField<TReturnType>(string name, string deprecationReason)
        {
            ThrowIfIsNotConfiguring();
            return Configurator.AddField<TReturnType>(name, deprecationReason);
        }

        private protected TypedField<TEntity, TReturnType, TExecutionContext> AddField<TProjection, TReturnType>(string name, string deprecationReason)
            where TProjection : ProjectionBase<TReturnType, TExecutionContext>, new()
            where TReturnType : class
        {
            ThrowIfIsNotConfiguring();
            return Configurator.AddField<TProjection, TReturnType>(name, deprecationReason);
        }

        private protected void AddFilter<TValueType>(string name, Func<TValueType, Expression<Func<TEntity, bool>>> filterPredicateFactory)
        {
            ThrowIfIsNotConfiguring();
            if (!IsConfiguringInputType)
            {
                Configurator.AddFilter(name, filterPredicateFactory);
            }
        }

        private protected void AddFilter<TValueType>(string name, Func<TExecutionContext, TValueType, Expression<Func<TEntity, bool>>> filterPredicateFactory)
        {
            ThrowIfIsNotConfiguring();
            if (!IsConfiguringInputType)
            {
                Configurator.AddFilter(name, filterPredicateFactory);
            }
        }

        private protected void AddSorter<TValueType>(string name, Expression<Func<TEntity, TValueType>> selector)
        {
            if (!IsConfiguringInputType)
            {
                ThrowIfIsNotConfiguring();
                Configurator.AddSorter(name, selector);
            }
        }

        private protected void AddOnEntityLoaded<T>(Expression<Func<TEntity, T>> proxyExpression, Action<TExecutionContext, T> hook)
        {
            if (!IsConfiguringInputType)
            {
                ThrowIfIsNotConfiguring();
                Configurator.AddOnEntityLoaded(proxyExpression, hook);
            }
        }
    }
}
