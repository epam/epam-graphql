// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration.Implementations;
using Epam.GraphQL.Mutation;

namespace Epam.GraphQL.Builders.Mutation.Implementations
{
    internal class MutationFieldBuilderBase<TField, TExecutionContext> :
        IMutationFieldBuilderBase<TExecutionContext>
        where TField : IUnionableField<object, TExecutionContext>
    {
        public MutationFieldBuilderBase(TField field)
        {
            Field = field ?? throw new ArgumentNullException(nameof(field));
        }

        protected TField Field { get; }

        public void Resolve<TReturnType>(Func<TExecutionContext, TReturnType> resolve)
        {
            Field.Resolve((ctx, entity) => resolve(ctx), null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Field.Resolve((ctx, entity) => resolve(ctx), build, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Field.Resolve((ctx, entity) => resolve(ctx), build, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<TReturnType>> resolve)
        {
            Field.Resolve((ctx, entity) => resolve(ctx), null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            Field.Resolve((ctx, entity) => resolve(ctx), optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            Field.Resolve((ctx, entity) => resolve(ctx), build, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            Field.Resolve((ctx, entity) => resolve(ctx), optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            Field.Resolve((ctx, entity) => resolve(ctx), build, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, MutationResult<TReturnType>> resolve)
        {
            Field.Resolve((ctx, entity) => resolve(ctx), null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<MutationResult<TReturnType>>> resolve)
        {
            Field.Resolve((ctx, entity) => resolve(ctx), null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.Resolve((ctx, entity) => resolve(ctx), optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.Resolve((ctx, entity) => resolve(ctx), optionsBuilder);
        }

        public IMutationFieldBuilderBase<TExecutionContext> AsUnionOf<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        public IMutationFieldBuilderBase<TExecutionContext> AsUnionOf<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>>? build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOf(build);
        }

        public IMutationFieldBuilderBase<TExecutionContext> And<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return AndImpl(build);
        }

        public IMutationFieldBuilderBase<TExecutionContext> And<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>>? build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return And(build);
        }

        private MutationFieldBuilderBase<IUnionableField<object, TExecutionContext>, TExecutionContext> AsUnionOfImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return new MutationFieldBuilderBase<IUnionableField<object, TExecutionContext>, TExecutionContext>(Field.AsUnionOf(build));
        }

        private MutationFieldBuilderBase<IUnionableField<object, TExecutionContext>, TExecutionContext> AndImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }
    }
}
