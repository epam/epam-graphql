// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Collections.Generic;
using GraphQL.Types;

#nullable enable

namespace Epam.GraphQL.Metadata
{
    internal class ForeignKeyMetadata
    {
        public ForeignKeyMetadata(IComplexGraphType toType, IEnumerable<IFieldType>? toField, IEnumerable<IFieldType>? fromField)
        {
            ToType = toType;
            ToField = toField;
            FromField = fromField;
        }

        public IComplexGraphType ToType { get; }

        public IEnumerable<IFieldType>? ToField { get; }

        public IEnumerable<IFieldType>? FromField { get; }
    }
}
