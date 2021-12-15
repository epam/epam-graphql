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
    public abstract class IdentifiableLoader<TEntity, TId, TExecutionContext> : Loader<TEntity, TExecutionContext>, IIdentifiableLoader
        where TEntity : class
    {
        Func<object, object> IIdentifiableLoader.IdGetter => obj => IdGetter((TEntity)obj);

        protected internal abstract Expression<Func<TEntity, TId>> IdExpression { get; }

        private protected Func<TEntity, TId> IdGetter { get; private set; }

        public override IOrderedQueryable<TEntity> ApplyNaturalOrderBy(IQueryable<TEntity> query)
        {
            return query.OrderBy(IdExpression);
        }

        public override IOrderedQueryable<TEntity> ApplyNaturalThenBy(IOrderedQueryable<TEntity> query)
        {
            return query.ThenBy(IdExpression);
        }

        internal override void AfterConstruction()
        {
            base.AfterConstruction();
            if (!IdExpression.IsProperty())
            {
                throw new ArgumentException($"{GetType()}: {IdExpression} is not a property");
            }

            IdGetter = IdExpression.Compile();

            Registry.Register(IdExpression);
        }
    }
}