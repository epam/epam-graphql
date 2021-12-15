// Copyright © 2020 EPAM Systems, Inc. All Rights Reserved. All information contained herein is, and remains the
// property of EPAM Systems, Inc. and/or its suppliers and is protected by international intellectual
// property law. Dissemination of this information or reproduction of this material is strictly forbidden,
// unless prior written permission is obtained from EPAM Systems, Inc

namespace Epam.GraphQL.Builders.Projection
{
    internal interface IRootProjectionArgumentBuilder<out TArgType, TExecutionContext> :
        IQueryArgumentBuilder<TArgType, TExecutionContext>,
        IMutationArgumentBuilder<TArgType, TExecutionContext>
    {
    }

    internal interface IRootProjectionArgumentBuilder<out TArgType1, out TArgType2, TExecutionContext> :
        IQueryArgumentBuilder<TArgType1, TArgType2, TExecutionContext>,
        IMutationArgumentBuilder<TArgType1, TArgType2, TExecutionContext>
    {
    }

    internal interface IRootProjectionArgumentBuilder<out TArgType1, out TArgType2, out TArgType3, TExecutionContext> :
        IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>,
        IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TExecutionContext>
    {
    }

    internal interface IRootProjectionArgumentBuilder<out TArgType1, out TArgType2, out TArgType3, out TArgType4, TExecutionContext> :
        IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>,
        IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TExecutionContext>
    {
    }

    internal interface IRootProjectionArgumentBuilder<out TArgType1, out TArgType2, out TArgType3, out TArgType4, out TArgType5, TExecutionContext> :
        IQueryArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>,
        IMutationArgumentBuilder<TArgType1, TArgType2, TArgType3, TArgType4, TArgType5, TExecutionContext>
    {
    }
}
