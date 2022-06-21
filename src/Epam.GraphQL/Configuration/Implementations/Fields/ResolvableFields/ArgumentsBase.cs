// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Epam.GraphQL.Extensions;
using GraphQL;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields
{
    internal abstract class ArgumentsBase<TResolveArgumentContext, TExecutionContext> : IArguments
    {
        protected ArgumentsBase(IRegistry<TExecutionContext> registry, IArgument<TResolveArgumentContext> arg)
            : this(registry, Enumerable.Empty<IArgument<TResolveArgumentContext>>(), arg)
        {
        }

        protected ArgumentsBase(ArgumentsBase<TResolveArgumentContext, TExecutionContext> args, IArgument<TResolveArgumentContext> lastArg)
            : this(args.Registry, args.Items, lastArg)
        {
        }

        private ArgumentsBase(IRegistry<TExecutionContext> registry, IEnumerable<IArgument<TResolveArgumentContext>> args, IArgument<TResolveArgumentContext> lastArg)
        {
            Items = args.Concat(Enumerable.Repeat(lastArg, 1)).ToArray();
            Registry = registry;
        }

        public IRegistry<TExecutionContext> Registry { get; }

        protected IArgument<TResolveArgumentContext>[] Items { get; }

        public abstract void ApplyTo(IArgumentCollection arguments);

        protected abstract TResolveArgumentContext GetContext(IResolveFieldContext context);
    }

    internal abstract class ArgumentsBase<TArg1, TResolveArgumentContext, TExecutionContext> : ArgumentsBase<TResolveArgumentContext, TExecutionContext>
    {
        protected ArgumentsBase(IRegistry<TExecutionContext> registry, IArgument<TResolveArgumentContext> arg)
            : base(registry, arg)
        {
        }

        public Func<IResolveFieldContext, TResult> GetResolver<TResult>(Func<TExecutionContext, TArg1, TResult> resolve)
        {
            return ctx =>
            {
                try
                {
                    var argCtx = GetContext(ctx);
                    var context = ctx.GetUserContext<TExecutionContext>();
                    var arg1 = Items[0].GetValue<TArg1>(argCtx);
                    return resolve(context, arg1);
                }
                catch (Exception e)
                {
                    ctx.LogFieldExecutionError(e);
                    throw;
                }
            };
        }

        public Func<IResolveFieldContext, Task<TResult>> GetResolver<TResult>(Func<TExecutionContext, TArg1, Task<TResult>> resolve)
        {
            return async ctx =>
            {
                try
                {
                    var argCtx = GetContext(ctx);
                    var context = ctx.GetUserContext<TExecutionContext>();
                    var arg1 = Items[0].GetValue<TArg1>(argCtx);
                    return await resolve(context, arg1).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    ctx.LogFieldExecutionError(e);
                    throw;
                }
            };
        }
    }

    internal abstract class ArgumentsBase<TArg1, TArg2, TResolveArgumentContext, TExecutionContext> : ArgumentsBase<TResolveArgumentContext, TExecutionContext>
    {
        protected ArgumentsBase(ArgumentsBase<TResolveArgumentContext, TExecutionContext> args, IArgument<TResolveArgumentContext> lastArg)
            : base(args, lastArg)
        {
        }

        public Func<IResolveFieldContext, TResult> GetResolver<TResult>(Func<TExecutionContext, TArg1, TArg2, TResult> resolve)
        {
            return ctx =>
            {
                try
                {
                    var argCtx = GetContext(ctx);
                    var context = ctx.GetUserContext<TExecutionContext>();
                    var arg1 = Items[0].GetValue<TArg1>(argCtx);
                    var arg2 = Items[1].GetValue<TArg2>(argCtx);
                    return resolve(context, arg1, arg2);
                }
                catch (Exception e)
                {
                    ctx.LogFieldExecutionError(e);
                    throw;
                }
            };
        }

        public Func<IResolveFieldContext, Task<TResult>> GetResolver<TResult>(Func<TExecutionContext, TArg1, TArg2, Task<TResult>> resolve)
        {
            return async ctx =>
            {
                try
                {
                    var argCtx = GetContext(ctx);
                    var context = ctx.GetUserContext<TExecutionContext>();
                    var arg1 = Items[0].GetValue<TArg1>(argCtx);
                    var arg2 = Items[1].GetValue<TArg2>(argCtx);
                    return await resolve(context, arg1, arg2).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    ctx.LogFieldExecutionError(e);
                    throw;
                }
            };
        }
    }

    internal abstract class ArgumentsBase<TArg1, TArg2, TArg3, TResolveArgumentContext, TExecutionContext> : ArgumentsBase<TResolveArgumentContext, TExecutionContext>
    {
        protected ArgumentsBase(ArgumentsBase<TResolveArgumentContext, TExecutionContext> args, IArgument<TResolveArgumentContext> lastArg)
            : base(args, lastArg)
        {
        }

        public Func<IResolveFieldContext, TResult> GetResolver<TResult>(Func<TExecutionContext, TArg1, TArg2, TArg3, TResult> resolve)
        {
            return ctx =>
            {
                try
                {
                    var argCtx = GetContext(ctx);
                    var context = ctx.GetUserContext<TExecutionContext>();
                    var arg1 = Items[0].GetValue<TArg1>(argCtx);
                    var arg2 = Items[1].GetValue<TArg2>(argCtx);
                    var arg3 = Items[2].GetValue<TArg3>(argCtx);
                    return resolve(context, arg1, arg2, arg3);
                }
                catch (Exception e)
                {
                    ctx.LogFieldExecutionError(e);
                    throw;
                }
            };
        }

        public Func<IResolveFieldContext, Task<TResult>> GetResolver<TResult>(Func<TExecutionContext, TArg1, TArg2, TArg3, Task<TResult>> resolve)
        {
            return async ctx =>
            {
                try
                {
                    var argCtx = GetContext(ctx);
                    var context = ctx.GetUserContext<TExecutionContext>();
                    var arg1 = Items[0].GetValue<TArg1>(argCtx);
                    var arg2 = Items[1].GetValue<TArg2>(argCtx);
                    var arg3 = Items[2].GetValue<TArg3>(argCtx);
                    return await resolve(context, arg1, arg2, arg3).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    ctx.LogFieldExecutionError(e);
                    throw;
                }
            };
        }
    }

    internal abstract class ArgumentsBase<TArg1, TArg2, TArg3, TArg4, TResolveArgumentContext, TExecutionContext> : ArgumentsBase<TResolveArgumentContext, TExecutionContext>
    {
        protected ArgumentsBase(ArgumentsBase<TResolveArgumentContext, TExecutionContext> args, IArgument<TResolveArgumentContext> lastArg)
            : base(args, lastArg)
        {
        }

        public Func<IResolveFieldContext, TResult> GetResolver<TResult>(Func<TExecutionContext, TArg1, TArg2, TArg3, TArg4, TResult> resolve)
        {
            return ctx =>
            {
                try
                {
                    var argCtx = GetContext(ctx);
                    var context = ctx.GetUserContext<TExecutionContext>();
                    var arg1 = Items[0].GetValue<TArg1>(argCtx);
                    var arg2 = Items[1].GetValue<TArg2>(argCtx);
                    var arg3 = Items[2].GetValue<TArg3>(argCtx);
                    var arg4 = Items[3].GetValue<TArg4>(argCtx);
                    return resolve(context, arg1, arg2, arg3, arg4);
                }
                catch (Exception e)
                {
                    ctx.LogFieldExecutionError(e);
                    throw;
                }
            };
        }

        public Func<IResolveFieldContext, Task<TResult>> GetResolver<TResult>(Func<TExecutionContext, TArg1, TArg2, TArg3, TArg4, Task<TResult>> resolve)
        {
            return async ctx =>
            {
                try
                {
                    var argCtx = GetContext(ctx);
                    var context = ctx.GetUserContext<TExecutionContext>();
                    var arg1 = Items[0].GetValue<TArg1>(argCtx);
                    var arg2 = Items[1].GetValue<TArg2>(argCtx);
                    var arg3 = Items[2].GetValue<TArg3>(argCtx);
                    var arg4 = Items[3].GetValue<TArg4>(argCtx);
                    return await resolve(context, arg1, arg2, arg3, arg4).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    ctx.LogFieldExecutionError(e);
                    throw;
                }
            };
        }
    }

    internal abstract class ArgumentsBase<TArg1, TArg2, TArg3, TArg4, TArg5, TResolveArgumentContext, TExecutionContext> : ArgumentsBase<TResolveArgumentContext, TExecutionContext>
    {
        protected ArgumentsBase(ArgumentsBase<TResolveArgumentContext, TExecutionContext> args, IArgument<TResolveArgumentContext> lastArg)
            : base(args, lastArg)
        {
        }

        public Func<IResolveFieldContext, TResult> GetResolver<TResult>(Func<TExecutionContext, TArg1, TArg2, TArg3, TArg4, TArg5, TResult> resolve)
        {
            return ctx =>
            {
                try
                {
                    var argCtx = GetContext(ctx);
                    var context = ctx.GetUserContext<TExecutionContext>();
                    var arg1 = Items[0].GetValue<TArg1>(argCtx);
                    var arg2 = Items[1].GetValue<TArg2>(argCtx);
                    var arg3 = Items[2].GetValue<TArg3>(argCtx);
                    var arg4 = Items[3].GetValue<TArg4>(argCtx);
                    var arg5 = Items[4].GetValue<TArg5>(argCtx);
                    return resolve(context, arg1, arg2, arg3, arg4, arg5);
                }
                catch (Exception e)
                {
                    ctx.LogFieldExecutionError(e);
                    throw;
                }
            };
        }

        public Func<IResolveFieldContext, Task<TResult>> GetResolver<TResult>(Func<TExecutionContext, TArg1, TArg2, TArg3, TArg4, TArg5, Task<TResult>> resolve)
        {
            return async ctx =>
            {
                try
                {
                    var argCtx = GetContext(ctx);
                    var context = ctx.GetUserContext<TExecutionContext>();
                    var arg1 = Items[0].GetValue<TArg1>(argCtx);
                    var arg2 = Items[1].GetValue<TArg2>(argCtx);
                    var arg3 = Items[2].GetValue<TArg3>(argCtx);
                    var arg4 = Items[3].GetValue<TArg4>(argCtx);
                    var arg5 = Items[4].GetValue<TArg5>(argCtx);
                    return await resolve(context, arg1, arg2, arg3, arg4, arg5).ConfigureAwait(false);
                }
                catch (Exception e)
                {
                    ctx.LogFieldExecutionError(e);
                    throw;
                }
            };
        }
    }
}
