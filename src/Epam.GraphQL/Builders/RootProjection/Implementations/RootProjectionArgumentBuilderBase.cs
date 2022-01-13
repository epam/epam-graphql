// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration.Implementations;

namespace Epam.GraphQL.Builders.RootProjection.Implementations
{
    internal class RootProjectionArgumentBuilderBase<TFieldType, TArgType, TExecutionContext> : IRootProjectionFieldBuilder<TArgType, TExecutionContext>
        where TFieldType : IResolvableField<object, TArgType, TExecutionContext>
    {
        protected RootProjectionArgumentBuilderBase(TFieldType field)
        {
            Field = field ?? throw new ArgumentNullException(nameof(field));
        }

        protected TFieldType Field { get; set; }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, TReturnType> resolve)
        {
            Field.Resolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Field.Resolve(resolve, build);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<TReturnType>> resolve)
        {
            Field.Resolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Field.Resolve(resolve, build);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, IEnumerable<TReturnType>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<IEnumerable<TReturnType>>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }

        public void Resolve<TReturnType>(
            Func<TExecutionContext, TArgType, IEnumerable<TReturnType>> resolve,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build,
            Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            Field.Resolve(resolve, build, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }

        public void Resolve<TReturnType>(
            Func<TExecutionContext, TArgType, Task<IEnumerable<TReturnType>>> resolve,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build,
            Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            Field.Resolve(resolve, build, optionsBuilder);
        }
    }

    internal class RootProjectionArgumentBuilderBase<TFieldType, TArgType1, TArgType2, TExecutionContext> : IRootProjectionFieldBuilder<TArgType1, TArgType2, TExecutionContext>
        where TFieldType : IResolvableField<object, TArgType1, TArgType2, TExecutionContext>
    {
        protected RootProjectionArgumentBuilderBase(TFieldType field)
        {
            Field = field ?? throw new ArgumentNullException(nameof(field));
        }

        protected TFieldType Field { get; set; }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TReturnType> resolve)
        {
            Field.Resolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Field.Resolve(resolve, build);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<TReturnType>> resolve)
        {
            Field.Resolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Field.Resolve(resolve, build);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, IEnumerable<TReturnType>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<IEnumerable<TReturnType>>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }

        public void Resolve<TReturnType>(
            Func<TExecutionContext, TArgType1, TArgType2, IEnumerable<TReturnType>> resolve,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build,
            Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            Field.Resolve(resolve, build, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }

        public void Resolve<TReturnType>(
            Func<TExecutionContext, TArgType1, TArgType2, Task<IEnumerable<TReturnType>>> resolve,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build,
            Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            Field.Resolve(resolve, build, optionsBuilder);
        }
    }

    internal class RootProjectionArgumentBuilderBase<TFieldType, TArgType1, TArgType2, TArgType3, TExecutionContext> : IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>
        where TFieldType : IResolvableField<object, TArgType1, TArgType2, TArgType3, TExecutionContext>
    {
        protected RootProjectionArgumentBuilderBase(TFieldType field)
        {
            Field = field ?? throw new ArgumentNullException(nameof(field));
        }

        protected TFieldType Field { get; set; }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TReturnType> resolve)
        {
            Field.Resolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Field.Resolve(resolve, build);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<TReturnType>> resolve)
        {
            Field.Resolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Field.Resolve(resolve, build);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, IEnumerable<TReturnType>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<IEnumerable<TReturnType>>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }

        public void Resolve<TReturnType>(
            Func<TExecutionContext, TArgType1, TArgType2, TArgType3, IEnumerable<TReturnType>> resolve,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build,
            Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            Field.Resolve(resolve, build, optionsBuilder);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }

        public void Resolve<TReturnType>(
            Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<IEnumerable<TReturnType>>> resolve,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build,
            Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            Field.Resolve(resolve, build, optionsBuilder);
        }
    }

    internal class RootProjectionArgumentBuilderBase<TFieldType, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> : IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
        where TFieldType : IResolvableField<object, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
    {
        protected RootProjectionArgumentBuilderBase(TFieldType field)
        {
            Field = field ?? throw new ArgumentNullException(nameof(field));
        }

        protected TFieldType Field { get; set; }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TReturnType> resolve)
        {
            Field.Resolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Field.Resolve(resolve, build);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<TReturnType>> resolve)
        {
            Field.Resolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Field.Resolve(resolve, build);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, IEnumerable<TReturnType>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<IEnumerable<TReturnType>>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(
            Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, IEnumerable<TReturnType>> resolve,
            Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }

        public void Resolve<TReturnType>(
            Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, IEnumerable<TReturnType>> resolve,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build,
            Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            Field.Resolve(resolve, build, optionsBuilder);
        }

        public void Resolve<TReturnType>(
            Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<IEnumerable<TReturnType>>> resolve,
            Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }

        public void Resolve<TReturnType>(
            Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<IEnumerable<TReturnType>>> resolve,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build,
            Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            Field.Resolve(resolve, build, optionsBuilder);
        }
    }

    internal class RootProjectionArgumentBuilderBase<TFieldType, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> : IRootProjectionFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
        where TFieldType : IResolvableField<object, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
    {
        internal RootProjectionArgumentBuilderBase(TFieldType field)
        {
            Field = field ?? throw new ArgumentNullException(nameof(field));
        }

        protected TFieldType Field { get; set; }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TReturnType> resolve)
        {
            Field.Resolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Field.Resolve(resolve, build);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<TReturnType>> resolve)
        {
            Field.Resolve(resolve);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Field.Resolve(resolve, build);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, IEnumerable<TReturnType>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<IEnumerable<TReturnType>>> resolve)
        {
            Resolve(resolve, (Action<ResolveOptionsBuilder>?)null);
        }

        public void Resolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            Resolve(resolve, build, null);
        }

        public void Resolve<TReturnType>(
            Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, IEnumerable<TReturnType>> resolve,
            Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }

        public void Resolve<TReturnType>(
            Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, IEnumerable<TReturnType>> resolve,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build,
            Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            Field.Resolve(resolve, build, optionsBuilder);
        }

        public void Resolve<TReturnType>(
            Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<IEnumerable<TReturnType>>> resolve,
            Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            Field.Resolve(resolve, optionsBuilder);
        }

        public void Resolve<TReturnType>(
            Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<IEnumerable<TReturnType>>> resolve,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build,
            Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            Field.Resolve(resolve, build, optionsBuilder);
        }
    }
}
