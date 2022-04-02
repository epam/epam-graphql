// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using Epam.GraphQL.Diagnostics;
using Epam.GraphQL.Helpers;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields
{
    internal class ResolvedField<TEntity, TReturnType, TExecutionContext> : TypedField<TEntity, TReturnType, TExecutionContext>, IVoid
        where TEntity : class
    {
        public ResolvedField(
            FieldConfigurationContext configurationContext,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IGraphTypeDescriptor<TExecutionContext> graphType,
            IFieldResolver resolver,
            LazyQueryArguments? arguments)
            : base(configurationContext, parent, name)
        {
            Resolver = resolver;
            GraphType = graphType;
            Arguments = arguments;
        }

        public override IGraphTypeDescriptor<TExecutionContext> GraphType { get; }

        public override IFieldResolver Resolver { get; }
    }
}
