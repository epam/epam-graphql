// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using GraphQL;
using GraphQL.Resolvers;

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal class SubmitField<TEntity, TExecutionContext> : Field<TEntity, TExecutionContext>
        where TEntity : class
    {
        private readonly Func<IResolveFieldContext, Dictionary<string, object>, Task<object>> _resolve;
        private readonly IGraphTypeDescriptor<TExecutionContext> _graphType;

        public SubmitField(RelationRegistry<TExecutionContext> registry, BaseObjectGraphTypeConfigurator<TEntity, TExecutionContext> parent, string name, IGraphTypeDescriptor<TExecutionContext> returnGraphType, string argName, Type argGraphType, Func<IResolveFieldContext, Dictionary<string, object>, Task<object>> resolve, Type fieldType)
            : base(registry, parent, name)
        {
            _graphType = returnGraphType ?? throw new ArgumentNullException(nameof(returnGraphType));
            _resolve = resolve ?? throw new ArgumentNullException(nameof(resolve));

            FieldType = fieldType;
            Arguments = new LazyQueryArguments
            {
                new LazyQueryArgument(argName, argGraphType),
            };

            parent.ProxyAccessor.AddAllMembers(Name);
        }

        public override IGraphTypeDescriptor<TExecutionContext> GraphType => _graphType;

        protected override IFieldResolver GetResolver()
        {
            return new AsyncFieldResolver<object>(ctx => _resolve(ctx, (Dictionary<string, object>)ctx.Arguments["payload"]));
        }
    }
}
