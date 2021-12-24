// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration.Enums;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Filters;
using GraphQL;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ExpressionFields
{
    internal class ExpressionField<TEntity, TReturnType, TExecutionContext> : TypedField<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        private readonly IFieldExpression<TEntity, TReturnType, TExecutionContext> _expression;

        public ExpressionField(RelationRegistry<TExecutionContext> registry, BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent, Expression<Func<TEntity, TReturnType>> expression, string name)
            : base(
                  registry,
                  parent,
                  GenerateName(name, expression))
        {
            if (name == null && !expression.IsProperty())
            {
                throw new InvalidOperationException($"Expression ({expression}), provided for field is not a property. Consider to give a name to field explicitly.");
            }

            _expression = new FieldExpression<TEntity, TReturnType, TExecutionContext>(this, Name, expression);
            EditSettings = new FieldEditSettings<TEntity, TReturnType, TExecutionContext>(_expression);
        }

        public ExpressionField(RelationRegistry<TExecutionContext> registry, BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent, Expression<Func<TExecutionContext, TEntity, TReturnType>> expression, string name)
            : base(
                  registry,
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

                if (Parent is InputObjectGraphTypeConfigurator<TEntity, TExecutionContext> && !EditSettings.IsMandatoryForUpdate)
                {
                    graphType = graphType.UnwrapIfNonNullable();
                }

                return graphType;
            }
        }

        public override bool IsExpression => true;

        public override bool IsFilterable { get; protected set; }

        public override bool IsGroupable { get; protected set; }

        public override PropertyInfo PropertyInfo => _expression?.PropertyInfo;

        public override bool CanResolve => true;

        public override LambdaExpression Expression => _expression.Expression;

        protected virtual bool IsSupportFiltering => false;

        protected virtual bool IsSupportSorting => false;

        protected virtual bool IsSupportGrouping => false;

        public override object Resolve(IResolveFieldContext context)
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
            if (!IsSupportSorting)
            {
                throw new NotSupportedException($".Sortable() call is not supported for field of type {typeof(TReturnType).Name}.");
            }

            Parent.AddSorter(_expression);
        }

        public void Sortable<TValue>(Expression<Func<TEntity, TValue>> sorter)
        {
            if (!IsSupportSorting)
            {
                throw new NotSupportedException($".Sortable() call is not supported for field of type {typeof(TReturnType).Name}.");
            }

            Parent.AddSorter(Name, sorter);
        }

        public void Groupable()
        {
            if (!IsSupportGrouping)
            {
                throw new NotSupportedException($".Groupable() call is not supported for field of type {typeof(TReturnType).Name}.");
            }

            IsGroupable = true;
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

        public override ResolvedField<TEntity, TReturnType1, TExecutionContext> ApplyResolve<TReturnType1>(Func<TExecutionContext, TEntity, TReturnType1> resolve, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            throw new NotSupportedException();
        }

        public override ResolvedField<TEntity, TReturnType1, TExecutionContext> ApplyResolve<TReturnType1>(Func<TExecutionContext, TEntity, Task<TReturnType1>> resolve, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            throw new NotSupportedException();
        }

        public override ResolvedField<TEntity, TReturnType1, TExecutionContext> ApplyResolve<TReturnType1>(Func<TExecutionContext, TEntity, TReturnType1> resolve, Action<IInlineObjectBuilder<TReturnType1, TExecutionContext>> build, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType1 : class
        {
            throw new NotSupportedException();
        }

        public override ResolvedField<TEntity, TReturnType1, TExecutionContext> ApplyResolve<TReturnType1>(Func<TExecutionContext, TEntity, Task<TReturnType1>> resolve, Action<IInlineObjectBuilder<TReturnType1, TExecutionContext>> build, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType1 : class
        {
            throw new NotSupportedException();
        }

        public override ResolvedField<TEntity, IEnumerable<TReturnType1>, TExecutionContext> ApplyResolve<TReturnType1>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType1>> resolve, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            throw new NotSupportedException();
        }

        public override ResolvedField<TEntity, IEnumerable<TReturnType1>, TExecutionContext> ApplyResolve<TReturnType1>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType1>>> resolve, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            throw new NotSupportedException();
        }

        public override ResolvedField<TEntity, IEnumerable<TReturnType1>, TExecutionContext> ApplyResolve<TReturnType1>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType1>> resolve, Action<IInlineObjectBuilder<TReturnType1, TExecutionContext>> build, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType1 : class
        {
            throw new NotSupportedException();
        }

        public override ResolvedField<TEntity, IEnumerable<TReturnType1>, TExecutionContext> ApplyResolve<TReturnType1>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType1>>> resolve, Action<IInlineObjectBuilder<TReturnType1, TExecutionContext>> build, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType1 : class
        {
            throw new NotSupportedException();
        }

        protected override IFieldResolver GetResolver() => _expression;

        private static string GenerateName(string name, Expression<Func<TEntity, TReturnType>> expression)
        {
            if (expression == null)
            {
                throw new ArgumentNullException(nameof(expression));
            }

            if (name == null && !expression.IsProperty())
            {
                throw new InvalidOperationException($"Expression ({expression}), provided for field is not a property. Consider to give a name to field explicitly.");
            }

            return name ?? expression.NameOf().ToCamelCase();
        }
    }

    internal class ExpressionField<TEntity, TReturnType, TFilterValueType, TExecutionContext> : ExpressionField<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        public ExpressionField(RelationRegistry<TExecutionContext> registry, BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent, Expression<Func<TEntity, TReturnType>> expression, string name)
            : base(registry, parent, expression, name)
        {
        }

        public ExpressionField(RelationRegistry<TExecutionContext> registry, BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent, Expression<Func<TExecutionContext, TEntity, TReturnType>> expression, string name)
            : base(registry, parent, expression, name)
        {
        }

        protected override bool IsSupportFiltering => true;

        protected TFilterValueType[] DefaultValues { get; private set; }

        protected NullOption? NullValue { get; private set; }

        public void Filterable(TFilterValueType[] defaultValues = null)
        {
            if (defaultValues != null && defaultValues.Any(value => value == null))
            {
                throw new ArgumentException(".Filterable() does not support nulls as parameters. Consider to use .Filterable(NullValues).");
            }

            if (!IsSupportFiltering)
            {
                throw new NotSupportedException($".Filterable() call is not supported for field of type {typeof(TReturnType).Name}.");
            }

            DefaultValues = defaultValues;
            IsFilterable = true;
        }

        public void Filterable(NullOption nullValue)
        {
            if (!IsSupportFiltering)
            {
                throw new NotSupportedException($".Filterable() call is not supported for field of type {typeof(TReturnType).Name}.");
            }

            NullValue = nullValue;
            IsFilterable = true;
        }

        public override IInlineFilter<TExecutionContext> CreateInlineFilter()
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
    }
}
