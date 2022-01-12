// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Epam.GraphQL.Loaders;
using GraphQL;
using GraphQL.DataLoader;

namespace Epam.GraphQL.Configuration.Implementations.Fields
{
    internal class FieldEditSettings<TEntity, TReturnType, TExecutionContext> : IFieldEditSettings<TEntity, TReturnType, TExecutionContext>
    {
        private readonly bool _canNotBeChangedDirectly;

        public FieldEditSettings()
            : this(!typeof(Input).IsAssignableFrom(typeof(TEntity)))
        {
        }

        public FieldEditSettings(IFieldExpression<TEntity, TReturnType, TExecutionContext> expression)
            : this(expression.IsReadOnly)
        {
        }

        private FieldEditSettings(bool canNotBeChangedDirectly)
        {
            _canNotBeChangedDirectly = canNotBeChangedDirectly;
        }

        public bool IsReadOnly => OnWrite == null && OnWriteAsync == null && _canNotBeChangedDirectly;

        public Func<IResolveFieldContext, object, object>? GetDefaultValue { get; set; }

        public Func<IResolveFieldContext, IDataLoader<IFieldChange<TEntity, TExecutionContext>, (bool CanEdit, string DisableReason)>>? CanEdit { get; set; }

        public Action<IResolveFieldContext, TEntity, TReturnType>? OnWrite { get; set; }

        public Func<IResolveFieldContext, TEntity, TReturnType, Task>? OnWriteAsync { get; set; }

        public bool IsMandatoryForUpdate { get; set; }

        Action<IResolveFieldContext, object, object?>? IFieldEditSettings<TExecutionContext>.OnWrite
        {
            get
            {
                if (OnWrite == null)
                {
                    return null;
                }

                return (ctx, e, value) =>
                {
                    value = value is IDictionary<string, object> dict
                        ? dict.ToObject(typeof(TReturnType))
                        : value.GetPropertyValue(typeof(TReturnType));

                    OnWrite(ctx, (TEntity)e, (TReturnType)value);
                };
            }
        }

        Func<IResolveFieldContext, object, object?, Task>? IFieldEditSettings<TExecutionContext>.OnWriteAsync
        {
            get
            {
                if (OnWriteAsync == null)
                {
                    return null;
                }

                return (ctx, e, value) =>
                {
                    value = value is IDictionary<string, object> dict
                        ? dict.ToObject(typeof(TReturnType))
                        : value.GetPropertyValue(typeof(TReturnType));

                    return OnWriteAsync(ctx, (TEntity)e, (TReturnType)value);
                };
            }
        }
    }
}
