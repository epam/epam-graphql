// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;
using Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields;
using GraphQL;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal class SelectField<TEntity, TReturnType, TExecutionContext> : TypedField<TEntity, TReturnType, TExecutionContext>,
        IFieldSupportsEditSettings<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        private readonly IGraphTypeDescriptor<TExecutionContext> _graphType;
        private readonly IResolver<TEntity> _resolver;

        public SelectField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IResolver<TEntity> resolver,
            IGraphTypeDescriptor<TExecutionContext> graphType,
            LazyQueryArguments arguments)
            : base(registry, parent, name)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            EditSettings = new FieldEditSettings<TEntity, TReturnType, TExecutionContext>();
            _graphType = graphType;
            Arguments = arguments;
        }

        public override bool CanResolve => true;

        public override IGraphTypeDescriptor<TExecutionContext> GraphType
        {
            get
            {
                var graphType = _graphType;

                if (Parent is InputObjectGraphTypeConfigurator<TEntity, TExecutionContext> && !EditSettings.IsMandatoryForUpdate)
                {
                    graphType = graphType.UnwrapIfNonNullable();
                }

                return graphType;
            }
        }

        IFieldEditSettings<TEntity, TReturnType, TExecutionContext> IFieldSupportsEditSettings<TEntity, TReturnType, TExecutionContext>.EditSettings => EditSettings;

        public override object Resolve(IResolveFieldContext context)
        {
            return _resolver.Resolve(context);
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

        protected override IFieldResolver GetResolver()
        {
            return _resolver;
        }
    }
}
