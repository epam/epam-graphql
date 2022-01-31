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
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Filters;
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

        public ExpressionField(BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent, Expression<Func<TEntity, TReturnType>> expression, string? name)
            : base(
                  parent,
                  GenerateName(name, expression))
        {
            if (name == null && !expression.IsProperty())
            {
                throw new InvalidOperationException($"Expression ({expression}), provided for field is not a property. Consider giving a name to the field explicitly.");
            }

            _expression = new FieldExpression<TEntity, TReturnType, TExecutionContext>(this, Name, expression);
            EditSettings = new FieldEditSettings<TEntity, TReturnType, TExecutionContext>(_expression);
        }

        public ExpressionField(BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent, Expression<Func<TExecutionContext, TEntity, TReturnType>> expression, string name)
            : base(
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

        public PropertyInfo? PropertyInfo => _expression?.PropertyInfo;

        public override bool CanResolve => true;

        public LambdaExpression ContextExpression => _expression.ContextedExpression;

        public LambdaExpression OriginalExpression => _expression.OriginalExpression;

        public override object? Resolve(IResolveFieldContext context)
        {
            return _expression.Resolve(context, context.Source);
        }

        public override void ValidateField()
        {
            if (string.IsNullOrEmpty(Name))
            {
                throw new InvalidOperationException("Field name cannot be null or empty.");
            }

            _expression.ValidateExpression();
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
            if (IsFilterable)
            {
                return OnCreateInlineFilter();
            }

            throw new NotSupportedException();
        }

        public virtual IInlineFilter<TExecutionContext> OnCreateInlineFilter()
        {
            throw new NotSupportedException();
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

        protected override IFieldResolver GetResolver() => _expression;

        private static string GenerateName(string? name, Expression<Func<TEntity, TReturnType>> expression)
        {
            if (name == null && !expression.IsProperty())
            {
                throw new InvalidOperationException($"Expression ({expression}), provided for field is not a property. Consider giving a name to the field explicitly.");
            }

            return name ?? expression.NameOf().ToCamelCase();
        }
    }

    internal class ExpressionField<TEntity, TReturnType, TFilterValueType, TExecutionContext> : ExpressionField<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        public ExpressionField(BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent, Expression<Func<TEntity, TReturnType>> expression, string? name)
            : base(parent, expression, name)
        {
        }

        public ExpressionField(BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent, Expression<Func<TExecutionContext, TEntity, TReturnType>> expression, string name)
            : base(parent, expression, name)
        {
        }

        protected TFilterValueType[]? DefaultValues { get; private set; }

        protected NullOption? NullValue { get; private set; }

        public void Filterable(TFilterValueType[]? defaultValues = null)
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
    }
}
