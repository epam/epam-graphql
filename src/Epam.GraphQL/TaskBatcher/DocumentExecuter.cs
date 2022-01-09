// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using GraphQL.Execution;
using GraphQL.Language.AST;

#nullable enable

namespace Epam.GraphQL.TaskBatcher
{
    internal class DocumentExecuter : global::GraphQL.DocumentExecuter
    {
        protected override IExecutionStrategy SelectExecutionStrategy(ExecutionContext context)
        {
#pragma warning disable IDE0072 // Add missing cases
            return context.Operation.OperationType switch
#pragma warning restore IDE0072 // Add missing cases
            {
                OperationType.Query => new SerialExecutionStrategy(),
                OperationType.Mutation => new SerialExecutionStrategy(),
                _ => base.SelectExecutionStrategy(context),
            };
        }
    }
}
