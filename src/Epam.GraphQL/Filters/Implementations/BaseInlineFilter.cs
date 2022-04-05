// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Enums;
using Epam.GraphQL.Configuration.Implementations.Fields.ExpressionFields;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Enums;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Filters.Inputs;

namespace Epam.GraphQL.Filters.Implementations
{
    internal abstract class BaseInlineFilter<TEntity, TReturnType, TItemType, TListItemType, TExecutionContext> : IInlineFilter<TExecutionContext>
        where TEntity : class
    {
        private readonly ExpressionField<TEntity, TReturnType, TExecutionContext> _field;
        private readonly TReturnType[]? _defaultValues;
        private readonly NullOption? _nullValue;

        protected BaseInlineFilter(ExpressionField<TEntity, TReturnType, TExecutionContext> field, TReturnType[]? defaultValues, NullOption? nullValue)
        {
            _field = field;
            _defaultValues = defaultValues;
            _nullValue = nullValue;
        }

        public Type FieldType => _field.FieldType;

        public string FieldName => _field.Name;

        public Type FilterType => FieldType.IsSupportComparisons()
            ? typeof(ComparisonsFilter<TItemType, TListItemType>)
            : typeof(InFilter<TItemType, TListItemType>);

        public MethodCallConfigurationContext ConfigurationContext => _field.ConfigurationContext;

        LambdaExpression IInlineFilter<TExecutionContext>.BuildExpression(TExecutionContext context, object? filter)
        {
            return BuildExpression(context, (EqFilter<TItemType>?)filter);
        }

        public Expression<Func<TEntity, bool>> BuildExpression(TExecutionContext context, EqFilter<TItemType>? filter)
        {
            Expression<Func<TEntity, bool>>? result = null;

            if (filter == null)
            {
                if (_defaultValues != null && _defaultValues.Any())
                {
                    result = BuildContainsAsExpression(context, _defaultValues);
                }

                if (_nullValue != null)
                {
                    result = result.SafeAnd(BuildIsNullExpression(context, _nullValue.Value == NullOption.NullValues));
                }
            }
            else
            {
                if (filter is ComparisonsFilter<TItemType, TListItemType> comparisonsFilter)
                {
                    if (comparisonsFilter.Gt != null)
                    {
                        result = BuildComparisonExpression(context, ComparisonType.Gt, comparisonsFilter.Gt);
                    }

                    if (comparisonsFilter.Lt != null)
                    {
                        result = result.SafeAnd(BuildComparisonExpression(context, ComparisonType.Lt, comparisonsFilter.Lt));
                    }

                    if (comparisonsFilter.Gte != null)
                    {
                        result = result.SafeAnd(BuildComparisonExpression(context, ComparisonType.Gte, comparisonsFilter.Gte));
                    }

                    if (comparisonsFilter.Lte != null)
                    {
                        result = result.SafeAnd(BuildComparisonExpression(context, ComparisonType.Lte, comparisonsFilter.Lte));
                    }
                }

                if (filter is InFilter<TItemType, TListItemType> inFilter)
                {
                    if (inFilter.In != null && inFilter.In.Any())
                    {
                        result = result.SafeAnd(BuildContainsAsExpression(context, inFilter.In.Cast<TReturnType>()));
                    }

                    if (inFilter.Nin != null && inFilter.Nin.Any())
                    {
                        result = result.SafeAnd(BuildContainsAsExpression(context, inFilter.Nin.Cast<TReturnType>()).Not());
                    }
                }

                if (filter.IsNull.HasValue)
                {
                    result = result.SafeAnd(BuildIsNullExpression(context, filter.IsNull.Value));
                }

                if (filter.Eq != null)
                {
                    result = result.SafeAnd(BuildComparisonExpression(context, ComparisonType.Eq, filter.Eq));
                }

                if (filter.Neq != null)
                {
                    result = result.SafeAnd(BuildComparisonExpression(context, ComparisonType.Neq, filter.Neq));
                }
            }

            if (result == null)
            {
                result = r => true;
            }

            return result;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(_field);
        }

        public override bool Equals(object? obj) => Equals(obj as BaseInlineFilter<TEntity, TReturnType, TItemType, TListItemType, TExecutionContext>);

        public bool Equals(BaseInlineFilter<TEntity, TReturnType, TItemType, TListItemType, TExecutionContext>? other)
        {
            if (other == null)
            {
                return false;
            }

            return other._field.Equals(_field);
        }

        public bool Equals(IInlineFilter<TExecutionContext> other) => Equals(other as BaseInlineFilter<TEntity, TReturnType, TItemType, TListItemType, TExecutionContext>);

        public Expression<Func<TEntity, TReturnType>> BuildExpression(TExecutionContext context)
        {
            var expression = (Expression<Func<TExecutionContext, TEntity, TReturnType>>)_field.ContextExpression;
            var result = expression.BindFirstParameter(context);
            return result;
        }

        protected abstract Expression<Func<TEntity, bool>> BuildContainsAsExpression(TExecutionContext context, IEnumerable<TReturnType> list);

        protected abstract Expression<Func<TEntity, bool>> BuildIsNullExpression(TExecutionContext context, bool isNull);

        protected abstract Expression<Func<TEntity, bool>> BuildComparisonExpression(TExecutionContext context, ComparisonType comparisonType, TItemType value);
    }
}
