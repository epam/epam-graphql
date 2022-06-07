// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Projection.Implementations;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Configuration.Implementations.Fields;
using Epam.GraphQL.Configuration.Implementations.Fields.BatchFields;
using Epam.GraphQL.Configuration.Implementations.Fields.ChildFields;
using Epam.GraphQL.Loaders;
using Epam.GraphQL.Sorters.Implementations;

namespace Epam.GraphQL.Builders.Loader.Implementations
{
    internal class InlineObjectFieldBuilder<TField, TEntity, TExecutionContext> :
        ProjectionFieldBuilder<TField, TEntity, TExecutionContext>,
        IInlineObjectFieldBuilder<TEntity, TExecutionContext>
        where TField : FieldBase<TEntity, TExecutionContext>, IUnionableField<TEntity, TExecutionContext>
    {
        private readonly RelationRegistry<TExecutionContext> _registry;

        internal InlineObjectFieldBuilder(RelationRegistry<TExecutionContext> registry, TField fieldType)
            : base(fieldType)
        {
            _registry = registry;
        }

        public IInlineLoaderField<TEntity, TChildEntity, TExecutionContext> FromLoader<TChildLoader, TChildEntity>(Expression<Func<TEntity, TChildEntity, bool>> condition)
            where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
        {
            var graphResultType = _registry.GetGraphTypeDescriptor<TChildLoader, TChildEntity>();
            var result = new InlineLoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>(
                Field.ConfigurationContext.Chain<TChildLoader, TChildEntity>(nameof(FromLoader)).Argument(condition),
                Field.Parent,
                Field.Name,
                condition,
                graphResultType,
                arguments: null,
                searcher: null,
                naturalSorters: SortingHelpers.Empty);

            return Field.Parent.ReplaceField(Field, result);
        }

        public IHasSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchBuilder<BatchField<TEntity, TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(
                _registry,
                Field.Parent.FromBatch(
                    Field.ConfigurationContext.Chain<TReturnType>(nameof(FromBatch))
                        .Argument(batchFunc)
                        .OptionalArgument(build),
                    Field,
                    batchFunc,
                    build));
        }

        public IHasSelectAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchBuilder<BatchEnumerableField<TEntity, TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(
                _registry,
                Field.Parent.FromBatch(
                    Field.ConfigurationContext.Chain<TReturnType>(nameof(FromBatch))
                        .Argument(batchFunc)
                        .OptionalArgument(build),
                    Field,
                    batchFunc,
                    build));
        }

        public IHasSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchBuilder<BatchField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(
                _registry,
                Field.Parent.FromBatch(
                    Field.ConfigurationContext.Chain<TKeyType, TReturnType>(nameof(FromBatch))
                        .Argument(keySelector)
                        .Argument(batchFunc)
                        .OptionalArgument(build),
                    Field,
                    keySelector,
                    batchFunc,
                    build));
        }

        public IHasSelectAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(
                _registry,
                Field.Parent.FromBatch(
                    Field.ConfigurationContext.Chain<TKeyType, TReturnType>(nameof(FromBatch))
                        .Argument(keySelector)
                        .Argument(batchFunc)
                        .OptionalArgument(build),
                    Field,
                    keySelector,
                    batchFunc,
                    build));
        }

        public IHasSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            Func<IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchBuilder<BatchField<TEntity, TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(
                _registry,
                Field.Parent.FromBatch(
                    Field.ConfigurationContext.Chain<TReturnType>(nameof(FromBatch))
                        .Argument(batchFunc)
                        .OptionalArgument(build),
                    Field,
                    batchFunc,
                    build));
        }

        public IHasSelectAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(
            Func<IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchBuilder<BatchEnumerableField<TEntity, TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(
                _registry,
                Field.Parent.FromBatch(
                    Field.ConfigurationContext.Chain<TReturnType>(nameof(FromBatch))
                        .Argument(batchFunc)
                        .OptionalArgument(build),
                    Field,
                    batchFunc,
                    build));
        }

        public IHasSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchBuilder<BatchField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(
                _registry,
                Field.Parent.FromBatch(
                    Field.ConfigurationContext.Chain<TKeyType, TReturnType>(nameof(FromBatch))
                        .Argument(keySelector)
                        .Argument(batchFunc)
                        .OptionalArgument(build),
                    Field,
                    keySelector,
                    batchFunc,
                    build));
        }

        public IHasSelectAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(
                _registry,
                Field.Parent.FromBatch(
                    Field.ConfigurationContext.Chain<TKeyType, TReturnType>(nameof(FromBatch))
                        .Argument(keySelector)
                        .Argument(batchFunc)
                        .OptionalArgument(build),
                    Field,
                    keySelector,
                    batchFunc,
                    build));
        }

        public IHasSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchBuilder<BatchField<TEntity, TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(
                _registry,
                Field.Parent.FromBatch(
                    Field.ConfigurationContext.Chain<TReturnType>(nameof(FromBatch))
                        .Argument(batchFunc)
                        .OptionalArgument(build),
                    Field,
                    batchFunc,
                    build));
        }

        public IHasSelectAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(
            Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchBuilder<BatchEnumerableField<TEntity, TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(
                _registry,
                Field.Parent.FromBatch(
                    Field.ConfigurationContext.Chain<TReturnType>(nameof(FromBatch))
                        .Argument(batchFunc)
                        .OptionalArgument(build),
                    Field,
                    batchFunc,
                    build));
        }

        public IHasSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchBuilder<BatchField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(
                _registry,
                Field.Parent.FromBatch(
                    Field.ConfigurationContext.Chain<TKeyType, TReturnType>(nameof(FromBatch))
                        .Argument(keySelector)
                        .Argument(batchFunc)
                        .OptionalArgument(build),
                    Field,
                    keySelector,
                    batchFunc,
                    build));
        }

        public IHasSelectAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(
                _registry,
                Field.Parent.FromBatch(
                    Field.ConfigurationContext.Chain<TKeyType, TReturnType>(nameof(FromBatch))
                        .Argument(keySelector)
                        .Argument(batchFunc)
                        .OptionalArgument(build),
                    Field,
                    keySelector,
                    batchFunc,
                    build));
        }

        public IHasSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchBuilder<BatchField<TEntity, TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(
                _registry,
                Field.Parent.FromBatch(
                    Field.ConfigurationContext.Chain<TReturnType>(nameof(FromBatch))
                        .Argument(batchFunc)
                        .OptionalArgument(build),
                    Field,
                    batchFunc,
                    build));
        }

        public IHasSelectAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(
            Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchBuilder<BatchEnumerableField<TEntity, TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(
                _registry,
                Field.Parent.FromBatch(
                    Field.ConfigurationContext.Chain<TReturnType>(nameof(FromBatch))
                        .Argument(batchFunc)
                        .OptionalArgument(build),
                    Field,
                    batchFunc,
                    build));
        }

        public IHasSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchBuilder<BatchField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(
                _registry,
                Field.Parent.FromBatch(
                    Field.ConfigurationContext.Chain<TKeyType, TReturnType>(nameof(FromBatch))
                        .Argument(keySelector)
                        .Argument(batchFunc)
                        .OptionalArgument(build),
                    Field,
                    keySelector,
                    batchFunc,
                    build));
        }

        public IHasSelectAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(
                _registry,
                Field.Parent.FromBatch(
                    Field.ConfigurationContext.Chain<TKeyType, TReturnType>(nameof(FromBatch))
                        .Argument(keySelector)
                        .Argument(batchFunc)
                        .OptionalArgument(build),
                    Field,
                    keySelector,
                    batchFunc,
                    build));
        }
    }
}
