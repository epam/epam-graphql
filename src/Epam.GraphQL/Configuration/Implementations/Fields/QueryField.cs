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
using Epam.GraphQL.Builders.Loader.Implementations;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Configuration.Implementations.Fields.ChildFields;
using Epam.GraphQL.Configuration.Implementations.Fields.Helpers;
using Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Sorters.Implementations;

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal class QueryField<TExecutionContext> :
        FieldBase<object, TExecutionContext>,
        IQueryField<TExecutionContext>
    {
        private static MethodInfo? _fromLoaderImplMethodInfo;

        public QueryField(BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent, string name)
            : base(parent, name)
        {
        }

        public IFromIQueryableBuilder<TReturnType, TExecutionContext> FromIQueryable<TReturnType>(
            Func<TExecutionContext, IQueryable<TReturnType>> query,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? configure)
            where TReturnType : class
        {
            return new FromIQueryableBuilder<object, TReturnType, TExecutionContext>(Parent.FromIQueryableClass(this, query, null, configure));
        }

        public ILoaderField<TChildEntity, TExecutionContext> FromLoader<TChildLoader, TChildEntity>()
            where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
            where TChildEntity : class
        {
            var graphResultType = Parent.GetGraphQLTypeDescriptor<TChildLoader, TChildEntity>();
            return Parent.ReplaceField(this, new LoaderField<object, TChildLoader, TChildEntity, TExecutionContext>(
                Parent,
                Name,
                condition: null,
                graphResultType,
                arguments: null,
                searcher: null,
                naturalSorters: SortingHelpers.Empty));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TReturnType> resolve)
        {
            var graphType = Parent.GetGraphQLTypeDescriptor<TReturnType>(this);
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(resolve)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<TReturnType>> resolve)
        {
            var graphType = Parent.GetGraphQLTypeDescriptor<TReturnType>(this);
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(resolve)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build);
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(resolve)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build);
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(resolve)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve)
        {
            var graphType = Parent.GetGraphQLTypeDescriptor<TReturnType>(this).MakeListDescriptor();
            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(resolve)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve)
        {
            var graphType = Parent.GetGraphQLTypeDescriptor<TReturnType>(this).MakeListDescriptor();
            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(resolve)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build).MakeListDescriptor();
            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(resolve)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build).MakeListDescriptor();
            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(resolve)));
        }

        public IUnionableRootField<TExecutionContext> AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            var unionField = UnionQueryField.Create(Parent, Name, build);
            return Parent.ReplaceField(this, unionField);
        }

        public IUnionableRootField<TExecutionContext> And<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            return AsUnionOf(build);
        }

        public IUnionableRootField<TExecutionContext> AsUnionOf<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class
        {
            return AsUnionOf(build);
        }

        public IUnionableRootField<TExecutionContext> And<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class
        {
            return AsUnionOf(build);
        }

        public IArgumentedQueryField<TArgType, TExecutionContext> Argument<TArgType>(string argName)
        {
            var argumentedField = new ArgumentedQueryField<TArgType, TExecutionContext>(Parent, Name, new Arguments<TArgType, TExecutionContext>(Registry, argName));
            return Parent.ReplaceField(this, argumentedField);
        }

        public IArgumentedQueryField<Expression<Func<TEntity, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity>(string argName)
            where TProjection : Projection<TEntity, TExecutionContext>
            where TEntity : class
        {
            var argumentedField = new ArgumentedQueryField<Expression<Func<TEntity, bool>>, TExecutionContext>(Parent, Name, new Arguments<Expression<Func<TEntity, bool>>, TExecutionContext>(Registry, argName, typeof(TProjection), typeof(TEntity)));
            return Parent.ReplaceField(this, argumentedField);
        }

        public IPayloadFieldedQueryField<TArgType, TExecutionContext> PayloadField<TArgType>(string argName)
        {
            var payloadedField = new ArgumentedQueryField<TArgType, TExecutionContext>(Parent, Name, new PayloadFields<TArgType, TExecutionContext>(Name, Registry, argName));
            return Parent.ReplaceField(this, payloadedField);
        }

        public IPayloadFieldedQueryField<Expression<Func<TEntity, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity>(string argName)
            where TProjection : Projection<TEntity, TExecutionContext>
            where TEntity : class
        {
            var argumentedField = new ArgumentedQueryField<Expression<Func<TEntity, bool>>, TExecutionContext>(Parent, Name, new PayloadFields<Expression<Func<TEntity, bool>>, TExecutionContext>(Name, Registry, argName, typeof(TProjection), typeof(TEntity)));
            return Parent.ReplaceField(this, argumentedField);
        }

        private static MethodInfo FromLoader(Type loaderType, Type entityType)
        {
            _fromLoaderImplMethodInfo ??= typeof(QueryField<TExecutionContext>).GetNonPublicGenericMethod(
                method => method
                    .HasName(nameof(FromLoader))
                    .HasTwoGenericTypeParameters());

            return _fromLoaderImplMethodInfo.MakeGenericMethod(loaderType, entityType);
        }
    }
}
