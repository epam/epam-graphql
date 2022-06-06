// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Epam.GraphQL.Configuration.Enums;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Filters;
using Epam.GraphQL.Helpers;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ExpressionFields
{
    internal class ExpressionField<TEntity, TReturnType, TExecutionContext> :
        TypedField<TEntity, TReturnType, TExecutionContext>,
        IExpressionFieldConfiguration<TEntity, TExecutionContext>,
        IExpressionField<TEntity, TReturnType, TExecutionContext>,
        IInlineExpressionField<TEntity, TReturnType, TExecutionContext>,
        IVoid
    {
        private readonly IFieldExpression<TEntity, TReturnType, TExecutionContext> _expression;
        private bool _isGroupable;

        public ExpressionField(
            Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            Expression<Func<TEntity, TReturnType>> expression,
            string? name)
            : base(
                  configurationContextFactory,
                  parent,
                  configurationContext => GenerateName(configurationContext, name, expression))
        {
            _expression = new FieldExpression<TEntity, TReturnType, TExecutionContext>(this, Name, expression);
            EditSettings = new FieldEditSettings<TEntity, TReturnType, TExecutionContext>(_expression);
        }

        public ExpressionField(
            Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            Expression<Func<TExecutionContext, TEntity, TReturnType>> expression,
            string name)
            : base(
                  configurationContextFactory,
                  parent,
                  configurationContext => name)
        {
            _expression = new FieldContextExpression<TEntity, TReturnType, TExecutionContext>(this, Name, expression);
            EditSettings = new FieldEditSettings<TEntity, TReturnType, TExecutionContext>(_expression);
        }

        public override IGraphTypeDescriptor<TExecutionContext> GraphType
        {
            get
            {
                IGraphTypeDescriptor<TExecutionContext> graphType = Parent.GetGraphQLTypeDescriptor<TReturnType>(this, null, ConfigurationContext);

                if (Parent is InputObjectGraphTypeConfigurator<TEntity, TExecutionContext> && !(EditSettings?.IsMandatoryForUpdate ?? false))
                {
                    graphType = graphType.UnwrapIfNonNullable();
                }

                return graphType;
            }
        }

        public bool IsFilterable { get; set; }

        public bool IsGroupable => _isGroupable || Registry.IsSimpleType(typeof(TEntity));

        public PropertyInfo? PropertyInfo => _expression.PropertyInfo;

        public LambdaExpression ContextExpression => _expression.ContextedExpression;

        public LambdaExpression OriginalExpression => _expression.OriginalExpression;

        public override IFieldResolver Resolver => _expression;

        protected TReturnType[]? DefaultValues { get; private set; }

        protected NullOption? NullValue { get; private set; }

        public override void Validate()
        {
            try
            {
                _expression.ValidateExpression();
            }
            catch (InvalidOperationException e)
            {
                ConfigurationContext.AddError(e.Message, ConfigurationContext);
            }

            base.Validate();
        }

        public ExpressionField<TEntity, TReturnType, TExecutionContext> Filterable(TReturnType[] defaultValues)
        {
            ConfigurationContext.Chain(nameof(Filterable))
                .OptionalArgument(defaultValues);

            if (defaultValues != null && defaultValues.Any(value => value == null))
            {
                throw new ArgumentException(".Filterable() does not support nulls as parameters. Consider using .Filterable(NullValues).");
            }

            DefaultValues = defaultValues;
            IsFilterable = true;

            return this;
        }

        public void Filterable(NullOption nullValue)
        {
            ConfigurationContext.Chain(nameof(Filterable))
                .Argument(nullValue.ToString());
            NullValue = nullValue;
            IsFilterable = true;
        }

        public void Filterable()
        {
            ConfigurationContext.Chain(nameof(Filterable));
            IsFilterable = true;
        }

        public void Sortable()
        {
            ConfigurationContext.Chain(nameof(Sortable));
            Parent.Sorter(_expression);
        }

        public void Sortable<TValue>(Expression<Func<TEntity, TValue>> sorter)
        {
            ConfigurationContext.Chain(nameof(Sortable)).Argument(sorter);
            Parent.Sorter(Name, sorter);
        }

        public void Sortable<TValue>(Func<TExecutionContext, Expression<Func<TEntity, TValue>>> sorterFactory)
        {
            ConfigurationContext.Chain(nameof(Sortable)).Argument(sorterFactory);
            Parent.Sorter(Name, sorterFactory);
        }

        public void Groupable()
        {
            ConfigurationContext.Chain(nameof(Groupable));
            _isGroupable = true;
        }

        public override FieldType AsFieldType()
        {
            var result = base.AsFieldType();

            // Hack against GraphQL.NET: ComplexGraphType<object>.ORIGINAL_EXPRESSION_PROPERTY_NAME is internal.
            result.Metadata.Add("ORIGINAL_EXPRESSION_PROPERTY_NAME", PropertyInfo?.Name);

            return result;
        }

        public IInlineFilter<TExecutionContext> CreateInlineFilter()
        {
            Guards.AssertType<TEntity>(!IsFilterable);

            return OnCreateInlineFilter();
        }

        public override int GetHashCode()
        {
            var hashCode = default(HashCode);
            hashCode.Add(Name);
            hashCode.Add(typeof(TEntity));
            hashCode.Add(typeof(TReturnType));
            hashCode.Add(typeof(TExecutionContext));
            return hashCode.ToHashCode();
        }

        public override bool Equals(object other)
        {
            if (other is null or not ExpressionField<TEntity, TReturnType, TExecutionContext>)
            {
                return false;
            }

            return base.Equals(other);
        }

        ISortableField<IVoid, TEntity, TExecutionContext> IFilterableField<ISortableField<IVoid, TEntity, TExecutionContext>, TEntity, TReturnType>.Filterable()
        {
            Filterable();
            return this;
        }

        ISortableField<IVoid, TEntity, TExecutionContext> IFilterableField<ISortableField<IVoid, TEntity, TExecutionContext>, TEntity, TReturnType>.Filterable(params TReturnType[] defaultValues)
        {
            Filterable(defaultValues);
            return this;
        }

        ISortableField<IVoid, TEntity, TExecutionContext> IFilterableField<ISortableField<IVoid, TEntity, TExecutionContext>, TEntity, TReturnType>.Filterable(NullOption nullValue)
        {
            Filterable(nullValue);
            return this;
        }

        IVoid ISortableField<IVoid, TEntity, TExecutionContext>.Sortable()
        {
            Sortable();
            return this;
        }

        IVoid ISortableField<IVoid, TEntity, TExecutionContext>.Sortable<TValue>(Expression<Func<TEntity, TValue>> sorter)
        {
            Sortable(sorter);
            return this;
        }

        IVoid ISortableField<IVoid, TEntity, TExecutionContext>.Sortable<TValue>(Func<TExecutionContext, Expression<Func<TEntity, TValue>>> sorterFactory)
        {
            Sortable(sorterFactory);
            return this;
        }

        ISortableGroupableField<TEntity, TExecutionContext> IFilterableField<ISortableGroupableField<TEntity, TExecutionContext>, TEntity, TReturnType>.Filterable()
        {
            Filterable();
            return this;
        }

        ISortableGroupableField<TEntity, TExecutionContext> IFilterableField<ISortableGroupableField<TEntity, TExecutionContext>, TEntity, TReturnType>.Filterable(params TReturnType[] defaultValues)
        {
            Filterable(defaultValues);
            return this;
        }

        ISortableGroupableField<TEntity, TExecutionContext> IFilterableField<ISortableGroupableField<TEntity, TExecutionContext>, TEntity, TReturnType>.Filterable(NullOption nullValue)
        {
            Filterable(nullValue);
            return this;
        }

        IGroupableField ISortableField<IGroupableField, TEntity, TExecutionContext>.Sortable()
        {
            Sortable();
            return this;
        }

        IGroupableField ISortableField<IGroupableField, TEntity, TExecutionContext>.Sortable<TValue>(Expression<Func<TEntity, TValue>> sorter)
        {
            Sortable(sorter);
            return this;
        }

        IGroupableField ISortableField<IGroupableField, TEntity, TExecutionContext>.Sortable<TValue>(Func<TExecutionContext, Expression<Func<TEntity, TValue>>> sorterFactory)
        {
            Sortable(sorterFactory);
            return this;
        }

        protected virtual IInlineFilter<TExecutionContext> OnCreateInlineFilter() => throw new NotSupportedException();

        private static string GenerateName(IChainConfigurationContext configurationContext, string? name, Expression<Func<TEntity, TReturnType>> expression)
        {
            if (name != null)
            {
                return name;
            }

            if (expression.TryGetNameOfProperty(out var propName))
            {
                return propName.ToCamelCase();
            }

            configurationContext.AddError($"Expression ({expression}), provided for field is not a property. Consider giving a name to the field explicitly.", configurationContext);

            return string.Empty;
        }
    }
}
