// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using Epam.GraphQL.Extensions;

namespace Epam.GraphQL.Savers
{
    internal static class InputItem
    {
        public static IInputItem Create(Type entityType, object payload, IDictionary<string, object> properties)
        {
            var type = typeof(InputItem<>).MakeGenericType(entityType);
            return (IInputItem)type.CreateInstanceAndHoistBaseException(payload, properties);
        }
    }

    internal class InputItem<TEntity> : IInputItem
    {
        public InputItem(TEntity payload, IDictionary<string, object> properties)
        {
            Payload = payload;
            Properties = properties;
        }

        public TEntity Payload { get; }

        public IDictionary<string, object> Properties { get; }

        object IInputItem.Payload => Payload;
    }
}
