// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Linq.Expressions;
using Epam.GraphQL.Loaders;
using GraphQL;

namespace Epam.GraphQL.Configuration.Implementations.Fields.ResolvableFields
{
    internal class PayloadFields<TArg1, TExecutionContext> : ArgumentsBase<TArg1, PayloadFieldsContext<TExecutionContext>, TExecutionContext>, IArguments<TArg1, TExecutionContext>
    {
        private readonly PayloadFieldsContextAccessor<TExecutionContext> _contextAccessor;

        public PayloadFields(string fieldName, IRegistry<TExecutionContext> registry, string argName)
            : base(registry, new PayloadField<TArg1, TExecutionContext>(argName))
        {
            _contextAccessor = new PayloadFieldsContextAccessor<TExecutionContext>(fieldName, Items);
        }

        public PayloadFields(string fieldName, IRegistry<TExecutionContext> registry, string argName, Type projectionType, Type entityType)
            : base(registry, new FilterPayloadField<TExecutionContext>(registry, argName, projectionType, entityType))
        {
            _contextAccessor = new PayloadFieldsContextAccessor<TExecutionContext>(fieldName, Items);
        }

        public string FieldName => _contextAccessor.FieldName;

        public IArguments<TArg1, TArg2, TExecutionContext> Add<TArg2>(string argName)
        {
            return new PayloadFields<TArg1, TArg2, TExecutionContext>(this, argName);
        }

        public IArguments<TArg1, Expression<Func<TEntity, bool>>, TExecutionContext> AddFilter<TProjection, TEntity>(string argName)
            where TProjection : Projection<TEntity, TExecutionContext>
        {
            return new PayloadFields<TArg1, Expression<Func<TEntity, bool>>, TExecutionContext>(this, argName, typeof(TProjection), typeof(TEntity));
        }

        public override void ApplyTo(IArgumentCollection arguments) => _contextAccessor.ApplyTo(arguments);

        protected override PayloadFieldsContext<TExecutionContext> GetContext(IResolveFieldContext context) => _contextAccessor.GetContext(context);
    }

    internal class PayloadFields<TArg1, TArg2, TExecutionContext> : ArgumentsBase<TArg1, TArg2, PayloadFieldsContext<TExecutionContext>, TExecutionContext>, IArguments<TArg1, TArg2, TExecutionContext>
    {
        private readonly PayloadFieldsContextAccessor<TExecutionContext> _contextAccessor;

        public PayloadFields(PayloadFields<TArg1, TExecutionContext> payloadFields, string argName)
            : base(payloadFields, new PayloadField<TArg2, TExecutionContext>(argName))
        {
            _contextAccessor = new PayloadFieldsContextAccessor<TExecutionContext>(payloadFields.FieldName, Items);
        }

        public PayloadFields(PayloadFields<TArg1, TExecutionContext> payloadFields, string argName, Type projectionType, Type entityType)
            : base(payloadFields, new FilterPayloadField<TExecutionContext>(payloadFields.Registry, argName, projectionType, entityType))
        {
            _contextAccessor = new PayloadFieldsContextAccessor<TExecutionContext>(payloadFields.FieldName, Items);
        }

        public string FieldName => _contextAccessor.FieldName;

        public IArguments<TArg1, TArg2, TArg3, TExecutionContext> Add<TArg3>(string argName)
        {
            return new PayloadFields<TArg1, TArg2, TArg3, TExecutionContext>(this, argName);
        }

        public IArguments<TArg1, TArg2, Expression<Func<TEntity, bool>>, TExecutionContext> AddFilter<TProjection, TEntity>(string argName)
            where TProjection : Projection<TEntity, TExecutionContext>
        {
            return new PayloadFields<TArg1, TArg2, Expression<Func<TEntity, bool>>, TExecutionContext>(this, argName, typeof(TProjection), typeof(TEntity));
        }

        public override void ApplyTo(IArgumentCollection arguments) => _contextAccessor.ApplyTo(arguments);

        protected override PayloadFieldsContext<TExecutionContext> GetContext(IResolveFieldContext context) => _contextAccessor.GetContext(context);
    }

    internal class PayloadFields<TArg1, TArg2, TArg3, TExecutionContext> : ArgumentsBase<TArg1, TArg2, TArg3, PayloadFieldsContext<TExecutionContext>, TExecutionContext>, IArguments<TArg1, TArg2, TArg3, TExecutionContext>
    {
        private readonly PayloadFieldsContextAccessor<TExecutionContext> _contextAccessor;

        public PayloadFields(PayloadFields<TArg1, TArg2, TExecutionContext> payloadFields, string argName)
            : base(payloadFields, new PayloadField<TArg3, TExecutionContext>(argName))
        {
            _contextAccessor = new PayloadFieldsContextAccessor<TExecutionContext>(payloadFields.FieldName, Items);
        }

