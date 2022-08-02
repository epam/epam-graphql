// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Helpers
{
    internal class DummyMutableLoader<TExecutionContext> : MutableLoader<object, int, TExecutionContext>
    {
        protected internal override Expression<Func<object, int>> IdExpression => throw new NotImplementedException();

        public override bool IsFakeId(int id) => throw new NotImplementedException();

        protected override IQueryable<object> GetBaseQuery(TExecutionContext context) => throw new NotImplementedException();

        protected override void OnConfigure() => throw new NotImplementedException();
    }
}
