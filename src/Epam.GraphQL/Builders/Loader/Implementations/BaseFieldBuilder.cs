// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Projection.Implementations;
using Epam.GraphQL.Configuration.Implementations.Fields;
using Epam.GraphQL.Configuration.Implementations.Fields.BatchFields;

namespace Epam.GraphQL.Builders.Loader.Implementations
{
    internal class BaseFieldBuilder<TEntity, TExecutionContext> : ProjectionFieldBuilder<TEntity, TExecutionContext>, IHasFromIQueryable<TEntity, TExecutionContext>
        where TEntity : class
    {
        internal BaseFieldBuilder(Field<TEntity, TExecutionContext> fieldType)
            : base(fieldType)
        {
        }

        public BatchClassField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            return SetField(Field.Parent.FromBatch(Field, batchFunc, build));
        }

        public BatchEnumerableClassField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return SetField(Field.Parent.FromBatch(Field, batchFunc, build));
        }

        public BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return SetField(Field.Parent.FromBatch(Field, keySelector, batchFunc, build));
        }

        public BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return SetField(Field.Parent.FromBatch(Field, keySelector, batchFunc, build));
        }

        public BatchField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc)
        {
            return SetField(Field.Parent.FromBatch(Field, batchFunc));
        }

        public BatchEnumerableField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc)
        {
            return SetField(Field.Parent.FromBatch(Field, batchFunc));
        }

        public BatchField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc)
        {
            return SetField(Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc)
        {
            return SetField(Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public BatchClassField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            return SetField(Field.Parent.FromBatch(Field, batchFunc, build));
        }

        public BatchEnumerableClassField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return SetField(Field.Parent.FromBatch(Field, batchFunc, build));
        }

        public BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return SetField(Field.Parent.FromBatch(Field, keySelector, batchFunc, build));
        }

        public BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return SetField(Field.Parent.FromBatch(Field, keySelector, batchFunc, build));
        }

        public BatchField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc)
        {
            return SetField(Field.Parent.FromBatch(Field, batchFunc));
        }

        public BatchEnumerableField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc)
        {
            return SetField(Field.Parent.FromBatch(Field, batchFunc));
        }

        public BatchField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc)
        {
            return SetField(Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc)
        {
            return SetField(Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public BatchClassField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            return SetField(Field.Parent.FromBatch(Field, batchFunc, build));
        }

        public BatchEnumerableClassField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return SetField(Field.Parent.FromBatch(Field, batchFunc, build));
        }

        public BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return SetField(Field.Parent.FromBatch(Field, keySelector, batchFunc, build));
        }

        public BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return SetField(Field.Parent.FromBatch(Field, keySelector, batchFunc, build));
        }

        public BatchField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc)
        {
            return SetField(Field.Parent.FromBatch(Field, batchFunc));
        }

        public BatchEnumerableField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc)
        {
            return SetField(Field.Parent.FromBatch(Field, batchFunc));
        }

        public BatchField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc)
        {
            return SetField(Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc)
        {
            return SetField(Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public BatchClassField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            return SetField(Field.Parent.FromBatch(Field, batchFunc, build));
        }

        public BatchEnumerableClassField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return SetField(Field.Parent.FromBatch(Field, batchFunc, build));
        }

        public BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return SetField(Field.Parent.FromBatch(Field, keySelector, batchFunc, build));
        }

        public BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return SetField(Field.Parent.FromBatch(Field, keySelector, batchFunc, build));
        }

        public BatchField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc)
        {
            return SetField(Field.Parent.FromBatch(Field, batchFunc));
        }

        public BatchEnumerableField<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc)
        {
            return SetField(Field.Parent.FromBatch(Field, batchFunc));
        }

        public BatchField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc)
        {
            return SetField(Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc)
        {
            return SetField(Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        private TField SetField<TField>(TField field)
            where TField : Field<TEntity, TExecutionContext>
        {
            Field = field;
            return field;
        }
    }
}
