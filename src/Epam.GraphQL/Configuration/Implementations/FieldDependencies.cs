// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Collections.Generic;
using System.Linq.Expressions;
using Epam.GraphQL.Helpers;

namespace Epam.GraphQL.Configuration.Implementations
{
    internal class FieldDependencies<TExecutionContext>
    {
        private readonly HashSet<LambdaExpression> _dependentOn = new(ExpressionEqualityComparer.Instance);

        public FieldDependencies()
        {
        }

        public IEnumerable<LambdaExpression> DependentOn => _dependentOn;

        public void AddRange(IEnumerable<LambdaExpression> expressions)
        {
            _dependentOn.UnionWith(expressions);
        }

        public void Add(LambdaExpression expression)
        {
            _dependentOn.Add(expression);
        }
    }
}
