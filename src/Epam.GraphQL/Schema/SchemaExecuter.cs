// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Threading.Tasks;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Infrastructure;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Metadata;
using Epam.GraphQL.Options;
using Epam.GraphQL.Types;
using GraphQL;
using GraphQL.Introspection;
using GraphQL.Resolvers;
using GraphQL.Types;
using Microsoft.Extensions.DependencyInjection;
using DocumentExecuter = Epam.GraphQL.TaskBatcher.DocumentExecuter;

namespace Epam.GraphQL
{
    /// <summary>
    /// The schema for the GraphQL request.
    /// </summary>
    /// <typeparam name="TExecutionContext"> The type of the execution context to be used for queries and mutations. </typeparam>
    public class SchemaExecuter<TExecutionContext> : IDisposable, ISchemaExecuter<TExecutionContext>
    {
        protected SchemaExecuter(SchemaOptions options)
        {
            Guards.ThrowIfNull(options, nameof(options));
            Options = options;

            var coreOptionsExtension = Options.FindExtension<CoreSchemaOptionsExtension<TExecutionContext>>();
            var schemaServiceProvider = new SchemaServiceProvider<TExecutionContext>();
            GraphQLSchema = new Schema(schemaServiceProvider);
            Registry = schemaServiceProvider.GetRequiredService<RelationRegistry<TExecutionContext>>();

            ValueConverter.Register(typeof(string), typeof(SortDirection), StringToSortDirection);
            GraphQLSchema.RegisterValueConverter(new SortDirectionAstValueConverter());
        }

        internal RelationRegistry<TExecutionContext> Registry { get; }

        internal SchemaOptions Options { get; }

        internal Schema GraphQLSchema { get; }

        /// <inheritdoc/>
        public void Dispose()
        {
            GraphQLSchema.Dispose();
        }

        public async Task<ExecutionResult> ExecuteAsync(SchemaExecutionOptions<TExecutionContext> schemaExecutionOptions)
        {
            Guards.ThrowIfNull(schemaExecutionOptions, nameof(schemaExecutionOptions));

            var executionOptions = schemaExecutionOptions.ToExecutionOptions(this);
            var profiler = ((GraphQLContext)executionOptions.UserContext["ctx"]).Profiler;

            using (profiler.CustomTiming($"GQL: {executionOptions.OperationName}", $"{executionOptions.Query}\r\n\r\nVARIABLES:\r\n{executionOptions.Inputs.ToFriendlyString()}"))
            {
                var documentExecuter = new DocumentExecuter();
                return await documentExecuter.ExecuteAsync(executionOptions).ConfigureAwait(false);
            }
        }

        private static object StringToSortDirection(object value)
        {
            switch (((string)value).ToUpperInvariant())
            {
                case "ASC":
                    return SortDirection.Asc;
                case "DESC":
                    return SortDirection.Desc;
                default:
                    break;
            }

            throw new FormatException($"Failed to parse {nameof(SortDirection)} from input '{value}'. Input should be a string (\"asc\", \"desc\") or enum (ASC, DESC).");
        }
    }

    /// <inheritdoc/>
    /// <typeparam name="TQuery"> The type of the query to be used for GraphQL schema. </typeparam>
    /// <typeparam name="TExecutionContext"> The type of the execution context to be used for queries and mutations. </typeparam>
    public class SchemaExecuter<TQuery, TExecutionContext> : SchemaExecuter<TExecutionContext>, ISchemaExecuter<TQuery, TExecutionContext>
        where TQuery : Query<TExecutionContext>, new()
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SchemaExecuter{TQuery, TExecutionContext}" /> class using the specified options.
        /// </summary>
        /// <param name="options"> The options for this schema. </param>
        public SchemaExecuter(SchemaOptions options)
            : this(options, BeforeSchemaInitialize)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SchemaExecuter{TQuery, TExecutionContext}" /> class.
        /// </summary>
        public SchemaExecuter()
            : this(new SchemaOptions())
        {
        }

        private protected SchemaExecuter(SchemaOptions options, Action<RelationRegistry<TExecutionContext>, Schema> beforeSchemaInitialize)
            : base(options)
        {
            Guards.ThrowIfNull(beforeSchemaInitialize, nameof(beforeSchemaInitialize));

            Registry.ResolveLoader<TQuery, object>();

            beforeSchemaInitialize(Registry, GraphQLSchema);

            GraphQLSchema.Initialize();

            var typeGraphType = (__Type)GraphQLSchema.FindType(nameof(__Type));
            var field = new FieldType()
            {
                Name = "metadata",
                Type = typeof(TypeMetadataGraphType),
                ResolvedType = new TypeMetadataGraphType((__Field)GraphQLSchema.FindType(nameof(__Field)), typeGraphType),
                Resolver = new FuncFieldResolver<IGraphType, TypeMetadata?>(ResolveTypeMetadata),
            };

            typeGraphType.AddField(field);
        }

        private protected static void BeforeSchemaInitialize(RelationRegistry<TExecutionContext> registry, Schema schema)
        {
            registry.ResolveLoader<TQuery, object>();
            schema.Query = registry.ResolveObjectGraphTypeWrapper<TQuery, object>();
        }

        private TypeMetadata? ResolveTypeMetadata(IResolveFieldContext<IGraphType> ctx)
        {
            var metadata = Registry.GetMetadata(ctx.Source);
            return metadata;
        }
    }

    /// <inheritdoc/>
    /// The schema for the GraphQL request.
    /// <typeparam name="TQuery"> The type of the query to be used for GraphQL schema. </typeparam>
    /// <typeparam name="TMutation"> The type of the mutation to be used for GraphQL schema. </typeparam>
    /// <typeparam name="TExecutionContext"> The type of the execution context to be used for queries and mutations. </typeparam>
    public class SchemaExecuter<TQuery, TMutation, TExecutionContext> : SchemaExecuter<TQuery, TExecutionContext>, ISchemaExecuter<TQuery, TMutation, TExecutionContext>
        where TQuery : Query<TExecutionContext>, new()
        where TMutation : Mutation<TExecutionContext>, new()
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="SchemaExecuter{TQuery, TMutation, TExecutionContext}" /> class using the specified options.
        /// </summary>
        /// <param name="options"> The options for this schema. </param>
        public SchemaExecuter(SchemaOptions options)
            : base(options, BeforeSchemaInitialize)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="SchemaExecuter{TQuery, TMutation, TExecutionContext}" /> class.
        /// </summary>
        public SchemaExecuter()
            : this(new SchemaOptions())
        {
        }

        private protected static new void BeforeSchemaInitialize(RelationRegistry<TExecutionContext> registry, Schema schema)
        {
            registry.ResolveLoader<TQuery, object>();
            registry.ResolveLoader<TMutation, object>();
            schema.Query = registry.ResolveObjectGraphTypeWrapper<TQuery, object>();
            schema.Mutation = registry.ResolveObjectGraphTypeWrapper<TMutation, object>();
        }
    }
}
