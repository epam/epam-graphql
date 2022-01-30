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

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal static class UnionField
    {
        public static UnionField<TEntity, TExecutionContext> Create<TEntity, TLastElementType, TExecutionContext>(
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEntity : class
            where TLastElementType : class
        {
            return new UnionField<TEntity, TExecutionContext>(parent, name, typeof(TLastElementType), CreateTypeResolver<TEntity, TLastElementType, TExecutionContext>(build));
        }

        public static Func<UnionFieldBase<TEntity, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> CreateTypeResolver<TEntity, TLastElementType, TExecutionContext>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEntity : class
            where TLastElementType : class
        {
            return field => field.Parent.GetGraphQLTypeDescriptor(field, build);
        }
    }

    internal class UnionField<TEntity, TExecutionContext> :
        UnionFieldBase<TEntity, TExecutionContext>,
        IUnionableField<TEntity, TExecutionContext>
        where TEntity : class
    {
        public UnionField(
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<TEntity, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> graphTypeFactory)
            : base(parent, name, unionType, graphTypeFactory)
        {
        }

        private UnionField(
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<TEntity, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> graphTypeFactory,
            List<Type> unionTypes,
            UnionGraphTypeDescriptor<TExecutionContext> unionGraphType)
            : base(
                parent,
                name,
                unionType,
                graphTypeFactory,
                unionTypes,
                unionGraphType)
        {
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve)
        {
            Parent.ApplyResolvedField<TReturnType>(
                this,
                GraphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(Name, resolve, Parent.ProxyAccessor)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve)
        {
            Parent.ApplyResolvedField<TReturnType>(
                this,
                GraphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(Name, resolve, Parent.ProxyAccessor)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            Parent.ApplyResolvedField<TReturnType>(
                this,
                GraphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(Name, resolve, Parent.ProxyAccessor)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            Parent.ApplyResolvedField<TReturnType>(
                this,
                GraphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(Name, resolve, Parent.ProxyAccessor)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve)
        {
            var graphType = GraphType.MakeListDescriptor();
            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(Name, resolve, Parent.ProxyAccessor)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve)
        {
            var graphType = GraphType.MakeListDescriptor();
            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(Name, resolve, Parent.ProxyAccessor)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            var graphType = GraphType.MakeListDescriptor();
            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(Name, resolve, Parent.ProxyAccessor)));
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            var graphType = GraphType.MakeListDescriptor();
            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                graphType,
                ResolvedFieldResolverFactory.Create(Resolvers.ConvertFieldResolver(Name, resolve, Parent.ProxyAccessor)));
        }

        public IUnionableField<TEntity, TExecutionContext> AsUnionOf<TLastElementType2>(Action<IInlineObjectBuilder<TLastElementType2, TExecutionContext>>? build)
            where TLastElementType2 : class
        {
            var unionField = new UnionField<TEntity, TExecutionContext>(Parent, Name, typeof(TLastElementType2), UnionField.CreateTypeResolver<TEntity, TLastElementType2, TExecutionContext>(build), UnionTypes, UnionGraphType);
            return ApplyField(unionField);
        }

        public IUnionableField<TEntity, TExecutionContext> And<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            return AsUnionOf(build);
        }

        public IUnionableField<TEntity, TExecutionContext> AsUnionOf<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class
        {
            return AsUnionOf(build);
        }

        public IUnionableField<TEntity, TExecutionContext> And<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class
        {
            return AsUnionOf(build);
        }
    }
}
