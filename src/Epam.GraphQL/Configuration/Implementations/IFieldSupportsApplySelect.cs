// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Diagnostics;

namespace Epam.GraphQL.Configuration.Implementations
{
    internal interface IFieldSupportsApplySelect<TEntity, TReturnType, TExecutionContext>
    {
        IFieldSupportsEditSettings<TEntity, TReturnType1, TExecutionContext> ApplySelect<TReturnType1>(
            IInlinedChainConfigurationContext configurationContext,
            Func<TReturnType, TReturnType1> selector,
            Action<IInlineObjectBuilder<TReturnType1, TExecutionContext>>? build);
    }
}
