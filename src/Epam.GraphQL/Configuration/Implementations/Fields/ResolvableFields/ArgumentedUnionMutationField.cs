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

namespace Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields
{
    internal class ArgumentedUnionMutationField<TArgType, TExecutionContext> :
        ArgumentedUnionFieldBase<IArguments<TArgType, TExecutionContext>, object, TExecutionContext>,
        IUnionableMutationField<TArgType, TExecutionContext>
    {
        public ArgumentedUnionMutationField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> typeResolver,
            IArguments<TArgType, TExecutionContext> arguments)
            : base(registry, parent, name, unionType, typeResolver, arguments)
        {
        }

        public ArgumentedUnionMutationField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> typeResolver,
            List<Type> unionTypes,
            UnionGraphTypeDescriptor<TExecutionContext> unionGraphType,
            IArguments<TArgType, TExecutionContext> arguments)
            : base(registry, parent, name, unionType, typeResolver, unionTypes, unionGraphType, arguments)
        {
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, TReturnType> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public IUnionableMutationField<TArgType, TExecutionContext> AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            var unionField = new ArgumentedUnionMutationField<TArgType, TExecutionContext>(Registry, Parent, Name, typeof(TLastElementType), UnionField.CreateTypeResolver<object, TLastElementType, TExecutionContext>(build), UnionTypes, UnionGraphType, Arguments);
            return ApplyField(unionField);
        }

        private Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType, TReturnType> resolve)
        {
            return Arguments.GetResolver(resolve);
        }
    }

    internal class ArgumentedUnionMutationField<TArgType1, TArgType2, TExecutionContext> :
        ArgumentedUnionFieldBase<IArguments<TArgType1, TArgType2, TExecutionContext>, object, TExecutionContext>,
        IUnionableMutationField<TArgType1, TArgType2, TExecutionContext>
    {
        public ArgumentedUnionMutationField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> typeResolver,
            IArguments<TArgType1, TArgType2, TExecutionContext> arguments)
            : base(registry, parent, name, unionType, typeResolver, arguments)
        {
        }

        public ArgumentedUnionMutationField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> typeResolver,
            List<Type> unionTypes,
            UnionGraphTypeDescriptor<TExecutionContext> unionGraphType,
            IArguments<TArgType1, TArgType2, TExecutionContext> arguments)
            : base(registry, parent, name, unionType, typeResolver, unionTypes, unionGraphType, arguments)
        {
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TReturnType> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public IUnionableMutationField<TArgType1, TArgType2, TExecutionContext> AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            var unionField = new ArgumentedUnionMutationField<TArgType1, TArgType2, TExecutionContext>(Registry, Parent, Name, typeof(TLastElementType), UnionField.CreateTypeResolver<object, TLastElementType, TExecutionContext>(build), UnionTypes, UnionGraphType, Arguments);
            return ApplyField(unionField);
        }

        private Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TReturnType> resolve)
        {
            return Arguments.GetResolver(resolve);
        }
    }

    internal class ArgumentedUnionMutationField<TArgType1, TArgType2, TArgType3, TExecutionContext> :
        ArgumentedUnionFieldBase<IArguments<TArgType1, TArgType2, TArgType3, TExecutionContext>, object, TExecutionContext>,
        IUnionableMutationField<TArgType1, TArgType2, TArgType3, TExecutionContext>
    {
        public ArgumentedUnionMutationField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> typeResolver,
            List<Type> unionTypes,
            UnionGraphTypeDescriptor<TExecutionContext> unionGraphType,
            IArguments<TArgType1, TArgType2, TArgType3, TExecutionContext> arguments)
            : base(registry, parent, name, unionType, typeResolver, unionTypes, unionGraphType, arguments)
        {
        }

        public ArgumentedUnionMutationField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> typeResolver,
            IArguments<TArgType1, TArgType2, TArgType3, TExecutionContext> arguments)
            : base(registry, parent, name, unionType, typeResolver, arguments)
        {
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TReturnType> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public IUnionableMutationField<TArgType1, TArgType2, TArgType3, TExecutionContext> AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            var unionField = new ArgumentedUnionMutationField<TArgType1, TArgType2, TArgType3, TExecutionContext>(Registry, Parent, Name, typeof(TLastElementType), UnionField.CreateTypeResolver<object, TLastElementType, TExecutionContext>(build), UnionTypes, UnionGraphType, Arguments);
            return ApplyField(unionField);
        }

        private Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TReturnType> resolve)
        {
            return Arguments.GetResolver(resolve);
        }
    }

    internal class ArgumentedUnionMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> :
        ArgumentedUnionFieldBase<IArguments<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, object, TExecutionContext>,
        IUnionableMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
    {
        public ArgumentedUnionMutationField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> typeResolver,
            List<Type> unionTypes,
            UnionGraphTypeDescriptor<TExecutionContext> unionGraphType,
            IArguments<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> arguments)
            : base(registry, parent, name, unionType, typeResolver, unionTypes, unionGraphType, arguments)
        {
        }

        public ArgumentedUnionMutationField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> typeResolver,
            IArguments<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> arguments)
            : base(registry, parent, name, unionType, typeResolver, arguments)
        {
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TReturnType> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public IUnionableMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            var unionField = new ArgumentedUnionMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>(Registry, Parent, Name, typeof(TLastElementType), UnionField.CreateTypeResolver<object, TLastElementType, TExecutionContext>(build), UnionTypes, UnionGraphType, Arguments);
            return ApplyField(unionField);
        }

        private Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TReturnType> resolve)
        {
            return Arguments.GetResolver(resolve);
        }
    }

    internal class ArgumentedUnionMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> :
        ArgumentedUnionFieldBase<IArguments<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, object, TExecutionContext>,
        IUnionableMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
    {
        public ArgumentedUnionMutationField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> typeResolver,
            List<Type> unionTypes,
            UnionGraphTypeDescriptor<TExecutionContext> unionGraphType,
            IArguments<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> arguments)
            : base(registry, parent, name, unionType, typeResolver, unionTypes, unionGraphType, arguments)
        {
        }

        public ArgumentedUnionMutationField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> typeResolver,
            IArguments<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> arguments)
            : base(registry, parent, name, unionType, typeResolver, arguments)
        {
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TReturnType> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            Parent.ApplyResolvedField<TReturnType>(
                this,
                graphType,
                resolver);
        }

        public IUnionableMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class
        {
            var unionField = new ArgumentedUnionMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>(Registry, Parent, Name, typeof(TLastElementType), UnionField.CreateTypeResolver<object, TLastElementType, TExecutionContext>(build), UnionTypes, UnionGraphType, Arguments);
            return ApplyField(unionField);
        }

        private Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TReturnType> resolve)
        {
            return Arguments.GetResolver(resolve);
        }
    }
}
