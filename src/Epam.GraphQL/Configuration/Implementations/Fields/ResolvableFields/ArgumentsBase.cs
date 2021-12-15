// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq;
using Epam.GraphQL.Extensions;
using GraphQL;

#nullable enable

namespace Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields
{
    internal abstract class ArgumentsBase<TResolveArgumentContext, TExecutionContext> : IArguments
    {
        protected ArgumentsBase(RelationRegistry<TExecutionContext> registry, IArgument<TResolveArgumentContext> arg)
            : this(registry, Enumerable.Empty<IArgument<TResolveArgumentContext>>(), arg)
        {
        }

        protected ArgumentsBase(ArgumentsBase<TResolveArgumentContext, TExecutionContext> args, IArgument<TResolveArgumentContext> lastArg)
            : this(args.Registry, args.Items, lastArg)
        {
        }

        private ArgumentsBase(RelationRegistry<TExecutionContext> registry, IEnumerable<IArgument<TResolveArgumentContext>> args, IArgument<TResolveArgumentContext> lastArg)
        {
            Items = args.Concat(Enumerable.Repeat(lastArg, 1)).ToArray();
            Registry = registry;
        }

        public RelationRegistry<TExecutionContext> Registry { get; }

        protected IArgument<TResolveArgumentContext>[] Items { get; }

        public abstract void ApplyTo(IArgumentCollection arguments);

        protected abstract TResolveArgumentContext GetContext(IResolveFieldContext context);
    }

    internal abstract class ArgumentsBase<TArg1, TResolveArgumentContext, TExecutionContext> : ArgumentsBase<TResolveArgumentContext, TExecutionContext>
    {
        protected ArgumentsBase(RelationRegistry<TExecutionContext> registry, IArgument<TResolveArgumentContext> arg)
            : base(registry, arg)
        {
        }

        protected ArgumentsBase(ArgumentsBase<TResolveArgumentContext, TExecutionContext> args, IArgument<TResolveArgumentContext> lastArg)
            : base(args, lastArg)
        {
        }

        public Func<IResolveFieldContext, TResult> GetResolver<TResult>(Func<TExecutionContext, TArg1, TResult> resolve)
        {
            return ctx =>
            {
                var argCtx = GetContext(ctx);
                var context = ctx.GetUserContext<TExecutionContext>();
                var arg1 = Items[0].GetValue<TArg1>(argCtx);
                return resolve(context, arg1);
            };
        }
    }

    internal abstract class ArgumentsBase<TArg1, TArg2, TResolveArgumentContext, TExecutionContext> : ArgumentsBase<TResolveArgumentContext, TExecutionContext>
    {
        protected ArgumentsBase(RelationRegistry<TExecutionContext> registry, IArgument<TResolveArgumentContext> arg)
            : base(registry, arg)
        {
        }

        protected ArgumentsBase(ArgumentsBase<TResolveArgumentContext, TExecutionContext> args, IArgument<TResolveArgumentContext> lastArg)
            : base(args, lastArg)
        {
        }

        public Func<IResolveFieldContext, TResult> GetResolver<TResult>(Func<TExecutionContext, TArg1, TArg2, TResult> resolve)
        {
            return ctx =>
            {
                var argCtx = GetContext(ctx);
                var context = ctx.GetUserContext<TExecutionContext>();
                var arg1 = Items[0].GetValue<TArg1>(argCtx);
                var arg2 = Items[1].GetValue<TArg2>(argCtx);
                return resolve(context, arg1, arg2);
            };
        }
    }

    internal abstract class ArgumentsBase<TArg1, TArg2, TArg3, TResolveArgumentContext, TExecutionContext> : ArgumentsBase<TResolveArgumentContext, TExecutionContext>
    {
        protected ArgumentsBase(RelationRegistry<TExecutionContext> registry, IArgument<TResolveArgumentContext> arg)
            : base(registry, arg)
        {
        }

        protected ArgumentsBase(ArgumentsBase<TResolveArgumentContext, TExecutionContext> args, IArgument<TResolveArgumentContext> lastArg)
            : base(args, lastArg)
        {
        }

        public Func<IResolveFieldContext, TResult> GetResolver<TResult>(Func<TExecutionContext, TArg1, TArg2, TArg3, TResult> resolve)
        {
            return ctx =>
            {
                var argCtx = GetContext(ctx);
                var context = ctx.GetUserContext<TExecutionContext>();
                var arg1 = Items[0].GetValue<TArg1>(argCtx);
                var arg2 = Items[1].GetValue<TArg2>(argCtx);
                var arg3 = Items[2].GetValue<TArg3>(argCtx);
                return resolve(context, arg1, arg2, arg3);
            };
        }
    }

    internal abstract class ArgumentsBase<TArg1, TArg2, TArg3, TArg4, TResolveArgumentContext, TExecutionContext> : ArgumentsBase<TResolveArgumentContext, TExecutionContext>
    {
        protected ArgumentsBase(RelationRegistry<TExecutionContext> registry, IArgument<TResolveArgumentContext> arg)
            : base(registry, arg)
        {
        }

        protected ArgumentsBase(ArgumentsBase<TResolveArgumentContext, TExecutionContext> args, IArgument<TResolveArgumentContext> lastArg)
            : base(args, lastArg)
        {
        }

        public Func<IResolveFieldContext, TResult> GetResolver<TResult>(Func<TExecutionContext, TArg1, TArg2, TArg3, TArg4, TResult> resolve)
        {
            return ctx =>
            {
                var argCtx = GetContext(ctx);
                var context = ctx.GetUserContext<TExecutionContext>();
                var arg1 = Items[0].GetValue<TArg1>(argCtx);
                var arg2 = Items[1].GetValue<TArg2>(argCtx);
                var arg3 = Items[2].GetValue<TArg3>(argCtx);
                var arg4 = Items[3].GetValue<TArg4>(argCtx);
                return resolve(context, arg1, arg2, arg3, arg4);
            };
        }
    }

    internal abstract class ArgumentsBase<TArg1, TArg2, TArg3, TArg4, TArg5, TResolveArgumentContext, TExecutionContext> : ArgumentsBase<TResolveArgumentContext, TExecutionContext>
    {
        protected ArgumentsBase(RelationRegistry<TExecutionContext> registry, IArgument<TResolveArgumentContext> arg)
            : base(registry, arg)
        {
        }

        protected ArgumentsBase(ArgumentsBase<TResolveArgumentContext, TExecutionContext> args, IArgument<TResolveArgumentContext> lastArg)
            : base(args, lastArg)
        {
        }

        public Func<IResolveFieldContext, TResult> GetResolver<TResult>(Func<TExecutionContext, TArg1, TArg2, TArg3, TArg4, TArg5, TResult> resolve)
        {
            return ctx =>
            {
                var argCtx = GetContext(ctx);
                var context = ctx.GetUserContext<TExecutionContext>();
                var arg1 = Items[0].GetValue<TArg1>(argCtx);
                var arg2 = Items[1].GetValue<TArg2>(argCtx);
                var arg3 = Items[2].GetValue<TArg3>(argCtx);
                var arg4 = Items[3].GetValue<TArg4>(argCtx);
                var arg5 = Items[4].GetValue<TArg5>(argCtx);
                return resolve(context, arg1, arg2, arg3, arg4, arg5);
            };
        }
    }
}
