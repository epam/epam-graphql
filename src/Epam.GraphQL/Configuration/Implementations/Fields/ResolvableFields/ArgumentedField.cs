// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields.Helpers;
using Epam.GraphQL.Loaders;
using GraphQL;

#nullable enable

namespace Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields
{
    internal class ArgumentedField<TEntity, TArgType, TExecutionContext> :
        ArgumentedFieldBase<IArguments<TArgType, TExecutionContext>, TEntity, TExecutionContext>,
        IResolvableField<TEntity, TArgType, TExecutionContext>,
        IArgumentedField<TEntity, TArgType, TExecutionContext>
        where TEntity : class
    {
        public ArgumentedField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IArguments<TArgType, TExecutionContext> arguments)
            : base(registry, parent, name, arguments)
        {
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType, TReturnType> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), false, optionsBuilder);
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType, Task<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), false, optionsBuilder);
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), build, false, optionsBuilder);
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), build, false, optionsBuilder);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), false, optionsBuilder);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), false, optionsBuilder);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), build, false, optionsBuilder);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), build, false, optionsBuilder);
        }

        public new IArgumentedField<TEntity, TArgType, TExecutionContext> ApplyUnion<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>> build, bool isList)
            where TLastElementType : class
        {
            var unionField = new ArgumentedUnionField<TEntity, TArgType, TExecutionContext>(Registry, Parent, Name, typeof(TLastElementType), UnionField.CreateTypeResolver<TEntity, TLastElementType, TExecutionContext>(build), isList, Arguments);
            return ApplyField(unionField);
        }

        public new IArgumentedField<TEntity, TArgType, TArgType2, TExecutionContext> ApplyArgument<TArgType2>(string argName)
        {
            var argumentedField = new ArgumentedField<TEntity, TArgType, TArgType2, TExecutionContext>(Registry, Parent, Name, Arguments.Add<TArgType2>(argName));
            return ApplyField(argumentedField);
        }

        public new IArgumentedField<TEntity, TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext> ApplyFilterArgument<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = new ArgumentedField<TEntity, TArgType, Expression<Func<TEntity1, bool>>, TExecutionContext>(Registry, Parent, Name, Arguments.AddFilter<TProjection, TEntity1>(argName));
            return ApplyField(argumentedField);
        }

        private Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType, TReturnType> resolve)
        {
            return Arguments.GetResolver(resolve);
        }
    }

    internal class ArgumentedField<TEntity, TArgType1, TArgType2, TExecutionContext> :
        ArgumentedFieldBase<IArguments<TArgType1, TArgType2, TExecutionContext>, TEntity, TExecutionContext>,
        IResolvableField<TEntity, TArgType1, TArgType2, TExecutionContext>,
        IArgumentedField<TEntity, TArgType1, TArgType2, TExecutionContext>
        where TEntity : class
    {
        public ArgumentedField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IArguments<TArgType1, TArgType2, TExecutionContext> arguments)
            : base(registry, parent, name, arguments)
        {
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TReturnType> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), false, optionsBuilder);
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), false, optionsBuilder);
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), build, false, optionsBuilder);
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), build, false, optionsBuilder);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), false, optionsBuilder);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), false, optionsBuilder);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), build, false, optionsBuilder);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), build, false, optionsBuilder);
        }

        public new IArgumentedField<TEntity, TArgType1, TArgType2, TExecutionContext> ApplyUnion<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>> build, bool isList)
            where TLastElementType : class
        {
            var unionField = new ArgumentedUnionField<TEntity, TArgType1, TArgType2, TExecutionContext>(Registry, Parent, Name, typeof(TLastElementType), UnionField.CreateTypeResolver<TEntity, TLastElementType, TExecutionContext>(build), isList, Arguments);
            return ApplyField(unionField);
        }

        public new IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext> ApplyArgument<TArgType3>(string argName)
        {
            var argumentedField = new ArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext>(Registry, Parent, Name, Arguments.Add<TArgType3>(argName));
            return ApplyField(argumentedField);
        }

        public new IArgumentedField<TEntity, TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext> ApplyFilterArgument<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = new ArgumentedField<TEntity, TArgType1, TArgType2, Expression<Func<TEntity1, bool>>, TExecutionContext>(Registry, Parent, Name, Arguments.AddFilter<TProjection, TEntity1>(argName));
            return ApplyField(argumentedField);
        }

        private Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TReturnType> resolve)
        {
            return Arguments.GetResolver(resolve);
        }
    }

    internal class ArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext> :
        ArgumentedFieldBase<IArguments<TArgType1, TArgType2, TArgType3, TExecutionContext>, TEntity, TExecutionContext>,
        IResolvableField<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext>,
        IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext>
        where TEntity : class
    {
        public ArgumentedField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IArguments<TArgType1, TArgType2, TArgType3, TExecutionContext> arguments)
            : base(registry, parent, name, arguments)
        {
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TReturnType> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), false, optionsBuilder);
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), false, optionsBuilder);
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), build, false, optionsBuilder);
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), build, false, optionsBuilder);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), false, optionsBuilder);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), false, optionsBuilder);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), build, false, optionsBuilder);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), build, false, optionsBuilder);
        }

        public new IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext> ApplyUnion<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>> build, bool isList)
            where TLastElementType : class
        {
            var unionField = new ArgumentedUnionField<TEntity, TArgType1, TArgType2, TArgType3, TExecutionContext>(Registry, Parent, Name, typeof(TLastElementType), UnionField.CreateTypeResolver<TEntity, TLastElementType, TExecutionContext>(build), isList, Arguments);
            return ApplyField(unionField);
        }

        public new IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> ApplyArgument<TArgType4>(string argName)
        {
            var argumentedField = new ArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>(Registry, Parent, Name, Arguments.Add<TArgType4>(argName));
            return ApplyField(argumentedField);
        }

        public new IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext> ApplyFilterArgument<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = new ArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, Expression<Func<TEntity1, bool>>, TExecutionContext>(Registry, Parent, Name, Arguments.AddFilter<TProjection, TEntity1>(argName));
            return ApplyField(argumentedField);
        }

        private Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TReturnType> resolve)
        {
            return Arguments.GetResolver(resolve);
        }
    }

    internal class ArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> :
        ArgumentedFieldBase<IArguments<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>, TEntity, TExecutionContext>,
        IResolvableField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>,
        IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
        where TEntity : class
    {
        public ArgumentedField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IArguments<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> arguments)
            : base(registry, parent, name, arguments)
        {
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TReturnType> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), false, optionsBuilder);
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), false, optionsBuilder);
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), build, false, optionsBuilder);
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), build, false, optionsBuilder);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), false, optionsBuilder);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), false, optionsBuilder);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), build, false, optionsBuilder);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), build, false, optionsBuilder);
        }

        public new IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext> ApplyUnion<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>> build, bool isList)
            where TLastElementType : class
        {
            var unionField = new ArgumentedUnionField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>(Registry, Parent, Name, typeof(TLastElementType), UnionField.CreateTypeResolver<TEntity, TLastElementType, TExecutionContext>(build), isList, Arguments);
            return ApplyField(unionField);
        }

        public new IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> ApplyArgument<TArgType5>(string argName)
        {
            var argumentedField = new ArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>(Registry, Parent, Name, Arguments.Add<TArgType5>(argName));
            return ApplyField(argumentedField);
        }

        public new IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext> ApplyFilterArgument<TProjection, TEntity1>(string argName)
            where TProjection : Projection<TEntity1, TExecutionContext>
            where TEntity1 : class
        {
            var argumentedField = new ArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, Expression<Func<TEntity1, bool>>, TExecutionContext>(Registry, Parent, Name, Arguments.AddFilter<TProjection, TEntity1>(argName));
            return ApplyField(argumentedField);
        }

        private Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TReturnType> resolve)
        {
            return Arguments.GetResolver(resolve);
        }
    }

    internal class ArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> :
        ArgumentedFieldBase<IArguments<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>, TEntity, TExecutionContext>,
        IResolvableField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>,
        IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
        where TEntity : class
    {
        public ArgumentedField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IArguments<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> arguments)
            : base(registry, parent, name, arguments)
        {
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TReturnType> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), false, optionsBuilder);
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), false, optionsBuilder);
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TReturnType> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), build, false, optionsBuilder);
        }

        public ResolvedField<TEntity, TReturnType, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), build, false, optionsBuilder);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, IEnumerable<TReturnType>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), false, optionsBuilder);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<IEnumerable<TReturnType>>> resolve, Action<ResolveOptionsBuilder>? optionsBuilder)
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), false, optionsBuilder);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, IEnumerable<TReturnType>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), build, false, optionsBuilder);
        }

        public ResolvedField<TEntity, IEnumerable<TReturnType>, TExecutionContext> ApplyResolve<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, Task<IEnumerable<TReturnType>>> resolve, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build, Action<ResolveOptionsBuilder>? optionsBuilder)
            where TReturnType : class
        {
            return ResolvableFieldHelpers.ApplyResolve(this, ConvertFieldResolver(resolve), build, false, optionsBuilder);
        }

        public new IArgumentedField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext> ApplyUnion<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>> build, bool isList)
            where TLastElementType : class
        {
            var unionField = new ArgumentedUnionField<TEntity, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>(Registry, Parent, Name, typeof(TLastElementType), UnionField.CreateTypeResolver<TEntity, TLastElementType, TExecutionContext>(build), isList, Arguments);
            return ApplyField(unionField);
        }

        private Func<IResolveFieldContext, TReturnType> ConvertFieldResolver<TReturnType>(Func<TExecutionContext, TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TReturnType> resolve)
        {
            return Arguments.GetResolver(resolve);
        }
    }
}