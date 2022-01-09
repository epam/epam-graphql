// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Mutation;
using Epam.GraphQL.Savers;
using Epam.GraphQL.Types;
using GraphQL;
using GraphQL.Resolvers;

#nullable enable

namespace Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields
{
    internal static class ResolvedField
    {
        public static ResolvedField<TEntity, TReturnType, TExecutionContext> Create<TEntity, TReturnType, TExecutionContext>(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IGraphTypeDescriptor<TExecutionContext> graphType,
            Func<IResolveFieldContext, TReturnType> resolver,
            LazyQueryArguments? arguments,
            Action<ResolveOptionsBuilder>? optionsBuilder)
            where TEntity : class
        {
            if (TypeHelpers.FindMatchingGenericBaseType(typeof(TReturnType), typeof(MutationResult<>)) != null)
            {
                var mutationConfiguratorType = TypeHelpers.FindMatchingGenericBaseType(parent.GetType(), typeof(ObjectGraphTypeConfigurator<,,>));
                if (mutationConfiguratorType != null)
                {
                    var mutationType = mutationConfiguratorType.GenericTypeArguments[0];
                    var entityType = mutationConfiguratorType.GenericTypeArguments[1];
                    if (registry.ResolveLoader(mutationType, entityType) is Mutation<TExecutionContext> mutation)
                    {
                        var builder = new ResolveOptionsBuilder();
                        optionsBuilder?.Invoke(builder);
                        var options = builder.Options;

                        graphType = GraphTypeDescriptor.Create<TExecutionContext>(typeof(MutationResultGraphType<,,>).MakeGenericType(mutation.GetType(), typeof(TExecutionContext), typeof(TReturnType).GenericTypeArguments[0]));
                        var fieldResolver = new AsyncFieldResolver<object>(context =>
                        {
                            return DbContextSaver.PerformManualMutationAndGetResult(registry, (IMutationResult?)resolver(context), mutation, context, typeof(TReturnType).GenericTypeArguments[0], options);
                        });

                        return new ResolvedField<TEntity, TReturnType, TExecutionContext>(
                            registry,
                            parent,
                            name,
                            graphType,
                            fieldResolver,
                            arguments);
                    }
                }
            }

            return new ResolvedField<TEntity, TReturnType, TExecutionContext>(
                registry,
                parent,
                name,
                graphType,
                new FuncFieldResolver<object, TReturnType>(resolver),
                arguments);
        }

        public static ResolvedField<TEntity, TReturnType, TExecutionContext> Create<TEntity, TReturnType, TExecutionContext>(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IGraphTypeDescriptor<TExecutionContext> graphType,
            Func<IResolveFieldContext, Task<TReturnType>> resolver,
            LazyQueryArguments? arguments,
            Action<ResolveOptionsBuilder>? optionsBuilder)
            where TEntity : class
        {
            if (TypeHelpers.FindMatchingGenericBaseType(typeof(TReturnType), typeof(MutationResult<>)) != null)
            {
                var mutationConfiguratorType = TypeHelpers.FindMatchingGenericBaseType(parent.GetType(), typeof(ObjectGraphTypeConfigurator<,,>));
                if (mutationConfiguratorType != null)
                {
                    var mutationType = mutationConfiguratorType.GenericTypeArguments[0];
                    var entityType = mutationConfiguratorType.GenericTypeArguments[1];
                    if (registry.ResolveLoader(mutationType, entityType) is Mutation<TExecutionContext> mutation)
                    {
                        var builder = new ResolveOptionsBuilder();
                        optionsBuilder?.Invoke(builder);
                        var options = builder.Options;

                        graphType = GraphTypeDescriptor.Create<TExecutionContext>(typeof(MutationResultGraphType<,,>).MakeGenericType(mutation.GetType(), typeof(TExecutionContext), typeof(TReturnType).GenericTypeArguments[0]));
                        var fieldResolver = new AsyncFieldResolver<object>(async context =>
                        {
                            var mutationResult = (IMutationResult?)await resolver(context).ConfigureAwait(false);
                            return await DbContextSaver.PerformManualMutationAndGetResult(registry, mutationResult, mutation, context, typeof(TReturnType).GenericTypeArguments[0], options)
                                .ConfigureAwait(false);
                        });

                        return new ResolvedField<TEntity, TReturnType, TExecutionContext>(
                            registry,
                            parent,
                            name,
                            graphType,
                            fieldResolver,
                            arguments);
                    }
                }
            }

            return new ResolvedField<TEntity, TReturnType, TExecutionContext>(
                registry,
                parent,
                name,
                graphType,
                new AsyncFieldResolver<object, TReturnType>(resolver),
                arguments);
        }

