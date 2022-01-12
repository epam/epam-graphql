// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Builders.Common;
using Epam.GraphQL.Builders.Projection.Implementations;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Configuration.Implementations;
using Epam.GraphQL.Configuration.Implementations.Fields;
using Epam.GraphQL.Configuration.Implementations.Fields.BatchFields;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Builders.Loader.Implementations
{
    internal class InlineObjectFieldBuilder<TField, TEntity, TExecutionContext> : ProjectionFieldBuilder<TField, TEntity, TExecutionContext>, IInlineObjectFieldBuilder<TEntity, TExecutionContext>
        where TEntity : class
        where TField : FieldBase<TEntity, TExecutionContext>, IResolvableField<TEntity, TExecutionContext>, IFieldSupportsApplyUnion<TEntity, TExecutionContext>
    {
        private readonly RelationRegistry<TExecutionContext> _registry;

        internal InlineObjectFieldBuilder(RelationRegistry<TExecutionContext> registry, TField fieldType)
            : base(fieldType)
        {
            _registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }

        public IFromLoaderInlineObjectBuilder<TEntity, TChildEntity, TChildEntity> FromLoader<TChildLoader, TChildEntity>(Expression<Func<TEntity, TChildEntity, bool>> condition)
            where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
            where TChildEntity : class
        {
            var field = Field.Parent.FromLoader<TChildLoader, TChildEntity>(Field, condition, Field.Parent.GetGraphQLTypeDescriptor<TChildLoader, TChildEntity>());
            return new FromLoaderInlineObjectBuilder<TEntity, TChildLoader, TChildEntity, TChildEntity, TExecutionContext>(_registry, field);
        }

        public IFromLoaderInlineObjectBuilder<TEntity, TChildEntity, TChildEntity> FromLoader<TChildEntity>(Type typeOfChildLoader, Expression<Func<TEntity, TChildEntity, bool>> condition)
            where TChildEntity : class
        {
            var fromLoaderMethodInfo = GetType().GetGenericMethod(
                nameof(FromLoader),
                new[] { typeOfChildLoader, typeof(TChildEntity) },
                new[] { typeof(Expression<Func<TEntity, TChildEntity, bool>>) });

            return (IFromLoaderInlineObjectBuilder<TEntity, TChildEntity, TChildEntity>)fromLoaderMethodInfo.InvokeAndHoistBaseException(this, condition);
        }

        public IHasSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchClassField<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, batchFunc, build));
        }

        public IHasSelectAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(
            Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchEnumerableClassField<TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, batchFunc, build));
        }

        public IHasSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, keySelector, batchFunc, build));
        }

        public IHasSelectAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, keySelector, batchFunc, build));
        }

        public IHasSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            Func<IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchClassField<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, batchFunc, build));
        }

        public IHasSelectAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(
            Func<IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchEnumerableClassField<TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, batchFunc, build));
        }

        public IHasSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, keySelector, batchFunc, build));
        }

        public IHasSelectAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, keySelector, batchFunc, build));
        }

        public IHasSelectAndReferenceTo<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasSelect<IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasSelect<IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasSelect<IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasSelect<IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, string, TExecutionContext> FromBatch(Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, string>> batchFunc)
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, string, TExecutionContext>, TEntity, string, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, string, TExecutionContext> FromBatch<TKeyType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, string>> batchFunc)
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TKeyType, string, TExecutionContext>, TEntity, string, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasSelect<IEnumerable<string>, TExecutionContext> FromBatch(Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<string>>> batchFunc)
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, string, TExecutionContext>, TEntity, IEnumerable<string>, TExecutionContext>(Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasSelect<IEnumerable<string>, TExecutionContext> FromBatch<TKeyType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<string>>> batchFunc)
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TKeyType, string, TExecutionContext>, TEntity, IEnumerable<string>, TExecutionContext>(Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, string, TExecutionContext> FromBatch(Func<IEnumerable<TEntity>, IDictionary<TEntity, string>> batchFunc)
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, string, TExecutionContext>, TEntity, string, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, string, TExecutionContext> FromBatch<TKeyType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, string>> batchFunc)
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TKeyType, string, TExecutionContext>, TEntity, string, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasSelect<IEnumerable<string>, TExecutionContext> FromBatch(Func<IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<string>>> batchFunc)
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, string, TExecutionContext>, TEntity, IEnumerable<string>, TExecutionContext>(Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasSelect<IEnumerable<string>, TExecutionContext> FromBatch<TKeyType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<string>>> batchFunc)
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TKeyType, string, TExecutionContext>, TEntity, IEnumerable<string>, TExecutionContext>(Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, TReturnType?, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TReturnType?>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TReturnType?, TExecutionContext>, TEntity, TReturnType?, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, TReturnType?, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType?>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TKeyType, TReturnType?, TExecutionContext>, TEntity, TReturnType?, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasSelect<IEnumerable<TReturnType?>, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType?>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TReturnType?, TExecutionContext>, TEntity, IEnumerable<TReturnType?>, TExecutionContext>(Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasSelect<IEnumerable<TReturnType?>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType?>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType?, TExecutionContext>, TEntity, IEnumerable<TReturnType?>, TExecutionContext>(Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, TReturnType?, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, IDictionary<TEntity, TReturnType?>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TReturnType?, TExecutionContext>, TEntity, TReturnType?, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, TReturnType?, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType?>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TKeyType, TReturnType?, TExecutionContext>, TEntity, TReturnType?, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasSelect<IEnumerable<TReturnType?>, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType?>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TReturnType?, TExecutionContext>, TEntity, IEnumerable<TReturnType?>, TExecutionContext>(Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasSelect<IEnumerable<TReturnType?>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType?>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType?, TExecutionContext>, TEntity, IEnumerable<TReturnType?>, TExecutionContext>(Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchClassField<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, batchFunc, build));
        }

        public IHasSelectAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(
            Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchEnumerableClassField<TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, batchFunc, build));
        }

        public IHasSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, keySelector, batchFunc, build));
        }

        public IHasSelectAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, keySelector, batchFunc, build));
        }

        public IHasSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(
            Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchClassField<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, batchFunc, build));
        }

        public IHasSelectAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(
            Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchEnumerableClassField<TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, batchFunc, build));
        }

        public IHasSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, keySelector, batchFunc, build));
        }

        public IHasSelectAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(
            Expression<Func<TEntity, TKeyType>> keySelector,
            Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, keySelector, batchFunc, build));
        }

        public IHasSelectAndReferenceTo<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasSelect<IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasSelect<IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasSelect<IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasSelect<IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, string, TExecutionContext> FromBatch(Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, string>>> batchFunc)
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, string, TExecutionContext>, TEntity, string, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, string, TExecutionContext> FromBatch<TKeyType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, string>>> batchFunc)
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TKeyType, string, TExecutionContext>, TEntity, string, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasSelect<IEnumerable<string>, TExecutionContext> FromBatch(Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<string>>>> batchFunc)
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, string, TExecutionContext>, TEntity, IEnumerable<string>, TExecutionContext>(Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasSelect<IEnumerable<string>, TExecutionContext> FromBatch<TKeyType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<string>>>> batchFunc)
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TKeyType, string, TExecutionContext>, TEntity, IEnumerable<string>, TExecutionContext>(Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, string, TExecutionContext> FromBatch(Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, string>>> batchFunc)
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, string, TExecutionContext>, TEntity, string, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, string, TExecutionContext> FromBatch<TKeyType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, string>>> batchFunc)
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TKeyType, string, TExecutionContext>, TEntity, string, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasSelect<IEnumerable<string>, TExecutionContext> FromBatch(Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<string>>>> batchFunc)
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, string, TExecutionContext>, TEntity, IEnumerable<string>, TExecutionContext>(Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasSelect<IEnumerable<string>, TExecutionContext> FromBatch<TKeyType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<string>>>> batchFunc)
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TKeyType, string, TExecutionContext>, TEntity, IEnumerable<string>, TExecutionContext>(Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, TReturnType?, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType?>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TReturnType?, TExecutionContext>, TEntity, TReturnType?, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, TReturnType?, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType?>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TKeyType, TReturnType?, TExecutionContext>, TEntity, TReturnType?, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasSelect<IEnumerable<TReturnType?>, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType?>>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TReturnType?, TExecutionContext>, TEntity, IEnumerable<TReturnType?>, TExecutionContext>(Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasSelect<IEnumerable<TReturnType?>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType?>>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType?, TExecutionContext>, TEntity, IEnumerable<TReturnType?>, TExecutionContext>(Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, TReturnType?, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType?>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TReturnType?, TExecutionContext>, TEntity, TReturnType?, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, TReturnType?, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType?>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TKeyType, TReturnType?, TExecutionContext>, TEntity, TReturnType?, TExecutionContext>(_registry, Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }

        public IHasSelect<IEnumerable<TReturnType?>, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType?>>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TReturnType?, TExecutionContext>, TEntity, IEnumerable<TReturnType?>, TExecutionContext>(Field.Parent.FromBatch(Field, batchFunc));
        }

        public IHasSelect<IEnumerable<TReturnType?>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType?>>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType?, TExecutionContext>, TEntity, IEnumerable<TReturnType?>, TExecutionContext>(Field.Parent.FromBatch(Field, keySelector, batchFunc));
        }
    }
}
