// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Configuration.Implementations.FieldResolvers;
using Epam.GraphQL.Diagnostics;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ChildFields
{
    internal sealed class EnumerableField<TEntity, TReturnType, TExecutionContext> :
        EnumerableFieldBase<
            EnumerableField<TEntity, TReturnType, TExecutionContext>,
            IEnumerableField<TEntity, TReturnType, TExecutionContext>,
            IEnumerableResolver<TEntity, TReturnType, TExecutionContext>,
            TEntity,
            TReturnType,
            TExecutionContext>,
        IEnumerableField<TEntity, TReturnType, TExecutionContext>
        where TEntity : class
    {
        public EnumerableField(
            MethodCallConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IEnumerableResolver<TEntity, TReturnType, TExecutionContext> resolver,
            IGraphTypeDescriptor<TReturnType, TExecutionContext> elementGraphType,
            LazyQueryArguments? arguments)
            : base(
                  configurationContext,
                  parent,
                  name,
                  resolver,
                  elementGraphType,
                  arguments)
        {
        }

        protected override EnumerableField<TEntity, TReturnType, TExecutionContext> CreateWhere(
            MethodCallConfigurationContext configurationContext,
            Expression<Func<TReturnType, bool>> predicate)
        {
            var enumerableField = new EnumerableField<TEntity, TReturnType, TExecutionContext>(
                configurationContext,
                Parent,
                Name,
                EnumerableFieldResolver.Where(predicate),
                ElementGraphType,
                Arguments);

            return enumerableField;
        }
    }
}
