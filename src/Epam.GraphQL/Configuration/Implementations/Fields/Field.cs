// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Filters;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.TaskBatcher;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;

#nullable enable

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class Field<TEntity, TExecutionContext> : IField<TEntity, TExecutionContext>, IArgumentCollection
        where TEntity : class
    {
        public Field(RelationRegistry<TExecutionContext> registry, BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent, string name)
        {
            Registry = registry ?? throw new ArgumentNullException(nameof(registry));
            Name = name.ToCamelCase();
            Parent = parent ?? throw new ArgumentNullException(nameof(parent));
        }

        public LazyQueryArguments? Arguments { get; set; }

        public virtual IGraphTypeDescriptor<TExecutionContext> GraphType => throw new NotImplementedException();

        public string Name { get; set; }

        public string? DeprecationReason { get; set; }

        public virtual PropertyInfo? PropertyInfo => null;

        public Type? FieldType { get; protected set; }

        public virtual bool IsExpression => false;

        public virtual bool IsFilterable { get => false; protected set => throw new NotSupportedException(); }

        public virtual bool IsGroupable { get => false; protected set => throw new NotSupportedException(); }

        public virtual bool CanResolve => false;

        public virtual LambdaExpression? ContextExpression => null;

        public virtual LambdaExpression? OriginalExpression => null;

        public IFieldEditSettings<TEntity, TExecutionContext>? EditSettings { get; protected set; }

        IFieldEditSettings<TExecutionContext>? IField<TExecutionContext>.EditSettings => EditSettings;

        internal RelationRegistry<TExecutionContext> Registry { get; }

        internal BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> Parent { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{GetType().GetGenericTypeDefinition().Name}({FieldType?.Name} {typeof(TEntity).Name}.{Name})";

        public FieldType AsFieldType()
        {
            var resolver = GetResolver();

            GraphType.Validate();

            var fieldType = new FieldType
            {
                Type = GraphType.Type,
                Arguments = Arguments?.ToQueryArguments(),
                Resolver = resolver,
                Name = Name,
                ResolvedType = GraphType.GraphType,
                DeprecationReason = DeprecationReason,
            };

            if (PropertyInfo != null)
            {
                // Hack against GraphQL.NET: ComplexGraphType<object>.ORIGINAL_EXPRESSION_PROPERTY_NAME is internal.
                fieldType.Metadata.Add("ORIGINAL_EXPRESSION_PROPERTY_NAME", PropertyInfo.Name);
            }

            return fieldType;
        }

        public void Argument<TArgumentType>(string name, string? description = null) => Argument(name, typeof(TArgumentType), description);

        public void Argument(string name, Type type, string? description = null)
        {
            Argument(name, () => new QueryArgument(Registry.GenerateInputGraphType(type))
            {
                Name = name,
                Description = description,
            });
        }

        public void Argument(string name, IGraphType graphType, string? description = null)
        {
            Argument(name, () => new QueryArgument(graphType)
            {
                Name = name,
                Description = description,
            });
        }

        public void Argument(string name, Func<QueryArgument> factory)
        {
            if (Arguments == null)
            {
                Arguments = new LazyQueryArguments();
            }

            var index = Arguments.TakeWhile(a => a.Name != name).Count();
            if (index == Arguments.Count)
            {
                Arguments.Add(new LazyQueryArgument(name, factory));
            }
            else
            {
                Arguments[index].Factory = factory;
            }
        }

        public virtual object? Resolve(IResolveFieldContext context) => throw new NotSupportedException();

        public IDataLoader<IFieldChange<TEntity, TExecutionContext>, (bool CanEdit, string DisableReason)> CanEdit(IResolveFieldContext context)
        {
            if (EditSettings != null)
            {
                if (EditSettings.IsReadOnly)
                {
                    return BatchLoader.FromResult<IFieldChange<TEntity, TExecutionContext>, (bool CanEdit, string DisableReason)>(change => (false, "The field is read only."));
                }

                if (EditSettings.CanEdit != null)
                {
                    return EditSettings.CanEdit(context);
                }
            }

            return BatchLoader.FromResult<IFieldChange<TEntity, TExecutionContext>, (bool CanEdit, string DisableReason)>(change => (false, "The field is not editable. Consider to use `Editable()` or `EditableIf(...)` methods."));
        }

        public virtual void ValidateField()
        {
            if (string.IsNullOrEmpty(Name))
            {
                throw new InvalidOperationException("Field name cannot be null or empty.");
            }

            if (GetResolver() == null)
            {
                throw new InvalidOperationException($"Field `{Name}` must have resolver.");
            }
        }

        public virtual UnionField<TEntity, TExecutionContext> ApplyUnion<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>> build, bool isList)
            where TLastElementType : class
            => Parent.ApplyUnion(this, build, isList);

        public virtual ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            => Parent.ApplyResolve(this, resolve, doesDependOnAllFields, optionsBuilder);

        public virtual ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            => Parent.ApplyResolve(this, resolve, doesDependOnAllFields, optionsBuilder);

        public virtual ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class
            => Parent.ApplyResolve(this, resolve, build, doesDependOnAllFields, optionsBuilder);

        public virtual ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class
            => Parent.ApplyResolve(this, resolve, build, doesDependOnAllFields, optionsBuilder);

        public virtual ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            => Parent.ApplyResolve(this, resolve, doesDependOnAllFields, optionsBuilder);

        public virtual ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            => Parent.ApplyResolve(this, resolve, doesDependOnAllFields, optionsBuilder);

        public virtual ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class
            => Parent.ApplyResolve(this, resolve, build, doesDependOnAllFields, optionsBuilder);

        public virtual ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, bool doesDependOnAllFields, Action<ResolveOptionsBuilder> optionsBuilder)
            where TReturnType : class
            => Parent.ApplyResolve(this, resolve, build, doesDependOnAllFields, optionsBuilder);

        public virtual ArgumentedField<TEntity, TArgType, TExecutionContext> ApplyArgument<TArgType>(string argName)
            => Parent.ApplyArgument<TArgType>(this, argName);

        public ArgumentedField<TEntity, Expression<Func<TEntity1, bool>>, TExecutionContext> ApplyFilterArgument<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
            => Parent.ApplyFilterArgument<TProjection, TEntity1>(this, argName);

        public virtual ArgumentedField<TEntity, TArgType, TExecutionContext> ApplyPayloadField<TArgType>(string argName)
            => Parent.ApplyPayloadField<TArgType>(this, argName);

        public ArgumentedField<TEntity, Expression<Func<TEntity1, bool>>, TExecutionContext> ApplyFilterPayloadField<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
            => Parent.ApplyFilterPayloadField<TProjection, TEntity1>(this, argName);

        public TField ApplyField<TField>(TField field)
            where TField : Field<TEntity, TExecutionContext>
        {
            return Parent.ReplaceField(this, field);
        }

        public override int GetHashCode()
        {
            var hash = default(HashCode);
            hash.Add(Name);
            hash.Add(typeof(TEntity));
            hash.Add(typeof(TExecutionContext));

            return hash.ToHashCode();
        }

        public string GetGraphQLTypePrefix()
        {
            return $"{Parent.GetGraphQLTypePrefix()}{Name.CapitalizeFirstLetter()}";
        }

        public override bool Equals(object other)
        {
            if (other == null)
            {
                return false;
            }

            if (other is Field<TEntity, TExecutionContext> field)
            {
                return field.Name == Name; // TODO check for actual field type equality
            }

            return false;
        }

        public virtual IInlineFilter<TExecutionContext> CreateInlineFilter()
        {
            throw new NotSupportedException();
        }

        // TODO make it property
        protected virtual IFieldResolver? GetResolver()
        {
            return null;
        }
    }
}
