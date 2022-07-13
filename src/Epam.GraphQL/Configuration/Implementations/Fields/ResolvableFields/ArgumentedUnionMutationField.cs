// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Helpers;
using GraphQL;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields
{
    internal class ArgumentedUnionMutationField<TArgType, TExecutionContext> :
        ArgumentedUnionFieldBase<IArguments<TArgType, TExecutionContext>, object, TExecutionContext>,
        IUnionableRootField<TArgType, TExecutionContext>
    {
        public ArgumentedUnionMutationField(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> typeResolver,
            IArguments<TArgType, TExecutionContext> arguments)
            : base(configurationContext, parent, name, unionType, typeResolver, arguments)
        {
        }

        public ArgumentedUnionMutationField(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> typeResolver,
            List<Type> unionTypes,
            UnionGraphTypeDescriptor<TExecutionContext> unionGraphType,
            IArguments<TArgType, TExecutionContext> arguments)
            : base(configurationContext, parent, name, unionType, typeResolver, unionTypes, unionGraphType, arguments)
        {
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build),
                this,
                graphType,
                resolver);
        }

        public IUnionableRootField<TArgType, TExecutionContext> AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
        {
            return And(build);
        }

        public IUnionableRootField<TArgType, TExecutionContext> And<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext
                .Chain<TLastElementType>(nameof(And))
                .OptionalArgument(build);

            var unionField = new ArgumentedUnionMutationField<TArgType, TExecutionContext>(
                configurationContext,
                Parent,
                Name,
                typeof(TLastElementType),
                UnionField.CreateTypeResolver<object, TLastElementType, TExecutionContext>(build, configurationContext),
                UnionTypes,
                UnionGraphType,
                Arguments);
            return ApplyField(unionField);
        }

        public IUnionableRootField<TArgType, TExecutionContext> AsUnionOf<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
        {
            return And<TEnumerable, TLastElementType>(build);
        }

        public IUnionableRootField<TArgType, TExecutionContext> And<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
        {
            return And(build);
        }

        private Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType, TReturnType> resolve)
        {
            return Arguments.GetResolver(resolve);
        }

        private Func<IResolveFieldContext, Task<TReturnType>> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType, Task<TReturnType>> resolve)
        {
            return Arguments.GetResolver(resolve);
        }
    }

    internal class ArgumentedUnionMutationField<TArgType1, TArgType2, TExecutionContext> :
        ArgumentedUnionFieldBase<IArguments<TArgType1, TArgType2, TExecutionContext>, object, TExecutionContext>,
        IUnionableRootField<TArgType1, TArgType2, TExecutionContext>
    {
        public ArgumentedUnionMutationField(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> typeResolver,
            IArguments<TArgType1, TArgType2, TExecutionContext> arguments)
            : base(configurationContext, parent, name, unionType, typeResolver, arguments)
        {
        }

        public ArgumentedUnionMutationField(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> typeResolver,
            List<Type> unionTypes,
            UnionGraphTypeDescriptor<TExecutionContext> unionGraphType,
            IArguments<TArgType1, TArgType2, TExecutionContext> arguments)
            : base(configurationContext, parent, name, unionType, typeResolver, unionTypes, unionGraphType, arguments)
        {
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build),
                this,
                graphType,
                resolver);
        }

        public IUnionableRootField<TArgType1, TArgType2, TExecutionContext> AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
        {
            return And(build);
        }

        public IUnionableRootField<TArgType1, TArgType2, TExecutionContext> And<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext
                .Chain<TLastElementType>(nameof(And))
                .OptionalArgument(build);

            var unionField = new ArgumentedUnionMutationField<TArgType1, TArgType2, TExecutionContext>(
                configurationContext,
                Parent,
                Name,
                typeof(TLastElementType),
                UnionField.CreateTypeResolver<object, TLastElementType, TExecutionContext>(build, configurationContext),
                UnionTypes,
                UnionGraphType,
                Arguments);
            return ApplyField(unionField);
        }

        public IUnionableRootField<TArgType1, TArgType2, TExecutionContext> AsUnionOf<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
        {
            return And<TEnumerable, TLastElementType>(build);
        }

        public IUnionableRootField<TArgType1, TArgType2, TExecutionContext> And<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
        {
            return And(build);
        }

        private Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TReturnType> resolve)
        {
            return Arguments.GetResolver(resolve);
        }

        private Func<IResolveFieldContext, Task<TReturnType>> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<TReturnType>> resolve)
        {
            return Arguments.GetResolver(resolve);
        }
    }

    internal class ArgumentedUnionMutationField<TArgType1, TArgType2, TArgType3, TExecutionContext> :
        ArgumentedUnionFieldBase<IArguments<TArgType1, TArgType2, TArgType3, TExecutionContext>, object, TExecutionContext>,
        IUnionableRootField<TArgType1, TArgType2, TArgType3, TExecutionContext>
    {
        public ArgumentedUnionMutationField(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> typeResolver,
            List<Type> unionTypes,
            UnionGraphTypeDescriptor<TExecutionContext> unionGraphType,
            IArguments<TArgType1, TArgType2, TArgType3, TExecutionContext> arguments)
            : base(configurationContext, parent, name, unionType, typeResolver, unionTypes, unionGraphType, arguments)
        {
        }

        public ArgumentedUnionMutationField(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> typeResolver,
            IArguments<TArgType1, TArgType2, TArgType3, TExecutionContext> arguments)
            : base(configurationContext, parent, name, unionType, typeResolver, arguments)
        {
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build),
                this,
                graphType,
                resolver);
        }

        public IUnionableRootField<TArgType1, TArgType2, TArgType3, TExecutionContext> AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
        {
            return And(build);
        }

        public IUnionableRootField<TArgType1, TArgType2, TArgType3, TExecutionContext> And<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext
                .Chain<TLastElementType>(nameof(And))
                .OptionalArgument(build);

            var unionField = new ArgumentedUnionMutationField<TArgType1, TArgType2, TArgType3, TExecutionContext>(
                configurationContext,
                Parent,
                Name,
                typeof(TLastElementType),
                UnionField.CreateTypeResolver<object, TLastElementType, TExecutionContext>(build, configurationContext),
                UnionTypes,
                UnionGraphType,
                Arguments);
            return ApplyField(unionField);
        }

        public IUnionableRootField<TArgType1, TArgType2, TArgType3, TExecutionContext> AsUnionOf<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
        {
            return And<TEnumerable, TLastElementType>(build);
        }

        public IUnionableRootField<TArgType1, TArgType2, TArgType3, TExecutionContext> And<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
        {
            return And(build);
        }

        private Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TReturnType> resolve)
        {
            return Arguments.GetResolver(resolve);
        }

        private Func<IResolveFieldContext, Task<TReturnType>> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<TReturnType>> resolve)
        {
            return Arguments.GetResolver(resolve);
        }
    }

    internal class ArgumentedUnionMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> :
        ArgumentedUnionFieldBase<IArguments<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, object, TExecutionContext>,
        IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
    {
        public ArgumentedUnionMutationField(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> typeResolver,
            List<Type> unionTypes,
            UnionGraphTypeDescriptor<TExecutionContext> unionGraphType,
            IArguments<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> arguments)
            : base(configurationContext, parent, name, unionType, typeResolver, unionTypes, unionGraphType, arguments)
        {
        }

        public ArgumentedUnionMutationField(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> typeResolver,
            IArguments<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> arguments)
            : base(configurationContext, parent, name, unionType, typeResolver, arguments)
        {
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build),
                this,
                graphType,
                resolver);
        }

        public IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
        {
            return And(build);
        }

        public IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> And<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext
                .Chain<TLastElementType>(nameof(And))
                .OptionalArgument(build);

            var unionField = new ArgumentedUnionMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>(
                configurationContext,
                Parent,
                Name,
                typeof(TLastElementType),
                UnionField.CreateTypeResolver<object, TLastElementType, TExecutionContext>(build, configurationContext),
                UnionTypes,
                UnionGraphType,
                Arguments);
            return ApplyField(unionField);
        }

        public IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> AsUnionOf<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
        {
            return And<TEnumerable, TLastElementType>(build);
        }

        public IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> And<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
        {
            return And(build);
        }

        private Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TReturnType> resolve)
        {
            return Arguments.GetResolver(resolve);
        }

        private Func<IResolveFieldContext, Task<TReturnType>> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<TReturnType>> resolve)
        {
            return Arguments.GetResolver(resolve);
        }
    }

    internal class ArgumentedUnionMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> :
        ArgumentedUnionFieldBase<IArguments<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, object, TExecutionContext>,
        IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
    {
        public ArgumentedUnionMutationField(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> typeResolver,
            List<Type> unionTypes,
            UnionGraphTypeDescriptor<TExecutionContext> unionGraphType,
            IArguments<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> arguments)
            : base(configurationContext, parent, name, unionType, typeResolver, unionTypes, unionGraphType, arguments)
        {
        }

        public ArgumentedUnionMutationField(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<object, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> typeResolver,
            IArguments<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> arguments)
            : base(configurationContext, parent, name, unionType, typeResolver, arguments)
        {
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType, ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build),
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var (resolver, graphType) = ResolvedMutationFieldResolverFactory.Create(GraphType.MakeListDescriptor(), ConvertFieldResolver(resolve));
            return Parent.ApplyResolvedField<TReturnType>(
                ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                    .Argument(resolve)
                    .OptionalArgument(build),
                this,
                graphType,
                resolver);
        }

        public IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
        {
            return And(build);
        }

        public IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> And<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext
                .Chain<TLastElementType>(nameof(And))
                .OptionalArgument(build);

            var unionField = new ArgumentedUnionMutationField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>(
                configurationContext,
                Parent,
                Name,
                typeof(TLastElementType),
                UnionField.CreateTypeResolver<object, TLastElementType, TExecutionContext>(build, configurationContext),
                UnionTypes,
                UnionGraphType,
                Arguments);
            return ApplyField(unionField);
        }

        public IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> AsUnionOf<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
        {
            return And<TEnumerable, TLastElementType>(build);
        }

        public IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> And<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
        {
            return And(build);
        }

        private Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TReturnType> resolve)
        {
            return Arguments.GetResolver(resolve);
        }

        private Func<IResolveFieldContext, Task<TReturnType>> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<TReturnType>> resolve)
        {
            return Arguments.GetResolver(resolve);
        }
    }
}
