// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Implementations.Fields;
using Epam.GraphQL.Configuration.Implementations.Fields.ChildFields;
using Epam.GraphQL.Helpers;

namespace Epam.GraphQL.Builders.Loader.Implementations
{
    internal static class FromIQueryableBuilder
    {
        public static IFromIQueryableBuilder<TSelectType, TExecutionContext> Create<TSourceType, TSelectType, TExecutionContext>(
            FieldBase<TSourceType, TExecutionContext> parentField,
            Func<TExecutionContext, IQueryable<TSelectType>> query,
            Expression<Func<TSourceType, TSelectType, bool>> condition,
            Action<IInlineObjectBuilder<TSelectType, TExecutionContext>>? build)
            where TSourceType : class
            where TSelectType : class
        {
            return new FromIQueryableBuilder<TSourceType, TSelectType, TExecutionContext>(parentField.Parent.FromIQueryableClass(parentField, query, condition, build));
        }
    }

    internal class FromIQueryableBuilder<TSourceType, TReturnType, TExecutionContext> : HasEnumerableMethodsAndSelect<TSourceType, TReturnType, TExecutionContext>,
        IFromIQueryableBuilder<TReturnType, TExecutionContext>,
        IHasEnumerableMethodsAndSelect<TReturnType, TExecutionContext>
        where TSourceType : class
    {
        private readonly QueryableField<TSourceType, TReturnType, TExecutionContext> _fieldType;

        public FromIQueryableBuilder(QueryableField<TSourceType, TReturnType, TExecutionContext> fieldType)
            : base(fieldType)
        {
            _fieldType = fieldType;
        }

        public IVoid AsConnection(Expression<Func<IQueryable<TReturnType>, IOrderedQueryable<TReturnType>>> order)
        {
            return _fieldType.AsConnection(order);
        }

        public IFromIQueryableBuilder<TReturnType, TExecutionContext> Where(Expression<Func<TReturnType, bool>> condition)
        {
            return new FromIQueryableBuilder<TSourceType, TReturnType, TExecutionContext>(_fieldType.ApplyWhere(condition));
        }
    }
}
