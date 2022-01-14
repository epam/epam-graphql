// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Builders.RootProjection.Implementations;
using Epam.GraphQL.Configuration.Implementations;
using Epam.GraphQL.Mutation;

namespace Epam.GraphQL.Builders.Mutation.Implementations
{
    internal class MutationArgumentBuilderBase<TFieldType, TArgType, TExecutionContext> :
        RootProjectionArgumentBuilderBase<TFieldType, TArgType, TExecutionContext>,
        IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType, TExecutionContext>, TArgType, TExecutionContext>
        where TFieldType : IUnionableField<object, TArgType, TExecutionContext>
    {
        protected MutationArgumentBuilderBase(TFieldType field)
            : base(field)
        {
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, MutationResult<TReturnType>> resolve)
        {
            Field.Resolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<MutationResult<TReturnType>>> resolve)
        {
            Field.Resolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType, TExecutionContext>, TArgType, TExecutionContext> AsUnionOf<TType>(
            Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType, TExecutionContext>, TArgType, TExecutionContext> AsUnionOf<TEnumerable, TElementType>(
            Action<IInlineObjectBuilder<TElementType, TExecutionContext>>? build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOf(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType, TExecutionContext>, TArgType, TExecutionContext> And<TType>(
            Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return AndImpl(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType, TExecutionContext>, TArgType, TExecutionContext> And<TEnumerable, TElementType>(
            Action<IInlineObjectBuilder<TElementType, TExecutionContext>>? build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return And(build);
        }

        private MutationArgumentBuilderBase<IUnionableField<object, TArgType, TExecutionContext>, TArgType, TExecutionContext> AsUnionOfImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return new MutationArgumentBuilderBase<IUnionableField<object, TArgType, TExecutionContext>, TArgType, TExecutionContext>(Field.AsUnionOf(build));
        }

        private MutationArgumentBuilderBase<IUnionableField<object, TArgType, TExecutionContext>, TArgType, TExecutionContext> AndImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }
    }

    internal class MutationArgumentBuilderBase<TFieldType, TArgType1, TArgType2, TExecutionContext> :
        RootProjectionArgumentBuilderBase<TFieldType, TArgType1, TArgType2, TExecutionContext>,
        IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext>
        where TFieldType : IUnionableField<object, TArgType1, TArgType2, TExecutionContext>
    {
        protected MutationArgumentBuilderBase(TFieldType field)
            : base(field)
        {
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, MutationResult<TReturnType>> resolve)
        {
            Field.Resolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<MutationResult<TReturnType>>> resolve)
        {
            Field.Resolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext> AsUnionOf<TType>(
            Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext> AsUnionOf<TEnumerable, TElementType>(
            Action<IInlineObjectBuilder<TElementType, TExecutionContext>>? build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOf(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext> And<TType>(
            Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return AndImpl(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext> And<TEnumerable, TElementType>(
            Action<IInlineObjectBuilder<TElementType, TExecutionContext>>? build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return And(build);
        }

        private MutationArgumentBuilderBase<IUnionableField<object, TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext> AsUnionOfImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return new MutationArgumentBuilderBase<IUnionableField<object, TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext>(Field.AsUnionOf(build));
        }

        private MutationArgumentBuilderBase<IUnionableField<object, TArgType1, TArgType2, TExecutionContext>, TArgType1, TArgType2, TExecutionContext> AndImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }
    }

    internal class MutationArgumentBuilderBase<TFieldType, TArgType1, TArgType2, TArgType3, TExecutionContext> :
        RootProjectionArgumentBuilderBase<TFieldType, TArgType1, TArgType2, TArgType3, TExecutionContext>,
        IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext>
        where TFieldType : IUnionableField<object, TArgType1, TArgType2, TArgType3, TExecutionContext>
    {
        protected MutationArgumentBuilderBase(TFieldType field)
            : base(field)
        {
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, MutationResult<TReturnType>> resolve)
        {
            Field.Resolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<MutationResult<TReturnType>>> resolve)
        {
            Field.Resolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext> AsUnionOf<TType>(
            Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext> AsUnionOf<TEnumerable, TElementType>(
            Action<IInlineObjectBuilder<TElementType, TExecutionContext>>? build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOf(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext> And<TType>(
            Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return AndImpl(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext> And<TEnumerable, TElementType>(
            Action<IInlineObjectBuilder<TElementType, TExecutionContext>>? build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return And(build);
        }

        private MutationArgumentBuilderBase<IUnionableField<object, TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext> AsUnionOfImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return new MutationArgumentBuilderBase<IUnionableField<object, TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext>(Field.AsUnionOf(build));
        }

        private MutationArgumentBuilderBase<IUnionableField<object, TArgType1, TArgType2, TArgType3, TExecutionContext>, TArgType1, TArgType2, TArgType3, TExecutionContext> AndImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }
    }

    internal class MutationArgumentBuilderBase<TFieldType, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> :
        RootProjectionArgumentBuilderBase<TFieldType, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>,
        IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
        where TFieldType : IUnionableField<object, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
    {
        protected MutationArgumentBuilderBase(TFieldType field)
            : base(field)
        {
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, MutationResult<TReturnType>> resolve)
        {
            Field.Resolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<MutationResult<TReturnType>>> resolve)
        {
            Field.Resolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> AsUnionOf<TType>(
            Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> AsUnionOf<TEnumerable, TElementType>(
            Action<IInlineObjectBuilder<TElementType, TExecutionContext>>? build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOf(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> And<TType>(
            Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return AndImpl(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> And<TEnumerable, TElementType>(
            Action<IInlineObjectBuilder<TElementType, TExecutionContext>>? build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return And(build);
        }

        private MutationArgumentBuilderBase<IUnionableField<object, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> AsUnionOfImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return new MutationArgumentBuilderBase<IUnionableField<object, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>(Field.AsUnionOf(build));
        }

        private MutationArgumentBuilderBase<IUnionableField<object, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> AndImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }
    }

    internal class MutationArgumentBuilderBase<TFieldType, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> :
        RootProjectionArgumentBuilderBase<TFieldType, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>,
        IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
        where TFieldType : IUnionableField<object, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
    {
        protected MutationArgumentBuilderBase(TFieldType field)
            : base(field)
        {
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, MutationResult<TReturnType>> resolve)
        {
            Field.Resolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<MutationResult<TReturnType>>> resolve)
        {
            Field.Resolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, MutationResult<TReturnType>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<MutationResult<TReturnType>>> resolve, Action<ResolveOptionsBuilder> optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> AsUnionOf<TType>(
            Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> AsUnionOf<TEnumerable, TElementType>(
            Action<IInlineObjectBuilder<TElementType, TExecutionContext>>? build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return AsUnionOf(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> And<TType>(
            Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return AndImpl(build);
        }

        public IMutationFieldBuilder<IMutationFieldBuilderBase<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> And<TEnumerable, TElementType>(
            Action<IInlineObjectBuilder<TElementType, TExecutionContext>>? build)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class
        {
            return And(build);
        }

        private MutationArgumentBuilderBase<IUnionableField<object, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> AsUnionOfImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return new MutationArgumentBuilderBase<IUnionableField<object, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>(Field.AsUnionOf(build));
        }

        private MutationArgumentBuilderBase<IUnionableField<object, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> AndImpl<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>>? build)
            where TType : class
        {
            return AsUnionOfImpl(build);
        }
    }
}
