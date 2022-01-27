// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using Epam.GraphQL.Builders.Loader;

namespace Epam.GraphQL.Configuration
{
    public interface IUnionableFieldBase<out TThisType, TExecutionContext>
    {
        TThisType AsUnionOf<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build = null)
            where TLastElementType : class;

        TThisType And<TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build = null)
            where TLastElementType : class;

        [Obsolete("Consider using AsUnionOf with one type parameter.")]
        TThisType AsUnionOf<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build = null)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class;

        [Obsolete("Consider using And with one type parameter.")]
        TThisType And<TEnumerable, TLastElementType>(Action<IInlineObjectBuilder<TLastElementType, TExecutionContext>>? build = null)
            where TEnumerable : IEnumerable<TLastElementType>
            where TLastElementType : class;
    }
}
