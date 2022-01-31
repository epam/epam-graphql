// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Epam.GraphQL.Extensions;
using GraphQL.DataLoader;

namespace Epam.GraphQL.TaskBatcher
{
    internal class BatchLoader
    {
        public static IDataLoader<TParameter, TResult> FromResult<TParameter, TResult>(Func<TParameter, TResult> transform) => new ParameterizedBatchTask<TParameter, TResult>(transform);

        public static IDataLoader<TParameter, TResult[]> WhenAll<TParameter, TResult>(params IDataLoader<TParameter, TResult>[] tasks)
        {
            return new WhenAllBatchTask<TParameter, TResult>(tasks);
        }

        public static IDataLoader<TParameter, TResult[]> WhenAll<TParameter, TResult>(IEnumerable<IDataLoader<TParameter, TResult>> tasks)
        {
            return WhenAll(tasks.ToArray());
        }

        private class WhenAllBatchTask<TParameter, TResult> :
            IDataLoader<TParameter, TResult[]>,
            IBatchLoaderContinuation<TParameter, TResult[]>
        {
            private readonly IDataLoader<TParameter, TResult>[] _batchTasks;

            public WhenAllBatchTask(IDataLoader<TParameter, TResult>[] tasks)
            {
                _batchTasks = tasks;
            }

            public IDataLoader<TParameter, T> Combine<T>(Func<TParameter, TResult[], T> continuation)
            {
                return new BatchLoaderContinuation<TResult[], TParameter, T>(this, continuation);
            }

            public IDataLoader<TParameter, T> Combine<T>(Func<TResult[], T> continuation)
            {
                return new BatchLoaderContinuation<TResult[], TParameter, T>(this, continuation);
            }

            public IDataLoaderResult<TResult[]> LoadAsync(TParameter source) => new WhenAllDataLoaderResult<TParameter, TResult>(source, _batchTasks);
        }

        private class WhenAllDataLoaderResult<TParameter, TResult> : IDataLoaderResult<TResult[]>
        {
            private readonly IDataLoaderResult<TResult>[] _results;

            public WhenAllDataLoaderResult(TParameter source, IEnumerable<IDataLoader<TParameter, TResult>> tasks)
            {
                _results = tasks.Select(task => task.LoadAsync(source)).ToArray();
            }

            public async Task<TResult[]> GetResultAsync(CancellationToken cancellationToken = default)
            {
                var array = new TResult[_results.Length];

                for (int i = 0; i < _results.Length; i++)
                {
                    array[i] = await _results[i].GetResultAsync(cancellationToken).ConfigureAwait(false);
                }

                return array;
            }

            async Task<object> IDataLoaderResult.GetResultAsync(CancellationToken cancellationToken)
            {
                return await GetResultAsync(cancellationToken).ConfigureAwait(false);
            }
        }

        private class ParameterizedBatchTask<TParameter, TResult> : IDataLoader<TParameter, TResult>,
            IBatchLoaderContinuation<TParameter, TResult>
        {
            private readonly Func<TParameter, TResult> _transform;

            public ParameterizedBatchTask(Func<TParameter, TResult> transform)
            {
                _transform = transform;
            }

            public IDataLoader<TParameter, T> Combine<T>(Func<TResult, T> continuation)
            {
                return new ParameterizedBatchTask<TParameter, T>(param => continuation(_transform(param)));
            }

            public IDataLoader<TParameter, T> Combine<T>(Func<TParameter, TResult, T> continuation)
            {
                return new ParameterizedBatchTask<TParameter, T>(param => continuation(param, _transform(param)));
            }

            public IDataLoaderResult<TResult> LoadAsync(TParameter param) => new DataLoaderResult<TResult>(_transform(param));
        }
    }

    internal class BatchLoader<TId, TItem> : DataLoaderBase<TId, TItem?>, IDataLoader<TId, TItem?>, IBatchLoaderContinuation<TId, TItem?>
    {
        private readonly Func<IEnumerable<TId>, IDictionary<TId, TItem>> _loader;
        private readonly Func<string> _stepNameFactory;
        private readonly IProfiler _profiler;

        public BatchLoader(Func<IEnumerable<TId>, IEnumerable<KeyValuePair<TId, TItem>>> batchLoad, Func<string> stepNameFactory, IProfiler profiler)
            : base()
        {
            _loader = keys =>
            {
                var ret = batchLoad(keys);
                return ret.ToDictionary(r => r.Key, r => r.Value);
            };

            _stepNameFactory = stepNameFactory;
            _profiler = profiler;
        }

