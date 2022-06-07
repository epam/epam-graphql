// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using Epam.GraphQL.Configuration.Implementations.Descriptors;
using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Helpers;

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal class UnionFieldBase<TEntity, TExecutionContext> : FieldBase<TEntity, TExecutionContext>
    {
        protected UnionFieldBase(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<TEntity, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> graphTypeFactory)
            : this(configurationContext, parent, name, unionType, graphTypeFactory, new List<Type>(), new UnionGraphTypeDescriptor<TExecutionContext>())
        {
        }

        protected UnionFieldBase(
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            Type unionType,
            Func<UnionFieldBase<TEntity, TExecutionContext>, IGraphTypeDescriptor<TExecutionContext>> graphTypeFactory,
            List<Type> unionTypes,
            UnionGraphTypeDescriptor<TExecutionContext> unionGraphType)
            : this(
                configurationContext,
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
            IChainConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            List<Type> unionTypes,
            UnionGraphTypeDescriptor<TExecutionContext> unionGraphType)
            : base(
                  configurationContext,
                  parent,
                  name)
        {
            UnionGraphType = unionGraphType;
            UnionTypes = unionTypes;
            UnionGraphType.Name = parent.GetGraphQLTypeName(ReflectionHelpers.GetTheBestCommonBaseType(unionTypes), null, this);
        }

        public override IGraphTypeDescriptor<TExecutionContext> GraphType => UnionGraphType;

        public override Type FieldType => ReflectionHelpers.GetTheBestCommonBaseType(UnionTypes);

        public UnionGraphTypeDescriptor<TExecutionContext> UnionGraphType { get; }

        protected List<Type> UnionTypes { get; }
    }
}
