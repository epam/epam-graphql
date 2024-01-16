// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Collections.Generic;

namespace Epam.GraphQL
{
    internal class AfterSaveContext<TExecutionContext> : IAfterSaveContext<TExecutionContext>
    {
        internal AfterSaveContext(TExecutionContext executionContext, IEnumerable<object> path)
        {
            ExecutionContext = executionContext;
            Path = path;
        }

        public TExecutionContext ExecutionContext { get; }

        public IEnumerable<object> Path { get; }
    }
}
