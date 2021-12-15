// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Threading.Tasks;
using GraphQL.Execution;
using GraphQL.Language.AST;
using GraphQL.Validation;

#nullable enable

namespace Epam.GraphQL.Infrastructure
{
    internal class DocumentExecutionListener<TExecutionContext> : IDocumentExecutionListener
    {
        private readonly GraphQLContext<TExecutionContext> _context;

        public DocumentExecutionListener(GraphQLContext<TExecutionContext> context)
        {
            _context = context;
        }

        public Task AfterExecutionAsync(IExecutionContext context)
        {
            if (context.Operation.OperationType == OperationType.Query)
            {
                _context.EnableTracking();
            }

            return Task.CompletedTask;
        }

        public Task AfterValidationAsync(IExecutionContext context, IValidationResult validationResult) => Task.CompletedTask;

        public Task BeforeExecutionAsync(IExecutionContext context)
        {
            if (context.Operation.OperationType == OperationType.Query)
            {
                _context.DisableTracking();
            }

            return Task.CompletedTask;
        }

        public Task BeforeExecutionAwaitedAsync(IExecutionContext context) => Task.CompletedTask;

        public Task BeforeExecutionStepAwaitedAsync(IExecutionContext context) => Task.CompletedTask;
    }
}
