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
        IExpressionField<TEntity, TExecutionContext>
        where TEntity : class
    {
        private readonly IFieldExpression<TEntity, TReturnType, TExecutionContext> _expression;

        public ExpressionField(
            FieldConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            Expression<Func<TEntity, TReturnType>> expression,
            string? name)
            : base(
                  configurationContext,
                  parent,
                  GenerateName(configurationContext, name, expression))
        {
            _expression = new FieldExpression<TEntity, TReturnType, TExecutionContext>(this, Name, expression);
            EditSettings = new FieldEditSettings<TEntity, TReturnType, TExecutionContext>(_expression);
        }

        public ExpressionField(
            FieldConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            Expression<Func<TExecutionContext, TEntity, TReturnType>> expression,
            string name)
            : base(
                  configurationContext,
                  parent,
                  name)
        {
            _expression = new FieldContextExpression<TEntity, TReturnType, TExecutionContext>(this, Name, expression);
            EditSettings = new FieldEditSettings<TEntity, TReturnType, TExecutionContext>(_expression);
        }

        public override IGraphTypeDescriptor<TExecutionContext> GraphType
        {
            get
            {
                IGraphTypeDescriptor<TExecutionContext> graphType = Parent.GetGraphQLTypeDescriptor<TReturnType>(this);

                if (Parent is InputObjectGraphTypeConfigurator<TEntity, TExecutionContext> && !(EditSettings?.IsMandatoryForUpdate ?? false))
                {
                    graphType = graphType.UnwrapIfNonNullable();
                }

                return graphType;
            }
        }

        public bool IsFilterable { get; set; }

        public bool IsGroupable { get; protected set; }

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
                ConfigurationContext.AddError(e.Message);
            }

            base.Validate();
        }

        public void Filterable(TReturnType[]? defaultValues = null)
        {
            if (defaultValues != null && defaultValues.Any(value => value == null))
            {
                throw new ArgumentException(".Filterable() does not support nulls as parameters. Consider using .Filterable(NullValues).");
            }

            DefaultValues = defaultValues;
            IsFilterable = true;
        }

        public void Filterable(NullOption nullValue)
        {
            NullValue = nullValue;
            IsFilterable = true;
        }

        public void Sortable()
        {
            Parent.AddSorter(_expression);
        }

        public void Sortable<TValue>(Expression<Func<TEntity, TValue>> sorter)
        {
            Parent.AddSorter(Name, sorter);
        }

        public void Groupable()
        {
            IsGroupable = true;
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

        protected virtual IInlineFilter<TExecutionContext> OnCreateInlineFilter() => throw new NotSupportedException();

        private static string GenerateName(FieldConfigurationContext configurationContext, string? name, Expression<Func<TEntity, TReturnType>> expression)
        {
            if (name != null)
            {
                return name;
            }

            if (expression.TryGetNameOfProperty(out var propName))
            {
                return propName.ToCamelCase();
            }

            configurationContext.AddError($"Expression ({expression}), provided for field is not a property. Consider giving a name to the field explicitly.");

            return string.Empty;
        }
    }
}
