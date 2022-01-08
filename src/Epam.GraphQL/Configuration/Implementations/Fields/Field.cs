// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields;

#nullable enable

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal class Field<TEntity, TExecutionContext> : FieldBase<TEntity, TExecutionContext>, IFieldSupportsApplyResolve<TEntity, TExecutionContext>
        where TEntity : class
    {
        public Field(RelationRegistry<TExecutionContext> registry, BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent, string name)
            : base(registry, parent, name)
        {
        }

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
    }
}
