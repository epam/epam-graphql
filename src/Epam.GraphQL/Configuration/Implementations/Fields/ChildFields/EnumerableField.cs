// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;

#nullable enable

namespace Epam.GraphQL.Configuration.Implementations.Fields.ChildFields
{
    internal class EnumerableField<TEntity, TReturnType, TExecutionContext> : EnumerableFieldBase<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        public EnumerableField(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IEnumerableResolver<TEntity, TReturnType, TExecutionContext> resolver,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> elementGraphType)
            : base(
                  registry,
                  parent,
                  name,
                  resolver,
                  elementGraphType)
        {
        }

        protected override EnumerableFieldBase<TEntity, TReturnType, TExecutionContext> CreateWhere(Expression<Func<TReturnType, bool>> predicate)
        {
            var enumerableField = new EnumerableField<TEntity, TReturnType, TExecutionContext>(
                Registry,
                Parent,
                Name,
                EnumerableFieldResolver.Where(predicate),
                ElementGraphType);

            return enumerableField;
        }

        protected override EnumerableFieldBase<TEntity, TReturnType1, TExecutionContext> CreateSelect<TReturnType1>(Expression<Func<TReturnType, TReturnType1>> selector, IGraphTypeDescriptor<TReturnType1, TExecutionContext> graphType)
        {
            var enumerableField = new EnumerableField<TEntity, TReturnType1, TExecutionContext>(
                Registry,
                Parent,
                Name,
                EnumerableFieldResolver.Select(selector, graphType.Configurator?.ProxyAccessor),
                graphType);

            return enumerableField;
        }
    }
}
