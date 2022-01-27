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
    internal static class UnionQueryField
    {
        public static UnionQueryField<TExecutionContext> Create<TLastElementType, TExecutionContext>(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            return new UnionQueryField<TExecutionContext>(registry, parent, name, typeof(TLastElementType), CreateTypeResolver(build));
        }

        public static Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> CreateTypeResolver<TLastElementType, TExecutionContext>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            return field => field.Parent.GetGraphQLTypeDescriptor(field, build);
        }
    }

    internal class UnionQueryField<TExecutionContext> :
        UnionFieldBase<object, TExecutionContext>,
        IUnionableRootField<TExecutionContext>
    {
        public UnionQueryField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> graphTypeFactory)
            : base(registry, parent, name, unionType, graphTypeFactory)
        {
        }

        private UnionQueryField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> graphTypeFactory,
            List<Type> unionTypes,
            UnionGraphTypeDescriptor<TExecutionContext> unionGraphType)
            : base(
                registry,
                parent,
                name,
                unionType,
                graphTypeFactory,
                unionTypes,
                unionGraphType)
        {
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TReturnType> resolve)
        {
            var resolver = ResolvedFieldResolverFactory.Create(
                Resolvers.ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                GraphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<TReturnType>> resolve)
        {
            var resolver = ResolvedFieldResolverFactory.Create(
                Resolvers.ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                GraphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            var resolver = ResolvedFieldResolverFactory.Create(
                Resolvers.ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                GraphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            var resolver = ResolvedFieldResolverFactory.Create(
                Resolvers.ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                GraphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve)
        {
            var resolver = ResolvedFieldResolverFactory.Create(
                Resolvers.ConvertFieldResolver(resolve));

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                GraphType.MakeListDescriptor(),
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve)
        {
            var resolver = ResolvedFieldResolverFactory.Create(
                Resolvers.ConvertFieldResolver(resolve));

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                GraphType.MakeListDescriptor(),
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            var resolver = ResolvedFieldResolverFactory.Create(
                Resolvers.ConvertFieldResolver(resolve));

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                GraphType.MakeListDescriptor(),
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            var resolver = ResolvedFieldResolverFactory.Create(
                Resolvers.ConvertFieldResolver(resolve));

            Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                this,
                GraphType.MakeListDescriptor(),
                resolver);
        }

        public IUnionableRootField<TExecutionContext> AsUnionOf<TLastElementType2>(Action<IInlineObjectBuilder<TLastElementType2, TExecutionContext>>? build)
            where TLastElementType2 : class
        {
            var unionField = new UnionQueryField<TExecutionContext>(Registry, Parent, Name, typeof(TLastElementType2), UnionMutationField.CreateTypeResolver(build), UnionTypes, UnionGraphType);
            return ApplyField(unionField);
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
    }
}
