// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Configuration.Implementations;
using Epam.GraphQL.Configuration.Implementations.Fields;

namespace Epam.GraphQL.Builders.MutableLoader.Implementations
{
    internal class FromBatchEnumerableEditableBuilder<TField, TSourceType, TReturnType, TExecutionContext> : FromBatchSelectableEditableBuilder<TField, TSourceType, TReturnType, TExecutionContext>,
        IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TSourceType, TReturnType, TExecutionContext>
        where TField : FieldBase<TSourceType, TExecutionContext>, IFieldSupportsApplySelect<TSourceType, TReturnType, TExecutionContext>, IFieldSupportsEditSettings<TSourceType, TReturnType, TExecutionContext>
    {
        internal FromBatchEnumerableEditableBuilder(RelationRegistry<TExecutionContext> registry, TField field)
            : base(registry, field)
        {
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndAndFromBatch<TSourceType, IEnumerable<object>, TExecutionContext> AndFromBatch<TAnotherReturnType>(
            Func<TExecutionContext, IEnumerable<TSourceType>, IDictionary<TSourceType, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
        {
            // TODO Implement AndFromBatch method
            throw new NotImplementedException();
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndAndFromBatch<TSourceType, IEnumerable<object>, TExecutionContext> AndFromBatch<TAnotherReturnType>(
            Func<IEnumerable<TSourceType>, IDictionary<TSourceType, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
        {
            // TODO Implement AndFromBatch method
            throw new NotImplementedException();
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndAndFromBatch<TSourceType, IEnumerable<object>, TExecutionContext> AndFromBatch<TAnotherReturnType, TKeyType>(
            Expression<Func<TSourceType, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
        {
            // TODO Implement AndFromBatch method
            throw new NotImplementedException();
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndAndFromBatch<TSourceType, IEnumerable<object>, TExecutionContext> AndFromBatch<TAnotherReturnType, TKeyType>(
            Expression<Func<TSourceType, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, IDictionary<TKeyType, TAnotherReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
        {
            // TODO Implement AndFromBatch method
            throw new NotImplementedException();
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndAndFromBatch<TSourceType, IEnumerable<object>, TExecutionContext> AndFromBatch<TAnotherReturnType>(
            Func<TExecutionContext, IEnumerable<TSourceType>, Task<IDictionary<TSourceType, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
        {
            // TODO Implement AndFromBatch method
            throw new NotImplementedException();
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndAndFromBatch<TSourceType, IEnumerable<object>, TExecutionContext> AndFromBatch<TAnotherReturnType>(
            Func<IEnumerable<TSourceType>, Task<IDictionary<TSourceType, TAnotherReturnType>>>? batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
        {
            // TODO Implement AndFromBatch method
            throw new NotImplementedException();
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndAndFromBatch<TSourceType, IEnumerable<object>, TExecutionContext> AndFromBatch<TAnotherReturnType, TKeyType>(
            Expression<Func<TSourceType, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
        {
            // TODO Implement AndFromBatch method
            throw new NotImplementedException();
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndAndFromBatch<TSourceType, IEnumerable<object>, TExecutionContext> AndFromBatch<TAnotherReturnType, TKeyType>(
            Expression<Func<TSourceType, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TAnotherReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TAnotherReturnType, TExecutionContext>>? build)
        {
            // TODO Implement AndFromBatch method
            throw new NotImplementedException();
        }
    }
}
