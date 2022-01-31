// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Epam.GraphQL.Configuration.Implementations.Fields.ExpressionFields;

namespace Epam.GraphQL.Builders.Common.Implementations
{
    internal class GroupableFieldBuilder<TEntity, TReturnType, TExecutionContext> : IHasGroupable
        where TEntity : class
    {
        internal GroupableFieldBuilder(ExpressionField<TEntity, TReturnType, TExecutionContext> field)
        {
            Field = field;
        }

        private protected ExpressionField<TEntity, TReturnType, TExecutionContext> Field { get; }

        public void Groupable()
        {
            Field.Groupable();
        }
    }
}
