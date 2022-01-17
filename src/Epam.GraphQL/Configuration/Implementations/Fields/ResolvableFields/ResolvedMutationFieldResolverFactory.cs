// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Mutation;
using Epam.GraphQL.Savers;
using Epam.GraphQL.Types;
using GraphQL;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields
{
    internal static class ResolvedMutationFieldResolverFactory
    {
        public static (IFieldResolver Resolver, IGraphTypeDescriptor<TExecutionContext> GraphType) Create<TReturnType, TExecutionContext>(
            IGraphTypeDescriptor<TExecutionContext> graphType,
            Func<IResolveFieldContext, TReturnType> resolver)
        {
            return (new FuncFieldResolver<object, TReturnType>(resolver), graphType);
        }

        public static (IFieldResolver Resolver, IGraphTypeDescriptor<TExecutionContext> GraphType) Create<TReturnType, TExecutionContext>(
            MutationField<TExecutionContext> mutationField,
            Func<IResolveFieldContext, MutationResult<TReturnType>> resolver,
            Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var builder = new ResolveOptionsBuilder();
            optionsBuilder?.Invoke(builder);
            var options = builder.Options;

            var graphType = GraphTypeDescriptor.Create<TExecutionContext>(typeof(MutationResultGraphType<,,>).MakeGenericType(mutationField.Mutation.GetType(), typeof(TExecutionContext), typeof(TReturnType)));
            var fieldResolver = new AsyncFieldResolver<object>(context =>
            {
                return DbContextSaver.PerformManualMutationAndGetResult(mutationField.Registry, resolver(context), mutationField.Mutation, context, options);
            });

            return (fieldResolver, graphType);
        }

        public static (IFieldResolver Resolver, IGraphTypeDescriptor<TExecutionContext> GraphType) Create<TReturnType, TExecutionContext>(
            IGraphTypeDescriptor<TExecutionContext> graphType,
            Func<IResolveFieldContext, Task<TReturnType>> resolver)
        {
            return (new AsyncFieldResolver<object, TReturnType>(resolver), graphType);
        }

        public static (IFieldResolver Resolver, IGraphTypeDescriptor<TExecutionContext> GraphType) Create<TReturnType, TExecutionContext>(
            MutationField<TExecutionContext> mutationField,
            Func<IResolveFieldContext, Task<MutationResult<TReturnType>>> resolver,
            Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            var builder = new ResolveOptionsBuilder();
            optionsBuilder?.Invoke(builder);
            var options = builder.Options;

            var graphType = GraphTypeDescriptor.Create<TExecutionContext>(typeof(MutationResultGraphType<,,>).MakeGenericType(mutationField.Mutation.GetType(), typeof(TExecutionContext), typeof(TReturnType)));
            var fieldResolver = new AsyncFieldResolver<object>(async context =>
            {
                var mutationResult = await resolver(context).ConfigureAwait(false);
                return await DbContextSaver.PerformManualMutationAndGetResult(mutationField.Registry, mutationResult, mutationField.Mutation, context, options).ConfigureAwait(false);
            });

            return (fieldResolver, graphType);
        }

        public static (IFieldResolver Resolver, IGraphTypeDescriptor<TExecutionContext> GraphType) Create<TReturnType, TExecutionContext>(
            MutationField<TExecutionContext> mutationField,
            IGraphTypeDescriptor<TExecutionContext> graphType,
            Func<IResolveFieldContext, IEnumerable<TReturnType>> resolver,
            Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            if (typeof(TReturnType) == typeof(object) || mutationField.Mutation.SubmitInputTypeRegistry.IsRegistered(mutationField.Mutation.GetType(), typeof(TReturnType)))
            {
                var builder = new ResolveOptionsBuilder();
                optionsBuilder?.Invoke(builder);
                var options = builder.Options;

                graphType = GraphTypeDescriptor.Create<TExecutionContext>(typeof(SubmitOutputGraphType<,>).MakeGenericType(mutationField.Mutation.GetType(), typeof(TExecutionContext)));
                var fieldResolver = new AsyncFieldResolver<object>(context =>
                {
                    var result = resolver(context).Cast<object>();
                    return DbContextSaver.PerformManualMutationAndGetResult(mutationField.Registry, result, mutationField.Mutation, context, options);
                });

                return (fieldResolver, graphType);
            }

            return Create(graphType, resolver);
        }

        public static (IFieldResolver Resolver, IGraphTypeDescriptor<TExecutionContext> GraphType) Create<TReturnType, TExecutionContext>(
            MutationField<TExecutionContext> mutationField,
            IGraphTypeDescriptor<TExecutionContext> graphType,
            Func<IResolveFieldContext, Task<IEnumerable<TReturnType>>> resolver,
            Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            if (typeof(TReturnType) == typeof(object) || mutationField.Mutation.SubmitInputTypeRegistry.IsRegistered(mutationField.Mutation.GetType(), typeof(TReturnType)))
            {
                var builder = new ResolveOptionsBuilder();
                optionsBuilder?.Invoke(builder);
                var options = builder.Options;

                graphType = GraphTypeDescriptor.Create<TExecutionContext>(typeof(SubmitOutputGraphType<,>).MakeGenericType(mutationField.Mutation.GetType(), typeof(TExecutionContext)));
                var fieldResolver = new AsyncFieldResolver<object>(async context =>
                {
                    var result = (await resolver(context).ConfigureAwait(false)).Cast<object>();
                    return await DbContextSaver.PerformManualMutationAndGetResult(mutationField.Registry, result, mutationField.Mutation, context, options)
                        .ConfigureAwait(false);
                });

                return (fieldResolver, graphType);
            }

            return Create(graphType, resolver);
        }
    }
}
