// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using System.Collections.Generic;
using Epam.GraphQL.Metadata;
using GraphQL.DataLoader;
using GraphQL.Types;

namespace Epam.GraphQL.Configuration.Implementations.Relations
{
    internal class ForeignKeyRelation<TPropertyType> : IRelation
    {
        private readonly string _propName;
        private readonly Predicate<TPropertyType> _isFakePropValue;
        private readonly Func<object, object?> _idGetter;

        public ForeignKeyRelation(string propName, Predicate<TPropertyType> isFakePropValue, Func<object, object?> idGetter)
        {
            _propName = propName;
            _isFakePropValue = isFakePropValue;
            _idGetter = idGetter;
        }

        public IDataLoaderResult<bool> CanViewParentAsync(object context, object? entity) => new DataLoaderResult<bool>(true);

        public ForeignKeyMetadata GetForeignKeyMetadata(IComplexGraphType childGraphType)
        {
            throw new NotImplementedException();
        }

        public bool HasFakePropertyValue(object? childEntity, IDictionary<string, object?> childPropertyValues)
        {
            if (childPropertyValues.TryGetValue(_propName, out var val))
            {
                if (val is TPropertyType value)
                {
                    return _isFakePropValue(value);
                }

                throw new InvalidCastException($"Property {_propName} must be of type {typeof(TPropertyType)}");
            }

            return false;
        }

        public void UpdateFakeProperties(object? entity, object? childEntity, IDictionary<string, object?> childPropertyValues, object? fakePropertyValue)
        {
            if (childPropertyValues.TryGetValue(_propName, out var val))
            {
                if (entity != null && ((val == null && fakePropertyValue == null) || (val != null && val.Equals(fakePropertyValue))))
                {
                    childPropertyValues[_propName] = _idGetter(entity);
                }
            }
        }
    }
}
