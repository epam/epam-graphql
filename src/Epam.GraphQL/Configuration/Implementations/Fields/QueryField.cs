// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using Epam.GraphQL.Builders.Loader;
using Epam.GraphQL.Configuration.Implementations.Fields.ChildFields;

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal class QueryField<TExecutionContext> : Field<object, TExecutionContext>, IQueryField<TExecutionContext>
    {
        public QueryField(RelationRegistry<TExecutionContext> registry, BaseObjectGraphTypeConfigurator<object, TExecutionContext> parent, string name)
            : base(registry, parent, name)
        {
        }

        public QueryableField<object, TReturnType, TExecutionContext> FromIQueryable<TReturnType>(
            Func<TExecutionContext, IQueryable<TReturnType>> query,
            Action<IInlineObjectBuilder<TReturnType, TExecutionContext>>? configure)
            where TReturnType : class
        {
            return Parent.FromIQueryableClass(this, query, null, configure);
        }
    }
}
