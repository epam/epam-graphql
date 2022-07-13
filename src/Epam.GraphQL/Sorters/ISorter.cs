// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Linq.Expressions;
using Epam.GraphQL.Diagnostics;

namespace Epam.GraphQL.Sorters
{
    internal interface ISorter<TExecutionContext>
    {
        string Name { get; }

        bool IsGroupable { get; }

        IChainConfigurationContext ConfigurationContext { get; }

        LambdaExpression BuildExpression(TExecutionContext context);

        LambdaExpression BuildOriginalExpression(TExecutionContext context);
    }
}
