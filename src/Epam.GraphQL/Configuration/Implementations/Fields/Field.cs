// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Configuration.Implementations.Fields.Helpers;
using Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields;
using Epam.GraphQL.Diagnostics;

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal class Field<TEntity, TExecutionContext> :
        FieldBase<TEntity, TExecutionContext>,
        IUnionableField<TEntity, TExecutionContext>
        where TEntity : class
    {
        public Field(MethodCallConfigurationContext configurationContext, BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent, string name)
            : base(configurationContext, parent, name)
        {
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve)
        {
            var graphType = Parent.GetGraphQLTypeDescriptor<TReturnType>(this);
            Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve)).Argument(resolve),
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(Name, resolve, Parent.ProxyAccessor)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve)
        {
            var graphType = Parent.GetGraphQLTypeDescriptor<TReturnType>(this);
            Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve)).Argument(resolve),
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(Name, resolve, Parent.ProxyAccessor)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var configurationContext = ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .Argument(build);

            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext);

            Parent.ApplyResolvedField<TReturnType>(
                configurationContext.Parent,
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(Name, resolve, Parent.ProxyAccessor)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var configurationContext = ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .Argument(build);

            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext);

            Parent.ApplyResolvedField<TReturnType>(
                configurationContext.Parent,
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(Name, resolve, Parent.ProxyAccessor)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve)
        {
            var graphType = Parent.GetGraphQLTypeDescriptor<TReturnType>(this).MakeListDescriptor();
            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve)).Argument(resolve),
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(Name, resolve, Parent.ProxyAccessor)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve)
        {
            var graphType = Parent.GetGraphQLTypeDescriptor<TReturnType>(this).MakeListDescriptor();
            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve)).Argument(resolve),
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(Name, resolve, Parent.ProxyAccessor)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var configurationContext = ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .Argument(build);

            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext).MakeListDescriptor();

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                configurationContext.Parent,
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(Name, resolve, Parent.ProxyAccessor)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var configurationContext = ConfigurationContext.NextOperation<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .Argument(build);

            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext).MakeListDescriptor();

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                configurationContext.Parent,
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(Name, resolve, Parent.ProxyAccessor)));
        }

        public IUnionableField<TEntity, TExecutionContext> AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            return And(build);
        }

        public IUnionableField<TEntity, TExecutionContext> And<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            var unionField = UnionField.Create(
                ConfigurationContext.NextOperation<TLastElementType>(nameof(And))
                    .OptionalArgument(build),
                Parent,
                Name,
                build);
            return Parent.ReplaceField(this, unionField);
        }

        public IUnionableField<TEntity, TExecutionContext> AsUnionOf<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class
        {
            return And<TEnumerable, TLastElementType>(build);
        }

        public IUnionableField<TEntity, TExecutionContext> And<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class
        {
            return And(build);
        }
    }
}
