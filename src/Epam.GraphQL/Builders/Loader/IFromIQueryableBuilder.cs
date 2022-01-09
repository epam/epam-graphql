// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Builders.Common;

#nullable enable

namespace Epam.GraphQL.Builders.Loader
{
    public interface IFromIQueryableBuilder<TReturnType, TExecutionContext> :
        IHasEnumerableMethodsAndSelect<TReturnType, TExecutionContext>,
        IHasAsConnection<TReturnType>
    {
        IFromIQueryableBuilder<TReturnType, TExecutionContext> Where(Expression<Func<TReturnType, bool>> predicate);
    }
}
