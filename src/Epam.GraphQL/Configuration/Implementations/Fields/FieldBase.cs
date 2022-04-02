// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Diagnostics;
using System.Linq;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.TaskBatcher;
using GraphQL;
using GraphQL.DataLoader;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    internal class FieldBase<TEntity, TExecutionContext> : IField<TEntity, TExecutionContext>, IArgumentCollection
        where TEntity : class
    {
        protected FieldBase(FieldConfigurationContext configurationContext, BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent, string name)
        {
            ConfigurationContext = configurationContext;
            Name = name.ToCamelCase();
            Parent = parent;
        }

        public LazyQueryArguments? Arguments { get; set; }

        public FieldConfigurationContext ConfigurationContext { get; }

        public virtual IGraphTypeDescriptor<TExecutionContext> GraphType => GraphTypeDescriptor<TExecutionContext>.NullInstance;

        public string Name { get; set; }

        public string? DeprecationReason { get; set; }

        public virtual IFieldResolver Resolver => throw new InvalidOperationException($"Field `{Name}` must have resolver.");

        public virtual Type FieldType => throw new NotImplementedException();

        public IFieldEditSettings<TEntity, TExecutionContext>? EditSettings { get; protected set; }

        IObjectGraphTypeConfigurator<TExecutionContext> IField<TExecutionContext>.Parent => Parent;

        internal IRegistry<TExecutionContext> Registry => Parent.Registry;

        internal BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> Parent { get; }

        [DebuggerBrowsable(DebuggerBrowsableState.Never)]
        private string DebuggerDisplay => $"{GetType().GetGenericTypeDefinition().Name}({FieldType.Name} {typeof(TEntity).Name}.{Name})";

        public override string ToString()
        {
            return ConfigurationContext.ToString();
        }

        public virtual FieldType AsFieldType()
        {
            var fieldType = new FieldType
            {
                Type = GraphType.Type,
                Arguments = Arguments?.ToQueryArguments(),
                Resolver = Resolver,
                Name = Name,
                ResolvedType = GraphType.GraphType,
                DeprecationReason = DeprecationReason,
            };

            return fieldType;
        }

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

        public virtual void Validate()
        {
            ConfigurationContext.AddErrorIf(string.IsNullOrEmpty(Name), "Field name cannot be null or empty.");

            // Force resolver; FieldBase throws exception if Resolver field is not overriden.
            try
            {
                _ = Resolver;
            }
            catch (InvalidOperationException e)
            {
                ConfigurationContext.AddError(e.Message);
            }

            GraphType.Validate(ConfigurationContext);
        }

        public TField ApplyField<TField>(TField field)
            where TField : FieldBase<TEntity, TExecutionContext>
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

        public override bool Equals(object other)
        {
            if (other == null)
            {
                return false;
            }

            if (other is FieldBase<TEntity, TExecutionContext> field)
            {
                return field.Name == Name; // TODO check for actual field type equality
            }

            return false;
        }
    }
}
