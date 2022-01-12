// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using GraphQL;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields.Helpers
{
    internal static class ResolvableTypedFieldHelpers
    {
        public static ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TEntity, TReturnType, TExecutionContext>(FieldBase<TEntity, TExecutionContext> field, Func<IResolveFieldContext, TReturnType> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TEntity : class
        {
            if (!typeof(TReturnType).IsAssignableFrom(field.FieldType))
            {
                // TODO Throw exception with meaningful message
                throw new NotSupportedException();
            }

            var resolvedField = ResolvedField.Create(field.Registry, field.Parent, field.Name, field.GraphType, resolve, field.Arguments, optionsBuilder);
            return field.ApplyField(resolvedField);
        }

        public static ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TEntity, TReturnType, TExecutionContext>(FieldBase<TEntity, TExecutionContext> field, Func<IResolveFieldContext, Task<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TEntity : class
        {
            if (!typeof(TReturnType).IsAssignableFrom(field.FieldType))
            {
                // TODO Throw exception with meaningful message
                throw new NotSupportedException();
            }

            var resolvedField = ResolvedField.Create(field.Registry, field.Parent, field.Name, field.GraphType, resolve, field.Arguments, optionsBuilder);
            return field.ApplyField(resolvedField);
        }

        public static ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TEntity, TReturnType, TExecutionContext>(FieldBase<TEntity, TExecutionContext> field, Func<IResolveFieldContext, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
            where TEntity : class
        {
            if (!typeof(TReturnType).IsAssignableFrom(field.FieldType))
            {
                // TODO Throw exception with meaningful message
                throw new NotSupportedException();
            }

            var graphType = field.Parent.GetGraphQLTypeDescriptor(field, build);
            var resolvedField = ResolvedField.Create(field.Registry, field.Parent, field.Name, graphType, resolve, field.Arguments, optionsBuilder);
            return field.ApplyField(resolvedField);
        }

        public static ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TEntity, TReturnType, TExecutionContext>(FieldBase<TEntity, TExecutionContext> field, Func<IResolveFieldContext, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
            where TEntity : class
        {
            if (!typeof(TReturnType).IsAssignableFrom(field.FieldType))
            {
                // TODO Throw exception with meaningful message
                throw new NotSupportedException();
            }

            var graphType = field.Parent.GetGraphQLTypeDescriptor(field, build);
            var resolvedField = ResolvedField.Create(field.Registry, field.Parent, field.Name, graphType, resolve, field.Arguments, optionsBuilder);
            return field.ApplyField(resolvedField);
        }

        public static ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TEntity, TReturnType, TExecutionContext>(FieldBase<TEntity, TExecutionContext> field, Func<IResolveFieldContext, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TEntity : class
        {
            if (typeof(IEnumerable<TReturnType>).IsAssignableFrom(field.FieldType))
            {
                var resolvedField = ResolvedField.Create(field.Registry, field.Parent, field.Name, field.GraphType, resolve, field.Arguments, optionsBuilder);
                return field.ApplyField(resolvedField);
            }

            if (typeof(TReturnType).IsAssignableFrom(field.FieldType))
            {
                var graphType = field.GraphType.MakeListDescriptor();
                var resolvedField = ResolvedField.Create(field.Registry, field.Parent, field.Name, graphType, resolve, field.Arguments, optionsBuilder);
                return field.ApplyField(resolvedField);
            }

            // TODO Throw exception with meaningful message
            throw new NotSupportedException();
        }

        public static ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TEntity, TReturnType, TExecutionContext>(FieldBase<TEntity, TExecutionContext> field, Func<IResolveFieldContext, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TEntity : class
        {
            if (typeof(IEnumerable<TReturnType>).IsAssignableFrom(field.FieldType))
            {
                var resolvedField = ResolvedField.Create(field.Registry, field.Parent, field.Name, field.GraphType, resolve, field.Arguments, optionsBuilder);
                return field.ApplyField(resolvedField);
            }

            if (typeof(TReturnType).IsAssignableFrom(field.FieldType))
            {
                var graphType = field.GraphType.MakeListDescriptor();
                var resolvedField = ResolvedField.Create(field.Registry, field.Parent, field.Name, graphType, resolve, field.Arguments, optionsBuilder);
                return field.ApplyField(resolvedField);
            }

            // TODO Throw exception with meaningful message
            throw new NotSupportedException();
        }

        public static ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TEntity, TReturnType, TExecutionContext>(FieldBase<TEntity, TExecutionContext> field, Func<IResolveFieldContext, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
            where TEntity : class
        {
            var graphType = field.Parent.GetGraphQLTypeDescriptor(field, build).MakeListDescriptor();
            var resolvedField = ResolvedField.Create(field.Registry, field.Parent, field.Name, graphType, resolve, field.Arguments, optionsBuilder);
            return field.ApplyField(resolvedField);
        }

        public static ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TEntity, TReturnType, TExecutionContext>(FieldBase<TEntity, TExecutionContext> field, Func<IResolveFieldContext, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
            where TEntity : class
        {
            var graphType = field.Parent.GetGraphQLTypeDescriptor(field, build).MakeListDescriptor();
            var resolvedField = ResolvedField.Create(field.Registry, field.Parent, field.Name, field.GraphType, resolve, field.Arguments, optionsBuilder);
            return field.ApplyField(resolvedField);
        }
    }
}