        public IDataLoader<TId, T> Combine<T>(Func<TId, TItem?, T> continuation)
        {
            return new BatchLoaderContinuation<TItem?, TId, T>(this, continuation);
        }

        public IDataLoader<TId, T> Combine<T>(Func<TItem?, T> continuation)
        {
            return new BatchLoaderContinuation<TItem?, TId, T>(this, continuation);
        }

        protected override Task FetchAsync(IEnumerable<DataLoaderPair<TId, TItem?>> list, CancellationToken cancellationToken)
        {
            using (_profiler?.Step(_stepNameFactory()))
            {
                var keys = list.Select(x => x.Key);
                var dictionary = _loader(keys);
                foreach (var item in list)
                {
                    if (!dictionary.TryGetValue(item.Key, out TItem? value))
                    {
                        value = default;
                    }

                    item.SetResult(value);
                }

                return Task.CompletedTask;
            }
        }
    }

    internal class AsyncBatchLoader<TId, TItem> : DataLoaderBase<TId, TItem?>, IDataLoader<TId, TItem?>, IBatchLoaderContinuation<TId, TItem?>
    {
        private readonly Func<IEnumerable<TId>, ValueTask<IDictionary<TId, TItem>>> _loader;
        private readonly Func<string> _stepNameFactory;
        private readonly IProfiler _profiler;
        private readonly Func<TId, TItem?> _defaultFactory;

        public AsyncBatchLoader(Func<IEnumerable<TId>, IAsyncEnumerable<KeyValuePair<TId, TItem>>> batchLoad, Func<TId, TItem?> defaultFactory, Func<string> stepNameFactory, IProfiler profiler)
            : base()
        {
            _loader = async keys =>
            {
                var ret = batchLoad(keys);
                return await ret.ToDictionaryAsync(r => r.Key, r => r.Value, null).ConfigureAwait(false);
            };

            _defaultFactory = defaultFactory;
            _stepNameFactory = stepNameFactory;
            _profiler = profiler;
        }

        public TItem? Default(TId id) => _defaultFactory(id);

        public IDataLoader<TId, T> Combine<T>(Func<TId, TItem?, T> continuation)
        {
            return new BatchLoaderContinuation<TItem?, TId, T>(this, continuation);
        }

        public IDataLoader<TId, T> Combine<T>(Func<TItem?, T> continuation)
        {
            return new BatchLoaderContinuation<TItem?, TId, T>(this, continuation);
        }

        protected override async Task FetchAsync(IEnumerable<DataLoaderPair<TId, TItem?>> list, CancellationToken cancellationToken)
        {
            using (_profiler?.Step(_stepNameFactory()))
            {
                var keys = list.Select(x => x.Key);
                var dictionary = await _loader(keys).ConfigureAwait(false);
                foreach (var item in list)
                {
                    if (!dictionary.TryGetValue(item.Key, out TItem? value))
                    {
                        value = _defaultFactory(item.Key);
                    }

                    item.SetResult(value);
                }
            }
        }
    }

    internal class TaskBatchLoader<TId, TItem> : DataLoaderBase<TId, TItem?>, IDataLoader<TId, TItem?>, IBatchLoaderContinuation<TId, TItem?>
    {
        private readonly Func<IEnumerable<TId>, Task<IDictionary<TId, TItem>>> _loader;
        private readonly Func<string> _stepNameFactory;
        private readonly IProfiler _profiler;

        public TaskBatchLoader(Func<IEnumerable<TId>, Task<IDictionary<TId, TItem>>> batchLoad, Func<string> stepNameFactory, IProfiler profiler)
            : base()
        {
            _loader = batchLoad;
            _stepNameFactory = stepNameFactory;
            _profiler = profiler;
        }

        public IDataLoader<TId, T> Combine<T>(Func<TId, TItem?, T> continuation)
        {
            return new BatchLoaderContinuation<TItem?, TId, T>(this, continuation);
        }

        public IDataLoader<TId, T> Combine<T>(Func<TItem?, T> continuation)
        {
            return new BatchLoaderContinuation<TItem?, TId, T>(this, continuation);
        }

        protected override async Task FetchAsync(IEnumerable<DataLoaderPair<TId, TItem?>> list, CancellationToken cancellationToken)
        {
            using (_profiler?.Step(_stepNameFactory()))
            {
                var keys = list.Select(x => x.Key);
                var dictionary = await _loader(keys).ConfigureAwait(false);
                foreach (var item in list)
                {
                    if (!dictionary.TryGetValue(item.Key, out TItem? value))
                    {
                        value = default;
                    }

                    item.SetResult(value);
                }
            }
        }
    }
}
