// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Builders.Loader.Implementations;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Configuration.Implementations.Fields;
using Epam.GraphQL.Configuration.Implementations.Fields.BatchFields;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Builders.MutableLoader.Implementations
{
    internal class MutableLoaderFieldBuilder<TEntity, TLoader, TExecutionContext> : BaseLoaderFieldBuilder<Field<TEntity, TExecutionContext>, TEntity, TLoader, TExecutionContext>,
        IMutableLoaderFieldBuilder<TEntity, TExecutionContext>
        where TLoader : Loader<TEntity, TExecutionContext>, new()
    {
        internal MutableLoaderFieldBuilder(RelationRegistry<TExecutionContext> registry, Field<TEntity, TExecutionContext> fieldType)
            : base(registry, fieldType)
        {
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchBuilder<BatchField<TEntity, TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(
                Registry,
                Field.Parent.FromBatch(
                    Field.ConfigurationContext.Chain<TReturnType>(nameof(FromBatch))
                        .Argument(batchFunc)
                        .OptionalArgument(build),
                    Field,
                    batchFunc,
                    build));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(
                Registry,
                Field.Parent.FromBatch(
                    Field.ConfigurationContext.Chain<TReturnType>(nameof(FromBatch))
                        .Argument(batchFunc)
                        .OptionalArgument(build),
                    Field,
                    batchFunc,
                    build));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchBuilder<BatchField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(
                Registry,
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

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(
                Registry,
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

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            Func<IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchBuilder<BatchField<TEntity, TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(
                Registry,
                Field.Parent.FromBatch(
                    Field.ConfigurationContext.Chain<TReturnType>(nameof(FromBatch))
                        .Argument(batchFunc)
                        .OptionalArgument(build),
                    Field,
                    batchFunc,
                    build));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(
            Func<IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(
                Registry,
                Field.Parent.FromBatch(
                    Field.ConfigurationContext.Chain<TReturnType>(nameof(FromBatch))
                        .Argument(batchFunc)
                        .OptionalArgument(build),
                    Field,
                    batchFunc,
                    build));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchBuilder<BatchField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(
                Registry,
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

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(
                Registry,
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

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchBuilder<BatchField<TEntity, TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(
                Registry,
                Field.Parent.FromBatch(
                    Field.ConfigurationContext.Chain<TReturnType>(nameof(FromBatch))
                        .Argument(batchFunc)
                        .OptionalArgument(build),
                    Field,
                    batchFunc,
                    build));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(
            Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(
                Registry,
                Field.Parent.FromBatch(
                    Field.ConfigurationContext.Chain<TReturnType>(nameof(FromBatch))
                        .Argument(batchFunc)
                        .OptionalArgument(build),
                    Field,
                    batchFunc,
                    build));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchBuilder<BatchField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(
                Registry,
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

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(
                Registry,
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

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchBuilder<BatchField<TEntity, TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(
                Registry,
                Field.Parent.FromBatch(
                    Field.ConfigurationContext.Chain<TReturnType>(nameof(FromBatch))
                        .Argument(batchFunc)
                        .OptionalArgument(build),
                    Field,
                    batchFunc,
                    build));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(
            Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(
                Registry,
                Field.Parent.FromBatch(
                    Field.ConfigurationContext.Chain<TReturnType>(nameof(FromBatch))
                        .Argument(batchFunc)
                        .OptionalArgument(build),
                    Field,
                    batchFunc,
                    build));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchBuilder<BatchField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(
                Registry,
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

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(
                Registry,
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
