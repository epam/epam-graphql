// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Configuration.Implementations.Fields.ChildFields;
using Epam.GraphQL.Configuration.Implementations.Fields.Helpers;
using Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Sorters.Implementations;

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal class QueryField<TExecutionContext> :
        FieldBase<object, TExecutionContext>,
        IQueryField<TExecutionContext>
    {
        public QueryField(Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory, BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent, string name)
            : base(configurationContextFactory, parent, name)
        {
        }

        public IRootQueryableField<TReturnType, TExecutionContext> FromIQueryable<TReturnType>(
            Func<TExecutionContext, IQueryable<TReturnType>> query)
        {
            var graphType = Parent.GetGraphQLTypeDescriptor<TReturnType>(this);
            var result = new RootQueryableField<TReturnType, TExecutionContext>(
                ConfigurationContext.Chain<TReturnType>(nameof(FromIQueryable)).Argument(query),
                Parent,
                Name,
                query,
                graphType,
                searcher: null,
                naturalSorters: SortingHelpers.Empty);

            return Parent.ReplaceField(this, result);
        }

        public IRootQueryableField<TReturnType, TExecutionContext> FromIQueryable<TReturnType>(
            Func<TExecutionContext, IQueryable<TReturnType>> query,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? configure)
            where TReturnType : class
        {
            var configurationContext = ConfigurationContext.Chain<TReturnType>(nameof(FromIQueryable))
                .Argument(query)
                .OptionalArgument(configure);

            var graphType = Parent.GetGraphQLTypeDescriptor(this, configure, configurationContext);
            var result = new RootQueryableField<TReturnType, TExecutionContext>(
                configurationContext,
                Parent,
                Name,
                query,
                graphType,
                searcher: null,
                naturalSorters: SortingHelpers.Empty);

            return Parent.ReplaceField(this, result);
        }

        public IRootLoaderField<TChildEntity, TExecutionContext> FromLoader<TChildLoader, TChildEntity>()
            where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
            where TChildEntity : class
        {
            var graphResultType = Registry.GetGraphTypeDescriptor<TChildLoader, TChildEntity>();
            return Parent.ReplaceField(this, new RootLoaderField<TChildLoader, TChildEntity, TExecutionContext>(
                ConfigurationContext.Chain<TChildLoader, TChildEntity>(nameof(FromLoader)),
                Parent,
                Name,
                graphResultType,
                arguments: null,
                searcher: null,
                naturalSorters: SortingHelpers.Empty));
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TReturnType> resolve)
        {
            var graphType = Parent.GetGraphQLTypeDescriptor<TReturnType>(this);
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve)).Argument(resolve),
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(resolve)));
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, Task<TReturnType>> resolve)
        {
            var graphType = Parent.GetGraphQLTypeDescriptor<TReturnType>(this);
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve)).Argument(resolve),
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(resolve)));
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var configurationContext = ConfigurationContext
                .Chain<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .OptionalArgument(build);

            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext);

            return Parent.ApplyResolvedField<TReturnType>(
                configurationContext,
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(resolve)));
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var configurationContext = ConfigurationContext
                .Chain<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .OptionalArgument(build);

            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext);

            return Parent.ApplyResolvedField<TReturnType>(
                configurationContext,
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(resolve)));
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve)
        {
            var graphType = Parent.GetGraphQLTypeDescriptor<TReturnType>(this).MakeListDescriptor();
            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve)).Argument(resolve),
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(resolve)));
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve)
        {
            var graphType = Parent.GetGraphQLTypeDescriptor<TReturnType>(this).MakeListDescriptor();
            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve)).Argument(resolve),
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(resolve)));
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var configurationContext = ConfigurationContext
                .Chain<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .OptionalArgument(build);

            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext).MakeListDescriptor();

            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                configurationContext,
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(resolve)));
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var configurationContext = ConfigurationContext
                .Chain<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .OptionalArgument(build);

            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext).MakeListDescriptor();

            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                configurationContext,
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(resolve)));
        }

        public IUnionableRootField<TExecutionContext> AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            return And(build);
        }

        public IUnionableRootField<TExecutionContext> And<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            var unionField = UnionQueryField.Create(
                ConfigurationContext
                    .Chain<TLastElementType>(nameof(And))
                    .OptionalArgument(build),
                Parent,
                Name,
                build);
            return Parent.ReplaceField(this, unionField);
        }

        public IUnionableRootField<TExecutionContext> AsUnionOf<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class
        {
            return And<TEnumerable, TLastElementType>(build);
        }

        public IUnionableRootField<TExecutionContext> And<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class
        {
            return And(build);
        }

        public IArgumentedQueryField<TArgType, TExecutionContext> Argument<TArgType>(string argName)
        {
            var argumentedField = new ArgumentedQueryField<TArgType, TExecutionContext>(
                ConfigurationContext.Chain<TArgType>(nameof(Argument)).Argument(argName),
                Parent,
                Name,
                new Arguments<TArgType, TExecutionContext>(Registry, argName));
            return Parent.ReplaceField(this, argumentedField);
        }

        public IArgumentedQueryField<Expression<Func<TEntity, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity>(string argName)
            where TProjection : Projection<TEntity, TExecutionContext>
            where TEntity : class
        {
            var argumentedField = new ArgumentedQueryField<Expression<Func<TEntity, bool>>, TExecutionContext>(
                ConfigurationContext.Chain<TProjection, TEntity>(nameof(FilterArgument)).Argument(argName),
                Parent,
                Name,
                new Arguments<Expression<Func<TEntity, bool>>, TExecutionContext>(Registry, argName, typeof(TProjection), typeof(TEntity)));
            return Parent.ReplaceField(this, argumentedField);
        }

        public IPayloadFieldedQueryField<TArgType, TExecutionContext> PayloadField<TArgType>(string argName)
        {
            var payloadedField = new ArgumentedQueryField<TArgType, TExecutionContext>(
                ConfigurationContext.Chain<TArgType>(nameof(PayloadField)).Argument(argName),
                Parent,
                Name,
                new PayloadFields<TArgType, TExecutionContext>(Name, Registry, argName));
            return Parent.ReplaceField(this, payloadedField);
        }

        public IPayloadFieldedQueryField<Expression<Func<TEntity, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity>(string argName)
            where TProjection : Projection<TEntity, TExecutionContext>
            where TEntity : class
        {
            var argumentedField = new ArgumentedQueryField<Expression<Func<TEntity, bool>>, TExecutionContext>(
                ConfigurationContext.Chain<TProjection, TEntity>(nameof(FilterPayloadField)).Argument(argName),
                Parent,
                Name,
                new PayloadFields<Expression<Func<TEntity, bool>>, TExecutionContext>(Name, Registry, argName, typeof(TProjection), typeof(TEntity)));
            return Parent.ReplaceField(this, argumentedField);
        }
    }
}