        public static ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> Create<TEntity, TReturnType, TExecutionContext>(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IGraphTypeDescriptor<TExecutionContext> graphType,
            Func<IResolveFieldContext, IEnumerable<TReturnType>> resolver,
            LazyQueryArguments? arguments,
            Action<ResolveOptionsBuilder>? optionsBuilder)
            where TEntity : class
        {
            var mutationConfiguratorType = TypeHelpers.FindMatchingGenericBaseType(parent.GetType(), typeof(ObjectGraphTypeConfigurator<,,>));
            if (mutationConfiguratorType != null)
            {
                var mutationType = mutationConfiguratorType.GenericTypeArguments[0];
                if (typeof(TReturnType) == typeof(object) || (registry.GetService<SubmitInputTypeRegistry<TExecutionContext>>()?.IsRegistered(mutationType, typeof(TReturnType)) ?? false))
                {
                    var entityType = mutationConfiguratorType.GenericTypeArguments[1];
                    if (registry.ResolveLoader(mutationType, entityType) is Mutation<TExecutionContext> mutation)
                    {
                        var builder = new ResolveOptionsBuilder();
                        optionsBuilder?.Invoke(builder);
                        var options = builder.Options;

                        graphType = GraphTypeDescriptor.Create<TExecutionContext>(typeof(SubmitOutputGraphType<,>).MakeGenericType(mutation.GetType(), typeof(TExecutionContext)));
                        var fieldResolver = new AsyncFieldResolver<object>(context =>
                        {
                            var result = resolver(context).Cast<object>();
                            return DbContextSaver.PerformManualMutationAndGetResult(registry, result, mutation, context, options);
                        });

                        return new ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext>(
                            registry,
                            parent,
                            name,
                            graphType,
                            fieldResolver,
                            arguments);
                    }
                }
            }

            return Create<TEntity, IEnumerable<TReturnType>, TExecutionContext>(
                registry,
                parent,
                name,
                graphType,
                resolver,
                arguments,
                optionsBuilder);
        }

        public static ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> Create<TEntity, TReturnType, TExecutionContext>(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IGraphTypeDescriptor<TExecutionContext> graphType,
            Func<IResolveFieldContext, Task<IEnumerable<TReturnType>>> resolver,
            LazyQueryArguments? arguments,
            Action<ResolveOptionsBuilder>? optionsBuilder)
            where TEntity : class
        {
            var mutationConfiguratorType = TypeHelpers.FindMatchingGenericBaseType(parent.GetType(), typeof(ObjectGraphTypeConfigurator<,,>));
            if (mutationConfiguratorType != null)
            {
                var mutationType = mutationConfiguratorType.GenericTypeArguments[0];
                if (typeof(TReturnType) == typeof(object) || (registry.GetService<SubmitInputTypeRegistry<TExecutionContext>>()?.IsRegistered(mutationType, typeof(TReturnType)) ?? false))
                {
                    var entityType = mutationConfiguratorType.GenericTypeArguments[1];

                    if (registry.ResolveLoader(mutationType, entityType) is Mutation<TExecutionContext> mutation)
                    {
                        var builder = new ResolveOptionsBuilder();
                        optionsBuilder?.Invoke(builder);
                        var options = builder.Options;

                        graphType = GraphTypeDescriptor.Create<TExecutionContext>(typeof(SubmitOutputGraphType<,>).MakeGenericType(mutation.GetType(), typeof(TExecutionContext)));
                        var fieldResolver = new AsyncFieldResolver<object>(async context =>
                        {
                            var result = (await resolver(context).ConfigureAwait(false)).Cast<object>();
                            return await DbContextSaver.PerformManualMutationAndGetResult(registry, result, mutation, context, options)
                                .ConfigureAwait(false);
                        });

                        return new ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext>(
                            registry,
                            parent,
                            name,
                            graphType,
                            fieldResolver,
                            arguments);
                    }
                }
            }

            return Create<TEntity, IEnumerable<TReturnType>, TExecutionContext>(
                registry,
                parent,
                name,
                graphType,
                resolver,
                arguments,
                optionsBuilder);
        }
    }

    internal class ResolvedField<TEntity, TReturnType, TExecutionContext> : TypedField<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        private readonly IFieldResolver _resolver;
        private readonly IGraphTypeDescriptor<TExecutionContext> _graphType;

        public ResolvedField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IGraphTypeDescriptor<TExecutionContext> graphType,
            IFieldResolver resolver,
            LazyQueryArguments? arguments)
            : base(registry, parent, name)
        {
            _resolver = resolver ?? throw new ArgumentNullException(nameof(resolver));
            _graphType = graphType;
            Arguments = arguments;

            parent.ProxyAccessor.AddAllMembers(Name);
        }

        public override IGraphTypeDescriptor<TExecutionContext> GraphType => _graphType;

        protected override IFieldResolver GetResolver()
        {
            return _resolver;
        }
    }
}
