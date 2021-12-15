// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Epam.GraphQL.Loaders;
using GraphQL.Language.AST;

namespace Epam.GraphQL.Types
{
    internal class SortDirectionValue : ValueNode<SortDirection>
    {
        public SortDirectionValue(SortDirection value)
        {
            Value = value;
        }

        protected override bool Equals(ValueNode<SortDirection> node)
        {
            return Value.Equals(node.Value);
        }
    }
}
