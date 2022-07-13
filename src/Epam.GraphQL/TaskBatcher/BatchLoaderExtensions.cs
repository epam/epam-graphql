// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using GraphQL.DataLoader;

namespace Epam.GraphQL.TaskBatcher
{
    internal static class BatchLoaderExtensions
    {
        public static IDataLoader<TParameter, T2> Then<TParameter, T1, T2>(this IDataLoader<TParameter, T1> task, Func<T1, T2> continuation)
        {
            if (task is IBatchLoaderContinuation<TParameter, T1> previousContinuation)
            {
                return previousContinuation.Combine(continuation);
            }

            throw new InvalidOperationException();
        }

        public static IDataLoader<TParameter, T2> Then<TParameter, T1, T2>(this IDataLoader<TParameter, T1> task, Func<TParameter, T1, T2> continuation)
        {
            if (task is IBatchLoaderContinuation<TParameter, T1> previousContinuation)
            {
                return previousContinuation.Combine(continuation);
            }

            throw new InvalidOperationException();
        }

        public static IDataLoader<TParameter1, TResult> Then<TParameter1, TParameter2, TResult>(this Func<TParameter1, TParameter2> func, IDataLoader<TParameter2, TResult> task)
        {
            return new FuncContinuation<TParameter1, TParameter2, TResult>(task, func);
        }

        public static IDataLoader<TParameter1, TResult> Then<TParameter1, TParameter2, TResult>(this Func<TParameter1, TParameter2> func, Func<TParameter2, bool> condition, IDataLoader<TParameter2, TResult> trueTask, IDataLoader<TParameter2, TResult> falseTask)
        {
            return new FuncCondition<TParameter1, TParameter2, TResult>(func, condition, trueTask, falseTask);
        }

        private class FuncContinuation<TParameter1, TParameter2, TResult> : IDataLoader<TParameter1, TResult>,
            IBatchLoaderContinuation<TParameter1, TResult>
        {
            private readonly IDataLoader<TParameter2, TResult> _task;
            private readonly Func<TParameter1, TParameter2> _func;

            public FuncContinuation(IDataLoader<TParameter2, TResult> task, Func<TParameter1, TParameter2> func)
            {
                _task = task;
                _func = func;
            }

            public IDataLoader<TParameter1, T> Combine<T>(Func<TParameter1, TResult, T> continuation)
            {
                return new BatchLoaderContinuation<TResult, TParameter1, T>(this, continuation);
            }

            public IDataLoader<TParameter1, T> Combine<T>(Func<TResult, T> continuation)
            {
                return new BatchLoaderContinuation<TResult, TParameter1, T>(this, continuation);
            }

            public IDataLoaderResult<TResult> LoadAsync(TParameter1 source) => _task.LoadAsync(_func(source));
        }

        private class FuncCondition<TParameter1, TParameter2, TResult> : IDataLoader<TParameter1, TResult>,
            IBatchLoaderContinuation<TParameter1, TResult>
        {
            private readonly IDataLoader<TParameter2, TResult> _trueTask;
            private readonly IDataLoader<TParameter2, TResult> _falseTask;
            private readonly Func<TParameter1, TParameter2> _func;
            private readonly Func<TParameter2, bool> _condition;

            public FuncCondition(Func<TParameter1, TParameter2> func, Func<TParameter2, bool> condition, IDataLoader<TParameter2, TResult> trueTask, IDataLoader<TParameter2, TResult> falseTask)
            {
                _func = func;
                _condition = condition;
                _trueTask = trueTask;
                _falseTask = falseTask;
            }

            public IDataLoader<TParameter1, T> Combine<T>(Func<TParameter1, TResult, T> continuation)
            {
                return new BatchLoaderContinuation<TResult, TParameter1, T>(this, continuation);
            }

            public IDataLoader<TParameter1, T> Combine<T>(Func<TResult, T> continuation)
            {
                return new BatchLoaderContinuation<TResult, TParameter1, T>(this, continuation);
            }

            public IDataLoaderResult<TResult> LoadAsync(TParameter1 source)
            {
                var funcResult = _func(source);
                return _condition(funcResult)
                    ? _trueTask.LoadAsync(funcResult)
                    : _falseTask.LoadAsync(funcResult);
            }
        }
    }
}
