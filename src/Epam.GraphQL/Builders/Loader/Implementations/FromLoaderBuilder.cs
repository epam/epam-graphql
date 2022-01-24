// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Builders.Common;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Configuration.Implementations.Fields.ChildFields;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Builders.Loader.Implementations
{
    internal class FromLoaderBuilder<TLoader, TEntity, TChildLoader, TChildEntity, TExecutionContext> :
        FromLoaderInlineObjectBuilder<LoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext>, TEntity, TChildLoader, TChildEntity, TChildEntity, TExecutionContext>,
        IFromLoaderBuilder<TEntity, TChildEntity, TChildEntity, TExecutionContext>
        where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
        where TChildEntity : class
        where TLoader : Projection<TEntity, TExecutionContext>, new()
        where TEntity : class
    {
        public FromLoaderBuilder(
            LoaderField<TEntity, TChildLoader, TChildEntity, TExecutionContext> fieldType)
            : base(fieldType)
        {
        }

        public IConnectionField AsConnection() =>
            (IConnectionField)Field.AsConnection();

        public IConnectionField AsConnection(Expression<Func<IQueryable<TChildEntity>, IOrderedQueryable<TChildEntity>>> order) =>
            (IConnectionField)Field.AsConnection(order);
    }
}
