// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Builders.Common;
using Epam.GraphQL.Builders.Projection.Implementations;
using Epam.GraphQL.Configuration;
using Epam.GraphQL.Configuration.Implementations;
using Epam.GraphQL.Configuration.Implementations.Fields;
using Epam.GraphQL.Enums;
using Epam.GraphQL.Extensions;
using Epam.GraphQL.Loaders;

namespace Epam.GraphQL.Builders.Loader.Implementations
{
    internal class BaseLoaderFieldBuilder<TField, TEntity, TLoader, TExecutionContext> : ProjectionFieldBuilder<TField, TEntity, TExecutionContext>
        where TLoader : Projection<TEntity, TExecutionContext>, new()
        where TEntity : class
        where TField : FieldBase<TEntity, TExecutionContext>, IFieldSupportsApplyResolve<TEntity, TExecutionContext>
    {
        internal BaseLoaderFieldBuilder(RelationRegistry<TExecutionContext> registry, TField fieldType)
            : base(fieldType)
        {
            Registry = registry ?? throw new ArgumentNullException(nameof(registry));
        }

        protected RelationRegistry<TExecutionContext> Registry { get; }

        public IFromLoaderBuilder<TEntity, TChildEntity, TChildEntity, TExecutionContext> FromLoader<TChildLoader, TChildEntity>(
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            RelationType relationType = RelationType.Association,
            Expression<Func<TChildEntity, TEntity>> navigationProperty = null,
            Expression<Func<TEntity, TChildEntity>> reverseNavigationProperty = null)
            where TChildLoader : Loader<TChildEntity, TExecutionContext>, new()
            where TChildEntity : class
        {
            return FromLoader(typeof(TChildLoader), condition, relationType, navigationProperty, reverseNavigationProperty);
        }

        public IFromLoaderBuilder<TEntity, TChildEntity, TChildEntity, TExecutionContext> FromLoader<TChildEntity>(
            Type childLoaderType,
            Expression<Func<TEntity, TChildEntity, bool>> condition,
            RelationType relationType = RelationType.Association,
            Expression<Func<TChildEntity, TEntity>> navigationProperty = null,
            Expression<Func<TEntity, TChildEntity>> reverseNavigationProperty = null)
            where TChildEntity : class
        {
            var fromLoaderParameterTypes = new[]
                {
                    typeof(Field<TEntity, TExecutionContext>),
                    typeof(Expression<Func<TEntity, TChildEntity, bool>>),
                    typeof(RelationType),
                    typeof(Expression<Func<TChildEntity, TEntity>>),
                    typeof(Expression<Func<TEntity, TChildEntity>>),
                    typeof(IGraphTypeDescriptor<TChildEntity, TExecutionContext>),
                };

            var getGraphQLTypeDescriptorMethod = Field.Parent.GetType().GetGenericMethod(
                nameof(Field.Parent.GetGraphQLTypeDescriptor),
                new[] { childLoaderType, typeof(TChildEntity) },
                Type.EmptyTypes);

            var descriptor = getGraphQLTypeDescriptorMethod.InvokeAndHoistBaseException(Field.Parent);

            var fromLoaderMethodInfo = Field.Parent.GetType().GetGenericMethod(
                nameof(Field.Parent.FromLoader),
                new[] { typeof(TLoader), childLoaderType, typeof(TChildEntity) },
                fromLoaderParameterTypes);

            var field = fromLoaderMethodInfo.InvokeAndHoistBaseException(
                Field.Parent,
                Field,
                condition,
                relationType,
                navigationProperty,
                reverseNavigationProperty,
                descriptor);

            var builderType = typeof(FromLoaderBuilder<,,,,>).MakeGenericType(typeof(TLoader), typeof(TEntity), childLoaderType, typeof(TChildEntity), typeof(TExecutionContext));
            var builder = (IFromLoaderBuilder<TEntity, TChildEntity, TChildEntity, TExecutionContext>)builderType.CreateInstanceAndHoistBaseException(
                Registry, field);
            return builder;
        }
    }
}
