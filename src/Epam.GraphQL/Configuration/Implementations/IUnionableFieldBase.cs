// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using Epam.GraphQL.Builders.Loader;

namespace Epam.GraphQL.Configuration.Implementations
{
    internal interface IUnionableFieldBase<out TThisType, TEntity, TExecutionContext>
        where TEntity : class
    {
        TThisType AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build)
            where TLastElementType : class;

        TThisType AsUnionOf<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>>? build)
            where TEnumerable : IEnumerable<TElementType>
            where TElementType : class;
    }
}