        public PayloadFields(PayloadFields<TArg1, TArg2, TExecutionContext> payloadFields, string argName, Type projectionType, Type entityType)
            : base(payloadFields, new FilterPayloadField<TExecutionContext>(payloadFields.Registry, argName, projectionType, entityType))
        {
            _contextAccessor = new PayloadFieldsContextAccessor<TExecutionContext>(payloadFields.FieldName, Items);
        }

        public string FieldName => _contextAccessor.FieldName;

        public IArguments<TArg1, TArg2, TArg3, TArg4, TExecutionContext> Add<TArg4>(string argName)
        {
            return new PayloadFields<TArg1, TArg2, TArg3, TArg4, TExecutionContext>(this, argName);
        }

        public IArguments<TArg1, TArg2, TArg3, Expression<Func<TEntity, bool>>, TExecutionContext> AddFilter<TProjection, TEntity>(string argName)
            where TProjection : Projection<TEntity, TExecutionContext>
        {
            return new PayloadFields<TArg1, TArg2, TArg3, Expression<Func<TEntity, bool>>, TExecutionContext>(this, argName, typeof(TProjection), typeof(TEntity));
        }

        public override void ApplyTo(IArgumentCollection arguments) => _contextAccessor.ApplyTo(arguments);

        protected override PayloadFieldsContext<TExecutionContext> GetContext(IResolveFieldContext context) => _contextAccessor.GetContext(context);
    }

    internal class PayloadFields<TArg1, TArg2, TArg3, TArg4, TExecutionContext> : ArgumentsBase<TArg1, TArg2, TArg3, TArg4, PayloadFieldsContext<TExecutionContext>, TExecutionContext>, IArguments<TArg1, TArg2, TArg3, TArg4, TExecutionContext>
    {
        private readonly PayloadFieldsContextAccessor<TExecutionContext> _contextAccessor;

        public PayloadFields(PayloadFields<TArg1, TArg2, TArg3, TExecutionContext> payloadFields, string argName)
            : base(payloadFields, new PayloadField<TArg4, TExecutionContext>(argName))
        {
            _contextAccessor = new PayloadFieldsContextAccessor<TExecutionContext>(payloadFields.FieldName, Items);
        }

        public PayloadFields(PayloadFields<TArg1, TArg2, TArg3, TExecutionContext> payloadFields, string argName, Type projectionType, Type entityType)
            : base(payloadFields, new FilterPayloadField<TExecutionContext>(payloadFields.Registry, argName, projectionType, entityType))
        {
            _contextAccessor = new PayloadFieldsContextAccessor<TExecutionContext>(payloadFields.FieldName, Items);
        }

        public string FieldName => _contextAccessor.FieldName;

        public IArguments<TArg1, TArg2, TArg3, TArg4, TArg5, TExecutionContext> Add<TArg5>(string argName)
        {
            return new PayloadFields<TArg1, TArg2, TArg3, TArg4, TArg5, TExecutionContext>(this, argName);
        }

        public IArguments<TArg1, TArg2, TArg3, TArg4, Expression<Func<TEntity, bool>>, TExecutionContext> AddFilter<TProjection, TEntity>(string argName)
            where TProjection : Projection<TEntity, TExecutionContext>
        {
            return new PayloadFields<TArg1, TArg2, TArg3, TArg4, Expression<Func<TEntity, bool>>, TExecutionContext>(this, argName, typeof(TProjection), typeof(TEntity));
        }

        public override void ApplyTo(IArgumentCollection arguments) => _contextAccessor.ApplyTo(arguments);

        protected override PayloadFieldsContext<TExecutionContext> GetContext(IResolveFieldContext context) => _contextAccessor.GetContext(context);
    }

    internal class PayloadFields<TArg1, TArg2, TArg3, TArg4, TArg5, TExecutionContext> : ArgumentsBase<TArg1, TArg2, TArg3, TArg4, TArg5, PayloadFieldsContext<TExecutionContext>, TExecutionContext>, IArguments<TArg1, TArg2, TArg3, TArg4, TArg5, TExecutionContext>
    {
        private readonly PayloadFieldsContextAccessor<TExecutionContext> _contextAccessor;

        public PayloadFields(PayloadFields<TArg1, TArg2, TArg3, TArg4, TExecutionContext> payloadFields, string argName)
            : base(payloadFields, new PayloadField<TArg5, TExecutionContext>(argName))
        {
            _contextAccessor = new PayloadFieldsContextAccessor<TExecutionContext>(payloadFields.FieldName, Items);
        }

        public PayloadFields(PayloadFields<TArg1, TArg2, TArg3, TArg4, TExecutionContext> payloadFields, string argName, Type projectionType, Type entityType)
            : base(payloadFields, new FilterPayloadField<TExecutionContext>(payloadFields.Registry, argName, projectionType, entityType))
        {
            _contextAccessor = new PayloadFieldsContextAccessor<TExecutionContext>(payloadFields.FieldName, Items);
        }

        public override void ApplyTo(IArgumentCollection arguments) => _contextAccessor.ApplyTo(arguments);

        protected override PayloadFieldsContext<TExecutionContext> GetContext(IResolveFieldContext context) => _contextAccessor.GetContext(context);
    }
}
