// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;

namespace Epam.GraphQL
{
    /// <inheritdoc/>
    [Obsolete("Use SchemaExecuter<,> instead")]
    public class Schema<TQuery, TExecutionContext> : SchemaExecuter<TQuery, TExecutionContext>
        where TQuery : Query<TExecutionContext>, new()
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Schema{TQuery, TExecutionContext}" /> class using the specified options.
        /// </summary>
        /// <param name="options"> The options for this schema. </param>
        public Schema(SchemaOptions options)
            : base(options)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Schema{TQuery, TExecutionContext}" /> class.
        /// </summary>
        public Schema()
            : base()
        {
        }
    }

    /// <inheritdoc/>
    [Obsolete("Use SchemaExecuter<,,> instead")]
    public class Schema<TQuery, TMutation, TExecutionContext> : SchemaExecuter<TQuery, TMutation, TExecutionContext>
        where TQuery : Query<TExecutionContext>, new()
        where TMutation : Mutation<TExecutionContext>, new()
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="Schema{TQuery, TMutation, TExecutionContext}" /> class using the specified options.
        /// </summary>
        /// <param name="options"> The options for this schema. </param>
        public Schema(SchemaOptions options)
            : base(options)
        {
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="Schema{TQuery, TMutation, TExecutionContext}" /> class.
        /// </summary>
        public Schema()
            : base()
        {
        }
    }
}
