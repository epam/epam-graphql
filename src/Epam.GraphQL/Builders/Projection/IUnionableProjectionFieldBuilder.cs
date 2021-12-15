// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using Epam.GraphQL.Builders.Loader;

namespace Epam.GraphQL.Builders.Projection
{
    public interface IUnionableProjectionFieldBuilder<out TThisType, TExecutionContext>
    {
        TThisType AsUnionOf<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build = null)
            where TType : class;

        TThisType AsUnionOf<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build = null)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class;

        TThisType And<TType>(Action<IInlineObjectBuilder<TType, TExecutionContext>> build = null)
            where TType : class;

        TThisType And<TEnumerable, TElementType>(Action<IInlineObjectBuilder<TElementType, TExecutionContext>> build = null)
            where TEnumerable : class, IEnumerable<TElementType>
            where TElementType : class;
    }
}
