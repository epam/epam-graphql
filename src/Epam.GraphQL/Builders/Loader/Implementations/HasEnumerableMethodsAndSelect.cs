// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Implementations.Fields.ChildFields;

namespace Epam.GraphQL.Builders.Loader.Implementations
{
    internal class HasEnumerableMethodsAndSelect<TSourceType, TReturnType, TExecutionContext> :
        IHasEnumerableMethodsAndSelect<TReturnType, TExecutionContext>
        where TSourceType : class
    {
        private readonly EnumerableField<TSourceType, TReturnType, TExecutionContext> _fieldType;

        public HasEnumerableMethodsAndSelect(EnumerableField<TSourceType, TReturnType, TExecutionContext> fieldType)
        {
            _fieldType = fieldType ?? throw new ArgumentNullException(nameof(fieldType));
        }

        public void SingleOrDefault(Expression<Func<TReturnType, bool>> predicate)
        {
            _fieldType.ApplySingleOrDefault(predicate);
        }

        public void FirstOrDefault(Expression<Func<TReturnType, bool>> predicate)
        {
            _fieldType.ApplyFirstOrDefault(predicate);
        }

        public IHasEnumerableMethodsAndSelect<TReturnType1, TExecutionContext> Select<TReturnType1>(Expression<Func<TReturnType, TReturnType1>> selector)
            where TReturnType1 : struct
        {
            return new HasEnumerableMethodsAndSelect<TSourceType, TReturnType1, TExecutionContext>(_fieldType.ApplySelect(selector));
        }

        public IHasEnumerableMethodsAndSelect<TReturnType1?, TExecutionContext> Select<TReturnType1>(Expression<Func<TReturnType, TReturnType1?>> selector)
            where TReturnType1 : struct
        {
            return new HasEnumerableMethodsAndSelect<TSourceType, TReturnType1?, TExecutionContext>(_fieldType.ApplySelect(selector));
        }

        public IHasEnumerableMethodsAndSelect<string, TExecutionContext> Select(Expression<Func<TReturnType, string>> selector)
        {
            return new HasEnumerableMethodsAndSelect<TSourceType, string, TExecutionContext>(_fieldType.ApplySelect(selector));
        }

        public IHasEnumerableMethodsAndSelect<TReturnType1, TExecutionContext> Select<TReturnType1>(Expression<Func<TReturnType, TReturnType1>> selector, Action<IInlineObjectBuilder<TReturnType1, TExecutionContext>> build = default)
            where TReturnType1 : class
        {
            return new HasEnumerableMethodsAndSelect<TSourceType, TReturnType1, TExecutionContext>(_fieldType.ApplySelect(selector, build));
        }
    }
}
