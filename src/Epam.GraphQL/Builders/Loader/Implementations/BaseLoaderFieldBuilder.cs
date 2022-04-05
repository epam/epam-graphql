// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Builders.Projection.Implementations;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Configuration.Implementations.Fields;
using Epam.GraphQL.Enums;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Builders.Loader.Implementations
{
    internal class BaseLoaderFieldBuilder<TField, TEntity, TLoader, TExecutionContext> : ProjectionFieldBuilder<TField, TEntity, TExecutionContext>
        where TLoader : Loader<TEntity, TExecutionContext>, new()
        where TEntity : class
        where TField : Field<TEntity, TExecutionContext>, IUnionableField<TEntity, TExecutionContext>
    {
        internal BaseLoaderFieldBuilder(RelationRegistry<TExecutionContext> registry, TField fieldType)
            : base(fieldType)
        {
            Registry = registry;
        }

        protected RelationRegistry<TExecutionContext> Registry { get; }

        public ILoaderField<TEntity, TChildEntity, TExecutionContext> FromLoader<TChildLoader, TChildEntity>(
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            RelationType relationType = RelationType.Association,
            Expression<Func<TChildEntity, TEntity>>? navigationProperty = null,
            Expression<Func<TEntity, TChildEntity>>? reverseNavigationProperty = null)
            where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
            where TChildEntity : class
        {
            var descriptor = Registry.GetGraphTypeDescriptor<TChildLoader, TChildEntity>();
            var loaderField = Field.Parent.FromLoader<TLoader, TChildLoader, TChildEntity>(
                Field.ConfigurationContext.NextOperation<TChildLoader, TChildEntity>(nameof(FromLoader))
                    .Argument(condition)
                    .Argument(relationType.ToString())
                    .OptionalArgument(navigationProperty)
                    .OptionalArgument(reverseNavigationProperty),
                Field,
                condition,
                relationType,
                navigationProperty,
                reverseNavigationProperty,
                descriptor);

            return loaderField;
        }
    }
}
