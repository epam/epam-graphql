// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq;
using System.Linq.Expressions;
using Epam.GraphQL.Extensions;

namespace Epam.GraphQL.Loaders
{
    public abstract class IdentifiableLoader<TEntity, TId, TExecutionContext> : Loader<TEntity, TExecutionContext>, IIdentifiableLoader<TEntity, TId>
        where TEntity : class
    {
        private Func<TEntity, TId> _idGetter = null!;

        protected internal abstract Expression<Func<TEntity, TId>> IdExpression { get; }

        public override IOrderedQueryable<TEntity> ApplyNaturalOrderBy(IQueryable<TEntity> query)
        {
            return query.OrderBy(IdExpression);
        }

        object? IIdentifiableLoader.GetId(object entity) => GetId((TEntity)entity);

        TId IIdentifiableLoader<TEntity, TId>.GetId(TEntity entity) => GetId(entity);

        internal override void AfterConstruction()
        {
            base.AfterConstruction();
            if (!IdExpression.IsProperty())
            {
                throw new ArgumentException($"{GetType()}: {IdExpression} is not a property");
            }

            _idGetter = IdExpression.Compile();

            Registry.Register(IdExpression);
        }

        private protected TId GetId(TEntity entity) => _idGetter(entity);
    }
}
