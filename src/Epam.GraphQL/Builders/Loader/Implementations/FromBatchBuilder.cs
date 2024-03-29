// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Configuration.Implementations;
using Epam.GraphQL.Configuration.Implementations.Fields;
using Epam.GraphQL.Configuration.Implementations.Fields.BatchFields;

namespace Epam.GraphQL.Builders.Loader.Implementations
{
    internal class FromBatchBuilder<TField, TSourceType, TReturnType, TExecutionContext> : SelectAndReferenceToBuilder<TField, TSourceType, TReturnType, TExecutionContext>,
        IHasSelect<TReturnType, TExecutionContext>,
        IHasSelectAndReferenceTo<TSourceType, TReturnType, TExecutionContext>,
        IHasSelectAndReferenceToAndAndFromBatch<TSourceType, TReturnType, TExecutionContext>
        where TField : FieldBase<TSourceType, TExecutionContext>,
            IFieldSupportsApplySelect<TSourceType, TReturnType, TExecutionContext>,
            IFieldSupportsApplyBatchUnion<TSourceType, TExecutionContext>
    {
        internal FromBatchBuilder(RelationRegistry<TExecutionContext> registry, TField field)
            : base(registry, field)
        {
            Field = field;
        }

        protected new TField Field { get; set; }

        public IHasSelectAndAndFromBatch<TSourceType, IEnumerable<object>, TExecutionContext> AndFromBatch<TAnotherReturnType>(
            Func<TExecutionContext, IEnumerable<TSourceType>, IDictionary<TSourceType, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
        {
            var newField = Field.ApplyBatchUnion(
                Field.ConfigurationContext.Chain<TAnotherReturnType>(nameof(AndFromBatch))
                    .Argument(batchFunc)
                    .OptionalArgument(build),
                batchFunc,
                build);
            return new FromBatchBuilder<BatchUnionField<TSourceType, TExecutionContext>, TSourceType, IEnumerable<object>, TExecutionContext>(Registry, newField);
        }

        public IHasSelectAndAndFromBatch<TSourceType, IEnumerable<object>, TExecutionContext> AndFromBatch<TAnotherReturnType>(
            Func<IEnumerable<TSourceType>, IDictionary<TSourceType, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
        {
            var newField = Field.ApplyBatchUnion(
                Field.ConfigurationContext.Chain<TAnotherReturnType>(nameof(AndFromBatch))
                    .Argument(batchFunc)
                    .OptionalArgument(build),
                batchFunc,
                build);
            return new FromBatchBuilder<BatchUnionField<TSourceType, TExecutionContext>, TSourceType, IEnumerable<object>, TExecutionContext>(Registry, newField);
        }

        public IHasSelectAndAndFromBatch<TSourceType, IEnumerable<object>, TExecutionContext> AndFromBatch<TAnotherReturnType, TKeyType>(
            Expression<Func<TSourceType, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
        {
            var newField = Field.ApplyBatchUnion(
                Field.ConfigurationContext.Chain<TAnotherReturnType, TKeyType>(nameof(AndFromBatch))
                    .Argument(keySelector)
                    .Argument(batchFunc)
                    .OptionalArgument(build),
                keySelector,
                batchFunc,
                build);
            return new FromBatchBuilder<BatchUnionField<TSourceType, TExecutionContext>, TSourceType, IEnumerable<object>, TExecutionContext>(Registry, newField);
        }

        public IHasSelectAndAndFromBatch<TSourceType, IEnumerable<object>, TExecutionContext> AndFromBatch<TAnotherReturnType, TKeyType>(
            Expression<Func<TSourceType, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, IDictionary<TKeyType, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
        {
            var newField = Field.ApplyBatchUnion(
                Field.ConfigurationContext.Chain<TAnotherReturnType, TKeyType>(nameof(AndFromBatch))
                    .Argument(keySelector)
                    .Argument(batchFunc)
                    .OptionalArgument(build),
                keySelector,
                batchFunc,
                build);
            return new FromBatchBuilder<BatchUnionField<TSourceType, TExecutionContext>, TSourceType, IEnumerable<object>, TExecutionContext>(Registry, newField);
        }

        public IHasSelectAndAndFromBatch<TSourceType, IEnumerable<object>, TExecutionContext> AndFromBatch<TAnotherReturnType>(
            Func<TExecutionContext, IEnumerable<TSourceType>, Task<IDictionary<TSourceType, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
        {
            var newField = Field.ApplyBatchUnion(
                Field.ConfigurationContext.Chain<TAnotherReturnType>(nameof(AndFromBatch))
                    .Argument(batchFunc)
                    .OptionalArgument(build),
                batchFunc,
                build);
            return new FromBatchBuilder<BatchUnionField<TSourceType, TExecutionContext>, TSourceType, IEnumerable<object>, TExecutionContext>(Registry, newField);
        }

        public IHasSelectAndAndFromBatch<TSourceType, IEnumerable<object>, TExecutionContext> AndFromBatch<TAnotherReturnType>(
            Func<IEnumerable<TSourceType>, Task<IDictionary<TSourceType, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
        {
            var newField = Field.ApplyBatchUnion(
                Field.ConfigurationContext.Chain<TAnotherReturnType>(nameof(AndFromBatch))
                    .Argument(batchFunc)
                    .OptionalArgument(build),
                batchFunc,
                build);
            return new FromBatchBuilder<BatchUnionField<TSourceType, TExecutionContext>, TSourceType, IEnumerable<object>, TExecutionContext>(Registry, newField);
        }

        public IHasSelectAndAndFromBatch<TSourceType, IEnumerable<object>, TExecutionContext> AndFromBatch<TAnotherReturnType, TKeyType>(
            Expression<Func<TSourceType, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
        {
            var newField = Field.ApplyBatchUnion(
                Field.ConfigurationContext.Chain<TAnotherReturnType, TKeyType>(nameof(AndFromBatch))
                    .Argument(keySelector)
                    .Argument(batchFunc)
                    .OptionalArgument(build),
                keySelector,
                batchFunc,
                build);
            return new FromBatchBuilder<BatchUnionField<TSourceType, TExecutionContext>, TSourceType, IEnumerable<object>, TExecutionContext>(Registry, newField);
        }

        public IHasSelectAndAndFromBatch<TSourceType, IEnumerable<object>, TExecutionContext> AndFromBatch<TAnotherReturnType, TKeyType>(
            Expression<Func<TSourceType, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build = null)
        {
            var newField = Field.ApplyBatchUnion(
                Field.ConfigurationContext.Chain<TAnotherReturnType, TKeyType>(nameof(AndFromBatch))
                    .Argument(keySelector)
                    .Argument(batchFunc)
                    .OptionalArgument(build),
                keySelector,
                batchFunc,
                build);
            return new FromBatchBuilder<BatchUnionField<TSourceType, TExecutionContext>, TSourceType, IEnumerable<object>, TExecutionContext>(Registry, newField);
        }
    }
}
