// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Loaders;
using GraphQL;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields
{
    internal class ArgumentedQueryField<TArgType, TExecutionContext> :
        ArgumentedFieldBase<IArguments<TArgType, TExecutionContext>, object, TExecutionContext>,
        IArgumentedQueryField<TArgType, TExecutionContext>,
        IPayloadFieldedQueryField<TArgType, TExecutionContext>
    {
        public ArgumentedQueryField(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            IArguments<TArgType, TExecutionContext> arguments)
            : base(configurationContext, parent, name, arguments)
        {
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .OptionalArgument(build);

            var resolver = ResolvedFieldResolverFactory.Create(ConvertFieldResolver(resolve));
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext);

            return Parent.ApplyResolvedField<TReturnType>(
                configurationContext,
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .OptionalArgument(build);

            var resolver = ResolvedFieldResolverFactory.Create(ConvertFieldResolver(resolve));
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext);

            return Parent.ApplyResolvedField<TReturnType>(
                configurationContext,
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .OptionalArgument(build);

            var resolver = ResolvedFieldResolverFactory.Create(ConvertFieldResolver(resolve));
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext).MakeListDescriptor();

            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                configurationContext,
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .OptionalArgument(build);

            var resolver = ResolvedFieldResolverFactory.Create(ConvertFieldResolver(resolve));
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext).MakeListDescriptor();

            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                configurationContext,
                this,
                graphType,
                resolver);
        }

        public IArgumentedQueryField<TArgType, TArgType2, TExecutionContext> Argument<TArgType2>(string argName)
        {
            return ArgumentImpl<TArgType2>(nameof(Argument), argName);
        }

        public IArgumentedQueryField<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterArgumentImpl<TProjection, TEntity1>(nameof(FilterArgument), argName);
        }

        public IPayloadFieldedQueryField<TArgType, TArgType2, TExecutionContext> PayloadField<TArgType2>(string argName)
        {
            return ArgumentImpl<TArgType2>(nameof(PayloadField), argName);
        }

        public IPayloadFieldedQueryField<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterArgumentImpl<TProjection, TEntity1>(nameof(FilterPayloadField), argName);
        }

        public new IUnionableRootField<TArgType, TExecutionContext> AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
        {
            return And(build);
        }

        public new IUnionableRootField<TArgType, TExecutionContext> And<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext.Chain<TLastElementType>(nameof(And))
                .OptionalArgument(build);

            var unionField = new ArgumentedUnionQueryField<TArgType, TExecutionContext>(
                configurationContext,
                Parent,
                Name,
                typeof(TLastElementType),
                UnionField.CreateTypeResolver<object, TLastElementType, TExecutionContext>(build, configurationContext),
                Arguments);

            return ApplyField(unionField);
        }

        public new IUnionableRootField<TArgType, TExecutionContext> AsUnionOf<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
        {
            return And<TEnumerable, TLastElementType>(build);
        }

        public new IUnionableRootField<TArgType, TExecutionContext> And<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
        {
            return And(build);
        }

        private ArgumentedQueryField<TArgType, TArgType2, TExecutionContext> ArgumentImpl<TArgType2>(string operation, string argName)
        {
            var argumentedField = new ArgumentedQueryField<TArgType, TArgType2, TExecutionContext>(
                ConfigurationContext.Chain<TArgType2>(operation).Argument(argName),
                Parent,
                Name,
                Arguments.Add<TArgType2>(argName));
            return ApplyField(argumentedField);
        }

        private ArgumentedQueryField<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgumentImpl<TProjection, TEntity1>(string operation, string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = new ArgumentedQueryField<TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext>(
                ConfigurationContext.Chain<TProjection, TEntity1>(operation).Argument(argName),
                Parent,
                Name,
                Arguments.AddFilter<TProjection, TEntity1>(argName));
            return ApplyField(argumentedField);
        }

        private Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType, TReturnType> resolve)
        {
            return Arguments.GetResolver(resolve);
        }
    }

    internal class ArgumentedQueryField<TArgType1, TArgType2, TExecutionContext> :
        ArgumentedFieldBase<IArguments<TArgType1, TArgType2, TExecutionContext>, object, TExecutionContext>,
        IArgumentedQueryField<TArgType1, TArgType2, TExecutionContext>,
        IPayloadFieldedQueryField<TArgType1, TArgType2, TExecutionContext>
    {
        public ArgumentedQueryField(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            IArguments<TArgType1, TArgType2, TExecutionContext> arguments)
            : base(configurationContext, parent, name, arguments)
        {
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .OptionalArgument(build);

            var resolver = ResolvedFieldResolverFactory.Create(ConvertFieldResolver(resolve));
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext);

            return Parent.ApplyResolvedField<TReturnType>(
                configurationContext,
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .OptionalArgument(build);

            var resolver = ResolvedFieldResolverFactory.Create(ConvertFieldResolver(resolve));
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext);

            return Parent.ApplyResolvedField<TReturnType>(
                configurationContext,
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .OptionalArgument(build);

            var resolver = ResolvedFieldResolverFactory.Create(ConvertFieldResolver(resolve));
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext).MakeListDescriptor();

            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                configurationContext,
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .OptionalArgument(build);

            var resolver = ResolvedFieldResolverFactory.Create(ConvertFieldResolver(resolve));
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext).MakeListDescriptor();

            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                configurationContext,
                this,
                graphType,
                resolver);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TExecutionContext> AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
        {
            return And(build);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TExecutionContext> And<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext
                .Chain<TLastElementType>(nameof(And))
                .OptionalArgument(build);

            var unionField = new ArgumentedUnionQueryField<TArgType1, TArgType2, TExecutionContext>(
                configurationContext,
                Parent,
                Name,
                typeof(TLastElementType),
                UnionField.CreateTypeResolver<object, TLastElementType, TExecutionContext>(build, configurationContext),
                Arguments);
            return ApplyField(unionField);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TExecutionContext> AsUnionOf<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
        {
            return And<TEnumerable, TLastElementType>(build);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TExecutionContext> And<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
        {
            return And(build);
        }

        public IArgumentedQueryField<TArgType1, TArgType2, TArgType3, TExecutionContext> Argument<TArgType3>(string argName)
        {
            return ArgumentImpl<TArgType3>(nameof(Argument), argName);
        }

        public IArgumentedQueryField<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterArgumentImpl<TProjection, TEntity1>(nameof(FilterArgument), argName);
        }

        public IPayloadFieldedQueryField<TArgType1, TArgType2, TArgType3, TExecutionContext> PayloadField<TArgType3>(string argName)
        {
            return ArgumentImpl<TArgType3>(nameof(PayloadField), argName);
        }

        public IPayloadFieldedQueryField<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterArgumentImpl<TProjection, TEntity1>(nameof(FilterPayloadField), argName);
        }

        private ArgumentedQueryField<TArgType1, TArgType2, TArgType3, TExecutionContext> ArgumentImpl<TArgType3>(string operation, string argName)
        {
            var argumentedField = new ArgumentedQueryField<TArgType1, TArgType2, TArgType3, TExecutionContext>(
                ConfigurationContext.Chain<TArgType3>(operation).Argument(argName),
                Parent,
                Name,
                Arguments.Add<TArgType3>(argName));
            return ApplyField(argumentedField);
        }

        private ArgumentedQueryField<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgumentImpl<TProjection, TEntity1>(string operation, string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = new ArgumentedQueryField<TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext>(
                ConfigurationContext.Chain<TProjection, TEntity1>(operation).Argument(argName),
                Parent,
                Name,
                Arguments.AddFilter<TProjection, TEntity1>(argName));
            return ApplyField(argumentedField);
        }

        private Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TReturnType> resolve)
        {
            return Arguments.GetResolver(resolve);
        }
    }

    internal class ArgumentedQueryField<TArgType1, TArgType2, TArgType3, TExecutionContext> :
        ArgumentedFieldBase<IArguments<TArgType1, TArgType2, TArgType3, TExecutionContext>, object, TExecutionContext>,
        IArgumentedQueryField<TArgType1, TArgType2, TArgType3, TExecutionContext>,
        IPayloadFieldedQueryField<TArgType1, TArgType2, TArgType3, TExecutionContext>
    {
        public ArgumentedQueryField(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            IArguments<TArgType1, TArgType2, TArgType3, TExecutionContext> arguments)
            : base(configurationContext, parent, name, arguments)
        {
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .OptionalArgument(build);

            var resolver = ResolvedFieldResolverFactory.Create(ConvertFieldResolver(resolve));
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext);

            return Parent.ApplyResolvedField<TReturnType>(
                configurationContext,
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .OptionalArgument(build);

            var resolver = ResolvedFieldResolverFactory.Create(ConvertFieldResolver(resolve));
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext);

            return Parent.ApplyResolvedField<TReturnType>(
                configurationContext,
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .OptionalArgument(build);

            var resolver = ResolvedFieldResolverFactory.Create(ConvertFieldResolver(resolve));
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext).MakeListDescriptor();

            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                configurationContext,
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .OptionalArgument(build);

            var resolver = ResolvedFieldResolverFactory.Create(ConvertFieldResolver(resolve));
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext).MakeListDescriptor();

            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                configurationContext,
                this,
                graphType,
                resolver);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TArgType3, TExecutionContext> AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
        {
            return And(build);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TArgType3, TExecutionContext> And<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext
                .Chain<TLastElementType>(nameof(And))
                .OptionalArgument(build);

            var unionField = new ArgumentedUnionQueryField<TArgType1, TArgType2, TArgType3, TExecutionContext>(
                configurationContext,
                Parent,
                Name,
                typeof(TLastElementType),
                UnionField.CreateTypeResolver<object, TLastElementType, TExecutionContext>(build, configurationContext),
                Arguments);
            return ApplyField(unionField);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TArgType3, TExecutionContext> AsUnionOf<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
        {
            return And<TEnumerable, TLastElementType>(build);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TArgType3, TExecutionContext> And<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
        {
            return And(build);
        }

        public IArgumentedQueryField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> Argument<TArgType4>(string argName)
        {
            return ArgumentImpl<TArgType4>(nameof(Argument), argName);
        }

        public IArgumentedQueryField<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterArgumentImpl<TProjection, TEntity1>(nameof(FilterArgument), argName);
        }

        public IPayloadFieldedQueryField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> PayloadField<TArgType4>(string argName)
        {
            return ArgumentImpl<TArgType4>(nameof(PayloadField), argName);
        }

        public IPayloadFieldedQueryField<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterArgumentImpl<TProjection, TEntity1>(nameof(FilterPayloadField), argName);
        }

        private ArgumentedQueryField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> ArgumentImpl<TArgType4>(string operation, string argName)
        {
            var argumentedField = new ArgumentedQueryField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>(
                ConfigurationContext.Chain<TArgType4>(operation).Argument(argName),
                Parent,
                Name,
                Arguments.Add<TArgType4>(argName));
            return ApplyField(argumentedField);
        }

        private ArgumentedQueryField<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgumentImpl<TProjection, TEntity1>(string operation, string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = new ArgumentedQueryField<TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext>(
                ConfigurationContext.Chain<TProjection, TEntity1>(operation).Argument(argName),
                Parent,
                Name,
                Arguments.AddFilter<TProjection, TEntity1>(argName));
            return ApplyField(argumentedField);
        }

        private Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TReturnType> resolve)
        {
            return Arguments.GetResolver(resolve);
        }
    }

    internal class ArgumentedQueryField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> :
        ArgumentedFieldBase<IArguments<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, object, TExecutionContext>,
        IArgumentedQueryField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>,
        IPayloadFieldedQueryField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
    {
        public ArgumentedQueryField(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            IArguments<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> arguments)
            : base(configurationContext, parent, name, arguments)
        {
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .OptionalArgument(build);

            var resolver = ResolvedFieldResolverFactory.Create(ConvertFieldResolver(resolve));
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext);

            return Parent.ApplyResolvedField<TReturnType>(
                configurationContext,
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .OptionalArgument(build);

            var resolver = ResolvedFieldResolverFactory.Create(ConvertFieldResolver(resolve));
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext);

            return Parent.ApplyResolvedField<TReturnType>(
                configurationContext,
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .OptionalArgument(build);

            var resolver = ResolvedFieldResolverFactory.Create(ConvertFieldResolver(resolve));
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext).MakeListDescriptor();

            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                configurationContext,
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .OptionalArgument(build);

            var resolver = ResolvedFieldResolverFactory.Create(ConvertFieldResolver(resolve));
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext).MakeListDescriptor();

            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                configurationContext,
                this,
                graphType,
                resolver);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
        {
            return And(build);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> And<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext
                .Chain<TLastElementType>(nameof(And))
                .OptionalArgument(build);

            var unionField = new ArgumentedUnionQueryField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>(
                configurationContext,
                Parent,
                Name,
                typeof(TLastElementType),
                UnionField.CreateTypeResolver<object, TLastElementType, TExecutionContext>(build, configurationContext),
                Arguments);
            return ApplyField(unionField);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> AsUnionOf<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
        {
            return And<TEnumerable, TLastElementType>(build);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> And<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
        {
            return And(build);
        }

        public IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> Argument<TArgType5>(string argName)
        {
            return ArgumentImpl<TArgType5>(nameof(Argument), argName);
        }

        public IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgument<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterArgumentImpl<TProjection, TEntity1>(nameof(FilterArgument), argName);
        }

        public IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> PayloadField<TArgType5>(string argName)
        {
            return ArgumentImpl<TArgType5>(nameof(PayloadField), argName);
        }

        public IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterPayloadField<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            return FilterArgumentImpl<TProjection, TEntity1>(nameof(FilterPayloadField), argName);
        }

        private ArgumentedQueryField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> ArgumentImpl<TArgType5>(string operation, string argName)
        {
            var argumentedField = new ArgumentedQueryField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>(
                ConfigurationContext.Chain<TArgType5>(operation).Argument(argName),
                Parent,
                Name,
                Arguments.Add<TArgType5>(argName));
            return ApplyField(argumentedField);
        }

        private ArgumentedQueryField<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> FilterArgumentImpl<TProjection, TEntity1>(string operation, string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = new ArgumentedQueryField<TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext>(
                ConfigurationContext.Chain<TProjection, TEntity1>(operation).Argument(argName),
                Parent,
                Name,
                Arguments.AddFilter<TProjection, TEntity1>(argName));
            return ApplyField(argumentedField);
        }

        private Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TReturnType> resolve)
        {
            return Arguments.GetResolver(resolve);
        }
    }

    internal class ArgumentedQueryField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> :
        ArgumentedFieldBase<IArguments<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, object, TExecutionContext>,
        IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
    {
        public ArgumentedQueryField(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent,
            string name,
            IArguments<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> arguments)
            : base(configurationContext, parent, name, arguments)
        {
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .OptionalArgument(build);

            var resolver = ResolvedFieldResolverFactory.Create(ConvertFieldResolver(resolve));
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext);

            return Parent.ApplyResolvedField<TReturnType>(
                configurationContext,
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .OptionalArgument(build);

            var resolver = ResolvedFieldResolverFactory.Create(ConvertFieldResolver(resolve));
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext);

            return Parent.ApplyResolvedField<TReturnType>(
                configurationContext,
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .OptionalArgument(build);

            var resolver = ResolvedFieldResolverFactory.Create(ConvertFieldResolver(resolve));
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext).MakeListDescriptor();

            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                configurationContext,
                this,
                graphType,
                resolver);
        }

        public IVoid Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext.Chain<TReturnType>(nameof(Resolve))
                .Argument(resolve)
                .OptionalArgument(build);

            var resolver = ResolvedFieldResolverFactory.Create(ConvertFieldResolver(resolve));
            var graphType = Parent.GetGraphQLTypeDescriptor(this, build, configurationContext).MakeListDescriptor();

            return Parent.ApplyResolvedField<IEnumerable<TReturnType>>(
                configurationContext,
                this,
                graphType,
                resolver);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
        {
            return And(build);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> And<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
        {
            var configurationContext = ConfigurationContext
                .Chain<TLastElementType>(nameof(And))
                .OptionalArgument(build);

            var unionField = new ArgumentedUnionQueryField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>(
                configurationContext,
                Parent,
                Name,
                typeof(TLastElementType),
                UnionField.CreateTypeResolver<object, TLastElementType, TExecutionContext>(build, configurationContext),
                Arguments);
            return ApplyField(unionField);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> AsUnionOf<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
        {
            return And<TEnumerable, TLastElementType>(build);
        }

        public new IUnionableRootField<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> And<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TLastElementType>
        {
            return And(build);
        }

        private Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TReturnType> resolve)
        {
            return Arguments.GetResolver(resolve);
        }
    }
}
