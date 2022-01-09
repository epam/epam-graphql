// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using Epam.GraphQL.Adapters;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Helpers;
using Epam.GraphQL.Infrastructure;
using Epam.GraphQL.TaskBatcher;
using Microsoft.Extensions.Logging;

#nullable enable

namespace Epam.GraphQL
{
    internal class GraphQLContext
    {
        public GraphQLContext(
            IDataContext? dataContext,
            IProfiler? profiler,
            IBatcher? batcher,
            IRegistry? registry,
            ILogger? logger,
            IEnumerable<ISchemaExecutionListener>? listeners)
        {
            Batcher = batcher ?? throw new ArgumentNullException(nameof(batcher));
            Profiler = profiler ?? throw new ArgumentNullException(nameof(profiler));
            DataContext = dataContext;
            Registry = registry ?? throw new ArgumentNullException(nameof(registry));
            Logger = logger ?? throw new ArgumentNullException(nameof(logger));
            ResolveFieldContextBinder = new ResolveFieldContextBinder();
            Listener = new CompoundSchemaExecutionListener(listeners);
            QueryExecuter = new QueryExecuter(Logger, DataContext, NullQueryableToAsyncEnumerableConverter.Instance);
        }

        public IBatcher Batcher { get; }

        public IDataContext? DataContext { get; }

        public IProfiler Profiler { get; }

        public IRegistry Registry { get; }

        public ILogger Logger { get; }

        public ResolveFieldContextBinder ResolveFieldContextBinder { get; }

        public ISchemaExecutionListener Listener { get; }

        public QueryExecuter QueryExecuter { get; }

        public void DisableTracking()
        {
            QueryExecuter.QueryableToAsNoTrackingQueryableConverter = DataContext;
        }

        public void EnableTracking()
        {
            QueryExecuter.QueryableToAsNoTrackingQueryableConverter = NullQueryableToAsyncEnumerableConverter.Instance;
        }
    }

    internal class GraphQLContext<TExecutionContext> : GraphQLContext, IExecutionContextAccessor<TExecutionContext>
    {
        internal GraphQLContext(
            IDataContext? dataContext,
            IProfiler? profiler,
            IBatcher? batcher,
            RelationRegistry<TExecutionContext>? registry,
            ILogger? logger,
            IEnumerable<ISchemaExecutionListener>? listeners,
            TExecutionContext? executionContext)
            : base(dataContext, profiler, batcher, registry, logger, listeners)
        {
            ExecutionContext = executionContext ?? throw new ArgumentNullException(nameof(executionContext));
        }

        public TExecutionContext ExecutionContext { get; }

        public new RelationRegistry<TExecutionContext> Registry => (RelationRegistry<TExecutionContext>)base.Registry;

        public TExecutionContext UserContext => ExecutionContext;
    }
}
