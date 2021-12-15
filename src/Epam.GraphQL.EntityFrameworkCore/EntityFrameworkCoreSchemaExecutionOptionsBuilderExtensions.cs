// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Epam.GraphQL
{
    public static class EntityFrameworkCoreSchemaExecutionOptionsBuilderExtensions
    {
        public static SchemaExecutionOptionsBuilder<TExecutionContext> WithDbContext<TExecutionContext>(this SchemaExecutionOptionsBuilder<TExecutionContext> builder, DbContext dbContext)
        {
            if (builder == null)
            {
                throw new ArgumentNullException(nameof(builder));
            }

            return builder.With(options => options.DataContext = new DbContextAdapter(dbContext));
        }

        [Obsolete("UseDbContext has been renamed. Use WithDbContext instead")]
        public static SchemaExecutionOptionsBuilder<TExecutionContext> UseDbContext<TExecutionContext>(this SchemaExecutionOptionsBuilder<TExecutionContext> builder, DbContext dbContext) =>
            WithDbContext(builder, dbContext);
    }
}
