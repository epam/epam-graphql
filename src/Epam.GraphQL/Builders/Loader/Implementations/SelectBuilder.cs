// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

using System;
using Epam.GraphQL.Configuration.Implementations;
using Epam.GraphQL.Configuration.Implementations.Fields;

namespace Epam.GraphQL.Builders.Loader.Implementations
{
    internal class SelectBuilder<TField, TSourceType, TReturnType, TExecutionContext> :
        IHasSelect<TReturnType, TExecutionContext>
        where TField : FieldBase<TSourceType, TExecutionContext>, IFieldSupportsApplySelect<TSourceType, TReturnType, TExecutionContext>
    {
        internal SelectBuilder(TField field)
        {
            Field = field;
        }

        protected TField Field { get; set; }

        public void Select<TReturnType1>(Func<TReturnType, TReturnType1> selector, Action<IInlineObjectBuilder<TReturnType1, TExecutionContext>>? build)
        {
            Field.ApplySelect(
                Field.ConfigurationContext.Chain<TReturnType>(nameof(Select))
                    .Argument(selector)
                    .OptionalArgument(build),
                selector,
                build);
        }
    }
}
