// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Configuration.Implementations.Fields;
using Epam.GraphQL.Configuration.Implementations.Fields.BatchFields;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Builders.Loader.Implementations
{
    internal class LoaderFieldBuilder<TEntity, TLoader, TExecutionContext> : BaseLoaderFieldBuilder<Field<TEntity, TExecutionContext>, TEntity, TLoader, TExecutionContext>,
        ILoaderFieldBuilder<TEntity, TExecutionContext>
        where TLoader : Projection<TEntity, TExecutionContext>, new()
        where TEntity : class
    {
        public LoaderFieldBuilder(RelationRegistry<TExecutionContext> registry, Field<TEntity, TExecutionContext> fieldType)
            : base(registry, fieldType)
        {
        }

        public new IHasSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchClassField<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, base.FromBatch(batchFunc, build));
        }

        public new IHasSelectAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchEnumerableClassField<TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Registry, base.FromBatch(batchFunc, build));
        }

        public new IHasSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, base.FromBatch(keySelector, batchFunc, build));
        }

        public new IHasSelectAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Registry, base.FromBatch(keySelector, batchFunc, build));
        }

        public new IHasSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchClassField<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, base.FromBatch(batchFunc, build));
        }

        public new IHasSelectAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchEnumerableClassField<TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Registry, base.FromBatch(batchFunc, build));
        }

        public new IHasSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, base.FromBatch(keySelector, batchFunc, build));
        }

        public new IHasSelectAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Registry, base.FromBatch(keySelector, batchFunc, build));
        }

        public new IHasSelectAndReferenceTo<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, base.FromBatch(batchFunc));
        }

        public new IHasSelect<IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(base.FromBatch(batchFunc));
        }

        public new IHasSelectAndReferenceTo<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, base.FromBatch(keySelector, batchFunc));
        }

        public new IHasSelect<IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(base.FromBatch(keySelector, batchFunc));
        }

        public new IHasSelectAndReferenceTo<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, IDictionary<TEntity, TReturnType>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, base.FromBatch(batchFunc));
        }

        public new IHasSelect<IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(base.FromBatch(batchFunc));
        }

        public new IHasSelectAndReferenceTo<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, base.FromBatch(keySelector, batchFunc));
        }

        public new IHasSelect<IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(base.FromBatch(keySelector, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, string, TExecutionContext> FromBatch(Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, string>> batchFunc)
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, string, TExecutionContext>, TEntity, string, TExecutionContext>(Registry, base.FromBatch(batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, string, TExecutionContext> FromBatch<TKeyType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, string>> batchFunc)
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TKeyType, string, TExecutionContext>, TEntity, string, TExecutionContext>(Registry, base.FromBatch(keySelector, batchFunc));
        }

        public IHasSelect<IEnumerable<string>, TExecutionContext> FromBatch(Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<string>>> batchFunc)
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, string, TExecutionContext>, TEntity, IEnumerable<string>, TExecutionContext>(base.FromBatch(batchFunc));
        }

        public IHasSelect<IEnumerable<string>, TExecutionContext> FromBatch<TKeyType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<string>>> batchFunc)
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TKeyType, string, TExecutionContext>, TEntity, IEnumerable<string>, TExecutionContext>(base.FromBatch(keySelector, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, string, TExecutionContext> FromBatch(Func<IEnumerable<TEntity>, IDictionary<TEntity, string>> batchFunc)
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, string, TExecutionContext>, TEntity, string, TExecutionContext>(Registry, base.FromBatch(batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, string, TExecutionContext> FromBatch<TKeyType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, string>> batchFunc)
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TKeyType, string, TExecutionContext>, TEntity, string, TExecutionContext>(Registry, base.FromBatch(keySelector, batchFunc));
        }

        public IHasSelect<IEnumerable<string>, TExecutionContext> FromBatch(Func<IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<string>>> batchFunc)
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, string, TExecutionContext>, TEntity, IEnumerable<string>, TExecutionContext>(base.FromBatch(batchFunc));
        }

        public IHasSelect<IEnumerable<string>, TExecutionContext> FromBatch<TKeyType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<string>>> batchFunc)
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TKeyType, string, TExecutionContext>, TEntity, IEnumerable<string>, TExecutionContext>(base.FromBatch(keySelector, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, TReturnType?, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, TReturnType?>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TReturnType?, TExecutionContext>, TEntity, TReturnType?, TExecutionContext>(Registry, base.FromBatch(batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, TReturnType?, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType?>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TKeyType, TReturnType?, TExecutionContext>, TEntity, TReturnType?, TExecutionContext>(Registry, base.FromBatch(keySelector, batchFunc));
        }

        public IHasSelect<IEnumerable<TReturnType?>, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType?>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TReturnType?, TExecutionContext>, TEntity, IEnumerable<TReturnType?>, TExecutionContext>(base.FromBatch(batchFunc));
        }

        public IHasSelect<IEnumerable<TReturnType?>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType?>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType?, TExecutionContext>, TEntity, IEnumerable<TReturnType?>, TExecutionContext>(base.FromBatch(keySelector, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, TReturnType?, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, IDictionary<TEntity, TReturnType?>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TReturnType?, TExecutionContext>, TEntity, TReturnType?, TExecutionContext>(Registry, base.FromBatch(batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, TReturnType?, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, TReturnType?>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TKeyType, TReturnType?, TExecutionContext>, TEntity, TReturnType?, TExecutionContext>(Registry, base.FromBatch(keySelector, batchFunc));
        }

        public IHasSelect<IEnumerable<TReturnType?>, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, IDictionary<TEntity, IEnumerable<TReturnType?>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TReturnType?, TExecutionContext>, TEntity, IEnumerable<TReturnType?>, TExecutionContext>(base.FromBatch(batchFunc));
        }

        public IHasSelect<IEnumerable<TReturnType?>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, IDictionary<TKeyType, IEnumerable<TReturnType?>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType?, TExecutionContext>, TEntity, IEnumerable<TReturnType?>, TExecutionContext>(base.FromBatch(keySelector, batchFunc));
        }

        public new IHasSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchClassField<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, base.FromBatch(batchFunc, build));
        }

        public new IHasSelectAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchEnumerableClassField<TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Registry, base.FromBatch(batchFunc, build));
        }

        public new IHasSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, base.FromBatch(keySelector, batchFunc, build));
        }

        public new IHasSelectAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Registry, base.FromBatch(keySelector, batchFunc, build));
        }

        public new IHasSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchClassField<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, base.FromBatch(batchFunc, build));
        }

        public new IHasSelectAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchEnumerableClassField<TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Registry, base.FromBatch(batchFunc, build));
        }

        public new IHasSelectAndReferenceToAndAndFromBatch<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchClassField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, base.FromBatch(keySelector, batchFunc, build));
        }

        public new IHasSelectAndAndFromBatch<TEntity, IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class
        {
            return new FromBatchBuilder<BatchEnumerableClassField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(Registry, base.FromBatch(keySelector, batchFunc, build));
        }

        public new IHasSelectAndReferenceTo<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, base.FromBatch(batchFunc));
        }

        public new IHasSelect<IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(base.FromBatch(batchFunc));
        }

        public new IHasSelectAndReferenceTo<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, base.FromBatch(keySelector, batchFunc));
        }

        public new IHasSelect<IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(base.FromBatch(keySelector, batchFunc));
        }

        public new IHasSelectAndReferenceTo<TEntity, TReturnType, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, base.FromBatch(batchFunc));
        }

        public new IHasSelect<IEnumerable<TReturnType>, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType>>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(base.FromBatch(batchFunc));
        }

        public new IHasSelectAndReferenceTo<TEntity, TReturnType, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, TReturnType, TExecutionContext>(Registry, base.FromBatch(keySelector, batchFunc));
        }

        public new IHasSelect<IEnumerable<TReturnType>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType>>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType, TExecutionContext>, TEntity, IEnumerable<TReturnType>, TExecutionContext>(base.FromBatch(keySelector, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, string, TExecutionContext> FromBatch(Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, string>>> batchFunc)
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, string, TExecutionContext>, TEntity, string, TExecutionContext>(Registry, base.FromBatch(batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, string, TExecutionContext> FromBatch<TKeyType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, string>>> batchFunc)
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TKeyType, string, TExecutionContext>, TEntity, string, TExecutionContext>(Registry, base.FromBatch(keySelector, batchFunc));
        }

        public IHasSelect<IEnumerable<string>, TExecutionContext> FromBatch(Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<string>>>> batchFunc)
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, string, TExecutionContext>, TEntity, IEnumerable<string>, TExecutionContext>(base.FromBatch(batchFunc));
        }

        public IHasSelect<IEnumerable<string>, TExecutionContext> FromBatch<TKeyType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<string>>>> batchFunc)
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TKeyType, string, TExecutionContext>, TEntity, IEnumerable<string>, TExecutionContext>(base.FromBatch(keySelector, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, string, TExecutionContext> FromBatch(Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, string>>> batchFunc)
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, string, TExecutionContext>, TEntity, string, TExecutionContext>(Registry, base.FromBatch(batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, string, TExecutionContext> FromBatch<TKeyType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, string>>> batchFunc)
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TKeyType, string, TExecutionContext>, TEntity, string, TExecutionContext>(Registry, base.FromBatch(keySelector, batchFunc));
        }

        public IHasSelect<IEnumerable<string>, TExecutionContext> FromBatch(Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<string>>>> batchFunc)
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, string, TExecutionContext>, TEntity, IEnumerable<string>, TExecutionContext>(base.FromBatch(batchFunc));
        }

        public IHasSelect<IEnumerable<string>, TExecutionContext> FromBatch<TKeyType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<string>>>> batchFunc)
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TKeyType, string, TExecutionContext>, TEntity, IEnumerable<string>, TExecutionContext>(base.FromBatch(keySelector, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, TReturnType?, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType?>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TReturnType?, TExecutionContext>, TEntity, TReturnType?, TExecutionContext>(Registry, base.FromBatch(batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, TReturnType?, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType?>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TKeyType, TReturnType?, TExecutionContext>, TEntity, TReturnType?, TExecutionContext>(Registry, base.FromBatch(keySelector, batchFunc));
        }

        public IHasSelect<IEnumerable<TReturnType?>, TExecutionContext> FromBatch<TReturnType>(Func<TExecutionContext, IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType?>>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TReturnType?, TExecutionContext>, TEntity, IEnumerable<TReturnType?>, TExecutionContext>(base.FromBatch(batchFunc));
        }

        public IHasSelect<IEnumerable<TReturnType?>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<TExecutionContext, IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType?>>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType?, TExecutionContext>, TEntity, IEnumerable<TReturnType?>, TExecutionContext>(base.FromBatch(keySelector, batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, TReturnType?, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, TReturnType?>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TReturnType?, TExecutionContext>, TEntity, TReturnType?, TExecutionContext>(Registry, base.FromBatch(batchFunc));
        }

        public IHasSelectAndReferenceTo<TEntity, TReturnType?, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, TReturnType?>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectAndReferenceToBuilder<BatchField<TEntity, TKeyType, TReturnType?, TExecutionContext>, TEntity, TReturnType?, TExecutionContext>(Registry, base.FromBatch(keySelector, batchFunc));
        }

        public IHasSelect<IEnumerable<TReturnType?>, TExecutionContext> FromBatch<TReturnType>(Func<IEnumerable<TEntity>, Task<IDictionary<TEntity, IEnumerable<TReturnType?>>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TReturnType?, TExecutionContext>, TEntity, IEnumerable<TReturnType?>, TExecutionContext>(base.FromBatch(batchFunc));
        }

        public IHasSelect<IEnumerable<TReturnType?>, TExecutionContext> FromBatch<TKeyType, TReturnType>(Expression<Func<TEntity, TKeyType>> keySelector, Func<IEnumerable<TKeyType>, Task<IDictionary<TKeyType, IEnumerable<TReturnType?>>>> batchFunc)
            where TReturnType : struct
        {
            return new SelectBuilder<BatchEnumerableField<TEntity, TKeyType, TReturnType?, TExecutionContext>, TEntity, IEnumerable<TReturnType?>, TExecutionContext>(base.FromBatch(keySelector, batchFunc));
        }
    }
}
