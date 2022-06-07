// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Filters.Inputs;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Infrastructure;

namespace Epam.GraphQL.Filters.Implementations
{
    internal sealed class InlineFilters<TEntity, TExecutionContext> : IInlineFilters<TEntity, TExecutionContext>
    {
        private readonly List<IInlineFilter<TExecutionContext>> _inlineFilters = new();
        private readonly string _name;
        private Type? _filterType;

        public InlineFilters(IEnumerable<IInlineFilter<TExecutionContext>> filters, string name)
        {
            _name = name;
            _inlineFilters.AddRange(filters);
        }

        public Type FilterType
        {
            get
            {
                if (_filterType == null)
                {
                    var typeName = $"{Guid.NewGuid()}.{_name}";

                    var typeBuilder = ILUtils.DefineType(typeName, typeof(FilterBase));
                    var listType = typeof(List<>).MakeGenericType(typeBuilder);

                    typeBuilder.DefineProperty(typeBuilder.DefineBackingField("Not", typeBuilder), "Not");
                    typeBuilder.DefineProperty(typeBuilder.DefineBackingField("And", listType), "And");
                    typeBuilder.DefineProperty(typeBuilder.DefineBackingField("Or", listType), "Or");

                    // TODO Checks for Not, And and Or property names and raise exception
                    foreach (var inlineFilter in _inlineFilters)
                    {
                        typeBuilder.DefineProperty(typeBuilder.DefineBackingField(inlineFilter.FieldName, inlineFilter.FilterType), inlineFilter.FieldName);
                    }

                    _filterType = typeBuilder.CreateTypeInfo();
                }

                return _filterType;
            }
        }

        public IQueryable<TEntity> All(ISchemaExecutionListener listener, IQueryable<TEntity> query, TExecutionContext context, object filter)
        {
            var resultExpression = BuildExpression(context, filter);

            if (resultExpression == null)
            {
                return query;
            }

            return query.Where(resultExpression);
        }

        public override int GetHashCode()
        {
            var hashCode = default(HashCode);

            foreach (var item in _inlineFilters)
            {
                hashCode.Add(item);
            }

            return hashCode.ToHashCode();
        }

        public override bool Equals(object obj) => Equals(obj as IFilter<TEntity, TExecutionContext>);

        public bool Equals(IFilter<TEntity, TExecutionContext>? obj)
        {
            if (obj == null)
            {
                return false;
            }

            if (obj is InlineFilters<TEntity, TExecutionContext> filter)
            {
                if (filter._inlineFilters.Count != _inlineFilters.Count)
                {
                    return false;
                }

                for (var i = 0; i < _inlineFilters.Count; i++)
                {
                    if (!filter._inlineFilters[i].Equals(_inlineFilters[i]))
                    {
                        return false;
                    }
                }

                return true;
            }

            return false;
        }

        LambdaExpression IInlineFilters<TExecutionContext>.BuildExpression(TExecutionContext executionContext, object? filter)
        {
            return BuildExpression(executionContext, filter);
        }

        private Expression<Func<TEntity, bool>>? BuildNotExpression(TExecutionContext context, object? filter)
        {
            if (filter == null)
            {
                return null;
            }

            var filterExpression = BuildExpression(context, filter);
            return filterExpression.Not();
        }

        private Expression<Func<TEntity, bool>>? BuildAndExpression(TExecutionContext context, object? filter)
        {
            if (filter == null || !(filter.GetType().IsGenericType && filter.GetType().GetGenericTypeDefinition() == typeof(List<>)))
            {
                return null;
            }

            var list = (IEnumerable<object>)filter;

            return list
                .Select(item => BuildExpression(context, item))
                .Aggregate((acc, f) => acc == null ? f : (f == null ? acc : acc.And(f)));
        }

        private Expression<Func<TEntity, bool>>? BuildOrExpression(TExecutionContext context, object? filter)
        {
            if (filter == null || !(filter.GetType().IsGenericType && filter.GetType().GetGenericTypeDefinition() == typeof(List<>)))
            {
                return null;
            }

            var list = (IEnumerable<object>)filter;

            return list
                .Select(item => BuildExpression(context, item))
                .Aggregate((acc, f) => acc == null ? f : (f == null ? acc : acc.Or(f)));
        }

        private Expression<Func<TEntity, bool>> BuildFieldExpression(TExecutionContext context, PropertyInfo propInfo, object? propValue)
        {
            var inlineFilter = _inlineFilters.First(f => f.FieldName == propInfo.Name);
            return (Expression<Func<TEntity, bool>>)inlineFilter.BuildExpression(context, propValue);
        }

        private Expression<Func<TEntity, bool>> BuildExpression(TExecutionContext context, object? filter)
        {
            Expression<Func<TEntity, bool>>? resultExpression = null;

            if (filter != null)
            {
                foreach (var propInfo in filter.GetType().GetProperties())
                {
                    var propValue = filter.GetPropertyValue(propInfo);
                    Expression<Func<TEntity, bool>>? expression = null;
                    if (propInfo.Name == "Not")
                    {
                        expression = BuildNotExpression(context, propValue);
                    }
                    else if (propInfo.Name == "And")
                    {
                        expression = BuildAndExpression(context, propValue);
                    }
                    else if (propInfo.Name == "Or")
                    {
                        expression = BuildOrExpression(context, propValue);
                    }
                    else if (_inlineFilters.Select(f => f.FieldName).Contains(propInfo.Name))
                    {
                        expression = BuildFieldExpression(context, propInfo, propValue);
                    }

                    if (resultExpression == null)
                    {
                        resultExpression = expression;
                    }
                    else if (expression != null)
                    {
                        resultExpression = resultExpression.And(expression);
                    }
                }
            }

            return resultExpression != null
                ? ExpressionRewriter.Rewrite(resultExpression)
                : FuncConstants<TEntity>.TrueExpression;
        }
    }
}
