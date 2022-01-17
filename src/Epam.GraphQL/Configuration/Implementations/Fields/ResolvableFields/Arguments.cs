// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Loaders;
using GraphQL;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields
{
    internal class Arguments<TArg1, TExecutionContext> : ArgumentsBase<TArg1, IResolveFieldContext, TExecutionContext>, IArguments<TArg1, TExecutionContext>
    {
        public Arguments(RelationRegistry<TExecutionContext> registry, string argName)
            : base(registry, new Argument<TArg1>(argName))
        {
        }

        public Arguments(RelationRegistry<TExecutionContext> registry, string argName, Type projectionType, Type entityType)
            : base(registry, new FilterArgument<TExecutionContext>(registry, argName, projectionType, entityType))
        {
        }

        public IArguments<TArg1, TArg2, TExecutionContext> Add<TArg2>(string argName)
        {
            return new Arguments<TArg1, TArg2, TExecutionContext>(this, argName);
        }

        public IArguments<TArg1, Expression<Func<TEntity, bool>>, TExecutionContext> AddFilter<TProjection, TEntity>(string argName)
            where TProjection : Projection<TEntity, TExecutionContext>
            where TEntity : class
        {
            return new Arguments<TArg1, Expression<Func<TEntity, bool>>, TExecutionContext>(this, argName, typeof(TProjection), typeof(TEntity));
        }

        public override void ApplyTo(IArgumentCollection arguments)
        {
            foreach (var arg in Items)
            {
                arguments.Argument(arg.Name, arg.InputType);
            }
        }

        protected override IResolveFieldContext GetContext(IResolveFieldContext context) => context;
    }

    internal class Arguments<TArg1, TArg2, TExecutionContext> : ArgumentsBase<TArg1, TArg2, IResolveFieldContext, TExecutionContext>, IArguments<TArg1, TArg2, TExecutionContext>
    {
        public Arguments(Arguments<TArg1, TExecutionContext> arguments, string argName)
            : base(arguments, new Argument<TArg2>(argName))
        {
        }

        public Arguments(Arguments<TArg1, TExecutionContext> arguments, string argName, Type projectionType, Type entityType)
            : base(arguments, new FilterArgument<TExecutionContext>(arguments.Registry, argName, projectionType, entityType))
        {
        }

        public IArguments<TArg1, TArg2, TArg3, TExecutionContext> Add<TArg3>(string argName)
        {
            return new Arguments<TArg1, TArg2, TArg3, TExecutionContext>(this, argName);
        }

        public IArguments<TArg1, TArg2, Expression<Func<TEntity, bool>>, TExecutionContext> AddFilter<TProjection, TEntity>(string argName)
            where TProjection : Projection<TEntity, TExecutionContext>
            where TEntity : class
        {
            return new Arguments<TArg1, TArg2, Expression<Func<TEntity, bool>>, TExecutionContext>(this, argName, typeof(TProjection), typeof(TEntity));
        }

        public override void ApplyTo(IArgumentCollection arguments)
        {
            foreach (var arg in Items)
            {
                arguments.Argument(arg.Name, arg.InputType);
            }
        }

        protected override IResolveFieldContext GetContext(IResolveFieldContext context) => context;
    }

    internal class Arguments<TArg1, TArg2, TArg3, TExecutionContext> : ArgumentsBase<TArg1, TArg2, TArg3, IResolveFieldContext, TExecutionContext>, IArguments<TArg1, TArg2, TArg3, TExecutionContext>
    {
        public Arguments(Arguments<TArg1, TArg2, TExecutionContext> arguments, string argName)
            : base(arguments, new Argument<TArg3>(argName))
        {
        }

        public Arguments(Arguments<TArg1, TArg2, TExecutionContext> arguments, string argName, Type projectionType, Type entityType)
            : base(arguments, new FilterArgument<TExecutionContext>(arguments.Registry, argName, projectionType, entityType))
        {
        }

        public IArguments<TArg1, TArg2, TArg3, TArg4, TExecutionContext> Add<TArg4>(string argName)
        {
            return new Arguments<TArg1, TArg2, TArg3, TArg4, TExecutionContext>(this, argName);
        }

        public IArguments<TArg1, TArg2, TArg3, Expression<Func<TEntity, bool>>, TExecutionContext> AddFilter<TProjection, TEntity>(string argName)
            where TProjection : Projection<TEntity, TExecutionContext>
            where TEntity : class
        {
            return new Arguments<TArg1, TArg2, TArg3, Expression<Func<TEntity, bool>>, TExecutionContext>(this, argName, typeof(TProjection), typeof(TEntity));
        }

        public override void ApplyTo(IArgumentCollection arguments)
        {
            foreach (var arg in Items)
            {
                arguments.Argument(arg.Name, arg.InputType);
            }
        }

        protected override IResolveFieldContext GetContext(IResolveFieldContext context) => context;
    }

    internal class Arguments<TArg1, TArg2, TArg3, TArg4, TExecutionContext> : ArgumentsBase<TArg1, TArg2, TArg3, TArg4, IResolveFieldContext, TExecutionContext>, IArguments<TArg1, TArg2, TArg3, TArg4, TExecutionContext>
    {
        public Arguments(Arguments<TArg1, TArg2, TArg3, TExecutionContext> arguments, string argName)
            : base(arguments, new Argument<TArg4>(argName))
        {
        }

        public Arguments(Arguments<TArg1, TArg2, TArg3, TExecutionContext> arguments, string argName, Type projectionType, Type entityType)
            : base(arguments, new FilterArgument<TExecutionContext>(arguments.Registry, argName, projectionType, entityType))
        {
        }

        public IArguments<TArg1, TArg2, TArg3, TArg4, TArg5, TExecutionContext> Add<TArg5>(string argName)
        {
            return new Arguments<TArg1, TArg2, TArg3, TArg4, TArg5, TExecutionContext>(this, argName);
        }

        public IArguments<TArg1, TArg2, TArg3, TArg4, Expression<Func<TEntity, bool>>, TExecutionContext> AddFilter<TProjection, TEntity>(string argName)
            where TProjection : Projection<TEntity, TExecutionContext>
            where TEntity : class
        {
            return new Arguments<TArg1, TArg2, TArg3, TArg4, Expression<Func<TEntity, bool>>, TExecutionContext>(this, argName, typeof(TProjection), typeof(TEntity));
        }

        public override void ApplyTo(IArgumentCollection arguments)
        {
            foreach (var arg in Items)
            {
                arguments.Argument(arg.Name, arg.InputType);
            }
        }

        protected override IResolveFieldContext GetContext(IResolveFieldContext context) => context;
    }

    internal class Arguments<TArg1, TArg2, TArg3, TArg4, TArg5, TExecutionContext> : ArgumentsBase<TArg1, TArg2, TArg3, TArg4, TArg5, IResolveFieldContext, TExecutionContext>, IArguments<TArg1, TArg2, TArg3, TArg4, TArg5, TExecutionContext>
    {
        public Arguments(Arguments<TArg1, TArg2, TArg3, TArg4, TExecutionContext> arguments, string argName)
            : base(arguments, new Argument<TArg5>(argName))
        {
        }

        public Arguments(Arguments<TArg1, TArg2, TArg3, TArg4, TExecutionContext> arguments, string argName, Type projectionType, Type entityType)
            : base(arguments, new FilterArgument<TExecutionContext>(arguments.Registry, argName, projectionType, entityType))
        {
        }

        public override void ApplyTo(IArgumentCollection arguments)
        {
            foreach (var arg in Items)
            {
                arguments.Argument(arg.Name, arg.InputType);
            }
        }

        protected override IResolveFieldContext GetContext(IResolveFieldContext context) => context;
    }
}
