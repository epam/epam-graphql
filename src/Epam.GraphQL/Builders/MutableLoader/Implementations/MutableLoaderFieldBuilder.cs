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
        where TLoader : Projection<TEntity, TExecutionContext>, new()
        where TEntity : class
    {
        internal MutableLoaderFieldBuilder(RelationRegistry<TExecutionContext> registry, Field<TEntity, TExecutionContext> fieldType)
            : base(registry, fieldType)
        {
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchClassField<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc, build));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableClassField<TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc, build));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc, build));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc, build));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            Func<IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchClassField<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc, build));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(
            Func<IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableClassField<TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc, build));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc, build));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc, build));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchSelectableEditableBuilder<BatchField<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchSelectableEditableBuilder<BatchField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchSelectableEditableBuilder<BatchField<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchSelectableEditableBuilder<BatchField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, string, TExecutionContext> FromBatch(Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, string>> batchFunc)
        {
            return new FromBatchSelectableEditableBuilder<BatchField<TEntity, string, TExecutionContext>, TEntity, string, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, string, TExecutionContext> FromBatch<TKeyType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, string>> batchFunc)
        {
            return new FromBatchSelectableEditableBuilder<BatchField<TEntity, TKeyType, string, TExecutionContext>, TEntity, string, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, string, TExecutionContext> FromBatch(Func<IEnumerable<TEntity>, IDictionary<TEntity, string>> batchFunc)
        {
            return new FromBatchSelectableEditableBuilder<BatchField<TEntity, string, TExecutionContext>, TEntity, string, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, string, TExecutionContext> FromBatch<TKeyType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, string>> batchFunc)
        {
            return new FromBatchSelectableEditableBuilder<BatchField<TEntity, TKeyType, string, TExecutionContext>, TEntity, string, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, IEnumerable<string>, TExecutionContext> FromBatch(Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<string>>> batchFunc)
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, string, TExecutionContext>, TEntity, IEnumerable<string>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, IEnumerable<string>, TExecutionContext> FromBatch<TKeyType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<string>>> batchFunc)
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TKeyType, string, TExecutionContext>, TEntity, IEnumerable<string>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, IEnumerable<string>, TExecutionContext> FromBatch(Func<IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<string>>> batchFunc)
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, string, TExecutionContext>, TEntity, IEnumerable<string>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, IEnumerable<string>, TExecutionContext> FromBatch<TKeyType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<string>>> batchFunc)
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TKeyType, string, TExecutionContext>, TEntity, IEnumerable<string>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, TReturnType?, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TReturnType?>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchSelectableEditableBuilder<BatchField<TEntity, TReturnType?, TExecutionContext>, TEntity, TReturnType?, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, TReturnType?, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType?>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchSelectableEditableBuilder<BatchField<TEntity, TKeyType, TReturnType?, TExecutionContext>, TEntity, TReturnType?, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, TReturnType?, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, IDictionary<TEntity, TReturnType?>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchSelectableEditableBuilder<BatchField<TEntity, TReturnType?, TExecutionContext>, TEntity, TReturnType?, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, TReturnType?, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType?>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchSelectableEditableBuilder<BatchField<TEntity, TKeyType, TReturnType?, TExecutionContext>, TEntity, TReturnType?, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, IEnumerable<TReturnType?>, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType?>>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TReturnType?, TExecutionContext>, TEntity, IEnumerable<TReturnType?>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, IEnumerable<TReturnType?>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType?>>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType?, TExecutionContext>, TEntity, IEnumerable<TReturnType?>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, IEnumerable<TReturnType?>, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType?>>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TReturnType?, TExecutionContext>, TEntity, IEnumerable<TReturnType?>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, IEnumerable<TReturnType?>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType?>>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType?, TExecutionContext>, TEntity, IEnumerable<TReturnType?>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchClassField<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc, build));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(
            Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableClassField<TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc, build));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc, build));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc, build));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchClassField<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc, build));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(
            Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableClassField<TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc, build));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc, build));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceToAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc, build));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchSelectableEditableBuilder<BatchField<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchSelectableEditableBuilder<BatchField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchSelectableEditableBuilder<BatchField<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchSelectableEditableBuilder<BatchField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, string, TExecutionContext> FromBatch(Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, string>>> batchFunc)
        {
            return new FromBatchSelectableEditableBuilder<BatchField<TEntity, string, TExecutionContext>, TEntity, string, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, string, TExecutionContext> FromBatch<TKeyType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, string>>> batchFunc)
        {
            return new FromBatchSelectableEditableBuilder<BatchField<TEntity, TKeyType, string, TExecutionContext>, TEntity, string, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, string, TExecutionContext> FromBatch(Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, string>>> batchFunc)
        {
            return new FromBatchSelectableEditableBuilder<BatchField<TEntity, string, TExecutionContext>, TEntity, string, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, string, TExecutionContext> FromBatch<TKeyType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, string>>> batchFunc)
        {
            return new FromBatchSelectableEditableBuilder<BatchField<TEntity, TKeyType, string, TExecutionContext>, TEntity, string, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, IEnumerable<string>, TExecutionContext> FromBatch(Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<string>>>> batchFunc)
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, string, TExecutionContext>, TEntity, IEnumerable<string>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, IEnumerable<string>, TExecutionContext> FromBatch<TKeyType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<string>>>> batchFunc)
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TKeyType, string, TExecutionContext>, TEntity, IEnumerable<string>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, IEnumerable<string>, TExecutionContext> FromBatch(Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<string>>>> batchFunc)
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, string, TExecutionContext>, TEntity, IEnumerable<string>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, IEnumerable<string>, TExecutionContext> FromBatch<TKeyType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<string>>>> batchFunc)
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TKeyType, string, TExecutionContext>, TEntity, IEnumerable<string>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, TReturnType?, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType?>>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchSelectableEditableBuilder<BatchField<TEntity, TReturnType?, TExecutionContext>, TEntity, TReturnType?, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, TReturnType?, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType?>>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchSelectableEditableBuilder<BatchField<TEntity, TKeyType, TReturnType?, TExecutionContext>, TEntity, TReturnType?, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, TReturnType?, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType?>>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchSelectableEditableBuilder<BatchField<TEntity, TReturnType?, TExecutionContext>, TEntity, TReturnType?, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, TReturnType?, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType?>>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchSelectableEditableBuilder<BatchField<TEntity, TKeyType, TReturnType?, TExecutionContext>, TEntity, TReturnType?, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, IEnumerable<TReturnType?>, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType?>>>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TReturnType?, TExecutionContext>, TEntity, IEnumerable<TReturnType?>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, IEnumerable<TReturnType?>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType?>>>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType?, TExecutionContext>, TEntity, IEnumerable<TReturnType?>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, IEnumerable<TReturnType?>, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType?>>>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TReturnType?, TExecutionContext>, TEntity, IEnumerable<TReturnType?>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasEditableAndOnWriteAndMandatoryForUpdateAndSelectAndReferenceTo<TEntity, IEnumerable<TReturnType?>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType?>>>> batchFunc)
            where TReturnType : struct
        {
            return new FromBatchEnumerableEditableBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType?, TExecutionContext>, TEntity, IEnumerable<TReturnType?>, TExecutionContext>(Registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }
    }
}
