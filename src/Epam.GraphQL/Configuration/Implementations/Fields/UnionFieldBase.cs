// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Helpers;

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal class UnionFieldBase<TEntity, TExecutionContext> : FieldBase<TEntity, TExecutionContext>
        where TEntity : class
    {
        protected UnionFieldBase(
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<TEntity, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> graphTypeFactory)
            : this(parent, name, unionType, graphTypeFactory, new List<Type>(), new UnionGraphTypeDescriptor<TExecutionContext>())
        {
        }

        protected UnionFieldBase(
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<TEntity, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> graphTypeFactory,
            List<Type> unionTypes,
            UnionGraphTypeDescriptor<TExecutionContext> unionGraphType)
            : this(
                parent,
                name,
                new List<Type>(unionTypes)
                {
                    unionType,
                },
                unionGraphType)
        {
            UnionGraphType.Add(graphTypeFactory(this));
        }

        protected UnionFieldBase(
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            List<Type> unionTypes,
            UnionGraphTypeDescriptor<TExecutionContext> unionGraphType)
            : base(
                  parent,
                  name)
        {
            UnionGraphType = unionGraphType;
            UnionTypes = unionTypes;
            UnionGraphType.Name = parent.GetGraphQLTypeName(TypeHelpers.GetTheBestCommonBaseType(unionTypes), null, this);
        }

        public override IGraphTypeDescriptor<TExecutionContext> GraphType => UnionGraphType;

        public override Type FieldType => TypeHelpers.GetTheBestCommonBaseType(UnionTypes);

        public UnionGraphTypeDescriptor<TExecutionContext> UnionGraphType { get; }

        protected List<Type> UnionTypes { get; }
    }
}
