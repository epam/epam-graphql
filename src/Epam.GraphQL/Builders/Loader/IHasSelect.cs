// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;

namespace Epam.GraphQL.Builders.Loader
{
    public interface IHasSelect<TSourceType, TExecutionContext>
    {
        void Select<TReturnType>(Func<TSourceType, TReturnType> selector)
            where TReturnType : struct;

        void Select<TReturnType>(Func<TSourceType, TReturnType?> selector)
            where TReturnType : struct;

        void Select(Func<TSourceType, string> selector);

        void Select<TReturnType>(Func<TSourceType, TReturnType> selector, Action<IInlineObjectBuilder<TReturnType, TExecutionContext>> build = null)
            where TReturnType : class;
    }
}
