// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

namespace Epam.GraphQL.Builders.Projection
{
    internal interface IRootProjectionPayloadFieldBuilder<out TArgType, TExecutionContext> :
        IQueryPayloadFieldBuilder<TArgType, TExecutionContext>,
        IMutationPayloadFieldBuilder<TArgType, TExecutionContext>
    {
    }

    internal interface IRootProjectionPayloadFieldBuilder<out TArgType1, out TArgType2, TExecutionContext> :
        IQueryPayloadFieldBuilder<TArgType1, TArgType2, TExecutionContext>,
        IMutationPayloadFieldBuilder<TArgType1, TArgType2, TExecutionContext>
    {
    }

    internal interface IRootProjectionPayloadFieldBuilder<out TArgType1, out TArgType2, out TArgType3, TExecutionContext> :
        IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>,
        IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>
    {
    }

    internal interface IRootProjectionPayloadFieldBuilder<out TArgType1, out TArgType2, out TArgType3, out TArgType4, TExecutionContext> :
        IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>,
        IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
    {
    }

    internal interface IRootProjectionPayloadFieldBuilder<out TArgType1, out TArgType2, out TArgType3, out TArgType4, out TArgType5, TExecutionContext> :
        IQueryPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>,
        IMutationPayloadFieldBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
    {
    }
}
