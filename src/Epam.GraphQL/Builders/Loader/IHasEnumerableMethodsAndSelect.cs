// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Builders.Common;

namespace Epam.GraphQL.Builders.Loader
{
    public interface IHasEnumerableMethodsAndSelect<TSourceType, TExecutionContext> : IHasEnumerableMethods<TSourceType>
    {
        IHasEnumerableMethodsAndSelect<TReturnType, TExecutionContext> Select<TReturnType>(Expression<Func<TSourceType, TReturnType>> selector)
            where TReturnType : struct;

        IHasEnumerableMethodsAndSelect<TReturnType?, TExecutionContext> Select<TReturnType>(Expression<Func<TSourceType, TReturnType?>> selector)
            where TReturnType : struct;

        IHasEnumerableMethodsAndSelect<string, TExecutionContext> Select(Expression<Func<TSourceType, string>> selector);

        IHasEnumerableMethodsAndSelect<TReturnType, TExecutionContext> Select<TReturnType>(Expression<Func<TSourceType, TReturnType>> selector, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class;
    }
}
