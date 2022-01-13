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
using Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields.Helpers;
using Epam.GraphQL.Helpers;

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal static class UnionField
    {
        public static UnionField<TEntity, TExecutionContext> Create<TEntity, TLastElementType, TExecutionContext>(RelationRegistry<TExecutionContext> registry, BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent, string name, Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build, bool isList)
            where TEntity : class
            where TLastElementType : class
        {
            return new UnionField<TEntity, TExecutionContext>(registry, parent, name, typeof(TLastElementType), CreateTypeResolver<TEntity, TLastElementType, TExecutionContext>(build), isList);
        }

        public static Func<UnionField<TEntity, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> CreateTypeResolver<TEntity, TLastElementType, TExecutionContext>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEntity : class
            where TLastElementType : class
        {
            return field => field.Parent.GetGraphQLTypeDescriptor(field, build);
        }
    }

    internal class UnionField<TEntity, TExecutionContext> :
        FieldBase<TEntity, TExecutionContext>,
        IUnionableField<TEntity, TExecutionContext>
        where TEntity : class
    {
        public UnionField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionField<TEntity, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> graphTypeFactory,
            bool isList)
            : this(registry, parent, name, unionType, graphTypeFactory, new List<Type>(), new UnionGraphTypeDescriptor<TExecutionContext>(), isList)
        {
        }

        public UnionField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionField<TEntity, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> graphTypeFactory,
            List<Type> unionTypes,
            UnionGraphTypeDescriptor<TExecutionContext> unionGraphType,
            bool isList)
            : this(
                registry,
                parent,
                name,
                new List<Type>(unionTypes)
                {
                    unionType,
                },
                unionGraphType,
                isList)
        {
            UnionGraphType.Add(graphTypeFactory(this));
        }

        protected UnionField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            List<Type> unionTypes,
            UnionGraphTypeDescriptor<TExecutionContext> unionGraphType,
            bool isList)
            : base(
                  registry,
                  parent,
                  name)
        {
            UnionGraphType = unionGraphType;
            IsList = isList;
            UnionTypes = unionTypes;
            UnionGraphType.Name = parent.GetGraphQLTypeName(TypeHelpers.GetTheBestCommonBaseType(unionTypes), null, this);
        }

        public override IGraphTypeDescriptor<TExecutionContext> GraphType => IsList ? UnionGraphType.MakeListDescriptor() : UnionGraphType;

        public override Type FieldType => IsList ? typeof(IEnumerable<>).MakeGenericType(TypeHelpers.GetTheBestCommonBaseType(UnionTypes)) : TypeHelpers.GetTheBestCommonBaseType(UnionTypes);

        public UnionGraphTypeDescriptor<TExecutionContext> UnionGraphType { get; }

        protected bool IsList { get; }

        protected List<Type> UnionTypes { get; }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            ResolvableTypedFieldHelpers.ApplyResolve(this, Resolvers.ConvertFieldResolver(resolve), optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            ResolvableTypedFieldHelpers.ApplyResolve(this, Resolvers.ConvertFieldResolver(resolve), optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            ResolvableTypedFieldHelpers.ApplyResolve(this, Resolvers.ConvertFieldResolver(resolve), build, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            ResolvableTypedFieldHelpers.ApplyResolve(this, Resolvers.ConvertFieldResolver(resolve), build, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            ResolvableTypedFieldHelpers.ApplyResolve(this, Resolvers.ConvertFieldResolver(resolve), optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            ResolvableTypedFieldHelpers.ApplyResolve(this, Resolvers.ConvertFieldResolver(resolve), optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            ResolvableTypedFieldHelpers.ApplyResolve(this, Resolvers.ConvertFieldResolver(resolve), build, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TEntity, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            ResolvableTypedFieldHelpers.ApplyResolve(this, Resolvers.ConvertFieldResolver(resolve), build, optionsBuilder);
        }

        public IUnionableField<TEntity, TExecutionContext> AsUnionOf<TLastElementType2>(Action<IInlineObjectBuilder<TLastElementType2, TExecutionContext>>? build)
            where TLastElementType2 : class
        {
            var unionField = new UnionField<TEntity, TExecutionContext>(Registry, Parent, Name, typeof(TLastElementType2), UnionField.CreateTypeResolver<TEntity, TLastElementType2, TExecutionContext>(build), UnionTypes, UnionGraphType, IsList);
            return ApplyField(unionField);
        }

        public IUnionableField<TEntity, TExecutionContext> AsUnionOf<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TElementType>
            where TElementType : class
        {
            var unionField = new UnionField<TEntity, TExecutionContext>(Registry, Parent, Name, typeof(TElementType), UnionField.CreateTypeResolver<TEntity, TElementType, TExecutionContext>(build), UnionTypes, UnionGraphType, true);
            return ApplyField(unionField);
        }
    }
}
