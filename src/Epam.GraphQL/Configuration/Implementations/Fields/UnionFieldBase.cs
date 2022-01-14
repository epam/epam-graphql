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
        public UnionFieldBase(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<TEntity, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> graphTypeFactory,
            bool isList)
            : this(registry, parent, name, unionType, graphTypeFactory, new List<Type>(), new UnionGraphTypeDescriptor<TExecutionContext>(), isList)
        {
        }

        public UnionFieldBase(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<TEntity, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> graphTypeFactory,
            List<Type> unionTypes,
            UnionGraphTypeDescriptor<TExecutionContext> unionGraphType,
            bool isList)
            : this(
                registry,
                parent,
                name,
                new List<Type>(unionTypes)
                {
                    unionType,
                },
                unionGraphType,
                isList)
        {
            UnionGraphType.Add(graphTypeFactory(this));
        }

        protected UnionFieldBase(
            RelationRegistry<TExecutionContext> registry,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            List<Type> unionTypes,
            UnionGraphTypeDescriptor<TExecutionContext> unionGraphType,
            bool isList)
            : base(
                  registry,
                  parent,
                  name)
        {
            UnionGraphType = unionGraphType;
            IsList = isList;
            UnionTypes = unionTypes;
            UnionGraphType.Name = parent.GetGraphQLTypeName(TypeHelpers.GetTheBestCommonBaseType(unionTypes), null, this);
        }

        public override IGraphTypeDescriptor<TExecutionContext> GraphType => IsList ? UnionGraphType.MakeListDescriptor() : UnionGraphType;

        public override Type FieldType => IsList ? typeof(IEnumerable<>).MakeGenericType(TypeHelpers.GetTheBestCommonBaseType(UnionTypes)) : TypeHelpers.GetTheBestCommonBaseType(UnionTypes);

        public UnionGraphTypeDescriptor<TExecutionContext> UnionGraphType { get; }

        protected bool IsList { get; }

        protected List<Type> UnionTypes { get; }
    }
}
