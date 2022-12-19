// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epam.GraphQL.Diagnostics;
using GraphQL;
using GraphQL.Resolvers;
using GraphQL.Types;

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal class SubmitField<TEntity, TExecutionContext> : FieldBase<TEntity, TExecutionContext>
    {
        public SubmitField(
            Func<IChainConfigurationContextOwner, IChainConfigurationContext> configurationContextFactory,
            BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent,
            string name,
            IGraphTypeDescriptor<TExecutionContext> returnGraphType,
            string argName,
            IInputObjectGraphType argGraphType,
            Func<IResolveFieldContext, Dictionary<string, object>, ValueTask<object?>> resolve,
            Type fieldType)
            : base(configurationContextFactory, parent, name)
        {
            GraphType = returnGraphType;
            Resolver = new FuncFieldResolver<object>(ctx => resolve(ctx, (Dictionary<string, object>)ctx.Arguments!["payload"].Value!));
            FieldType = fieldType;

            Arguments = new()
            {
                new(argName, argGraphType),
            };
        }

        public override IGraphTypeDescriptor<TExecutionContext> GraphType { get; }

        public override Type FieldType { get; }

        public override IFieldResolver Resolver { get; }
    }
}
