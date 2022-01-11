// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System.Collections.Generic;
using System.Linq;
using Epam.GraphQL.Extensions;
using GraphQL.Types;

#nullable enable

namespace Epam.GraphQL.Metadata
{
    internal class TypeMetadata
    {
        public TypeMetadata(IEnumerable<IFieldType>? primaryKey, IEnumerable<ForeignKeyMetadata>? foreignKeys)
        {
            PrimaryKey = primaryKey.SafeNull().ToArray();
            ForeignKeys = foreignKeys.SafeNull().ToArray();
        }

        public IEnumerable<IFieldType> PrimaryKey { get; }

        public IEnumerable<ForeignKeyMetadata> ForeignKeys { get; }
    }
}
