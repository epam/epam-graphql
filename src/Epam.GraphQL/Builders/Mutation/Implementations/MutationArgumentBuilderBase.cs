// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.RootProjection.Implementations;
using Epam.GraphQL.Configuration.Implementations;
using Epam.GraphQL.Mutation;

namespace Epam.GraphQL.Builders.Mutation.Implementations
{
    internal class MutationArgumentBuilderBase<TFieldType, TEntity, TArgType, TExecutionContext> : RootProjectionArgumentBuilderBase<TFieldType, TEntity, TArgType, TExecutionContext>
        where TEntity : class
        where TFieldType : IResolvableField<TEntity, TArgType, TExecutionContext>
    {
        protected MutationArgumentBuilderBase(TFieldType field)
            : base(field)
        {
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, MutationResult<TReturnType>> resolve)
        {
            Field.ApplyResolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<MutationResult<TReturnType>>> resolve)
        {
            Field.ApplyResolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.ApplyResolve(resolve, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.ApplyResolve(resolve, optionsBuilder);
        }
    }

    internal class MutationArgumentBuilderBase<TFieldType, TEntity, TArgType1, TArgType2, TExecutionContext> : RootProjectionArgumentBuilderBase<TFieldType, TEntity, TArgType1, TArgType2, TExecutionContext>
        where TEntity : class
        where TFieldType : IResolvableField<TEntity, TArgType1, TArgType2, TExecutionContext>
    {
        protected MutationArgumentBuilderBase(TFieldType field)
            : base(field)
        {
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, MutationResult<TReturnType>> resolve)
        {
            Field.ApplyResolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<MutationResult<TReturnType>>> resolve)
        {
            Field.ApplyResolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.ApplyResolve(resolve, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.ApplyResolve(resolve, optionsBuilder);
        }
    }

    internal class MutationArgumentBuilderBase<TFieldType, TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext> : RootProjectionArgumentBuilderBase<TFieldType, TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext>
        where TEntity : class
        where TFieldType : IResolvableField<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext>
    {
        protected MutationArgumentBuilderBase(TFieldType field)
            : base(field)
        {
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, MutationResult<TReturnType>> resolve)
        {
            Field.ApplyResolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<MutationResult<TReturnType>>> resolve)
        {
            Field.ApplyResolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.ApplyResolve(resolve, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.ApplyResolve(resolve, optionsBuilder);
        }
    }

    internal class MutationArgumentBuilderBase<TFieldType, TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> : RootProjectionArgumentBuilderBase<TFieldType, TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
        where TEntity : class
        where TFieldType : IResolvableField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
    {
        protected MutationArgumentBuilderBase(TFieldType field)
            : base(field)
        {
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, MutationResult<TReturnType>> resolve)
        {
            Field.ApplyResolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<MutationResult<TReturnType>>> resolve)
        {
            Field.ApplyResolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.ApplyResolve(resolve, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.ApplyResolve(resolve, optionsBuilder);
        }
    }

    internal class MutationArgumentBuilderBase<TFieldType, TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> : RootProjectionArgumentBuilderBase<TFieldType, TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
        where TEntity : class
        where TFieldType : IResolvableField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
    {
        protected MutationArgumentBuilderBase(TFieldType field)
            : base(field)
        {
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, MutationResult<TReturnType>> resolve)
        {
            Field.ApplyResolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<MutationResult<TReturnType>>> resolve)
        {
            Field.ApplyResolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.ApplyResolve(resolve, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.ApplyResolve(resolve, optionsBuilder);
        }
    }
}
