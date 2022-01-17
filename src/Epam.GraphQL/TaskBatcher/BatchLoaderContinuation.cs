// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using GraphQL.DataLoader;

namespace Epam.GraphQL.TaskBatcher
{
    internal class BatchLoaderContinuation<TItem, TParameter, TResult> :
        IDataLoader<TParameter, TResult>,
        IBatchLoaderContinuation<TParameter, TResult>
    {
        private readonly IDataLoader<TParameter, TItem> _batchTask;
        private readonly Func<TParameter, TItem, TResult> _continuation;

        public BatchLoaderContinuation(IDataLoader<TParameter, TItem> batchTask, Func<TParameter, TItem, TResult> continuation)
        {
            _continuation = continuation ?? throw new ArgumentNullException(nameof(continuation));
            _batchTask = batchTask ?? throw new ArgumentNullException(nameof(batchTask));
        }

        public BatchLoaderContinuation(IDataLoader<TParameter, TItem> batchTask, Func<TItem, TResult> continuation)
        {
            if (continuation == null)
            {
                throw new ArgumentNullException(nameof(continuation));
            }

            _continuation = (param, item) => continuation(item);
            _batchTask = batchTask ?? throw new ArgumentNullException(nameof(batchTask));
        }

        public IDataLoader<TParameter, T> Combine<T>(Func<TResult, T> continuation)
        {
            return new BatchLoaderContinuation<TItem, TParameter, T>(_batchTask, (param, item) => continuation(_continuation(param, item)));
        }

        public IDataLoader<TParameter, T> Combine<T>(Func<TParameter, TResult, T> continuation)
        {
            return new BatchLoaderContinuation<TItem, TParameter, T>(_batchTask, (param, item) => continuation(param, _continuation(param, item)));
        }

        public IDataLoaderResult<TResult> LoadAsync(TParameter source)
        {
            return _batchTask.LoadAsync(source).Then(item => _continuation(source, item));
        }
    }
}
